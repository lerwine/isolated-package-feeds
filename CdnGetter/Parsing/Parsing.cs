using System.Numerics;
using CdnGetter.Parsing.Version;

namespace CdnGetter.Parsing;

public static class Parsing
{
    public const ushort ROMAN_NUMERAL_MAX_VALUE = 3999;

    internal const char DELIMITER_DASH = '-';
    
    public const char ZERO = '0';

    public static readonly StringComparer NoCaseComparer = StringComparer.InvariantCultureIgnoreCase;

    public static INumericalToken ToNumericalToken(short value) => (value == 0) ? Digits8Bit.Zero : (value > byte.MaxValue || value < sbyte.MinValue) ? new Digits16Bit(value) : (value < 0) ? new Digits8Bit((sbyte)value) : new Digits8Bit((byte)value);

    public static INumericalToken ToNumericalToken(ushort value) => (value == 0) ? Digits8Bit.Zero : (value > byte.MaxValue) ? new Digits16Bit(value) : new Digits8Bit((byte)value);

    public static INumericalToken ToNumericalToken(int value) => (value == 0) ? Digits8Bit.Zero : (value > ushort.MaxValue || value < short.MinValue) ? new Digits32Bit(value) : (value > short.MaxValue) ? new Digits16Bit((ushort)value) :
        (value > byte.MaxValue || value < sbyte.MinValue) ? new Digits16Bit((short)value) : (value < 0) ? new Digits8Bit((sbyte)value) : new Digits8Bit((byte)value);

    public static INumericalToken ToNumericalToken(uint value) => (value == 0u) ? Digits8Bit.Zero : (value > ushort.MaxValue) ? new Digits32Bit(value) : (value > byte.MaxValue) ? new Digits16Bit((ushort)value) : new Digits8Bit((byte)value);

    public static INumericalToken ToNumericalToken(long value) => (value == 0L) ? Digits8Bit.Zero : (value > uint.MaxValue || value < int.MinValue) ? new Digits64Bit(value) : (value > int.MaxValue) ? new Digits32Bit((uint)value) :
        (value > ushort.MaxValue || value < short.MinValue) ? new Digits32Bit((int)value) : (value > short.MaxValue) ? new Digits16Bit((ushort)value) :
        (value > byte.MaxValue || value < sbyte.MinValue) ? new Digits16Bit((short)value) : (value < 0) ? new Digits8Bit((sbyte)value) : new Digits8Bit((byte)value);

    public static INumericalToken ToNumericalToken(ulong value) => (value == 0UL) ? Digits8Bit.Zero : (value > uint.MinValue) ? new Digits64Bit(value) : (value > ushort.MaxValue) ? new Digits32Bit((uint)value) :
        (value > byte.MaxValue) ? new Digits16Bit((ushort)value) : new Digits8Bit((byte)value);

    public static INumericalToken ToNumericalToken(BigInteger value) => (value == BigInteger.Zero) ? Digits8Bit.Zero : (value < long.MinValue || value > ulong.MaxValue) ? new DigitsNBit(value) :
        (value > long.MaxValue) ? new Digits64Bit((ulong)value) : (value > uint.MaxValue || value < int.MinValue) ? new Digits64Bit((long)value) : (value > int.MaxValue) ? new Digits32Bit((uint)value) :
        (value > ushort.MaxValue || value < short.MinValue) ? new Digits32Bit((int)value) : (value > short.MaxValue) ? new Digits16Bit((ushort)value) :
        (value > byte.MaxValue || value < sbyte.MinValue) ? new Digits16Bit((short)value) : (value < 0) ? new Digits8Bit((sbyte)value) : new Digits8Bit((byte)value);
        

    public static readonly CharacterToken DotToken = new('.');

    public static DelimitedNumericalToken ToDelimitedNumericalToken(short value, CharacterToken? delimiter = null) => (value == 0) ? new(delimiter ?? DotToken, Digits8Bit.Zero) :
        (value > byte.MaxValue || value < sbyte.MinValue) ? new(delimiter ?? DotToken, new Digits16Bit(value)) : (value < 0) ? new(delimiter ?? DotToken, new Digits8Bit((sbyte)value)) :
        new(delimiter ?? DotToken, new Digits8Bit((byte)value));

