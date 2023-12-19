using System.Text.RegularExpressions;

namespace IsolatedPackageFeeds.Shared;

public static partial class CommonStatic
{
    public static readonly StringComparer NoCaseComparer = StringComparer.CurrentCultureIgnoreCase;

    public static readonly Regex NonNormalizedWhiteSpaceRegex = CreateNonNormalizedWhiteSpaceRegex();

    public static readonly Regex LineBreakRegex = CreateLineBreakRegex();

    public static readonly Uri EmptyURI = new(string.Empty, UriKind.Relative);

    [GeneratedRegex(@"( |(?! ))[\r\n\s]+", RegexOptions.Compiled)]
    private static partial Regex CreateNonNormalizedWhiteSpaceRegex();

    [GeneratedRegex(@"\r?\n|\n", RegexOptions.Compiled)]
    private static partial Regex CreateLineBreakRegex();

    public static FileStream OpenFileStream(string path, FileMode mode, FileAccess access, FileShare share, Func<string, Exception, LoggedException> onAccessDenied, Func<string, Exception, LoggedException> onException)
    {
        try { return new FileStream(path, mode, access, share); }
        catch (UnauthorizedAccessException exception) { throw onAccessDenied(path, exception); }
        catch (System.Security.SecurityException exception) { throw onAccessDenied(path, exception); }
        catch (ArgumentException exception) { throw onException(path, exception); }
        catch (DirectoryNotFoundException exception) { throw onException(path, exception); }
        catch (PathTooLongException exception) { throw onException(path, exception); }
        catch (IOException exception) { throw onException(path, exception); }
    }

    public static StreamWriter OpenStreamWriter(string path, Func<string, Exception, LoggedException> onAccessDenied, Func<string, Exception, LoggedException> onException)
    {
        try { return new StreamWriter(path, false, new System.Text.UTF8Encoding(false, false)); }
        catch (UnauthorizedAccessException exception) { throw onAccessDenied(path, exception); }
        catch (System.Security.SecurityException exception) { throw onAccessDenied(path, exception); }
        catch (ArgumentException exception) { throw onException(path, exception); }
        catch (DirectoryNotFoundException exception) { throw onException(path, exception); }
        catch (PathTooLongException exception) { throw onException(path, exception); }
        catch (IOException exception) { throw onException(path, exception); }
    }

    public static StreamReader OpenStreamReader(string path, Func<string, Exception, LoggedException> onAccessDenied, Func<string, Exception, LoggedException> onException)
    {
        try { return new StreamReader(path); }
        catch (UnauthorizedAccessException exception) { throw onAccessDenied(path, exception); }
        catch (System.Security.SecurityException exception) { throw onAccessDenied(path, exception); }
        catch (ArgumentException exception) { throw onException(path, exception); }
        catch (FileNotFoundException exception) { throw onException(path, exception); }
        catch (DirectoryNotFoundException exception) { throw onException(path, exception); }
        catch (PathTooLongException exception) { throw onException(path, exception); }
        catch (IOException exception) { throw onException(path, exception); }
    }

    public static FileInfo GetUniqueFileInfo(DirectoryInfo basePath, string baseName, string extension, Func<string, Exception, LoggedException> onAccessDenied,
        Func<string, Exception, LoggedException> onException)
    {
        ArgumentNullException.ThrowIfNull(basePath);
        if (!basePath.Exists) throw new ArgumentException("Base path does not exist", nameof(basePath));
        ArgumentException.ThrowIfNullOrWhiteSpace(baseName);
        ArgumentNullException.ThrowIfNull(onAccessDenied);
        ArgumentNullException.ThrowIfNull(onException);
        if (extension is null)
            extension = "";
        if (extension.Length > 0 && extension[0] != '.')
            extension = $".{extension}";
        string path = baseName;
        try
        {
            path = Path.Combine(basePath.FullName, baseName + extension);
            FileInfo result = new(path);
            path = result.FullName;
            if (!result.Exists && !Directory.Exists(path))
                return result;
            int index = 0;
            do
            {
                path = Path.Combine(basePath.FullName, $"{baseName}{++index}{extension}");
                path = (result = new(path)).FullName;
            }
            while (result.Exists || Directory.Exists(path));
            return result;
        }
        catch (UnauthorizedAccessException exception) { throw onAccessDenied(path, exception); }
        catch (System.Security.SecurityException exception) { throw onAccessDenied(path, exception); }
        catch (PathTooLongException exception) { throw onException(path, exception); }
        catch (NotSupportedException exception) { throw onException(path, exception); }
        catch (ArgumentException exception) { throw onException(path, exception); }
    }

    public static FileInfo GetFileInfo(string path, Func<string, Exception, LoggedException> onAccessDenied, Func<string, Exception, LoggedException> onException, Func<string, LoggedException> onPathIsDirectory, Func<string, LoggedException> onDirectoryNotFound)
    {
        try
        {
            FileInfo result = new(path);
            path = result.FullName;
            if (result.Exists)
                return result;
            if (Directory.Exists(path))
                throw onPathIsDirectory(path);
            if (result.Directory is not null && result.Directory.Exists)
                return result;
        }
        catch (UnauthorizedAccessException exception) { throw onAccessDenied(path, exception); }
        catch (System.Security.SecurityException exception) { throw onAccessDenied(path, exception); }
        catch (PathTooLongException exception) { throw onException(path, exception); }
        catch (NotSupportedException exception) { throw onException(path, exception); }
        catch (ArgumentException exception) { throw onException(path, exception); }
        throw onDirectoryNotFound(path);
    }

