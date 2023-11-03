using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

namespace CdnGetter;

[Serializable]
public class LocalNugetRepositoryCreationException : LoggedException
{
    public string Path { get; private set; } = null!;

    public LocalNugetRepositoryCreationException() { }

    private LocalNugetRepositoryCreationException(string message, string path, IOException inner) : base(message, inner) => Path = path;
    
    protected LocalNugetRepositoryCreationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    internal static LocalNugetRepositoryCreationException LogAndCreate(ILogger logger, string path, IOException error) =>
        LogAndCreate(logger, () => new LocalNugetRepositoryCreationException($"Unable to create local NuGet repository subdirectory {JsonValue.Create(path)?.ToJsonString() ?? path}", path, error));

    protected override void Log(ILogger logger) => logger.LogLocalNugetRepositoryCreationError(Path, InnerException);
}
