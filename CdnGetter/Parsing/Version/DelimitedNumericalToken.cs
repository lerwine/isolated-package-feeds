using System.Collections;
using System.Numerics;
using static CdnGetter.Parsing.Version.Version;

namespace CdnGetter.Parsing.Version;

/// <summary>
/// Represents a numerical token that is preceded by a delimiter.
/// </summary>
#pragma warning disable CA2231
public readonly struct DelimitedNumericalToken : INumericalToken, IDelimitedToken
#pragma warning restore CA2231
{
    /// <summary>
    /// Gets the delimiter that precedes the numerical token.
    /// </summary>
    /// <value></value>
    public IStringToken DelimiterToken { get; }

    public INumericalToken ValueToken { get; }

    IToken IDelimitedToken.ValueToken => ValueToken;
    
    private static StringToken AssertValidDelimiter(StringToken delimiter)
    {
        if (delimiter.Value.Length > 0 && char.IsNumber(delimiter.Value[^1]))
            throw new ArgumentException($"The final character of the {nameof(delimiter)} cannot be a number.", nameof(delimiter));
        return delimiter;
    }

    private static CharacterToken AssertValidDelimiter(CharacterToken delimiter)
    {
        if (char.IsNumber(delimiter.Value))
            throw new ArgumentException($"The {nameof(delimiter)} cannot be a number.", nameof(delimiter));
        return delimiter;
    }

    public DelimitedNumericalToken() => (DelimiterToken, ValueToken) = (DotToken, Digits8Bit.Zero);

    public DelimitedNumericalToken(StringToken delimiter, Digits8Bit valueToken) => (DelimiterToken, ValueToken) = (AssertValidDelimiter(delimiter), valueToken);

    public DelimitedNumericalToken(CharacterToken delimiter, Digits8Bit valueToken) => (DelimiterToken, ValueToken) = (AssertValidDelimiter(delimiter), valueToken);

    public DelimitedNumericalToken(StringToken delimiter, Digits16Bit valueToken) => (DelimiterToken, ValueToken) = (AssertValidDelimiter(delimiter), valueToken);

    public DelimitedNumericalToken(CharacterToken delimiter, Digits16Bit valueToken) => (DelimiterToken, ValueToken) = (AssertValidDelimiter(delimiter), valueToken);

    public DelimitedNumericalToken(StringToken delimiter, Digits32Bit valueToken) => (DelimiterToken, ValueToken) = (AssertValidDelimiter(delimiter), valueToken);

    public DelimitedNumericalToken(CharacterToken delimiter, Digits32Bit valueToken) => (DelimiterToken, ValueToken) = (AssertValidDelimiter(delimiter), valueToken);

    public DelimitedNumericalToken(StringToken delimiter, Digits64Bit valueToken) => (DelimiterToken, ValueToken) = (AssertValidDelimiter(delimiter), valueToken);

    public DelimitedNumericalToken(CharacterToken delimiter, Digits64Bit valueToken) => (DelimiterToken, ValueToken) = (AssertValidDelimiter(delimiter), valueToken);

    public DelimitedNumericalToken(StringToken delimiter, DigitsNBit valueToken) => (DelimiterToken, ValueToken) = (AssertValidDelimiter(delimiter), valueToken);

    public DelimitedNumericalToken(CharacterToken delimiter, DigitsNBit valueToken) => (DelimiterToken, ValueToken) = (AssertValidDelimiter(delimiter), valueToken);

    public DelimitedNumericalToken(StringToken delimiter, RomanNumeral valueToken)
    {
        if (delimiter.Value.Length > 0)
        {
            char c = delimiter.Value[^1];
            if (char.IsNumber(c) || RomanNumeral.IsRomanNumeralChar(c))
                throw new ArgumentException($"The final character of the {nameof(delimiter)} cannot be a number or roman numeral character.", nameof(delimiter));
        }
        if (valueToken.Value == 0)
            throw new ArgumentException($"{nameof(valueToken)} cannot be a zero-length.", nameof(valueToken));
        DelimiterToken = delimiter;
        ValueToken = valueToken;
    }

    public DelimitedNumericalToken(CharacterToken delimiter, RomanNumeral valueToken)
    {
        if (char.IsNumber(delimiter.Value) || RomanNumeral.IsRomanNumeralChar(delimiter.Value))
            throw new ArgumentException($"The {nameof(delimiter)} cannot be a number or roman numeral character.", nameof(delimiter));
        if (valueToken.Value == 0)
            throw new ArgumentException($"{nameof(valueToken)} cannot be a zero-length.", nameof(valueToken));
        DelimiterToken = delimiter;
        ValueToken = valueToken;
    }

    public DelimitedNumericalToken(StringToken delimiter, NamedNumericalToken valueToken) => (DelimiterToken, ValueToken) = (AssertValidDelimiter(delimiter), valueToken);

    public DelimitedNumericalToken(CharacterToken delimiter, NamedNumericalToken valueToken) => (DelimiterToken, ValueToken) = (AssertValidDelimiter(delimiter), valueToken);

    bool INumericalToken.HasNegativeSign => ValueToken.HasNegativeSign;

    int INumericalToken.ZeroPadLength => ValueToken.ZeroPadLength;

    bool INumericalToken.IsZero => ValueToken.IsZero;

    int IReadOnlyCollection<IToken>.Count => 2;

    IToken IReadOnlyList<IToken>.this[int index] => index switch
    {
        0 => DelimiterToken,
        1 => ValueToken,
        _ => throw new IndexOutOfRangeException()
    };

    BigInteger INumericalToken.AsBigInteger() => ValueToken.AsBigInteger();

    int INumericalToken.CompareAbs(BigInteger other) => ValueToken.CompareAbs(other);

    int INumericalToken.CompareAbs(ulong other) => ValueToken.CompareAbs(other);

    int INumericalToken.CompareAbs(uint other) => ValueToken.CompareAbs(other);

    int INumericalToken.CompareAbs(ushort other) => ValueToken.CompareAbs(other);

    int INumericalToken.CompareAbs(byte other) => ValueToken.CompareAbs(other);

    int IComparable<IToken>.CompareTo(IToken? other) => ValueToken.CompareTo(other);

    bool IEquatable<IToken>.Equals(IToken? other) => ValueToken.Equals(other);

    bool INumericalToken.EqualsAbs(BigInteger other) => ValueToken.EqualsAbs(other);

    bool INumericalToken.EqualsAbs(ulong other) => ValueToken.EqualsAbs(other);

    bool INumericalToken.EqualsAbs(uint other) => ValueToken.EqualsAbs(other);

    bool INumericalToken.EqualsAbs(ushort other) => ValueToken.EqualsAbs(other);

    bool INumericalToken.EqualsAbs(byte other) => ValueToken.EqualsAbs(other);

    public int GetLength(bool allParsedValues) => DelimiterToken.GetLength(allParsedValues) + ValueToken.GetLength(allParsedValues);

    public string GetValue() => DelimiterToken.GetValue() + ValueToken.GetValue();

    public override string ToString() => DelimiterToken.GetValue() + ValueToken.GetValue();

    bool INumericalToken.TryGet16Bit(out ushort value) => ValueToken.TryGet16Bit(out value);

    bool INumericalToken.TryGet32Bit(out uint value) => ValueToken.TryGet32Bit(out value);

    bool INumericalToken.TryGet64Bit(out ulong value) => ValueToken.TryGet64Bit(out value);

    bool INumericalToken.TryGet8Bit(out byte value) => ValueToken.TryGet8Bit(out value);

    private IEnumerable<IToken> GetValues()
    {
        yield return DelimiterToken;
        yield return ValueToken;
    }

    IEnumerator<IToken> IEnumerable<IToken>.GetEnumerator() => GetValues().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)GetValues()).GetEnumerator();

    public IEnumerable<char> GetSourceValues() => DelimiterToken.GetSourceValues().Concat(ValueToken.GetSourceValues());
}
