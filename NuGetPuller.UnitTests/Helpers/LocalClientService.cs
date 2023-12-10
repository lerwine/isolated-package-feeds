using Microsoft.Extensions.Logging;

namespace NuGetPuller.UnitTests.Helpers;

public sealed class LocalClientService(ValidatedRepositoryPathsService validatedSettings, ILogger<LocalClientService> logger) : LocalClientServiceBase(validatedSettings, logger)
{
}
