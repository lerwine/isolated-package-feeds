using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace CdnGetter;

public static class ValueConverters
{
    public static readonly ValueConverter<JsonNode?, string?> JsonValueConverter = new(
        v => (v == null) ? null : v.ToJsonString(JsonSerializerOptions.Default),
        s => s.ConvertToJsonNode()
    );

    public static Uri? ForceCreateUri(string? uriString)
    {
        if (uriString is null)
            return null;
        if (uriString.Length > 0 && Uri.TryCreate(uriString, UriKind.Absolute, out Uri? uri))
            return uri;
        if (Uri.IsWellFormedUriString(uriString, UriKind.Relative))
            return new Uri(uriString, UriKind.Relative);
        try
        {
            int i = uriString.IndexOf('#');
            UriBuilder ub = new() { Host = null, Scheme = null };
            if (i > 0)
            {
                ub.Fragment = uriString[(i + 1)..];
                string path = uriString[..i];
                if ((i = path.IndexOf('?')) > 0)
                {
                    ub.Query = path[(i + 1)..];
                    ub.Path = path[..i];
                }
                else
                    ub.Path = path;
            }
            else if ((i = uriString.IndexOf('?')) > 0)
            {
                ub.Query = uriString[(i + 1)..];
                ub.Path = uriString[..i];
            }
            else
                ub.Path = uriString;
            return new Uri(ub.ToString(), UriKind.Relative);
        }
        catch { return new Uri(Uri.EscapeDataString(uriString), UriKind.Relative); }
    }
    
    public static readonly ValueConverter<Uri?, string?> UriConverter = new(
        u => (u == null) ? null : u.IsAbsoluteUri ? u.AbsoluteUri : u.OriginalString,
        s => ForceCreateUri(s)
    );

    public static Model.ErrorLevel ToErrorLevel(this LogLevel level) => level switch
    {
        LogLevel.Warning => Model.ErrorLevel.Warning,
        LogLevel.Error => Model.ErrorLevel.Error,
        LogLevel.Critical => Model.ErrorLevel.Critical,
        _ => Model.ErrorLevel.Information,
    };

    private static readonly Model.ErrorLevel[] _allErrorLevels = Enum.GetValues<Model.ErrorLevel>();
    public static readonly ValueConverter<Model.ErrorLevel, byte> ErrorLevelConverter = new(
        a => (byte)a,
        b => _allErrorLevels.FirstOrDefault(a => (byte)a == b)
    );

    private static readonly Model.LibraryAction[] _allLibraryActions = Enum.GetValues<Model.LibraryAction>();
    public static readonly ValueConverter<Model.LibraryAction, byte> LibraryActionConverter = new(
        a => (byte)a,
        b => _allLibraryActions.FirstOrDefault(a => (byte)a == b)
    );

    public static string? ConvertFromJsonNode(this JsonNode? value) => value?.ToJsonString(JsonSerializerOptions.Default);

    public static JsonNode? ConvertToJsonNode(this string? value)
    {
        string? normalized = value.ToTrimmedOrNullIfEmpty();
        if (normalized is null)
            return null;
        try { return JsonNode.Parse(normalized); }
        catch
        {
            try { return JsonValue.Create(value); }
            catch { return null; }
        }
    }
    
    public static bool TryConvertToJsonNode(this string? value, out JsonNode? result)
    {
        string? normalized = value.ToTrimmedOrNullIfEmpty();
        if (normalized is not null)
            try
            {
                result = JsonNode.Parse(normalized)!;
                return true;
            }
            catch
            {
                try
                {
                    result = JsonValue.Create(value!)!;
                    return true;
                }
                catch { /* Okay to ignore */ }
            }
        result = null;
        return false;
    }
    
    public static JsonObject? ToJsonObject(this Dictionary<string, JsonElement>? source)
    {
        if (source is null)
            return null;
        JsonObject result = new();
        foreach (KeyValuePair<string, JsonElement> kvp in source)
            switch (kvp.Value.ValueKind)
            {
                case JsonValueKind.Object:
                    result.Add(kvp.Key, JsonObject.Create(kvp.Value));
                    break;
                case JsonValueKind.Array:
                    result.Add(kvp.Key, JsonArray.Create(kvp.Value));
                    break;
                default:
                    result.Add(kvp.Key, JsonValue.Create(kvp.Value));
                    break;
            }
        return result;
    }
}
