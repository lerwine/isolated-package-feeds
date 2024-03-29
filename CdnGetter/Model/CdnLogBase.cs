using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CdnGetter.SqlExtensions;
using IsolatedPackageFeeds.Shared;

namespace CdnGetter.Model;

public abstract class CdnLogBase : ICdnLog, IValidatableObject
{
    private readonly object _syncRoot = new();

    /// <summary>
    /// Gets or sets the unique identifier for the library version.
    /// </summary>
    public Guid Id { get; set; }

    private string _message = string.Empty;
    /// <summary>
    /// Gets or sets the log message.
    /// </summary>
    public string Message
    {
        get => _message;
        set => _message = value.ToTrimmedOrEmptyIfNull();
    }

    /// <summary>
    /// Gets or sets the action being performed, which precipitated the log entry.
    /// </summary>
    public LibraryAction Action { get; set; }

    /// <summary>
    /// Gets or sets the severity level of the log entry.
    /// </summary>
    public ErrorLevel Level { get; set; }

    /// <summary>
    /// Gets or sets the numerical log event ID which corresponds to an event defined in <see cref="LoggerMessages" />.
    /// </summary>
    public int? EventId { get; set; }

    /// <summary>
    /// Gets or sets the URL of the upstream request associated with the event log entry.
    /// </summary>
    public Uri? Url { get; set; }

    /// <summary>
    /// Optional provider-specific data for <see cref="UpstreamCdn" />.
    /// </summary>
    public JsonNode? ProviderData { get; set; }

    /// <summary>
    /// Gets or sets the date and time that the log event happened.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the unique identifier of the parent <see cref="UpstreamCdn" />.
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
        if (validationContext.GetService(typeof(EntityEntry)) is EntityEntry entry)
            entry.EnsurePrimaryKey(nameof(Id));
        switch (state)
        {
            case EntityState.Added:
                Timestamp = DateTime.Now;
                break;
            case EntityState.Modified:
                Timestamp = Timestamp.AsLocalDateTime();
                break;
        }
        if (Url is not null && (Url.IsAbsoluteUri ? Url.AbsoluteUri : Url.OriginalString).Length > MAXLENGTH_Url)
            results.Add(new ValidationResult($"{nameof(Url)} cannot be greater than {MAXLENGTH_Url} characters", new[] { nameof(Version) }));
    }
    
    protected static void OnBuildCdnLogModel<TEntity>(EntityTypeBuilder<TEntity> builder)
        where TEntity : CdnLogBase
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(nameof(UpstreamCdnId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(c => c.Message).IsRequired();
        _ = builder.Property(nameof(Action)).HasConversion(ValueConverters.LibraryActionConverter);
        _ = builder.Property(nameof(Level)).HasConversion(ValueConverters.ErrorLevelConverter);
        _ = builder.Property(nameof(Url)).HasConversion(ValueConverters.UriConverter).HasMaxLength(MAXLENGTH_Url);
        _ = builder.Property(nameof(ProviderData)).HasConversion(ValueConverters.JsonValueConverter);
        _ = builder.Property(nameof(Timestamp)).IsRequired().HasDefaultValueSql(DEFAULT_SQL_NOW);
    }
}