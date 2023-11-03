using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace CdnGetter;

[Serializable]
public class DoesNuGetPackageExistException : LoggedException
{
    public string PackageID { get; private set; } = null!;

    public NuGetVersion Version { get; private set; } = null!;
    
    public string Url { get; private set; } = null!;
    
    public DoesNuGetPackageExistException() { }

    private DoesNuGetPackageExistException(string message, string packageID, NuGetVersion version, string url, Exception inner) : base(message, inner) => (PackageID, Url, Version) = (packageID, url, version);
    
    protected DoesNuGetPackageExistException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    internal static DoesNuGetPackageExistException LogAndCreate(ILogger logger, string packageID, NuGetVersion version, string url, Exception inner) => LogAndCreate(logger, () =>
        new DoesNuGetPackageExistException($"Failed to get test whether NuGet package with ID {JsonValue.Create(packageID)?.ToJsonString() ?? packageID} and version {version} exists from {url}", packageID, version, url, inner));

    protected override void Log(ILogger logger) => logger.LogDoesNuGetPackageExistFailure(PackageID, Version, Url, InnerException!);
}
