using IsolatedPackageFeeds.Shared.LazyInit;

namespace NuGetPuller;

public interface IValidatedRepositoryPathsService
{
    LazyChainedConversion<string, Uri> UpstreamServiceIndex { get; }

    LazyChainedConversion<(string Path, bool IsDefault), DirectoryInfo> LocalRepository { get; }

    LazyChainedConversion<string, DirectoryInfo> GlobalPackagesFolder { get; }
}
