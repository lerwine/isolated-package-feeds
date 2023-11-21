using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGetAirGap;

public abstract class ClientService : IDisposable
{
    private bool _isDisposed;
    private readonly object _syncRoot = new();
    private Task<DownloadResource>? _getDownloadResourceAsync;
    private Task<PackageMetadataResource>? _getPackageMetadataResourceAsync;
    private Task<FindPackageByIdResource>? _getFindPackageByIdResourceAsync;
    private Task<DependencyInfoResource>? _getDependencyInfoResourceAsync;

    protected string GlobalPackagesFolder { get; }

    protected NugetLogWrapper NuGetLogger { get; }

    protected SourceCacheContext CacheContext { get; } = new();

    public bool IsUpstream { get; }

    protected ILogger Logger { get; }

    protected SourceRepository SourceRepository { get; }

    public Uri PackageSourceUri => SourceRepository.PackageSource.SourceUri;

    public string PackageSourceLocation => SourceRepository.PackageSource.Source;

    protected ClientService(SourceRepository sourceRepository, IOptions<AppSettings> options, ILogger logger, bool isUpstream)
    {
        GlobalPackagesFolder = options.Value.Validated.GlobalPackagesFolderPath;
        NuGetLogger = new(Logger = logger);
        SourceRepository = sourceRepository;
        IsUpstream = isUpstream;
    }

    public sealed class ContextScope<T> : IDisposable
    {
        public ContextScope(T context, IDisposable? scope)
        {
            Context = context;
            _scope = scope;
        }
        private readonly IDisposable? _scope;
        public T Context { get; }

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }

    #region DownloadResource methods

    protected async Task<DownloadResource> GetDownloadResourceAsync(CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _getDownloadResourceAsync ??= SourceRepository.GetResourceAsync<DownloadResource>(cancellationToken);
        return await _getDownloadResourceAsync;
    }

    protected async Task<ContextScope<DownloadResource>> GetDownloadResourceScopeAsync(Func<IDisposable?> scopeFactory, CancellationToken cancellationToken) =>
        new(await GetDownloadResourceAsync(cancellationToken), scopeFactory());

    /// <summary>
    /// Gets the downloaded resource result.
    /// </summary>
    /// <param name="identity">Identifies the package to be downloaded.</param>
    /// <param name="downloadContext">Specifies how package is to be downloaded.</param>
    /// <param name="globalPackagesFolder">Local folder containing global packages.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>The downloaded resource result.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/DownloadResourceV3.cs#L126"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalDownloadResource.cs#L38"/>
    public async Task<DownloadResourceResult> GetDownloadResourceResultAsync(PackageIdentity identity, PackageDownloadContext downloadContext, string globalPackagesFolder, CancellationToken cancellationToken)
    {
        using var scope = await GetDownloadResourceScopeAsync(() => Logger.BeginGetDownloadResourceResultScope(identity, downloadContext, globalPackagesFolder, this, IsUpstream), cancellationToken);
        return await scope.Context.GetDownloadResourceResultAsync(identity, downloadContext, globalPackagesFolder, NuGetLogger, cancellationToken);
    }

    #endregion

    #region Methods using the PackageUpdateResource API.

    protected async Task<PackageMetadataResource> GetPackageMetadataResourceAsync(CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _getPackageMetadataResourceAsync ??= SourceRepository.GetResourceAsync<PackageMetadataResource>(cancellationToken);
        return await _getPackageMetadataResourceAsync;
    }

    protected async Task<ContextScope<PackageMetadataResource>> GePackageMetadataResourceScopeAsync(Func<IDisposable?> scopeFactory, CancellationToken cancellationToken) =>
        new(await GetPackageMetadataResourceAsync(cancellationToken), scopeFactory());

