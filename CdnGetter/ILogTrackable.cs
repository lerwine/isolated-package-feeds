using Microsoft.Extensions.Logging;

namespace CdnGetter;

public interface ILogTrackable
{
    bool IsLogged { get; }

    void Log(ILogger logger, bool force = false);
}
