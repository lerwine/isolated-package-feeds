using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace NuGetAirGap;

public sealed class UpstreamClientService(IOptions<AppSettings> options, ILogger<UpstreamClientService> logger) : ClientService(Repository.Factory.GetCoreV3(options.Value.Validated.UpstreamServiceLocation), options, logger, true)
{
}
