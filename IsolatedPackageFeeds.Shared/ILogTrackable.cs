using Microsoft.Extensions.Logging;

namespace IsolatedPackageFeeds.Shared;

public interface ILogTrackable
{
    bool WasLogged { get; }

    public void Log(ILogger logger);
}
