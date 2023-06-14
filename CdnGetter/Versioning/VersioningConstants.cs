using System.Text;
using System.Text.RegularExpressions;

namespace CdnGetter.Versioning;

public static class VersioningConstants
{
    internal static readonly StringComparer NoCaseComparer = StringComparer.InvariantCultureIgnoreCase;

    internal const char DELIMITER_DOT = '.';
    internal const char DELIMITER_DASH = '-';
    internal const char DELIMITER_SLASH = '/';
    internal const char DELIMITER_COLON = ':';
    internal const char DELIMITER_SEMICOLON = ';';
    internal const char DELIMITER_PLUS = '+';
    internal const char DELIMITER_UNDERSCORE = '_';

    public static bool IsRomanNumeralLetter(char c) => c switch
    {
        ROMAN_NUM_1000_UC or ROMAN_NUM_1000_LC or ROMAN_NUM_500_UC or ROMAN_NUM_500_LC or ROMAN_NUM_100_UC or ROMAN_NUM_100_LC or ROMAN_NUM_50_UC or ROMAN_NUM_50_LC or
            ROMAN_NUM_10_UC or ROMAN_NUM_10_LC or ROMAN_NUM_5_UC or ROMAN_NUM_5_LC or ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => true,
        _ => false
    };

    public static bool IsVersionPrefixOrBuildSeparator(char c) => c switch
    {
        DELIMITER_DOT or DELIMITER_DASH or DELIMITER_PLUS or ROMAN_NUM_500_LC or ROMAN_NUM_100_UC or ROMAN_NUM_100_LC or ROMAN_NUM_50_UC or ROMAN_NUM_50_LC or
            ROMAN_NUM_10_UC or ROMAN_NUM_10_LC or ROMAN_NUM_5_UC or ROMAN_NUM_5_LC or ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => true,
        _ => false
    };

    public static bool IsPrefixOrBuildSeparator(char c) => c switch
    {
        ROMAN_NUM_1000_UC or ROMAN_NUM_1000_LC or ROMAN_NUM_500_UC or ROMAN_NUM_500_LC or ROMAN_NUM_100_UC or ROMAN_NUM_100_LC or ROMAN_NUM_50_UC or ROMAN_NUM_50_LC or
            ROMAN_NUM_10_UC or ROMAN_NUM_10_LC or ROMAN_NUM_5_UC or ROMAN_NUM_5_LC or ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => true,
        _ => false
    };

    internal const char ROMAN_NUM_1000_UC = 'M';
    internal const char ROMAN_NUM_1000_LC = 'm';
    internal const int ROMAN_NUM_MMM = 3000;
    internal const string ROMAN_NUM_3000 = "MMM";
    internal const int ROMAN_NUM_MM = 2000;
    internal const string ROMAN_NUM_2000 = "MM";
    internal const ushort ROMAN_NUM_M = 1000;
    internal const string ROMAN_NUM_1000 = "M";

    internal const int ROMAN_NUM_CM = 900;
    internal const string ROMAN_NUM_900 = "CM";
    internal const int ROMAN_NUM_DCCC = 800;
    internal const string ROMAN_NUM_800 = "DCCC";
    internal const int ROMAN_NUM_DCC = 700;
    internal const string ROMAN_NUM_700 = "DCC";
    internal const int ROMAN_NUM_DC = 600;
    internal const string ROMAN_NUM_600 = "DC";

    internal const char ROMAN_NUM_500_UC = 'D';
    internal const char ROMAN_NUM_500_LC = 'd';
    internal const ushort ROMAN_NUM_D = 500;
    internal const string ROMAN_NUM_500 = "D";

    internal const int ROMAN_NUM_CD = 400;
    internal const string ROMAN_NUM_400 = "CD";
    internal const int ROMAN_NUM_CCC = 300;
    internal const string ROMAN_NUM_300 = "CCC";
    internal const int ROMAN_NUM_CC = 200;
    internal const string ROMAN_NUM_200 = "CC";

    internal const char ROMAN_NUM_100_UC = 'C';
    internal const char ROMAN_NUM_100_LC = 'c';
    internal const ushort ROMAN_NUM_C = 100;
    internal const string ROMAN_NUM_100 = "C";

