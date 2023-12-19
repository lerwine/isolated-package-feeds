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
