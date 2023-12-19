using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using static NuGetPuller.NuGetPullerStatic;

namespace NuGetPuller;

/// <summary>
/// Base class providing methods for managing the Local NuGet Feed.
/// </summary>
/// <param name="settings">The validated settings service.</param>
/// <param name="logger">THe logger to write log information to.</param>
public class LocalNuGetFeedService(IValidatedRepositoryPathsService settings, ILogger<LocalNuGetFeedService> logger) : NuGetClientService(Repository.Factory.GetCoreV3(settings.LocalFeedPath.GetResult().FullName), settings, logger, false), ILocalNuGetFeedService
{
    #region Methods using the Search Query API

    /// <summary>
    /// Gets the NuGet resource for the NuGet Search Query API.
    /// </summary>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>A task that asynchronously returns the the NuGet resource for the NuGet Search Query API.</returns>
    public Task<PackageSearchResource> GetPackageSearchResourceAsync(CancellationToken cancellationToken) => SourceRepository.GetResourceAsync<PackageSearchResource>(cancellationToken);


    /// <summary>
    /// Gets all packages in the local repository.
    /// </summary>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>Metadata for packages in Local NuGet Feed.</returns>
    public IAsyncEnumerable<IPackageSearchMetadata> GetAllPackagesAsync(CancellationToken cancellationToken) => GetAllPackagesAsync(null, cancellationToken);

    /// <summary>
    /// Gets all packages in the Local NuGet Feed.
    /// </summary>
    /// <param name="packageSearchResource">The NuGet resource for the NuGet Search Query API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>Metadata for packages in Local NuGet Feed.</returns>
    public async IAsyncEnumerable<IPackageSearchMetadata> GetAllPackagesAsync(PackageSearchResource? packageSearchResource, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var scope = Logger.BeginGetAllLocalPackagesScope(SourceRepository.PackageSource.Source);
        packageSearchResource ??= await GetPackageSearchResourceAsync(cancellationToken);
        var skip = 0;
        int count;
        do
        {
            count = 0;
            foreach (var item in await packageSearchResource.SearchAsync(null, null, skip, 50, NuGetLogger, cancellationToken))
            {
                count++;
                cancellationToken.ThrowIfCancellationRequested();
                yield return item;
            }
            skip += count;
        } while (count > 0);
    }

    #endregion

    #region Methods using the NuGet V3 Push and Delete API

    /// <summary>
    /// Gets the NuGet resource for the NuGet V3 Push and Delete API.
    /// </summary>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>A task that asynchronously returns the the NuGet resource for the NuGet V3 Push and Delete API.</returns>
    public Task<PackageUpdateResource> GetPackageUpdateResourceAsync(CancellationToken cancellationToken) => SourceRepository.GetResourceAsync<PackageUpdateResource>(cancellationToken);

    /// <summary>
    /// Asyncrhonously deletes packages.
    /// </summary>
    /// <param name="packageIds">Package IDs of packages to be deleted.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>The Package IDs of the packages that were actually deleted.</returns>
    public IAsyncEnumerable<(PackageIdentity Package, bool Success)> DeleteAsync(IEnumerable<string> packageIds, CancellationToken cancellationToken) => DeleteAsync(packageIds, null, null, cancellationToken);

    public IAsyncEnumerable<(PackageIdentity Package, bool Success)> DeleteAsync(IEnumerable<string> packageIds, FindPackageByIdResource? findPackageByIdResource, CancellationToken cancellationToken) =>
        DeleteAsync(packageIds, findPackageByIdResource, null, cancellationToken);

    /// <summary>
    /// Asyncrhonously deletes packages.
    /// </summary>
    /// <param name="packageIds">Package IDs of packages to be deleted.</param>
    /// <param name="packageUpdateResource">The NuGet resource for the NuGet V3 Push and Delete API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>The Package IDs of the packages that were actually deleted.</returns>
    public IAsyncEnumerable<(PackageIdentity Package, bool Success)> DeleteAsync(IEnumerable<string> packageIds, PackageUpdateResource? packageUpdateResource, CancellationToken cancellationToken) =>
        DeleteAsync(packageIds, null, packageUpdateResource, cancellationToken);

