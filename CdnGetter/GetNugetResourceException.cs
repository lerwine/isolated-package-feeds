using Microsoft.Extensions.Logging;

namespace CdnGetter;

[Serializable]
public class GetNugetResourceException : LoggedException
{
    public Type Type { get; private set; } = null!;

    public GetNugetResourceException() { }

    private GetNugetResourceException(string message, Type type) : base(message) => Type = type;
    
    protected GetNugetResourceException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    internal static GetNugetResourceException LogAndCreate<T>(ILogger logger) => LogAndCreate(logger, typeof(T));

    internal static GetNugetResourceException LogAndCreate(ILogger logger, Type type) => LogAndCreate(logger, () => new GetNugetResourceException($"Failed to get NuGet resource type {type.FullName}", type));

    protected override void Log(ILogger logger) => logger.LogGetNugetResourceFailure(Type);
}
