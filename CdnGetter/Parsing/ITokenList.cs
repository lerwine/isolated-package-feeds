namespace CdnGetter.Parsing;

/// <summary>
/// A compound token comprised of a sequence of tokens.
/// </summary>
public interface ITokenList : IToken, IReadOnlyList<IToken> { }
