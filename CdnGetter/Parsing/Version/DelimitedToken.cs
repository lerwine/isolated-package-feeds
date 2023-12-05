using System.Collections;
using static CdnGetter.Parsing.Version.Version;

namespace CdnGetter.Parsing.Version;

#pragma warning disable CA2231
public readonly struct DelimitedToken : IDelimitedToken, IStringToken
#pragma warning restore CA2231
{
    public IStringToken DelimiterToken { get; }

    public IStringToken ValueToken { get; }

    IToken IDelimitedToken.ValueToken => ValueToken;

    public DelimitedToken() => (DelimiterToken, ValueToken) = (DashToken, WildcardToken);

    public DelimitedToken(CharacterToken delimiter, CharacterToken valueToken) => (DelimiterToken, ValueToken) = (delimiter, valueToken);

    public DelimitedToken(CharacterToken delimiter, StringToken valueToken) => (DelimiterToken, ValueToken) = (delimiter, valueToken);

    public DelimitedToken(StringToken delimiter, CharacterToken valueToken) => (DelimiterToken, ValueToken) = (delimiter, valueToken);

    public DelimitedToken(StringToken delimiter, StringToken valueToken) => (DelimiterToken, ValueToken) = (delimiter, valueToken);

    int IReadOnlyCollection<IToken>.Count => 2;

    int IReadOnlyCollection<char>.Count => 2;

    char IReadOnlyList<char>.this[int index]
    {
        get
        {
            int c = DelimiterToken.Count;
            return (index > c) ? ValueToken[index - c] : DelimiterToken[c];
        }
    }

    IToken IReadOnlyList<IToken>.this[int index] => index switch
    {
        0 => DelimiterToken,
        1 => ValueToken,
        _ => throw new IndexOutOfRangeException()
    };

    public int GetLength(bool allParsedValues) => DelimiterToken.GetLength(allParsedValues) + ValueToken.GetLength(allParsedValues);

    public string GetValue() => DelimiterToken.GetValue() + ValueToken.GetValue();

    public override string ToString() => DelimiterToken.GetValue() + ValueToken.GetValue();

    private IEnumerable<IToken> GetValues()
    {
        yield return DelimiterToken;
        yield return ValueToken;
    }

    IEnumerator<IToken> IEnumerable<IToken>.GetEnumerator() => GetValues().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)GetValues()).GetEnumerator();

    int IComparable<IToken>.CompareTo(IToken? other) => ValueToken.CompareTo(other);

    bool IEquatable<IToken>.Equals(IToken? other) => ValueToken.Equals(other);

    IEnumerable<char> IToken.GetSourceValues() => DelimiterToken.GetSourceValues().Concat(ValueToken.GetSourceValues());

    IEnumerator<char> IEnumerable<char>.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    class Asdf(IEnumerable<IToken> items)
    {
        static Asdf Get(IEnumerable<IStringToken> tokens) => new(tokens);
    }
}
