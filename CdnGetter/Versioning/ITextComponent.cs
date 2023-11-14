namespace CdnGetter.Versioning
{
    public interface ITextComponent : IEquatable<ITextComponent>, IEquatable<string>, IEquatable<char>, IComparable<ITextComponent>, IComparable<string>, IComparable<char>, IComparable, IReadOnlyList<char>, IVersionComponent
    {
        int Length { get; }
        char First();
        char Last();
    }
}