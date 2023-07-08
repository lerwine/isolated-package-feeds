namespace CdnGetter.Parsing.Version;

public interface IDelimitedTokenList : ITokenList, IEnumerable<IDelimitedToken> {
    new IDelimitedToken this[int index] { get; }
}
