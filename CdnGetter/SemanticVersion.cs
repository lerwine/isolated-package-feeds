using System.Numerics;

namespace CdnGetter;

public partial class SemanticVersion : IComparable<SemanticVersion>, IEquatable<SemanticVersion>
{
    string Prefix { get; }

    INumericalToken Major { get; }

    IDelimitedNumericalToken? Minor { get; }

    IDelimitedNumericalToken? Patch { get; }

    IList<IDelimitedNumericalToken> Micro { get; }

    public static int CompareCharacterSpanTokens(ICharacterSpanToken? x, ICharacterSpanToken? y)
    {
        throw new NotImplementedException();
    }

    public static bool CharacterSpanTokensEqual(ICharacterSpanToken? x, ICharacterSpanToken? y)
    {
        throw new NotImplementedException();
    }

    public int CompareTo(SemanticVersion? other)
    {
        throw new NotImplementedException();
    }

    public bool Equals(SemanticVersion? other)
    {
        throw new NotImplementedException();
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is null)
        {
            return false;
        }

        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public static INumericalToken ToNumericalToken(short value) => (value > byte.MaxValue || value < sbyte.MinValue) ? new NumericalToken16(value) : (value < 0) ? new NumericalToken8((sbyte)value) : new NumericalToken8((byte)value);
    
    public static INumericalToken ToNumericalToken(ushort value)
    {
        throw new NotImplementedException();
    }
    
    public static INumericalToken ToNumericalToken(int value)
    {
        throw new NotImplementedException();
    }
    
    public static INumericalToken ToNumericalToken(uint value)
    {
        throw new NotImplementedException();
    }
    
    public static INumericalToken ToNumericalToken(long value)
    {
        throw new NotImplementedException();
    }
    
    public static INumericalToken ToNumericalToken(ulong value)
    {
        throw new NotImplementedException();
    }
    
    public static INumericalToken ToNumericalToken(BigInteger value)
    {
        if (value.IsZero)
            return NumericalToken8.Zero;
        if (value.IsOne)
            return NumericalToken8.One;
        if (value < NumericalToken64.RANGE_MIN_VALUE || value > NumericalToken64.RANGE_MAX_VALUE)
            return new NumericalTokenN(value);
        bool isNegative = value.Sign < 0;
        if (isNegative && (value = BigInteger.Negate(value)).IsOne)
            return NumericalToken8.NegativeOne;
        if (value > NumericalToken32.RANGE_MAX_VALUE)
            return new NumericalToken64((ulong)value, isNegative);
        if (value > NumericalToken16.RANGE_MAX_VALUE)
            return new NumericalToken32((uint)value, isNegative);
        return (value > NumericalToken8.RANGE_MAX_VALUE) ? new NumericalToken16((ushort)value, isNegative) : new NumericalToken8((byte)value, isNegative);
    }
    
    public static bool operator ==(SemanticVersion left, SemanticVersion right) => (left is null) ? right is null : left.Equals(right);

    public static bool operator !=(SemanticVersion left, SemanticVersion right) => (left is null) ? right is not null : !left.Equals(right);

    public static bool operator <(SemanticVersion left, SemanticVersion right) => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(SemanticVersion left, SemanticVersion right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(SemanticVersion left, SemanticVersion right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(SemanticVersion left, SemanticVersion right) => left is null ? right is null : left.CompareTo(right) >= 0;
}
