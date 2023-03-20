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
    /// <remarks>Use <c>--CdnGet:ShowRemotes=true</c> from the command line.</remarks>
    public bool? ShowRemotes { get; set; }

    /// <summary>
    /// Display information.
    /// </summary>
    /// <remarks>
    /// --Show=Remotes displays the names of registered remote content delivery services.
    /// --Show=Libraries displays the names of libraries being tracked. Use the --Source switch to display names of libraries for just one remote content delivery service.
    /// </remarks>
    public string? Show { get; set; }

    public IEnumerable<(LibraryAction Action, string[] Names)> GetLibraryActions()
    {
        StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;
        IEnumerable<string> toReload;
        
        IEnumerable<string> toReloadExisting = ReloadExistingVersions?.Select(l => l.ToWsNormalizedOrEmptyIfNull()).Where(l => l.Length > 0).Distinct(comparer).ToArray() ?? Enumerable.Empty<string>();
        IEnumerable<string> toGetNew = GetNewVersions?.Select(l => l.ToWsNormalizedOrEmptyIfNull()).Where(l => l.Length > 0).Distinct(comparer).ToArray() ?? Enumerable.Empty<string>();
        if (toGetNew.Any())
        {
            if (toReloadExisting.Any(e => toGetNew.Contains(e, comparer)))
            {
                string[] r = toReloadExisting.Where(e => toGetNew.Contains(e, comparer)).ToArray();
                toGetNew = toGetNew.Where(n => !r.Contains(n, comparer));
                toReloadExisting = toReloadExisting.Where(n => !r.Contains(n, comparer));
                if (Reload is null || !(toReload = Reload.Select(l => l.ToWsNormalizedOrEmptyIfNull()).Where(l => l.Length > 0)).Any())
                    toReload = r;
                else
                    toReload = toReload.Concat(r).Distinct(comparer);
            }
            else if (Reload is null)
                toReload = Enumerable.Empty<string>();
            else if ((toReload = Reload.Select(l => l.ToWsNormalizedOrEmptyIfNull()).Where(l => l.Length > 0)).Distinct(comparer).Any())
            {
                toReloadExisting = toReloadExisting.Where(r => !toReload.Contains(r, comparer));
                toGetNew = toGetNew.Where(r => !toReload.Contains(r, comparer));
            }
        }
        else if (Reload is null)
            toReload = Enumerable.Empty<string>();
        else if ((toReload = Reload.Select(l => l.ToWsNormalizedOrEmptyIfNull()).Where(l => l.Length > 0)).Distinct(comparer).Any())
            toReloadExisting = toReloadExisting.Where(r => !toReload.Contains(r, comparer));
            
        IEnumerable<string> toAdd = Add?.Select(l => l.ToWsNormalizedOrEmptyIfNull()).Where(l => l.Length > 0).Distinct(comparer) ?? Enumerable.Empty<string>();
        string[] toRemove;
        if (Remove is not null && (toRemove = Remove.Select(l => l.ToWsNormalizedOrEmptyIfNull()).Where(l => l.Length > 0).Distinct(comparer).ToArray()).Length > 0)
        {
            toAdd = toAdd.Where(a => !toRemove.Contains(a, comparer));
            toReload = toReload.Where(a => !toRemove.Contains(a, comparer));
            toReloadExisting = toReloadExisting.Where(a => !toRemove.Contains(a, comparer));
            toGetNew = toGetNew.Where(a => !toRemove.Contains(a, comparer));
            yield return (LibraryAction.Remove, toRemove);
        }
        if (toReloadExisting.Any())
            yield return (LibraryAction.ReloadExisting, (toReloadExisting is string[] arr) ? arr : toReloadExisting.ToArray());
        if (toGetNew.Any())
            yield return (LibraryAction.GetNew, (toGetNew is string[] arr) ? arr : toGetNew.ToArray());
        if (toReload.Any())
            yield return (LibraryAction.Reload, toReload.ToArray());
        if (toAdd.Any())
            yield return (LibraryAction.Add, toAdd.ToArray());
    }

    public static string GetDbFileName(AppSettings? settings) { return (settings?.DbFile).ToTrimmedOrDefaultIfEmpty(DEFAULT_DbFile); }
}
