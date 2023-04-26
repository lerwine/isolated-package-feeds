using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CdnGetter;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class UriSchemaTypeAttribute : Attribute
{
    public string Name { get; set; } = string.Empty;

    public ushort? DefaultPort { get; }

    public URI.SyntaxFlags Flags { get; }
    
    public UriSchemaTypeAttribute(ushort defaultPort, URI.SyntaxFlags flags)
    {
        DefaultPort = defaultPort;
        Flags = flags;
    }
    
    public UriSchemaTypeAttribute(URI.SyntaxFlags flags)
    {
        DefaultPort = null;
        Flags = flags;
    }
}
/// <summary>
/// Object representation of the decoded components of a uniform resource identifier (URI), which is an alternative to the <see cref="Uri" /> class.
/// </summary>
public class URI
{
    private const char SCHEME_DELIMITER_CHAR = ':';
    private const char PATH_DELIMITER_CHAR = '/';
    private const char ALT_PATH_DELIMITER_CHAR = '\\';
    private const char QUERY_DELIMITER_CHAR = '?';
    private const char PARAMETER_DELIMITER_CHAR = '&';
    private const char FRAGMENT_DELIMITER_CHAR = '#';

    private static readonly StringComparer Comparer = StringComparer.InvariantCultureIgnoreCase;

    /// <summary>
    /// Matches a path separator character.
    /// </summary>
    private static readonly Regex PathSeparatorRegex = new(@"[\\/:]", RegexOptions.Compiled);
    private static readonly Regex AltPathSegmentRegex = new(@"([\\/:]+)([^\\/:]*)", RegexOptions.Compiled);
    
    private static readonly Regex PathSegmentEncodeRegex = new(@"[^!$&'()*+,:;@=\w.~-]", RegexOptions.Compiled);
    
    public static readonly Regex QueryKeyEncodeRegex = new(@"[^!$'()*+,:;@/\w.~-]", RegexOptions.Compiled);
    
    private static readonly Regex QueryValueEncodeRegex = new(@"[^!$'()*+,:;@=/\w.~-]", RegexOptions.Compiled);
    
