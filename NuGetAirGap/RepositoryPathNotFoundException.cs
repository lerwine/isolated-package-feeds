using Microsoft.Extensions.Logging;

namespace NuGetAirGap;

/// <summary>
/// Represents a RepositoryPathNotFound exception.
/// </summary>
[Serializable]
public class RepositoryPathNotFoundException : Exception, ILogTrackable
{
    /// <summary>
    /// Gets the path that was not found.
    /// </summary>
    public string Path { get; } = null!;

    /// <summary>
    /// Gets a value indicating whether the exception was for an upstream NuGet repository.
    /// </summary>
    public bool IsUpstream { get; }

    bool ILogTrackable.WasLogged => true;

    public RepositoryPathNotFoundException() { }

    /// <summary>
    /// Initializes a new <c>RepositoryPathNotFoundException</c>.
    /// </summary>
    /// <param name="path">The path that was not found.</param>
    /// <param name="isUpstream">A value indicating whether the exception was for an upstream NuGet repository.</param>
    /// <param name="innerException">The optional exception that is the cause of the current exception.</param>
    public RepositoryPathNotFoundException(string path, bool isUpstream, Exception? innerException = null) : base(null, innerException) =>
        (Path, IsUpstream) = (path, isUpstream);

    /// <summary>
    /// Initializes a new <c>RepositoryPathNotFoundException</c>.
    /// </summary>
    /// <param name="path">The path that was not found.</param>
    /// <param name="isUpstream">A value indicating whether the exception was for an upstream NuGet repository.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The optional exception that is the cause of the current exception.</param>
    public RepositoryPathNotFoundException(string path, bool isUpstream, string message, Exception? innerException = null) : base(message, innerException) =>
        (Path, IsUpstream) = (path, isUpstream);

    protected RepositoryPathNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
        Path = info.GetString(nameof(Path))!;
        IsUpstream = info.GetBoolean(nameof(IsUpstream));
    }

    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
        info.AddValue(nameof(Path), Path);
        info.AddValue(nameof(IsUpstream), IsUpstream);
        base.GetObjectData(info, context);
    }

    void ILogTrackable.Log(ILogger logger, bool force) => logger.LogRepositoryPathNotFound(Path, IsUpstream, m => this, InnerException);
}
