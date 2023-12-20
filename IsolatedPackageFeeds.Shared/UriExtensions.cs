using static IsolatedPackageFeeds.Shared.CommonStatic;

namespace IsolatedPackageFeeds.Shared;

public static class UriExtensions
{
    public static Uri ToNormalized(this Uri? uri)
    {
        if (uri is null) return EmptyURI;
        string path;
        if (uri.IsAbsoluteUri)
        {
            if ((path = uri.GetComponents(UriComponents.Path, UriFormat.UriEscaped)).Length == 0 || path[^1] != '/')
                return uri;
            do
            {
                if (path.Length == 1)
                    return new(uri.GetLeftPart(UriPartial.Authority) + uri.GetComponents(UriComponents.Query | UriComponents.Fragment, UriFormat.UriEscaped));
                path = path[..^1];
            } while (path[^1] == '/');
            return new($"{uri.GetLeftPart(UriPartial.Authority)}/{path}{uri.GetComponents(UriComponents.Query | UriComponents.Fragment, UriFormat.UriEscaped)}", UriKind.Absolute);
        }
        if (uri.OriginalString.Length == 0 || !Uri.TryCreate($"http://tempuri.org/{uri.OriginalString}", UriKind.Absolute, out Uri? tempUri))
            return uri;
        if ((path = uri.GetComponents(UriComponents.Path, UriFormat.UriEscaped)).Length == 1 || path[^1] != '/')
        {
            path += uri.GetComponents(UriComponents.Query | UriComponents.Fragment, UriFormat.UriEscaped);
            return (path == uri.OriginalString) ? uri : new(path, UriKind.Relative);
        }
        do
        {
            if (path.Length == 1)
                break;
            path = path[..^1];
        } while (path[^1] == '/');
        return new($"{path}{uri.GetComponents(UriComponents.Query | UriComponents.Fragment, UriFormat.UriEscaped)}", UriKind.Relative);
    }

    public static Uri SplitQueryAndFragment(this Uri? uri, out string? query, out string? fragment)
    {
        if (uri is null)
        {
            query = fragment = null;
            return EmptyURI;
        }
        if (uri.IsAbsoluteUri)
        {
            if (string.IsNullOrEmpty(uri.Fragment))
            {
                fragment = null;
                if (string.IsNullOrEmpty(uri.Query))
                {
                    query = null;
                    return uri;
                }
                query = (uri.Query[0] == '?') ? uri.Query[1..] : uri.Query;
            }
            else
            {
                fragment = (uri.Fragment[0] == '#') ? uri.Fragment[1..] : uri.Fragment;
                query = string.IsNullOrEmpty(uri.Query) ? null : (uri.Query[0] == '?') ? uri.Query[1..] : uri.Query;
            }
            return new Uri(uri.GetLeftPart(UriPartial.Path), UriKind.Absolute);
        }
        string path = uri.OriginalString;
        if (path.Length == 0)
        {
            query = fragment = null;
            return uri;
        }
        int index = path.IndexOf('#');
        if (index < 0)
        {
            fragment = null;
            if ((index = path.IndexOf('?')) < 0)
            {
                query = null;
                return uri;
            }
            if (index == 0)
            {
                query = (path.Length == 1) ? string.Empty : path[(index + 1)..];
                return EmptyURI; 
            }
            query = (index < path.Length - 1) ? path[(index + 1)..] : string.Empty;
            path = path[..index];
        }
        else
        {
            if (index == 0)
            {
                query = null;
                fragment = (path.Length == 1) ? string.Empty : path[(index + 1)..];
                return EmptyURI; 
            }
            fragment = (index < path.Length - 1) ? path[(index + 1)..] : string.Empty;
            path = path[..index];
            if ((index = path.IndexOf('?')) < 0)
                query = null;
            else
            {
                if (index == 0)
                {
                    query = (path.Length == 1) ? string.Empty : path[(index + 1)..];
                    return EmptyURI;   
                }
                query = (index < path.Length - 1) ? path[(index + 1)..] : string.Empty;
                path = path[..index];
            }
        }
        return new(path, UriKind.Relative);
    }

