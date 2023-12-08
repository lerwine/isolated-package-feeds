using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NuGetPuller.CLI;

public sealed class LocalClientService(IOptions<AppSettings> options, ILogger<LocalClientService> logger) : LocalClientService<AppSettings>(options, logger)
{
}