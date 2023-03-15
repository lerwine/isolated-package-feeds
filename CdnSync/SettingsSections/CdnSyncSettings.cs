namespace CdnSync.SettingsSections;

public class CdnSyncSettings
{
    public const string DEFAULT_DbFile = $"{nameof(CdnSync)}.db";

    public string? dbFile;

    public CdnJsSettings? cdnJs;

    public static string GetDbFile(CdnSyncSettings? settings) { return (settings?.dbFile).ToTrimmedOrDefaultIfEmpty(DEFAULT_DbFile); }
}