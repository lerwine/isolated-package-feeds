using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NuGetPuller.UnitTests.Helpers;

public sealed class UpstreamClientService(IOptions<TestAppSettings> options, TestAppSettingsValidatorService validatorService, ILogger<UpstreamClientService> logger) : UpstreamClientServiceBase(options.Value, validatorService.Validated, logger)
{
}
