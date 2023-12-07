using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static NuGetPuller.Constants;

namespace NuGetPuller;

public partial class AppSettingsValidatorService : IValidateOptions<AppSettings>
{
    private readonly ILogger<AppSettingsValidatorService> _logger;
    private readonly IHostEnvironment _hostEnvironment;

    public AppSettingsValidatorService(ILogger<AppSettingsValidatorService> logger, IHostEnvironment hostEnvironment) => (_logger, _hostEnvironment) = (logger, hostEnvironment);

    private bool CheckUpstreamServiceIndex(AppSettings options, [NotNullWhen(true)] out ValidationResult? validationResult)
    {
        options.Validated.UpstreamServiceLocation = options.OverrideUpstreamServiceIndex.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(AppSettings.OverrideUpstreamServiceIndex), Directory.GetCurrentDirectory()), options.UpstreamServiceIndex, () => (nameof(AppSettings.UpstreamServiceIndex), _hostEnvironment.ContentRootPath), out var opt);
        try
        {
            if (ResourceLocatorUtil.TryParseHttpOrFileAsDirectoryInfo(opt.BasePath, options.Validated.UpstreamServiceLocation, out Uri absoluteUri, out DirectoryInfo? directory))
            {
                options.Validated.UpstreamServiceLocation = directory.FullName;
                if (!directory.Exists)
                {
                    validationResult = new ValidationResult(_logger.LogRepositoryPathNotFound(options.Validated.UpstreamServiceLocation, true));
                    return true;
                }
            }
            else
                options.Validated.UpstreamServiceLocation = absoluteUri.AbsoluteUri;
            options.Validated.UpstreamServiceUri = absoluteUri;
            validationResult = null;
            return false;
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(_logger.LogRepositorySecurityException(options.Validated.UpstreamServiceLocation, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(_logger.LogInvalidRepositoryUrl(options.Validated.UpstreamServiceLocation, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(_logger.LogInvalidRepositoryUrl(options.Validated.UpstreamServiceLocation, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(_logger.LogInvalidRepositoryUrl(options.Validated.UpstreamServiceLocation, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        return true;

    }

    private bool CheckLocalRepository(AppSettings options, [NotNullWhen(true)] out ValidationResult? validationResult)
    {
        options.Validated.LocalRepositoryPath = options.OverrideLocalRepository.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(AppSettings.OverrideLocalRepository), Directory.GetCurrentDirectory()), options.LocalRepository, () => (nameof(AppSettings.LocalRepository), _hostEnvironment.ContentRootPath), out var opt);
        try
        {
            DirectoryInfo directoryInfo = ResourceLocatorUtil.GetDirectoryInfo(opt.BasePath, options.Validated.LocalRepositoryPath);
            options.Validated.LocalRepositoryPath = directoryInfo.FullName;
            if (directoryInfo.Exists)
            {
                if (directoryInfo.Parent is null || File.Exists(options.Validated.LocalRepositoryPath) || !directoryInfo.Parent.Exists)
                {
                    validationResult = new ValidationResult(_logger.LogRepositoryPathNotFound(options.Validated.LocalRepositoryPath, false), Enumerable.Repeat(opt.SettingName, 1));
                    return true;
                }
            }
            else
                directoryInfo.Create();
            validationResult = null;
            return false;
        }
        catch (DirectoryNotFoundException exception) // Parent directory not found.
        {
            validationResult = new ValidationResult(_logger.LogRepositoryPathNotFound(options.Validated.LocalRepositoryPath, false, exception), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(_logger.LogUnsupportedRepositoryUrlScheme(options.Validated.LocalRepositoryPath, false, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(_logger.LogRepositorySecurityException(options.Validated.LocalRepositoryPath, false, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(_logger.LogInvalidRepositoryUrl(options.Validated.LocalRepositoryPath, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (IOException exception) // Error creating folder.
        {
            validationResult = new ValidationResult(_logger.LogLocalRepositoryIOException(options.Validated.LocalRepositoryPath, exception), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(_logger.LogInvalidRepositoryUrl(options.Validated.LocalRepositoryPath, false, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        return true;
    }

    private bool CheckExportLocalManifest(AppSettings options, [NotNullWhen(true)] out ValidationResult? validationResult)
    {
        options.Validated.ExportLocalMetaDataPath = options.ExportLocalManifest;
        if (string.IsNullOrWhiteSpace(options.Validated.ExportLocalMetaDataPath))
        {
            options.Validated.ExportLocalMetaDataPath = null;
            validationResult = null;
            return false;
        }
        try
        {
            FileInfo fileInfo = ResourceLocatorUtil.GetFileInfo(Directory.GetCurrentDirectory(), options.Validated.ExportLocalMetaDataPath);
            options.Validated.ExportLocalMetaDataPath = fileInfo.FullName;
            if (fileInfo.Exists || (fileInfo.Directory is not null && fileInfo.Directory.Exists))
            {
                validationResult = null;
                return false;
            }
            validationResult = new ValidationResult(_logger.LogExportLocalMetaDataDirectoryNotFound(options.Validated.ExportLocalMetaDataPath), Enumerable.Repeat(nameof(AppSettings.ExportLocalManifest), 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(_logger.LogMetaDataExportPathAccessDenied(options.Validated.ExportLocalMetaDataPath, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalManifest), 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(_logger.LogInvalidExportLocalMetaData(options.Validated.ExportLocalMetaDataPath, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalManifest), 1));
        }
        catch (DirectoryNotFoundException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(_logger.LogExportLocalMetaDataDirectoryNotFound(options.Validated.ExportLocalMetaDataPath, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalManifest), 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(_logger.LogInvalidExportLocalMetaData(options.Validated.ExportLocalMetaDataPath, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalManifest), 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(_logger.LogInvalidExportLocalMetaData(options.Validated.ExportLocalMetaDataPath, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalManifest), 1));
        }
        return true;
    }

    private bool CheckExportBundle(AppSettings options, [NotNullWhen(true)] out ValidationResult? validationResult)
    {
        options.Validated.ExportBundlePath = options.ExportBundle;
        if (string.IsNullOrWhiteSpace(options.Validated.ExportBundlePath))
        {
            options.Validated.ExportBundlePath = null;
            options.Validated.TargetManifestFilePath = string.Empty;
            options.Validated.TargetManifestSaveAsPath = string.Empty;
            validationResult = null;
            return false;
        }
        FileInfo fileInfo;
        try
        {
            fileInfo = ResourceLocatorUtil.GetFileInfo(Directory.GetCurrentDirectory(), options.Validated.ExportBundlePath);
            options.Validated.ExportBundlePath = fileInfo.FullName;
            if (!fileInfo.Exists && (fileInfo.Directory is null || !fileInfo.Directory.Exists))
            {
                validationResult = new ValidationResult(_logger.LogExportBundleDirectoryNotFound(options.Validated.ExportBundlePath), Enumerable.Repeat(nameof(AppSettings.ExportBundle), 1));
                return true;
            }
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(_logger.LogExportBundlePathAccessDenied(options.Validated.ExportBundlePath, error), Enumerable.Repeat(nameof(AppSettings.ExportBundle), 1));
            return true;
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(_logger.LogInvalidExportBundle(options.Validated.ExportBundlePath, error), Enumerable.Repeat(nameof(AppSettings.ExportBundle), 1));
            return true;
        }
        catch (DirectoryNotFoundException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(_logger.LogExportBundleDirectoryNotFound(options.Validated.ExportBundlePath, error), Enumerable.Repeat(nameof(AppSettings.ExportBundle), 1));
            return true;
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(_logger.LogInvalidExportBundle(options.Validated.ExportBundlePath, error), Enumerable.Repeat(nameof(AppSettings.ExportBundle), 1));
            return true;
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(_logger.LogInvalidExportBundle(options.Validated.ExportBundlePath, error), Enumerable.Repeat(nameof(AppSettings.ExportBundle), 1));
            return true;
        }
        try
        {
            if (string.IsNullOrWhiteSpace(options.TargetManifestFile))
            {
                options.Validated.TargetManifestFilePath = $"{Path.GetFileNameWithoutExtension(fileInfo.Name)}.json";
                if (fileInfo.Directory is not null)
                    options.Validated.TargetManifestFilePath = Path.Combine(fileInfo.DirectoryName!, options.Validated.TargetManifestFilePath);
                options.Validated.TargetManifestFilePath = new FileInfo(options.Validated.TargetManifestFilePath).FullName;
            }
            else
            {
                FileInfo f = ResourceLocatorUtil.GetFileInfo(Directory.GetCurrentDirectory(), options.Validated.TargetManifestFilePath);
                options.Validated.TargetManifestFilePath = f.FullName;
                if (!f.Exists && (f.Directory is null || !f.Directory.Exists))
                {
                    validationResult = new ValidationResult(_logger.LogTargetManifestFileDirectoryNotFound(options.Validated.TargetManifestFilePath), Enumerable.Repeat(nameof(AppSettings.TargetManifestFile), 1));
                    return true;
                }
            }
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(_logger.LogTargetManifestFilePathAccessDenied(options.Validated.TargetManifestFilePath, error), Enumerable.Repeat(nameof(AppSettings.TargetManifestFile), 1));
            return true;
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(_logger.LogInvalidTargetManifestFile(options.Validated.TargetManifestFilePath, error), Enumerable.Repeat(nameof(AppSettings.TargetManifestFile), 1));
            return true;
        }
        catch (DirectoryNotFoundException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(_logger.LogTargetManifestFileDirectoryNotFound(options.Validated.TargetManifestFilePath, error), Enumerable.Repeat(nameof(AppSettings.TargetManifestFile), 1));
            return true;
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(_logger.LogInvalidTargetManifestFile(options.Validated.TargetManifestFilePath, error), Enumerable.Repeat(nameof(AppSettings.TargetManifestFile), 1));
            return true;
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(_logger.LogInvalidTargetManifestFile(options.Validated.TargetManifestFilePath, error), Enumerable.Repeat(nameof(AppSettings.TargetManifestFile), 1));
            return true;
        }
        if (string.IsNullOrWhiteSpace(options.SaveTargetManifestAs))
        {
            options.Validated.TargetManifestSaveAsPath = options.Validated.TargetManifestFilePath;
            validationResult = null;
            return false;
        }
        try
        {
            FileInfo f = ResourceLocatorUtil.GetFileInfo(Directory.GetCurrentDirectory(), options.Validated.TargetManifestSaveAsPath);
            options.Validated.TargetManifestSaveAsPath = f.FullName;
            if (f.Exists || (f.Directory is not null && f.Directory.Exists))
            {
                validationResult = null;
                return false;
            }
            validationResult = new ValidationResult(_logger.LogSaveTargetManifestAsDirectoryNotFound(options.Validated.TargetManifestSaveAsPath), Enumerable.Repeat(nameof(AppSettings.SaveTargetManifestAs), 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(_logger.LogSaveTargetManifestAsPathAccessDenied(options.Validated.TargetManifestSaveAsPath, error), Enumerable.Repeat(nameof(AppSettings.SaveTargetManifestAs), 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(_logger.LogInvalidSaveTargetManifestAs(options.Validated.TargetManifestSaveAsPath, error), Enumerable.Repeat(nameof(AppSettings.SaveTargetManifestAs), 1));
        }
        catch (DirectoryNotFoundException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(_logger.LogSaveTargetManifestAsDirectoryNotFound(options.Validated.TargetManifestSaveAsPath, error), Enumerable.Repeat(nameof(AppSettings.SaveTargetManifestAs), 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(_logger.LogInvalidSaveTargetManifestAs(options.Validated.TargetManifestSaveAsPath, error), Enumerable.Repeat(nameof(AppSettings.SaveTargetManifestAs), 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(_logger.LogInvalidSaveTargetManifestAs(options.Validated.TargetManifestSaveAsPath, error), Enumerable.Repeat(nameof(AppSettings.SaveTargetManifestAs), 1));
        }
        return true;
    }

    private bool CheckImport(AppSettings options, [NotNullWhen(true)] out ValidationResult? validationResult)
    {
        options.Validated.ImportPath = options.Import;
        if (string.IsNullOrWhiteSpace(options.Validated.ImportPath))
        {
            options.Validated.ImportPath = null;
            validationResult = null;
            return false;
        }
        try
        {
            FileSystemInfo fileSystemInfo = ResourceLocatorUtil.GetFileOrDirectory(Directory.GetCurrentDirectory(), options.Validated.ImportPath);
            options.Validated.ImportPath = fileSystemInfo.FullName;
            if (fileSystemInfo.Exists)
            {
                validationResult = null;
                return false;
            }
            validationResult = new ValidationResult(_logger.LogImportFileOrDirectoryNotFound(options.Validated.ImportPath), Enumerable.Repeat(nameof(AppSettings.Import), 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(_logger.LogImportPathAccessDenied(options.Validated.ImportPath, error), Enumerable.Repeat(nameof(AppSettings.Import), 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(_logger.LogInvalidImportPath(options.Validated.ImportPath, error), Enumerable.Repeat(nameof(AppSettings.Import), 1));
        }
        catch (DirectoryNotFoundException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(_logger.LogImportFileOrDirectoryNotFound(options.Validated.ImportPath, error), Enumerable.Repeat(nameof(AppSettings.Import), 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(_logger.LogInvalidImportPath(options.Validated.ImportPath, error), Enumerable.Repeat(nameof(AppSettings.Import), 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(_logger.LogInvalidImportPath(options.Validated.ImportPath, error), Enumerable.Repeat(nameof(AppSettings.Import), 1));
        }
        return true;
    }

    private bool CheckGlobalPackagesFolder(AppSettings options, [NotNullWhen(true)] out ValidationResult? validationResult)
    {
        options.Validated.GlobalPackagesFolderPath = options.OverrideGlobalPackagesFolder.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(AppSettings.OverrideGlobalPackagesFolder), Directory.GetCurrentDirectory()), options.GlobalPackagesFolder, () => (nameof(AppSettings.GlobalPackagesFolder), _hostEnvironment.ContentRootPath), out var opt);
        try
        {
            DirectoryInfo directoryInfo = ResourceLocatorUtil.GetDirectoryInfo(opt.BasePath, options.Validated.GlobalPackagesFolderPath);
            options.Validated.GlobalPackagesFolderPath = directoryInfo.FullName;
            if (directoryInfo.Exists)
            {
                validationResult = null;
                return false;
            }
            validationResult = new ValidationResult(_logger.LogGlobalPackagesFolderNotFound(options.Validated.GlobalPackagesFolderPath), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(_logger.LogGlobalPackagesFolderSecurityException(options.Validated.GlobalPackagesFolderPath, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(_logger.LogGlobalPackagesFolderNotFileUri(options.Validated.GlobalPackagesFolderPath, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(_logger.LogInvalidGlobalPackagesFolder(options.Validated.GlobalPackagesFolderPath, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(_logger.LogInvalidGlobalPackagesFolder(options.Validated.GlobalPackagesFolderPath, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        return true;
    }

    public ValidateOptionsResult Validate(string? name, AppSettings options)
    {
        var validationResults = new List<ValidationResult>();
        if (CheckUpstreamServiceIndex(options, out ValidationResult? validationResult))
            validationResults.Add(validationResult);
        if (CheckLocalRepository(options, out validationResult))
            validationResults.Add(validationResult);
        if (CheckGlobalPackagesFolder(options, out validationResult))
            validationResults.Add(validationResult);
        if (validationResults.Count == 0)
        {
            if (NoCaseComparer.Equals(options.Validated.LocalRepositoryPath, options.Validated.UpstreamServiceLocation))
                validationResults.Add(new ValidationResult(_logger.LogLocalSameAsUpstreamNugetRepository(options.Validated.LocalRepositoryPath), new string[] { nameof(AppSettings.LocalRepository), nameof(AppSettings.UpstreamServiceIndex) }));
            else if (NoCaseComparer.Equals(options.Validated.LocalRepositoryPath, options.Validated.GlobalPackagesFolderPath))
                validationResults.Add(new ValidationResult(_logger.LogLocalRepositorySameAsGlobalPackagesFolder(options.Validated.LocalRepositoryPath), new string[] { nameof(AppSettings.LocalRepository), nameof(AppSettings.GlobalPackagesFolder) }));
            else if (NoCaseComparer.Equals(options.Validated.UpstreamServiceLocation, options.Validated.GlobalPackagesFolderPath))
                validationResults.Add(new ValidationResult(_logger.LogUpstreamRepositorySameAsGlobalPackagesFolder(options.Validated.LocalRepositoryPath), new string[] { nameof(AppSettings.UpstreamServiceIndex), nameof(AppSettings.GlobalPackagesFolder) }));
        }
        if (CheckExportLocalManifest(options, out validationResult))
            validationResults.Add(validationResult);
        if (CheckExportBundle(options, out validationResult))
            validationResults.Add(validationResult);
        if (CheckImport(options, out validationResult))
            validationResults.Add(validationResult);
        else if (validationResults.Count == 0)
            return ValidateOptionsResult.Success;
        return ValidateOptionsResult.Fail(validationResults.Select(r => r.ToString()));
    }
}