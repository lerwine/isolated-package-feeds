namespace CdnGetter.Parsing;

/// <summary>
/// A compound token comprised of a sequence of numerical tokens.
/// </summary>
public interface INumericalTokenList : ITokenList, IReadOnlyList<INumericalToken> { }
