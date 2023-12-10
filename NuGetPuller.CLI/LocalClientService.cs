using Microsoft.Extensions.Logging;

namespace NuGetPuller.CLI;

public sealed class LocalClientService(ValidatedPathsService validatedSettings, ILogger<LocalClientService> logger) : LocalClientServiceBase(validatedSettings, logger)
{
}
