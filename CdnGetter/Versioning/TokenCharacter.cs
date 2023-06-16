using System.Collections;
using static CdnGetter.Versioning.VersioningConstants;

namespace CdnGetter.Versioning;

#pragma warning disable CA2231
[Obsolete("Use types from CdnGetter.Parsing namespace")]
public readonly struct TokenCharacter : ITokenCharacters
#pragma warning restore CA2231
{
    public static readonly TokenCharacter Dot = new(DELIMITER_DOT);

    public static readonly TokenCharacter Dash = new(DELIMITER_DASH);

    public static readonly TokenCharacter Slash = new(DELIMITER_SLASH);

    public static readonly TokenCharacter Colon = new(DELIMITER_COLON);

    public static readonly TokenCharacter SemiColon = new(DELIMITER_SEMICOLON);

    public static readonly TokenCharacter Plus = new(DELIMITER_PLUS);

    public static readonly TokenCharacter UnderScore = new(DELIMITER_UNDERSCORE);

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
    
    public ReadOnlySpan<char> AsSpan() => new ReadOnlySpan<char>(new char[] { Value });

    public int CompareTo(ITokenCharacters? other) => (other is null) ? 1 : NoCaseComparer.Compare(ToString(), other.ToString());

    public bool Equals(ITokenCharacters? other) => other is not null && NoCaseComparer.Equals(ToString(), other.ToString());

    public override bool Equals(object? obj) => obj is IToken other && Equals(other);

    public IEnumerator<char> GetEnumerator() => Enumerable.Repeat(Value, 1).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Enumerable.Repeat(Value, 1)).GetEnumerator();

    public override int GetHashCode() => NoCaseComparer.GetHashCode(ToString());

    public override string ToString() => new string(Value, 1);
}
