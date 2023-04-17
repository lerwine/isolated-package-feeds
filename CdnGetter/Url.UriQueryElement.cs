using System.Diagnostics.CodeAnalysis;

namespace CdnGetter;

public partial class Url
{
    public struct UriQueryElement : IEquatable<UriQueryElement>, IComparable<UriQueryElement>
    {
        public string Key { get; private set; }
        
        public string? Value { get; private set; }
        
        public UriQueryElement(string key, string? value)
        {
            Key = key ?? "";
            Value = value;
        }

        public bool Equals(UriQueryElement other) => Comparer.Equals(Key, other.Key) && ((Value is null) ? other.Value is null : other.Value is not null && Comparer.Equals(Value, other.Value));

        public override bool Equals([NotNullWhen(true)] object? obj) => obj is UriQueryElement other && Equals(other);

        public int CompareTo(UriQueryElement other)
        {
            int result = Comparer.Compare(Key, other.Key);
            return (result != 0) ? result : (Value is null) ? ((other.Value is null) ? 0 : -1) : (other.Value is null) ? 1 : Comparer.Compare(Value, other.Value);
        }

        public override int GetHashCode()
        {
            int hashCode = 3;
            unchecked
            {
                hashCode = (hashCode * 7) + Key.GetHashCode();
                return (Value is null) ? hashCode : (hashCode * 7) + Value.GetHashCode();
            }
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
