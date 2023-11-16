using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace NuGetAirGap;

public sealed class UpstreamClientService : ClientService
{
    public UpstreamClientService(UpstreamRepositoryProvider repositoryProvider, IOptions<AppSettings> options, ILogger<UpstreamClientService> logger) : base(repositoryProvider, options, logger, true) { }
}
