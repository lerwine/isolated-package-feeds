using CdnGet.Config;
using CdnGet.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CdnGet.Services;

public class MainService : BackgroundService
{
    private static readonly StringComparer NameComparer = StringComparer.InvariantCultureIgnoreCase;
    private readonly ILogger<MainService> _logger;
    private readonly ContentDb _dbContext;
    private readonly CdnJsRemoteService _cdnJs;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly Config.AppSettings _appSettings;
    private readonly IServiceScopeFactory _scopeFactory;

    public MainService(ILogger<MainService> logger, ContentDb dbContext, IOptions<Config.AppSettings> options, CdnJsRemoteService cdnJs, IHostApplicationLifetime applicationLifetime, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _dbContext = dbContext;
        _appSettings = options.Value ?? new();
        _cdnJs = cdnJs;
        _applicationLifetime = applicationLifetime;
        _scopeFactory = scopeFactory;
    }

    private async Task<(RemoteService?, Type)> GetRemoteServiceAsync(string remoteName, System.Threading.CancellationToken stoppingToken)
    {
        RemoteService? rsvc = await _dbContext.RemoteServices.FirstOrDefaultAsync(r => r.Name == remoteName);
        if (rsvc is null)
        {
            using IEnumerator<KeyValuePair<Guid, (Type Type, string Name, string Description)>> enumerator = RemoteUpdateServiceAttribute.RemoteUpdateServices
                .Where(kvp => kvp.Value.Name.Length > 0 && NameComparer.Equals(remoteName, kvp.Value.Name)).GetEnumerator();
            if (!enumerator.MoveNext())
            {
                _logger.LogRemoteServiceNotFound(remoteName);
                return (null, null!);
            }
            Guid id = enumerator.Current.Key;
            if ((rsvc = await _dbContext.RemoteServices.FirstOrDefaultAsync(r => r.Id == id)) is null)
            {
                (Type type, string? name, string? description) = enumerator.Current.Value;
                rsvc = new()
                {
                    Id = id,
                    Name = name!,
                    Description = description
                };
                await _dbContext.RemoteServices.AddAsync(rsvc, stoppingToken);
                await _dbContext.SaveChangesAsync(stoppingToken);
                return (rsvc, type);
            }
            return (rsvc, enumerator.Current.Value.Type);
        }
        if (RemoteUpdateServiceAttribute.RemoteUpdateServices.TryGetValue(rsvc.Id, out (Type Type, string Name, string Description) result))
            return (rsvc, result.Type);
        _logger.LogRemoteServiceNotSupported(remoteName);
        return (null, null!);
    }

    protected async override Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
    {
        try
        {
            if (_appSettings.ShowRemotes ?? false)
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                int count = 0;
                foreach (KeyValuePair<Guid, (Type Type, string Name, string Description)> item in RemoteUpdateServiceAttribute.RemoteUpdateServices)
                {
                    Guid id = item.Key;
                    RemoteService? rsvc = await _dbContext.RemoteServices.FirstOrDefaultAsync(r => r.Id == id);
                    (Type type, string name, string description) = item.Value;
                    if (scope.ServiceProvider.GetService(type) is RemoteUpdateService)
                    {
                        count++;
                        if (rsvc is not null)
                        {
                            if (name.Length > 0 && !NameComparer.Equals(rsvc.Name, name))
                                Console.WriteLine("{0}; {1}", rsvc.Name, name);
                            else
                                Console.WriteLine(rsvc.Name);
                        }
                        else if (name.Length > 0)
                            Console.WriteLine(name);
                        if (description.Length > 0)
                            foreach (string d in description.SplitLines().Select(l => l.ToWsNormalizedOrEmptyIfNull()!))
                                Console.WriteLine((d.Length > 0) ? $"\t{d}" : d);
                    }
                    else
                        _logger.LogServiceTypeNotFound(type);
                }
                if (count > 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("{0:d} remotes total.", count);
                }
                else
                    _logger.LogNoRemotesFound();
            }
            else if (_appSettings.Remote.IsWsNormalizedNotEmpty(out string? remoteName))
            {
                (RemoteService? rsvc, Type type) = await GetRemoteServiceAsync(remoteName, stoppingToken);
                if (rsvc is not null)
                {
                    using IServiceScope scope = _scopeFactory.CreateScope();
                    RemoteUpdateService? svc = scope.ServiceProvider.GetService(type) as RemoteUpdateService;
                    if (svc is null)
                        _logger.LogRemoteServiceNotSupported(remoteName);
                    else
                    {
                        (LibraryAction Action, string[] Names)[] actions = _appSettings.GetLibraryActions().ToArray();
                        if (actions.Length > 0)
                            foreach ((LibraryAction action, string[] names) in actions)
                                switch (action)
                                {
                                    case LibraryAction.Remove:
                                        await svc.RemoveAsync(rsvc, _dbContext, _appSettings, names, stoppingToken);
                                        break;
                                    case LibraryAction.ReloadExisting:
                                        await svc.ReloadExistingAsync(rsvc, _dbContext, _appSettings, names, stoppingToken);
                                        break;
                                    case LibraryAction.GetNew:
                                        await svc.GetNewAsync(rsvc, _dbContext, _appSettings, names, stoppingToken);
                                        break;
                                    case LibraryAction.Reload:
                                        await svc.ReloadAsync(rsvc, _dbContext, _appSettings, names, stoppingToken);
                                        break;
                                    default:
                                        await svc.AddAsync(rsvc, _dbContext, _appSettings, names, stoppingToken);
                                        break;
                                }
                        else
                            _logger.LogRemoteServiceNoActions(remoteName);
                    }
                }
            }
            else
                _logger.LogNothingToDo();
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