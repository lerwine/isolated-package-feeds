using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

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

    private async Task<ContextScope<PackageUpdateResource>> GetPackageUpdateResourceScopeAsync(Func<IDisposable?> scopeFactory, CancellationToken cancellationToken) =>
        new(await GetPackageUpdateResourceAsync(cancellationToken), scopeFactory());

    private async Task<IEnumerable<string>> DeleteAsync(IEnumerable<string> packageIds, FindPackageByIdResource findPackageById, PackageUpdateResource resource, CancellationToken cancellationToken)
    {
        HashSet<string> result = new(StringComparer.CurrentCultureIgnoreCase);
        foreach (string id in packageIds.Distinct(StringComparer.CurrentCultureIgnoreCase))
        {
            using var scope = Logger.BeginDeleteLocalPackageScope(id, PackageSourceLocation);
            var lc = id.ToLower();
            var versions = await findPackageById.GetAllVersionsAsync(lc, CacheContext, NuGetLogger, cancellationToken);
            if (versions is null || !versions.Any())
                continue;
            result.Add(id);
            foreach (var v in versions)
            {
                using var scope2 = Logger.BeginDeleteLocalPackageVersionScope(lc, v, PackageSourceLocation);
                await resource.Delete(lc, v.ToString(), s => string.Empty, s => true, true, NuGetLogger);
            }
        }
        return result;
    }

    public async Task<IEnumerable<string>> DeleteAsync(IEnumerable<string> packageIds, CancellationToken cancellationToken)
    {
        if (packageIds is null || !(packageIds = packageIds.Select(i => i?.Trim()!).Where(i => !string.IsNullOrEmpty(i))).Any())
            return Enumerable.Empty<string>();
        var findPackageById = await GetFindPackageByIdResourceAsync(cancellationToken);
        var packageUpdate = await GetPackageUpdateResourceAsync(cancellationToken);
        return await DeleteAsync(packageIds, findPackageById, packageUpdate, cancellationToken);
    }

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
            await packageUpdate.Delete(lc, v.ToString(), s => string.Empty, s => true, true, NuGetLogger);
        }
        return true;
    }

    public async Task<bool> AddPackageAsync(string path, bool skipDuplicate, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (File.Exists(path))
        {
            var packageUpdateResource = await GetPackageUpdateResourceAsync(cancellationToken);
            try
            {
                await packageUpdateResource.Push(packagePaths: new string[] { path }, symbolSource: null, timeoutInSecond: 60, disableBuffering: false, getApiKey: s => null, getSymbolApiKey: null, noServiceEndpoint: false, skipDuplicate: skipDuplicate, symbolPackageUpdateResource: null, log: NuGetLogger);
                return true;
            }
            catch (InvalidDataException error)
            {
                Logger.LogPackageFileNotZipArchive(path, error);
            }
            catch (PackagingException error)
            {
                Logger.LogPackageFileInvalidContent(path, error);
            }
        }
        else
            Logger.LogPackageFileNotFound(path);
        return false;
    }

    #endregion
}