    internal const int ROMAN_NUM_XC = 90;
    internal const string ROMAN_NUM_90 = "XC";
    internal const int ROMAN_NUM_LXXX = 80;
    internal const string ROMAN_NUM_80 = "LXXX";
    internal const int ROMAN_NUM_LXX = 70;
    internal const string ROMAN_NUM_70 = "LXX";
    internal const int ROMAN_NUM_LX = 60;
    internal const string ROMAN_NUM_60 = "LX";

    internal const char ROMAN_NUM_50_UC = 'L';
    internal const char ROMAN_NUM_50_LC = 'l';
    internal const ushort ROMAN_NUM_L = 50;
    internal const string ROMAN_NUM_50 = "L";

    internal const int ROMAN_NUM_XL = 40;
    internal const string ROMAN_NUM_40 = "XL";
    internal const int ROMAN_NUM_XXX = 30;
    internal const string ROMAN_NUM_30 = "XXX";
    internal const int ROMAN_NUM_XX = 20;
    internal const string ROMAN_NUM_20 = "XX";
    
    internal const char ROMAN_NUM_10_UC = 'X';
    internal const char ROMAN_NUM_10_LC = 'x';
    internal const ushort ROMAN_NUM_X = 10;
    internal const string ROMAN_NUM_10 = "X";

    internal const int ROMAN_NUM_IX = 9;
    internal const string ROMAN_NUM_9 = "IX";
    internal const int ROMAN_NUM_VIII = 8;
    internal const string ROMAN_NUM_8 = "VIII";
    internal const int ROMAN_NUM_VII = 7;
    internal const string ROMAN_NUM_7 = "VII";
    internal const int ROMAN_NUM_VI = 6;
    internal const string ROMAN_NUM_6 = "VI";
    
    internal const char ROMAN_NUM_5_UC = 'V';
    internal const char ROMAN_NUM_5_LC = 'v';
    internal const ushort ROMAN_NUM_V = 5;
    internal const string ROMAN_NUM_5 = "V";

    internal const int ROMAN_NUM_IV = 4;
    internal const string ROMAN_NUM_4 = "IV";
    internal const int ROMAN_NUM_III = 3;
    internal const string ROMAN_NUM_3 = "III";
    internal const int ROMAN_NUM_II = 2;
    internal const string ROMAN_NUM_2 = "II";
    
    internal const char ROMAN_NUM_1_UC = 'I';
    internal const char ROMAN_NUM_1_LC = 'i';
    internal const ushort ROMAN_NUM_I = 1;
    internal const string ROMAN_NUM_1 = "I";

    public static bool IsValidValidNumericDelimiter(ITokenCharacters? delimiter)
    {
        if (delimiter is null || delimiter.Count == 0)
            return true;
        char c = delimiter[^1];
        return c != DELIMITER_DASH && !char.IsLetterOrDigit(c);
    }

    public static ITokenCharacters? AssertValidNumericDelimiter(ITokenCharacters? delimiter)
    {
        if (delimiter is null || delimiter.Count == 0)
            return null;
        char c = delimiter[^1];
        if (c == DELIMITER_DASH || char.IsLetterOrDigit(c))
            throw new ArgumentException($"Delimiter for a numeric value cannot be a digit, letter or a dash (-).", nameof(delimiter));
        return delimiter;
    }

    public static bool IsValidNumericalPostfix(string? nonNumerical)
    {
        if (string.IsNullOrEmpty(nonNumerical))
            return true;
        char c = nonNumerical[0];
        return c != DELIMITER_DOT && !char.IsDigit(c);
    }
    
    public static bool IsValidNumericalPostfix(ReadOnlySpan<char> nonNumerical)
    {
        if (nonNumerical.Length == 0)
            return true;
        char c = nonNumerical[0];
        return c != DELIMITER_DOT && !char.IsDigit(c);
    }
    
    public static string? AssertValidNumericalPostfix(string? nonNumerical)
    {
        if (string.IsNullOrEmpty(nonNumerical))
            return null;
        char c = nonNumerical[0];
        if (c == DELIMITER_DOT || char.IsDigit(c))
            throw new ArgumentException($"Delimiter for a numeric value cannot be a digit or a period (.).", nameof(nonNumerical));
        return nonNumerical;
    }

