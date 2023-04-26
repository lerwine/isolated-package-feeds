namespace CdnGetter;

public partial class ParsedUri
{
    public class UriAuthority : IEquatable<UriAuthority>, IComparable<UriAuthority>
    {
        public UserInfo? UserInfo { get; }

        public string HostName { get; }

        public ushort? Port { get; }

        public UriAuthority(UserInfo userInfo, string hostName, ushort? port = null) => (UserInfo, HostName, Port) = (userInfo, hostName ?? "", port);

        public UriAuthority(string hostName, ushort? port = null) => (HostName, Port) = (hostName ?? "", port);

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
