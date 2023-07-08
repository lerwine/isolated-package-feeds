namespace CdnGetter.Parsing.Version;

public interface IDelimitedToken : ITokenList
{
    IStringToken DelimiterToken { get; }

    IToken ValueToken { get; }
}
