using System.Collections;
using System.Numerics;

namespace CdnGetter.Parsing;

public readonly struct DelimitedNumericalToken : INumericalToken, ITokenList
{
    public IStringToken DelimiterToken { get; }

    public INumericalToken ValueToken { get; }
    
    public DelimitedNumericalToken() => (DelimiterToken, ValueToken) = (StringToken.Empty, Digits8Bit.Zero);
    
    public DelimitedNumericalToken(IStringToken delimiter, INumericalToken valueToken)
    {
        if ((valueToken ?? throw new ArgumentNullException(nameof(valueToken))) is DelimitedNumericalToken || valueToken is NamedNumericalToken)
            throw new ArgumentException($"{nameof(valueToken)} cannot be a delimited or named numerical token.", nameof(valueToken));
        if ((delimiter ?? throw new ArgumentNullException(nameof(delimiter))) is INumericalToken)
            throw new ArgumentException($"{nameof(delimiter)} cannot be a numerical token.", nameof(delimiter));
        if (delimiter.Count > 0 && char.IsDigit(delimiter[^1]))
                throw new ArgumentException($"The final character of the {nameof(delimiter)} cannot be a digit.", nameof(delimiter));
        if (valueToken.GetLength() == 0)
            throw new ArgumentException($"{nameof(valueToken)} cannot be a zero-length.", nameof(valueToken));
        DelimiterToken = delimiter;
        ValueToken = valueToken;
    }
    
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

    IEnumerable<char> IToken.GetSourceValues() => DelimiterToken.GetSourceValues().Concat(ValueToken.GetSourceValues());
}
