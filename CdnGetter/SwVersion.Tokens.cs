namespace CdnGetter;

public readonly partial struct SwVersion
{   
    public interface IToken : IComparable<IToken>, IEquatable<IToken>
    {
        long? Value { get; }

        IToken? Previous { get; }

        IToken? Next { get; }

        string ToString();
    }

    public interface INumericToken : IToken
    {
        int LeadingZeroCount { get; }

        new long Value { get; }
        ulong GetMaxValue();
    }

    public sealed record Int32Token(int Value) : INumericToken, IComparable<Int32Token>, IEquatable<Int32Token>
    {
        public int LeadingZeroCount { get; init; }
        
        public IToken? Previous { get; init; }

        public IToken? Next { get; init; }

        long INumericToken.Value => Value;

        long? IToken.Value => Value;

        ulong INumericToken.GetMaxValue() => int.MaxValue;

        public int CompareTo(Int32Token? other) => (other is null) ? 1 : Value - other.Value;

        int IComparable<IToken>.CompareTo(IToken? other)
        {
            if (other is null)
                return 1;
            if (other is Int32Token t)
                return Value - t.Value;
            int result = ToString().CompareTo(other.ToString());
            return (result != 0) ? result : (other is INumericToken n) ? ((ulong)int.MaxValue).CompareTo(n.GetMaxValue()) : -1;
        }

        public bool Equals(Int32Token? other) => other is not null && Value == other.Value;

        bool IEquatable<IToken>.Equals(IToken? other) => Equals(other as Int32Token);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();
    }

    public sealed record Int64Token(long Value) : INumericToken, IComparable<Int64Token>, IEquatable<Int64Token>
    {
        public int LeadingZeroCount { get; init; }

        public IToken? Previous { get; init; }

        public IToken? Next { get; init; }

        long? IToken.Value => throw new NotImplementedException();

        ulong INumericToken.GetMaxValue() => long.MaxValue;

        public int CompareTo(Int64Token? other)
        {
            if (other is null)
                return 1;
            if (other is Int64Token t)
                return Value.CompareTo(t.Value);
            int result = ToString().CompareTo(other.ToString());
            return (result != 0) ? result : (other is INumericToken n) ? ((ulong)long.MaxValue).CompareTo(n.GetMaxValue()) : -1;
        }

        int IComparable<IToken>.CompareTo(IToken? other) => (other is null) ? 1 : (other is Int64Token t) ? Value.CompareTo(t.Value) : ToString().CompareTo(other.ToString());

        public bool Equals(Int64Token? other) => other is not null && Value == other.Value;

        bool IEquatable<IToken>.Equals(IToken? other) => Equals(other as Int64Token);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();
    }

    public sealed record SeparatorToken(char Value) : IToken, IComparable<SeparatorToken>, IEquatable<SeparatorToken>
    {
        public IToken? Previous { get; init; }

        public IToken? Next { get; init; }

        long? IToken.Value => null;

        public int CompareTo(SeparatorToken? other) => (other is null) ? 1 : Value.CompareTo(other.Value);

        int IComparable<IToken>.CompareTo(IToken? other) => (other is null) ? 1 : (other is SeparatorToken t) ? Value.CompareTo(t.Value) : ToString().CompareTo(other.ToString());

        public bool Equals(SeparatorToken? other) => other is not null && Value == other.Value;

        bool IEquatable<IToken>.Equals(IToken? other) => Equals(other as SeparatorToken);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();
    }

    public sealed record AlphaToken(string Value) : IToken, IComparable<AlphaToken>, IEquatable<AlphaToken>
    {
        public IToken? Previous { get; init; }

        public IToken? Next { get; init; }

        long? IToken.Value => null;

        public int CompareTo(AlphaToken? other) => (other is null) ? 1 : TextComparer.Compare(Value, other.Value);

        int IComparable<IToken>.CompareTo(IToken? other) => (other is null) ? 1 : (other is AlphaToken t) ? TextComparer.Compare(Value, t.Value) : ToString().CompareTo(other.ToString());

        public bool Equals(AlphaToken? other) => other is not null && Value == other.Value;

        bool IEquatable<IToken>.Equals(IToken? other) => Equals(other as AlphaToken);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;
    }

    public sealed record OtherToken(string Value) : IToken, IComparable<OtherToken>, IEquatable<OtherToken>
    {
        public IToken? Previous { get; init; }

        public IToken? Next { get; init; }

        long? IToken.Value => null;

        public int CompareTo(OtherToken? other) => (other is null) ? 1 : Value.CompareTo(other.Value);

        int IComparable<IToken>.CompareTo(IToken? other) => (other is null) ? 1 : (other is SeparatorToken t) ? Value.CompareTo(t.Value) : ToString().CompareTo(other.ToString());

        public bool Equals(OtherToken? other) => other is not null && Value == other.Value;

        bool IEquatable<IToken>.Equals(IToken? other) => Equals(other as OtherToken);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;
    }

    private static IEnumerable<IToken> Tokenize(string? versionString)
    {
        if (string.IsNullOrEmpty(versionString))
            return Enumerable.Empty<IToken>();
        int dotIndex = versionString.IndexOf(SEPARATOR_DOT);
        int prIndex = versionString.IndexOf(DELIMITER_PRERELEASE);
        int buildIndex = versionString.IndexOf(DELIMITER_BUILD);
        for (int numberIndex = 0; numberIndex < versionString.Length; numberIndex++)
            if (char.IsNumber(versionString[numberIndex]))
            {
                //                         numberIndex = dotIndex = prIndex = buildIndex - 1;

                // numberIndex;                         dotIndex = prIndex = buildIndex - 1;
                // numberIndex;                         dotIndex = prIndex = buildIndex - 1;
                // numberIndex;                         dotIndex = prIndex = buildIndex - 1;
                // numberIndex;                         dotIndex = prIndex = buildIndex - 1;

                // dotIndex;               numberIndex =            prIndex = buildIndex - 1;
                // dotIndex = numberIndex; numberIndex =            prIndex = buildIndex - 1;
                // dotIndex = prIndex;     numberIndex =            prIndex = buildIndex - 1;
                // dotIndex = buildIndex;  numberIndex =            prIndex = buildIndex - 1;

                // prIndex;     numberIndex = dotIndex =           buildIndex - 1;
                // prIndex;     numberIndex = dotIndex =           buildIndex - 1;
                // prIndex;     numberIndex = dotIndex =           buildIndex - 1;
                // prIndex;     numberIndex = dotIndex =           buildIndex - 1;

                // buildIndex;  numberIndex = dotIndex = prIndex =            - 1;
                // buildIndex;  numberIndex = dotIndex = prIndex =            - 1;
                // buildIndex;  numberIndex = dotIndex = prIndex =            - 1;
                // buildIndex;  numberIndex = dotIndex = prIndex =            - 1;
                break;
            }
        
        throw new NotImplementedException();
    }
}
