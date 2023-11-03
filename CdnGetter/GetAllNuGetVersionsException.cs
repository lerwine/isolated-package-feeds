using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

namespace CdnGetter;

[Serializable]
public class GetAllNuGetVersionsException : LoggedException
{
    public string PackageID { get; private set; } = null!;

    public string Url { get; private set; } = null!;
    
    public GetAllNuGetVersionsException() { }

    private GetAllNuGetVersionsException(string message, string packageID, string url, Exception inner) : base(message, inner) => (PackageID, Url) = (packageID, url);
    
    protected GetAllNuGetVersionsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    internal static GetAllNuGetVersionsException LogAndCreate(ILogger logger, string packageID, string url, Exception inner) => LogAndCreate(logger, () =>
        new GetAllNuGetVersionsException($"Failed to get NuGet versoins for package with ID {JsonValue.Create(packageID)?.ToJsonString() ?? packageID} from {url}", packageID, url, inner));

    protected override void Log(ILogger logger) => logger.LogGetAllNuGetVersionsFailure(PackageID, Url, InnerException!);
}
