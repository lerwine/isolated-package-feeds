using System.Text;
using System.Text.RegularExpressions;

namespace CdnGetter;

public partial class ParsedUri
{
    /// <summary>
    /// Represents a URI query sub-component.
    /// </summary>
    public class QuerySubComponent : IEquatable<QuerySubComponent>, IComparable<QuerySubComponent>
    {
        /// <summary>
        /// Gets the preceding query sub-component separator character.
        /// </summary>
        /// <value>The character that separates the current sub-component from the previous one or <see langword="null" /> if this is the first query sub-component.</value>
        public char? Separator { get; }

        /// <summary>
        /// Gets the key for the query sub-component.
        /// </summary>
        /// <value>The key for the query sub-componet or <see langword="null" /> if the component is value-only.</value>
        public string? Key { get; }
        
        /// <summary>
        /// Gets the value for the query sub-component.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Creates a new subsequent <c>QuerySubComponent</c> object.
        /// </summary>
        /// <param name="separator">The character that separates the new sub-component with the preceding one.</param>
        /// <param name="key">The key for the query parameter.</param>
        /// <param name="value">The value of the query parameter.</param>
        public QuerySubComponent(char separator, string key, string value) => (Separator, Key, Value) = (separator, key ?? "", value ?? "");

        /// <summary>
        /// Creates a new <c>QuerySubComponent</c> object.
        /// </summary>
        /// <param name="separator">The character that separates the new sub-component with the preceding one.</param>
        /// <param name="value">The value of the query sub-component.</param>
        public QuerySubComponent(char separator, string value) => (Separator, Value) = (separator, value);

        /// <summary>
        /// Creates a new leading <c>QuerySubComponent</c> object.
        /// </summary>
        /// <param name="key">The key for the query parameter.</param>
        /// <param name="value">The value of the query parameter.</param>
        public QuerySubComponent(string key, string value) => (Key, Value) = (key ?? "", value ?? "");
        
        /// <summary>
        /// Creates a new leading <c>QuerySubComponent</c> object.
        /// </summary>
        /// <param name="value">The value of the query sub-component.</param>
        public QuerySubComponent(string value) => Value = value;
        
