using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CdnSync
{
    public static class ExtensionMethods
    {
        public static readonly Regex NonNormalizedWhiteSpaceRegex = new Regex(@"( |(?! ))[\r\n\s]+", RegexOptions.Compiled);

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
            lock (syncRoot)
            {
                if (target is not null)
                {
                    if (newValue.Equals(keyAcessor(target)))
                        return;
                    target = null;
                }
                foreignKey = newValue;
            }
        }

        public static void SetNavigation<TFK, TTarget>(this TTarget? newValue, object syncRoot, Func<TTarget, TFK> keyAcessor, ref TFK foreignKey, ref TTarget? target)
            where TFK : struct, IEquatable<TFK>
            where TTarget : class
        {
            if (newValue is null)
                throw new ArgumentNullException(nameof(newValue));
            lock (syncRoot)
            {
                if (target is null || !ReferenceEquals(newValue, target))
                    foreignKey = keyAcessor(target = newValue);
            }
        }

        public static void SetNavigation<TFK, TTarget>(this TFK? newValue, object syncRoot, Func<TTarget, TFK> keyAcessor, ref TFK? foreignKey, ref TTarget? target)
            where TFK : struct, IEquatable<TFK>
            where TTarget : class
        {
            lock (syncRoot)
            {
                if (target is not null)
                {
                    if (newValue.HasValue && newValue.Value.Equals(keyAcessor(target)))
                        return;
                    target = null;
                }
                foreignKey = newValue;
            }
        }

        public static void SetNavigation<TFK, TTarget>(this TTarget? newValue, object syncRoot, Func<TTarget, TFK> keyAcessor, ref TFK? foreignKey, ref TTarget? target)
            where TFK : struct, IEquatable<TFK>
            where TTarget : class
        {
            lock (syncRoot)
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
}