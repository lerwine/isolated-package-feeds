using Microsoft.Extensions.Logging;

namespace NuGetPuller;

public interface ILogTrackable
{
    bool WasLogged { get; }

    public void Log(ILogger logger, bool force = false);
}
