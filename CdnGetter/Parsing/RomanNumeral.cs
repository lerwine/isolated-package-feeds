using System.Numerics;

namespace CdnGetter.Parsing;

/// <summary>
/// Represents a numerical token derrived from roman numerals.
/// </summary>
#pragma warning disable CA2231
public readonly partial struct RomanNumeral : INumericalToken
#pragma warning restore CA2231
{
    public static readonly RomanNumeral MaxValue = new(ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE);
    
    public static readonly RomanNumeral Empty = new();
    
    /// <summary>
    /// Gets the roman numeral characters that make up this token.
    /// </summary>
    public string? Value { get; }

    bool INumericalToken.HasNegativeSign => false;

    int INumericalToken.ZeroPadLength => 0;

    /// <summary>
    /// Gets a value that indicates whether this token represents a zero value.
    /// </summary>
    public bool IsZero => Value is null;

    /// <summary>
    /// Creates a new <c>RomanNumeral</c> token.
    /// </summary>
    public RomanNumeral() => Value = null;
    
    /// <summary>
    /// Creates a new <c>RomanNumeral</c> token.
    /// </summary>
    /// <param name="value">The value of the token.</param>
    public RomanNumeral(ushort value) => Value = ToRomanNumeral(value);
    
    /// <summary>
    /// Creates a new <c>RomanNumeral</c> token.
    /// </summary>
    /// <param name="value">The string value of the token.</param>
    private RomanNumeral(string? value) => Value = value;
    
    /// <summary>
    /// Gets the absolute value of the current token as a <see cref="BigInteger" />.
    /// </summary>
    /// <returns>The absolute value of the current token as a <see cref="BigInteger" />.</returns>
    public BigInteger AsBigInteger() => new(GetValue());

    public int CompareTo(IToken? other)
    {
        if (other is null)
            return 1;
        if (other is INumericalToken numericalToken)
        {
            if (Value is null)
                return numericalToken.IsZero ? 0 : numericalToken.HasNegativeSign ? 1 : -1;
            return (numericalToken.IsZero || numericalToken.HasNegativeSign) ? 1 : (numericalToken.TryGet16Bit(out ushort s) && s <= ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE) ? GetValue().CompareTo(s) : -1;
        }
        return ParsingExtensionMethods.NoCaseComparer.Compare(GetValue(), other.GetValue());
    }

    public bool Equals(IToken? other)
    {
        if (other is null)
            return false;
        if (other is INumericalToken numericalToken)
            return (Value is null) ? numericalToken.IsZero : !numericalToken.IsZero && !numericalToken.HasNegativeSign && numericalToken.TryGet16Bit(out ushort value) &&
                value <= ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE && GetValue() == value;
        return ParsingExtensionMethods.NoCaseComparer.Equals(GetValue(), other.GetValue());
    }

    public override bool Equals(object? obj) => obj is IToken other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(false, (ulong)GetValue());

    /// <summary>
    /// Gets the length of the current token.
    /// </summary>
    /// <returns>The token length.</returns>
    public int GetLength() => Value?.Length ?? 0;

    int IToken.GetLength(bool allChars) => GetLength();

    /// <summary>
    /// Gets the numerical value of the current token.
    /// </summary>
    /// <returns>The numerical value of the current token.</returns>
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

    /// <summary>
    /// Attempts to return the absolute value of the current token as a <see cref="byte" /> value.
    /// </summary>
    /// <param name="value">The byte value of the absolute value of the current token or <c>0</c> if the absolute value is greater than <see cref="byte.MaxValue /></param>
    /// <returns><see langword="true" /> if  the absolute numerical value of the current token is less than or equal to <see cref="byte.MaxValue />; otherwise, <see langword="false" />.</returns>
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
    
    int INumericalToken.CompareAbs(BigInteger other) => (other.CompareTo(ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE) > 0) ? 1: GetValue().CompareTo((ushort)other);

    int INumericalToken.CompareAbs(ulong other) => (other > ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE) ? 1 : GetValue().CompareTo((ushort)other);

    int INumericalToken.CompareAbs(uint other) => (other > ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE) ? 1 : GetValue().CompareTo((ushort)other);

    int INumericalToken.CompareAbs(ushort other) => (other > ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE) ? 1 : GetValue().CompareTo((ushort)other);

    int INumericalToken.CompareAbs(byte other) => GetValue().CompareTo(other);

    bool INumericalToken.EqualsAbs(BigInteger other) => other.CompareTo(ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE) <= 0 && GetValue().Equals((ushort)other);

    bool INumericalToken.EqualsAbs(ulong other) => other <= ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE && GetValue().Equals((ushort)other);

    bool INumericalToken.EqualsAbs(uint other) => other <= ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE && GetValue().Equals((ushort)other);

    bool INumericalToken.EqualsAbs(ushort other) => other <= ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE && GetValue().Equals((ushort)other);

    bool INumericalToken.EqualsAbs(byte other) => GetValue().Equals(other);

    public IEnumerable<char> GetSourceValues() => Value ?? "";
}
