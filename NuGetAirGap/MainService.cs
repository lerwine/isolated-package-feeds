using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var localClientService = scope.ServiceProvider.GetRequiredService<LocalClientService>();
                var upstreamClientService = scope.ServiceProvider.GetRequiredService<UpstreamClientService>();
                var appSettings = _settingsOptions.Value;
                var packageIds = appSettings.Delete?.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).Distinct(StringComparer.CurrentCultureIgnoreCase);
                if (packageIds is not null && packageIds.Any())
                    await localClientService.DeleteAsync(packageIds, stoppingToken);
                if ((packageIds = appSettings.Add?.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).Distinct(StringComparer.CurrentCultureIgnoreCase)) is not null && packageIds.Any())
                    await localClientService.AddAsync(packageIds, upstreamClientService, stoppingToken);
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