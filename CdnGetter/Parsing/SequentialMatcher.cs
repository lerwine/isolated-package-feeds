using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace CdnGetter.Parsing;

/// <summary>
/// Matches a sequence of matchers.
/// </summary>
/// <typeparam name="TInput">The input element type.</typeparam>
public class SequentialMatcher<TInput> : IMatcher<TInput>
{
    public ReadOnlyCollection<IMatcher<TInput>> ElementMatchers { get; }
    
    public SequentialMatcher(params IMatcher<TInput>[] elementMatchers) => ElementMatchers = new(elementMatchers?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher<TInput>>());
    
    public SequentialMatcher(IEnumerable<IMatcher<TInput>> elementMatchers) => ElementMatchers = new(elementMatchers?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher<TInput>>());
    
    public bool Match(ReadOnlySpan<TInput> span, int startIndex, int endIndex, out int nextIndex)
    {
        if (span.ValidateExtentsIsEmpty(ref startIndex, ref endIndex))
        {
            nextIndex = startIndex;
            return false;
        }
        nextIndex = startIndex;
        foreach (IMatcher<TInput> matcher in ElementMatchers)
        {
            if (!matcher.Match(span, nextIndex, endIndex, out nextIndex))
            {
                nextIndex = startIndex;
                return false;
            }
            if (nextIndex < startIndex)
                nextIndex = startIndex;
            else if (nextIndex > endIndex)
                nextIndex = endIndex;
        }
        return true;
    }

    public bool TryParse(ReadOnlySpan<TInput> span, int startIndex, int endIndex, [NotNullWhen(true)] out TokenList? result, out int nextIndex)
    {
        if (span.ValidateExtentsIsEmpty(ref startIndex, ref endIndex))
        {
            nextIndex = startIndex;
            result = null;
            return false;
        }
        Collection<IToken> items = new();
        nextIndex = startIndex;
        foreach (IMatcher<TInput> matcher in ElementMatchers)
        {
            if (!matcher.TryParse(span, nextIndex, endIndex, out IToken? item, out nextIndex))
            {
                nextIndex = startIndex;
                result = null;
                return false;
            }
            items.Add(item);
            if (nextIndex < startIndex)
                nextIndex = startIndex;
            else if (nextIndex > endIndex)
                nextIndex = endIndex;
        }
        result = new TokenList(items);
        return true;
    }

    bool IMatcher<TInput>.TryParse(ReadOnlySpan<TInput> span, int startIndex, int endIndex, [NotNullWhen(true)] out IToken? result, out int nextIndex)
    {
        if (TryParse(span, startIndex, endIndex, out TokenList? list, out nextIndex))
        {
            result = list;
            return true;
        }
        result = null;
        return false;
    }
}

public class SequentialMatcher<TInput, TAggregate> : IMatcher<TInput> where TAggregate : IToken
{
    public ReadOnlyCollection<IMatcher<TInput>> ElementMatchers { get; }

    public Func<IList<IToken>, TAggregate> Aggregator { get; }
    
    public SequentialMatcher(Func<IList<IToken>, TAggregate> aggregator, params IMatcher<TInput>[] elementMatchers) => (Aggregator, ElementMatchers) = (aggregator, new(elementMatchers?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher<TInput>>()));
    
    public SequentialMatcher(IEnumerable<IMatcher<TInput>> elementMatchers, Func<IList<IToken>, TAggregate> aggregator) => (ElementMatchers, Aggregator) = (new(elementMatchers?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher<TInput>>()), aggregator);
    
    public bool Match(ReadOnlySpan<TInput> span, int startIndex, int endIndex, out int nextIndex)
    {
        if (span.ValidateExtentsIsEmpty(ref startIndex, ref endIndex))
        {
            nextIndex = startIndex;
            return false;
        }
        nextIndex = startIndex;
        foreach (IMatcher<TInput> matcher in ElementMatchers)
        {
            if (!matcher.Match(span, nextIndex, endIndex, out nextIndex))
            {
                nextIndex = startIndex;
                return false;
            }
            if (nextIndex < startIndex)
                nextIndex = startIndex;
            else if (nextIndex > endIndex)
                nextIndex = endIndex;
        }
        return true;
    }

    public bool TryParse(ReadOnlySpan<TInput> span, int startIndex, int endIndex, [MaybeNullWhen(false)] out TAggregate result, out int nextIndex)
    {
        if (span.ValidateExtentsIsEmpty(ref startIndex, ref endIndex))
        {
            nextIndex = startIndex;
            result = default;
            return false;
        }
        Collection<IToken> items = new();
        nextIndex = startIndex;
        foreach (IMatcher<TInput> matcher in ElementMatchers)
        {
            if (!matcher.TryParse(span, nextIndex, endIndex, out IToken? item, out nextIndex))
            {
                nextIndex = startIndex;
                result = default;
                return false;
            }
            items.Add(item);
            if (nextIndex < startIndex)
                nextIndex = startIndex;
            else if (nextIndex > endIndex)
                nextIndex = endIndex;
        }
        result = Aggregator(items);
        return true;
    }

    bool IMatcher<TInput>.TryParse(ReadOnlySpan<TInput> span, int startIndex, int endIndex, [NotNullWhen(true)] out IToken? result, out int nextIndex)
    {
        if (TryParse(span, startIndex, endIndex, out TAggregate? aggregate, out nextIndex))
        {
            result = aggregate;
            return true;
        }
        result = null;
        return false;
    }
}
