namespace CdnGetter.Parsing;

/// <summary>
/// Represents a parsed token.
/// </summary>
public interface IToken : IComparable<IToken>, IEquatable<IToken>
{
    /// <summary>
    /// Gets the length of the current token.
    /// </summary>
    /// <param name="allParsedValues">If <see langword="true" />, then the length, including any extraneous source values, is returned;
    /// otherwise this will return the length of the consequential source values.</param>
    /// <returns>The token length.</returns>
    int GetLength(bool allParsedValues = false);
    
    /// <summary>
    /// Gets the string value of the current token, excluding extraneous values.
    /// </summary>
    string GetValue();

    /// <summary>
    /// Gets combined string values of all the source values.
    /// </summary>
    string ToString();

    /// <summary>
    /// Gets all of the source character values.
    /// </summary>
    IEnumerable<char> GetSourceValues();
}
