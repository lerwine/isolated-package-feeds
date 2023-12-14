using Microsoft.Extensions.Logging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace NuGetPuller;

/// <summary>
/// Base class providing methods for managing the upstream NuGet repository.
/// </summary>
/// <param name="settings">The validated settings service.</param>
/// <param name="logger">THe logger to write log information to.</param>
public class UpstreamNuGetClientService(IValidatedRepositoryPathsService settings, ILogger<UpstreamNuGetClientService> logger) : NuGetClientService(Repository.Factory.GetCoreV3(settings.UpstreamServiceIndexUrl.GetResult().AbsoluteUri), settings, logger, true), IUpstreamNuGetClientService
{
    /// <summary>
    /// Retrieve dependency info for all versions of a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependency information for all versions of a single package.</returns>
    public Task<IEnumerable<RemoteSourceDependencyInfo>> ResolvePackagesAsync(string packageId, CancellationToken cancellationToken) => ResolvePackagesAsync(packageId, null, cancellationToken);

    /// <summary>
    /// Retrieve dependency info for all versions of a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="dependencyInfoResource">The NuGet resource for the NuGet Dependency Info API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependency information for all versions of a single package.</returns>
    /// <seealso href="https://github.com/NuGet/NuGet.Client/blob/release-6.7.x/src/NuGet.Core/NuGet.Protocol/Resources/DependencyInfoResourceV3.cs#L142"/>
    public async Task<IEnumerable<RemoteSourceDependencyInfo>> ResolvePackagesAsync(string packageId, DependencyInfoResource? dependencyInfoResource, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        using var scope = Logger.BeginResolvePackagesScope(packageId, this);
        return await (dependencyInfoResource ?? await GetDependencyInfoResourceAsync(cancellationToken)).ResolvePackages(packageId, CacheContext, NuGetLogger, cancellationToken);
    }
}
