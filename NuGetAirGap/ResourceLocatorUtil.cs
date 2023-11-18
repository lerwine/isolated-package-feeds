using System.Diagnostics.CodeAnalysis;

namespace NuGetAirGap;

/// <summary>
/// Utility methods for parsing URIs and file system paths.
/// </summary>
public class ResourceLocatorUtil
{
    private static string CombinePath(string? basePath, string path)
    {
        if (path is null)
            throw new ArgumentNullException(nameof(path));
        if (path.Length == 0 || !(path = Environment.ExpandEnvironmentVariables(path)).Any(c => !char.IsWhiteSpace(c)))
            throw new ArgumentException($"{nameof(path)} cannot be empty.");
        if (Uri.TryCreate(path, UriKind.Absolute, out Uri? uri))
        {
            if (!uri.IsFile)
                throw new UriSchemeNotSupportedException(uri);
            return uri.LocalPath;
        }
        return (string.IsNullOrWhiteSpace(basePath) || Path.IsPathFullyQualified(path)) ? path : Path.Combine(basePath, path);
    }

    private static string? CombinePath(string? basePath, string pathOrUriString, out Uri? absoluteUri)
    {
        if (pathOrUriString is null)
            throw new ArgumentNullException(nameof(pathOrUriString));
        if (pathOrUriString.Length == 0 || !(pathOrUriString = Environment.ExpandEnvironmentVariables(pathOrUriString)).Any(c => !char.IsWhiteSpace(c)))
            throw new ArgumentException($"{nameof(pathOrUriString)} cannot be empty.");
        if (Uri.TryCreate(pathOrUriString, UriKind.Absolute, out absoluteUri))
            return absoluteUri.IsFile ? absoluteUri.LocalPath : null;
        absoluteUri = null;
        return (string.IsNullOrWhiteSpace(basePath) || Path.IsPathFullyQualified(pathOrUriString)) ? pathOrUriString : Path.Combine(basePath, pathOrUriString);
    }

    private static T GetFileSystemInfo<T>(string? basePath, string path, Func<string, T> factory) where T : FileSystemInfo
    {
        try { return factory(CombinePath(basePath, path)); }
        catch (NotSupportedException exception) { throw new ArgumentException(exception.Message, nameof(path)); }
    }

    private static bool TryParseAsFileSystemInfo<T>(string? basePath, string pathOrUriString, Func<string, T> factory, out Uri absoluteUri, [NotNullWhen(true)] out T? result) where T : FileSystemInfo
    {
        if ((basePath = CombinePath(basePath, pathOrUriString, out Uri? uri)) is null)
        {
            result = null;
            absoluteUri = uri!;
            return false;
        }

        try { result = factory(basePath); ; }
        catch (NotSupportedException exception) { throw new ArgumentException(exception.Message, nameof(pathOrUriString)); }
        try { absoluteUri = uri ?? new(result.FullName, UriKind.Absolute); }
        catch (UriFormatException exception) { throw new ArgumentException(exception.Message, nameof(pathOrUriString)); }
        return true;
    }

    private static bool TryParseHttpOrFileAsFileSystemInfo<T>(string? basePath, string pathOrUriString, Func<string, T> factory, out Uri absoluteUri, [NotNullWhen(true)] out T? result) where T : FileSystemInfo
    {
        if ((basePath = CombinePath(basePath, pathOrUriString, out Uri? uri)) is null)
        {
            if (uri!.Scheme != Uri.UriSchemeHttps && uri!.Scheme != Uri.UriSchemeHttp)
                throw new UriSchemeNotSupportedException(uri);
            result = null;
            absoluteUri = uri;
            return false;
        }
        try { result = factory(basePath); }
        catch (NotSupportedException exception) { throw new ArgumentException(exception.Message, nameof(pathOrUriString)); }
        try { absoluteUri = uri ?? new(result.FullName, UriKind.Absolute); }
        catch (UriFormatException exception) { throw new ArgumentException(exception.Message, nameof(pathOrUriString)); }
        return true;
    }

