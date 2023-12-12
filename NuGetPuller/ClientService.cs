using System.Runtime.CompilerServices;
using IsolatedPackageFeeds.Shared;
using Microsoft.Extensions.Logging;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using static IsolatedPackageFeeds.Shared.CommonStatic;

namespace NuGetPuller;

/// <summary>
/// Base class providing methods for managing a NuGet repository.
/// </summary>
public abstract class ClientService : IDisposable
{
    #region Fields and Properties

    private bool _isDisposed;

    protected string GlobalPackagesFolder { get; }

    protected NugetLogWrapper NuGetLogger { get; }

    protected SourceCacheContext CacheContext { get; } = new();

    public bool IsUpstream { get; }

    protected ILogger Logger { get; }

    protected SourceRepository SourceRepository { get; }

    public Uri PackageSourceUri => SourceRepository.PackageSource.SourceUri;

    public string PackageSourceLocation => SourceRepository.PackageSource.Source;

    #endregion

    /// <summary>
    /// Initializes a new <c>ClientService<c>.
    /// </summary>
    /// <param name="sourceRepository">The source NuGet repository.</param>
    /// <param name="settings">The validated settings service.</param>
    /// <param name="logger">THe logger to write log information to.</param>
    /// <param name="isUpstream"><see langword="true"/> if the source repository represents the upstream repository; otherwise, <see langword="false"/>.</param>
    protected ClientService(SourceRepository sourceRepository, IValidatedRepositoryPathsService settings, ILogger logger, bool isUpstream)
    {
        GlobalPackagesFolder = settings.GlobalPackagesFolder.GetResult().FullName;
        NuGetLogger = new(Logger = logger);
        SourceRepository = sourceRepository;
        IsUpstream = isUpstream;
    }

    #region Methods using the Package Metadata Resource API.

    /// <summary>
    /// Gets the NuGet resource for the NuGet Package Metadata Resource API.
    /// </summary>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>A task that asynchronously returns the the NuGet resource for the NuGet Package Update API.</returns>
    public Task<PackageMetadataResource> GetPackageMetadataResourceAsync(CancellationToken cancellationToken) => SourceRepository.GetResourceAsync<PackageMetadataResource>(cancellationToken);

