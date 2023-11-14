using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace CdnGetter;

public partial class SemanticVersion
{
    public readonly struct NumericalToken64 : INumericalToken<ulong>, IComparable<NumericalToken64>, IEquatable<NumericalToken64>
    {
        public NumericalToken64(ulong value, bool isNegative = false, int zeroPadLength = 0, ICharacterSpanToken? suffix = null)
        {
            if (zeroPadLength < 0)
                throw new ArgumentOutOfRangeException(nameof(zeroPadLength));
            Value = value;
            IsNegative = isNegative;
            ZeroPadLength = zeroPadLength;
            Suffix = (suffix is not null && suffix.Length > 0) ? suffix : null;
        }
        
        public NumericalToken64(ulong value, bool isNegative, int zeroPadLength, string? suffix) : this(value, isNegative, zeroPadLength, string.IsNullOrEmpty(suffix) ? null : (suffix.Length == 1) ? new CharacterToken(suffix[0]) : new StringToken(suffix)) { }
        
        public NumericalToken64(ulong value, bool isNegative, int zeroPadLength, char suffix) : this(value, isNegative, zeroPadLength, new CharacterToken(suffix)) { }
        
        public NumericalToken64(ulong value, int zeroPadLength, ICharacterSpanToken? suffix = null) : this(value, false, zeroPadLength, suffix) { }
        
        public NumericalToken64(ulong value, int zeroPadLength, string? suffix) : this(value, false, zeroPadLength, suffix) { }
        
        public NumericalToken64(ulong value, int zeroPadLength, char suffix) : this(value, false, zeroPadLength, suffix) { }
        
        public NumericalToken64(long value, int zeroPadLength = 0, ICharacterSpanToken? suffix = null)
        {
            if (zeroPadLength < 0)
                throw new ArgumentOutOfRangeException(nameof(zeroPadLength));
            if (value < 0)
            {
                Value = (ulong)(0 - value);
                IsNegative = true;
            }
            else
            {
                Value = (ulong)value;
                IsNegative = false;
            }
            ZeroPadLength = zeroPadLength;
            Suffix = (suffix is not null && suffix.Length > 0) ? suffix : null;
        }
        
        public NumericalToken64(long value, int zeroPadLength, string? suffix) : this(value, zeroPadLength, string.IsNullOrEmpty(suffix) ? null : (suffix.Length == 1) ? new CharacterToken(suffix[0]) : new StringToken(suffix)) { }
        
        public NumericalToken64(long value, int zeroPadLength, char suffix) : this(value, zeroPadLength, new CharacterToken(suffix)) { }

        public NumericalToken64(ulong value, bool isNegative, ICharacterSpanToken? suffix) : this(value, isNegative, 0, suffix) { }
        
        public NumericalToken64(ulong value, bool isNegative, string? suffix) : this(value, isNegative, 0, suffix) { }
        
        public NumericalToken64(ulong value, bool isNegative, char suffix) : this(value, isNegative, 0, suffix) { }
        
        public NumericalToken64(ulong value, ICharacterSpanToken? suffix) : this(value, false, 0, suffix) { }
        
        public NumericalToken64(ulong value, string? suffix) : this(value, false, 0, suffix) { }
        
        public NumericalToken64(ulong value, char suffix)  : this(value, false, 0, suffix) { }
        
        public NumericalToken64(long value, ICharacterSpanToken? suffix) : this(value, 0, suffix) { }
        
        public NumericalToken64(long value, string? suffix) : this(value, 0, suffix) { }
        
        public NumericalToken64(long value, char suffix) : this(value, 0, suffix) { }

        public static readonly BigInteger RANGE_MAX_VALUE = new(ulong.MaxValue);
        
        public static readonly BigInteger RANGE_MIN_VALUE = BigInteger.Negate(RANGE_MAX_VALUE);

        public bool IsNegative { get; }

        public int ZeroPadLength { get; }

        public ICharacterSpanToken? Suffix { get; }

        public ulong Value { get; }

        public int CompareTo(NumericalToken64 other)
        {
            int result;
            if (IsNegative)
            {
                if (!other.IsNegative)
                    return -1;
                result = other.Value.CompareTo(Value);
            }
            else
            {
                 if (other.IsNegative)
                    return 1; 
                result = Value.CompareTo(other.Value);
            }
            return (result != 0) ? result : CompareCharacterSpanTokens(Suffix, other.Suffix);
        }

        public int CompareTo(INumericalToken? other)
        {
            if (other is null)
                return 1;
            if (other is NumericalToken64 nt)
                return CompareTo(nt);
            int result;
            if (IsNegative)
            {
                if (!other.IsNegative)
                    return -1;
                result = other.CompareTo(Value);
            }
            else
            {
                 if (other.IsNegative)
                    return 1;
                result = 0 - other.CompareTo(Value);
            }
            return (result != 0) ? result : CompareCharacterSpanTokens(Suffix, other.Suffix);
        }

        public int CompareTo(IToken<ulong>? other)
        {
            if (other is null)
                return 1;
            if (other is NumericalToken64 nt)
                return CompareTo(nt);
            int result;
            if (other is INumericalToken n)
            {
                if (IsNegative)
                {
                    if (!n.IsNegative)
                        return -1;
                    result = n.CompareTo(Value);
                }
                else
                {
                    if (n.IsNegative)
                        return 1;
                    result = 0 - n.CompareTo(Value);
                }
            }
            else
            {
                if (IsNegative)
                    return -1;
                result = Value.CompareTo(other.Value);
            }
            return (result != 0) ? result : (Suffix is null) ? 0 : 1;
        }

        public int CompareTo(IToken? other)
        {
            if (other is null)
                return 1;
            if (other is NumericalToken64 nt)
                return CompareTo(nt);
            int result;
            if (other is INumericalToken n)
            {
                if (IsNegative)
                {
                    if (!n.IsNegative)
                        return -1;
                    result = n.CompareTo(Value);
                }
                else
                {
                    if (n.IsNegative)
                        return 1;
                    result = 0 - n.CompareTo(Value);
                }
            }
            else if (other is IToken<ulong> tb)
            {
                if (IsNegative)
                    return -1;
                result = Value.CompareTo(tb.Value);
            }
            else
                return TextComparer.Compare(ToString(), other.ToString());
            if (result != 0)
                return result;
            return CompareCharacterSpanTokens(Suffix, (other is IDelimitedToken d) ? d.Delimiter : null);
        }

        public int CompareTo(sbyte other) => (IsNegative ? 0 - Value : Value).CompareTo(other);

        public int CompareTo(byte other) => IsNegative ? -1 : Value.CompareTo(other);

        public int CompareTo(short other) => (IsNegative ? 0 - Value : Value).CompareTo(other);

        public int CompareTo(ushort other) => IsNegative ? -1 : Value.CompareTo(other);

        public int CompareTo(int other) => (IsNegative ? 0 - Value : Value).CompareTo(other);

        public int CompareTo(uint other) => IsNegative ? -1 : Value.CompareTo(other);

        public int CompareTo(long other) => IsNegative ? ((other < 0L) ? ((ulong)(0L - other)).CompareTo(Value) : -1) : Value.CompareTo((ulong)other);

        public int CompareTo(ulong other) => IsNegative ? -1 : Value.CompareTo(other);

        public int CompareTo(BigInteger other) => IsNegative ? ((other.Sign < 0) ? BigInteger.Negate(other).CompareTo(Value) : -1) : other.IsZero ? ((Value == 0UL) ? 0 : 1) : 0 - other.CompareTo(Value);

        public bool Equals(NumericalToken64 other) => IsNegative == other.IsNegative && Value == other.Value && CharacterSpanTokensEqual(Suffix, other.Suffix);

        public bool Equals([NotNullWhen(true)] INumericalToken? other)
        {
            if (other is null)
                return false;
            if (other is NumericalToken64 nt)
                return Equals(nt);
            return IsNegative == other.IsNegative && other.Equals(Value) && CharacterSpanTokensEqual(Suffix, other.Suffix);
        }

        public bool Equals([NotNullWhen(true)] IToken<ulong>? other)
        {
            if (other is null)
                return false;
            if (other is NumericalToken64 nt)
                return Equals(nt);
            if (other is INumericalToken n)
                return IsNegative == n.IsNegative && n.Equals(Value) && CharacterSpanTokensEqual(Suffix, n.Suffix);
            return !IsNegative && other.Value.Equals(Value) && CharacterSpanTokensEqual(Suffix, other is IDelimitedToken d ? d.Delimiter : null);
        }

        public bool Equals([NotNullWhen(true)] IToken? other)
        {
            if (other is null)
                return false;
            if (other is NumericalToken64 nt)
                return Equals(nt);
            if (other is INumericalToken n)
                return IsNegative == n.IsNegative && n.Equals(Value) && CharacterSpanTokensEqual(Suffix, n.Suffix);
            if (other is IToken<ulong> tb)
                return !IsNegative && tb.Value.Equals(Value) && CharacterSpanTokensEqual(Suffix, tb is IDelimitedToken d ? d.Delimiter : null);
            return TextComparer.Equals(ToString(), other.ToString());
        }

        public bool Equals(sbyte other) => Value <= (ulong)sbyte.MaxValue && (IsNegative ? (other < 0 && (0 - other) == (sbyte)Value) : other >= 0 && (sbyte)Value == other);

        public bool Equals(byte other) => !IsNegative && Value == other;

        public bool Equals(short other) => Value <= (ulong)short.MaxValue && (IsNegative ? (other < 0 && (0 - other) == (short)Value) : other >= 0 && (short)Value == other);

        public bool Equals(ushort other) => !IsNegative && Value == other;

        public bool Equals(int other) => Value <= int.MaxValue && (IsNegative ? (other < 0 && (0 - other) == (int)Value) : other >= 0 && (int)Value == other);

        public bool Equals(uint other) => !IsNegative && Value == other;

        public bool Equals(long other) => Value <= long.MaxValue && (IsNegative ? (other < 0L && (0L - other) == (long)Value) : other >= 0L && (long)Value == other);

        public bool Equals(ulong other) => !IsNegative && Value == other;

        public bool Equals(BigInteger other) => Value == (IsNegative ? BigInteger.Negate(other) : other);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is null)
                return false;
            if (obj is NumericalToken64 nt)
                return Equals(nt);
            if (obj is INumericalToken n)
                return IsNegative == n.IsNegative && n.Equals(Value) && CharacterSpanTokensEqual(Suffix, n.Suffix);
            if (obj is IToken<ulong> tb)
                return !IsNegative && tb.Value.Equals(Value) && CharacterSpanTokensEqual(Suffix, tb is IDelimitedToken d ? d.Delimiter : null);
            if (obj is IToken t)
                return TextComparer.Equals(ToString(), t.ToString());
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