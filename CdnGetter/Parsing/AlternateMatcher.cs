using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace CdnGetter.Parsing;

/// <summary>
/// A compound matcher that parses/matches tokens from the first matcher that can produce a token.
/// </summary>
/// <typeparam name="TInput">The item value type.</typeparam>
/// <seealso cref="SequentialMatcher{TInput}" />
/// <seealso cref="SequentialMatcher{TInput, TAggregate}" />
public class AlternateMatcher<TInput> : IMatcher<TInput>
{
    /// <summary>
    /// Gets the ordered matchers that make up this compound matcher.
    /// </summary>
    public ReadOnlyCollection<IMatcher<TInput>> Options { get; }
    
    /// <summary>
    /// Gets the optional delegate that creates a token when none of the matcher <see cref="Options" /> could produce a token.
    /// </summary>
    public Func<IToken>? NoMatchFactory { get; }
    
    /// <summary>
    /// Initializes a new <c>AlternateMatcher</c>.
    /// </summary>
    /// <param name="options">The matchers that will make up the new compound matcher.</param>
    /// <param name="noMatchFactory">The optional delegate that creates a token when none of the matchers can produce a token.</param>
    public AlternateMatcher(IEnumerable<IMatcher<TInput>> options, Func<IToken>? noMatchFactory = null) => (Options, NoMatchFactory) = (new(options?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher<TInput>>()), noMatchFactory);
    
    /// <summary>
    /// Initializes a new <c>AlternateMatcher</c>.
    /// </summary>
    /// <param name="noMatchFactory">The delegate that creates a token when none of the matchers can produce a token.</param>
    /// <param name="options">The matchers that will make up the new compound matcher.</param>
    public AlternateMatcher(Func<IToken> noMatchFactory, params IMatcher<TInput>[] options) => (NoMatchFactory, Options) = (noMatchFactory ?? throw new ArgumentNullException(nameof(noMatchFactory)), new(options?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher<TInput>>()));
    
    /// <summary>
    /// Initializes a new <c>AlternateMatcher</c>.
    /// </summary>
    /// <param name="options">The matchers that will make up the new compound matcher.</param>
    public AlternateMatcher(params IMatcher<TInput>[] options) => (NoMatchFactory, Options) = (null, new(options?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher<TInput>>()));
    
    /// <summary>
    /// Tests whether any matcher can produce a token starting from the specified index.
    /// </summary>
    /// <param name="span">The source sequence of values.</param>
    /// <param name="startIndex">The index of the first value to be tested.</param>
    /// <param name="endIndex">The exclusive index of the end of the range of values to be tested.</param>
    /// <param name="nextIndex">Returns the index following the last matched value or the value of <paramref name="startIndex" /> if there is no match.</param>
    /// <returns><see langword="true" /> if the any of the matcher <see cref="Options" /> can parse an <see cref="IToken" /> from one or more values starting from the specified <paramref name="startIndex" />; otherwise, <see langword="false" />.</returns>
    public bool Match(ReadOnlySpan<TInput> span, int startIndex, int endIndex, out int nextIndex)
    {
        if (span.ValidateExtentsNotEmpty(ref startIndex, ref endIndex))
            foreach (IMatcher<TInput> matcher in Options)
                if (matcher.Match(span, startIndex, endIndex, out nextIndex))
                    return true;
        nextIndex = startIndex;
        return NoMatchFactory is not null;
    }

    /// <summary>
    /// Attempts to parse a token from any of the matchers, starting from the specified index.
    /// </summary>
    /// <param name="span">The source sequence of values.</param>
    /// <param name="startIndex">The index of the first value to be parsed.</param>
    /// <param name="endIndex">The exclusive index of the end of the range of values to be parsed.</param>
    /// <param name="result">Returns the <see cref="IToken" /> parsed by the first of the <see cref="Options" /> or <see langword="null" /> if no token could be parsed.</param>
    /// <param name="nextIndex">Returns the index following the last matched value or the value of <paramref name="startIndex" /> if there is no match.</param>
    /// <returns><see langword="true" /> if any of the <see cref="Options" /> parsed an <see cref="IToken" /> from one or more values starting from the specified <paramref name="startIndex" />; otherwise, <see langword="false" />.</returns>
    public bool TryParse(ReadOnlySpan<TInput> span, int startIndex, int endIndex, [NotNullWhen(true)] out IToken? result, out int nextIndex)
    {
        if (span.ValidateExtentsNotEmpty(ref startIndex, ref endIndex))
            foreach (IMatcher<TInput> matcher in Options)
                if (matcher.TryParse(span, startIndex, endIndex, out result, out nextIndex))
                    return true;
        nextIndex = startIndex;
        if (NoMatchFactory is null)
        {
            result = null;
            return false;
        }
        result = NoMatchFactory();
        return true;
    }
}
