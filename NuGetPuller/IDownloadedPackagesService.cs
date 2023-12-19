using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGetPuller;

/// <summary>
/// Interface for reading from the NuGet Packages Folder containing the downloaded packages.
/// </summary>
public interface IDownloadedPackagesService : INuGetClientService
{
    #region Methods using the Search Query API

    /// <summary>
    /// Gets the NuGet resource for the NuGet Search Query API.
    /// </summary>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>A task that asynchronously returns the the NuGet resource for the NuGet Search Query API.</returns>
    Task<PackageSearchResource> GetPackageSearchResourceAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets all packages in the Downloaded NuGet Packages Folder.
    /// </summary>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>Metadata for packages in Downloaded NuGet Packages Folder.</returns>
    IAsyncEnumerable<IPackageSearchMetadata> GetAllPackagesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets all packages in the Downloaded NuGet Packages Folder.
    /// </summary>
    /// <param name="packageSearchResource">The NuGet resource for the NuGet Search Query API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>Metadata for packages in Downloaded NuGet Packages Folder.</returns>
    IAsyncEnumerable<IPackageSearchMetadata> GetAllPackagesAsync(PackageSearchResource? packageSearchResource, CancellationToken cancellationToken);

    #endregion

    #region Methods using the NuGet V3 Push and Delete API

    /// <summary>
    /// Gets the NuGet resource for the NuGet V3 Push and Delete API.
    /// </summary>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>A task that asynchronously returns the the NuGet resource for the NuGet V3 Push and Delete API.</returns>
    Task<PackageUpdateResource> GetPackageUpdateResourceAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Asyncrhonously deletes packages.
    /// </summary>
    /// <param name="packageIds">Package IDs of packages to be deleted.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>The Package IDs of the packages that were actually deleted.</returns>
    IAsyncEnumerable<(PackageIdentity Package, bool Success)> DeleteAsync(IEnumerable<string> packageIds, CancellationToken cancellationToken);

    IAsyncEnumerable<(PackageIdentity Package, bool Success)> DeleteAsync(IEnumerable<string> packageIds, FindPackageByIdResource? findPackageByIdResource, CancellationToken cancellationToken);


    /// <summary>
    /// Asyncrhonously deletes packages.
    /// </summary>
    /// <param name="packageIds">Package IDs of packages to be deleted.</param>
    /// <param name="packageUpdateResource">The NuGet resource for the NuGet V3 Push and Delete API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>The Package IDs of the packages that were actually deleted.</returns>
    IAsyncEnumerable<(PackageIdentity Package, bool Success)> DeleteAsync(IEnumerable<string> packageIds, PackageUpdateResource? packageUpdateResource, CancellationToken cancellationToken);


    /// <summary>
    /// Asyncrhonously deletes packages.
    /// </summary>
    /// <param name="packageIds">Package IDs of packages to be deleted.</param>
    /// <param name="findPackageByIdResource">The NuGet resource for the NuGet V3 Package Content API.</param>
    /// <param name="packageUpdateResource">The NuGet resource for the NuGet V3 Push and Delete API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>The Package IDs of the packages that were actually deleted.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageUpdateResource.cs#L157"/>
    IAsyncEnumerable<(PackageIdentity Package, bool Success)> DeleteAsync(IEnumerable<string> packageIds, FindPackageByIdResource? findPackageByIdResource, PackageUpdateResource? packageUpdateResource, CancellationToken cancellationToken);


    /// <summary>
    /// Asynchronously deletes a package.
    /// </summary>
    /// <param name="packageId">The identifier of the package to be deleted.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see langword="true"/> if the package was found and deleted; otherwise, <see langword="false"/>.</returns>
    IAsyncEnumerable<(NuGetVersion Version, bool Success)> DeleteAsync(string packageId, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously deletes a package.
    /// </summary>
    /// <param name="packageId">The identifier of the package to be deleted.</param>
    /// <param name="findPackageByIdResource">The NuGet resource for the NuGet V3 Package Content API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see langword="true"/> if the package was found and deleted; otherwise, <see langword="false"/>.</returns>
    IAsyncEnumerable<(NuGetVersion Version, bool Success)> DeleteAsync(string packageId, FindPackageByIdResource? findPackageByIdResource, CancellationToken cancellationToken);


    /// <summary>
    /// Asynchronously adds a NuGet package.
    /// </summary>
    /// <param name="fileName">The path to the NuGet package file.</param>
    /// <param name="skipDuplicate">Whether to skip duplicate packages.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see langword="true"/> if the package was added; otherwise, <see langword="false"/>.</returns>
    Task AddPackageAsync(string fileName, bool skipDuplicate, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously adds a NuGet package.
    /// </summary>
    /// <param name="fileName">The path to the NuGet package file.</param>
    /// <param name="skipDuplicate">Whether to skip duplicate packages.</param>
    /// <param name="packageUpdateResource">NuGet resource for the NuGet V3 Push and Delete API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see langword="true"/> if the package was added; otherwise, <see langword="false"/>.</returns>
    Task AddPackageAsync(string fileName, bool skipDuplicate, PackageUpdateResource? packageUpdateResource, CancellationToken cancellationToken);

    #endregion
}

