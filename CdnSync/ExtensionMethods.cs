using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}