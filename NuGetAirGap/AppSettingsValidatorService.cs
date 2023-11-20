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
        options.Validated.UpstreamServiceLocation = options.OverrideUpstreamServiceIndex.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(AppSettings.OverrideUpstreamServiceIndex), Environment.CurrentDirectory), options.UpstreamServiceIndex, () => (nameof(AppSettings.UpstreamServiceIndex), _hostingEnvironment.ContentRootPath), out var opt);
        try
        {
            if (ResourceLocatorUtil.TryParseHttpOrFileAsDirectoryInfo(opt.BasePath, options.Validated.UpstreamServiceLocation, out Uri absoluteUri, out DirectoryInfo? directory))
            {
                options.Validated.UpstreamServiceLocation = directory.FullName;
                if (!directory.Exists)
                    return new ValidationResult(_logger.LogRepositoryPathNotFound(options.UpstreamServiceIndex, true));
            }
            else
                options.Validated.UpstreamServiceLocation = absoluteUri.AbsoluteUri;
            options.Validated.UpstreamServiceUri = absoluteUri;
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            return new ValidationResult(_logger.LogRepositorySecurityException(options.Validated.UpstreamServiceLocation, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.Validated.UpstreamServiceLocation, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.Validated.UpstreamServiceLocation, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.Validated.UpstreamServiceLocation, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        return null;
    }

    private ValidationResult? ValidateLocalRepository(AppSettings options)
    {
        options.Validated.LocalRepositoryPath = options.OverrideLocalRepository.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(AppSettings.OverrideLocalRepository), Environment.CurrentDirectory), options.LocalRepository, () => (nameof(AppSettings.LocalRepository), _hostingEnvironment.ContentRootPath), out var opt);
        try
        {
            DirectoryInfo directoryInfo = ResourceLocatorUtil.GetDirectoryInfo(opt.BasePath, options.Validated.LocalRepositoryPath);
            options.Validated.LocalRepositoryPath = directoryInfo.FullName;
            if (directoryInfo.Exists)
                return null;
            if (directoryInfo.Parent is null || File.Exists(options.Validated.LocalRepositoryPath) || !directoryInfo.Parent.Exists)
                return new ValidationResult(_logger.LogRepositoryPathNotFound(options.Validated.LocalRepositoryPath, false), Enumerable.Repeat(opt.SettingName, 1));
            directoryInfo.Create();
            return null;
        }
        catch (DirectoryNotFoundException exception) // Parent directory not found.
        {
            return new ValidationResult(_logger.LogRepositoryPathNotFound(options.Validated.LocalRepositoryPath, false, exception), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            return new ValidationResult(_logger.LogUnsupportedRepositoryUrlScheme(options.Validated.LocalRepositoryPath, false, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            return new ValidationResult(_logger.LogRepositorySecurityException(options.Validated.LocalRepositoryPath, false, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.Validated.LocalRepositoryPath, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (IOException exception) // Error creating folder.
        {
            return new ValidationResult(_logger.LogLocalRepositoryIOException(options.Validated.LocalRepositoryPath, exception), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.Validated.LocalRepositoryPath, false, error), Enumerable.Repeat(opt.SettingName, 1));
        }
    }

    private ValidationResult? ValidateExportLocalMetaData(AppSettings options)
    {
        options.Validated.ExportLocalMetaDataPath = options.ExportLocalMetaData;
        if (string.IsNullOrWhiteSpace(options.Validated.ExportLocalMetaDataPath))
        {
            options.Validated.ExportLocalMetaDataPath = null;
            return null;
        }
        try
        {
            FileInfo fileInfo = ResourceLocatorUtil.GetFileInfo(Environment.CurrentDirectory, options.Validated.ExportLocalMetaDataPath);
            options.Validated.ExportLocalMetaDataPath = fileInfo.FullName;
            if (!fileInfo.Exists && (fileInfo.Directory is null || !fileInfo.Directory.Exists))
                return new ValidationResult(_logger.LogExportLocalMetaDataDirectoryNotFound(options.Validated.ExportLocalMetaDataPath), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            return new ValidationResult(_logger.LogMetaDataExportPathAccessDenied(options.Validated.ExportLocalMetaDataPath, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            return new ValidationResult(_logger.LogInvalidExportLocalMetaData(options.Validated.ExportLocalMetaDataPath, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        catch (DirectoryNotFoundException error) // Path is a URI, but not an https, http or file.
        {
            return new ValidationResult(_logger.LogExportLocalMetaDataDirectoryNotFound(options.Validated.ExportLocalMetaDataPath, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            return new ValidationResult(_logger.LogInvalidExportLocalMetaData(options.Validated.ExportLocalMetaDataPath, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            return new ValidationResult(_logger.LogInvalidExportLocalMetaData(options.Validated.ExportLocalMetaDataPath, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalMetaData), 1));
        }
        return null;
    }

    private ValidationResult? ValidateGlobalPackagesFolder(AppSettings options)
    {
        options.Validated.GlobalPackagesFolderPath = options.OverrideGlobalPackagesFolder.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(AppSettings.OverrideGlobalPackagesFolder), Environment.CurrentDirectory), options.GlobalPackagesFolder, () => (nameof(AppSettings.GlobalPackagesFolder), _hostingEnvironment.ContentRootPath), out var opt);
        try
        {
            DirectoryInfo directoryInfo = ResourceLocatorUtil.GetDirectoryInfo(opt.BasePath, options.Validated.GlobalPackagesFolderPath);
            options.Validated.GlobalPackagesFolderPath = directoryInfo.FullName;
            if (!directoryInfo.Exists)
                return new ValidationResult(_logger.LogGlobalPackagesFolderNotFound(options.Validated.GlobalPackagesFolderPath), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            return new ValidationResult(_logger.LogGlobalPackagesFolderSecurityException(options.Validated.GlobalPackagesFolderPath, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            return new ValidationResult(_logger.LogGlobalPackagesFolderNotFileUri(options.Validated.GlobalPackagesFolderPath, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            return new ValidationResult(_logger.LogInvalidGlobalPackagesFolder(options.Validated.GlobalPackagesFolderPath, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            return new ValidationResult(_logger.LogInvalidGlobalPackagesFolder(options.Validated.GlobalPackagesFolderPath, error), Enumerable.Repeat(opt.SettingName, 1));
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
            if (StringComparer.CurrentCultureIgnoreCase.Equals(options.Validated.LocalRepositoryPath, options.Validated.UpstreamServiceLocation))
                validationResults.Add(new ValidationResult(_logger.LogLocalSameAsUpstreamNugetRepository(options.Validated.LocalRepositoryPath), new string[] { nameof(AppSettings.LocalRepository), nameof(AppSettings.UpstreamServiceIndex) }));
            else if (StringComparer.CurrentCultureIgnoreCase.Equals(options.Validated.LocalRepositoryPath, options.Validated.GlobalPackagesFolderPath))
                validationResults.Add(new ValidationResult(_logger.LogLocalRepositorySameAsGlobalPackagesFolder(options.Validated.LocalRepositoryPath), new string[] { nameof(AppSettings.LocalRepository), nameof(AppSettings.GlobalPackagesFolder) }));
            else if (StringComparer.CurrentCultureIgnoreCase.Equals(options.Validated.UpstreamServiceLocation, options.Validated.GlobalPackagesFolderPath))
                validationResults.Add(new ValidationResult(_logger.LogUpstreamRepositorySameAsGlobalPackagesFolder(options.Validated.LocalRepositoryPath), new string[] { nameof(AppSettings.UpstreamServiceIndex), nameof(AppSettings.GlobalPackagesFolder) }));
            else
                return ValidateOptionsResult.Success;
        }
        return ValidateOptionsResult.Fail(validationResults.Select(r => r.ToString()));
    }
}