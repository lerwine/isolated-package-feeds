using System;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CdnSync;

public class CdnJsSyncService
{
    public const string PROVIDER_ID = "026ce2f6-c574-4869-bd4c-73bd73dfb640";
    private readonly CdnSyncDb _dbContext;
    private readonly ILogger<CdnJsSyncService> _logger;
    private readonly Uri _baseUrl;
    private readonly string _defaultProviderName;
    private readonly string[] _removeLibraries;
    private readonly string[] _addLibraries;

    public CdnJsSyncService(CdnSyncDb dbContext, IOptions<CdnJsSettings> options, ILogger<CdnJsSyncService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
        CdnJsSettings settings = options.Value;
        string? baseUrl = settings.BaseUrl.ToTrimmedOrNullIfEmpty();
        if (baseUrl is null)
            _baseUrl = new Uri(CdnJsSettings.DEFAULT_BaseUrl, UriKind.Absolute);
        else if (Uri.TryCreate(baseUrl, UriKind.Absolute, out Uri? uri))
            _baseUrl = uri;
        else
        {
            _baseUrl = new Uri("", UriKind.Relative);
            logger.LogError("Base URI for cdnjs settings is not an absolute URI.");
        }
        _defaultProviderName = settings.ProviderName.ToWsNormalizedOrNullIfEmpty() ?? CdnJsSettings.DEFAULT_ProviderName;
        string[]? libraries = settings.RemoveLibraries;
        StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;
        _removeLibraries = (libraries is null) ? Array.Empty<string>() : libraries.Select(l => l.ToTrimmedOrNullIfEmpty()!).Where(l => l is not null).Distinct(comparer).ToArray();
        _addLibraries = ((libraries = settings.AddLibraries) is null) ? Array.Empty<string>() : libraries.Select(l => l.ToTrimmedOrNullIfEmpty()!).Where(l => l is not null).Distinct(comparer).ToArray();
    }

    internal async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_baseUrl.IsAbsoluteUri)
            return;
        Guid id = new Guid(PROVIDER_ID);
        LibraryProvider? provider = await _dbContext.Providers.Where(p => p.Id == id).Include(p => p.Libraries).FirstOrDefaultAsync(stoppingToken);
        if (provider is null)
        {
            provider = new()
            {
                Id = id,
                Name = _defaultProviderName
            };
            await _dbContext.Providers.AddAsync(provider, stoppingToken);
            await _dbContext.SaveChangesAsync(stoppingToken);
        }
        else
            foreach (string r in _removeLibraries)
            {
                ContentLibrary? library = await _dbContext.Libraries.FirstOrDefaultAsync(l => l.ProviderId == id && l.Name == r);
                if (library is not null)
                    await LibraryVersion.DeleteAsync(_dbContext, library.Id, stoppingToken);
            }
        bool added = false;
        foreach (string a in _addLibraries)
            if ((await _dbContext.Libraries.FirstOrDefaultAsync(l => l.ProviderId == id && l.Name == a)) is null)
            {
                added = true;
                await _dbContext.Libraries.AddAsync(new()
                {
                    Id = Guid.NewGuid(),
                    Name = a,
                    ProviderId = id
                }, stoppingToken);
            }
        if (added)
            await _dbContext.SaveChangesAsync(stoppingToken);
        using HttpClient client = new HttpClient();
        await _dbContext.Libraries.Where(l => l.ProviderId == id).ForEachAsync(async library => await SyncrhonizeAsync(library, client, stoppingToken), stoppingToken);
    }
    

    internal async Task SyncrhonizeAsync(ContentLibrary library, CancellationToken stoppingToken)
    {
        using HttpClient client = new HttpClient();
        await SyncrhonizeAsync(library, client, stoppingToken);
    }

    private async Task SyncrhonizeAsync(ContentLibrary library, HttpClient client, CancellationToken stoppingToken)
    {
        Guid id = library.Id;
        LibraryVersion[] versions = await _dbContext.Versions.Where(v => v.LibraryId == id).ToArrayAsync(stoppingToken);
        Uri uri = new Uri(_baseUrl, $"/libraries/{Uri.EscapeDataString(library.Name)}");
        HttpResponseMessage response = await client.GetAsync(uri, stoppingToken);
        if (response.IsSuccessStatusCode)
        {
            // TODO: Parse JSON
        }
        else
        {
            // TODO: Log error
        }
    }
}