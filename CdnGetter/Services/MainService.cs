using CdnGetter.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CdnGetter.Services;

public class MainService : BackgroundService
{
    internal static readonly StringComparer NameComparer = StringComparer.InvariantCultureIgnoreCase;
    private readonly ILogger<MainService> _logger;
    private readonly ContentDb _dbContext;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly Config.CommandSettings _commandSettings;
    private readonly IServiceScopeFactory _scopeFactory;

    public MainService(ILogger<MainService> logger, ContentDb dbContext, IOptions<Config.CommandSettings> options, IHostApplicationLifetime applicationLifetime, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _dbContext = dbContext;
        _commandSettings = options.Value ?? new();
        _applicationLifetime = applicationLifetime;
        _scopeFactory = scopeFactory;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (_commandSettings.Show.IsTrimmedNotEmpty(out string? sv))
            {
                if (_commandSettings.AddLibrary.TrimmedNotEmptyValues().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.AddLibrary)}");
                else if (_commandSettings.GetNewVersions.TrimmedNotEmptyValues().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.GetNewVersions)}");
                else if (_commandSettings.RemoveLibrary.TrimmedNotEmptyValues().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.RemoveLibrary)}");
                else if (_commandSettings.ReloadLibrary.TrimmedNotEmptyValues().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadLibrary)}");
                else if (_commandSettings.ReloadExistingVersions.TrimmedNotEmptyValues().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadExistingVersions)}");
                else if (sv.Equals(Config.CommandSettings.SHOW_CDNs, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_commandSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Library)}");
                    else if (_commandSettings.Version.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Version)}");
                    else if (_commandSettings.Upstream.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Upstream)}");
                    else
                        await UpstreamCdn.ShowAsync(_dbContext, _logger, _scopeFactory, stoppingToken);
                }
                else if (sv.Equals(Config.CommandSettings.SHOW_Libraries, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_commandSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Library)}");
                    else if (_commandSettings.Version.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Version)}");
                    else
                        await CdnLibrary.ShowAsync(_commandSettings.Upstream.TrimmedNotEmptyValues().Distinct(NameComparer), _dbContext, _logger, stoppingToken);
                }
                else if (sv.Equals(Config.CommandSettings.SHOW_Versions, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_commandSettings.Version.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Version)}");
                    else
                        await CdnVersion.ShowAsync(_commandSettings.Library.TrimmedNotEmptyValues().Distinct(NameComparer), _commandSettings.Upstream.TrimmedNotEmptyValues().Distinct(NameComparer), _dbContext, _logger, stoppingToken);
                }
                else if (sv.Equals(Config.CommandSettings.SHOW_Files, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_commandSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Library)}");
                    else
                        await CdnFile.ShowAsync(_commandSettings.Library.TrimmedNotEmptyValues().Distinct(NameComparer), _commandSettings.Version.TrimmedNotEmptyValues().Distinct(NameComparer), _commandSettings.Upstream.TrimmedNotEmptyValues().Distinct(NameComparer), _dbContext, _logger, stoppingToken);
                }
                else
                    _logger.LogInvalidParameterValueWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Show)}", sv);
            }
            else
            {
                IEnumerable<string> libraryNames = _commandSettings.AddLibrary.TrimmedNotEmptyValues();
                if (libraryNames.Any())
                {
                    if (_commandSettings.GetNewVersions.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.AddLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.GetNewVersions)}");
                    else if (_commandSettings.RemoveLibrary.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.AddLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.RemoveLibrary)}");
                    else if (_commandSettings.ReloadLibrary.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.AddLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadLibrary)}");
                    else if (_commandSettings.ReloadExistingVersions.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.AddLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadExistingVersions)}");
                    else if (_commandSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.AddLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Library)}");
                    else
                        await CdnLibrary.AddAsync(_commandSettings.Upstream.TrimmedNotEmptyValues().Distinct(NameComparer), _commandSettings.Version.TrimmedNotEmptyValues().Distinct(NameComparer), _dbContext, _logger, stoppingToken);
                }
                else if ((libraryNames = _commandSettings.GetNewVersions.TrimmedNotEmptyValues()).Any())
                {
                    if (_commandSettings.RemoveLibrary.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.GetNewVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.RemoveLibrary)}");
                    else if (_commandSettings.ReloadLibrary.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.GetNewVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadLibrary)}");
                    else if (_commandSettings.ReloadExistingVersions.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.GetNewVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadExistingVersions)}");
                    else if (_commandSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.GetNewVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Library)}");
                    else if (_commandSettings.Version.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.GetNewVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Version)}");
                    else
                        await CdnVersion.AddNewAsync(_commandSettings.Upstream.TrimmedNotEmptyValues().Distinct(NameComparer), _dbContext, _logger, stoppingToken);
                }
                else if ((libraryNames = _commandSettings.RemoveLibrary.TrimmedNotEmptyValues()).Any())
                {
                    if (_commandSettings.ReloadLibrary.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.RemoveLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadLibrary)}");
                    else if (_commandSettings.ReloadExistingVersions.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.RemoveLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadExistingVersions)}");
                    else if (_commandSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.RemoveLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Library)}");
                    else
                        await CdnLibrary.RemoveAsync(_commandSettings.Upstream.TrimmedNotEmptyValues().Distinct(NameComparer), _commandSettings.Version.TrimmedNotEmptyValues().Distinct(NameComparer), _dbContext, _logger, stoppingToken);
                }
                else if ((libraryNames = _commandSettings.ReloadLibrary.TrimmedNotEmptyValues()).Any())
                {
                    if (_commandSettings.ReloadExistingVersions.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadExistingVersions)}");
                    else if (_commandSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Library)}");
                    else
                        await CdnLibrary.ReloadAsync(_commandSettings.Upstream.TrimmedNotEmptyValues().Distinct(NameComparer), _commandSettings.Version.TrimmedNotEmptyValues().Distinct(NameComparer), _dbContext, _logger, stoppingToken);
                }
                else if ((libraryNames = _commandSettings.ReloadExistingVersions.TrimmedNotEmptyValues()).Any())
                {
                    if (_commandSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadExistingVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Library)}");
                    else if (_commandSettings.Version.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadExistingVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Version)}");
                    else
                        await CdnLibrary.ReloadExistingAsync(_commandSettings.Upstream.TrimmedNotEmptyValues().Distinct(NameComparer), _commandSettings.Version.TrimmedNotEmptyValues().Distinct(NameComparer), _dbContext, _logger, stoppingToken);
                }
                else if (_commandSettings.Library.TrimmedNotEmptyValues().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadExistingVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Library)}");
                else if (_commandSettings.Version.TrimmedNotEmptyValues().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadExistingVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Version)}");
                else
                    await LocalLibrary.GetNewVersionsPreferredAsync(_dbContext, _logger, stoppingToken);
            }
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception error)
        {
            _logger.LogUnexpectedServiceError<MainService>(error);
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