    /// <summary>
    /// Gets metadata for a specified package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns a list of package meta data objects.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageMetadataResourceV3.cs#L46"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalPackageMetadataResource.cs#L29"/>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/registration-base-url-resource#registration-page"/>
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
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalPackageMetadataResource.cs#L29"/>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/registration-base-url-resource#registration-page"/>
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
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalPackageMetadataResource.cs#L29"/>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/registration-base-url-resource#registration-page"/>
    public async Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, bool includeUnlisted, CancellationToken cancellationToken)
    {
        using var scope = await GePackageMetadataResourceScopeAsync(() => Logger.BeginGetMetadataScope(packageId, includePrerelease, includeUnlisted, this, IsUpstream), cancellationToken);
        return await scope.Context.GetMetadataAsync(packageId.ToLower(), includePrerelease, includeUnlisted, CacheContext, NuGetLogger, cancellationToken);
    }

    /// <summary>
    /// Gets metadata for a specified package version.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the package meta data object or <see langword="null"/> if not found.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageMetadataResourceV3.cs#L66"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalPackageMetadataResource.cs#L51"/>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/registration-base-url-resource#registration-page"/>
    public async Task<IPackageSearchMetadata?> GetMetadataAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken)
    {
        using var scope = await GePackageMetadataResourceScopeAsync(() => Logger.BeginGetMetadataScope(packageId, version, this, IsUpstream), cancellationToken);
        return await scope.Context.GetMetadataAsync(new PackageIdentity(packageId.ToLower(), version), CacheContext, NuGetLogger, cancellationToken);
    }

    #endregion

    #region Methods using the NuGet V3 Package Content API

    protected async Task<FindPackageByIdResource> GetFindPackageByIdResourceAsync(CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _getFindPackageByIdResourceAsync ??= SourceRepository.GetResourceAsync<FindPackageByIdResource>(cancellationToken);
        return await _getFindPackageByIdResourceAsync;
    }

    protected async Task<ContextScope<FindPackageByIdResource>> GetFindPackageByIdResourceScopeAsync(Func<IDisposable?> scopeFactory, CancellationToken cancellationToken) =>
        new(await GetFindPackageByIdResourceAsync(cancellationToken), scopeFactory());

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
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalV3FindPackageByIdResource.cs#L108"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/RemoteRepositories/RemoteV3FindPackageByIdResource.cs#L85"/>
    public async Task<IEnumerable<NuGetVersion>> GetAllVersionsAsync(string packageId, CancellationToken cancellationToken)
    {
        using var scope = await GetFindPackageByIdResourceScopeAsync(() => Logger.BeginGetAllVersionsScope(packageId, this, IsUpstream), cancellationToken);
        return await scope.Context.GetAllVersionsAsync(packageId.ToLower(), CacheContext, NuGetLogger, cancellationToken);
    }

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
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalV3FindPackageByIdResource.cs#L248"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/RemoteRepositories/RemoteV3FindPackageByIdResource.cs#L144"/>
    public async Task<FindPackageByIdDependencyInfo?> GetDependencyInfoAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken)
    {
        using var scope = await GetFindPackageByIdResourceScopeAsync(() => Logger.BeginGetDependencyInfoScope(packageId, version, this, IsUpstream), cancellationToken);
        return await scope.Context.GetDependencyInfoAsync(packageId.ToLower(), version, CacheContext, NuGetLogger, cancellationToken);
    }

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
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/FindPackageByIdResource.cs#L61"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalV3FindPackageByIdResource.cs#L388"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/RemoteRepositories/RemoteV3FindPackageByIdResource.cs#L368"/>
    public async Task<bool> DoesPackageExistAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken)
    {
        using var scope = await GetFindPackageByIdResourceScopeAsync(() => Logger.BeginDoesPackageExistScope(packageId, version, this, IsUpstream), cancellationToken);
        return await scope.Context.DoesPackageExistAsync(packageId.ToLower(), version, CacheContext, NuGetLogger, cancellationToken);
    }

    #endregion

    #region DependencyInfoResource methods

    protected async Task<DependencyInfoResource> GetDependencyInfoResourceAsync(CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _getDependencyInfoResourceAsync ??= SourceRepository.GetResourceAsync<DependencyInfoResource>(cancellationToken);
        return await _getDependencyInfoResourceAsync;
    }

    protected async Task<ContextScope<DependencyInfoResource>> GeDependencyInfoResourceScopeAsync(Func<IDisposable?> scopeFactory, CancellationToken cancellationToken) =>
        new(await GetDependencyInfoResourceAsync(cancellationToken), scopeFactory());

    /// <summary>
    /// Retrieve dependency info for a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="framework">The project target framework.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependency information or <see langword="null"/> if the package does not exist.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/DependencyInfoResourceV3.cs#L63"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalDependencyInfoResource.cs#L45"/>
    public async Task<SourcePackageDependencyInfo?> ResolvePackage(string packageId, NuGetVersion version, NuGetFramework framework, CancellationToken cancellationToken)
    {
        using var scope = await GeDependencyInfoResourceScopeAsync(() => Logger.BeginResolvePackageScope(packageId, version, framework, this, IsUpstream), cancellationToken);
        return await scope.Context.ResolvePackage(new(packageId, version), framework, CacheContext, NuGetLogger, cancellationToken);
    }

    /// <summary>
    /// Retrieve dependency info for all versions of a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="framework">The project target framework.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependency information for all versions of a single package.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/DependencyInfoResourceV3.cs#L102"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalDependencyInfoResource.cs#L92"/>
    public async Task<IEnumerable<SourcePackageDependencyInfo>> ResolvePackages(string packageId, NuGetFramework framework, CancellationToken cancellationToken)
    {
        using var scope = await GeDependencyInfoResourceScopeAsync(() => Logger.BeginResolvePackagesScope(packageId, framework, this, IsUpstream), cancellationToken);
        return await scope.Context.ResolvePackages(packageId, framework, CacheContext, NuGetLogger, cancellationToken);
    }

    /// <summary>
    /// Retrieve dependency info for all versions of a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="framework">The project target framework.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependency information for all versions of a single package.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/DependencyInfoResourceV3.cs#L142"/>
    public async Task<IEnumerable<RemoteSourceDependencyInfo>> ResolvePackages(string packageId, CancellationToken cancellationToken)
    {
        using var scope = await GeDependencyInfoResourceScopeAsync(() => Logger.BeginResolvePackagesScope(packageId, this, IsUpstream), cancellationToken);
        return await scope.Context.ResolvePackages(packageId, CacheContext, NuGetLogger, cancellationToken);
    }

    private async Task GetPackageDependenciesAsync(PackageIdentity package, NuGetFramework framework, DependencyInfoResource dependencyInfoResource, ISet<SourcePackageDependencyInfo> availablePackages, CancellationToken cancellationToken)
    {
        if (availablePackages.Contains(package)) return;

        var dependencyInfo = await dependencyInfoResource.ResolvePackage(package, framework, CacheContext, NuGetLogger, cancellationToken);

        if (dependencyInfo == null) return;

        availablePackages.Add(dependencyInfo);
        foreach (var dependency in dependencyInfo.Dependencies)
            await GetPackageDependenciesAsync(new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion), framework, dependencyInfoResource, availablePackages, cancellationToken);
    }

    private async Task<HashSet<SourcePackageDependencyInfo>> GetPackageDependenciesAsync(string packageId, NuGetVersion version, NuGetFramework framework, DependencyInfoResource dependencyInfoResource, CancellationToken cancellationToken)
    {
        PackageIdentity package = new(packageId, version);
        var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
        var dependencyInfo = await dependencyInfoResource.ResolvePackage(package, framework, CacheContext, NuGetLogger, cancellationToken);
        if (dependencyInfo == null) return availablePackages;
        availablePackages.Add(dependencyInfo);
        foreach (var dependency in dependencyInfo.Dependencies)
            await GetPackageDependenciesAsync(new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion), framework, dependencyInfoResource, availablePackages, cancellationToken);
        availablePackages.Remove(dependencyInfo);
        return availablePackages;
    }

    /// <summary>
    /// Retrieve all dependencies for a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="framework">The project target framework.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependencies for the specified package.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/DependencyInfoResourceV3.cs#L102"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalDependencyInfoResource.cs#L92"/>
    public async Task<HashSet<SourcePackageDependencyInfo>> GetPackageDependenciesAsync(string packageId, NuGetVersion version, NuGetFramework framework, CancellationToken cancellationToken)
    {
        using var scope = await GeDependencyInfoResourceScopeAsync(() => Logger.BeginGetPackageDependenciesScope(packageId, version, framework, this, IsUpstream), cancellationToken);
        return await GetPackageDependenciesAsync(packageId, version, framework, scope.Context, cancellationToken);
    }

    #endregion

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;
        if (disposing)
            CacheContext.Dispose();
        _isDisposed = true;
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
