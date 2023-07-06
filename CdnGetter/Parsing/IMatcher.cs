using System.Diagnostics.CodeAnalysis;

namespace CdnGetter.Parsing;

/// <summary>
/// Matches a specific pattern of values.
/// </summary>
public interface IMatcher
{
    /// <summary>
    /// Tests whether a token can be parsed from one or more values starting from the specified index.
    /// </summary>
    /// <param name="span">The source sequence of values.</param>
    /// <param name="startIndex">The index of the first value to be tested.</param>
    /// <param name="endIndex">The exclusive index of the end of the range of values to be tested.</param>
    /// <param name="nextIndex">Returns the index following the last matched value or the value of <paramref name="startIndex" /> if there is no match.</param>
    /// <returns><see langword="true" /> if the current <see cref="IMatcher" /> can parse an <see cref="IToken" /> from one or more values starting from the specified <paramref name="startIndex" />; otherwise, <see langword="false" />.</returns>
    bool Match(ReadOnlySpan<char> span, int startIndex, int endIndex, out int nextIndex);

    /// <summary>
    /// Attempts to parse a token from one or more values starting from the specified index.
    /// </summary>
    /// <param name="span">The source sequence of values.</param>
    /// <param name="startIndex">The index of the first value to be parsed.</param>
    /// <param name="endIndex">The exclusive index of the end of the range of values to be parsed.</param>
    /// <param name="result">Returns the parsed <see cref="IToken" /> or <see langword="null" /> if no token could be parsed.</param>
    /// <param name="nextIndex">Returns the index following the last matched value or the value of <paramref name="startIndex" /> if there is no match.</param>
    /// <returns><see langword="true" /> if the current <see cref="IMatcher" /> parsed an <see cref="IToken" /> from one or more values starting from the specified <paramref name="startIndex" />; otherwise, <see langword="false" />.</returns>
    bool TryParse(ReadOnlySpan<char> span, int startIndex, int endIndex, [NotNullWhen(true)] out IToken? result, out int nextIndex);
}