    /// <summary>
    /// Gets metadata for a specified package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns a list of package meta data objects.</returns>
    public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, CancellationToken cancellationToken) =>
        GetMetadataAsync(packageId, false, false, cancellationToken);

    /// <summary>
    /// Gets metadata for a specified package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="packageMetadataResource">The NuGet resource for the NuGet Package Metadata Resource API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns a list of package meta data objects.</returns>
    public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, PackageMetadataResource? packageMetadataResource, CancellationToken cancellationToken) =>
        GetMetadataAsync(packageId, false, false, packageMetadataResource, cancellationToken);

    /// <summary>
    /// Gets metadata for a specified package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="includePrerelease">Whether to include PreRelease versions into result.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns a list of package meta data objects.</returns>
    public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, CancellationToken cancellationToken) =>
        GetMetadataAsync(packageId, includePrerelease, false, cancellationToken);

    /// <summary>
    /// Gets metadata for a specified package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="includePrerelease">Whether to include PreRelease versions into result.</param>
    /// <param name="packageMetadataResource">The NuGet resource for the NuGet Package Metadata Resource API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns a list of package meta data objects.</returns>
    public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, PackageMetadataResource? packageMetadataResource, CancellationToken cancellationToken) =>
        GetMetadataAsync(packageId, includePrerelease, false, packageMetadataResource, cancellationToken);

    /// <summary>
    /// Gets metadata for a specified package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="includePrerelease">Whether to include PreRelease versions into result.</param>
    /// <param name="includeUnlisted">Whether to include Unlisted versions into result.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns a list of package meta data objects.</returns>
    public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, bool includeUnlisted, CancellationToken cancellationToken) =>
        GetMetadataAsync(packageId, includePrerelease, includeUnlisted, null, cancellationToken);

    /// <summary>
    /// Gets metadata for a specified package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="includePrerelease">Whether to include PreRelease versions into result.</param>
    /// <param name="includeUnlisted">Whether to include Unlisted versions into result.</param>
    /// <param name="packageMetadataResource">The NuGet resource for the NuGet Package Metadata Resource API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns a list of package meta data objects.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageMetadataResourceV3.cs#L46"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalPackageMetadataResource.cs#L29"/>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/registration-base-url-resource#registration-page"/>
    public async Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, bool includeUnlisted, PackageMetadataResource? packageMetadataResource, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        using var scope = Logger.BeginGetMetadataScope(packageId, includePrerelease, includeUnlisted, this);
        return await (packageMetadataResource ?? await GetPackageMetadataResourceAsync(cancellationToken))
            .GetMetadataAsync(packageId.ToLower(), includePrerelease, includeUnlisted, CacheContext, NuGetLogger, cancellationToken);
    }

    /// <summary>
    /// Gets metadata for a specified package version.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the package meta data object or <see langword="null"/> if not found.</returns>
    public Task<IPackageSearchMetadata?> GetMetadataAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken) => GetMetadataAsync(packageId, version, null, cancellationToken);

    /// <summary>
    /// Gets metadata for a specified package version.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="packageMetadataResource">The NuGet resource for the NuGet Package Metadata Resource API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the package meta data object or <see langword="null"/> if not found.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageMetadataResourceV3.cs#L66"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalPackageMetadataResource.cs#L51"/>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/registration-base-url-resource#registration-page"/>
    public async Task<IPackageSearchMetadata?> GetMetadataAsync(string packageId, NuGetVersion version, PackageMetadataResource? packageMetadataResource, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        ArgumentNullException.ThrowIfNull(version);
        using var scope = Logger.BeginGetMetadataScope(packageId, version, this);
        return await (packageMetadataResource ?? await GetPackageMetadataResourceAsync(cancellationToken)).GetMetadataAsync(new PackageIdentity(packageId.ToLower(), version), CacheContext, NuGetLogger, cancellationToken);
    }

    #endregion

    #region Methods using the NuGet V3 Package Content API

    /// <summary>
    /// Gets the NuGet resource for the NuGet V3 Package Content API.
    /// </summary>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>A task that asynchronously returns the the NuGet resource for the NuGet V3 Package Content API.</returns>
    public Task<FindPackageByIdResource> GetFindPackageByIdResourceAsync(CancellationToken cancellationToken) => SourceRepository.GetResourceAsync<FindPackageByIdResource>(cancellationToken);

    /// <summary>
    /// Asynchronously gets all package versions for a package ID.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the package versions for the specified <paramref name="packageId"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="packageId" /> is either <c>null</c> or an empty string.</exception>
    /// <exception cref="OperationCanceledException">Thrown if <paramref name="cancellationToken" /> is cancelled.</exception>
    public Task<IEnumerable<NuGetVersion>> GetAllVersionsAsync(string packageId, CancellationToken cancellationToken) => GetAllVersionsAsync(packageId, null, cancellationToken);

    /// <summary>
    /// Asynchronously gets all package versions for a package ID.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="findPackageByIdResource">The NuGet resource for the NuGet V3 Package Content API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the package versions for the specified <paramref name="packageId"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="packageId" /> is either <c>null</c> or an empty string.</exception>
    /// <exception cref="OperationCanceledException">Thrown if <paramref name="cancellationToken" /> is cancelled.</exception>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/package-base-address-resource#enumerate-package-versions"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/FindPackageByIdResource.cs#L37"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalV3FindPackageByIdResource.cs#L108"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/RemoteRepositories/RemoteV3FindPackageByIdResource.cs#L85"/>
    public async Task<IEnumerable<NuGetVersion>> GetAllVersionsAsync(string packageId, FindPackageByIdResource? findPackageByIdResource, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        using var scope = Logger.BeginGetAllVersionsScope(packageId, this);
        return await (findPackageByIdResource ?? await GetFindPackageByIdResourceAsync(cancellationToken)).GetAllVersionsAsync(packageId.ToLower(), CacheContext, NuGetLogger, cancellationToken);
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
    public Task<FindPackageByIdDependencyInfo?> GetDependencyInfoAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken) => GetDependencyInfoAsync(packageId, version, null, cancellationToken);

    /// <summary>
    /// Asynchronously gets dependency information for a specific package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="findPackageByIdResource">The NuGet resource for the NuGet V3 Package Content API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns dependency information for the specified <paramref name="packageId"/> and <paramref name="version"/> or <see langword="null"/> if not found.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="packageId" /> is either <c>null</c> or an empty string.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="version" /> <c>null</c>.</exception>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/FindPackageByIdResource.cs#L61"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalV3FindPackageByIdResource.cs#L248"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/RemoteRepositories/RemoteV3FindPackageByIdResource.cs#L144"/>
    public async Task<FindPackageByIdDependencyInfo?> GetDependencyInfoAsync(string packageId, NuGetVersion version, FindPackageByIdResource? findPackageByIdResource, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        ArgumentNullException.ThrowIfNull(version);
        using var scope = Logger.BeginGetDependencyInfoScope(packageId, version, this);
        return await (findPackageByIdResource ?? await GetFindPackageByIdResourceAsync(cancellationToken)).GetDependencyInfoAsync(packageId.ToLower(), version, CacheContext, NuGetLogger, cancellationToken);
    }

    /// <summary>
    /// Asynchronously downloads and copies NuGet package to output stream.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="destination">The output stream.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    public Task CopyNupkgToStreamAsync(string packageId, NuGetVersion version, Stream destination, CancellationToken cancellationToken) => CopyNupkgToStreamAsync(packageId, version, destination, null, cancellationToken);

    /// <summary>
    /// Asynchronously downloads and copies NuGet package to output stream.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="destination">The output stream.</param>
    /// <param name="findPackageByIdResource">The NuGet resource for the NuGet V3 Package Content API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/package-publish-resource#push-a-package"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/Resources/FindPackageByIdResource.cs#L88"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalV3FindPackageByIdResource.cs#L167"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.8.x/src/NuGet.Core/NuGet.Protocol/RemoteRepositories/RemoteV3FindPackageByIdResource.cs#L222"/>
    public async Task CopyNupkgToStreamAsync(string packageId, NuGetVersion version, Stream destination, FindPackageByIdResource? findPackageByIdResource, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(destination);
        using var scope = Logger.BeginDownloadNupkgScope(packageId, version, this);
        await (findPackageByIdResource ?? await GetFindPackageByIdResourceAsync(cancellationToken)).CopyNupkgToStreamAsync(packageId, version, destination, CacheContext, NuGetLogger, cancellationToken);
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
    public Task<bool> DoesPackageExistAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken) => DoesPackageExistAsync(packageId, version, null, cancellationToken);

    /// <summary>
    /// Asynchronously checks whether the exact package id and version exists at this source.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="findPackageByIdResource">The NuGet resource for the NuGet V3 Package Content API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns <see langword="true"/> if the exact package id and version exists at this source; otherwise, <see langword="true"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="id" />
    /// is either <c>null</c> or an empty string.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="version" /> <c>null</c>.</exception>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/FindPackageByIdResource.cs#L61"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalV3FindPackageByIdResource.cs#L388"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/RemoteRepositories/RemoteV3FindPackageByIdResource.cs#L368"/>
    public async Task<bool> DoesPackageExistAsync(string packageId, NuGetVersion version, FindPackageByIdResource? findPackageByIdResource, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        ArgumentNullException.ThrowIfNull(version);
        using var scope = Logger.BeginDoesPackageExistScope(packageId, version, this);
        return await (findPackageByIdResource ?? await GetFindPackageByIdResourceAsync(cancellationToken)).DoesPackageExistAsync(packageId.ToLower(), version, CacheContext, NuGetLogger, cancellationToken);
    }

    #endregion

    #region Dependency Info Resource methods

    /// <summary>
    /// Gets the NuGet resource for the NuGet Dependency Info API.
    /// </summary>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>A task that asynchronously returns the the NuGet resource for the NuGet Dependency Info API.</returns>
    public Task<DependencyInfoResource> GetDependencyInfoResourceAsync(CancellationToken cancellationToken) => SourceRepository.GetResourceAsync<DependencyInfoResource>(cancellationToken);

    /// <summary>
    /// Retrieve dependency info for a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="framework">The project target framework.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependency information or <see langword="null"/> if the package does not exist.</returns>
    public Task<SourcePackageDependencyInfo?> ResolvePackage(string packageId, NuGetVersion version, NuGetFramework framework, CancellationToken cancellationToken) => ResolvePackage(packageId, version, framework, null, cancellationToken);

    /// <summary>
    /// <summary>
    /// Retrieve dependency info for a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="framework">The project target framework.</param>
    /// <param name="dependencyInfoResource">The NuGet resource for the NuGet Dependency Info API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependency information or <see langword="null"/> if the package does not exist.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/DependencyInfoResourceV3.cs#L63"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalDependencyInfoResource.cs#L45"/>
    public async Task<SourcePackageDependencyInfo?> ResolvePackage(string packageId, NuGetVersion version, NuGetFramework framework, DependencyInfoResource? dependencyInfoResource, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(framework);
        using var scope = Logger.BeginResolvePackageScope(packageId, version, framework, this);
        return await (dependencyInfoResource ?? await GetDependencyInfoResourceAsync(cancellationToken)).ResolvePackage(new(packageId, version), framework, CacheContext, NuGetLogger, cancellationToken);
    }

    /// <summary>
    /// Retrieve dependency info for all versions of a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="framework">The project target framework.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependency information for all versions of a single package.</returns>
    public Task<IEnumerable<SourcePackageDependencyInfo>> ResolvePackages(string packageId, NuGetFramework framework, CancellationToken cancellationToken) => ResolvePackages(packageId, framework, null, cancellationToken);

    /// <summary>
    /// Retrieve dependency info for all versions of a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="framework">The project target framework.</param>
    /// <param name="dependencyInfoResource">The NuGet resource for the NuGet Dependency Info API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependency information for all versions of a single package.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/DependencyInfoResourceV3.cs#L102"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalDependencyInfoResource.cs#L92"/>
    public async Task<IEnumerable<SourcePackageDependencyInfo>> ResolvePackages(string packageId, NuGetFramework framework, DependencyInfoResource? dependencyInfoResource, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        ArgumentNullException.ThrowIfNull(framework);
        using var scope = Logger.BeginResolvePackagesScope(packageId, framework, this);
        return await (dependencyInfoResource ?? await GetDependencyInfoResourceAsync(cancellationToken)).ResolvePackages(packageId, framework, CacheContext, NuGetLogger, cancellationToken);
    }

    /// <summary>
    /// Retrieve all dependencies for a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="framework">The project target framework.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependencies for the specified package.</returns>
    public IAsyncEnumerable<SourcePackageDependencyInfo> GetAllDependenciesAsync(string packageId, NuGetVersion version, NuGetFramework framework, CancellationToken cancellationToken) =>
        GetAllDependenciesAsync(packageId, version, framework, null, cancellationToken);

    /// <summary>
    /// Retrieve all dependencies for a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="framework">The project target framework.</param>
    /// <param name="dependencyInfoResource">The NuGet resource for the NuGet Dependency Info API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependencies for the specified package.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/DependencyInfoResourceV3.cs#L102"/>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/LocalRepositories/LocalDependencyInfoResource.cs#L92"/>
    public async IAsyncEnumerable<SourcePackageDependencyInfo> GetAllDependenciesAsync(string packageId, NuGetVersion version, NuGetFramework framework, DependencyInfoResource? dependencyInfoResource, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(framework);
        dependencyInfoResource ??= await GetDependencyInfoResourceAsync(cancellationToken);
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
    public IAsyncEnumerable<PackageIdentity> GetAllVersionsWithDependenciesAsync(IEnumerable<string> packageIds, CancellationToken cancellationToken) => GetAllVersionsWithDependenciesAsync(packageIds, null, null, cancellationToken);
    
    /// <summary>
    /// Retrieve all versions of packages, including all versions of all dependencies.
    /// </summary>
    /// <param name="packageIds">The IDs of the packages to retrieve versions and dependencies for.</param>
    /// <param name="findPackageByIdResource">The NuGet resource for the NuGet V3 Package Content API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>All versions of packages, including all versions of all dependencies</returns>
    public IAsyncEnumerable<PackageIdentity> GetAllVersionsWithDependenciesAsync(IEnumerable<string> packageIds, FindPackageByIdResource? findPackageByIdResource, CancellationToken cancellationToken) =>
        GetAllVersionsWithDependenciesAsync(packageIds, findPackageByIdResource, null, cancellationToken);
    
    /// <summary>
    /// Retrieve all versions of packages, including all versions of all dependencies.
    /// </summary>
    /// <param name="packageIds">The IDs of the packages to retrieve versions and dependencies for.</param>
    /// <param name="dependencyInfoResource">The NuGet resource for the NuGet Dependency Info API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>All versions of packages, including all versions of all dependencies</returns>
    public IAsyncEnumerable<PackageIdentity> GetAllVersionsWithDependenciesAsync(IEnumerable<string> packageIds, DependencyInfoResource? dependencyInfoResource, CancellationToken cancellationToken) =>
        GetAllVersionsWithDependenciesAsync(packageIds, null, dependencyInfoResource, cancellationToken);
    
    /// <summary>
    /// Retrieve all versions of packages, including all versions of all dependencies.
    /// </summary>
    /// <param name="packageIds">The IDs of the packages to retrieve versions and dependencies for.</param>
    /// <param name="findPackageByIdResource">The NuGet resource for the NuGet V3 Package Content API.</param>
    /// <param name="dependencyInfoResource">The NuGet resource for the NuGet Dependency Info API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns>All versions of packages, including all versions of all dependencies</returns>
    // BUG: This never returns
    public async IAsyncEnumerable<PackageIdentity> GetAllVersionsWithDependenciesAsync(IEnumerable<string> packageIds, FindPackageByIdResource? findPackageByIdResource, DependencyInfoResource? dependencyInfoResource, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(packageIds);
        if (!(packageIds = packageIds.Select(id => id.ToTrimmedOrNullIfEmpty()!).Where(id => id is not null)).Any())
            yield break;
        findPackageByIdResource ??= await GetFindPackageByIdResourceAsync(cancellationToken);
        dependencyInfoResource ??= await GetDependencyInfoResourceAsync(cancellationToken);
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

    // public Task<DownloadResource> GetDownloadResourceAsync(CancellationToken cancellationToken) => SourceRepository.GetResourceAsync<DownloadResource>(cancellationToken);

    // public Task<DownloadResourceResult> GetDownloadResourceResultAsync(PackageIdentity identity, CancellationToken cancellationToken) => GetDownloadResourceResultAsync(identity, null, cancellationToken);

    // public async Task<DownloadResourceResult> GetDownloadResourceResultAsync(PackageIdentity identity, DownloadResource? downloadResource, CancellationToken cancellationToken)
    // {
    //     using var scope = Logger.BeginGetDownloadResourceResultScope(identity, this);
    //     return await (downloadResource ?? await GetDownloadResourceAsync(cancellationToken)).GetDownloadResourceResultAsync(identity, new PackageDownloadContext(CacheContext), GlobalPackagesFolder, NuGetLogger, cancellationToken);
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
