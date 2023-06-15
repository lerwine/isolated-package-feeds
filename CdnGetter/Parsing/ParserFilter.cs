namespace CdnGetter.Parsing;

public class ParserFilter<TInput, TOutput> : IParserFilter<TInput, TOutput>
{
    public Func<IParser<TInput, TOutput>, bool>? Predicate { get; }

    public Func<IEnumerable<IParser<TInput, TOutput>>, IEnumerable<IParser<TInput, TOutput>>>? Sort { get; }

    public ParserFilter(Func<IParser<TInput, TOutput>, bool> predicate, Func<IEnumerable<IParser<TInput, TOutput>>, IEnumerable<IParser<TInput, TOutput>>>? sort = null) =>
        (Predicate, Sort) = (predicate ?? throw new ArgumentNullException(nameof(predicate)), sort);

    public ParserFilter(Func<IEnumerable<IParser<TInput, TOutput>>, IEnumerable<IParser<TInput, TOutput>>> sort) => (Predicate, Sort) = (null, sort ?? throw new ArgumentNullException(nameof(sort)));
}

public class ParserFilter<TInput, TOutput, TKey> : IParserFilter<TInput, TOutput>
{
    public Func<IParser<TInput, TOutput>, bool>? Predicate { get; }

    public Func<IParser<TInput, TOutput>, TKey> KeySelector { get; }

    public IComparer<TKey> Comparer { get; }

    Func<IEnumerable<IParser<TInput, TOutput>>, IEnumerable<IParser<TInput, TOutput>>>? IParserFilter<TInput, TOutput>.Sort => Sort;

    public ParserFilter(Func<IParser<TInput, TOutput>, TKey> keySelector, IComparer<TKey>? comparer = null, Func<IParser<TInput, TOutput>, bool>? predicate = null) => (KeySelector, Comparer, Predicate) = (keySelector, comparer ?? Comparer<TKey>.Default, predicate);

    public ParserFilter(Func<IParser<TInput, TOutput>, TKey> keySelector, Func<IParser<TInput, TOutput>, bool> predicate) => (KeySelector, Comparer, Predicate) = (keySelector, Comparer<TKey>.Default, predicate);

    public IEnumerable<IParser<TInput, TOutput>> Sort(IEnumerable<IParser<TInput, TOutput>> source) => source.OrderBy(KeySelector, Comparer);
}
