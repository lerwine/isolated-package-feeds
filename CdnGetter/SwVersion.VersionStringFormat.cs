namespace CdnGetter;

public readonly partial struct SwVersion
{
    /// <summary>
    /// Indicates the format of a version string.
    /// </summary>
    public enum VersionStringFormat
    {
        /// <summary>
        /// Version string contains no numerical components that were parsable by <see cref="SwVersion" />.
        /// </summary>
        NonNumerical,
        
        /// <summary>
        /// Version string that eitehr has no <see cref="PreRelease" /> component or the <see cref="PreRelease" /> component begins with a <c>'-'</c> character.
        /// </summary>
        Standard,
        
        /// <summary>
        /// Version string with a <see cref="PreRelease" /> component that does not begin with a <c>'-'</c> character.
        /// </summary>
        Alt
    }
}
