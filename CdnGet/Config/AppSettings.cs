using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CdnGet.Config;

/// <summary>
/// Top-level section for custom app settings.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Default name of database file.
    /// </summary>
    public const string DEFAULT_DbFile = $"{nameof(CdnGet)}.db";

    /// <summary>
    /// Specifies path of database file.
    /// </summary>
    /// <remarks>This pathis relative to the <see cref="Microsoft.Extensions.Hosting.ContentRootPath" />. The default value of this setting is defined in the <see cref="DEFAULT_DbFile" /> constant.</remarks>
    public string? DbFile  { get; set; }
    
    /// <summary>
    /// Configuration settings for the <see cref="CdnJsRemoteService" />.
    /// </summary>
    public CdnJsSettings? CdnJs { get; set; }

    public static string GetDbFileName(AppSettings? settings) { return (settings?.DbFile).ToTrimmedOrDefaultIfEmpty(DEFAULT_DbFile); }
}