    /// <summary>
    /// Gets the full path from URI string or relative to a base path.
    /// </summary>
    /// <param name="basePath">The base filesystem path or <see langword="null"/> for no base path.</param>
    /// <param name="path">The relative filesystem path or URI representing a filesystem path.</param>
    /// <returns>The full path string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty or has invalid characters.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permissions.</exception>
    /// <exception cref="PathTooLongException">The combined path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="UriSchemeNotSupportedException"><paramref name="path"/> is an absolute URI, but the scheme is not <see cref="Uri.UriSchemeFile"/>.</exception>
    public static string GetLocalPath(string? basePath, string path)
    {
        if (path is null)
            throw new ArgumentNullException(nameof(path));
        if (path.Length == 0 || !(path = Environment.ExpandEnvironmentVariables(path)).Any(c => !char.IsWhiteSpace(c)))
            throw new ArgumentException($"{nameof(path)} cannot be empty.");
        try
        {
            if (Uri.TryCreate(path, UriKind.Absolute, out Uri? uri))
            {
                if (!uri.IsFile)
                    throw new UriSchemeNotSupportedException(uri);
                return Path.GetFullPath(uri.LocalPath);
            }
            return (string.IsNullOrWhiteSpace(basePath) || Path.IsPathFullyQualified(path)) ? Path.GetFullPath(path) : Path.GetFullPath(path, basePath);
        }
        catch (NotSupportedException exception) { throw new ArgumentException(exception.Message, nameof(path)); }
    }

    /// <summary>
    /// Parses or builds an absolute URI and tries extract the local filesystem path.
    /// </summary>
    /// <param name="basePath"></param>
    /// <param name="pathOrUriString"></param>
    /// <param name="absoluteUri"></param>
    /// <param name="fullPath"></param>
    /// <returns><see langword="true"/> if <paramref name="fullPath"/> is not <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pathOrUriString"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="pathOrUriString"/> is empty or has invalid characters.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permissions.</exception>
    /// <exception cref="PathTooLongException">The combined path, file name, or both exceed the system-defined maximum length.</exception>
    public static bool TryParseLocalPath(string? basePath, string pathOrUriString, out Uri absoluteUri, [NotNullWhen(true)] out string? fullPath)
    {
        if (pathOrUriString is null)
            throw new ArgumentNullException(nameof(pathOrUriString));
        if (pathOrUriString.Length == 0 || !(pathOrUriString = Environment.ExpandEnvironmentVariables(pathOrUriString)).Any(c => !char.IsWhiteSpace(c)))
            throw new ArgumentException($"{nameof(pathOrUriString)} cannot be empty.");
        try
        {
            if (Uri.TryCreate(pathOrUriString, UriKind.Absolute, out Uri? uri))
            {
                absoluteUri = uri;
                if (uri.IsFile)
                {
                    fullPath = Path.GetFullPath(uri.LocalPath);
                    return true;
                }
                fullPath = null;
                return false;
            }
            fullPath = (string.IsNullOrWhiteSpace(basePath) || Path.IsPathFullyQualified(pathOrUriString)) ? Path.GetFullPath(pathOrUriString) : Path.GetFullPath(pathOrUriString, basePath);
            absoluteUri = new(fullPath, UriKind.Absolute);
        }
        catch (NotSupportedException exception) { throw new ArgumentException(exception.Message, nameof(pathOrUriString)); }
        catch (UriFormatException exception) { throw new ArgumentException(exception.Message, nameof(pathOrUriString)); }
        return true;
    }

    /// <summary>
    /// Gets a <see cref="FileSystemInfo"/> from a base and relative path, where the return value is a <see cref="FileInfo"/> only if the referenced file exists.
    /// </summary>
    /// <param name="basePath">The base filesystem path or <see langword="null"/> for no base path.</param>
    /// <param name="path">The relative filesystem path or URI representing a filesystem path.</param>
    /// <returns>A <see cref="FileInfo"/> object if it exists, otherwise, a <see cref="DirectoryInfo"/> object, regardless of whether it exists or not.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty or has invalid characters.</exception>
    /// <exception cref="PathTooLongException">The combined path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permissions.</exception>
    /// <exception cref="UnauthorizedAccessException">Access to combined path is denied.</exception>
    public static FileSystemInfo GetFileOrDirectory(string? basePath, string path) => GetFileSystemInfo<FileSystemInfo>(basePath, path, p => File.Exists(path) ? new FileInfo(path) : new DirectoryInfo(path));

