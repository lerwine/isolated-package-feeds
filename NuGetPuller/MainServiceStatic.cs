using Microsoft.Extensions.Logging;
using IsolatedPackageFeeds.Shared;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Newtonsoft.Json;

namespace NuGetPuller;

public partial class MainServiceStatic
{
    public static readonly StringComparer PackageIdentityComparer = StringComparer.OrdinalIgnoreCase;

    public static readonly JsonSerializerSettings MetadataDeSerializationSettings = new()
    {
        MaxDepth = 512,
        NullValueHandling = NullValueHandling.Ignore,
        TypeNameHandling = TypeNameHandling.None,
        Converters = [new OfflinePackageManifestConverter()],
        Formatting = Formatting.Indented
    };

    /// <summary>
    /// Opens a new <see cref="StreamWriter"/> for a given path.
    /// </summary>
    /// <param name="path">The path of the file to create.</param>
    /// <param name="logger">The logger for writing errors.</param>
    /// <returns>A writer for writing JSON output.</returns>
    /// <exception cref="MetaDataExportPathException"><paramref name="path"/> was invalid, not found, or there was an access error.</exception>
    public static StreamWriter OpenPackageMetaDataWriter(string path, ILogger logger)
    {
        try { return new StreamWriter(path, false, new System.Text.UTF8Encoding(false, false)); }
        catch (ArgumentException exception)
        {
            throw logger.InvalidLocalMetaDataExportPath(path, m => new MetaDataExportPathException(path, m, exception), exception);
        }
        catch (UnauthorizedAccessException exception)
        {
            throw logger.MetaDataExportPathAccessDenied(path, m => new MetaDataExportPathException(path, m, exception), exception);
        }
        catch (DirectoryNotFoundException exception)
        {
            throw logger.InvalidLocalMetaDataExportPath(path, m => new MetaDataExportPathException(path, m, exception), exception);
        }
        catch (PathTooLongException exception)
        {
            throw logger.InvalidLocalMetaDataExportPath(path, m => new MetaDataExportPathException(path, m, exception), exception);
        }
        catch (IOException exception)
        {
            throw logger.InvalidLocalMetaDataExportPath(path, m => new MetaDataExportPathException(path, m, exception), exception);
        }
        catch (System.Security.SecurityException exception)
        {
            throw logger.MetaDataExportPathAccessDenied(path, m => new MetaDataExportPathException(path, m, exception), exception);
        }
    }

    public static Task ExportBundleAsync(string bundlePath, string? targetManifestInput, string? targetManifestOutput, ILocalNuGetFeedService localClientService, ILogger logger, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.ExportBundleAsync(string bundlePath, string?, string?, ILocalNuGetFeedService, ILogger, CancellationToken) not implemented"));
    }

    public static Task ExportBundleAsync(string bundlePath, string? targetManifestInput, string? targetManifestOutput, string[] packageIds, ILocalNuGetFeedService localClientService, ILogger logger, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.ExportBundleAsync(string bundlePath, string?, string?, string[], ILocalNuGetFeedService, ILogger, CancellationToken) not implemented"));
    }

    public static Task ExportBundleAsync(string bundlePath, string? targetManifestInput, string? targetManifestOutput, string[] packageIds, NuGetVersion[] versions, ILocalNuGetFeedService localClientService, ILogger logger, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.ExportBundleAsync(string bundlePath, string?, string?, string[], NuGetVersion[], ILocalNuGetFeedService, ILogger, CancellationToken) not implemented"));
    }

    public static async Task ExportLocalManifestAsync(IEnumerable<IPackageSearchMetadata> packages, string exportPath, ILogger logger, CancellationToken cancellationToken)
    {
        using var writer = OpenPackageMetaDataWriter(exportPath, logger);
        await OfflinePackageManifestConverter.ExportLocalManifestAsync(packages, writer, cancellationToken);
        await writer.FlushAsync(cancellationToken);
        writer.Close();
    }

    public static async Task AddToLocalFromRemote(string packageId, Dictionary<string, HashSet<NuGetVersion>> packagesAdded, ILocalNuGetFeedService localClientService, IUpstreamNuGetClientService upstreamClientService, ILogger logger, CancellationToken cancellationToken)
    {
        var upstreamVersions = await localClientService.GetAllVersionsAsync(packageId, cancellationToken);
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
                await localClientService.AddPackageAsync(packageFile.FullName, false, cancellationToken);
            else
                logger.DownloadPackageIsEmpty(packageId, v);
        }
    }

    public static Task CheckDependenciesAsync(ILocalNuGetFeedService localClientService, ILogger logger, string[] packageIds, NuGetVersion[] versions, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.CheckDependenciesAsync(ILocalNuGetFeedService, ILogger, string[], NuGetVersion[], CancellationToken) not implemented"));
    }

    public static Task CheckDependenciesAsync(ILocalNuGetFeedService localClientService, ILogger logger, string[] packageIds, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.ExportBundleAsync(ILocalNuGetFeedService, ILogger, string[], CancellationToken) not implemented"));
    }

    public static Task CheckAllDependenciesAsync(ILocalNuGetFeedService localClientService, ILogger logger, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.CheckAllDependenciesAsync not implemented"));
    }

    public static Task DownloadMissingDependenciesAsync(ILocalNuGetFeedService localClientService, IUpstreamNuGetClientService upstreamClientService, ILogger logger, string[] packageIds, NuGetVersion[] nuGetVersions, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.ExportBundleAsync not implemented"));
    }

    public static Task DownloadMissingDependenciesAsync(ILocalNuGetFeedService localClientService, IUpstreamNuGetClientService upstreamClientService, ILogger logger, string[] packageIds, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.ExportBundleAsync not implemented"));
    }

    public static Task DownloadAllMissingDependenciesAsync(ILocalNuGetFeedService localClientService, IUpstreamNuGetClientService upstreamClientService, ILogger logger, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.ExportBundleAsync not implemented"));
    }

    public Task DeletePackagesAsync(ILocalNuGetFeedService localClientService, ILogger logger, string[] packageIds, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.ExportBundleAsync not implemented"));
    }

    public Task DeletePackageVersionsAsync(ILocalNuGetFeedService localClientService, ILogger logger, string[] packageIds, NuGetVersion[] nuGetVersions, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException("NuGetPuller.MainServiceStatic.ExportBundleAsync not implemented"));
    }
}
