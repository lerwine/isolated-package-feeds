using System.Numerics;

namespace CdnGetter;

public partial class SemanticVersion : IComparable<SemanticVersion>, IEquatable<SemanticVersion>
{
    string Prefix { get; }

    INumericalToken Major { get; }

    IDelimitedNumericalToken? Minor { get; }

    IDelimitedNumericalToken? Patch { get; }

    IList<IDelimitedNumericalToken> Micro { get; }

    public interface IDelimitedNumericalToken : INumericalToken
    {

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

    public static readonly StringComparer AlphaComparer = StringComparer.CurrentCultureIgnoreCase;

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
        throw new NotImplementedException();
    }
    
    public static bool operator ==(SemanticVersion left, SemanticVersion right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(SemanticVersion left, SemanticVersion right)
    {
        return !(left == right);
    }

    public static bool operator <(SemanticVersion left, SemanticVersion right)
    {
        return left is null ? right is not null : left.CompareTo(right) < 0;
    }

    public static bool operator <=(SemanticVersion left, SemanticVersion right)
    {
        return left is null || left.CompareTo(right) <= 0;
    }

    public static bool operator >(SemanticVersion left, SemanticVersion right)
    {
        return left is not null && left.CompareTo(right) > 0;
    }

    public static bool operator >=(SemanticVersion left, SemanticVersion right)
    {
        return left is null ? right is null : left.CompareTo(right) >= 0;
    }
}
