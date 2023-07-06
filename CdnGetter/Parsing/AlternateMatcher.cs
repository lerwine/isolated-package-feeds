using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace CdnGetter.Parsing;

/// <summary>
/// A compound matcher that parses/matches tokens from the first matcher that can produce a token.
/// </summary>
/// <typeparam name="TInput">The item value type.</typeparam>
/// <seealso cref="SequentialMatcher" />
/// <seealso cref="SequentialMatcher{TAggregate}" />
public class AlternateMatcher : IMatcher
{
    /// <summary>
    /// Gets the ordered matchers that make up this compound matcher.
    /// </summary>
    public ReadOnlyCollection<IMatcher> Options { get; }
    
    /// <summary>
    /// Gets the optional delegate that creates a token when none of the matcher <see cref="Options" /> could produce a token.
    /// </summary>
    public Func<IToken>? NoMatchFactory { get; }
    
    /// <summary>
    /// Initializes a new <c>AlternateMatcher</c>.
    /// </summary>
    /// <param name="options">The matchers that will make up the new compound matcher.</param>
    /// <param name="noMatchFactory">The optional delegate that creates a token when none of the matchers can produce a token.</param>
    public AlternateMatcher(IEnumerable<IMatcher> options, Func<IToken>? noMatchFactory = null) => (Options, NoMatchFactory) = (new(options?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher>()), noMatchFactory);
    
    /// <summary>
    /// Initializes a new <c>AlternateMatcher</c>.
    /// </summary>
    /// <param name="noMatchFactory">The delegate that creates a token when none of the matchers can produce a token.</param>
    /// <param name="options">The matchers that will make up the new compound matcher.</param>
    public AlternateMatcher(Func<IToken> noMatchFactory, params IMatcher[] options) => (NoMatchFactory, Options) = (noMatchFactory ?? throw new ArgumentNullException(nameof(noMatchFactory)), new(options?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher>()));
    
    /// <summary>
    /// Initializes a new <c>AlternateMatcher</c>.
    /// </summary>
    /// <param name="options">The matchers that will make up the new compound matcher.</param>
    public AlternateMatcher(params IMatcher[] options) => (NoMatchFactory, Options) = (null, new(options?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher>()));
    
    /// <summary>
    /// Tests whether any matcher can produce a token starting from the specified index.
    /// </summary>
    /// <param name="source">The source character values.</param>
    /// <param name="startIndex">The index of the first value to be tested.</param>
    /// <param name="count">The number of characters  to be tested.</param>
    /// <param name="nextIndex">Returns the index following the last matched value or the value of <paramref name="startIndex" /> if there is no match.</param>
    /// <returns><see langword="true" /> if the any of the matcher <see cref="Options" /> can parse an <see cref="IToken" /> from one or more values starting from the specified <paramref name="startIndex" />; otherwise, <see langword="false" />.</returns>
    public bool Match(ParsingSource source, int startIndex, int count, out int nextIndex)
    {
        if (source.ValidateSourceNotEmpty(ref startIndex, ref count))
        {
            nextIndex = startIndex;
            int endIndex = startIndex + count;
            foreach (IMatcher matcher in Options)
                if (matcher.Match(source, nextIndex, endIndex - nextIndex, out nextIndex))
                    return true;
        }
        nextIndex = startIndex;
        return NoMatchFactory is not null;
    }

    /// <summary>
    /// Attempts to parse a token from any of the matchers, starting from the specified index.
    /// </summary>
    /// <param name="source">The source character values.</param>
    /// <param name="startIndex">The index of the first value to be tested.</param>
    /// <param name="count">The number of characters  to be tested.</param>
    /// <param name="result">Returns the <see cref="IToken" /> parsed by the first of the <see cref="Options" /> or <see langword="null" /> if no token could be parsed.</param>
    /// <param name="nextIndex">Returns the index following the last matched value or the value of <paramref name="startIndex" /> if there is no match.</param>
    /// <returns><see langword="true" /> if any of the <see cref="Options" /> parsed an <see cref="IToken" /> from one or more values starting from the specified <paramref name="startIndex" />; otherwise, <see langword="false" />.</returns>
    public bool TryParse(ParsingSource source, int startIndex, int count, [NotNullWhen(true)] out IToken? result, out int nextIndex)
    {
        if (source.ValidateSourceNotEmpty(ref startIndex, ref count))
        {
            nextIndex = startIndex;
            int endIndex = startIndex + count;
            foreach (IMatcher matcher in Options)
                if (matcher.TryParse(source, nextIndex,  endIndex - nextIndex, out result, out nextIndex))
                    return true;
        }
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
