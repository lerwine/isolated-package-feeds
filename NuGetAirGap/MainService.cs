using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace NuGetAirGap
{
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

        private async Task ListLocalPackagesAsync(List<IPackageSearchMetadata> packages, string? exportPath, CancellationToken cancellationToken)
        {
            switch (packages.Count)
            {
                case 0:
                    Console.WriteLine("0 packages in local NuGet source.");
                    if (!string.IsNullOrWhiteSpace(exportPath))
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
                    Console.WriteLine("{0} packages in local NuGet source:", packages.Count);
                    break;
            }
            if (!string.IsNullOrWhiteSpace(exportPath))
            {
                using var writer = OpenPackageMetaDataWriterAsync(exportPath);
                await writer.WriteLineAsync('[');
                foreach (var p in packages.SkipLast(1))
                {
                    await writer.WriteLineAsync($"{p.ToJson()},");
                    Console.WriteLine("{0}: {1}", p.Identity.Id, p.Title);
                }
                await writer.WriteLineAsync(packages.Last().ToJson());
                await writer.WriteLineAsync(']');
                await writer.FlushAsync();
                writer.Close();
            }
            else
                foreach (var p in packages)
                    Console.WriteLine("{0}: {1}", p.Identity.Id, p.Title);
        }

        private async Task ExportLocalPackageMetaDataAsync(List<IPackageSearchMetadata> packages, string exportPath, CancellationToken cancellationToken)
        {
            using var writer = OpenPackageMetaDataWriterAsync(exportPath);
            if (packages.Count > 0)
            {
                await writer.WriteLineAsync('[');
                foreach (var p in packages.SkipLast(1))
                    await writer.WriteLineAsync($"{p.ToJson()},");
                await writer.WriteLineAsync(packages.Last().ToJson());
                await writer.WriteLineAsync(']');
            }
            else
                await writer.WriteLineAsync("[]");
            await writer.FlushAsync();
            writer.Close();
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var localClientService = scope.ServiceProvider.GetRequiredService<LocalClientService>();
                var upstreamClientService = scope.ServiceProvider.GetRequiredService<UpstreamClientService>();
                var appSettings = _settingsOptions.Value;
                var packageIds = appSettings.Delete?.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).Distinct(StringComparer.CurrentCultureIgnoreCase);
                HashSet<string> deletedPackages = (packageIds is not null && packageIds.Any()) ? new(await localClientService.DeleteAsync(packageIds, stoppingToken), StringComparer.CurrentCultureIgnoreCase) : new(StringComparer.CurrentCultureIgnoreCase);
                if ((packageIds = appSettings.Add?.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).Distinct(StringComparer.CurrentCultureIgnoreCase)) is not null && packageIds.Any())
                    await localClientService.AddAsync(packageIds, upstreamClientService, stoppingToken);
                if (appSettings.ListLocal)
                {
                    var packages = await localClientService.GetAllPackagesAsync(stoppingToken);
                    await ListLocalPackagesAsync(packages, appSettings.ExportLocalMetaData, stoppingToken);
                }
                else if (!string.IsNullOrWhiteSpace(appSettings.ExportLocalMetaData))
                {
                    var packages = await localClientService.GetAllPackagesAsync(stoppingToken);
                    await ExportLocalPackageMetaDataAsync(packages, appSettings.ExportLocalMetaData, stoppingToken);
                }
                else if (appSettings.UpdateAll)
                {
                    var packages = await localClientService.GetAllPackagesAsync(stoppingToken);
                    await localClientService.UpdateAsync(packages.Select(p => p.Identity.Id), upstreamClientService, deletedPackages, stoppingToken);
                }
            }
            catch (OperationCanceledException) { throw; }
            finally
            {
                if (!stoppingToken.IsCancellationRequested)
                    _applicationLifetime.StopApplication();
            }
        }
    }
}