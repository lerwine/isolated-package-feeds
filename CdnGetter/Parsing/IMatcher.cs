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
    /// <param name="source">The source character values.</param>
    /// <param name="startIndex">The index of the first value to be tested.</param>
    /// <param name="count">The number of characters  to be tested.</param>
    /// <param name="nextIndex">Returns the index following the last matched value or the value of <paramref name="startIndex" /> if there is no match.</param>
    /// <returns><see langword="true" /> if the current <see cref="IMatcher" /> can parse an <see cref="IToken" /> from one or more values starting from the specified <paramref name="startIndex" />; otherwise, <see langword="false" />.</returns>
    bool Match(ParsingSource source, int startIndex, int count, out int nextIndex);

    /// <summary>
    /// Attempts to parse a token from one or more values starting from the specified index.
    /// </summary>
    /// <param name="source">The source character values.</param>
    /// <param name="startIndex">The index of the first value to be parsed.</param>
    /// <param name="count">The number of characters  to be tested.</param>
    /// <param name="result">Returns the parsed <see cref="IToken" /> or <see langword="null" /> if no token could be parsed.</param>
    /// <param name="nextIndex">Returns the index following the last matched value or the value of <paramref name="startIndex" /> if there is no match.</param>
    /// <returns><see langword="true" /> if the current <see cref="IMatcher" /> parsed an <see cref="IToken" /> from one or more values starting from the specified <paramref name="startIndex" />; otherwise, <see langword="false" />.</returns>
    bool TryParse(ParsingSource source, int startIndex, int count, [NotNullWhen(true)] out IToken? result, out int nextIndex);
}
