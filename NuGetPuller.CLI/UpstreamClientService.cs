using Microsoft.Extensions.Logging;

namespace NuGetPuller.CLI;

public sealed class UpstreamClientService(ValidatedAppSettings validatedSettings, ILogger<UpstreamClientService> logger) : UpstreamClientServiceBase(validatedSettings, logger)
{
}
