namespace CdnGetter.Versioning
{
    public interface IDelimitedVersionNumber : IVersionNumber
    {
        ITextComponent? Delimiter { get; }
    }
}