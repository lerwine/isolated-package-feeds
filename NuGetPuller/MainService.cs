using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Linq;
using static NuGetPuller.Constants;

namespace NuGetPuller;

/// <summary>
/// Main application service that performs actions according to command line arguments.
/// </summary>
public class MainService : BackgroundService
{
    private readonly ILogger<MainService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IOptions<AppSettings> _settingsOptions;

    public MainService(ILogger<MainService> logger, IServiceProvider serviceProvider, IOptions<AppSettings> settingsOptions, IHostApplicationLifetime applicationLifetime) =>
        (_logger, _serviceProvider, _applicationLifetime, _settingsOptions) = (logger, serviceProvider, applicationLifetime, settingsOptions);

    private StreamWriter OpenPackageMetaDataWriterAsync(string path)
    {
        try { return new StreamWriter(path, false, new System.Text.UTF8Encoding(false, false)); }
        catch (ArgumentException exception)
        {
            throw _logger.LogInvalidExportLocalMetaData(path, m => new MetaDataExportPathException(path, m, exception), exception);
        }
        catch (UnauthorizedAccessException exception)
        {
            throw _logger.LogMetaDataExportPathAccessDenied(path, m => new MetaDataExportPathException(path, m, exception), exception);
        }
        catch (DirectoryNotFoundException exception)
        {
            throw _logger.LogInvalidExportLocalMetaData(path, m => new MetaDataExportPathException(path, m, exception), exception);
        }
        catch (PathTooLongException exception)
        {
            throw _logger.LogInvalidExportLocalMetaData(path, m => new MetaDataExportPathException(path, m, exception), exception);
        }
        catch (IOException exception)
        {
            throw _logger.LogInvalidExportLocalMetaData(path, m => new MetaDataExportPathException(path, m, exception), exception);
        }
        catch (System.Security.SecurityException exception)
        {
            throw _logger.LogMetaDataExportPathAccessDenied(path, m => new MetaDataExportPathException(path, m, exception), exception);
        }
    }

