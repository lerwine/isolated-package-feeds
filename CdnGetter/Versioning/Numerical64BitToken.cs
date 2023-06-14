using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using static CdnGetter.Versioning.VersioningConstants;

namespace CdnGetter.Versioning;

#pragma warning disable CA2231
public readonly struct Numerical64BitToken : INumericalToken
#pragma warning restore CA2231
{
    internal static readonly int MAX_STRING_LENGTH;

    /// <summary>
    /// Gets the leading delimiter characters.
    /// </summary>
    /// <value>The leading delimiter characters or <see langword="null" /> if there aren't any.</value>
    public ITokenCharacters? Delimiter { get; }

    /// <summary>
    /// Gets a value indicating whether the numeric value of the token is preceded by a negative symbol.
    /// </summary>
    public bool HasNegativeSign { get; }

    /// <summary>
    /// Gets the number of padded zeros that precede the token value.
    /// </summary>
    public int ZeroPadCount { get; }

    /// <summary>
    /// Gets the unsigned value.
    /// </summary>
    public ulong Value { get; }

    /// <summary>
    /// Gets the postfixed non-numerical characters.
    /// </summary>
    /// <value>The postfixed non-numerical characters or <see langword="null" /> if there aren't any.</value>
    public string? NonNumerical { get; }

    bool INumericalToken.IsZero => Value == 0;

    static Numerical64BitToken() => MAX_STRING_LENGTH = ulong.MaxValue.ToString().Length;
    
    /// <summary>
    /// Constructs a new <see cref="Numerical64BitToken" />.
    /// </summary>
    /// <param name="delimiter">The leading delimiter characters.</param>
    /// <param name="value">The numeric value.</param>
    /// <param name="zeroPadCount">The number of padded zeros to precede the token value. The default is <c>0</c>.</param>
    /// <param name="nonNumerical">The trailing non-numerical characters or <see langword="null" /> for none.</param>
    /// <param name="hasNegativeSign">Indicates whether the numeric value of the token is to be preceded by a negative symbol, regardless of whether the numeric value is negative. The default is <see langword="false" />.</param>
    public Numerical64BitToken(ITokenCharacters delimiter, ulong value, int zeroPadCount = 0, string? nonNumerical = null, bool hasNegativeSign = false) =>
        (Delimiter, Value, ZeroPadCount, NonNumerical, HasNegativeSign) = (AssertValidNumericDelimiter(delimiter), value, (zeroPadCount < 0) ? 0 : zeroPadCount, AssertValidNumericalPostfix(nonNumerical), hasNegativeSign);

    /// <summary>
    /// Constructs a new <see cref="Numerical64BitToken" />.
    /// </summary>
    /// <param name="delimiter">The leading delimiter characters.</param>
    /// <param name="value">The numeric value.</param>
    /// <param name="zeroPadCount">The number of padded zeros to precede the token value.</param>
    /// <param name="hasNegativeSign">Indicates whether the numeric value of the token is to be preceded by a negative symbol, regardless of whether the numeric value is negative.</param>
    public Numerical64BitToken(ITokenCharacters delimiter, ulong value, int zeroPadCount, bool hasNegativeSign) =>
        (Delimiter, Value, ZeroPadCount, NonNumerical, HasNegativeSign) = (AssertValidNumericDelimiter(delimiter), value, (zeroPadCount < 0) ? 0 : zeroPadCount, null, hasNegativeSign);

    /// <summary>
    /// Constructs a new <see cref="Numerical64BitToken" />.
    /// </summary>
    /// <param name="delimiter">The leading delimiter characters.</param>
    /// <param name="value">The numeric value.</param>
    /// <param name="nonNumerical">The trailing non-numerical characters.</param>
    /// <param name="hasNegativeSign">Indicates whether the numeric value of the token is to be preceded by a negative symbol, regardless of whether the numeric value is negative. The default is <see langword="false" />.</param>
    public Numerical64BitToken(ITokenCharacters delimiter, ulong value, string nonNumerical, bool hasNegativeSign = false) =>
        (Delimiter, Value, ZeroPadCount, NonNumerical, HasNegativeSign) = (AssertValidNumericDelimiter(delimiter), value, 0, AssertValidNumericalPostfix(nonNumerical), hasNegativeSign);

    /// <summary>
    /// Constructs a new <see cref="Numerical64BitToken" />.
    /// </summary>
    /// <param name="value">The numeric value.</param>
    /// <param name="zeroPadCount">The number of padded zeros to precede the token value. The default is <c>0</c>.</param>
    /// <param name="nonNumerical">The trailing non-numerical characters or <see langword="null" /> for none.</param>
    /// <param name="hasNegativeSign">Indicates whether the numeric value of the token is to be preceded by a negative symbol, regardless of whether the numeric value is negative. The default is <see langword="false" />.</param>
    public Numerical64BitToken(ulong value, int zeroPadCount = 0, string? nonNumerical = null, bool hasNegativeSign = false) =>
        (Delimiter, Value, ZeroPadCount, NonNumerical, HasNegativeSign) = (null, value, (zeroPadCount < 0) ? 0 : zeroPadCount, AssertValidNumericalPostfix(nonNumerical), hasNegativeSign);

    /// <summary>
    /// Constructs a new <see cref="Numerical64BitToken" />.
    /// </summary>
    /// <param name="delimiter">The leading delimiter characters.</param>
    /// <param name="value">The numeric value.</param>
    /// <param name="hasNegativeSign">Indicates whether the numeric value of the token is to be preceded by a negative symbol, regardless of whether the numeric value is negative.</param>
    public Numerical64BitToken(ITokenCharacters delimiter, ulong value, bool hasNegativeSign) => (Delimiter, Value, ZeroPadCount, NonNumerical, HasNegativeSign) = (AssertValidNumericDelimiter(delimiter), value, 0, null, hasNegativeSign);

    /// <summary>
    /// Constructs a new <see cref="Numerical64BitToken" />.
    /// </summary>
    /// <param name="value">The target value.</param>
    /// <param name="zeroPadCount">The number of padded zeros to precede the token value.</param>
    /// <param name="hasNegativeSign">Indicates whether the numeric value of the token is to be preceded by a negative symbol, regardless of whether the numeric value is negative.</param>
    public Numerical64BitToken(ulong value, int zeroPadCount, bool hasNegativeSign) => (Delimiter, Value, ZeroPadCount, NonNumerical, HasNegativeSign) = (null, value, (zeroPadCount < 0) ? 0 : zeroPadCount, null, hasNegativeSign);

    /// <summary>
    /// Constructs a new <see cref="Numerical64BitToken" />.
    /// </summary>
    /// <param name="value">The numeric value.</param>
    /// <param name="nonNumerical">The trailing non-numerical characters.</param>
    /// <param name="hasNegativeSign">Indicates whether the numeric value of the token is to be preceded by a negative symbol, regardless of whether the numeric value is negative. The default is <see langword="false" />.</param>
    public Numerical64BitToken(ulong value, string nonNumerical, bool hasNegativeSign = false) =>
        (Delimiter, Value, ZeroPadCount, NonNumerical, HasNegativeSign) = (null, value, 0, AssertValidNumericalPostfix(nonNumerical), hasNegativeSign);

    /// <summary>
    /// Constructs a new <see cref="Numerical64BitToken" />.
    /// </summary>
    /// <param name="value">The numeric value.</param>
    /// <param name="hasNegativeSign">Indicates whether the numeric value of the token is to be preceded by a negative symbol, regardless of whether the numeric value is negative.</param>
    public Numerical64BitToken(ulong value, bool hasNegativeSign) => (Delimiter, Value, ZeroPadCount, NonNumerical, HasNegativeSign) = (null, value, 0, null, hasNegativeSign);

    BigInteger INumericalToken.AsUnsignedBigInteger() => Value;

    public int CompareTo(IToken? other)
    {
        if (other is null)
            return 1;
        int result;
        if (other is INumericalToken n)
        {
            if (n.IsZero)
                result = Value.CompareTo(0);
            else
            {
                if (Value == 0)
                    return n.HasNegativeSign ? 1 : -1;
                if (n.HasNegativeSign)
                {
                    if (!HasNegativeSign)
                        return 1;
                    if (other is Numerical8BitToken t8a)
                        result = t8a.Value.CompareTo(Value);
                    else if (other is Numerical16BitToken t16)
                        result = t16.Value.CompareTo(Value);
                    else if (other is Numerical32BitToken t32)
                        result = t32.Value.CompareTo(Value);
                    else if (other is Numerical64BitToken t64)
                        result = t64.Value.CompareTo(Value);
                    else
                    {
                        BigInteger b = n.AsUnsignedBigInteger();
                        if (b > BigNumericalToken.MAX_8BIT)
                            return 1;
                        result = b.CompareTo(Value);
                    }
                    if (result != 0)
                        return result;
                    if (string.IsNullOrEmpty(n.NonNumerical))
                        return (NonNumerical is null) ? 0 : -1;
                    return (NonNumerical is null) ? 1 : n.NonNumerical.CompareTo(NonNumerical);
                }
                
                if (HasNegativeSign)
                    return -1;
                if (other is Numerical8BitToken t8b)
                    result = Value.CompareTo(t8b.Value);
                else if (other is RomanNumeralToken r)
                    result = (Value > RomanNumeralToken.MAX_VALUE) ? 1 : Value.CompareTo(r.GetNumericalValue());
                else if (other is Numerical16BitToken t16)
                    result = Value.CompareTo(t16.Value);
                else if (other is Numerical32BitToken t32)
                    result = Value.CompareTo(t32.Value);
                else if (other is Numerical64BitToken t64)
                    result = Value.CompareTo(t64.Value);
                else
                {
                    BigInteger b = n.AsUnsignedBigInteger();
                    if (b > BigNumericalToken.MAX_64BIT)
                        return -1;
                    result =  new BigInteger(Value).CompareTo(b);
                }
            }
            if (result != 0)
                return result;
        }
        else if ((result = NoCaseComparer.Compare(GetValue(), other.GetValue())) != 0)
            return result;
        string? p = other.GetPostFixed();
        if (string.IsNullOrEmpty(p))
            return (NonNumerical is null) ? 0 : 1;
        return (NonNumerical is null) ? -1 : NoCaseComparer.Compare(NonNumerical, p);
    }

    public bool Equals(IToken? other)
    {
        if (other is null)
            return false;
        if (other is INumericalToken n)
        {
            if (n.IsZero)
            {
                if (Value != 0)
                    return false;
            }
            else if (Value == 0 || n.HasNegativeSign != HasNegativeSign || !((n is Numerical8BitToken t8b) ? Value.Equals(t8b.Value) : (n is RomanNumeralToken r) ? (Value <= RomanNumeralToken.MAX_VALUE && Value.Equals(r.GetNumericalValue())) :
                    (n is Numerical16BitToken t16) ? Value.Equals(t16.Value) : (n is Numerical32BitToken t32) ? Value.Equals(t32.Value) : (n is Numerical64BitToken t64) ? Value.Equals(t64.Value) : n.AsUnsignedBigInteger().Equals(Value)))
                return false;
        }
        else if (!NoCaseComparer.Equals(GetValue(), other.GetValue()))
            return false;
        string? p = other.GetPostFixed();
        return string.IsNullOrEmpty(p) ? NonNumerical is null : NonNumerical is not null && NoCaseComparer.Equals(NonNumerical, p);
    }

    public override bool Equals(object? obj) => obj is IToken other && Equals(other);

    ITokenCharacters? IToken.GetDelimiter() => Delimiter;

    public override int GetHashCode() => HashCode.Combine(HasNegativeSign, Value, NonNumerical);

    string? IToken.GetPostFixed() => NonNumerical;

    public string GetValue() => HasNegativeSign ?
        ((ZeroPadCount > 0) ?
            $"-{new string('0', ZeroPadCount)}{Value}" :
            $"-{Value}"
        ) :
        (ZeroPadCount > 0) ?
            $"{new string('0', ZeroPadCount)}{Value}" :
            Value.ToString();

    public override string ToString() => (Delimiter is null) ?
        (
            (NonNumerical is null) ?
            (HasNegativeSign ?
                ((ZeroPadCount > 0) ?
                    $"-{new string('0', ZeroPadCount)}{Value}" :
                    $"-{Value}"
                ) :
                (ZeroPadCount > 0) ?
                    $"{new string('0', ZeroPadCount)}{Value}" :
                    Value.ToString()) :
            HasNegativeSign ?
            ((ZeroPadCount > 0) ?
                $"-{new string('0', ZeroPadCount)}{Value}{NonNumerical}" :
                $"-{Value}{NonNumerical}"
            ) :
            (ZeroPadCount > 0) ?
                $"{new string('0', ZeroPadCount)}{Value}{NonNumerical}" :
                $"{Value}{NonNumerical}"
        ) :
        (NonNumerical is null) ?
        (HasNegativeSign ?
            ((ZeroPadCount > 0) ?
                $"{Delimiter}-{new string('0', ZeroPadCount)}{Value}" :
                $"{Delimiter}-{Value}"
            ) :
            (ZeroPadCount > 0) ?
                $"{Delimiter}{new string('0', ZeroPadCount)}{Value}" :
                $"{Delimiter}{Value}") :
        HasNegativeSign ?
        ((ZeroPadCount > 0) ?
            $"{Delimiter}-{new string('0', ZeroPadCount)}{Value}{NonNumerical}" :
            $"{Delimiter}-{Value}{NonNumerical}"
        ) :
        (ZeroPadCount > 0) ?
            $"{Delimiter}{new string('0', ZeroPadCount)}{Value}{NonNumerical}" :
            $"{Delimiter}{Value}{NonNumerical}";

    bool INumericalToken.TryGet8Bit(out byte value)
    {
        if (Value > byte.MaxValue)
        {
            value = default;
            return false;
        }
        value = (byte)Value;
        return true;
    }

    bool INumericalToken.TryGet16Bit(out ushort value)
    {
        if (Value > ushort.MaxValue)
        {
            value = default;
            return false;
        }
        value = (ushort)Value;
        return true;
    }

    bool INumericalToken.TryGet32Bit(out uint value)
    {
        if (Value > uint.MaxValue)
        {
            value = default;
            return false;
        }
        value = (uint)Value;
        return true;
    }

    bool INumericalToken.TryGet64Bit(out ulong value)
    {
        value = Value;
        return true;
    }

    public bool TryGetDelimiter([NotNullWhen(true)] out string? delimiter)
    {
        if (Delimiter is null)
        {
            delimiter = null;
            return false;
        }
        delimiter = Delimiter.ToString();
        return true;
    }

    public bool TryGetNonNumerical([NotNullWhen(true)] out string? nonNumerical) => (nonNumerical = NonNumerical) is not null;

    bool IToken.TryGetPostFixed([NotNullWhen(true)] out string? postfixed) => TryGetNonNumerical(out postfixed);
}
