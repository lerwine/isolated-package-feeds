using System.Runtime.CompilerServices;
using IsolatedPackageFeeds.Shared;
using Microsoft.Extensions.Logging;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using static IsolatedPackageFeeds.Shared.CommonStatic;

namespace NuGetPuller;

public abstract class ClientService
{
    #region Fields and Properties

    private bool _isDisposed;
    private readonly object _syncRoot = new();
    private Task<PackageMetadataResource>? _getPackageMetadataResourceAsync;
    private Task<FindPackageByIdResource>? _getFindPackageByIdResourceAsync;
    private Task<DependencyInfoResource>? _getDependencyInfoResourceAsync;
    // private Task<DownloadResource>? _getDownloadResourceAsync;

    protected string GlobalPackagesFolder { get; }

    protected NugetLogWrapper NuGetLogger { get; }

    protected SourceCacheContext CacheContext { get; } = new();

    public bool IsUpstream { get; }

    protected ILogger Logger { get; }

    protected SourceRepository SourceRepository { get; }

    public Uri PackageSourceUri => SourceRepository.PackageSource.SourceUri;

    public string PackageSourceLocation => SourceRepository.PackageSource.Source;

    #endregion

    protected ClientService(SourceRepository sourceRepository, IValidatedRepositoryPathsService settings, ILogger logger, bool isUpstream)
    {
        GlobalPackagesFolder = settings.GlobalPackagesFolder.GetResult().FullName;
        NuGetLogger = new(Logger = logger);
        SourceRepository = sourceRepository;
        IsUpstream = isUpstream;
    }

    public sealed class ContextScope<T>(T context, IDisposable? scope) : IDisposable
    {
        private readonly IDisposable? _scope = scope;
        public T Context { get; } = context;

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }

    #region Methods using the PackageUpdateResource API.

    protected async Task<PackageMetadataResource> GetPackageMetadataResourceAsync(CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _getPackageMetadataResourceAsync ??= SourceRepository.GetResourceAsync<PackageMetadataResource>(cancellationToken);
        return await _getPackageMetadataResourceAsync;
    }

    protected async Task<ContextScope<PackageMetadataResource>> GetPackageMetadataResourceScopeAsync(Func<IDisposable?> scopeFactory, CancellationToken cancellationToken) =>
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
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        using var scope = await GetPackageMetadataResourceScopeAsync(() => Logger.BeginGetMetadataScope(packageId, includePrerelease, includeUnlisted, this), cancellationToken);
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
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        ArgumentNullException.ThrowIfNull(version);
        using var scope = await GetPackageMetadataResourceScopeAsync(() => Logger.BeginGetMetadataScope(packageId, version, this), cancellationToken);
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
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/FindPackageByIdResource.cs#L37"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalV3FindPackageByIdResource.cs#L108"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/RemoteRepositories/RemoteV3FindPackageByIdResource.cs#L85"/>
    public async Task<IEnumerable<NuGetVersion>> GetAllVersionsAsync(string packageId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        using var scope = await GetFindPackageByIdResourceScopeAsync(() => Logger.BeginGetAllVersionsScope(packageId, this), cancellationToken);
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
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/FindPackageByIdResource.cs#L61"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalV3FindPackageByIdResource.cs#L248"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/RemoteRepositories/RemoteV3FindPackageByIdResource.cs#L144"/>
    public async Task<FindPackageByIdDependencyInfo?> GetDependencyInfoAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        ArgumentNullException.ThrowIfNull(version);
        using var scope = await GetFindPackageByIdResourceScopeAsync(() => Logger.BeginGetDependencyInfoScope(packageId, version, this), cancellationToken);
        return await scope.Context.GetDependencyInfoAsync(packageId.ToLower(), version, CacheContext, NuGetLogger, cancellationToken);
    }

