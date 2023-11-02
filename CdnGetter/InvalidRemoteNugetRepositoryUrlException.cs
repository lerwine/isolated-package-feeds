using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

namespace CdnGetter;

[Serializable]
public class InvalidRemoteNugetRepositoryUrlException : LoggedException
{
    public string UriString { get; private set; } = null!;

    public InvalidRemoteNugetRepositoryUrlException() { }

    private InvalidRemoteNugetRepositoryUrlException(string message, string path) : base(message) => UriString = path;
    
    private InvalidRemoteNugetRepositoryUrlException(string message, string path, Exception inner) : base(message, inner) => UriString = path;
    
    protected InvalidRemoteNugetRepositoryUrlException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    internal static InvalidRemoteNugetRepositoryUrlException LogAndCreate(ILogger logger, string path, Exception error) =>
        LogAndCreate(logger, () => new InvalidRemoteNugetRepositoryUrlException($"Remote NuGet repository URL {JsonValue.Create(path)?.ToJsonString() ?? path} is invalid.", path, error));

    internal static InvalidRemoteNugetRepositoryUrlException LogAndCreate(ILogger logger, string path) =>
        LogAndCreate(logger, () => new InvalidRemoteNugetRepositoryUrlException($"Remote NuGet repository URL {JsonValue.Create(path)?.ToJsonString() ?? path} must not refer to a local path.", path));

    protected override void Log(ILogger logger)
    {
        if (InnerException is null)
            logger.LogRemoteRepositoryUrlIsLocal(UriString);
        else
            logger.LogInvalidRemoteRepositoryUrl(UriString, InnerException);
    }
}
