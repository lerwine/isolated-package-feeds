namespace NuGetPuller;

/// <summary>
/// Represents a Invalid filesystem URI exception.
/// </summary>
public class InvalidRepositoryUriException : LoggedException
{
    /// <summary>
    /// Gets the invalid filesystem URI string.
    /// </summary>
    public string? URIstring { get; }

    public InvalidRepositoryUriException() { }

    /// <summary>
    /// Initializes a new <c>InvalidFileSystemUriException</c>.
    /// </summary>
    /// <param name="uriString">The invalid URI.</param>
    /// <param name="innerException">The optional exception that is the cause of the current exception.</param>
    public InvalidRepositoryUriException(string uriString, Exception? innerException = null) : base(null, innerException) => URIstring = uriString;

    /// <summary>
    /// Initializes a new <c>InvalidFileSystemUriException</c>.
    /// </summary>
    /// <param name="uriString">The invalid URI.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The optional exception that is the cause of the current exception.</param>
    public InvalidRepositoryUriException(string uriString, string message, Exception? innerException = null) : base(message, innerException) => URIstring = uriString;
}
