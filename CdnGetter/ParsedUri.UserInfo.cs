namespace CdnGetter;

public partial class ParsedUri
{
    public class UserInfo : IEquatable<UserInfo>, IComparable<UserInfo>
    {
        public string UserName { get; }

        public string? Password { get; }

        public UserInfo(string userName, string? password = null) => (UserName, Password) = (userName ?? "", password);

        public bool Equals(UserInfo? other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object? obj) => Equals(obj as UserInfo);

        public int CompareTo(UserInfo? other)
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

        internal static UserInfo Parse(string userInfo, bool parsePasswordSubcomponent = false)
        {
            if (string.IsNullOrEmpty(userInfo))
                return new(string.Empty, null);
            if (userInfo.Length > 1)
            {
                if (parsePasswordSubcomponent)
                {
                    int index = userInfo.IndexOf(DELIMITER_CHAR_SCHEME);
                    if (index == 0)
                        return new(string.Empty, UriDecode(userInfo[1..]));
                    if (index == userInfo.Length - 1)
                        return new(UriDecode(userInfo[..index]), string.Empty);
                    if (index > 0)
                        return new(UriDecode(userInfo[..index]), UriDecode(userInfo[(index + 1)..]));
                }
            }
            else if (parsePasswordSubcomponent && userInfo[0] == DELIMITER_CHAR_SCHEME)
                return new(string.Empty, string.Empty);
            return new(UriDecode(userInfo), null);
        }
    }
}
