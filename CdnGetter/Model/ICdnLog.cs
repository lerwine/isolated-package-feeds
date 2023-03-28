namespace CdnGetter.Model;

/// <summary>
/// Log entry for an upstream CDN.
/// </summary>
public interface ICdnLog
{
    /// <summary>
    /// The unique identifier for the log entry.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// The log message.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// The action being performed, which precipitated the log entry.
    /// </summary>
    LibraryAction Action { get; }

    /// <summary>
    /// The severity level of the log entry.
    /// </summary>
    ErrorLevel Level { get; }
    
    /// <summary>
    /// The numerical log event ID which corresponds to an event defined in <see cref="LoggerMessages" />.
    /// </summary>
    int? EventId { get; }
    
    /// <summary>
    /// The URL of the upstream request associated with the event log entry.
    /// </summary>
    Uri? Url { get; }
    
    /// <summary>
    /// Optional provider-specific data for <see cref="UpstreamCdn" />.
    /// </summary>
    System.Text.Json.Nodes.JsonNode? ProviderData { get; }
    
    /// <summary>
    /// The date and time that the log event happened.
    /// </summary>
    DateTime Timestamp { get; }
    
    /// <summary>
    /// The unique identifier of the parent <see cref="UpstreamCdn" />.
    /// </summary>
    Guid UpstreamCdnId { get; }
}
