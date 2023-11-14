
using System.Diagnostics.CodeAnalysis;

namespace CdnGetter;

public partial class SemanticVersion
{
    public readonly struct CharacterToken : IToken<char>, ICharacterSpanToken, IComparable<CharacterToken>, IEquatable<CharacterToken>
    {
        public CharacterToken(char value) => Value = value;
        
        public char Value { get; }

        int ICharacterSpanToken.Length => 1;

        public int CompareTo(CharacterToken other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(ICharacterSpanToken? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IToken<char>? other)
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

        public bool Equals(CharacterToken other) => VersionComponentComparer.AreEqual(Value.ToString(), other.Value.ToString());

        public bool Equals([NotNullWhen(true)] ICharacterSpanToken? other)
        {
            if (other is null)
                return false;
            if (other is CharacterToken ct)
                return VersionComponentComparer.AreEqual(ct.Value.ToString(), Value.ToString());
            if (other is IToken<char> ts)
                return VersionComponentComparer.AreEqual(ts.Value.ToString(), Value.ToString());
            return other.Length == 1 && VersionComponentComparer.AreEqual(other.GetCharacters().First().ToString(), Value.ToString());
        }

        public bool Equals([NotNullWhen(true)] IToken<char>? other)
        {
            if (other is null)
                return false;
            if (other is CharacterToken ct)
                return VersionComponentComparer.AreEqual(ct.Value.ToString(), Value.ToString());
            if (other is ICharacterSpanToken cst)
                return cst.Length == 1 && VersionComponentComparer.AreEqual(cst.GetCharacters().First().ToString(), Value.ToString());
            return VersionComponentComparer.AreEqual(other.Value.ToString(), Value.ToString());
        }

        public bool Equals([NotNullWhen(true)] IToken? other)
        {
            if (other is null)
                return false;
            if (other is CharacterToken ct)
                return VersionComponentComparer.AreEqual(ct.Value.ToString(), Value.ToString());
            if (other is ICharacterSpanToken cst)
                return cst.Length == 1 && VersionComponentComparer.AreEqual(cst.GetCharacters().First().ToString(), Value.ToString());
            if (other is IToken<char> ts)
                return VersionComponentComparer.AreEqual(ts.Value.ToString(), Value.ToString());
            return VersionComponentComparer.AreEqual(other.ToString(), Value.ToString());
        }

        public bool Equals([NotNullWhen(true)] string? other) => other is not null && other.Length == 1 && VersionComponentComparer.AreEqual(Value.ToString(), other);

        public bool Equals(char other) => VersionComponentComparer.AreEqual(Value.ToString(), other.ToString());

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is null)
                return false;
            if (obj is CharacterToken ct)
                return VersionComponentComparer.AreEqual(ct.Value.ToString(), Value.ToString());
            if (obj is ICharacterSpanToken cst)
                return cst.Length == 1 && VersionComponentComparer.AreEqual(cst.GetCharacters().First().ToString(), Value.ToString());
            if (obj is IToken<char> ts)
                return VersionComponentComparer.AreEqual(ts.Value.ToString(), Value.ToString());
            if (obj is IToken t)
                return VersionComponentComparer.AreEqual(t.ToString(), Value.ToString());
            if (obj is char c)
                return Value == c;
            return obj is string s && s.Length == 1 && VersionComponentComparer.AreEqual(Value.ToString(), s);
        }

        public IEnumerable<char> GetCharacters(bool normalized = false) { yield return Value; }

        public override int GetHashCode() => VersionComponentComparer.GetHashCodeOf(Value.ToString());

        public override string ToString() => Value.ToString();
    }
}
