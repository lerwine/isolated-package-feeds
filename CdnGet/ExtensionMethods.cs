using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CdnGet;

public static class ExtensionMethods
{
    public static readonly Regex NonNormalizedWhiteSpaceRegex = new(@"( |(?! ))[\r\n\s]+", RegexOptions.Compiled);

    public static readonly ValueConverter<JsonNode?, string?> JsonValueConverter = new(
        v => (v == null) ? null : v.ToJsonString(JsonSerializerOptions.Default),
        s => s.ConvertToJsonNode()
    );

    public static string? ConvertFromJsonNode(this JsonNode? value) => value is null ? null : value.ToJsonString(JsonSerializerOptions.Default);

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

    public static Guid EnsureGuid(this ref Guid? target, object syncRoot)
    {
        Guid? guid;
        Monitor.Enter(syncRoot);
        try
        {
            guid = target;
            if (!guid.HasValue)
                target = guid = Guid.NewGuid();
        }
        finally { Monitor.Exit(syncRoot); }
        return guid.Value;
    }

    public static DateTime EnsureCreatedOn(this ref DateTime? createdOn, ref DateTime? modifiedOn, ref DateTime? lastChecked, object syncRoot)
    {
        DateTime? value;
        Monitor.Enter(syncRoot);
        try
        {
            if (!(value = createdOn).HasValue)
            {
                if (modifiedOn.HasValue)
                {
                    if (lastChecked.HasValue)
                        createdOn = value = (lastChecked.Value < modifiedOn.Value) ? lastChecked : modifiedOn;
                    else
                        createdOn = lastChecked = value = modifiedOn;
                }
                else if (lastChecked.HasValue)
                    createdOn = modifiedOn = value = lastChecked;
                else
                    createdOn = modifiedOn = lastChecked = value = DateTime.Now;
            }
        }
        finally { Monitor.Exit(syncRoot); }
        return value.Value;
    }

    public static void SetCreatedOn(this DateTime value, ref DateTime? createdOn, ref DateTime? modifiedOn, ref DateTime? lastChecked, object syncRoot)
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (createdOn.HasValue && createdOn.Value == value)
                return;
            createdOn = value;
            if (!modifiedOn.HasValue || modifiedOn.Value < value)
                modifiedOn = value;
            if (!lastChecked.HasValue || lastChecked.Value < value)
                lastChecked = value;
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static DateTime EnsureModifiedOn(this ref DateTime? modifiedOn, ref DateTime? createdOn, ref DateTime? lastChecked, object syncRoot)
    {
        DateTime? value;
        Monitor.Enter(syncRoot);
        try
        {
            if (!(value = modifiedOn).HasValue)
            {
                if (createdOn.HasValue)
                {
                    if ((value = DateTime.Now) > createdOn)
                        value = createdOn;
                    if (!lastChecked.HasValue)
                        lastChecked = createdOn;
                }
                else if (lastChecked.HasValue)
                    createdOn = modifiedOn = value = lastChecked;
                else
                    createdOn = modifiedOn = lastChecked = value = DateTime.Now;
            }
        }
        finally { Monitor.Exit(syncRoot); }
        return value.Value;
    }

    public static void SetModifiedOn(this DateTime value, ref DateTime? createdOn, ref DateTime? modifiedOn, ref DateTime? lastChecked, object syncRoot)
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (modifiedOn.HasValue && modifiedOn.Value == value)
                return;
            modifiedOn = value;
            if (!createdOn.HasValue || createdOn.Value > value)
            {
                createdOn = value;
                if (!lastChecked.HasValue || lastChecked.Value < value)
                    lastChecked = value;
            }
            else if (!lastChecked.HasValue || lastChecked.Value > value)
                lastChecked = value;
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static DateTime EnsureLastChecked(this ref DateTime? lastChecked, ref DateTime? createdOn, ref DateTime? modifiedOn, object syncRoot)
    {
        DateTime? value;
        Monitor.Enter(syncRoot);
        try
        {
            if (!(value = lastChecked).HasValue)
            {
                if (createdOn.HasValue)
                {
                    value = createdOn;
                    if (!modifiedOn.HasValue)
                        modifiedOn = createdOn;
                }
                if (modifiedOn.HasValue)
                    lastChecked = createdOn = value = modifiedOn;
                else
                    createdOn = modifiedOn = lastChecked = value = DateTime.Now;
            }
        }
        finally { Monitor.Exit(syncRoot); }
        return value.Value;
    }

