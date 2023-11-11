using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;

namespace NuGetAirGap;

/// <summary>
/// Represents a InvalidRepositoryUrl exception.
/// </summary>
[Serializable]
public class InvalidRepositoryUrlException : Exception, ILogTrackable
{
    /// <summary>
    /// Gets the invalid repository URL/path.
    /// </summary>
    public string URL { get; } = null!;

    /// <summary>
    /// Gets a value indicating whether the exception was for an upstream repository.
    /// </summary>
    public bool IsUpstream { get; }

    bool ILogTrackable.WasLogged => true;

    public InvalidRepositoryUrlException() { }

    /// <summary>
    /// Initializes a new <c>INVALIDRepositoryUrlException</c>.
    /// </summary>
    /// <param name="url">The invalid repository URL/path.</param>
    /// <param name="isUpstream">A value indicating whether the exception was for an upstream repository.</param>
    /// <param name="innerException">The optional exception that is the cause of the current exception.</param>
    public InvalidRepositoryUrlException(string url, bool isUpstream, Exception? innerException = null) : base(null, innerException) =>
        (URL, IsUpstream) = (url, isUpstream);

    /// <summary>
    /// Initializes a new <c>INVALIDRepositoryUrlException</c>.
    /// </summary>
    /// <param name="url">The invalid repository URL/path.</param>
    /// <param name="isUpstream">A value indicating whether the exception was for an upstream repository.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The optional exception that is the cause of the current exception.</param>
    public InvalidRepositoryUrlException(string url, bool isUpstream, string message, Exception? innerException = null) : base(message, innerException) =>
        (URL, IsUpstream) = (url, isUpstream);

    protected InvalidRepositoryUrlException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        URL = info.GetString(nameof(URL))!;
        IsUpstream = info.GetBoolean(nameof(IsUpstream));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(URL), URL);
        info.AddValue(nameof(IsUpstream), IsUpstream);
        base.GetObjectData(info, context);
    }

    void ILogTrackable.Log(ILogger logger, bool force) => logger.LogInvalidRepositoryUrl(URL, IsUpstream, m => this, InnerException);
}
