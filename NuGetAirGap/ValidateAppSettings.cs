using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NuGetAirGap;

public partial class ValidateAppSettings : IValidateOptions<AppSettings>
{
    public const string SCHEME_START_FILE = "file://";
    
    private readonly ILogger<ValidateAppSettings> _logger;
    private readonly HostingEnvironment _hostingEnvironment;

    public ValidateAppSettings(ILogger<ValidateAppSettings> logger, HostingEnvironment hostingEnvironment) => (_logger, _hostingEnvironment) = (logger, hostingEnvironment);

    private static string? GetFileOrHttpUri(Uri uri, out bool isUnsupportedScheme, out Uri? result)
    {
        if (uri.IsFile)
        {
            isUnsupportedScheme = false;
            result = uri;
            return uri.LocalPath;
        }
        isUnsupportedScheme = uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp;
        result = isUnsupportedScheme ? null : (uri.AbsoluteUri != uri.OriginalString) ? new(uri.AbsoluteUri, UriKind.Absolute) : uri;
        return null;
    }

    /// <summary>
    /// Attempts to parse a string value as an absolute filesystem path or as a URI with the <c>https</c> or <c>https</c> scheme.
    /// </summary>
    /// <param name="uriOrPath">The string value to parse.</param>
    /// <param name="basePath">The base filesystem path for any relative path.</param>
    /// <param name="isUnsupportedScheme">Returns <see langword="true"/> if the <paramref name="uriOrPath"/> parameter contains a URI string that has neither the <c>file</c>, <c>https</c> nor the <c>https</c> scheme; otherwise, <see langword="false"/>.</param>
    /// <param name="uri">The <paramref name="uriOrPath"/> parsed as an absolute <see cref="Uri"/> or <see langword="null"/> if it couldn't be parsed an absolute <see cref="Uri"/> or the scheme was not <c>https</c>, <c>http</c>, or <c>file</c>.</param>
    /// <returns>The absolute filesystem path or <see langword="null"/> if <paramref name="overrideValue"/>/<paramref name="settingValue"/> does not represent a filesystem path.</returns>
    /// <exception cref="ArgumentException"><paramref name="overrideValue"/> or <paramref name="settingValue"/> contains invalid characters or system could not retrieve the absolute path.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permissions to the path specified <paramref name="overrideValue"/> or <paramref name="settingValue"/>.</exception>
    /// <exception cref="NotSupportedException"><paramref name="overrideValue"/> or <paramref name="settingValue"/> contains a colon (":") that is not part of a volume identifier.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length for <paramref name="overrideValue"/> or <paramref name="settingValue"/>.</exception>
    internal static string? TryParseAbsoluteFileOrHttpUri(string uriOrPath, string? basePath, out bool isUnsupportedScheme, out Uri? uri)
    {
        if (string.IsNullOrWhiteSpace(uriOrPath))
        {
            isUnsupportedScheme = false;
            uri = null;
            return null;
        }
        uriOrPath = Environment.ExpandEnvironmentVariables(uriOrPath);
        if (Uri.TryCreate(uriOrPath, UriKind.Absolute, out uri))
            return GetFileOrHttpUri(uri, out isUnsupportedScheme, out uri);
        uriOrPath = Path.GetFullPath(Path.IsPathFullyQualified(uriOrPath) ? uriOrPath : string.IsNullOrEmpty(basePath) ? uriOrPath : Path.Combine(basePath, uriOrPath));
        isUnsupportedScheme = false;
        if (!Uri.TryCreate(uriOrPath, UriKind.Absolute, out uri))
            uri = null;
        return uriOrPath;
    }

