namespace CdnGetter.Parsing;

public interface ISoftwareVersion
{
    IToken? Prefix { get; }

    INumericalToken Major { get; }

    INumericalToken? Minor { get; }

    INumericalToken? Patch { get; }

    IEnumerable<INumericalToken> Micro { get; }

    IEnumerable<IToken> PreRelease { get; }

    IEnumerable<IToken> Build { get; }
}