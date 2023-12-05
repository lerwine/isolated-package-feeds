using System.Numerics;

namespace CdnGetter;

public partial class SemanticVersion
{
    public interface IToken : IEquatable<IToken>, IComparable<IToken>
    {
        IEnumerable<char> GetCharacters(bool normalized = false);
    }

    public interface IToken<T> : IToken, IEquatable<IToken<T>>, IComparable<IToken<T>>
    {
        T Value { get; }
    }

    public interface ICharacterSpanToken : IToken, IEquatable<ICharacterSpanToken>, IComparable<ICharacterSpanToken>, IEquatable<string>, IComparable<string>, IEquatable<char>, IComparable<char>
    {
        int Length { get; }
    }

    public interface INumericalToken : IEquatable<INumericalToken>, IComparable<INumericalToken>, IEquatable<sbyte>, IComparable<sbyte>, IEquatable<byte>, IComparable<byte>, IEquatable<short>, IComparable<short>, IEquatable<ushort>, IComparable<ushort>, IEquatable<int>, IComparable<int>, IEquatable<uint>, IComparable<uint>, IEquatable<long>, IComparable<long>, IEquatable<ulong>, IComparable<ulong>, IEquatable<BigInteger>, IComparable<BigInteger>
    {
        bool IsNegative { get; }

        int ZeroPadLength { get; }

        ICharacterSpanToken? Suffix { get; }
    }

    public interface INumericalToken<T> : INumericalToken, IToken<T>, IEquatable<INumericalToken>, IComparable<INumericalToken>, IEquatable<sbyte>, IComparable<sbyte>, IEquatable<byte>, IComparable<byte>, IEquatable<short>, IComparable<short>, IEquatable<ushort>, IComparable<ushort>, IEquatable<int>, IComparable<int>, IEquatable<uint>, IComparable<uint>, IEquatable<long>, IComparable<long>, IEquatable<ulong>, IComparable<ulong>, IEquatable<BigInteger>, IComparable<BigInteger>
        where T : struct, IComparable<T>, IEquatable<T>, ISpanFormattable, IFormattable
    {
    }

    public interface IDelimitedToken : IToken
    {
        ICharacterSpanToken Delimiter { get; }
    }

    public interface IDelimitedToken<T> : IDelimitedToken, IToken<T> { }

    public interface IDelimitedNumericalToken : IDelimitedToken, INumericalToken { }

    public interface IDelimitedNumericalToken<T> : IDelimitedNumericalToken, IDelimitedToken<T>, INumericalToken<T> where T : struct, IComparable<T>, IEquatable<T>, ISpanFormattable, IFormattable { }
}
