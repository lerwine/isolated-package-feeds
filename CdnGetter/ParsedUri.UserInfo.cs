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
    }
}
