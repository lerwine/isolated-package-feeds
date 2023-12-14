using System.Diagnostics.CodeAnalysis;
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
        try { result = new(path); }
        catch (Exception exception)
        {
            error = exception;
            result = null;
            return false;
        }
        error = null;
        try { return result.Exists; }
        catch (Exception exception) { error = exception; }
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
        try { result = new(path); }
        catch (Exception exception)
        {
            error = exception;
            result = null;
            return false;
        }
        error = null;
        try { return result.Exists || (result.Directory is not null && result.Directory.Exists); }
        catch (Exception exception) { error = exception; }
        return false;
    }
    
    public static bool TryGetExistingDirectoryInfo(this string? path, out Exception? error, [NotNullWhen(true)] out DirectoryInfo? result)
    {
        if (string.IsNullOrEmpty(path))
        {
            result = null;
            error = null;
            return false;
        }
        try { result = new(path); }
        catch (Exception exception)
        {
            error = exception;
            result = null;
            return false;
        }
        error = null;
        try { return result.Exists; }
        catch (Exception exception) { error = exception; }
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
        try { result = new(path); }
        catch (Exception exception)
        {
            error = exception;
            result = null;
            return false;
        }
        error = null;
        try { return result.Exists || (result.Parent is not null && result.Parent.Exists); }
        catch (Exception exception) { error = exception; }
        return false;
    }
    
    public static bool TryGetExistingFileSystemInfo(this string? path, out Exception? error, [NotNullWhen(true)] out FileSystemInfo? fileSystemInfo)
    {
        if (string.IsNullOrEmpty(path))
        {
            fileSystemInfo = null;
            error = null;
            return false;
        }
        try { fileSystemInfo = new DirectoryInfo(path); }
        catch (Exception exception)
        {
            error = exception;
            fileSystemInfo = null;
            return false;
        }
        try
        {
            if (fileSystemInfo.Exists)
            {
                error = null;
                return true;
            }
            fileSystemInfo = new FileInfo(path);
        }
        catch (Exception exception)
        {
            error = exception;
            return false;
        }
        error = null;
        try { return fileSystemInfo.Exists; }
        catch (Exception exception) { error = exception; }
        return false;
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