    /// <summary>
    /// Attempts to get the absolute filesystem path of a setting/override value.
    /// </summary>
    /// <param name="uriOrPath">The string value to parse.</param>
    /// <param name="basePath">The base filesystem path for any relative path.</param>
    /// <param name="isUri">Returns <see langword="true"/> if <paramref name="uriOrPath"/> was successfully parsed as an absolute URI; otherwise, <see langword="false"/>.</param>
    /// <param name="localPath">Returns the absolute filesystem path or <see langword="null"/> if the input value was not a filesystem path.</param>
    /// <returns><see langword="true"/> if <paramref name="localPath"/> returns an absolute filesystem path; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="overrideValue"/> or <paramref name="uriOrPath"/> contains invalid characters or system could not retrieve the absolute path.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permissions to the path specified <paramref name="overrideValue"/> or <paramref name="uriOrPath"/>.</exception>
    /// <exception cref="NotSupportedException"><paramref name="overrideValue"/> or <paramref name="uriOrPath"/> contains a colon (":") that is not part of a volume identifier.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length for <paramref name="overrideValue"/>/<paramref name="uriOrPath"/>.</exception>
    internal static bool TryGetAbsoluteLocalPath(string? uriOrPath, string? basePath, out bool isUri, [NotNullWhen(true)] out string? localPath)
    {
        if (string.IsNullOrWhiteSpace(uriOrPath))
            isUri = false;
        else
        {
            uriOrPath = Environment.ExpandEnvironmentVariables(uriOrPath);
            if (Uri.TryCreate(uriOrPath, UriKind.Absolute, out Uri? uri))
            {
                isUri = true;
                if (uri.IsFile)
                {
                    localPath = uri.LocalPath;
                    return true;
                }
            }
            else
            {
                isUri = false;
                localPath = Path.GetFullPath(Path.IsPathFullyQualified(uriOrPath) ? uriOrPath : string.IsNullOrEmpty(basePath) ? uriOrPath : Path.Combine(basePath, uriOrPath));
                return true;
            }
        }
        localPath = null;
        return false;
    }
    
