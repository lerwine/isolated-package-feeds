using System.Text;

namespace CdnGetter;

public partial class ParsedUri
{
    /// <summary>
    /// Represents the UserInfo URI component.
    /// </summary>
    public class UserInfo : IEquatable<UserInfo>, IComparable<UserInfo>
    {
        /// <summary>
        /// Gets the user name sub-component.
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// Gets the password sub-component.
        /// </summary>
        /// <value>The password sub-component or <see langword="null" /> if there is no password.</value>
        public string? Password { get; }

        /// <summary>
        /// Creates a new <c>UserInfo</c> component object.
        /// </summary>
        /// <param name="userName">The user name sub-component</param>
        /// <param name="password">The password sub-component or <see langword="null" /> if there is no password.</param>
        public UserInfo(string userName, string? password = null) => (UserName, Password) = (userName ?? "", password);

        public bool Equals(UserInfo? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (!DefaultComponentComparer.Equals(UserName, other.UserName))
                return false;
            if (Password is null)
                return other.Password is null;
            return other.Password is not null && Password.Equals(other.Password);
        }

        public override bool Equals(object? obj) => Equals(obj as UserInfo);

        public int CompareTo(UserInfo? other)
        {
            if (other is null)
                return 1;
            if (ReferenceEquals(this, other))
                return 0;
            int result = DefaultComponentComparer.Compare(UserName, other.UserName);
            if (result != 0)
                return result;
            if (Password is null)
                return (other.Password is null) ? 0 : -1;
            return (other.Password is null) ? 1 : Password.CompareTo(other.Password);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 21 + DefaultComponentComparer.GetHashCode(UserName);
                return (Password is null) ? hash : hash * 7 + DefaultComponentComparer.GetHashCode(Password);
            }
        }

        public override string ToString()
        {
            if (UserName.Length > 0)
            {
                if (Password is null)
                    return Encode(UserName, PasswordEncodeRegex);
                return (Password.Length > 0) ? $"{Encode(UserName, NameEncodeRegex)}:{Encode(Password, PasswordEncodeRegex)}" : $"{Encode(UserName, NameEncodeRegex)}:";
            }
            if (Password is null)
                return string.Empty;
            return (Password.Length > 1) ? $":{Encode(Password, PasswordEncodeRegex)}" : ":";
        }

        internal static UserInfo Parse(string userInfo, bool parsePasswordSubcomponent = false)
        {
            if (string.IsNullOrEmpty(userInfo))
                return new(string.Empty, null);
            if (userInfo.Length > 1)
            {
                if (parsePasswordSubcomponent)
                {
                    int index = userInfo.IndexOf(DELIMITER_CHAR_COLON);
                    if (index == 0)
                        return new(string.Empty, UriDecode(userInfo[1..]));
                    if (index == userInfo.Length - 1)
                        return new(UriDecode(userInfo[..index]), string.Empty);
                    if (index > 0)
                        return new(UriDecode(userInfo[..index]), UriDecode(userInfo[(index + 1)..]));
                }
            }
            else if (parsePasswordSubcomponent && userInfo[0] == DELIMITER_CHAR_COLON)
                return new(string.Empty, string.Empty);
            return new(UriDecode(userInfo), null);
        }

        internal void AppendTo(StringBuilder sb)
        {
            if (UserName.Length > 0)
            {
                if (Password is null)
                    sb.Append(Encode(UserName, PasswordEncodeRegex));
                else
                {
                    sb.Append(Encode(UserName, NameEncodeRegex)).Append(':');
                    if (Password.Length > 0)
                        sb.Append(Encode(Password, PasswordEncodeRegex));
                }
            }
            else if (Password is not null)
            {
                sb.Append(':');
                if (Password.Length > 0)
                    sb.Append(Encode(Password, PasswordEncodeRegex));
            }
        }
    }
}
