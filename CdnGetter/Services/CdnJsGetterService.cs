using CdnGetter.Config;
using CdnGetter.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CdnGetter.Services;

[ContentGetter(CdnJsSettings.PROVIDER_ID, CdnJsSettings.PROVIDER_NAME, Description = "api.cdnjs.com")]
[Obsolete("Storage and retrieval should exist as separate services")]
public class CdnJsGetterService(ContentDb dbContext, IOptions<AppSettings> options, ILogger<CdnJsGetterService> logger, IHostEnvironment hostEnvironment) : ContentGetterService(dbContext, options, hostEnvironment)
{
    private readonly ILogger<CdnJsGetterService> _logger = logger;

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