using System.Collections;
using static CdnGetter.Parsing.ParsingExtensionMethods;

namespace CdnGetter.Parsing;

#pragma warning disable CA2231
public readonly struct TokenCharacter : ITokenCharacters
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

    public TokenCharacter(char value) => Value = value;
    
    public int CompareTo(IToken? other) => (other is null) ? 1 : NoCaseComparer.Compare(ToString(), other.GetValue());

    public bool Equals(IToken? other) => other is not null && NoCaseComparer.Equals(ToString(), other.GetValue());

    public override bool Equals(object? obj) => obj is IToken other && Equals(other);

    public IEnumerator<char> GetEnumerator() => Enumerable.Repeat(Value, 1).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Enumerable.Repeat(Value, 1)).GetEnumerator();

    public override int GetHashCode() => NoCaseComparer.GetHashCode(ToString());

    public override string ToString() => new(Value, 1);

    int IToken.GetLength(bool allChars) => 1;

    string IToken.GetValue() => ToString();
}
