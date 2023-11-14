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

        public string Text { get; }

        int ITextComponent.Length => Text.Length;

        public int CompareTo(ITextComponent? other)
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

        public int CompareTo(object? obj)
        {
            throw new NotImplementedException();
        }

        public bool Equals(ITextComponent? other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(string? other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(char other)
        {
            throw new NotImplementedException();
        }

        IEnumerable<char> ITextComponent.GetChars() => Text;
    }
}