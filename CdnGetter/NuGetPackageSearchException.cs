using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using NuGet.Protocol.Core.Types;

namespace CdnGetter;

[Serializable]
public class NuGetPackageSearchException : LoggedException
{
    public string SearchTerm { get; private set; } = null!;

    public string Url { get; private set; } = null!;
    
    public SearchFilter Filters { get; private set; } = null!;
    
    public int Skip { get; private set; }

    public int Take { get; private set; }

    public NuGetPackageSearchException() { }

    private NuGetPackageSearchException(string message, string searchTerm, string url, SearchFilter filters, int skip, int take, Exception inner) : base(message, inner) => (SearchTerm, Url, Filters, Skip, Take) = (searchTerm, url, filters, skip, take);
    
    protected NuGetPackageSearchException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    internal static NuGetPackageSearchException LogAndCreate(ILogger logger, string searchTerm, string url, SearchFilter filters, int skip, int take, Exception inner) => LogAndCreate(logger, () =>
        new NuGetPackageSearchException($"Failed to search for NuGet MetaData for search term {JsonValue.Create(searchTerm)?.ToJsonString() ?? searchTerm} from {url}: Filters={filters}; Skip={skip}, {take}", searchTerm, url, filters, skip, take, inner));

    protected override void Log(ILogger logger) => logger.LogNuGetPackageSearchFailure(SearchTerm, Url, Filters, Skip, Take, InnerException!);
}
