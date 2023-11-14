using System.Numerics;
using System.Runtime.InteropServices;
using static CdnGetter.Parsing.Parsing;

namespace CdnGetter.Parsing;

/// <summary>
/// Represents a numerical token that has an 8-bit absolute value.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
#pragma warning disable CA2231
public readonly struct Digits8Bit : INumericalToken
#pragma warning restore CA2231
{
    public static readonly Digits8Bit Zero = new(0);
    
    public static readonly Digits8Bit MaxValue = new(byte.MaxValue);
    
    /// <summary>
    /// Gets the maximum length of the absolute-value string representation.
    /// </summary>

    public static readonly int MAX_ABS_LENGTH = byte.MaxValue.ToString().Length;

    /// <summary>
    /// Gets the absolute value of this token.
    /// </summary>
    public byte Value { get; }

    /// <summary>
    /// Gets a value that indicates whether the numerical value of the token is prefixed by a negative sign.
    /// </summary>
    public bool HasNegativeSign { get; }

    /// <summary>
    /// Gets the number of leading zeros for the numerical value.
    /// </summary>
    public int ZeroPadLength { get; }

    bool INumericalToken.IsZero => Value == 0;


    /// <summary>
    /// Creates a new <c>Digits8Bit</c> token.
    /// </summary>
    public Digits8Bit()
    {
        ZeroPadLength = 0;
        HasNegativeSign = false;
        Value = 0;
    }
    
    /// <summary>
    /// Creates a new <c>Digits8Bit</c> token.
    /// </summary>
    /// <param name="value">The value of the token.</param>
    /// <param name="leadingZeroCount">The number of leading <c>'0'</c> characters.</param>
    public Digits8Bit(sbyte value, int leadingZeroCount = 0)
    {
        HasNegativeSign = value < 0;
        Value = (byte)(HasNegativeSign ? value * -1 : value);
        ZeroPadLength = (leadingZeroCount < 0) ? 0 : leadingZeroCount;
    }

    /// <summary>
    /// Creates a new <c>Digits8Bit</c> token.
    /// </summary>
    /// <param name="value">The absolute value of the token.</param>
    /// <param name="hasNegativeSign">Indicates whether the numeric value is preceded by a negative sign.</param>
    /// <param name="leadingZeroCount">The number of leading <c>'0'</c> characters.</param>
    public Digits8Bit(byte value, bool hasNegativeSign = false, int leadingZeroCount = 0)
    {
        HasNegativeSign = hasNegativeSign;
        Value = value;
        ZeroPadLength = (leadingZeroCount < 0) ? 0 : leadingZeroCount;
    }

    /// <summary>
    /// Creates a new <c>Digits8Bit</c> token.
    /// </summary>
    /// <param name="value">The value of the token.</param>
    /// <param name="leadingZeroCount">The number of leading <c>'0'</c> characters.</param>
    public Digits8Bit(byte value, int leadingZeroCount)
    {
        HasNegativeSign = false;
        Value = value;
        ZeroPadLength = (leadingZeroCount < 0) ? 0 : leadingZeroCount;
    }

    public static bool TryParse(ParsingSource source, int startIndex, int count, out Digits8Bit result, out int nextIndex)
    {
        if (source.ValidateSourceIsEmpty(ref startIndex, ref count))
        {
            nextIndex = startIndex;
            result = Zero;
            return false;
        }
        nextIndex = startIndex;
        char c = source[nextIndex++];
        bool isNegative = c == DELIMITER_DASH;
        int firstNz;
        if (isNegative)
        {
            if (count == 1)
            {
                nextIndex = startIndex;
                result = Zero;
                return false;
            }
            firstNz = startIndex + 1;
            c = source[nextIndex++];
        }
        else
            firstNz = startIndex;
        int endIndex = startIndex + count;
        if (c == '0')
        {
            do
            {
                firstNz++;
                if (nextIndex == endIndex)
                    break;
                c = source[nextIndex++];
            }
            while (c == '0');
            if (!char.IsNumber(c) || nextIndex == endIndex)
            {
                result = new(0, isNegative, nextIndex - (startIndex + (isNegative ? 2 : 1)));
                return true;
            }
        }
        else if (!char.IsNumber(c))
        {
            nextIndex = startIndex;
            result = Zero;
            return false;
        }
        
        count = 1;
        while (char.IsNumber(source[nextIndex++]))
        {
            count++;
            if (count == MAX_ABS_LENGTH)
            {
                if ((nextIndex < endIndex && char.IsNumber(source[nextIndex])) || !byte.TryParse(source.ToString(nextIndex - 3, 3), out byte value))
                {
                    nextIndex = startIndex;
                    result = Zero;
                    return false;
                }
                result = new(value, isNegative, firstNz - (isNegative ? startIndex + 1 : startIndex));
                return true;
            }
            if (nextIndex == endIndex)
                break;
        }
        result = new(byte.Parse(source.ToString(firstNz, count)), isNegative, firstNz - (isNegative ? startIndex + 1 : startIndex));
        return true;
    }

    public static Digits8Bit Parse(string text)
    {
        if (!string.IsNullOrEmpty(text) && TryParse(new ParsingSource(text), 0, text.Length, out Digits8Bit result, out int nextIndex))
        {
            if (nextIndex == text.Length)
                return result;
        }
        else
            nextIndex = 0;
        throw new ArgumentException($"Invalid tiny integer sequence at index {nextIndex}.", nameof(text));
    }

    public static Digits8Bit Parse(ReadOnlySpan<char> text)
    {
        if (!text.IsEmpty && TryParse(new ParsingSource(new string(text)), 0, text.Length, out Digits8Bit result, out int nextIndex))
        {
            if (nextIndex == text.Length)
                return result;
        }
        else
            nextIndex = 0;
        throw new ArgumentException($"Invalid tiny integer sequence at index {nextIndex}.", nameof(text));
    }

    public static bool TryParse(string text, out Digits8Bit result)
    {
        if (string.IsNullOrEmpty(text) || !TryParse(new ParsingSource(text), 0, text.Length, out result, out int nextIndex) || nextIndex < text.Length)
        {
            result = Zero;
            return false;
        }
        return true;
    }

    public static bool TryParse(ReadOnlySpan<char> text, out Digits8Bit result)
    {
        if (text.IsEmpty || !TryParse(new ParsingSource(new string(text)), 0, text.Length, out result, out int nextIndex) || nextIndex < text.Length)
        {
            result = Zero;
            return false;
        }
        return true;
    }

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
            if (Value.Equals(0))
                return numericalToken.IsZero ? 0 : numericalToken.HasNegativeSign ? 1 : -1;
            if (numericalToken.IsZero)
                return HasNegativeSign ? -1 : 1;
            if (numericalToken.HasNegativeSign)
                return HasNegativeSign ? numericalToken.CompareAbs(Value) : 1;
            return HasNegativeSign ? -1 : 0 - numericalToken.CompareAbs(Value);
        }
        return VersionComponentComparer.CompareTo(GetValue(), other.GetValue());
    }

    public bool Equals(IToken? other)
    {
        if (other is null)
            return false;
        if (other is INumericalToken numericalToken)
            return (Value == 0) ? numericalToken.IsZero : !numericalToken.IsZero && HasNegativeSign == numericalToken.HasNegativeSign && numericalToken.EqualsAbs(Value);
        return VersionComponentComparer.AreEqual(GetValue(), other.GetValue());
    }

    public override bool Equals(object? obj) => obj is IToken other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(HasNegativeSign && Value != 0, (ulong)Value);

    /// <summary>
    /// Gets the length of the current token.
    /// </summary>
    /// <param name="includeZeroPad">If <see langword="true" />, then the length, including any padded zero characters, is returned;
    /// otherwise this will return the length of the current token without padded zero characters.</param>
    /// <returns>The token length.</returns>
    public int GetLength(bool includeZeroPad = false) => includeZeroPad ? (HasNegativeSign ? Value.ToString().Length + ZeroPadLength + 1 : Value.ToString().Length + ZeroPadLength) : GetValue().Length;

    /// <summary>
    /// Gets the string value of the current token, excluding padded zero values.
    /// </summary>
    public string GetValue() => (HasNegativeSign && Value != 0) ? $"-{Value}" : Value.ToString();

    /// <summary>
    /// Gets the string value of the current token, including padded zero values.
    /// </summary>
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

    public IEnumerable<char> GetSourceValues()
    {
        if (HasNegativeSign)
            yield return '-';
        for (int i = 0; i < ZeroPadLength; i++)
            yield return '0';
        foreach (char c in Value.ToString())
            yield return c;
    }
}
