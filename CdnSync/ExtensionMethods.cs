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

        public static string ToTrimmedOrEmptyIfNull(this string? value) { return (value is null) ? "" : value.Trim(); }

        public static string? ToTrimmedOrNullIfEmpty(this string? value) { return (value is not null && (value = value.Trim()).Length > 0) ? value : null; }

        public static string ToTrimmedOrDefaultIfEmpty(this string? value, Func<string> getDefault) { return (value is not null && (value = value.Trim()).Length > 0) ? value : getDefault(); }

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