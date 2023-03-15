using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace CdnSync.CdnJs.Response;
public class Error
{
    public bool error = true;
    public int status = (int)HttpStatusCode.NoContent;
    public string message = "";

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData;

    public static Error Create(string? message, HttpStatusCode code = HttpStatusCode.ExpectationFailed) => new() { status = (int)code, message = message.ToTrimmedOrDefaultIfEmpty(() => ((int)code).ToStatusMessage()) };
    
    public static Error Create(HttpStatusCode code = HttpStatusCode.ExpectationFailed) => new() { status = (int)code, message = ((int)code).ToStatusMessage() };
    
    public static Error Deserialize(string? json)
    {
        if ((json = json.ToTrimmedOrNullIfEmpty()) is null)
            return Error.Create(HttpStatusCode.NoContent);
        try { return JsonSerializer.Deserialize<Error>(json) ?? Create($"Unexpect JSON type: {json}"); }
        catch { return Error.Create($"Invalid JSON string: {json}"); }
    }

    public static bool TryDeserialize(string? json, [NotNullWhen(true)] out Error? result)
    {
        if ((json = json.ToTrimmedOrNullIfEmpty()) is null)
            result = null;
        else
            try { return (result = JsonSerializer.Deserialize<Error>(json)) is not null; }
            catch { result = null; }
        return false;
    }

    public static bool TryDeserialize<T>(string? json, [NotNullWhen(true)] out T? ifNotError, [NotNullWhen(false)] out Error? error)
        where T : class, new()
    {
        if ((json = json.ToTrimmedOrNullIfEmpty()) is not null)
        {
            JsonNode? node;
            try { node = JsonNode.Parse(json); }
            catch
            {
                error = Error.Create($"Invalid JSON string: {json}");
                ifNotError = null;
                return false;
            }
            if (node is not null)
            {
                if (node is JsonObject obj)
                    try
                    {
                        if (obj.TryGetPropertyValue(nameof(error), out node) && node is JsonValue value && value.TryGetValue<bool>(out bool isError) && isError)
                        {
                            if ((error = JsonSerializer.Deserialize<Error>(obj)) is not null)
                            {
                                ifNotError = null;
                                return false;
                            }
                        }
                        else if ((ifNotError = JsonSerializer.Deserialize<T>(obj)) is not null)
                        {
                            error = null;
                            return true;
                        }
                    }
                    catch { /* Okay to ignore */ }
                error = Error.Create($"Unexpect JSON type: {json}");
                ifNotError = null;
                return false;
            }
        }
        error = Error.Create(HttpStatusCode.NoContent);
        ifNotError = null;
        return false;
    }
}
