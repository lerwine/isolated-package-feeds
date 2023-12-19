using Microsoft.Extensions.Logging;
using IsolatedPackageFeeds.Shared;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using static NuGetPuller.NuGetPullerStatic;

namespace NuGetPuller;

/// <summary>
/// Updates packages in the Downloaded NuGet Packages Folder with new packages/versions from the Upstream NuGet Repository.
/// </summary>
/// <param name="downloadedPackagesClient">The service for managing packages in the Downloaded NuGet Packages Folder.</param>
/// <param name="upstreamClient">The service for retrieving packages from the Upstream NuGet Repository.</param>
/// <param name="logger">The logger to write events to.</param>
public class PackageUpdateService(IDownloadedPackagesService downloadedPackagesClient, IUpstreamNuGetClientService upstreamClient, ILogger<PackageUpdateService> logger)
{
    private readonly IDownloadedPackagesService _downloadedPackagesClient = downloadedPackagesClient;
    private readonly IUpstreamNuGetClientService _upstreamClient = upstreamClient;
    private readonly ILogger _logger = logger;

    public async Task UpdatePackagesAsync(IEnumerable<string> packageIds, CancellationToken cancellationToken)
    {
        var updater = await Updater.CreateAsync(this, cancellationToken);
        var downloadedFindPackageById = await _downloadedPackagesClient.GetFindPackageByIdResourceAsync(cancellationToken);
        var versionComparer = VersionComparer.VersionReleaseMetadata;
        HashSet<PackageIdentity> downloaded = new(PackageIdentity.Comparer);
        HashQueue<PackageIdentity> toCheck = new(PackageIdentity.Comparer);
        foreach (string packageId in packageIds.Where(i => !string.IsNullOrWhiteSpace(i)).Distinct(PackageIdentitifierComparer))
        {
            var upstreamVersions = await _upstreamClient.GetAllVersionsAsync(packageId, updater.UpstreamFindPackageById, cancellationToken);
            if (upstreamVersions is null || !upstreamVersions.Any())
            {
                _logger.NuGetPackageNotFound(packageId, _upstreamClient);
                continue;
            }
            var downloadedVersions = await _downloadedPackagesClient.GetAllVersionsAsync(packageId, downloadedFindPackageById, cancellationToken);
            if (downloadedVersions is null || !downloadedVersions.Any())
            {
                _logger.NuGetPackageNotFound(packageId, _downloadedPackagesClient);
                continue;
            }
            foreach (NuGetVersion version in upstreamVersions.Where(u => !downloadedVersions.Contains(u, versionComparer)))
            {
                PackageIdentity identity = new(packageId, version);
                toCheck.TryEnqueue(identity);
                downloaded.Add(identity);
                await updater.DownloadFileAsync(identity, _logger, cancellationToken);
            }
        }
        while (toCheck.TryDequeue(out PackageIdentity? identity))
        {
            var dependencyInfo = await _upstreamClient.GetDependencyInfoAsync(identity.Id, identity.Version, updater.UpstreamFindPackageById, cancellationToken);
            if (dependencyInfo is null)
                _logger.NuGetPackageNotFound(identity.Id, identity.Version, _upstreamClient);
            else if (dependencyInfo.DependencyGroups is not null)
                foreach (var pkg in dependencyInfo.DependencyGroups.Where(g => g.Packages is not null).SelectMany(g => g.Packages.Select(p => new PackageIdentity(p.Id, p.VersionRange.MinVersion))))
                {
                    if (downloaded.Contains(pkg))
                        continue;
                    downloaded.Add(pkg);
                    if (!await _downloadedPackagesClient.DoesPackageExistAsync(pkg.Id, pkg.Version, downloadedFindPackageById, cancellationToken) && toCheck.TryEnqueue(pkg))
                        await updater.DownloadFileAsync(identity, _logger, cancellationToken);
                }
        }
    }
    class Updater : IDisposable
    {
        private readonly VersionFolderPathResolver _pathResolver;
        private readonly TempStagingFolder _tempStaging;
        private readonly IDownloadedPackagesService _downloadedPackagesClient;
        private readonly PackageUpdateResource _updateResource;
        private readonly IUpstreamNuGetClientService _upstreamClient;
        internal FindPackageByIdResource UpstreamFindPackageById { get; }
        private Updater(PackageUpdateService updateService, PackageUpdateResource updateResource, FindPackageByIdResource findPackageById)
        {
            _pathResolver = new((_tempStaging = new()).Directory.FullName);
            _downloadedPackagesClient = updateService._downloadedPackagesClient;
            _updateResource = updateResource;
            _upstreamClient = updateService._upstreamClient;
            UpstreamFindPackageById = findPackageById;
        }
        internal static async Task<Updater> CreateAsync(PackageUpdateService updateService, CancellationToken cancellationToken) =>
            new Updater(
                updateService: updateService,
                updateResource: await updateService._downloadedPackagesClient.GetPackageUpdateResourceAsync(cancellationToken),
                findPackageById: await updateService._upstreamClient.GetFindPackageByIdResourceAsync(cancellationToken)
            );
        internal async Task DownloadFileAsync(PackageIdentity identity, ILogger logger, CancellationToken cancellationToken)
        {
            FileInfo packageFile;
            using (var scope = logger.BeginGetDownloadResourceResultScope(identity, _upstreamClient))
            try
            {
                logger.DownloadingNuGetPackage(identity, _upstreamClient);
                packageFile = await _tempStaging.NewFileInfoAsync(_pathResolver.GetPackageFileName(identity.Id, identity.Version), async (stream, token) =>
                {
                    await _upstreamClient.CopyNupkgToStreamAsync(identity.Id, identity.Version, stream, UpstreamFindPackageById, cancellationToken);
                }, cancellationToken);
            }
            catch (Exception error)
            {
                logger.UnexpectedPackageDownloadFailure(identity.Id, identity.Version, error);
                return;
            }
            await _downloadedPackagesClient.AddPackageAsync(packageFile.FullName, false, _updateResource, cancellationToken);
        }
        public void Dispose() => _tempStaging.Dispose();
    }
}
