using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace CdnGetter.Versioning
{
    public readonly struct StringComponent : ITextComponent
    {
        public StringComponent(string text)
        {
            if (text is null || text.Length < 2)
                throw new ArgumentException($"'{nameof(text)}' cannot be null or less than 1 character.", nameof(text));
            Text = text;
        }

        char IReadOnlyList<char>.this[int index] => Text[index];

        public string Text { get; }

        int ITextComponent.Length => Text.Length;

        int IReadOnlyCollection<char>.Count => Text.Length;

        public int CompareTo(ITextComponent? other) => NoCaseComparer.Compare(Text, other);

        public int CompareTo(string? other) => NoCaseComparer.Compare(Text, other);

        public int CompareTo(char other) => NoCaseComparer.Compare(Text, other);

        public int CompareTo(object? obj) => (obj is null) ? 1 : (obj is ITextComponent textComponent) ? NoCaseComparer.Compare(Text, textComponent) :
            (obj is char c) ? NoCaseComparer.Compare(Text, c) : (obj is string other) ? NoCaseComparer.Compare(Text, other) : -1;

        public bool Equals(ITextComponent? other) => NoCaseComparer.Equals(Text, other);

        public bool Equals(string? other) => NoCaseComparer.Equals(Text, other);

        public bool Equals(char other) => NoCaseComparer.Equals(Text, other);

        public override bool Equals([NotNullWhen(true)] object? obj) => obj is not null && ((obj is ITextComponent textComponent) ? NoCaseComparer.Equals(Text, textComponent) :
            (obj is char c) ? NoCaseComparer.Equals(Text, c) : obj is string other && NoCaseComparer.Equals(Text, other));

        IEnumerable<char> ITextComponent.GetChars() => Text;

        IEnumerator<char> IEnumerable<char>.GetEnumerator() => Text.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Text).GetEnumerator();

        public override int GetHashCode() => NoCaseComparer.GetHashCode(Text);

        public override string ToString() => Text;
    }
}