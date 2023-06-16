namespace CdnGetter.Parsing;

public interface IToken : IComparable<IToken>, IEquatable<IToken>
{
    int GetLength(bool allChars = false);
    
    string GetValue();
}
