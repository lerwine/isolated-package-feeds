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
    
    private readonly ParsingSource _source;

    private readonly int _startIndex;
    
    /// <summary>
    /// Gets the length of the current token.
    /// </summary>
    public int Length { get; }

    public ushort Value { get; }

    bool INumericalToken.HasNegativeSign => false;

    int INumericalToken.ZeroPadLength => 0;

    bool INumericalToken.IsZero => Value  == 0;

    /// <summary>
    /// Creates a new <c>RomanNumeral</c> token.
    /// </summary>
    public RomanNumeral() => (_source, _startIndex, Length, Value) = (ParsingSource.Empty, 0, 0, 0);
    
    /// <summary>
    /// Creates a new <c>RomanNumeral</c> token.
    /// </summary>
    /// <param name="value">The value of the token.</param>
    public RomanNumeral(ushort value)
    {
        _source = new(ToRomanNumeral(value) ?? "");
        _startIndex = 0;
        Length = _source.Count;
        Value = value;
    }
    
    private RomanNumeral(ParsingSource source, int startIndex, int length, ushort value) => (_source, _startIndex, Length, Value) = (source, startIndex, length, value);
    
    /// <summary>
    /// Gets the absolute value of the current token as a <see cref="BigInteger" />.
    /// </summary>
    /// <returns>The absolute value of the current token as a <see cref="BigInteger" />.</returns>
    public BigInteger AsBigInteger() => new(Value);

    public int CompareTo(IToken? other)
    {
        if (other is null)
            return 1;
        if (other is INumericalToken numericalToken)
        {
            if (Value == 0)
                return numericalToken.IsZero ? 0 : numericalToken.HasNegativeSign ? 1 : -1;
            return (numericalToken.IsZero || numericalToken.HasNegativeSign) ? 1 : (numericalToken.TryGet16Bit(out ushort s) && s <= ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE) ? Value.CompareTo(s) : -1;
        }
        return ParsingExtensionMethods.NoCaseComparer.Compare(Value, other.GetValue());
    }

    public bool Equals(IToken? other)
    {
        if (other is null)
            return false;
        if (other is INumericalToken numericalToken)
            return (Value == 0) ? numericalToken.IsZero : !numericalToken.IsZero && !numericalToken.HasNegativeSign && numericalToken.TryGet16Bit(out ushort value) &&
                value <= ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE && Value == value;
        return ParsingExtensionMethods.NoCaseComparer.Equals(Value, other.GetValue());
    }

    public override bool Equals(object? obj) => obj is IToken other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(false, (ulong)Value);

    int IToken.GetLength(bool allChars) => Length;

    string IToken.GetValue() => _source.ToString(_startIndex, Length);

    public override string ToString() => _source.ToString(_startIndex, Length);

    /// <summary>
    /// Attempts to return the absolute value of the current token as a <see cref="byte" /> value.
    /// </summary>
    /// <param name="value">The byte value of the absolute value of the current token or <c>0</c> if the absolute value is greater than <see cref="byte.MaxValue /></param>
    /// <returns><see langword="true" /> if  the absolute numerical value of the current token is less than or equal to <see cref="byte.MaxValue />; otherwise, <see langword="false" />.</returns>
    public bool TryGet8Bit(out byte value)
    {
        if (Value <= byte.MaxValue)
        {
            value = (byte)Value;
            return true;
        }
        value = 0;
        return false;
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

    public static RomanNumeral Parse(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Empty;
            
        if (TryParse(new ParsingSource(text), 0, text.Length, out RomanNumeral result, out int nextIndex))
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
        if (TryParse(new ParsingSource(new string(text)), 0, text.Length, out RomanNumeral result, out int nextIndex))
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
        else if (!TryParse(new ParsingSource(text), 0, text.Length, out result, out int nextIndex) || nextIndex < text.Length)
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
        else if (!TryParse(new ParsingSource(new string(text)), 0, text.Length, out result, out int nextIndex) || nextIndex < text.Length)
        {
            result = Empty;
            return false;
        }
        return true;
    }
    
    int INumericalToken.CompareAbs(BigInteger other) => (other.CompareTo(ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE) > 0) ? 1: Value.CompareTo((ushort)other);

    int INumericalToken.CompareAbs(ulong other) => (other > ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE) ? 1 : Value.CompareTo((ushort)other);

    int INumericalToken.CompareAbs(uint other) => (other > ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE) ? 1 : Value.CompareTo((ushort)other);

    int INumericalToken.CompareAbs(ushort other) => (other > ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE) ? 1 : Value.CompareTo((ushort)other);

    int INumericalToken.CompareAbs(byte other) => Value.CompareTo(other);

    bool INumericalToken.EqualsAbs(BigInteger other) => other.CompareTo(ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE) <= 0 && Value.Equals((ushort)other);

    bool INumericalToken.EqualsAbs(ulong other) => other <= ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE && Value.Equals((ushort)other);

    bool INumericalToken.EqualsAbs(uint other) => other <= ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE && Value.Equals((ushort)other);

    bool INumericalToken.EqualsAbs(ushort other) => other <= ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE && Value.Equals((ushort)other);

    bool INumericalToken.EqualsAbs(byte other) => Value.Equals(other);

    public IEnumerable<char> GetSourceValues() => _source.GetCharacters(_startIndex, Length);
}
