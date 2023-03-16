namespace CdnGet.Config;

public class CdnJsSettings
{
    public const string PROVIDER_ID = "026ce2f6-c574-4869-bd4c-73bd73dfb640";
    public const string DEFAULT_DisplayName = "cdnjs";
    public const string DEFAULT_BaseUrl = "https://api.cdnjs.com";

    /// <summary>
    /// Specifies the display name for the cdnjs remote service.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets the display name for the cdnjs remote service.
    /// </summary>
    public string GetDisplayName() { return DisplayName.ToWsNormalizedOrDefaultIfEmpty(DEFAULT_DisplayName); }
    
    /// <summary>
    /// Specifies the verbose description for the cdnjs remote service.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Specifies the base url for the cdnjs remote service.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Attempts to contruct the absolute base URL of the cdnjs remote.
    /// </summary>
    /// <param name="result">The constructed URI.</param>
    /// <returns><see langword="true" /> if <paramref name="result" /> contains an absolute URI; otherwise <see langword="true" /> if <paramref name="result" /> contains a relative URI,
    /// indicating that <see cref="BaseUrl" did not contain an absolute URI or a URI could not be constructed.</returns>
    public bool TryGetBaseUrl(out Uri result)
    {
        string? uriString = BaseUrl.ToTrimmedOrNullIfEmpty();
        if (uriString is null)
            result = new Uri(DEFAULT_BaseUrl, UriKind.Absolute);
        else if (!Uri.TryCreate(uriString, UriKind.Absolute, out result!))
        {
            if (!Uri.TryCreate(uriString, UriKind.Relative, out result!))
                result = new Uri("", UriKind.Relative);
            return false;
        }
        return true;
    }
    
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
    /// Gets the database modification actions for libraries.
    /// </summary>
    /// <returns>A tuple whose first value specifies a library name and the second value specifies the action to be performed.</returns>
    public IEnumerable<(string Name, LibraryAction Action)> GetLibraryActions()
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
            foreach (string n in toRemove)
                yield return (n, LibraryAction.Remove);
        }
        foreach (string n in toReloadExisting)
            yield return (n, LibraryAction.ReloadExisting);
        foreach (string n in toGetNew)
            yield return (n, LibraryAction.GetNew);
        foreach (string n in toReload)
            yield return (n, LibraryAction.Reload);
        foreach (string n in toAdd)
            yield return (n, LibraryAction.Add);
    }

}
