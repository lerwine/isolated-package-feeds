using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using static CdnGetter.Versioning.TextComponent;

namespace CdnGetter.Versioning
{
    public readonly struct BigVersionNumber : IVersionNumber
    {
        public BigVersionNumber(BigInteger value, int zeroPadCount = 0, ITextComponent? suffix = null)
        {
            if (zeroPadCount < 0)
                throw new ArgumentOutOfRangeException(nameof(zeroPadCount));
            if (suffix is null || suffix.Length == 0)
                Suffix = null;
            else
            {
                if (char.IsNumber(suffix.GetChars().First()))
                    throw new ArgumentOutOfRangeException(nameof(suffix));
                Suffix = suffix;
            }
            IsNegative = value.Sign < 0;
            ZeroPadCount = zeroPadCount;
            Value = IsNegative ? BigInteger.Negate(value) : value;
        }

        public BigVersionNumber(BigInteger value, int zeroPadCount, string suffix) : this(value, zeroPadCount, AsTextComponent(suffix)) { }

        public BigVersionNumber(BigInteger value, int zeroPadCount, char suffix) : this(value, zeroPadCount, new CharacterComponent(suffix)) { }

        public BigVersionNumber(BigInteger value, ITextComponent suffix) : this(value, 0, suffix) { }

        public BigVersionNumber(BigInteger value, string suffix) : this(value, 0, AsTextComponent(suffix)) { }

        public BigVersionNumber(BigInteger value, char suffix) : this(value, 0, new CharacterComponent(suffix)) { }

        public BigInteger Value { get; }
        
        public bool IsNegative { get; }

        public int ZeroPadCount { get; }

        public ITextComponent? Suffix { get; }

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

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string ToString(bool omitPaddedZeroes)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => ToString(false);
    }
}