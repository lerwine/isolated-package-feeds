using System.Text;

namespace CdnGetter;

public partial class ParsedUri
{
    /// <summary>
    /// Represents the authority components for a URI.
    /// </summary>
    public class UriAuthority : IEquatable<UriAuthority>, IComparable<UriAuthority>
    {
        /// <summary>
        /// Gets the user info URI component.
        /// </summary>
        /// <value>The user info URI component or <see langword="null" /> if there is no user info component.</value>
        public UserInfo? UserInfo { get; }

        /// <summary>
        /// Gets the host name component.
        /// </summary>
        public string HostName { get; }

        /// <summary>
        /// Gets the port number.
        /// </summary>
        /// <value>The port number or <see langword="null" /> if there is no port component.</value>
        public ushort? Port { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Port" /> is implicit, based upon the associated <see cref="SchemeName" />.
        /// </summary>
        /// <value><see langword="true" /> if the <see cref="Port" /> is implied, based upon the associated <see cref="SchemeName" />;
        /// otherwise <see langword="false" /> if the <see cref="Port" /> is explicitly specified.</value>
        public bool PortIsImplicit { get; }

        /// <summary>
        /// Creates a new <c>UriAuthority</c> object.
        /// </summary>
        /// <param name="userInfo">The user info.</param>
        /// <param name="hostName">The host name.</param>
        /// <param name="port">The port number.</param>
        /// <param name="portIsImplicit"><see langword="true" /> if the <paramref name="port" /> is implied, based upon the associated <see cref="SchemeName" />;
        /// otherwise <see langword="false" /> if the <paramref name="port" /> is explicitly specified.</param>
        public UriAuthority(UserInfo userInfo, string hostName, ushort port, bool portIsImplicit = false) => (UserInfo, HostName, Port, PortIsImplicit) = (userInfo, hostName ?? "", port, portIsImplicit);

        /// <summary>
        /// Creates a new <c>UriAuthority</c> object.
        /// </summary>
        /// <param name="userInfo">The user info.</param>
        /// <param name="hostName">The host name.</param>
        public UriAuthority(UserInfo userInfo, string hostName) => (UserInfo, HostName) = (userInfo, hostName ?? "");

        /// <summary>
        /// Creates a new <c>UriAuthority</c> object.
        /// </summary>
        /// <param name="hostName">The host name.</param>
        public UriAuthority(string hostName) => HostName = hostName ?? "";

        /// <summary>
        /// Creates a new <c>UriAuthority</c> object.
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="hostName">The host name.</param>
        /// <param name="port">The port number.</param>
        /// <param name="portIsImplicit"><see langword="true" /> if the <paramref name="port" /> is implied, based upon the associated <see cref="SchemeName" />;
        /// otherwise <see langword="false" /> if the <paramref name="port" /> is explicitly specified.</param>
        public UriAuthority(string hostName, ushort port, bool portIsImplicit = false) => (HostName, Port, PortIsImplicit) = (hostName ?? "", port, portIsImplicit);

        /// <summary>
        /// URI authority with no user info or port, and an empty host name.
        /// </summary>
        public static readonly UriAuthority Empty = new(string.Empty);

        public bool Equals(UriAuthority? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (UserInfo is null)
            {
                if (other.UserInfo is not null)
                    return false;
            }
            else if (!UserInfo.Equals(other.UserInfo))
                return false;
            if (!DefaultComponentComparer.Equals(HostName, other.HostName))
                return false;
            return Port.HasValue ? other.Port.HasValue && Port.Value == other.Port.Value : !other.Port.HasValue;
        }

        public override bool Equals(object? obj) => Equals(obj as UriAuthority);

        public int CompareTo(UriAuthority? other)
        {
            if (other is null)
                return 1;
            if (ReferenceEquals(this, other))
                return 0;
            int result;
            if (UserInfo is null)
            {
                if (other.UserInfo is not null)
                    return -1;
            }
            else if ((result = UserInfo.CompareTo(other.UserInfo)) != 0)
                return result;
            if ((result = DefaultComponentComparer.Compare(HostName, other.HostName)) != 0)
                return result;
            if (Port.HasValue)
                return other.Port.HasValue ? Port.Value.CompareTo(other.Port.Value) : 1;
            return other.Port.HasValue ? -1 : 0;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = ((UserInfo is null) ? 5 : 55 + UserInfo.GetHashCode()) * 11 + HostName.GetHashCode();
                return Port.HasValue ? hash * 11 + Port.GetHashCode() : hash;
            }
        }

        public override string ToString()
        {
            if (UserInfo is null)
            {
                if (HostName.Length > 0)
                    return (Port.HasValue && !PortIsImplicit) ? $"{Encode(HostName, NameEncodeRegex)}:{Port.Value}" : Encode(HostName, NameEncodeRegex);
                return (Port.HasValue && !PortIsImplicit) ? $":{Port.Value}" : string.Empty;
            }
            string userInfo = UserInfo.ToString();
            if (userInfo.Length > 0)
            {
                if (HostName.Length > 0)
                    return (Port.HasValue && !PortIsImplicit) ? $"{userInfo}@{Encode(HostName, NameEncodeRegex)}:{Port.Value}" : $"{userInfo}@{Encode(HostName, NameEncodeRegex)}";
                return (Port.HasValue && !PortIsImplicit) ? $"{userInfo}@:{Port.Value}" : $"{userInfo}@";
            }
            if (HostName.Length > 0)
                return (Port.HasValue && !PortIsImplicit) ? $"@{Encode(HostName, NameEncodeRegex)}:{Port.Value}" : $"@{Encode(HostName, NameEncodeRegex)}";
            return (Port.HasValue && !PortIsImplicit) ? $"@:{Port.Value}" : "@";
        }

        internal void AppendTo(StringBuilder sb)
        {
            if (UserInfo is not null)
            {
                UserInfo.AppendTo(sb);
                sb.Append('@');
            }
            if (HostName.Length > 0)
                sb.Append(Encode(HostName, NameEncodeRegex));
            if (Port.HasValue && !PortIsImplicit)
                sb.Append(':').Append(Port.Value);
        }
    }
}
