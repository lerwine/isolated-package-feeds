using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using CdnSync.SettingsSections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CdnSync;

public class CdnJsSyncService
{
    public const string PROVIDER_ID = "026ce2f6-c574-4869-bd4c-73bd73dfb640";
    private readonly CdnSyncDb _dbContext;
    private readonly ILogger<CdnJsSyncService> _logger;
    private readonly CdnJsSettings _settings;

    public CdnJsSyncService(CdnSyncDb dbContext, IOptions<CdnSyncSettings> options, ILogger<CdnJsSyncService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
        _settings = options.Value.cdnJs ?? new();
    }

    internal async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_settings.TryGetBaseUrl(out Uri baseUrl))
        {
            _logger.LogError("Base URI for cdnjs settings is not an absolute URI.");
            return;
        }
        Guid id = new(PROVIDER_ID);
        LibraryProvider? provider = await _dbContext.Providers.Where(p => p.Id == id).Include(p => p.Libraries).FirstOrDefaultAsync(stoppingToken);
        if (stoppingToken.IsCancellationRequested)
            return;
        bool modified = false;
        if (provider is null)
        {
            provider = new()
            {
                Id = id,
                Name = _settings.GetProviderName()
            };
            await _dbContext.Providers.AddAsync(provider, stoppingToken);
            await _dbContext.SaveChangesAsync(stoppingToken);
            _logger.LogDebug("Added new {RecordType} for {ProviderName}", provider.GetType(), provider.Name);
            if (stoppingToken.IsCancellationRequested)
                return;
        }
        else
        {
            foreach (string r in _settings.GetRemoveLibraries())
            {
                if (stoppingToken.IsCancellationRequested)
                    return;
                ContentLibrary? library = await _dbContext.Libraries.FirstOrDefaultAsync(l => l.ProviderId == id && l.Name == r, stoppingToken);
                if (library is not null)
                {
                    if (stoppingToken.IsCancellationRequested)
                        return;
                    modified = true;
                    await LibraryVersion.DeleteAsync(_dbContext, library.Id, stoppingToken);
                    _logger.LogInformation("Deleting {LibraryRecordType} \"{LibraryName}\" from {ProviderRecordType} \"{ProviderName}\".", typeof(ContentLibrary), library.Name, typeof(LibraryProvider), provider.Name);
                }
            }
            if (stoppingToken.IsCancellationRequested)
                return;
            if (modified)
            {
                await _dbContext.SaveChangesAsync(stoppingToken);
                modified = false;
            }
        }
        foreach (string a in _settings.GetAddLibraries())
        {
            if (stoppingToken.IsCancellationRequested)
                return;
            if ((await _dbContext.Libraries.FirstOrDefaultAsync(l => l.ProviderId == id && l.Name == a, stoppingToken)) is null)
            {
                modified = true;
                await _dbContext.Libraries.AddAsync(new()
                {
                    Id = Guid.NewGuid(),
                    Name = a,
                    ProviderId = id
                }, stoppingToken);
                _logger.LogInformation("Adding new {LibraryRecordType} \"{LibraryName}\" from {RecordType} \"{ProviderProviderName}\".", typeof(ContentLibrary), a, typeof(LibraryProvider), provider.Name);
            }
        }
        if (stoppingToken.IsCancellationRequested)
            return;
        if (modified)
            await _dbContext.SaveChangesAsync(stoppingToken);
        if (stoppingToken.IsCancellationRequested)
            return;
        using HttpClient client = new()
        {
            BaseAddress = baseUrl
        };
        await _dbContext.Libraries.Where(l => l.ProviderId == id).ForEachAsync(async library => await SyncrhonizeAsync(library, client, stoppingToken), stoppingToken);
    }

    internal async Task SyncrhonizeAsync(ContentLibrary library, CancellationToken stoppingToken)
    {
        if (!_settings.TryGetBaseUrl(out Uri baseUrl))
        {
            _logger.LogError("Base URI for cdnjs settings is not an absolute URI.");
            return;
        }
        if (stoppingToken.IsCancellationRequested)
            return;
        using HttpClient client = new()
        {
            BaseAddress = baseUrl
        };
        await SyncrhonizeAsync(library, client, stoppingToken);
    }

    private async Task SyncrhonizeAsync(ContentLibrary library, HttpClient client, CancellationToken stoppingToken)
    {
        Guid id = library.Id;
        LibraryVersion[] localVersions = await _dbContext.Versions.Where(v => v.LibraryId == id).ToArrayAsync(stoppingToken);
        if (stoppingToken.IsCancellationRequested)
            return;
        using HttpResponseMessage response = await client.GetAsync($"/libraries/{Uri.EscapeDataString(library.Name)}", stoppingToken);
        if (stoppingToken.IsCancellationRequested)
            return;
        System.Net.HttpStatusCode statusCode = response.StatusCode;
        int statusValue = (int)statusCode;
        if (response.IsSuccessStatusCode)
        {
            string jsonString  = await response.Content.ReadAsStringAsync(cancellationToken: stoppingToken);
            if (stoppingToken.IsCancellationRequested)
                return;
            _logger.LogDebug("Read: {json}", jsonString);
            if (CdnJs.Response.Error.TryDeserialize<CdnJs.Response.LibraryAllVersions>(jsonString, out CdnJs.Response.LibraryAllVersions? rc, out CdnJs.Response.Error? error))
                foreach (string versionString in rc.versions.TrimmedNotEmptyValues())
                    if (!await _dbContext.Versions.AnyAsync(v => v.LibraryId == id && v.VersionString == versionString, stoppingToken))
                    {
                        if (stoppingToken.IsCancellationRequested)
                            return;
                        // TODO: Download new version
                    }
                    else if (stoppingToken.IsCancellationRequested)
                        return;
            else
                _logger.LogError("Response code: {ResponseCode}; Status code {StatusCode}; Message: {Message}", statusCode, error.status, error.message.ToTrimmedOrDefaultIfEmpty(() => error.status.ToStatusMessage()));
        }
        else
        {
            string? jsonString;
            try { jsonString = (await response.Content.ReadAsStringAsync(cancellationToken: stoppingToken)).ToTrimmedOrNullIfEmpty(); }
            catch { jsonString = null; }
            if (jsonString is null)
                _logger.LogError("Response code: {ResponseCode}", statusCode);
            else if (CdnJs.Response.Error.TryDeserialize(jsonString, out CdnJs.Response.Error? error))
                _logger.LogError("Response code: {ResponseCode}; Status code {StatusCode}; Message: {Message}", statusCode, error.status, error.message.ToTrimmedOrDefaultIfEmpty(() => error.status.ToStatusMessage()));
            else
                _logger.LogError("Response code: {ResponseCode}; Content: {Content}", statusCode, jsonString);
        }
    }
}