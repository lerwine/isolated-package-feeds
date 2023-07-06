using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace CdnGetter.Parsing;

/// <summary>
/// A compound matcher that parses/matches tokens from the a sequence matchers that can each produce a token.
/// </summary>
/// <seealso cref="AlternateMatcher" />
/// <seealso cref="SequentialMatcher{TAggregate}" />
public class SequentialMatcher : IMatcher
{
    /// <summary>
    /// The ordered matchers that make up this compound matcher.
    /// </summary>
    public ReadOnlyCollection<IMatcher> ElementMatchers { get; }
    
    /// <summary>
    /// The delegate that creates a <see cref="ITokenList" /> from the matched tokens.
    /// </summary>
    public Func<IList<IToken>, ITokenList> Aggregator { get; }

    /// <summary>
    /// Intializes a new <c>SequentialMatcher</c> that parses matches as a <see cref="TokenList" />.
    /// </summary>
    /// <param name="elementMatchers">The ordered matchers that will make up the new compound matcher.</param>
    public SequentialMatcher(params IMatcher[] elementMatchers) =>
        (ElementMatchers, Aggregator) = (new(elementMatchers?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher>()), new Func<IList<IToken>, ITokenList>(t => new TokenList(t)));
    
    /// <summary>
    /// Intializes a new <c>SequentialMatcher</c>.
    /// </summary>
    /// <param name="elementMatchers">The ordered matchers that will make up the new compound matcher.</param>
    /// <param name="aggregator">The delegate that creates a <see cref="ITokenList" /> from the matched tokens or <see langword="null" /> to parse as a <see cref="TokenList" />.</param>
    public SequentialMatcher(IEnumerable<IMatcher> elementMatchers, Func<IList<IToken>, ITokenList>? aggregator = null) =>
        (ElementMatchers, Aggregator) = (new(elementMatchers?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher>()), aggregator ?? new Func<IList<IToken>, ITokenList>(t => new TokenList(t)));
    
    /// <summary>
    /// Intializes a new <c>SequentialMatcher</c>.
    /// </summary>
    /// <param name="aggregator">The delegate that creates a <see cref="ITokenList" /> from the matched tokens.</param>
    /// <param name="elementMatchers">The ordered matchers that will make up the new compound matcher.</param>
    public SequentialMatcher(Func<IList<IToken>, ITokenList> aggregator, params IMatcher[] elementMatchers) =>
        (ElementMatchers, Aggregator) = (new(elementMatchers?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher>()), aggregator ?? throw new ArgumentNullException(nameof(aggregator)));
    
