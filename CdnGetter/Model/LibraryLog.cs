using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CdnGetter.SqlExtensions;

namespace CdnGetter.Model;

/// <summary>
/// Log entry for a request relating to an existing <see cref="CdnLibrary" /> that does not correspond to an existing <see cref="CdnVersion" />.
/// </summary>
public class LibraryLog : CdnLogBase
{
    private readonly object _syncRoot = new();

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
    public override Guid UpstreamCdnId
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
    /// Performs configuration of the <see cref="LibraryLog" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<LibraryLog> builder)
    {
        _ = builder.Property(nameof(LibraryId)).UseCollation(COLLATION_NOCASE);
        CdnLogBase.OnBuildCdnLogModel(builder);
        _ = builder.HasOne(f => f.Library).WithMany(f => f.Logs).HasForeignKey(nameof(LibraryId), nameof(UpstreamCdnId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }

    internal static void CreateTable(Action<string> executeNonQuery)
    {
        executeNonQuery(@$"CREATE TABLE ""{nameof(Services.ContentDb.LibraryLogs)}"" (
    ""{nameof(Id)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(Message)}"" TEXT NOT NULL DEFAULT '' CHECK(length(trim(""{nameof(Message)}""))=length(""{nameof(Message)}"")),
    ""{nameof(Action)}"" UNSIGNED TINYINT NOT NULL DEFAULT 0,
    ""{nameof(Level)}"" UNSIGNED TINYINT NOT NULL DEFAULT 0,
    ""{nameof(EventId)}"" INTEGER DEFAULT NULL,
    ""{nameof(Url)}"" NVARCHAR({MAX_LENGTH_Url}) DEFAULT NULL,
    ""{nameof(ProviderData)}"" TEXT DEFAULT NULL,
    ""{nameof(Timestamp)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(LibraryId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(UpstreamCdnId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    FOREIGN KEY(""{nameof(LibraryId)}"",""{nameof(UpstreamCdnId)}"") REFERENCES ""{nameof(Services.ContentDb.CdnLibraries)}""(""{nameof(CdnLibrary.LocalId)}"",""{nameof(CdnLibrary.CdnId)}"") ON DELETE RESTRICT,
    PRIMARY KEY(""{nameof(Id)}"")
)");
        executeNonQuery($"CREATE INDEX \"IDX_LibraryLogs_Timestamp\" ON \"{nameof(Services.ContentDb.LibraryLogs)}\" (\"{nameof(Timestamp)}\" DESC)");
    }
}
