using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace NuGetAirGap;

public sealed class UpstreamClientService : ClientService
{
    public UpstreamClientService(IOptions<AppSettings> options, ILogger<UpstreamClientService> logger) :
        base(Repository.Factory.GetCoreV3(options.Value.Validated.UpstreamServiceLocation), options, logger, true) { }
}
