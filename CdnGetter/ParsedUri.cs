using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CdnGetter;

/// <summary>
/// Normalized alternative to <see cref="Uri" /> for more consistent comparison.
/// </summary>
/// <remarks>Default comparisons for this class are case-insensitive, using the word comparison rules of the invariant culture.</remarks>
public partial class ParsedUri : IEquatable<ParsedUri>, IComparable<ParsedUri>
{
    #region Static Fields / Constants

    /// <summary>
    /// Gets the default URI component comparer.
    /// </summary>
    public static readonly StringComparer DefaultComponentComparer = StringComparer.InvariantCultureIgnoreCase;

    /// <summary>
    /// Delimiter character for 
    /// </summary>
    private const char DELIMITER_CHAR_COLON = ':';
    private const char DELIMITER_CHAR_SLASH = '/';
    private const char DELIMITER_CHAR_BACKSLASH = '\\';
    private const char DELIMITER_CHAR_QUERY = '?';
    private const char DELIMITER_CHAR_EQUALS = '=';
    private const char DELIMITER_CHAR_AMPERSAND = '&';
    private const char DELIMITER_CHAR_HASH = '#';
    private static readonly char[] URL_DELIMITERS = new char[] { DELIMITER_CHAR_COLON, DELIMITER_CHAR_SLASH, DELIMITER_CHAR_BACKSLASH, DELIMITER_CHAR_QUERY, DELIMITER_CHAR_HASH };
    private const int PORT_NUMBER_FTP = 21;
    private const int PORT_NUMBER_SFTP = 22;
    private const int PORT_NUMBER_GOPHER = 70;
    private const int PORT_NUMBER_NNTP = 119;
    private const string SCHEME_SEPARATOR_DSLASH = "//";

    #region Regular Expressions

    public static readonly Regex QueryDelimiterRegex = new(@"[&?]", RegexOptions.Compiled);

    private const string GROUP_NAME_sep = "sep";
    private const string GROUP_NAME_sub = "sub";
    public static readonly Regex QuerySubcomponentRegex = new($@"^[^&?]+|\G(?<{GROUP_NAME_sep}>[&?])(<{GROUP_NAME_sub}>[^&?]+)?", RegexOptions.Compiled);

    public static readonly Regex PathDelimiterRegex = new(@"[/:\\]", RegexOptions.Compiled);

    public static readonly Regex DosPathDelimiterRegex = new(@"[\\/]", RegexOptions.Compiled);

