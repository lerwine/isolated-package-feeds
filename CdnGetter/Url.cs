using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace CdnGetter;

public partial class Url : IEquatable<Url>, IComparable<Url>
{
    public static readonly StringComparer Comparer = StringComparer.InvariantCultureIgnoreCase;

    public const string PRIMARY_PATH_SEPARATOR = "/";

    public const string QUERY_LEAD_CHAR = "?";

    public const string QUERY_SEPARATOR = "&";

    public const string FRAGMENT_SEPARATOR = "#";

    // public const string REGEX_PATTERN_DIGIT = @"\d";
    // public const string REGEX_PATTERN_HEXDIG = @"[\da-f]";
    // public const string REGEX_PATTERN_ALPHA = @"[a-z]";
    // public const string REGEX_PATTERN_PCT_ENCODED = @"%[\da-f]{2}";
    // public const string REGEX_PATTERN_GEN_DELIM = @"[:/?#\[\]@]";
    // public const string REGEX_PATTERN_SUB_DELIM = @"[!$&'()*+,;=]";
    // public const string REGEX_PATTERN_RESERVED = @"[:/?#\[\]@!$&'()*+,;=]";
    // public const string REGEX_PATTERN_UNRESERVED = @"[\w.~-]";
    
    /*
URI         = scheme ":" hier-part [ "?" query ] [ "#" fragment ]

authority   = [ userinfo "@" ] host [ ":" port ]
   userinfo    = *( unreserved / pct-encoded / sub-delims / ":" )
   host        = IP-literal / IPv4address / reg-name
       IPv6address =                            6( h16 ":" ) ls32
                   /                       "::" 5( h16 ":" ) ls32
                   / [               h16 ] "::" 4( h16 ":" ) ls32
                   / [ *1( h16 ":" ) h16 ] "::" 3( h16 ":" ) ls32
                   / [ *2( h16 ":" ) h16 ] "::" 2( h16 ":" ) ls32
                   / [ *3( h16 ":" ) h16 ] "::"    h16 ":"   ls32
                   / [ *4( h16 ":" ) h16 ] "::"              ls32
                   / [ *5( h16 ":" ) h16 ] "::"              h16
                   / [ *6( h16 ":" ) h16 ] "::"
           ls32        = ( h16 ":" h16 ) / IPv4address
                       ; least-significant 32 bits of address
           h16         = 1*4HEXDIG
                       ; 16 bits of address represented in hexadecimal
       IPvFuture   = "v" 1*HEXDIG "." 1*( unreserved / sub-delims / ":" )
       IP-literal  = "[" ( IPv6address / IPvFuture  ) "]"
       IPv4address = dec-octet "." dec-octet "." dec-octet "." dec-octet
           dec-octet   = DIGIT                 ; 0-9
                       / %x31-39 DIGIT         ; 10-99
                       / "1" 2DIGIT            ; 100-199
                       / "2" %x30-34 DIGIT     ; 200-249
                       / "25" %x30-35          ; 250-255
   port        = *DIGIT
       reg-name    = *( unreserved / pct-encoded / sub-delims )

hier-part   = "//" authority path-abempty
           / path-absolute
           / path-rootless
           / path-empty

path          = path-abempty    ; begins with "/" or is empty
               / path-absolute   ; begins with "/" but not "//"
               / path-noscheme   ; begins with a non-colon segment
               / path-rootless   ; begins with a segment
               / path-empty      ; zero characters

   path-abempty  = *( "/" segment )
   path-absolute = "/" [ segment-nz *( "/" segment ) ]
   path-noscheme = segment-nz-nc *( "/" segment )
   path-rootless = segment-nz *( "/" segment )
   path-empty    = 0<pchar>
       segment       = *pchar
       segment-nz    = 1*pchar
       segment-nz-nc = 1*( unreserved / pct-encoded / sub-delims / "@" )
                       ; non-zero-length segment without any colon ":"
           pchar         = unreserved / pct-encoded / sub-delims / ":" / "@"

query       = *( pchar / "/" / "?" )
fragment    = *( pchar / "/" / "?" )
*/


