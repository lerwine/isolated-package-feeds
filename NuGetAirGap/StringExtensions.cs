using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace NuGetAirGap;

public static class StringExtensions
{
    public static readonly Regex NonNormalizedWhiteSpaceRegex = new(@"( |(?! ))[\r\n\s]+", RegexOptions.Compiled);

    public static readonly Regex LineBreakRegex = new(@"\r?\n|\n", RegexOptions.Compiled);

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


}