using Microsoft.Extensions.Logging;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGetAirGap;

public abstract class ClientService : IDisposable
{
    private bool _isDisposed;
    private readonly object _syncRoot = new();
    private readonly NugetLogWrapper _nugetLogger;
    private readonly Task<SourceRepository> _getSourceReposityAsync;
    private Task<(DownloadResource Resource, string URL)>? _getDownloadResourceAsync;
    private Task<(PackageMetadataResource Resource, string URL)>? _getPackageMetadataResourceAsync;
    private Task<(FindPackageByIdResource Resource, string URL)>? _getFindPackageByIdResourceAsync;
    private Task<(DependencyInfoResource Resource, string URL)>? _getDependencyInfoResourceAsync;
    private readonly SourceCacheContext _sourceCacheContext = new();

    public abstract bool IsUpstream { get; }

    protected ILogger Logger { get; }

    protected ClientService(ILogger logger, Task<SourceRepository> getSourceReposityAsync) =>
        (_nugetLogger, _getSourceReposityAsync) = (new(Logger = logger), getSourceReposityAsync);

    private async Task<T> WithSourceRepositoryAsync<T>(Func<SourceRepository, CancellationToken, Task<T>> asyncFunc, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SourceRepository sourceRepository = await _getSourceReposityAsync;
        return await asyncFunc(sourceRepository, cancellationToken);
    }

    #region DownloadResource methods

    private async Task<T> WithDownloadResourceScopeAsync<T>(Func<string, IDisposable?> scopeFactory, Func<DownloadResource, CancellationToken, Task<T>> asyncFunc, CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _getDownloadResourceAsync ??= WithSourceRepositoryAsync(async (s, c) => (await s.GetResourceAsync<DownloadResource>(cancellationToken), s.PackageSource.SourceUri.OriginalString), cancellationToken);
        (var resource, var uri) = await _getDownloadResourceAsync;
        using var scope = scopeFactory(uri);
        return await asyncFunc(resource, cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="identity">Identifies the package to be downloaded.</param>
    /// <param name="downloadContext">Specifies how package is to be downloaded.</param>
    /// <param name="globalPackagesFolder">Local folder containing global packages.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns></returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/DownloadResourceV3.cs#L126"/>
    public Task<DownloadResourceResult> GetDownloadResourceResultAsync(PackageIdentity identity, PackageDownloadContext downloadContext, string globalPackagesFolder, CancellationToken cancellationToken) =>
        WithDownloadResourceScopeAsync(uri => Logger.BeginGetDownloadResourceResultScope(identity, downloadContext, globalPackagesFolder, uri, IsUpstream), (resource, token) =>
            resource.GetDownloadResourceResultAsync(identity, downloadContext, globalPackagesFolder, _nugetLogger, token), cancellationToken);

    #endregion
    
    #region PackageMetadataResource methods

    private async Task<T> WithPackageMetadataResourceScopeAsync<T>(Func<string, IDisposable?> scopeFactory, Func<PackageMetadataResource, CancellationToken, Task<T>> asyncFunc, CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _getPackageMetadataResourceAsync ??= WithSourceRepositoryAsync(async (s, c) => (await s.GetResourceAsync<PackageMetadataResource>(cancellationToken), s.PackageSource.SourceUri.OriginalString), cancellationToken);
        (var resource, var uri) = await _getPackageMetadataResourceAsync;
        using var scope = scopeFactory(uri);
        return await asyncFunc(resource, cancellationToken);
    }

    /// <summary>
    /// Gets metadata for a specified package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns a list of package meta data objects.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageMetadataResourceV3.cs#L46"/>
    public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, CancellationToken cancellationToken) =>
        GetMetadataAsync(packageId, false, false, cancellationToken);

    /// <summary>
    /// Gets metadata for a specified package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="includePrerelease">Whether to include PreRelease versions into result.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns a list of package meta data objects.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageMetadataResourceV3.cs#L46"/>
    public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, CancellationToken cancellationToken) =>
        GetMetadataAsync(packageId, includePrerelease, false, cancellationToken);

