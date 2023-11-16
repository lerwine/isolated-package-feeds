using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NuGetAirGap;

public sealed class UpstreamClientService : ClientService
{
    public UpstreamClientService(UpstreamRepositoryProvider repositoryProvider, IOptions<AppSettings> options, ILogger<UpstreamClientService> logger) : base(repositoryProvider, options, logger, true) { }
}
