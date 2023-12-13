using System.Diagnostics.CodeAnalysis;
using static IsolatedPackageFeeds.Shared.CommonStatic;

namespace IsolatedPackageFeeds.Shared;

public static class StringExtensions
{
    #region From CdnGetter

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

    public static IEnumerable<string> TrimmedNotEmptyValues(this IEnumerable<string?>? source)
    {
        if (source is null)
            return Enumerable.Empty<string>();
        return source.Select(ToTrimmedOrNullIfEmpty).Where(t => t is not null)!;
    }

    public static IEnumerable<string> FromCsv(this string? source) => string.IsNullOrWhiteSpace(source) ? Enumerable.Empty<string>() : source.Split(',').TrimmedNotEmptyValues();

    #endregion

    public static string[] SplitLines(this string? value)
    {
        if (value is null)
            return [];
        return LineBreakRegex.Split(value);
    }

    public static bool TrySplitToNonWhiteSpaceTrimmed(this string? value, char separator, [NotNullWhen(true)]out string[]? result)
    {
        if (value is null || (value = value.Trim()).Length == 0)
        {
            result = null;
            return false;
        }
        if (value.Contains(separator))
            return (result = value.Split(separator).Select(s => s.Trim()).Where(s => s.Length > 0).ToArray()).Length > 0;
        result = [value];
        return true;
    }

    public static bool IsWsNormalizedNotEmpty(this string? value, [NotNullWhen(true)] out string? wsNormalized) => (wsNormalized = value.ToWsNormalizedOrNullIfEmpty()) is not null;

    public static bool IsTrimmedNotEmpty(this string? value, [NotNullWhen(true)] out string? wsNormalized) => (wsNormalized = value.ToTrimmedOrNullIfEmpty()) is not null;

    public static string ToWsNormalizedOrEmptyIfNull(this string? value) => (value is not null && (value = value.Trim()).Length > 0) ? NonNormalizedWhiteSpaceRegex.Replace(value, " ") : "";

    public static string? ToWsNormalizedOrNullIfEmpty(this string? value) => (value is not null && (value = value.Trim()).Length > 0) ? NonNormalizedWhiteSpaceRegex.Replace(value, " ") : null;

    public static string ToWsNormalizedOrDefaultIfEmpty(this string? value, Func<string> getDefaultValue) => (value is not null && (value = value.Trim()).Length > 0) ? NonNormalizedWhiteSpaceRegex.Replace(value, " ") : getDefaultValue();

    public static string ToWsNormalizedOrDefaultIfEmpty(this string? value, string defaultValue) => (value is not null && (value = value.Trim()).Length > 0) ?
        NonNormalizedWhiteSpaceRegex.Replace(value, " ") : defaultValue;

    public static string ToTrimmedOrEmptyIfNull(this string? value) => (value is null) ? "" : value.Trim();

    public static string? ToTrimmedOrNullIfEmpty(this string? value) => (value is not null && (value = value.Trim()).Length > 0) ? value : null;

    public static string ToTrimmedOrDefaultIfEmpty(this string? value, Func<string> getDefault) => (value is not null && (value = value.Trim()).Length > 0) ? value : getDefault();

    /// <summary>
    /// Gets a fallback value if the input string is null, empty or contains only whitespace characters.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="defaultValue">The value to return if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <returns>The value of <paramref name="value"/> if it contains at least one non-whitespace character; otherwise the <paramref name="defaultValue"/>.</returns>
    public static string DefaultIfWhiteSpace(this string? value, string defaultValue) => string.IsNullOrWhiteSpace(value) ? defaultValue : value;

    /// <summary>
    /// Gets a fallback value if the input string is null, empty or contains only whitespace characters.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="getDefault">The delegate that returns the fallback value if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <returns>The value of <paramref name="value"/> if it contains at least one non-whitespace character; otherwise the value returned by the <paramref name="getDefault"/> delegate.</returns>
    public static string DefaultIfWhiteSpace(this string? value, Func<string> getDefault) => string.IsNullOrWhiteSpace(value) ? getDefault() : value;

    /// <summary>
    /// Gets a fallback value if the input string is null, empty or contains only whitespace characters.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="defaultValue">The value to return if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <param name="isDefaultValue">Returns <see langword="true"/> if <paramref name="value"/> if it contains at least one non-whitespace character; otherwise <see langword="false"/> if the <paramref name="defaultValue"/> was returned.</param>
    /// <returns>The value of <paramref name="value"/> if it contains at least one non-whitespace character; otherwise the <paramref name="defaultValue"/>.</returns>
    public static string DefaultIfWhiteSpace(this string? value, string defaultValue, out bool isDefaultValue) => (isDefaultValue = string.IsNullOrWhiteSpace(value)) ? defaultValue : value!;

    /// <summary>
    /// Gets a fallback value if the input string is null, empty or contains only whitespace characters.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="getDefault">The delegate that returns the fallback value if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <param name="isDefaultValue">Returns <see langword="true"/> if <paramref name="value"/> if it contains at least one non-whitespace character; otherwise <see langword="false"/> if the output of <paramref name="getDefault"/> was returned.</param>
    /// <returns>The value of <paramref name="value"/> if it contains at least one non-whitespace character; otherwise the value returned by the <paramref name="getDefault"/> delegate.</returns>
    public static string DefaultIfWhiteSpace(this string? value, Func<string> getDefault, out bool isDefaultValue) => (isDefaultValue = string.IsNullOrWhiteSpace(value)) ? getDefault() : value!;

