using Newtonsoft.Json;
using NuGet.Versioning;

namespace NuGetPuller;

/// <summary>
/// Static NuGetPuller classes, defaults, and definitions.
/// </summary>
public static class NuGetPullerStatic
{
    /// <summary>
    /// Gets the default URI path for <see cref="ISharedAppSettings.UpstreamServiceIndexUrl"/>.
    /// </summary>
    public const string Default_Upstream_Service_Index_Path = "/v3/index.json";

    /// <summary>
    /// Gets the default value for <see cref="ISharedAppSettings.UpstreamServiceIndexUrl"/>.
    /// </summary>
    public const string Default_Upstream_Service_Index_URL = $"https://api.nuget.org{Default_Upstream_Service_Index_Path}";

    /// <summary>
    /// The default path of the local feed, relative to the <see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />.
    /// </summary>
    public const string Default_Downloaded_Packages_Folder_Name = "LocalFeed";

    /// <summary>
    /// Comparer for package identifier and version strings.
    /// </summary>
    public static readonly StringComparer PackageIdentitifierComparer = StringComparer.OrdinalIgnoreCase;

    /// <summary>
    /// Inverted comparer for package identifier and version strings.
    /// </summary>
    public static readonly IsolatedPackageFeeds.Shared.InvertedComparer<string> InvertedPackageIdentitifierComparer = IsolatedPackageFeeds.Shared.InvertedComparer.StringOrdinalIgnoreCase;

    /// <summary>
    /// Inverted comparer for NuGet version including release and metadata.
    /// </summary>
    public static readonly IsolatedPackageFeeds.Shared.InvertedComparer<NuGetVersion> InvertedNuGetVersionReleaseMetadataComparer = new(VersionComparer.VersionReleaseMetadata);

    /// <summary>
    /// Inverted comparer for NuGet version including release.
    /// </summary>
    public static readonly IsolatedPackageFeeds.Shared.InvertedComparer<NuGetVersion> InvertedNuGetVersionReleaseComparer = new(VersionComparer.VersionRelease);

    /// <summary>
    /// Inverted comparer for NuGet version including.
    /// </summary>
    public static readonly IsolatedPackageFeeds.Shared.InvertedComparer<NuGetVersion> InvertedNuGetVersionComparer = new(VersionComparer.Version);

    /// <summary>
    /// Settings for deserializing <see cref="OfflinePackageMetadata" /> objects.
    /// </summary>
    public static readonly JsonSerializerSettings MetadataDeSerializationSettings = new()
    {
        MaxDepth = 512,
        NullValueHandling = NullValueHandling.Ignore,
        TypeNameHandling = TypeNameHandling.None,
        Converters = [new OfflinePackageMetadataConverter()],
        Formatting = Formatting.Indented
    };
}
