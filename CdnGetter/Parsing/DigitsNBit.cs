using System.Numerics;

namespace CdnGetter.Parsing;

#pragma warning disable CA2231
    public readonly struct DigitsNBit : INumericalToken
#pragma warning restore CA2231
{
    public BigInteger Value { get; }

    public bool HasNegativeSign { get; }

    public int ZeroPadLength  { get; }

    bool INumericalToken.IsZero => Value.Equals(0);

    public DigitsNBit()
    {
        ZeroPadLength = 0;
        HasNegativeSign = false;
        Value = new(0);
    }

    public DigitsNBit(BigInteger value, bool hasNegativeSign, int leadingZeroCount = 0)
    {
        ZeroPadLength = (leadingZeroCount < 0) ? 0 : leadingZeroCount;
        HasNegativeSign = hasNegativeSign;
        Value = (value.Sign < 0) ? BigInteger.Negate(value) : value;
    }

    public DigitsNBit(BigInteger value, int leadingZeroCount = 0)
    {
        ZeroPadLength = (leadingZeroCount < 0) ? 0 : leadingZeroCount;
        HasNegativeSign = value.Sign < 0;
        Value = HasNegativeSign ? BigInteger.Negate(value) : value;
    }

    BigInteger INumericalToken.AsBigInteger() => Value;

    public int CompareTo(IToken? other)
    {
        if (other is null)
            return 1;
        if (other is INumericalToken numericalToken)
        {
            if (Value.Equals(0))
                return numericalToken.IsZero ? 0 : numericalToken.HasNegativeSign ? 1 : -1;
            if (numericalToken.IsZero)
                return HasNegativeSign ? -1 : 1;
            if (numericalToken.HasNegativeSign)
                return HasNegativeSign ? numericalToken.CompareAbs(Value) : 1;
            return HasNegativeSign ? -1 : 0 - numericalToken.CompareAbs(Value);
        }
        return ParsingExtensionMethods.NoCaseComparer.Compare(GetValue(), other.GetValue());
    }

    public bool Equals(IToken? other)
    {
        if (other is null)
            return false;
        if (other is INumericalToken numericalToken)
            return Value.Equals(0) ? numericalToken.IsZero : !numericalToken.IsZero && HasNegativeSign == numericalToken.HasNegativeSign && numericalToken.EqualsAbs(Value);
        return ParsingExtensionMethods.NoCaseComparer.Equals(GetValue(), other.GetValue());
    }

    public override bool Equals(object? obj) => obj is IToken other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(HasNegativeSign && !Value.Equals(0), Value);

    public int GetLength(bool allChars = false) => allChars ? (HasNegativeSign ? Value.ToString().Length + ZeroPadLength + 1 : Value.ToString().Length + ZeroPadLength) : GetValue().Length;

    public string GetValue() => (HasNegativeSign && !Value.Equals(0)) ? $"-{Value}" : Value.ToString();

    public override string ToString() => HasNegativeSign ? ((ZeroPadLength > 0) ? $"-{new string('0', ZeroPadLength)}{Value}" : $"-{Value}") :
        (ZeroPadLength > 0) ? $"{new string('0', ZeroPadLength)}{Value}" : Value.ToString();
    
    public bool TryGet8Bit(out byte value)
    {
        if (Value.CompareTo(byte.MaxValue) > 0)
        {
            value = default;
            return false;
        }
        value = (byte)Value;
        return true;
    }

    public bool TryGet16Bit(out ushort value)
    {
        if (Value.CompareTo(ushort.MaxValue) > 0)
        {
            value = default;
            return false;
        }
        value = (ushort)Value;
        return true;
    }

    public bool TryGet32Bit(out uint value)
    {
        if (Value.CompareTo(uint.MaxValue) > 0)
        {
            value = default;
            return false;
        }
        value = (uint)Value;
        return true;
    }

    public bool TryGet64Bit(out ulong value)
    {
        if (Value.CompareTo(ulong.MaxValue) > 0)
        {
            value = default;
            return false;
        }
        value = (ulong)Value;
        return true;
    }

    int INumericalToken.CompareAbs(BigInteger other) => Value.CompareTo(other);

    int INumericalToken.CompareAbs(ulong other) => Value.CompareTo(other);

    int INumericalToken.CompareAbs(uint other) => Value.CompareTo(other);

    int INumericalToken.CompareAbs(ushort other) => Value.CompareTo(other);

    int INumericalToken.CompareAbs(byte other) => Value.CompareTo(other);

    bool INumericalToken.EqualsAbs(BigInteger other) => Value.Equals(other);

    bool INumericalToken.EqualsAbs(ulong other) => Value.Equals(other);

    bool INumericalToken.EqualsAbs(uint other) => Value.Equals(other);

    bool INumericalToken.EqualsAbs(ushort other) => Value.Equals(other);

    bool INumericalToken.EqualsAbs(byte other) => Value.Equals(other);
}
