using System.Numerics;
using static CdnGetter.Parsing.Parsing;

namespace CdnGetter.Parsing;

/// <summary>
/// Represents a numerical token that has an arbitrary number of digits.
/// </summary>
#pragma warning disable CA2231
public readonly struct DigitsNBit : INumericalToken
#pragma warning restore CA2231
{
    public static readonly DigitsNBit Zero = new(0);

    /// <summary>
    /// Gets the absolute value of this token.
    /// </summary>
    public BigInteger Value { get; }

    /// <summary>
    /// Gets a value that indicates whether the numerical value of the token is prefixed by a negative sign.
    /// </summary>
    public bool HasNegativeSign { get; }

    /// <summary>
    /// Gets the number of leading zeros for the numerical value.
    /// </summary>
    public int ZeroPadLength  { get; }

    bool INumericalToken.IsZero => Value.Equals(0);

    /// <summary>
    /// Creates a new <c>DigitsNBit</c> token.
    /// </summary>
    public DigitsNBit()
    {
        ZeroPadLength = 0;
        HasNegativeSign = false;
        Value = new(0);
    }

    /// <summary>
    /// Creates a new <c>DigitsNBit</c> token.
    /// </summary>
    /// <param name="value">The value of the token.</param>
    /// <param name="hasNegativeSign">Indicates whether the numeric value is preceded by a negative sign.</param>
    /// <param name="leadingZeroCount">The number of leading <c>'0'</c> characters.</param>
    public DigitsNBit(BigInteger value, bool hasNegativeSign, int leadingZeroCount = 0)
    {
        ZeroPadLength = (leadingZeroCount < 0) ? 0 : leadingZeroCount;
        HasNegativeSign = hasNegativeSign;
        Value = (value.Sign < 0) ? BigInteger.Negate(value) : value;
    }

    /// <summary>
    /// Creates a new <c>DigitsNBit</c> token.
    /// </summary>
    /// <param name="value">The value of the token.</param>
    /// <param name="leadingZeroCount">The number of leading <c>'0'</c> characters.</param>
    public DigitsNBit(BigInteger value, int leadingZeroCount = 0)
    {
        ZeroPadLength = (leadingZeroCount < 0) ? 0 : leadingZeroCount;
        HasNegativeSign = value.Sign < 0;
        Value = HasNegativeSign ? BigInteger.Negate(value) : value;
    }

    public static bool TryParse(ParsingSource source, int startIndex, int count, out DigitsNBit result, out int nextIndex)
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
        count = 0;
        do
        {
            count++;
            if (!char.IsNumber(source[nextIndex++]))
                break;
        }
        while (nextIndex < endIndex);
        result = new(BigInteger.Parse(source.ToString(firstNz, count)), isNegative, firstNz - (isNegative ? startIndex + 1 : startIndex));
        return true;
    }

    public static DigitsNBit Parse(string text)
    {
        if (!string.IsNullOrEmpty(text) && TryParse(new ParsingSource(text), 0, text.Length, out DigitsNBit result, out int nextIndex))
        {
            if (nextIndex == text.Length)
                return result;
        }
        else
            nextIndex = 0;
        throw new ArgumentException($"Invalid big integer sequence at index {nextIndex}.", nameof(text));
    }

    public static DigitsNBit Parse(ReadOnlySpan<char> text)
    {
        if (!text.IsEmpty && TryParse(new ParsingSource(new string(text)), 0, text.Length, out DigitsNBit result, out int nextIndex))
        {
            if (nextIndex == text.Length)
                return result;
        }
        else
            nextIndex = 0;
        throw new ArgumentException($"Invalid big integer sequence at index {nextIndex}.", nameof(text));
    }

    public static bool TryParse(string text, out DigitsNBit result)
    {
        if (string.IsNullOrEmpty(text) || !TryParse(new ParsingSource(text), 0, text.Length, out result, out int nextIndex) || nextIndex < text.Length)
        {
            result = Zero;
            return false;
        }
        return true;
    }

    public static bool TryParse(ReadOnlySpan<char> text, out DigitsNBit result)
    {
        if (text.IsEmpty || !TryParse(new ParsingSource(new string(text)), 0, text.Length, out result, out int nextIndex) || nextIndex < text.Length)
        {
            result = Zero;
            return false;
        }
        return true;
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
        return VersionComponentComparer.CompareTo(GetValue(), other.GetValue());
    }

    public bool Equals(IToken? other)
    {
        if (other is null)
            return false;
        if (other is INumericalToken numericalToken)
            return Value.Equals(0) ? numericalToken.IsZero : !numericalToken.IsZero && HasNegativeSign == numericalToken.HasNegativeSign && numericalToken.EqualsAbs(Value);
        return VersionComponentComparer.AreEqual(GetValue(), other.GetValue());
    }

    public override bool Equals(object? obj) => obj is IToken other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(HasNegativeSign && !Value.Equals(0), Value);

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
    public string GetValue() => (HasNegativeSign && !Value.Equals(0)) ? $"-{Value}" : Value.ToString();

    /// <summary>
    /// Gets the string value of the current token, including padded zero values.
    /// </summary>
    public override string ToString() => HasNegativeSign ? ((ZeroPadLength > 0) ? $"-{new string('0', ZeroPadLength)}{Value}" : $"-{Value}") :
        (ZeroPadLength > 0) ? $"{new string('0', ZeroPadLength)}{Value}" : Value.ToString();
    
    /// <summary>
    /// Attempts to return the absolute value of the current token as a <see cref="byte" /> value.
    /// </summary>
    /// <param name="value">The byte value of the absolute value of the current token or <c>0</c> if the absolute value is greater than <see cref="byte.MaxValue /></param>
    /// <returns><see langword="true" /> if  the absolute numerical value of the current token is less than or equal to <see cref="byte.MaxValue />; otherwise, <see langword="false" />.</returns>
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

    /// <summary>
    /// Attempts to return the absolute value of the current token as a <see cref="ushort" /> value.
    /// </summary>
    /// <param name="value">The byte value of the absolute value of the current token or <c>0</c> if the absolute value is greater than <see cref="ushort.MaxValue /></param>
    /// <returns><see langword="true" /> if  the absolute numerical value of the current token is less than or equal to <see cref="ushort.MaxValue />; otherwise, <see langword="false" />.</returns>
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

    /// <summary>
    /// Attempts to return the absolute value of the current token as a <see cref="uint" /> value.
    /// </summary>
    /// <param name="value">The byte value of the absolute value of the current token or <c>0</c> if the absolute value is greater than <see cref="uint.MaxValue /></param>
    /// <returns><see langword="true" /> if  the absolute numerical value of the current token is less than or equal to <see cref="uint.MaxValue />; otherwise, <see langword="false" />.</returns>
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

    /// <summary>
    /// Attempts to return the absolute value of the current token as a <see cref="ulong" /> value.
    /// </summary>
    /// <param name="value">The byte value of the absolute value of the current token or <c>0</c> if the absolute value is greater than <see cref="ulong.MaxValue /></param>
    /// <returns><see langword="true" /> if  the absolute numerical value of the current token is less than or equal to <see cref="ulong.MaxValue />; otherwise, <see langword="false" />.</returns>
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
