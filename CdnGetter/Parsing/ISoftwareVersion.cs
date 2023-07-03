namespace CdnGetter.Parsing;

public interface ISoftwareVersion : ITokenList
{
    IToken? Prefix { get; }

    IToken Major { get; }

    IToken? Minor { get; }

    IToken? Patch { get; }

    ITokenList Micro { get; }

    ITokenList PreRelease { get; }

    ITokenList Build { get; }
}
