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
    private readonly Config.AppSettings _appSettings;
    private readonly IServiceScopeFactory _scopeFactory;

    public MainService(ILogger<MainService> logger, ContentDb dbContext, IOptions<Config.AppSettings> options, IHostApplicationLifetime applicationLifetime, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _dbContext = dbContext;
        _appSettings = options.Value ?? new();
        _applicationLifetime = applicationLifetime;
        _scopeFactory = scopeFactory;
    }

    private async Task<(UpstreamCdn?, Type)> GetUpstreamCdnAsync(string remoteName, System.Threading.CancellationToken stoppingToken)
    {
        UpstreamCdn? rsvc = await _dbContext.UpstreamCdns.FirstOrDefaultAsync(r => r.Name == remoteName, cancellationToken: stoppingToken);
        if (rsvc is null)
        {
            using IEnumerator<KeyValuePair<Guid, (Type Type, string Name, string Description)>> enumerator = ContentGetterAttribute.UpstreamCdnServices
                .Where(kvp => kvp.Value.Name.Length > 0 && NameComparer.Equals(remoteName, kvp.Value.Name)).GetEnumerator();
            if (!enumerator.MoveNext())
            {
                _logger.LogUpstreamCdnNotFound(remoteName);
                return (null, null!);
            }
            Guid id = enumerator.Current.Key;
            if ((rsvc = await _dbContext.UpstreamCdns.FirstOrDefaultAsync(r => r.Id == id, cancellationToken: stoppingToken)) is null)
            {
                (Type type, string? name, string? description) = enumerator.Current.Value;
                rsvc = new()
                {
                    Id = id,
                    Name = name!,
                    Description = description
                };
                await _dbContext.UpstreamCdns.AddAsync(rsvc, stoppingToken);
                await _dbContext.SaveChangesAsync(stoppingToken);
                return (rsvc, type);
            }
            return (rsvc, enumerator.Current.Value.Type);
        }
        if (ContentGetterAttribute.UpstreamCdnServices.TryGetValue(rsvc.Id, out (Type Type, string Name, string Description) result))
            return (rsvc, result.Type);
        _logger.LogUpstreamCdnNotSupported(remoteName);
        return (null, null!);
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (_appSettings.Show.IsTrimmedNotEmpty(out string? sv))
            {
                if (_appSettings.AddLibrary.TrimmedNotEmptyValues().Any())
                    _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.AddLibrary)}");
                else if (_appSettings.GetNewVersions.TrimmedNotEmptyValues().Any())
                    _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.GetNewVersions)}");
                else if (_appSettings.RemoveLibrary.TrimmedNotEmptyValues().Any())
                    _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.RemoveLibrary)}");
                else if (_appSettings.ReloadLibrary.TrimmedNotEmptyValues().Any())
                    _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.ReloadLibrary)}");
                else if (_appSettings.ReloadExistingVersions.TrimmedNotEmptyValues().Any())
                    _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.ReloadExistingVersions)}");
                else if (sv.Equals(Config.AppSettings.SHOW_CDNs, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_appSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Library)}");
                    else if (_appSettings.Version.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Version)}");
                    else if (_appSettings.Upstream.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Upstream)}");
                    else
                        await UpstreamCdn.ShowCDNsAsync(_dbContext, _logger, _scopeFactory, stoppingToken);
                }
                else if (sv.Equals(Config.AppSettings.SHOW_Libraries, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_appSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Library)}");
                    else if (_appSettings.Version.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Version)}");
                    else
                    {
                        IEnumerable<string> upstream = _appSettings.Upstream.TrimmedNotEmptyValues();
                        if (upstream.Any())
                        {
                            // TODO: Show libraries for upstream CDN(s)
                        }
                        else
                        {
                            // TODO: Show libraries
                        }
                    }
                }
                else if (sv.Equals(Config.AppSettings.SHOW_Versions, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_appSettings.Version.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Version)}");
                    else
                    {
                        IEnumerable<string> libraries = _appSettings.Library.TrimmedNotEmptyValues();
                        IEnumerable<string> upstream = _appSettings.Upstream.TrimmedNotEmptyValues();
                        if (upstream.Any())
                        {
                            // TODO: Show library versions for upstream CDN(s)
                        }
                        else
                        {
                            // TODO: Show library versions
                        }
                    }
                }
                else if (sv.Equals(Config.AppSettings.SHOW_Files, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_appSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Library)}");
                    else
                    {
                        // TODO: Show files
                    }
                }
                else
                    _logger.LogInvalidParameterValue($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Show)}", sv);
            }
            else
            {
                IEnumerable<string> libraryNames = _appSettings.AddLibrary.TrimmedNotEmptyValues();
                if (libraryNames.Any())
                {
                    if (_appSettings.GetNewVersions.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.AddLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.GetNewVersions)}");
                    else if (_appSettings.RemoveLibrary.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.AddLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.RemoveLibrary)}");
                    else if (_appSettings.ReloadLibrary.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.AddLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.ReloadLibrary)}");
                    else if (_appSettings.ReloadExistingVersions.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.AddLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.ReloadExistingVersions)}");
                    else if (_appSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.AddLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Library)}");
                    else
                    {
                        IEnumerable<string> upstreams = _appSettings.Upstream.TrimmedNotEmptyValues();
                        if (upstreams.Any())
                        {
                            IEnumerable<string> versions = _appSettings.Version.TrimmedNotEmptyValues();
                            // TODO: Add libraries
                        }
                        else
                            _logger.LogRequiredDependentParameter1($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Upstream)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.AddLibrary)}");
                    }
                }
                else if ((libraryNames = _appSettings.GetNewVersions.TrimmedNotEmptyValues()).Any())
                {
                    if (_appSettings.RemoveLibrary.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.GetNewVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.RemoveLibrary)}");
                    else if (_appSettings.ReloadLibrary.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.GetNewVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.ReloadLibrary)}");
                    else if (_appSettings.ReloadExistingVersions.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.GetNewVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.ReloadExistingVersions)}");
                    else if (_appSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.GetNewVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Library)}");
                    else if (_appSettings.Version.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.GetNewVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Version)}");
                    else
                    {
                        IEnumerable<string> upstream = _appSettings.Upstream.TrimmedNotEmptyValues();
                        // TODO: Add new versions
                    }
                }
                else if ((libraryNames = _appSettings.RemoveLibrary.TrimmedNotEmptyValues()).Any())
                {
                    if (_appSettings.ReloadLibrary.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.RemoveLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.ReloadLibrary)}");
                    else if (_appSettings.ReloadExistingVersions.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.RemoveLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.ReloadExistingVersions)}");
                    else if (_appSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.RemoveLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Library)}");
                    else
                    {
                        IEnumerable<string> upstream = _appSettings.Upstream.TrimmedNotEmptyValues();
                        IEnumerable<string> versions = _appSettings.Version.TrimmedNotEmptyValues();
                        // TODO: Remove libraries
                    }
                }
                else if ((libraryNames = _appSettings.ReloadLibrary.TrimmedNotEmptyValues()).Any())
                {
                    if (_appSettings.ReloadExistingVersions.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.ReloadLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.ReloadExistingVersions)}");
                    else if (_appSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.ReloadLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Library)}");
                    else
                    {
                        IEnumerable<string> upstreams = _appSettings.Upstream.TrimmedNotEmptyValues();
                        IEnumerable<string> versions = _appSettings.Version.TrimmedNotEmptyValues();
                        if (upstreams.Any())
                        {
                            // TODO: Reload libraries
                        }
                        else if (versions.Any())
                        {
                            // TODO: Reload libraries
                        }
                        else
                            _logger.LogRequiredAltDependentParameter($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Upstream)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Version)}",
                                $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.ReloadLibrary)}");
                    }
                }
                else if ((libraryNames = _appSettings.ReloadExistingVersions.TrimmedNotEmptyValues()).Any())
                {
                    if (_appSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.ReloadExistingVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Library)}");
                    else if (_appSettings.Version.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.ReloadExistingVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Version)}");
                    else
                    {
                        IEnumerable<string> upstreams = _appSettings.Upstream.TrimmedNotEmptyValues();
                        IEnumerable<string> versions = _appSettings.Version.TrimmedNotEmptyValues();
                        if (upstreams.Any())
                        {
                            // TODO: Reload libraries
                        }
                        else if (versions.Any())
                        {
                            // TODO: Reload libraries
                        }
                        else
                            _logger.LogRequiredAltDependentParameter($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Upstream)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Version)}",
                                $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.ReloadExistingVersions)}");
                    }
                }
                else if (_appSettings.Library.TrimmedNotEmptyValues().Any())
                    _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.ReloadExistingVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Library)}");
                else if (_appSettings.Version.TrimmedNotEmptyValues().Any())
                    _logger.LogMutuallyExclusiveSwitchError($"{nameof(CdnGetter)}:{nameof(Config.AppSettings.ReloadExistingVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.AppSettings.Version)}");
                else
                {
                    LocalLibrary[] libraries = await _dbContext.LocalLibraries.ToArrayAsync(stoppingToken);
                    if (libraries.Length == 0)
                        _logger.LogNothingToDo();
                    else
                        foreach (LocalLibrary l in libraries)
                            await l.GetNewVersionsPreferredAsync(_dbContext, stoppingToken);
                }
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