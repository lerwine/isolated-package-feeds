namespace CdnGetter.Parsing;

public static class ParsingExtensionMethods
{
    public const ushort ROMAN_NUMERAL_MAX_VALUE = 3999;

    internal const char DELIMITER_DOT = '.';
    internal const char DELIMITER_DASH = '-';
    internal const char DELIMITER_SLASH = '/';
    internal const char DELIMITER_COLON = ':';
    internal const char DELIMITER_SEMICOLON = ';';
    internal const char DELIMITER_PLUS = '+';
    internal const char DELIMITER_UNDERSCORE = '_';
    public const char ZERO = '0';

    public static readonly StringComparer NoCaseComparer = StringComparer.InvariantCultureIgnoreCase;
}
