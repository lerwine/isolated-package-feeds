namespace CdnGetter.Versioning;

public interface ITokenCharacters : IComparable<ITokenCharacters>, IEquatable<ITokenCharacters>, IReadOnlyList<char>
{
    ReadOnlySpan<char> AsSpan();
    
    string ToString();
}
