using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using static CdnGetter.SqlDefinitions;

namespace CdnGetter.Model;

public class LibraryLog : ILibraryLog
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

    private string _message = string.Empty;
    /// <summary>
    /// The verbose log message.
    /// </summary>
    public string Message
    {
        get => _message;
        set => _message = value.ToTrimmedOrEmptyIfNull();
    }

    public LibraryAction Action { get; set; }

    public int? EventId { get; set; }

    public Uri? Url { get; set; }

    /// <summary>
    /// Optional provider-specific data for <see cref="RemoteService" />.
    /// </summary>
    public JsonNode? ProviderData { get; set; }

    /// <summary>
    /// The date and time that the log event happened.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    private Guid _libraryId;
    /// <summary>
    /// The unique identifier of the parent <see cref="RemoteLibrary" />.
    /// </summary>
    public Guid LibraryId
    {
        get => _libraryId;
        set => value.SetNavigation(_remoteServiceId, _syncRoot, p => (p.LocalId, p.RemoteServiceId), ref _libraryId, ref _remoteServiceId, ref _library);
    }

    private Guid _remoteServiceId;
    /// <summary>
    /// The unique identifier of the parent <see cref="RemoteService" />.
    /// </summary>
    public Guid RemoteServiceId
    {
        get => _remoteServiceId;
        set => _libraryId.SetNavigation(value, _syncRoot, p => (p.LocalId, p.RemoteServiceId), ref _libraryId, ref _remoteServiceId, ref _library);
    }

    private RemoteLibrary? _library;
    /// <summary>
    /// The parent content library.
    /// </summary>
    public RemoteLibrary? Library
    {
        get => _library;
        set => value.SetNavigation(_syncRoot, p => (p.LocalId, p.RemoteServiceId), ref _libraryId, ref _remoteServiceId, ref _library);
    }

    /// <summary>
    /// Performs configuration of the <see cref="LibraryLog" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<LibraryLog> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(nameof(LibraryId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(RemoteServiceId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(c => c.Message).IsRequired();
        _ = builder.Property(nameof(Action)).HasConversion(ExtensionMethods.LibraryActionConverter);
        _ = builder.Property(nameof(Url)).HasConversion(ExtensionMethods.UriConverter).HasMaxLength(MAX_LENGTH_Url);
        _ = builder.Property(nameof(ProviderData)).HasConversion(ExtensionMethods.JsonValueConverter);
        _ = builder.Property(nameof(Timestamp)).IsRequired().HasDefaultValueSql(DEFAULT_SQL_NOW);
        _ = builder.HasOne(f => f.Library).WithMany(f => f.Logs).HasForeignKey(nameof(LibraryId), nameof(RemoteServiceId)).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
    }

    internal static void CreateTable(Action<string> executeNonQuery)
    {
        executeNonQuery(@$"CREATE TABLE ""{nameof(Services.ContentDb.LibraryLogs)}"" (
    ""{nameof(Id)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(Message)}"" TEXT NOT NULL DEFAULT '' CHECK(length(trim(""{nameof(Message)}""))=length(""{nameof(Message)}"")),
    ""{nameof(Action)}"" UNSIGNED TINYINT NOT NULL DEFAULT {(byte)default(LibraryAction)},
    ""{nameof(EventId)}"" INTEGER DEFAULT NULL,
    ""{nameof(Url)}"" NVARCHAR({MAX_LENGTH_Url}) DEFAULT NULL,
    ""{nameof(ProviderData)}"" TEXT DEFAULT NULL,
    ""{nameof(Timestamp)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(LibraryId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(RemoteServiceId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    FOREIGN KEY(""{nameof(LibraryId)}"",""{nameof(RemoteServiceId)}"") REFERENCES ""{nameof(Services.ContentDb.RemoteLibraries)}""(""{nameof(RemoteLibrary.LocalId)}"",""{nameof(RemoteLibrary.RemoteServiceId)}"") ON DELETE RESTRICT,
    PRIMARY KEY(""{nameof(Id)}"")
)");
        executeNonQuery($"CREATE INDEX \"IDX_LibraryLogs_Timestamp\" ON \"{nameof(Services.ContentDb.LibraryLogs)}\" (\"{nameof(Timestamp)}\" DESC)");
    }
}