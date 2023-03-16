using CdnGet.Config;
using CdnGet.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CdnGet.Services;

public abstract class RemoteUpdateService
{
    protected abstract ILogger GetLogger();

    public abstract Task GetNewAsync(RemoteService remoteService, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken stoppingToken);
    
    public abstract Task AddAsync(RemoteService remoteService, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken stoppingToken);
    
    public abstract Task ReloadAsync(RemoteService remoteService, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken stoppingToken);
    
    public abstract Task ReloadExistingAsync(RemoteService remoteService, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken stoppingToken);
    
    public async Task RemoveAsync(RemoteService remoteService, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;
        CdnJsSettings settings = appSettings.CdnJs ?? new();
        if (!settings.TryGetBaseUrl(out Uri? baseUrl))
        {
            GetLogger().LogInvalidBaseUrl(GetType(), remoteService.Name);
            return;
        }
        LinkedList<ContentLibrary> toRemove = await GetMatchingLibrariesAsync(remoteService.Name, remoteService.Id, dbContext, libraryNames, stoppingToken);
        if (stoppingToken.IsCancellationRequested || toRemove.First is null)
            return;
        foreach (ContentLibrary library in toRemove)
        {
            await library.RemoveAsync(dbContext, stoppingToken);
            if (stoppingToken.IsCancellationRequested)
                return;
        }
        await dbContext.SaveChangesAsync(true, stoppingToken);
    }

    protected async Task<LinkedList<ContentLibrary>> GetMatchingLibrariesAsync(string remoteName, Guid remoteServiceId, ContentDb dbContext, IEnumerable<string> libraryNames, CancellationToken stoppingToken)
    {
        LinkedList<ContentLibrary> result = new();
        foreach (string name in libraryNames)
        {
            if (stoppingToken.IsCancellationRequested)
                return result;
            ContentLibrary? library = await dbContext.Libraries.FirstOrDefaultAsync(l => l.Name == name && l.RemoteServiceId == remoteServiceId, stoppingToken);
            if (library is null)
                GetLogger().LogLocalLibraryNotFound(name, remoteName);
            else
                result.AddLast(library);
        }
        return result;
    }

    protected async Task<ContentLibrary?> GetMatchingLibraryAsync(Guid remoteServiceId, ContentDb dbContext, string libraryName, CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return null;
        return await dbContext.Libraries.FirstOrDefaultAsync(l => l.Name == libraryName && l.RemoteServiceId == remoteServiceId, stoppingToken);
    }
}
