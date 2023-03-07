namespace CdnSync;

public class CdnJsSettings
{
    public const string CONFIG_SECTION_NAME = "cdnjs";
    
    public const string DEFAULT_BaseUrl = "https://api.cdnjs.com";

    public string? BaseUrl { get; set; }

    public const string DEFAULT_ProviderName = "cdnjs";

    public string? ProviderName { get; set; }

    public string[]? AddLibraries { get; set; }

    public string[]? RemoveLibraries { get; set; }
}