    public static Uri SplitLeaf(this Uri? uri, out string? leaf)
    {
        if (uri is null)
        {
            leaf = null;
            return EmptyURI;
        }
        string path;
        int index;
        if (uri.IsAbsoluteUri)
        {
            if ((path = uri.GetComponents(UriComponents.Path, UriFormat.UriEscaped)).Length == 0)
            {
                leaf = null;
                return uri;
            }
            if ((index = path.LastIndexOf('/')) < 0)
            {
                leaf = path;
                return new(uri.GetLeftPart(UriPartial.Authority) + uri.GetComponents(UriComponents.Query | UriComponents.Fragment, UriFormat.UriEscaped), UriKind.Absolute);
            }
            if (index == 0)
            {
                leaf = (path.Length > 1) ? path[1..] : string.Empty;
                return new(uri.GetLeftPart(UriPartial.Authority) + uri.GetComponents(UriComponents.Query | UriComponents.Fragment, UriFormat.UriEscaped), UriKind.Absolute);
            }
            leaf = (index < path.Length - 1) ? path[(index + 1)..] : string.Empty;
            return new($"{uri.GetLeftPart(UriPartial.Authority)}/{path[..index]}{uri.GetComponents(UriComponents.Query | UriComponents.Fragment, UriFormat.UriEscaped)}", UriKind.Absolute);
        }
        if (uri.OriginalString.Length == 0)
        {
            leaf = null;
            return uri;
        }
        string queryAndFragment;
        Uri? tempUri;
        path = uri.OriginalString;
        if ((index = path.IndexOfAny(['?', '#'])) < 0)
        {
            queryAndFragment = string.Empty;
            if (path.Contains('\\'))
                path = path.Replace('\\', '/');
            if (Uri.TryCreate($"http://tempuri.org/{path}", UriKind.Absolute, out tempUri))
                path = tempUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
        }
        else
        {
            if (index == 0)
            {
                leaf = null;
                return uri;
            }
            queryAndFragment = path[index..];
            path = path[..index];
            if (path.Contains('\\'))
                path = path.Replace('\\', '/');
            if (Uri.TryCreate($"http://tempuri.org/{path}{queryAndFragment}", UriKind.Absolute, out tempUri))
            {
                path = tempUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                queryAndFragment = uri.GetComponents(UriComponents.Query | UriComponents.Fragment, UriFormat.UriEscaped);
            }
        }
        if ((index = path.LastIndexOf('/')) < 0)
        {
            leaf = path;
            return (queryAndFragment.Length > 0) ? new(queryAndFragment, UriKind.Relative) : EmptyURI;
        }
        if (index == 0)
        {
            leaf = (path.Length > 1) ? path[1..] : null;
            if (queryAndFragment.Length > 0)
                path = $"/{queryAndFragment}";
            return (uri.OriginalString == path) ? uri : new(path, UriKind.Relative);
        }
        leaf = (index < path.Length - 1) ? path[(index + 1)..] : string.Empty;
        return new(path[..index] + queryAndFragment, UriKind.Relative);
    }

    public static Uri SplitLeaf(this Uri? uri, out string? leaf, out string? queryAndFragment)
    {
        if (uri is null)
        {
            leaf = queryAndFragment = null;
            return EmptyURI;
        }
        string path;
        int index;
        if (uri.IsAbsoluteUri)
        {
            if ((queryAndFragment = uri.GetComponents(UriComponents.Query | UriComponents.Fragment | UriComponents.KeepDelimiter, UriFormat.UriEscaped)).Length == 0)
                queryAndFragment = null;
            if ((path = uri.GetComponents(UriComponents.Path, UriFormat.UriEscaped)).Length == 0)
            {
                leaf = null;
                return (queryAndFragment is null) ? uri : new(uri.GetLeftPart(UriPartial.Authority), UriKind.Absolute);
            }
            if ((index = path.LastIndexOf('/')) < 0)
            {
                leaf = path;
                return new(uri.GetLeftPart(UriPartial.Authority), UriKind.Absolute);
            }
            if (index == 0)
            {
                leaf = (path.Length > 1) ? path[1..] : string.Empty;
                return new(uri.GetLeftPart(UriPartial.Authority), UriKind.Absolute);
            }
            leaf = (index < path.Length - 1) ? path[(index + 1)..] : string.Empty;
            return new($"{uri.GetLeftPart(UriPartial.Authority)}/{path[..index]}", UriKind.Absolute);
        }
        if (uri.OriginalString.Length == 0)
        {
            leaf = queryAndFragment = null;
            return uri;
        }
        Uri? tempUri;
        path = uri.OriginalString;
        if ((index = path.IndexOfAny(['?', '#'])) < 0)
        {
            queryAndFragment = string.Empty;
            if (path.Contains('\\'))
                path = path.Replace('\\', '/');
            if (Uri.TryCreate($"http://tempuri.org/{path}", UriKind.Absolute, out tempUri))
                path = tempUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
        }
        else
        {
            if (index == 0)
            {
                queryAndFragment = path;
                leaf = null;
                return new(uri.GetLeftPart(UriPartial.Authority), UriKind.Absolute);
            }
            queryAndFragment = path[index..];
            path = path[..index];
            if (path.Contains('\\'))
                path = path.Replace('\\', '/');
            if (Uri.TryCreate($"http://tempuri.org/{path}", UriKind.Absolute, out tempUri))
            {
                path = tempUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                queryAndFragment = uri.GetComponents(UriComponents.Query | UriComponents.Fragment, UriFormat.UriEscaped);
            }
        }
        if ((index = path.LastIndexOf('/')) < 0)
        {
            leaf = path;
            return EmptyURI;
        }
        if (index == 0)
        {
            leaf = (path.Length > 1) ? path[1..] : null;
            return (uri.OriginalString == path) ? uri : new(path, UriKind.Relative);
        }
        leaf = (index < path.Length - 1) ? path[(index + 1)..] : string.Empty;
        return new(path[..index], UriKind.Relative);
    }

