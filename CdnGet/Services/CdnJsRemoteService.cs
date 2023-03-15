using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CdnGet.Config;
using CdnGet.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CdnGet.Services;

public class CdnJsRemoteService
{
    private readonly ContentDb _dbContext;
    private readonly ILogger<CdnJsRemoteService> _logger;
    private readonly CdnJsSettings _settings;

    public CdnJsRemoteService(ContentDb dbContext, IOptions<AppSettings> options, ILogger<CdnJsRemoteService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
        _settings = options.Value.CdnJs ?? new();
    }

    internal async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Guid providerId = new Guid(CdnJsSettings.PROVIDER_ID);
        RemoteService? remoteService = await _dbContext.RemoteServices.Where(r => r.Id == providerId).Include(r => r.Libraries).FirstOrDefaultAsync(stoppingToken);
        if (stoppingToken.IsCancellationRequested)
            return;
        if (remoteService is null)
        {
            remoteService = new()
            {
                Name = _settings.GetDisplayName(),
                Description = _settings.Description.ToTrimmedOrEmptyIfNull(),
                Libraries = new()
            };
            await _dbContext.RemoteServices.AddAsync(remoteService, stoppingToken);
            await _dbContext.SaveChangesAsync(stoppingToken);
        }
    }
}