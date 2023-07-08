namespace CdnGetter.Parsing.Version;

public static class Version
{
    internal const char DELIMITER_DOT = '.';

    internal const char DELIMITER_SLASH = '/';
    
    internal const char DELIMITER_COLON = ':';
    
    internal const char DELIMITER_SEMICOLON = ';';
    
    internal const char DELIMITER_PLUS = '+';
    
    internal const char DELIMITER_UNDERSCORE = '_';
    
    internal const char WILDCARD_CHAR = '*';

    public static readonly CharacterToken DotToken = new(DELIMITER_DOT);
    
    public static readonly CharacterToken DashToken = new(Parsing.DELIMITER_DASH);
    
    public static readonly CharacterToken PlusToken = new(DELIMITER_PLUS);
    
    public static readonly CharacterToken WildcardToken = new(WILDCARD_CHAR);
    
    public static ISoftwareVersion? ParseSoftwareVersion(string? s)
    {
        throw new NotImplementedException();
    }

    public static bool IsNamedOrSimpleNumericToken(this INumericalToken? token) =>
        token is not null && (token is Digits8Bit || token is Digits16Bit || token is Digits32Bit || token is Digits64Bit || token is DigitsNBit || token is RomanNumeral || token is DelimitedNumericalToken);

    public static bool IsSimpleStringToken(this IStringToken? token) =>
        token is not null && (token is CharacterToken || token is StringToken || token is DelimitedToken);
}