    public static bool IsValidValidRomanNumericDelimiter(ReadOnlySpan<char> target, int startIndex, int endIndex) => target.Length == 0 || !char.IsLetterOrDigit(target[^1]);

    public static ITokenCharacters? AssertValidRomanNumericDelimiter(ITokenCharacters? delimiter)
    {
        if (delimiter is null || delimiter.Count == 0)
            return null;
        if (char.IsLetterOrDigit(delimiter[^1]))
            throw new ArgumentException($"Delimiter for a roman numeral cannot be a digit or letter.", nameof(delimiter));
        return delimiter;
    }

    public static string ToRomanNumeral(ushort value)
    {
        if (value == 0)
            return string.Empty;
        if (value > RomanNumeralToken.MAX_VALUE)
            throw new ArgumentOutOfRangeException(nameof(value));
        switch (value)
        {
            case 0: return string.Empty;
            case ROMAN_NUM_I: return ROMAN_NUM_1;
            case ROMAN_NUM_II: return ROMAN_NUM_2;
            case ROMAN_NUM_III: return ROMAN_NUM_3;
            case ROMAN_NUM_IV: return ROMAN_NUM_4;
            case ROMAN_NUM_V: return ROMAN_NUM_5;
            case ROMAN_NUM_VI: return ROMAN_NUM_6;
            case ROMAN_NUM_VII: return ROMAN_NUM_7;
            case ROMAN_NUM_VIII: return ROMAN_NUM_8;
            case ROMAN_NUM_IX: return ROMAN_NUM_9;
            case ROMAN_NUM_X: return ROMAN_NUM_10;
            case ROMAN_NUM_XX: return ROMAN_NUM_20;
            case ROMAN_NUM_XXX: return ROMAN_NUM_30;
            case ROMAN_NUM_XL: return ROMAN_NUM_40;
            case ROMAN_NUM_L: return ROMAN_NUM_50;
            case ROMAN_NUM_LX: return ROMAN_NUM_60;
            case ROMAN_NUM_LXX: return ROMAN_NUM_70;
            case ROMAN_NUM_LXXX: return ROMAN_NUM_80;
            case ROMAN_NUM_XC: return ROMAN_NUM_90;
            case ROMAN_NUM_C: return ROMAN_NUM_100;
            case ROMAN_NUM_CC: return ROMAN_NUM_200;
            case ROMAN_NUM_CCC: return ROMAN_NUM_300;
            case ROMAN_NUM_CD: return ROMAN_NUM_400;
            case ROMAN_NUM_D: return ROMAN_NUM_500;
            case ROMAN_NUM_DC: return ROMAN_NUM_600;
            case ROMAN_NUM_DCC: return ROMAN_NUM_700;
            case ROMAN_NUM_DCCC: return ROMAN_NUM_800;
            case ROMAN_NUM_CM: return ROMAN_NUM_900;
            case ROMAN_NUM_M: return ROMAN_NUM_1000;
            case ROMAN_NUM_MM: return ROMAN_NUM_2000;
            case ROMAN_NUM_MMM: return ROMAN_NUM_3000;
        }
        
        StringBuilder sb = new();
        while (value > ROMAN_NUM_M)
        {
            value -= ROMAN_NUM_M;
            sb.Append(ROMAN_NUM_1000_UC);
        }
        if (value >= ROMAN_NUM_CM)
        {
            value -= ROMAN_NUM_CM;
            sb.Append(ROMAN_NUM_900);
        }
        else if (value >= ROMAN_NUM_D)
        {
            value -= ROMAN_NUM_D;
            sb.Append(ROMAN_NUM_500_UC);
            while (value >= ROMAN_NUM_C)
            {
                value -= ROMAN_NUM_C;
                sb.Append(ROMAN_NUM_100_UC);
            }
        }
        else if (value >= ROMAN_NUM_CD)
        {
            value -= ROMAN_NUM_CD;
            sb.Append(ROMAN_NUM_400);
        }
        else
            while (value >= ROMAN_NUM_C)
            {
                value -= ROMAN_NUM_C;
                sb.Append(ROMAN_NUM_100_UC);
            }
        if (value >= ROMAN_NUM_XC)
        {
            value -= ROMAN_NUM_XC;
            sb.Append(ROMAN_NUM_90);
        }
        else if (value >= ROMAN_NUM_L)
        {
            value -= ROMAN_NUM_L;
            sb.Append(ROMAN_NUM_50_UC);
            while (value >= ROMAN_NUM_X)
            {
                value -= 10;
                sb.Append(ROMAN_NUM_10_UC);
            }
        }
        else if (value >= ROMAN_NUM_XL)
        {
            value -= ROMAN_NUM_XL;
            sb.Append(ROMAN_NUM_40);
        }
        else
            while (value >= ROMAN_NUM_X)
            {
                value -= ROMAN_NUM_X;
                sb.Append(ROMAN_NUM_10_UC);
            }
        if (value == ROMAN_NUM_IX)
            return sb.Append(ROMAN_NUM_9).ToString();
        if (value >= ROMAN_NUM_V)
        {
            value -= ROMAN_NUM_V;
            sb.Append(ROMAN_NUM_5_UC);
            while (value > 0)
            {
                value--;
                sb.Append(ROMAN_NUM_1_UC);
            }
        }
        else
        {
            if (value == ROMAN_NUM_IV)
                return sb.Append(ROMAN_NUM_4).ToString();
            while (value > 0)
            {
                value--;
                sb.Append(ROMAN_NUM_1_UC);
            }
        }
        return sb.ToString();
    }

