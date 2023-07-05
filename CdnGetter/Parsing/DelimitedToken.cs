using System.Collections;

namespace CdnGetter.Parsing;

public readonly struct DelimitedToken : ITokenList
{
    public IStringToken DelimiterToken { get; }

    public IToken ValueToken { get; }
    
    public DelimitedToken() => (DelimiterToken, ValueToken) = (StringToken.Empty, Digits8Bit.Zero);
    
    public DelimitedToken(IStringToken delimiter, IToken valueToken) =>
        (DelimiterToken, ValueToken) = (delimiter ?? throw new ArgumentNullException(nameof(delimiter)), valueToken ?? throw new ArgumentNullException(nameof(valueToken)));
    
    int IReadOnlyCollection<IToken>.Count => 2;

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
}
