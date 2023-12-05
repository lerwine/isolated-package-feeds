using System.Collections;
using System.Numerics;

namespace CdnGetter.Versioning
{
    public readonly struct VersionNumber : IVersionNumber
    {
        public VersionNumber(uint value, bool isNegative, int zeroPadCount, ITextComponent? suffix)
        {
            if (zeroPadCount < 0)
                throw new ArgumentOutOfRangeException(nameof(zeroPadCount));
            if (suffix is null || suffix.Length == 0)
                Suffix = null;
            else
            {
                if (char.IsNumber(suffix.First()))
                    throw new ArgumentOutOfRangeException(nameof(suffix));
                Suffix = suffix;
            }
            Value = value;
            IsNegative = isNegative;
            ZeroPadCount = zeroPadCount;
        }
        
        public VersionNumber(uint value, bool isNegative, int zeroPadCount, string suffix) : this(value, isNegative, zeroPadCount, string.IsNullOrEmpty(suffix) ? null : (suffix.Length > 1) ? new StringComponent(suffix) : new CharacterComponent(suffix[0])) { }
        
        public VersionNumber(uint value, bool isNegative, int zeroPadCount, char suffix) : this(value, isNegative, zeroPadCount, new CharacterComponent(suffix)) { }
        
        public VersionNumber(uint value, bool isNegative, ITextComponent? suffix) : this(value, isNegative, 0, suffix) { }
        
        public VersionNumber(uint value, bool isNegative, string suffix) : this(value, isNegative, 0, suffix) { }
        
        public VersionNumber(uint value, bool isNegative, char suffix) : this(value, isNegative, 0, suffix) { }
        
        public VersionNumber(uint value, int zeroPadCount, ITextComponent? suffix) : this(value, false, zeroPadCount, suffix) { }
        
        public VersionNumber(uint value, int zeroPadCount, string suffix) : this(value, false, zeroPadCount, suffix) { }
        
        public VersionNumber(uint value, int zeroPadCount, char suffix) : this(value, false, zeroPadCount, suffix) { }
        
        public VersionNumber(int value, int zeroPadCount, ITextComponent? suffix)
        {
            if (zeroPadCount < 0)
                throw new ArgumentOutOfRangeException(nameof(zeroPadCount));
            if (suffix is null || suffix.Length == 0)
                Suffix = null;
            else
            {
                if (char.IsNumber(suffix.First()))
                    throw new ArgumentOutOfRangeException(nameof(suffix));
                Suffix = suffix;
            }
            if (value < 0)
            {
                Value = (uint)(0 - value);
                IsNegative = true;
            }
            else
            {
                Value = (uint)value;
                IsNegative = false;
            }
            ZeroPadCount = zeroPadCount;
        }
        
        public VersionNumber(int value, int zeroPadCount, string suffix) : this(value, zeroPadCount, string.IsNullOrEmpty(suffix) ? null : (suffix.Length > 1) ? new StringComponent(suffix) : new CharacterComponent(suffix[0])) { }
        
        public VersionNumber(int value, int zeroPadCount, char suffix) : this(value, zeroPadCount, new CharacterComponent(suffix)) { }
        
        public uint Value { get; }
        
        public bool IsNegative { get; }

        public int ZeroPadCount { get; }

        public ITextComponent? Suffix { get; }

        public DelimitedVersionNumber AsDelimited(ITextComponent delimiter) => new(delimiter, this);

        public int CompareTo(IVersionNumber? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(BigInteger other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(ulong other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(long other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(uint other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(int other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(ushort other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(short other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(byte other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(sbyte other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IVersionComponent? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object? obj)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IVersionNumber? other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(BigInteger other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(ulong other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(long other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(uint other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(int other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(ushort other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(short other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(byte other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(sbyte other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IVersionComponent? other)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<char> GetCharacters(bool includePaddedZeroes)
        {
            if (Suffix is null)
                return (includePaddedZeroes && ZeroPadCount > 0) ? (IsNegative ? Enumerable.Repeat('-', 1).Concat(Enumerable.Repeat('0', ZeroPadCount)) : Enumerable.Repeat('0', ZeroPadCount)).Concat(Value.ToString()) :
                    IsNegative ? Enumerable.Repeat('-', 1).Concat(Value.ToString()) : Value.ToString();
            return (includePaddedZeroes && ZeroPadCount > 0) ? (IsNegative ? Enumerable.Repeat('-', 1).Concat(Enumerable.Repeat('0', ZeroPadCount)).Concat(Suffix) :
                Enumerable.Repeat('0', ZeroPadCount)).Concat(Value.ToString()).Concat(Suffix) :
                (IsNegative ? Enumerable.Repeat('-', 1).Concat(Value.ToString()) : Value.ToString()).Concat(Suffix);
        }

        public IEnumerator<char> GetEnumerator()
        {
            if (Suffix is null)
                return (IsNegative ? Enumerable.Repeat('-', 1).Concat(Value.ToString()) : Value.ToString()).GetEnumerator();
            return (IsNegative ? Enumerable.Repeat('-', 1).Concat(Value.ToString()) : Value.ToString()).Concat(Suffix).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (Suffix is null)
                return ((IEnumerable)(IsNegative ? Enumerable.Repeat('-', 1).Concat(Value.ToString()) : Value.ToString())).GetEnumerator();
            return ((IEnumerable)(IsNegative ? Enumerable.Repeat('-', 1).Concat(Value.ToString()) : Value.ToString()).Concat(Suffix)).GetEnumerator();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 21 + Value.GetHashCode();
                return (Suffix is null) ? hash : 7 * hash + Suffix.GetHashCode();
            }
        }

        public string ToString(bool includePaddedZeroes)
        {
            if (Suffix is null)
            {
                if (includePaddedZeroes)
                    return IsNegative ? $"-{Enumerable.Repeat('-', ZeroPadCount)}{Value}" : $"{Enumerable.Repeat('-', ZeroPadCount)}{Value}";
                return IsNegative ? $"-{Value}" : Value.ToString();
            }
            if (includePaddedZeroes)
                return IsNegative ? $"-{Enumerable.Repeat('-', ZeroPadCount)}{Value}{Suffix}" : $"{Enumerable.Repeat('-', ZeroPadCount)}{Value}{Suffix}";
            return IsNegative ? $"-{Value}{Suffix}" : $"{Value}{Suffix}";
        }

        public override string ToString()
        {
            if (Suffix is null)
                return IsNegative ? $"-{Value}" : Value.ToString();
            return IsNegative ? $"-{Value}{Suffix}" : $"{Value}{Suffix}";
        }
    }
}