using System.Collections;

namespace CdnGetter.Parsing;

/// <summary>
/// Version string to be treated as an alphabetical name.
/// </summary>
public readonly struct NameVersion : ISoftwareVersion
{
    public static readonly NameVersion Empty = new();
    
    IToken IReadOnlyList<IToken>.this[int index] => throw new NotImplementedException();

    public IToken? Prefix => throw new NotImplementedException();

    public IToken Major => throw new NotImplementedException();

    public IToken? Minor => throw new NotImplementedException();

    public IToken? Patch => throw new NotImplementedException();

    public ITokenList Micro => throw new NotImplementedException();

    public ITokenList PreRelease => throw new NotImplementedException();

    public ITokenList Build => throw new NotImplementedException();

    int IReadOnlyCollection<IToken>.Count => throw new NotImplementedException();

    internal static ISoftwareVersion Create(string s)
    {
        throw new NotImplementedException();
    }

    public int CompareTo(IToken? other)
    {
        throw new NotImplementedException();
    }

    public bool Equals(IToken? other)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<IToken> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    int IToken.GetLength(bool allChars)
    {
        throw new NotImplementedException();
    }

    string IToken.GetValue()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<char> GetSourceValues()
    {
        throw new NotImplementedException();
    }
}