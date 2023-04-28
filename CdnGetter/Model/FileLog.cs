using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CdnGetter.SqlExtensions;

namespace CdnGetter.Model;

/// <summary>
/// Log entry for a request relating to an existing <see cref="CdnFile" />.
/// </summary>
public class FileLog : CdnLogBase
{
    private readonly object _syncRoot = new();

    private Guid _fileId;
    /// <summary>
    /// Gets or sets the unique identifier of the parent <see cref="CdnVersion" />.
    /// </summary>
    public Guid FileId
    {
        get => _fileId;
        set => value.SetNavigation(_versionId, _libraryId, _upstreamCdnId, _syncRoot, p => (p.LocalId, p.VersionId, p.LibraryId, p.UpstreamCdnId), ref _fileId, ref _versionId, ref _libraryId, ref _upstreamCdnId, ref _file);
    }

    private Guid _versionId;
    /// <summary>
    /// Gets or sets the unique identifier of the parent <see cref="CdnVersion" />.
    /// </summary>
    public Guid VersionId
    {
        get => _versionId;
        set => _fileId.SetNavigation(value, _libraryId, _upstreamCdnId, _syncRoot, p => (p.LocalId, p.VersionId, p.LibraryId, p.UpstreamCdnId), ref _fileId, ref _versionId, ref _libraryId, ref _upstreamCdnId, ref _file);
    }

    private Guid _libraryId;
    /// <summary>
    /// Gets or sets the unique identifier of the parent <see cref="CdnLibrary" />.
    /// </summary>
    public Guid LibraryId
    {
        get => _libraryId;
        set => _fileId.SetNavigation(_versionId, value, _upstreamCdnId, _syncRoot, p => (p.LocalId, p.VersionId, p.LibraryId, p.UpstreamCdnId), ref _fileId, ref _versionId, ref _libraryId, ref _upstreamCdnId, ref _file);
    }

    private Guid _upstreamCdnId;
    /// <summary>
    /// Gets or sets the unique identifier of the parent <see cref="UpstreamCdn" />.
    /// </summary>
    public override Guid UpstreamCdnId
    {
        get => _upstreamCdnId;
        set => _fileId.SetNavigation(_versionId, _libraryId, value, _syncRoot, p => (p.LocalId, p.VersionId, p.LibraryId, p.UpstreamCdnId), ref _fileId, ref _versionId, ref _libraryId, ref _upstreamCdnId, ref _file);
    }

    private CdnFile? _file;
    /// <summary>
    /// Gets or sets the content library version that the current file belongs to.
    /// </summary>
    public CdnFile? File
    {
        get => _file;
        set => value.SetNavigation(_syncRoot, p => (p.LocalId, p.VersionId, p.LibraryId, p.UpstreamCdnId), ref _fileId, ref _versionId, ref _libraryId, ref _upstreamCdnId, ref _file);
    }

    /// <summary>
    /// Performs configuration of the <see cref="FileLog" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<FileLog> builder)
    {
        _ = builder.Property(nameof(FileId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(VersionId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(LibraryId)).UseCollation(COLLATION_NOCASE);
        CdnLogBase.OnBuildCdnLogModel(builder);
        _ = builder.HasOne(f => f.File).WithMany(f => f.Logs).HasForeignKey(nameof(FileId), nameof(VersionId), nameof(LibraryId), nameof(UpstreamCdnId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }

    internal static void CreateTable(Action<string> executeNonQuery)
    {
        executeNonQuery(@$"CREATE TABLE ""{nameof(Services.ContentDb.FileLogs)}"" (
    ""{nameof(Id)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(Message)}"" TEXT NOT NULL DEFAULT '' CHECK(length(trim(""{nameof(Message)}""))=length(""{nameof(Message)}"")),
    ""{nameof(Action)}"" UNSIGNED TINYINT NOT NULL DEFAULT 0,
    ""{nameof(Level)}"" UNSIGNED TINYINT NOT NULL DEFAULT 0,
    ""{nameof(EventId)}"" INTEGER DEFAULT NULL,
    ""{nameof(Url)}"" NVARCHAR({MAXLENGTH_Url}) DEFAULT NULL,
    ""{nameof(ProviderData)}"" TEXT DEFAULT NULL,
    ""{nameof(Timestamp)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(FileId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(VersionId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(LibraryId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(UpstreamCdnId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    FOREIGN KEY(""{nameof(FileId)}"",""{nameof(VersionId)}"",""{nameof(LibraryId)}"",""{nameof(UpstreamCdnId)}"") REFERENCES ""{nameof(Services.ContentDb.CdnFiles)}""(""{nameof(CdnFile.LocalId)}"",""{nameof(CdnFile.VersionId)}"",""{nameof(CdnFile.LibraryId)}"",""{nameof(CdnFile.UpstreamCdnId)}"") ON DELETE RESTRICT,
    PRIMARY KEY(""{nameof(Id)}"")
)");
        executeNonQuery($"CREATE INDEX \"IDX_FileLogs_Timestamp\" ON \"{nameof(Services.ContentDb.FileLogs)}\" (\"{nameof(Timestamp)}\" DESC)");
    }
}
