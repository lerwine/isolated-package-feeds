using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using System;

namespace CdnGetter;

public static class ExtensionMethods
{
    public static readonly Regex NonNormalizedWhiteSpaceRegex = new(@"( |(?! ))[\r\n\s]+", RegexOptions.Compiled);

    public static readonly Regex LineBreakRegex = new(@"\r?\n|\n", RegexOptions.Compiled);
    
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

    public static IEnumerable<T> PrependValue<T>(this IEnumerable<T>? source,  T element)
        where T : struct
    {
        return (source is null) ? Enumerate(element) : source.Prepend(element);
    }

    public static IEnumerable<T> AppendValue<T>(this IEnumerable<T>? source,  T element)
        where T : struct
    {
        return (source is null) ? Enumerate(element) : source.Append(element);
    }

    public static IEnumerable<T> PrependIfNotNull<T>(this IEnumerable<T>? source,  T? element)
        where T : class
    {
        if (source is null)
            return (element is null) ? Enumerable.Empty<T>() : Enumerate(element);
        return (element is null) ? source : source.Prepend(element);
    }

    public static IEnumerable<T> AppendIfNotNull<T>(this IEnumerable<T>? source,  T? element)
        where T : class
    {
        if (source is null)
            return (element is null) ? Enumerable.Empty<T>() : Enumerate(element);
        return (element is null) ? source : source.Append(element);
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

    // public static Uri? AsNormalizedUriOrNullIfEmpty(this Uri? uri)
    // {
    //     if (uri is null)
    //         return null;
    //     if (uri.IsAbsoluteUri)
    //     {
    //         if (uri.AbsolutePath == "/")
    //         {
    //             string uriString;
    //             if (uri.Fragment.Length > 1)
    //             {
    //                 if (uri.Query.Length > 1)
    //                     uriString = uri.GetLeftPart(UriPartial.Authority) + uri.Query + uri.Fragment;
    //                 else
    //                     uriString = uri.GetLeftPart(UriPartial.Authority) + uri.Fragment;
    //             }
    //             else if (uri.Query.Length > 1)
    //                 uriString = uri.GetLeftPart(UriPartial.Authority) + uri.Query;
    //             else
    //                 uriString = uri.GetLeftPart(UriPartial.Authority);
    //             return (uri.OriginalString == uriString) ? uri : new(uriString, UriKind.Absolute);
    //         }
            
    //         if (uri.AbsolutePath.Length > 1 && uri.AbsolutePath[^1] == '/')
    //         {
    //             if (uri.Fragment.Length > 0 && uri.Fragment != "#")
    //             {
    //                 if (uri.Query.Length > 0 && uri.Query != "?")
    //                     return new(uri.GetLeftPart(UriPartial.Path)[..^1] + uri.Query + uri.Fragment);
    //                 return new(uri.GetLeftPart(UriPartial.Path)[..^1] + uri.Fragment);
    //             }
    //             if (uri.Query.Length > 0 && uri.Query != "?")
    //                 return new(uri.GetLeftPart(UriPartial.Path)[..^1] + uri.Query);
    //             return new(uri.GetLeftPart(UriPartial.Path)[..^1]);
    //         }
    //         if (uri.Fragment == "#")
    //             return new(uri.GetLeftPart((uri.Query == "?") ? UriPartial.Path : UriPartial.Query), UriKind.Absolute);
    //         if (uri.Query == "?")
    //         {
    //             if (uri.Fragment.Length > 0)
    //                 return new(uri.GetLeftPart(UriPartial.Path) + uri.Fragment, UriKind.Absolute);
    //             return new(uri.GetLeftPart(UriPartial.Path), UriKind.Absolute);
    //         }
    //         return (uri.AbsoluteUri == uri.OriginalString) ? uri : new(uri.AbsoluteUri, UriKind.Absolute);
    //     }
    //     string path = uri.OriginalString;
    //     int index = path.IndexOf('#');
        
    //     ReadOnlySpan<char> fragmentSpan;
    //     ReadOnlySpan<char> pathSpan;
    //     ReadOnlySpan<char> querySpan;
    //     if (index == 0)
    //     {
    //         fragmentSpan = path.AsSpan();
    //         pathSpan = ReadOnlySpan<char>.Empty;
    //         querySpan = ReadOnlySpan<char>.Empty;
    //     }
    //     else if (index < 0)
    //     {
    //         fragmentSpan = ReadOnlySpan<char>.Empty;
    //         if ((index = path.IndexOf('?')) == 0)
    //         {
    //             pathSpan = ReadOnlySpan<char>.Empty;
    //             querySpan = path.AsSpan();
    //         }
    //         else if (index < 0)
    //         {
    //             pathSpan = path.AsSpan();
    //             querySpan = ReadOnlySpan<char>.Empty;
    //         }

    //     }
    //     else if (index > 0)
    //     {
    //         fragmentSpan = path.AsSpan(index);
    //         pathSpan = path.AsSpan(0, index);
    //     }
        
    //     if (index == 0)
    //         return (path.Length == 1) ? new Uri(string.Empty, UriKind.Relative) : uri;
    //     if (index > 0)
    //     {
    //         string fragment = path[index..];
    //         if ((path = path[..index])[0] == '?')
    //         {
    //             if (path.Length == 1)
    //                 return new Uri((fragment.Length > 1) ? fragment : string.Empty, UriKind.Relative);
    //             if (Uri.IsWellFormedUriString(path, UriKind.Relative))
    //                 return (fragment.Length > 1) ? uri : new(path, UriKind.Relative);
    //             if (fragment.Length > 1)
    //                 return new(new Uri(new Uri("http://localhost"), uri).GetComponents(UriComponents.Query | UriComponents.Fragment, UriFormat.UriEscaped), UriKind.Relative);
    //             return new(new Uri(new Uri("http://localhost"), uri).GetComponents(UriComponents.Query, UriFormat.UriEscaped), UriKind.Relative);
    //         }
    //         // path[0] == '?'; path[0] == '/'; path[0] == '.'
    //         if (fragment.Length > 1)
    //         {
    //             if (Uri.IsWellFormedUriString(path, UriKind.Relative))
    //             {
    //                 if (path.Length == 1)
    //                     return path[0] switch
    //                     {
    //                         '.' or '?' => new Uri(fragment, UriKind.Relative),
    //                         _ => uri
    //                     };
    //                 if (path[^1] == '?')
    //                 {
    //                     if (path.Length == 2)
    //                         return path[0] switch
    //                         {
    //                             '.' or '?' => new Uri(fragment, UriKind.Relative),
    //                             _ => new(string.Concat(path.AsSpan(0, 1), fragment), UriKind.Relative)
    //                         };
    //                     if (path[0] == '.')
    //                         return new(new Uri(new Uri("http://localhost"), uri).GetComponents(UriComponents.Path | UriComponents.Fragment, UriFormat.UriEscaped)[1..], UriKind.Relative);
    //                     Uri u = new(path[..^1], UriKind.Relative); 
    //                     return (u.AbsolutePath[1] == path[0]) ? new(u.OriginalString[1..], UriKind.Relative) : u;
    //                 }
    //             }
    //                 return uri;
    //             if (uri.PathAndQuery.Length > 1 && uri.PathAndQuery[1] == path[0])
    //             if (u.Query == "?")
    //                 return new Uri(u.GetComponents(UriComponents.Path | UriComponents.Fragment, UriFormat.UriEscaped), UriKind.Relative);
    //             return new Uri(u.GetComponents(UriComponents.PathAndQuery | UriComponents.Fragment, UriFormat.UriEscaped), UriKind.Relative);
    //         }
    //         if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
    //             return (fragment.Length == 1) ? new Uri(path, UriKind.Relative) : uri;
    //         if (u.)
    //         return new(u.GetComponents(UriComponents.PathAndQuery))
    //     }

    // }
    public static readonly Regex EmptyPathSegment = new(@"//+", RegexOptions.Compiled);
    public static readonly Regex AltPathSeparator = new(@"\\[\\/]*|/[\\/]+", RegexOptions.Compiled);

    private static string CombineUriComponents(string authority, string path, string query, string fragment)
    {
        if (authority.Length > 0)
        {
            if (query.Length > 0)
                return (fragment.Length > 0) ? $"{authority}{path}?{query}#{fragment}" : $"{authority}{path}?{query}";
            return (fragment.Length > 0) ? $"{authority}{path}#{fragment}" : authority + path;
        }
        if (query.Length > 0)
            return (fragment.Length > 0) ? $"{path}?{query}#{fragment}" : $"{path}?{query}";
        return (fragment.Length > 0) ? $"{path}#{fragment}" : path;
    }


    public static (string Authority, string Path, string QueryFragmentComponent, string FragmentComponent, bool WasNormalized) SplitComponents(this Uri? uri)
    {
        if (uri is null)
            return (string.Empty, string.Empty, string.Empty, string.Empty, true);
        
        string path, query, fragment;
        if (uri.IsAbsoluteUri)
        {
            if (Uri.IsWellFormedUriString(uri.OriginalString, UriKind.Absolute))
            {
                if (uri.Fragment == "#")
                    return (uri.GetLeftPart(UriPartial.Authority), ((path = EmptyPathSegment.Replace(uri.AbsolutePath, "/")).Length > 2 && path[^1] == '/') ? path[..^1] : path, uri.GetComponents(UriComponents.Query, UriFormat.UriEscaped), string.Empty, true);
                if (uri.Query == "?")
                    return (uri.GetLeftPart(UriPartial.Authority), ((path = EmptyPathSegment.Replace(uri.AbsolutePath, "/")).Length > 2 && path[^1] == '/') ? path[..^1] : path, string.Empty, uri.GetComponents(UriComponents.Fragment, UriFormat.UriEscaped), true);
                if (EmptyPathSegment.IsMatch(uri.AbsolutePath))
                    return (uri.GetLeftPart(UriPartial.Authority), ((path = EmptyPathSegment.Replace(uri.AbsolutePath, "/")).Length > 2 && path[^1] == '/') ? path[..^1] : path, uri.GetComponents(UriComponents.Query, UriFormat.UriEscaped), uri.GetComponents(UriComponents.Fragment, UriFormat.UriEscaped), true);
                if ((path = uri.AbsolutePath).Length > 2 && path[^1] == '/')
                    return (uri.GetLeftPart(UriPartial.Authority), path[..^1], uri.GetComponents(UriComponents.Query, UriFormat.UriEscaped), uri.GetComponents(UriComponents.Fragment, UriFormat.UriEscaped), true);
                return (uri.GetLeftPart(UriPartial.Authority), path, uri.GetComponents(UriComponents.Query, UriFormat.UriEscaped), uri.GetComponents(UriComponents.Fragment, UriFormat.UriEscaped), uri.OriginalString != uri.AbsoluteUri);
            }
            return (uri.GetLeftPart(UriPartial.Authority), ((path = EmptyPathSegment.Replace(uri.AbsolutePath, "/")).Length > 2 && path[^1] == '/') ? path[..^1] : path, uri.GetComponents(UriComponents.Query, UriFormat.UriEscaped), uri.GetComponents(UriComponents.Fragment, UriFormat.UriEscaped), true);
        }
        if ((path = uri.OriginalString).Length == 0)
            return (string.Empty, string.Empty, string.Empty, string.Empty, false);
        int index;
        if (Uri.IsWellFormedUriString(path, UriKind.Relative))
        {
            if ((index = path.IndexOf('#')) < 0)
            {
                if ((index = path.IndexOf('?')) < 0)
                {
                    if (EmptyPathSegment.IsMatch(path))
                        return (string.Empty, ((path = EmptyPathSegment.Replace(path, "/")).Length > ((path[0] == '/') ? 2 : 1) && path[^1] == '/') ? path[..^1] : path, string.Empty, string.Empty, true);
                    if (path.Length > ((path[0] == '/') ? 2 : 1) && path[^1] == '/')
                        return (string.Empty, path[..^1], string.Empty, string.Empty, true);
                    return (string.Empty, path, string.Empty, string.Empty, false);
                }
                if (index == 0)
                {
                    if (path.Length == 1)
                        return (string.Empty, string.Empty, string.Empty, string.Empty, true);
                    return (string.Empty, string.Empty, path[1..], string.Empty, false);
                }
                if (index == path.Length - 1)
                    return (string.Empty, ((path = EmptyPathSegment.Replace(path[..index], "/")).Length > ((path[0] == '/') ? 2 : 1) && path[^1] == '/') ? path[..^1] : path, string.Empty, string.Empty, true);
                query = path[(index + 1)..];
                path = path[..index];
                if (EmptyPathSegment.IsMatch(path))
                    return (string.Empty, ((path = EmptyPathSegment.Replace(path[..index], "/")).Length > ((path[0] == '/') ? 2 : 1) && path[^1] == '/') ? path[..^1] : path, query, string.Empty, true);
                if (path.Length > ((path[0] == '/') ? 2 : 1) && path[^1] == '/')
                    return (string.Empty, path[..^1], query, string.Empty, true);
                return (string.Empty, path, query, string.Empty, false);
            }
            if (index == 0)
            {
                if (path.Length == 1)
                    return (string.Empty, string.Empty, string.Empty, string.Empty, true);
                return (string.Empty, string.Empty, string.Empty, path[1..], false);
            }
            if (index == path.Length - 1)
            {
                if ((path = EmptyPathSegment.Replace(path[..index], "/")).Length > ((path[0] == '/') ? 2 : 1) && path[^1] == '/')
                    path = path[..^1];
                if ((index = path.IndexOf('?')) < 0)
                    return (string.Empty, path, string.Empty, string.Empty, true);
                if (index == 0)
                    return (string.Empty, string.Empty, (path.Length == 1) ? string.Empty : path[1..], string.Empty, true);
                return (string.Empty, path[..^1], (index < path.Length - 1) ? path[(index + 1)..] : string.Empty, string.Empty, true);
            }
            fragment = path[(index + 1)..];
            if ((index = (path = path[..index]).IndexOf('?')) < 0)
            {
                if (EmptyPathSegment.IsMatch(path))
                    return (string.Empty, ((path = EmptyPathSegment.Replace(path[..index], "/")).Length > ((path[0] == '/') ? 2 : 1) && path[^1] == '/') ? path[..^1] : path, string.Empty, fragment, true);
                if (path.Length > ((path[0] == '/') ? 2 : 1) && path[^1] == '/')
                    return (string.Empty, path[..^1], string.Empty, fragment, true);
                return (string.Empty, path, string.Empty, fragment, false);
            }
            if (index == 0)
            {
                if (path.Length == 1)
                    return (string.Empty, string.Empty, string.Empty, fragment, true);
                return (string.Empty, string.Empty, path[1..], fragment, false);
            }
            if (index == path.Length - 1)
                return (string.Empty, ((path = EmptyPathSegment.Replace(path[..index], "/")).Length > ((path[0] == '/') ? 2 : 1) && path[^1] == '/') ? path[..^1] : path, string.Empty, fragment, true);
            query = path[(index + 1)..];
            path = path[1..];
            if (EmptyPathSegment.IsMatch(path))
                return (string.Empty, ((path = EmptyPathSegment.Replace(path, "/")).Length > ((path[0] == '/') ? 2 : 1) && path[^1] == '/') ? path[..^1] : path, query, fragment, true);
            if (path.Length > ((path[0] == '/') ? 2 : 1) && path[^1] == '/')
                return (string.Empty, path[..^1], query, fragment, true);
            return (string.Empty, path, query, fragment, false);
        }
        Uri? u;
        UriBuilder b;
        bool isRooted;
        if ((index = path.IndexOf('#')) < 0)
        {
            if ((index = path.IndexOf('?')) < 0)
            {
                if ((path = AltPathSeparator.Replace(path, "/"))[0] == '/')
                {
                    if (Uri.TryCreate($"http://localhost{path}", UriKind.Absolute, out u))
                        return (string.Empty, ((path = u.AbsolutePath).Length > 2 && path[^1] == '/') ? path[..^1] : path, string.Empty, string.Empty, true);
                    isRooted = true;
                }
                else
                {
                    if (Uri.TryCreate($"http://localhost/{path}", UriKind.Absolute, out u))
                        return (string.Empty, ((path = u.AbsolutePath).Length > 3 && path[^1] == '/') ? path[1..^1] : path[1..], string.Empty, string.Empty, true);
                    isRooted = false;
                }
                b = new UriBuilder("http://localhost")
                {
                    Path = path
                };
            }
            else if (index == 0)
            {
                if (Uri.TryCreate($"http://localhost{path}", UriKind.Absolute, out u))
                    return (string.Empty, string.Empty, u.Query[1..], string.Empty, true);
                    b = new UriBuilder("http://localhost")
                    {
                        Query = path
                    };
                    isRooted = false;
            }
            else if (index == path.Length - 1)
            {
                path = AltPathSeparator.Replace(path[..^1], "/");
                if (path[0] == '/')
                {
                    if (Uri.TryCreate($"http://localhost{path}", UriKind.Absolute, out u))
                        return (string.Empty, ((path = u.AbsolutePath).Length > 2 && path[^1] == '/') ? path[..^1] : path, string.Empty, string.Empty, true);
                    isRooted = true;
                }
                else
                {
                    if (Uri.TryCreate($"http://localhost/{path}", UriKind.Absolute, out u))
                        return (string.Empty, ((path = u.AbsolutePath).Length > 3 && path[^1] == '/') ? path[1..^1] : path[1..], string.Empty, string.Empty, true);
                    isRooted = false;
                }
                b = new UriBuilder("http://localhost")
                {
                    Path = path
                };
            }
            else
            {
                query = path[(index + 1)..];
                path = AltPathSeparator.Replace(path[..index], "/");
                if (path[0] == '/')
                {
                    if (Uri.TryCreate($"http://localhost{path}?{query}", UriKind.Absolute, out u))
                        return (string.Empty, ((path = u.AbsolutePath).Length > 2 && path[^1] == '/') ? path[..^1] : path, u.Query[1..], string.Empty, true);
                    isRooted = true;
                }
                else
                {
                    if (Uri.TryCreate($"http://localhost/{path}?{query}", UriKind.Absolute, out u))
                        return (string.Empty, ((path = u.AbsolutePath).Length > 3 && path[^1] == '/') ? path[1..^1] : path[1..], u.Query[1..], string.Empty, true);
                    isRooted = false;
                }
                b = new UriBuilder("http://localhost")
                {
                    Path = path,
                    Query = query
                };
            }
        }
        else if (index == 0)
        {
            if (path.Length == 1)
                return (string.Empty, string.Empty, string.Empty, string.Empty, true);
            if (Uri.TryCreate($"http://localhost{path}", UriKind.Absolute, out u))
                return (string.Empty, string.Empty, string.Empty, u.Fragment[1..], true);
            b = new UriBuilder("http://localhost")
            {
                Fragment = path
            };
            isRooted = false;
        }
        // else if (index == path.Length - 1)
        // {
        //     if ((index = (path = AltPathSeparator.Replace(path[..^1], "/")).IndexOf('?')) < 0)
        //     {

        //     }
        //     else if (index == 0)
        //     {

        //     }
        //     else if (index < path.Length - 1)
        //     {

        //     }
        //     else
        //     {

        //     }
        // }
        // else
        // {
        //     fragment = path[(index + 1)..];
        //     if ((index = (path = AltPathSeparator.Replace(path[..index], "/")).IndexOf('?')) < 0)
        //     {

        //     }
        //     else if (index == 0)
        //     {

        //     }
        //     else if (index < path.Length - 1)
        //     {

        //     }
        //     else
        //     {

        //     }
        // }
        throw new NotImplementedException();
    }

    public static bool SplitComponents(this Uri? uri, out string authority, out string path, out bool isRooted, out string query, out string fragment)
    {
        if (uri is null)
        {
            authority = path = query = fragment = string.Empty;
            isRooted = false;
            return false;
        }
        throw new NotImplementedException();
    }

    public static string SplitComponents(this Uri? uri, out string path, out string query, out string fragment)
    {
        if (uri is null)
        {
            path = query = fragment = string.Empty;
            return string.Empty;
        }
        if (uri.IsAbsoluteUri)
        {
            path = uri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
            if (path.Length > 0)
            {
                if (path[^1] == '/')
                    path = (EmptyPathSegment.IsMatch(path) ? EmptyPathSegment.Replace(path, "/") : path)[0..^1];
                else if (EmptyPathSegment.IsMatch(path))
                    path = EmptyPathSegment.Replace(path, "/");
                path = $"/{path}";
            }
            else
                path = "/";
            query = uri.GetComponents(UriComponents.Query, UriFormat.UriEscaped);
            fragment = uri.GetComponents(UriComponents.Fragment, UriFormat.UriEscaped); 
            return uri.GetLeftPart(UriPartial.Authority);
        }
        string uriString = uri.OriginalString;
        switch (uriString.Length)
        {
            case 0:
                path = query = fragment = string.Empty;
                return string.Empty;
            case 1:
                switch (uriString[0])
                {
                    case '\\':
                        path = "/";
                        query = fragment = string.Empty;
                        break;
                    case '#':
                    case '?':
                        path = query = fragment = string.Empty;
                        break;
                    default:
                        path = uriString;
                        query = fragment = string.Empty;
                        break;
                }
                break;
            case 2:
                switch (uriString[0])
                {
                    case '\\':
                        path = uriString[1] switch
                        {
                            '\\' or '/' or '#' or '?' => "/",
                            _ => $"/{uriString[0..1]}",
                        };
                        query = fragment = string.Empty;
                        break;
                    case '#':
                        path = query = string.Empty;
                        fragment = uriString[1..];
                        break;
                    case '?':
                        path = fragment = string.Empty;
                        query = (uriString[1] == '#') ? string.Empty : uriString[1..];
                        break;
                    default:
                        path = uriString[1] switch
                        {
                            '\\' or '/' or '#' or '?' => uriString[0..1],
                            _ => uriString,
                        };
                        query = fragment = string.Empty;
                        break;
                }
                break;
            default:
                int index = uriString.IndexOf('#');
                if (index < 0)
                {
                    fragment = string.Empty;
                    if ((index = uriString.IndexOf('?')) < 0)
                    {
                        query = string.Empty;
                        path = uriString;
                    }
                    else
                    {
                        path = uriString[0..index];
                        query = (index < uriString.Length - 1) ? uriString[(index + 1)..] : string.Empty;
                    }
                }
                else if (index == 0)
                {
                    fragment = uriString[(index + 1)..];
                    path = query = string.Empty;
                }
                else
                {
                    fragment = (index < uriString.Length - 1) ? uriString[(index + 1)..] : string.Empty;
                    int i = uriString.IndexOf('?', 0, index);
                    if (i < 0)
                    {
                        query = string.Empty;
                        path = uriString[0..index];
                    }
                    else if (i == 0)
                    {
                        path = string.Empty;
                        query = (index == 1) ? string.Empty : uriString[1..index];
                    }
                    else
                    {
                        path = uriString[0..index];
                        query = (i == index - 1) ? string.Empty : uriString[(i + 1)..index];
                    }
                }
                break;
        }
        bool isRooted = path.Length > 0;
        if (isRooted)
        {
            if (AltPathSeparator.IsMatch(path))
                path = AltPathSeparator.Replace(path, "/");
            isRooted = path[0] == '/';
            if (isRooted)
                path = (path.Length == 1) ? string.Empty : path[1..];
            if (path.Length > 0 && path[^1] == '/')
                path = (path.Length == 1) ? string.Empty : path[0..^1];
        }
        if (fragment.Length > 0)
        {
            if (query.Length > 0)
                query = (uri = new($"http://localhost/{path}?{query}#{fragment}", UriKind.Absolute)).GetComponents(UriComponents.Query, UriFormat.UriEscaped);
            else
                uri = new($"http://localhost/{path}#{fragment}", UriKind.Absolute);
            fragment = uri.GetComponents(UriComponents.Fragment, UriFormat.UriEscaped);
        }
        else if (query.Length > 0)
            query = (uri = new($"http://localhost/{path}?{query}", UriKind.Absolute)).GetComponents(UriComponents.Query, UriFormat.UriEscaped);
        else if (path.Length > 0)
            uri = new($"http://localhost/{path}", UriKind.Absolute);
        else
            return string.Empty;
        path = isRooted ? uri.AbsolutePath : uri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
        return string.Empty;
    }

    public static Uri AsNormalizedUri(this Uri? uri)
    {
        if (uri is null)
            return new(string.Empty, UriKind.Relative);
        throw new NotImplementedException();
        // switch (c) // path chars
        // {
        //     case ' ':
        //     case '"':
        //     case '%':
        //     case ':':
        //     case '<':
        //     case '>':
        //     case '[':
        //     case '\':
        //     case ']':
        //     case '^':
        //     case '`':
        //     case '{':
        //     case '|':
        //     case '}':
        // }
        // switch (c) // query chars
        // {
        //     case ' ':
        //     case '"':
        //     case '%':
        //     case '<':
        //     case '>':
        //     case '[':
        //     case '\':
        //     case ']':
        //     case '^':
        //     case '`':
        //     case '{':
        //     case '|':
        //     case '}':
        // }
        // switch (c) // fragment chars
        // {
        //     case '"':
        //     case '#':
        //     case '%':
        //     case '<':
        //     case '>':
        //     case '[':
        //     case '\':
        //     case ']':
        //     case '^':
        //     case '`':
        //     case '{':
        //     case '|':
        //     case '}':
        // }
    }
    public static bool IsNullOrEmpty([NotNullWhen(false)] this Uri? uri)
    {
        if (uri is null)
            return true;
        if (uri.IsAbsoluteUri)
            return false;
        string s = uri.OriginalString;
        if (s.Length == 0)
            return true;
        throw new NotImplementedException();
    }
    public static bool TryGetEndpointURI(this Config.ICdnSettings? settings, Uri? relativeUri, ILogger logger, out Uri? result)
    {
        throw new NotImplementedException();
    }

    public static bool TryGetEndpointURI(this Config.ICdnSettings? settings, string? relativeUri, out Uri result)
    {
        throw new NotImplementedException();
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
