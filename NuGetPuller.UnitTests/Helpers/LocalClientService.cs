using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NuGetPuller.UnitTests.Helpers;

public sealed class LocalClientService(IOptions<TestAppSettings> options, TestAppSettingsValidatorService validatorService, ILogger<LocalClientService> logger) : LocalClientServiceBase(options.Value, validatorService.Validated, logger)
{
}