    public static bool TryParseNumericalToken(ReadOnlySpan<char> target, int delimiterStart, int startIndex, int endIndex, out int nextIndex, out INumericalToken? result)
    {
        if ((nextIndex = startIndex) >= target.Length)
        {
            nextIndex = target.Length;
            result = null;
            return false;
        }
        if (endIndex <= startIndex)
        {
            result = null;
            return false;
        }
        char c = target[nextIndex];
        bool isNegative = c == DELIMITER_DASH;
        if (isNegative)
        {
            int index = nextIndex + 1;
            if (index == target.Length || !char.IsNumber(target[index]))
            {
                result = null;
                return false;
            }
            nextIndex = index + 1;
        }
        else if (!char.IsDigit(c))
        {
            if (IsRomanNumeralLetter(c))
            {
                int end = startIndex + RomanNumeralToken.MAX_STRING_LENGTH;
                if (end > target.Length)
                    end = target.Length;
                while (++nextIndex < end)
                {
                    if (!IsRomanNumeralLetter(c))
                        break;
                }
                int diff = startIndex - delimiterStart;
                if (diff > 0)
                {
                    if (IsValidValidRomanNumericDelimiter(target, delimiterStart, startIndex))
                    {
                        if (diff == 1)
                            result = new RomanNumeralToken(new TokenCharacter(target[delimiterStart]), target, startIndex, nextIndex);
                        else
                            result = new RomanNumeralToken(new TokenString(target, delimiterStart, startIndex), target, startIndex, nextIndex);
                        return true;
                    }
                }
                else
                {
                    result = new RomanNumeralToken(target, startIndex, nextIndex);
                    return true;
                }
            }
            result = null;
            return false;
        }
        throw new NotImplementedException();
    }

