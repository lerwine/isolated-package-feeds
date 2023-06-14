using System.Collections;
using static CdnGetter.Versioning.VersioningConstants;

namespace CdnGetter.Versioning;

public readonly struct TokenString : ITokenCharacters
{
    public static readonly TokenString Empty = new();
    
    public readonly string Value { get; }

    int IReadOnlyCollection<char>.Count => Value.Length;

    char IReadOnlyList<char>.this[int index] => Value[index];

    public TokenString() => Value = string.Empty;

    public TokenString(string value) => Value = value ?? string.Empty;

    ReadOnlySpan<char> ITokenCharacters.AsSpan() => Value.AsSpan();

    public int CompareTo(ITokenCharacters? other) => (other is null) ? 1 : NoCaseComparer.Compare(Value, other.ToString());

    public bool Equals(ITokenCharacters? other) => other is not null && NoCaseComparer.Equals(Value, other.ToString());

    public override bool Equals(object? obj) => obj is IToken other && Equals(other);

    public IEnumerator<char> GetEnumerator() => Value.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Value).GetEnumerator();

    public override int GetHashCode() => NoCaseComparer.GetHashCode(Value);

    public override string ToString() => Value;
}