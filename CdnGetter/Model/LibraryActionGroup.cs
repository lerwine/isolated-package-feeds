namespace CdnGetter.Model;

[Obsolete("Do not use")]
public record LibraryActionGroup(LibraryAction Action, string[] LibraryNames)
{
    [Obsolete("Do not use")]
    public static IEnumerable<LibraryActionGroup> FromSettings(Config.AppSettings settings)
    {
        StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;
        IEnumerable<string> toReload;
        
        IEnumerable<string> toReloadExisting = settings.ReloadExistingVersions?.Select(l => l.ToWsNormalizedOrEmptyIfNull()).Where(l => l.Length > 0).Distinct(comparer).ToArray() ?? Enumerable.Empty<string>();
        IEnumerable<string> toGetNew = settings.GetNewVersions?.Select(l => l.ToWsNormalizedOrEmptyIfNull()).Where(l => l.Length > 0).Distinct(comparer).ToArray() ?? Enumerable.Empty<string>();
        if (toGetNew.Any())
        {
            if (toReloadExisting.Any(e => toGetNew.Contains(e, comparer)))
            {
                string[] r = toReloadExisting.Where(e => toGetNew.Contains(e, comparer)).ToArray();
                toGetNew = toGetNew.Where(n => !r.Contains(n, comparer));
                toReloadExisting = toReloadExisting.Where(n => !r.Contains(n, comparer));
                if (settings.ReloadLibrary is null || !(toReload = settings.ReloadLibrary.Select(l => l.ToWsNormalizedOrEmptyIfNull()).Where(l => l.Length > 0)).Any())
                    toReload = r;
                else
                    toReload = toReload.Concat(r).Distinct(comparer);
            }
            else if (settings.ReloadLibrary is null)
                toReload = Enumerable.Empty<string>();
            else if ((toReload = settings.ReloadLibrary.Select(l => l.ToWsNormalizedOrEmptyIfNull()).Where(l => l.Length > 0)).Distinct(comparer).Any())
            {
                toReloadExisting = toReloadExisting.Where(r => !toReload.Contains(r, comparer));
                toGetNew = toGetNew.Where(r => !toReload.Contains(r, comparer));
            }
        }
        else if (settings.ReloadLibrary is null)
            toReload = Enumerable.Empty<string>();
        else if ((toReload = settings.ReloadLibrary.Select(l => l.ToWsNormalizedOrEmptyIfNull()).Where(l => l.Length > 0)).Distinct(comparer).Any())
            toReloadExisting = toReloadExisting.Where(r => !toReload.Contains(r, comparer));
            
        IEnumerable<string> toAdd = settings.AddLibrary?.Select(l => l.ToWsNormalizedOrEmptyIfNull()).Where(l => l.Length > 0).Distinct(comparer) ?? Enumerable.Empty<string>();
        string[] toRemove;
        if (settings.RemoveLibrary is not null && (toRemove = settings.RemoveLibrary.Select(l => l.ToWsNormalizedOrEmptyIfNull()).Where(l => l.Length > 0).Distinct(comparer).ToArray()).Length > 0)
        {
            toAdd = toAdd.Where(a => !toRemove.Contains(a, comparer));
            toReload = toReload.Where(a => !toRemove.Contains(a, comparer));
            toReloadExisting = toReloadExisting.Where(a => !toRemove.Contains(a, comparer));
            toGetNew = toGetNew.Where(a => !toRemove.Contains(a, comparer));
            yield return new(LibraryAction.Remove, toRemove);
        }
        if (toReloadExisting.Any())
            yield return new(LibraryAction.ReloadExistingVersions, (toReloadExisting is string[] arr) ? arr : toReloadExisting.ToArray());
        if (toGetNew.Any())
            yield return new(LibraryAction.GetNewVersions, (toGetNew is string[] arr) ? arr : toGetNew.ToArray());
        if (toReload.Any())
            yield return new(LibraryAction.Reload, toReload.ToArray());
        if (toAdd.Any())
            yield return new(LibraryAction.Add, toAdd.ToArray());
    }

}
