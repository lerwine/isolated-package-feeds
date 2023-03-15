namespace CdnSync.SettingsSections;

public class CdnJsSettings
{
    public const string DEFAULT_BaseUrl = "https://api.cdnjs.com";

#pragma warning disable IDE1006
    public string? baseUrl { get; set; }
#pragma warning restore IDE1006

    public bool TryGetBaseUrl(out Uri result)
    {
        string? uriString = baseUrl.ToTrimmedOrNullIfEmpty();
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

    public const string DEFAULT_ProviderName = "cdnjs";

#pragma warning disable IDE1006
    public string? providerName { get; set; }
#pragma warning restore IDE1006

    public string GetProviderName() { return providerName.ToWsNormalizedOrDefaultIfEmpty(DEFAULT_ProviderName); }
    
#pragma warning disable IDE1006
    public string[]? addLibraries { get; set; }
#pragma warning restore IDE1006

    public IEnumerable<string> GetAddLibraries() { return (addLibraries is null) ? Enumerable.Empty<string>() : addLibraries.Select(l => l.ToWsNormalizedOrEmptyIfNull()).Where(l => l.Length > 0).Distinct(StringComparer.InvariantCultureIgnoreCase); }

#pragma warning disable IDE1006
    public string[]? removeLibraries { get; set; }
#pragma warning restore IDE1006

    public IEnumerable<string> GetRemoveLibraries() { return (removeLibraries is null) ? Enumerable.Empty<string>() : removeLibraries.Select(l => l.ToWsNormalizedOrEmptyIfNull()).Where(l => l.Length > 0).Distinct(StringComparer.InvariantCultureIgnoreCase); }
}