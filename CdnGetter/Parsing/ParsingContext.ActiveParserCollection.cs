using System.Collections;

namespace CdnGetter.Parsing;

public sealed partial class ParsingContext<TInput, TOutput>
{
    class ActiveParserCollection : IReadOnlyCollection<IParser<TInput, TOutput>>
    {
        private readonly ParsingContext<TInput, TOutput> _source;

        internal ActiveParserCollection(ParsingContext<TInput, TOutput> source) => _source = source;

        public int Count => _source._count;

        public IEnumerator<IParser<TInput, TOutput>> GetEnumerator() => FilterNode.GetFilters(_source).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)FilterNode.GetFilters(_source)).GetEnumerator();
    }
}
