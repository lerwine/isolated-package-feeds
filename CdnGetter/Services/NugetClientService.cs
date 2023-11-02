using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace CdnGetter.Services;

public abstract class NugetClientService : IDisposable
{
    private bool _isDisposed;
    private readonly NugetLogWrapper _nugetLogger;
    private SourceCacheContext _sourceCacheContext = new();

    protected abstract Task<SourceRepository> GetRepositoryAsync();

    protected ILogger<NugetClientService> Logger { get; }

    protected NugetClientService(IOptions<Config.NuGetSettings> options, ILogger<NugetClientService> logger) => _nugetLogger = new(Logger = logger);

    public async Task<(PackageMetadataResource Resource, Uri SourceUri)> GetPackageMetadataResourceAsync(CancellationToken cancellationToken)
    {
        var source = await GetRepositoryAsync();
        return (await source.GetResourceAsync<PackageMetadataResource>(cancellationToken) ?? throw GetNugetResourceException.LogAndCreate<PackageMetadataResource>(Logger), source.PackageSource.SourceUri);
    }

    public async Task<(FindPackageByIdResource Resource, Uri SourceUri)> GetFindPackageByIdResourceAsync(CancellationToken cancellationToken)
    {
        var source = await GetRepositoryAsync();
        return (await source.GetResourceAsync<FindPackageByIdResource>(cancellationToken) ?? throw GetNugetResourceException.LogAndCreate<PackageMetadataResource>(Logger), source.PackageSource.SourceUri);
    }

    public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, CancellationToken cancellationToken) =>
        GetMetadataAsync(packageId, false, false, cancellationToken);
        
    public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, CancellationToken cancellationToken) =>
        GetMetadataAsync(packageId, includePrerelease, false, cancellationToken);
        
    public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, PackageMetadataResource packageMetadataResource, Uri sourceUri, CancellationToken cancellationToken) =>
        GetMetadataAsync(packageId, false, false, packageMetadataResource, sourceUri, cancellationToken);
        
    public async Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, bool includeUnlisted, CancellationToken cancellationToken)
    {
        (PackageMetadataResource packageMetadataResource, Uri sourceUri) = await GetPackageMetadataResourceAsync(cancellationToken);
        return await GetMetadataAsync(packageId, includePrerelease, includeUnlisted, packageMetadataResource, sourceUri, cancellationToken); 
    }
        
    public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, PackageMetadataResource packageMetadataResource, Uri sourceUri, CancellationToken cancellationToken) =>
        GetMetadataAsync(packageId, includePrerelease, false, packageMetadataResource, sourceUri, cancellationToken);
        
    public async Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, bool includeUnlisted, PackageMetadataResource packageMetadataResource, Uri sourceUri, CancellationToken cancellationToken)
    {
        using var scope = Logger.BeginGetNugetMetaDataScope(sourceUri, packageId, includePrerelease, includeUnlisted);
        try { return await packageMetadataResource.GetMetadataAsync(packageId, includePrerelease, includeUnlisted, _sourceCacheContext, _nugetLogger, cancellationToken); }
        catch (Exception exception)
        {
            throw GetNuGetMetaDataException.LogAndCreate(Logger, packageId, sourceUri, includePrerelease, includeUnlisted, exception);
        }
        
    }

    public async Task<IEnumerable<NuGetVersion>> GetAllVersionsAsync(string packageId, CancellationToken cancellationToken)
    {
        (FindPackageByIdResource resource, Uri sourceUri) = await GetFindPackageByIdResourceAsync(cancellationToken);
        return await GetAllVersionsAsync(packageId, resource, sourceUri, cancellationToken);
    }

    public async Task<IEnumerable<NuGetVersion>> GetAllVersionsAsync(string packageId, FindPackageByIdResource resource, Uri sourceUri, CancellationToken cancellationToken)
    {
        using var scope = Logger.BeginGetNuGetPackageVersionsScope(sourceUri, packageId);
        try { return await resource.GetAllVersionsAsync(packageId, _sourceCacheContext, _nugetLogger, cancellationToken); }
        catch (Exception exception)
        {
            throw GetAllNuGetVersionsException.LogAndCreate(Logger, packageId, sourceUri, exception);
        }
    }

    public async Task<FindPackageByIdDependencyInfo> GetDependencyInfoAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken)
    {
        (FindPackageByIdResource resource, Uri sourceUri) = await GetFindPackageByIdResourceAsync(cancellationToken);
        return await GetDependencyInfoAsync(packageId, version, resource, sourceUri, cancellationToken);
    }

    public async Task<FindPackageByIdDependencyInfo> GetDependencyInfoAsync(string packageId, NuGetVersion version, FindPackageByIdResource resource, Uri sourceUri, CancellationToken cancellationToken)
    {
        using var scope = Logger.BeginFindNuGetPackageByIdScope(sourceUri, packageId);
        try { return await resource.GetDependencyInfoAsync(packageId, version, _sourceCacheContext, _nugetLogger, cancellationToken); }
        catch (Exception exception)
        {
            throw GetNuGetDependencyInfoException.LogAndCreate(Logger, packageId, version, sourceUri, exception);
        }
    }

    public async Task<bool> DoesPackageExistAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken)
    {
        (FindPackageByIdResource resource, Uri sourceUri) = await GetFindPackageByIdResourceAsync(cancellationToken);
        return await DoesPackageExistAsync(packageId, version, resource, sourceUri, cancellationToken);
    }

    public async Task<bool> DoesPackageExistAsync(string packageId, NuGetVersion version, FindPackageByIdResource resource, Uri sourceUri, CancellationToken cancellationToken)
    {
        using var scope = Logger.BeginFindNuGetPackageByIdScope(sourceUri, packageId);
        try { return await resource.DoesPackageExistAsync(packageId, version, _sourceCacheContext, _nugetLogger, cancellationToken); }
        catch (Exception exception)
        {
            throw DoesNuGetPackageExistException.LogAndCreate(Logger, packageId, version, sourceUri, exception);
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