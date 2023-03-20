using CdnGet.Config;
using CdnGet.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CdnGet.Services;

public abstract class ContentGetterService
{
    protected abstract ILogger GetLogger();

    protected abstract Task ReloadAsync(RemoteLibrary library, ContentDb dbContext, AppSettings appSettings, CancellationToken cancellationToken);

    protected abstract Task AddNewVersionsAsync(RemoteService remoteService, string libraryName, ContentDb dbContext, AppSettings appSettings, CancellationToken cancellationToken);

    public abstract Task GetNewVersionsAsync(RemoteLibrary remoteLibrary, ContentDb dbContext, AppSettings appSettings, CancellationToken cancellationToken);
    
    public async Task GetNewVersionsAsync(RemoteService remoteService, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken cancellationToken)
    {
        foreach (string n in libraryNames)
        {
            RemoteLibrary? existing = await GetMatchingLibraryAsync(remoteService.Id, dbContext, n, cancellationToken);
            if (existing is null)
                await AddNewVersionsAsync(remoteService, n, dbContext, appSettings, cancellationToken);
            else
                await ReloadAsync(existing, dbContext, appSettings, cancellationToken);
        }
    }
    
    public abstract Task AddAsync(RemoteService remoteService, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken cancellationToken);
    
    public async Task ReloadAsync(RemoteService remoteService, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken cancellationToken)
    {
        foreach (string n in libraryNames)
        {
            RemoteLibrary? existing = await GetMatchingLibraryAsync(remoteService.Id, dbContext, n, cancellationToken);
            if (existing is null)
                await AddNewVersionsAsync(remoteService, n, dbContext, appSettings, cancellationToken);
            else
                await ReloadAsync(existing, dbContext, appSettings, cancellationToken);
        }
    }
    
    /// <summary>
    /// Reloads libraries by name for the current remote service.
    /// </summary>
    /// <param name="remoteService">The remote service to remove libraries from.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="appSettings">Application settings.</param>
    /// <param name="libraryNames">The names of libraries to reload.</param>
    /// <param name="cancellationToken">Triggered when <see cref="Microsoft.Extensions.Hosting.IHostedService.StopAsync(CancellationToken)"/> is called.</param>
    public async Task ReloadExistingAsync(RemoteService remoteService, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken cancellationToken)
    {
        LinkedList<RemoteLibrary> toReload = await GetMatchingLibrariesAsync(remoteService.Name, remoteService.Id, dbContext, libraryNames, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            foreach (RemoteLibrary r in toReload)
                await ReloadAsync(r, dbContext, appSettings, cancellationToken);
    }
    
    /// <summary>
    /// Removes libararies by name for the current remote service.
    /// </summary>
    /// <param name="remoteService">The remote service to remove libraries from.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="libraryNames">The names of the libraries to remove.</param>
    /// <param name="cancellationToken">Triggered when <see cref="Microsoft.Extensions.Hosting.IHostedService.StopAsync(CancellationToken)"/> is called.</param>
    public async Task RemoveAsync(RemoteService remoteService, ContentDb dbContext, IEnumerable<string> libraryNames, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;
        LinkedList<RemoteLibrary> toRemove = await GetMatchingLibrariesAsync(remoteService.Name, remoteService.Id, dbContext, libraryNames, cancellationToken);
        if (cancellationToken.IsCancellationRequested || toRemove.First is null)
            return;
        foreach (RemoteLibrary library in toRemove)
        {
            await library.RemoveAsync(dbContext, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
        }
        await dbContext.SaveChangesAsync(true, cancellationToken);
    }

    protected async Task<LinkedList<RemoteLibrary>> GetMatchingLibrariesAsync(string remoteName, Guid remoteServiceId, ContentDb dbContext, IEnumerable<string> libraryNames, CancellationToken cancellationToken)
    {
        LinkedList<RemoteLibrary> result = new();
        foreach (string name in libraryNames)
        {
            if (cancellationToken.IsCancellationRequested)
                return result;
            RemoteLibrary? library = await dbContext.RemoteLibraries.Include(r => r.Local).FirstOrDefaultAsync(l => l.Local!.Name == name && l.RemoteServiceId == remoteServiceId, cancellationToken);
            if (library is null)
                GetLogger().LogRemoteLibraryNotFound(name, remoteName);
            else
                result.AddLast(library);
        }
        return result;
    }

    protected async Task<RemoteLibrary?> GetMatchingLibraryAsync(Guid remoteServiceId, ContentDb dbContext, string libraryName, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return null;
        return await dbContext.RemoteLibraries.Include(r => r.Local).FirstOrDefaultAsync(l => l.Local!.Name == libraryName && l.RemoteServiceId == remoteServiceId, cancellationToken);
    }

    internal async Task UpdateLibrariesAsync(RemoteService rsvc, ContentDb dbContext, Config.AppSettings appSettings, ILogger logger, CancellationToken cancellationToken)
    {
        LibraryActionGroup[] actions = LibraryActionGroup.FromSettings(appSettings).ToArray();
        if (actions.Length > 0)
            foreach (LibraryActionGroup g in actions)
                switch (g.Action)
                {
                    case LibraryAction.Remove:
                        await RemoveAsync(rsvc, dbContext, g.LibraryNames, cancellationToken);
                        break;
                    case LibraryAction.ReloadExistingVersions:
                        await ReloadExistingAsync(rsvc, dbContext, appSettings, g.LibraryNames, cancellationToken);
                        break;
                    case LibraryAction.GetNewVersions:
                        await GetNewVersionsAsync(rsvc, dbContext, appSettings, g.LibraryNames, cancellationToken);
                        break;
                    case LibraryAction.Reload:
                        await ReloadAsync(rsvc, dbContext, appSettings, g.LibraryNames, cancellationToken);
                        break;
                    default:
                        await AddAsync(rsvc, dbContext, appSettings, g.LibraryNames, cancellationToken);
                        break;
                }
        else
        {
            Guid id = rsvc.Id;
            RemoteLibrary[] rl = await dbContext.RemoteLibraries.Where(r => r.RemoteServiceId == id).ToArrayAsync(cancellationToken);
            if (rl.Length == 0)
                logger.LogNothingToDo();
            else
                foreach (RemoteLibrary r in rl)
                    await GetNewVersionsAsync(r, dbContext, appSettings, cancellationToken);
        }
    }
}
