using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace CdnGetter.Versioning;

/// <summary>
/// Represents a token that represents a numerical value.
/// </summary>
public interface INumericalToken : IToken
{
    /// <summary>
    /// Gets the leading delimiter characters.
    /// </summary>
    /// <value>The leading delimiter characters or <see langword="null" /> if there aren't any.</value>
    ITokenCharacters? Delimiter { get; }

    /// <summary>
    /// Gets a value indicating whether the numeric value of the token is preceded by a negative symbol.
    /// </summary>
    bool HasNegativeSign { get; }

    /// <summary>
    /// Gets the number of padded zeros that precede the token value.
    /// </summary>
    int ZeroPadCount { get; }

    /// <summary>
    /// Gets the postfixed non-numerical characters.
    /// </summary>
    /// <value>The postfixed non-numerical characters or <see langword="null" /> if there aren't any.</value>
    string? NonNumerical { get; }

    /// <summary>
    /// Tries to gets the current value as a <see cref="byte" /> value.
    /// </summary>
    /// <param name="value">An 8-bit unsigned value.</param>
    /// <returns><see langword="true" /> if the current unsigned value is within range to be represented as a <see cref="byte" /> value; otherwise, <see langword="false" />.</returns>
    bool TryGet8Bit(out byte value);

    /// <summary>
    /// Tries to gets the current value as a <see cref="ushort" /> value.
    /// </summary>
    /// <param name="value">A 16-bit unsigned value.</param>
    /// <returns><see langword="true" /> if the current unsigned value is within range to be represented as a <see cref="ushort" /> value; otherwise, <see langword="false" />.</returns>
    bool TryGet16Bit(out ushort value);

    /// <summary>
    /// Tries to gets the current value as a <see cref="uint" /> value.
    /// </summary>
    /// <param name="value">A 32-bit unsigned value.</param>
    /// <returns><see langword="true" /> if the current unsigned value is within range to be represented as a <see cref="uint" /> value; otherwise, <see langword="false" />.</returns>
    bool TryGet32Bit(out uint value);

    /// <summary>
    /// Tries to gets the current value as a <see cref="ulong" /> value.
    /// </summary>
    /// <param name="value">A 64-bit unsigned value.</param>
    /// <returns><see langword="true" /> if the current unsigned value is within range to be represented as a <see cref="ulong" /> value; otherwise, <see langword="false" />.</returns>
    bool TryGet64Bit(out ulong value);

    /// <summary>
    /// Gets the current value as a <see cref="BigInteger" />.
    /// </summary>
    BigInteger AsUnsignedBigInteger();

    bool TryGetNonNumerical([NotNullWhen(true)] out string? nonNumerical);
}
