namespace CdnGetter;

public partial class Url
{
    /// <summary>
    /// Specifies the characters that follow the URI scheme separator.
    /// </summary>
    public enum SchemeSeparatorType : byte
    {
        /// <summary>
        /// Schema separator is followed by 2 slashes (not including path separator).
        /// </summary>
        DoubleSlash = 2,
        
        /// <summary>
        /// Schema separator is followed by 2 slashes (not including path separator).
        /// </summary>
        SingleSlash = 1,
        
        /// <summary>
        /// Schema separator is not followed by a slash (not including path separator).
        /// </summary>
        NoSlash = 0
    }
}
