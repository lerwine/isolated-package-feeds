using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

namespace CdnGetter;

[Serializable]
public class InvalidLocalNugetRepositoryPathException : LoggedException
{
    public string Path { get; private set; } = null!;

    public InvalidLocalNugetRepositoryPathException() { }

    private InvalidLocalNugetRepositoryPathException(string message, string path) : base(message) => Path = path;
    
    private InvalidLocalNugetRepositoryPathException(string message, string path, Exception inner) : base(message, inner) => Path = path;
    
    protected InvalidLocalNugetRepositoryPathException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    internal static InvalidLocalNugetRepositoryPathException LogAndCreate(ILogger logger, string path) => LogAndCreate(logger, () => new InvalidLocalNugetRepositoryPathException($"Local repository path {JsonValue.Create(path)?.ToJsonString() ?? path} is not a subdirectory.", path));

    internal static InvalidLocalNugetRepositoryPathException LogAndCreate(ILogger logger, string path, PathTooLongException error) =>
        LogAndCreate(logger, () => new InvalidLocalNugetRepositoryPathException($"Local repository path {JsonValue.Create(path)?.ToJsonString() ?? path} is too long.", path, error));

    internal static InvalidLocalNugetRepositoryPathException LogAndCreate(ILogger logger, string path, ArgumentException error) =>
        LogAndCreate(logger, () => new InvalidLocalNugetRepositoryPathException($"Local repository path {JsonValue.Create(path)?.ToJsonString() ?? path} is invalid.", path, error));

    protected override void Log(ILogger logger)
    {
        if (InnerException is null)
            logger.LogLocalNugetRepositoryPathNotSubdirectory(Path);
        else if (InnerException is PathTooLongException error)
            logger.LogLocalNugetRepositoryPathTooLong(Path, error);
        else
            logger.LogInvalidLocalNugetRepositoryPath(Path, InnerException);
    }
}
