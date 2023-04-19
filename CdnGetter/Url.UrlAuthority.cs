namespace CdnGetter;

public partial class Url
{
    public class UrlAuthority : IEquatable<UrlAuthority>, IComparable<UrlAuthority>
    {
        public UserInfo? UserInfo { get; set; }
        
        private string _hostName;
        
        /// <summary>
        /// The host name.
        /// </summary>
        public string HostName
        {
            get => _hostName;
            set => _hostName = value ?? "";
        }

        public ushort? Port { get; set; }

        public UrlAuthority(string hostName, ushort? port = null)
        {
            _hostName = hostName ?? "";
            Port = port;
        }

        public UrlAuthority(UserInfo? userInfo, string hostName, ushort? port = null) : this(hostName, port)
        {
            UserInfo = userInfo;
        }

        public bool Equals(UrlAuthority? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (!Comparer.Equals(_hostName, other._hostName))
                return false;
            UserInfo? x = UserInfo;
            UserInfo? y = other.UserInfo;
            if ((x is null) ? y is not null : x is null || !x.Equals(y))
                return false;
            ushort? a = Port;
            ushort? b = other.Port;
            return a.HasValue ? b.HasValue && a.Value == b.Value : !b.HasValue;
        }

        public override bool Equals(object? obj) => obj is UrlAuthority other && Equals(other);

        public int CompareTo(UrlAuthority? other)
        {
            if (other is null)
                return 1;
            if (ReferenceEquals(this, other))
                return 0;
            int result = Comparer.Compare(_hostName, other._hostName);
            if (result != 0)
                return result;
            UserInfo? x = UserInfo;
            UserInfo? y = other.UserInfo;
            if (x is null)
            {
                if (y is not null)
                    return -1;
            }
            else
            {
                if (y is null)
                    return 1;
                if ((result = x.CompareTo(y)) != 0)
                    return result;
            }
            ushort? a = Port;
            ushort? b = other.Port;
            return a.HasValue ? (b.HasValue ? a.Value.CompareTo(b.Value) : 1) : b.HasValue ? -1 : 0;
        }

        public override int GetHashCode()
        {
            int result = 7;
            unchecked
            {
                UserInfo? userInfo = UserInfo;
                if (userInfo is null)
                    result *= 11;
                else
                    result = (result * 11) + userInfo.GetHashCode();
                result = (result * 11) + _hostName.GetHashCode();
                ushort? port = Port;
                return port.HasValue ? (result * 11) + port.Value.GetHashCode() : result * 11;
            }
        }

        public override string ToString()
        {
            UserInfo? userInfo = UserInfo;
            ushort? port = Port;
            if (userInfo is null)
            {
                return port.HasValue ? ((_hostName.Length > 0) ? $"{NameEncodeRegex.Replace(_hostName, m => Uri.HexEscape(m.Value[0]))}:{port.Value}" : $":{port.Value}") :
                    (_hostName.Length > 0) ? NameEncodeRegex.Replace(_hostName, m => Uri.HexEscape(m.Value[0])) : string.Empty;
            }
            return port.HasValue ? ((_hostName.Length > 0) ? $"{userInfo}@{NameEncodeRegex.Replace(_hostName, m => Uri.HexEscape(m.Value[0]))}:{port.Value}" : $"{userInfo}@:{port.Value}") :
                (_hostName.Length > 0) ? $"{userInfo}@{NameEncodeRegex.Replace(_hostName, m => Uri.HexEscape(m.Value[0]))}" : $"{userInfo}@";
        }
    }
}
