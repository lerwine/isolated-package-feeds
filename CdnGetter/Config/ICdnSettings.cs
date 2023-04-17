namespace CdnGetter.Config;

public interface ICdnSettings
{
    /// <summary>
    /// Specifies the server url for the CDN API.
    /// </summary>
    string? ApiServerUrl { get; }

    /// <summary>
    /// Attempts to contruct the URL for the CDN API server.
    /// </summary>
    /// <param name="logger">The logger service.</param>
    /// <param name="result">The constructed URI or <see langword="null" /> if this method returned <see langword="false" />.</param>
    /// <returns><see langword="true" /> if <paramref name="result" /> represents the absolute URL of the API server; otherwise <see langword="true" /> if <paramref name="result" /> contains a relative URI,
    /// indicating that <see cref="ApiServerUrl" /> did not represent an absolute <see cref="Uri.UriSchemeHttp">http</see> or <see cref="Uri.UriSchemeHttps">https</see> URL - or - a URI could not be constructed.</returns>
    bool TryGetApiServerUrl(Microsoft.Extensions.Logging.ILogger logger, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Uri? result);
}