using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace CdnSync.CdnJs
{
    public abstract class CdnJsObjectResponse : CdnJsResponse<JsonObject>
    {
        protected CdnJsObjectResponse(JsonObject rawJson) : base(rawJson) { }

        public bool TryGetPropertyValue<T>(string propertyName, [NotNullWhen(true)] out T? result)
        {
            if (RawJson[propertyName] is JsonValue node)
                return node.TryGetValue(out result);
            result = default;
            return false;
        }

        protected void SetPropertyValue<T>(string propertyName, T value)
            where T : IEquatable<T>
        {
            if (RawJson.ContainsKey(propertyName))
            {
                if (RawJson[propertyName] is JsonValue node && node.TryGetValue(out T? result) && result.Equals(value))
                    return;
                RawJson[propertyName] = JsonValue.Create(value);
            }
            else
                RawJson.Add(propertyName, JsonValue.Create(value));
        }
        
        protected void SetPropertyValueOrRemoveIfNull<T>(string propertyName, T? value)
            where T : class, IEquatable<T>
        {
            if (value is null)
            {
                if (RawJson.ContainsKey(propertyName))
                    RawJson.Remove(propertyName);
            }
            else if (RawJson.ContainsKey(propertyName))
            {
                if (RawJson[propertyName] is JsonValue node && node.TryGetValue(out T? result) && result.Equals(value))
                    return;
                RawJson[propertyName] = JsonValue.Create(value);
            }
            else
                RawJson.Add(propertyName, JsonValue.Create(value));
        }
        
        protected T EnsurePropertyValue<T>(string propertyName, Func<T> getDefaultValue)
        {
            T? result;
            if (RawJson.ContainsKey(propertyName))
            {
                if (RawJson[propertyName] is JsonValue node)
                {
                    if (node.TryGetValue(out result))
                        return result;
                }
                result = getDefaultValue();
                RawJson[propertyName] = JsonValue.Create(result);
            }
            else
            {
                result = getDefaultValue();
                RawJson.Add(propertyName, JsonValue.Create(result));
            }
            return result;
        }

    }
}