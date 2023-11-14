namespace CdnGetter.Versioning
{
    public interface IVersionComponent : IEquatable<IVersionComponent>, IComparable<IVersionComponent>, IComparable, IEnumerable<char>
    {
    }
}