using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using static IsolatedPackageFeeds.Shared.CommonStatic;

namespace NuGetPuller;

public abstract class LocalClientServiceBase(IValidatedRepositoryPathsService settings, ILogger logger) : ClientService(Repository.Factory.GetCoreV3(settings.LocalRepository.GetResult().FullName), settings, logger, false)
{
    private readonly object _syncRoot = new();

    #region Methods using the Search Query API

    private Task<PackageSearchResource>? _getPackageSearchResourceAsync;

    private async Task<PackageSearchResource> GetPackageSearchResourceAsync(CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _getPackageSearchResourceAsync ??= SourceRepository.GetResourceAsync<PackageSearchResource>(cancellationToken);
        return await _getPackageSearchResourceAsync;
    }

    private async Task<ContextScope<PackageSearchResource>> GePackageSearchResourceScopeAsync(Func<IDisposable?> scopeFactory, CancellationToken cancellationToken) =>
        new(await GetPackageSearchResourceAsync(cancellationToken), scopeFactory());

    /// <summary>
    /// Gets all packages in the local repository.
    /// </summary>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>Metadata for packages in local repository.</returns>
    public async IAsyncEnumerable<IPackageSearchMetadata> GetAllPackagesAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var scope = await GePackageSearchResourceScopeAsync(() => Logger.BeginGetAllLocalPackagesScope(SourceRepository.PackageSource.Source), cancellationToken);
        var resource = scope.Context;
        var skip = 0;
        int count;
        do
        {
            count = 0;
            foreach (var item in await resource.SearchAsync(null, null, skip, 50, NuGetLogger, cancellationToken))
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

    private Task<PackageUpdateResource>? _getPackageUpdateResourceAsync;

    private async Task<PackageUpdateResource> GetPackageUpdateResourceAsync(CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _getPackageUpdateResourceAsync ??= SourceRepository.GetResourceAsync<PackageUpdateResource>(cancellationToken);
        return await _getPackageUpdateResourceAsync;
    }

    // private async Task<ContextScope<PackageUpdateResource>> GetPackageUpdateResourceScopeAsync(Func<IDisposable?> scopeFactory, CancellationToken cancellationToken) =>
    //     new(await GetPackageUpdateResourceAsync(cancellationToken), scopeFactory());

    /// <summary>
    /// Asyncrhonously deletes packages.
    /// </summary>
    /// <param name="packageIds">Package IDs of packages to be deleted.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>The Package IDs of the packages that were actually deleted.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageUpdateResource.cs#L157"/>
    public async IAsyncEnumerable<(PackageIdentity Package, bool Success)> DeleteAsync(IEnumerable<string> packageIds, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(packageIds);
        if (!(packageIds = packageIds.Select(i => i?.Trim()!).Where(i => !string.IsNullOrEmpty(i))).Any())
            yield break;
        var findPackageById = await GetFindPackageByIdResourceAsync(cancellationToken);
        var packageUpdate = await GetPackageUpdateResourceAsync(cancellationToken);
        foreach (string id in packageIds.Distinct(NoCaseComparer))
        {
            using var scope = Logger.BeginDeleteLocalPackageScope(id, PackageSourceLocation);
            var lc = id.ToLower();
            var allVersions = await findPackageById.GetAllVersionsAsync(lc, CacheContext, NuGetLogger, cancellationToken);
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
                    await packageUpdate.Delete(lc, version.ToString(), s => string.Empty, s => true, true, NuGetLogger);
                    success = true;
                }
                catch (ArgumentException error)
                {
                    success = false;
                    Logger.LogPackageVersionDeleteFailure(id, version, error);
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
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageUpdateResource.cs#L157"/>
    public async IAsyncEnumerable<(NuGetVersion Version, bool Success)> DeleteAsync(string packageId, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        using var scope = Logger.BeginDeleteLocalPackageScope(packageId, PackageSourceLocation);
        var findPackageById = await GetFindPackageByIdResourceAsync(cancellationToken);
        var lc = packageId.ToLower();
        var allVersions = await findPackageById.GetAllVersionsAsync(lc, CacheContext, NuGetLogger, cancellationToken);
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
                Logger.LogPackageVersionDeleteFailure(packageId, version, error);
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
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageUpdateResource.cs#L55"/>
    public async Task AddPackageAsync(string fileName, bool skipDuplicate, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        var packageUpdateResource = await GetPackageUpdateResourceAsync(cancellationToken);
        try
        {
            await packageUpdateResource.Push(
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
            Logger.LogPackageFileNotZipArchive(fileName, error);
        }
        catch (PackagingException error)
        {
            Logger.LogPackageFileInvalidContent(fileName, error);
        }
    }

    #endregion
}

