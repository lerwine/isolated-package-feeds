using System.Diagnostics.CodeAnalysis;

namespace CdnGetter;

public partial class Url
{
    public readonly struct UriQueryElement : IEquatable<UriQueryElement>, IComparable<UriQueryElement>
    {
        public string Key { get; }
        
        public string? Value { get; }
        
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
                hashCode = (hashCode * 7) + Comparer.GetHashCode(Key);
                return (Value is null) ? hashCode : (hashCode * 7) + Comparer.GetHashCode(Value);
            }
        }

        public override string ToString()
        {
            if (Key.Length > 0)
            {
                if (Value is null)
                    return QueryKeyEncodeRegex.Replace(Key, m => Uri.HexEscape(m.Value[0]));
                if (Value.Length > 0)
                    return $"{QueryKeyEncodeRegex.Replace(Key, m => Uri.HexEscape(m.Value[0]))}={QueryValueEncodeRegex.Replace(Value, m => Uri.HexEscape(m.Value[0]))}";
                return $"{QueryKeyEncodeRegex.Replace(Key, m => Uri.HexEscape(m.Value[0]))}=";
            }
            if (Value is null)
                return string.Empty;
            if (Value.Length > 0)
                return $"={QueryValueEncodeRegex.Replace(Value, m => Uri.HexEscape(m.Value[0]))}";
            return "=";
        }

        public static bool operator ==(UriQueryElement left, UriQueryElement right) => left.Equals(right);

        public static bool operator !=(UriQueryElement left, UriQueryElement right) => !(left == right);

        public static bool operator <(UriQueryElement left, UriQueryElement right) => left.CompareTo(right) < 0;

        public static bool operator <=(UriQueryElement left, UriQueryElement right) => left.CompareTo(right) <= 0;

        public static bool operator >(UriQueryElement left, UriQueryElement right) => left.CompareTo(right) > 0;

        public static bool operator >=(UriQueryElement left, UriQueryElement right) => left.CompareTo(right) >= 0;
    }
}
