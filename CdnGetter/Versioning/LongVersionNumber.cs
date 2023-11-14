using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace CdnGetter.Versioning
{
    public readonly struct LongVersionNumber : IVersionNumber
    {
        public LongVersionNumber(ulong value, bool isNegative = false, int zeroPadCount = 0, ITextComponent? suffix = null)
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
            ZeroPadCount = zeroPadCount;
            IsNegative = isNegative;
            Value = value;
        }
        
        public LongVersionNumber(ulong value, bool isNegative, int zeroPadCount, string suffix) : this(value, isNegative, zeroPadCount, TextComponent.AsTextComponent(suffix)) { }
        
        public LongVersionNumber(ulong value, bool isNegative, int zeroPadCount, char suffix) : this(value, isNegative, zeroPadCount, new CharacterComponent(suffix)) { }
        
        public LongVersionNumber(ulong value, bool isNegative, ITextComponent? suffix) : this(value, isNegative, 0, suffix) { }
        
        public LongVersionNumber(ulong value, bool isNegative, string suffix) : this(value, isNegative, 0, suffix) { }
        
        public LongVersionNumber(ulong value, bool isNegative, char suffix) : this(value, isNegative, 0, suffix) { }
        
        public LongVersionNumber(ulong value, int zeroPadCount, ITextComponent? suffix = null) : this(value, false, zeroPadCount, suffix) { }
        
        public LongVersionNumber(ulong value, int zeroPadCount, string suffix) : this(value, false, zeroPadCount, suffix) { }
        
        public LongVersionNumber(ulong value, int zeroPadCount, char suffix) : this(value, false, zeroPadCount, suffix) { }
        
        public LongVersionNumber(ulong value, ITextComponent? suffix) : this(value, false, 0, suffix) { }
        
        public LongVersionNumber(ulong value, char suffix) : this(value, false, 0, suffix) { }
        
        public LongVersionNumber(long value, int zeroPadCount = 0, ITextComponent? suffix = null)
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
            ZeroPadCount = zeroPadCount;
            IsNegative = value < 0L;
            Value = (ulong)(IsNegative ? 0L - value : value);
        }
        
        public LongVersionNumber(long value, ITextComponent? suffix = null) : this(value, 0, suffix) { }
        
        public ulong Value { get; }
        
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