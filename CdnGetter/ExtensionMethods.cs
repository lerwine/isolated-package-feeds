using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace CdnGetter;

public static class ExtensionMethods
{
    public static readonly Regex NonNormalizedWhiteSpaceRegex = new(@"( |(?! ))[\r\n\s]+", RegexOptions.Compiled);

    public static readonly Regex LineBreakRegex = new(@"\r?\n|\n", RegexOptions.Compiled);

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
                ub.Fragment = uriString.Substring(i + 1);
                string path = uriString.Substring(0, i);
                if ((i = path.IndexOf('?')) > 0)
                {
                    ub.Query = path.Substring(i + 1);
                    ub.Path = path.Substring(0, i);
                }
                else
                    ub.Path = path;
            }
            else if ((i = uriString.IndexOf('?')) > 0)
            {
                ub.Query = uriString.Substring(i + 1);
                ub.Path = uriString.Substring(0, i);
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
    public static readonly ValueConverter<Model.ErrorLevel, int> ErrorLevelConverter = new(
        a => (byte)a,
        b => _allErrorLevels.FirstOrDefault(a => (int)a == b)
    );

    private static readonly Model.LibraryAction[] _allLibraryActions = Enum.GetValues<Model.LibraryAction>();
    public static readonly ValueConverter<Model.LibraryAction, byte> LibraryActionConverter = new(
        a => (byte)a,
        b => _allLibraryActions.FirstOrDefault(a => (byte)a == b)
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

    [Obsolete("Doesn't need to be this complicated")]
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

    [Obsolete("Doesn't need to be this complicated")]
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

    [Obsolete("Doesn't need to be this complicated")]
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

    [Obsolete("Doesn't need to be this complicated")]
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

    [Obsolete("Doesn't need to be this complicated")]
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

    [Obsolete("Doesn't need to be this complicated")]
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

    public static T[] EmptyIfNull<T>(this T[]? source) { return (source is null) ? Array.Empty<T>() : source; }

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) { return (source is null) ? Enumerable.Empty<T>() : source; }

    public static T[]? NullIfEmpty<T>(this T[]? source) { return (source is null || source.Length == 0) ? null : source; }

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
    
    public static string[] SplitLines(this string? value)
    {
        if (value is null)
            return Array.Empty<string>();
        return LineBreakRegex.Split(value);
    }

    public static bool IsWsNormalizedNotEmpty(this string? value, [NotNullWhen(true)] out string? wsNormalized)
    {
        return (wsNormalized = value.ToWsNormalizedOrNullIfEmpty()) is not null;
    }
    
    public static bool IsTrimmedNotEmpty(this string? value, [NotNullWhen(true)] out string? wsNormalized)
    {
        return (wsNormalized = value.ToTrimmedOrNullIfEmpty()) is not null;
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

    public static void SetNavigation<T>(this Guid newValue, object syncRoot, Func<T, Guid> keyAcessor, ref Guid foreignKey, ref T? target)
        where T : class
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

    public static void SetNavigation<T>(this Guid newValue1, Guid newValue2, object syncRoot, Func<T, (Guid, Guid)> keyAcessor, ref Guid foreignKey1, ref Guid foreignKey2, ref T? target)
        where T : class
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (target is not null)
            {
                (Guid fk1, Guid fk2) = keyAcessor(target);
                if (newValue1.Equals(fk1) && newValue2.Equals(fk2))
                    return;
                target = null;
            }
            foreignKey1 = newValue1;
            foreignKey2 = newValue2;
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static void SetNavigation<T>(this Guid newValue1, Guid newValue2, Guid newValue3, object syncRoot, Func<T, (Guid, Guid, Guid)> keyAcessor, ref Guid foreignKey1, ref Guid foreignKey2, ref Guid foreignKey3, ref T? target)
        where T : class
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (target is not null)
            {
                (Guid fk1, Guid fk2, Guid fk3) = keyAcessor(target);
                if (newValue1.Equals(fk1) && newValue2.Equals(fk2) && newValue3.Equals(fk3))
                    return;
                target = null;
            }
            foreignKey1 = newValue1;
            foreignKey2 = newValue2;
            foreignKey3 = newValue3;
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static void SetNavigation<T>(this Guid newValue1, Guid newValue2, Guid newValue3, Guid newValue4, object syncRoot, Func<T, (Guid, Guid, Guid, Guid)> keyAcessor, ref Guid foreignKey1, ref Guid foreignKey2, ref Guid foreignKey3, ref Guid foreignKey4, ref T? target)
        where T : class
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (target is not null)
            {
                (Guid fk1, Guid fk2, Guid fk3, Guid fk4) = keyAcessor(target);
                if (newValue1.Equals(fk1) && newValue2.Equals(fk2) && newValue3.Equals(fk3) && newValue4.Equals(fk4))
                    return;
                target = null;
            }
            foreignKey1 = newValue1;
            foreignKey2 = newValue2;
            foreignKey3 = newValue3;
            foreignKey4 = newValue4;
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static void SetNavigation<T>(this T? newValue, object syncRoot, Func<T, Guid> keyAcessor, ref Guid foreignKey, ref T? target)
        where T : class
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (newValue is null)
                target = null;
            else if (target is null || ReferenceEquals(newValue, target))
                foreignKey = keyAcessor(target = newValue);
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static void SetNavigation<T>(this T? newValue, object syncRoot, Func<T, (Guid, Guid)> keyAcessor, ref Guid foreignKey1, ref Guid foreignKey2, ref T? target)
        where T : class
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (newValue is null)
                target = null;
            else if (target is null || !ReferenceEquals(newValue, target))
                (foreignKey1, foreignKey2) = keyAcessor(target = newValue);
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static void SetNavigation<T>(this T? newValue, object syncRoot, Func<T, (Guid, Guid, Guid)> keyAcessor, ref Guid foreignKey1, ref Guid foreignKey2, ref Guid foreignKey3, ref T? target)
        where T : class
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (newValue is null)
                target = null;
            else if (target is null || !ReferenceEquals(newValue, target))
                (foreignKey1, foreignKey2, foreignKey3) = keyAcessor(target = newValue);
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static void SetNavigation<T>(this T? newValue, object syncRoot, Func<T, (Guid, Guid, Guid, Guid)> keyAcessor, ref Guid foreignKey1, ref Guid foreignKey2, ref Guid foreignKey3, ref Guid foreignKey4, ref T? target)
        where T : class
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (newValue is null)
                target = null;
            else if (target is null || !ReferenceEquals(newValue, target))
                (foreignKey1, foreignKey2, foreignKey3, foreignKey4) = keyAcessor(target = newValue);
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static async Task<IEnumerable<TProperty>> EnsureCollectionAsync<TEntity, TProperty>(this EntityEntry<TEntity> entityEntry, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        CollectionEntry<TEntity, TProperty> collectionEntry = entityEntry.Collection(propertyExpression);
        if (!collectionEntry.IsLoaded)
            await collectionEntry.LoadAsync(cancellationToken);
        return collectionEntry.CurrentValue ?? Enumerable.Empty<TProperty>();
    }

    public static async Task<TProperty?> EnsureRelatedAsync<TEntity, TProperty>(this EntityEntry<TEntity> entityEntry, Expression<Func<TEntity, TProperty?>> propertyExpression, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        ReferenceEntry<TEntity, TProperty> referenceEntry = entityEntry.Reference(propertyExpression);
        if (!referenceEntry.IsLoaded)
            await referenceEntry.LoadAsync(cancellationToken);
        return referenceEntry.CurrentValue;
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
