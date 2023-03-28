using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CdnGetter.SqlDefinitions;

namespace CdnGetter.Model;

/// <summary>
/// Log entry for a request relating to an <see cref="UpstreamCdn" /> that does not correspond to an existing <see cref="CdnLibrary" />.
/// </summary>
public class CdnLog : ICdnLog
{
    private readonly object _syncRoot = new();

    private Guid? _id;
    /// <summary>
    /// The unique identifier for the log entry.
    /// </summary>
    public Guid Id
    {
        get => _id.EnsureGuid(_syncRoot);
        set => _id = value;
    }

    private string _message = string.Empty;
    /// <summary>
    /// The log message.
    /// </summary>
    public string Message
    {
        get => _message;
        set => _message = value.ToTrimmedOrEmptyIfNull();
    }

    /// <summary>
    /// The action being performed, which precipitated the log entry.
    /// </summary>
    public LibraryAction Action { get; set; }

    /// <summary>
    /// The severity level of the log entry.
    /// </summary>
    public ErrorLevel Level { get; set; }

    /// <summary>
    /// The numerical log event ID which corresponds to an event defined in <see cref="LoggerMessages" />.
    /// </summary>
    public int? EventId { get; set; }

    /// <summary>
    /// The URL of the upstream request associated with the event log entry.
    /// </summary>
    public Uri? Url { get; set; }

    /// <summary>
    /// Optional provider-specific data for <see cref="UpstreamCdn" />.
    /// </summary>
    public JsonNode? ProviderData { get; set; }

    /// <summary>
    /// The date and time that the log event happened.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    private Guid _upstreamCdnId;
    /// <summary>
    /// The unique identifier of the parent <see cref="UpstreamCdn" />.
    /// </summary>
    public Guid UpstreamCdnId
    {
        get => _upstreamCdnId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _upstreamCdnId, ref _cdn);
    }

    private UpstreamCdn? _cdn;
    /// <summary>
    /// The parent upstream content delivery network.
    /// </summary>
    public UpstreamCdn? CDN
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
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(nameof(UpstreamCdnId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(c => c.Message).IsRequired();
        _ = builder.Property(nameof(Action)).HasConversion(ExtensionMethods.LibraryActionConverter);
        _ = builder.Property(nameof(Level)).HasConversion(ExtensionMethods.ErrorLevelConverter);
        _ = builder.Property(nameof(Url)).HasConversion(ExtensionMethods.UriConverter).HasMaxLength(MAX_LENGTH_Url);
        _ = builder.Property(nameof(ProviderData)).HasConversion(ExtensionMethods.JsonValueConverter);
        _ = builder.Property(nameof(Timestamp)).IsRequired().HasDefaultValueSql(DEFAULT_SQL_NOW);
        _ = builder.HasOne(f => f.CDN).WithMany(f => f.Logs).HasForeignKey(nameof(UpstreamCdnId)).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
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
    ""{nameof(UpstreamCdnId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    FOREIGN KEY(""{nameof(UpstreamCdnId)}"") REFERENCES ""{nameof(Services.ContentDb.UpstreamCdns)}""(""{nameof(LocalLibrary.Id)}"") ON DELETE RESTRICT,
    PRIMARY KEY(""{nameof(Id)}"")
)");
        executeNonQuery($"CREATE INDEX \"IDX_LibraryLogs_Timestamp\" ON \"{nameof(Services.ContentDb.LibraryLogs)}\" (\"{nameof(Timestamp)}\" DESC)");
    }
}