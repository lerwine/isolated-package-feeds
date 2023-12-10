using Microsoft.Extensions.Logging;

namespace IsolatedPackageFeeds.Shared;

public class LoggedException : Exception, ILogTrackable
{
    public LoggedException() { }
    public LoggedException(string message) : base(message) { }
    public LoggedException(string? message, Exception? inner = null) : base(message, inner) { }

    public bool WasLogged => true;

    void ILogTrackable.Log(ILogger logger) { }
}
