using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using static CdnGetter.Parsing.ParsingExtensionMethods;

namespace CdnGetter.Parsing;

#pragma warning disable CA2231
public readonly partial struct RomanNumeral : INumericalToken
#pragma warning restore CA2231
{
    public static readonly RomanNumeral MaxValue = new(ROMAN_NUMERAL_MAX_VALUE);
    
    public static readonly RomanNumeral Empty = new();
    
    public string? Value { get; }

    bool INumericalToken.HasNegativeSign => false;

    int INumericalToken.ZeroPadLength => 0;

    public bool IsZero => Value is null;

    public RomanNumeral(ushort value) => Value = ToRomanNumeral(value);
    
    private RomanNumeral(string? value) => Value = value;
    
    public BigInteger AsBigInteger() => new(GetValue());

    public int CompareTo(IToken? other)
    {
        if (other is null)
            return 1;
        if (other is INumericalToken numericalToken)
        {
            if (Value is null)
                return numericalToken.IsZero ? 0 : numericalToken.HasNegativeSign ? 1 : -1;
            return (numericalToken.IsZero || numericalToken.HasNegativeSign) ? 1 : (numericalToken.TryGet16Bit(out ushort s) && s <= ROMAN_NUMERAL_MAX_VALUE) ? GetValue().CompareTo(s) : -1;
        }
        return NoCaseComparer.Compare(GetValue(), other.GetValue());
    }

    public bool Equals(IToken? other)
    {
        if (other is null)
            return false;
        if (other is INumericalToken numericalToken)
            return (Value is null) ? numericalToken.IsZero : !numericalToken.IsZero && !numericalToken.HasNegativeSign && numericalToken.TryGet16Bit(out ushort value) &&
                value <= ROMAN_NUMERAL_MAX_VALUE && GetValue() == value;
        return NoCaseComparer.Equals(GetValue(), other.GetValue());
    }

    public override bool Equals(object? obj) => obj is IToken other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(false, (ulong)GetValue());

    public int GetLength() => Value?.Length ?? 0;

    int IToken.GetLength(bool allChars) => GetLength();

    public ushort GetValue() => string.IsNullOrEmpty(Value) ? (ushort)0 : Value[0] switch
    {
        ROMAN_NUM_1000_UC or ROMAN_NUM_1000_LC => ParseFromM(Value.AsSpan(), 0, Value.Length, out _),
        ROMAN_NUM_500_UC or ROMAN_NUM_500_LC => ParseFromD(Value.AsSpan(), 0, Value.Length, out _),
        ROMAN_NUM_100_UC or ROMAN_NUM_100_LC => ParseFromC(Value.AsSpan(), 0, Value.Length, out _),
        ROMAN_NUM_50_UC or ROMAN_NUM_50_LC => ParseFromL(Value.AsSpan(), 0, Value.Length, out _),
        ROMAN_NUM_10_UC or ROMAN_NUM_10_LC => ParseFromX(Value.AsSpan(), 0, Value.Length, out _),
        ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => ParseFromV(Value.AsSpan(), 0, Value.Length, out _),
        _ => ParseFromI(Value.AsSpan(), 0, Value.Length, out _)
    };

    string IToken.GetValue() => Value ?? "";

    public override string ToString() => Value ?? "";

    public bool TryGet8Bit(out byte value)
    {
        if (Value is null)
        {
            value = 0;
            return true;
        }
        ushort v = GetValue();
        if (v <= byte.MaxValue)
        {
            value = (byte)v;
            return true;
        }
        value = 0;
        return false;
    }

    bool INumericalToken.TryGet16Bit(out ushort value)
    {
        value = GetValue();
        return true;
    }

    bool INumericalToken.TryGet32Bit(out uint value)
    {
        value = GetValue();
        return true;
    }

    bool INumericalToken.TryGet64Bit(out ulong value)
    {
        value = GetValue();
        return true;
    }

    public static RomanNumeral Parse(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Empty;
        if (TryParse(text.AsSpan(), 0, text.Length, out RomanNumeral result, out int nextIndex))
        {
            if (nextIndex == text.Length)
                return result;
        }
        else
            nextIndex = 0;
        throw new ArgumentException($"Invalid roman numeral at index {nextIndex}.", nameof(text));
    }
    
    public static RomanNumeral Parse(ReadOnlySpan<char> text)
    {
        if (text.IsEmpty)
            return Empty;
        if (TryParse(text, 0, text.Length, out RomanNumeral result, out int nextIndex))
        {
            if (nextIndex == text.Length)
                return result;
        }
        else
            nextIndex = 0;
        throw new ArgumentException($"Invalid roman numeral at index {nextIndex}.", nameof(text));
    }
    
    public static bool TryParse(string text, out RomanNumeral result)
    {
        if (string.IsNullOrEmpty(text))
            result = Empty;
        else if (!TryParse(text.AsSpan(), 0, text.Length, out result, out int nextIndex) || nextIndex < text.Length)
        {
            result = Empty;
            return false;
        }
        return true;
    }
    
    public static bool TryParse(ReadOnlySpan<char> text, out RomanNumeral result)
    {
        if (text.IsEmpty)
            result = Empty;
        else if (!TryParse(text, 0, text.Length, out result, out int nextIndex) || nextIndex < text.Length)
        {
            result = Empty;
            return false;
        }
        return true;
    }
    
    int INumericalToken.CompareAbs(BigInteger other) => (other.CompareTo(ROMAN_NUMERAL_MAX_VALUE) > 0) ? 1: GetValue().CompareTo((ushort)other);

    int INumericalToken.CompareAbs(ulong other) => (other > ROMAN_NUMERAL_MAX_VALUE) ? 1 : GetValue().CompareTo((ushort)other);

    int INumericalToken.CompareAbs(uint other) => (other > ROMAN_NUMERAL_MAX_VALUE) ? 1 : GetValue().CompareTo((ushort)other);

    int INumericalToken.CompareAbs(ushort other) => (other > ROMAN_NUMERAL_MAX_VALUE) ? 1 : GetValue().CompareTo((ushort)other);

    int INumericalToken.CompareAbs(byte other) => GetValue().CompareTo(other);

    bool INumericalToken.EqualsAbs(BigInteger other) => other.CompareTo(ROMAN_NUMERAL_MAX_VALUE) <= 0 && GetValue().Equals((ushort)other);

    bool INumericalToken.EqualsAbs(ulong other) => other <= ROMAN_NUMERAL_MAX_VALUE && GetValue().Equals((ushort)other);

    bool INumericalToken.EqualsAbs(uint other) => other <= ROMAN_NUMERAL_MAX_VALUE && GetValue().Equals((ushort)other);

    bool INumericalToken.EqualsAbs(ushort other) => other <= ROMAN_NUMERAL_MAX_VALUE && GetValue().Equals((ushort)other);

    bool INumericalToken.EqualsAbs(byte other) => GetValue().Equals(other);

}
