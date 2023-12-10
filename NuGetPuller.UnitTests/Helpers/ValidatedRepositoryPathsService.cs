using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NuGetPuller.UnitTests.Helpers;

public class ValidatedRepositoryPathsService(IOptions<TestAppSettings> options, IHostEnvironment hostEnvironment, ILogger<ValidatedRepositoryPathsService> logger) : ValidatedRepositoryPathsService<TestAppSettings>(options.Value, hostEnvironment, logger)
{
}