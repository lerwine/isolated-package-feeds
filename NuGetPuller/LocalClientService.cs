using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using static NuGetPuller.Constants;

namespace NuGetPuller;

public sealed class LocalClientService(IOptions<AppSettings> options, ILogger<UpstreamClientService> logger) : ClientService(Repository.Factory.GetCoreV3(options.Value.Validated.LocalRepositoryPath), options, logger, false)
{
    private readonly object _syncRoot = new();
    private Task<PackageUpdateResource>? _getPackageUpdateResourceAsync;

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

    public async Task<List<IPackageSearchMetadata>> GetAllPackagesAsync(CancellationToken cancellationToken)
    {
        using var scope = await GePackageSearchResourceScopeAsync(() => Logger.BeginGetAllLocalPackagesScope(SourceRepository.PackageSource.Source), cancellationToken);
        var resource = scope.Context;
        List<IPackageSearchMetadata> result = new();
        var skip = 0;
        IPackageSearchMetadata[] items;
        while ((items = (await resource.SearchAsync(null, null, skip, 50, NuGetLogger, cancellationToken)).ToArray()).Length > 0)
        {
            skip += items.Length;
            result.AddRange(items);
        }
        return result;
    }

    #endregion

    #region Methods using the NuGet V3 Push and Delete API

    private async Task<PackageUpdateResource> GetPackageUpdateResourceAsync(CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _getPackageUpdateResourceAsync ??= SourceRepository.GetResourceAsync<PackageUpdateResource>(cancellationToken);
        return await _getPackageUpdateResourceAsync;
    }

    // private async Task<ContextScope<PackageUpdateResource>> GetPackageUpdateResourceScopeAsync(Func<IDisposable?> scopeFactory, CancellationToken cancellationToken) =>
    //     new(await GetPackageUpdateResourceAsync(cancellationToken), scopeFactory());

    private async Task<IEnumerable<string>> DeleteAsync(IEnumerable<string> packageIds, FindPackageByIdResource findPackageById, PackageUpdateResource resource, CancellationToken cancellationToken)
    {
        HashSet<string> result = new(NoCaseComparer);
        foreach (string id in packageIds.Distinct(NoCaseComparer))
        {
            using var scope = Logger.BeginDeleteLocalPackageScope(id, PackageSourceLocation);
            var lc = id.ToLower();
            var versions = await findPackageById.GetAllVersionsAsync(lc, CacheContext, NuGetLogger, cancellationToken);
            if (versions is null || !versions.Any())
                continue;
            result.Add(id);
            foreach (var v in versions)
            {
                using var scope2 = Logger.BeginDeleteLocalPackageVersionScope(id, v, PackageSourceLocation);
                try { await resource.Delete(lc, v.ToString(), s => string.Empty, s => true, true, NuGetLogger); }
                catch (ArgumentException error) { Logger.LogPackageVersionDeleteFailure(id, v, error); }
            }
        }
        return result;
    }

    /// <summary>
    /// Asyncrhonously deletes packages.
    /// </summary>
    /// <param name="packageIds">Package IDs of packages to be deleted.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>The Package IDs of the packages that were actually deleted.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageUpdateResource.cs#L157"/>
    public async Task<IEnumerable<string>> DeleteAsync(IEnumerable<string> packageIds, CancellationToken cancellationToken)
    {
        if (packageIds is null || !(packageIds = packageIds.Select(i => i?.Trim()!).Where(i => !string.IsNullOrEmpty(i))).Any())
            return Enumerable.Empty<string>();
        var findPackageById = await GetFindPackageByIdResourceAsync(cancellationToken);
        var packageUpdate = await GetPackageUpdateResourceAsync(cancellationToken);
        return await DeleteAsync(packageIds, findPackageById, packageUpdate, cancellationToken);
    }

    /// <summary>
    /// Asynchronously deletes a package.
    /// </summary>
    /// <param name="packageId">The identifier of the package to be deleted.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see langword="true"/> if the package was found and deleted; otherwise, <see langword="false"/>.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageUpdateResource.cs#L157"/>
    public async Task<bool> DeleteAsync(string packageId, CancellationToken cancellationToken)
    {
        if ((packageId = packageId.ToTrimmedOrNullIfEmpty()!) is null)
            return false;
        using var scope = Logger.BeginDeleteLocalPackageScope(packageId, PackageSourceLocation);
        var findPackageById = await GetFindPackageByIdResourceAsync(cancellationToken);
        var lc = packageId.ToLower();
        var versions = await findPackageById.GetAllVersionsAsync(lc, CacheContext, NuGetLogger, cancellationToken);
        if (versions is null || !versions.Any())
            return false;
        var packageUpdate = await GetPackageUpdateResourceAsync(cancellationToken);
        foreach (var v in versions)
        {
            using var scope2 = Logger.BeginDeleteLocalPackageVersionScope(lc, v, PackageSourceLocation);
            try { await packageUpdate.Delete(lc, v.ToString(), s => string.Empty, s => true, true, NuGetLogger); }
            catch (ArgumentException error) { Logger.LogPackageVersionDeleteFailure(packageId, v, error); }
        }
        return true;
    }

    /// <summary>
    /// Asynchronously adds a NuGet package.
    /// </summary>
    /// <param name="fileName">The path to the NuGet package file.</param>
    /// <param name="skipDuplicate">Whether to skip duplicate packages.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see langword="true"/> if the package was added; otherwise, <see langword="false"/>.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageUpdateResource.cs#L55"/>
    public async Task<bool> AddPackageAsync(string fileName, bool skipDuplicate, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        if (File.Exists(fileName))
        {
            var packageUpdateResource = await GetPackageUpdateResourceAsync(cancellationToken);
            try
            {
                await packageUpdateResource.Push(packagePaths: new string[] { fileName }, symbolSource: null, timeoutInSecond: 60, disableBuffering: false, getApiKey: s => null, getSymbolApiKey: null, noServiceEndpoint: false, skipDuplicate: skipDuplicate, symbolPackageUpdateResource: null, log: NuGetLogger);
                return true;
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
        else
            Logger.LogPackageFileNotFound(fileName);
        return false;
    }

    #endregion
}

