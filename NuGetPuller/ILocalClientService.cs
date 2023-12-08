using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGetPuller;

public interface ILocalClientService : IClientService
{
    /// <summary>
    /// Gets all packages in the local repository.
    /// </summary>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>Metadata for packages in local repository.</returns>
    IAsyncEnumerable<IPackageSearchMetadata> GetAllPackagesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Asyncrhonously deletes packages.
    /// </summary>
    /// <param name="packageIds">Package IDs of packages to be deleted.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>The Package IDs of the packages that were actually deleted.</returns>
    IAsyncEnumerable<(PackageIdentity Package, bool Success)> DeleteAsync(IEnumerable<string> packageIds, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously deletes a package.
    /// </summary>
    /// <param name="packageId">The identifier of the package to be deleted.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see langword="true"/> if the package was found and deleted; otherwise, <see langword="false"/>.</returns>
    IAsyncEnumerable<(NuGetVersion Version, bool Success)> DeleteAsync(string packageId, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously adds a NuGet package.
    /// </summary>
    /// <param name="fileName">The path to the NuGet package file.</param>
    /// <param name="skipDuplicate">Whether to skip duplicate packages.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see langword="true"/> if the package was added; otherwise, <see langword="false"/>.</returns>
    Task AddPackageAsync(string fileName, bool skipDuplicate, CancellationToken cancellationToken);
}
