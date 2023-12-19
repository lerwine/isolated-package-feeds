using Microsoft.Extensions.Logging;
using IsolatedPackageFeeds.Shared;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Newtonsoft.Json;
using static IsolatedPackageFeeds.Shared.CommonStatic;
using static NuGetPuller.NuGetPullerStatic;

namespace NuGetPuller;

/// <summary>
/// Static methods for user interface commands.
/// </summary>
public partial class MainServiceStatic
{
    public static OfflinePackageMetadata[] LoadOfflinePackageMetadata(string? path, ILogger logger)
    {
        if (string.IsNullOrEmpty(path))
            return [];
        using StreamReader reader = OpenStreamReader(path,
            (p, e) => logger.PackageMetadataFileAccessDenied(path, m => new OfflineMetaDataIOException(p, m, e), e),
            (p, e) => logger.PackageMetadataOpenError(path, m => new OfflineMetaDataIOException(p, m, e), e));
        try
        {
            using JsonTextReader jsonReader = new(reader);
            JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(MetadataDeSerializationSettings);
            return jsonSerializer.Deserialize<OfflinePackageMetadata[]>(jsonReader) ?? [];
        }
        catch (JsonReaderException exception)
        {
            throw logger.PackageMetadataFileReadError(path, m => new OfflineMetaDataIOException(path, m, exception), exception);
        }
    }

    private static FileInfo GetExportBundleFileInfos(string bundlePath, string? targetMetadataInput, string? targetMetaDataOutput, ILogger logger, CancellationToken cancellationToken,
        out FileInfo targetMetadataFileInfo, out List<OfflinePackageMetadata> existingPackages)
    {
        var bundleFileInfo = GetFileInfo(bundlePath,
            (p, e) => logger.ExportBundleAccessError(bundlePath, m => new OfflineMetaDataIOException(p, m, e), e),
            (p, e) => logger.InvalidExportBundle(bundlePath, m => new OfflineMetaDataIOException(p, m, e), e),
            p => logger.ExportBundlePathNotAFile(p, m => new OfflineMetaDataIOException(p, m)),
            p => logger.ExportBundleDirectoryNotFound(p, m => new OfflineMetaDataIOException(p, m)));
        if (string.IsNullOrEmpty(targetMetadataInput))
        {
            if (string.IsNullOrWhiteSpace(targetMetaDataOutput))
            {
                if (bundleFileInfo.Directory is null)
                    throw logger.InvalidExportBundle(bundlePath, m => new OfflineMetaDataIOException(bundleFileInfo.FullName, m));
                cancellationToken.ThrowIfCancellationRequested();
                targetMetadataFileInfo = GetUniqueFileInfo(bundleFileInfo.Directory, Path.GetFileNameWithoutExtension(bundleFileInfo.Name), ".json",
                    (p, e) => logger.ExportBundleAccessError(bundlePath, m => new OfflineMetaDataIOException(p, m, e), e),
                    (p, e) => logger.InvalidExportBundle(bundlePath, m => new OfflineMetaDataIOException(p, m, e), e));
            }
            else
                targetMetadataFileInfo = GetFileInfo(targetMetaDataOutput,
                    (p, e) => logger.PackageMetadataFileAccessDenied(bundlePath, m => new OfflineMetaDataIOException(p, m, e), e),
                    (p, e) => logger.PackageMetadataOpenError(bundlePath, m => new OfflineMetaDataIOException(p, m, e), e),
                    p => logger.OfflinePackageMetadataNotAFile(p, m => new OfflineMetaDataIOException(p, m)),
                    p => logger.OfflinePackageMetadataDirectoryNotFound(p, m => new OfflineMetaDataIOException(p, m)));
            existingPackages = [];
        }
        else
        {
            var fileInfo = GetExistingFileInfo(targetMetadataInput,
                (p, e) => logger.PackageMetadataFileAccessDenied(bundlePath, m => new OfflineMetaDataIOException(p, m, e), e),
                (p, e) => logger.PackageMetadataOpenError(bundlePath, m => new OfflineMetaDataIOException(p, m, e), e),
                p => logger.OfflinePackageMetadataNotAFile(p, m => new OfflineMetaDataIOException(p, m)),
                p => logger.OfflinePackageMetadataDirectoryNotFound(p, m => new OfflineMetaDataIOException(p, m)));
            cancellationToken.ThrowIfCancellationRequested();
            existingPackages = new(LoadOfflinePackageMetadata(fileInfo.FullName, logger));
            if (string.IsNullOrWhiteSpace(targetMetaDataOutput))
                targetMetadataFileInfo = fileInfo;
            else
                targetMetadataFileInfo = GetFileInfo(targetMetaDataOutput,
                    (p, e) => logger.PackageMetadataFileAccessDenied(bundlePath, m => new OfflineMetaDataIOException(p, m, e), e),
                    (p, e) => logger.PackageMetadataOpenError(bundlePath, m => new OfflineMetaDataIOException(p, m, e), e),
                    p => logger.OfflinePackageMetadataNotAFile(p, m => new OfflineMetaDataIOException(p, m)),
                    p => logger.OfflinePackageMetadataDirectoryNotFound(p, m => new OfflineMetaDataIOException(p, m)));
        }
        return bundleFileInfo;
    }

