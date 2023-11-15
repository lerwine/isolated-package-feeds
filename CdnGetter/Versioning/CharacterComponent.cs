using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace CdnGetter.Versioning
{
    public readonly struct CharacterComponent : ITextComponent
    {
        public CharacterComponent(char value) => Value = value;

        public char Value { get; }

        int ITextComponent.Length => 1;

        int IReadOnlyCollection<char>.Count => 1;

        char IReadOnlyList<char>.this[int index]
        {
            get
            {
                if (index != 0)
                    throw new IndexOutOfRangeException();
                return Value;
            }
        }

        public int CompareTo(string? other) => VersionComponentComparer.CompareTo(Value, other);

        public int CompareTo(char other) => VersionComponentComparer.CompareTo(Value, other);

        public int CompareTo(ITextComponent? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IVersionComponent? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object? obj)
        {
            throw new NotImplementedException();
        }

        public bool Equals(string? other) => VersionComponentComparer.AreEqual(Value, other);

        public bool Equals(char other) => VersionComponentComparer.AreEqual(Value, other);

        public bool Equals(ITextComponent? other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IVersionComponent? other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            throw new NotImplementedException();
        }

        char ITextComponent.First() => Value;

        public IEnumerator<char> GetEnumerator()
        {
            yield return Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return Value;
        }

        char ITextComponent.Last() => Value;

        public override int GetHashCode() => VersionComponentComparer.GetHashCodeOf(Value);

        public override string ToString() => Value.ToString();
    }
}