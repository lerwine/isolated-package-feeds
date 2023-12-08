using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NuGetPuller.CLI;

public sealed class LocalClientService(IOptions<AppSettings> options, AppSettingsValidatorService validatorService, ILogger<LocalClientService> logger) : LocalClientServiceBase(options.Value, validatorService.Validated, logger)
{
}
