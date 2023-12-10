using System.Diagnostics.CodeAnalysis;
using IsolatedPackageFeeds.Shared;

namespace CdnGetter.Config;

public class CdnJsSettings : ICdnSettings
{
    /// <summary>
    /// The unique GUID string value for the <see cref="Model.UpstreamCdn.Id" /> property of the associated database entity.
    /// </summary>
    public const string PROVIDER_ID = "026ce2f6-c574-4869-bd4c-73bd73dfb640";

    /// <summary>
    /// The unique provider name that is specified in the <see cref="Services.ContentGetterAttribute.Name" /> attribute property and used for the <see cref="Model.UpstreamCdn.Name" /> property of the associated database entity.
    /// </summary>
    public const string PROVIDER_NAME = "cdnjs";

    /// <summary>
    /// The default server URL for the cdnjs CDN API.
    /// </summary>
    public const string DEFAULT_API_SERVER_URL = "https://api.cdnjs.com";

    /// <summary>
    /// Specifies the server url for the cdnjs CDN API.
    /// </summary>
    /// <remarks>This needs to be an absolute <see cref="Uri.UriSchemeHttp">http</see> or <see cref="Uri.UriSchemeHttps">https</see> URL with no path, query or fragment.
    /// <see cref="DEFAULT_API_SERVER_URL" /> contains the default value for this setting.</remarks>
    /// <seealso href="https://cdnjs.com/api" />
    public string? ApiServerUrl { get; set; }

    /// <summary>
    /// Attempts to contruct the URL for the cdnjs CDN API server.
    /// </summary>
    /// <param name="logger">The logger service.</param>
    /// <param name="result">The constructed URI or <see langword="null" /> if this method returned <see langword="false" />.</param>
    /// <returns><see langword="true" /> if <paramref name="result" /> represents the absolute URL of the cdnjs API server; otherwise <see langword="true" /> if <paramref name="result" /> contains a relative URI,
    /// indicating that <see cref="ApiServerUrl" /> did not represent an absolute <see cref="Uri.UriSchemeHttp">http</see> or <see cref="Uri.UriSchemeHttps">https</see> URL - or - a URI could not be constructed.</returns>
    /// <seealso href="https://cdnjs.com/api" />
    public bool TryGetApiServerUrl(Microsoft.Extensions.Logging.ILogger logger, [NotNullWhen(true)] out Uri? result)
    {
        string? uriString = ApiServerUrl.ToTrimmedOrNullIfEmpty();
        if (uriString is null)
        {
            try { result = new Uri(DEFAULT_API_SERVER_URL, UriKind.Absolute); }
            catch (Exception exception)
            {
                logger.LogInvalidBaseUrlError<CdnJsSettings>(PROVIDER_NAME, DEFAULT_API_SERVER_URL, exception);
                result = null;
                return false;
            }
            if (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps)
                return true;
            logger.LogInvalidBaseUrlError<CdnJsSettings>(PROVIDER_NAME, DEFAULT_API_SERVER_URL);
            return false;
        }
        if (Uri.TryCreate(uriString, UriKind.Absolute, out result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps))
            return true;
        logger.LogInvalidBaseUrlError<CdnJsSettings>(PROVIDER_NAME, uriString);
        return false;
    }
}
