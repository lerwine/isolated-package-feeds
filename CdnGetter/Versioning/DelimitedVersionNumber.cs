namespace CdnGetter.Versioning
{
    public readonly struct DelimitedVersionNumber : IDelimitedVersionComponent
    {
        public ITextComponent Delimiter { get; }

        public IVersionNumber Value { get; }

        IVersionComponent IDelimitedVersionComponent.Value => Value;

        public DelimitedVersionNumber(ITextComponent delimiter, BigVersionNumber value)
        {
            if (delimiter.Length == 0 || char.IsNumber(delimiter.Last()))
                throw new ArgumentOutOfRangeException(nameof(delimiter));
            Delimiter = delimiter;
            Value = value;
        }

        public DelimitedVersionNumber(ITextComponent delimiter, LongVersionNumber value)
        {
            if (delimiter.Length == 0 || char.IsNumber(delimiter.Last()))
                throw new ArgumentOutOfRangeException(nameof(delimiter));
            Delimiter = delimiter;
            Value = value;
        }

        public DelimitedVersionNumber(ITextComponent delimiter, VersionNumber value)
        {
            if (delimiter.Length == 0 || char.IsNumber(delimiter.Last()))
                throw new ArgumentOutOfRangeException(nameof(delimiter));
            Delimiter = delimiter;
            Value = value;
        }

        public DelimitedVersionNumber(ITextComponent delimiter, ShortVersionNumber value)
        {
            if (delimiter.Length == 0 || char.IsNumber(delimiter.Last()))
                throw new ArgumentOutOfRangeException(nameof(delimiter));
            Delimiter = delimiter;
            Value = value;
        }

        public DelimitedVersionNumber(ITextComponent delimiter, TinyVersionNumber value)
        {
            if (delimiter.Length == 0 || char.IsNumber(delimiter.Last()))
                throw new ArgumentOutOfRangeException(nameof(delimiter));
            Delimiter = delimiter;
            Value = value;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}