namespace CdnGetter.Parsing.Version;

public interface ISoftwareVersion : ITokenList
{
    IToken? Prefix { get; }

    IToken Major { get; }

    IDelimitedToken? Minor { get; }

    IDelimitedToken? Patch { get; }

    IDelimitedTokenList Micro { get; }

    IDelimitedTokenList PreRelease { get; }

    IDelimitedTokenList Build { get; }
}