    /// <summary>
    /// Gets a fallback value if the input string is null, empty or contains only whitespace characters.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="ifPrimaryValue">The value to returned by the <paramref name="result"/> output parameter if <paramref name="value"/> contains at least one non-whitespace character.</param>
    /// <param name="defaultValue">The value to return if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <param name="ifDefaultValue">The value to returned by the <paramref name="result"/> output parameter if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <param name="result">Returns the value of the <paramref name="ifPrimaryValue"/> parameter if <paramref name="value"/> contains at least one non-whitespace character; otherwise, the value of the <paramref name="ifDefaultValue"/> parameter is returned.</param>
    /// <typeparam name="T">The type of associated result value.</typeparam>
    /// <returns>The value of <paramref name="value"/> if it contains at least one non-whitespace character; otherwise the <paramref name="defaultValue"/>.</returns>
    public static string DefaultIfWhiteSpace<T>(this string? value, T ifPrimaryValue, string defaultValue, T ifDefaultValue, out T result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = ifDefaultValue;
            return defaultValue;
        }
        result = ifPrimaryValue;
        return value;
    }

    /// <summary>
    /// Gets a fallback value if the input string is null, empty or contains only whitespace characters.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="ifPrimaryValue">The value to returned by the <paramref name="result"/> output parameter if <paramref name="value"/> contains at least one non-whitespace character.</param>
    /// <param name="getDefault">The delegate that returns the fallback value if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <param name="ifDefaultValue">The value to returned by the <paramref name="result"/> output parameter if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <param name="result">Returns the value of the <paramref name="ifPrimaryValue"/> parameter if <paramref name="value"/> contains at least one non-whitespace character; otherwise, the value of the <paramref name="ifDefaultValue"/> parameter is returned.</param>
    /// <typeparam name="T">The type of associated result value.</typeparam>
    /// <returns>The value of <paramref name="value"/> if it contains at least one non-whitespace character; otherwise the value returned by the <paramref name="getDefault"/> delegate.</returns>
    public static string DefaultIfWhiteSpace<T>(this string? value, T ifPrimaryValue, Func<string> getDefault, T ifDefaultValue, out T result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = ifDefaultValue;
            return getDefault();
        }
        result = ifPrimaryValue;
        return value;
    }

    /// <summary>
    /// Gets a fallback value if the input string is null, empty or contains only whitespace characters.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="ifPrimaryValue">The value to returned by the <paramref name="result"/> output parameter if <paramref name="value"/> contains at least one non-whitespace character.</param>
    /// <param name="defaultValue">The value to return if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <param name="ifDefaultValue">The delegate that gets the value to returned by the <paramref name="result"/> output parameter if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <param name="result">Returns the value of the <paramref name="ifPrimaryValue"/> parameter if <paramref name="value"/> contains at least one non-whitespace character; otherwise, the value returned by <paramref name="ifDefaultValue"/> delegate is returned.</param>
    /// <typeparam name="T">The type of associated result value.</typeparam>
    /// <returns>The value of <paramref name="value"/> if it contains at least one non-whitespace character; otherwise the <paramref name="defaultValue"/>.</returns>
    public static string DefaultIfWhiteSpace<T>(this string? value, T ifPrimaryValue, string defaultValue, Func<T> ifDefaultValue, out T result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = ifDefaultValue();
            return defaultValue;
        }
        result = ifPrimaryValue;
        return value;
    }

    /// <summary>
    /// Gets a fallback value if the input string is null, empty or contains only whitespace characters.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="ifPrimaryValue">The value to returned by the <paramref name="result"/> output parameter if <paramref name="value"/> contains at least one non-whitespace character.</param>
    /// <param name="getDefault">The delegate that returns the fallback value if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <param name="ifDefaultValue">The delegate that gets the value to returned by the <paramref name="result"/> output parameter if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <param name="result">Returns the value of the <paramref name="ifPrimaryValue"/> parameter if <paramref name="value"/> contains at least one non-whitespace character; otherwise, the value returned by <paramref name="ifDefaultValue"/> delegate is returned.</param>
    /// <typeparam name="T">The type of associated result value.</typeparam>
    /// <returns>The value of <paramref name="value"/> if it contains at least one non-whitespace character; otherwise the value returned by the <paramref name="getDefault"/> delegate.</returns>
    public static string DefaultIfWhiteSpace<T>(this string? value, T ifPrimaryValue, Func<string> getDefault, Func<T> ifDefaultValue, out T result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = ifDefaultValue();
            return getDefault();
        }
        result = ifPrimaryValue;
        return value;
    }

    /// <summary>
    /// Gets a fallback value if the input string is null, empty or contains only whitespace characters.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="ifPrimaryValue">The delegate that gets the value to returned by the <paramref name="result"/> output parameter if <paramref name="value"/> contains at least one non-whitespace character.</param>
    /// <param name="defaultValue">The value to return if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <param name="ifDefaultValue">The value to returned by the <paramref name="result"/> output parameter if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <param name="result">Returns the value of the <paramref name="ifPrimaryValue"/> parameter if <paramref name="value"/> contains at least one non-whitespace character; otherwise, the value of the <paramref name="ifDefaultValue"/> parameter is returned.</param>
    /// <typeparam name="T">The type of associated result value.</typeparam>
    /// <returns>The value of <paramref name="value"/> if it contains at least one non-whitespace character; otherwise the <paramref name="defaultValue"/>.</returns>
    public static string DefaultIfWhiteSpace<T>(this string? value, Func<T> ifPrimaryValue, string defaultValue, T ifDefaultValue, out T result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = ifDefaultValue;
            return defaultValue;
        }
        result = ifPrimaryValue();
        return value;
    }

    /// <summary>
    /// Gets a fallback value if the input string is null, empty or contains only whitespace characters.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="ifPrimaryValue">The delegate that gets the value to returned by the <paramref name="result"/> output parameter if <paramref name="value"/> contains at least one non-whitespace character.</param>
    /// <param name="getDefault">The delegate that returns the fallback value if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <param name="ifDefaultValue">The value to returned by the <paramref name="result"/> output parameter if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <param name="result">Returns the value of the <paramref name="ifPrimaryValue"/> parameter if <paramref name="value"/> contains at least one non-whitespace character; otherwise, the value of the <paramref name="ifDefaultValue"/> parameter is returned.</param>
    /// <typeparam name="T">The type of associated result value.</typeparam>
    /// <returns>The value of <paramref name="value"/> if it contains at least one non-whitespace character; otherwise the value returned by the <paramref name="getDefault"/> delegate.</returns>
    public static string DefaultIfWhiteSpace<T>(this string? value, Func<T> ifPrimaryValue, Func<string> getDefault, T ifDefaultValue, out T result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = ifDefaultValue;
            return getDefault();
        }
        result = ifPrimaryValue();
        return value;
    }

    /// <summary>
    /// Gets a fallback value if the input string is null, empty or contains only whitespace characters.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="ifPrimaryValue">The delegate that gets the value to returned by the <paramref name="result"/> output parameter if <paramref name="value"/> contains at least one non-whitespace character.</param>
    /// <param name="defaultValue">The value to return if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <param name="ifDefaultValue">The delegate that gets the value to returned by the <paramref name="result"/> output parameter if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <param name="result">Returns the value of the <paramref name="ifPrimaryValue"/> parameter if <paramref name="value"/> contains at least one non-whitespace character; otherwise, tthe value returned by <paramref name="ifDefaultValue"/> delegate is returned.</param>
    /// <typeparam name="T">The type of associated result value.</typeparam>
    /// <returns>The value of <paramref name="value"/> if it contains at least one non-whitespace character; otherwise the <paramref name="defaultValue"/>.</returns>
    public static string DefaultIfWhiteSpace<T>(this string? value, Func<T> ifPrimaryValue, string defaultValue, Func<T> ifDefaultValue, out T result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = ifDefaultValue();
            return defaultValue;
        }
        result = ifPrimaryValue();
        return value;
    }

    /// <summary>
    /// Gets a fallback value if the input string is null, empty or contains only whitespace characters.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="ifPrimaryValue">The delegate that gets the value to returned by the <paramref name="result"/> output parameter if <paramref name="value"/> contains at least one non-whitespace character.</param>
    /// <param name="getDefault">The delegate that returns the fallback value if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <param name="ifDefaultValue">The delegate that gets the value to returned by the <paramref name="result"/> output parameter if <paramref name="value"/> is null, empty or contains only whitespace characters.</param>
    /// <param name="result">Returns the value of the <paramref name="ifPrimaryValue"/> parameter if <paramref name="value"/> contains at least one non-whitespace character; otherwise, tthe value returned by <paramref name="ifDefaultValue"/> delegate is returned.</param>
    /// <typeparam name="T">The type of associated result value.</typeparam>
    /// <returns>The value of <paramref name="value"/> if it contains at least one non-whitespace character; otherwise the value returned by the <paramref name="getDefault"/> delegate.</returns>
    public static string DefaultIfWhiteSpace<T>(this string? value, Func<T> ifPrimaryValue, Func<string> getDefault, Func<T> ifDefaultValue, out T result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = ifDefaultValue();
            return getDefault();
        }
        result = ifPrimaryValue();
        return value;
    }

    public static bool TryGetNonWhitesSpace(this string? value, out string result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = string.Empty;
            return false;
        }
        result = value;
        return true;
    }

    public static bool TryGetNonWhitesSpace(this string? value, string? fallback, out string result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            if (string.IsNullOrWhiteSpace(fallback))
            {
                result = string.Empty;
                return false;
            }
            result = fallback;
        }
        else
            result = value;
        return true;
    }

    public static string ToTrimmedOrDefaultIfEmpty(this string? value, string defaultValue) => (value is not null && (value = value.Trim()).Length > 0) ? value : defaultValue;
}