    [Obsolete("Use ParseRomanNumeral(ReadOnlySpan<char>, int, int, out int)")]
    public static ushort ParseRomanNumeral(int startIndex, ReadOnlySpan<char> target, out int nextIndex)
    {
        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        if ((nextIndex = startIndex) >= target.Length)
        {
            nextIndex = target.Length;
            return 0;
        }
        static bool is1000(char c) => c == ROMAN_NUM_1000_UC || c == ROMAN_NUM_1000_LC;
        ushort result;
        if (is1000(target[nextIndex]))
        {
            if (++nextIndex == target.Length)
                return ROMAN_NUM_M; 
            result = ROMAN_NUM_M;
            if (is1000(target[nextIndex]))
            {
                result += ROMAN_NUM_M;
                if (++nextIndex == target.Length)
                    return result; 
                if (is1000(target[nextIndex]))
                {
                    result += ROMAN_NUM_M;
                    if (++nextIndex == target.Length)
                        return result; 
                }
            }
        }
        else
            result = 0;
        
        static bool is500(char c) => c == ROMAN_NUM_500_UC || c == ROMAN_NUM_500_LC;
        static bool is100(char c) => c == ROMAN_NUM_100_UC || c == ROMAN_NUM_100_LC;

        if (is500(target[nextIndex]))
        {
            result += ROMAN_NUM_D;
            if (++nextIndex == target.Length)
                return result; 
            for (int i = 0; i < 3 && is100(target[nextIndex]); i++)
            {
                result += ROMAN_NUM_C;
                if (++nextIndex == target.Length)
                    return result;
            }
        }
        else if (is100(target[nextIndex]))
        {
            result += ROMAN_NUM_C;
            if (++nextIndex == target.Length)
                return result;
            if (is500(target[nextIndex]))
            {
                result += ROMAN_NUM_CCC;
                if (++nextIndex == target.Length)
                    return result;
            }
            else if (is1000(target[nextIndex]))
            {
                result += ROMAN_NUM_DCCC;
                if (++nextIndex == target.Length)
                    return result;
            }
            else if (is100(target[nextIndex]))
            {
                result += ROMAN_NUM_C;
                if (++nextIndex == target.Length)
                    return result;
                if (is100(target[nextIndex]))
                {
                    result += ROMAN_NUM_C;
                    if (++nextIndex == target.Length)
                        return result;
                }
            }
        }

        static bool is50(char c) => c == ROMAN_NUM_50_UC || c == ROMAN_NUM_50_LC;
        static bool is10(char c) => c == ROMAN_NUM_10_UC || c == ROMAN_NUM_10_LC;
        
        if (is50(target[nextIndex]))
        {
            result += ROMAN_NUM_L;
            if (++nextIndex == target.Length)
                return result; 
            for (int i = 0; i < 3 && is10(target[nextIndex]); i++)
            {
                result += ROMAN_NUM_X;
                if (++nextIndex == target.Length)
                    return result;
            }
        }
        else if (is10(target[nextIndex]))
        {
            result += ROMAN_NUM_X;
            if (++nextIndex == target.Length)
                return result;
            if (is50(target[nextIndex]))
            {
                result += ROMAN_NUM_XXX;
                if (++nextIndex == target.Length)
                    return result;
            }
            else if (is100(target[nextIndex]))
            {
                result += ROMAN_NUM_LXXX;
                if (++nextIndex == target.Length)
                    return result;
            }
            else if (is10(target[nextIndex]))
            {
                result += ROMAN_NUM_X;
                if (++nextIndex == target.Length)
                    return result;
                if (is10(target[nextIndex]))
                {
                    result += ROMAN_NUM_X;
                    if (++nextIndex == target.Length)
                        return result;
                }
            }
        }

        static bool is5(char c) => c == ROMAN_NUM_5_UC || c == ROMAN_NUM_5_LC;
        static bool is1(char c) => c == ROMAN_NUM_1_UC || c == ROMAN_NUM_1_LC;
        
        if (is5(target[nextIndex]))
        {
            result += ROMAN_NUM_V;
            if (++nextIndex == target.Length)
                return result; 
            for (int i = 0; i < 3 && is1(target[nextIndex]); i++)
            {
                result++;
                if (++nextIndex == target.Length)
                    return result;
            }
        }
        else if (is1(target[nextIndex]))
        {
            result++;
            if (++nextIndex == target.Length)
                return result;
            if (is5(target[nextIndex]))
            {
                result += ROMAN_NUM_III;
                nextIndex++;
            }
            else if (is10(target[nextIndex]))
            {
                result += ROMAN_NUM_VIII;
                nextIndex++;
            }
            else if (is1(target[nextIndex]))
            {
                result++;
                if (++nextIndex == target.Length)
                    return result;
                if (is1(target[nextIndex]))
                {
                    result++;
                    nextIndex++;
                }
            }
        }
        return result;
    }

    public static ushort ParseRomanNumeral(ReadOnlySpan<char> target, out int nextIndex) => ParseRomanNumeral(target, 0, target.Length, out nextIndex);
    
    public static ushort ParseRomanNumeral(ReadOnlySpan<char> target, int startIndex, out int nextIndex) => ParseRomanNumeral(target, startIndex, target.Length, out nextIndex);
    
