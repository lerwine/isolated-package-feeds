namespace CdnGetter;

public partial class ParsedUri
{
    public class QuerySubComponent : IEquatable<QuerySubComponent>, IComparable<QuerySubComponent>
    {
        public char? Separator { get; }

        public string Key { get; }
        
        public string? Value { get; }

        public QuerySubComponent(char separator, string key, string? value = null) => (Separator, Key, Value) = (separator, key ?? "", value);

        public QuerySubComponent(string key, string? value = null) => (Key, Value) = (key ?? "", value);

        public static IEnumerable<QuerySubComponent> Parse(string queryString)
        {
            throw new NotImplementedException();
        }

        public bool Equals(QuerySubComponent? other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object? obj) => Equals(obj as QuerySubComponent);

        public int CompareTo(QuerySubComponent? other)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
