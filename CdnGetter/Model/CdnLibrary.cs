using System.Collections.ObjectModel;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CdnGetter.SqlDefinitions;

namespace CdnGetter.Model;

/// <summary>
/// Represents a content library.
/// </summary>
public class CdnLibrary
{
    private readonly object _syncRoot = new();

    private Guid _localId;
    /// <summary>
    /// The unique identifier of the parent <see cref="LocalLibrary" />.
    /// </summary>
    public Guid LocalId
    {
        get => _upstreamCdnId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _upstreamCdnId, ref _upstreamCdn);
    }

    private LocalLibrary? _local;
    /// <summary>
    /// The remote service for this content library.
    /// </summary>
    public LocalLibrary? Local
    {
        get => _local;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _localId, ref _local);
    }

    private Guid _upstreamCdnId;
    /// <summary>
    /// The unique identifier of the parent <see cref="UpstreamCdn" />.
    /// </summary>
    public Guid UpstreamCdnId
    {
        get => _upstreamCdnId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _upstreamCdnId, ref _upstreamCdn);
    }

    private UpstreamCdn? _upstreamCdn;
    /// <summary>
    /// The remote service for this content library.
    /// </summary>
    public UpstreamCdn? UpstreamCdn
    {
        get => _upstreamCdn;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _upstreamCdnId, ref _upstreamCdn);
    }
    
    /// <summary>
    /// The preferential order override for the remote CDN or <see langword="null" /> to use <see cref="UpstreamCdn.Priority" />.
    /// </summary>
    public ushort? Priority { get; set; }

    private string? _description;
    public string? Description
    {
        get => _description;
        set => _description = value.ToTrimmedOrNullIfEmpty();
    }

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
    /// Library versions for this content library.
    /// </summary>
    public Collection<CdnVersion> Versions { get; set; } = new();
    
    /// <summary>
    /// CDN acess logs for this content library.
    /// </summary>
    public Collection<LibraryLog> Logs { get; set; } = new();
    
    /// <summary>
    /// Performs configuration of the <see cref="CdnLibrary" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<CdnLibrary> builder)
    {
        _ = builder.HasKey(nameof(LocalId), nameof(UpstreamCdnId));
        _ = builder.Property(nameof(LocalId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(UpstreamCdnId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(ProviderData)).HasConversion(ExtensionMethods.JsonValueConverter);
        _ = builder.Property(nameof(CreatedOn)).IsRequired().HasDefaultValueSql(DEFAULT_SQL_NOW);
        _ = builder.Property(nameof(ModifiedOn)).IsRequired().HasDefaultValueSql(DEFAULT_SQL_NOW);
        _ = builder.HasOne(f => f.UpstreamCdn).WithMany(v => v.Libraries).HasForeignKey(f => f.UpstreamCdnId).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
        _ = builder.HasOne(r => r.Local).WithMany(l => l.CDNs).HasForeignKey(r => r.LocalId).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
    }

    internal static void CreateTable(Action<string> executeNonQuery)
    {
        executeNonQuery(@$"CREATE TABLE ""{nameof(Services.ContentDb.CdnLibraries)}"" (
    ""{nameof(LocalId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(UpstreamCdnId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(Priority)}"" UNSIGNED SMALLINT DEFAULT NULL,
    ""{nameof(Description)}"" TEXT DEFAULT NULL CHECK(""{nameof(Description)}"" IS NULL OR length(trim(""{nameof(Description)}""))=length(""{nameof(Description)}"")),
    ""{nameof(ProviderData)}"" TEXT DEFAULT NULL,
    ""{nameof(CreatedOn)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(ModifiedOn)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    PRIMARY KEY(""{nameof(LocalId)}"",""{nameof(UpstreamCdnId)}""),
    FOREIGN KEY(""{nameof(UpstreamCdnId)}"") REFERENCES ""{nameof(Services.ContentDb.UpstreamCdns)}""(""{nameof(Model.UpstreamCdn.Id)}"") ON DELETE RESTRICT,
    FOREIGN KEY(""{nameof(LocalId)}"") REFERENCES ""{nameof(Services.ContentDb.LocalLibraries)}""(""{nameof(LocalLibrary.Id)}"") ON DELETE RESTRICT,
    CHECK(""{nameof(CreatedOn)}""<=""{nameof(ModifiedOn)}"")
)");
    }

    internal async Task ClearVersionsAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        Guid id = _localId;
        Guid upstreamCdnId = _upstreamCdnId;
        foreach (CdnVersion toRemove in await dbContext.CdnVersions.Where(l => l.LibraryId == id && l.UpstreamCdnId == upstreamCdnId).ToArrayAsync(cancellationToken))
        {
            await toRemove.RemoveAsync(dbContext, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
        }
        Versions.Clear();
    }

    internal async Task RemoveAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        await ClearVersionsAsync(dbContext, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        dbContext.CdnLibraries.Remove(this);
        await dbContext.SaveChangesAsync(true, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        LocalLibrary? local = await dbContext.CdnLibraries.Entry(this).EnsureRelatedAsync(l => l.Local, cancellationToken);
        if (local is not null)
        {
            Guid id = local.Id;
            if (!await dbContext.CdnLibraries.AnyAsync(r => r.LocalId == id, cancellationToken: cancellationToken))
                await local.RemoveAsync(dbContext, cancellationToken);
        }
    }
}