    /// <summary>
    /// Parses or builds an absolute URI and tries extract a <see cref="FileSystemInfo"/>, where the extracted value is a <see cref="FileInfo"/> only if it exists.
    /// </summary>
    /// <param name="basePath">The base filesystem path or <see langword="null"/> for no base path.</param>
    /// <param name="pathOrUriString">A filesystem path or an absolute URI string.</param>
    /// <param name="absoluteUri">The parsed absolute URI.</param>
    /// <param name="fileSystemInfo">A <see cref="FileInfo"/>, if it exists;
    /// A <see cref="DirectoryInfo"/> if <paramref name="absoluteUri"/> is a <see cref="Uri.UriSchemeFile"/>, regardless of whether it exists;
    /// otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="fileSystemInfo"/> is not <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pathOrUriString"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="pathOrUriString"/> is empty or has invalid characters.</exception>
    /// <exception cref="PathTooLongException">The combined path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permissions.</exception>
    /// <exception cref="UnauthorizedAccessException">Access to combined path is denied.</exception>
    public static bool TryParseAsFileOrDirectory(string? basePath, string pathOrUriString, out Uri absoluteUri, [NotNullWhen(true)] out FileSystemInfo? fileSystemInfo) =>
        TryParseAsFileSystemInfo(basePath, pathOrUriString, p => File.Exists(pathOrUriString) ? new FileInfo(pathOrUriString) : new DirectoryInfo(pathOrUriString), out absoluteUri, out fileSystemInfo);

    /// <summary>
    /// Parses or builds a <see cref="Uri.UriSchemeFile"/>, <see cref="Uri.UriSchemeHttps"/>, or <see cref="Uri.UriSchemeHttp"/> URI and tries extract a <see cref="FileSystemInfo"/>,
    /// where the extracted value is a <see cref="FileInfo"/> only if it exists.
    /// </summary>
    /// <param name="basePath">The base filesystem path or <see langword="null"/> for no base path.</param>
    /// <param name="pathOrUriString">A filesystem path or an absolute URI with the <see cref="Uri.UriSchemeFile"/>, <see cref="Uri.UriSchemeHttps"/>, or <see cref="Uri.UriSchemeHttp"/> scheme.</param>
    /// <param name="absoluteUri">The parsed <see cref="Uri.UriSchemeFile"/>, <see cref="Uri.UriSchemeHttps"/>, or <see cref="Uri.UriSchemeHttp"/> URI.</param>
    /// <param name="fileSystemInfo">A <see cref="FileInfo"/>, if it exists;
    /// A <see cref="DirectoryInfo"/> if <paramref name="absoluteUri"/> is a <see cref="Uri.UriSchemeFile"/>, regardless of whether it exists;
    /// otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="fileSystemInfo"/> is not <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pathOrUriString"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="pathOrUriString"/> is empty or has invalid characters.</exception>
    /// <exception cref="PathTooLongException">The combined path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permissions.</exception>
    /// <exception cref="UnauthorizedAccessException">Access to combined path is denied.</exception>
    /// <exception cref="UriSchemeNotSupportedException"><paramref name="pathOrUriString"/> is an absolute URI, but the scheme is not <see cref="Uri.UriSchemeHttps"/>,
    /// <see cref="Uri.UriSchemeHttp"/>, or <see cref="Uri.UriSchemeFile"/>.</exception>
    public static bool TryParseHttpOrFileAsFileOrDirectory(string? basePath, string pathOrUriString, out Uri absoluteUri, [NotNullWhen(true)] out FileSystemInfo? fileSystemInfo) =>
        TryParseHttpOrFileAsFileSystemInfo(basePath, pathOrUriString, p => File.Exists(pathOrUriString) ? new FileInfo(pathOrUriString) : new DirectoryInfo(pathOrUriString), out absoluteUri, out fileSystemInfo);

