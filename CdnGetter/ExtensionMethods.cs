using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace CdnGetter;

public static class ExtensionMethods
{
    public static readonly Regex NonNormalizedWhiteSpaceRegex = new(@"( |(?! ))[\r\n\s]+", RegexOptions.Compiled);

    public static readonly Regex LineBreakRegex = new(@"\r?\n|\n", RegexOptions.Compiled);

    public static bool ValidateSourceIsEmpty(this Parsing.ParsingSource source, ref int startIndex, ref int count)
    {
        if (source.Count == 0)
            startIndex = 0;
        else if (startIndex >= source.Count)
            startIndex = source.Count;
        else
        {
            if (startIndex < 0)
            {
                count += startIndex;
                startIndex = 0;
            }
            if (count > 0)
            {
                if (count + startIndex > source.Count)
                    count = source.Count - startIndex;
                return false;
            }
        }
        count = 0;
        return true;
    }

    public static bool ValidateExtentsIsEmpty<T>(this ReadOnlySpan<T> span, ref int startIndex, ref int endIndex)
    {
        if (span.IsEmpty)
            startIndex = endIndex = 0;
        else if (startIndex >= span.Length)
            startIndex = endIndex = span.Length;
        else if (endIndex <= startIndex)
        {
            if (startIndex < 0)
                startIndex = 0;
            endIndex = startIndex;
        }
        else
        {
            if (startIndex < 0)
                startIndex = 0;
            if (endIndex > span.Length)
                endIndex = span.Length;
            return false;
        }
        return true;
    }

    public static bool ValidateSourceNotEmpty(this Parsing.ParsingSource source, ref int startIndex, ref int count)
    {
        if (source.Count == 0)
            startIndex = 0;
        else if (startIndex >= source.Count)
            startIndex = source.Count;
        else
        {
            if (startIndex < 0)
            {
                count += startIndex;
                startIndex = 0;
            }
            if (count > 0)
            {
                if (count + startIndex > source.Count)
                    count = source.Count - startIndex;
                return true;
            }
        }
        count = 0;
        return false;
    }

    public static bool ValidateExtentsNotEmpty<T>(this ReadOnlySpan<T> span, ref int startIndex, ref int endIndex)
    {
        if (span.IsEmpty)
            startIndex = endIndex = 0;
        else if (startIndex >= span.Length)
            startIndex = endIndex = span.Length;
        else if (endIndex <= startIndex)
        {
            if (startIndex < 0)
                startIndex = 0;
            endIndex = startIndex;
        }
        else
        {
            if (startIndex < 0)
                startIndex = 0;
            if (endIndex > span.Length)
                endIndex = span.Length;
            return true;
        }
        return false;
    }

    public static DateTime AsLocalDateTime(this DateTime value) => value.Kind switch
    {
        DateTimeKind.Unspecified => value,
        DateTimeKind.Utc => value,
        _ => value
    };

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

    public static bool TryGetFirst<T>([NotNullWhen(true)] this IEnumerable<T>? source, out T first)
    {
        if (source is not null && source.Any())
            try
            {
                first = source.First();
                return true;
            }
            catch { /* Okay to ignore */ }
        first = default!;
        return false;
    }

    public static IEnumerable<T> Enumerate<T>(params T[] elements) => (elements is null) ? Enumerable.Empty<T>() : elements;

    public static IEnumerable<T> PrependValue<T>(this IEnumerable<T>? source, T element) where T : struct => (source is null) ? Enumerate(element) : source.Prepend(element);

    public static IEnumerable<T> AppendValue<T>(this IEnumerable<T>? source, T element) where T : struct => (source is null) ? Enumerate(element) : source.Append(element);

    public static IEnumerable<T> PrependIfNotNull<T>(this IEnumerable<T>? source, T? element)
        where T : class
    {
        if (source is null)
            return (element is null) ? Enumerable.Empty<T>() : Enumerate(element);
        return (element is null) ? source : source.Prepend(element);
    }

    public static IEnumerable<T> AppendIfNotNull<T>(this IEnumerable<T>? source, T? element)
        where T : class
    {
        if (source is null)
            return (element is null) ? Enumerable.Empty<T>() : Enumerate(element);
        return (element is null) ? source : source.Append(element);
    }

