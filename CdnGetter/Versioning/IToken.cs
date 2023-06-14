using System.Diagnostics.CodeAnalysis;

namespace CdnGetter.Versioning;

public interface IToken : IComparable<IToken>, IEquatable<IToken>
{
    ITokenCharacters? GetDelimiter();

    bool TryGetDelimiter([NotNullWhen(true)] out string? delimiter);
    
    string GetValue();

    string? GetPostFixed();

    bool TryGetPostFixed([NotNullWhen(true)] out string? postfixed);
}
