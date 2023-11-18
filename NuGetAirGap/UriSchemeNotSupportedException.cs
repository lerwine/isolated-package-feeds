namespace NuGetAirGap;

/// <summary>
/// Represents a Invalid filesystem URI exception.
/// </summary>
[Serializable]
public class UriSchemeNotSupportedException : Exception
{
    /// <summary>
    /// Gets the invalid filesystem URI string.
    /// </summary>
    public string? URIstring { get; }

    /// <summary>
    /// Gets the invalid URI scheme name.
    /// </summary>
    public string? UnsupportedScheme { get; }

    public UriSchemeNotSupportedException() { }

    /// <summary>
    /// Initializes a new <c>InvalidFileSystemUriException</c>.
    /// </summary>
    /// <param name="uri">The invalid URI.</param>
    /// <param name="innerException">The optional exception that is the cause of the current exception.</param>
    public UriSchemeNotSupportedException(Uri uri, Exception? innerException = null) : base(null, innerException) =>
        (UnsupportedScheme, URIstring) = uri.IsAbsoluteUri ? (uri.Scheme, uri.AbsoluteUri) : (null, uri.OriginalString);

    /// <summary>
    /// Initializes a new <c>InvalidFileSystemUriException</c>.
    /// </summary>
    /// <param name="uri">The invalid URI.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The optional exception that is the cause of the current exception.</param>
    public UriSchemeNotSupportedException(Uri uri, string message, Exception? innerException = null) : base(message, innerException) =>
        (UnsupportedScheme, URIstring) = uri.IsAbsoluteUri ? (uri.Scheme, uri.AbsoluteUri) : (null, uri.OriginalString);

    protected UriSchemeNotSupportedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
        UnsupportedScheme = info.GetString(nameof(UnsupportedScheme));
        URIstring = info.GetString(nameof(URIstring));
    }

    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
        info.AddValue(nameof(UnsupportedScheme), UnsupportedScheme);
        info.AddValue(nameof(URIstring), URIstring);
        base.GetObjectData(info, context);
    }
}
