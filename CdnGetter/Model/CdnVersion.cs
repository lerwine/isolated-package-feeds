using System.Collections.ObjectModel;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CdnGetter.SqlDefinitions;

namespace CdnGetter.Model;

/// <summary>
/// Represents a specific version of an upstream content library.
/// </summary>
public class CdnVersion
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
    /// The unique identifier of the parent <see cref="CdnLibrary" />.
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
    /// The date and time that the record was created.
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.Now;


    /// <summary>
    /// The date and time that the record was last modified.
    /// </summary>
    public DateTime ModifiedOn { get; set; } = DateTime.Now;

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
        _ = builder.Property(nameof(CreatedOn)).IsRequired().HasDefaultValueSql(DEFAULT_SQL_NOW);
        _ = builder.Property(nameof(ModifiedOn)).IsRequired().HasDefaultValueSql(DEFAULT_SQL_NOW);
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
}
