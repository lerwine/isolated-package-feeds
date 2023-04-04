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
[ContentGetter(JsDelivrSettings.PROVIDER_ID, JsDelivrSettings.PROVIDER_NAME, Description = "data.jsdelivr.com")]
public class JsDelivrGetterService : ContentGetterService
{
    private readonly ILogger<JsDelivrGetterService> _logger;

    public JsDelivrGetterService(ContentDb dbContext, IOptions<AppSettings> options, ILogger<JsDelivrGetterService> logger, IHostEnvironment hostEnvironment)
        : base(dbContext, options, hostEnvironment)
    {
        _logger = logger;
    }

#pragma warning disable CS1998
    public override async Task AddAsync(UpstreamCdn upstreamCdn, IEnumerable<string> libraryNames, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
#pragma warning restore CS1998

#pragma warning disable CS1998
    public override async Task GetNewVersionsAsync(CdnLibrary cdnLibrary, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
#pragma warning restore CS1998

#pragma warning disable CS1998
    protected override async Task AddNewVersionsAsync(UpstreamCdn upstreamCdn, string libraryName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
#pragma warning restore CS1998

    protected override ILogger GetLogger() => _logger;

#pragma warning disable CS1998
    protected override async Task ReloadAsync(CdnLibrary library, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
#pragma warning restore CS1998
}
