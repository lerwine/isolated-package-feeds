using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using CdnGetter.Parsing.Version;

namespace CdnGetter;

public partial class SemanticVersion
{
    public readonly struct NumericalTokenN : INumericalToken<BigInteger>, IComparable<NumericalTokenN>, IEquatable<NumericalTokenN>
    {
        public NumericalTokenN(BigInteger value, int zeroPadLength = 0, ICharacterSpanToken? suffix = null)
        {
            if (zeroPadLength < 0)
                throw new ArgumentOutOfRangeException(nameof(zeroPadLength));
            if (value.Sign < 0)
            {
                IsNegative = true;
                Value = BigInteger.Negate(value);
            }
            else
            {
                IsNegative = false;
                Value = value;
            }
            ZeroPadLength = zeroPadLength;
            Suffix = (suffix is not null && suffix.Length > 0) ? suffix : null;
        }
        
        public NumericalTokenN(BigInteger value, int zeroPadLength, string? suffix) : this(value, zeroPadLength, string.IsNullOrEmpty(suffix) ? null : (suffix.Length == 1) ? new CharacterToken(suffix[0]) : new StringToken(suffix)) { }
        
        public NumericalTokenN(BigInteger value, int zeroPadLength, char suffix) : this(value, zeroPadLength, new CharacterToken(suffix)) { }
        
        public NumericalTokenN(BigInteger value, ICharacterSpanToken? suffix) : this(value, 0, suffix) { }
        
        public NumericalTokenN(BigInteger value, string? suffix) : this(value, 0, suffix) { }
        
        public NumericalTokenN(BigInteger value, char suffix) : this(value, 0, suffix) { }
        
        public bool IsNegative { get; }

        public int ZeroPadLength { get; }

        public ICharacterSpanToken? Suffix { get; }

        public BigInteger Value { get; }

        public int CompareTo(NumericalTokenN other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(INumericalToken? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IToken<BigInteger>? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IToken? other)
        {
            if (other is null)
                return 1;
            if (other is NumericalTokenN nt)
                return CompareTo(nt);
            int result;
            if (other is INumericalToken n)
                result = IsNegative ? (n.IsNegative ? n.CompareTo(Value) : -1) : n.IsNegative ? 1 : 0 - n.CompareTo(Value);
            else if (other is IToken<BigInteger> tb)
            {
                if (IsNegative)
                    return -1;
                result = Value.CompareTo(tb.Value);
            }
            else
                return VersionComponentComparer.CompareTo(ToString(), other.ToString());
            if (result != 0)
                return result;
            return CompareCharacterSpanTokens(Suffix, (other is IDelimitedToken d) ? d.Delimiter : null);
        }

        public int CompareTo(sbyte other) => IsNegative ? BigInteger.Negate(Value).CompareTo(other) : Value.IsZero ? 0 - other : Value.CompareTo(other);

        public int CompareTo(byte other) => IsNegative ? -1 : Value.IsZero ? ((other == 0) ? 0 : -1) : Value.CompareTo(other);

        public int CompareTo(short other) => IsNegative ? BigInteger.Negate(Value).CompareTo(other) : Value.IsZero ? 0 - other : Value.CompareTo(other);

        public int CompareTo(ushort other) => IsNegative ? -1 : Value.IsZero ? ((other == 0) ? 0 : -1) : Value.CompareTo(other);

        public int CompareTo(int other) => IsNegative ? BigInteger.Negate(Value).CompareTo(other) : Value.IsZero ? 0 - other : Value.CompareTo(other);

        public int CompareTo(uint other) => IsNegative ? -1 : Value.IsZero ? ((other == 0u) ? 0 : -1) : Value.CompareTo(other);

        public int CompareTo(long other) => (IsNegative ? BigInteger.Negate(Value) : Value).CompareTo(other);

        public int CompareTo(ulong other) => IsNegative ? -1 : Value.IsZero ? ((other == 0L) ? 0 : -1) : Value.CompareTo(other);

        public int CompareTo(BigInteger other) => other.IsZero ? (Value.IsZero ? 0 : IsNegative ? -1 : 1) : Value.IsZero ? other.Sign : IsNegative ? ((other.Sign > 0) ? -1 : BigInteger.Negate(Value).CompareTo(other)) :
            (other.Sign < 0) ? 1 : Value.CompareTo(other);

        public bool Equals(NumericalTokenN other) => IsNegative == other.IsNegative && Value == other.Value && CharacterSpanTokensEqual(Suffix, other.Suffix);

        public bool Equals([NotNullWhen(true)] INumericalToken? other)
        {
            if (other is null)
                return false;
            if (other is NumericalTokenN ntn)
                return Equals(ntn);
            return IsNegative == other.IsNegative && other.Equals(Value) && CharacterSpanTokensEqual(Suffix, other.Suffix);
        }

        public bool Equals([NotNullWhen(true)] IToken<BigInteger>? other)
        {
            if (other is null)
                return false;
            if (other is NumericalTokenN ntn)
                return Equals(ntn);
            if (other is INumericalToken n)
                return IsNegative == n.IsNegative && n.Equals(Value) && CharacterSpanTokensEqual(Suffix, n.Suffix);
            return other.Value.Equals(IsNegative ? BigInteger.Negate(Value) : Value) && CharacterSpanTokensEqual(Suffix, other is IDelimitedToken d ? d.Delimiter : null);
        }

        public bool Equals([NotNullWhen(true)] IToken? other)
        {
            if (other is null)
                return false;
            if (other is NumericalTokenN ntn)
                return Equals(ntn);
            if (other is INumericalToken n)
                return IsNegative == n.IsNegative && n.Equals(Value) && CharacterSpanTokensEqual(Suffix, n.Suffix);
            if (other is IToken<BigInteger> tb)
                return tb.Value.Equals(IsNegative ? BigInteger.Negate(Value) : Value) && CharacterSpanTokensEqual(Suffix, tb is IDelimitedToken d ? d.Delimiter : null);
            return VersionComponentComparer.AreEqual(ToString(), other.ToString());
        }

        public bool Equals(sbyte other) => (IsNegative ? BigInteger.Negate(Value) : Value) == other;

        public bool Equals(byte other) => (IsNegative ? BigInteger.Negate(Value) : Value) == other;

        public bool Equals(short other) => (IsNegative ? BigInteger.Negate(Value) : Value) == other;

        public bool Equals(ushort other) => (IsNegative ? BigInteger.Negate(Value) : Value) == other;

        public bool Equals(int other) => (IsNegative ? BigInteger.Negate(Value) : Value) == other;

        public bool Equals(uint other) => (IsNegative ? BigInteger.Negate(Value) : Value) == other;

        public bool Equals(long other) => (IsNegative ? BigInteger.Negate(Value) : Value) == other;

        public bool Equals(ulong other) => (IsNegative ? BigInteger.Negate(Value) : Value) == other;

        public bool Equals(BigInteger other) => (IsNegative ? BigInteger.Negate(Value) : Value) == other;

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is null)
                return false;
            if (obj is NumericalTokenN ntn)
                return Equals(ntn);
            if (obj is INumericalToken n)
                return IsNegative == n.IsNegative && n.Equals(Value) && CharacterSpanTokensEqual(Suffix, n.Suffix);
            if (obj is IToken<BigInteger> tb)
                return tb.Value.Equals(IsNegative ? BigInteger.Negate(Value) : Value) && CharacterSpanTokensEqual(Suffix, tb is IDelimitedToken d ? d.Delimiter : null);
            if (obj is IToken t)
                return VersionComponentComparer.AreEqual(ToString(), t.ToString());
            if (obj is BigInteger bi)
                return Equals(bi);
            if (obj is ulong ul)
                return Equals(ul);
            if (obj is long l)
                return Equals(l);
            if (obj is uint u)
                return Equals(u);
            if (obj is int i)
                return Equals(i);
            if (obj is ushort us)
                return Equals(us);
            if (obj is short v)
                return Equals(v);
            if (obj is byte b)
                return Equals(b);
            return obj is sbyte s && Equals(s);
        }

