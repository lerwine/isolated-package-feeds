namespace NuGetPuller;

/// <summary>
/// Represents a FileSystemAccess exception.
/// </summary>
public class FileSystemAccessException : Exception
{
    /// <summary>
    /// Gets the target path.
    /// </summary>
    public string Path { get; }

    public FileSystemAccessException() => Path = string.Empty;

    /// <summary>
    /// Initializes a new <c>FileSystemAccessException</c>.
    /// </summary>
    /// <param name="path">The target path.</param>
    /// <param name="innerException">The optional exception that is the cause of the current exception.</param>
    public FileSystemAccessException(string path, Exception? innerException = null) : base(null, innerException) => Path = path;

    /// <summary>
    /// Initializes a new <c>FileSystemAccessException</c>.
    /// </summary>
    /// <param name="path">The target path.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The optional exception that is the cause of the current exception.</param>
    public FileSystemAccessException(string path, string message, Exception? innerException = null) : base(message, innerException) => Path = path;
}