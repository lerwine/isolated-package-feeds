using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CdnGetter.Config;
using CdnGetter.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CdnGetter.Services;

[ContentGetter(CdnJsSettings.PROVIDER_ID, CdnJsSettings.PROVIDER_NAME, Description = "api.cdnjs.com")]
public class CdnJsGetterService : ContentGetterService
{
    // private readonly ContentDb _dbContext;
    private readonly ILogger<CdnJsGetterService> _logger;
    // private readonly CdnJsSettings _settings;

    // public CdnJsRemoteService(ContentDb dbContext, IOptions<AppSettings> options, ILogger<CdnJsRemoteService> logger)
    public CdnJsGetterService(ILogger<CdnJsGetterService> logger)
    {
        // _dbContext = dbContext;
        _logger = logger;
        // _settings = options.Value.CdnJs ?? new();
    }

    public override async Task AddAsync(RemoteService remoteService, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override async Task GetNewVersionsAsync(RemoteLibrary remoteLibrary, ContentDb dbContext, AppSettings appSettings, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override async Task AddNewVersionsAsync(RemoteService remoteService, string libraryName, ContentDb dbContext, AppSettings appSettings, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override ILogger GetLogger() => _logger;

    protected override async Task ReloadAsync(RemoteLibrary library, ContentDb dbContext, AppSettings appSettings, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}