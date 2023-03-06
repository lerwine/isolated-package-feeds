namespace CdnSync;

public class CdnSyncAppSettings
{
    public const string DEFAULT_DbFile = $"{nameof(CdnSync)}.db";

    public string? DbFile { get; set; }

    public const string DEFAULT_CdnJsBaseUrl = "https://api.cdnjs.com";

    public string? CdnJsBaseUrl { get; set; }

}