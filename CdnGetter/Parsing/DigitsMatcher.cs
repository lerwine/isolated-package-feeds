using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace CdnGetter.Parsing;

/// <summary>
/// Matches one or more digit characters that can be parsed as an <see cref="INumericalToken" />.
/// </summary>
public class DigitsMatcher : IMatcher
{
    public static readonly DigitsMatcher Instance = new();
    
    private DigitsMatcher() { }

    /// <summary>
    /// Tests whether a numerical token can be parsed from one or more characters starting from the specified index.
    /// </summary>
    /// <param name="source">The source character values.</param>
    /// <param name="startIndex">The index of the first value to be tested.</param>
    /// <param name="count">The number of characters  to be tested.</param>
    /// <param name="nextIndex">Returns the index following the last matched character or the value of <paramref name="startIndex" /> if there is no match.</param>
    /// <returns><see langword="true" /> if the current <see cref="IMatcher" /> can parse an <see cref="IToken" /> from one or more characters starting from the specified <paramref name="startIndex" />; otherwise, <see langword="false" />.</returns>
    public bool Match(ParsingSource source, int startIndex, int count, out int nextIndex)
    {
        if (source.ValidateSourceIsEmpty(ref startIndex, ref count))
        {
            nextIndex = startIndex;
            return false;
        }
        nextIndex = startIndex;
        ReadOnlySpan<char> span = source.AsSpan(startIndex, count);
        int endIndex = startIndex + count;
        char c = span[nextIndex];
        if (c == ParsingExtensionMethods.DELIMITER_DASH)
        {
            if (++nextIndex == endIndex)
            {
                nextIndex = startIndex;
                return false;
            }
            c = span[nextIndex];
        }
        if (!char.IsDigit(c))
        {
            nextIndex = startIndex;
            return false;
        }
        do
        {
            if (++nextIndex == endIndex)
                break;
        }
        while (char.IsDigit(span[nextIndex]));
        return true;
    }

    /// <summary>
    /// Attempts to parse a token from one or more values starting from the specified index.
    /// </summary>
    /// <param name="source">The source character values.</param>
    /// <param name="startIndex">The index of the first value to be tested.</param>
    /// <param name="count">The number of characters  to be tested.</param>
    /// <param name="result">Returns the parsed <see cref="IToken" /> or <see langword="null" /> if no token could be parsed.</param>
    /// <param name="nextIndex">Returns the index following the last matched value or the value of <paramref name="startIndex" /> if there is no match.</param>
    /// <returns><see langword="true" /> if the current <see cref="IMatcher" /> parsed an <see cref="IToken" /> from one or more values starting from the specified <paramref name="startIndex" />; otherwise, <see langword="false" />.</returns>
    public static bool TryParse(ParsingSource source, int startIndex, int count, [NotNullWhen(true)] out INumericalToken? result, out int nextIndex)
    {
        if (source.ValidateSourceIsEmpty(ref startIndex, ref count))
        {
            nextIndex = startIndex;
            result = null;
            return false;
        }
        nextIndex = startIndex;
        ReadOnlySpan<char> span = source.AsSpan(startIndex, count);
        int endIndex = startIndex + count;
        char c = span[nextIndex];
        bool hasNegativeSign = c == ParsingExtensionMethods.DELIMITER_DASH;
        if (hasNegativeSign)
        {
            if (++nextIndex == endIndex)
            {
                nextIndex = startIndex;
                result = null;
                return false;
            }
            c = span[nextIndex];
        }
        if (!char.IsDigit(c))
        {
            nextIndex = startIndex;
            result = null;
            return false;
        }
        int zeroPadLength = 0;
        if (c == ParsingExtensionMethods.ZERO)
        {
            do
            {
                zeroPadLength++;
                if (++nextIndex == endIndex)
                {
                    result = new Digits8Bit(0, hasNegativeSign, zeroPadLength - 1);
                    return true;
                }
                c = span[nextIndex];
            }
            while (c == ParsingExtensionMethods.ZERO);
            if (!char.IsDigit(c))
            {
                result = new Digits8Bit(0, hasNegativeSign, zeroPadLength - 1);
                return true;
            }
        }
        int index = nextIndex;
        while (++nextIndex < endIndex)
        {
            if (!char.IsDigit(span[nextIndex]))
                break;
        }
        ReadOnlySpan<char> text = span[index..nextIndex];
        if (text.Length <= Digits8Bit.MAX_ABS_LENGTH && byte.TryParse(text, out byte b))
            result = new Digits8Bit(b, hasNegativeSign, zeroPadLength);
        else if (text.Length <= Digits16Bit.MAX_ABS_LENGTH && ushort.TryParse(text, out ushort s))
            result = new Digits16Bit(s, hasNegativeSign, zeroPadLength);
        else if (text.Length <= Digits32Bit.MAX_ABS_LENGTH && uint.TryParse(text, out uint i))
            result = new Digits32Bit(i, hasNegativeSign, zeroPadLength);
        else if (text.Length <= Digits64Bit.MAX_ABS_LENGTH && ulong.TryParse(text, out ulong l))
            result = new Digits64Bit(l, hasNegativeSign, zeroPadLength);
        else if (BigInteger.TryParse(text, out BigInteger n))
            result = new DigitsNBit(n, hasNegativeSign, zeroPadLength);
        else
        {
            nextIndex = startIndex;
            result = null;
            return false;
        }
        return true;
    }

    bool IMatcher.TryParse(ParsingSource source, int startIndex, int count, [NotNullWhen(true)] out IToken? result, out int nextIndex)
    {
        if (TryParse(source, startIndex, count, out INumericalToken? token, out nextIndex))
        {
            result = token;
            return true;
        }
        result = null;
        return false;
    }
}
