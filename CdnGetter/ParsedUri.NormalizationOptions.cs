namespace CdnGetter;

public partial class ParsedUri
{
    /// <summary>
    /// Normalization options for URI parsing.
    /// </summary>
    [Flags]
    public enum NormalizationOptions : byte
    {
        /// <summary>
        /// Do not normalize URI components.
        /// </summary>
        None = 0b0000_0000,

        /// <summary>
        /// Remove empty path segments.
        /// </summary>
        StripEmptyPathSegments = 0b0000_0001,

        /// <summary>
        /// Remove trailing path separators.
        /// </summary>
        StripTrailingPathSeparator = 0b0000_0010,

        /// <summary>
        /// Indicates that the path component should always end with a <c>'/'</c> character.
        /// </summary>
        EnsureTrailingPathSeparator = 0b0000_0100,

        /// <summary>
        /// Normalize relative path segments <c>&quot;.&quot;</c> and <c>&quot;..&quot;</c>.
        /// </summary>
        NormalizeDotPathSegments = 0b0000_1000,

        /// <summary>
        /// Remove query component if it is empty.
        /// </summary>
        StripEmptyQuery = 0b0001_0000,

        /// <summary>
        /// Remove fragment component if it is empty.
        /// </summary>
        StripEmptyFragment = 0b0010_0000,

        /// <summary>
        /// Remove port component if it is the same as the default port for the associated URI scheme.
        /// </summary>
        StripDefaultPort = 0b0100_0000
    }
}
