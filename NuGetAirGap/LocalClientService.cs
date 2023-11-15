using System;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGetAirGap;

public sealed class LocalClientService : ClientService
{
    private readonly object _syncRoot = new();
    private Task<PackageUpdateResource>? _getPackageUpdateResourceAsync;

    public override bool IsUpstream => false;

    public LocalClientService(IOptions<AppSettings> appSettings, IHostEnvironment hostingEnvironment, ILogger<UpstreamClientService> logger) : base(logger, Task.Run(() =>
    {
        var path = appSettings.Value.UpstreamServiceIndex.DefaultIfWhiteSpace(() => Path.Combine(hostingEnvironment.ContentRootPath, AppSettings.DEFAULT_LOCAL_REPOSITORY));
        using var scope = logger.BeginValidateLocalPathScope(path);
        Uri? uri;
        try
        {
            if (!Uri.TryCreate(path, UriKind.Absolute, out uri))
                uri = new Uri(path, UriKind.Relative);
        }
        catch (UriFormatException error)
        {
            throw logger.LogInvalidRepositoryUrl(path, false, message => new InvalidRepositoryUrlException(path, false, message, error), error);
        }
        catch (ArgumentException error)
        {
            throw logger.LogInvalidRepositoryUrl(path, false, message => new InvalidRepositoryUrlException(path, false, error), error);
        }
        DirectoryInfo directoryInfo;
        if (uri.IsAbsoluteUri && !uri.IsFile)
        {
            if (!uri.IsFile)
                throw logger.LogInvalidRepositoryUrl(uri, false, message => new InvalidRepositoryUrlException(uri.AbsoluteUri, false, message));
            path = uri.LocalPath;
        }
        try { path = (directoryInfo = new(path)).FullName; }
        catch (System.Security.SecurityException error)
        {
            throw logger.LogRepositorySecurityException(path, false, message => new RepositorySecurityException(path, false, message, error), error);
        }
        catch (PathTooLongException error)
        {
            throw logger.LogInvalidRepositoryUrl(path, false, message => new InvalidRepositoryUrlException(path, false, error), error);
        }
        catch (ArgumentException error)
        {
            throw logger.LogInvalidRepositoryUrl(path, false, message => new InvalidRepositoryUrlException(path, false, error), error);
        }
        if (!directoryInfo.Exists)
        {
            if (directoryInfo.Parent is not null && directoryInfo.Parent.Exists && !File.Exists(directoryInfo.FullName))
                try { directoryInfo.Create(); }
                catch (DirectoryNotFoundException exception)
                {
                    throw logger.LogRepositoryPathNotFound(path, false, message => new RepositoryPathNotFoundException(path, false, message, exception), exception);
                }
                catch (IOException exception)
                {
                    throw logger.LogLocalRepositoryIOException(path, message => new LocalRepositoryIOExceptionException(path, message, exception), exception);
                }
                catch (System.Security.SecurityException exception)
                {
                    throw logger.LogRepositorySecurityException(path, false, message => new RepositorySecurityException(path, false, message, exception), exception);
                }
            else
                throw logger.LogRepositoryPathNotFound(path, false, message => new RepositoryPathNotFoundException(path, false, message));
        }
        return Repository.Factory.GetCoreV3(path);
    })) { }

    #region Methods using the Search Query API

    private Task<(PackageSearchResource Resource, string URL)>? _getPackageSearchResourceAsync;

    private async Task<T> WithPackageSearchResourceScopeAsync<T>(Func<string, IDisposable?> scopeFactory, Func<PackageSearchResource, CancellationToken, Task<T>> asyncFunc, CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _getPackageSearchResourceAsync ??= WithSourceRepositoryAsync(async (s, c) => (await s.GetResourceAsync<PackageSearchResource>(cancellationToken), s.PackageSource.SourceUri.OriginalString), cancellationToken);
        (var resource, var uri) = await _getPackageSearchResourceAsync;
        using var scope = scopeFactory(uri);
        return await asyncFunc(resource, cancellationToken);
    }

    public async Task<List<IPackageSearchMetadata>> GetAllPackagesAsync(CancellationToken cancellationToken) => await WithPackageSearchResourceScopeAsync(uri => Logger.BeginGetAllLocalPackagesScope(uri),
        async (resource, token) =>
        {
            List<IPackageSearchMetadata> result = new();
            var skip = 0;
            IPackageSearchMetadata[] items;
            while ((items = (await resource.SearchAsync(null, null, skip, 50, NuGetLogger, token)).ToArray()).Length > 0)
            {
                skip += items.Length;
                result.AddRange(items);
            }
            return result;
        }, cancellationToken);

    #endregion

    #region Methods using the NuGet V3 Push and Delete API

    private async Task WithPackageUpdateResourceScopeAsync(Func<PackageUpdateResource, Uri, CancellationToken, Task> asyncAction, CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _getPackageUpdateResourceAsync ??= WithSourceRepositoryAsync(async (s, c) => await s.GetResourceAsync<PackageUpdateResource>(cancellationToken), cancellationToken);
        var resource = await _getPackageUpdateResourceAsync;
        await asyncAction(resource, await GetPackageSourceUriAsync(), cancellationToken);
    }

    private async Task<T> WithPackageUpdateResourceScopeAsync<T>(Func<PackageUpdateResource, Uri, CancellationToken, Task<T>> asyncFunc, CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _getPackageUpdateResourceAsync ??= WithSourceRepositoryAsync(async (s, c) => await s.GetResourceAsync<PackageUpdateResource>(cancellationToken), cancellationToken);
        var resource = await _getPackageUpdateResourceAsync;
        return await asyncFunc(resource, await GetPackageSourceUriAsync(), cancellationToken);
    }

