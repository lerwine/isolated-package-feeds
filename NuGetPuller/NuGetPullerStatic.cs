using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NuGetPuller;

/// <summary>
/// Static NuGetPuller classes, defaults, and definitions.
/// </summary>
public static class NuGetPullerStatic
{
    /// <summary>
    /// Gets the default value for <see cref="ISharedAppSettings.UpstreamServiceIndexUrl"/>.
    /// </summary>
    public const string Default_Service_Index_URL = "https://api.nuget.org/v3/index.json";

    /// <summary>
    /// The default path of the local feed, relative to the <see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />.
    /// </summary>
    public const string Default_Local_Feed_Folder_Name = "LocalFeed";
    
    /// <summary>
    /// Comparer for package identifier and version strings.
    /// </summary>
    public static readonly StringComparer PackageIdentitifierComparer = StringComparer.OrdinalIgnoreCase;

    /// <summary>
    /// Settings for deserializing <see cref="OfflinePackageManifest" /> objects.
    /// </summary>
    public static readonly JsonSerializerSettings MetadataDeSerializationSettings = new()
    {
        MaxDepth = 512,
        NullValueHandling = NullValueHandling.Ignore,
        TypeNameHandling = TypeNameHandling.None,
        Converters = [new OfflinePackageManifestConverter()],
        Formatting = Formatting.Indented
    };
}
