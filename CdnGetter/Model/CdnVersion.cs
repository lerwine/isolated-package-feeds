using System.Collections.ObjectModel;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using static CdnGetter.SqlExtensions;

namespace CdnGetter.Model;

/// <summary>
/// Represents a specific version of an upstream content library.
/// </summary>
public class CdnVersion : ModificationTrackingModelBase
{
    private readonly object _syncRoot = new();

    private Guid _localId;
    /// <summary>
    /// The unique identifier of the parent <see cref="LocalVersion" />.
    /// </summary>
    public Guid LocalId
    {
        get => _localId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _localId, ref _local);
    }

    private LocalVersion? _local;
    /// <summary>
    /// The content library version that the current file belongs to.
    /// </summary>
    public LocalVersion? Local
    {
        get => _local;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _localId, ref _local);
    }

    private Guid _libraryId;
    /// <summary>
    /// The unique identifier of the ancestor <see cref="LocalLibrary" />.
    /// </summary>
    public Guid LibraryId
    {
        get => _libraryId;
        set => value.SetNavigation(_upstreamCdnId, _syncRoot, p => (p.LocalId, p.CdnId), ref _libraryId, ref _upstreamCdnId, ref _library);
    }

    private Guid _upstreamCdnId;
    /// <summary>
    /// The unique identifier of the parent <see cref="UpstreamCdn" />.
    /// </summary>
    public Guid UpstreamCdnId
    {
        get => _upstreamCdnId;
        set => _libraryId.SetNavigation(value, _syncRoot, p => (p.LocalId, p.CdnId), ref _libraryId, ref _upstreamCdnId, ref _library);
    }

    private CdnLibrary? _library;
    /// <summary>
    /// The parent content library.
    /// </summary>
    public CdnLibrary? Library
    {
        get => _library;
        set => value.SetNavigation(_syncRoot, p => (p.LocalId, p.CdnId), ref _libraryId, ref _upstreamCdnId, ref _library);
    }
    
    /// <summary>
    /// The preferential order override for the upstream CDN or <see langword="null" /> to use <see cref="CdnLibrary.Priority" /> or <see cref="UpstreamCdn.Priority" />.
    /// </summary>
    public ushort? Priority { get; set; }

    /// <summary>
    /// Optional provider-specific data for <see cref="UpstreamCdn" />.
    /// </summary>
    public JsonNode? ProviderData { get; set; }

    /// <summary>
    /// The files that belong to the current version of the content library.
    /// </summary>
    public Collection<CdnFile> Files { get; set; } = new();
    
    /// <summary>
    /// CDN acess logs for this content library version.
    /// </summary>
    public Collection<VersionLog> Logs { get; set; } = new();

    /// <summary>
    /// Performs configuration of the <see cref="CdnVersion" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<CdnVersion> builder)
    {
        _ = builder.HasKey(nameof(LocalId), nameof(LibraryId), nameof(UpstreamCdnId));
        _ = builder.Property(nameof(LocalId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(LibraryId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(UpstreamCdnId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(ProviderData)).HasConversion(ValueConverters.JsonValueConverter);
        ModificationTrackingModelBase.OnBuildModificationTrackingModel(builder);
        _ = builder.HasOne(f => f.Local).WithMany(f => f.Upstream).HasForeignKey(nameof(LocalId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(v => v.Library).WithMany(l => l.Versions).HasForeignKey(nameof(LibraryId), nameof(UpstreamCdnId)).HasPrincipalKey(nameof(LocalId), nameof(UpstreamCdnId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }

    internal static void CreateTable(Action<string> executeNonQuery)
    {
        executeNonQuery(@$"CREATE TABLE ""{nameof(Services.ContentDb.CdnVersions)}"" (
    ""{nameof(LocalId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(LibraryId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(UpstreamCdnId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(Priority)}"" UNSIGNED SMALLINT DEFAULT NULL,
    ""{nameof(ProviderData)}"" TEXT DEFAULT NULL,
    ""{nameof(CreatedOn)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(ModifiedOn)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    FOREIGN KEY(""{nameof(LocalId)}"") REFERENCES ""{nameof(Services.ContentDb.LocalVersions)}""(""{nameof(LocalVersion.Id)}"") ON DELETE RESTRICT,
    FOREIGN KEY(""{nameof(LibraryId)}"",""{nameof(UpstreamCdnId)}"") REFERENCES ""{nameof(Services.ContentDb.CdnLibraries)}""(""{nameof(CdnLibrary.LocalId)}"",""{nameof(CdnLibrary.CdnId)}"") ON DELETE RESTRICT,
    PRIMARY KEY(""{nameof(LocalId)}"", ""{nameof(LibraryId)}"", ""{nameof(UpstreamCdnId)}""),
    CHECK(""{nameof(CreatedOn)}""<=""{nameof(ModifiedOn)}"")
)");
    }

    internal async Task ClearFilesAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        Guid id = _localId;
        Guid libraryId = _libraryId;
        Guid upstreamCdnId = _upstreamCdnId;
        CdnFile[] toRemove = await dbContext.CdnFiles.Where(f => f.VersionId == id && f.LibraryId == libraryId && f.UpstreamCdnId == upstreamCdnId).ToArrayAsync(cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        if (toRemove.Length > 0)
        {
            dbContext.CdnFiles.RemoveRange(toRemove);
            await dbContext.SaveChangesAsync(true, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
            Files.Clear();
        }
    }
    
    internal async Task RemoveAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        await ClearFilesAsync(dbContext, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        dbContext.CdnVersions.Remove(this);
        await dbContext.SaveChangesAsync(true, cancellationToken);
    }
    
    internal static async Task ShowAsync(IEnumerable<string> libraryNames, IEnumerable<string> upstreamNames, Services.ContentDb dbContext, ILogger logger, CancellationToken cancellationToken)
    {
        if (libraryNames.Any())
        {
            LinkedList<LocalLibrary> localLibraries = new();
            foreach (string n in libraryNames)
            {
                LocalLibrary? l = await dbContext.FindLibraryByNameAsync(n, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    return;
                if (l is null)
                    logger.LogLocalLibraryNotFoundWarning(n);
                else
                    localLibraries.AddLast(l);
            }
            if (localLibraries.First is null)
                return;
            LinkedList<UpstreamCdn> upstreamCdns = new();
            foreach (string n in upstreamNames)
            {
                UpstreamCdn? c = await dbContext.FindCdnByNameAsync(n, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    return;
                if (c is null)
                    logger.LogUpstreamCdnNotFoundError(n);
                else
                    upstreamCdns.AddLast(c);
            }
            if (localLibraries.First.Next is null)
            {
                Guid libraryId = localLibraries.First.Value.Id;
                string libName = localLibraries.First.Value.Name;
                if (upstreamCdns.First is null)
                {
                    LocalVersion[] localVersions = await dbContext.LocalVersions.Where(v => v.LibraryId == libraryId).OrderByDescending(v => v.Order).ToArrayAsync(cancellationToken);
                    if (localVersions.Length == 0)
                        Console.WriteLine("(none)");
                    else
                        foreach (LocalVersion v in localVersions)
                            Console.WriteLine(v.Version.ToString());
                }
                else if (upstreamCdns.First.Next is null)
                {
                    Guid cdnId = upstreamCdns.First.Value.Id;
                    CdnVersion[] cdnVersions = await dbContext.CdnVersions.Where(v => v.LibraryId == libraryId && v.UpstreamCdnId == cdnId).Include(v => v.Local)
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
                        CdnVersion[] cdnVersions = await dbContext.CdnVersions.Where(v => v.LibraryId == libraryId && v.UpstreamCdnId == cdnId).Include(v => v.Local)
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
                    foreach (LocalVersion localVersion in await dbContext.LocalVersions.Where(v => v.LibraryId == libraryId).OrderByDescending(v => v.Order).ToArrayAsync(cancellationToken))
                    {
                        Console.WriteLine($"    {localVersion.Version}");
                        Guid versionId = localVersion.Id;
                        string[] names = await dbContext.CdnVersions.Where(v => v.LibraryId == libraryId && v.LocalId == versionId).Include(v => v.Library).ThenInclude(v => v!.Cdn).OrderBy(v => v.Priority)
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
                    CdnVersion[] cdnVersions = await dbContext.CdnVersions.Where(v => v.LibraryId == libraryId && v.UpstreamCdnId == cdnId).Include(v => v.Local)
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
                        CdnVersion[] cdnVersions = await dbContext.CdnVersions.Where(v => v.LibraryId == libraryId && v.UpstreamCdnId == cdnId).Include(v => v.Local)
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
        else
            logger.LogNoLibraryNameSpecifiedWarning(nameof(Config.CommandSettings.SHOW_Versions));
    }

    internal static async Task AddNewAsync(IEnumerable<string> cdnNames, Services.ContentDb dbContext, ILogger<Services.MainService> logger, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Add new versions functionality not implemented.");
    }
}
