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

    public string? TryParseAbsoluteFileOrHttpUri(string? settingValue, string primaryName, string? overrideValue, string overrideName,
        out bool isUnsupportedScheme, out string settingName, out Uri? uri)
    {
        if (string.IsNullOrWhiteSpace(overrideValue))
        {
            settingName = primaryName;
            if (string.IsNullOrWhiteSpace(settingValue))
            {
                isUnsupportedScheme = false;
                uri = null;
                return null;
            }
            settingValue = Environment.ExpandEnvironmentVariables(settingValue);
            if (Uri.TryCreate(settingValue, UriKind.Absolute, out uri))
            {
                if (uri.IsFile)
                {
                    isUnsupportedScheme = false;
                    return uri.LocalPath;
                }
                isUnsupportedScheme = uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp;
                if (isUnsupportedScheme)
                    uri = null;
                else if (uri.AbsoluteUri != uri.OriginalString)
                    uri = new(uri.AbsoluteUri, UriKind.Absolute);
                return null;
            }
            isUnsupportedScheme = false;
            if (Path.GetInvalidPathChars().Any(c => settingValue.Contains(c)))
                try
                {
                    string localPath = Path.GetFullPath(Path.Combine(_hostingEnvironment.ContentRootPath, settingValue));
                    if (!Uri.TryCreate(localPath, UriKind.Absolute, out uri))
                        uri = null;
                    return localPath;
                } catch { /* okay to ignore */ }
        }
        else
        {
            settingName = overrideName;
            overrideValue = Environment.ExpandEnvironmentVariables(overrideValue);
            if (Uri.TryCreate(overrideValue, UriKind.Absolute, out uri))
            {
                if (uri.IsFile)
                {
                    isUnsupportedScheme = false;
                    return uri.LocalPath;
                }
                isUnsupportedScheme = uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp;
                if (isUnsupportedScheme)
                    uri = null;
                else if (uri.AbsoluteUri != uri.OriginalString)
                    uri = new(uri.AbsoluteUri, UriKind.Absolute);
                return null;
            }
            isUnsupportedScheme = false;
            if (Path.GetInvalidPathChars().Any(c => overrideValue.Contains(c)))
                try
                {
                    string localPath = Path.GetFullPath(overrideValue);
                    if (!Uri.TryCreate(localPath, UriKind.Absolute, out uri))
                        uri = null;
                    return localPath;
                } catch { /* okay to ignore */ }
        }
        uri = null;
        return null;
    }

    public bool TryParseFullLocalPath(string? settingValue, string primaryName, string? overrideValue, string overrideName,
        out bool isUri, out string settingName, [NotNullWhen(true)] out string? localPath)
    {
        if (string.IsNullOrWhiteSpace(overrideValue))
        {
            settingName = primaryName;
            if (string.IsNullOrWhiteSpace(settingValue))
                isUri = false;
            else
            {
                settingValue = Environment.ExpandEnvironmentVariables(settingValue);
                if (Uri.TryCreate(settingValue, UriKind.Absolute, out Uri? uri))
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
                    if (!Path.GetInvalidPathChars().Any(c => settingValue.Contains(c)))
                    try
                    {
                        localPath = Path.GetFullPath(Path.IsPathFullyQualified(settingValue) ? settingValue : Path.Combine(_hostingEnvironment.ContentRootPath, settingValue));
                        return true;
                    }
                    catch { /* okay to ignore */ }
                }
            }
        }
        else
        {
            settingName = overrideName;
            overrideValue = Environment.ExpandEnvironmentVariables(overrideValue);
            if (Uri.TryCreate(overrideValue, UriKind.Absolute, out Uri? uri))
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
                if (!Path.GetInvalidPathChars().Any(c => overrideValue.Contains(c)))
                try
                {
                    localPath = Path.GetFullPath(overrideValue);
                    return true;
                }
                catch { /* okay to ignore */ }
            }
        }
        localPath = null;
        return false;
    }

    private ValidationResult? ValidateUpstreamServiceIndex(AppSettings options)
    {
        string? localPath = TryParseAbsoluteFileOrHttpUri(options.UpstreamServiceIndex, nameof(AppSettings.UpstreamServiceIndex), options.OverrideUpstreamServiceIndex,
            nameof(AppSettings.OverrideUpstreamServiceIndex), out bool isUnsupportedScheme, out string settingName, out Uri? uri);
        if (localPath is null)
        {
            if (uri is null)
            {
                if (isUnsupportedScheme)
                    return new ValidationResult(_logger.LogUnsupportedRepositoryUrlScheme(options.OverrideUpstreamServiceIndex.DefaultIfWhiteSpace(options.UpstreamServiceIndex), true));
                return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.OverrideUpstreamServiceIndex.DefaultIfWhiteSpace(options.UpstreamServiceIndex), true));
            }
            options.UpstreamServiceIndex = uri.AbsoluteUri;
            return null;
        }
        DirectoryInfo directoryInfo;
        try { options.UpstreamServiceIndex = (directoryInfo = new(options.UpstreamServiceIndex)).FullName; }
        catch (System.Security.SecurityException error)
        {
            return new ValidationResult(_logger.LogRepositorySecurityException(localPath, true, error), Enumerable.Repeat(nameof(AppSettings.UpstreamServiceIndex), 1));
        }
        catch (PathTooLongException error)
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(localPath, true, error), Enumerable.Repeat(nameof(AppSettings.UpstreamServiceIndex), 1));
        }
        catch (ArgumentException error)
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(localPath, true, error), Enumerable.Repeat(nameof(AppSettings.UpstreamServiceIndex), 1));
        }
        if (directoryInfo.Exists)
            return null;
        return new ValidationResult(_logger.LogRepositoryPathNotFound(options.UpstreamServiceIndex, true));
    }

    private ValidationResult? ValidateLocalRepository(AppSettings options)
    {
        if (TryParseFullLocalPath(options.LocalRepository, nameof(AppSettings.LocalRepository), options.OverrideLocalRepository, nameof(AppSettings.OverrideLocalRepository),
            out bool isUri, out string settingName, out string? localPath))
        {
            DirectoryInfo directoryInfo;
            try { options.LocalRepository = (directoryInfo = new(localPath)).FullName; }
            catch (System.Security.SecurityException error)
            {
                return new ValidationResult(_logger.LogRepositorySecurityException(options.LocalRepository, false, error), Enumerable.Repeat(settingName, 1));
            }
            catch (PathTooLongException error)
            {
                return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.LocalRepository, false, error), Enumerable.Repeat(settingName, 1));
            }
            catch (ArgumentException error)
            {
                return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.LocalRepository, false, error), Enumerable.Repeat(settingName, 1));
            }
            if (!directoryInfo.Exists)
            {
                if (directoryInfo.Parent is not null && directoryInfo.Parent.Exists && !File.Exists(directoryInfo.FullName))
                    try { directoryInfo.Create(); }
                    catch (DirectoryNotFoundException exception)
                    {
                        return new ValidationResult(_logger.LogRepositoryPathNotFound(options.LocalRepository, false, exception), Enumerable.Repeat(settingName, 1));
                    }
                    catch (IOException exception)
                    {
                        return new ValidationResult(_logger.LogLocalRepositoryIOException(options.LocalRepository, exception), Enumerable.Repeat(settingName, 1));
                    }
                    catch (System.Security.SecurityException exception)
                    {
                        return new ValidationResult(_logger.LogRepositorySecurityException(options.LocalRepository, false, exception), Enumerable.Repeat(settingName, 1));
                    }
                else
                {
                    return new ValidationResult(_logger.LogRepositoryPathNotFound(options.LocalRepository, false), Enumerable.Repeat(settingName, 1));
                }
            }
            return null;
        }
        if (isUri)
            return new ValidationResult(_logger.LogUnsupportedRepositoryUrlScheme(options.OverrideLocalRepository.DefaultIfWhiteSpace(options.LocalRepository), false), Enumerable.Repeat(settingName, 1));
        return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.OverrideLocalRepository.DefaultIfWhiteSpace(options.LocalRepository), false), Enumerable.Repeat(settingName, 1));
    }
    
    private ValidationResult? ValidateExportLocalMetaData(AppSettings options)
    {
        if (string.IsNullOrWhiteSpace(options.ExportLocalMetaData))
        {
            options.ExportLocalMetaData = null;
            return null;
        }
        FileInfo fileInfo;
        try { options.ExportLocalMetaData = (fileInfo = new(options.ExportLocalMetaData)).FullName; }
        catch (System.Security.SecurityException error)
        {
            return new ValidationResult(_logger.LogMetaDataExportPathAccessDenied(options.ExportLocalMetaData, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        catch (PathTooLongException error)
        {
            return new ValidationResult(_logger.LogInvalidExportLocalMetaData(options.ExportLocalMetaData, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        catch (ArgumentException error)
        {
            return new ValidationResult(_logger.LogInvalidExportLocalMetaData(options.ExportLocalMetaData, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        if (fileInfo.Exists || (fileInfo.Directory is not null && fileInfo.Directory.Exists))
            return null;
        return new ValidationResult(_logger.LogExportLocalMetaDataDirectoryNotFound(options.ExportLocalMetaData), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
    }

    private ValidationResult? ValidateGlobalPackagesFolder(AppSettings options)
    {
        DirectoryInfo directoryInfo;
        string settingsName;
        Uri? uri;
        if (string.IsNullOrEmpty(options.OverrideGlobalPackagesFolder))
        {
            settingsName = nameof(AppSettings.GlobalPackagesFolder);
            options.LocalRepository = Environment.ExpandEnvironmentVariables(options.LocalRepository);
            if (options.LocalRepository.StartsWith(SCHEME_START_FILE))
            {
                if (Uri.TryCreate(options.LocalRepository, UriKind.Absolute, out uri))
                {
                    if (!uri.IsFile)
                        return new ValidationResult(_logger.LogInvalidRepositoryUrl(uri, false), Enumerable.Repeat(settingsName, 1));
                    options.LocalRepository = uri.LocalPath;
                }
                else
                    return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.LocalRepository, false), Enumerable.Repeat(settingsName, 1));
            }
            else
                try
                {
                    if (!Path.IsPathFullyQualified(options.LocalRepository))
                        options.LocalRepository = Path.Combine(_hostingEnvironment.ContentRootPath, options.LocalRepository);
                }
                catch (ArgumentException exception)
                {
                    return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.LocalRepository, false, exception), Enumerable.Repeat(settingsName, 1));
                }
        }
        else
        {
            settingsName = nameof(AppSettings.OverrideGlobalPackagesFolder);

        }
        try { options.GlobalPackagesFolder = (directoryInfo = new(options.GlobalPackagesFolder)).FullName; }
        catch (System.Security.SecurityException error)
        {
            return new ValidationResult(_logger.LogGlobalPackagesFolderSecurityException(options.GlobalPackagesFolder, error), Enumerable.Repeat(nameof(AppSettings.GlobalPackagesFolder), 1));
        }
        catch (PathTooLongException error)
        {
            return new ValidationResult(_logger.LogInvalidGlobalPackagesFolder(options.GlobalPackagesFolder, error), Enumerable.Repeat(nameof(AppSettings.GlobalPackagesFolder), 1));
        }
        catch (ArgumentException error)
        {
            return new ValidationResult(_logger.LogInvalidGlobalPackagesFolder(options.GlobalPackagesFolder, error), Enumerable.Repeat(nameof(AppSettings.GlobalPackagesFolder), 1));
        }
        if (directoryInfo.Exists)
            return null;
        return new ValidationResult(_logger.LogGlobalPackagesFolderNotFound(options.GlobalPackagesFolder), Enumerable.Repeat(nameof(AppSettings.GlobalPackagesFolder), 1));
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
