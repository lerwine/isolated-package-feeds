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
            UpstreamCdn? cdn = await _dbContext.FindCdnByNameAsync(n, stoppingToken);
            if (cdn is null)
                _logger.LogUpstreamCdnNotFoundError(n);
            else
                result.AddLast(cdn);
        }
        return result;
    }

    private async Task<(UpstreamCdn?, Type)> GetUpstreamCdnAsync(string cdnName, CancellationToken stoppingToken)
    {
        UpstreamCdn? cdn = await _dbContext.FindCdnByNameAsync(cdnName, stoppingToken);
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
                        await ShowLibrariesAsync(stoppingToken);
                }
                else if (sv.Equals(Config.CommandSettings.SHOW_Versions, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_commandSettings.Version.TrimmedNotEmptyValues().Any())
                        _logger.LogMutuallyExclusiveSwitchWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Show)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Version)}");
                    else
                        await ShowVersionsAsync(stoppingToken);
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

    private async Task ShowVersionsAsync(CancellationToken cancellationToken)
    {
        string[] libraryNames = _commandSettings.Library.TrimmedNotEmptyValues().Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();
        if (libraryNames.Length == 0)
            _logger.LogNoLibraryNameSpecifiedWarning(nameof(Config.CommandSettings.SHOW_Versions));
        else
        {
            LinkedList<LocalLibrary> localLibraries = new();
            foreach (string n in libraryNames)
            {
                LocalLibrary? l = await _dbContext.FindLibraryByNameAsync(n, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    return;
                if (l is null)
                    _logger.LogLocalLibraryNotFoundWarning(n);
                else
                    localLibraries.AddLast(l);
            }
            if (localLibraries.First is null)
                return;
            LinkedList<UpstreamCdn> upstreamCdns = new();
            foreach (string n in _commandSettings.Upstream.TrimmedNotEmptyValues().Distinct(StringComparer.InvariantCultureIgnoreCase))
            {
                UpstreamCdn? c = await _dbContext.FindCdnByNameAsync(n, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    return;
                if (c is null)
                    _logger.LogUpstreamCdnNotFoundError(n);
                else
                    upstreamCdns.AddLast(c);
            }
            if (localLibraries.First.Next is null)
            {
                Guid libraryId = localLibraries.First.Value.Id;
                string libName = localLibraries.First.Value.Name;
                if (upstreamCdns.First is null)
                {
                    LocalVersion[] localVersions = await _dbContext.LocalVersions.Where(v => v.LibraryId == libraryId).OrderByDescending(v => v.Order).ToArrayAsync(cancellationToken);
                    if (localVersions.Length == 0)
                        Console.WriteLine("(none)");
                    else
                        foreach (LocalVersion v in localVersions)
                            Console.WriteLine(v.Version.ToString());
                }
                else if (upstreamCdns.First.Next is null)
                {
                    Guid cdnId = upstreamCdns.First.Value.Id;
                    CdnVersion[] cdnVersions = await _dbContext.CdnVersions.Where(v => v.LibraryId == libraryId && v.UpstreamCdnId == cdnId).Include(v => v.Local)
                        .OrderByDescending(v => v.Local!.Order).ToArrayAsync(cancellationToken);
                    if (cdnVersions.Length == 0)
                        Console.WriteLine("(none)");
                    else
                        foreach (CdnVersion v in cdnVersions)
                            Console.WriteLine(v.Local!.Version.ToString());
                }
                else
                    foreach (UpstreamCdn cdn in upstreamCdns)
                    {
                        Guid cdnId = cdn.Id;
                        CdnVersion[] cdnVersions = await _dbContext.CdnVersions.Where(v => v.LibraryId == libraryId && v.UpstreamCdnId == cdnId).Include(v => v.Local)
                            .OrderByDescending(v => v.Local!.Order).ToArrayAsync(cancellationToken);
                        Console.WriteLine($"{cdn.Name}:");
                        if (cdnVersions.Length == 0)
                            Console.WriteLine("    (none)");
                        else
                            foreach (CdnVersion v in cdnVersions)
                                Console.WriteLine($"    {v.Local!.Version}");
                    }
            }
            else if (upstreamCdns.First is null)
            {
                foreach (LocalLibrary localLibrary in localLibraries)
                {
                    Console.WriteLine($"Library: {localLibrary.Name}");
                    Guid libraryId = localLibrary.Id;
                    foreach (LocalVersion localVersion in await _dbContext.LocalVersions.Where(v => v.LibraryId == libraryId).OrderByDescending(v => v.Order).ToArrayAsync(cancellationToken))
                    {
                        Console.WriteLine($"    {localVersion.Version}");
                        Guid versionId = localVersion.Id;
                        string[] names = await _dbContext.CdnVersions.Where(v => v.LibraryId == libraryId && v.LocalId == versionId).Include(v => v.Library).ThenInclude(v => v!.Cdn).OrderBy(v => v.Priority)
                            .Select(v => v.Library!.Cdn!.Name).ToArrayAsync(cancellationToken);
                        if (names.Length == 1)
                            Console.WriteLine($"        CDN: {names[0]}");
                        else
                        {
                            string t = string.Join(", ", names);
                            Console.WriteLine($"        CDNs: {t}");
                        }
                    }
                }
            }
            else if (upstreamCdns.First.Next is null)
            {
                Guid cdnId = upstreamCdns.First.Value.Id;
                foreach (LocalLibrary localLibrary in localLibraries)
                {
                    Guid libraryId = localLibrary.Id;
                    CdnVersion[] cdnVersions = await _dbContext.CdnVersions.Where(v => v.LibraryId == libraryId && v.UpstreamCdnId == cdnId).Include(v => v.Local)
                        .OrderByDescending(v => v.Local!.Order).ToArrayAsync(cancellationToken);
                    Console.WriteLine($"{localLibrary.Name}:");
                    if (cdnVersions.Length == 0)
                        Console.WriteLine("    (none)");
                    else
                    {
                        string t = string.Join("; ", cdnVersions.Select(v => v.Local!.Version.ToString()));
                        Console.WriteLine($"    Versions: {t}");
                    }
                }
            }
            else
                foreach (LocalLibrary localLibrary in localLibraries)
                {
                    Console.WriteLine($"{localLibrary.Name}:");
                    Guid libraryId = localLibrary.Id;
                    foreach (UpstreamCdn upstreamCdn in upstreamCdns)
                    {
                        Guid cdnId = upstreamCdn.Id;
                        CdnVersion[] cdnVersions = await _dbContext.CdnVersions.Where(v => v.LibraryId == libraryId && v.UpstreamCdnId == cdnId).Include(v => v.Local)
                            .OrderByDescending(v => v.Local!.Order).ToArrayAsync(cancellationToken);
                        Console.WriteLine($"    CDN: {upstreamCdn.Name}:");
                        if (cdnVersions.Length == 0)
                            Console.WriteLine("        (none)");
                        else
                            foreach (CdnVersion v in cdnVersions)
                                Console.WriteLine($"    {v.Local!.Version}");
                    }
                }
        }
    }

    private async Task ShowVersionsAsynOldc(CancellationToken cancellationToken)
    {
        using AsyncLookupEnumerator<string, LocalLibrary> libraryEnumerator = new(_commandSettings.Library.TrimmedNotEmptyValues().Distinct(StringComparer.InvariantCultureIgnoreCase), async (n, t) =>
        {
            LocalLibrary? l = await _dbContext.FindLibraryByNameAsync(n, t);
            if (l is null)
                _logger.LogLocalLibraryNotFoundWarning(n);
            return l;
        });
        if (cancellationToken.IsCancellationRequested)
            return;
        if (await libraryEnumerator.MoveNextAsync(cancellationToken))
        {
            LocalLibrary localLibrary = libraryEnumerator.Current;
            Guid libraryId = localLibrary.Id;
            using AsyncLookupEnumerator<string, UpstreamCdn> cdnEnumerator = new(_commandSettings.Upstream.TrimmedNotEmptyValues().Distinct(StringComparer.InvariantCultureIgnoreCase), async (n, t) =>
            {
                UpstreamCdn? c = await _dbContext.FindCdnByNameAsync(n, t);
                if (c is null)
                    _logger.LogUpstreamCdnNotFoundError(n);
                return c;
            });
            if (await cdnEnumerator.MoveNextAsync(cancellationToken))
            {
                UpstreamCdn cdn = cdnEnumerator.Current;
                Guid cdnId = cdn.Id;
                if (await cdnEnumerator.MoveNextAsync(cancellationToken))
                {
                    if (await libraryEnumerator.MoveNextAsync(cancellationToken))
                    {
                        Console.WriteLine($"Library: {localLibrary.Name}");
                        Console.WriteLine($"    CDN: {cdn.Name}");
                        CdnVersion[] versionEntities = await _dbContext.CdnVersions.Where(v => v.LibraryId == libraryId && v.UpstreamCdnId == cdnId).Include(v => v.Local).ToArrayAsync(cancellationToken);

                        cdn = cdnEnumerator.Current;
                        // Emit each CDN and Library name.
                        throw new NotImplementedException("Show library versions for upstream CDNs functionality not implemented");
                    }
                    else
                    {
                        // Emit each CDN name.
                        throw new NotImplementedException("Show versions for specific library of each upstream CDN functionality not implemented");
                    }
                }
                else if (await libraryEnumerator.MoveNextAsync(cancellationToken))
                {
                    // Emit each Library name.
                    throw new NotImplementedException("Show versions for libraries functionality not implemented");
                }
                else
                {
                    throw new NotImplementedException("Show versions for specific library functionality not implemented");
                }
            }
            else if (await libraryEnumerator.MoveNextAsync(cancellationToken))
            {
                // Emit each Library name.
                throw new NotImplementedException("Show versions for libraries functionality not implemented");
            }
            else
            {
                throw new NotImplementedException("Show versions for specific library functionality not implemented");
            }
        }
        else if (libraryEnumerator.IterationCount == 0 && !cancellationToken.IsCancellationRequested)
            _logger.LogNoLibraryNameSpecifiedWarning(nameof(Config.CommandSettings.SHOW_Versions));
    }

    private async Task ShowLibrariesAsync(CancellationToken stoppingToken)
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

sealed class AsyncLookupEnumerator<TInput, TResult> : IDisposable
    where TResult : class
{
    private readonly IEnumerator<TInput> _backingEnumerator;
    private readonly Func<TInput, CancellationToken, Task<TResult?>> _getResult;

    internal int IterationCount { get; private set; }

    internal TResult Current { get; private set; } = null!;

    internal AsyncLookupEnumerator(IEnumerable<TInput> source, Func<TInput, CancellationToken, Task<TResult?>> getResult)
    {
        _backingEnumerator = source.GetEnumerator();
        _getResult = getResult;
    }
    
    public async Task<bool> MoveNextAsync(CancellationToken cancellationToken)
    {
        while (_backingEnumerator.MoveNext())
        {
            IterationCount++;
            TResult? result = await _getResult(_backingEnumerator.Current, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                break;
            if (result is not null)
            {
                Current = result;
                return true;
            }
        }
        return false;
    }

    public void Dispose() => _backingEnumerator.Dispose();
}