        public IEnumerable<char> GetCharacters(bool normalized = false)
        {
            if (IsNegative)
                return Enumerable.Repeat('-', 1).Concat((normalized || ZeroPadLength == 0) ? ((Suffix is null) ? Value.ToString() : Value.ToString().Concat(Suffix.GetCharacters(true))) :
                    Enumerable.Repeat('0', ZeroPadLength).Concat((Suffix is null) ? Value.ToString() : Value.ToString().Concat(Suffix.GetCharacters(true))));
            return (normalized || ZeroPadLength == 0) ? ((Suffix is null) ? Value.ToString() : Value.ToString().Concat(Suffix.GetCharacters(true))) :
                Enumerable.Repeat('0', ZeroPadLength).Concat((Suffix is null) ? Value.ToString() : Value.ToString().Concat(Suffix.GetCharacters(true)));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (IsNegative ? 616 : 605) + Value.GetHashCode() * 11;
                return (Suffix is null) ? hash : hash + Suffix.GetHashCode();
            }
        }

        public override string ToString()
        {
            if (IsNegative)
                return (ZeroPadLength == 0) ? ((Suffix is null) ? $"-{Value}" : $"-{Value}{Suffix}") :
                    (Suffix is null) ? $"-{new string('0', ZeroPadLength)}{Value}" : $"-{new string('0', ZeroPadLength)}{Value}{Suffix}";
            return (ZeroPadLength == 0) ? ((Suffix is null) ? Value.ToString() : $"{Value}{Suffix}") :
                (Suffix is null) ? $"{new string('0', ZeroPadLength)}{Value}" : $"{new string('0', ZeroPadLength)}{Value}{Suffix}";
        }
    }
}
