namespace NuGetPuller;

/// <summary>
/// Represents a Invalid filesystem URI exception.
/// </summary>
public class InvalidGlobalPackagesPathException : LoggedException
{
    /// <summary>
    /// Gets the invalid filesystem URI string.
    /// </summary>
    public string? Path { get; }

    public InvalidGlobalPackagesPathException() { }

    /// <summary>
    /// Initializes a new <c>InvalidFileSystemUriException</c>.
    /// </summary>
    /// <param name="path">The invalid path.</param>
    /// <param name="innerException">The optional exception that is the cause of the current exception.</param>
    public InvalidGlobalPackagesPathException(string path, Exception? innerException = null) : base(null, innerException) => Path = path;

    /// <summary>
    /// Initializes a new <c>InvalidFileSystemUriException</c>.
    /// </summary>
    /// <param name="path">The invalid path.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The optional exception that is the cause of the current exception.</param>
    public InvalidGlobalPackagesPathException(string path, string message, Exception? innerException = null) : base(message, innerException) => Path = path;
}
