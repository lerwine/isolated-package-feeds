namespace CdnGetter.Versioning
{
    public readonly struct CharacterComponent : ITextComponent
    {
        public CharacterComponent(char suffix)
        {
        }

        public int Length => throw new NotImplementedException();

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
    }
}