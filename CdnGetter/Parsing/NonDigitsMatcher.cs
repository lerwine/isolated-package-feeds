using System.Diagnostics.CodeAnalysis;

namespace CdnGetter.Parsing;

/// <summary>
/// Matches one or more non-digit values.
/// </summary>
public class NonDigitsMatcher : IMatcher<char>
{
    public static readonly NonDigitsMatcher Instance = new();

    private NonDigitsMatcher() { }

    /// <summary>
    /// Tests whether a non-numerical token can be parsed from one or more characters starting from the specified index.
    /// </summary>
    /// <param name="span">The source sequence of characters.</param>
    /// <param name="startIndex">The index of the first character to be tested.</param>
    /// <param name="endIndex">The exclusive index of the end of the range of characters to be tested.</param>
    /// <param name="nextIndex">Returns the index following the last matched character or the value of <paramref name="startIndex" /> if there is no match.</param>
    /// <returns><see langword="true" /> if the current <see cref="IMatcher{TInput}" /> can parse an <see cref="IToken" /> from one or more characters starting from the specified <paramref name="startIndex" />; otherwise, <see langword="false" />.</returns>
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

    /// <summary>
    /// Attempts to parse a non-numerical token from one or more characters starting from the specified index.
    /// </summary>
    /// <param name="span">The source sequence of characters.</param>
    /// <param name="startIndex">The index of the first character to be parsed.</param>
    /// <param name="endIndex">The exclusive index of the end of the range of characters to be parsed.</param>
    /// <param name="result">Returns the parsed <see cref="IToken" /> or <see langword="null" /> if no token could be parsed.</param>
    /// <param name="nextIndex">Returns the index following the last matched characters or the value of <paramref name="startIndex" /> if there is no match.</param>
    /// <returns><see langword="true" /> if the current <see cref="IMatcher{TInput}" /> parsed an <see cref="IToken" /> from one or more characters starting from the specified <paramref name="startIndex" />; otherwise, <see langword="false" />.</returns>
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