namespace CdnGetter.Parsing;

/// <summary>
/// Represents a token parsed from character values.
/// </summary>
public interface IStringToken : IToken, IReadOnlyList<char> { }
