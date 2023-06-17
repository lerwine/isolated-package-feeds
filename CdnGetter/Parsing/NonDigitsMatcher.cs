using System.Diagnostics.CodeAnalysis;

namespace CdnGetter.Parsing;

public class NonDigitsMatcher : IMatcher<char>
{
    public static readonly NonDigitsMatcher Instance = new();

    private NonDigitsMatcher() { }

    public bool Match(ReadOnlySpan<char> span, int startIndex, int endIndex, out int nextIndex)
    {
        if (span.ValidateExtentsIsEmpty(ref startIndex, ref endIndex))
        {
            nextIndex = startIndex;
            return false;
        }
        nextIndex = startIndex;
        char c = span[nextIndex];
        if (char.IsNumber(c))
        {
            nextIndex = startIndex;
            return false;
        }
        while (++nextIndex != endIndex)
        {
            if (char.IsNumber(span[nextIndex]))
                break;
        }
        return true;
    }

    public bool TryParse(ReadOnlySpan<char> span, int startIndex, int endIndex, [NotNullWhen(true)] out IToken? result, out int nextIndex)
    {
        if (span.ValidateExtentsIsEmpty(ref startIndex, ref endIndex))
        {
            nextIndex = startIndex;
            result = null;
            return false;
        }
        nextIndex = startIndex;
        char c = span[nextIndex];
        if (char.IsNumber(c))
        {
            nextIndex = startIndex;
            result = null;
            return false;
        }
        if (++nextIndex == endIndex || char.IsNumber(span[nextIndex]))
        {
            result = new CharacterToken(c);
            return true;
        }
        while (++nextIndex < endIndex)
        {
            if (char.IsNumber(span[nextIndex]))
                break;
        }
        result = new StringToken(span, startIndex, nextIndex);
        return true;
    }
}