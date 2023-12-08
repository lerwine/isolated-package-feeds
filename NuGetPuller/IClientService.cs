using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGetPuller;

public interface IClientService : IDisposable
{
    bool IsUpstream { get; }
    string PackageSourceLocation { get; }
    Uri PackageSourceUri { get; }
    
    /// <summary>
    /// Gets metadata for a specified package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets metadata for a specified package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="includePrerelease">Whether to include PreRelease versions into result.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns a list of package meta data objects.</returns>
    Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, CancellationToken cancellationToken);

    /// <summary>
    /// Gets metadata for a specified package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="includePrerelease">Whether to include PreRelease versions into result.</param>
    /// <param name="includeUnlisted">Whether to include Unlisted versions into result.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns a list of package meta data objects.</returns>
    Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, bool includeUnlisted, CancellationToken cancellationToken);

    /// <summary>
    /// Gets metadata for a specified package version.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the package meta data object or <see langword="null"/> if not found.</returns>
    Task<IPackageSearchMetadata?> GetMetadataAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously gets all package versions for a package ID.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="resource">The resource capable of fetching package versions.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the package versions for the specified <paramref name="packageId"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="packageId" /> is either <c>null</c> or an empty string.</exception>
    /// <exception cref="OperationCanceledException">Thrown if <paramref name="cancellationToken" /> is cancelled.</exception>
    Task<IEnumerable<NuGetVersion>> GetAllVersionsAsync(string packageId, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously gets dependency information for a specific package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns dependency information for the specified <paramref name="packageId"/> and <paramref name="version"/> or <see langword="null"/> if not found.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="packageId" /> is either <c>null</c> or an empty string.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="version" /> <c>null</c>.</exception>
    Task<FindPackageByIdDependencyInfo?> GetDependencyInfoAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously downloads and copies NuGet package to output stream.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="destination">The output stream.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    Task CopyNupkgToStreamAsync(string packageId, NuGetVersion version, Stream destination, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously checks whether the exact package id and version exists at this source.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns <see langword="true"/> if the exact package id and version exists at this source; otherwise, <see langword="true"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="id" />
    /// is either <c>null</c> or an empty string.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="version" /> <c>null</c>.</exception>
    Task<bool> DoesPackageExistAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieve dependency info for a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="framework">The project target framework.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependency information or <see langword="null"/> if the package does not exist.</returns>
    Task<SourcePackageDependencyInfo?> ResolvePackage(string packageId, NuGetVersion version, NuGetFramework framework, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieve dependency info for all versions of a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="framework">The project target framework.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependency information for all versions of a single package.</returns>
    Task<IEnumerable<SourcePackageDependencyInfo>> ResolvePackages(string packageId, NuGetFramework framework, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieve all dependencies for a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="framework">The project target framework.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependencies for the specified package.</returns>
    IAsyncEnumerable<SourcePackageDependencyInfo> GetAllDependenciesAsync(string packageId, NuGetVersion version, NuGetFramework framework, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieve all versions of packages, including all versions of all dependencies.
    /// </summary>
    /// <param name="packageIds">The IDs of the packages to retrieve versions and dependencies for.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>All versions of packages, including all versions of all dependencies</returns>
    IAsyncEnumerable<PackageIdentity> GetAllVersionsWithDependenciesAsync(IEnumerable<string> packageIds, CancellationToken cancellationToken);
}
