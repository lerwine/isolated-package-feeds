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
    private readonly ILogger<CdnJsGetterService> _logger;

    // public CdnJsUpstreamCdn(ContentDb dbContext, IOptions<AppSettings> options, ILogger<CdnJsUpstreamCdn> logger)
    public CdnJsGetterService(ContentDb dbContext, IOptions<AppSettings> options, ILogger<CdnJsGetterService> logger, IHostEnvironment hostEnvironment)
        : base(dbContext, options, hostEnvironment)
    {
        _logger = logger;
    }

    public override async Task AddAsync(UpstreamCdn upstreamCdn, IEnumerable<string> libraryNames, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override async Task GetNewVersionsAsync(CdnLibrary cdnLibrary, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override async Task AddNewVersionsAsync(UpstreamCdn upstreamCdn, string libraryName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override ILogger GetLogger() => _logger;

    protected override async Task ReloadAsync(CdnLibrary library, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}