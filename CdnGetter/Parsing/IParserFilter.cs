namespace CdnGetter.Parsing;

public interface IParserFilter<TInput, TOutput>
{
    Func<IParser<TInput, TOutput>, bool>? Predicate { get; }

    Func<IEnumerable<IParser<TInput, TOutput>>, IEnumerable<IParser<TInput, TOutput>>>? Sort { get; }
}
