using System.Diagnostics.CodeAnalysis;
using static NuGetPuller.CommonStatic;

namespace NuGetPuller;

public static class StringExtensions
{
    public static string[] SplitLines(this string? value)
    {
        if (value is null)
            return [];
        return LineBreakRegex.Split(value);
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

    public static string ToTrimmedOrDefaultIfEmpty(this string? value, string defaultValue) => (value is not null && (value = value.Trim()).Length > 0) ? value : defaultValue;
}