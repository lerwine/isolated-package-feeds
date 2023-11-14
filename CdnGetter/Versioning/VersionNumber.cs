using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace CdnGetter.Versioning
{
    public readonly struct VersionNumber : IVersionNumber
    {
        public VersionNumber(uint value, bool isNegative, int zeroPadCount, ITextComponent? suffix)
        {
            if (zeroPadCount < 0)
                throw new ArgumentOutOfRangeException(nameof(zeroPadCount));
            Value = value;
            IsNegative = isNegative;
            ZeroPadCount = zeroPadCount;
            Suffix = (suffix is not null && suffix.Length > 0) ? suffix : null;
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
            Suffix = (suffix is not null && suffix.Length > 0) ? suffix : null;
        }
        
        public VersionNumber(int value, int zeroPadCount, string suffix) : this(value, zeroPadCount, string.IsNullOrEmpty(suffix) ? null : (suffix.Length > 1) ? new StringComponent(suffix) : new CharacterComponent(suffix[0])) { }
        
        public VersionNumber(int value, int zeroPadCount, char suffix) : this(value, zeroPadCount, new CharacterComponent(suffix)) { }
        
        // public VersionNumber(uint value, ITextComponent? suffix)
        // {

        // }
        
        // public VersionNumber(uint value, string suffix)
        // {

        // }
        
        // public VersionNumber(uint value, char suffix)
        // {

        // }
        
        // public VersionNumber(int value, ITextComponent? suffix)
        // {

        // }
        
        // public VersionNumber(int value, string suffix)
        // {

        // }
        
        // public VersionNumber(int value, char suffix)
        // {

        // }
        
        public uint Value { get; }
        
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