    /// <summary>
    /// Tests whether all matchers can sequentially produce tokens, with the first one starting from the specified index.
    /// </summary>
    /// <param name="source">The source character values.</param>
    /// <param name="startIndex">The index of the first value to be tested.</param>
    /// <param name="count">The number of characters  to be tested.</param>
    /// <param name="nextIndex">Returns the index following the last matched value or the value of <paramref name="startIndex" /> if there is no match.</param>
    /// <returns><see langword="true" /> if the all <see cref="ElementMatchers" /> can sequentially parse <see cref="IToken" />s from values starting from the specified <paramref name="startIndex" />; otherwise, <see langword="false" />.</returns>
    public bool Match(ParsingSource source, int startIndex, int count, out int nextIndex)
    {
        if (source.ValidateSourceIsEmpty(ref startIndex, ref count))
        {
            nextIndex = startIndex;
            return false;
        }
        nextIndex = startIndex;
        int endIndex = startIndex + count;
        foreach (IMatcher matcher in ElementMatchers)
        {
            count = endIndex - nextIndex;
            if (count < 1 || !matcher.Match(source, nextIndex, count, out nextIndex))
            {
                nextIndex = startIndex;
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Attempts to sequentially parse tokens, starting from the specified index.
    /// </summary>
    /// <param name="source">The source character values.</param>
    /// <param name="startIndex">The index of the first value to be tested.</param>
    /// <param name="count">The number of characters  to be tested.</param>
    /// <param name="result">Returns the parsed <see cref="ITokenList" /> or <see langword="null" /> if any of the <see cref="ElementMatchers" /> could not produced a token.</param>
    /// <param name="nextIndex">Returns the index following the last matched value or the value of <paramref name="startIndex" /> if there is no match.</param>
    /// <returns><see langword="true" /> if all <see cref="ElementMatchers" /> sequentially parsed <see cref="IToken" />s beginning from the specified <paramref name="startIndex" />; otherwise, <see langword="false" />.</returns>
    public bool TryParse(ParsingSource source, int startIndex, int count, [NotNullWhen(true)] out ITokenList? result, out int nextIndex)
    {
        if (source.ValidateSourceIsEmpty(ref startIndex, ref count))
        {
            nextIndex = startIndex;
            result = default;
            return false;
        }
        int endIndex = startIndex + count;
        Collection<IToken> items = new();
        nextIndex = startIndex;
        foreach (IMatcher matcher in ElementMatchers)
        {
            count = endIndex - nextIndex;
            if (count < 1 || !matcher.TryParse(source, nextIndex, count, out IToken? item, out nextIndex))
            {
                nextIndex = startIndex;
                result = null;
                return false;
            }
            items.Add(item);
        }
        result = Aggregator(items);
        return true;
    }

    bool IMatcher.TryParse(ParsingSource source, int startIndex, int count, [NotNullWhen(true)] out IToken? result, out int nextIndex)
    {
        if (TryParse(source, startIndex, count, out ITokenList? list, out nextIndex))
        {
            result = list;
            return true;
        }
        result = null;
        return false;
    }
}

/// <summary>
/// A compound matcher that parses/matches tokens from the a sequence matchers that can each produce a token.
/// </summary>
/// <typeparam name="TAggregate">The aggregate token type.</typeparam>
/// <seealso cref="AlternateMatcher" />
/// <seealso cref="SequentialMatcher" />
public class SequentialMatcher<TAggregate> : IMatcher where TAggregate : IToken
{
    /// <summary>
    /// The ordered matchers that make up this compound matcher.
    /// </summary>
    public ReadOnlyCollection<IMatcher> ElementMatchers { get; }

    /// <summary>
    /// The delegate that creates a <typeparamref name="TAggregate" /> from the matched tokens.
    /// </summary>
    public Func<IList<IToken>, TAggregate> Aggregator { get; }
    
    /// <summary>
    /// Initializes a new <c>SequentialMatcher</c>.
    /// </summary>
    /// <param name="aggregator">The delegate that creates a <typeparamref name="TAggregate" /> from the matched tokens.</param>
    /// <param name="elementMatchers">The ordered matchers will make up the new compound matcher.</param>
    public SequentialMatcher(Func<IList<IToken>, TAggregate> aggregator, params IMatcher[] elementMatchers) => (Aggregator, ElementMatchers) = (aggregator, new(elementMatchers?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher>()));
    
    /// <summary>
    /// Initializes a new <c>SequentialMatcher</c>.
    /// </summary>
    /// <param name="elementMatchers">The ordered matchers will make up the new compound matcher.</param>
    /// <param name="aggregator">The delegate that creates a <typeparamref name="TAggregate" /> from the matched tokens.</param>
    public SequentialMatcher(IEnumerable<IMatcher> elementMatchers, Func<IList<IToken>, TAggregate> aggregator) => (ElementMatchers, Aggregator) = (new(elementMatchers?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher>()), aggregator);

    /// <summary>
    /// Tests whether all matchers can sequentially produce tokens, with the first one starting from the specified index.
    /// </summary>
    /// <param name="source">The source character values.</param>
    /// <param name="startIndex">The index of the first value to be tested.</param>
    /// <param name="count">The number of characters  to be tested.</param>
    /// <param name="nextIndex">Returns the index following the last matched value or the value of <paramref name="startIndex" /> if there is no match.</param>
    /// <returns><see langword="true" /> if the all <see cref="ElementMatchers" /> can sequentially parse <see cref="IToken" />s from values starting from the specified <paramref name="startIndex" />; otherwise, <see langword="false" />.</returns>
    public bool Match(ParsingSource source, int startIndex, int count, out int nextIndex)
    {
        int endIndex = startIndex + count;
        if (endIndex > source.Count)
            endIndex = source.Count;
        nextIndex = startIndex;
        foreach (IMatcher matcher in ElementMatchers)
        {
            count = endIndex - nextIndex;
            if (count < 1 || !matcher.Match(source, nextIndex, endIndex, out nextIndex))
            {
                nextIndex = startIndex;
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Attempts to sequentially parse tokens, starting from the specified index.
    /// </summary>
    /// <param name="source">The source character values.</param>
    /// <param name="startIndex">The index of the first value to be tested.</param>
    /// <param name="count">The number of characters  to be tested.</param>
    /// <param name="result">Returns the combined <see cref="TAggregate" /> or <see langword="default" /> if any of the <see cref="ElementMatchers" /> could not produced a token.</param>
    /// <param name="nextIndex">Returns the index following the last matched value or the value of <paramref name="startIndex" /> if there is no match.</param>
    /// <returns><see langword="true" /> if all <see cref="ElementMatchers" /> sequentially parsed <see cref="IToken" />s beginning from the specified <paramref name="startIndex" />; otherwise, <see langword="false" />.</returns>
    public bool TryParse(ParsingSource source, int startIndex, int count, [MaybeNullWhen(false)] out TAggregate result, out int nextIndex)
    {
        int endIndex = startIndex + count;
        if (endIndex > source.Count)
            endIndex = source.Count;
        Collection<IToken> items = new();
        nextIndex = startIndex;
        foreach (IMatcher matcher in ElementMatchers)
        {
            count = endIndex - nextIndex;
            if (count < 1 || !matcher.TryParse(source, nextIndex, count, out IToken? item, out nextIndex))
            {
                nextIndex = startIndex;
                result = default;
                return false;
            }
            items.Add(item);
        }
        result = Aggregator(items);
        return true;
    }

    bool IMatcher.TryParse(ParsingSource source, int startIndex, int count, [NotNullWhen(true)] out IToken? result, out int nextIndex)
    {
        if (TryParse(source, startIndex, count, out TAggregate? aggregate, out nextIndex))
        {
            result = aggregate;
            return true;
        }
        result = null;
        return false;
    }
}
