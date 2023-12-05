using System.Collections;

namespace CdnGetter.Parsing.Version;

public class BuildList : IDelimitedTokenList, IReadOnlyList<IDelimitedToken>
{
    private readonly IDelimitedToken[] _tokens;

    public BuildList() => _tokens = [];

    public BuildList(IEnumerable<IDelimitedToken> tokens)
    {
        _tokens = tokens?.Where(t => t is not null).ToArray() ?? [];
    }

    public IDelimitedToken this[int index] => _tokens[index];

    IToken IReadOnlyList<IToken>.this[int index] => _tokens[index];

    public int Count => _tokens.Length;

    public int CompareTo(IToken? other)
    {
        throw new NotImplementedException();
    }

    public bool Equals(IToken? other)
    {
        throw new NotImplementedException();
    }

    public override bool Equals(object? obj) => Equals(obj as IToken);

    public IEnumerator<IDelimitedToken> GetEnumerator() => _tokens.AsEnumerable().GetEnumerator();

    IEnumerator<IToken> IEnumerable<IToken>.GetEnumerator() => _tokens.Cast<IToken>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _tokens.GetEnumerator();

    public override int GetHashCode() => base.GetHashCode();

    public int GetLength(bool allParsedValues = false)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<char> GetSourceValues()
    {
        throw new NotImplementedException();
    }

    public string GetValue()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        throw new NotImplementedException();
    }
}
