using Microsoft.Extensions.Logging;

namespace NuGetPuller.UnitTests.Helpers;

public sealed class LocalClientService(ValidatedTestAppSettings validatedSettings, ILogger<LocalClientService> logger) : LocalClientServiceBase(validatedSettings, logger)
{
}