    public static FileInfo GetExistingFileInfo(string path, Func<string, Exception, LoggedException> onAccessDenied, Func<string, Exception, LoggedException> onException, Func<string, LoggedException> onPathIsDirectory, Func<string, LoggedException> onFileNotFound)
    {
        try
        {
            FileInfo result = new(path);
            path = result.FullName;
            if (result.Exists)
                return result;
            if (Directory.Exists(path))
                throw onPathIsDirectory(path);
        }
        catch (UnauthorizedAccessException exception) { throw onAccessDenied(path, exception); }
        catch (System.Security.SecurityException exception) { throw onAccessDenied(path, exception); }
        catch (PathTooLongException exception) { throw onException(path, exception); }
        catch (NotSupportedException exception) { throw onException(path, exception); }
        catch (ArgumentException exception) { throw onException(path, exception); }
        throw onFileNotFound(path);
    }

    public static DirectoryInfo GetDirectoryInfo(string path, Func<string, Exception, LoggedException> onAccessDenied, Func<string, Exception, LoggedException> onException, Func<string, LoggedException> onPathIsFile, Func<string, LoggedException> onDirectoryNotFound)
    {
        try
        {
            DirectoryInfo result = new(path);
            path = result.FullName;
            if (result.Exists)
                return result;
            if (File.Exists(path))
                throw onPathIsFile(path);
            if (result.Parent is not null && result.Parent.Exists)
                return result;
        }
        catch (UnauthorizedAccessException exception) { throw onAccessDenied(path, exception); }
        catch (System.Security.SecurityException exception) { throw onAccessDenied(path, exception); }
        catch (PathTooLongException exception) { throw onException(path, exception); }
        catch (NotSupportedException exception) { throw onException(path, exception); }
        catch (ArgumentException exception) { throw onException(path, exception); }
        throw onDirectoryNotFound(path);
    }

    public static DirectoryInfo GetExistingDirectoryInfo(string path, Func<string, Exception, LoggedException> onAccessDenied, Func<string, Exception, LoggedException> onException, Func<string, LoggedException> onPathIsFile, Func<string, LoggedException> onDirectoryNotFound)
    {
        try
        {
            DirectoryInfo result = new(path);
            path = result.FullName;
            if (result.Exists)
                return result;
            if (File.Exists(path))
                throw onPathIsFile(path);
        }
        catch (UnauthorizedAccessException exception) { throw onAccessDenied(path, exception); }
        catch (System.Security.SecurityException exception) { throw onAccessDenied(path, exception); }
        catch (PathTooLongException exception) { throw onException(path, exception); }
        catch (NotSupportedException exception) { throw onException(path, exception); }
        catch (ArgumentException exception) { throw onException(path, exception); }
        throw onDirectoryNotFound(path);
    }

    public static FileSystemInfo GetFileOrDirectoryInfo(string path, Func<string, Exception, LoggedException> onAccessDenied, Func<string, Exception, LoggedException> onException, Func<string, LoggedException> onDirectoryNotFound)
    {
        try
        {
            FileInfo fileInfo = new(path);
            path = fileInfo.FullName;
            if (fileInfo.Exists)
                return fileInfo;
            DirectoryInfo directoryInfo = new(path);
            path = directoryInfo.FullName;
            if (directoryInfo.Exists || (directoryInfo.Parent is not null && directoryInfo.Parent.Exists))
                return directoryInfo;
        }
        catch (UnauthorizedAccessException exception) { throw onAccessDenied(path, exception); }
        catch (System.Security.SecurityException exception) { throw onAccessDenied(path, exception); }
        catch (PathTooLongException exception) { throw onException(path, exception); }
        catch (NotSupportedException exception) { throw onException(path, exception); }
        catch (ArgumentException exception) { throw onException(path, exception); }
        throw onDirectoryNotFound(path);
    }

    public static FileSystemInfo GetExistingFileOrDirectoryInfo(string path, Func<string, Exception, LoggedException> onAccessDenied, Func<string, Exception, LoggedException> onException, Func<string, LoggedException> onNotFound)
    {
        try
        {
            FileInfo fileInfo = new(path);
            path = fileInfo.FullName;
            if (fileInfo.Exists)
                return fileInfo;
            DirectoryInfo directoryInfo = new(path);
            path = directoryInfo.FullName;
            if (directoryInfo.Exists)
                return directoryInfo;
        }
        catch (UnauthorizedAccessException exception) { throw onAccessDenied(path, exception); }
        catch (System.Security.SecurityException exception) { throw onAccessDenied(path, exception); }
        catch (PathTooLongException exception) { throw onException(path, exception); }
        catch (NotSupportedException exception) { throw onException(path, exception); }
        catch (ArgumentException exception) { throw onException(path, exception); }
        throw onNotFound(path);
    }
}
