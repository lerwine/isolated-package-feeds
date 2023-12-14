using IsolatedPackageFeeds.Shared.LazyInit;

namespace NuGetPuller;

/// <summary>
/// Interface for validation of repository paths.
/// </summary>
public interface IValidatedRepositoryPathsService
{
    /// <summary>
    /// Gets the the original settings values.
    /// </summary>
    ISharedAppSettings Settings { get; }

    /// <summary>
    /// Lazy validation for the <see cref="ISharedAppSettings.UpstreamServiceIndexUrl"/> setting.
    /// </summary>
    LazyChainedConversion<string, Uri> UpstreamServiceIndexUrl { get; }

    /// <summary>
    /// Lazy validation for the <see cref="ISharedAppSettings.LocalFeedPath"/> setting.
    /// </summary>
    LazyChainedConversion<string, DirectoryInfo> LocalFeedPath { get; }

    /// <summary>
    /// Lazy validation for the <see cref="ISharedAppSettings.UpstreamServicGlobalPackagesFoldereIndex"/> setting.
    /// </summary>
    LazyChainedConversion<string, DirectoryInfo> GlobalPackagesFolder { get; }
}
