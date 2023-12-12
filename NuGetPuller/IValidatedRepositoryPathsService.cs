using IsolatedPackageFeeds.Shared.LazyInit;

namespace NuGetPuller;

public interface IValidatedRepositoryPathsService
{
    LazyChainedConversion<string, Uri> UpstreamServiceIndex { get; }

    LazyChainedConversion<string, DirectoryInfo> LocalRepository { get; }

    LazyChainedConversion<string, DirectoryInfo> GlobalPackagesFolder { get; }
}