    /// <summary>
    /// Gets a <see cref="FileSystemInfo"/> from a base and relative path, where the return value is a <see cref="DirectoryInfo"/> only if the referenced subdirectory exists.
    /// </summary>
    /// <param name="basePath">The base filesystem path or <see langword="null"/> for no base path.</param>
    /// <param name="path">The relative filesystem path or URI representing a filesystem path.</param>
    /// <returns>A <see cref="DirectoryInfo"/> object if it exists, otherwise, a <see cref="FileInfo"/> object, regardless of whether it exists or not.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty or has invalid characters.</exception>
    /// <exception cref="PathTooLongException">The combined path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permissions.</exception>
    /// <exception cref="UnauthorizedAccessException">Access to combined path is denied.</exception>
    /// <exception cref="UriSchemeNotSupportedException"><paramref name="path"/> is an absolute URI, but the scheme is not <see cref="Uri.UriSchemeFile"/>.</exception>
    public static FileSystemInfo GetDirectoryOrFile(string? basePath, string path) => GetFileSystemInfo<FileSystemInfo>(basePath, path,
        p => Directory.Exists(path) ? new DirectoryInfo(path) : new FileInfo(path));

    /// <summary>
    /// Parses or builds an absolute URI and tries extract a <see cref="FileSystemInfo"/>, where the extracted value is a <see cref="DirectoryInfo"/> only if it exists.
    /// </summary>
    /// <param name="basePath">The base filesystem path or <see langword="null"/> for no base path.</param>
    /// <param name="pathOrUriString">A filesystem path or an absolute URI string.</param>
    /// <param name="absoluteUri">The parsed absolute URI.</param>
    /// <param name="fileSystemInfo">A <see cref="DirectoryInfo"/>, if it exists;
    /// A <see cref="FileInfo"/> if <paramref name="absoluteUri"/> is a <see cref="Uri.UriSchemeFile"/>, regardless of whether it exists;
    /// otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="fileSystemInfo"/> is not <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pathOrUriString"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="pathOrUriString"/> is empty or has invalid characters.</exception>
    /// <exception cref="PathTooLongException">The combined path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permissions.</exception>
    /// <exception cref="UnauthorizedAccessException">Access to combined path is denied.</exception>
    public static bool TryParseAsDirectoryOrFile(string? basePath, string pathOrUriString, out Uri absoluteUri, [NotNullWhen(true)] out FileSystemInfo? fileSystemInfo) =>
        TryParseAsFileSystemInfo(basePath, pathOrUriString, p => Directory.Exists(pathOrUriString) ? new DirectoryInfo(pathOrUriString) : new FileInfo(pathOrUriString), out absoluteUri, out fileSystemInfo);

    /// <summary>
    /// Parses or builds a <see cref="Uri.UriSchemeFile"/>, <see cref="Uri.UriSchemeHttps"/>, or <see cref="Uri.UriSchemeHttp"/> URI and tries extract a <see cref="FileSystemInfo"/>,
    /// where the extracted value is a <see cref="DirectoryInfo"/> only if it exists.
    /// </summary>
    /// <param name="basePath">The base filesystem path or <see langword="null"/> for no base path.</param>
    /// <param name="pathOrUriString">A filesystem path or an absolute URI with the <see cref="Uri.UriSchemeFile"/>, <see cref="Uri.UriSchemeHttps"/>, or <see cref="Uri.UriSchemeHttp"/> scheme.</param>
    /// <param name="absoluteUri">The parsed <see cref="Uri.UriSchemeFile"/>, <see cref="Uri.UriSchemeHttps"/>, or <see cref="Uri.UriSchemeHttp"/> URI.</param>
    /// <param name="fileSystemInfo">A <see cref="DirectoryInfo"/>, if it exists;
    /// A <see cref="FileInfo"/> if <paramref name="absoluteUri"/> is a <see cref="Uri.UriSchemeFile"/>, regardless of whether it exists;
    /// otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="fileSystemInfo"/> is not <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pathOrUriString"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="pathOrUriString"/> is empty or has invalid characters.</exception>
    /// <exception cref="PathTooLongException">The combined path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permissions.</exception>
    /// <exception cref="UnauthorizedAccessException">Access to combined path is denied.</exception>
    /// <exception cref="UriSchemeNotSupportedException"><paramref name="pathOrUriString"/> is an absolute URI, but the scheme is not <see cref="Uri.UriSchemeHttps"/>,
    /// <see cref="Uri.UriSchemeHttp"/>, or <see cref="Uri.UriSchemeFile"/>.</exception>
    public static bool TryParseHttpOrFileAsDirectoryOrFile(string? basePath, string pathOrUriString, out Uri absoluteUri, [NotNullWhen(true)] out FileSystemInfo? fileSystemInfo) =>
        TryParseHttpOrFileAsFileSystemInfo(basePath, pathOrUriString, p => Directory.Exists(pathOrUriString) ? new DirectoryInfo(pathOrUriString) : new FileInfo(pathOrUriString), out absoluteUri, out fileSystemInfo);

