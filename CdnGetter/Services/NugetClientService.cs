using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace CdnGetter.Services;

public abstract class NugetClientService : IDisposable
{
    private bool _isDisposed;
    private readonly NugetLogWrapper _nugetLogger;
    private readonly SourceCacheContext _sourceCacheContext = new();

    protected abstract LazyTransform<string, SourceRepository> SourceRepo { get; }

    protected ILogger<NugetClientService> Logger { get; }

    protected NugetClientService(ILogger<NugetClientService> logger) => _nugetLogger = new(Logger = logger);

    public async Task<ListResource> GetListResourceAsync(CancellationToken cancellationToken)
    {
        var source = SourceRepo.GetOutput();
        return await source.GetResourceAsync<ListResource>(cancellationToken) ?? throw GetNugetResourceException.LogAndCreate<ListResource>(Logger);
    }

    public async Task<IEnumerableAsync<IPackageSearchMetadata>> ListAsync(string searchTerm, bool prerelease, bool allVersions, bool includeDelisted, ListResource resource, CancellationToken cancellationToken)
    {
        using var scope = Logger.BeginListNuGetPackagesScope(SourceRepo.GetInput(), searchTerm, prerelease, allVersions, includeDelisted);
        try { return await resource.ListAsync(searchTerm, prerelease, allVersions, includeDelisted, _nugetLogger, cancellationToken); }
        catch (Exception exception)
        {
            throw ListNuGetPackagesException.LogAndCreate(Logger, searchTerm, SourceRepo.Input, prerelease, allVersions, includeDelisted, exception);
        }
    }

    public async Task<DownloadResource> GetDownloadResourceAsync(CancellationToken cancellationToken)
    {
        var source = SourceRepo.GetOutput();
        return await source.GetResourceAsync<DownloadResource>(cancellationToken) ?? throw GetNugetResourceException.LogAndCreate<DownloadResource>(Logger);
    }

    public async Task<DownloadResourceResult> GetDownloadResourceResultAsync(PackageIdentity identity, PackageDownloadContext downloadContext, string globalPackagesFolder, DownloadResource resource, CancellationToken cancellationToken)
    {
        using var scope = Logger.BeginGetNuGetDownloadResourceResultScope(identity, SourceRepo.GetInput(), downloadContext, globalPackagesFolder);
        try { return await resource.GetDownloadResourceResultAsync(identity, downloadContext, globalPackagesFolder, _nugetLogger, cancellationToken); }
        catch (Exception exception)
        {
            throw GetNuGetDownloadResourceResultsException.LogAndCreate(Logger, identity, SourceRepo.Input, downloadContext, globalPackagesFolder, exception);
        }
    }

    public async Task<PackageSearchResource> GetPackageSearchResourceAsync(CancellationToken cancellationToken)
    {
        var source = SourceRepo.GetOutput();
        return await source.GetResourceAsync<PackageSearchResource>(cancellationToken) ?? throw GetNugetResourceException.LogAndCreate<PackageSearchResource>(Logger);
    }

    public async Task<IEnumerable<IPackageSearchMetadata>> SearchAsync(string searchTerm, SearchFilter filters, int skip, int take, PackageSearchResource resource, CancellationToken cancellationToken)
    {
        using var scope = Logger.BeginSearchNuGetPackageScope(searchTerm, SourceRepo.GetInput(), filters, skip, take);
        try { return await resource.SearchAsync(searchTerm, filters, skip, take, _nugetLogger, cancellationToken); }
        catch (Exception exception)
        {
            throw NuGetPackageSearchException.LogAndCreate(Logger, searchTerm, SourceRepo.Input, filters, skip, take, exception);
        }
    }

    public async Task<PackageMetadataResource> GetPackageMetadataResourceAsync(CancellationToken cancellationToken)
    {
        var source = SourceRepo.GetOutput();
        return await source.GetResourceAsync<PackageMetadataResource>(cancellationToken) ?? throw GetNugetResourceException.LogAndCreate<PackageMetadataResource>(Logger);
    }

