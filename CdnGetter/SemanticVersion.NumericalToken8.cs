using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace CdnGetter;

public partial class SemanticVersion
{
#pragma warning disable CA2231
    public readonly struct NumericalToken8 : INumericalToken<byte>, IComparable<NumericalToken8>, IEquatable<NumericalToken8>
#pragma warning restore CA2231
    {
        public static readonly NumericalToken8 Zero = new(0);

        public static readonly NumericalToken8 One = new(1);

        public static readonly NumericalToken8 NegativeOne = new(-1);

        public NumericalToken8(byte value, bool isNegative = false, int zeroPadLength = 0, ICharacterSpanToken? suffix = null)
        {
            if (zeroPadLength < 0)
                throw new ArgumentOutOfRangeException(nameof(zeroPadLength));
            Value = value;
            IsNegative = isNegative;
            ZeroPadLength = zeroPadLength;
            Suffix = (suffix is not null && suffix.Length > 0) ? suffix : null;
        }

        public NumericalToken8(byte value, bool isNegative, int zeroPadLength, string? suffix) : this(value, isNegative, zeroPadLength, string.IsNullOrEmpty(suffix) ? null : (suffix.Length == 1) ? new CharacterToken(suffix[0]) : new StringToken(suffix)) { }

        public NumericalToken8(byte value, bool isNegative, int zeroPadLength, char suffix) : this(value, isNegative, zeroPadLength, new CharacterToken(suffix)) { }

        public NumericalToken8(byte value, int zeroPadLength, ICharacterSpanToken? suffix = null) : this(value, false, zeroPadLength, suffix) { }

        public NumericalToken8(byte value, int zeroPadLength, string? suffix) : this(value, false, zeroPadLength, suffix) { }

        public NumericalToken8(byte value, int zeroPadLength, char suffix) : this(value, false, zeroPadLength, suffix) { }

        public NumericalToken8(sbyte value, int zeroPadLength = 0, ICharacterSpanToken? suffix = null)
        {
            if (zeroPadLength < 0)
                throw new ArgumentOutOfRangeException(nameof(zeroPadLength));
            if (value < 0)
            {
                Value = (byte)(0 - value);
                IsNegative = true;
            }
            else
            {
                Value = (byte)value;
                IsNegative = false;
            }
            ZeroPadLength = zeroPadLength;
            Suffix = (suffix is not null && suffix.Length > 0) ? suffix : null;
        }

        public NumericalToken8(sbyte value, int zeroPadLength, string? suffix) : this(value, zeroPadLength, string.IsNullOrEmpty(suffix) ? null : (suffix.Length == 1) ? new CharacterToken(suffix[0]) : new StringToken(suffix)) { }

        public NumericalToken8(sbyte value, int zeroPadLength, char suffix) : this(value, zeroPadLength, new CharacterToken(suffix)) { }

        public NumericalToken8(byte value, bool isNegative, ICharacterSpanToken? suffix) : this(value, isNegative, 0, suffix) { }

        public NumericalToken8(byte value, bool isNegative, string? suffix) : this(value, isNegative, 0, suffix) { }

        public NumericalToken8(byte value, bool isNegative, char suffix) : this(value, isNegative, 0, suffix) { }

        public NumericalToken8(byte value, ICharacterSpanToken? suffix) : this(value, false, 0, suffix) { }

        public NumericalToken8(byte value, string? suffix) : this(value, false, 0, suffix) { }

        public NumericalToken8(byte value, char suffix) : this(value, false, 0, suffix) { }

        public NumericalToken8(sbyte value, ICharacterSpanToken? suffix) : this(value, 0, suffix) { }

        public NumericalToken8(sbyte value, string? suffix) : this(value, 0, suffix) { }

        public NumericalToken8(sbyte value, char suffix) : this(value, 0, suffix) { }

        public const short RANGE_MAX_VALUE = byte.MaxValue;

        public const short RANGE_MIN_VALUE = 0 - byte.MaxValue;

        public bool IsNegative { get; }

        public int ZeroPadLength { get; }

        public ICharacterSpanToken? Suffix { get; }

        public byte Value { get; }

        public int CompareTo(NumericalToken8 other)
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
            if (other is NumericalToken8 nt)
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

        public int CompareTo(IToken<byte>? other)
        {
            if (other is null)
                return 1;
            if (other is NumericalToken8 nt)
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
            if (other is NumericalToken8 nt)
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
            else if (other is IToken<byte> tb)
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

        public int CompareTo(sbyte other) => IsNegative ? ((other < 0) ? ((byte)(0 - other)).CompareTo(Value) : -1) : Value.CompareTo((byte)other);

        public int CompareTo(byte other) => IsNegative ? -1 : Value.CompareTo(other);

        public int CompareTo(short other) => IsNegative ? (0 - Value).CompareTo(other) : ((short)Value).CompareTo(other);

        public int CompareTo(ushort other) => IsNegative ? -1 : ((ushort)Value).CompareTo(other);

        public int CompareTo(int other) => IsNegative ? (0 - Value).CompareTo(other) : ((int)Value).CompareTo(other);

        public int CompareTo(uint other) => IsNegative ? -1 : ((uint)Value).CompareTo(other);

        public int CompareTo(long other) => IsNegative ? (0L - Value).CompareTo(other) : ((long)Value).CompareTo(other);

        public int CompareTo(ulong other) => IsNegative ? -1 : ((ulong)Value).CompareTo(other);

        public int CompareTo(BigInteger other) => IsNegative ? ((other.Sign < 0) ? BigInteger.Negate(other).CompareTo(Value) : -1) : other.IsZero ? ((Value == 0) ? 0 : 1) : 0 - other.CompareTo(Value);

        public bool Equals(NumericalToken8 other) => IsNegative == other.IsNegative && Value == other.Value && CharacterSpanTokensEqual(Suffix, other.Suffix);

        public bool Equals([NotNullWhen(true)] INumericalToken? other)
        {
            if (other is null)
                return false;
            if (other is NumericalToken8 nt)
                return Equals(nt);
            return IsNegative == other.IsNegative && other.Equals(Value) && CharacterSpanTokensEqual(Suffix, other.Suffix);
        }

        public bool Equals([NotNullWhen(true)] IToken<byte>? other)
        {
            if (other is null)
                return false;
            if (other is NumericalToken8 nt)
                return Equals(nt);
            if (other is INumericalToken n)
                return IsNegative == n.IsNegative && n.Equals(Value) && CharacterSpanTokensEqual(Suffix, n.Suffix);
            return !IsNegative && other.Value.Equals(Value) && CharacterSpanTokensEqual(Suffix, other is IDelimitedToken d ? d.Delimiter : null);
        }

        public bool Equals([NotNullWhen(true)] IToken? other)
        {
            if (other is null)
                return false;
            if (other is NumericalToken8 nt)
                return Equals(nt);
            if (other is INumericalToken n)
                return IsNegative == n.IsNegative && n.Equals(Value) && CharacterSpanTokensEqual(Suffix, n.Suffix);
            if (other is IToken<byte> tb)
                return !IsNegative && tb.Value.Equals(Value) && CharacterSpanTokensEqual(Suffix, tb is IDelimitedToken d ? d.Delimiter : null);
            return VersionComponentComparer.AreEqual(ToString(), other.ToString());
        }

        public bool Equals(sbyte other) => Value <= (ulong)sbyte.MaxValue && (IsNegative ? (other < 0 && (0 - other) == (sbyte)Value) : other >= 0 && (sbyte)Value == other);

        public bool Equals(byte other) => !IsNegative && Value == other;

        public bool Equals(short other) => IsNegative ? (other < 0 && (0 - other) == Value) : other >= 0 && Value == other;

        public bool Equals(ushort other) => !IsNegative && Value == other;

        public bool Equals(int other) => IsNegative ? (other < 0 && (0 - other) == Value) : other >= 0 && Value == other;

        public bool Equals(uint other) => !IsNegative && Value == other;

        public bool Equals(long other) => IsNegative ? (other < 0 && (0L - other) == Value) : other >= 0 && Value == other;

        public bool Equals(ulong other) => !IsNegative && Value == other;

        public bool Equals(BigInteger other) => Value == (IsNegative ? BigInteger.Negate(other) : other);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is null)
                return false;
            if (obj is NumericalToken8 nt)
                return Equals(nt);
            if (obj is INumericalToken n)
                return IsNegative == n.IsNegative && n.Equals(Value) && CharacterSpanTokensEqual(Suffix, n.Suffix);
            if (obj is IToken<byte> tb)
                return !IsNegative && tb.Value.Equals(Value) && CharacterSpanTokensEqual(Suffix, tb is IDelimitedToken d ? d.Delimiter : null);
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