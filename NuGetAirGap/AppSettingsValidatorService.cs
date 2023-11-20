using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NuGetAirGap;

public partial class AppSettingsValidatorService : IValidateOptions<AppSettings>
{
    private readonly ILogger<AppSettingsValidatorService> _logger;
    private readonly HostingEnvironment _hostingEnvironment;
    

    public AppSettingsValidatorService(ILogger<AppSettingsValidatorService> logger, HostingEnvironment hostingEnvironment) => (_logger, _hostingEnvironment) = (logger, hostingEnvironment);

    private ValidationResult? ValidateUpstreamServiceIndex(AppSettings options)
    {
        string settingsValue = options.OverrideUpstreamServiceIndex.DefaultIfWhiteSpace(
            ifPrimaryValue: () => (SettingName: nameof(AppSettings.OverrideUpstreamServiceIndex), BasePath: Environment.CurrentDirectory),
            defaultValue: options.UpstreamServiceIndex,
            ifDefaultValue: () => (SettingName: nameof(AppSettings.UpstreamServiceIndex), BasePath: _hostingEnvironment.ContentRootPath),
            result: out (string SettingName, string BasePath) opt);
        try
        {
            if (ResourceLocatorUtil.TryParseHttpOrFileAsDirectoryInfo(opt.BasePath, settingsValue, out Uri absoluteUri, out DirectoryInfo? directory))
            {
                options.UpstreamServiceIndex = directory.FullName;
                if (!directory.Exists)
                    return new ValidationResult(_logger.LogRepositoryPathNotFound(options.UpstreamServiceIndex, true));

            }
            else
                options.UpstreamServiceIndex = absoluteUri.AbsoluteUri;
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            return new ValidationResult(_logger.LogRepositorySecurityException(settingsValue, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
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
        return null;
    }

    private ValidationResult? ValidateLocalRepository(AppSettings options)
    {
        string settingsValue = options.OverrideLocalRepository.DefaultIfWhiteSpace(
            ifPrimaryValue: () => (SettingName: nameof(AppSettings.OverrideLocalRepository), BasePath: Environment.CurrentDirectory),
            defaultValue: options.LocalRepository,
            ifDefaultValue: () => (SettingName: nameof(AppSettings.LocalRepository), BasePath: _hostingEnvironment.ContentRootPath),
            result: out (string SettingName, string BasePath) opt);
        try
        {
            DirectoryInfo directoryInfo = ResourceLocatorUtil.GetDirectoryInfo(opt.BasePath, settingsValue);
            options.LocalRepository = settingsValue = directoryInfo.FullName;
            if (directoryInfo.Exists)
                return null;
            if (directoryInfo.Parent is null || File.Exists(settingsValue) || !directoryInfo.Parent.Exists)
                return new ValidationResult(_logger.LogRepositoryPathNotFound(settingsValue, false), Enumerable.Repeat(opt.SettingName, 1));
            directoryInfo.Create();
            return null;
        }
        catch (DirectoryNotFoundException exception) // Parent directory not found.
        {
            return new ValidationResult(_logger.LogRepositoryPathNotFound(settingsValue, false, exception), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            return new ValidationResult(_logger.LogUnsupportedRepositoryUrlScheme(settingsValue, false, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            return new ValidationResult(_logger.LogRepositorySecurityException(settingsValue, false, error), Enumerable.Repeat(opt.SettingName, 1));
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
        try
        {
            FileInfo fileInfo = ResourceLocatorUtil.GetFileInfo(Environment.CurrentDirectory, options.ExportLocalMetaData);
            options.ExportLocalMetaData = fileInfo.FullName;
            if (!fileInfo.Exists && (fileInfo.Directory is null || !fileInfo.Directory.Exists))
                return new ValidationResult(_logger.LogExportLocalMetaDataDirectoryNotFound(options.ExportLocalMetaData), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            return new ValidationResult(_logger.LogMetaDataExportPathAccessDenied(options.ExportLocalMetaData, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            return new ValidationResult(_logger.LogInvalidExportLocalMetaData(options.ExportLocalMetaData, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        catch (DirectoryNotFoundException error) // Path is a URI, but not an https, http or file.
        {
            return new ValidationResult(_logger.LogExportLocalMetaDataDirectoryNotFound(options.ExportLocalMetaData, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            return new ValidationResult(_logger.LogInvalidExportLocalMetaData(options.ExportLocalMetaData, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            return new ValidationResult(_logger.LogInvalidExportLocalMetaData(options.ExportLocalMetaData, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        return null;
    }

    private ValidationResult? ValidateGlobalPackagesFolder(AppSettings options)
    {
        string settingsValue = options.OverrideGlobalPackagesFolder.DefaultIfWhiteSpace(
            ifPrimaryValue: () => (SettingName: nameof(AppSettings.OverrideGlobalPackagesFolder), BasePath: Environment.CurrentDirectory),
            defaultValue: options.GlobalPackagesFolder,
            ifDefaultValue: () => (SettingName: nameof(AppSettings.GlobalPackagesFolder), BasePath: _hostingEnvironment.ContentRootPath),
            result: out (string SettingName, string BasePath) opt);
        try
        {
            DirectoryInfo directoryInfo = ResourceLocatorUtil.GetDirectoryInfo(opt.BasePath, settingsValue);
            options.GlobalPackagesFolder = settingsValue = directoryInfo.FullName;
            if (!directoryInfo.Exists)
                return new ValidationResult(_logger.LogGlobalPackagesFolderNotFound(settingsValue), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            return new ValidationResult(_logger.LogGlobalPackagesFolderSecurityException(settingsValue, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            return new ValidationResult(_logger.LogGlobalPackagesFolderNotFileUri(settingsValue, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            return new ValidationResult(_logger.LogInvalidGlobalPackagesFolder(settingsValue, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            return new ValidationResult(_logger.LogInvalidGlobalPackagesFolder(settingsValue, error), Enumerable.Repeat(opt.SettingName, 1));
        }
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
        if (validationResults.Count == 0)
        {
            if (StringComparer.CurrentCultureIgnoreCase.Equals(options.LocalRepository, options.UpstreamServiceIndex))
                validationResults.Add(new ValidationResult(_logger.LogLocalSameAsUpstreamNugetRepository(options.LocalRepository), new string[] { nameof(AppSettings.LocalRepository), nameof(AppSettings.UpstreamServiceIndex) }));
            else if (StringComparer.CurrentCultureIgnoreCase.Equals(options.LocalRepository, options.GlobalPackagesFolder))
                validationResults.Add(new ValidationResult(_logger.LogLocalRepositorySameAsGlobalPackagesFolder(options.LocalRepository), new string[] { nameof(AppSettings.LocalRepository), nameof(AppSettings.GlobalPackagesFolder) }));
            else if (StringComparer.CurrentCultureIgnoreCase.Equals(options.UpstreamServiceIndex, options.GlobalPackagesFolder))
                validationResults.Add(new ValidationResult(_logger.LogUpstreamRepositorySameAsGlobalPackagesFolder(options.LocalRepository), new string[] { nameof(AppSettings.UpstreamServiceIndex), nameof(AppSettings.GlobalPackagesFolder) }));
            else
                return ValidateOptionsResult.Success;
        }
        return ValidateOptionsResult.Fail(validationResults.Select(r => r.ToString()));
    }
}