    /// <summary>
    /// Asyncrhonously deletes packages.
    /// </summary>
    /// <param name="packageIds">Package IDs of packages to be deleted.</param>
    /// <param name="findPackageByIdResource">The NuGet resource for the NuGet V3 Package Content API.</param>
    /// <param name="packageUpdateResource">The NuGet resource for the NuGet V3 Push and Delete API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>The Package IDs of the packages that were actually deleted.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageUpdateResource.cs#L157"/>
    public async IAsyncEnumerable<(PackageIdentity Package, bool Success)> DeleteAsync(IEnumerable<string> packageIds, FindPackageByIdResource? findPackageByIdResource, PackageUpdateResource? packageUpdateResource, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(packageIds);
        if (!(packageIds = packageIds.Select(i => i?.Trim()!).Where(i => !string.IsNullOrEmpty(i))).Any())
            yield break;
        findPackageByIdResource ??= await GetFindPackageByIdResourceAsync(cancellationToken);
        packageUpdateResource ??= await GetPackageUpdateResourceAsync(cancellationToken);
        foreach (string id in packageIds.Distinct(PackageIdentitifierComparer))
        {
            using var scope = Logger.BeginDeleteLocalPackageScope(id, PackageSourceLocation);
            var lc = id.ToLower();
            var allVersions = await findPackageByIdResource.GetAllVersionsAsync(lc, CacheContext, NuGetLogger, cancellationToken);
            if (allVersions is null || !allVersions.Any())
            {
                yield return (new PackageIdentity(id, null), false);
                continue;
            }
            foreach (var version in allVersions)
            {
                bool success;
                using var scope2 = Logger.BeginDeleteLocalPackageVersionScope(id, version, PackageSourceLocation);
                try
                {
                    await packageUpdateResource.Delete(lc, version.ToString(), s => string.Empty, s => true, true, NuGetLogger);
                    success = true;
                }
                catch (ArgumentException error)
                {
                    success = false;
                    Logger.PackageVersionDeleteFailure(id, version, error);
                }
                yield return (new PackageIdentity(id, version), success);
            }
        }
    }

    /// <summary>
    /// Asynchronously deletes a package.
    /// </summary>
    /// <param name="packageId">The identifier of the package to be deleted.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see langword="true"/> if the package was found and deleted; otherwise, <see langword="false"/>.</returns>
    public IAsyncEnumerable<(NuGetVersion Version, bool Success)> DeleteAsync(string packageId, CancellationToken cancellationToken) => DeleteAsync(packageId, null, cancellationToken);

    /// <summary>
    /// Asynchronously deletes a package.
    /// </summary>
    /// <param name="packageId">The identifier of the package to be deleted.</param>
    /// <param name="findPackageByIdResource">The NuGet resource for the NuGet V3 Package Content API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see langword="true"/> if the package was found and deleted; otherwise, <see langword="false"/>.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageUpdateResource.cs#L157"/>
    public async IAsyncEnumerable<(NuGetVersion Version, bool Success)> DeleteAsync(string packageId, FindPackageByIdResource? findPackageByIdResource, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        using var scope = Logger.BeginDeleteLocalPackageScope(packageId, PackageSourceLocation);
        var lc = packageId.ToLower();
        var allVersions = await (findPackageByIdResource ?? await GetFindPackageByIdResourceAsync(cancellationToken)).GetAllVersionsAsync(lc, CacheContext, NuGetLogger, cancellationToken);
        if (allVersions is null || !allVersions.Any())
            yield break;
        var packageUpdate = await GetPackageUpdateResourceAsync(cancellationToken);
        foreach (var version in allVersions)
        {
            using var scope2 = Logger.BeginDeleteLocalPackageVersionScope(lc, version, PackageSourceLocation);
            bool success;
            try
            {
                await packageUpdate.Delete(lc, version.ToString(), s => string.Empty, s => true, true, NuGetLogger);
                success = true;
            }
            catch (ArgumentException error)
            {
                success = false;
                Logger.PackageVersionDeleteFailure(packageId, version, error);
            }
            yield return (version, success);
        }
    }

    /// <summary>
    /// Asynchronously adds a NuGet package.
    /// </summary>
    /// <param name="fileName">The path to the NuGet package file.</param>
    /// <param name="skipDuplicate">Whether to skip duplicate packages.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see langword="true"/> if the package was added; otherwise, <see langword="false"/>.</returns>
    public Task AddPackageAsync(string fileName, bool skipDuplicate, CancellationToken cancellationToken) => AddPackageAsync(fileName, skipDuplicate, null, cancellationToken);

    /// <summary>
    /// Asynchronously adds a NuGet package.
    /// </summary>
    /// <param name="fileName">The path to the NuGet package file.</param>
    /// <param name="skipDuplicate">Whether to skip duplicate packages.</param>
    /// <param name="packageUpdateResource">NuGet resource for the NuGet V3 Push and Delete API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see langword="true"/> if the package was added; otherwise, <see langword="false"/>.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageUpdateResource.cs#L55"/>
    public async Task AddPackageAsync(string fileName, bool skipDuplicate, PackageUpdateResource? packageUpdateResource, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        try
        {
            await (packageUpdateResource ?? await GetPackageUpdateResourceAsync(cancellationToken)).Push(
                packagePaths: new[] { fileName },
                symbolSource: null,
                timeoutInSecond: 60,
                disableBuffering: false,
                getApiKey: s => null,
                getSymbolApiKey: null,
                noServiceEndpoint: false,
                skipDuplicate: skipDuplicate,
                symbolPackageUpdateResource: null,
                log: NuGetLogger
            );
        }
        catch (InvalidDataException error)
        {
            Logger.NupkgFileNotZipArchive(fileName, error);
        }
        catch (PackagingException error)
        {
            Logger.InvalidNupkgFileContent(fileName, error);
        }
    }

    #endregion
}

