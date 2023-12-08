using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using static NuGetPuller.CommonStatic;
using static NuGetPuller.MainServiceStatic;

namespace NuGetPuller.CLI;

public class MainService : BackgroundService
{
    private readonly ILogger<MainService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IOptions<AppSettings> _settingsOptions;

    public MainService(ILogger<MainService> logger, IServiceProvider serviceProvider, IOptions<AppSettings> settingsOptions, IHostApplicationLifetime applicationLifetime) =>
        (_logger, _serviceProvider, _applicationLifetime, _settingsOptions) = (logger, serviceProvider, applicationLifetime, settingsOptions);

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var localClientService = scope.ServiceProvider.GetRequiredService<LocalClientService>();
            var upstreamClientService = scope.ServiceProvider.GetRequiredService<UpstreamClientService>();
            var validatedSettings = scope.ServiceProvider.GetRequiredService<ValidatedAppSettings>();
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
                    await AddToLocalFromRemote(id, packagesAdded, localClientService, upstreamClientService, _logger, stoppingToken);
            }
            if (validatedSettings.Import is not null)
                throw new NotImplementedException();
            // await ImportAsync(validatedSettings.ImportPath, localClientService, _logger, stoppingToken);
            if (appSettings.UpdateAll)
            {
                var asyncEn = localClientService.GetAllPackagesAsync(stoppingToken);
                if (!(packageIds = asyncEn.ToBlockingEnumerable(stoppingToken).Select(p => p.Identity.Id)).Any())
                    _logger.LogNoLocalPackagesExist();
                else
                    await UpdateLocalFromRemote(packageIds, localClientService, upstreamClientService, _logger, stoppingToken);
            }
            else if ((packageIds = appSettings.Update?.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).Distinct(NoCaseComparer)) is not null && packageIds.Any())
                await UpdateLocalFromRemote(packageIds, localClientService, upstreamClientService, _logger, stoppingToken);
            if (appSettings.ListLocal)
                await ListLocalPackagesAsync(localClientService.GetAllPackagesAsync(stoppingToken).ToBlockingEnumerable(stoppingToken), validatedSettings.ExportLocalManifest?.FullName, _logger, stoppingToken);
            if (validatedSettings.ExportLocalManifest is not null)
                await ExportLocalManifestAsync(localClientService.GetAllPackagesAsync(stoppingToken).ToBlockingEnumerable(stoppingToken), validatedSettings.ExportLocalManifest.FullName, _logger, stoppingToken);
            if (validatedSettings.ExportBundle is not null)
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