    /// <summary>
    /// Matches a leading path separator character.
    /// </summary>
    public static readonly Regex PathRootedRegex = new(@"^[\\/]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Matches a path separator character.
    /// </summary>
    public static readonly Regex PathSeparatorRegex = new(@"[\\/]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    public static readonly Regex PathSegmentEncodeRegex = new(@"[^!$&'()*+,:;@=\w.~-]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    public static readonly Regex QueryKeyEncodeRegex = new(@"[^!$'()*+,:;@/\w.~-]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    public static readonly Regex QueryValueEncodeRegex = new(@"[^!$'()*+,:;@=/\w.~-]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    public static readonly Regex FragmentEEncodeRegex = new(@"[^!$&'()*+,:;@=/?\w.~-]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    /// <summary>
    /// Matches a URI-encoded character sequence.
    /// </summary>
    public static readonly Regex EncodedSequenceRegex = new(@"%[\da-f]{2}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a URI scheme name.
    /// </summary>
    public static readonly Regex ValidSchemeRegex = new(@"^[a-z][a-z\d+.-]*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a Host or UserName character that should be encoded.
    /// </summary>
    public static readonly Regex NameEncodeRegex = new(@"[^!$&'()*+,;=\w.~-]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a Host or UserName character that is not properly encoded.
    /// </summary>
    public static readonly Regex NameEncodeRegexAlt = new(@"%(?![\da-f]{2})|[^%!$&'()*+,;=\w.~-]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a Pasword character that should be encoded.
    /// </summary>
    public static readonly Regex PasswordEncodeRegex = new(@"[^!$&'()*+,;=\w.~:-]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a password character that is not properly encoded.
    /// </summary>
    public static readonly Regex PasswordEncodeRegexAlt = new(@"%(?![\da-f]{2})|[^%!$&'()*+,;=\w.~:-]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a valid URI-encoded user info component.
    /// </summary>
    public static readonly Regex UserInfoRegex = new(@"(?<username>([!$&'()*+,;=\w.~-]|%[\da-f]{2})*)(?::(?<pw>([!$&'()*+,;=\w.~:-]|%[\da-f]{2})*))?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a valid IPv4 address.
    /// </summary>
    public static readonly Regex ValidIpV4Regex = new(@"^(([01](\d\d?)?|2([0-4]\d?|5[0-5]?|[6-9])?|[3-9]\d?)\.){3}([01](\d\d?)?|2([0-4]\d?|5[0-5]?|[6-9])?|[3-9]\d?)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a valid IPv6 address that is surrounded by square brackets.
    /// </summary>
    public static readonly Regex ValidUriIpV6Regex = new(@"^\[(:((:[\da-f]{1,4}){1,7}|:)|(?=([^:]+:)+((:[^:]+)+|:)$)([\da-f]{1,4}::?){1,6}(:|[\da-f]{0,4}(::)?)|([\da-f]{1,4}:){7}[\da-f]{1,4})\]$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
    
    /// <summary>
    /// Matches a valid IPv6 address.
    /// </summary>
    public static readonly Regex ValidIpV6Regex = new(@"^(:((:[\da-f]{1,4}){1,7}|:)|(?=([^:]+:)+((:[^:]+)+|:)$)([\da-f]{1,4}::?){1,6}(:|[\da-f]{0,4}(::)?)|([\da-f]{1,4}:){7}[\da-f]{1,4})$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
    
    /// <summary>
    /// Matches a valid IPv6 address with an embedded IPv4 address that is surrounded by square brackets.
    /// </summary>
    public static readonly Regex ValidUriIpV6wV4Regex = new(@"^\[(:(:[\da-f]{1,4}){0,5}:|[\da-f]{1,4}:(:([\da-f]{1,4}:){0,4}|[\da-f]{1,4}:(:([\da-f]{1,4}:){0,3}|[\da-f]{1,4}:(:([\da-f]{1,4}:){0,2}|[\da-f]{1,4}:(:([\da-f]{1,4}:)?|[\da-f]{1,4}:([\da-f]{1,4}?)?:)))))(([01](\d\d?)?|2([0-4]\d?|5[0-5]?|[6-9])?|[3-9]\d?)\.){3}([01](\d\d?)?|2([0-4]\d?|5[0-5]?|[6-9])?|[3-9]\d?)\]$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
    
    /// <summary>
    /// Matches a valid IPv6 address with an embedded IPv4 address.
    /// </summary>
    public static readonly Regex ValidIpV6wV4Regex = new(@"^(:(:[\da-f]{1,4}){0,5}:|[\da-f]{1,4}:(:([\da-f]{1,4}:){0,4}|[\da-f]{1,4}:(:([\da-f]{1,4}:){0,3}|[\da-f]{1,4}:(:([\da-f]{1,4}:){0,2}|[\da-f]{1,4}:(:([\da-f]{1,4}:)?|[\da-f]{1,4}:([\da-f]{1,4}?)?:)))))(([01](\d\d?)?|2([0-4]\d?|5[0-5]?|[6-9])?|[3-9]\d?)\.){3}([01](\d\d?)?|2([0-4]\d?|5[0-5]?|[6-9])?|[3-9]\d?)$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
    
    /// <summary>
    /// Matches a valid URI-encoded host name.
    /// </summary>
    public static readonly Regex ValidHostNameRegex = new(@"^([!$&'()*+,;=\w.~-]+|%[\da-f]{2})*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a valid URI port string.
    /// </summary>
    public static readonly Regex ValidPortRegex = new(@"^(6(5(5(3[0-5]?|[0-2]\d?|[4-9])?|[0-4]\d{0,2}|[6-9]\d?)?|[0-4]\d{0,3}|[6-9]\d{0,2})?|[0-5]\d{0,4}|[7-9]\d{0,3})$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Pattern for parsing URI components.
    /// </summary>
    public static readonly Regex UrlParseComponentsRegex = new(@"^(?:(?<scheme>[^?#:/\\@]+):(?<sep>//?)?)?(?:(?:(?<user>[^?#:/\\@]*)(?::(?<pw>[^?#/\\@]*))?@)(?<host>[^?#:/\\]+)(?::(?<port>[^?#:/\\]+))?)(?<path>[^?#]*)(?:\?(?<query>[^#]*))?(?:#(?<fragment>.*))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    private string _scheme;
    
    /// <summary>
    /// Gets or sets the URL Scheme name.
    /// </summary>
    /// <remarks>An empty value indicates that the URL has no schema specified.</remarks>
    public string Scheme
    {
        get => _scheme;
        set
        {
            if (string.IsNullOrEmpty(value))
                _scheme = "";
            else
            {
                if (!ValidSchemeRegex.IsMatch(value))
                    throw new ArgumentException($"Invalid URI {nameof(Scheme)} name.", nameof(value));
                _scheme = value;
            }
        }
    }

    private SchemeSeparatorType _schemeSeparator;
    public SchemeSeparatorType SchemeSeparator
    {
        get => (_scheme.Length > 0) ? _schemeSeparator : SchemeSeparatorType.NoSlash;
        set => _schemeSeparator = value;
    }
    
    /// <summary>
    /// Gets or sets the URL authority components.
    /// </summary>
    /// <remarks>An empty value indicates that the URL has no authority components.</remarks>
    public UrlAuthority? Authority { get; set; }

    /// <summary>
    /// Gets or sets the URL path segments.
    /// </summary>
    public PathSegmentCollection PathSegments { get; }

    /// <summary>
    /// Gets or sets the URL query sub-components.
    /// </summary>
    /// <remarks>An empty value indicates that the URL has no query component.</remarks>
    public UriQueryCollection? Query { get; set; }

    /// <summary>
    /// Gets or sets the URL fragment component.
    /// </summary>
    /// <remarks>An empty value indicates that the URL has no fragment component.</remarks>
    public string? Fragment { get; set; }
    
    private Url(string scheme, SchemeSeparatorType schemeSeparator, UrlAuthority? authority, IEnumerable<string> pathSegments, bool isRooted, IEnumerable<UriQueryElement>? query, string? fragment)
    {
        _scheme = scheme ?? "";
        _schemeSeparator = schemeSeparator;
        Authority = authority;
        PathSegments = new(this, pathSegments, isRooted);
        Query = (query is null) ? null : new(query);
        Fragment = fragment;
    }

    /// <summary>
    /// Creates a new absolute URL object.
    /// </summary>
    /// <param name="scheme">The scheme name.</param>
    /// <param name="schemeSeparator">The scheme separator type.</param>
    /// <param name="authority">The URI authority components.</param>
    /// <param name="path">The raw (not URI-encoded) path string.</param>
    /// <param name="query">The URI query sub-components or <see langword="null" /> for no query component.</param>
    /// <param name="fragment">The fragment component or <see langword="null" /> for no fragment component.</param>
    public Url(string scheme, SchemeSeparatorType schemeSeparator, UrlAuthority authority, string? path = null, IEnumerable<UriQueryElement>? query = null, string? fragment = null)
    {
        if (string.IsNullOrEmpty(scheme) || !ValidSchemeRegex.IsMatch(scheme))
            throw new ArgumentException($"Invalid URI {nameof(scheme)} name.", nameof(scheme));
        _scheme = scheme;
        _schemeSeparator = schemeSeparator;
        Authority = authority;
        PathSegments = new(this, string.IsNullOrEmpty(path) ? Enumerable.Empty<string>() : PathSeparatorRegex.Split(path).Where(p => p.Length > 0), true);
        Query = (query is null) ? null : new(query);
        Fragment = fragment;
    }

    /// <summary>
    /// Creates a new absolute URL object.
    /// </summary>
    /// <param name="scheme">The scheme name.</param>
    /// <param name="schemeSeparator">The scheme separator type.</param>
    /// <param name="path">The raw (not URI-encoded) path string.</param>
    /// <param name="query">The URI query sub-components or <see langword="null" /> for no query component.</param>
    /// <param name="fragment">The fragment component or <see langword="null" /> for no fragment component.</param>
    public Url(string scheme, SchemeSeparatorType schemeSeparator, string path, IEnumerable<UriQueryElement>? query = null, string? fragment = null)
    {
        if (string.IsNullOrEmpty(scheme) || !ValidSchemeRegex.IsMatch(scheme))
            throw new ArgumentException($"Invalid URI {nameof(scheme)} name.", nameof(scheme));
        _scheme = scheme;
        _schemeSeparator = schemeSeparator;
        PathSegments = new(this, string.IsNullOrEmpty(path) ? Enumerable.Empty<string>() : PathSeparatorRegex.Split(path).Where(p => p.Length > 0), true);
        Query = (query is null) ? null : new(query);
        Fragment = fragment;
    }

    /// <summary>
    /// Creates a new relative URL object.
    /// </summary>
    /// <param name="authority">The URI authority components or <see langword="null" /> for no authority components.</param>
    /// <param name="path">The raw (not URI-encoded) path string.</param>
    /// <param name="query">The URI query sub-components or <see langword="null" /> for no query component.</param>
    /// <param name="fragment">The fragment component or <see langword="null" /> for no fragment component.</param>
    public Url(UrlAuthority? authority = null, string? path = null, IEnumerable<UriQueryElement>? query = null, string? fragment = null)
    {
        _scheme = string.Empty;
        _schemeSeparator = SchemeSeparatorType.NoSlash;
        Authority = authority;
        if (string.IsNullOrEmpty(path))
            PathSegments = new(this, Enumerable.Empty<string>(), false);
        else
            PathSegments = new(this, PathSeparatorRegex.Split(path).Where(p => p.Length > 0), PathRootedRegex.IsMatch(path));
        Query = (query is null) ? null : new(query);
        Fragment = fragment;
    }

    /// <summary>
    /// Creates a new relative URL object.
    /// </summary>
    /// <param name="path">The raw (not URI-encoded) path string.</param>
    /// <param name="query">The URI query sub-components or <see langword="null" /> for no query component.</param>
    /// <param name="fragment">The fragment component or <see langword="null" /> for no fragment component.</param>
    public Url(string path, IEnumerable<UriQueryElement>? query = null, string? fragment = null)
    {
        _scheme = string.Empty;
        _schemeSeparator = SchemeSeparatorType.NoSlash;
        if (string.IsNullOrEmpty(path))
            PathSegments = new(this, Enumerable.Empty<string>(), false);
        else
            PathSegments = new(this, PathSeparatorRegex.Split(path).Where(p => p.Length > 0), PathRootedRegex.IsMatch(path));
        Query = (query is null) ? null : new(query);
        Fragment = fragment;
    }

    public static string UriDecode(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        return EncodedSequenceRegex.Replace(value, m => new string((char)int.Parse(m.Value.AsSpan(1), System.Globalization.NumberStyles.HexNumber), 1));
    }

    public static bool TryParse(string? urlString, UriKind kind, [NotNullWhen(true)] out Url? result)
    {
        if (string.IsNullOrEmpty(urlString))
        {
            result = new Url(string.Empty, SchemeSeparatorType.NoSlash, null, Enumerable.Empty<string>(), false, null, null);
            return true;
        }
        Match match = UrlParseComponentsRegex.Match(urlString);
        if (!match.Success)
        {
            result = null;
            return false;
        }
        string scheme;
        SchemeSeparatorType separatorType;
        Group g = match.Groups["scheme"];
        if (g.Success)
        {
            if (kind == UriKind.Relative)
            {
                result = null;
                return false;
            }
            scheme = g.Value;
            if (!ValidSchemeRegex.IsMatch(scheme))
            {
                result = null;
                return false;
            }
            if ((g = match.Groups["sep"]).Success)
                separatorType = (g.Length > 1) ? SchemeSeparatorType.DoubleSlash : SchemeSeparatorType.SingleSlash;
            else
                separatorType = SchemeSeparatorType.NoSlash;
        }
        else
        {
            if (kind == UriKind.Absolute)
            {
                result = null;
                return false;
            }
            scheme = string.Empty;
            separatorType = SchemeSeparatorType.NoSlash;
        }
        UrlAuthority? authority;
        if ((g = match.Groups["host"]).Success)
        {
            Match m = ValidIpV6Regex.Match(g.Value);
            if (ValidIpV6Regex.IsMatch(g.Value))
                authority = new(m.Groups[1].Value);
            else
            {
                if (g.Value.Length > 1 && g.Value[0] == '[' && g.Value[^1] == ']')
                {
                    result = null;
                    return false;
                }
                authority = new(UriDecode(g.Value));
            }
            if ((g = match.Groups["port"]).Success)
            {
                if (ushort.TryParse(g.Value, out ushort p))
                    authority.Port = p;
                else
                {
                    result = null;
                    return false;
                }
            }
            if ((g = match.Groups["user"]).Success)
                authority.UserInfo = new(UriDecode(g.Value), (g = match.Groups["pwd"]).Success ? UriDecode(g.Value) : null);
        }
        else
            authority = null;
        IEnumerable<UriQueryElement>? query;
        if ((g = match.Groups["query"]).Success)
        {
            query = (g.Length == 1) ? Enumerable.Empty<UriQueryElement>() : g.Value.Split('&').Select(kvp =>
            {
                int i = kvp.IndexOf('=');
                if (i < 0)
                    return new UriQueryElement(UriDecode(kvp), null);
                if (i == 0)
                    return new UriQueryElement(string.Empty, (kvp.Length == 1) ? string.Empty : UriDecode(kvp[2..]));
                return new UriQueryElement(UriDecode(kvp[..i]), (i < kvp.Length - 1) ? UriDecode(kvp[(i + 1)..]) : (string?)string.Empty);
            });
        }
        else
            query = null;
        string? fragment = (g = match.Groups["fragment"]).Success ? UriDecode(match.Value) : null;
        if ((g = match.Groups["path"]).Length == 0)
            result = new Url(scheme, separatorType, authority, Enumerable.Empty<string>(), scheme is not null || authority is not null, query, fragment);
        else
            result = new Url(scheme, separatorType, authority, PathSeparatorRegex.Split(g.Value).Where(s => s.Length > 0).Select(UriDecode), scheme is not null || authority is not null || PathRootedRegex.IsMatch(g.Value), query, fragment);
        return true;
    }

    public int CompareTo(Url? other)
    {
        if (other is null)
            return 1;
        if (ReferenceEquals(this, other))
            return 0;
        string sX = _scheme;
        string sY = other._scheme;
        int result;
        if (sX.Length > 0)
        {
            if (sY.Length == 0)
                return 1;
            if ((result = Comparer.Compare(sX, sY)) != 0 || (result = SchemeSeparator.CompareTo(other.SchemeSeparator)) != 0)
                return result;
        }
        else if (sY.Length > 0)
            return -1;
        UrlAuthority? aX = Authority;
        UrlAuthority? aY = other.Authority;
        if (aX is null)
        {
            if (aY is not null)
                return -1;
        }
        else
        {
            if (aY is null)
                return 1;
            if ((result = aX.CompareTo(aY)) != 0)
                return result;
        }
        if ((result = PathSegments.CompareTo(other.PathSegments)) != 0)
            return result;
        UriQueryCollection? qX = Query;
        UriQueryCollection? qY = other.Query;
        if (qX is null)
        {
            if (qY is not null)
                return -1;
        }
        else
        {
            if (qY is null)
                return 1;
            if ((result = qX.CompareTo(qY)) != 0)
                return result;
        }
        string? fX = Fragment;
        string? fY = other.Fragment;
        if (fX is null)
            return (fY is null) ? 0 : -1;
        return (fY is null) ? 1 : Comparer.Compare(fX, fY);
    }

    public bool Equals(Url? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        string sX = _scheme;
        string sY = other._scheme;
        if ((sX.Length > 0) ? !Comparer.Equals(sX, sY) : sY.Length > 0)
            return false;
        UrlAuthority? aX = Authority;
        UrlAuthority? aY = other.Authority;
        if (((aX is null) ? aY is not null : !aX.Equals(aY)) || !PathSegments.Equals(other.PathSegments))
            return false;
        UriQueryCollection? qX = Query;
        UriQueryCollection? qY = other.Query;
        if ((qX is null) ? qY is not null : !qX.Equals(qY))
            return false;
        string? fX = Fragment;
        string? fY = other.Fragment;
        return (fX is null) ? fY is null : fY is not null && Comparer.Equals(fX, fY);
    }

    public override bool Equals(object? obj) => obj is Url other && Equals(other);

    public override int GetHashCode()
    {
        int hashCode = 13;
        unchecked
        {
            string s = _scheme;
            hashCode = (hashCode * 17) + Comparer.GetHashCode(s);
            if (s.Length > 0)
                hashCode = (hashCode * 17) + SchemeSeparator.GetHashCode();
            else
                hashCode *= 17;
            UrlAuthority? authority = Authority;
            if (authority is null)
                hashCode *= 17;
            else
                hashCode = (hashCode * 17) + authority.GetHashCode();
            hashCode = (hashCode * 17) + PathSegments.GetHashCode();
            UriQueryCollection? query = Query;
            if (query is null)
                hashCode *= 17;
            else
                hashCode = (hashCode * 17) + query.GetHashCode();
            string? f = Fragment;
            return (f is null) ? hashCode * 17 : (hashCode * 17) + Comparer.GetHashCode(f);
        }
    }

    public override string ToString()
    {
        string s = _scheme;
        UrlAuthority? authority = Authority;
        UriQueryCollection? query = Query;
        string? f = Fragment;
        if (s.Length > 0)
        {
            if (authority is not null)
            {
                string a = authority.ToString();
                if (a.Length > 0)
                    s += a;
            }
            s = SchemeSeparator switch
            {
                SchemeSeparatorType.DoubleSlash => "://",
                SchemeSeparatorType.SingleSlash => "://",
                _ => ":",
            };
        }
        else
        {
            if (authority is null)
            {
                if ((s = PathSegments.ToString()).Length > 0)
                {
                    if (query is null)
                        return (f is null) ? s : (f.Length > 0) ? $"{s}#{FragmentEEncodeRegex.Replace(f, m => Uri.HexEscape(m.Value[0]))}" : "{s}#";
                    s += query.ToString();
                    return (f is null) ? s : (f.Length > 0) ? $"{s}#{FragmentEEncodeRegex.Replace(f, m => Uri.HexEscape(m.Value[0]))}" : "{s}#";
                }
                else if (query is null)
                    return (f is null) ? s : (f.Length > 0) ? $"#{FragmentEEncodeRegex.Replace(f, m => Uri.HexEscape(m.Value[0]))}" : "#";
                return (f is null) ? query.ToString() : (f.Length > 0) ? $"{query}#{FragmentEEncodeRegex.Replace(f, m => Uri.HexEscape(m.Value[0]))}" : $"{query}#";
            }
            s = authority.ToString() + PathSegments.ToString();
            if (query is null)
                return (f is null) ? s : (f.Length > 0) ? $"{authority}{PathSegments}{Query}#{f}" : $"{authority}{PathSegments}{Query}#";
        }
        
        throw new NotImplementedException();
    }

    public static bool operator == (Url left, Url right) => (left is null) ? right is null : left.Equals(right);

    public static bool operator !=(Url left, Url right) => !(left == right);

    public static bool operator <(Url left, Url right) => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Url left, Url right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Url left, Url right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Url left, Url right) => left is null ? right is null : left.CompareTo(right) >= 0;
}
