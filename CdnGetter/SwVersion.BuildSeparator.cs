namespace CdnGetter;

public readonly partial struct SwVersion
{
    /// <summary>
    /// Indicates the leading character for a <see cref="BuildSegment" />.
    /// </summary>
    public enum BuildSeparator
    {
        /// <summary>
        /// The <see cref="BuildSegment" /> starts with a <c>'+'</c> character.
        /// </summary>
        Plus,
        
        /// <summary>
        /// The <see cref="BuildSegment" /> starts with a <c>'.'</c> character.
        /// </summary>
        Dot,

        /// <summary>
        /// The <see cref="BuildSegment" /> starts with a <c>'-'</c> character.
        /// </summary>
        Dash
    }
}
