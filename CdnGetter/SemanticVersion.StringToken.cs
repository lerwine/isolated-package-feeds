
using System.Diagnostics.CodeAnalysis;

namespace CdnGetter;

public partial class SemanticVersion
{
    public readonly struct StringToken : IToken<string>, ICharacterSpanToken, IComparable<StringToken>, IEquatable<StringToken>
    {
        public string Value { get; }

        int ICharacterSpanToken.Length => Value.Length;

        public StringToken(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));
            Value = value;
        }

        public int CompareTo(StringToken other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(ICharacterSpanToken? other)
        {
            throw new NotImplementedException();
        }
        
        public int CompareTo(IToken<string>? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IToken? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(string? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(char other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(StringToken other) => NoCaseComparer.Equals(Value, other.Value);

        public bool Equals([NotNullWhen(true)] ICharacterSpanToken? other)
        {
            if (other is null)
                return false;
            if (other is StringToken st)
                return NoCaseComparer.Equals(Value, st.Value);
            if (other is IToken<string> ts)
                return NoCaseComparer.Equals(Value, ts.ToString());
            return NoCaseComparer.Equals(Value, other.ToString());
        }

        public bool Equals([NotNullWhen(true)] IToken<string>? other)
        {
            if (other is null)
                return false;
            if (other is StringToken st)
                return NoCaseComparer.Equals(Value, st.Value);
            if (other is ICharacterSpanToken cst)
                return NoCaseComparer.Equals(Value, cst.ToString());
            return NoCaseComparer.Equals(Value, other.ToString());
        }

        public bool Equals([NotNullWhen(true)] IToken? other)
        {
            if (other is null)
                return false;
            if (other is StringToken st)
                return NoCaseComparer.Equals(Value, st.Value);
            if (other is ICharacterSpanToken cst)
                return NoCaseComparer.Equals(Value, cst.ToString());
            if (other is IToken<string> ts)
                return NoCaseComparer.Equals(Value, ts.ToString());
            return NoCaseComparer.Equals(Value, other.ToString());
        }

        public bool Equals([NotNullWhen(true)] string? other) => !string.IsNullOrEmpty(other) && NoCaseComparer.Equals(Value, other);

        public bool Equals(char other) => Value.Length == 1 && NoCaseComparer.Equals(Value, other.ToString());

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is null)
                return false;
            if (obj is StringToken st)
                return NoCaseComparer.Equals(Value, st.Value);
            if (obj is ICharacterSpanToken cst)
                return NoCaseComparer.Equals(Value, cst.ToString());
            if (obj is IToken<string> ts)
                return NoCaseComparer.Equals(Value, ts.ToString());
            if (obj is IToken t)
                return NoCaseComparer.Equals(Value, t.ToString());
            if (obj is string s)
                return s.Length > 0 && NoCaseComparer.Equals(Value, s);
            return obj is char c && Value.Length == 1 && NoCaseComparer.Equals(Value, c.ToString());
        }

        IEnumerable<char> IToken.GetCharacters(bool normalized) => Value;

        public override int GetHashCode() => NoCaseComparer.GetHashCode(Value);

        public override string ToString() => Value;
    }
}