    public static void SetLastChecked(this DateTime value, ref DateTime? createdOn, ref DateTime? modifiedOn, ref DateTime? lastChecked, object syncRoot)
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (lastChecked.HasValue && lastChecked.Value == value)
                return;
            lastChecked = value;
            if (createdOn.HasValue && createdOn.Value > value)
                createdOn = value;
            if (modifiedOn.HasValue && modifiedOn.Value < value)
                modifiedOn = value;
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) { return (source is null) ? Enumerable.Empty<T>() : source; }

    public static IEnumerable<T> NonNullValues<T>(this IEnumerable<T?>? source) where T : class
    {
        if (source is null)
            return Enumerable.Empty<T>();
        return source.Where(t => t is not null)!;
    }
    
    public static IEnumerable<string> TrimmedNotEmptyValues(this IEnumerable<string?>? source)
    {
        if (source is null)
            return Enumerable.Empty<string>();
        return source.Select(ToTrimmedOrNullIfEmpty).Where(t => t is not null)!;
    }
    
    public static string ToWsNormalizedOrEmptyIfNull(this string? value)
    {
        return (value is not null && (value = value.Trim()).Length > 0) ? NonNormalizedWhiteSpaceRegex.Replace(value, " ") : "";
    }

    public static string? ToWsNormalizedOrNullIfEmpty(this string? value)
    {
        return (value is not null && (value = value.Trim()).Length > 0) ? NonNormalizedWhiteSpaceRegex.Replace(value, " ") : null;
    }

    public static string ToWsNormalizedOrDefaultIfEmpty(this string? value, Func<string> getDefaultValue)
    {
        return (value is not null && (value = value.Trim()).Length > 0) ? NonNormalizedWhiteSpaceRegex.Replace(value, " ") : getDefaultValue();
    }

    public static string ToWsNormalizedOrDefaultIfEmpty(this string? value, string defaultValue)
    {
        return (value is not null && (value = value.Trim()).Length > 0) ? NonNormalizedWhiteSpaceRegex.Replace(value, " ") : defaultValue;
    }

    public static string ToTrimmedOrEmptyIfNull(this string? value) { return (value is null) ? "" : value.Trim(); }

    public static string? ToTrimmedOrNullIfEmpty(this string? value) { return (value is not null && (value = value.Trim()).Length > 0) ? value : null; }

    public static string ToTrimmedOrDefaultIfEmpty(this string? value, Func<string> getDefault) { return (value is not null && (value = value.Trim()).Length > 0) ? value : getDefault(); }

    public static string ToTrimmedOrDefaultIfEmpty(this string? value, string defaultValue) { return (value is not null && (value = value.Trim()).Length > 0) ? value : defaultValue; }

    public static void SetNavigation<TFK, TTarget>(this TFK newValue, object syncRoot, Func<TTarget, TFK> keyAcessor, ref TFK foreignKey, ref TTarget? target)
        where TFK : struct, IEquatable<TFK>
        where TTarget : class
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (target is not null)
            {
                if (newValue.Equals(keyAcessor(target)))
                    return;
                target = null;
            }
            foreignKey = newValue;
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static void SetNavigation<TFK, TTarget>(this TTarget? newValue, object syncRoot, Func<TTarget, TFK> keyAcessor, ref TFK foreignKey, ref TTarget? target)
        where TFK : struct, IEquatable<TFK>
        where TTarget : class
    {
        if (newValue is null)
            throw new ArgumentNullException(nameof(newValue));
        Monitor.Enter(syncRoot);
        try
        {
            if (target is null || !ReferenceEquals(newValue, target))
                foreignKey = keyAcessor(target = newValue);
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static void SetNavigation<TFK, TTarget>(this TFK? newValue, object syncRoot, Func<TTarget, TFK> keyAcessor, ref TFK? foreignKey, ref TTarget? target)
        where TFK : struct, IEquatable<TFK>
        where TTarget : class
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (target is not null)
            {
                if (newValue.HasValue && newValue.Value.Equals(keyAcessor(target)))
                    return;
                target = null;
            }
            foreignKey = newValue;
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static void SetNavigation<TFK, TTarget>(this TTarget? newValue, object syncRoot, Func<TTarget, TFK> keyAcessor, ref TFK? foreignKey, ref TTarget? target)
        where TFK : struct, IEquatable<TFK>
        where TTarget : class
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (newValue is null)
                foreignKey = null;
            else
            {
                if (target is not null && ReferenceEquals(target, newValue))
                    return;
                foreignKey = keyAcessor(newValue);
            }
            target = newValue;
        }
        finally { Monitor.Exit(syncRoot); }
    }
    
    public static string ToStatusMessage(this int statusCode) => statusCode switch
    {
        100 => "Continue",
        101 => "Switching Protocols",
        102 => "Processing",
        103 => "Early Hints",

        200 => "OK",
        201 => "Created",
        202 => "Accepted",
        203 => "Non-Authoritative Information",
        204 => "No Content",
        205 => "Reset Content",
        206 => "Partial Content",
        207 => "Multi-Status",
        208 => "Already Reported",
        226 => "IM Used",

        300 => "Multiple Choices",
        301 => "Moved Permanently",
        302 => "Found",
        303 => "See Other",
        304 => "Not Modified",
        305 => "Use Proxy",
        307 => "Temporary Redirect",
        308 => "Permanent Redirect",

        400 => "Bad Request",
        401 => "Unauthorized",
        402 => "Payment Required",
        403 => "Forbidden",
        404 => "Not Found",
        405 => "Method Not Allowed",
        406 => "Not Acceptable",
        407 => "Proxy Authentication Required",
        408 => "Request Timeout",
        409 => "Conflict",
        410 => "Gone",
        411 => "Length Required",
        412 => "Precondition Failed",
        413 => "Request Entity Too Large",
        414 => "Request-Uri Too Long",
        415 => "Unsupported Media Type",
        416 => "Requested Range Not Satisfiable",
        417 => "Expectation Failed",
        421 => "Misdirected Request",
        422 => "Unprocessable Entity",
        423 => "Locked",
        424 => "Failed Dependency",
        426 => "Upgrade Required", // RFC 2817
        428 => "Precondition Required",
        429 => "Too Many Requests",
        431 => "Request Header Fields Too Large",
        451 => "Unavailable For Legal Reasons",

        500 => "Internal Server Error",
        501 => "Not Implemented",
        502 => "Bad Gateway",
        503 => "Service Unavailable",
        504 => "Gateway Timeout",
        505 => "Http Version Not Supported",
        506 => "Variant Also Negotiates",
        507 => "Insufficient Storage",
        508 => "Loop Detected",
        510 => "Not Extended",
        511 => "Network Authentication Required",
        _ => $"Status Code {statusCode}"
    };
}
