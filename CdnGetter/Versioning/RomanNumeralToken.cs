using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using static CdnGetter.Versioning.VersioningConstants;

namespace CdnGetter.Versioning;

#pragma warning disable CA2231
public readonly struct RomanNumeralToken : INumericalToken
#pragma warning restore CA2231
{
    internal const int MAX_STRING_LENGTH = 15;

    public const ushort MAX_VALUE = 3999;

    public static readonly RomanNumeralToken MaxValue = new(3999);

    /// <summary>
    /// Gets the leading delimiter characters.
    /// </summary>
    /// <value>The leading delimiter characters or <see langword="null" /> if there aren't any.</value>
    public ITokenCharacters? Delimiter { get; }

    bool INumericalToken.HasNegativeSign => false;

    int INumericalToken.ZeroPadCount => 0;

    /// <summary>
    /// Gets the roman numeral characters for this token.
    /// </summary>
    /// <value>The roman numeral characters or <see cref="string.Empty" /> if this represents a zero value.</value>
    public string Value { get; }

    public bool IsZero => Value.Length == 0;

    string? INumericalToken.NonNumerical => null;

    public RomanNumeralToken()
    {
        Delimiter = null;
        Value = string.Empty;
    }

    public RomanNumeralToken(ITokenCharacters delimiter, ushort value)
    {
        Delimiter = AssertValidRomanNumericDelimiter(delimiter);
        Value = ToRomanNumeral(value);
    }

    public RomanNumeralToken(ushort value)
    {
        if (value > MAX_VALUE)
            throw new ArgumentOutOfRangeException(nameof(value));
        Delimiter = null;
        Value = ToRomanNumeral(value);
    }

    public RomanNumeralToken(ITokenCharacters delimiter, ReadOnlySpan<char> value, int startIndex, int endIndex)
    {
        if (value.Length == 0)
            Value = string.Empty;
        else
        {
            if (!TestRomanNumeral(value, startIndex, endIndex, out endIndex) || endIndex < value.Length)
                throw new ArgumentException("Input string is not a valid roman numeral", nameof(value));
            Value = new string(value);
        }
        Delimiter = AssertValidRomanNumericDelimiter(delimiter);
    }

    public RomanNumeralToken(ITokenCharacters delimiter, ReadOnlySpan<char> value, int startIndex)
    {
        if (value.Length == 0)
            Value = string.Empty;
        else
        {
            if (!TestRomanNumeral(value, startIndex, out int endIndex) || endIndex < value.Length)
                throw new ArgumentException("Input string is not a valid roman numeral", nameof(value));
            Value = new string(value);
        }
        Delimiter = AssertValidRomanNumericDelimiter(delimiter);
    }

    public RomanNumeralToken(ITokenCharacters delimiter, ReadOnlySpan<char> value)
    {
        if (value.Length == 0)
            Value = string.Empty;
        else
        {
            if (!TestRomanNumeral(value, out int endIndex) || endIndex < value.Length)
                throw new ArgumentException("Input string is not a valid roman numeral", nameof(value));
            Value = new string(value);
        }
        Delimiter = AssertValidRomanNumericDelimiter(delimiter);
    }

    public RomanNumeralToken(ITokenCharacters delimiter, string value)
    {
        if (string.IsNullOrEmpty(value))
            Value = string.Empty;
        else
        {
            if (!TestRomanNumeral(value.AsSpan(), out int endIndex) || endIndex < value.Length)
                throw new ArgumentException("Input string is not a valid roman numeral", nameof(value));
            Value = value;
        }
        Delimiter = AssertValidRomanNumericDelimiter(delimiter);
    }

    public RomanNumeralToken(ReadOnlySpan<char> value, int startIndex, int endIndex)
    {
        if (value.Length == 0)
            Value = string.Empty;
        else
        {
            if (!TestRomanNumeral(value, startIndex, endIndex, out endIndex) || endIndex < value.Length)
                throw new ArgumentException("Input string is not a valid roman numeral", nameof(value));
            Value = new string(value);
        }
        Delimiter = null;
    }

    public RomanNumeralToken(ReadOnlySpan<char> value, int startIndex)
    {
        if (value.Length == 0)
            Value = string.Empty;
        else
        {
            if (!TestRomanNumeral(value, startIndex, out int endIndex) || endIndex < value.Length)
                throw new ArgumentException("Input string is not a valid roman numeral", nameof(value));
            Value = new string(value);
        }
        Delimiter = null;
    }

    public RomanNumeralToken(ReadOnlySpan<char> value)
    {
        if (value.Length == 0)
            Value = string.Empty;
        else
        {
            if (!TestRomanNumeral(value, out int endIndex) || endIndex < value.Length)
                throw new ArgumentException("Input string is not a valid roman numeral", nameof(value));
            Value = new string(value);
        }
        Delimiter = null;
    }

    public RomanNumeralToken(string value)
    {
        if (!TestRomanNumeral(value.AsSpan(), out int endIndex) || endIndex < value.Length)
            throw new ArgumentException("Input string is not a valid roman numeral", nameof(value));
        Delimiter = null;
        Value = value;
    }

    public ushort GetNumericalValue()
    {
        if (Value.Length == 0)
            return 0;
        static bool is1000(char c) => c == ROMAN_NUM_1000_UC || c == ROMAN_NUM_1000_LC;
        ushort result;
        int endIndex;
        if (is1000(Value[0]))
        {
            if (Value.Length == 1)
                return ROMAN_NUM_M; 
            result = ROMAN_NUM_M;
            if (is1000(Value[1]))
            {
                result += ROMAN_NUM_M;
                if (Value.Length == 2)
                    return result; 
                if (is1000(Value[2]))
                {
                    result += ROMAN_NUM_M;
                    if (Value.Length == 3)
                        return result; 
                    endIndex = 3;
                }
                else
                    endIndex = 2;
            }
            else
                endIndex = 1;
        }
        else
        {
            endIndex = 0;
            result = 0;
        }
        
        static bool is500(char c) => c == ROMAN_NUM_500_UC || c == ROMAN_NUM_500_LC;
        static bool is100(char c) => c == ROMAN_NUM_100_UC || c == ROMAN_NUM_100_LC;

        if (is500(Value[endIndex]))
        {
            result += ROMAN_NUM_D;
            if (++endIndex == Value.Length)
                return result; 
            for (int i = 0; i < 3 && is100(Value[endIndex]); i++)
            {
                result += ROMAN_NUM_C;
                if (++endIndex == Value.Length)
                    return result;
            }
        }
        else if (is100(Value[endIndex]))
        {
            result += ROMAN_NUM_C;
            if (++endIndex == Value.Length)
                return result;
            if (is500(Value[endIndex]))
            {
                result += ROMAN_NUM_CCC;
                if (++endIndex == Value.Length)
                    return result;
            }
            else if (is1000(Value[endIndex]))
            {
                result += ROMAN_NUM_DCCC;
                if (++endIndex == Value.Length)
                    return result;
            }
            else if (is100(Value[endIndex]))
            {
                result += ROMAN_NUM_C;
                if (++endIndex == Value.Length)
                    return result;
                if (is100(Value[endIndex]))
                {
                    result += ROMAN_NUM_C;
                    if (++endIndex == Value.Length)
                        return result;
                }
            }
        }

        static bool is50(char c) => c == ROMAN_NUM_50_UC || c == ROMAN_NUM_50_LC;
        static bool is10(char c) => c == ROMAN_NUM_10_UC || c == ROMAN_NUM_10_LC;
        
        if (is50(Value[endIndex]))
        {
            result += ROMAN_NUM_L;
            if (++endIndex == Value.Length)
                return result; 
            for (int i = 0; i < 3 && is10(Value[endIndex]); i++)
            {
                result += ROMAN_NUM_X;
                if (++endIndex == Value.Length)
                    return result;
            }
        }
        else if (is10(Value[endIndex]))
        {
            result += ROMAN_NUM_X;
            if (++endIndex == Value.Length)
                return result;
            if (is50(Value[endIndex]))
            {
                result += ROMAN_NUM_XXX;
                if (++endIndex == Value.Length)
                    return result;
            }
            else if (is100(Value[endIndex]))
            {
                result += ROMAN_NUM_LXXX;
                if (++endIndex == Value.Length)
                    return result;
            }
            else if (is10(Value[endIndex]))
            {
                result += ROMAN_NUM_X;
                if (++endIndex == Value.Length)
                    return result;
                if (is10(Value[endIndex]))
                {
                    result += ROMAN_NUM_X;
                    if (++endIndex == Value.Length)
                        return result;
                }
            }
        }

        static bool is5(char c) => c == ROMAN_NUM_5_UC || c == ROMAN_NUM_5_LC;
        static bool is1(char c) => c == ROMAN_NUM_1_UC || c == ROMAN_NUM_1_LC;
        
        if (is5(Value[endIndex]))
        {
            result += ROMAN_NUM_V;
            if (++endIndex == Value.Length)
                return result; 
            for (int i = 0; i < 3 && is1(Value[endIndex]); i++)
            {
                result++;
                if (++endIndex == Value.Length)
                    return result;
            }
        }
        else if (is1(Value[endIndex]))
        {
            result++;
            if (++endIndex == Value.Length)
                return result;
            if (is5(Value[endIndex]))
                result += ROMAN_NUM_III;
            else if (is10(Value[endIndex]))
                result += ROMAN_NUM_VIII;
            else if (is1(Value[endIndex]))
            {
                result++;
                if (++endIndex == Value.Length)
                    return result;
                if (is1(Value[endIndex]))
                    result++;
            }
        }
        return result;
    }

    BigInteger INumericalToken.AsUnsignedBigInteger() => new(GetNumericalValue());

    public int CompareTo(IToken? other)
    {
        if (other is null)
            return 1;
        if (other is INumericalToken n)
        {
            if (Value.Length > 0)
            {
                if (n.IsZero || n.HasNegativeSign)
                    return 1;
                int result;
                if (other is RomanNumeralToken rn)
                    result = NoCaseComparer.Compare(Value, rn.Value);
                else if (other is Numerical8BitToken t8b)
                    result = GetNumericalValue().CompareTo(t8b.Value);
                else if (other is Numerical16BitToken t16)
                    result = (t16.Value > MAX_VALUE) ? -1 : GetNumericalValue().CompareTo(t16.Value);
                else if (other is Numerical32BitToken t32)
                    result = (t32.Value > MAX_VALUE) ? -1 : GetNumericalValue().CompareTo((ushort)t32.Value);
                else if (other is Numerical64BitToken t64)
                    result = (t64.Value > MAX_VALUE) ? -1 : GetNumericalValue().CompareTo((ushort)t64.Value);
                else
                {
                    BigInteger b = n.AsUnsignedBigInteger();
                    result = (b > BigNumericalToken.MAX_ROMAN_NUM) ? -1 : new BigInteger(GetNumericalValue()).CompareTo(b);
                }
                if (result != 0)
                    return result;
            }
            else if (!n.IsZero)
                return n.HasNegativeSign ? 1 : -1;
        }
        return (other.GetPostFixed() is null) ? 0 : -1;
    }

    public bool Equals(IToken? other)
    {
        if (other is null)
            return false;
        if (other is INumericalToken n)
        {
            if (Value.Length > 0)
            {
                if (n.IsZero || n.HasNegativeSign)
                    return false;
                if (other is RomanNumeralToken rn)
                {
                    if (!NoCaseComparer.Equals(Value, rn.Value))
                        return false;
                }
                else if (other is Numerical8BitToken t8b)
                {
                    if (!GetNumericalValue().Equals(t8b.Value))
                        return false;
                }
                else if (other is Numerical16BitToken t16)
                {
                    if (t16.Value > MAX_VALUE || !GetNumericalValue().Equals(t16.Value))
                        return false;
                }
                else if (other is Numerical32BitToken t32)
                {
                    if (t32.Value > MAX_VALUE || !GetNumericalValue().Equals((ushort)t32.Value))
                        return false;
                }
                else if (other is Numerical64BitToken t64)
                {
                    if (t64.Value > MAX_VALUE || !GetNumericalValue().Equals((ushort)t64.Value))
                        return false;
                }
                else
                {
                    BigInteger b = n.AsUnsignedBigInteger();
                    if (b > BigNumericalToken.MAX_ROMAN_NUM || !new BigInteger(GetNumericalValue()).Equals(b))
                        return false;
                }
            }
        }
        else if (!NoCaseComparer.Equals(Value, other.GetValue()))
            return false;
        return other.GetPostFixed() is null;
    }

    ITokenCharacters? IToken.GetDelimiter() => Delimiter;

    string? IToken.GetPostFixed() => null;

    string IToken.GetValue() => Value;

    bool INumericalToken.TryGet8Bit(out byte value)
    {
        ushort s = GetNumericalValue();
        if (s > byte.MaxValue)
        {
            value = default;
            return false;
        }
        value = (byte)s;
        return true;
    }

    bool INumericalToken.TryGet16Bit(out ushort value)
    {
        value = GetNumericalValue();
        return true;
    }

    bool INumericalToken.TryGet32Bit(out uint value)
    {
        value = GetNumericalValue();
        return true;
    }

    bool INumericalToken.TryGet64Bit(out ulong value)
    {
        value = GetNumericalValue();
        return true;
    }

    public bool TryGetDelimiter([NotNullWhen(true)] out string? delimiter)
    {
        if (Delimiter is null)
        {
            delimiter = null;
            return false;
        }
        delimiter = Delimiter.ToString();
        return true;
    }

    bool IToken.TryGetPostFixed([NotNullWhen(true)] out string? postfixed)
    {
        postfixed = null;
        return false;
    }

    bool INumericalToken.TryGetNonNumerical([NotNullWhen(true)] out string? nonNumerical)
    {
        nonNumerical = null;
        return false;
    }
}
