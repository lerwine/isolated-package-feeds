namespace CdnGetter.Parsing;

public static class Parsing
{
    public const ushort ROMAN_NUMERAL_MAX_VALUE = 3999;

    internal const char DELIMITER_DASH = '-';
    
    public const char ZERO = '0';

    public static readonly StringComparer NoCaseComparer = StringComparer.InvariantCultureIgnoreCase;
}
