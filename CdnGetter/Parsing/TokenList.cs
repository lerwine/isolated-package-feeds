using System.Collections;
using System.Text;

namespace CdnGetter.Parsing;

/// <summary>
/// A compound token comprised of a sequence of tokens.
/// </summary>
public class TokenList : ITokenList
{
    private readonly IToken[] _items;

    /// <summary>
    /// Intializes a new <c>TokenList</c>.
    /// </summary>
    /// <param name="items">The tokens that will make up this compound token.</param>
    public TokenList(params IToken[] items) => _items = items ?? [];
    
    /// <summary>
    /// Intializes a new <c>TokenList</c>.
    /// </summary>
    /// <param name="items">The tokens that will make up this compound token.</param>
    public TokenList(IEnumerable<IToken> items) => _items = items?.ToArray() ?? [];
    
    /// <summary>
    /// Gets the token at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the token to get.</param>
    public IToken this[int index] => _items[index];

    /// <summary>
    /// Gets the number of tokens in the current token list.
    /// </summary>
    public int Count => _items.Length;

    public int CompareTo(IToken? other)
    {
        if (other is null)
            return 1;
        if (ReferenceEquals(this, other))
            return 0;
        if (other is IEnumerable<IToken> enumerable)
        {
            using IEnumerator<IToken> x = GetEnumerator();
            using IEnumerator<IToken> y = enumerable.GetEnumerator();
            while (x.MoveNext())
            {
                if (!y.MoveNext())
                    return -1;
                if (x.Current is IToken a)
                {
                    if (y.Current is IToken b)
                    {
                        int diff = a.CompareTo(b);
                        if (diff != 0)
                            return diff;
                    }
                    else
                        return 1;
                }
                else if (y.Current is not null)
                    return -1;
            }
            return y.MoveNext() ? -1 : 0;
        }
        using IEnumerator<string> values = _items.AsEnumerable().Select(i => i?.GetValue()!).Where(i => !string.IsNullOrEmpty(i)).GetEnumerator();
        string v = other.GetValue();
        if (!values.MoveNext())
            return string.IsNullOrEmpty(v) ? 0 : -1;
        if (string.IsNullOrEmpty(v))
            return 1;
        string s = values.Current;
        if (s.Length > v.Length || !values.MoveNext())
            return Parsing.NoCaseComparer.Compare(s, v);
        StringBuilder sb = new(s);
        do { sb.Append(values.Current); } while (sb.Length <= v.Length && values.MoveNext());
        return Parsing.NoCaseComparer.Compare(sb.ToString(), v);
    }

    public bool Equals(IToken? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        if (other is IEnumerable<IToken> enumerable)
        {
            using IEnumerator<IToken> x = GetEnumerator();
            using IEnumerator<IToken> y = enumerable.GetEnumerator();
            while (x.MoveNext())
                if (!y.MoveNext() || ((x.Current is IToken a) ? y.Current is not IToken b || !a.Equals(b) : y.Current is not null))
                    return false;
            return !y.MoveNext();
        }
        using IEnumerator<string> values = _items.AsEnumerable().Select(i => i?.GetValue()!).Where(i => !string.IsNullOrEmpty(i)).GetEnumerator();
        string v = other.GetValue();
        if (!values.MoveNext())
            return string.IsNullOrEmpty(v);
        if (string.IsNullOrEmpty(v))
            return false;
        string s = values.Current;
        if (s.Length > v.Length || !values.MoveNext())
            return Parsing.NoCaseComparer.Equals(s, v);
        StringBuilder sb = new(s);
        do { sb.Append(values.Current); } while (sb.Length <= v.Length && values.MoveNext());
        return Parsing.NoCaseComparer.Equals(sb.ToString(), v);
    }

    public IEnumerator<IToken> GetEnumerator() => _items.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

    public int GetLength(bool allChars = false) => _items.AsEnumerable().Sum(i => i?.GetLength() ?? 0);

    public string GetValue()
    {
        using IEnumerator<string> values = _items.AsEnumerable().Select(i => i?.GetValue()!).Where(i => !string.IsNullOrEmpty(i)).GetEnumerator();
        if (!values.MoveNext())
            return string.Empty;
        string s = values.Current;
        if (!values.MoveNext())
            return s;
        StringBuilder sb = new(s);
        do { sb.Append(values.Current); } while (values.MoveNext());
        return sb.ToString();
    }

    public override string ToString()
    {
        using IEnumerator<string> values = _items.AsEnumerable().Select(i => i?.ToString()!).Where(i => !string.IsNullOrEmpty(i)).GetEnumerator();
        if (!values.MoveNext())
            return string.Empty;
        string s = values.Current;
        if (!values.MoveNext())
            return s;
        StringBuilder sb = new(s);
        do { sb.Append(values.Current); } while (values.MoveNext());
        return sb.ToString();
    }

    public IEnumerable<char> GetSourceValues() => _items.SelectMany(i => i.GetSourceValues());
}
