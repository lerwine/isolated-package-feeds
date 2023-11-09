using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace CdnGetter;

[Serializable]
public class GetNuGetMetaDataException : LoggedException
{
    public string PackageID { get; private set; } = null!;

    public string Url { get; private set; } = null!;
    
    public NuGetVersion? Version { get; private set; }

    public bool IncludePreRelease { get; private set; }
    
    public bool IncludeUnlisted { get; private set; }

    public GetNuGetMetaDataException() { }

    private GetNuGetMetaDataException(string message, string packageID, string url, bool includePreRelease, bool includeUnlisted, Exception inner) : base(message, inner) => (PackageID, Url, IncludePreRelease, IncludeUnlisted) = (packageID, url, includePreRelease, includeUnlisted);
    
    private GetNuGetMetaDataException(string message, string packageID, string url, NuGetVersion version, Exception inner) : base(message, inner) => (PackageID, Url, Version) = (packageID, url, version);
    
    protected GetNuGetMetaDataException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    internal static GetNuGetMetaDataException LogAndCreate(ILogger logger, string packageID, string url, bool includePreRelease, bool includeUnlisted, Exception inner) => LogAndCreate(logger, () =>
        new GetNuGetMetaDataException($"Failed to get NuGet MetaData for package with ID {JsonValue.Create(packageID)?.ToJsonString() ?? packageID} from {url}: IncludePreRelease={includePreRelease}; IncludeUnlisted={includeUnlisted}", packageID, url, includePreRelease, includeUnlisted, inner));

    internal static GetNuGetMetaDataException LogAndCreate(ILogger logger, string packageID, string url, NuGetVersion version, Exception inner) => LogAndCreate(logger, () =>
        new GetNuGetMetaDataException($"Failed to get NuGet MetaData for package with ID {JsonValue.Create(packageID)?.ToJsonString() ?? packageID} from {url}: Version={version}", packageID, url, version, inner));

    protected override void Log(ILogger logger)
    {
        if (Version is null)
            logger.LogGetNuGetMetaDataFailure(PackageID, Url, IncludePreRelease, IncludeUnlisted, InnerException!);
        else
            logger.LogGetNuGetMetaDataFailure(PackageID, Url, Version, InnerException!);
    }
}
