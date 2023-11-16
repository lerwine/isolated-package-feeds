using Microsoft.Extensions.Logging;

namespace NuGetAirGap;

/// <summary>
/// Represents a MetaDataExportPathException exception.
/// </summary>
[Serializable]
public class MetaDataExportPathException : Exception, ILogTrackable
{
    /// <summary>
    /// Gets the path that was not found.
    /// </summary>
    public string Path { get; } = null!;

    bool ILogTrackable.WasLogged => true;

    public MetaDataExportPathException() { }

    /// <summary>
    /// Initializes a new <c>MetaDataExportPathExceptionException</c>.
    /// </summary>
    /// <param name="path">The path that was not found.</param>
    /// <param name="innerException">The optional exception that is the cause of the current exception.</param>
    public MetaDataExportPathException(string path, Exception? innerException = null) : base(null, innerException) => Path = path;

    /// <summary>
    /// Initializes a new <c>MetaDataExportPathExceptionException</c>.
    /// </summary>
    /// <param name="path">The path that was not found.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The optional exception that is the cause of the current exception.</param>
    public MetaDataExportPathException(string path, string message, Exception? innerException = null) : base(message, innerException) => Path = path;

    protected MetaDataExportPathException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
        Path = info.GetString(nameof(Path))!;
    }

    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
        info.AddValue(nameof(Path), Path);
        base.GetObjectData(info, context);
    }

    void ILogTrackable.Log(ILogger logger, bool force) => logger.LogInvalidExportLocalMetaData(Path, m => this, InnerException);
}
