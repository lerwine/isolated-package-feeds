using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CdnGetter;

public readonly partial struct SwVersion
{
    public const char SEPARATOR_UNDERSCORE = '_';
    public const char SEPARATOR_DASH = '-';
    public const char SEPARATOR_DOT = '.';
    public const char SEPARATOR_SLASH = '/';
    public const char SEPARATOR_PLUS = '+';
    public static readonly char[] NUMBER_CHARS = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    public static readonly char[] ALPHA_CHARS = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

    private static readonly char[] TOKEN_CHARS = new char[] { SEPARATOR_DOT, '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    private static int ZeroCount(ReadOnlySpan<char> source, int index)
    {
        int end = index;
        while (end < source.Length && source[end] == 0)
            end++;
        return end - index;
    }

    public static IList<IToken> Tokenize(string? versionString)
    {
        if (string.IsNullOrEmpty(versionString))
            return Array.Empty<IToken>();
        ReadOnlySpan<char> source = versionString.AsSpan();
        if (!TryGetNextNonOther(source, 0, out int start, out TokenType type))
            return new IToken[] { new OtherToken(versionString) };
        List<IToken> result;
        int end;
        IToken lastToken;
        switch (type)
        {
            case TokenType.Numeric:
                if (TryGetNextNonNumeric(source, start, out end, out type))
                {
                    result = new();
                    ReadOnlySpan<char> s = source[start..end];
                    if (ulong.TryParse(s, out ulong value))
                    {
                        if (start > 0)
                            result.Add(new OtherToken(source[..start].ToString()));
                        lastToken = NumericTokenFromUInt64(source, start, value);
                    }
                    else
                        lastToken = new OtherToken(source[..end].ToString());
                }
                else
                {
                    if (ulong.TryParse(source[start..], out ulong value))
                    {
                        if (start > 0)
                            return new IToken[] { new OtherToken(source[..start].ToString()), NumericTokenFromUInt64(source, start, value) };
                        return new IToken[] { NumericTokenFromUInt64(source, start, value) };
                    }
                    return new IToken[] { new OtherToken(versionString) };
                }
                break;
            case TokenType.Alpha:
                if (TryGetNextNonAlpha(source, start, out end, out type))
                {
                    result = new();
                    if (start > 0)
                        result.Add(new OtherToken(source[..start].ToString()));
                    lastToken = new AlphaToken(source[start..end].ToString());
                }
                else
                {
                    if (start > 0)
                        return new IToken[] { new OtherToken(source[..start].ToString()), new AlphaToken(source[start..].ToString()) };
                    return new IToken[] { new AlphaToken(source[start..].ToString()) };
                }
                break;
            case TokenType.WhiteSpace:
                if (TryGetNextNonWhiteSpace(source, start, out end, out type))
                {
                    result = new();
                    if (start > 0)
                        result.Add(new OtherToken(source[..start].ToString()));
                    lastToken = new OtherToken(source[start..end].ToString());
                }
                else
                {
                    if (start > 0)
                        return new IToken[] { new OtherToken(source[..start].ToString()), new OtherToken(source[start..].ToString()) };
                    return new IToken[] { new OtherToken(source[start..].ToString()) };
                }
                break;
            default:
                char c = source[start];
                if (TryGetNextNonChar(source, c, start, out end, out type))
                {
                    result = new();
                    if (start > 0)
                        result.Add(new OtherToken(source[..start].ToString()));
                    lastToken = new SeparatorToken(c, end - start);
                }
                else
                {
                    if (start > 0)
                        return new IToken[] { new OtherToken(source[..start].ToString()), new SeparatorToken(c, versionString.Length - start) };
                    return new IToken[] { new SeparatorToken(c, versionString.Length - start) };
                }
                break;
        }
        
        do
        {
            result.Add(lastToken);
            start = end;
            string s;
            switch (type)
            {
                case TokenType.Numeric:
                    if (TryGetNextNonNumeric(source, start, out end, out type))
                    {
                        s = source[start..end].ToString();
                        if (lastToken is SeparatorToken st1)
                        {
                            if (st1.Value == SEPARATOR_DASH)
                            {
                                if (long.TryParse(st1.Value.ToString() + s, out long value))
                                    lastToken = NumericTokenFromInt64(source, start, result, s, st1.Length, value);
                                else
                                    lastToken = new OtherToken(s);
                            }
                            else if (ulong.TryParse(s, out ulong value))
                                lastToken = NumericTokenFromUInt64(source, start, value);
                            else
                                lastToken = new OtherToken(s);
                        }
                        else if (ulong.TryParse(s, out ulong value))
                            lastToken = NumericTokenFromUInt64(source, start, value);
                        else
                            lastToken = new OtherToken(s);
                    }
                    else
                    {
                        end = source.Length;
                        s = source[start..].ToString();
                        if (lastToken is SeparatorToken st2)
                        {
                            if (st2.Value == SEPARATOR_DASH)
                            {
                                if (long.TryParse(st2.Value.ToString() + s, out long value))
                                    lastToken = NumericTokenFromInt64(source, start, result, s, st2.Length, value);
                                else
                                    lastToken = new OtherToken(s);
                            }
                            else if (ulong.TryParse(s, out ulong value))
                                lastToken = NumericTokenFromUInt64(source, start, value);
                            else
                                lastToken = new OtherToken(s);
                        }
                        else if (ulong.TryParse(s, out ulong value))
                                lastToken = NumericTokenFromUInt64(source, start, value);
                        else
                            lastToken = new OtherToken(s);
                    }
                    break;
                case TokenType.Separator:
                    char c = source[start];
                    if (TryGetNextNonChar(source, c, start, out end, out type))
                        lastToken = new SeparatorToken(c, end - start);
                    else
                    {
                        end = source.Length;
                        lastToken = new SeparatorToken(c, source.Length - start);
                    }
                    break;
                case TokenType.Alpha:
                    if (TryGetNextNonAlpha(source, start, out end, out type))
                        lastToken = new AlphaToken(source[start..end].ToString());
                    else
                    {
                        end = source.Length;
                        lastToken = new AlphaToken(source[start..].ToString());
                    }
                    break;
                case TokenType.WhiteSpace:
                    if (TryGetNextNonWhiteSpace(source, start, out end, out type))
                        lastToken = new OtherToken(source[start..end].ToString());
                    else
                    {
                        end = source.Length;
                        lastToken = new OtherToken(source[start..].ToString());
                    }
                    break;
                default:
                    if (TryGetNextNonOther(source, start, out end, out type))
                        lastToken = GetOtherTypeToken(result, lastToken, source[start..end].ToString());
                    else
                    {
                        end = source.Length;
                        lastToken = GetOtherTypeToken(result, lastToken, source[start..].ToString());
                    }
                    break;
            }
        }
        while (end < source.Length);
        result.Add(lastToken);
        return result;
    }

    private static string? GetString(IEnumerable<IToken> tokens)
    {
        using IEnumerator<IToken> enumerator = tokens.GetEnumerator();
        if (enumerator.MoveNext())
        {
            string s = enumerator.Current.ToString();
            if (enumerator.MoveNext())
            {
                StringBuilder sb = new StringBuilder(s).Append(enumerator.Current.ToString());
                while (enumerator.MoveNext())
                    sb.Append(enumerator.Current.ToString());
                return sb.ToString();
            }
            return s;
        }
        return null;
    }

    private static OtherToken GetOtherTypeToken(List<IToken> result, IToken lastToken, string value)
    {
        if (lastToken is OtherToken ot1 && ot1.Type == TokenType.Other)
        {
            result.RemoveAt(result.Count - 1);
            return new OtherToken(ot1.Value + value);
        }
        return new OtherToken(value);
    }

    private static INumericToken NumericTokenFromInt32(int value)
    {
        if (value == 0)
            return new ByteToken(0, 0);
        if (value < ushort.MinValue || value > ushort.MaxValue)
            return new Int32Token(value, 0);
        if (value < 0 || value > byte.MaxValue)
            return new Int16Token((short)value, 0);
        return new ByteToken((byte)value, 0);
    }

    private static INumericToken NumericTokenFromInt64(ReadOnlySpan<char> source, int start, List<IToken> result, string s, int length, long value)
    {
        if (value == 0L)
            return new ByteToken(0, s.Length - 1);
        if (length > 1)
            result[^1] = new SeparatorToken(SEPARATOR_DASH, length - 1);
        else
            result.RemoveAt(result.Count - 1);
        if (value < (long)int.MinValue)
            return new Int64Token(value, ZeroCount(source, start));
        if (value < (long)short.MaxValue)
            return new Int32Token((int)value, ZeroCount(source, start));
        return new Int16Token((short)value, ZeroCount(source, start));
    }

    private static INumericToken NumericTokenFromUInt64(ReadOnlySpan<char> source, int start, ulong value)
    {
        if (value == 0UL)
            return new ByteToken(0, ZeroCount(source, start) - 1);
        if (value > (ulong)long.MaxValue)
            return new UInt64Token(value, ZeroCount(source, start));
        if (value > (ulong)int.MaxValue)
            return new Int64Token((long)value, ZeroCount(source, start));
        if (value > (ulong)short.MaxValue)
            return new Int32Token((int)value, ZeroCount(source, start));
        if (value > (ulong)byte.MaxValue)
            return new Int16Token((short)value, ZeroCount(source, start));
        return new ByteToken((byte)value, ZeroCount(source, start));
    }

    class UInt64Token : IUInt64Token
    {
        public ulong Value { get; }

        public int ZeroPadLength { get; }

        internal UInt64Token(ulong value, int zeroPadLength) => (Value, ZeroPadLength) = (value, zeroPadLength);

        TokenType IToken.Type => TokenType.Numeric;

        public ReadOnlySpan<char> AsSpan() => (ZeroPadLength > 0) ? new(Enumerable.Repeat('0', ZeroPadLength).Concat(Value.ToString()).ToArray()) : Value.ToString().AsSpan();

        public IEnumerable<char> AsEnumerable() => (ZeroPadLength > 0) ? Enumerable.Repeat('0', ZeroPadLength).Concat(Value.ToString()) : Value.ToString();

        public int CompareTo(IToken? other)
        {
            if (other is null)
                return 1;
            if (other is UInt64Token t)
            {
                int result = Value.CompareTo(t.Value);
                return (result == 0) ? ZeroPadLength.CompareTo(t.ZeroPadLength) : result;
            }
            return (other is OtherToken o && o.Type == TokenType.WhiteSpace) ? 1 : ToString().CompareTo(other.ToString());
        }

        public bool Equals(IToken? other) => other is UInt64Token t && Value == t.Value;

        public override bool Equals(object? obj) => Equals(obj as IToken);

        public override string ToString() => (ZeroPadLength > 0) ? new(Enumerable.Repeat('0', ZeroPadLength).Concat(Value.ToString()).ToArray()) : Value.ToString();

        public override int GetHashCode() { unchecked { return (21 + Value.GetHashCode()) * 7 + ZeroPadLength; } }
    }

    class Int64Token : IInt64Token
    {
        public long Value { get; }

        public int ZeroPadLength { get; }

        TokenType IToken.Type => TokenType.Numeric;

        internal Int64Token(long value, int zeroPadLength) => (Value, ZeroPadLength) = (value, zeroPadLength);

        public ReadOnlySpan<char> AsSpan()
        {
            string a = Value.ToString();
            if (ZeroPadLength > 0)
            {
                if (Value < 0)
                    return new(a.Take(1).Concat(Enumerable.Repeat('0', ZeroPadLength)).Concat(a.Skip(1)).ToArray());
                return new(Enumerable.Repeat('0', ZeroPadLength).Concat(a).ToArray());
            }
            return a.AsSpan();
        }

        public IEnumerable<char> AsEnumerable()
        {
            string a = Value.ToString();
            if (ZeroPadLength > 0)
            {
                if (Value < 0)
                    return a.Take(1).Concat(Enumerable.Repeat('0', ZeroPadLength)).Concat(a.Skip(1));
                return Enumerable.Repeat('0', ZeroPadLength).Concat(a);
            }
            return a;
        }

        public int CompareTo(IToken? other)
        {
            if (other is null)
                return 1;
            if (other is UInt64Token t)
            {
                int result = Value.CompareTo(t.Value);
                return (result == 0) ? ZeroPadLength.CompareTo(t.ZeroPadLength) : result;
            }
            return (other is OtherToken o && o.Type == TokenType.WhiteSpace) ? 1 : ToString().CompareTo(other.ToString());
        }

        public bool Equals(IToken? other) => other is Int64Token t && Value == t.Value;

        public override bool Equals(object? obj) => Equals(obj as IToken);

        public override string ToString() => (ZeroPadLength > 0) ? new(Enumerable.Repeat('0', ZeroPadLength).Concat(Value.ToString()).ToArray()) : Value.ToString();

        public override int GetHashCode() { unchecked { return (21 + Value.GetHashCode()) * 7 + ZeroPadLength; } }
    }

    class Int32Token : IInt32Token
    {
        public int Value { get; }

        public int ZeroPadLength { get; }

        TokenType IToken.Type => TokenType.Numeric;

        internal Int32Token(int value, int zeroPadLength) => (Value, ZeroPadLength) = (value, zeroPadLength);

        public ReadOnlySpan<char> AsSpan()
        {
            string a = Value.ToString();
            if (ZeroPadLength > 0)
            {
                if (Value < 0)
                    return new(a.Take(1).Concat(Enumerable.Repeat('0', ZeroPadLength)).Concat(a.Skip(1)).ToArray());
                return new(Enumerable.Repeat('0', ZeroPadLength).Concat(a).ToArray());
            }
            return a.AsSpan();
        }

        public IEnumerable<char> AsEnumerable()
        {
            string a = Value.ToString();
            if (ZeroPadLength > 0)
            {
                if (Value < 0)
                    return a.Take(1).Concat(Enumerable.Repeat('0', ZeroPadLength)).Concat(a.Skip(1));
                return Enumerable.Repeat('0', ZeroPadLength).Concat(a);
            }
            return a;
        }

        public int CompareTo(IToken? other)
        {
            if (other is null)
                return 1;
            if (other is Int32Token t)
            {
                int result = Value.CompareTo(t.Value);
                return (result == 0) ? ZeroPadLength.CompareTo(t.ZeroPadLength) : result;
            }
            return (other is OtherToken o && o.Type == TokenType.WhiteSpace) ? 1 : ToString().CompareTo(other.ToString());
        }

        public bool Equals(IToken? other) => other is Int32Token t && Value == t.Value;

        public override bool Equals(object? obj) => Equals(obj as IToken);

        public override string ToString() => (ZeroPadLength > 0) ? new(Enumerable.Repeat('0', ZeroPadLength).Concat(Value.ToString()).ToArray()) : Value.ToString();

        public override int GetHashCode() { unchecked { return (21 + Value) * 7 + ZeroPadLength; } }
    }

    class Int16Token : IInt16Token
    {
        public short Value { get; }

        public int ZeroPadLength { get; }

        TokenType IToken.Type => TokenType.Numeric;

        internal Int16Token(short value, int zeroPadLength) => (Value, ZeroPadLength) = (value, zeroPadLength);

        public ReadOnlySpan<char> AsSpan()
        {
            string a = Value.ToString();
            if (ZeroPadLength > 0)
            {
                if (Value < 0)
                    return new(a.Take(1).Concat(Enumerable.Repeat('0', ZeroPadLength)).Concat(a.Skip(1)).ToArray());
                return new(Enumerable.Repeat('0', ZeroPadLength).Concat(a).ToArray());
            }
            return a.AsSpan();
        }

        public IEnumerable<char> AsEnumerable()
        {
            string a = Value.ToString();
            if (ZeroPadLength > 0)
            {
                if (Value < 0)
                    return a.Take(1).Concat(Enumerable.Repeat('0', ZeroPadLength)).Concat(a.Skip(1));
                return Enumerable.Repeat('0', ZeroPadLength).Concat(a);
            }
            return a;
        }

        public int CompareTo(IToken? other)
        {
            if (other is null)
                return 1;
            if (other is Int16Token t)
            {
                int result = Value.CompareTo(t.Value);
                return (result == 0) ? ZeroPadLength.CompareTo(t.ZeroPadLength) : result;
            }
            return (other is OtherToken o && o.Type == TokenType.WhiteSpace) ? 1 : ToString().CompareTo(other.ToString());
        }

        public bool Equals(IToken? other) => other is Int16Token t && Value == t.Value;

        public override bool Equals(object? obj) => Equals(obj as IToken);

        public override string ToString() => (ZeroPadLength > 0) ? new(Enumerable.Repeat('0', ZeroPadLength).Concat(Value.ToString()).ToArray()) : Value.ToString();

        public override int GetHashCode() { unchecked { return (21 + Value) * 7 + ZeroPadLength; } }
    }

    class ByteToken : IByteToken
    {
        public byte Value { get; }

        public int ZeroPadLength { get; }

        internal ByteToken(byte value, int zeroPadLength) => (Value, ZeroPadLength) = (value, zeroPadLength);

        TokenType IToken.Type => TokenType.Numeric;

        public ReadOnlySpan<char> AsSpan() => (ZeroPadLength > 0) ? new(Enumerable.Repeat('0', ZeroPadLength).Concat(Value.ToString()).ToArray()) : Value.ToString().AsSpan();

        public IEnumerable<char> AsEnumerable() => (ZeroPadLength > 0) ? Enumerable.Repeat('0', ZeroPadLength).Concat(Value.ToString()) : Value.ToString();

        public int CompareTo(IToken? other)
        {
            if (other is null)
                return 1;
            if (other is ByteToken t)
            {
                int result = Value.CompareTo(t.Value);
                return (result == 0) ? ZeroPadLength.CompareTo(t.ZeroPadLength) : result;
            }
            return (other is OtherToken o && o.Type == TokenType.WhiteSpace) ? 1 : ToString().CompareTo(other.ToString());
        }

        public bool Equals(IToken? other) => other is ByteToken t && Value == t.Value;

        public override bool Equals(object? obj) => Equals(obj as IToken);

        public override string ToString() => (ZeroPadLength > 0) ? new(Enumerable.Repeat('0', ZeroPadLength).Concat(Value.ToString()).ToArray()) : Value.ToString();

        public override int GetHashCode() { unchecked { return (21 + Value) * 7 + ZeroPadLength; } }
    }

    class AlphaToken : IStringToken
    {
        public string Value { get; }

        TokenType IToken.Type => TokenType.Alpha;

        internal AlphaToken(string value) => Value = value;

        public ReadOnlySpan<char> AsSpan() => Value.AsSpan();

        public IEnumerable<char> AsEnumerable() => Value;

        public int CompareTo(IToken? other)
        {
            if (other is null)
                return 1;
            if (other is AlphaToken t)
                return TextComparer.Compare(Value, t.Value);
            return (other is OtherToken o && o.Type == TokenType.WhiteSpace) ? 1 : ToString().CompareTo(other.ToString());
        }

        public bool Equals(IToken? other) => other is AlphaToken t && TextComparer.Equals(Value, t.Value);

        public override bool Equals(object? obj) => Equals(obj as IToken);

        public override string ToString() => Value;

        public override int GetHashCode() => Value.GetHashCode();
    }

    class SeparatorToken : ISeparatorToken
    {
        public char Value { get; }

        public int Length { get; }

        TokenType IToken.Type => TokenType.Separator;

        internal SeparatorToken(char value, int length) => (Value, Length) = (value, length);

        ReadOnlySpan<char> IToken.AsSpan() => new((Length > 1) ? Enumerable.Repeat(Value, Length).ToArray() : new char[] { Value });

        public IEnumerable<char> AsEnumerable() => Enumerable.Repeat(Value, Length);

        public int CompareTo(IToken? other)
        {
            if (other is null)
                return 1;
            if (other is SeparatorToken t)
            {
                int result = Value.CompareTo(t.Value);
                return (result == 0) ? Length.CompareTo(t.Length) : result;
            }
            return (other is OtherToken o && o.Type == TokenType.WhiteSpace) ? 1 : ToString().CompareTo(other.ToString());
        }

        public bool Equals(IToken? other) => other is SeparatorToken t && Value == t.Value && Length == t.Length;

        public override bool Equals(object? obj) => Equals(obj as IToken);

        public override string ToString() => (Length > 1) ? new string(Value, Length) : Value.ToString();

        public override int GetHashCode() { unchecked { return (21 + Value.GetHashCode()) * 7 + Length; } }

        internal static readonly SeparatorToken Dot = new(SEPARATOR_DOT, 1);

        internal static readonly SeparatorToken Dash = new(SEPARATOR_DASH, 1);

        internal static readonly SeparatorToken Plus = new(SEPARATOR_PLUS, 1);
    }

    class OtherToken : IStringToken
    {
        public string Value { get; }

        public TokenType Type { get; }

        private OtherToken() => (Value, Type) = (string.Empty, TokenType.WhiteSpace);

        internal OtherToken(string value) => (Value, Type) = (value, char.IsWhiteSpace(value[0]) ? TokenType.WhiteSpace : TokenType.Other);

        public ReadOnlySpan<char> AsSpan() => Value.AsSpan();

        public IEnumerable<char> AsEnumerable() => Value;

        public int CompareTo(IToken? other)
        {
            if (other is null)
                return 1;
            if (other is OtherToken t)
                return (Type == TokenType.WhiteSpace) ? Value.Length.CompareTo(t.Value.Length) : Value.CompareTo(t.Value);
            return (Type == TokenType.WhiteSpace) ? -1 : Value.CompareTo(other.ToString());
        }

        public bool Equals(IToken? other) => other is OtherToken t && ((Type == TokenType.WhiteSpace) ? Value.Length == t.Value.Length : Value == t.Value);

        public override bool Equals(object? obj) => Equals(obj as IToken);

        public override string ToString() => Value;

        public override int GetHashCode() { unchecked { return (21 + Value.GetHashCode()) * 7 + Type.GetHashCode(); } }

        internal static readonly OtherToken Empty = new();
    }

    private static bool TryGetNextNonChar(ReadOnlySpan<char> source, char value, int index, out int nextIndex, out TokenType type)
    {
        while (index < source.Length)
        {
            char c = source[index];
            if (c != value)
            {
                type = c switch
                {
                    SEPARATOR_UNDERSCORE or SEPARATOR_DASH or SEPARATOR_DOT or SEPARATOR_SLASH or SEPARATOR_PLUS => TokenType.Separator,
                    _ => char.IsNumber(c) ? TokenType.Numeric : char.IsLetter(c) ? TokenType.Alpha : char.IsWhiteSpace(c) ? TokenType.WhiteSpace : TokenType.Other,
                };
                nextIndex = index;
                return true;
            }
        }
        nextIndex = index;
        type = TokenType.Other;
        return false;
    }
    
    private static bool TryGetNextNonNumeric(ReadOnlySpan<char> source, int index, out int nextIndex, out TokenType type)
    {
        while (index < source.Length)
        {
            char c = source[index];
            if (!char.IsNumber(c))
            {
                type = c switch
                {
                    SEPARATOR_UNDERSCORE or SEPARATOR_DASH or SEPARATOR_DOT or SEPARATOR_SLASH or SEPARATOR_PLUS => TokenType.Separator,
                    _ => char.IsNumber(c) ? TokenType.Numeric : char.IsLetter(c) ? TokenType.Alpha : char.IsWhiteSpace(c) ? TokenType.WhiteSpace : TokenType.Other,
                };
                nextIndex = index;
                return true;
            }
        }
        nextIndex = index;
        type = TokenType.Other;
        return false;
    }
    
    private static bool TryGetNextNonAlpha(ReadOnlySpan<char> source, int index, out int nextIndex, out TokenType type)
    {
        while (index < source.Length)
        {
            char c = source[index];
            if (!char.IsLetter(c))
            {
                type = c switch
                {
                    SEPARATOR_UNDERSCORE or SEPARATOR_DASH or SEPARATOR_DOT or SEPARATOR_SLASH or SEPARATOR_PLUS => TokenType.Separator,
                    _ => char.IsNumber(c) ? TokenType.Numeric : char.IsLetter(c) ? TokenType.Alpha : char.IsWhiteSpace(c) ? TokenType.WhiteSpace : TokenType.Other,
                };
                nextIndex = index;
                return true;
            }
        }
        nextIndex = index;
        type = TokenType.Other;
        return false;
    }
    
    private static bool TryGetNextNonWhiteSpace(ReadOnlySpan<char> source, int index, out int nextIndex, out TokenType type)
    {
        while (index < source.Length)
        {
            char c = source[index];
            if (!char.IsWhiteSpace(c))
            {
                type = c switch
                {
                    SEPARATOR_UNDERSCORE or SEPARATOR_DASH or SEPARATOR_DOT or SEPARATOR_SLASH or SEPARATOR_PLUS => TokenType.Separator,
                    _ => char.IsNumber(c) ? TokenType.Numeric : char.IsLetter(c) ? TokenType.Alpha : char.IsWhiteSpace(c) ? TokenType.WhiteSpace : TokenType.Other,
                };
                nextIndex = index;
                return true;
            }
        }
        nextIndex = index;
        type = TokenType.Other;
        return false;
    }
    
    private static bool TryGetNextNonOther(ReadOnlySpan<char> source, int index, out int nextIndex, out TokenType type)
    {
        nextIndex = index;
        while (nextIndex < source.Length)
        {
            char c = source[nextIndex];
            if (!char.IsWhiteSpace(c))
            {
                switch (c)
                {
                    case SEPARATOR_UNDERSCORE:
                    case SEPARATOR_DASH:
                    case SEPARATOR_DOT:
                    case SEPARATOR_SLASH:
                    case SEPARATOR_PLUS:
                        type = TokenType.Separator;
                        return true;
                    default:
                        if (char.IsNumber(c))
                        {
                            type = TokenType.Numeric;
                            return true;
                        }
                        if (char.IsLetter(c))
                        {
                            type = TokenType.Alpha;
                            return true;
                        }
                        if (char.IsWhiteSpace(c))
                        {
                            type = TokenType.WhiteSpace;
                            return true;
                        }
                        break;
                }
                type = c switch
                {
                    SEPARATOR_UNDERSCORE or SEPARATOR_DASH or SEPARATOR_DOT or SEPARATOR_SLASH or SEPARATOR_PLUS => TokenType.Separator,
                    _ => char.IsNumber(c) ? TokenType.Numeric : char.IsLetter(c) ? TokenType.Alpha : char.IsWhiteSpace(c) ? TokenType.WhiteSpace : TokenType.Other,
                };
                nextIndex = index;
                return true;
            }
        }
        nextIndex = index;
        type = TokenType.Other;
        return false;
    }
    
    public enum TokenType
    {
        Separator,
        Numeric,
        Alpha,
        WhiteSpace,
        Other
    }

    public interface IToken : IComparable<IToken>, IEquatable<IToken>
    {
        TokenType Type { get; }

        ReadOnlySpan<char> AsSpan();

        string ToString();

        IEnumerable<char> AsEnumerable();
    }

    public interface IDelimitedToken<T> : IEquatable<IDelimitedToken<T>>, IComparable<IDelimitedToken<T>>
        where T : IToken
    {
        ISeparatorToken? Delimiter { get; }
        
        T Value { get; }

        bool Equals(IDelimitedToken<T>? other, ISeparatorToken defaultDelimiter);

        int CompareTo(IDelimitedToken<T>? other, ISeparatorToken defaultDelimiter);
    }

    public class DelimitedToken<T> : IDelimitedToken<T> where T : IToken
    {
        public T Value { get; }

        public ISeparatorToken Delimiter { get; }

        public DelimitedToken(ISeparatorToken delimiter, T value) => (Delimiter, Value) = (delimiter, value);

        public bool Equals([NotNullWhen(true)] IDelimitedToken<T>? other) => other is not null && (ReferenceEquals(this, other) || Delimiter.Equals(other.Delimiter) && Value.Equals(other.Value));

        public bool Equals(IDelimitedToken<T>? other, ISeparatorToken defaultDelimiter)
        {
            throw new NotImplementedException();
        }

        public override bool Equals([NotNullWhen(true)] object? obj) => Equals(obj as IDelimitedToken<T>);

        public int CompareTo(IDelimitedToken<T>? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IDelimitedToken<T>? other, ISeparatorToken defaultDelimiter)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode() { unchecked { return (21 + Value.GetHashCode()) * 7 + Delimiter.GetHashCode(); } }
    }

    public class DefaultDelimitedToken<T> : IDelimitedToken<T> where T : IToken
    {
        public T Value { get; }

        ISeparatorToken? IDelimitedToken<T>.Delimiter => null;

        public DefaultDelimitedToken(T value) => Value = value;

        public bool Equals([NotNullWhen(true)] IDelimitedToken<T>? other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals([NotNullWhen(true)] object? obj) => Equals(obj as IDelimitedToken<T>);

        public bool Equals(IDelimitedToken<T>? other, ISeparatorToken defaultDelimiter)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IDelimitedToken<T>? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IDelimitedToken<T>? other, ISeparatorToken defaultDelimiter)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode() => Value.GetHashCode();
    }

    public interface IDelimitedTokenList<T> : IReadOnlyCollection<T>, IEquatable<IDelimitedTokenList<T>>, IComparable<IDelimitedTokenList<T>>
        where T : IToken
    {
        ISeparatorToken? Delimiter { get; }

        bool Equals(IDelimitedTokenList<T>? other, ISeparatorToken defaultLeadDelimiter);

        int CompareTo(IDelimitedTokenList<T>? other, ISeparatorToken defaultDelimiter);
    }

    public class DelimitedTokenList<T> : ReadOnlyCollection<T>, IDelimitedTokenList<T>
        where T : IToken
    {
        public ISeparatorToken Delimiter { get; }

        public DelimitedTokenList(ISeparatorToken delimiter, IList<T> list) : base(list) => Delimiter = delimiter;

        public bool Equals([NotNullWhen(true)] IDelimitedTokenList<T>? other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IDelimitedTokenList<T>? other, ISeparatorToken defaultLeadDelimiter)
        {
            throw new NotImplementedException();
        }

        public override bool Equals([NotNullWhen(true)] object? obj) => Equals(obj as IDelimitedTokenList<T>);

        public int CompareTo(IDelimitedTokenList<T>? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IDelimitedTokenList<T>? other, ISeparatorToken defaultDelimiter)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 21 + Delimiter.GetHashCode();
                int h = 3;
                foreach (T token in this)
                    h = h * 7 + token.GetHashCode();
                return hash * 7 + h;
            }
        }
    }

    public class DefaultDelimitedTokenList<T> : ReadOnlyCollection<T>, IDelimitedTokenList<T>
        where T : IToken
    {
        public DefaultDelimitedTokenList(IList<T> list) : base(list) { }

        ISeparatorToken? IDelimitedTokenList<T>.Delimiter => null;

        public int CompareTo(IDelimitedTokenList<T>? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IDelimitedTokenList<T>? other, ISeparatorToken defaultDelimiter)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IDelimitedTokenList<T>? other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IDelimitedTokenList<T>? other, ISeparatorToken defaultLeadDelimiter)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object? obj) => Equals(obj as IDelimitedTokenList<T>);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 3;
                foreach (T token in this)
                    hash = hash * 7 + token.GetHashCode();
                return hash;
            }
        }
    }

    public interface INumericToken : IToken
    {
        int ZeroPadLength { get; }
    }

    public interface IByteToken : INumericToken
    {
        byte Value { get; }
    }
    
    public interface IInt16Token : INumericToken
    {
        short Value { get; }
    }
    
    public interface IInt32Token : INumericToken
    {
        int Value { get; }
    }
    
    public interface IInt64Token : INumericToken
    {
        long Value { get; }
    }
        
    public interface IUInt64Token : INumericToken
    {
        ulong Value { get; }
    }
    
    public interface IStringToken : IToken
    {
        string Value { get; }
    }

    public interface ISeparatorToken : IToken
    {
        int Length { get; }
        char Value { get; }
    }
}