    private async Task<IEnumerable<string>> DeleteAsync(IEnumerable<string> packageIds, FindPackageByIdResource findPackageById, PackageUpdateResource resource, Uri url, CancellationToken cancellationToken)
    {
        HashSet<string> result = new(StringComparer.CurrentCultureIgnoreCase);
        foreach (string id in packageIds.Distinct(StringComparer.CurrentCultureIgnoreCase))
        {
            using var scope = Logger.BeginDeleteLocalPackageScope(id, url.LocalPath);
            var lc = id.ToLower();
            var versions = await findPackageById.GetAllVersionsAsync(lc, CacheContext, NuGetLogger, cancellationToken);
            if (versions is null || !versions.Any())
                continue;
            result.Add(id);
            foreach (var v in versions)
            {
                using var scope2 = Logger.BeginDeleteLocalPackageVersionScope(lc, v, url.LocalPath);
                await resource.Delete(lc, v.ToString(), s => string.Empty, s => true, true, NuGetLogger);
            }
        }
        return result;
    }

    public async Task<IEnumerable<string>> DeleteAsync(IEnumerable<string> packageIds, CancellationToken cancellationToken)
    {
        if (packageIds is null || !(packageIds = packageIds.Select(i => i?.Trim()!).Where(i => !string.IsNullOrEmpty(i))).Any())
            return Enumerable.Empty<string>();
        return await WithFindPackageByIdResourceScopeAsync<IEnumerable<string>>(async (findPackageById, token) =>
            await WithPackageUpdateResourceScopeAsync<IEnumerable<string>>(async (resource, url, t) =>
                await DeleteAsync(packageIds, findPackageById, resource, url, t), token), cancellationToken);
    }
    
    private async Task<IEnumerable<string>> AddAsync(IEnumerable<string> packageIds, FindPackageByIdResource findPackageById, PackageUpdateResource resource, UpstreamClientService upstreamClientService, Uri url, CancellationToken cancellationToken)
    {
        HashSet<string> result = new(StringComparer.CurrentCultureIgnoreCase);
        foreach (string id in packageIds.Distinct(StringComparer.CurrentCultureIgnoreCase))
        {
            using var scope = Logger.BeginAddLocalPackageScope(id, url.LocalPath);
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
        return await WithFindPackageByIdResourceScopeAsync(async (findPackageById, token) =>
            await WithPackageUpdateResourceScopeAsync(async (resource, url, t) =>
                await AddAsync(packageIds, findPackageById, resource, upstreamClientService, url, cancellationToken), token), cancellationToken);
    }
    
    private async Task UpdateAsync(string packageId, PackageDownloadContext downloadContext, NuGetVersion version, UpstreamClientService upstreamClientService, Uri upstreamUri, Uri localUri, Dictionary<string, HashSet<NuGetVersion>> updated, CancellationToken cancellationToken)
    {
        // PackageDownloadContext downloadContext = new PackageDownloadContext(CacheContext);
        var localPackage = await GetDependencyInfoAsync(packageId, version, cancellationToken);
        if (localPackage is null)
        {
            Logger.LogPackageNotFound(packageId, version, localUri, false);
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
        var downloaded = await upstreamClientService.GetDownloadResourceResultAsync(new PackageIdentity(packageId, version), downloadContext, null, cancellationToken);
        if (downloaded is null)
        {
            Logger.LogPackageNotFound(packageId, version, upstreamUri, true);
            return;
        }
    }

    private async Task UpdateAsync(HashSet<string> toUpdate, FindPackageByIdResource findPackageById, PackageUpdateResource resource, UpstreamClientService upstreamClientService, HashSet<string> deletedIds, Uri localUrl, CancellationToken cancellationToken)
    {
        Dictionary<string, HashSet<NuGetVersion>> updated = new(StringComparer.CurrentCultureIgnoreCase);
        Uri upstreamUri = await upstreamClientService.GetPackageSourceUriAsync();
        while (toUpdate.Count > 0)
        {
            var id = toUpdate.First();
            toUpdate.Remove(id);
            if (updated.ContainsKey(id))
                continue;
            HashSet<NuGetVersion> versionsUpdated = new(VersionComparer.VersionReleaseMetadata);
            updated.Add(id, versionsUpdated);
            var lc = id.ToLower();
            using var scope = Logger.BeginUpdateLocalPackageScope(id, localUrl.LocalPath);
            var localVersions = await findPackageById.GetAllVersionsAsync(lc, CacheContext, NuGetLogger, cancellationToken);
            if (localVersions is null || !localVersions.Any())
            {
                if (deletedIds.Contains(lc, StringComparer.CurrentCultureIgnoreCase))
                    Logger.LogPackageDeleted(id, localUrl.LocalPath);
                else
                    Logger.LogPackageNotFound(id, localUrl, false);
                continue;
            }
            var upstreamVersions = await upstreamClientService.GetAllVersionsAsync(lc, cancellationToken);
            if (upstreamVersions is null || !upstreamVersions.Any())
            {
                Logger.LogPackageNotFound(id, upstreamUri, true);
                continue;
            }
            if (!(upstreamVersions = upstreamVersions.Where(v => !localVersions.Contains(v, VersionComparer.VersionReleaseMetadata))).Any())
                continue;
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
        await WithFindPackageByIdResourceScopeAsync(async (findPackageById, token) =>
        {
            await WithPackageUpdateResourceScopeAsync(async (resource, url, t) =>
            {
                await UpdateAsync(toUpdate, findPackageById, resource, upstreamClientService, deletedIds, url, t);
            }, token);
        }, cancellationToken);
    }

    #endregion
}

