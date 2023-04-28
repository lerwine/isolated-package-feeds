namespace CdnGetter.Model;

/// <summary>
/// Log entry for an upstream CDN.
/// </summary>
public interface ICdnLog
{
    /// <summary>
    /// Gets the unique identifier for the log entry.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the log message.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Gets the action being performed, which precipitated the log entry.
    /// </summary>
    LibraryAction Action { get; }

    /// <summary>
    /// Gets the severity level of the log entry.
    /// </summary>
    ErrorLevel Level { get; }
    
    /// <summary>
    /// Gets the numerical log event ID which corresponds to an event defined in <see cref="LoggerMessages" />.
    /// </summary>
    int? EventId { get; }
    
    /// <summary>
    /// Gets the URL of the upstream request associated with the event log entry.
    /// </summary>
    Uri? Url { get; }
    
    /// <summary>
    /// Optional provider-specific data for <see cref="UpstreamCdn" />.
    /// </summary>
    System.Text.Json.Nodes.JsonNode? ProviderData { get; }
    
    /// <summary>
    /// Gets the date and time that the log event happened.
    /// </summary>
    DateTime Timestamp { get; }
    
    /// <summary>
    /// Gets the unique identifier of the parent <see cref="UpstreamCdn" />.
    /// </summary>
    Guid UpstreamCdnId { get; }
}
