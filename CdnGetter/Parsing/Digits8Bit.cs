using System.Numerics;
using static CdnGetter.Parsing.ParsingExtensionMethods;

namespace CdnGetter.Parsing;

#pragma warning disable CA2231
    public readonly struct Digits8Bit : INumericalToken
#pragma warning restore CA2231
{
    public static readonly Digits8Bit MaxValue = new(byte.MaxValue);
    
    public static readonly int MAX_ABS_LENGTH;

    public byte Value { get; }

    public bool HasNegativeSign { get; }

    public int ZeroPadLength { get; }

    bool INumericalToken.IsZero => Value == 0;

    static Digits8Bit()  => MAX_ABS_LENGTH = byte.MaxValue.ToString().Length;

    public Digits8Bit()
    {
        ZeroPadLength = 0;
        HasNegativeSign = false;
        Value = 0;
    }
    
    public Digits8Bit(sbyte value, int leadingZeroCount = 0)
    {
        HasNegativeSign = value < 0;
        Value = (byte)(HasNegativeSign ? value * -1 : value);
        ZeroPadLength = (leadingZeroCount < 0) ? 0 : leadingZeroCount;
    }

    public Digits8Bit(byte value, bool hasNegativeSign = false, int leadingZeroCount = 0)
    {
        HasNegativeSign = hasNegativeSign;
        Value = value;
        ZeroPadLength = (leadingZeroCount < 0) ? 0 : leadingZeroCount;
    }

    public Digits8Bit(byte value, int leadingZeroCount)
    {
        HasNegativeSign = false;
        Value = value;
        ZeroPadLength = (leadingZeroCount < 0) ? 0 : leadingZeroCount;
    }

    public BigInteger AsBigInteger() => new(Value);

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
        return NoCaseComparer.Compare(GetValue(), other.GetValue());
    }

    public bool Equals(IToken? other)
    {
        if (other is null)
            return false;
        if (other is INumericalToken numericalToken)
            return (Value == 0) ? numericalToken.IsZero : !numericalToken.IsZero && HasNegativeSign == numericalToken.HasNegativeSign && numericalToken.EqualsAbs(Value);
        return NoCaseComparer.Equals(GetValue(), other.GetValue());
    }

    public override bool Equals(object? obj) => obj is IToken other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(HasNegativeSign && Value != 0, (ulong)Value);

    public int GetLength(bool allChars = false) => allChars ? (HasNegativeSign ? Value.ToString().Length + ZeroPadLength + 1 : Value.ToString().Length + ZeroPadLength) : GetValue().Length;

    public string GetValue() => (HasNegativeSign && Value != 0) ? $"-{Value}" : Value.ToString();

    public override string ToString() => HasNegativeSign ? ((ZeroPadLength > 0) ? $"-{new string('0', ZeroPadLength)}{Value}" : $"-{Value}") :
        (ZeroPadLength > 0) ? $"{new string('0', ZeroPadLength)}{Value}" : Value.ToString();
    
    bool INumericalToken.TryGet8Bit(out byte value)
    {
        value = Value;
        return true;
    }

    bool INumericalToken.TryGet16Bit(out ushort value)
    {
        value = Value;
        return true;
    }

    bool INumericalToken.TryGet32Bit(out uint value)
    {
        value = Value;
        return true;
    }

    bool INumericalToken.TryGet64Bit(out ulong value)
    {
        value = Value;
        return true;
    }

    int INumericalToken.CompareAbs(BigInteger other) => (other.CompareTo(byte.MaxValue) > 0) ? 1: Value.CompareTo((byte)other);

    int INumericalToken.CompareAbs(ulong other) => (other > byte.MaxValue) ? 1 : Value.CompareTo((byte)other);

    int INumericalToken.CompareAbs(uint other) => (other > byte.MaxValue) ? 1 : Value.CompareTo((byte)other);

    int INumericalToken.CompareAbs(ushort other) => (other > byte.MaxValue) ? 1 : Value.CompareTo((byte)other);

    int INumericalToken.CompareAbs(byte other) => Value.CompareTo(other);

    bool INumericalToken.EqualsAbs(BigInteger other) => other.CompareTo(byte.MaxValue) <= 0 && Value.Equals((byte)other);

    bool INumericalToken.EqualsAbs(ulong other) => other <= byte.MaxValue && Value.Equals((byte)other);

    bool INumericalToken.EqualsAbs(uint other) => other <= byte.MaxValue && Value.Equals((byte)other);

    bool INumericalToken.EqualsAbs(ushort other) => other <= byte.MaxValue && Value.Equals((byte)other);

    bool INumericalToken.EqualsAbs(byte other) => Value.Equals(other);
}
