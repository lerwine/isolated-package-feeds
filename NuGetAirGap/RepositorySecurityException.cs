using Microsoft.Extensions.Logging;

namespace NuGetAirGap;

/// <summary>
/// Represents a Repository security exception.
/// </summary>
[Serializable]
public class RepositorySecurityException : Exception, ILogTrackable
{
    /// <summary>
    /// Gets the NuGet repository path.
    /// </summary>
    public string Path { get; } = null!;

    /// <summary>
    /// Gets a value indicating whether the exception was for an upstream repository.
    /// </summary>
    public bool IsUpstream { get; }

    bool ILogTrackable.WasLogged => true;

    public RepositorySecurityException() { }

    /// <summary>
    /// Initializes a new <c>REpositorySecurityException</c>.
    /// </summary>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="isUpstream">A value indicating whether the exception was for an upstream repository.</param>
    /// <param name="innerException">The optional exception that is the cause of the current exception.</param>
    public RepositorySecurityException(string path, bool isUpstream, Exception? innerException = null) : base(null, innerException) =>
        (Path, IsUpstream) = (path, isUpstream);

    /// <summary>
    /// Initializes a new <c>REpositorySecurityException</c>.
    /// </summary>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="isUpstream">A value indicating whether the exception was for an upstream repository.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The optional exception that is the cause of the current exception.</param>
    public RepositorySecurityException(string path, bool isUpstream, string message, Exception? innerException = null) : base(message, innerException) =>
        (Path, IsUpstream) = (path, isUpstream);

    protected RepositorySecurityException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
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

    void ILogTrackable.Log(ILogger logger, bool force) => logger.LogRepositorySecurityException(Path, IsUpstream, m => this, InnerException);
}
