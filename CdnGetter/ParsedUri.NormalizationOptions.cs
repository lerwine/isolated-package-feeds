namespace CdnGetter;

public partial class ParsedUri
{
    public enum NormalizationOptions : byte
    {
        None                        = 0b0000_0000,
        StripEmptyPathSegments      = 0b0000_0001,
        NormalizePathSeparators     = 0b0000_0010,
        StripTrailingPathSeparator  = 0b0000_0100,
        EnsureTrailingPathSeparator = 0b0000_1000,
        NormalizeDotPathSegments    = 0b0001_0000,
        StripEmptyQuery             = 0b0010_0000,
        StripEmptyFragment          = 0b0100_0000,
        StripDefaultPort            = 0b1000_0000
    }
}
