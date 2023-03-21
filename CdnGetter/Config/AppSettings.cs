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
    /// Specifies path of database file.
    /// </summary>
    /// <remarks>This pathis relative to the <see cref="Microsoft.Extensions.Hosting.ContentRootPath" />. The default value of this setting is defined in the <see cref="DEFAULT_DbFile" /> constant.</remarks>
    public string? DbFile  { get; set; }
    
    /// <summary>
    /// Configuration settings for the <see cref="CdnJsRemoteService" />.
    /// </summary>
    public CdnJsSettings? CdnJs { get; set; }

    /// <summary>
    /// Gets names of libraries to remove from the database.
    /// </summary>
    public List<string>? Remove { get; set; }

    /// <summary>
    /// Gets names of libraries in the database that are to be reloaded from the remote CDN.
    /// </summary>
    public List<string>? Reload { get; set; }

    /// <summary>
    /// Gets names of libraries in the database whose existing library versions are to be reloaded from the remote CDN.
    /// </summary>
    public List<string>? ReloadExistingVersions { get; set; }

    /// <summary>
    /// Gets names of libraries in the database that are to be checked for new versions on the remote CDN.
    /// </summary>
    public List<string>? GetNewVersions { get; set; }

    /// <summary>
    /// Gets names of libraries on the remote CDN to be added to the database.
    /// </summary>
    public List<string>? Add { get; set; }

    /// <summary>
    /// The <see cref="Model.RemoteService.Name" /> of the remote content delivery service.
    /// </summary>
    public string? Remote { get; set; }

    /// <summary>
    /// Displays the names of registered remote content delivery services.
    /// </summary>
    /// <remarks>Use <c>--CdnGetter:ShowRemotes=true</c> from the command line.</remarks>
    public bool? ShowRemotes { get; set; }

    /// <summary>
    /// Display information.
    /// </summary>
    /// <remarks>
    /// --Show=Remotes displays the names of registered remote content delivery services.
    /// --Show=Libraries displays the names of libraries being tracked. Use the --Source switch to display names of libraries for just one remote content delivery service.
    /// </remarks>
    public string? Show { get; set; }

    public static string GetDbFileName(AppSettings? settings) { return (settings?.DbFile).ToTrimmedOrDefaultIfEmpty(DEFAULT_DbFile); }
}
