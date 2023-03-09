using System.Text.Json.Nodes;

namespace CdnSync.CdnJs
{
    public abstract class CdnJsResponse
    {
        protected internal JsonNode RawJson { get; }

        protected CdnJsResponse(JsonNode rawJson) { RawJson = rawJson; }
    }

    public abstract class CdnJsResponse<T> : CdnJsResponse
        where T : JsonNode
    {
        protected internal new T RawJson => (T)base.RawJson;

        protected CdnJsResponse(T rawJson) : base(rawJson) { }
    }
}