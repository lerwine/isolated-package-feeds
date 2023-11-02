using Microsoft.Extensions.Logging;

namespace CdnGetter;

public abstract class LoggedException : Exception, ILogTrackable
{
    protected LoggedException() { }

    protected LoggedException(string message) : base(message) { }
    
    protected LoggedException(string message, System.Exception inner) : base(message, inner) { }
    
    protected LoggedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    
    internal static T LogAndCreate<T>(ILogger logger, Func<T> factory)
        where T : LoggedException
    {
        T result = factory();
        result.Log(logger);
        return result;
    }

    bool ILogTrackable.IsLogged => true;

    protected abstract void Log(ILogger logger);

    void ILogTrackable.Log(ILogger logger, bool force)
    {
        if (force)
            Log(logger);
    }
}