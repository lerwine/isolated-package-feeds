using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace CdnGetter.Versioning
{
    public readonly struct TinyVersionNumber : IVersionNumber
    {
        public byte Value { get; }
        
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