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

        public int CompareTo(ITextComponent? other) => NoCaseComparer.Compare(Value, other);

        public int CompareTo(string? other) => NoCaseComparer.Compare(Value, other);

        public int CompareTo(char other) => NoCaseComparer.Compare(Value, other);

        public int CompareTo(object? obj) => (obj is null) ? 1 : (obj is ITextComponent textComponent) ? NoCaseComparer.Compare(Value, textComponent) :
            (obj is char c) ? NoCaseComparer.Compare(Value, c) : (obj is string other) ? NoCaseComparer.Compare(Value, other) : -1;

        public bool Equals(ITextComponent? other) => NoCaseComparer.Equals(Value, other);

        public bool Equals(string? other) => NoCaseComparer.Equals(Value, other);

        public bool Equals(char other) => NoCaseComparer.Equals(Value, other);

        public override bool Equals([NotNullWhen(true)] object? obj) => obj is not null && ((obj is ITextComponent textComponent) ? NoCaseComparer.Equals(Value, textComponent) :
            (obj is char c) ? NoCaseComparer.Equals(Value, c) : obj is string other && NoCaseComparer.Equals(Value, other));

        public IEnumerable<char> GetChars()
        {
            yield return Value;
        }

        public override int GetHashCode() => NoCaseComparer.GetHashCode(Value);

        public override string ToString() => Value.ToString();

        IEnumerator<char> IEnumerable<char>.GetEnumerator() => GetChars().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)GetChars()).GetEnumerator();
    }
}