    private static readonly Regex FragmentEEncodeRegex = new(@"[^!$&'()*+,:;@=/?\w.~-]", RegexOptions.Compiled);
    /// <summary>
    /// Matches a URI-encoded character sequence.
    /// </summary>
    private static readonly Regex EncodedSequenceRegex = new(@"%[\da-f]{2}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a URI scheme name.
    /// </summary>
    private static readonly Regex ValidSchemeRegex = new(@"^[a-z][a-z\d+.-]*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a Host or UserName character that should be encoded.
    /// </summary>
    private static readonly Regex NameEncodeRegex = new(@"[^!$&'()*+,;=\w.~-]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a Host or UserName character that is not properly encoded.
    /// </summary>
    private static readonly Regex NameEncodeRegexAlt = new(@"%(?![\da-f]{2})|[^%!$&'()*+,;=\w.~-]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a Pasword character that should be encoded.
    /// </summary>
    private static readonly Regex PasswordEncodeRegex = new(@"[^!$&'()*+,;=\w.~:-]", RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a password character that is not properly encoded.
    /// </summary>
    private static readonly Regex PasswordEncodeRegexAlt = new(@"%(?![\da-f]{2})|[^%!$&'()*+,;=\w.~:-]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    /// <summary>
    /// Matches a valid URI-encoded user info component.
    /// </summary>
    private static readonly Regex UserInfoRegex = new(@"(?<username>([!$&'()*+,;=\w.~-]|%[\da-f]{2})*)(?::(?<pw>([!$&'()*+,;=\w.~:-]|%[\da-f]{2})*))?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    private const string GROUP_NAME_scheme = "scheme";
    private const string GROUP_NAME_sep = "sep";
    private const string GROUP_NAME_user = "user";
    private const string GROUP_NAME_host = "host";
    private const string GROUP_NAME_port = "port";
    private const string GROUP_NAME_path = "path";
    private const string GROUP_NAME_query = "query";
    private const string GROUP_NAME_fragment = "fragment";
    /// <summary>
    /// Pattern for parsing URI components.
    /// </summary>
    private static readonly Regex UrlParseCopmonentsRegex = new($@"^((?<{GROUP_NAME_scheme}>[^:/\\@?#]+):(?<{GROUP_NAME_sep}>//?)?((?<{GROUP_NAME_user}>[^/\\@?#]+)@)?(?<{GROUP_NAME_host}>[^/\\:?#]*)(:(?<{GROUP_NAME_port}>\d+))?)?(?<{GROUP_NAME_path}>[^?#]+)(\?(?<{GROUP_NAME_query}>[^#]*))?(#(?<{GROUP_NAME_fragment}>.*))?$", RegexOptions.Compiled);
    
    private UriSchemeType? _type;

    public string? Scheme { get; }

    public IAuthority? Authority { get; }

    public ReadOnlyCollection<IPathSegment> PathSegments { get; }

    public ReadOnlyCollection<IQuerySubComponent>? Query { get; }

    public string? Fragment { get; }

    private URI(string? scheme, UriSchemeType? type, IAuthority? authority, IEnumerable<IPathSegment> pathSegments, IEnumerable<IQuerySubComponent>? query, string? fragment)
    {
        _type = type;
        Scheme = scheme;
        Authority = authority;
        PathSegments = new ReadOnlyCollection<IPathSegment>(pathSegments.ToArray());
        Query = (query is null) ? null : new ReadOnlyCollection<IQuerySubComponent>(query.ToArray());
    }
    
    public static bool TryParse(string? uriString, [NotNullWhen(true)] out URI? uri)
    {
        if (string.IsNullOrEmpty(uriString))
        {
            uri = new(null, null, null, Enumerable.Empty<IPathSegment>(), null, null);
            return true;
        }
        Match match = UrlParseCopmonentsRegex.Match(uriString);
        if (match.Success)
        {
            Group g = match.Groups[GROUP_NAME_scheme];
            if (g.Success)
            {
                string scheme = g.Value.ToLower();
                switch (g.Value.ToLower())
                {
                    case "http":
                    case "ws":
                        return TryParseHttp(scheme, match, 80, out uri);
                    case "https":
                    case "wss":
                        return TryParseHttp(scheme, match, 443, out uri);
                    case "ftp":
                        return TryParseFtp(match, out uri);
                    case "sftp":
                        return TryParseSftp(match, out uri);
                    case "file":
                        return TryParseFile(match, out uri);
                    case "gopher":
                        return TryParseGopher(match, out uri);
                    case "nntp":
                        return TryParseNntp(match, out uri);
                    case "news":
                        return TryParseNews(match, out uri);
                    case "mailto":
                        return TryParseMailTo(match, out uri);
                    case "uuid":
                        return TryParseUuid(match, out uri);
                    case "telnet":
                        return TryParseTelnet(match, out uri);
                    case "ssh":
                        return TryParseSsh(match, out uri);
                    case "ldap":
                        return TryParseLdap(scheme, match, 389, out uri);
                    case "ldaps":
                        return TryParseLdap(scheme, match, 636, out uri);
                    case "net.tcp":
                        return TryParseNetTcp(match, out uri);
                    case "net.pipe":
                        return TryParseNetPipe(match, out uri);
                    default:
                        return TryParseGeneric(scheme, match, out uri);
                }
            }
        }
        throw new NotImplementedException();
    }

    private static bool TryParseHttp(string scheme, Match match, int defaultPort, out URI? uri)
    {
        Group g = match.Groups[GROUP_NAME_host];
        if (g.Length > 0)
        {
            string? userInfo = ((g = match.Groups[GROUP_NAME_user]).Success) ? g.Value : null;
            string hostName = g.Value;
            if ((g = match.Groups[GROUP_NAME_port]).Success)
            {
                if (ushort.TryParse(g.Value, out ushort explicitPort))
                {

                }
            }
        }
        throw new NotImplementedException();
    }

    private static bool TryParseFtp(Match match, out URI? uri)
    {
        throw new NotImplementedException();
    }

    private static bool TryParseSftp(Match match, out URI? uri)
    {
        throw new NotImplementedException();
    }

    private static bool TryParseFile(Match match, out URI? uri)
    {
        throw new NotImplementedException();
    }

    private static bool TryParseGopher(Match match, out URI? uri)
    {
        throw new NotImplementedException();
    }

    private static bool TryParseNntp(Match match, out URI? uri)
    {
        throw new NotImplementedException();
    }

    private static bool TryParseNews(Match match, out URI? uri)
    {
        throw new NotImplementedException();
    }

    private static bool TryParseMailTo(Match match, out URI? uri)
    {
        throw new NotImplementedException();
    }

    private static bool TryParseUuid(Match match, out URI? uri)
    {
        throw new NotImplementedException();
    }

    private static bool TryParseTelnet(Match match, out URI? uri)
    {
        throw new NotImplementedException();
    }

    private static bool TryParseSsh(Match match, out URI? uri)
    {
        throw new NotImplementedException();
    }

    private static bool TryParseLdap(string scheme, Match match, int defaultPort, out URI? uri)
    {
        throw new NotImplementedException();
    }

    private static bool TryParseNetTcp(Match match, out URI? uri)
    {
        throw new NotImplementedException();
    }

    private static bool TryParseNetPipe(Match match, out URI? uri)
    {
        throw new NotImplementedException();
    }

    private static bool TryParseGeneric(string scheme, Match match, out URI? uri)
    {
        throw new NotImplementedException();
    }
    
    public interface IUserInfo : IEquatable<IUserInfo>, IComparable<IUserInfo>
    {
        string? Password { get; }
        string UserName { get; }
    }

    public interface IAuthority : IEquatable<IAuthority>, IComparable<IAuthority>
    {
        IUserInfo? UserInfo { get; }
        string HostName { get; }
        ushort? ExplicitPort { get; }
        ushort? Port { get; }
    }

    public interface IPathSegment : IEquatable<IPathSegment>, IComparable<IPathSegment>
    {
        char? Separator { get; }
        string Name { get; }
    }

    public interface IQuerySubComponent : IEquatable<IQuerySubComponent>, IComparable<IQuerySubComponent>
    {
        string? Separator { get; }
        string Key { get; }
        string? Value { get; }
    }
    
    [Flags]
    public enum SyntaxFlags : ushort
    {
        SchemaSeparatorOnly             = 0b0000_0000_0000_0000,
        SchemaSeparatorPlusSlash        = 0b0000_0000_0000_0001,
        SchemaSeparatorPlusDoubleSlash  = 0b0000_0000_0000_0010,
        AllowHost                       = 0b0000_0000_0000_0100,
        AllowEmptyHost                  = 0b0000_0000_0000_1100,
        RequireHost                     = 0b0000_0000_0001_0100,
        AllowUserName                   = 0b0000_0000_0010_0100,
        AllowPassword                   = 0b0000_0000_0110_0100,
        // sftp://[<user>[;fingerprint=<host-key fingerprint>]@]<host>[:<port>]/<path>/<file>
        AllowFingerprint                = 0b0000_0000_1010_0100,
        RequireUserName                 = 0b0000_0001_0010_0100,
        AllowPort                       = 0b0000_0010_0000_0100,
        AllowPath                       = 0b0000_0100_0000_0000,
        RequirePath                     = 0b0000_1100_0000_0000,
        PathIsRooted                    = 0b0001_0100_0000_0000,
        AllowQuery                      = 0b0010_0000_0000_0000,
        AllowFragment                   = 0b0100_0000_0000_0000
    }

    public enum UriSchemeType
    {
        [UriSchemaType(80, SyntaxFlags.SchemaSeparatorPlusDoubleSlash | SyntaxFlags.RequireHost | SyntaxFlags.AllowPassword | SyntaxFlags.AllowPort | SyntaxFlags.PathIsRooted | SyntaxFlags.AllowQuery | SyntaxFlags.AllowFragment)]
        http,
        
        [UriSchemaType(443, SyntaxFlags.SchemaSeparatorPlusDoubleSlash | SyntaxFlags.RequireHost | SyntaxFlags.AllowPassword | SyntaxFlags.AllowPort | SyntaxFlags.PathIsRooted | SyntaxFlags.AllowQuery | SyntaxFlags.AllowFragment)]
        https,
        
        [UriSchemaType(80, SyntaxFlags.SchemaSeparatorPlusDoubleSlash | SyntaxFlags.RequireHost | SyntaxFlags.AllowPassword | SyntaxFlags.AllowPort | SyntaxFlags.PathIsRooted | SyntaxFlags.AllowQuery | SyntaxFlags.AllowFragment)]
        ws,
        
        [UriSchemaType(443, SyntaxFlags.SchemaSeparatorPlusDoubleSlash | SyntaxFlags.RequireHost | SyntaxFlags.AllowPassword | SyntaxFlags.AllowPort | SyntaxFlags.PathIsRooted | SyntaxFlags.AllowQuery | SyntaxFlags.AllowFragment)]
        wss,

        [UriSchemaType(21, SyntaxFlags.SchemaSeparatorPlusDoubleSlash | SyntaxFlags.RequireHost | SyntaxFlags.AllowPassword | SyntaxFlags.AllowPort | SyntaxFlags.PathIsRooted | SyntaxFlags.AllowFragment)]
        ftp,

        [UriSchemaType(22, SyntaxFlags.SchemaSeparatorPlusDoubleSlash | SyntaxFlags.RequireHost | SyntaxFlags.AllowFingerprint | SyntaxFlags.AllowPort | SyntaxFlags.PathIsRooted | SyntaxFlags.AllowFragment)]
        sftp,

        [UriSchemaType(SyntaxFlags.SchemaSeparatorPlusDoubleSlash | SyntaxFlags.RequireHost | SyntaxFlags.AllowPassword | SyntaxFlags.AllowEmptyHost | SyntaxFlags.PathIsRooted | SyntaxFlags.AllowFragment)]
        file,

        [UriSchemaType(70, SyntaxFlags.SchemaSeparatorPlusDoubleSlash | SyntaxFlags.RequireHost | SyntaxFlags.AllowPassword | SyntaxFlags.PathIsRooted | SyntaxFlags.AllowFragment)]
        gopher,

        [UriSchemaType(119, SyntaxFlags.SchemaSeparatorPlusDoubleSlash | SyntaxFlags.RequireHost | SyntaxFlags.AllowPassword | SyntaxFlags.AllowPort | SyntaxFlags.PathIsRooted | SyntaxFlags.AllowFragment)]
        nntp,

        [UriSchemaType(SyntaxFlags.AllowPath | SyntaxFlags.AllowFragment)]
        news,

        [UriSchemaType(SyntaxFlags.RequireUserName | SyntaxFlags.RequireHost | SyntaxFlags.AllowPort | SyntaxFlags.AllowPath | SyntaxFlags.AllowQuery | SyntaxFlags.AllowFragment)]
        mailto,

        [UriSchemaType(SyntaxFlags.AllowPath | SyntaxFlags.AllowFragment)]
        uuid,

        [UriSchemaType(23, SyntaxFlags.SchemaSeparatorPlusDoubleSlash | SyntaxFlags.RequireHost | SyntaxFlags.AllowPassword | SyntaxFlags.AllowPort | SyntaxFlags.PathIsRooted | SyntaxFlags.AllowFragment)]
        telnet,

        [UriSchemaType(22, SyntaxFlags.SchemaSeparatorPlusDoubleSlash | SyntaxFlags.RequireHost | SyntaxFlags.AllowFingerprint | SyntaxFlags.AllowPort)]
        ssh,

        [UriSchemaType(389, SyntaxFlags.SchemaSeparatorPlusDoubleSlash | SyntaxFlags.RequireHost | SyntaxFlags.AllowEmptyHost | SyntaxFlags.AllowPassword | SyntaxFlags.AllowPort | SyntaxFlags.PathIsRooted | SyntaxFlags.AllowQuery | SyntaxFlags.AllowFragment)]
        ldap,

        [UriSchemaType(636, SyntaxFlags.SchemaSeparatorPlusDoubleSlash | SyntaxFlags.RequireHost | SyntaxFlags.AllowEmptyHost | SyntaxFlags.AllowPassword | SyntaxFlags.AllowPort | SyntaxFlags.PathIsRooted | SyntaxFlags.AllowQuery | SyntaxFlags.AllowFragment)]
        ldaps,

        [UriSchemaType(808, SyntaxFlags.SchemaSeparatorPlusDoubleSlash | SyntaxFlags.RequireHost | SyntaxFlags.AllowPort | SyntaxFlags.PathIsRooted | SyntaxFlags.AllowQuery | SyntaxFlags.AllowFragment, Name = "net.tcp")]
        net_tcp,

        [UriSchemaType(SyntaxFlags.SchemaSeparatorPlusDoubleSlash | SyntaxFlags.RequireHost | SyntaxFlags.PathIsRooted | SyntaxFlags.AllowQuery | SyntaxFlags.AllowFragment, Name = "net.pipe")]
        net_pipe,

        [UriSchemaType(SyntaxFlags.RequireHost | SyntaxFlags.AllowQuery)]
        bitcoin,

        [UriSchemaType(9418, SyntaxFlags.SchemaSeparatorPlusDoubleSlash | SyntaxFlags.RequireHost | SyntaxFlags.AllowPassword | SyntaxFlags.AllowPort | SyntaxFlags.RequirePath | SyntaxFlags.PathIsRooted)]
        git
    }
}
