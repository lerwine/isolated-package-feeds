using System.Collections.ObjectModel;

namespace CdnGetter.Parsing;

public sealed partial class ParsingContext<TInput, TOutput>
{
    private readonly object _syncRoot = new();
    private FilterNode _firstActive = null!;
    private FilterNode _lastActive = null!;
    private int _count;
    private IParserFilter<TInput, TOutput>? _filter;
    private readonly LinkedList<Range> _skippedRanges = new();

    public ReadOnlyCollection<IParser<TInput, TOutput>> AllParsers { get; }

    public IReadOnlyCollection<IParser<TInput, TOutput>> ActiveParsers { get; }
    
    public int StartIndex { get; }

    public int CurrentIndex { get; private set; }

    public int EndIndex { get; }

    private ParsingContext(int startIndex, int endIndex, IEnumerable<IParser<TInput, TOutput>> allParsers, IParserFilter<TInput, TOutput>? initialParserFilter = null)
    {
        StartIndex = startIndex;
        EndIndex = endIndex;
        if (allParsers is null || (AllParsers = new(allParsers.Where(i => i is not null).ToArray())).Count == 0)
            throw new InvalidOperationException("No parsers defined.");
        _filter = initialParserFilter;
        FilterNode.ApplyFilter(this);
        ActiveParsers = new ActiveParserCollection(this);
    }

    public void FilterParsers(IParserFilter<TInput, TOutput>? filter)
    {
        lock (_syncRoot)
        {
            if ((_filter is null) ? filter is not null : filter is null || !ReferenceEquals(_filter, filter))
            {
                _filter = filter;
                FilterNode.ApplyFilter(this);
            }
        }
    }
    
    private IEnumerable<TOutput> Parse(ReadOnlySpan<TInput> source)
    {
        void addSkipped(int index)
        {
            if (_skippedRanges.Last?.Value is Range range && range.End.Value == CurrentIndex)
            {
                range = new(range.Start, new Index(index));
                _skippedRanges.RemoveLast();
                _skippedRanges.AddLast(range);
            }
            else
                _skippedRanges.AddLast(new Range(CurrentIndex, index));
            CurrentIndex = index;
        }
        do
        {
            int previousIndex = CurrentIndex;
            foreach (IParser<TInput, TOutput> filter in FilterNode.GetFilters(this))
            {
                if (filter.TryParse(source, this, out TOutput? result, out int nextIndex))
                {
                    if (nextIndex > EndIndex)
                        CurrentIndex = EndIndex;
                    else if (nextIndex > CurrentIndex)
                        CurrentIndex = nextIndex;
                    else
                        CurrentIndex++;
                    yield return result;
                    break;
                }
                if (nextIndex > EndIndex)
                {
                    if (CurrentIndex < EndIndex)
                        addSkipped(EndIndex);
                }
                else if (nextIndex > CurrentIndex)
                    addSkipped(nextIndex);
                    CurrentIndex = nextIndex;
            }
            if (previousIndex == CurrentIndex)
                    addSkipped(CurrentIndex + 1);
        }
        while (EndIndex > CurrentIndex);
    }
    
    public static IEnumerable<TOutput> Parse(ReadOnlySpan<TInput> source, int startIndex, int endIndex, IParserFilter<TInput, TOutput> initialParserFilter, IEnumerable<IParser<TInput, TOutput>> parsers, out ICollection<Range> skippedRanges)
    {
        if (source.IsEmpty || endIndex <= startIndex || startIndex >= source.Length || endIndex < 1)
        {
            skippedRanges = Array.Empty<Range>();
            return Enumerable.Empty<TOutput>();
        }
         ParsingContext<TInput, TOutput> parsingContext = new((startIndex < 0) ? 0 : startIndex, (endIndex > source.Length) ? source.Length : endIndex, parsers, initialParserFilter);
        IEnumerable<TOutput> results = parsingContext.Parse(source);
        skippedRanges = parsingContext._skippedRanges;
        return results;
    }
    
    public static (IEnumerable<TOutput> Results, ICollection<Range> SkippedRanges) Parse(ReadOnlySpan<TInput> source, int startIndex, int endIndex, IParserFilter<TInput, TOutput> initialParserFilter, params IParser<TInput, TOutput>[] parsers)
    {
        if (source.IsEmpty || endIndex <= startIndex || startIndex >= source.Length || endIndex < 1)
            return (Enumerable.Empty<TOutput>(), Array.Empty<Range>());
         ParsingContext<TInput, TOutput> parsingContext = new((startIndex < 0) ? 0 : startIndex, (endIndex > source.Length) ? source.Length : endIndex, parsers, initialParserFilter);
        return (parsingContext.Parse(source), parsingContext._skippedRanges);
    }
    
    public static IEnumerable<TOutput> Parse(ReadOnlySpan<TInput> source, int startIndex, int endIndex, IEnumerable<IParser<TInput, TOutput>> parsers, out ICollection<Range> skippedRanges)
    {
        if (source.IsEmpty || endIndex <= startIndex || startIndex >= source.Length || endIndex < 1)
        {
            skippedRanges = Array.Empty<Range>();
            return Enumerable.Empty<TOutput>();
        }
        ParsingContext<TInput, TOutput> parsingContext = new((startIndex < 0) ? 0 : startIndex, (endIndex > source.Length) ? source.Length : endIndex, parsers);
        IEnumerable<TOutput> results = parsingContext.Parse(source);
        skippedRanges = parsingContext._skippedRanges;
        return results;
    }
    
    public static (IEnumerable<TOutput> Results, ICollection<Range> SkippedRanges) Parse(ReadOnlySpan<TInput> source, int startIndex, int endIndex, params IParser<TInput, TOutput>[] parsers)
    {
        if (source.IsEmpty || endIndex <= startIndex || startIndex >= source.Length || endIndex < 1)
            return (Enumerable.Empty<TOutput>(), Array.Empty<Range>());
        ParsingContext<TInput, TOutput> parsingContext = new((startIndex < 0) ? 0 : startIndex, (endIndex > source.Length) ? source.Length : endIndex, parsers);
        return (parsingContext.Parse(source), parsingContext._skippedRanges);
    }
}
