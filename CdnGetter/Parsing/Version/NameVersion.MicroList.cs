using System.Collections;

namespace CdnGetter.Parsing.Version;

public readonly partial struct NameVersion
{
    class MicroList : IDelimitedTokenList, IReadOnlyList<DelimitedToken>
    {
        private readonly DelimitedToken[] _tokens;

        public DelimitedToken this[int index] => _tokens[index];

        IDelimitedToken IDelimitedTokenList.this[int index] => _tokens[index];

        IToken IReadOnlyList<IToken>.this[int index] => _tokens[index];

        public int Count => _tokens.Length;

        public MicroList() => _tokens = Array.Empty<DelimitedToken>();

        public MicroList(IEnumerable<DelimitedToken> tokens) => _tokens = tokens?.ToArray() ?? Array.Empty<DelimitedToken>();

        public int CompareTo(IToken? other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IToken? other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object? obj) => Equals(obj as IToken);

        public IEnumerator<DelimitedToken> GetEnumerator() => _tokens.AsEnumerable().GetEnumerator();

        IEnumerator<IDelimitedToken> IEnumerable<IDelimitedToken>.GetEnumerator() => _tokens.Cast<IDelimitedToken>().GetEnumerator();

        IEnumerator<IToken> IEnumerable<IToken>.GetEnumerator() => _tokens.Cast<IToken>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _tokens.GetEnumerator();

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

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
}