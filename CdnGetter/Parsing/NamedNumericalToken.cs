using System.Collections;
using System.Numerics;

namespace CdnGetter.Parsing;

public readonly struct NamedNumericalToken : INumericalToken, ITokenList
{
    public INumericalToken ValueToken { get; }
    
    public IStringToken NameToken { get; }

    bool INumericalToken.HasNegativeSign => ValueToken.HasNegativeSign;

    int INumericalToken.ZeroPadLength => ValueToken.ZeroPadLength;

    bool INumericalToken.IsZero => ValueToken.IsZero;

    int IReadOnlyCollection<IToken>.Count => 2;

    IToken IReadOnlyList<IToken>.this[int index] => index switch
    {
        0 => ValueToken,
        1 => NameToken,
        _ => throw new IndexOutOfRangeException()
    };

    public NamedNumericalToken() => (ValueToken, NameToken) = (Digits8Bit.Zero, StringToken.Empty);
    
    public NamedNumericalToken(INumericalToken valueToken, IStringToken name) =>
        (ValueToken, NameToken) = (valueToken ?? throw new ArgumentNullException(nameof(valueToken)), name ?? throw new ArgumentNullException(nameof(name)));

    int INumericalToken.CompareAbs(BigInteger other) => ValueToken.CompareAbs(other);

    int INumericalToken.CompareAbs(ulong other) => ValueToken.CompareAbs(other);

    int INumericalToken.CompareAbs(uint other) => ValueToken.CompareAbs(other);

    int INumericalToken.CompareAbs(ushort other) => ValueToken.CompareAbs(other);

    int INumericalToken.CompareAbs(byte other) => ValueToken.CompareAbs(other);

    bool INumericalToken.EqualsAbs(BigInteger other) => ValueToken.EqualsAbs(other);

    bool INumericalToken.EqualsAbs(ulong other) => ValueToken.EqualsAbs(other);

    bool INumericalToken.EqualsAbs(uint other) => ValueToken.EqualsAbs(other);

    bool INumericalToken.EqualsAbs(ushort other) => ValueToken.EqualsAbs(other);

    bool INumericalToken.EqualsAbs(byte other) => ValueToken.EqualsAbs(other);

    bool INumericalToken.TryGet8Bit(out byte value) => ValueToken.TryGet8Bit(out value);

    bool INumericalToken.TryGet16Bit(out ushort value) => ValueToken.TryGet16Bit(out value);

    bool INumericalToken.TryGet32Bit(out uint value) => ValueToken.TryGet32Bit(out value);

    bool INumericalToken.TryGet64Bit(out ulong value) => ValueToken.TryGet64Bit(out value);

    BigInteger INumericalToken.AsBigInteger() => ValueToken.AsBigInteger();

    public int GetLength(bool allParsedValues = false) => ValueToken.GetLength(allParsedValues) + NameToken.GetLength(allParsedValues);

    public string GetValue() => ValueToken.GetValue() + NameToken.GetValue();

    public int CompareTo(IToken? other)
    {
        return ValueToken.CompareTo(other);
    }

    public bool Equals(IToken? other)
    {
        return ValueToken.Equals(other);
    }

    public override string ToString() => ValueToken.ToString() + NameToken.ToString();

    private IEnumerable<IToken> GetValues()
    {
        yield return ValueToken;
        yield return NameToken;
    }

    IEnumerator<IToken> IEnumerable<IToken>.GetEnumerator() => GetValues().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)GetValues()).GetEnumerator();

    IEnumerable<char> IToken.GetSourceValues() => ValueToken.GetSourceValues().Concat(NameToken.GetSourceValues());
}
