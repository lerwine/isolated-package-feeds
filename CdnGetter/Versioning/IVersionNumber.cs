using System.Numerics;

namespace CdnGetter.Versioning
{
    public interface IVersionNumber : IEquatable<IVersionNumber>, IEquatable<BigInteger>, IEquatable<ulong>, IEquatable<long>, IEquatable<uint>, IEquatable<int>, IEquatable<ushort>,
        IEquatable<short>, IEquatable<byte>, IEquatable<sbyte>, IComparable<IVersionNumber>, IComparable<BigInteger>, IComparable<ulong>, IComparable<long>, IComparable<uint>,
        IComparable<int>, IComparable<ushort>, IComparable<short>, IComparable<byte>, IComparable<sbyte>, IComparable
    {
        bool IsNegative { get; }
        
        int ZeroPadCount { get; }

        ITextComponent? Suffix { get; }

        string ToString(bool omitPaddedZeroes);
    }
}