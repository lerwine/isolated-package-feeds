using System.Text.Json;
using System.Text.Json.Serialization;

namespace CdnSync.CdnJs.Response;

public class LibraryAllVersions
{
    public string name = "";
    public string? description;
    public List<string>? versions;

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData;
}
