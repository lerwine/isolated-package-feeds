using System.Collections;
using static CdnGetter.Parsing.Version.Version;

namespace CdnGetter.Parsing;

/// <summary>
/// Represents a parsed token containing a string of characters.
/// </summary>
#pragma warning disable CA2231
public readonly struct StringToken : IStringToken
#pragma warning restore CA2231
{
    public readonly string Value { get; }

    int IReadOnlyCollection<char>.Count => Value.Length;

    char IReadOnlyList<char>.this[int index] => Value[index];

    public StringToken() => Value = WILDCARD_CHAR.ToString();

    public StringToken(string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("Value cannot be empty.", nameof(value));
        Value = value;
    }

    public StringToken(ReadOnlySpan<char> value)
    {
        if (value.Length == 0)
            throw new ArgumentException("Value cannot be empty.", nameof(value));
        Value = new(value);
    }

    public StringToken(ReadOnlySpan<char> value, int startIndex)
    {
        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        if (startIndex >= value.Length)
            throw new ArgumentException("Value cannot be empty.", nameof(value));
        Value = new((startIndex > 0) ? value[startIndex..] : value);
    }

    public StringToken(ReadOnlySpan<char> value, int startIndex, int endIndex)
    {
        if (startIndex < 0 || startIndex >= value.Length)
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        if (endIndex <= startIndex || endIndex > value.Length)
            throw new ArgumentOutOfRangeException(nameof(endIndex));
        Value = new((startIndex < 1) ? ((endIndex >= value.Length) ? value : value[..endIndex]) : (endIndex >= value.Length) ? value[startIndex..] : value[startIndex..endIndex]);
    }

    public int CompareTo(IToken? other) => (other is null) ? 1 : VersionComponentComparer.CompareTo(Value, other.GetValue());

    public bool Equals(IToken? other) => other is not null && VersionComponentComparer.AreEqual(Value, other.GetValue());

    public override bool Equals(object? obj) => obj is IToken other && Equals(other);

    IEnumerator<char> IEnumerable<char>.GetEnumerator() => Value.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Value).GetEnumerator();

    int IToken.GetLength(bool allChars) => Value.Length;

    string IToken.GetValue() => Value;

    public override int GetHashCode() => VersionComponentComparer.GetHashCodeOf(Value);

    public override string ToString() => Value;

    IEnumerable<char> IToken.GetSourceValues() => Value;
}