using NuGet.Protocol.Core.Types;

namespace NuGetPuller;

public interface IUpstreamNuGetClientService : INuGetClientService
{
    /// <summary>
    /// Retrieve dependency info for all versions of a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependency information for all versions of a single package.</returns>
    Task<IEnumerable<RemoteSourceDependencyInfo>> ResolvePackagesAsync(string packageId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieve dependency info for all versions of a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="dependencyInfoResource">The NuGet resource for the NuGet Dependency Info API.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependency information for all versions of a single package.</returns>
    Task<IEnumerable<RemoteSourceDependencyInfo>> ResolvePackagesAsync(string packageId, DependencyInfoResource? dependencyInfoResource, CancellationToken cancellationToken);
}