        /// <summary>
        /// Tries to parse query sub-components from a query string.
        /// </summary>
        /// <param name="queryString">A string representing a URI query component.</param>
        /// <param name="options">Normalization options.</param>
        /// <param name="normalizeQuerySeparators"><see langword="true" /> if query sub-component separator characters should be normalized as <c>'&'</c>;
        /// otherwise, <see langword="false" /> to leave the separator character as-is.</param>
        /// <returns>The parsed query sub-components.</returns>
        public static IEnumerable<QuerySubComponent>? Parse(string queryString, NormalizationOptions options, bool normalizeQuerySeparators = false)
        {
            if (string.IsNullOrEmpty(queryString))
                return options.HasFlag(NormalizationOptions.StripEmptyQuery) ? null : Enumerable.Empty<QuerySubComponent>();
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
                                new QuerySubComponent(UriQueryDecode(kvp));
                        default:
                            int index = kvp.IndexOf(DELIMITER_CHAR_EQUALS);
                            if (index < 0)
                                return new QuerySubComponent(UriQueryDecode(kvp));
                            if (index == 0)
                                return new QuerySubComponent(string.Empty, UriQueryDecode(kvp[1..]));
                            return new QuerySubComponent(UriQueryDecode(kvp[..index]), (index < kvp.Length - 1) ? UriQueryDecode(kvp[(index + 1)..]) : string.Empty);
                    }
                }).Concat(kvpItems.Skip(1).Select(kvp =>
                {
                    switch (kvp.Length)
                    {
                        case 0:
                            return new QuerySubComponent(DELIMITER_CHAR_AMPERSAND, string.Empty);
                        case 1:
                            return (kvp[0] == DELIMITER_CHAR_EQUALS) ? new QuerySubComponent(DELIMITER_CHAR_AMPERSAND, string.Empty, string.Empty) :
                                new QuerySubComponent(DELIMITER_CHAR_AMPERSAND, UriQueryDecode(kvp));
                        default:
                            int index = kvp.IndexOf(DELIMITER_CHAR_EQUALS);
                            if (index < 0)
                                return new QuerySubComponent(DELIMITER_CHAR_AMPERSAND, UriQueryDecode(kvp));
                            if (index == 0)
                                return new QuerySubComponent(DELIMITER_CHAR_AMPERSAND, string.Empty, UriQueryDecode(kvp[1..]));
                            return new QuerySubComponent(DELIMITER_CHAR_AMPERSAND, UriQueryDecode(kvp[..index]), (index < kvp.Length - 1) ? UriQueryDecode(kvp[(index + 1)..]) : string.Empty);
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
                                    return new QuerySubComponent(c, UriQueryDecode(g.Value));
                                if (index == 0)
                                    return new QuerySubComponent(c, string.Empty, UriQueryDecode(g.Value[1..]));
                                return new QuerySubComponent(c, UriQueryDecode(g.Value[..index]), (index < g.Length - 1) ? UriQueryDecode(g.Value[(index + 1)..]) : string.Empty);
                        }
                    }
                    return new QuerySubComponent(c, string.Empty);
                }
                if (m.Length == 1)
                    return (m.Value[0] == DELIMITER_CHAR_EQUALS) ? new QuerySubComponent(string.Empty, string.Empty) : new QuerySubComponent(m.Value);
                if ((index = g.Value.IndexOf(DELIMITER_CHAR_EQUALS)) < 0)
                    return new QuerySubComponent(UriQueryDecode(g.Value));
                if (index == 0)
                    return new QuerySubComponent(string.Empty, UriQueryDecode(g.Value[1..]));
                return new QuerySubComponent(UriQueryDecode(g.Value[..index]), (index < g.Length - 1) ? UriQueryDecode(g.Value[(index + 1)..]) : string.Empty);
            });
        }

        public bool Equals(QuerySubComponent? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (Separator.HasValue)
            {
                if (!(other.Separator.HasValue && Separator.Value.Equals(other.Separator.Value)))
                    return false;
            }
            else if (other.Separator.HasValue)
                return false;
            if (Key is null)
            {
                if (other.Key is not null)
                    return false;
            }
            else if (other.Key is null || !DefaultComponentComparer.Equals(Key, other.Key))
                return false;
            return DefaultComponentComparer.Equals(Value, other.Value);
        }

        public override bool Equals(object? obj) => Equals(obj as QuerySubComponent);

        public int CompareTo(QuerySubComponent? other)
        {
            if (other is null)
                return 1;
            if (ReferenceEquals(this, other))
                return 0;
            int result;
            if (Separator.HasValue)
            {
                if (!other.Separator.HasValue)
                    return 1;
                if ((result = Separator.Value.CompareTo(other.Separator.Value)) != 0)
                    return result;
            }
            else if (other.Separator.HasValue)
                return -1;
            if (Key is null)
            {
                if (other.Key is not null)
                    return -1;
            }
            else
            {
                if (other.Key is null)
                    return 1;
                if ((result = DefaultComponentComparer.Compare(Key, other.Key)) != 0)
                    return result;
            }
            return DefaultComponentComparer.Compare(Value, other.Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Separator.HasValue ? 55 + Separator.Value.GetHashCode() : 5;
                return ((Key is null) ? hash : hash * 11 + DefaultComponentComparer.GetHashCode(Key)) * 11 + DefaultComponentComparer.GetHashCode(Value);
            }
        }

        public override string ToString()
        {
            if (Separator.HasValue)
            {
                if (Key is null)
                {
                    if (Value.Length > 0)
                        return $"{Separator.Value}{EncodeQueryKey(Value, QueryKeyEncodeRegex)}";
                    return Separator.Value.ToString();
                }
                if (Key.Length > 0)
                    return (Value.Length > 0) ? $"{Separator.Value}{EncodeQueryKey(Key, QueryKeyEncodeRegex)}={EncodeQueryValue(Value, QueryValueEncodeRegex)}" : $"{Separator.Value}{EncodeQueryKey(Key, QueryKeyEncodeRegex)}=";
                return (Value.Length > 0) ? $"{Separator.Value}={EncodeQueryValue(Value, QueryValueEncodeRegex)}" : $"{Separator.Value}=";
            }
            
            if (Key is null)
            {
                if (Value.Length > 0)
                    return EncodeQueryKey(Value, QueryKeyEncodeRegex);
                return string.Empty;
            }
            if (Key.Length > 0)
                return (Value.Length > 0) ? $"{EncodeQueryKey(Key, QueryKeyEncodeRegex)}={EncodeQueryValue(Value, QueryValueEncodeRegex)}" : $"{EncodeQueryKey(Key, QueryKeyEncodeRegex)}=";
            return (Value.Length > 0) ? $"={EncodeQueryValue(Value, QueryValueEncodeRegex)}" : "=";
        }

        internal void AppendTo(StringBuilder sb)
        {
            if (Separator.HasValue)
                sb.Append(Separator.Value);
            if (Key is not null)
            {
                if (Key.Length > 0)
                    sb.Append(EncodeQueryKey(Key, QueryKeyEncodeRegex));
                sb.Append('=');
            }
            if (Value.Length > 0)
                sb.Append(EncodeQueryValue(Value, QueryValueEncodeRegex));
        }
    }
}
