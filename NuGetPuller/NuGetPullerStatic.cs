using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NuGetPuller;

public static class NuGetPullerStatic
{
    public static readonly StringComparer PackageIdentitifierComparer = StringComparer.OrdinalIgnoreCase;

    public static readonly JsonSerializerSettings MetadataDeSerializationSettings = new()
    {
        MaxDepth = 512,
        NullValueHandling = NullValueHandling.Ignore,
        TypeNameHandling = TypeNameHandling.None,
        Converters = [new OfflinePackageManifestConverter()],
        Formatting = Formatting.Indented
    };
    
}