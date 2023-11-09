using System.Numerics;

namespace CdnGetter;

public partial class SemanticVersion
{
    public readonly struct NumericalToken16 : INumericalToken
    {
        public NumericalToken16(short value, string? suffix = null) : this()
        {
            if (string.IsNullOrEmpty(suffix))
                Suffix = string.Empty;
            else
            {
                if (char.IsNumber(suffix[0]))
                    throw new ArgumentException($"{nameof(suffix)} cannot start with a number.", nameof(suffix));
                Suffix = suffix;
            }
            IsNegative = value < 0;
            Value = (ushort)(IsNegative ? Math.Abs(value) : value);
        }

        public NumericalToken16(ushort value, string? suffix = null) : this()
        {
            if (string.IsNullOrEmpty(suffix))
                Suffix = string.Empty;
            else
            {
                if (char.IsNumber(suffix[0]))
                    throw new ArgumentException($"{nameof(suffix)} cannot start with a number.", nameof(suffix));
                Suffix = suffix;
            }
            IsNegative = false;
            Value = value;
        }

        public string Suffix { get; }

        public bool IsNegative { get; }

        public ushort Value { get; }

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

        public bool Equals(short other) => Suffix.Length == 0 && (IsNegative ? other.Equals(((int)Value) * -1) : other.Equals(Value));

        public bool Equals(ushort other) => Suffix.Length == 0 && !IsNegative && other.Equals(Value);

        public bool Equals(int other) => Suffix.Length == 0 && (IsNegative ? other.Equals(((int)Value) * -1) : other.Equals(Value));

        public bool Equals(uint other) => Suffix.Length == 0 && !IsNegative && other.Equals(Value);

        public bool Equals(long other) => Suffix.Length == 0 && (IsNegative ? other.Equals(((int)Value) * -1) : other.Equals(Value));

        public bool Equals(ulong other) => Suffix.Length == 0 && !IsNegative && other.Equals(Value);

        public bool Equals(BigInteger other) => Suffix.Length == 0 && (IsNegative ? other.Equals(BigInteger.Negate(new BigInteger(Value))) : other.Equals(Value));

        public override bool Equals(object? obj) => obj is not null && ((obj is NumericalTokenN n) ? IsNegative == n.IsNegative && Value == n.Value && AlphaComparer.Equals(Suffix, n.Suffix) :
            (obj is INumericalToken other) ? Equals(other) : (Suffix.Length == 0 && (obj is sbyte s) ? Value.Equals(s) : (obj is byte b) ? Value.Equals(b) : (obj is short t) ? Value.Equals(t) :
            (obj is ushort v) ? Value.Equals(v) : (obj is int i) ? i.Equals(Value) : (obj is uint u) ? u.Equals(Value) : (obj is long l) ? l.Equals(Value) : (obj is ulong o) ? o.Equals(Value) :
            obj is BigInteger x && x.Equals(Value)));

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