using System.Diagnostics.CodeAnalysis;

namespace CdnGetter.Parsing;

/// <summary>
/// Represents a token parser than produces a parsed value.
/// </summary>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TOutput"></typeparam>
public interface IParser<TInput, TOutput>
{
    /// <summary>
    /// Tests whether one or more elements starting at a specified index can be parsed as a token.
    /// </summary>
    /// <param name="source">The source elements.</param>
    /// <param name="startIndex">The starting index within the <paramref name="source" /> span.</param>
    /// <param name="endIndex">The exclusive ending index of the range of elements to be parsed. This should be greater than <paramref name="startIndex" />
    /// and not greater than the <see cref="ReadOnlySpan{T}.Length" /> of the <paramref name="source" />.</param>
    /// <param name="nextIndex">Returns next index after the end of the series of elements that can be parsed as a token;
    /// otherwise, this returns the value of <paramref name="startIndex" /> if no token can be parsed from the starting index.</param>
    /// <returns><see langword="true" /> if one or more elements beginning from the <paramref name="startIndex" /> can be parsed as a token; otherwise <see langword="false" />.</returns>
    /// <remarks>If this method returns <see langword="true" />, <paramref name="nextIndex" /> should return a value greater than <paramref name="startIndex" />, but not greater than <paramref name="endIndex" />;
    /// otherwise, if this method returns <see langword="false" />, <paramref name="nextIndex" /> should return the same value as the <paramref name="startIndex" />.</remarks>
    bool Test(ReadOnlySpan<TInput> source, ParsingContext<TInput, TOutput> context, out int nextIndex);

    /// <summary>
    /// Gets the index of the next token from within a given range of elements.
    /// </summary>
    /// <param name="source">The source elements.</param>
    /// <param name="startIndex">The starting index within the <paramref name="source" /> span.</param>
    /// <param name="endIndex">The exclusive ending index of the range of elements to be parsed. This should be greater than <paramref name="startIndex" />
    /// and not greater than the <see cref="ReadOnlySpan{T}.Length" /> of the <paramref name="source" />.</param>
    /// <returns>The next index after the end of the series of elements that can be parsed as a token; otherwise, the value of <paramref name="startIndex" /> if no token can be parsed from the starting index.</returns>
    int GetIndexOfNextToken(ReadOnlySpan<TInput> source, ParsingContext<TInput, TOutput> context);
    /// <summary>
    /// Attempts to parse a token starting at a specified index.
    /// </summary>
    /// <param name="source">The source elements.</param>
    /// <param name="startIndex">The starting index within the <paramref name="source" /> span.</param>
    /// <param name="endIndex">The exclusive ending index of the range of elements to be parsed. This should be greater than <paramref name="startIndex" />
    /// and not greater than the <see cref="ReadOnlySpan{T}.Length" /> of the <paramref name="source" />.</param>
    /// <param name="result">Returns the parsed token value if this method returns <see langword="true" />.</param>
    /// <param name="nextIndex">Returns next index after the end of the series of parsable elements;
    /// otherwise, this returns the value of <paramref name="startIndex" /> if the elements beginning from the starting index do not represent parsable token elements.</param>
    /// <returns><see langword="true" /> if one or more elements beginning at the <paramref name="startIndex" /> can be parsed as a token; otherwise <see langword="false" />.</returns>
    /// <remarks>If this method returns <see langword="true" />, <paramref name="nextIndex" /> should return a value greater than <paramref name="startIndex" />, but not greater than <paramref name="endIndex" />;
    /// If this method returns <see langword="false" />, and <paramref name="nextIndex" /> is greater than <paramref name="startIndex" />, then that means elements were skipped;
    /// Lastly, if this method returns <see langword="false" />, and <paramref name="nextIndex" /> is equal to <paramref name="startIndex" />, then that indicates the elements beginning at the <paramref name="startIndex" /> were not parsable values.</remarks>
    bool TryParse(ReadOnlySpan<TInput> source, ParsingContext<TInput, TOutput> context, [MaybeNullWhen(false)] out TOutput result, out int nextIndex);
}