    /// <summary>
    /// Gets a <see cref="FileInfo"/> from a base and relative path.
    /// </summary>
    /// <param name="basePath">The base filesystem path or <see langword="null"/> for no base path.</param>
    /// <param name="path">The relative filesystem path or URI representing a filesystem path.</param>
    /// <returns>A <see cref="FileInfo"/> object, regardless of whether it exists or not.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty or has invalid characters.</exception>
    /// <exception cref="PathTooLongException">The combined path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permissions.</exception>
    /// <exception cref="UnauthorizedAccessException">Access to combined path is denied.</exception>
    /// <exception cref="UriSchemeNotSupportedException"><paramref name="path"/> is an absolute URI, but the scheme is not <see cref="Uri.UriSchemeFile"/>.</exception>
    public static FileInfo GetFileInfo(string? basePath, string path) => GetFileSystemInfo(basePath, path, p => new FileInfo(p));

    /// <summary>
    /// Parses or builds an absolute URI and tries extract a <see cref="FileInfo"/>.
    /// </summary>
    /// <param name="basePath">The base filesystem path or <see langword="null"/> for no base path.</param>
    /// <param name="pathOrUriString">A filesystem path or an absolute URI string.</param>
    /// <param name="absoluteUri">The parsed absolute URI.</param>
    /// <param name="fileInfo">A <see cref="FileInfo"/> if <paramref name="absoluteUri"/> is a <see cref="Uri.UriSchemeFile"/>, regardless of whether it exists;
    /// otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="fileInfo"/> is not <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pathOrUriString"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="pathOrUriString"/> is empty or has invalid characters.</exception>
    /// <exception cref="PathTooLongException">The combined path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permissions.</exception>
    /// <exception cref="UnauthorizedAccessException">Access to combined path is denied.</exception>
    public static bool TryParseAsFileInfo(string? basePath, string pathOrUriString, out Uri absoluteUri, [NotNullWhen(true)] out FileInfo? fileInfo) =>
        TryParseAsFileSystemInfo(basePath, pathOrUriString, p => new FileInfo(pathOrUriString), out absoluteUri, out fileInfo);

    /// <summary>
    /// Parses or builds a <see cref="Uri.UriSchemeFile"/>, <see cref="Uri.UriSchemeHttps"/>, or <see cref="Uri.UriSchemeHttp"/> URI and tries extract a <see cref="FileInfo"/>.
    /// </summary>
    /// <param name="basePath">The base filesystem path or <see langword="null"/> for no base path.</param>
    /// <param name="pathOrUriString">A filesystem path or an absolute URI with the <see cref="Uri.UriSchemeFile"/>, <see cref="Uri.UriSchemeHttps"/>, or <see cref="Uri.UriSchemeHttp"/> scheme.</param>
    /// <param name="absoluteUri">The parsed <see cref="Uri.UriSchemeFile"/>, <see cref="Uri.UriSchemeHttps"/>, or <see cref="Uri.UriSchemeHttp"/> URI.</param>
    /// <param name="fileInfo">A <see cref="FileInfo"/> if <paramref name="absoluteUri"/> is a <see cref="Uri.UriSchemeFile"/>, regardless of whether it exists;
    /// otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="fileInfo"/> is not <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pathOrUriString"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="pathOrUriString"/> is empty or has invalid characters.</exception>
    /// <exception cref="PathTooLongException">The combined path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permissions.</exception>
    /// <exception cref="UnauthorizedAccessException">Access to combined path is denied.</exception>
    /// <exception cref="UriSchemeNotSupportedException"><paramref name="pathOrUriString"/> is an absolute URI, but the scheme is not <see cref="Uri.UriSchemeHttps"/>,
    /// <see cref="Uri.UriSchemeHttp"/>, or <see cref="Uri.UriSchemeFile"/>.</exception>
    public static bool TryParseHttpOrFileAsFileInfo(string? basePath, string pathOrUriString, out Uri absoluteUri, [NotNullWhen(true)] out FileInfo? fileInfo) =>
        TryParseHttpOrFileAsFileSystemInfo(basePath, pathOrUriString, p => new FileInfo(pathOrUriString), out absoluteUri, out fileInfo);

