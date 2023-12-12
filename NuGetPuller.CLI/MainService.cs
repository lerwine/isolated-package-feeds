using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using static IsolatedPackageFeeds.Shared.CommonStatic;
using static NuGetPuller.MainServiceStatic;

namespace NuGetPuller.CLI;

public class MainService : BackgroundService
{
    private readonly AppSettings _settings;
    private readonly ILogger<MainService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public MainService(IOptions<AppSettings> options, ILogger<MainService> logger, IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime) =>
        (_settings, _logger, _serviceProvider, _applicationLifetime) = (options.Value, logger, serviceProvider, applicationLifetime);

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var validatedSettings = scope.ServiceProvider.GetRequiredService<ValidatedPathsService>();
            var packageIds = _settings.Delete?.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).Distinct(NoCaseComparer);
            HashSet<PackageIdentity> deletedPackages = new(PackageIdentityComparer.Default);
            if (packageIds is not null && packageIds.Any())
                await foreach (var (package, success) in scope.ServiceProvider.GetRequiredService<ILocalClientService>().DeleteAsync(packageIds, stoppingToken))
                    if (success)
                        deletedPackages.Add(package);
            if ((packageIds = _settings.Add?.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).Distinct(NoCaseComparer)) is not null && packageIds.Any())
            {
                Dictionary<string, HashSet<NuGetVersion>> packagesAdded = new(NoCaseComparer);
                var localClientService = scope.ServiceProvider.GetRequiredService<ILocalClientService>();
                foreach (string id in packageIds)
                    await AddToLocalFromRemote(id, packagesAdded, localClientService, scope.ServiceProvider.GetRequiredService<IUpstreamClientService>(), _logger, stoppingToken);
            }
            if (validatedSettings.Import.TryGetResult(out FileSystemInfo? importFrom))
                throw new NotImplementedException();
            // await ImportAsync(validatedSettings.ImportPath, localClientService, _logger, stoppingToken);
            if (_settings.UpdateAll)
            {
                var asyncEn = scope.ServiceProvider.GetRequiredService<ILocalClientService>().GetAllPackagesAsync(stoppingToken);
                if (!(packageIds = asyncEn.ToBlockingEnumerable(stoppingToken).Select(p => p.Identity.Id)).Any())
                    _logger.NoLocalPackagesExist();
                else
                    await scope.ServiceProvider.GetRequiredService<PackageUpdateService>().UpdatePackagesAsync(packageIds, stoppingToken);
            }
            else if ((packageIds = _settings.Update?.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).Distinct(NoCaseComparer)) is not null && packageIds.Any())
                await scope.ServiceProvider.GetRequiredService<PackageUpdateService>().UpdatePackagesAsync(packageIds, stoppingToken);
            if (_settings.ListLocal)
                await ListLocalPackagesAsync(scope.ServiceProvider.GetRequiredService<ILocalClientService>().GetAllPackagesAsync(stoppingToken).ToBlockingEnumerable(stoppingToken), validatedSettings.ExportLocalManifest.TryGetResult(out FileInfo? exportLocalManifest) ? exportLocalManifest.FullName : null, _logger, stoppingToken);
            if (validatedSettings.ExportLocalManifest.TryGetResult(out FileInfo? fileInfo))
                await ExportLocalManifestAsync(scope.ServiceProvider.GetRequiredService<ILocalClientService>().GetAllPackagesAsync(stoppingToken).ToBlockingEnumerable(stoppingToken), fileInfo.FullName, _logger, stoppingToken);
            if (validatedSettings.ExportBundle.TryGetResult(out fileInfo))
                throw new NotImplementedException();
            // await ExportBundleAsync(validatedSettings.ExportBundlePath, validatedSettings.TargetManifestFilePath, validatedSettings.TargetManifestSaveAsPath, localClientService, _logger, stoppingToken);
        }
        catch (OperationCanceledException) { throw; }
        finally
        {
            if (!stoppingToken.IsCancellationRequested)
                _applicationLifetime.StopApplication();
        }
    }
}