    /// <summary>
    /// Gets metadata for a specified package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="includePrerelease">Whether to include PreRelease versions into result.</param>
    /// <param name="includeUnlisted">Whether to include Unlisted versions into result.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns a list of package meta data objects.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageMetadataResourceV3.cs#L46"/>
    public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, bool includeUnlisted, CancellationToken cancellationToken) =>
        WithPackageMetadataResourceScopeAsync(uri => Logger.BeginGetMetadataAsyncScope(packageId, includePrerelease, includeUnlisted, uri, IsUpstream), (resource, token) =>
            resource.GetMetadataAsync(packageId.ToLower(), includePrerelease, includeUnlisted, _sourceCacheContext, _nugetLogger, token), cancellationToken);

    /// <summary>
    /// Gets metadata for a specified package version.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the package meta data object or <see langword="null"/> if not found.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageMetadataResourceV3.cs#L66"/>
    public Task<IPackageSearchMetadata?> GetMetadataAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken) =>
        WithPackageMetadataResourceScopeAsync(uri => Logger.BeginGetMetadataAsyncScope(packageId, version, uri, IsUpstream), (resource, token) =>
            resource.GetMetadataAsync(new PackageIdentity(packageId.ToLower(), version), _sourceCacheContext, _nugetLogger, token), cancellationToken);

    #endregion
    
    #region FindPackageByIdResource methods

    private async Task<T> WithFindPackageByIdResourceScopeAsync<T>(Func<string, IDisposable?> scopeFactory, Func<FindPackageByIdResource, CancellationToken, Task<T>> asyncFunc, CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _getFindPackageByIdResourceAsync ??= WithSourceRepositoryAsync(async (s, c) => (await s.GetResourceAsync<FindPackageByIdResource>(cancellationToken), s.PackageSource.SourceUri.OriginalString), cancellationToken);
        (var resource, var uri) = await _getFindPackageByIdResourceAsync;
        using var scope = scopeFactory(uri);
        return await asyncFunc(resource, cancellationToken);
    }

    /// <summary>
    /// Asynchronously gets all package versions for a package ID.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="resource">The resource capable of fetching package versions.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the package versions for the specified <paramref name="packageId"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="packageId" /> is either <c>null</c> or an empty string.</exception>
    /// <exception cref="OperationCanceledException">Thrown if <paramref name="cancellationToken" /> is cancelled.</exception>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/package-base-address-resource#enumerate-package-versions"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/FindPackageByIdResource.cs#L37"/>
    public Task<IEnumerable<NuGetVersion>> GetAllVersionsAsync(string packageId, CancellationToken cancellationToken) =>
        WithFindPackageByIdResourceScopeAsync(uri => Logger.BeginGetAllVersionsScope(packageId, uri, IsUpstream), (resource, token) =>
            resource.GetAllVersionsAsync(packageId.ToLower(), _sourceCacheContext, _nugetLogger, token), cancellationToken);

    /// <summary>
    /// Asynchronously gets dependency information for a specific package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns dependency information for the specified <paramref name="packageId"/> and <paramref name="version"/> or <see langword="null"/> if not found.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="packageId" /> is either <c>null</c> or an empty string.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="version" /> <c>null</c>.</exception>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/FindPackageByIdResource.cs#L61"/>
    public Task<FindPackageByIdDependencyInfo?> GetDependencyInfoAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken) =>
        WithFindPackageByIdResourceScopeAsync(uri => Logger.BeginGetDependencyInfoScope(packageId, version, uri, IsUpstream), (resource, token) =>
            resource.GetDependencyInfoAsync(packageId.ToLower(), version, _sourceCacheContext, _nugetLogger, cancellationToken), cancellationToken);

    /// <summary>
    /// Asynchronously checks whether the exact package id and version exists at this source.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns <see langword="true"/> if the exact package id and version exists at this source; otherwise, <see langword="true"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="id" />
    /// is either <c>null</c> or an empty string.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="version" /> <c>null</c>.</exception>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/FindPackageByIdResource.cs#L134"/>
    public Task<bool> DoesPackageExistAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken) =>
        WithFindPackageByIdResourceScopeAsync(uri => Logger.BeginDoesPackageExistScope(packageId, version, uri, IsUpstream), (resource, token) =>
            resource.DoesPackageExistAsync(packageId.ToLower(), version, _sourceCacheContext, _nugetLogger, cancellationToken), cancellationToken);

    #endregion
    
    #region DependencyInfoResource methods

    private async Task<T> WithDependencyInfoResourceScopeAsync<T>(Func<string, IDisposable?> scopeFactory, Func<DependencyInfoResource, CancellationToken, Task<T>> asyncFunc, CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _getDependencyInfoResourceAsync ??= WithSourceRepositoryAsync(async (s, c) => (await s.GetResourceAsync<DependencyInfoResource>(cancellationToken), s.PackageSource.SourceUri.OriginalString), cancellationToken);
        (var resource, var uri) = await _getDependencyInfoResourceAsync;
        using var scope = scopeFactory(uri);
        return await asyncFunc(resource, cancellationToken);
    }

    private async Task GetPackageDependenciesAsync(PackageIdentity package, NuGetFramework framework, DependencyInfoResource dependencyInfoResource, ISet<SourcePackageDependencyInfo> availablePackages, CancellationToken cancellationToken)
    {
        if (availablePackages.Contains(package)) return;

        var dependencyInfo = await dependencyInfoResource.ResolvePackage(package, framework, _sourceCacheContext, _nugetLogger, cancellationToken);

        if (dependencyInfo == null) return;

        availablePackages.Add(dependencyInfo);
        foreach (var dependency in dependencyInfo.Dependencies)
            await GetPackageDependenciesAsync(new(dependency.Id, dependency.VersionRange.MinVersion), framework, dependencyInfoResource, availablePackages, cancellationToken);
    }

    public Task<HashSet<SourcePackageDependencyInfo>> GetPackageDependenciesAsync(string packageId, NuGetVersion version, NuGetFramework framework, CancellationToken cancellationToken) =>
        WithDependencyInfoResourceScopeAsync(uri => Logger.BeginGetPackageDependenciesScope(packageId, version, framework, uri, IsUpstream), async (resource, token) =>
        {
            PackageIdentity package = new(packageId, version);
            var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
            var dependencyInfo = await resource.ResolvePackage(package, framework, _sourceCacheContext, _nugetLogger, cancellationToken);
            if (dependencyInfo == null) return availablePackages;
            availablePackages.Add(dependencyInfo);
            foreach (var dependency in dependencyInfo.Dependencies)
                await GetPackageDependenciesAsync(new(dependency.Id, dependency.VersionRange.MinVersion), framework, resource, availablePackages, cancellationToken);
            availablePackages.Remove(dependencyInfo);
            return availablePackages;
        }, cancellationToken);

    #endregion
    
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;
        if (disposing)
            _sourceCacheContext.Dispose();
        _isDisposed = true;
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
