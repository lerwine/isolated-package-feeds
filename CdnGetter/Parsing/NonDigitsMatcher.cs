using System.Diagnostics.CodeAnalysis;

namespace CdnGetter.Parsing;

/// <summary>
/// Matches one or more non-digit values.
/// </summary>
public class NonDigitsMatcher : IMatcher
{
    public static readonly NonDigitsMatcher Instance = new();

    private NonDigitsMatcher() { }

    /// <summary>
    /// Tests whether a non-numerical token can be parsed from one or more characters starting from the specified index.
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
    /// <param name="source">The source character values.</param>
    /// <param name="startIndex">The index of the first value to be tested.</param>
    /// <param name="count">The number of characters  to be tested.</param>
    /// <param name="result">Returns the parsed <see cref="IToken" /> or <see langword="null" /> if no token could be parsed.</param>
    /// <param name="nextIndex">Returns the index following the last matched characters or the value of <paramref name="startIndex" /> if there is no match.</param>
    /// <returns><see langword="true" /> if the current <see cref="IMatcher" /> parsed an <see cref="IToken" /> from one or more characters starting from the specified <paramref name="startIndex" />; otherwise, <see langword="false" />.</returns>
    public bool TryParse(ParsingSource source, int startIndex, int count, [NotNullWhen(true)] out IToken? result, out int nextIndex)
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