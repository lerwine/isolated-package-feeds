namespace CdnGetter.Versioning
{
    public interface IDelimitedVersionComponent
    {
        ITextComponent Delimiter { get; }
        IVersionComponent Value { get; }
    }
}