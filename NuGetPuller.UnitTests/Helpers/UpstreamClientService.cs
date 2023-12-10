using Microsoft.Extensions.Logging;

namespace NuGetPuller.UnitTests.Helpers;

public sealed class UpstreamClientService(ValidatedRepositoryPathsService validatedSettings, ILogger<UpstreamClientService> logger) : UpstreamClientServiceBase(validatedSettings, logger)
{
}
