using System.Collections;

namespace CdnGetter.Parsing;

/// <summary>
/// Represents a parsed token containing a single character.
/// </summary>
#pragma warning disable CA2231
public readonly struct CharacterToken : IStringToken
#pragma warning restore CA2231
{
    public readonly char Value { get; }

    int IReadOnlyCollection<char>.Count => 1;

    char IReadOnlyList<char>.this[int index]
    {
        get
        {
            if (index != 0)
                throw new IndexOutOfRangeException();
            return Value;
        }
    }

    public CharacterToken(char value) => Value = value;
    
    public int CompareTo(IToken? other) => (other is null) ? 1 : Parsing.NoCaseComparer.Compare(ToString(), other.GetValue());

    public bool Equals(IToken? other) => other is not null && Parsing.NoCaseComparer.Equals(ToString(), other.GetValue());

    public override bool Equals(object? obj) => obj is IToken other && Equals(other);

    public IEnumerator<char> GetEnumerator() => Enumerable.Repeat(Value, 1).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Enumerable.Repeat(Value, 1)).GetEnumerator();

    public override int GetHashCode() => Parsing.NoCaseComparer.GetHashCode(ToString());

    public override string ToString() => new(Value, 1);

    int IToken.GetLength(bool allChars) => 1;

    string IToken.GetValue() => ToString();

    IEnumerable<char> IToken.GetSourceValues() { yield return Value; }
}
