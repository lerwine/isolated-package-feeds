namespace CdnGetter.Versioning
{
    public interface ITextComponent : IEquatable<ITextComponent>, IEquatable<string>, IEquatable<char>, IComparable<ITextComponent>, IComparable<string>, IComparable<char>, IComparable
    {
        int Length { get; }

        IEnumerable<char> GetChars();
    }
}