    /// <summary>
    /// Asynchronously downloads and copies NuGet package to output stream.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="destination">The output stream.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/package-publish-resource#push-a-package"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/FindPackageByIdResource.cs#L88"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalV3FindPackageByIdResource.cs#L167"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/RemoteRepositories/RemoteV3FindPackageByIdResource.cs#L222"/>
    public async Task CopyNupkgToStreamAsync(string packageId, NuGetVersion version, Stream destination, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(destination);
        using var scope = await GetFindPackageByIdResourceScopeAsync(() => Logger.BeginDownloadNupkgScope(packageId, version, this), cancellationToken);
        await scope.Context.CopyNupkgToStreamAsync(packageId, version, destination, CacheContext, NuGetLogger, cancellationToken);
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
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        ArgumentNullException.ThrowIfNull(version);
        using var scope = await GetFindPackageByIdResourceScopeAsync(() => Logger.BeginDoesPackageExistScope(packageId, version, this), cancellationToken);
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
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(framework);
        using var scope = await GeDependencyInfoResourceScopeAsync(() => Logger.BeginResolvePackageScope(packageId, version, framework, this), cancellationToken);
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
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        ArgumentNullException.ThrowIfNull(framework);
        using var scope = await GeDependencyInfoResourceScopeAsync(() => Logger.BeginResolvePackagesScope(packageId, framework, this), cancellationToken);
        return await scope.Context.ResolvePackages(packageId, framework, CacheContext, NuGetLogger, cancellationToken);
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
    public async IAsyncEnumerable<SourcePackageDependencyInfo> GetAllDependenciesAsync(string packageId, NuGetVersion version, NuGetFramework framework, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(framework);
        var dependencyInfoResource = await GetDependencyInfoResourceAsync(cancellationToken);
        PackageIdentity package = new(packageId, version);
        HashSet<PackageIdentity> expanded = new(PackageIdentityComparer.Default) { package };
        var dependencyInfo = await dependencyInfoResource.ResolvePackage(package, framework, CacheContext, NuGetLogger, cancellationToken);
        if (dependencyInfo is null)
        {
            Logger.LogPackageNotFound(packageId, this);
            yield break;
        }
        if (dependencyInfo.Dependencies is null)
            yield break;
        HashQueue<PackageDependency> toExpand = new(dependencyInfo.Dependencies, PackageDependencyComparer.Default);
        while (toExpand.TryDequeue(out PackageDependency? dependency))
        {
            package = new(dependency.Id, dependency.VersionRange.MinVersion);
            if (expanded.Contains(package))
                continue;
            expanded.Add(package);
            if ((dependencyInfo = await dependencyInfoResource.ResolvePackage(package, framework, CacheContext, NuGetLogger, cancellationToken)) is null)
                continue;
            yield return dependencyInfo;
            foreach (var pd in dependencyInfo.Dependencies)
                toExpand.TryEnqueue(pd);
        }
    }

    /// <summary>
    /// Retrieve all versions of packages, including all versions of all dependencies.
    /// </summary>
    /// <param name="packageIds">The IDs of the packages to retrieve versions and dependencies for.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>All versions of packages, including all versions of all dependencies</returns>
    // BUG: This never returns
    public async IAsyncEnumerable<PackageIdentity> GetAllVersionsWithDependenciesAsync(IEnumerable<string> packageIds, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(packageIds);
        if (!(packageIds = packageIds.Select(id => id.ToTrimmedOrNullIfEmpty()!).Where(id => id is not null)).Any())
            yield break;
        var findPackageByIdResource = await GetFindPackageByIdResourceAsync(cancellationToken);
        var dependencyInfoResource = await GetDependencyInfoResourceAsync(cancellationToken);
        HashSet<string> expanded = new(NoCaseComparer);
        HashQueue<string> toExpand = new(packageIds, NoCaseComparer);
        while (toExpand.TryDequeue(out string? id))
        {
            expanded.Add(id);
            var allVersions = await findPackageByIdResource.GetAllVersionsAsync(id, CacheContext, NuGetLogger, cancellationToken);
            if (allVersions is null || !allVersions.Any())
            {
                Logger.LogPackageNotFound(id, this);
                continue;
            }
            foreach (var version in allVersions)
            {
                PackageIdentity package = new(id, version);
                yield return package;
                var frameworksForVersion = ((await GetDependencyInfoAsync(id, version, cancellationToken))?.FrameworkReferenceGroups?.Select(g => g.TargetFramework) ?? Enumerable.Empty<NuGetFramework>())
                    .DefaultIfEmpty(NuGetFramework.AnyFramework);
                foreach (var framework in frameworksForVersion)
                {
                    using var scope = Logger.BeginGetPackageDependenciesScope(id, version, framework, this);
                    var dependencyIds = (await dependencyInfoResource.ResolvePackage(package, framework, CacheContext, NuGetLogger, cancellationToken))?.Dependencies?.Select(d => d.Id);
                    if (dependencyIds is not null)
                        foreach (var pkgId in dependencyIds)
                            if (!expanded.Contains(pkgId))
                                toExpand.TryEnqueue(pkgId);
                }
            }
        }
    }

    #endregion

    // #region DownloadResource methods

    // protected async Task<DownloadResource> GetDownloadResourceAsync(CancellationToken cancellationToken)
    // {
    //     lock (_syncRoot)
    //         _getDownloadResourceAsync ??= SourceRepository.GetResourceAsync<DownloadResource>(cancellationToken);
    //     return await _getDownloadResourceAsync;
    // }

    // protected async Task<ContextScope<DownloadResource>> GetDownloadResourceScopeAsync(Func<IDisposable?> scopeFactory, CancellationToken cancellationToken) =>
    //     new(await GetDownloadResourceAsync(cancellationToken), scopeFactory());

    // public async Task<DownloadResourceResult> GetDownloadResourceResultAsync(PackageIdentity identity, CancellationToken cancellationToken)
    // {
    //     using var scope = await GetDownloadResourceScopeAsync(() => Logger.BeginGetDownloadResourceResultScope(identity, this, IsUpstream), cancellationToken);
    //     return await scope.Context.GetDownloadResourceResultAsync(identity, new PackageDownloadContext(CacheContext), GlobalPackagesFolder, NuGetLogger, cancellationToken);
    // }

    // #endregion

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