    private static async Task ExportNupkgAsync(string id, NuGetVersion version, TempStagingFolder tempStagingFolder, VersionFolderPathResolver pathResolver, IDownloadedPackagesService downloadedPackagesService, ILogger logger, CancellationToken cancellationToken)
    {
        var fileName = pathResolver.GetPackageFileName(id, version);
        var fullName = Path.Combine(tempStagingFolder.Directory.FullName, fileName);
        using var stream = OpenFileStream(fullName, FileMode.Create, FileAccess.Write, FileShare.None,
                (p, e) => logger.PackageExportAccessDenied(p, m => new OfflineMetaDataIOException(p, m, e), e),
                (p, e) => logger.PackageExportWriteError(p, m => new OfflineMetaDataIOException(p, m, e), e));
        await downloadedPackagesService.CopyNupkgToStreamAsync(id, version, stream, cancellationToken);
        await stream.FlushAsync(cancellationToken);
    }

    private static Task CreateBundleFileFileAsync(DirectoryInfo directory, FileInfo bundleFileInfo, ILogger logger, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.CreateBundleFileFileAsync not implemented"));
    }

    private static Task SaveOfflineMetaDataAsync(List<OfflinePackageMetadata> existingTargetPackages, FileInfo targetMetadataFileInfo, FileInfo bundleFileInfo, CancellationToken cancellationToken)
    {
        // try
        // {
        //     // TODO: Save changes
        // }
        // catch (Exception exception)
        // {
        //     try
        //     {
        //         bundleFileInfo.Refresh();
        //         if (bundleFileInfo.Exists)
        //             bundleFileInfo.Delete();
        //     }
        //     catch (Exception e) { throw new AggregateException(exception, e); }
        //     throw;
        // }
        return Task.FromException(new NotImplementedException());
    }

