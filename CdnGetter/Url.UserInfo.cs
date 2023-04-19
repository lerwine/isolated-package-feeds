namespace CdnGetter;

public partial class Url
{
    public class UserInfo : IEquatable<UserInfo>, IComparable<UserInfo>
    {
        private string _userName;
        
        /// <summary>
        /// The user name.
        /// </summary>
        public string UserName
        {
            get => _userName;
            set => _userName = value ?? "";
        }

        public string? Password { get; set; }   

        public UserInfo(string userName, string? password = null)
        {
            _userName = userName ?? "";
            Password = password;    
        }

        public bool Equals(UserInfo? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (!Comparer.Equals(_userName, other._userName))
                return false;
            string? x = Password;
            string? y = other.Password;
            return (x is null) ? y is null : y is not null && Comparer.Equals(x, y);
        }

        public override bool Equals(object? obj) => obj is UserInfo other && Equals(other);

        public int CompareTo(UserInfo? other)
        {
            if (other is null)
                return 1;
            if (ReferenceEquals(this, other))
                return 0;
            int result = Comparer.Compare(_userName, other._userName);
            if (result != 0)
                return result;
            string? x = Password;
            string? y = other.Password;
            return (x is null) ? ((y is null) ? 0 : -1) : (y is null) ? 1 : Comparer.Compare(x, y);
        }

        public override int GetHashCode()
        {
            int hashCode = 3;
            unchecked
            {
                hashCode = (hashCode * 7) + _userName.GetHashCode();
                string? password = Password;
                return (password is null) ? hashCode * 7 : (hashCode * 7) + password.GetHashCode();
            }
        }

        public override string ToString()
        {
            string userName = _userName;
            string? password = Password;
            if (password is null)
                return (userName.Length > 0) ? NameEncodeRegex.Replace(userName, m => Uri.HexEscape(m.Value[0])) : userName;
            return (userName.Length > 0) ? $"{NameEncodeRegex.Replace(userName, m => Uri.HexEscape(m.Value[0]))}:{PasswordEncodeRegex.Replace(password, m => Uri.HexEscape(m.Value[0]))}" :
                $":{PasswordEncodeRegex.Replace(password, m => Uri.HexEscape(m.Value[0]))}";
        }
    }
}
