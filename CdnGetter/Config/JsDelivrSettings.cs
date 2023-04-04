namespace CdnGetter.Config;

public class JsDelivrSettings
{
    public const string PROVIDER_ID = "8856d6ab-4acd-44e1-9966-02acc6ded235";
    public const string PROVIDER_NAME = "jsdelivr";
    public const string DEFAULT_BaseUrl = "https://data.jsdelivr.com";

    /// <summary>
    /// Specifies the base url for the cdnjs upstream CDN service.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Attempts to contruct the absolute base URL of the upstream cdnjs service.
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
}