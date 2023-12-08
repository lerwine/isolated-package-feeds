using Microsoft.Extensions.Logging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace NuGetPuller;

public abstract class UpstreamClientServiceBase(IValidatedSharedAppSettings settings, ILogger logger) : ClientService(Repository.Factory.GetCoreV3(settings.GetUpstreamServiceIndex()), settings, logger, true)
{
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
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        using var scope = await GeDependencyInfoResourceScopeAsync(() => Logger.BeginResolvePackagesScope(packageId, this), cancellationToken);
        return await scope.Context.ResolvePackages(packageId, CacheContext, NuGetLogger, cancellationToken);
    }
}
