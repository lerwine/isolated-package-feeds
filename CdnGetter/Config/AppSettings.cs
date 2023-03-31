namespace CdnGetter.Config;

/// <summary>
/// Top-level section for custom app settings.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Default name of database file.
    /// </summary>
    public const string DEFAULT_DbFile = $"{nameof(CdnGetter)}.db";

    /// <summary>
    /// Default name of database file.
    /// </summary>
    public const string DEFAULT_LocalStoragePath = "Content";

    /// <summary>
    /// Specifies the relative or absolute path of the database file.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the <see cref="Microsoft.Extensions.Hosting.ContentRootPath" />. The default value of this setting is defined in the <see cref="DEFAULT_DbFile" /> constant.</remarks>
    public string? DbFile  { get; set; }
    
    /// <summary>
    /// Specifies the path for local storage of CDN content files.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the <see cref="Microsoft.Extensions.Hosting.ContentRootPath" />. The default value of this setting is defined in the <see cref="DEFAULT_LocalStoragePath" /> constant.</remarks>
    public string? LocalStoragePath  { get; set; }
    
    /// <summary>
    /// Configuration settings for the <see cref="CdnJsUpstreamCdn" />.
    /// </summary>
    public CdnJsSettings? CdnJs { get; set; }

    public static string GetDbFileName(AppSettings? settings) { return (settings?.DbFile).ToTrimmedOrDefaultIfEmpty(DEFAULT_DbFile); }

    public static string GetLocalStoragePath(AppSettings? settings) { return (settings?.LocalStoragePath).ToTrimmedOrDefaultIfEmpty(DEFAULT_LocalStoragePath); }
}
