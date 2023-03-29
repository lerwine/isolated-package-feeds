using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CdnGetter.SqlExtensions;

namespace CdnGetter.Model;

public abstract class CdnLogBase : ICdnLog, IValidatableObject
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

    /// <summary>
    /// The unique identifier of the parent <see cref="UpstreamCdn" />.
    /// </summary>
    public abstract Guid UpstreamCdnId { get; set; }
    
    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        EntityState state = (validationContext.GetService(typeof(EntityEntry)) as EntityEntry)?.State ?? EntityState.Detached;
        Validate(validationContext, state, results);
        return results;
    }

    protected virtual void Validate(ValidationContext validationContext, EntityState state, List<ValidationResult> results)
    {
        switch (state)
        {
            case EntityState.Added:
                Timestamp = DateTime.Now;
                break;
            case EntityState.Modified:
                Timestamp = Timestamp.AsLocalDateTime();
                break;
        }
        if (Url is not null && (Url.IsAbsoluteUri ? Url.AbsoluteUri : Url.OriginalString).Length > MAX_LENGTH_Url)
            results.Add(new ValidationResult($"{nameof(Url)} cannot be greater than {MAX_LENGTH_Url} characters", new[] { nameof(Version) }));
    }
    
    protected static void OnBuildCdnLogModel<TEntity>(EntityTypeBuilder<TEntity> builder)
        where TEntity : CdnLogBase
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(nameof(UpstreamCdnId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(c => c.Message).IsRequired();
        _ = builder.Property(nameof(Action)).HasConversion(ValueConverters.LibraryActionConverter);
        _ = builder.Property(nameof(Level)).HasConversion(ValueConverters.ErrorLevelConverter);
        _ = builder.Property(nameof(Url)).HasConversion(ValueConverters.UriConverter).HasMaxLength(MAX_LENGTH_Url);
        _ = builder.Property(nameof(ProviderData)).HasConversion(ValueConverters.JsonValueConverter);
        _ = builder.Property(nameof(Timestamp)).IsRequired().HasDefaultValueSql(DEFAULT_SQL_NOW);
    }
}