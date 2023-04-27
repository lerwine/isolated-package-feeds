namespace CdnGetter;

public partial class ParsedUri
{
    public class UriAuthority : IEquatable<UriAuthority>, IComparable<UriAuthority>
    {
        public UserInfo? UserInfo { get; }

        public string HostName { get; }

        public ushort? Port { get; }

        public bool PortIsImplicit { get; }
        
        public UriAuthority(UserInfo userInfo, string hostName, ushort port, bool portIsImplicit = false) => (UserInfo, HostName, Port, PortIsImplicit) = (userInfo, hostName ?? "", port, portIsImplicit);

        public UriAuthority(UserInfo userInfo, string hostName) => (UserInfo, HostName) = (userInfo, hostName ?? "");
        
        public UriAuthority(string hostName) => HostName = hostName ?? "";

        public UriAuthority(string hostName, ushort port, bool portIsImplicit = false) => (HostName, Port, PortIsImplicit) = (hostName ?? "", port, portIsImplicit);

        public static readonly UriAuthority Empty = new(string.Empty);

        public bool Equals(UriAuthority? other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object? obj) => Equals(obj as UriAuthority);

        public int CompareTo(UriAuthority? other)
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
