using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CdnGetter.SqlExtensions;

namespace CdnGetter.Model;

/// <summary>
/// Log entry for a request relating to an <see cref="UpstreamCdn" /> that does not correspond to an existing <see cref="CdnLibrary" />.
/// </summary>
public class CdnLog : CdnLogBase
{
    private readonly object _syncRoot = new();

    private Guid _upstreamCdnId;
    /// <summary>
    /// The unique identifier of the parent <see cref="UpstreamCdn" />.
    /// </summary>
    public override Guid UpstreamCdnId
    {
        get => _upstreamCdnId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _upstreamCdnId, ref _cdn);
    }

    private UpstreamCdn? _cdn;
    /// <summary>
    /// The parent upstream content delivery network.
    /// </summary>
    public UpstreamCdn? Cdn
    {
        get => _cdn;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _upstreamCdnId, ref _cdn);
    }

    /// <summary>
    /// Performs configuration of the <see cref="LibraryLog" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<CdnLog> builder)
    {
        CdnLogBase.OnBuildCdnLogModel(builder);
        _ = builder.HasOne(f => f.Cdn).WithMany(f => f.Logs).HasForeignKey(nameof(UpstreamCdnId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }

    internal static void CreateTable(Action<string> executeNonQuery)
    {
        executeNonQuery(@$"CREATE TABLE ""{nameof(Services.ContentDb.LibraryLogs)}"" (
    ""{nameof(Id)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(Message)}"" TEXT NOT NULL DEFAULT '' CHECK(length(trim(""{nameof(Message)}""))=length(""{nameof(Message)}"")),
    ""{nameof(Action)}"" UNSIGNED TINYINT NOT NULL DEFAULT 0,
    ""{nameof(Level)}"" UNSIGNED TINYINT NOT NULL DEFAULT 0,
    ""{nameof(EventId)}"" INTEGER DEFAULT NULL,
    ""{nameof(Url)}"" NVARCHAR({MAXLENGTH_Url}) DEFAULT NULL,
    ""{nameof(ProviderData)}"" TEXT DEFAULT NULL,
    ""{nameof(Timestamp)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(UpstreamCdnId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    FOREIGN KEY(""{nameof(UpstreamCdnId)}"") REFERENCES ""{nameof(Services.ContentDb.UpstreamCdns)}""(""{nameof(LocalLibrary.Id)}"") ON DELETE RESTRICT,
    PRIMARY KEY(""{nameof(Id)}"")
)");
        executeNonQuery($"CREATE INDEX \"IDX_LibraryLogs_Timestamp\" ON \"{nameof(Services.ContentDb.LibraryLogs)}\" (\"{nameof(Timestamp)}\" DESC)");
    }
}