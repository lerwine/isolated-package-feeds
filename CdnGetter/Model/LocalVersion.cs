using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using static CdnGetter.SqlDefinitions;

namespace CdnGetter.Model;

public class LocalVersion
{
    private readonly object _syncRoot = new();

    private Guid? _id;
    /// <summary>
    /// The unique identifier for the library version.
    /// </summary>
    public Guid Id
    {
        get => _id.EnsureGuid(_syncRoot);
        set => _id = value;
    }
    
    public const int MAXLENGTH_Version = 1024;
    /// <summary>
    /// The library version.
    /// </summary>
    public SwVersion Version { get; set; }
    
    public const ushort DEFAULTVALUE_Order = ushort.MaxValue;
    /// <summary>
    /// The release order for the library version.
    /// </summary>
    public ushort Order { get; set; } = DEFAULTVALUE_Order;

    /// <summary>
    /// The date and time that the record was created.
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.Now;


    /// <summary>
    /// The date and time that the record was last modified.
    /// </summary>
    public DateTime ModifiedOn { get; set; } = DateTime.Now;

    private Guid _libraryId;
    /// <summary>
    /// The unique identifier of the parent <see cref="LocalLibrary" />.
    /// </summary>
    public Guid LibraryId
    {
        get => _libraryId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _libraryId, ref _library);
    }

    private LocalLibrary? _library;
    /// <summary>
    /// The parent content library.
    /// </summary>
    public LocalLibrary? Library
    {
        get => _library;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _libraryId, ref _library);
    }

    /// <summary>
    /// The files that belong to the current version of the content library.
    /// </summary>
    public Collection<LocalFile> Files { get; set; } = new();
    
    public Collection<RemoteVersion> Remotes { get; set; } = new();

    /// <summary>
    /// Performs configuration of the <see cref="LocalVersion" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<LocalVersion> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(nameof(Id)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(LibraryId)).UseCollation(COLLATION_NOCASE);
        _ = builder.HasIndex(nameof(Version));
        _ = builder.HasIndex(nameof(Version), nameof(LibraryId)).IsUnique();
        _ = builder.Property(nameof(Version)).HasConversion(SwVersion.Converter).HasMaxLength(MAXLENGTH_Version).IsRequired().UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(Order)).IsRequired();
        _ = builder.HasIndex(nameof(Order));
        _ = builder.HasIndex(nameof(Order), nameof(LibraryId)).IsUnique();
        _ = builder.HasOne(v => v.Library).WithMany(l => l.Versions).HasForeignKey(nameof(LibraryId)).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
    }

    internal static void CreateTable(Action<string> executeNonQuery, ILogger logger)
    {
        /*
        CREATE TABLE IF NOT EXISTS "LocalVersions" (
            "Id" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
            "Version" NVARCHAR(1024) NOT NULL CHECK(length(trim("Version"))=length("Version") AND length("Version")>0) COLLATE NOCASE,
            "Order" UNSIGNED SMALLINT NOT NULL DEFAULT 65535,
            "CreatedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
            "ModifiedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
            "LibraryId" UNIQUEIDENTIFIER NOT NULL CONSTRAINT "FK_LocalVersion_LocalLibrary" REFERENCES "LocalLibraries"("Id") ON DELETE RESTRICT COLLATE NOCASE,
            CONSTRAINT "PK_LocalVersions" PRIMARY KEY("Id"),
            CHECK("CreatedOn"<="ModifiedOn")
        );
        */
        executeNonQuery(@$"CREATE TABLE IF NOT EXISTS ""{nameof(Services.ContentDb.LocalVersions)}"" (
    {SqlUniqueIdentifier(nameof(Id))},
    {VarCharTrimmedNotEmptyNoCase(nameof(Version), MAXLENGTH_Version)},
    {SqlSmallUInt(nameof(Order), DEFAULTVALUE_Order)},
    {SqlDateTime(nameof(CreatedOn))},
    {SqlDateTime(nameof(ModifiedOn))},
    {SqlReferenceColumn(nameof(LocalVersion), nameof(LibraryId), nameof(LocalLibrary), nameof(LocalLibrary.Id), nameof(Services.ContentDb.LocalLibraries))},
    {SqlPkConstraint(nameof(Services.ContentDb.LocalVersions), nameof(Id))},
    CHECK(""{nameof(CreatedOn)}""<=""{nameof(ModifiedOn)}"")
)");
        // CREATE INDEX "IDX_LocalVersions_Version" ON "LocalVersions" ("Version" COLLATE NOCASE);
        executeNonQuery(SqlIndex(nameof(Services.ContentDb.LocalVersions), nameof(Version), true));
        // CREATE INDEX "IDX_LocalVersions_Order" ON "LocalVersions" ("Order");
        executeNonQuery(SqlIndex(nameof(Services.ContentDb.LocalVersions), nameof(Order)));
    }

    internal async Task ClearRemotesAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        Guid id = Id;
        foreach (RemoteVersion toRemove in await dbContext.RemoteVersions.Where(r => r.LocalId == id).ToArrayAsync(cancellationToken))
        {
            await toRemove.RemoveAsync(dbContext, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
        }
        Remotes.Clear();
    }

    internal async Task ClearFilesAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        Guid id = Id;
        foreach (LocalFile toRemove in await dbContext.LocalFiles.Where(f => f.VersionId == id).ToArrayAsync(cancellationToken))
        {
            await toRemove.RemoveAsync(dbContext, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
        }
        Files.Clear();
    }

    internal async Task RemoveAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        await ClearRemotesAsync(dbContext, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        await ClearFilesAsync(dbContext, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        dbContext.LocalVersions.Remove(this);
        await dbContext.SaveChangesAsync(true, cancellationToken);
    }

    internal async Task ReloadAsync(Services.ContentDb dbContext, CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
