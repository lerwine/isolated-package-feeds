using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using static CdnGetter.SqlDefinitions;

namespace CdnGetter.Model;

/// <summary>
/// Represents a content library.
/// </summary>
public class RemoteLibrary
{
    private readonly object _syncRoot = new();

    private Guid _localId;
    /// <summary>
    /// The unique identifier of the parent <see cref="LocalLibrary" />.
    /// </summary>
    public Guid LocalId
    {
        get => _remoteServiceId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _remoteServiceId, ref _remoteService);
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

    private Guid _remoteServiceId;
    /// <summary>
    /// The unique identifier of the parent <see cref="RemoteService" />.
    /// </summary>
    public Guid RemoteServiceId
    {
        get => _remoteServiceId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _remoteServiceId, ref _remoteService);
    }

    private RemoteService? _remoteService;
    /// <summary>
    /// The remote service for this content library.
    /// </summary>
    public RemoteService? RemoteService
    {
        get => _remoteService;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _remoteServiceId, ref _remoteService);
    }
    
    /// <summary>
    /// The preferential order override for the remote CDN or <see langword="null" /> to use <see cref="RemoteService.Priority" />.
    /// </summary>
    public ushort? Priority { get; set; }

    private string? _description;
    public string? Description
    {
        get => _description;
        set => _description = value.ToTrimmedOrNullIfEmpty();
    }

    /// <summary>
    /// Optional provider-specific data for <see cref="RemoteService" />.
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
    public Collection<RemoteVersion> Versions { get; set; } = new();
    
    /// <summary>
    /// Remote acess logs for this content library.
    /// </summary>
    public Collection<LibraryLog> Logs { get; set; } = new();
    
    /// <summary>
    /// Performs configuration of the <see cref="RemoteLibrary" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<RemoteLibrary> builder)
    {
        _ = builder.HasKey(nameof(LocalId), nameof(RemoteServiceId));
        _ = builder.Property(nameof(LocalId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(RemoteServiceId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(ProviderData)).HasConversion(ExtensionMethods.JsonValueConverter);
        _ = builder.HasOne(f => f.RemoteService).WithMany(v => v.Libraries).HasForeignKey(f => f.RemoteServiceId).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
        _ = builder.HasOne(r => r.Local).WithMany(l => l.Remotes).HasForeignKey(r => r.LocalId).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
    }

    internal static void CreateTable(Action<string> executeNonQuery, ILogger logger)
    {
        /*
        CREATE TABLE IF NOT EXISTS "RemoteLibraries" (
            "LocalId" UNIQUEIDENTIFIER NOT NULL CONSTRAINT "FK_RemoteLibrary_LocalLibrary" REFERENCES "LocalLibraries"("Id") ON DELETE RESTRICT COLLATE NOCASE,
            "RemoteServiceId" UNIQUEIDENTIFIER NOT NULL CONSTRAINT "FK_RemoteLibrary_RemoteService" REFERENCES "RemoteServices"("Id") ON DELETE RESTRICT COLLATE NOCASE,
            "Priority" UNSIGNED SMALLINT DEFAULT NULL,
            "Description" TEXT NOT NULL CHECK(length(trim("Description"))=length("Description")),
            "ProviderData" TEXT DEFAULT NULL,
            "CreatedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
            "ModifiedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
            CONSTRAINT "PK_RemoteLibraries" PRIMARY KEY("LocalId", "RemoteServiceId"),
            CHECK("CreatedOn"<="ModifiedOn")
        );
        */
        executeNonQuery(@$"CREATE TABLE IF NOT EXISTS ""{nameof(Services.ContentDb.RemoteLibraries)}"" (
    {SqlReferenceColumn(nameof(RemoteLibrary), nameof(LocalId), nameof(LocalLibrary), nameof(LocalLibrary.Id), nameof(Services.ContentDb.LocalLibraries))},
    {SqlReferenceColumn(nameof(RemoteLibrary), nameof(RemoteServiceId), nameof(Model.RemoteService), nameof(Model.RemoteService.Id), nameof(Services.ContentDb.RemoteServices))},
    {SqlSmallUInt(nameof(Priority), true)},
    {SqlTextTrimmed(nameof(Description))},
    {SqlText(nameof(ProviderData), true)}
    {SqlDateTime(nameof(CreatedOn))},
    {SqlDateTime(nameof(ModifiedOn))},
    {SqlPkConstraint(nameof(Services.ContentDb.RemoteLibraries), nameof(LocalId), nameof(RemoteServiceId))},
    CHECK(""{nameof(CreatedOn)}""<=""{nameof(ModifiedOn)}"")
)");
    }

    internal async Task ClearVersionsAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        Guid id = _localId;
        Guid remoteServiceId = _remoteServiceId;
        foreach (RemoteVersion toRemove in await dbContext.RemoteVersions.Where(l => l.LibraryId == id && l.RemoteServiceId == remoteServiceId).ToArrayAsync(cancellationToken))
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
        dbContext.RemoteLibraries.Remove(this);
        await dbContext.SaveChangesAsync(true, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        LocalLibrary? local = await dbContext.RemoteLibraries.Entry(this).EnsureRelatedAsync(l => l.Local, cancellationToken);
        if (local is not null)
        {
            Guid id = local.Id;
            if (!(await dbContext.RemoteLibraries.AnyAsync(r => r.LocalId == id)))
                await local.RemoveAsync(dbContext, cancellationToken);
        }
    }
}