    public static DelimitedNumericalToken ToDelimitedNumericalToken(ushort value, CharacterToken? delimiter = null) => (value == 0) ? new(delimiter ?? DotToken, Digits8Bit.Zero) :
        (value > byte.MaxValue) ? new(delimiter ?? DotToken, new Digits16Bit(value)) : new(delimiter ?? DotToken, new Digits8Bit((byte)value));

    public static DelimitedNumericalToken ToDelimitedNumericalToken(int value, CharacterToken? delimiter = null) => (value == 0) ? new(delimiter ?? DotToken, Digits8Bit.Zero) :
        (value > ushort.MaxValue || value < short.MinValue) ? new(delimiter ?? DotToken, new Digits32Bit(value)) : (value > short.MaxValue) ? new(delimiter ?? DotToken, new Digits16Bit((ushort)value)) :
        (value > byte.MaxValue || value < sbyte.MinValue) ? new(delimiter ?? DotToken, new Digits16Bit((short)value)) : (value < 0) ? new(delimiter ?? DotToken, new Digits8Bit((sbyte)value)) :
        new(delimiter ?? DotToken, new Digits8Bit((byte)value));

    public static DelimitedNumericalToken ToDelimitedNumericalToken(uint value, CharacterToken? delimiter = null) => (value == 0u) ? new(delimiter ?? DotToken, Digits8Bit.Zero) :
        (value > ushort.MaxValue) ? new(delimiter ?? DotToken, new Digits32Bit(value)) : (value > byte.MaxValue) ? new(delimiter ?? DotToken, new Digits16Bit((ushort)value)) :
        new(delimiter ?? DotToken, new Digits8Bit((byte)value));

    public static DelimitedNumericalToken ToDelimitedNumericalToken(long value, CharacterToken? delimiter = null) => (value == 0L) ? new(delimiter ?? DotToken, Digits8Bit.Zero) :
        (value > uint.MaxValue || value < int.MinValue) ? new(delimiter ?? DotToken, new Digits64Bit(value)) : (value > int.MaxValue) ? new(delimiter ?? DotToken, new Digits32Bit((uint)value)) :
        (value > ushort.MaxValue || value < short.MinValue) ? new(delimiter ?? DotToken, new Digits32Bit((int)value)) : (value > short.MaxValue) ? new(delimiter ?? DotToken, new Digits16Bit((ushort)value)) :
        (value > byte.MaxValue || value < sbyte.MinValue) ? new(delimiter ?? DotToken, new Digits16Bit((short)value)) : (value < 0) ? new(delimiter ?? DotToken, new Digits8Bit((sbyte)value)) :
        new(delimiter ?? DotToken, new Digits8Bit((byte)value));

    public static DelimitedNumericalToken ToDelimitedNumericalToken(ulong value, CharacterToken? delimiter = null) => (value == 0UL) ? new(delimiter ?? DotToken, Digits8Bit.Zero) :
        (value > uint.MinValue) ? new(delimiter ?? DotToken, new Digits64Bit(value)) : (value > ushort.MaxValue) ? new(delimiter ?? DotToken, new Digits32Bit((uint)value)) :
        (value > byte.MaxValue) ? new(delimiter ?? DotToken, new Digits16Bit((ushort)value)) : new(delimiter ?? DotToken, new Digits8Bit((byte)value));

    public static DelimitedNumericalToken ToDelimitedNumericalToken(BigInteger value, CharacterToken? delimiter = null) => (value == BigInteger.Zero) ? new(delimiter ?? DotToken, Digits8Bit.Zero) :
        (value < long.MinValue || value > ulong.MaxValue) ? new(delimiter ?? DotToken, new DigitsNBit(value)) : (value > long.MaxValue) ? new(delimiter ?? DotToken, new Digits64Bit((ulong)value)) :
        (value > uint.MaxValue || value < int.MinValue) ? new(delimiter ?? DotToken, new Digits64Bit((long)value)) : (value > int.MaxValue) ? new(delimiter ?? DotToken, new Digits32Bit((uint)value)) :
        (value > ushort.MaxValue || value < short.MinValue) ? new(delimiter ?? DotToken, new Digits32Bit((int)value)) : (value > short.MaxValue) ? new(delimiter ?? DotToken, new Digits16Bit((ushort)value)) :
        (value > byte.MaxValue || value < sbyte.MinValue) ? new(delimiter ?? DotToken, new Digits16Bit((short)value)) : (value < 0) ? new(delimiter ?? DotToken, new Digits8Bit((sbyte)value)) :
        new(delimiter ?? DotToken, new Digits8Bit((byte)value));
}
