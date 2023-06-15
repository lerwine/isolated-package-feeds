namespace CdnGetter.Parsing;

public sealed partial class ParsingContext<TInput, TOutput>
{
    class FilterNode
    {
        private readonly FilterNode? _previous;
        private readonly IParser<TInput, TOutput> _parser;
        private FilterNode? _next;

        private FilterNode(IParser<TInput, TOutput> parser, FilterNode? previous = null)
        {
            _parser = parser;
            if ((_previous = previous) is not null)
                _previous._next = this;
        }

        internal static void ApplyFilter(ParsingContext<TInput, TOutput> parsingContext)
        {
            if (parsingContext._filter is IParserFilter<TInput, TOutput> filter)
            {
                using IEnumerator<IParser<TInput, TOutput>> enumerator = ((filter.Sort is null) ? parsingContext.AllParsers.Where(filter.Predicate!) :
                    (filter.Predicate is null) ? filter.Sort(parsingContext.AllParsers) : filter.Sort(parsingContext.AllParsers.Where(filter.Predicate))).GetEnumerator();
                if (!enumerator.MoveNext())
                    throw new InvalidOperationException("Filter allowed no parsers.");
                parsingContext._count = 1;
                parsingContext._firstActive = parsingContext._lastActive = new(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    parsingContext._lastActive = new(enumerator.Current, parsingContext._lastActive);
                    parsingContext._count++;
                }
            }
            else
            {
                using IEnumerator<IParser<TInput, TOutput>> enumerator = parsingContext.AllParsers.GetEnumerator();
                enumerator.MoveNext();
                parsingContext._count = 1;
                parsingContext._firstActive = parsingContext._lastActive = new(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    parsingContext._lastActive = new(enumerator.Current, parsingContext._lastActive);
                    parsingContext._count++;
                }
            }
        }

        internal static IEnumerable<IParser<TInput, TOutput>> GetFilters(ParsingContext<TInput, TOutput> parsingContext)
        {
            for (FilterNode? node = parsingContext._firstActive; node is not null; node = node._next)
                yield return node._parser;
        }
    }
}
