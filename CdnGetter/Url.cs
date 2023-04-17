using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CdnGetter;

public partial class Url : IEquatable<Url>, IComparable<Url>
{
    public static readonly StringComparer Comparer = StringComparer.InvariantCultureIgnoreCase;
    // public const string REGEX_PATTERN_DIGIT = @"\d";
    // public const string REGEX_PATTERN_HEXDIG = @"[\da-f]";
    // public const string REGEX_PATTERN_ALPHA = @"[a-z]";
    // public const string REGEX_PATTERN_PCT_ENCODED = @"%[\da-f]{2}";
    // public const string REGEX_PATTERN_GEN_DELIM = @"[:/?#\[\]@]";
    // public const string REGEX_PATTERN_SUB_DELIM = @"[!$&'()*+,;=]";
    // public const string REGEX_PATTERN_RESERVED = @"[:/?#\[\]@!$&'()*+,;=]";
    // public const string REGEX_PATTERN_UNRESERVED = @"[\w.~-]";
    public static readonly Regex PathRootedRegex = new(@"^[\\/]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    public static readonly Regex PathSeparatorRegex = new(@"[\\/]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    public static readonly Regex EncodedSequenceRegex = new(@"%[\da-f]{2}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    public static readonly Regex ValidSchemeRegex = new(@"^[a-z][a-z\d+.-]*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    /// <summary>
    /// Matches a Host or UserName character that should be encoded.
    /// </summary>
    public static readonly Regex NameEncodeRegex = new(@"[^!$&'()*+,;=\w.~-]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    public static readonly Regex NameEncodeRegexAlt = new(@"%(?![\da-f]{2})|[^%!$&'()*+,;=\w.~-]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    /// <summary>
    /// Matches a Pasword character that should be encoded.
    /// </summary>
    public static readonly Regex PasswordEncodeRegex = new(@"[^!$&'()*+,;=\w.~:-]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    public static readonly Regex PasswordEncodeRegexAlt = new(@"%(?![\da-f]{2})|[^%!$&'()*+,;=\w.~:-]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    public static readonly Regex UserInfoRegex = new(@"(?<username>([!$&'()*+,;=\w.~-]|%[\da-f]{2})*)(?::(?<pw>([!$&'()*+,;=\w.~:-]|%[\da-f]{2})*))?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    public static readonly Regex ValidIpV4Regex = new(@"^(?:(?:[0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3})(?:[0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    public static readonly Regex ValidIpV6Regex = new(@"^\[((?:::f{4}:|[\da-f]{1,4}(?::[\da-f]{1,4}){7}|(?:(?:[\da-f]]{1,4}(?::[\da-f]]{1,4}){0,5})?)::(?:(?:[\da-f]]{1,4}(?::[\da-f]]{1,4}){0,5})?)))\]$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    public static readonly Regex ValidHostNameRegex = new(@"^([!$&'()*+,;=\w.~-]+|%[\da-f]{2})*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    public static readonly Regex ValidPortRegex = new(@"^(6(5(5(3[0-5]?|[0-2]\d?|[4-9])?|[0-4]\d{0,2}|[6-9]\d?)?|[0-4]\d{0,3}|[6-9]\d{0,2})?|[0-5]\d{0,4}|[7-9]\d{0,3})$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    public static readonly Regex UrlParseComponentsRegex = new(@"^(?:(?<scheme>[^?#:/\\@]+):/(?<sep>/)?)?(?:(?:(?<user>[^?#:/\\@]*)(?::(?<pw>[^?#/\\@]*))?@)(?<host>[^?#:/\\]+)(?::(?<port>[^?#:/\\]+))?)(?<path>[^?#]*)(?:\?(?<query>[^#]*))?(?:#(?<fragment>.*))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    private string _scheme;
    
    /// <summary>
    /// Url Scheme name.
    /// </summary>
    public string Scheme
    {
        get => _scheme;
        set => _scheme = value ?? "";
    }

    public bool UseDoubleSchemeSeparator { get; set; }

    public UrlAuthority? Authority { get; set; }

    public PathSegmentCollection PathSegments { get; }

    public UriQueryCollection? Query { get; set; }

    public string? Fragment { get; set; }
    
    private Url(string? originalString, string scheme, bool doubleSeparator, UrlAuthority? authority, IEnumerable<string> pathSegments, bool isRooted, IEnumerable<UriQueryElement>? query, string? fragment)
    {
        _scheme = scheme ?? "";
        Authority = authority;
        PathSegments = new(this, pathSegments, isRooted);
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
            result = new Url(urlString, string.Empty, false, null, Enumerable.Empty<string>(), false, null, null);
            return true;
        }
        Match match = UrlParseComponentsRegex.Match(urlString);
        if (!match.Success)
        {
            result = null;
            return false;
        }
        string scheme;
        bool doubleSeparator;
        Group g = match.Groups["scheme"];
        if (g.Success)
        {
            scheme = g.Value;
            if (!ValidSchemeRegex.IsMatch(scheme))
            {
                result = null;
                return false;
            }
            doubleSeparator = match.Groups["sep"].Success;
        }
        else
        {
            scheme = string.Empty;
            doubleSeparator = false;
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
            result = new Url(urlString, scheme, doubleSeparator, authority, Enumerable.Empty<string>(), scheme is not null || authority is not null, query, fragment);
        else
            result = new Url(urlString, scheme, doubleSeparator, authority, PathSeparatorRegex.Split(g.Value).Where(s => s.Length > 0).Select(UriDecode), scheme is not null || authority is not null || PathRootedRegex.IsMatch(g.Value), query, fragment);
        return true;
    }

    public int CompareTo(Url? other)
    {
        throw new NotImplementedException();
    }

    public bool Equals(Url? other)
    {
        throw new NotImplementedException();
    }

    public override bool Equals(object? obj) => obj is Url other && Equals(other);

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        throw new NotImplementedException();
    }
}
