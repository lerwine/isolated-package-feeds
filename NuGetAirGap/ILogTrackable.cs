using Microsoft.Extensions.Logging;

namespace NuGetAirGap;

public interface ILogTrackable
{
    bool WasLogged { get; }

    public void Log(ILogger logger, bool force = false);
}