    private async Task ListLocalPackagesAsync(IEnumerable<IPackageSearchMetadata> packages, string? exportPath, CancellationToken cancellationToken)
    {
        var pkgArr = packages.ToArray();
        switch (pkgArr.Length)
        {
            case 0:
                Console.WriteLine("0 packages in local NuGet source.");
                if (exportPath is not null)
                {
                    using var writer = OpenPackageMetaDataWriterAsync(exportPath);
                    await writer.WriteLineAsync("[]");
                    await writer.FlushAsync();
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
            using var writer = OpenPackageMetaDataWriterAsync(exportPath);
            await writer.WriteLineAsync('[');
            foreach (var p in pkgArr.SkipLast(1))
            {
                await writer.WriteLineAsync($"{p.ToJson()},");
                Console.WriteLine("{0}: {1}", p.Identity.Id, p.Title);
            }
            await writer.WriteLineAsync(pkgArr.Last().ToJson());
            await writer.WriteLineAsync(']');
            await writer.FlushAsync();
            writer.Close();
        }
    }

    private async Task ExportLocalPackageMetaDataAsync(IEnumerable<IPackageSearchMetadata> packages, string exportPath, CancellationToken cancellationToken)
    {
        var pkgArr = packages.ToArray();
        using var writer = OpenPackageMetaDataWriterAsync(exportPath);
        if (pkgArr.Length > 0)
        {
            await writer.WriteLineAsync('[');
            foreach (var p in pkgArr.SkipLast(1))
                await writer.WriteLineAsync($"{p.ToJson()},");
            await writer.WriteLineAsync(pkgArr.Last().ToJson());
            await writer.WriteLineAsync(']');
        }
        else
            await writer.WriteLineAsync("[]");
        await writer.FlushAsync();
        writer.Close();
    }

    private async Task AddToLocalFromRemote(string packageId, Dictionary<string, HashSet<NuGetVersion>> packagesAdded, LocalClientService localClientService, UpstreamClientService upstreamClientService, CancellationToken cancellationToken)
    {
        var upstreamVersions = await localClientService.GetAllVersionsAsync(packageId, cancellationToken);
        if (upstreamVersions is not null && upstreamVersions.Any())
        {
            _logger.LogPackageAlreadyAdded(packageId);
            return;
        }
        if ((upstreamVersions = await upstreamClientService.GetAllVersionsAsync(packageId, cancellationToken)) is null || !upstreamVersions.Any())
        {
            _logger.LogPackageNotFound(packageId, upstreamClientService, true);
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
        var tempFile = new FileInfo(Path.GetTempFileName());
        try
        {
            foreach (NuGetVersion v in upstreamVersions)
            {
                using FileStream stream = new(tempFile.FullName, FileMode.Create, FileAccess.Write);
                try { await upstreamClientService.CopyNupkgToStreamAsync(packageId, v, stream, cancellationToken); }
                catch (Exception error)
                {
                    _logger.LogUnexpectedPackageDownloadFailure(packageId, v, error);
                    continue;
                }
                finally
                {
                    stream.Flush();
                    stream.Close();
                }
                tempFile.Refresh();
                if (tempFile.Length > 0)
                {
                    if (!await localClientService.AddPackageAsync(tempFile.FullName, false, cancellationToken))
                        _logger.LogUnexpectedAddFailure(packageId, v);
                }
                else
                    _logger.LogEmptyPackageDownload(packageId, v);
            }
        }
        finally
        {
            tempFile.Refresh();
            if (tempFile.Exists)
                tempFile.Delete();
        }
    }

    private async Task UpdateLocalFromRemote(IEnumerable<string> packageIds, LocalClientService localClientService, UpstreamClientService upstreamClientService, CancellationToken stoppingToken)
    {
        var asyncEn = upstreamClientService.ExpandPackagesWithDependenciesAsync(packageIds, stoppingToken).GetAsyncEnumerator(stoppingToken);
        if (!await asyncEn.MoveNextAsync())
            return;
        var tempFile = new FileInfo(Path.GetTempFileName());
        try
        {
            do
            {
                var identity = asyncEn.Current;
                if (await localClientService.DoesPackageExistAsync(identity.Id, identity.Version, stoppingToken))
                    continue;
                using FileStream stream = new(tempFile.FullName, FileMode.Create, FileAccess.Write);
                try { await upstreamClientService.CopyNupkgToStreamAsync(identity.Id, identity.Version, stream, stoppingToken); }
                catch (Exception error)
                {
                    _logger.LogUnexpectedPackageDownloadFailure(identity.Id, identity.Version, error);
                    continue;
                }
                finally
                {
                    stream.Flush();
                    stream.Close();
                }
                tempFile.Refresh();
                if (tempFile.Length > 0)
                {
                    if (!await localClientService.AddPackageAsync(tempFile.FullName, false, stoppingToken))
                        _logger.LogUnexpectedAddFailure(identity.Id, identity.Version);
                }
                else
                    _logger.LogEmptyPackageDownload(identity.Id, identity.Version);
            }
            while (!await asyncEn.MoveNextAsync());
        }
        finally
        {
            tempFile.Refresh();
            if (tempFile.Exists)
                tempFile.Delete();
        }
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var localClientService = scope.ServiceProvider.GetRequiredService<LocalClientService>();
            var upstreamClientService = scope.ServiceProvider.GetRequiredService<UpstreamClientService>();
            var appSettings = _settingsOptions.Value;
            var packageIds = appSettings.Delete?.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).Distinct(NoCaseComparer);
            HashSet<PackageIdentity> deletedPackages = new(PackageIdentityComparer.Default);
            if (packageIds is not null && packageIds.Any())
                await foreach (var (package, success) in localClientService.DeleteAsync(packageIds, stoppingToken))
                    if (success)
                        deletedPackages.Add(package);
            if ((packageIds = appSettings.Add?.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).Distinct(NoCaseComparer)) is not null && packageIds.Any())
            {
                Dictionary<string, HashSet<NuGetVersion>> packagesAdded = new(NoCaseComparer);
                foreach (string id in packageIds)
                    await AddToLocalFromRemote(id, packagesAdded, localClientService, upstreamClientService, stoppingToken);
            }
            if (appSettings.ListLocal)
                await ListLocalPackagesAsync(localClientService.GetAllPackagesAsync(stoppingToken).ToBlockingEnumerable(stoppingToken), appSettings.Validated.ExportLocalMetaDataPath, stoppingToken);
            else if (appSettings.Validated.ExportLocalMetaDataPath is not null)
                await ExportLocalPackageMetaDataAsync(localClientService.GetAllPackagesAsync(stoppingToken).ToBlockingEnumerable(stoppingToken), appSettings.Validated.ExportLocalMetaDataPath, stoppingToken);
            else if (appSettings.UpdateAll)
            {
                var asyncEn = localClientService.GetAllPackagesAsync(stoppingToken);
                if ((packageIds = asyncEn.ToBlockingEnumerable(stoppingToken).Select(p => p.Identity.Id)) is null || !packageIds.Any())
                    _logger.LogNoLocalPackagesExist();
                else
                    await UpdateLocalFromRemote(packageIds, localClientService, upstreamClientService, stoppingToken);
            }
            else if ((packageIds = appSettings.Update?.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).Distinct(NoCaseComparer)) is not null && packageIds.Any())
                await UpdateLocalFromRemote(packageIds, localClientService, upstreamClientService, stoppingToken);
        }
        catch (OperationCanceledException) { throw; }
        finally
        {
            if (!stoppingToken.IsCancellationRequested)
                _applicationLifetime.StopApplication();
        }
    }
}