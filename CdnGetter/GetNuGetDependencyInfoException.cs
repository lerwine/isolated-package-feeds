using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace CdnGetter;

[Serializable]
public class GetNuGetDependencyInfoException : LoggedException
{
    public string PackageID { get; private set; } = null!;

    public NuGetVersion Version { get; private set; } = null!;
    
    public string Url { get; private set; } = null!;
    
    public GetNuGetDependencyInfoException() { }

    private GetNuGetDependencyInfoException(string message, string packageID, NuGetVersion version, string url, Exception inner) : base(message, inner) => (PackageID, Url, Version) = (packageID, url, version);
    
    protected GetNuGetDependencyInfoException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    internal static GetNuGetDependencyInfoException LogAndCreate(ILogger logger, string packageID, NuGetVersion version, string url, Exception inner) => LogAndCreate(logger, () =>
        new GetNuGetDependencyInfoException($"Failed to get NuGet dependency information for package with ID {JsonValue.Create(packageID)?.ToJsonString() ?? packageID} and version {version} from {url}", packageID, version, url, inner));

    protected override void Log(ILogger logger) => logger.LogGetNuGetDependencyInfoFailure(PackageID, Version, Url, InnerException!);
}
