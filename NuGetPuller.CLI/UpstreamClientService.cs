using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NuGetPuller.CLI;

public sealed class UpstreamClientService(IOptions<AppSettings> options, AppSettingsValidatorService validatorService, ILogger<UpstreamClientService> logger) : UpstreamClientServiceBase(options.Value, validatorService.Validated, logger)
{
}
