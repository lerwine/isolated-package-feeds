using CdnGetter.Config;
using CdnGetter.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CdnGetter.Services;

public abstract class ContentGetterService
{
    protected abstract ILogger GetLogger();

    protected abstract Task ReloadAsync(CdnLibrary library, ContentDb dbContext, AppSettings appSettings, CancellationToken cancellationToken);

    protected abstract Task AddNewVersionsAsync(UpstreamCdn upstreamCdn, string libraryName, ContentDb dbContext, AppSettings appSettings, CancellationToken cancellationToken);

    public abstract Task GetNewVersionsAsync(CdnLibrary cdnLibrary, ContentDb dbContext, AppSettings appSettings, CancellationToken cancellationToken);
    
    public async Task GetNewVersionsAsync(UpstreamCdn upstreamCdn, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken cancellationToken)
    {
        foreach (string n in libraryNames)
        {
            CdnLibrary? existing = await GetMatchingLibraryAsync(upstreamCdn.Id, dbContext, n, cancellationToken);
            if (existing is null)
                await AddNewVersionsAsync(upstreamCdn, n, dbContext, appSettings, cancellationToken);
            else
                await ReloadAsync(existing, dbContext, appSettings, cancellationToken);
        }
    }
    
    public abstract Task AddAsync(UpstreamCdn upstreamCdn, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken cancellationToken);
    
    public async Task ReloadAsync(UpstreamCdn upstreamCdn, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken cancellationToken)
    {
        foreach (string n in libraryNames)
        {
            CdnLibrary? existing = await GetMatchingLibraryAsync(upstreamCdn.Id, dbContext, n, cancellationToken);
            if (existing is null)
                await AddNewVersionsAsync(upstreamCdn, n, dbContext, appSettings, cancellationToken);
            else
                await ReloadAsync(existing, dbContext, appSettings, cancellationToken);
        }
    }
    
    /// <summary>
    /// Reloads libraries by name for the current upstream CDN service.
    /// </summary>
    /// <param name="upstreamCdn">The upstream service to remove libraries from.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="appSettings">Application settings.</param>
    /// <param name="libraryNames">The names of libraries to reload.</param>
    /// <param name="cancellationToken">Triggered when <see cref="Microsoft.Extensions.Hosting.IHostedService.StopAsync(CancellationToken)"/> is called.</param>
    public async Task ReloadExistingAsync(UpstreamCdn upstreamCdn, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken cancellationToken)
    {
        LinkedList<CdnLibrary> toReload = await GetMatchingLibrariesAsync(upstreamCdn.Name, upstreamCdn.Id, dbContext, libraryNames, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            foreach (CdnLibrary r in toReload)
                await ReloadAsync(r, dbContext, appSettings, cancellationToken);
    }
    
    /// <summary>
    /// Removes libararies by name for the current upstream CDN service.
    /// </summary>
    /// <param name="upstreamCdn">The upstream CDN service to remove libraries from.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="libraryNames">The names of the libraries to remove.</param>
    /// <param name="cancellationToken">Triggered when <see cref="Microsoft.Extensions.Hosting.IHostedService.StopAsync(CancellationToken)"/> is called.</param>
    public async Task RemoveAsync(UpstreamCdn upstreamCdn, ContentDb dbContext, IEnumerable<string> libraryNames, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;
        LinkedList<CdnLibrary> toRemove = await GetMatchingLibrariesAsync(upstreamCdn.Name, upstreamCdn.Id, dbContext, libraryNames, cancellationToken);
        if (cancellationToken.IsCancellationRequested || toRemove.First is null)
            return;
        foreach (CdnLibrary library in toRemove)
        {
            await library.RemoveAsync(dbContext, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
        }
        await dbContext.SaveChangesAsync(true, cancellationToken);
    }

    protected async Task<LinkedList<CdnLibrary>> GetMatchingLibrariesAsync(string cdnName, Guid upstreamCdnId, ContentDb dbContext, IEnumerable<string> libraryNames, CancellationToken cancellationToken)
    {
        LinkedList<CdnLibrary> result = new();
        foreach (string name in libraryNames)
        {
            if (cancellationToken.IsCancellationRequested)
                return result;
            CdnLibrary? library = await dbContext.CdnLibraries.Include(r => r.Local).FirstOrDefaultAsync(l => l.Local!.Name == name && l.UpstreamCdnId == upstreamCdnId, cancellationToken);
            if (library is null)
                GetLogger().LogCdnLibraryNotFound(name, cdnName);
            else
                result.AddLast(library);
        }
        return result;
    }

    protected static async Task<CdnLibrary?> GetMatchingLibraryAsync(Guid upstreamCdnId, ContentDb dbContext, string libraryName, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return null;
        return await dbContext.CdnLibraries.Include(r => r.Local).FirstOrDefaultAsync(l => l.Local!.Name == libraryName && l.UpstreamCdnId == upstreamCdnId, cancellationToken);
    }
}
