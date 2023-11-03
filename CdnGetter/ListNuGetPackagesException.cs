using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

namespace CdnGetter;

[Serializable]
public class ListNuGetPackagesException : LoggedException
{
    public string SearchTerm { get; private set; } = null!;

    public string Url { get; private set; } = null!;
    
    public bool Prerelease { get; private set; }
    
    public bool AllVersions { get; private set; }
    
    public bool IncludeDelisted { get; private set; }
    
    public ListNuGetPackagesException() { }

    private ListNuGetPackagesException(string message, string searchTerm, string url, bool prerelease, bool allVersions, bool includeDelisted, Exception inner) : base(message, inner) =>
        (SearchTerm, Url, Prerelease, AllVersions, IncludeDelisted) = (searchTerm, url, prerelease, allVersions, includeDelisted);
    
    protected ListNuGetPackagesException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    internal static ListNuGetPackagesException LogAndCreate(ILogger logger, string searchTerm, string url, bool prerelease, bool allVersions, bool includeDelisted, Exception inner) => LogAndCreate(logger, () =>
        new ListNuGetPackagesException($"Failed to list NuGet packages for search term {JsonValue.Create(searchTerm)?.ToJsonString() ?? searchTerm} from {url}: Prerelease={prerelease}; AllVersions={allVersions}, IncludeDelisted={includeDelisted}", searchTerm, url, prerelease, allVersions, includeDelisted, inner));

    protected override void Log(ILogger logger) => logger.LogListNuGetPackagesFailure(SearchTerm, Url, Prerelease, AllVersions, IncludeDelisted, InnerException!);
}