    public static ushort ParseRomanNumeral(ReadOnlySpan<char> target, int startIndex, int endIndex, out int nextIndex)
    {
        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        if ((nextIndex = startIndex) >= target.Length)
        {
            nextIndex = target.Length;
            return 0;
        }
        if (endIndex <= startIndex)
        {
            nextIndex = startIndex;
            return 0;
        }
        if (endIndex > target.Length)
            endIndex = target.Length;

        static bool is1000(char c) => c == ROMAN_NUM_1000_UC || c == ROMAN_NUM_1000_LC;
        ushort result;
        if (is1000(target[nextIndex]))
        {
            if (++nextIndex == endIndex)
                return ROMAN_NUM_M; 
            result = ROMAN_NUM_M;
            if (is1000(target[nextIndex]))
            {
                result += ROMAN_NUM_M;
                if (++nextIndex == endIndex)
                    return result; 
                if (is1000(target[nextIndex]))
                {
                    result += ROMAN_NUM_M;
                    if (++nextIndex == endIndex)
                        return result; 
                }
            }
        }
        else
            result = 0;
        
        static bool is500(char c) => c == ROMAN_NUM_500_UC || c == ROMAN_NUM_500_LC;
        static bool is100(char c) => c == ROMAN_NUM_100_UC || c == ROMAN_NUM_100_LC;

        if (is500(target[nextIndex]))
        {
            result += ROMAN_NUM_D;
            if (++nextIndex == endIndex)
                return result; 
            for (int i = 0; i < 3 && is100(target[nextIndex]); i++)
            {
                result += ROMAN_NUM_C;
                if (++nextIndex == endIndex)
                    return result;
            }
        }
        else if (is100(target[nextIndex]))
        {
            result += ROMAN_NUM_C;
            if (++nextIndex == endIndex)
                return result;
            if (is500(target[nextIndex]))
            {
                result += ROMAN_NUM_CCC;
                if (++nextIndex == endIndex)
                    return result;
            }
            else if (is1000(target[nextIndex]))
            {
                result += ROMAN_NUM_DCCC;
                if (++nextIndex == endIndex)
                    return result;
            }
            else if (is100(target[nextIndex]))
            {
                result += ROMAN_NUM_C;
                if (++nextIndex == endIndex)
                    return result;
                if (is100(target[nextIndex]))
                {
                    result += ROMAN_NUM_C;
                    if (++nextIndex == endIndex)
                        return result;
                }
            }
        }

        static bool is50(char c) => c == ROMAN_NUM_50_UC || c == ROMAN_NUM_50_LC;
        static bool is10(char c) => c == ROMAN_NUM_10_UC || c == ROMAN_NUM_10_LC;
        
        if (is50(target[nextIndex]))
        {
            result += ROMAN_NUM_L;
            if (++nextIndex == endIndex)
                return result; 
            for (int i = 0; i < 3 && is10(target[nextIndex]); i++)
            {
                result += ROMAN_NUM_X;
                if (++nextIndex == endIndex)
                    return result;
            }
        }
        else if (is10(target[nextIndex]))
        {
            result += ROMAN_NUM_X;
            if (++nextIndex == endIndex)
                return result;
            if (is50(target[nextIndex]))
            {
                result += ROMAN_NUM_XXX;
                if (++nextIndex == endIndex)
                    return result;
            }
            else if (is100(target[nextIndex]))
            {
                result += ROMAN_NUM_LXXX;
                if (++nextIndex == endIndex)
                    return result;
            }
            else if (is10(target[nextIndex]))
            {
                result += ROMAN_NUM_X;
                if (++nextIndex == endIndex)
                    return result;
                if (is10(target[nextIndex]))
                {
                    result += ROMAN_NUM_X;
                    if (++nextIndex == endIndex)
                        return result;
                }
            }
        }

        static bool is5(char c) => c == ROMAN_NUM_5_UC || c == ROMAN_NUM_5_LC;
        static bool is1(char c) => c == ROMAN_NUM_1_UC || c == ROMAN_NUM_1_LC;
        
        if (is5(target[nextIndex]))
        {
            result += ROMAN_NUM_V;
            if (++nextIndex == endIndex)
                return result; 
            for (int i = 0; i < 3 && is1(target[nextIndex]); i++)
            {
                result++;
                if (++nextIndex == endIndex)
                    return result;
            }
        }
        else if (is1(target[nextIndex]))
        {
            result++;
            if (++nextIndex == endIndex)
                return result;
            if (is5(target[nextIndex]))
            {
                result += ROMAN_NUM_III;
                nextIndex++;
            }
            else if (is10(target[nextIndex]))
            {
                result += ROMAN_NUM_VIII;
                nextIndex++;
            }
            else if (is1(target[nextIndex]))
            {
                result++;
                if (++nextIndex == endIndex)
                    return result;
                if (is1(target[nextIndex]))
                {
                    result++;
                    nextIndex++;
                }
            }
        }
        return result;
    }

