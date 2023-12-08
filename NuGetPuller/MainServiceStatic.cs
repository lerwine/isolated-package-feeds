using Microsoft.Extensions.Logging;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGetPuller;

public static class MainServiceStatic
{
    public static StreamWriter OpenPackageMetaDataWriter(string path, ILogger logger)
    {
        try { return new StreamWriter(path, false, new System.Text.UTF8Encoding(false, false)); }
        catch (ArgumentException exception)
        {
            throw logger.LogInvalidExportLocalMetaData(path, m => new MetaDataExportPathException(path, m, exception), exception);
        }
        catch (UnauthorizedAccessException exception)
        {
            throw logger.LogMetaDataExportPathAccessDenied(path, m => new MetaDataExportPathException(path, m, exception), exception);
        }
        catch (DirectoryNotFoundException exception)
        {
            throw logger.LogInvalidExportLocalMetaData(path, m => new MetaDataExportPathException(path, m, exception), exception);
        }
        catch (PathTooLongException exception)
        {
            throw logger.LogInvalidExportLocalMetaData(path, m => new MetaDataExportPathException(path, m, exception), exception);
        }
        catch (IOException exception)
        {
            throw logger.LogInvalidExportLocalMetaData(path, m => new MetaDataExportPathException(path, m, exception), exception);
        }
        catch (System.Security.SecurityException exception)
        {
            throw logger.LogMetaDataExportPathAccessDenied(path, m => new MetaDataExportPathException(path, m, exception), exception);
        }
    }

    public static async Task ListLocalPackagesAsync(IEnumerable<IPackageSearchMetadata> packages, string? exportPath, ILogger logger, CancellationToken cancellationToken)
    {
        var pkgArr = packages.ToArray();
        switch (pkgArr.Length)
        {
            case 0:
                Console.WriteLine("0 packages in local NuGet source.");
                if (exportPath is not null)
                {
                    using var writer = OpenPackageMetaDataWriter(exportPath, logger);
                    await writer.WriteLineAsync("[]");
                    await writer.FlushAsync(cancellationToken);
                    writer.Close();
                }
                return;
            case 1:
                Console.WriteLine("1 package in local NuGet source:");
                break;
            default:
                Console.WriteLine("{0} packages in local NuGet source:", pkgArr.Length);
                break;
        }
        if (exportPath is null)
        {
            foreach (var p in pkgArr)
                Console.WriteLine("{0}: {1}", p.Identity.Id, p.Title);
        }
        else
        {
            using var writer = OpenPackageMetaDataWriter(exportPath, logger);
            await writer.WriteLineAsync('[');
            foreach (var p in pkgArr.SkipLast(1))
            {
                await writer.WriteLineAsync($"{p.ToJson()},");
                Console.WriteLine("{0}: {1}", p.Identity.Id, p.Title);
            }
            await writer.WriteLineAsync(pkgArr.Last().ToJson());
            await writer.WriteLineAsync(']');
            await writer.FlushAsync(cancellationToken);
            writer.Close();
        }
    }

    // private async Task ImportAsync(string path, LocalClientService localClientService, ILogger logger, CancellationToken cancellationToken)
    // {
    //     throw new NotImplementedException();
    // }

    // private async Task ExportBundleAsync(string bundlePath, string targetManifestInput, string targetManifestOutput, LocalClientService localClientService, ILogger logger, CancellationToken cancellationToken)
    // {
    //     throw new NotImplementedException();
    // }

    public static async Task ExportLocalManifestAsync(IEnumerable<IPackageSearchMetadata> packages, string exportPath, ILogger logger, CancellationToken cancellationToken)
    {
        var pkgArr = packages.ToArray();
        using var writer = OpenPackageMetaDataWriter(exportPath, logger);
        if (pkgArr.Length > 0)
        {
            await writer.WriteLineAsync('[');
            foreach (var p in pkgArr.SkipLast(1))
                await writer.WriteLineAsync($"  {p.ToJson()},");
            await writer.WriteLineAsync($"  {pkgArr.Last().ToJson()}");
            await writer.WriteLineAsync(']');
        }
        else
            await writer.WriteLineAsync("[]");
        await writer.FlushAsync(cancellationToken);
        writer.Close();
    }

    public static async Task AddToLocalFromRemote(string packageId, Dictionary<string, HashSet<NuGetVersion>> packagesAdded, LocalClientServiceBase localClientService, UpstreamClientServiceBase upstreamClientService, ILogger logger, CancellationToken cancellationToken)
    {
        var upstreamVersions = await localClientService.GetAllVersionsAsync(packageId, cancellationToken);
        if (upstreamVersions is not null && upstreamVersions.Any())
        {
            logger.LogPackageAlreadyAdded(packageId);
            return;
        }
        if ((upstreamVersions = await upstreamClientService.GetAllVersionsAsync(packageId, cancellationToken)) is null || !upstreamVersions.Any())
        {
            logger.LogPackageNotFound(packageId, upstreamClientService);
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
                logger.LogUnexpectedPackageDownloadFailure(packageId, v, error);
                continue;
            }
            if (packageFile.Length > 0)
                await localClientService.AddPackageAsync(packageFile.FullName, false, cancellationToken);
            else
                logger.LogEmptyPackageDownload(packageId, v);
        }
    }

    public static async Task UpdateLocalFromRemote(IEnumerable<string> packageIds, LocalClientServiceBase localClientService, UpstreamClientServiceBase upstreamClientService, ILogger logger, CancellationToken cancellationToken)
    {
        var asyncEn = upstreamClientService.GetAllVersionsWithDependenciesAsync(packageIds, cancellationToken).GetAsyncEnumerator(cancellationToken);
        if (!await asyncEn.MoveNextAsync())
            return;
        using TempStagingFolder tempStagingFolder = new();
        var pathResolver = new VersionFolderPathResolver(tempStagingFolder.Directory.FullName);
        do
        {
            var identity = asyncEn.Current;
            if (await localClientService.DoesPackageExistAsync(identity.Id, identity.Version, cancellationToken))
                continue;
            FileInfo packageFile;
            using (var scope = logger.BeginGetDownloadResourceResultScope(identity, upstreamClientService))
                try
                {
                    logger.LogDownloadingNuGetPackage(identity, upstreamClientService);
                    packageFile = await tempStagingFolder.NewFileInfoAsync(pathResolver.GetPackageFileName(identity.Id, identity.Version), async (stream, token) =>
                    {
                        await upstreamClientService.CopyNupkgToStreamAsync(identity.Id, identity.Version, stream, cancellationToken);
                    }, cancellationToken);
                }
                catch (Exception error)
                {
                    logger.LogUnexpectedPackageDownloadFailure(identity.Id, identity.Version, error);
                    continue;
                }
            await localClientService.AddPackageAsync(packageFile.FullName, false, cancellationToken);
        }
        while (!await asyncEn.MoveNextAsync());
    }
}
