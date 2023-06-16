using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using static CdnGetter.Parsing.ParsingExtensionMethods;

namespace CdnGetter.Parsing;

public class DigitsMatcher : IMatcher<char, INumericalToken>
{
    public bool Match(ReadOnlySpan<char> span, int startIndex, int endIndex, out int nextIndex)
    {
        if (span.ValidateExtentsIsEmpty(ref startIndex, ref endIndex))
        {
            nextIndex = startIndex;
            return false;
        }
        nextIndex = startIndex;
        char c = span[nextIndex];
        if (c == DELIMITER_DASH)
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

    public bool TryParse(ReadOnlySpan<char> span, int startIndex, int endIndex, [NotNullWhen(true)] out INumericalToken? result, out int nextIndex)
    {
        if (span.ValidateExtentsIsEmpty(ref startIndex, ref endIndex))
        {
            nextIndex = startIndex;
            result = null;
            return false;
        }
        nextIndex = startIndex;
        char c = span[nextIndex];
        bool hasNegativeSign = c == DELIMITER_DASH;
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
        if (c == ZERO)
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
            while (c == ZERO);
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
}
