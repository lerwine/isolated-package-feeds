using IsolatedPackageFeeds.Shared;

namespace NuGetPuller;

/// <summary>
/// Represents a Metadata Export Pathexception.
/// </summary>
public class OfflineMetaDataIOException : LoggedException
{
    /// <summary>
    /// Gets the path that was not found.
    /// </summary>
    public string Path { get; } = null!;

    public OfflineMetaDataIOException() { }

    /// <summary>
    /// Initializes a new <c>MetaDataExportPathExceptionException</c>.
    /// </summary>
    /// <param name="path">The path that was not found.</param>
    /// <param name="innerException">The optional exception that is the cause of the current exception.</param>
    public OfflineMetaDataIOException(string path, Exception? innerException = null) : base(null, innerException) => Path = path;

    /// <summary>
    /// Initializes a new <c>MetaDataExportPathExceptionException</c>.
    /// </summary>
    /// <param name="path">The path that was not found.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The optional exception that is the cause of the current exception.</param>
    public OfflineMetaDataIOException(string path, string message, Exception? innerException = null) : base(message, innerException) => Path = path;
}
