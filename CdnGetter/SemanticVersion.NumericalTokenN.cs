using System.Numerics;

namespace CdnGetter;

public partial class SemanticVersion
{
    public readonly struct NumericalTokenN : INumericalToken
    {
        public string Suffix { get; }

        public bool IsNegative { get; }

        public BigInteger Value { get; }

        public NumericalTokenN(BigInteger value, string? suffix = null) : this()
        {
            if (string.IsNullOrEmpty(suffix))
                Suffix = string.Empty;
            else
            {
                if (char.IsNumber(suffix[0]))
                    throw new ArgumentException($"{nameof(suffix)} cannot start with a number.", nameof(suffix));
                Suffix = suffix;
            }
            IsNegative = value.Sign < 0;
            Value = IsNegative ? BigInteger.Abs(value) : value;
        }

        public int CompareTo(INumericalToken? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(sbyte other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(byte other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(short other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(ushort other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(int other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(uint other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(long other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(ulong other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(BigInteger other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(INumericalToken? other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(sbyte other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(byte other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(short other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(ushort other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(int other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(uint other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(long other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(ulong other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(BigInteger other) => Suffix.Length == 0 && (IsNegative ? other.Equals(BigInteger.Negate(Value)) : other.Equals(Value));

        public override bool Equals(object? obj) => obj is not null && ((obj is NumericalTokenN n) ? IsNegative == n.IsNegative && Value == n.Value && AlphaComparer.Equals(Suffix, n.Suffix) :
            (obj is INumericalToken other) ? Equals(other) : (Suffix.Length == 0 && (obj is sbyte s) ? Value.Equals(s) : (obj is byte b) ? Value.Equals(b) : (obj is short t) ? Value.Equals(t) :
            (obj is ushort v) ? Value.Equals(v) : (obj is int i) ? Value.Equals(i) : (obj is uint u) ? Value.Equals(u) : (obj is long l) ? Value.Equals(l) : (obj is ulong o) ? Value.Equals(o) :
            obj is BigInteger x && Value.Equals(x)));

        public override int GetHashCode()
        {
            unchecked
            {
                return ((IsNegative ? 616 : 605) + Value.GetHashCode()) * 11 + AlphaComparer.GetHashCode(Suffix).GetHashCode();
            }
        }

        public override string ToString() => IsNegative ? ((Suffix.Length > 0) ? $"-{Value}{Suffix}" : $"-{Value}") : (Suffix.Length > 0) ? $"{Value}{Suffix}" : Value.ToString();
    }
}
