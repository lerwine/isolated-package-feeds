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

        public int CompareTo(string? other) => VersionComponentComparer.CompareTo(Text, other);

        public int CompareTo(char other) => VersionComponentComparer.CompareTo(Text, other);

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

        public bool Equals(string? other) => VersionComponentComparer.AreEqual(Text, other);

        public bool Equals(char other) => VersionComponentComparer.AreEqual(Text, other);

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

        char ITextComponent.First() => Text[0];

        public IEnumerator<char> GetEnumerator() => Text.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Text).GetEnumerator();

        char ITextComponent.Last() => Text[^1];

        public override int GetHashCode() => VersionComponentComparer.GetHashCodeOf(Text);

        public override string ToString() => Text;
    }
}