    public static async Task<int> ExportBundleAsync(string bundlePath, string? targetMetadataInput, string? targetMetaDataOutput, IDownloadedPackagesService downloadedPackagesService, ILogger logger, CancellationToken cancellationToken)
    {
        var bundleFileInfo = GetExportBundleFileInfos(bundlePath, targetMetadataInput, targetMetaDataOutput, logger, cancellationToken, out FileInfo targetMetadataFileInfo,
            out List<OfflinePackageMetadata> existingTargetPackages);
        Dictionary<string, List<IPackageSearchMetadata>> allPackages = new(PackageIdentitifierComparer);
        await foreach (var pkg in downloadedPackagesService.GetAllPackagesAsync(cancellationToken))
        {
            var id = pkg.Identity.Id;
            if (allPackages.TryGetValue(id, out var list))
                list.Add(pkg);
            else
                allPackages.Add(id, [pkg]);
        }
        if (allPackages.Count == 0)
        {
            logger.NoDownloadedPackagesPackagesExist();
            return -1;
        }
        List<IPackageSearchMetadata> toExport = [];
        var startCount = existingTargetPackages.Count;
        foreach (var pair in allPackages)
        {
            IEnumerable<IPackageSearchMetadata> packageSearchMetaData = pair.Value;
            var id = pair.Key;
            IEnumerable<IPackageSearchMetadata> withVersions = packageSearchMetaData.Where(p => p.Identity.HasVersion).OrderByDescending(p => p.Identity.Version, VersionComparer.VersionReleaseMetadata);
            if (!withVersions.Any())
                continue;
            packageSearchMetaData = withVersions.Concat(packageSearchMetaData.Where(p => !p.Identity.HasVersion));
            var existing = existingTargetPackages.FirstOrDefault(i => PackageIdentitifierComparer.Equals(i.Identifier, id));
            if (existing is null)
            {
                toExport.AddRange(withVersions);
                existing = new(withVersions.First());
                if (packageSearchMetaData.Skip(1).Any())
                {
                    existing.Title ??= packageSearchMetaData.Select(p => p.Title).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));
                    existing.Summary ??= packageSearchMetaData.Select(p => p.Summary).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));
                    existing.Description ??= packageSearchMetaData.Select(p => p.Description).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));
                    existing.Versions.AddRange(withVersions.Skip(1).Select(p => p.Identity.Version));
                }
                existingTargetPackages.Add(existing);
            }
            else if ((withVersions = withVersions.Where(p => !existing.Versions.Contains(p.Identity.Version, VersionComparer.VersionReleaseMetadata))).Any())
            {
                toExport.AddRange(withVersions);
                existing.Versions.AddRange(withVersions.Skip(1).Select(p => p.Identity.Version));
                existing.Versions.Sort(InvertedNuGetVersionReleaseMetadataComparer);
            }
        }
        if (toExport.Count == 0)
            return 0;
        if (existingTargetPackages.Count > startCount)
            existingTargetPackages.Sort();
        using TempStagingFolder tempStagingFolder = new();
        var pathResolver = new VersionFolderPathResolver(tempStagingFolder.Directory.FullName);
        foreach (var identity in toExport.Select(p => p.Identity))
            await ExportNupkgAsync(identity.Id, identity.Version, tempStagingFolder, pathResolver, downloadedPackagesService, logger, cancellationToken);
        await CreateBundleFileFileAsync(tempStagingFolder.Directory, bundleFileInfo, logger, cancellationToken);
        await SaveOfflineMetaDataAsync(existingTargetPackages, targetMetadataFileInfo, bundleFileInfo, cancellationToken);
        return toExport.Count;
    }

    public static async Task<int> ExportBundleAsync(string bundlePath, string? targetMetadataInput, string? targetMetaDataOutput, string[] packageIds, IDownloadedPackagesService downloadedPackagesService, ILogger logger, CancellationToken cancellationToken)
    {
        var bundleFileInfo = GetExportBundleFileInfos(bundlePath, targetMetadataInput, targetMetaDataOutput, logger, cancellationToken, out FileInfo targetMetadataFileInfo,
            out List<OfflinePackageMetadata> existingTargetPackages);
        List<IPackageSearchMetadata> toExport = [];
        var startCount = existingTargetPackages.Count;
        bool packageNotFound = false;
        foreach (string id in packageIds)
        {
            var packageSearchMetaData = await downloadedPackagesService.GetMetadataAsync(id, cancellationToken);
            if (packageSearchMetaData is null || !packageSearchMetaData.Any())
            {
                packageNotFound = true;
                logger.NuGetPackageNotFound(id, downloadedPackagesService);
            }
            else
            {
                IEnumerable<IPackageSearchMetadata> withVersions = packageSearchMetaData.Where(p => p.Identity.HasVersion).OrderByDescending(p => p.Identity.Version, VersionComparer.VersionReleaseMetadata);
                if (!withVersions.Any())
                {
                    packageNotFound = true;
                    logger.NuGetPackageNotFound(id, downloadedPackagesService);
                    continue;
                }
                packageSearchMetaData = withVersions.Concat(packageSearchMetaData.Where(p => !p.Identity.HasVersion));
                var existing = existingTargetPackages.FirstOrDefault(i => PackageIdentitifierComparer.Equals(i.Identifier, id));
                if (existing is null)
                {
                    toExport.AddRange(withVersions);
                    existing = new(withVersions.First());
                    if (packageSearchMetaData.Skip(1).Any())
                    {
                        existing.Title ??= packageSearchMetaData.Select(p => p.Title).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));
                        existing.Summary ??= packageSearchMetaData.Select(p => p.Summary).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));
                        existing.Description ??= packageSearchMetaData.Select(p => p.Description).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));
                        existing.Versions.AddRange(withVersions.Skip(1).Select(p => p.Identity.Version));
                    }
                    existingTargetPackages.Add(existing);
                }
                else if ((withVersions = withVersions.Where(p => !existing.Versions.Contains(p.Identity.Version, VersionComparer.VersionReleaseMetadata))).Any())
                {
                    toExport.AddRange(withVersions);
                    existing.Versions.AddRange(withVersions.Skip(1).Select(p => p.Identity.Version));
                    existing.Versions.Sort(InvertedNuGetVersionReleaseMetadataComparer);
                }
            }
        }
        if (toExport.Count == 0)
            return packageNotFound ? -1 : 0;
        if (existingTargetPackages.Count > startCount)
            existingTargetPackages.Sort();
        using TempStagingFolder tempStagingFolder = new();
        var pathResolver = new VersionFolderPathResolver(tempStagingFolder.Directory.FullName);
        foreach (var identity in toExport.Select(p => p.Identity))
            await ExportNupkgAsync(identity.Id, identity.Version, tempStagingFolder, pathResolver, downloadedPackagesService, logger, cancellationToken);
        await CreateBundleFileFileAsync(tempStagingFolder.Directory, bundleFileInfo, logger, cancellationToken);
        await SaveOfflineMetaDataAsync(existingTargetPackages, targetMetadataFileInfo, bundleFileInfo, cancellationToken);
        return toExport.Count;
    }

    [Obsolete("Only one package with multiple versions")]
    public static Task ExportBundleAsync(string bundlePath, string? targetMetadataInput, string? targetMetaDataOutput, string[] packageIds, NuGetVersion[] versions, IDownloadedPackagesService downloadedPackagesService, ILogger logger, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotSupportedException());
    }

    public static async Task<int> ExportBundleAsync(string bundlePath, string? targetMetadataInput, string? targetMetaDataOutput, string packageId, IEnumerable<NuGetVersion> versions, IDownloadedPackagesService downloadedPackagesService, ILogger logger, CancellationToken cancellationToken)
    {
        var bundleFileInfo = GetExportBundleFileInfos(bundlePath, targetMetadataInput, targetMetaDataOutput, logger, cancellationToken, out FileInfo targetMetadataFileInfo,
            out List<OfflinePackageMetadata> existingTargetPackages);
        var packageSearchMetaData = await downloadedPackagesService.GetMetadataAsync(packageId, cancellationToken);
        if (packageSearchMetaData is null || !packageSearchMetaData.Any())
        {
            logger.NuGetPackageNotFound(packageId, downloadedPackagesService);
            return -1;
        }
        var withVersions = packageSearchMetaData.Where(p => p.Identity.HasVersion).OrderByDescending(p => p.Identity.Version, VersionComparer.VersionReleaseMetadata);
        packageSearchMetaData = withVersions.Concat(packageSearchMetaData.Where(p => !p.Identity.HasVersion));
        var existing = existingTargetPackages.FirstOrDefault(p => PackageIdentitifierComparer.Equals(p.Identifier, packageId));
        List<IPackageSearchMetadata> toExport = [];
        if (existing is null)
        {
            if (!withVersions.Any())
            {
                foreach (var v in versions)
                    logger.NuGetPackageNotFound(packageId, v, downloadedPackagesService);
                return -1;
            }
            foreach (var v in versions)
            {
                var item = withVersions.FirstOrDefault(p => VersionComparer.VersionReleaseMetadata.Equals(p.Identity.Version, v));
                if (item is null)
                    logger.NuGetPackageNotFound(packageId, v, downloadedPackagesService);
                else
                    toExport.Add(item);
            }
            if (toExport.Count == 0)
                return -1;
            if (toExport.Count > 1)
                toExport.Sort((x, y) => InvertedNuGetVersionReleaseMetadataComparer.Compare(x.Identity.Version, y.Identity.Version));
            existing = new(toExport[0]);
            if (packageSearchMetaData.Skip(1).Any())
            {
                existing.Title ??= packageSearchMetaData.Select(p => p.Title).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));
                existing.Summary ??= packageSearchMetaData.Select(p => p.Summary).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));
                existing.Description ??= packageSearchMetaData.Select(p => p.Description).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));
                if (toExport.Count > 1)
                    existing.Versions.AddRange(toExport.Skip(1).Select(p => p.Identity.Version));
            }
            existingTargetPackages.Add(existing);
            existingTargetPackages.Sort();
        }
        else
        {
            if (!(versions = versions.Where(v => !existing.Versions.Contains(v, VersionComparer.VersionReleaseMetadata))).Any())
                return 0;
            foreach (var v in versions)
            {
                var item = withVersions.FirstOrDefault(p => VersionComparer.VersionReleaseMetadata.Equals(p.Identity.Version, v));
                if (item is null)
                    logger.NuGetPackageNotFound(packageId, v, downloadedPackagesService);
                else
                    toExport.Add(item);
            }
            if (toExport.Count == 0)
                return -1;
            existing.Versions.AddRange(toExport.Skip(1).Select(p => p.Identity.Version));
            existing.Versions.Sort(InvertedNuGetVersionReleaseMetadataComparer);
        }
        using TempStagingFolder tempStagingFolder = new();
        var pathResolver = new VersionFolderPathResolver(tempStagingFolder.Directory.FullName);
        foreach (var identity in toExport.Select(p => p.Identity))
            await ExportNupkgAsync(identity.Id, identity.Version, tempStagingFolder, pathResolver, downloadedPackagesService, logger, cancellationToken);
        await CreateBundleFileFileAsync(tempStagingFolder.Directory, bundleFileInfo, logger, cancellationToken);
        await SaveOfflineMetaDataAsync(existingTargetPackages, targetMetadataFileInfo, bundleFileInfo, cancellationToken);
        return toExport.Count;
    }

    public static async Task ExportDownloadedPackageManifestAsync(IEnumerable<IPackageSearchMetadata> packages, string exportPath, ILogger logger, CancellationToken cancellationToken)
    {
        using var writer = OpenStreamWriter(exportPath,
            (p, e) => logger.MetaDataExportPathAccessDenied(p, m => new OfflineMetaDataIOException(p, m, e), e),
            (p, e) => logger.InvalidMetaDataExportPath(p, m => new OfflineMetaDataIOException(p, m, e), e));
        await OfflinePackageMetadataConverter.ExportDownloadedPackageManifestAsync(packages, writer, cancellationToken);
        await writer.FlushAsync(cancellationToken);
        writer.Close();
    }

    public static async Task AddToDownloadedPackagesFolderFromRemote(string packageId, Dictionary<string, HashSet<NuGetVersion>> packagesAdded, IDownloadedPackagesService downloadedPackagesService, IUpstreamNuGetClientService upstreamClientService, ILogger logger, CancellationToken cancellationToken)
    {
        var upstreamVersions = await downloadedPackagesService.GetAllVersionsAsync(packageId, cancellationToken);
        if (upstreamVersions is not null && upstreamVersions.Any())
        {
            logger.NuGetPackageAlreadyAdded(packageId);
            return;
        }
        if ((upstreamVersions = await upstreamClientService.GetAllVersionsAsync(packageId, cancellationToken)) is null || !upstreamVersions.Any())
        {
            logger.NuGetPackageNotFound(packageId, upstreamClientService);
            return;
        }
        if (packagesAdded.TryGetValue(packageId, out HashSet<NuGetVersion>? versionsAdded))
        {
            if (!(upstreamVersions = upstreamVersions.Where(v => !versionsAdded.Contains(v))).Any())
                return;
            foreach (NuGetVersion v in upstreamVersions)
                versionsAdded.Add(v);
        }
        else
        {
            versionsAdded = new(upstreamVersions, VersionComparer.VersionReleaseMetadata);
            packagesAdded.Add(packageId, versionsAdded);
        }
        using TempStagingFolder tempStagingFolder = new();
        var pathResolver = new VersionFolderPathResolver(tempStagingFolder.Directory.FullName);
        foreach (NuGetVersion v in upstreamVersions)
        {
            FileInfo packageFile;
            try
            {
                packageFile = await tempStagingFolder.NewFileInfoAsync(pathResolver.GetPackageFileName(packageId, v), async (stream, token) =>
                {
                    await upstreamClientService.CopyNupkgToStreamAsync(packageId, v, stream, token);
                }, cancellationToken);
            }
            catch (Exception error)
            {
                logger.UnexpectedPackageDownloadFailure(packageId, v, error);
                continue;
            }
            if (packageFile.Length > 0)
                await downloadedPackagesService.AddPackageAsync(packageFile.FullName, false, cancellationToken);
            else
                logger.DownloadPackageIsEmpty(packageId, v);
        }
    }

    public static Task CheckDependenciesAsync(IDownloadedPackagesService downloadedPackagesService, ILogger logger, string[] packageIds, NuGetVersion[] versions, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.CheckDependenciesAsync(IDownloadedPackagesService, ILogger, string[], NuGetVersion[], CancellationToken) not implemented"));
    }

    public static Task CheckDependenciesAsync(IDownloadedPackagesService downloadedPackagesService, ILogger logger, string[] packageIds, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.ExportBundleAsync(IDownloadedPackagesService, ILogger, string[], CancellationToken) not implemented"));
    }

    public static Task CheckAllDependenciesAsync(IDownloadedPackagesService downloadedPackagesService, ILogger logger, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.CheckAllDependenciesAsync not implemented"));
    }

    public static Task DownloadMissingDependenciesAsync(IDownloadedPackagesService downloadedPackagesService, IUpstreamNuGetClientService upstreamClientService, ILogger logger, string[] packageIds, NuGetVersion[] nuGetVersions, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.ExportBundleAsync not implemented"));
    }

    public static Task DownloadMissingDependenciesAsync(IDownloadedPackagesService downloadedPackagesService, IUpstreamNuGetClientService upstreamClientService, ILogger logger, string[] packageIds, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.ExportBundleAsync not implemented"));
    }

    public static Task DownloadAllMissingDependenciesAsync(IDownloadedPackagesService downloadedPackagesService, IUpstreamNuGetClientService upstreamClientService, ILogger logger, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.ExportBundleAsync not implemented"));
    }

    public Task DeletePackagesAsync(IDownloadedPackagesService downloadedPackagesService, ILogger logger, string[] packageIds, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.ExportBundleAsync not implemented"));
    }

    public Task DeletePackageVersionsAsync(IDownloadedPackagesService downloadedPackagesService, ILogger logger, string[] packageIds, NuGetVersion[] nuGetVersions, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.ExportBundleAsync not implemented"));
    }
}
