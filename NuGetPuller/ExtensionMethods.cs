using System.Diagnostics.CodeAnalysis;
using IsolatedPackageFeeds.Shared;
using NuGet.Versioning;

namespace NuGetPuller;

public static class ExtensionMethods
{
    public static bool TryGetExistingFileInfo(this string? path, out Exception? error, [NotNullWhen(true)] out FileInfo? result)
    {
        if (string.IsNullOrEmpty(path))
        {
            result = null;
            error = null;
            return false;
        }
        try
        {
            FileInfo fileInfo = new(path);
            if (fileInfo.Exists)
            {
                result = fileInfo;
                error = null;
                return true;
            }
            result = fileInfo;
            error = null;
        }
        catch (Exception exception)
        {
            error = exception;
            result = null;
        }
        return false;
    }
    
    public static bool TryGetFileInfo(this string? path, out Exception? error, [NotNullWhen(true)] out FileInfo? result)
    {
        if (string.IsNullOrEmpty(path))
        {
            result = null;
            error = null;
            return false;
        }
        FileInfo fileInfo;
        try
        {
            fileInfo = new(path);
            if (fileInfo.Exists || (fileInfo.Directory is not null && fileInfo.Directory.Exists))
                error = null;
            else
            {
                error = null;
                result = fileInfo;
                return false;
            }
        }
        catch (Exception exception)
        {
            error = exception;
            result = null;
            return false;
        }
        result = fileInfo;
        return true;
    }
    
    public static bool TryGetExistingDirectoryInfo(this string? path, out Exception? error, [NotNullWhen(true)] out DirectoryInfo? result)
    {
        if (string.IsNullOrEmpty(path))
        {
            result = null;
            error = null;
            return false;
        }
        try
        {
            DirectoryInfo directoryInfo = new(path);
            if (directoryInfo.Exists)
            {
                result = directoryInfo;
                error = null;
                return true;
            }
            result = directoryInfo;
            error = null;
        }
        catch (Exception exception)
        {
            error = exception;
            result = null;
        }
        return false;
    }
    
    public static bool TryGetDirectoryInfo(this string? path, out Exception? error, [NotNullWhen(true)] out DirectoryInfo? result)
    {
        if (string.IsNullOrEmpty(path))
        {
            result = null;
            error = null;
            return false;
        }
        DirectoryInfo directoryInfo;
        try
        {
            directoryInfo = new(path);
            if (directoryInfo.Exists || (directoryInfo.Parent is not null && directoryInfo.Parent.Exists))
                error = null;
            else
            {
                error = null;
                result = directoryInfo;
                return false;
            }
        }
        catch (Exception exception)
        {
            error = exception;
            result = null;
            return false;
        }
        result = directoryInfo;
        return true;
    }
    
    public static IEnumerable<FileInfo> ExpandFileInfos(this string? path)
    {
        if (string.IsNullOrEmpty(path))
            yield break;
        FileInfo? fileInfo;
        if (path.Contains(';'))
        {
            foreach (string p in path.Split(';'))
            {
                if (p.Length == 0)
                    throw new FileNotFoundException("Path cannot be empty.", string.Empty);
                try
                {
                    if (!(fileInfo = new(p)).Exists)
                        fileInfo = null;
                }
                catch (Exception exception)
                {
                    throw new FileSystemAccessException(exception.Message, p, exception);
                }
                if (fileInfo is null)
                {
                    FileInfo[]? files;
                    try
                    {
                        DirectoryInfo? directoryInfo = new(p);
                        files = directoryInfo.Exists ? directoryInfo.GetFiles("*.nupkg") : null;
                    }
                    catch (Exception exception)
                    {
                        throw new FileSystemAccessException(exception.Message, p, exception);
                    }
                    if (files is null)
                        throw new FileNotFoundException($"File or subdirectory \"{p}\" not found.", p);
                    foreach (var f in files)
                        yield return f;
                }
                else
                    yield return fileInfo;
            }
        }
        else
        {
            try
            {
                if (!(fileInfo = new(path)).Exists)
                    fileInfo = null;
            }
            catch (Exception exception)
            {
                throw new FileSystemAccessException(exception.Message, path, exception);
            }
            if (fileInfo is null)
            {
                FileInfo[]? files;
                try
                {
                    DirectoryInfo? directoryInfo = new(path);
                    files = directoryInfo.Exists ? directoryInfo.GetFiles("*.nupkg") : null;
                }
                catch (Exception exception)
                {
                    throw new FileSystemAccessException(exception.Message, path, exception);
                }
                if (files is null)
                    throw new FileNotFoundException($"File or subdirectory \"{path}\" not found.", path);
                foreach (var f in files)
                    yield return f;
            }
            else
                yield return fileInfo;
        }
    }

    /// <summary>
    /// Attempts to parse list of valid NuGet package identifiers.
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <param name="result">The parsed package identifiers.</param>
    /// <returns><see langword="true"/> if there is one or more package identifiers in <paramref name="result"/>;
    /// <see langword="false"/> if <paramref name="value"/> contained at least one invalid package identifer;
    /// otherwise, <see langword="null"/> if <paramref name="value"/> was null, empty, or whitespace-only.</returns>
    public static bool? TryGetValidNuGetPackageIdentifierList(this string? value, out string[] result)
    {
        if (value is null || (value = value.Trim()).Length == 0)
        {
            result = [];
            return null;
        }
        if (value.Contains(','))
        {
            result = value.Split(',').Select(s => s.Trim()).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();
            return result.All(id => id.Length > 0 && NuGet.Packaging.PackageIdValidator.IsValidPackageId(id));
        }
        if (NuGetVersion.TryParse(value, out NuGetVersion? version))
        {
            result = [value];
            return NuGet.Packaging.PackageIdValidator.IsValidPackageId(value);
        }
        result = [];
        return false;
    }

    /// <summary>
    /// Attempts to parse list of NuGet version strings.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="result"></param>
    /// <returns><see langword="true"/> if there is one or more versions in <paramref name="result"/>;
    /// <see langword="false"/> if <paramref name="value"/> contained at least one invalid version string;
    /// otherwise, <see langword="null"/> if <paramref name="value"/> was null, empty, or whitespace-only.</returns>
    public static bool? TryParseNuGetVersionList(this string? value, out NuGetVersion[] result)
    {
        if (value is null || (value = value.Trim()).Length == 0)
        {
            result = [];
            return null;
        }
        NuGetVersion? version;
        if (value.Contains(','))
        {
            string[] arr = value.Split(',').Select(s => s.Trim()).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray(); 
            var len = arr.Length;
            result = new NuGetVersion[len];
            for (var i = 0; i < len; i++)
            {
                var v = arr[i];
                if (v.Length == 0 || !NuGetVersion.TryParse(v, out version))
                    return false;
                result[i] = version;
            }
            return true;
        }
        if (NuGetVersion.TryParse(value, out version))
        {
            result = [version];
            return true;
        }
        result = [];
        return false;
    }
}
