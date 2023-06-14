using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using static CdnGetter.Versioning.VersioningConstants;

namespace CdnGetter.Versioning;

public struct RomanNumeralToken : INumericalToken
{
    public const ushort MAX_VALUE = 3999;

    public static readonly RomanNumeralToken MaxValue = new(3999);

    /// <summary>
    /// Gets the leading delimiter characters.
    /// </summary>
    /// <value>The leading delimiter characters or <see langword="null" /> if there aren't any.</value>
    public ITokenCharacters? Delimiter { get; }

    bool INumericalToken.HasNegativeSign => false;

    int INumericalToken.ZeroPadCount => 0;

    public string Value { get; }

    /// <summary>
    /// Gets the postfixed non-numerical characters.
    /// </summary>
    /// <value>The postfixed non-numerical characters or <see langword="null" /> if there aren't any.</value>
    public string? NonNumerical { get; }

    public RomanNumeralToken(ITokenCharacters delimiter, ushort value, string? nonNumerical = null)
    {
        Delimiter = AssertValidRomanNumericDelimiter(delimiter);
        Value = ToRomanNumeral(value);
        NonNumerical = AssertValidRomanNumeralPostfix(nonNumerical);
    }

    public RomanNumeralToken(ushort value, string? nonNumerical = null)
    {
        if (value > MAX_VALUE)
            throw new ArgumentOutOfRangeException(nameof(value));
        Delimiter = null;
        Value = ToRomanNumeral(value);
        NonNumerical = AssertValidRomanNumeralPostfix(nonNumerical);
    }

    private ushort GetNumericalValue()
    {
        int endIndex = 0;
            
        static bool is1000(char c) => c == ROMAN_NUM_1000_UC || c == ROMAN_NUM_1000_LC;

        ushort result = 0;
        
        #region Parse thousands

        while (is1000(Value[endIndex]))
        {
            endIndex++;
            result += 1000;
            if (endIndex == Value.Length)
                return result;
        }

        #endregion

        static bool is500(char c) => c == ROMAN_NUM_500_UC || c == ROMAN_NUM_500_LC;
        static bool is100(char c) => c == ROMAN_NUM_100_UC || c == ROMAN_NUM_100_LC;

        #region Parse hundreds

        if (is500(Value[endIndex]))
        {
            result += 500;
            if (++endIndex == Value.Length)
                return result;
            while (is100(Value[endIndex]))
            {
                endIndex++;
                result += 100;
                if (endIndex == Value.Length)
                    return result;
            }
        }
        else if (is100(Value[endIndex]))
        {
            result += 100;
            if (++endIndex == Value.Length)
                return result;
            if (is1000(Value[endIndex]))
            {
                result += 900;
                if (++endIndex == Value.Length)
                    return result;
            }
            else if (is500(Value[endIndex]))
            {
                result += 400;
                if (++endIndex == Value.Length)
                    return result;
            }
            else
                while (is100(Value[endIndex]))
                {
                    result += 100;
                    if (++endIndex == Value.Length)
                        return result;
                }
        }

        #endregion

        static bool is50(char c) => c == ROMAN_NUM_50_UC || c == ROMAN_NUM_50_LC;
        static bool is10(char c) => c == ROMAN_NUM_10_UC || c == ROMAN_NUM_10_LC;

        #region Parse tens

        if (is50(Value[endIndex]))
        {
            result += 50;
            if (++endIndex == Value.Length)
                return result;
            while (is10(Value[endIndex]))
            {
                result += 10;
                if (++endIndex == Value.Length)
                    return result;
            }
        }
        else if (is10(Value[endIndex]))
        {
            result += 10;
            if (++endIndex == Value.Length)
                return result;
            if (is100(Value[endIndex]))
            {
                result += 90;
                if (++endIndex == Value.Length)
                    return result;
            }
            else if (is50(Value[endIndex]))
            {
                result += 40;
                if (++endIndex == Value.Length)
                    return result;
            }
            else
                while (is10(Value[endIndex]))
                {
                    result += 10;
                    if (++endIndex == Value.Length)
                        return result;
                }
        }

        #endregion

        static bool is5(char c) => c == ROMAN_NUM_5_UC || c == ROMAN_NUM_5_LC;
        static bool is1(char c) => c == ROMAN_NUM_1_UC || c == ROMAN_NUM_1_LC;

        #region Parse ones

        if (is5(Value[endIndex]))
        {
            result += 5;
            if (++endIndex == Value.Length)
                return result;
            while (is1(Value[endIndex]))
            {
                result++;
                if (++endIndex == Value.Length)
                    return result;
            }
        }
        else if (is1(Value[endIndex]))
        {
            if (++endIndex == Value.Length)
                return (ushort)(result + 1);
            if (is10(Value[endIndex]))
                result += 9;
            else if (is5(Value[endIndex]))
                result += 4;
            else
            {
                result += 1;
                while (is1(Value[endIndex]))
                {
                    result++;
                    if (++endIndex == Value.Length)
                        break;
                }
            }
        }

        #endregion

        return result;
    }

    BigInteger INumericalToken.AsUnsignedBigInteger() => new(GetNumericalValue());

    public int CompareTo(IToken? other)
    {
        throw new NotImplementedException();
    }

    public bool Equals(IToken? other)
    {
        throw new NotImplementedException();
    }

    ITokenCharacters? IToken.GetDelimiter() => Delimiter;

    string? IToken.GetPostFixed() => NonNumerical;

    string IToken.GetValue() => Value;

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

    bool IToken.TryGetDelimiter([NotNullWhen(true)] out string? delimiter)
    {
        throw new NotImplementedException();
    }

    bool INumericalToken.TryGetNonNumerical([NotNullWhen(true)] out string? nonNumerical)
    {
        throw new NotImplementedException();
    }

    bool IToken.TryGetPostFixed([NotNullWhen(true)] out string? postfixed)
    {
        throw new NotImplementedException();
    }
}
