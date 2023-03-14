using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CdnSync.CdnJs.Response;

// public class LibraryAuthor
// {
//     public string name = "";
//     public string url = "";
// }
public class LibraryAllVersions
{
    public string name = "";
    // public List<LibraryAuthor>? authors;
    public string? description;
    // public string? homepage;
    // public List<string>? keywords;
    // public string? license;
    // public string? version;
    // public string? author;
    public List<string>? versions;

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData;
}
