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

    private async Task<LinkedList<UpstreamCdn>> GetUpstreamCdns(IEnumerable<string> names, CancellationToken stoppingToken)
    {
        LinkedList<UpstreamCdn> result = new();
        foreach (string n in names.Distinct(NameComparer))
        {
            UpstreamCdn? cdn = await _dbContext.UpstreamCdns.FirstOrDefaultAsync(r => r.Name == n, cancellationToken: stoppingToken);
            if (cdn is null)
                _logger.LogUpstreamCdnNotFoundError(n);
            else
                result.AddLast(cdn);
        }
        return result;
    }

    private async Task<(UpstreamCdn?, Type)> GetUpstreamCdnAsync(string cdnName, CancellationToken stoppingToken)
    {
        UpstreamCdn? cdn = await _dbContext.UpstreamCdns.FirstOrDefaultAsync(r => r.Name == cdnName, cancellationToken: stoppingToken);
        if (cdn is null)
        {
            using IEnumerator<KeyValuePair<Guid, (Type Type, string Name, string Description)>> enumerator = ContentGetterAttribute.UpstreamCdnServices
                .Where(kvp => kvp.Value.Name.Length > 0 && NameComparer.Equals(cdnName, kvp.Value.Name)).GetEnumerator();
            if (!enumerator.MoveNext())
            {
                _logger.LogUpstreamCdnNotFoundError(cdnName);
                return (null, null!);
            }
            Guid id = enumerator.Current.Key;
            if ((cdn = await _dbContext.UpstreamCdns.FirstOrDefaultAsync(r => r.Id == id, cancellationToken: stoppingToken)) is null)
            {
                (Type type, string? name, string? description) = enumerator.Current.Value;
                cdn = new()
                {
                    Id = id,
                    Name = name!,
                    Description = description
                };
                await _dbContext.UpstreamCdns.AddAsync(cdn, stoppingToken);
                await _dbContext.SaveChangesAsync(stoppingToken);
                return (cdn, type);
            }
            return (cdn, enumerator.Current.Value.Type);
        }
        if (ContentGetterAttribute.UpstreamCdnServices.TryGetValue(cdn.Id, out (Type Type, string Name, string Description) result))
            return (cdn, result.Type);
        _logger.LogUpstreamCdnNotSupportedError(cdnName);
        return (null, null!);
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
                        await UpstreamCdn.ShowCDNsAsync(_dbContext, _logger, _scopeFactory, stoppingToken);
                }
                else if (sv.Equals(Config.CommandSettings.SHOW_Libraries, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_commandSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Library)}");
                    else if (_commandSettings.Version.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Version)}");
                    else
                    {
                        IEnumerable<string> cdnNames = _commandSettings.Upstream.TrimmedNotEmptyValues();
                        if (cdnNames.Any())
                        {
                            LinkedList<UpstreamCdn> cdns = await GetUpstreamCdns(cdnNames, stoppingToken);
                            if (cdns.Count == 1)
                            {
                                Guid id = cdns.First!.Value.Id;
                                await _dbContext.CdnLibraries.Where(l => l.LocalId == id).Include(l => l.Local).Select(l => l.Local!.Name).ForEachAsync(n => Console.WriteLine(n), stoppingToken);
                            }
                            else
                                foreach (UpstreamCdn cdn in cdns)
                                {
                                    Console.WriteLine(cdn.Name);
                                    Guid id = cdn.Id;
                                    await _dbContext.CdnLibraries.Where(l => l.LocalId == id).Include(l => l.Local).Select(l => l.Local!.Name).ForEachAsync(ln => Console.WriteLine($"\t{ln}"), stoppingToken);
                                }
                        }
                        else
                            await _dbContext.LocalLibraries.ForEachAsync(l => Console.WriteLine(l.Name), stoppingToken);
                    }
                }
                else if (sv.Equals(Config.CommandSettings.SHOW_Versions, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_commandSettings.Version.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Version)}");
                    else
                    {
                        IEnumerable<string> libraries = _commandSettings.Library.TrimmedNotEmptyValues();
                        IEnumerable<string> upstream = _commandSettings.Upstream.TrimmedNotEmptyValues();
                        if (upstream.Any())
                        {
                            throw new NotImplementedException("Show library versions for upstream CDN(s) functionality not implemented");
                        }
                        else
                        {
                            throw new NotImplementedException("Show library versions functionality not implemented");
                        }
                    }
                }
                else if (sv.Equals(Config.CommandSettings.SHOW_Files, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_commandSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Library)}");
                    else
                    {
                        throw new NotImplementedException("Show files functionality not implemented");
                    }
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
                    {
                        IEnumerable<string> cdns = _commandSettings.Upstream.TrimmedNotEmptyValues();
                        if (cdns.Any())
                        {
                            IEnumerable<string> versions = _commandSettings.Version.TrimmedNotEmptyValues();
                            throw new NotImplementedException("Add libraries functionality not implemented.");
                        }
                        else
                            _logger.LogRequiredDependentParameterWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Upstream)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.AddLibrary)}");
                    }
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
                    {
                        IEnumerable<string> upstream = _commandSettings.Upstream.TrimmedNotEmptyValues();
                        throw new NotImplementedException("Add new versions functionality not implemented.");
                    }
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
                    {
                        IEnumerable<string> upstream = _commandSettings.Upstream.TrimmedNotEmptyValues();
                        IEnumerable<string> versions = _commandSettings.Version.TrimmedNotEmptyValues();
                        throw new NotImplementedException("Remove libraries functionality not implemented.");
                    }
                }
                else if ((libraryNames = _commandSettings.ReloadLibrary.TrimmedNotEmptyValues()).Any())
                {
                    if (_commandSettings.ReloadExistingVersions.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadExistingVersions)}");
                    else if (_commandSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadLibrary)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Library)}");
                    else
                    {
                        IEnumerable<string> cdns = _commandSettings.Upstream.TrimmedNotEmptyValues();
                        IEnumerable<string> versions = _commandSettings.Version.TrimmedNotEmptyValues();
                        if (cdns.Any())
                        {
                            throw new NotImplementedException("Reload libraries for specific CDNs functionality not implemented.");
                        }
                        else if (versions.Any())
                        {
                            throw new NotImplementedException("Reload libraries for specific versions functionality not implemented.");
                        }
                        else
                            _logger.LogRequiredAltDependentParameterWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Upstream)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Version)}",
                                $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadLibrary)}");
                    }
                }
                else if ((libraryNames = _commandSettings.ReloadExistingVersions.TrimmedNotEmptyValues()).Any())
                {
                    if (_commandSettings.Library.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadExistingVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Library)}");
                    else if (_commandSettings.Version.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadExistingVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Version)}");
                    else
                    {
                        IEnumerable<string> cdns = _commandSettings.Upstream.TrimmedNotEmptyValues();
                        IEnumerable<string> versions = _commandSettings.Version.TrimmedNotEmptyValues();
                        if (cdns.Any())
                        {
                            throw new NotImplementedException("Reload existing versions for specific CDNs functionality not implemented.");
                        }
                        else if (versions.Any())
                        {
                            throw new NotImplementedException("Reload libraries for specific existing versions functionality not implemented.");
                        }
                        else
                            _logger.LogRequiredAltDependentParameterWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Upstream)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Version)}",
                                $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadExistingVersions)}");
                    }
                }
                else if (_commandSettings.Library.TrimmedNotEmptyValues().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadExistingVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Library)}");
                else if (_commandSettings.Version.TrimmedNotEmptyValues().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadExistingVersions)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Version)}");
                else
                {
                    LocalLibrary[] libraries = await _dbContext.LocalLibraries.ToArrayAsync(stoppingToken);
                    if (libraries.Length == 0)
                        _logger.LogNothingToDoWarning();
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