    public static T[] EmptyIfNull<T>(this T[]? source) => (source is null) ? Array.Empty<T>() : source;

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) => (source is null) ? Enumerable.Empty<T>() : source;

    public static string? NullIfEmpty(this string? source) => (source is null || source.Length > 0) ? source : null;

    public static string? NullIfWhiteSpace(this string? source) => string.IsNullOrWhiteSpace(source) ? null : source;

    public static string? AsNonEmptyStringOrNull(this ReadOnlySpan<char> source) => (source.Length > 0) ? new(source) : null;

    public static string? AsNonEmptyStringOrNull(this ReadOnlySpan<char> source, int startIndex, int endIndex) =>
        (startIndex >= source.Length || endIndex <= startIndex) ? null : new string((endIndex < source.Length) ? source[startIndex..endIndex] : (startIndex > 0) ? source[startIndex..] : source);

    public static string AsString(this ReadOnlySpan<char> source, int startIndex, int endIndex)
    {
        if (startIndex >= source.Length || endIndex <= startIndex)
            return string.Empty;
        return new((endIndex < source.Length) ? source[startIndex..endIndex] : (startIndex > 0) ? source[startIndex..] : source);
    }

    public static string? AsNonWhiteSpaceStringOrNull(this ReadOnlySpan<char> source) => (source.Length > 0) ? new string(source).NullIfWhiteSpace() : null;

    public static string? AsNonWhiteSpaceStringOrNull(this ReadOnlySpan<char> source, int startIndex, int endIndex) =>
        (startIndex >= source.Length || endIndex <= startIndex) ? null : new string((endIndex < source.Length) ? source[startIndex..endIndex] : (startIndex > 0) ? source[startIndex..] : source).NullIfWhiteSpace();

    public static T[]? NullIfEmpty<T>(this T[]? source) => (source is null || source.Length == 0) ? null : source;

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

    public static IEnumerable<string> FromCsv(this string? source) => string.IsNullOrWhiteSpace(source) ? Enumerable.Empty<string>() : source.Split(',').TrimmedNotEmptyValues();

    public static string[] SplitLines(this string? value)
    {
        if (value is null)
            return Array.Empty<string>();
        return LineBreakRegex.Split(value);
    }

    public static bool IsWsNormalizedNotEmpty(this string? value, [NotNullWhen(true)] out string? wsNormalized) => (wsNormalized = value.ToWsNormalizedOrNullIfEmpty()) is not null;

    public static bool IsTrimmedNotEmpty(this string? value, [NotNullWhen(true)] out string? wsNormalized) => (wsNormalized = value.ToTrimmedOrNullIfEmpty()) is not null;

    public static string ToWsNormalizedOrEmptyIfNull(this string? value) => (value is not null && (value = value.Trim()).Length > 0) ? NonNormalizedWhiteSpaceRegex.Replace(value, " ") : "";

    public static string? ToWsNormalizedOrNullIfEmpty(this string? value) => (value is not null && (value = value.Trim()).Length > 0) ? NonNormalizedWhiteSpaceRegex.Replace(value, " ") : null;

    public static string ToWsNormalizedOrDefaultIfEmpty(this string? value, Func<string> getDefaultValue) => (value is not null && (value = value.Trim()).Length > 0) ? NonNormalizedWhiteSpaceRegex.Replace(value, " ") : getDefaultValue();

    public static string ToWsNormalizedOrDefaultIfEmpty(this string? value, string defaultValue) => (value is not null && (value = value.Trim()).Length > 0) ? NonNormalizedWhiteSpaceRegex.Replace(value, " ") : defaultValue;

    public static string ToTrimmedOrEmptyIfNull(this string? value) => (value is null) ? "" : value.Trim();

    public static string? ToTrimmedOrNullIfEmpty(this string? value) => (value is not null && (value = value.Trim()).Length > 0) ? value : null;

    public static string ToTrimmedOrDefaultIfEmpty(this string? value, Func<string> getDefault) => (value is not null && (value = value.Trim()).Length > 0) ? value : getDefault();

    public static string DefaultIfWhiteSpace(this string? value, string defaultValue) => string.IsNullOrWhiteSpace(value) ? defaultValue : value;

    public static string DefaultIfWhiteSpace(this string? value, Func<string> getDefault) => string.IsNullOrWhiteSpace(value) ? getDefault() : value;

    public static string ToTrimmedOrDefaultIfEmpty(this string? value, string defaultValue) => (value is not null && (value = value.Trim()).Length > 0) ? value : defaultValue;

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
