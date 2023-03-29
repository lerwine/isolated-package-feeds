using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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

    private string _dirName = string.Empty;
    /// <summary>
    /// The name of the local subdirectory where the content for this version is stored.
    /// </summary>
    public string DirName
    {
        get => _dirName;
        set => _dirName = value.ToTrimmedOrEmptyIfNull();
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
    
    public Collection<CdnVersion> Upstream { get; set; } = new();

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
        _ = builder.Property(nameof(Order)).IsRequired().HasDefaultValue(DEFAULTVALUE_Order);
        _ = builder.HasIndex(nameof(Order));
        _ = builder.HasIndex(nameof(Order), nameof(LibraryId)).IsUnique();
        _ = builder.Property(nameof(DirName)).HasMaxLength(MAX_LENGTH_FileName).IsRequired().UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(CreatedOn)).IsRequired().HasDefaultValueSql(DEFAULT_SQL_NOW);
        _ = builder.Property(nameof(ModifiedOn)).IsRequired().HasDefaultValueSql(DEFAULT_SQL_NOW);
        _ = builder.HasOne(v => v.Library).WithMany(l => l.Versions).HasForeignKey(nameof(LibraryId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }

    internal static void CreateTable(Action<string> executeNonQuery)
    {
        executeNonQuery(@$"CREATE TABLE ""{nameof(Services.ContentDb.LocalVersions)}"" (
    ""{nameof(Id)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(Version)}"" NVARCHAR({MAXLENGTH_Version}) NOT NULL CHECK(length(trim(""{nameof(Version)}""))=length(""{nameof(Version)}"") AND length(""{nameof(Version)}"")>0) COLLATE NOCASE,
    ""{nameof(DirName)}"" NVARCHAR({MAX_LENGTH_FileName}) NOT NULL CHECK(length(trim(""{nameof(DirName)}""))=length(""{nameof(DirName)}"") AND length(""{nameof(DirName)}"")>0) UNIQUE COLLATE NOCASE,
    ""{nameof(Order)}"" UNSIGNED SMALLINT NOT NULL DEFAULT {DEFAULTVALUE_Order},
    ""{nameof(CreatedOn)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(ModifiedOn)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(LibraryId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    FOREIGN KEY(""{nameof(LibraryId)}"") REFERENCES ""{nameof(Services.ContentDb.LocalLibraries)}""(""{nameof(LocalLibrary.Id)}"") ON DELETE RESTRICT,
    PRIMARY KEY(""{nameof(Id)}""),
    CHECK(""{nameof(CreatedOn)}""<=""{nameof(ModifiedOn)}"")
)");
        executeNonQuery($"CREATE INDEX \"IDX_LocalVersions_Version\" ON \"{nameof(Services.ContentDb.LocalVersions)}\" (\"{nameof(Version)}\" COLLATE NOCASE DESC)");
        executeNonQuery($"CREATE INDEX \"IDX_LocalVersions_Order\" ON \"{nameof(Services.ContentDb.LocalVersions)}\" (\"{nameof(Order)}\" ASC)");
    }

    internal async Task ClearCDNsAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        Guid id = Id;
        foreach (CdnVersion toRemove in await dbContext.CdnVersions.Where(r => r.LocalId == id).ToArrayAsync(cancellationToken))
        {
            await toRemove.RemoveAsync(dbContext, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
        }
        Upstream.Clear();
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
        await ClearCDNsAsync(dbContext, cancellationToken);
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
