using Microsoft.Extensions.Logging;

namespace NuGetPuller.CLI;

public sealed class LocalClientService(ValidatedAppSettings validatedSettings, ILogger<LocalClientService> logger) : LocalClientServiceBase(validatedSettings, logger)
{
}
