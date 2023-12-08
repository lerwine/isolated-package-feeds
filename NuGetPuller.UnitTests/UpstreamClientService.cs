using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NuGetPuller.UnitTests;

public sealed class UpstreamClientService(IOptions<TestAppSettings> options, ILogger<UpstreamClientService> logger) : UpstreamClientService<TestAppSettings>(options, logger)
{
}