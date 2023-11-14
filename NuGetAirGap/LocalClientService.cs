using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGetAirGap;

public sealed class LocalClientService : ClientService
{
    private readonly object _syncRoot = new();
    private Task<(PackageUpdateResource Resource, string URL)>? _getPackageUpdateResourceAsync;

    public override bool IsUpstream => false;

    public LocalClientService(IOptions<AppSettings> appSettings, IHostEnvironment hostingEnvironment, ILogger<UpstreamClientService> logger) : base(logger, Task.Run(() =>
    {
        var path = appSettings.Value.ServiceIndexUrl.DefaultIfWhiteSpace(() => AppSettings.DEFAULT_LOCAL_REPOSITORY);
        using var scope = logger.BeginValidateLocalPathScope(path);
        Uri? uri;
        try
        {
            if (!Uri.TryCreate(path, UriKind.Absolute, out uri))
                uri = new Uri(path, UriKind.Absolute);
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
        if (uri.IsAbsoluteUri)
        {
            if (!uri.IsFile)
                throw logger.LogInvalidRepositoryUrl(uri, false, message => new InvalidRepositoryUrlException(uri.AbsoluteUri, false, message));
            path = uri.LocalPath;
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
        }
        else
            try { path = (directoryInfo = new(Path.IsPathFullyQualified(path) ? path : Path.Combine(hostingEnvironment.ContentRootPath, path))).FullName; }
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
        if (directoryInfo.Exists)
            return Repository.Factory.GetCoreV3(path);
        throw logger.LogRepositoryPathNotFound(path, false, message => new RepositoryPathNotFoundException(path, false, message));
    })) { }

    #region Methods using the NuGet V3 Push and Delete API

    private async Task WithPackageUpdateResourceScopeAsync(Func<PackageUpdateResource, CancellationToken, Task> asyncAction, CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _getPackageUpdateResourceAsync ??= WithSourceRepositoryAsync(async (s, c) => (await s.GetResourceAsync<PackageUpdateResource>(cancellationToken), s.PackageSource.SourceUri.OriginalString), cancellationToken);
        (var resource, var uri) = await _getPackageUpdateResourceAsync;
        await asyncAction(resource, cancellationToken);
    }

    private async Task<T> WithPackageUpdateResourceScopeAsync<T>(Func<string, IDisposable?> scopeFactory, Func<PackageUpdateResource, CancellationToken, Task<T>> asyncFunc, CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _getPackageUpdateResourceAsync ??= WithSourceRepositoryAsync(async (s, c) => (await s.GetResourceAsync<PackageUpdateResource>(cancellationToken), s.PackageSource.SourceUri.OriginalString), cancellationToken);
        (var resource, var uri) = await _getPackageUpdateResourceAsync;
        using var scope = scopeFactory(uri);
        return await asyncFunc(resource, cancellationToken);
    }

    private async Task<T> WithPackageUpdateResourceScopeAsync<T>(Func<PackageUpdateResource, CancellationToken, Task<T>> asyncFunc, CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _getPackageUpdateResourceAsync ??= WithSourceRepositoryAsync(async (s, c) => (await s.GetResourceAsync<PackageUpdateResource>(cancellationToken), s.PackageSource.SourceUri.OriginalString), cancellationToken);
        (var resource, var uri) = await _getPackageUpdateResourceAsync;
        return await asyncFunc(resource, cancellationToken);
    }

    
    #endregion

    private async Task<IEnumerable<string>> DeleteAsync(IEnumerable<string> packageIds, FindPackageByIdResource findPackageById, PackageUpdateResource resource, CancellationToken cancellationToken)
    {
        HashSet<string> result = new(StringComparer.CurrentCultureIgnoreCase);
        foreach (string id in packageIds.Distinct(StringComparer.CurrentCultureIgnoreCase))
        {
            using var scope = Logger.BeginDeleteLocalPackageScope(id);
            var lc = id.ToLower();
            var versions = await findPackageById.GetAllVersionsAsync(lc, CacheContext, NuGetLogger, cancellationToken);
            if (versions is null || !versions.Any())
                continue;
            result.Add(id);
            foreach (var v in versions)
            {
                using var scope2 = Logger.BeginDeleteLocalPackageVersionScope(lc, v);
                await resource.Delete(lc, v.ToString(), s => string.Empty, s => true, true, NuGetLogger);
            }
        }
        return result;
    }

    public async Task<IEnumerable<string>> DeleteAsync(IEnumerable<string> packageIds, CancellationToken cancellationToken)
    {
        if (packageIds is null || !(packageIds = packageIds.Select(i => i?.Trim()!).Where(i => !string.IsNullOrEmpty(i))).Any())
            return Enumerable.Empty<string>();
        return await WithFindPackageByIdResourceScopeAsync(async (findPackageById, token) =>
            await WithPackageUpdateResourceScopeAsync(async (resource, t) =>
                await DeleteAsync(packageIds, findPackageById, resource, t), token), cancellationToken);
    }
    
    private async Task<IEnumerable<string>> AddAsync(IEnumerable<string> packageIds, FindPackageByIdResource findPackageById, PackageUpdateResource resource, UpstreamClientService upstreamClientService, CancellationToken cancellationToken)
    {
        HashSet<string> result = new(StringComparer.CurrentCultureIgnoreCase);
        foreach (string id in packageIds.Distinct(StringComparer.CurrentCultureIgnoreCase))
        {
            using var scope = Logger.BeginAddLocalPackageScope(id);
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
            await WithPackageUpdateResourceScopeAsync(async (resource, t) =>
                await AddAsync(packageIds, findPackageById, resource, upstreamClientService, cancellationToken), token), cancellationToken);
    }
}

