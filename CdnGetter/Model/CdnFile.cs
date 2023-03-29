using System.Collections.ObjectModel;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CdnGetter.SqlDefinitions;

namespace CdnGetter.Model;

/// <summary>
/// Represents an individual file in a specific version of an upstream content library.
/// </summary>
public class CdnFile
{
    private readonly object _syncRoot = new();

    private Guid _localId;
    /// <summary>
    /// The unique identifier of the parent <see cref="LocalFile" />.
    /// </summary>
    public Guid LocalId
    {
        get => _localId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _localId, ref _local);
    }

    private LocalFile? _local;
    /// <summary>
    /// The content library version that the current file belongs to.
    /// </summary>
    public LocalFile? Local
    {
        get => _local;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _localId, ref _local);
    }

    private Guid _versionId;
    /// <summary>
    /// The unique identifier of the parent <see cref="CdnVersion" />.
    /// </summary>
    public Guid VersionId
    {
        get => _versionId;
        set => value.SetNavigation(_libraryId, _upstreamCdnId, _syncRoot, p => (p.LocalId, p.LibraryId, p.UpstreamCdnId), ref _versionId, ref _libraryId, ref _upstreamCdnId, ref _version);
    }

    private Guid _libraryId;
    /// <summary>
    /// The unique identifier of the parent <see cref="CdnLibrary" />.
    /// </summary>
    public Guid LibraryId
    {
        get => _libraryId;
        set => _versionId.SetNavigation(value, _upstreamCdnId, _syncRoot, p => (p.LocalId, p.LibraryId, p.UpstreamCdnId), ref _versionId, ref _libraryId, ref _upstreamCdnId, ref _version);
    }

    private Guid _upstreamCdnId;
    /// <summary>
    /// The unique identifier of the parent <see cref="UpstreamCdn" />.
    /// </summary>
    public Guid UpstreamCdnId
    {
        get => _upstreamCdnId;
        set => _versionId.SetNavigation(_libraryId, value, _syncRoot, p => (p.LocalId, p.LibraryId, p.UpstreamCdnId), ref _versionId, ref _libraryId, ref _upstreamCdnId, ref _version);
    }

    private CdnVersion? _version;
    /// <summary>
    /// The content library version that the current file belongs to.
    /// </summary>
    public CdnVersion? Version
    {
        get => _version;
        set => value.SetNavigation(_syncRoot, p => (p.LocalId, p.LibraryId, p.UpstreamCdnId), ref _versionId, ref _libraryId, ref _upstreamCdnId, ref _version);
    }
    private string? _encoding;
    /// <summary>
    /// The content encoding override for <see cref="LocalFile.Encoding" /> or <see langword="null" /> for no override.
    /// </summary>
    public string? Encoding
    {
        get => _encoding;
        set => _encoding = value.ToTrimmedOrNullIfEmpty();
    }

    private string? _sri;
    /// <summary>
    /// The cryptographic hash override for <see cref="LocalFile.SRI" /> or <see langword="null" /> for no override.
    /// </summary>
    public string? SRI
    {
        get => _sri;
        set => _sri = value.ToTrimmedOrNullIfEmpty();
    }

    private string? _fileName = string.Empty;
    /// <summary>
    /// The local file name override for <see cref="LocalFile.FileName" /> or <see langword="null" /> for no override.
    /// </summary>
    public string? FileName
    {
        get => _fileName;
        set => _fileName = value.ToTrimmedOrNullIfEmpty();
    }


    private byte[]? _data;
    /// <summary>
    /// The library file content override or <see langword="null" /> for no override.
    /// </summary>
    [Obsolete("Use FileName, instead")]
    public byte[]? Data
    {
        get => _data;
        set => _data = (value is null || value.Length == 0) ? null : value;
    }

    /// <summary>
    /// The preferential order override for the upstream CDN or <see langword="null" /> to use <see cref="CdnVersion.Priority" />, <see cref="CdnLibrary.Priority" /> or <see cref="UpstreamCdn.Priority" />.
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
    /// CDN acess logs for this content library file.
    /// </summary>
    public Collection<FileLog> Logs { get; set; } = new();

    /// <summary>
    /// Performs configuration of the <see cref="CdnFile" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<CdnFile> builder)
    {
        _ = builder.HasKey(nameof(LocalId), nameof(VersionId), nameof(LibraryId), nameof(UpstreamCdnId));
        _ = builder.Property(nameof(LocalId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(VersionId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(LibraryId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(UpstreamCdnId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(SRI)).HasMaxLength(MAXLENGTH_SRI).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(Encoding)).HasMaxLength(MAXLENGTH_Encoding);
        _ = builder.Property(nameof(FileName)).HasMaxLength(MAX_LENGTH_FileName);
        _ = builder.Property(nameof(ProviderData)).HasConversion(ExtensionMethods.JsonValueConverter);
        _ = builder.Property(nameof(CreatedOn)).IsRequired().HasDefaultValueSql(DEFAULT_SQL_NOW);
        _ = builder.Property(nameof(ModifiedOn)).IsRequired().HasDefaultValueSql(DEFAULT_SQL_NOW);
        _ = builder.HasOne(f => f.Local).WithMany(f => f.Upstream).HasForeignKey(nameof(LocalId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(f => f.Version).WithMany(v => v.Files).HasForeignKey(nameof(VersionId), nameof(LibraryId), nameof(UpstreamCdnId)).HasPrincipalKey(nameof(CdnVersion.LocalId), nameof(CdnVersion.LibraryId), nameof(CdnVersion.UpstreamCdnId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }

    internal static void CreateTable(Action<string> executeNonQuery)
    {
        executeNonQuery(@$"CREATE TABLE ""{nameof(Services.ContentDb.CdnFiles)}"" (
    ""{nameof(LocalId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(VersionId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(LibraryId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(UpstreamCdnId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(Encoding)}"" NVARCHAR({MAXLENGTH_Encoding}) DEFAULT NULL CHECK(""{nameof(Encoding)}"" IS NULL OR (length(trim(""{nameof(Encoding)}""))=length(""{nameof(Encoding)}""))),
    ""{nameof(SRI)}"" NVARCHAR({MAXLENGTH_SRI}) DEFAULT NULL CHECK(""{nameof(SRI)}"" IS NULL OR (length(trim(""{nameof(SRI)}""))=length(""{nameof(SRI)}"") AND length(""{nameof(SRI)}"")>0)) COLLATE NOCASE,
    ""{nameof(FileName)}"" NVARCHAR({MAX_LENGTH_FileName}) DEFAULT NULL CHECK(""{nameof(FileName)}"" IS NULL OR (length(trim(""{nameof(FileName)}""))=length(""{nameof(FileName)}"") AND length(""{nameof(FileName)}"")>0)) COLLATE NOCASE,
    ""{nameof(Priority)}"" UNSIGNED SMALLINT DEFAULT NULL,
    ""{nameof(ProviderData)}"" TEXT DEFAULT NULL,
    ""{nameof(CreatedOn)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(ModifiedOn)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    FOREIGN KEY(""{nameof(LocalId)}"") REFERENCES ""{nameof(Services.ContentDb.LocalFiles)}""(""{nameof(LocalFile.Id)}"") ON DELETE RESTRICT,
    PRIMARY KEY(""{nameof(LocalId)}"",""{nameof(VersionId)}"",""{nameof(LibraryId)}"",""{nameof(UpstreamCdnId)}""),
    FOREIGN KEY(""{nameof(VersionId)}"",""{nameof(LibraryId)}"",""{nameof(UpstreamCdnId)}"") REFERENCES ""{nameof(Services.ContentDb.CdnVersions)}""(""{nameof(CdnVersion.LocalId)}"",""{nameof(CdnVersion.LibraryId)}"",""{nameof(CdnVersion.UpstreamCdnId)}""),
    CHECK(""{nameof(CreatedOn)}""<=""{nameof(ModifiedOn)}"")
)");
    }
}