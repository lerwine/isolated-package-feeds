using System.Numerics;

namespace CdnGetter.Parsing;

/// <summary>
/// Represents a token that represents a numerical value.
/// </summary>
public interface INumericalToken : IToken
{
    /// <summary>
    /// Gets a value that indicates whether the numerical value of the token is prefixed by a negative sign.
    /// </summary>
    bool HasNegativeSign { get; }

    /// <summary>
    /// Gets the number of leading zeros for the numerical value.
    /// </summary>
    int ZeroPadLength { get; }

    /// <summary>
    /// Gets a value that indicates whether this token represents a zero value.
    /// </summary>
    bool IsZero { get; }

    /// <summary>
    /// Compares the absolute value of the current token with a <see cref="BigInteger" /> value.
    /// </summary>
    /// <param name="other">The value to compare to.</param>
    /// <returns>A value less than <c>0</c> if the absolute numerical value of the current token is less than the <paramref name="other" /> value;
    /// greater than <c>0</c> if the absolute numerical value of the current token is greater than the <paramref name="other" /> value;
    /// otherwise <c>0</> fi the absolute value fo the current token is equal to the <paramref name="other" /> value.</returns>
    int CompareAbs(BigInteger other);
    
    /// <summary>
    /// Compares the absolute value of the current token with a <see cref="ulong" /> value.
    /// </summary>
    /// <param name="other">The value to compare to.</param>
    /// <returns>A value less than <c>0</c> if the absolute numerical value of the current token is less than the <paramref name="other" /> value;
    /// greater than <c>0</c> if the absolute numerical value of the current token is greater than the <paramref name="other" /> value;
    /// otherwise <c>0</> fi the absolute value fo the current token is equal to the <paramref name="other" /> value.</returns>
    int CompareAbs(ulong other);
    
    /// <summary>
    /// Compares the absolute value of the current token with a <see cref="uint" /> value.
    /// </summary>
    /// <param name="other">The value to compare to.</param>
    /// <returns>A value less than <c>0</c> if the absolute numerical value of the current token is less than the <paramref name="other" /> value;
    /// greater than <c>0</c> if the absolute numerical value of the current token is greater than the <paramref name="other" /> value;
    /// otherwise <c>0</> fi the absolute value fo the current token is equal to the <paramref name="other" /> value.</returns>
    int CompareAbs(uint other);
    
    /// <summary>
    /// Compares the absolute value of the current token with a <see cref="ushort" /> value.
    /// </summary>
    /// <param name="other">The value to compare to.</param>
    /// <returns>A value less than <c>0</c> if the absolute numerical value of the current token is less than the <paramref name="other" /> value;
    /// greater than <c>0</c> if the absolute numerical value of the current token is greater than the <paramref name="other" /> value;
    /// otherwise <c>0</> fi the absolute value fo the current token is equal to the <paramref name="other" /> value.</returns>
    int CompareAbs(ushort other);
    
    /// <summary>
    /// Compares the absolute value of the current token with a <see cref="byte" /> value.
    /// </summary>
    /// <param name="other">The value to compare to.</param>
    /// <returns>A value less than <c>0</c> if the absolute numerical value of the current token is less than the <paramref name="other" /> value;
    /// greater than <c>0</c> if the absolute numerical value of the current token is greater than the <paramref name="other" /> value;
    /// otherwise <c>0</> fi the absolute value fo the current token is equal to the <paramref name="other" /> value.</returns>
    int CompareAbs(byte other);
    
    /// <summary>
    /// Tests whether the absolute value of the current token is equal to a <see cref="BigInteger" /> value.
    /// </summary>
    /// <param name="other">The value to compare to.</param>
    /// <returns><see langword="true" /> if the absolute numerical value of the current token is equal to the <paramref name="other" /> value; otherwise, <see langword="false" />.</returns>
    bool EqualsAbs(BigInteger other);
    
    /// <summary>
    /// Tests whether the absolute value of the current token is equal to a <see cref="ulong" /> value.
    /// </summary>
    /// <param name="other">The value to compare to.</param>
    /// <returns><see langword="true" /> if the absolute numerical value of the current token is equal to the <paramref name="other" /> value; otherwise, <see langword="false" />.</returns>
    bool EqualsAbs(ulong other);
    
    /// <summary>
    /// Tests whether the absolute value of the current token is equal to a <see cref="uint" /> value.
    /// </summary>
    /// <param name="other">The value to compare to.</param>
    /// <returns><see langword="true" /> if the absolute numerical value of the current token is equal to the <paramref name="other" /> value; otherwise, <see langword="false" />.</returns>
    bool EqualsAbs(uint other);
    
    /// <summary>
    /// Tests whether the absolute value of the current token is equal to a <see cref="ushort" /> value.
    /// </summary>
    /// <param name="other">The value to compare to.</param>
    /// <returns><see langword="true" /> if the absolute numerical value of the current token is equal to the <paramref name="other" /> value; otherwise, <see langword="false" />.</returns>
    bool EqualsAbs(ushort other);
    
    /// <summary>
    /// Tests whether the absolute value of the current token is equal to a <see cref="byte" /> value.
    /// </summary>
    /// <param name="other">The value to compare to.</param>
    /// <returns><see langword="true" /> if the absolute numerical value of the current token is equal to the <paramref name="other" /> value; otherwise, <see langword="false" />.</returns>
    bool EqualsAbs(byte other);
    
    /// <summary>
    /// Attempts to return the absolute value of the current token as a <see cref="byte" /> value.
    /// </summary>
    /// <param name="value">The byte value of the absolute value of the current token or <c>0</c> if the absolute value is greater than <see cref="byte.MaxValue /></param>
    /// <returns><see langword="true" /> if  the absolute numerical value of the current token is less than or equal to <see cref="byte.MaxValue />; otherwise, <see langword="false" />.</returns>
    bool TryGet8Bit(out byte value);

    /// <summary>
    /// Attempts to return the absolute value of the current token as a <see cref="ushort" /> value.
    /// </summary>
    /// <param name="value">The byte value of the absolute value of the current token or <c>0</c> if the absolute value is greater than <see cref="ushort.MaxValue /></param>
    /// <returns><see langword="true" /> if  the absolute numerical value of the current token is less than or equal to <see cref="ushort.MaxValue />; otherwise, <see langword="false" />.</returns>
    bool TryGet16Bit(out ushort value);

    /// <summary>
    /// Attempts to return the absolute value of the current token as a <see cref="uint" /> value.
    /// </summary>
    /// <param name="value">The byte value of the absolute value of the current token or <c>0</c> if the absolute value is greater than <see cref="uint.MaxValue /></param>
    /// <returns><see langword="true" /> if  the absolute numerical value of the current token is less than or equal to <see cref="uint.MaxValue />; otherwise, <see langword="false" />.</returns>
    bool TryGet32Bit(out uint value);

    /// <summary>
    /// Attempts to return the absolute value of the current token as a <see cref="ulong" /> value.
    /// </summary>
    /// <param name="value">The byte value of the absolute value of the current token or <c>0</c> if the absolute value is greater than <see cref="ulong.MaxValue /></param>
    /// <returns><see langword="true" /> if  the absolute numerical value of the current token is less than or equal to <see cref="ulong.MaxValue />; otherwise, <see langword="false" />.</returns>
    bool TryGet64Bit(out ulong value);

    /// <summary>
    /// Gets the absolute value of the current otken as a <see cref="BigInteger" /> value.
    /// </summary>
    /// <returns>The absolute value of the current otken as a <see cref="BigInteger" /> value.</returns>
    BigInteger AsBigInteger();
}
