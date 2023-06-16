using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace CdnGetter.Parsing;

/// <summary>
/// Matches a sequence of matchers.
/// </summary>
/// <typeparam name="TInput">The input element type.</typeparam>
/// <typeparam name="TOutput">The output element type.</typeparam>
public class SequentialMatcher<TInput, TOutput> : IMatcher<TInput, IList<TOutput>>
{
    public ReadOnlyCollection<IMatcher<TInput, TOutput>> ElementMatchers { get; }
    
    public SequentialMatcher(params IMatcher<TInput, TOutput>[] elementMatchers) => ElementMatchers = new(elementMatchers?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher<TInput, TOutput>>());
    
    public SequentialMatcher(IEnumerable<IMatcher<TInput, TOutput>> elementMatchers) => ElementMatchers = new(elementMatchers?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher<TInput, TOutput>>());
    
    public bool Match(ReadOnlySpan<TInput> span, int startIndex, int endIndex, out int nextIndex)
    {
        if (span.ValidateExtentsIsEmpty(ref startIndex, ref endIndex))
        {
            nextIndex = startIndex;
            return false;
        }
        nextIndex = startIndex;
        foreach (IMatcher<TInput, TOutput> matcher in ElementMatchers)
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

    public bool TryParse(ReadOnlySpan<TInput> span, int startIndex, int endIndex, [NotNullWhen(true)] out IList<TOutput>? result, out int nextIndex)
    {
        if (span.ValidateExtentsIsEmpty(ref startIndex, ref endIndex))
        {
            nextIndex = startIndex;
            result = null;
            return false;
        }
        result = new Collection<TOutput>();
        nextIndex = startIndex;
        foreach (IMatcher<TInput, TOutput> matcher in ElementMatchers)
        {
            if (!matcher.TryParse(span, nextIndex, endIndex, out TOutput? item, out nextIndex))
            {
                nextIndex = startIndex;
                result = null;
                return false;
            }
            result.Add(item);
            if (nextIndex < startIndex)
                nextIndex = startIndex;
            else if (nextIndex > endIndex)
                nextIndex = endIndex;
        }
        return true;
    }
}

public class SequentialMatcher<TInput, TOutput, TAggregate> : IMatcher<TInput, TAggregate>
{
    public ReadOnlyCollection<IMatcher<TInput, TOutput>> ElementMatchers { get; }

    public Func<IList<TOutput>, TAggregate> Aggregator { get; }
    
    public SequentialMatcher(Func<IList<TOutput>, TAggregate> aggregator, params IMatcher<TInput, TOutput>[] elementMatchers) => (Aggregator, ElementMatchers) = (aggregator, new(elementMatchers?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher<TInput, TOutput>>()));
    
    public SequentialMatcher(IEnumerable<IMatcher<TInput, TOutput>> elementMatchers, Func<IList<TOutput>, TAggregate> aggregator) => (ElementMatchers, Aggregator) = (new(elementMatchers?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher<TInput, TOutput>>()), aggregator);
    
    public bool Match(ReadOnlySpan<TInput> span, int startIndex, int endIndex, out int nextIndex)
    {
        if (span.ValidateExtentsIsEmpty(ref startIndex, ref endIndex))
        {
            nextIndex = startIndex;
            return false;
        }
        nextIndex = startIndex;
        foreach (IMatcher<TInput, TOutput> matcher in ElementMatchers)
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
        Collection<TOutput> items = new();
        nextIndex = startIndex;
        foreach (IMatcher<TInput, TOutput> matcher in ElementMatchers)
        {
            if (!matcher.TryParse(span, nextIndex, endIndex, out TOutput? item, out nextIndex))
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
}
