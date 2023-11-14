namespace CdnGetter.Versioning
{
    public static class TextComponent
    {
        public static ITextComponent? AsTextComponent(string? value) => string.IsNullOrEmpty(value) ? null : (value.Length > 1) ? new StringComponent(value) : new CharacterComponent(value[0]);
    }
}