using Microsoft.Extensions.Logging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;

namespace CdnGetter;

[Serializable]
public class GetNuGetDownloadResourceResultsException : LoggedException
{
    public PackageIdentity Identity { get; private set; } = null!;

    public string Url { get; private set; } = null!;
    
    public PackageDownloadContext DownloadContext { get; private set; } = null!;
    
    public string GlobalPackagesFolder { get; private set; } = null!;

    public GetNuGetDownloadResourceResultsException() { }

    private GetNuGetDownloadResourceResultsException(string message, PackageIdentity identity, string url, PackageDownloadContext downloadContext, string globalPackagesFolder, Exception inner) : base(message, inner) => (Identity, Url, DownloadContext, GlobalPackagesFolder) = (identity, url, downloadContext, globalPackagesFolder);
    
    protected GetNuGetDownloadResourceResultsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    internal static GetNuGetDownloadResourceResultsException LogAndCreate(ILogger logger, PackageIdentity identity, string url, PackageDownloadContext downloadContext, string globalPackagesFolder, Exception inner) => LogAndCreate(logger, () =>
        new GetNuGetDownloadResourceResultsException($"Failed to get NuGet download resource results for identity {identity} from {url}: DownloadContext={downloadContext}; GlobalPackagesFolder={globalPackagesFolder}", identity, url, downloadContext, globalPackagesFolder, inner));

    protected override void Log(ILogger logger) => logger.LogGetNuGetDownloadResourceResultsFailure(Identity, Url, DownloadContext, GlobalPackagesFolder, InnerException!);
}
