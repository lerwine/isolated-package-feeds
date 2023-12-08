using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NuGetPuller.CLI;

public sealed class UpstreamClientService(IOptions<AppSettings> options, ILogger<UpstreamClientService> logger) : UpstreamClientService<AppSettings>(options, logger)
{
}