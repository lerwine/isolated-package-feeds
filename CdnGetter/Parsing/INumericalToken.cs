using System.Numerics;

namespace CdnGetter.Parsing;

public interface INumericalToken : IToken
{
    bool HasNegativeSign { get; }

    int ZeroPadLength { get; }

    bool IsZero { get; }

    int CompareAbs(BigInteger other);
    
    int CompareAbs(ulong other);
    
    int CompareAbs(uint other);
    
    int CompareAbs(ushort other);
    
    int CompareAbs(byte other);
    
    bool EqualsAbs(BigInteger other);
    
    bool EqualsAbs(ulong other);
    
    bool EqualsAbs(uint other);
    
    bool EqualsAbs(ushort other);
    
    bool EqualsAbs(byte other);
    
    bool TryGet8Bit(out byte value);

    bool TryGet16Bit(out ushort value);

    bool TryGet32Bit(out uint value);

    bool TryGet64Bit(out ulong value);

    BigInteger AsBigInteger();
}
