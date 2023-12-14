using Microsoft.Extensions.Logging;
using IsolatedPackageFeeds.Shared;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGetPuller;

public partial class MainServiceStatic
{
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

    // private async Task ImportAsync(string path, LocalNuGetFeedService localClientService, ILogger logger, CancellationToken cancellationToken)
    // {
    //     throw new NotImplementedException();
    // }

    // private async Task ExportBundleAsync(string bundlePath, string targetManifestInput, string targetManifestOutput, LocalNuGetFeedService localClientService, ILogger logger, CancellationToken cancellationToken)
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

    [Obsolete("Use NuGetPuller.CLI.MainService.CheckIgnoredDependentCommandLineArgument")]
    public static void LogIgnoredDependentCommandLineArgumentIfSet(bool value, Func<(ILogger Logger, string DependentSwitch, string SwitchName)> getSwitchNames)
    {
        if (!value)
            return;
        (ILogger logger, string dependentSwitch, string switchName) = getSwitchNames();
        logger.IgnoredDependentCommandLineArgument(dependentSwitch, switchName);
    }
    
    [Obsolete("Use NuGetPuller.CLI.MainService.CheckIgnoredDependentCommandLineArgument")]
    public static void LogIgnoredDependentCommandLineArgumentIfSet(string? value, Func<(ILogger Logger, string DependentSwitch, string SwitchName)> getSwitchNames)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        (ILogger logger, string dependentSwitch, string switchName) = getSwitchNames();
        logger.IgnoredDependentCommandLineArgument(dependentSwitch, switchName);
    }
    
    [Obsolete("Use NuGetPuller.CLI.MainService.CheckIgnoredDependentCommandLineArgument")]
    public static void LogIgnoredDependentCommandLineArgumentIfSet(string? value, Func<(ILogger Logger, string DependentSwitch, string SwitchName1, string SwitchName2)> getSwitchNames)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        (ILogger logger, string dependentSwitch, string switchName1, string switchName2) = getSwitchNames();
        logger.IgnoredDependentCommandLineArgument(dependentSwitch, switchName1, switchName2);
    }
    
    [Obsolete("Use NuGetPuller.CLI.MainService.CheckIgnoredDependentCommandLineArgument")]
    public static void LogIgnoredDependentCommandLineArgumentIfSet(string? value, Func<(ILogger Logger, string DependentSwitch, string SwitchName1, string SwitchName2, string SwitchName3)> getSwitchNames)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        (ILogger logger, string dependentSwitch, string switchName1, string switchName2, string switchName3) = getSwitchNames();
        logger.IgnoredDependentCommandLineArgument(dependentSwitch, switchName1, switchName2, switchName3);
    }
    
    [Obsolete("Use NuGetPuller.CLI.MainService.CheckIgnoredDependentCommandLineArgument")]
    public static void LogIgnoredDependentCommandLineArgumentIfSet(string? value, Func<(ILogger Logger, string DependentSwitch, string SwitchName1, string SwitchName2, string SwitchName3, string SwitchName4)> getSwitchNames)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        (ILogger logger, string dependentSwitch, string switchName1, string switchName2, string switchName3, string switchName4) = getSwitchNames();
        logger.IgnoredDependentCommandLineArgument(dependentSwitch, switchName1, switchName2, switchName3, switchName4);
    }
    
    public static Task CheckDependenciesAsync(ILocalNuGetFeedService localClientService, ILogger logger, string[] packageIds, NuGetVersion[] nuGetVersions, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException());
    }

    public static Task CheckDependenciesAsync(ILocalNuGetFeedService localClientService, ILogger logger, string[] packageIds, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException());
    }

    public static Task CheckAllDependenciesAsync(ILocalNuGetFeedService localClientService, ILogger logger, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException());
    }

    public static Task DownloadMissingDependenciesAsync(ILocalNuGetFeedService localClientService, IUpstreamNuGetClientService upstreamClientService, ILogger logger, string[] packageIds, NuGetVersion[] nuGetVersions, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException());
    }

    public static Task DownloadMissingDependenciesAsync(ILocalNuGetFeedService localClientService, IUpstreamNuGetClientService upstreamClientService, ILogger logger, string[] packageIds, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException());
    }

    public static Task DownloadAllMissingDependenciesAsync(ILocalNuGetFeedService localClientService, IUpstreamNuGetClientService upstreamClientService, ILogger logger, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException());
    }

    public Task DeletePackagesAsync(ILocalNuGetFeedService localClientService, ILogger logger, string[] packageIds, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException());
    }

    public Task DeletePackageVersionsAsync(ILocalNuGetFeedService localClientService, ILogger logger, string[] packageIds, NuGetVersion[] nuGetVersions, CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException());
    }
}
