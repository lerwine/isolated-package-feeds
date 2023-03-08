using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CdnSync;

public class CdnSyncService : BackgroundService
{
    private readonly ILogger<CdnSyncService> _logger;
    private readonly CdnJsSyncService _cdnJsSync;

    public CdnSyncService(ILogger<CdnSyncService> logger, CdnJsSyncService cdnJsSync)
    {   
        _logger = logger;
        _cdnJsSync = cdnJsSync;
    }

    protected async override Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
    {
        try { await _cdnJsSync.ExecuteAsync(stoppingToken); }
        catch (OperationCanceledException) { throw; }
        catch (Exception error)
        {
            _logger.LogError(error, "Error executing {TypeName}", _cdnJsSync.GetType().FullName);
        }
#pragma warning disable CA2016, CS4014
        finally { Program.Host.StopAsync(); }
#pragma warning restore CA2016, CS4014
    }
}