    private ValidationResult? ValidateUpstreamServiceIndex(AppSettings options)
    {
        string settingsValue = options.OverrideUpstreamServiceIndex.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(AppSettings.OverrideUpstreamServiceIndex), Environment.CurrentDirectory), options.UpstreamServiceIndex, () => (nameof(AppSettings.UpstreamServiceIndex), _hostingEnvironment.ContentRootPath), out var opt);
        DirectoryInfo directoryInfo;
        try
        {
            string? localPath = TryParseAbsoluteFileOrHttpUri(settingsValue, opt.BasePath, out bool isUnsupportedScheme, out Uri? uri);
            if (localPath is null)
            {
                if (uri is null)
                {
                    if (isUnsupportedScheme)
                        return new ValidationResult(_logger.LogUnsupportedRepositoryUrlScheme(settingsValue, true), Enumerable.Repeat(opt.SettingName, 1));
                    return new ValidationResult(_logger.LogInvalidRepositoryUrl(settingsValue, true), Enumerable.Repeat(opt.SettingName, 1));
                }
                options.UpstreamServiceIndex = uri.AbsoluteUri;
                return null;
            }
            options.UpstreamServiceIndex = (directoryInfo = new(settingsValue)).FullName;
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            return new ValidationResult(_logger.LogRepositorySecurityException(settingsValue, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (NotSupportedException error) // Path contains a colon (":") that is not part of a volume identifier.
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(settingsValue, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(settingsValue, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(settingsValue, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        if (directoryInfo.Exists)
            return null;
        return new ValidationResult(_logger.LogRepositoryPathNotFound(options.UpstreamServiceIndex, true));
    }

    private ValidationResult? ValidateLocalRepository(AppSettings options)
    {
        string settingsValue = options.OverrideLocalRepository.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(AppSettings.OverrideLocalRepository), Environment.CurrentDirectory), options.LocalRepository, () => (nameof(AppSettings.LocalRepository), _hostingEnvironment.ContentRootPath), out var opt);
        DirectoryInfo directoryInfo;
        try
        {
            if (TryGetAbsoluteLocalPath(settingsValue, opt.BasePath, out bool isUri, out string? localPath))
            {
                options.LocalRepository = settingsValue = (directoryInfo = new(localPath)).FullName;
                if (!directoryInfo.Exists)
                {
                    if (directoryInfo.Parent is not null && directoryInfo.Parent.Exists && !File.Exists(directoryInfo.FullName))
                        directoryInfo.Create();
                    else
                    {
                        return new ValidationResult(_logger.LogRepositoryPathNotFound(settingsValue, false), Enumerable.Repeat(opt.SettingName, 1));
                    }
                }
                return null;
            }
            if (isUri)
                return new ValidationResult(_logger.LogUnsupportedRepositoryUrlScheme(settingsValue, false), Enumerable.Repeat(opt.SettingName, 1));
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(settingsValue, false), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (DirectoryNotFoundException exception) // Parent directory not found.
        {
            return new ValidationResult(_logger.LogRepositoryPathNotFound(settingsValue, false, exception), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            return new ValidationResult(_logger.LogRepositorySecurityException(settingsValue, false, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (NotSupportedException error) // Path contains a colon (":") that is not part of a volume identifier.
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(settingsValue, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(settingsValue, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (IOException exception) // Error creating folder.
        {
            return new ValidationResult(_logger.LogLocalRepositoryIOException(settingsValue, exception), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(settingsValue, false, error), Enumerable.Repeat(opt.SettingName, 1));
        }
    }
    
    private ValidationResult? ValidateExportLocalMetaData(AppSettings options)
    {
        if (string.IsNullOrWhiteSpace(options.ExportLocalMetaData))
        {
            options.ExportLocalMetaData = null;
            return null;
        }
        FileInfo fileInfo;
        try { options.ExportLocalMetaData = (fileInfo = new(Environment.ExpandEnvironmentVariables(options.ExportLocalMetaData))).FullName; }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            return new ValidationResult(_logger.LogMetaDataExportPathAccessDenied(options.ExportLocalMetaData, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        catch (NotSupportedException error) // Path contains a colon (":") that is not part of a volume identifier.
        {
            return new ValidationResult(_logger.LogInvalidExportLocalMetaData(options.ExportLocalMetaData, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            return new ValidationResult(_logger.LogInvalidExportLocalMetaData(options.ExportLocalMetaData, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            return new ValidationResult(_logger.LogInvalidExportLocalMetaData(options.ExportLocalMetaData, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        if (fileInfo.Exists || (fileInfo.Directory is not null && fileInfo.Directory.Exists))
            return null;
        return new ValidationResult(_logger.LogExportLocalMetaDataDirectoryNotFound(options.ExportLocalMetaData), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
    }

    private ValidationResult? ValidateGlobalPackagesFolder(AppSettings options)
    {
        string settingsValue = options.OverrideGlobalPackagesFolder.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(AppSettings.OverrideGlobalPackagesFolder), Environment.CurrentDirectory), options.GlobalPackagesFolder, () => (nameof(AppSettings.GlobalPackagesFolder), _hostingEnvironment.ContentRootPath), out var opt);
        DirectoryInfo directoryInfo;
        try
        {
            if (TryGetAbsoluteLocalPath(settingsValue, opt.BasePath, out bool isUri, out string? localPath))
                options.GlobalPackagesFolder = settingsValue = (directoryInfo = new(settingsValue = localPath)).FullName;
            else
            {
                if (isUri)
                    return new ValidationResult(_logger.LogGlobalPackagesFolderNotFileUri(settingsValue), Enumerable.Repeat(opt.SettingName, 1));
                return new ValidationResult(_logger.LogInvalidGlobalPackagesFolder(settingsValue), Enumerable.Repeat(opt.SettingName, 1));
            }
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            return new ValidationResult(_logger.LogGlobalPackagesFolderSecurityException(settingsValue, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (NotSupportedException error) // Path contains a colon (":") that is not part of a volume identifier.
        {
            return new ValidationResult(_logger.LogInvalidGlobalPackagesFolder(settingsValue, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            return new ValidationResult(_logger.LogInvalidGlobalPackagesFolder(settingsValue, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            return new ValidationResult(_logger.LogInvalidGlobalPackagesFolder(settingsValue, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        if (!directoryInfo.Exists)
            return new ValidationResult(_logger.LogGlobalPackagesFolderNotFound(settingsValue), Enumerable.Repeat(opt.SettingName, 1));
        return null;
    }

    public ValidateOptionsResult Validate(string? name, AppSettings options)
    {
        var validationResults = new List<ValidationResult>();
        var validationResult = ValidateUpstreamServiceIndex(options);
        if (validationResult is not null)
            validationResults.Add(validationResult);
        if ((validationResult = ValidateLocalRepository(options)) is not null)
            validationResults.Add(validationResult);
        if ((validationResult = ValidateExportLocalMetaData(options)) is not null)
            validationResults.Add(validationResult);
        if ((validationResult = ValidateGlobalPackagesFolder(options)) is not null)
            validationResults.Add(validationResult);
        ValidateOptionsResult vr = (validationResults.Count > 0) ? ValidateOptionsResult.Fail(validationResults.Select(r => r.ToString())) : ValidateOptionsResult.Success;
        return vr;
    }
}
