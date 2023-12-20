using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IsolatedPackageFeeds.Shared;

public class ResourceLocation
{
    public ResourceLocation(FileInfo fileInfo)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        FullName = fileInfo.FullName;
        Name = fileInfo.Name;
        Url = new Uri(fileInfo.FullName, UriKind.Absolute);
        Parent = (fileInfo.Directory is null) ? null : new ResourceLocation(fileInfo.Directory);
        int index = Name.IndexOf('.', 1);
        if (index < 0)
        {
            BaseName = Name;
            Extension = string.Empty;
        }
        else
        {
            BaseName = Name[..index];
            Extension = Name[index..];
        }
    }

    public ResourceLocation(DirectoryInfo directoryInfo)
    {
        ArgumentNullException.ThrowIfNull(directoryInfo);
        FullName = directoryInfo.FullName;
        Name = directoryInfo.Name;
        Url = new Uri(directoryInfo.FullName, UriKind.Absolute);
        Parent = (directoryInfo.Parent is null) ? null : new ResourceLocation(directoryInfo.Parent);
        int index = Name.IndexOf('.', 1);
        if (index < 0)
        {
            BaseName = Name;
            Extension = string.Empty;
        }
        else
        {
            BaseName = Name[..index];
            Extension = Name[index..];
        }
    }

    public ResourceLocation(Uri url)
    {
        ArgumentNullException.ThrowIfNull(url);
        if (!url.IsAbsoluteUri) throw new ArgumentException($"{nameof(url)} cannot be relative.", nameof(url));
        if ((url = url.ToNormalized()).IsFile)
        {
            FullName = Path.TrimEndingDirectorySeparator(Path.GetFullPath(url.LocalPath));
            string directoryName = Path.GetDirectoryName(FullName)!;
            if (string.IsNullOrEmpty(directoryName))
                Name = FullName;
            else
            {
                string name = Path.GetFileName(FullName);
                if (string.IsNullOrEmpty(name))
                    Name = FullName;
                else
                {
                    Name = name;
                    Parent = new(new DirectoryInfo(directoryName));
                }
            }
            Url = (url.LocalPath == FullName) ? url : new(FullName, UriKind.Absolute);
        }
        else
        {
            if (url.Scheme != Uri.UriSchemeHttps && url.Scheme != Uri.UriSchemeHttp)
                throw new ArgumentException($"{nameof(url)} must have the scheme {Uri.UriSchemeFile}, {Uri.UriSchemeHttps}, or {Uri.UriSchemeHttp}.");
            Uri parent = url.SplitLeaf(out string? name);
            while (name is not null && name.Length == 0)
            {
                url = parent;
                parent = url.SplitLeaf(out name);
            }
            FullName = (Url = url).AbsoluteUri;
            if (name is null)
            {
                Name = BaseName = Extension = string.Empty;
                return;
            }
            Parent = new(parent);
            Name = name;
        }
        int index = Name.IndexOf('.', 1);
        if (index < 0)
        {
            BaseName = Name;
            Extension = string.Empty;
        }
        else
        {
            BaseName = Name[..index];
            Extension = Name[index..];
        }
    }

    public string FullName { get; }
    public string Name { get; }
    public string BaseName { get; }
    public string Extension { get; }
    public Uri Url { get; }
    public ResourceLocation? Parent { get; }
}