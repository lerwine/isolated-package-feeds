using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CdnSync.CdnJs
{
    public class LibraryResponse : CdnJsObjectResponse
    {
        public const string PROPERTYNAME_name = "name";
        public const string PROPERTYNAME_description = "description";
        public const string PROPERTYNAME_versions = "versions";

        public string Name
        {
            get => TryGetPropertyValue(PROPERTYNAME_name, out string? name) ? name : "";
            set => SetPropertyValue(PROPERTYNAME_name, value.ToWsNormalizedOrEmptyIfNull());
        }

        public string Message
        {
            get => TryGetPropertyValue(PROPERTYNAME_description, out string? name) ? name : "";
            set => SetPropertyValueOrRemoveIfNull(PROPERTYNAME_description, value.ToTrimmedOrNullIfEmpty());
        }

        public IEnumerable<SwVersion> GetVersions()
        {
            JsonNode? node = RawJson[PROPERTYNAME_versions];
            if (node is not null && node is JsonArray arr)
                return arr.OfType<JsonValue>().Select(n => n.TryGetValue(out string? value) ? value.ToWsNormalizedOrEmptyIfNull() : "").Where(s => s.Length > 0).Select(s => new SwVersion(s));
            return Enumerable.Empty<SwVersion>();
        }

        public void SetVersions(IEnumerable<SwVersion>? versions)
        {
            if (versions is not null)
            {
                using IEnumerator<SwVersion> enumerator = versions.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    JsonArray arr = new();
                    do { arr.Add(JsonValue.Create(enumerator.Current.ToString())); }
                    while (enumerator.MoveNext());
                    if (RawJson.ContainsKey(PROPERTYNAME_versions))
                        RawJson[PROPERTYNAME_versions] = arr;
                    else
                        RawJson.Add(PROPERTYNAME_versions, arr);
                    return;
                }
            }
            RawJson.Remove(PROPERTYNAME_versions);
        }

        private LibraryResponse(JsonObject rawJson) : base(rawJson) { }

        public static TernaryOption<LibraryResponse, ErrorResponse> Parse(string? jsonString) => ErrorResponse.Create<LibraryResponse>(jsonString, node => (node is JsonObject obj) ? new(obj) : null);
    }
}