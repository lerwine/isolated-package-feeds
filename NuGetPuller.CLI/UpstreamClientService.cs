using Microsoft.Extensions.Logging;

namespace NuGetPuller.CLI;

public sealed class UpstreamClientService(ValidatedPathsService validatedSettings, ILogger<UpstreamClientService> logger) : UpstreamClientServiceBase(validatedSettings, logger)
{
}