    public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, CancellationToken cancellationToken) =>
        GetMetadataAsync(packageId, false, false, cancellationToken);
        
    public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, CancellationToken cancellationToken) =>
        GetMetadataAsync(packageId, includePrerelease, false, cancellationToken);
        
    public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, PackageMetadataResource packageMetadataResource, CancellationToken cancellationToken) =>
        GetMetadataAsync(packageId, false, false, packageMetadataResource, cancellationToken);
        
    public async Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, bool includeUnlisted, CancellationToken cancellationToken)
    {
        PackageMetadataResource packageMetadataResource = await GetPackageMetadataResourceAsync(cancellationToken);
        return await GetMetadataAsync(packageId, includePrerelease, includeUnlisted, packageMetadataResource, cancellationToken); 
    }
        
    public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, PackageMetadataResource packageMetadataResource, CancellationToken cancellationToken) =>
        GetMetadataAsync(packageId, includePrerelease, false, packageMetadataResource, cancellationToken);
        
    public async Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, bool includeUnlisted, PackageMetadataResource packageMetadataResource, CancellationToken cancellationToken)
    {
        using var scope = Logger.BeginGetNugetMetaDataScope(SourceRepo.GetInput(), packageId, includePrerelease, includeUnlisted);
        try { return await packageMetadataResource.GetMetadataAsync(packageId, includePrerelease, includeUnlisted, _sourceCacheContext, _nugetLogger, cancellationToken); }
        catch (Exception exception)
        {
            throw GetNuGetMetaDataException.LogAndCreate(Logger, packageId, SourceRepo.Input, includePrerelease, includeUnlisted, exception);
        }
    }

    public async Task<FindPackageByIdResource> GetFindPackageByIdResourceAsync(CancellationToken cancellationToken)
    {
        var source = SourceRepo.GetOutput();
        return await source.GetResourceAsync<FindPackageByIdResource>(cancellationToken) ?? throw GetNugetResourceException.LogAndCreate<PackageMetadataResource>(Logger);
    }

    public async Task<IEnumerable<NuGetVersion>> GetAllVersionsAsync(string packageId, CancellationToken cancellationToken)
    {
        FindPackageByIdResource resource = await GetFindPackageByIdResourceAsync(cancellationToken);
        return await GetAllVersionsAsync(packageId, resource, cancellationToken);
    }

    public async Task<IEnumerable<NuGetVersion>> GetAllVersionsAsync(string packageId, FindPackageByIdResource resource, CancellationToken cancellationToken)
    {
        using var scope = Logger.BeginGetNuGetPackageVersionsScope(SourceRepo.GetInput(), packageId);
        try { return await resource.GetAllVersionsAsync(packageId, _sourceCacheContext, _nugetLogger, cancellationToken); }
        catch (Exception exception)
        {
            throw GetAllNuGetVersionsException.LogAndCreate(Logger, packageId, SourceRepo.Input, exception);
        }
    }

    public async Task<FindPackageByIdDependencyInfo> GetDependencyInfoAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken)
    {
        FindPackageByIdResource resource = await GetFindPackageByIdResourceAsync(cancellationToken);
        return await GetDependencyInfoAsync(packageId, version, resource, cancellationToken);
    }

    public async Task<FindPackageByIdDependencyInfo> GetDependencyInfoAsync(string packageId, NuGetVersion version, FindPackageByIdResource resource, CancellationToken cancellationToken)
    {
        using var scope = Logger.BeginFindNuGetPackageByIdScope(SourceRepo.GetInput(), packageId);
        try { return await resource.GetDependencyInfoAsync(packageId, version, _sourceCacheContext, _nugetLogger, cancellationToken); }
        catch (Exception exception)
        {
            throw GetNuGetDependencyInfoException.LogAndCreate(Logger, packageId, version, SourceRepo.Input, exception);
        }
    }

    public async Task<bool> DoesPackageExistAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken)
    {
        FindPackageByIdResource resource = await GetFindPackageByIdResourceAsync(cancellationToken);
        return await DoesPackageExistAsync(packageId, version, resource, cancellationToken);
    }

    public async Task<bool> DoesPackageExistAsync(string packageId, NuGetVersion version, FindPackageByIdResource resource, CancellationToken cancellationToken)
    {
        using var scope = Logger.BeginFindNuGetPackageByIdScope(SourceRepo.GetInput(), packageId);
        try { return await resource.DoesPackageExistAsync(packageId, version, _sourceCacheContext, _nugetLogger, cancellationToken); }
        catch (Exception exception)
        {
            throw DoesNuGetPackageExistException.LogAndCreate(Logger, packageId, version, SourceRepo.Input, exception);
        }
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
                _sourceCacheContext.Dispose();
            _isDisposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}