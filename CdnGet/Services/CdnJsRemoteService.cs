using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CdnGet.Config;
using CdnGet.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CdnGet.Services;

[RemoteUpdateService(CdnJsSettings.PROVIDER_ID, CdnJsSettings.PROVIDER_NAME, Description = "api.cdnjs.com")]
public class CdnJsRemoteService : RemoteUpdateService
{
    // private readonly ContentDb _dbContext;
    private readonly ILogger<CdnJsRemoteService> _logger;
    // private readonly CdnJsSettings _settings;

    // public CdnJsRemoteService(ContentDb dbContext, IOptions<AppSettings> options, ILogger<CdnJsRemoteService> logger)
    public CdnJsRemoteService(ILogger<CdnJsRemoteService> logger)
    {
        // _dbContext = dbContext;
        _logger = logger;
        // _settings = options.Value.CdnJs ?? new();
    }

    public override async Task GetNewAsync(RemoteService remoteService, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;
        CdnJsSettings settings = appSettings.CdnJs ?? new();
        if (!settings.TryGetBaseUrl(out Uri? baseUrl))
        {
            _logger.LogInvalidBaseUrl<CdnJsRemoteService>(remoteService.Name);
            return;
        }
        LinkedList<ContentLibrary> libraryModels = await GetMatchingLibrariesAsync(remoteService.Name, remoteService.Id, dbContext, libraryNames, stoppingToken);
        _logger.LogWarning("{MethodName} not implemented ({libraryNames})", nameof(GetNewAsync), string.Join(", ", libraryNames));
    }
    
    public override async Task AddAsync(RemoteService remoteService, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;
        CdnJsSettings settings = appSettings.CdnJs ?? new();
        if (!settings.TryGetBaseUrl(out Uri? baseUrl))
        {
            _logger.LogInvalidBaseUrl<CdnJsRemoteService>(remoteService.Name);
            return;
        }
        foreach (string ln in libraryNames)
        {
            if ((await GetMatchingLibraryAsync(remoteService.Id, dbContext, ln, stoppingToken)) is null)
                _logger.LogWarning("{MethodName} not implemented (Library: {name})", nameof(AddAsync), ln);
            else
                _logger.LogLibraryAlreadyExistsLocal(ln, remoteService.Name);
        }
    }
    
    public override async Task ReloadAsync(RemoteService remoteService, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;
        CdnJsSettings settings = appSettings.CdnJs ?? new();
        if (!settings.TryGetBaseUrl(out Uri? baseUrl))
        {
            _logger.LogInvalidBaseUrl<CdnJsRemoteService>(remoteService.Name);
            return;
        }
        LinkedList<ContentLibrary> toRemove = await GetMatchingLibrariesAsync(remoteService.Name, remoteService.Id, dbContext, libraryNames, stoppingToken);
        if (stoppingToken.IsCancellationRequested || toRemove.First is null)
            return;
        _logger.LogWarning("{MethodName} not implemented ({libraryNames})", nameof(ReloadAsync), string.Join(", ", libraryNames));
    }
    
    public override async Task ReloadExistingAsync(RemoteService remoteService, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;
        CdnJsSettings settings = appSettings.CdnJs ?? new();
        if (!settings.TryGetBaseUrl(out Uri? baseUrl))
        {
            _logger.LogInvalidBaseUrl<CdnJsRemoteService>(remoteService.Name);
            return;
        }
        LinkedList<ContentLibrary> toRemove = await GetMatchingLibrariesAsync(remoteService.Name, remoteService.Id, dbContext, libraryNames, stoppingToken);
        if (stoppingToken.IsCancellationRequested || toRemove.First is null)
            return;
        _logger.LogWarning("{MethodName} not implemented ({libraryNames})", nameof(ReloadExistingAsync), string.Join(", ", libraryNames));
    }

    protected override ILogger GetLogger() => _logger;
}