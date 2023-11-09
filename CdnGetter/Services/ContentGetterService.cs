using System.Diagnostics.CodeAnalysis;
using CdnGetter.Config;
using CdnGetter.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CdnGetter.Services;

/// <summary>
/// Base class for services that retrieve content from remote CDNs.
/// </summary>
[Obsolete("Storage and retrieval should exist as separate services")]
public abstract class ContentGetterService
{
    private readonly AppSettings _settings;
    private readonly IHostEnvironment _hostEnvironment;

    /// <summary>
    /// Gets the current database context.
    /// </summary>
    protected ContentDb DbContext { get; }

    /// <summary>
    /// Initializes a new <c>ContentGetterService</c> object.
    /// </summary>
    /// <param name="dbContext">The injected database context.</param>
    /// <param name="options">The injected application settings.</param>
    /// <param name="hostEnvironment">The injected hosting environment information.</param>
    protected ContentGetterService(ContentDb dbContext, IOptions<AppSettings> options, IHostEnvironment hostEnvironment)
    {
        DbContext = dbContext;
        _settings = options.Value ?? new();
        _hostEnvironment = hostEnvironment;
    }
    
    /// <summary>
    /// Gets the current logger object.
    /// </summary>
    protected abstract ILogger GetLogger();

    private object? _contentDirectory;

    protected bool TryGetContentDirectory([NotNullWhen(true)] out DirectoryInfo? result)
    {
        if (_contentDirectory is null)
        {
            string path = AppSettings.GetLocalStoragePath(_settings); 
            try
            {
                _contentDirectory = result = new DirectoryInfo(Path.IsPathFullyQualified(path) ? path : Path.Combine(_hostEnvironment.ContentRootPath, path));
                if (result.Exists)
                    return true;
            }
            catch (Exception exception)
            {
                _contentDirectory = exception;
                GetLogger().LogInvalidContentRootError(path, exception);
                result = null;
                return false;
            }
            if (result.Parent is null)
            {
                GetLogger().LogCannotCreateCdnContentRootError(result.FullName);
                return false;
            }
            try
            {
                if (File.Exists(result.FullName))
                    throw new InvalidOperationException("Local storage path does not refer to a subdirectory.");
                if (!result.Parent.Exists)
                    result.Parent.Create();
                result.Create();
            }
            catch (Exception exception)
            {
                GetLogger().LogCannotCreateCdnContentRootError(result.FullName, exception);
                return false;
            }
            return true;
        }
        return (result = _contentDirectory as DirectoryInfo) is not null && result.Exists;
    }

    /// <summary>
    /// Asynchonously reloads local content from a remote content library.
    /// </summary>
    /// <param name="library">The remote content library.</param>
    /// <param name="cancellationToken">The token that indicates when the application is stopping.</param>
    protected abstract Task ReloadAsync(CdnLibrary library, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="upstreamCdn"></param>
    /// <param name="libraryName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task AddNewVersionsAsync(UpstreamCdn upstreamCdn, string libraryName, CancellationToken cancellationToken);

    public abstract Task GetNewVersionsAsync(CdnLibrary cdnLibrary, CancellationToken cancellationToken);
    
    // public async Task GetNewVersionsAsync(UpstreamCdn upstreamCdn, ContentDb dbContext, AppSettings appSettings, IEnumerable<string> libraryNames, CancellationToken cancellationToken)
    // {
    //     foreach (string n in libraryNames)
    //     {
    //         CdnLibrary? existing = await GetMatchingLibraryAsync(upstreamCdn.Id, n, cancellationToken);
    //         if (existing is null)
    //             await AddNewVersionsAsync(upstreamCdn, n, cancellationToken);
    //         else
    //             await ReloadAsync(existing, cancellationToken);
    //     }
    // }
    
    public abstract Task AddAsync(UpstreamCdn upstreamCdn, IEnumerable<string> libraryNames, CancellationToken cancellationToken);
    
    public async Task ReloadAsync(UpstreamCdn upstreamCdn, IEnumerable<string> libraryNames, CancellationToken cancellationToken)
    {
        foreach (string n in libraryNames)
        {
            CdnLibrary? existing = await GetMatchingLibraryAsync(upstreamCdn.Id, n, cancellationToken);
            if (existing is null)
                await AddNewVersionsAsync(upstreamCdn, n, cancellationToken);
            else
                await ReloadAsync(existing, cancellationToken);
        }
    }

    /// <summary>
    /// Reloads libraries by name for the current upstream CDN service.
    /// </summary>
    /// <param name="upstreamCdn">The upstream service to remove libraries from.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="appSettings">Application settings.</param>
    /// <param name="libraryNames">The names of libraries to reload.</param>
    /// <param name="cancellationToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)"/> is called.</param>
    public async Task ReloadExistingAsync(UpstreamCdn upstreamCdn, IEnumerable<string> libraryNames, CancellationToken cancellationToken)
    {
        LinkedList<CdnLibrary> toReload = await GetMatchingLibrariesAsync(upstreamCdn.Name, upstreamCdn.Id, libraryNames, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            foreach (CdnLibrary r in toReload)
                await ReloadAsync(r, cancellationToken);
    }

    /// <summary>
    /// Removes libararies by name for the current upstream CDN service.
    /// </summary>
    /// <param name="upstreamCdn">The upstream CDN service to remove libraries from.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="libraryNames">The names of the libraries to remove.</param>
    /// <param name="cancellationToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)"/> is called.</param>
    public async Task RemoveAsync(UpstreamCdn upstreamCdn, IEnumerable<string> libraryNames, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;
        LinkedList<CdnLibrary> toRemove = await GetMatchingLibrariesAsync(upstreamCdn.Name, upstreamCdn.Id, libraryNames, cancellationToken);
        if (cancellationToken.IsCancellationRequested || toRemove.First is null)
            return;
        foreach (CdnLibrary library in toRemove)
        {
            await library.RemoveAsync(this.DbContext, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
        }
        await this.DbContext.SaveChangesAsync(true, cancellationToken);
    }

    protected async Task<LinkedList<CdnLibrary>> GetMatchingLibrariesAsync(string cdnName, Guid upstreamCdnId, IEnumerable<string> libraryNames, CancellationToken cancellationToken)
    {
        LinkedList<CdnLibrary> result = new();
        foreach (string name in libraryNames)
        {
            if (cancellationToken.IsCancellationRequested)
                return result;
            CdnLibrary? library = await this.DbContext.CdnLibraries.Include(r => r.Local).FirstOrDefaultAsync(l => l.Local!.Name == name && l.CdnId == upstreamCdnId, cancellationToken);
            if (library is null)
                GetLogger().LogCdnLibraryNotFoundWarning(name, cdnName);
            else
                result.AddLast(library);
        }
        return result;
    }

    protected async Task<CdnLibrary?> GetMatchingLibraryAsync(Guid upstreamCdnId, string libraryName, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return null;
        return await this.DbContext.CdnLibraries.Include(r => r.Local).FirstOrDefaultAsync(l => l.Local!.Name == libraryName && l.CdnId == upstreamCdnId, cancellationToken);
    }
}