    public static readonly Regex PathSegmentRegex = new($@"^[^/:\\]+|\G(?<{GROUP_NAME_sep}>[/:\\])[^/:\\]*", RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a URI scheme name.
    /// </summary>
    public static readonly Regex ValidSchemeNameRegex = new(@"^[a-z][a-z\d+.-]*$", RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a URI scheme name.
    /// </summary>
    public static readonly Regex ValidSchemeSeparatorRegex = new(@"^[:/]+$", RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a Host or UserName character that should be encoded.
    /// </summary>
    public static readonly Regex NameEncodeRegex = new(@"[^!$&'()*+,;=\w.~-]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a URI-encoded character sequence.
    /// </summary>
    public static readonly Regex EncodedSequenceRegex = new(@"%[\da-f]{2}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private const string GROUP_NAME_user = "user";
    private const string GROUP_NAME_host = "host";
    private const string GROUP_NAME_port = "port";
    private const string GROUP_NAME_path = "path";
    private static readonly Regex UserInfoHostAndPortRegex = new($@"^((?<{GROUP_NAME_user}>[^@\\/]+)@)?(?<{GROUP_NAME_host}>[^:\\/]*)(:(?<{GROUP_NAME_port}>\d+))?(?<{GROUP_NAME_path}>.*)$", RegexOptions.Compiled);

    private const string GROUP_NAME_drive = "drive";
    private static readonly Regex DosPathRegex = new($@"^([\\/]{2}[.?][\\/])?(?<{GROUP_NAME_drive}>[a-z]:)[\\/](?<{GROUP_NAME_path}>[\\/]*[^""*/:<>?\\|]+([\\/]+[^""*/:<>?\\|]+)*[\\/]*|[\\/]+)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    private static readonly Regex UncPathLooseRegex = new($@"^[\\/][\\/]+(?<{GROUP_NAME_host}>[^""*/:<>?\\|]+|[\da-f]{{2}}(::?[\da-f]{{2}}){0,8}|::)[\\/](?<{GROUP_NAME_path}>[\\/]*[^""*/:<>?\\|]+([\\/]+[^""*/:<>?\\|]+)*[\\/]*|[\\/]+)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    private static readonly Regex UncPathAsUriStringRegex = new($@"^\\\\(?<{GROUP_NAME_host}>[^""*/:<>?\\|]+|[\da-f]{{2}}(::?[\da-f]{{2}}){0,8}|::)\\(?<{GROUP_NAME_path}>([^""#&*/:<>?@\\^|]+[\\/]+[^""#&*/:<>?@\\^|]+)*[\\/]*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    private static readonly Regex PsPathRegex = new($@"^(?<{GROUP_NAME_drive}>[^\x00-\x1F*:?\[\\\]]+:)[\\/]+(?<{GROUP_NAME_path}>([^""*/:<>?\\|]+[\\/]+[^""*/:<>?\\|]+)*[\\/]*)?$");
    
    private static readonly Regex UnixPathRegex = new(@"^(((/+[^\x00/]+)+|[^\x00/]+(/+[^\x00/]+)*)/*|/+)$");

    #endregion

    public static readonly ParsedUri Empty = new();

    #endregion

    /// <summary>
    /// Gets the scheme name for this URI.
    /// </summary>
    /// <value>The scheme name of the URI represented by this instance or <see langword="null" /> if this represents a relative URI.</value>
    /// <remarks>If not <see langword="null" />, this will always contain a valid, lower-case URI scheme name.</remarks>
    public string? SchemeName { get; }

    /// <summary>
    /// Gets the additional scheme separator characters for this URI.
    /// </summary>
    /// <value>The additional scheme separator characters for the URI represented by this instance or <see langword="null" /> if this represents a relative URI.</value>
    public string? SchemeSeparator { get; }

    /// <summary>
    /// Gets the authority component.
    /// </summary>
    /// <value>The authority component of the URI represented by this instance or <see langword="null" /> if this represents a relative URI.</value>
    public UriAuthority? Authority { get; }

    /// <summary>
    /// Gets the path segments for this URI.
    /// </summary>
    public ReadOnlyCollection<PathSegment> PathSegments { get; }
    
    /// <summary>
    /// Gets the query sub-components for this URI.
    /// </summary>
    /// <value>The query sub-components for the URI represented by this instance or <see langword="null" /> if this URI has no query component.</value>
    /// <remarks>If this not <see langword="null" /> and does not have any elements, then that indicates that there is a query component, but it is empty.</remarks>
    public ReadOnlyCollection<QuerySubComponent>? Query { get; }

    /// <summary>
    /// Gets the URI fragment or <see langword="null" /> if the URI has no fragment component.
    /// </summary>
    /// <value>The fragment component for the URI represented by this instance or <see langword="null" /> if this URI has no fragment component.</value>
    /// <remarks>Unlike <see cref="Uri.Fragment" />, this is the un-escaped value, and does not include the delimiting <c>#</c> character.
    /// An empty string indicatse that this has an empty fragment component.</remarks>
    public string? Fragment { get; }
    
    #region Constructors

    /// <summary>
    /// Creates a new absolute <c>ParsedUri</c> object from un-escaped component values.
    /// </summary>
    /// <param name="schemeName">The name of the URI scheme.</param>
    /// <param name="schemeSeparator">The additional URI scheme separator characters.</param>
    /// <param name="authority">The URI authority component.</param>
    /// <param name="pathSegments">The optional segments of the URI path component.</param>
    /// <param name="query">The optional sub-components of the URI query component or <see langword="null" /> if there is no query component.</param>
    /// <param name="fragment">The optional fragment component of the URI or <see langword="null" /> if there is no fragment component.</param>
    public ParsedUri(string schemeName, string schemeSeparator, UriAuthority authority, IEnumerable<PathSegment>? pathSegments = null, IEnumerable<QuerySubComponent>? query = null, string? fragment = null)
    {
        if (string.IsNullOrEmpty(schemeName))
            throw new ArgumentException($"'{nameof(schemeName)}' cannot be null or empty.", nameof(schemeName));
        if (!ValidSchemeNameRegex.IsMatch(schemeName))
            throw new ArgumentException($"'{schemeName}' is an invalid scheme name.", nameof(schemeName));
        if (string.IsNullOrEmpty(schemeSeparator))
           SchemeSeparator = string.Empty;
        else
        {
            if (!ValidSchemeSeparatorRegex.IsMatch(schemeSeparator))
                throw new ArgumentException($"'{schemeSeparator}' is an invalid scheme separator.", nameof(schemeSeparator));
            SchemeSeparator = schemeSeparator;
        }
        Authority = authority ?? UriAuthority.Empty;
        PathSegments = new((pathSegments is null) ? Array.Empty<PathSegment>() : pathSegments.Where(s => s is not null).ToArray());
        Query = new((query is null) ? Array.Empty<QuerySubComponent>() : query.Where(q => q is not null).ToArray());
        Fragment = fragment;
    }
    
    /// <summary>
    /// Creates a new absolute <c>ParsedUri</c> object from un-escaped component values.
    /// </summary>
    /// <param name="schemeName">The name of the URI scheme.</param>
    /// <param name="schemeSeparator">The additional URI scheme separator characters.</param>
    /// <param name="authority">The URI authority component.</param>
    /// <param name="pathSegments">The segments of the URI path component.</param>
    public ParsedUri(string schemeName, string schemeSeparator, UriAuthority authority, params PathSegment[] pathSegments) : this(schemeName, schemeSeparator, authority, (IEnumerable<PathSegment>)pathSegments) { }

    /// <summary>
    /// Creates a new relative <c>ParsedUri</c> object from un-escaped component values.
    /// </summary>
    /// <param name="rootSegment">The root segment representing the path component.</param>
    /// <param name="query">The optional sub-components of the URI query component or <see langword="null" /> if there is no query component.</param>
    /// <param name="fragment">The optional fragment component of the URI or <see langword="null" /> if there is no fragment component.</param>
    public ParsedUri(PathSegment rootSegment, IEnumerable<QuerySubComponent>? query = null, string? fragment = null)
    {
        PathSegments = new(new PathSegment[] { rootSegment ?? PathSegment.EmptyRoot });
        Query = new((query is null) ? Array.Empty<QuerySubComponent>() : query.Where(q => q is not null).ToArray());
        Fragment = fragment;
    }
    
    /// <summary>
    /// Creates a new relative <c>ParsedUri</c> object from un-escaped component values.
    /// </summary>
    /// <param name="pathSegments">The segments of the URI path component.</param>
    /// <param name="query">The optional sub-components of the URI query component or <see langword="null" /> if there is no query component.</param>
    /// <param name="fragment">The optional fragment component of the URI or <see langword="null" /> if there is no fragment component.</param>
    public ParsedUri(IEnumerable<PathSegment>? pathSegments, IEnumerable<QuerySubComponent>? query = null, string? fragment = null)
    {
        PathSegments = new((pathSegments is null) ? Array.Empty<PathSegment>() : pathSegments.Where(s => s is not null).ToArray());
        Query = new((query is null) ? Array.Empty<QuerySubComponent>() : query.Where(q => q is not null).ToArray());
        Fragment = fragment;
    }
    
    /// <summary>
    /// Creates a new relative <c>ParsedUri</c> object with a query component.
    /// </summary>
    /// <param name="pathSegments">The segments of the URI path component.</param>
    /// <param name="query">The sub-components of the URI query component.</param>
    public ParsedUri(IEnumerable<PathSegment>? pathSegments, params QuerySubComponent[] query) : this(pathSegments, query, null) { }
    
    /// <summary>
    /// Creates a new relative <c>ParsedUri</c> object.
    /// </summary>
    /// <param name="pathSegments">The segments of the URI path component.</param>
    public ParsedUri(params PathSegment[] pathSegments) : this(pathSegments, (IEnumerable<QuerySubComponent>?)null, null) { }

    #endregion

    /// <summary>
    /// Unescapes URI-encoded character sequences.
    /// </summary>
    /// <param name="value">The string that might have URI-encoded character sequences.</param>
    /// <returns>The un-escaped string.</returns>
    public static string UriDecode(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        return EncodedSequenceRegex.Replace(value, m => new string((char)int.Parse(m.Value.AsSpan(1), System.Globalization.NumberStyles.HexNumber), 1));
    }

    /// <summary>
    /// Tries to parse a string as a URI.
    /// </summary>
    /// <param name="uriString">The source URI string.</param>
    /// <param name="kind">The kind of URI to parse.</param>
    /// <param name="options">Normalization options.</param>
    /// <param name="uri">The parsed URI or <see langword="null" /> if the <paramref name="uriString" /> could not be parsed.</param>
    /// <returns><see langword="true" /> if the <paramref name="uriString" /> could be parsed; otherwise, <see langword="false" />.</returns>
    public static bool TryParse(string uriString, UriKind kind, NormalizationOptions options, [NotNullWhen(true)] out ParsedUri? uri)
    {
        if (string.IsNullOrEmpty(uriString))
        {
            uri = new ParsedUri();
            return true;
        }
        int index = uriString.IndexOfAny(URL_DELIMITERS);
        if (index < 0)
        {
            uri = new ParsedUri(new PathSegment(UriDecode(uriString)));
            return true;
        }
        string? query, fragment;
        switch (uriString[index])
        {
            case '#':
                if (kind == UriKind.Absolute)
                {
                    uri = null;
                    return false;
                }
                if (uriString.Length > 1)
                {
                    if (index == 0) // uriString == "#fragment"
                        uri = new ParsedUri((IEnumerable<PathSegment>?)null, null, UriDecode(uriString[1..]));
                    else if (index < uriString.Length - 1) // uriString == "path#fragment"
                        uri = new ParsedUri(new PathSegment(UriDecode(uriString[..index])), null, UriDecode(uriString[(index + 1)..]));
                    else // uriString == "path#"
                        uri = options.HasFlag(NormalizationOptions.StripEmptyFragment) ? new ParsedUri(new PathSegment(UriDecode(uriString[..index]))) :
                            new ParsedUri(new PathSegment(UriDecode(uriString[..index])), null, string.Empty);
                }
                else // uriString == "#"
                    uri = options.HasFlag(NormalizationOptions.StripEmptyFragment) ? new ParsedUri() : new ParsedUri((IEnumerable<PathSegment>?)null, null, string.Empty);
                return true;
            case '?':
                if (kind == UriKind.Absolute)
                {
                    uri = null;
                    return false;
                }
                if (uriString.Length > 1)
                {
                    if (index == 0)
                    {
                        if ((index = uriString.IndexOf(DELIMITER_CHAR_HASH)) < 0) // uriString == "?query"
                            uri = new ParsedUri(new PathSegment(uriString), QuerySubComponent.Parse(uriString[1..]));
                        else if (uriString.Length > 2)
                        {
                            if (index == 1) // uriString == "?#fragment"
                                uri = new ParsedUri((IEnumerable<PathSegment>?)null, options.HasFlag(NormalizationOptions.StripEmptyQuery) ? null : Enumerable.Empty<QuerySubComponent>(), UriDecode(uriString[(index + 1)..]));
                            else if (index < uriString.Length - 1) // uriString == "?query#fragment"
                                uri = new ParsedUri((IEnumerable<PathSegment>?)null, QuerySubComponent.Parse(uriString[..index]), UriDecode(uriString[(index + 1)..]));
                            else  // uriString == "?query#"
                                uri = new ParsedUri((IEnumerable<PathSegment>?)null, QuerySubComponent.Parse(uriString[..index]), options.HasFlag(NormalizationOptions.StripEmptyFragment) ? null : string.Empty);
                        }
                        else // uriString == "?#"
                            uri = new ParsedUri((IEnumerable<PathSegment>?)null, options.HasFlag(NormalizationOptions.StripEmptyQuery) ? null : Enumerable.Empty<QuerySubComponent>(), options.HasFlag(NormalizationOptions.StripEmptyFragment) ? null : string.Empty);

                    }
                    else if (index < uriString.Length - 1)
                    {
                        query = uriString[(index + 1)..];
                        uriString = UriDecode(uriString[..index]);
                        if ((index = query.IndexOf(DELIMITER_CHAR_HASH)) < 0) // uriString == "path"; query = "query"
                            uri = new ParsedUri(new PathSegment(uriString), QuerySubComponent.Parse(query));
                        else if (query.Length > 1)
                        {
                            if (index == 0) // uriString == "path"; query = "#fragment"
                                uri = new ParsedUri(new PathSegment(uriString), options.HasFlag(NormalizationOptions.StripEmptyQuery) ? null : Enumerable.Empty<QuerySubComponent>(), UriDecode(query[1..]));
                            else if (index < query.Length - 1) // uriString == "path"; query = "query#fragment"
                                uri = new ParsedUri(new PathSegment(uriString), QuerySubComponent.Parse(query[..index]), UriDecode(query[(index + 1)..]));
                            else // uriString == "path"; query = "query#"
                                uri = new ParsedUri(new PathSegment(uriString), QuerySubComponent.Parse(query[..index]), options.HasFlag(NormalizationOptions.StripEmptyFragment) ? null :string.Empty);
                        }
                        else // uriString == "path"; query = "#"
                            uri = new ParsedUri(new PathSegment(uriString), options.HasFlag(NormalizationOptions.StripEmptyQuery) ? null : Enumerable.Empty<QuerySubComponent>(), options.HasFlag(NormalizationOptions.StripEmptyFragment) ? null : string.Empty);
                    }
                    else // uriString == "path?"
                        uri = new ParsedUri(new PathSegment(UriDecode(uriString[..index])), options.HasFlag(NormalizationOptions.StripEmptyQuery) ? null : Enumerable.Empty<QuerySubComponent>());
                }
                else // uriString == "?"
                    uri = new ParsedUri(new PathSegment(uriString), options.HasFlag(NormalizationOptions.StripEmptyQuery) ? null : Enumerable.Empty<QuerySubComponent>());
                return true;
            case ':':
                if (index > 0 && index < uriString.Length - 1)
                {
                    if (kind == UriKind.Relative)
                    {
                        uri = null;
                        return false;
                    }
                    Match match = DosPathRegex.Match(uriString);
                    if (match.Success)
                    {
                        uri = ParseDosPath(match, options);
                        return true;
                    }
                    string scheme = uriString[0..index];
                    if (!ValidSchemeNameRegex.IsMatch(scheme))
                    {
                        uri = null;
                        return false;
                    }
                    switch (scheme)
                    {
                        case "http":
                        case "ws":
                            return TryParseHttp(scheme, uriString[(index + 1)..], options, 80, out uri);
                        case "https":
                        case "wss":
                            return TryParseHttp(scheme, uriString[(index + 1)..], options, 443, out uri);
                        case "ftp":
                            return TryParseFtp(uriString[(index + 1)..], options, out uri);
                        case "sftp":
                            return TryParseSftp(uriString[(index + 1)..], options, out uri);
                        case "file":
                            return TryParseFile(uriString[(index + 1)..], options, out uri);
                        case "gopher":
                            return TryParseGopher(uriString[(index + 1)..], options, out uri);
                        case "nntp":
                            return TryParseNntp(uriString[(index + 1)..], options, out uri);
                        case "news":
                            return TryParseNews(uriString[(index + 1)..], options, out uri);
                        case "mailto":
                            return TryParseMailTo(uriString[(index + 1)..], options, out uri);
                        case "uuid":
                            return TryParseUuid(uriString[(index + 1)..], options, out uri);
                        case "telnet":
                            return TryParseTelnet(uriString[(index + 1)..], options, out uri);
                        case "ssh":
                            return TryParseSsh(uriString[(index + 1)..], options, out uri);
                        case "ldap":
                            return TryParseLdap(scheme, uriString[(index + 1)..], options, 389, out uri);
                        case "ldaps":
                            return TryParseLdap(scheme, uriString[(index + 1)..], options, 636, out uri);
                        case "net.tcp":
                            return TryParseNetTcp(uriString[(index + 1)..], options, out uri);
                        case "net.pipe":
                            return TryParseNetPipe(uriString[(index + 1)..], options, out uri);
                        default:
                            return TryParseGeneric(scheme, uriString[(index + 1)..], options, out uri);
                    }
                }
                break;
        }
        if (kind == UriKind.Absolute)
        {
            Match match = UncPathAsUriStringRegex.Match(uriString);
            if (match.Success)
            {
                uri = ParseUncPath(match, options);
                return true;
            }
            uri = null;
            return false;
        }
        if (kind != UriKind.Relative)
        {
            Match match = UncPathAsUriStringRegex.Match(uriString);
            if (match.Success)
            {
                uri = ParseUncPath(match, options);
                return true;
            }
        }
        // uriString == "/";	index == 0
        // uriString == "/path";	index == 0
        // uriString == "/?";	index == 0
        // uriString == "/path?";	index == 0
        // uriString == "/?query";	index == 0
        // uriString == "/path?query";	index == 0
        // uriString == "/#";	index == 0
        // uriString == "/path#";	index == 0
        // uriString == "/?#";	index == 0
        // uriString == "/path?#";	index == 0
        // uriString == "/?query#";	index == 0
        // uriString == "/path?query#";	index == 0
        // uriString == "/#fragment";	index == 0
        // uriString == "/path#fragment";	index == 0
        // uriString == "/?#fragment";	index == 0
        // uriString == "/path?#fragment";	index == 0
        // uriString == "/?query#fragment";	index == 0
        // uriString == "/path?query#fragment";	index == 0
        // uriString == "path/subpath";	index < uriString.Length - 1
        // uriString == "path/?";	index < uriString.Length - 1
        // uriString == "path/subpath?";	index < uriString.Length - 1
        // uriString == "path/?query";	index < uriString.Length - 1
        // uriString == "path/subpath?query";	index < uriString.Length - 1
        // uriString == "path/#";	index < uriString.Length - 1
        // uriString == "path/subpath#";	index < uriString.Length - 1
        // uriString == "path/?#";	index < uriString.Length - 1
        // uriString == "path/subpath?#";	index < uriString.Length - 1
        // uriString == "path/?query#";	index < uriString.Length - 1
        // uriString == "path/subpath?query#";	index < uriString.Length - 1
        // uriString == "path/#fragment";	index < uriString.Length - 1
        // uriString == "path/subpath#fragment";	index < uriString.Length - 1
        // uriString == "path/?#fragment";	index < uriString.Length - 1
        // uriString == "path/subpath?#fragment";	index < uriString.Length - 1
        // uriString == "path/?query#fragment";	index < uriString.Length - 1
        // uriString == "path/subpath?query#fragment";	index < uriString.Length - 1
        // uriString == "path/";	index == uriString.Length - 1
        throw new NotImplementedException();
    }

    /// <summary>
    /// Tries to parse a string as a URI.
    /// </summary>
    /// <param name="uriString">The source URI string.</param>
    /// <param name="kind">The kind of URI to parse.</param>
    /// <param name="uri">The parsed URI or <see langword="null" /> if the <paramref name="uriString" /> could not be parsed.</param>
    /// <returns><see langword="true" /> if the <paramref name="uriString" /> could be parsed; otherwise, <see langword="false" />.</returns>
    public static bool TryParse(string uriString, UriKind kind, [NotNullWhen(true)] out ParsedUri? uri) => TryParse(uriString, kind, NormalizationOptions.None, out uri);

    /// <summary>
    /// Tries to parse a string as a URI.
    /// </summary>
    /// <param name="uriString">The source URI string.</param>
    /// <param name="options">Normalization options.</param>
    /// <param name="uri">The parsed URI or <see langword="null" /> if the <paramref name="uriString" /> could not be parsed.</param>
    /// <returns><see langword="true" /> if the <paramref name="uriString" /> could be parsed; otherwise, <see langword="false" />.</returns>
    public static bool TryParse(string uriString, NormalizationOptions options, [NotNullWhen(true)] out ParsedUri? uri) => TryParse(uriString, UriKind.RelativeOrAbsolute, options, out uri);

    /// <summary>
    /// Tries to parse a string as a URI.
    /// </summary>
    /// <param name="uriString">The source URI string.</param>
    /// <param name="uri">The parsed URI or <see langword="null" /> if the <paramref name="uriString" /> could not be parsed.</param>
    /// <returns><see langword="true" /> if the <paramref name="uriString" /> could be parsed; otherwise, <see langword="false" />.</returns>
    public static bool TryParse(string uriString, [NotNullWhen(true)] out ParsedUri? uri) => TryParse(uriString, UriKind.RelativeOrAbsolute, NormalizationOptions.None, out uri);

    private static bool TryParseHttp(string scheme, string uriString, NormalizationOptions options, ushort defaultPort, out ParsedUri? uri)
    {
        if (uriString.Length < 3 || uriString[0] != DELIMITER_CHAR_SLASH || uriString[1] != DELIMITER_CHAR_SLASH ||
            !TrySplitPathQueryAndFragment(uriString[2..], options, out string? userInfo, out string hostName, out ushort? port, out string path, out string? query, out string? fragment)
            || hostName.Length == 0)
        {
            uri = null;
            return false;
        }
        if (options.HasFlag(NormalizationOptions.StripDefaultPort) && port.HasValue && port.Value == defaultPort)
            port = null;
        if (fragment is not null)
            fragment = UriDecode(fragment);
        uri = new(scheme, SCHEME_SEPARATOR_DSLASH, (userInfo is null) ?
            (port.HasValue ? new(UriDecode(hostName), port.Value) : new(hostName, defaultPort, true)) :
            port.HasValue ? new(UserInfo.Parse(userInfo, true), UriDecode(hostName), port.Value) : new(UserInfo.Parse(userInfo, true), UriDecode(hostName), defaultPort, true),
            PathSegment.Parse(path, options, true, true), (query is null) ? null : QuerySubComponent.Parse(query, true), fragment);
        return true;
    }

    private static bool TryParseFtp(string uriString, NormalizationOptions options, out ParsedUri? uri)
    {
        if (uriString.Length < 3 || uriString[0] != DELIMITER_CHAR_SLASH || uriString[1] != DELIMITER_CHAR_SLASH || !TrySplitPathAndFragment(uriString[2..], options, out string? userInfo, out string hostName, out ushort? port, out string path, out string? fragment))
        {
            uri = null;
            return false;
        }
        if (options.HasFlag(NormalizationOptions.StripDefaultPort) && port.HasValue && port.Value == PORT_NUMBER_FTP)
            port = null;
        throw new NotImplementedException();
    }

    private static bool TryParseSftp(string uriString, NormalizationOptions options, out ParsedUri? uri)
    {
        if (uriString.Length < 3 || uriString[0] != DELIMITER_CHAR_SLASH || uriString[1] != DELIMITER_CHAR_SLASH || !TrySplitPathAndFragment(uriString[2..], options, out string? userInfo, out string hostName, out ushort? port, out string path, out string? fragment))
        {
            uri = null;
            return false;
        }
        if (options.HasFlag(NormalizationOptions.StripDefaultPort) && port.HasValue && port.Value == PORT_NUMBER_SFTP)
            port = null;
        throw new NotImplementedException();
    }

    private static bool TryParseFile(string uriString, NormalizationOptions options, out ParsedUri? uri)
    {
        if (uriString.Length < 3 || uriString[0] != DELIMITER_CHAR_SLASH || uriString[1] != DELIMITER_CHAR_SLASH || (uriString = SplitPathAndFragment(uriString, options, out string? userInfo, out string hostName, out string? fragment)).Length == 0)
        {
            uri = null;
            return false;
        }
        
        throw new NotImplementedException();
    }

    private static bool TryParseGopher(string uriString, NormalizationOptions options, out ParsedUri? uri)
    {
        if (uriString.Length < 3 || uriString[0] != DELIMITER_CHAR_SLASH || uriString[1] != DELIMITER_CHAR_SLASH || !TrySplitPathAndFragment(uriString[2..], options, out string? userInfo, out string hostName, out ushort? port, out string path, out string? fragment) || hostName.Length == 0)
        {
            uri = null;
            return false;
        }
        if (options.HasFlag(NormalizationOptions.StripDefaultPort) && port.HasValue && port.Value == PORT_NUMBER_GOPHER)
            port = null;
        throw new NotImplementedException();
    }

    private static bool TryParseNntp(string uriString, NormalizationOptions options, out ParsedUri? uri)
    {
        if (uriString.Length < 3 || uriString[0] != DELIMITER_CHAR_SLASH || uriString[1] != DELIMITER_CHAR_SLASH || !TrySplitPathAndFragment(uriString[2..], options, out string? userInfo, out string hostName, out ushort? port, out string path, out string? fragment) || hostName.Length == 0)
        {
            uri = null;
            return false;
        }
        if (options.HasFlag(NormalizationOptions.StripDefaultPort) && port.HasValue && port.Value == PORT_NUMBER_NNTP)
            port = null;
        throw new NotImplementedException();
    }

    private static bool TryParseNews(string uriString, NormalizationOptions options, out ParsedUri? uri)
    {
        uriString = SplitFragment(uriString, options, out string? fragment);
        throw new NotImplementedException();
    }

    private static bool TryParseMailTo(string uriString, NormalizationOptions options, out ParsedUri? uri)
    {
        if (!TrySplitPathAndFragment(uriString, options, out string? userInfo, out string hostName, out ushort? port, out string path, out string? fragment) || hostName.Length == 0)
        {
            uri = null;
            return false;
        }
        throw new NotImplementedException();
    }

    private static bool TryParseUuid(string uriString, NormalizationOptions options, out ParsedUri? uri)
    {
        if ((uriString = SplitFragment(uriString, options, out string? fragment)).Length == 0)
        {
            uri = null;
            return false;
        }
        throw new NotImplementedException();
    }

    private static bool TryParseTelnet(string uriString, NormalizationOptions options, out ParsedUri? uri)
    {
        if (uriString.Length < 3 || uriString[0] != DELIMITER_CHAR_SLASH || uriString[1] != DELIMITER_CHAR_SLASH)
        {
            uri = null;
            return false;
        }
        uriString = SplitFragment(uriString[2..], options, out string? fragment);
        throw new NotImplementedException();
    }

    private static bool TryParseSsh(string uriString, NormalizationOptions options, out ParsedUri? uri)
    {
        if (uriString.Length < 3 || uriString[0] != DELIMITER_CHAR_SLASH || uriString[1] != DELIMITER_CHAR_SLASH)
        {
            uri = null;
            return false;
        }
        uriString = uriString[2..];
        throw new NotImplementedException();
    }

    private static bool TryParseLdap(string scheme, string uriString, NormalizationOptions options, int defaultPort, out ParsedUri? uri)
    {
        if (uriString.Length < 3 || uriString[0] != DELIMITER_CHAR_SLASH || uriString[1] != DELIMITER_CHAR_SLASH)
        {
            uri = null;
            return false;
        }
        uriString = SplitQueryAndFragment(uriString[2..], options, out string? query, out string? fragment);
        throw new NotImplementedException();
    }

    private static bool TryParseNetTcp(string uriString, NormalizationOptions options, out ParsedUri? uri)
    {
        if (uriString.Length < 3 || uriString[0] != DELIMITER_CHAR_SLASH || uriString[1] != DELIMITER_CHAR_SLASH)
        {
            uri = null;
            return false;
        }
        uriString = SplitQueryAndFragment(uriString[2..], options, out string? query, out string? fragment);
        throw new NotImplementedException();
    }

    private static bool TryParseNetPipe(string uriString, NormalizationOptions options, out ParsedUri? uri)
    {
        if (uriString.Length < 3 || uriString[0] != DELIMITER_CHAR_SLASH || uriString[1] != DELIMITER_CHAR_SLASH)
        {
            uri = null;
            return false;
        }
        uriString = SplitQueryAndFragment(uriString[2..], options, out string? query, out string? fragment);
        throw new NotImplementedException();
    }

    private static bool TryParseGeneric(string scheme, string uriString, NormalizationOptions options, out ParsedUri? uri)
    {
        uriString = SplitQueryAndFragment(uriString[2..], options, out string? query, out string? fragment);
        throw new NotImplementedException();
    }

    /// <summary>
    /// Tries to parse a UNIX path as a URI.
    /// </summary>
    /// <param name="path">The source unix path.</param>
    /// <param name="options">Normalization options.</param>
    /// <param name="uri">The parsed URI or <see langword="null" /> if the <paramref name="path" /> could not be parsed.</param>
    /// <returns><see langword="true" /> if the <paramref name="path" /> could be parsed; otherwise, <see langword="false" />.</returns>
    public static bool TryParseUnixPath(string? path, NormalizationOptions options, [NotNullWhen(true)] out ParsedUri? uri)
    {
        if (string.IsNullOrEmpty(path))
        {
            uri = Empty;
            return true;
        }
        string[] ps;
        if (UnixPathRegex.IsMatch(path))
        {
            ps = path.Split(DELIMITER_CHAR_SLASH);
            bool isRooted = ps[0].Length == 0;
            if (options.HasFlag(NormalizationOptions.StripEmptyPathSegments))
            {
                if (options.HasFlag(NormalizationOptions.NormalizeDotPathSegments))
                    ps = NormalizeDotPathSegments(ps.Take(1).Concat(ps.Skip(1).Where(s => s.Length > 0)));
                else if (ps.Skip(1).Any(s => s.Length == 0))
                    ps = ps.Take(1).Concat(ps.Skip(1).Where(s => s.Length > 0)).ToArray();
            }
            else if (options.HasFlag(NormalizationOptions.NormalizeDotPathSegments))
                ps = NormalizeDotPathSegments(ps);
            if (options.HasFlag(NormalizationOptions.EnsureTrailingPathSeparator))
                EnsureTrailingEmpty(ref ps);
            else if (options.HasFlag(NormalizationOptions.StripTrailingPathSeparator))
                StripTrailingEmpty(ref ps);
            if (isRooted)
                uri = ps.Length switch
                {
                    0 => new ParsedUri("file", "//", UriAuthority.Empty, PathSegment.EmptyRoot),
                    1 => new ParsedUri("file", "//", UriAuthority.Empty, (ps[0].Length > 0) ? new PathSegment(DELIMITER_CHAR_SLASH, ps[0]) : PathSegment.EmptyRoot),
                    _ => new ParsedUri("file", "//", UriAuthority.Empty, ((ps[0].Length > 0) ? ps : ps.Skip(1))
                        .Select(p => (p.Length > 0) ? new PathSegment(DELIMITER_CHAR_SLASH, p) : PathSegment.EmptyRoot)),
                };
            else
                uri = (ps.Length > 0) ? new ParsedUri(ps.Take(1).Select(p => new PathSegment(p)).Concat(ps.Skip(1).Select(p => new PathSegment(DELIMITER_CHAR_SLASH, p)))) : Empty;
            return true;
        }
        uri = null;
        return false;
    }

    /// <summary>
    /// Tries to parse a DOS path with a drive letter or a UNC path as a URI.
    /// </summary>
    /// <param name="path">The source windows path.</param>
    /// <param name="options">Normalization options.</param>
    /// <param name="includePsPath"><see langword="true" /> to attempt to parse a PowerShell path if a DOS or UNC path could not be parsed;
    /// otherwise, <see langword="false" /> to only attempt to parse a DOS or UNC path.</param>
    /// <param name="uri">The parsed URI or <see langword="null" /> if the <paramref name="path" /> could not be parsed.</param>
    /// <returns><see langword="true" /> if the <paramref name="path" /> could be parsed; otherwise, <see langword="false" />.</returns>
    public static bool TryParseWindowsPath(string? path, NormalizationOptions options, bool includePsPath, [NotNullWhen(true)] out ParsedUri? uri)
    {
        if (string.IsNullOrEmpty(path))
        {
            uri = Empty;
            return true;
        }
        Match match = DosPathRegex.Match(path);
        Group g;
        if (!match.Success)
        {
            if ((match = UncPathLooseRegex.Match(path)).Success)
            {
                uri = ParseUncPath(match, options);
                return true;
            }
            if (!(includePsPath && (match = PsPathRegex.Match(path)).Success))
            {
                uri = null;
                return false;
            }
        }
        uri = ParseDosPath(match, options);
        return true;
    }

    /// <summary>
    /// Tries to parse a DOS path with a drive letter or a UNC path as a URI.
    /// </summary>
    /// <param name="path">The source windows path.</param>
    /// <param name="options">Normalization options.</param>
    /// <param name="uri">The parsed URI or <see langword="null" /> if the <paramref name="path" /> could not be parsed.</param>
    /// <returns><see langword="true" /> if the <paramref name="path" /> could be parsed; otherwise, <see langword="false" />.</returns>
    public static bool TryParseWindowsPath(string? path, NormalizationOptions options, [NotNullWhen(true)] out ParsedUri? uri) => TryParseWindowsPath(path, options, false, out uri);

    private static ParsedUri ParseUncPath(Match match, NormalizationOptions options)
    {
        string hostName = match.Groups[GROUP_NAME_host].Value;
        Group g = match.Groups[GROUP_NAME_path];
        if (g.Success)
        {
            string[] segments = DosPathDelimiterRegex.Split(g.Value);
            if (options.HasFlag(NormalizationOptions.StripEmptyPathSegments))
            {
                if (options.HasFlag(NormalizationOptions.NormalizeDotPathSegments))
                {
                    segments = NormalizeDotPathSegments(segments.Where(s => s.Length > 0));
                    if (options.HasFlag(NormalizationOptions.EnsureTrailingPathSeparator))
                        EnsureTrailingEmpty(ref segments);
                    else if (options.HasFlag(NormalizationOptions.StripTrailingPathSeparator))
                        StripTrailingEmpty(ref segments);
                }
                else if (segments.Any(s => s.Length == 0))
                    segments = segments.Where(s => s.Length > 0).ToArray();
            }
            else if (options.HasFlag(NormalizationOptions.NormalizeDotPathSegments))
                segments = NormalizeDotPathSegments(segments);
            if (options.HasFlag(NormalizationOptions.EnsureTrailingPathSeparator))
                EnsureTrailingEmpty(ref segments);
            else if (options.HasFlag(NormalizationOptions.StripTrailingPathSeparator))
                StripTrailingEmpty(ref segments);
            if (segments.Length > 0)
                return new("file", "//", new UriAuthority(hostName), segments.Select(s => (s.Length > 0) ? new PathSegment(DELIMITER_CHAR_SLASH, s) : PathSegment.EmptyRoot));
        }
        return new("file", "//", new UriAuthority(hostName), PathSegment.EmptyRoot);
    }

    private static ParsedUri ParseDosPath(Match match, NormalizationOptions options)
    {
        string drive = match.Groups[GROUP_NAME_drive].Value;
        Group g = match.Groups[GROUP_NAME_path]; 
        if (g.Success)
        {
            string[] segments = DosPathDelimiterRegex.Split(g.Value);
            if (options.HasFlag(NormalizationOptions.StripEmptyPathSegments))
            {
                if (options.HasFlag(NormalizationOptions.NormalizeDotPathSegments))
                {
                    segments = NormalizeDotPathSegments(segments.Where(s => s.Length > 0));
                    if (options.HasFlag(NormalizationOptions.EnsureTrailingPathSeparator))
                        EnsureTrailingEmpty(ref segments);
                    else if (options.HasFlag(NormalizationOptions.StripTrailingPathSeparator))
                        StripTrailingEmpty(ref segments);
                }
                else if (segments.Any(s => s.Length == 0))
                    segments = segments.Where(s => s.Length > 0).ToArray();
            }
            else if (options.HasFlag(NormalizationOptions.NormalizeDotPathSegments))
                segments = NormalizeDotPathSegments(segments);
            if (options.HasFlag(NormalizationOptions.EnsureTrailingPathSeparator))
                EnsureTrailingEmpty(ref segments);
            else if (options.HasFlag(NormalizationOptions.StripTrailingPathSeparator))
                StripTrailingEmpty(ref segments);
            if (segments.Length > 0)
                return new("file", "//", UriAuthority.Empty, new string[] { drive }.Concat(segments).Select(s => (s.Length > 0) ? new PathSegment(DELIMITER_CHAR_SLASH, s) : PathSegment.EmptyRoot));
        }
        return new("file", "//", UriAuthority.Empty, new PathSegment(DELIMITER_CHAR_SLASH, drive), PathSegment.EmptyRoot);
    }

    private static string[] NormalizeDotPathSegments(IEnumerable<string> segments)
    {
        if (segments.Contains(".."))
        {
            List<string> s = segments.Where(p => p != ".").ToList();
            int index = 0;
            while (index < s.Count)
            {
                if (s[index] == "..")
                {
                    s.RemoveAt(index);
                    if (index > 0)
                    {
                        index--;
                        s.RemoveAt(index);
                    }
                }
                else
                    index++;
            }
            return segments.ToArray();
        }
        if (segments.Contains("."))
            return segments.Where(p => p != ".").ToArray();
        return (segments is string[] sArr) ? sArr : segments.ToArray();
    }

    private static void StripTrailingEmpty(ref string[] segments)
    {
        if (segments.Length > 1 && segments[^1].Length == 0)
        {
            int index = segments.Length - 2;
            while (index > 0 && segments[index].Length == 0)
                index--;
            Array.Resize(ref segments, index + 1);
        }
    }

    private static void EnsureTrailingEmpty(ref string[] segments)
    {
        if (segments.Length == 0 || segments[^1].Length > 0)
        {
            Array.Resize(ref segments, segments.Length + 1);
            segments[^1] = string.Empty;
        }
    }

    private static string SplitFragment(string uriString, NormalizationOptions options, out string? fragment)
    {
        if (string.IsNullOrEmpty(uriString))
        {
            fragment = null;
            return string.Empty;
        }
        int index = uriString.IndexOf(DELIMITER_CHAR_HASH);
        if (index < 0)
        {
            fragment = null;
            return uriString;
        }
        fragment = ((index > 0) ? index < uriString.Length - 1 : uriString.Length > 1) ? uriString[(index + 1)..] : options.HasFlag(NormalizationOptions.StripEmptyFragment) ? null : string.Empty;
        return uriString[..index];
    }

    private static bool TrySplitPathQueryAndFragment(string uriString, NormalizationOptions options, out string? userInfo, out string hostName, out ushort? port, out string path, out string? query, out string? fragment)
    {
        Match match = UserInfoHostAndPortRegex.Match(SplitQueryAndFragment(uriString, options, out query, out fragment));
        Group g = match.Groups[GROUP_NAME_port];
        if (g.Success)
        {
            if (!ushort.TryParse(g.Value, out ushort p))
            {
                userInfo = query = fragment = null;
                hostName = path = string.Empty;
                port = null;
                return false;
            }
            port = p;
        }
        else
            port = null;
        userInfo = (g = match.Groups[GROUP_NAME_user]).Success ? g.Value : null;
        hostName = match.Groups[GROUP_NAME_host].Value;
        path = match.Groups[GROUP_NAME_path].Value;
        return true;
    }

    private static string SplitPathQueryAndFragment(string uriString, NormalizationOptions options, out string? userInfo, out string hostName, out string? query, out string? fragment)
    {
        Match match = UserInfoHostAndPortRegex.Match(SplitQueryAndFragment(uriString, options, out query, out fragment));
        Group g = match.Groups[GROUP_NAME_user];
        userInfo = g.Success ? g.Value : null;
        hostName = match.Groups[GROUP_NAME_host].Value;
        return match.Groups[GROUP_NAME_path].Value;
    }

    private static bool TrySplitPathAndFragment(string uriString, NormalizationOptions options, out string? userInfo, out string hostName, out ushort? port, out string path, out string? fragment)
    {
        Match match = UserInfoHostAndPortRegex.Match(SplitFragment(uriString, options, out fragment));
        Group g = match.Groups[GROUP_NAME_port];
        if (g.Success)
        {
            if (!ushort.TryParse(g.Value, out ushort p))
            {
                userInfo = fragment = null;
                hostName = path = string.Empty;
                port = null;
                return false;
            }
            port = p;
        }
        else
            port = null;
        userInfo = (g = match.Groups[GROUP_NAME_user]).Success ? g.Value : null;
        hostName = match.Groups[GROUP_NAME_host].Value;
        path = match.Groups[GROUP_NAME_path].Value;
        return true;
    }

    private static string SplitPathAndFragment(string uriString, NormalizationOptions options, out string? userInfo, out string hostName, out string? fragment)
    {
        Match match = UserInfoHostAndPortRegex.Match(SplitFragment(uriString, options, out fragment));
        Group g = match.Groups[GROUP_NAME_user];
        userInfo = g.Success ? g.Value : null;
        hostName = match.Groups[GROUP_NAME_host].Value;
        return match.Groups[GROUP_NAME_path].Value;
    }

    private static string SplitQueryAndFragment(string uriString, NormalizationOptions options, out string? query, out string? fragment)
    {
        if (string.IsNullOrEmpty(uriString))
        {
            query = fragment = null;
            return string.Empty;
        }
        if (uriString.Length == 1)
        {
            switch (uriString[0])
            {
                case DELIMITER_CHAR_HASH:
                    query = null;
                    fragment = options.HasFlag(NormalizationOptions.StripEmptyFragment) ? null : string.Empty;
                    break;
                case DELIMITER_CHAR_QUERY:
                    fragment = null;
                    query = options.HasFlag(NormalizationOptions.StripEmptyQuery) ? null : string.Empty;
                    break;
                default:
                    query = fragment = null;
                    break;
            }
            return string.Empty;
        }
        
        int index = uriString.IndexOf(DELIMITER_CHAR_HASH);
        if ((index) < 0)
        {
            fragment = null;
            if ((index = uriString.IndexOf(DELIMITER_CHAR_QUERY)) < 0)
            {
                query = null;
                return string.Empty;
            }
            if (index == 0)
            {
                query = uriString[1..];
                return string.Empty;
            }
            query = (index < uriString.Length - 1) ? uriString[(index + 1)..] : options.HasFlag(NormalizationOptions.StripEmptyQuery) ? null : string.Empty;
            return uriString[..index];
        }
        if (index == 0)
        {
            fragment = uriString[1..];
            query = null;
            return string.Empty;
        }
        fragment = (index < uriString.Length - 1) ? uriString[(index + 1)..] : options.HasFlag(NormalizationOptions.StripEmptyFragment) ? null : string.Empty;
        if ((index = (uriString = uriString[..index]).IndexOf('?')) < 0)
            query = null;
        else if (index == 0)
            query = (uriString.Length > 1) ? uriString[1..] : options.HasFlag(NormalizationOptions.StripEmptyQuery) ? null : string.Empty;
        else
        {
            query = (index < uriString.Length - 1) ? uriString[(index + 1)..] : options.HasFlag(NormalizationOptions.StripEmptyQuery) ? null : string.Empty;
            return uriString[..index];
        }
        return string.Empty;
    }

    public bool Equals(ParsedUri? other)
    {
        throw new NotImplementedException();
    }

    public override bool Equals(object? obj) => Equals(obj as ParsedUri);

    public int CompareTo(ParsedUri? other)
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

    public static bool operator ==(ParsedUri? left, ParsedUri? right) => (left is null) ? right is null : left is not null && left.Equals(right);

    public static bool operator !=(ParsedUri? left, ParsedUri? right) => (left is null) ? right is not null : left is null || !left.Equals(right);

    public static bool operator <(ParsedUri? left, ParsedUri? right) => (left is null) ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(ParsedUri? left, ParsedUri? right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(ParsedUri? left, ParsedUri? right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(ParsedUri? left, ParsedUri? right) => (left is null) ? right is null : left.CompareTo(right) >= 0;
}