    [Obsolete("Use TestRomanNumeral(ReadOnlySpan<char>, int, int, out int)")]
    public static bool TestRomanNumeral(int startIndex, ReadOnlySpan<char> target, out int nextIndex)
    {
        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        if ((nextIndex = startIndex) >= target.Length)
        {
            nextIndex = target.Length;
            return false;
        }
        static bool is1000(char c) => c == ROMAN_NUM_1000_UC || c == ROMAN_NUM_1000_LC;
        bool result = is1000(target[nextIndex]);
        if (result)
        {
            if (++nextIndex == target.Length)
                return true; 
            if (is1000(target[nextIndex]))
            {
                if (++nextIndex == target.Length)
                    return true; 
                if (is1000(target[nextIndex]) && ++nextIndex == target.Length)
                    return true; 
            }
        }
        
        static bool is500(char c) => c == ROMAN_NUM_500_UC || c == ROMAN_NUM_500_LC;
        static bool is100(char c) => c == ROMAN_NUM_100_UC || c == ROMAN_NUM_100_LC;

        if (is500(target[nextIndex]))
        {
            if (++nextIndex == target.Length)
                return true; 
            for (int i = 0; i < 3 && is100(target[nextIndex]); i++)
            {
                if (++nextIndex == target.Length)
                    return true; 
            }
            result = true;
        }
        else if (is100(target[nextIndex]))
        {
            if (++nextIndex == target.Length)
                return true; 
            if (is500(target[nextIndex]) || is1000(target[nextIndex]))
            {
                if (++nextIndex == target.Length)
                    return true; 
            }
            else if (is100(target[nextIndex]))
            {
                if (++nextIndex == target.Length)
                    return true; 
                if (is100(target[nextIndex]))
                {
                    if (++nextIndex == target.Length)
                        return true; 
                }
            }
            result = true;
        }

        static bool is50(char c) => c == ROMAN_NUM_50_UC || c == ROMAN_NUM_50_LC;
        static bool is10(char c) => c == ROMAN_NUM_10_UC || c == ROMAN_NUM_10_LC;
        
        if (is50(target[nextIndex]))
        {
            if (++nextIndex == target.Length)
                return true; 
            for (int i = 0; i < 3 && is10(target[nextIndex]); i++)
            {
                if (++nextIndex == target.Length)
                    return true; 
            }
            result = true;
        }
        else if (is10(target[nextIndex]))
        {
            if (++nextIndex == target.Length)
                return true; 
            if (is50(target[nextIndex]) || is100(target[nextIndex]))
            {
                if (++nextIndex == target.Length)
                    return true; 
            }
            else if (is10(target[nextIndex]))
            {
                if (++nextIndex == target.Length)
                    return true; 
                if (is10(target[nextIndex]))
                {
                    if (++nextIndex == target.Length)
                        return true; 
                }
            }
            result = true;
        }

        static bool is5(char c) => c == ROMAN_NUM_5_UC || c == ROMAN_NUM_5_LC;
        static bool is1(char c) => c == ROMAN_NUM_1_UC || c == ROMAN_NUM_1_LC;
        
        if (is5(target[nextIndex]))
        {
            if (++nextIndex == target.Length)
                return true; 
            for (int i = 0; i < 3 && is1(target[nextIndex]); i++)
                if (++nextIndex == target.Length)
                    return true;
            return true;
        }
        if (is1(target[nextIndex]))
        {
            if (++nextIndex < target.Length && (is5(target[nextIndex]) || is10(target[nextIndex]) || (is1(target[nextIndex]) && ++nextIndex < target.Length && is1(target[nextIndex]))))
                nextIndex++;
            return true;
        }
        return result;
    }

