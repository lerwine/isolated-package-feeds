using System.Numerics;

namespace CdnGetter;

public partial class SemanticVersion
{
    public interface INumericalToken : IEquatable<INumericalToken>, IComparable<INumericalToken>, IEquatable<sbyte>, IComparable<sbyte>, IEquatable<byte>, IComparable<byte>, IEquatable<short>, IComparable<short>, IEquatable<ushort>, IComparable<ushort>, IEquatable<int>, IComparable<int>, IEquatable<uint>, IComparable<uint>, IEquatable<long>, IComparable<long>, IEquatable<ulong>, IComparable<ulong>, IEquatable<BigInteger>, IComparable<BigInteger>
    {
        string Suffix { get; }
        bool IsNegative { get; }
    }
}
