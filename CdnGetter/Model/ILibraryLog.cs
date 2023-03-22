namespace CdnGetter.Model;

public interface ILibraryLog
{
    Guid Id { get; }
    string Message { get; }
    LibraryAction Action { get; }
    int? EventId { get; }
    Uri? Url { get; }
    System.Text.Json.Nodes.JsonNode? ProviderData { get; }
    DateTime Timestamp { get; }
    Guid LibraryId { get; }
    Guid RemoteServiceId { get; }
    RemoteLibrary? Library { get; }
}
