using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CdnGet.Services;

public class MainService : BackgroundService
{
    private readonly ILogger<MainService> _logger;
    private readonly CdnJsRemoteService _cdnJs;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public MainService(ILogger<MainService> logger, CdnJsRemoteService cdnJs, IHostApplicationLifetime applicationLifetime)
    {   
        _logger = logger;
        _cdnJs = cdnJs;
        _applicationLifetime = applicationLifetime;
    }

    protected async override Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
    {
        try { await _cdnJs.ExecuteAsync(stoppingToken); }
        catch (OperationCanceledException) { throw; }
        catch (Exception error)
        {
            _logger.LogError(error, "Error executing {TypeName}", _cdnJs.GetType().FullName);
        }
#pragma warning disable CA2016, CS4014
        finally
        {
            if (!stoppingToken.IsCancellationRequested)
                _applicationLifetime.StopApplication();
        }
#pragma warning restore CA2016, CS4014
    }
}