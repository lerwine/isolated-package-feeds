using Microsoft.Extensions.Logging;
using NuGet.Frameworks;
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

    private async Task<DownloadResource> GetDownloadResourceAsync(CancellationToken cancellationToken)
    {
        var source = SourceRepo.GetOutput();
        return await source.GetResourceAsync<DownloadResource>(cancellationToken) ?? throw GetNugetResourceException.LogAndCreate<DownloadResource>(Logger);
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
    public async Task<DownloadResourceResult> GetDownloadResourceResultAsync(PackageIdentity identity, PackageDownloadContext downloadContext, string globalPackagesFolder, CancellationToken cancellationToken)
    {
        using var scope = Logger.BeginGetNuGetDownloadResourceResultScope(identity, SourceRepo.GetInput(), downloadContext, globalPackagesFolder);
        try { return await (await GetDownloadResourceAsync(cancellationToken)).GetDownloadResourceResultAsync(identity, downloadContext, globalPackagesFolder, _nugetLogger, cancellationToken); }
        catch (Exception exception)
        {
            throw GetNuGetDownloadResourceResultsException.LogAndCreate(Logger, identity, SourceRepo.Input, downloadContext, globalPackagesFolder, exception);
        }
    }

    private async Task<PackageMetadataResource> GetPackageMetadataResourceAsync(CancellationToken cancellationToken)
    {
        var source = SourceRepo.GetOutput();
        return await source.GetResourceAsync<PackageMetadataResource>(cancellationToken) ?? throw GetNugetResourceException.LogAndCreate<PackageMetadataResource>(Logger);
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
    public async Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, bool includeUnlisted, CancellationToken cancellationToken)
    {
        using var scope = Logger.BeginGetNugetMetaDataScope(SourceRepo.GetInput(), packageId, includePrerelease, includeUnlisted);
        try { return await (await GetPackageMetadataResourceAsync(cancellationToken)).GetMetadataAsync(packageId.ToLower(), includePrerelease, includeUnlisted, _sourceCacheContext, _nugetLogger, cancellationToken); }
        catch (Exception exception)
        {
            throw GetNuGetMetaDataException.LogAndCreate(Logger, packageId, SourceRepo.Input, includePrerelease, includeUnlisted, exception);
        }
    }

    /// <summary>
    /// Gets metadata for a specified package version.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the package meta data object or <see langword="null"/> if not found.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/PackageMetadataResourceV3.cs#L66"/>
    public async Task<IPackageSearchMetadata?> GetMetadataAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken)
    {
        using var scope = Logger.BeginGetNugetMetaDataScope(SourceRepo.GetInput(), packageId, version);
        try { return await (await GetPackageMetadataResourceAsync(cancellationToken)).GetMetadataAsync(new PackageIdentity(packageId.ToLower(), version), _sourceCacheContext, _nugetLogger, cancellationToken); }
        catch (Exception exception)
        {
            throw GetNuGetMetaDataException.LogAndCreate(Logger, packageId, SourceRepo.Input, version, exception);
        }
    }

    private async Task<FindPackageByIdResource> GetFindPackageByIdResourceAsync(CancellationToken cancellationToken)
    {
        var source = SourceRepo.GetOutput();
        return await source.GetResourceAsync<FindPackageByIdResource>(cancellationToken) ?? throw GetNugetResourceException.LogAndCreate<FindPackageByIdResource>(Logger);
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
    public async Task<IEnumerable<NuGetVersion>> GetAllVersionsAsync(string packageId, CancellationToken cancellationToken)
    {
        using var scope = Logger.BeginGetNuGetPackageVersionsScope(SourceRepo.GetInput(), packageId);
        try { return await (await GetFindPackageByIdResourceAsync(cancellationToken)).GetAllVersionsAsync(packageId.ToLower(), _sourceCacheContext, _nugetLogger, cancellationToken); }
        catch (Exception exception)
        {
            throw GetAllNuGetVersionsException.LogAndCreate(Logger, packageId, SourceRepo.Input, exception);
        }
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
    public async Task<FindPackageByIdDependencyInfo?> GetDependencyInfoAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken)
    {
        using var scope = Logger.BeginFindNuGetPackageByIdScope(SourceRepo.GetInput(), packageId);
        try { return await (await GetFindPackageByIdResourceAsync(cancellationToken)).GetDependencyInfoAsync(packageId.ToLower(), version, _sourceCacheContext, _nugetLogger, cancellationToken); }
        catch (Exception exception)
        {
            throw GetNuGetDependencyInfoException.LogAndCreate(Logger, packageId, version, SourceRepo.Input, exception);
        }
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
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/FindPackageByIdResource.cs#L134"/>
    public async Task<bool> DoesPackageExistAsync(string packageId, NuGetVersion version, CancellationToken cancellationToken)
    {
        using var scope = Logger.BeginFindNuGetPackageByIdScope(SourceRepo.GetInput(), packageId);
        try { return await (await GetFindPackageByIdResourceAsync(cancellationToken)).DoesPackageExistAsync(packageId.ToLower(), version, _sourceCacheContext, _nugetLogger, cancellationToken); }
        catch (Exception exception)
        {
            throw DoesNuGetPackageExistException.LogAndCreate(Logger, packageId, version, SourceRepo.Input, exception);
        }
    }

    private async Task<DependencyInfoResource> GetDependencyInfoResourceAsync(CancellationToken cancellationToken)
    {
        var source = SourceRepo.GetOutput();
        return await source.GetResourceAsync<DependencyInfoResource>(cancellationToken) ?? throw GetNugetResourceException.LogAndCreate<DependencyInfoResource>(Logger);
    }

    public async Task<ISet<SourcePackageDependencyInfo>> GetPackageDependenciesAsync(string packageId, NuGetVersion version, NuGetFramework framework, CancellationToken cancellationToken)
    {
        PackageIdentity package = new(packageId, version);
        var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
        var dependencyInfoResource = await GetDependencyInfoResourceAsync(cancellationToken);
        var dependencyInfo = await dependencyInfoResource.ResolvePackage(package, framework, _sourceCacheContext, _nugetLogger, cancellationToken);
        if (dependencyInfo == null) return availablePackages;
        availablePackages.Add(dependencyInfo);
        foreach (var dependency in dependencyInfo.Dependencies)
            await GetPackageDependenciesAsync(new(dependency.Id, dependency.VersionRange.MinVersion), framework, dependencyInfoResource, availablePackages, cancellationToken);
        availablePackages.Remove(dependencyInfo);
        return availablePackages;
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