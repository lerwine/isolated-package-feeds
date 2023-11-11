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