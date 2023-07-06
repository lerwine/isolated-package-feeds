using System.Collections;

namespace CdnGetter.Parsing;

/// <summary>
/// Represents a parsed token containing a string of characters.
/// </summary>
#pragma warning disable CA2231
public readonly struct StringToken : IStringToken
#pragma warning restore CA2231
{
    public static readonly StringToken Empty = new();
    
    public readonly string Value { get; }

    int IReadOnlyCollection<char>.Count => Value.Length;

    char IReadOnlyList<char>.this[int index] => Value[index];

    public StringToken() => Value = string.Empty;

    public StringToken(string value) => Value = value ?? string.Empty;

    public StringToken(ReadOnlySpan<char> value) => Value = (value.Length > 0) ? new(value) : string.Empty;

    public StringToken(ReadOnlySpan<char> value, int startIndex) => Value = (startIndex >= value.Length) ? string.Empty : new((startIndex < 1) ? value : value[startIndex..]);

    public StringToken(ReadOnlySpan<char> value, int startIndex, int endIndex) => Value = (endIndex <= startIndex || startIndex >= value.Length) ? string.Empty : new((startIndex < 1) ?
        ((endIndex >= value.Length) ? value : value[..endIndex]) : (endIndex >= value.Length) ? value[startIndex..] : value[startIndex..endIndex]);

    public int CompareTo(IToken? other) => (other is null) ? 1 : ParsingExtensionMethods.NoCaseComparer.Compare(Value, other.GetValue());

    public bool Equals(IToken? other) => other is not null && ParsingExtensionMethods.NoCaseComparer.Equals(Value, other.GetValue());

    public override bool Equals(object? obj) => obj is IToken other && Equals(other);

    IEnumerator<char> IEnumerable<char>.GetEnumerator() => Value.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Value).GetEnumerator();

    int IToken.GetLength(bool allChars) => Value.Length;

    string IToken.GetValue() => Value;

    public override int GetHashCode() => ParsingExtensionMethods.NoCaseComparer.GetHashCode(Value);

    public override string ToString() => Value;

    IEnumerable<char> IToken.GetSourceValues() => Value;
}