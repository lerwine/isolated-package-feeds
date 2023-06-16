namespace CdnGetter.Versioning;

[Obsolete("Use types from CdnGetter.Parsing namespace")]
public interface ITokenCharacters : IComparable<ITokenCharacters>, IEquatable<ITokenCharacters>, IReadOnlyList<char>
{
    ReadOnlySpan<char> AsSpan();
    
    string ToString();
}
