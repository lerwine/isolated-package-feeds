using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

    private async Task<ContextScope<PackageUpdateResource>> GePackageUpdateResourceScopeAsync(Func<IDisposable?> scopeFactory, CancellationToken cancellationToken) =>
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

    private async Task<IEnumerable<string>> AddAsync(IEnumerable<string> packageIds, FindPackageByIdResource findPackageById, PackageUpdateResource resource, UpstreamClientService upstreamClientService, CancellationToken cancellationToken)
    {
        HashSet<string> result = new(StringComparer.CurrentCultureIgnoreCase);
        foreach (string id in packageIds.Distinct(StringComparer.CurrentCultureIgnoreCase))
        {
            using var scope = Logger.BeginAddLocalPackageScope(id, PackageSourceLocation);
            var lc = id.ToLower();
            var upstreamVersions = await upstreamClientService.GetAllVersionsAsync(lc, cancellationToken);
            if (upstreamVersions is null || !upstreamVersions.Any())
                continue;
            HashSet<NuGetVersion> toAdd = new();
            foreach (NuGetVersion v in upstreamVersions)
            {
                if (!await findPackageById.DoesPackageExistAsync(lc, v, CacheContext, NuGetLogger, cancellationToken))
                    toAdd.Add(v);
            }
            if (toAdd.Count == 0)
                continue;
            result.Add(id);
            // TODO: Download and add packages for frameworks
            throw new NotImplementedException();
        }
        return result;
    }

    public async Task<IEnumerable<string>> AddAsync(IEnumerable<string> packageIds, UpstreamClientService upstreamClientService, CancellationToken cancellationToken)
    {
        if (packageIds is null || !(packageIds = packageIds.Select(i => i?.Trim()!).Where(i => !string.IsNullOrEmpty(i))).Any())
            return Enumerable.Empty<string>();
        var findPackageById = await GetFindPackageByIdResourceAsync(cancellationToken);
        var resource = await GetPackageUpdateResourceAsync(cancellationToken);
        return await AddAsync(packageIds, findPackageById, resource, upstreamClientService, cancellationToken);
    }

    private async Task UpdateAsync(string packageId, NuGetVersion version, PackageDownloadContext downloadContext, UpstreamClientService upstreamClientService, Dictionary<string, HashSet<NuGetVersion>> updated, CancellationToken cancellationToken)
    {
        var localPackage = await GetDependencyInfoAsync(packageId, version, cancellationToken);
        if (localPackage is null)
        {
            Logger.LogPackageNotFound(packageId, version, this, false);
            return;
        }
        if (updated.TryGetValue(packageId, out HashSet<NuGetVersion>? versionsUpdated))
        {
            if (versionsUpdated.Contains(version))
                return;
        }
        else
        {
            versionsUpdated = new HashSet<NuGetVersion>(VersionComparer.VersionReleaseMetadata);
            updated.Add(packageId, versionsUpdated);
        }
        versionsUpdated.Add(version);
        var downloaded = await upstreamClientService.GetDownloadResourceResultAsync(new PackageIdentity(packageId, version), downloadContext, GlobalPackagesFolder, cancellationToken);
        if (downloaded is null)
        {
            Logger.LogPackageNotFound(packageId, version, this, true);
            return;
        }
    }

    private async Task UpdateAsync(HashSet<string> toUpdate, FindPackageByIdResource findPackageById, PackageUpdateResource resource, UpstreamClientService upstreamClientService, HashSet<string> deletedIds, CancellationToken cancellationToken)
    {
        Dictionary<string, HashSet<NuGetVersion>> updated = new(StringComparer.CurrentCultureIgnoreCase);
        while (toUpdate.Count > 0)
        {
            var id = toUpdate.First();
            toUpdate.Remove(id);
            if (updated.ContainsKey(id))
                continue;
            HashSet<NuGetVersion> versionsUpdated = new(VersionComparer.VersionReleaseMetadata);
            updated.Add(id, versionsUpdated);
            var lc = id.ToLower();
            using var scope = Logger.BeginUpdateLocalPackageScope(id, PackageSourceLocation);
            var localVersions = await findPackageById.GetAllVersionsAsync(lc, CacheContext, NuGetLogger, cancellationToken);
            if (localVersions is null || !localVersions.Any())
            {
                if (deletedIds.Contains(lc, StringComparer.CurrentCultureIgnoreCase))
                    Logger.LogPackageDeleted(id, PackageSourceLocation);
                else
                    Logger.LogPackageNotFound(id, this, false);
                continue;
            }
            var upstreamVersions = await upstreamClientService.GetAllVersionsAsync(lc, cancellationToken);
            if (upstreamVersions is null || !upstreamVersions.Any())
            {
                Logger.LogPackageNotFound(id, this, true);
                continue;
            }
            if (!(upstreamVersions = upstreamVersions.Where(v => !localVersions.Contains(v, VersionComparer.VersionReleaseMetadata))).Any())
                continue;
            PackageDownloadContext downloadContext = new(CacheContext);

            foreach (NuGetVersion version in upstreamVersions)
            {

            }
        }
    }

    public async Task UpdateAsync(IEnumerable<string> packageIds, UpstreamClientService upstreamClientService, HashSet<string> deletedIds, CancellationToken cancellationToken)
    {
        if (packageIds is null || !(packageIds = packageIds.Select(i => i?.Trim()!).Where(i => !string.IsNullOrEmpty(i))).Any())
            return;
        HashSet<string> toUpdate = new(packageIds.Distinct(StringComparer.CurrentCultureIgnoreCase), StringComparer.CurrentCultureIgnoreCase);
        var findPackageById = await GetFindPackageByIdResourceAsync(cancellationToken);
        var resource = await GetPackageUpdateResourceAsync(cancellationToken);
        await UpdateAsync(toUpdate, findPackageById, resource, upstreamClientService, deletedIds, cancellationToken);
    }

    #endregion
}