    public static Uri SplitLeaf(this Uri? uri, out string? leaf, out string? query, out string? fragment)
    {
        if (uri is null)
        {
            leaf = query = fragment = null;
            return EmptyURI;
        }
        string path;
        int index;
        if (uri.IsAbsoluteUri)
        {
            if ((query = uri.GetComponents(UriComponents.Query | UriComponents.KeepDelimiter, UriFormat.UriEscaped)).Length == 0)
                query = null;
            if ((fragment = uri.GetComponents(UriComponents.Fragment | UriComponents.KeepDelimiter, UriFormat.UriEscaped)).Length == 0)
                fragment = null;
            if ((path = uri.GetComponents(UriComponents.Path, UriFormat.UriEscaped)).Length == 0)
            {
                leaf = null;
                return (query is null && fragment is null) ? uri : new(uri.GetLeftPart(UriPartial.Authority), UriKind.Absolute);
            }
            if ((index = path.LastIndexOf('/')) < 0)
            {
                leaf = path;
                return new(uri.GetLeftPart(UriPartial.Authority), UriKind.Absolute);
            }
            if (index == 0)
            {
                leaf = (path.Length > 1) ? path[1..] : string.Empty;
                return new(uri.GetLeftPart(UriPartial.Authority), UriKind.Absolute);
            }
            leaf = (index < path.Length - 1) ? path[(index + 1)..] : string.Empty;
            return new($"{uri.GetLeftPart(UriPartial.Authority)}/{path[..index]}", UriKind.Absolute);
        }
        if (uri.OriginalString.Length == 0)
        {
            leaf = query = fragment = null;
            return uri;
        }
        Uri? tempUri;
        path = uri.OriginalString;
        if ((index = path.IndexOf('#')) < 0)
        {
            fragment = null;
            if ((index = path.IndexOf('?')) < 0)
            {
                query = null;
                if (Uri.TryCreate($"http://tempuri.org/{path}", UriKind.Absolute, out tempUri))
                    path = tempUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                // path
            }
            else
            {
                if (index == 0)
                {
                    if (path.Length > 1)
                    {
                        query = path[1..];
                        // ?query
                        if (Uri.TryCreate($"http://tempuri.org?{query}", UriKind.Absolute, out tempUri))
                            query = tempUri.GetComponents(UriComponents.Query, UriFormat.UriEscaped);
                    }
                    else
                    {
                        query = string.Empty;
                        // ?
                    }
                    leaf = null;
                    return EmptyURI;
                }
                if (index < path.Length - 1)
                {
                    query = path[(index + 1)..];
                    path = path[..index];
                    if (Uri.TryCreate($"http://tempuri.org/{path}?{query}", UriKind.Absolute, out tempUri))
                    {
                        path = tempUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                        query = tempUri.GetComponents(UriComponents.Query, UriFormat.UriEscaped);
                    }
                    // path?query
                }
                else
                {
                    query = string.Empty;
                    path = path[..index];
                    if (Uri.TryCreate($"http://tempuri.org/{path}", UriKind.Absolute, out tempUri))
                        path = tempUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                    // path?
                }
            }
        }
        else
        {
            if (index == 0)
            {
                query = null;
                if (path.Length > 1)
                {
                    // #fragment
                    fragment = path[(index + 1)..];
                    if (Uri.TryCreate($"http://tempuri.org#{fragment}", UriKind.Absolute, out tempUri))
                        fragment = tempUri.GetComponents(UriComponents.Fragment, UriFormat.UriEscaped);
                }
                else
                {
                    fragment = string.Empty;
                    // #
                }
                leaf = null;
                return EmptyURI;
            }
            if (index < path.Length - 1)
            {
                fragment = path[(index + 1)..];
                path = path[..index];
                if ((index = path.IndexOf('?')) < 0)
                {
                    query = null;
                    if (Uri.TryCreate($"http://tempuri.org/{path}#{fragment}", UriKind.Absolute, out tempUri))
                    {
                        path = tempUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                        fragment = tempUri.GetComponents(UriComponents.Fragment, UriFormat.UriEscaped);
                    }
                    // path#fragment
                }
                else
                {
                    if (index == 0)
                    {
                        if (path.Length > 1)
                        {
                            // ?query#fragment
                            query = path[1..];
                            if (Uri.TryCreate($"http://tempuri.org?{query}#{fragment}", UriKind.Absolute, out tempUri))
                            {
                                query = tempUri.GetComponents(UriComponents.Query, UriFormat.UriEscaped);
                                fragment = tempUri.GetComponents(UriComponents.Fragment, UriFormat.UriEscaped);
                            }
                        }
                        else
                        {
                            query = string.Empty;
                            if (Uri.TryCreate($"http://tempuri.org#{fragment}", UriKind.Absolute, out tempUri))
                                fragment = tempUri.GetComponents(UriComponents.Fragment, UriFormat.UriEscaped);
                            // ?#fragment
                        }
                        leaf = null;
                        return EmptyURI;
                    }
                    if (index < path.Length - 1)
                    {
                        // path?query#fragment
                        query = path[(index + 1)..];
                        path = path[..index];
                        if (Uri.TryCreate($"http://tempuri.org/{path}?{query}#{fragment}", UriKind.Absolute, out tempUri))
                        {
                            path = tempUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                            query = tempUri.GetComponents(UriComponents.Query, UriFormat.UriEscaped);
                            fragment = tempUri.GetComponents(UriComponents.Fragment, UriFormat.UriEscaped);
                        }
                    }
                    else
                    {
                        query = string.Empty;
                        path = path[..index];
                        if (Uri.TryCreate($"http://tempuri.org/{path}#{fragment}", UriKind.Absolute, out tempUri))
                        {
                            path = tempUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                            fragment = tempUri.GetComponents(UriComponents.Fragment, UriFormat.UriEscaped);
                        }
                        // path?#fragment
                    }
                }
            }
            else
            {
                fragment = string.Empty;
                path = path[..index];
                if ((index = path.IndexOf('?')) < 0)
                {
                    query = null;
                    if (Uri.TryCreate($"http://tempuri.org/{path}", UriKind.Absolute, out tempUri))
                        path = tempUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                    // path#
                }
                else
                {
                    if (index == 0)
                    {
                        if (path.Length > 1)
                        {
                            // ?query#
                            query = path[(index + 1)..];
                            if (Uri.TryCreate($"http://tempuri.org?{query}", UriKind.Absolute, out tempUri))
                                query = tempUri.GetComponents(UriComponents.Query, UriFormat.UriEscaped);
                        }
                        else
                        {
                            // ?#
                            query = string.Empty;
                        }
                        leaf = null;
                        return EmptyURI;
                    }
                    if (index < path.Length - 1)
                    {
                        query = path[(index + 1)..];
                        path = path[..index];
                        if (Uri.TryCreate($"http://tempuri.org/{path}?{query}", UriKind.Absolute, out tempUri))
                        {
                            path = tempUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                            query = tempUri.GetComponents(UriComponents.Query, UriFormat.UriEscaped);
                        }
                        // path?query#
                    }
                    else
                    {
                        query = string.Empty;
                        path = path[..index];
                        if (Uri.TryCreate($"http://tempuri.org/{path}", UriKind.Absolute, out tempUri))
                            path = tempUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                        // path?#
                    }
                }
            }
        }
        if (path.Length == 1 || (index = path.LastIndexOf('/')) < 0)
        {
            leaf = path;
            return EmptyURI;
        }
        if (index == 0)
        {
            leaf = (path.Length > 0) ? path[1..] : string.Empty;
            return new("/", UriKind.Relative);
        }
        leaf = (index < path.Length - 1) ? path[(index + 1)..] : string.Empty;
        path = path[..index];
        while (path.Length > 1 && path[^1] == '/')
        {
            path = path[..^1];
        }
        return new(path, UriKind.Relative);
    }
}
