using NuGet.Frameworks;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGetPuller;

/// <summary>
/// Base interface for reading from a NuGet web API or Local NuGet Feed.
/// </summary>
public interface INuGetClientService : IDisposable
{
    bool IsUpstream { get; }

    Uri PackageSourceUri { get; }

    string PackageSourceLocation { get; }

    #region Methods using the Package Metadata Resource API.

    /// <summary>
    /// Gets the NuGet resource for the NuGet Package Metadata Resource API.
    /// </summary>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>A task that asynchronously returns the the NuGet resource for the NuGet Package Update API.</returns>
    Task<PackageMetadataResource> GetPackageMetadataResourceAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets metadata for a specified package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns a list of package meta data objects.</returns>
    Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets metadata for a specified package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="packageMetadataResource">The NuGet resource for the NuGet Package Metadata Resource API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns a list of package meta data objects.</returns>
    Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, PackageMetadataResource? packageMetadataResource, CancellationToken cancellationToken);

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
    /// <param name="packageMetadataResource">The NuGet resource for the NuGet Package Metadata Resource API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns a list of package meta data objects.</returns>
    Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, PackageMetadataResource? packageMetadataResource, CancellationToken cancellationToken);

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
    /// Gets metadata for a specified package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="includePrerelease">Whether to include PreRelease versions into result.</param>
    /// <param name="includeUnlisted">Whether to include Unlisted versions into result.</param>
    /// <param name="packageMetadataResource">The NuGet resource for the NuGet Package Metadata Resource API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns a list of package meta data objects.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageMetadataResourceV3.cs#L46"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalPackageMetadataResource.cs#L29"/>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/registration-base-url-resource#registration-page"/>
    Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, bool includeUnlisted, PackageMetadataResource? packageMetadataResource, CancellationToken cancellationToken);

    /// <summary>
    /// Gets metadata for a specified package version.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the package meta data object or <see langword="null"/> if not found.</returns>
    Task<IPackageSearchMetadata?> GetMetadataAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken);

    /// <summary>
    /// Gets metadata for a specified package version.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="packageMetadataResource">The NuGet resource for the NuGet Package Metadata Resource API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the package meta data object or <see langword="null"/> if not found.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageMetadataResourceV3.cs#L66"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalPackageMetadataResource.cs#L51"/>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/registration-base-url-resource#registration-page"/>
    Task<IPackageSearchMetadata?> GetMetadataAsync(string packageId, NuGetVersion version, PackageMetadataResource? packageMetadataResource, CancellationToken cancellationToken);

    #endregion

    #region Methods using the NuGet V3 Package Content API

    /// <summary>
    /// Gets the NuGet resource for the NuGet V3 Package Content API.
    /// </summary>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>A task that asynchronously returns the the NuGet resource for the NuGet V3 Package Content API.</returns>
    Task<FindPackageByIdResource> GetFindPackageByIdResourceAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously gets all package versions for a package ID.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the package versions for the specified <paramref name="packageId"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="packageId" /> is either <c>null</c> or an empty string.</exception>
    /// <exception cref="OperationCanceledException">Thrown if <paramref name="cancellationToken" /> is cancelled.</exception>
    Task<IEnumerable<NuGetVersion>> GetAllVersionsAsync(string packageId, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously gets all package versions for a package ID.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="findPackageByIdResource">The NuGet resource for the NuGet V3 Package Content API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the package versions for the specified <paramref name="packageId"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="packageId" /> is either <c>null</c> or an empty string.</exception>
    /// <exception cref="OperationCanceledException">Thrown if <paramref name="cancellationToken" /> is cancelled.</exception>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/package-base-address-resource#enumerate-package-versions"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/FindPackageByIdResource.cs#L37"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalV3FindPackageByIdResource.cs#L108"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/RemoteRepositories/RemoteV3FindPackageByIdResource.cs#L85"/>
    Task<IEnumerable<NuGetVersion>> GetAllVersionsAsync(string packageId, FindPackageByIdResource? findPackageByIdResource, CancellationToken cancellationToken);

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
    /// Asynchronously gets dependency information for a specific package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="findPackageByIdResource">The NuGet resource for the NuGet V3 Package Content API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns dependency information for the specified <paramref name="packageId"/> and <paramref name="version"/> or <see langword="null"/> if not found.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="packageId" /> is either <c>null</c> or an empty string.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="version" /> <c>null</c>.</exception>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/FindPackageByIdResource.cs#L61"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalV3FindPackageByIdResource.cs#L248"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/RemoteRepositories/RemoteV3FindPackageByIdResource.cs#L144"/>
    Task<FindPackageByIdDependencyInfo?> GetDependencyInfoAsync(string packageId, NuGetVersion version, FindPackageByIdResource? findPackageByIdResource, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously downloads and copies NuGet package to output stream.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="destination">The output stream.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    Task CopyNupkgToStreamAsync(string packageId, NuGetVersion version, Stream destination, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously downloads and copies NuGet package to output stream.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="destination">The output stream.</param>
    /// <param name="findPackageByIdResource">The NuGet resource for the NuGet V3 Package Content API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/package-publish-resource#push-a-package"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/FindPackageByIdResource.cs#L88"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalV3FindPackageByIdResource.cs#L167"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/RemoteRepositories/RemoteV3FindPackageByIdResource.cs#L222"/>
    Task CopyNupkgToStreamAsync(string packageId, NuGetVersion version, Stream destination, FindPackageByIdResource? findPackageByIdResource, CancellationToken cancellationToken);

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
    /// Asynchronously checks whether the exact package id and version exists at this source.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="findPackageByIdResource">The NuGet resource for the NuGet V3 Package Content API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns <see langword="true"/> if the exact package id and version exists at this source; otherwise, <see langword="true"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="id" />
    /// is either <c>null</c> or an empty string.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="version" /> <c>null</c>.</exception>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/FindPackageByIdResource.cs#L61"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalV3FindPackageByIdResource.cs#L388"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/RemoteRepositories/RemoteV3FindPackageByIdResource.cs#L368"/>
    Task<bool> DoesPackageExistAsync(string packageId, NuGetVersion version, FindPackageByIdResource? findPackageByIdResource, CancellationToken cancellationToken);

    #endregion

    #region Dependency Info Resource methods

    /// <summary>
    /// Gets the NuGet resource for the NuGet Dependency Info API.
    /// </summary>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>A task that asynchronously returns the the NuGet resource for the NuGet Dependency Info API.</returns>
    Task<DependencyInfoResource> GetDependencyInfoResourceAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieve dependency info for a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="framework">The project target framework.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependency information or <see langword="null"/> if the package does not exist.</returns>
    Task<SourcePackageDependencyInfo?> ResolvePackageAsync(string packageId, NuGetVersion version, NuGetFramework framework, CancellationToken cancellationToken);

    /// <summary>
    /// <summary>
    /// Retrieve dependency info for a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="framework">The project target framework.</param>
    /// <param name="dependencyInfoResource">The NuGet resource for the NuGet Dependency Info API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependency information or <see langword="null"/> if the package does not exist.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/DependencyInfoResourceV3.cs#L63"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalDependencyInfoResource.cs#L45"/>
    Task<SourcePackageDependencyInfo?> ResolvePackageAsync(string packageId, NuGetVersion version, NuGetFramework framework, DependencyInfoResource? dependencyInfoResource, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieve dependency info for all versions of a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="framework">The project target framework.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependency information for all versions of a single package.</returns>
    Task<IEnumerable<SourcePackageDependencyInfo>> ResolvePackagesAsync(string packageId, NuGetFramework framework, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieve dependency info for all versions of a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="framework">The project target framework.</param>
    /// <param name="dependencyInfoResource">The NuGet resource for the NuGet Dependency Info API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependency information for all versions of a single package.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/DependencyInfoResourceV3.cs#L102"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalDependencyInfoResource.cs#L92"/>
    Task<IEnumerable<SourcePackageDependencyInfo>> ResolvePackagesAsync(string packageId, NuGetFramework framework, DependencyInfoResource? dependencyInfoResource, CancellationToken cancellationToken);

    #endregion
}