    /// <summary>
    /// Gets a <see cref="DirectoryInfo"/> from a base and relative path.
    /// </summary>
    /// <param name="basePath">The base filesystem path or <see langword="null"/> for no base path.</param>
    /// <param name="path">The relative filesystem path or URI representing a filesystem path.</param>
    /// <returns>The <see cref="DirectoryInfo"/> object, regardless of whether it exists or not.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty or has invalid characters.</exception>
    /// <exception cref="PathTooLongException">The combined path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permissions.</exception>
    /// <exception cref="UriSchemeNotSupportedException"><paramref name="path"/> is an absolute URI, but the scheme is not <see cref="Uri.UriSchemeFile"/>.</exception>
    public static DirectoryInfo GetDirectoryInfo(string? basePath, string path) => GetFileSystemInfo(basePath, path, p => new DirectoryInfo(p));

    /// <summary>
    /// Parses or builds an absolute URI and tries extract a <see cref="DirectoryInfo"/>.
    /// </summary>
    /// <param name="basePath">The base filesystem path or <see langword="null"/> for no base path.</param>
    /// <param name="pathOrUriString">A filesystem path or an absolute URI string.</param>
    /// <param name="absoluteUri">The parsed absolute URI.</param>
    /// <param name="directoryInfo">A <see cref="DirectoryInfo"/> if <paramref name="absoluteUri"/> is a <see cref="Uri.UriSchemeFile"/>, regardless of whether it exists;
    /// otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="directoryInfo"/> is not <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pathOrUriString"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="pathOrUriString"/> is empty or has invalid characters.</exception>
    /// <exception cref="PathTooLongException">The combined path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permissions.</exception>
    public static bool TryParseAsDirectoryInfo(string? basePath, string pathOrUriString, out Uri absoluteUri, [NotNullWhen(true)] out DirectoryInfo? directoryInfo) =>
        TryParseAsFileSystemInfo(basePath, pathOrUriString, p => new DirectoryInfo(pathOrUriString), out absoluteUri, out directoryInfo);

    /// <summary>
    /// Parses or builds a <see cref="Uri.UriSchemeFile"/>, <see cref="Uri.UriSchemeHttps"/>, or <see cref="Uri.UriSchemeHttp"/> URI and tries extract a <see cref="DirectoryInfo"/>.
    /// </summary>
    /// <param name="basePath">The base filesystem path or <see langword="null"/> for no base path.</param>
    /// <param name="pathOrUriString">A filesystem path or an absolute URI with the <see cref="Uri.UriSchemeFile"/>, <see cref="Uri.UriSchemeHttps"/>, or <see cref="Uri.UriSchemeHttp"/> scheme.</param>
    /// <param name="absoluteUri">The parsed <see cref="Uri.UriSchemeFile"/>, <see cref="Uri.UriSchemeHttps"/>, or <see cref="Uri.UriSchemeHttp"/> URI.</param>
    /// <param name="directoryInfo">A <see cref="DirectoryInfo"/> if <paramref name="absoluteUri"/> is a <see cref="Uri.UriSchemeFile"/>, regardless of whether it exists;
    /// otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="directoryInfo"/> is not <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pathOrUriString"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="pathOrUriString"/> is empty or has invalid characters.</exception>
    /// <exception cref="PathTooLongException">The combined path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permissions.</exception>
    /// <exception cref="UriSchemeNotSupportedException"><paramref name="pathOrUriString"/> is an absolute URI, but the scheme is not <see cref="Uri.UriSchemeHttps"/>,
    /// <see cref="Uri.UriSchemeHttp"/>, or <see cref="Uri.UriSchemeFile"/>.</exception>
    public static bool TryParseHttpOrFileAsDirectoryInfo(string? basePath, string pathOrUriString, out Uri absoluteUri, [NotNullWhen(true)] out DirectoryInfo? directoryInfo) =>
        TryParseHttpOrFileAsFileSystemInfo(basePath, pathOrUriString, p => new DirectoryInfo(pathOrUriString), out absoluteUri, out directoryInfo);
}