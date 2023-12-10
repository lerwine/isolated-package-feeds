using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CdnGetter.SqlExtensions;
using IsolatedPackageFeeds.Shared;

namespace CdnGetter.Model;

public class LocalVersion : ModificationTrackingModelBase
{
    private readonly object _syncRoot = new();

    /// <summary>
    /// Gets or sets the unique identifier for the library version.
    /// </summary>
    public Guid Id { get; set; }
    
    public const int MAXLENGTH_Version = 1024;
    
    private Parsing.Version.ISoftwareVersion _version = Parsing.Version.NameVersion.Empty;

    /// <summary>
    /// Gets or sets the library version.
    /// </summary>
    public Parsing.Version.ISoftwareVersion Version
    {
        get => _version;
        set => _version = value ?? Parsing.Version.NameVersion.Empty;
    }

    public const ushort DEFAULTVALUE_Order = ushort.MaxValue;
    /// <summary>
    /// Gets or sets the release order for the library version.
    /// </summary>
    public ushort Order { get; set; } = DEFAULTVALUE_Order;

    private Guid _libraryId;
    /// <summary>
    /// Gets or sets the unique identifier of the parent <see cref="LocalLibrary" />.
    /// </summary>
    public Guid LibraryId
    {
        get => _libraryId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _libraryId, ref _library);
    }

    private string _dirName = string.Empty;
    /// <summary>
    /// Gets or sets the name of the local subdirectory where the content for this version is stored.
    /// </summary>
    public string DirName
    {
        get => _dirName;
        set => _dirName = value.ToTrimmedOrEmptyIfNull();
    }

    private LocalLibrary? _library;
    /// <summary>
    /// Gets or sets the parent content library.
    /// </summary>
    public LocalLibrary? Library
    {
        get => _library;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _libraryId, ref _library);
    }

    /// <summary>
    /// Gets or sets the files that belong to the current version of the content library.
    /// </summary>
    public Collection<LocalFile> Files { get; set; } = [];
    
    public Collection<CdnVersion> Upstream { get; set; } = [];

    protected override void Validate(ValidationContext validationContext, EntityState state, List<ValidationResult> results)
    {
        if (validationContext.GetService(typeof(EntityEntry)) is EntityEntry entry)
            entry.EnsurePrimaryKey(nameof(Id));
        Validator.TryValidateProperty(DirName, new ValidationContext(this, null, null) { MemberName = nameof(DirName) }, results);
        if (Version.ToString().Length > MAXLENGTH_Version)
            results.Add(new ValidationResult($"{nameof(Version)} cannot be greater than {MAXLENGTH_Version} characters", new[] { nameof(Version) }));
        base.Validate(validationContext, state, results);
    }

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
        _ = builder.Property(nameof(Version)).HasConversion(ValueConverters.VersionConverter).HasMaxLength(MAXLENGTH_Version).IsRequired().UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(Order)).IsRequired().HasDefaultValue(DEFAULTVALUE_Order);
        _ = builder.HasIndex(nameof(Order));
        _ = builder.HasIndex(nameof(Order), nameof(LibraryId)).IsUnique();
        _ = builder.Property(nameof(DirName)).HasMaxLength(MAXLENGTH_FileName).IsRequired().UseCollation(COLLATION_NOCASE);
        ModificationTrackingModelBase.OnBuildModificationTrackingModel(builder);
        _ = builder.HasOne(v => v.Library).WithMany(l => l.Versions).HasForeignKey(nameof(LibraryId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }

    internal static void CreateTable(Action<string> executeNonQuery)
    {
        executeNonQuery(@$"CREATE TABLE ""{nameof(Services.ContentDb.LocalVersions)}"" (
    ""{nameof(Id)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(Version)}"" NVARCHAR({MAXLENGTH_Version}) NOT NULL CHECK(length(trim(""{nameof(Version)}""))=length(""{nameof(Version)}"") AND length(""{nameof(Version)}"")>0) COLLATE NOCASE,
    ""{nameof(DirName)}"" NVARCHAR({MAXLENGTH_FileName}) NOT NULL CHECK(length(trim(""{nameof(DirName)}""))=length(""{nameof(DirName)}"") AND length(""{nameof(DirName)}"")>0) UNIQUE COLLATE NOCASE,
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
}
