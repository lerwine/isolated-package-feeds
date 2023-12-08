using NuGet.Protocol.Core.Types;

namespace NuGetPuller;

public interface IUpstreamClientService : IClientService
{
    /// <summary>
    /// Retrieve dependency info for all versions of a single package.
    /// </summary>
    /// <param name="packageId">The package ID.</param>
    /// <param name="framework">The project target framework.</param>
    /// <param name="cancellationToken">The token to observe during the asynchronous operation.</param>
    /// <returns><see cref="Task{TResult}.Result" /> returns the dependency information for all versions of a single package.</returns>
    Task<IEnumerable<RemoteSourceDependencyInfo>> ResolvePackages(string packageId, CancellationToken cancellationToken);
}
