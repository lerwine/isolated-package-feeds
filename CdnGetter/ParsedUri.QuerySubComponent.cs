using System.Text.RegularExpressions;

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

        public static IEnumerable<QuerySubComponent> Parse(string queryString, bool normalizeQuerySeparators = false)
        {
            if (string.IsNullOrEmpty(queryString))
                return Enumerable.Empty<QuerySubComponent>();
            if (normalizeQuerySeparators)
            {
                string[] kvpItems = QueryDelimiterRegex.Split(queryString);
                return kvpItems.Take(1).Select(kvp =>
                {
                    switch (kvp.Length)
                    {
                        case 0:
                            return new QuerySubComponent(string.Empty);
                        case 1:
                            return (kvp[0] == DELIMITER_CHAR_EQUALS) ? new QuerySubComponent(string.Empty, string.Empty) :
                                new QuerySubComponent(UriDecode(kvp), null);
                        default:
                            int index = kvp.IndexOf(DELIMITER_CHAR_EQUALS);
                            if (index < 0)
                                return new QuerySubComponent(UriDecode(kvp), null);
                            if (index == 0)
                                return new QuerySubComponent(string.Empty, UriDecode(kvp[1..]));
                            return new QuerySubComponent(UriDecode(kvp[..index]), (index < kvp.Length - 1) ? UriDecode(kvp[(index + 1)..]) : string.Empty);
                    }
                }).Concat(kvpItems.Skip(1).Select(kvp =>
                {
                    switch (kvp.Length)
                    {
                        case 0:
                            return new QuerySubComponent(DELIMITER_CHAR_AMPERSAND, string.Empty);
                        case 1:
                            return (kvp[0] == DELIMITER_CHAR_EQUALS) ? new QuerySubComponent(DELIMITER_CHAR_AMPERSAND, string.Empty, string.Empty) :
                                new QuerySubComponent(DELIMITER_CHAR_AMPERSAND, UriDecode(kvp), null);
                        default:
                            int index = kvp.IndexOf(DELIMITER_CHAR_EQUALS);
                            if (index < 0)
                                return new QuerySubComponent(DELIMITER_CHAR_AMPERSAND, UriDecode(kvp), null);
                            if (index == 0)
                                return new QuerySubComponent(DELIMITER_CHAR_AMPERSAND, string.Empty, UriDecode(kvp[1..]));
                            return new QuerySubComponent(DELIMITER_CHAR_AMPERSAND, UriDecode(kvp[..index]), (index < kvp.Length - 1) ? UriDecode(kvp[(index + 1)..]) : string.Empty);
                    }
                }));
            }
            return QuerySubcomponentRegex.Matches(queryString).Select(m =>
            {
                int index;
                Group g = m.Groups[GROUP_NAME_sep];
                if (g.Success)
                {
                    char c = g.Value[0];
                    if ((g = m.Groups[GROUP_NAME_sub]).Success)
                    {
                        switch (g.Length)
                        {
                            case 0:
                                return new QuerySubComponent(c, string.Empty);
                            case 1:
                                return (g.Value[0] == DELIMITER_CHAR_EQUALS) ? new QuerySubComponent(c, string.Empty, string.Empty) : new QuerySubComponent(c, g.Value);
                            default:
                                if ((index = g.Value.IndexOf(DELIMITER_CHAR_EQUALS)) < 0)
                                    return new QuerySubComponent(c, UriDecode(g.Value), null);
                                if (index == 0)
                                    return new QuerySubComponent(c, string.Empty, UriDecode(g.Value[1..]));
                                return new QuerySubComponent(c, UriDecode(g.Value[..index]), (index < g.Length - 1) ? UriDecode(g.Value[(index + 1)..]) : string.Empty);
                        }
                    }
                    return new QuerySubComponent(c, string.Empty);
                }
                if (m.Length == 1)
                    return (m.Value[0] == DELIMITER_CHAR_EQUALS) ? new QuerySubComponent(string.Empty, string.Empty) : new QuerySubComponent(m.Value);
                if ((index = g.Value.IndexOf(DELIMITER_CHAR_EQUALS)) < 0)
                    return new QuerySubComponent(UriDecode(g.Value), null);
                if (index == 0)
                    return new QuerySubComponent(string.Empty, UriDecode(g.Value[1..]));
                return new QuerySubComponent(UriDecode(g.Value[..index]), (index < g.Length - 1) ? UriDecode(g.Value[(index + 1)..]) : string.Empty);
            });
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