    public static bool TestRomanNumeral(ReadOnlySpan<char> target, out int nextIndex) => TestRomanNumeral(target, 0, target.Length, out nextIndex);
    
    public static bool TestRomanNumeral(ReadOnlySpan<char> target, int startIndex, out int nextIndex) => TestRomanNumeral(target, startIndex, target.Length, out nextIndex);
    
    public static bool TestRomanNumeral(ReadOnlySpan<char> target, int startIndex, int endIndex, out int nextIndex)
    {
        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        if ((nextIndex = startIndex) >= target.Length)
        {
            nextIndex = target.Length;
            return false;
        }
        if (endIndex <= startIndex)
        {
            nextIndex = startIndex;
            return false;
        }
        if (endIndex > target.Length)
            endIndex = target.Length;
        static bool is1000(char c) => c == ROMAN_NUM_1000_UC || c == ROMAN_NUM_1000_LC;
        bool result = is1000(target[nextIndex]);
        if (result)
        {
            if (++nextIndex == endIndex)
                return true; 
            if (is1000(target[nextIndex]))
            {
                if (++nextIndex == endIndex)
                    return true; 
                if (is1000(target[nextIndex]) && ++nextIndex == endIndex)
                    return true; 
            }
        }
        
        static bool is500(char c) => c == ROMAN_NUM_500_UC || c == ROMAN_NUM_500_LC;
        static bool is100(char c) => c == ROMAN_NUM_100_UC || c == ROMAN_NUM_100_LC;

        if (is500(target[nextIndex]))
        {
            if (++nextIndex == endIndex)
                return true; 
            for (int i = 0; i < 3 && is100(target[nextIndex]); i++)
            {
                if (++nextIndex == endIndex)
                    return true; 
            }
            result = true;
        }
        else if (is100(target[nextIndex]))
        {
            if (++nextIndex == endIndex)
                return true; 
            if (is500(target[nextIndex]) || is1000(target[nextIndex]))
            {
                if (++nextIndex == endIndex)
                    return true; 
            }
            else if (is100(target[nextIndex]))
            {
                if (++nextIndex == endIndex)
                    return true; 
                if (is100(target[nextIndex]))
                {
                    if (++nextIndex == endIndex)
                        return true; 
                }
            }
            result = true;
        }

        static bool is50(char c) => c == ROMAN_NUM_50_UC || c == ROMAN_NUM_50_LC;
        static bool is10(char c) => c == ROMAN_NUM_10_UC || c == ROMAN_NUM_10_LC;
        
        if (is50(target[nextIndex]))
        {
            if (++nextIndex == endIndex)
                return true; 
            for (int i = 0; i < 3 && is10(target[nextIndex]); i++)
            {
                if (++nextIndex == endIndex)
                    return true; 
            }
            result = true;
        }
        else if (is10(target[nextIndex]))
        {
            if (++nextIndex == endIndex)
                return true; 
            if (is50(target[nextIndex]) || is100(target[nextIndex]))
            {
                if (++nextIndex == endIndex)
                    return true; 
            }
            else if (is10(target[nextIndex]))
            {
                if (++nextIndex == endIndex)
                    return true; 
                if (is10(target[nextIndex]))
                {
                    if (++nextIndex == endIndex)
                        return true; 
                }
            }
            result = true;
        }

        static bool is5(char c) => c == ROMAN_NUM_5_UC || c == ROMAN_NUM_5_LC;
        static bool is1(char c) => c == ROMAN_NUM_1_UC || c == ROMAN_NUM_1_LC;
        
        if (is5(target[nextIndex]))
        {
            if (++nextIndex == endIndex)
                return true; 
            for (int i = 0; i < 3 && is1(target[nextIndex]); i++)
                if (++nextIndex == endIndex)
                    return true;
            return true;
        }
        if (is1(target[nextIndex]))
        {
            if (++nextIndex < endIndex && (is5(target[nextIndex]) || is10(target[nextIndex]) || (is1(target[nextIndex]) && ++nextIndex < endIndex && is1(target[nextIndex]))))
                nextIndex++;
            return true;
        }
        return result;
    }

}
