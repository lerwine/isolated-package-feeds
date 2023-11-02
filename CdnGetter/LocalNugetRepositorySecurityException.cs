using System.Security;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

namespace CdnGetter;

[Serializable]
public class LocalNugetRepositorySecurityException : LoggedException
{
    public string Path { get; private set; } = null!;

    public LocalNugetRepositorySecurityException() { }

    private LocalNugetRepositorySecurityException(string message, string path, SecurityException inner) : base(message, inner) => Path = path;
    
    protected LocalNugetRepositorySecurityException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    internal static LocalNugetRepositorySecurityException LogAndCreate(ILogger logger, string path, SecurityException error) =>
        LogAndCreate(logger, () => new LocalNugetRepositorySecurityException($"Access denied while accessing local NuGet repository path {JsonValue.Create(path)?.ToJsonString() ?? path}", path, error));

    protected override void Log(ILogger logger) => logger.LogLocalNugetRepositorySecurityError(Path, InnerException);
}
