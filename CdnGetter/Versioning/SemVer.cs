namespace CdnGetter.Versioning;

public class SemVer
{
    ITextComponent? Prefix { get; }

    IVersionNumber Major { get; }

    DelimitedVersionNumber? Minor { get; }

    DelimitedVersionNumber? Patch { get; }

    IList<DelimitedVersionNumber> Micro { get; }

    
}