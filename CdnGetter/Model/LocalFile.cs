using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CdnGetter.SqlDefinitions;

namespace CdnGetter.Model;

public class LocalFile
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
    
    public const int MAXLENGTH_Name = 1024;
    private string _name = string.Empty;
    /// <summary>
    /// The name of the library file.
    /// </summary>
    public string Name
    {
        get => _name;
        set => _name = value.ToWsNormalizedOrEmptyIfNull();
    }

    private string _sri = string.Empty;
    /// <summary>
    /// The Cryptographic hash.
    /// </summary>
    public string SRI
    {
        get => _sri;
        set => _sri = value.ToWsNormalizedOrEmptyIfNull();
    }

    public const ushort DEFAULTVALUE_Order = ushort.MaxValue;
    /// <summary>
    /// The display order for the library file.
    /// </summary>
    public ushort Order { get; set; } = DEFAULTVALUE_Order;

    private string _contentType = System.Net.Mime.MediaTypeNames.Application.Octet;
    /// <summary>
    /// The library file MIME content type.
    /// </summary>
    public string ContentType
    {
        get => _contentType;
        set => _contentType = value.ToTrimmedOrDefaultIfEmpty(() => System.Net.Mime.MediaTypeNames.Application.Octet);
    }

    private string _encoding = string.Empty;
    /// <summary>
    /// The content encoding for library file or <see cref="string.Empty" /> for binary files.
    /// </summary>
    public string Encoding
    {
        get => _encoding;
        set => _encoding = value.ToTrimmedOrEmptyIfNull();
    }

    private byte[] _data = Array.Empty<byte>();
    /// <summary>
    /// The contents of the library file.
    /// </summary>
    public byte[] Data
    {
        get => _data;
        set => _data = value ?? Array.Empty<byte>();
    }

    /// <summary>
    /// The date and time that the record was created.
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    /// <summary>
    /// The date and time that the record was last modified.
    /// </summary>
    public DateTime ModifiedOn { get; set; } = DateTime.Now;

    private Guid _versionId;
    /// <summary>
    /// The unique identifier of the parent <see cref="CdnVersion" />.
    /// </summary>
    public Guid VersionId
    {
        get => _versionId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _versionId, ref _version);
    }

    private LocalVersion? _version;
    /// <summary>
    /// The content library version that the current file belongs to.
    /// </summary>
    public LocalVersion? Version
    {
        get => _version;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _versionId, ref _version);
    }
    
    public Collection<CdnFile> CDNs { get; set; } = new();
    
    /// <summary>
    /// Performs configuration of the <see cref="CdnFile" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<LocalFile> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(nameof(Id)).UseCollation(COLLATION_NOCASE);
        _ = builder.HasIndex(nameof(Name));
        _ = builder.HasIndex(nameof(Name), nameof(VersionId)).IsUnique();
        _ = builder.Property(nameof(Name)).HasMaxLength(MAXLENGTH_Name).IsRequired().UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(SRI)).HasMaxLength(MAXLENGTH_SRI).IsRequired().UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(Order)).IsRequired().HasDefaultValue(DEFAULTVALUE_Order);
        _ = builder.Property(nameof(Data)).IsRequired();
        _ = builder.Property(nameof(ContentType)).HasMaxLength(MAXLENGTH_ContentType).IsRequired();
        _ = builder.Property(nameof(Encoding)).HasMaxLength(MAXLENGTH_Encoding).IsRequired();
        _ = builder.HasIndex(nameof(Order));
        _ = builder.HasIndex(nameof(Order), nameof(VersionId)).IsUnique();
        _ = builder.Property(nameof(CreatedOn)).IsRequired().HasDefaultValueSql(DEFAULT_SQL_NOW);
        _ = builder.Property(nameof(ModifiedOn)).IsRequired().HasDefaultValueSql(DEFAULT_SQL_NOW);
        _ = builder.HasOne(f => f.Version).WithMany(v => v.Files).HasForeignKey(nameof(VersionId)).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
        _ = builder.Property(nameof(VersionId)).UseCollation(COLLATION_NOCASE);
    }

    internal static void CreateTable(Action<string> executeNonQuery)
    {
        executeNonQuery(@$"CREATE TABLE ""{nameof(Services.ContentDb.LocalFiles)}"" (
    ""{nameof(Id)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(Name)}"" NVARCHAR({MAXLENGTH_Name}) NOT NULL CHECK(length(trim(""{nameof(Name)}""))=length(""{nameof(Name)}"") AND length(""{nameof(Name)}"")>0) COLLATE NOCASE,
    ""{nameof(SRI)}"" NVARCHAR({MAXLENGTH_SRI}) NOT NULL CHECK(length(trim(""{nameof(SRI)}""))=length(""{nameof(SRI)}"") AND length(""{nameof(SRI)}"")>0) COLLATE NOCASE,
    ""{nameof(Order)}"" UNSIGNED SMALLINT NOT NULL DEFAULT {DEFAULTVALUE_Order},
    ""{nameof(ContentType)}"" NVARCHAR({MAXLENGTH_ContentType}) NOT NULL CHECK(length(trim(""{nameof(ContentType)}""))=length(""{nameof(ContentType)}"") AND length(""{nameof(ContentType)}"")>0),
    ""{nameof(Encoding)}"" NVARCHAR({MAXLENGTH_Encoding}) NOT NULL CHECK(length(trim(""{nameof(Encoding)}""))=length(""{nameof(Encoding)}"")),
    ""{nameof(Data)}"" BLOB NOT NULL,
    ""{nameof(CreatedOn)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(ModifiedOn)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(VersionId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    FOREIGN KEY(""{nameof(VersionId)}"") REFERENCES ""{nameof(Services.ContentDb.LocalVersions)}""(""{nameof(LocalVersion.Id)}"") ON DELETE RESTRICT,
    PRIMARY KEY(""{nameof(Id)}""),
    CHECK(""{nameof(CreatedOn)}""<=""{nameof(ModifiedOn)}"")
)");
        executeNonQuery($"CREATE INDEX \"IDX_LocalFiles_Name\" ON \"{nameof(Services.ContentDb.LocalFiles)}\" (\"{nameof(Name)}\" COLLATE NOCASE ASC)");
        executeNonQuery($"CREATE INDEX \"IDX_LocalFiles_Order\" ON \"{nameof(Services.ContentDb.LocalFiles)}\" (\"{nameof(Order)}\" ASC)");
    }

    internal async Task RemoveAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        Guid id = Id;
        CdnFile[] toRemove = await dbContext.CdnFiles.Where(f => f.LocalId == id).ToArrayAsync(cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        if (toRemove.Length > 0)
        {
            dbContext.CdnFiles.RemoveRange(toRemove);
            await dbContext.SaveChangesAsync(true, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
            CDNs.Clear();
        }
        if (cancellationToken.IsCancellationRequested)
            return;
        dbContext.LocalFiles.Remove(this);
        await dbContext.SaveChangesAsync(true, cancellationToken);
    }
}
