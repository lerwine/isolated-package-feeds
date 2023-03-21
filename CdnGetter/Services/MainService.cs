using CdnGetter.Config;
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
    private readonly CdnJsGetterService _cdnJs;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly Config.AppSettings _appSettings;
    private readonly IServiceScopeFactory _scopeFactory;

    public MainService(ILogger<MainService> logger, ContentDb dbContext, IOptions<Config.AppSettings> options, CdnJsGetterService cdnJs, IHostApplicationLifetime applicationLifetime, IServiceScopeFactory scopeFactory)
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
            using IEnumerator<KeyValuePair<Guid, (Type Type, string Name, string Description)>> enumerator = ContentGetterAttribute.RemoteUpdateServices
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
        if (ContentGetterAttribute.RemoteUpdateServices.TryGetValue(rsvc.Id, out (Type Type, string Name, string Description) result))
            return (rsvc, result.Type);
        _logger.LogRemoteServiceNotSupported(remoteName);
        return (null, null!);
    }

    protected async override Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
    {
        try
        {
            if (_appSettings.ShowRemotes ?? false)
                await RemoteService.ShowRemotesAsync(_dbContext, _logger, _scopeFactory, stoppingToken);
            else if (_appSettings.Remote.IsWsNormalizedNotEmpty(out string? remoteName))
            {
                (RemoteService? rsvc, Type type) = await GetRemoteServiceAsync(remoteName, stoppingToken);
                if (rsvc is null)
                    return;
                using IServiceScope scope = _scopeFactory.CreateScope();
                ContentGetterService? svc = scope.ServiceProvider.GetService(type) as ContentGetterService;
                if (svc is null)
                    _logger.LogRemoteServiceNotSupported(remoteName);
                else
                    await svc.UpdateLibrariesAsync(rsvc, _dbContext, _appSettings, _logger, stoppingToken);
            }
            else
            {
                Model.LibraryActionGroup[] actions = Model.LibraryActionGroup.FromSettings(_appSettings).ToArray();
                if (actions.Length > 0)
                    await UpdateLibrariesAsync(actions, stoppingToken);
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

    private async Task UpdateLibrariesAsync(Model.LibraryActionGroup[] actions, System.Threading.CancellationToken stoppingToken)
    {
        foreach (Model.LibraryActionGroup g in actions)
            switch (g.Action)
            {
                case Model.LibraryAction.Remove:
                    foreach (string n in g.LibraryNames)
                    {
                        LocalLibrary? ll = await _dbContext.LocalLibraries.FirstOrDefaultAsync(l => l.Name == n, stoppingToken);
                        if (ll is not null)
                            await ll.RemoveAsync(_dbContext, stoppingToken);
                    }
                    break;
                case Model.LibraryAction.ReloadExistingVersions:
                    foreach (string n in g.LibraryNames)
                    {
                        LocalLibrary? ll = await _dbContext.LocalLibraries.Where(l => l.Name == n).Include(l => l.Versions).FirstAsync(stoppingToken);
                        if (ll is null)
                            _logger.LogLocalLibraryNotFound(n);
                        else
                            foreach (LocalVersion lv in ll.Versions)
                                await lv.ReloadAsync(_dbContext, stoppingToken);
                    }
                    break;
                case Model.LibraryAction.GetNewVersions:
                    foreach (string n in g.LibraryNames)
                    {
                        LocalLibrary? ll = await _dbContext.LocalLibraries.FirstOrDefaultAsync(l => l.Name == n);
                        if (ll is null)
                            _logger.LogLocalLibraryNotFound(n);
                        else
                            await ll.GetNewVersionsAsync(_dbContext, stoppingToken);
                    }
                    break;
                case Model.LibraryAction.Reload:
                    foreach (string n in g.LibraryNames)
                    {
                        LocalLibrary? ll = await _dbContext.LocalLibraries.FirstOrDefaultAsync(l => l.Name == n);
                        if (ll is null)
                            _logger.LogLocalLibraryNotFound(n);
                        else
                            await ll.ReloadAsync(_dbContext, stoppingToken);
                    }
                    break;
                default:
                    foreach (string n in g.LibraryNames)
                    {
                        if (await _dbContext.LocalLibraries.AnyAsync(l => l.Name == n))
                            _logger.LogLocalLibraryAlreadyExists(n);
                        else
                        await LocalLibrary.AddAsync(n, _dbContext, _appSettings, stoppingToken);
                    }
                    break;
            }
    }
}