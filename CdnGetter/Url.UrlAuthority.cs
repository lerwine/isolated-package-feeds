namespace CdnGetter;

public partial class Url
{
    public class UrlAuthority
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
    }
}
