using System.ComponentModel.DataAnnotations;
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

    private ValidationResult? ValidateUpstreamServiceIndex(AppSettings options)
    {
        Uri? uri;
        DirectoryInfo directoryInfo;
        try
        {
            if (!Uri.TryCreate(options.UpstreamServiceIndex, UriKind.Absolute, out uri))
                uri = new Uri(options.UpstreamServiceIndex, UriKind.Absolute);
        }
        catch (UriFormatException error)
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.UpstreamServiceIndex, true, error), Enumerable.Repeat(nameof(AppSettings.UpstreamServiceIndex), 1));
        }
        catch (ArgumentException error)
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.UpstreamServiceIndex, true, error), Enumerable.Repeat(nameof(AppSettings.UpstreamServiceIndex), 1));
        }
        if (uri.IsAbsoluteUri)
        {
            if (!uri.IsFile)
            {
                options.UpstreamServiceIndex = uri.AbsoluteUri;
                return null;
            }
            options.UpstreamServiceIndex = uri.LocalPath;
            try { options.UpstreamServiceIndex = (directoryInfo = new(options.UpstreamServiceIndex)).FullName; }
            catch (System.Security.SecurityException error)
            {
                return new ValidationResult(_logger.LogRepositorySecurityException(options.UpstreamServiceIndex, true, error), Enumerable.Repeat(nameof(AppSettings.UpstreamServiceIndex), 1));
            }
            catch (PathTooLongException error)
            {
                return new ValidationResult(_logger.LogInvalidRepositoryUrl(uri, true, error), Enumerable.Repeat(nameof(AppSettings.UpstreamServiceIndex), 1));
            }
            catch (ArgumentException error)
            {
                return new ValidationResult(_logger.LogInvalidRepositoryUrl(uri, true, error), Enumerable.Repeat(nameof(AppSettings.UpstreamServiceIndex), 1));
            }
        }
        else
        {
            if (Path.GetInvalidPathChars().Any(c => options.UpstreamServiceIndex.Contains(c)))
                return new ValidationResult(_logger.LogInvalidRepositoryUrl(uri, true), Enumerable.Repeat(nameof(AppSettings.UpstreamServiceIndex), 1));
            try { options.UpstreamServiceIndex = (directoryInfo = new(options.UpstreamServiceIndex)).FullName; }
            catch (System.Security.SecurityException error)
            {
                return new ValidationResult(_logger.LogRepositorySecurityException(options.UpstreamServiceIndex, true, error), Enumerable.Repeat(nameof(AppSettings.UpstreamServiceIndex), 1));
            }
            catch (PathTooLongException error)
            {
                return new ValidationResult(_logger.LogInvalidRepositoryUrl(uri, true, error), Enumerable.Repeat(nameof(AppSettings.UpstreamServiceIndex), 1));
            }
            catch (ArgumentException error)
            {
                return new ValidationResult(_logger.LogInvalidRepositoryUrl(uri, true, error), Enumerable.Repeat(nameof(AppSettings.UpstreamServiceIndex), 1));
            }
        }
        if (directoryInfo.Exists)
            return null;
        return new ValidationResult((uri.IsAbsoluteUri && uri.IsFile) ? _logger.LogRepositoryPathNotFound(options.UpstreamServiceIndex, true) :
            _logger.LogInvalidRepositoryUrl(uri, true), Enumerable.Repeat(nameof(AppSettings.UpstreamServiceIndex), 1));
    }

    private string ExpandLocalFile(string value, string overriddeValue, out bool isOverride, out bool isValid)
    {
        isOverride = !string.IsNullOrWhiteSpace(overriddeValue);
        if (isOverride)
            value = overriddeValue;
        if ((value = Environment.ExpandEnvironmentVariables(value)).StartsWith(SCHEME_START_FILE))
        {
            if (Uri.TryCreate(value, UriKind.Absolute, out Uri? uri))
            {
                isValid = uri.IsFile;
                return isValid ? uri.LocalPath : value;
            }
            isValid = false;
            return value;
        }
        isValid = true;
        return Path.IsPathFullyQualified(value) ? value : Path.Combine(_hostingEnvironment.ContentRootPath, value);
    }
    
    private ValidationResult? ValidateLocalRepository(AppSettings options)
    {
        options.LocalRepository = ExpandLocalFile(options.LocalRepository, options.OverrideLocalRepository, out bool isOverride, out bool isValid);
        string settingsName = isOverride ? nameof(AppSettings.OverrideLocalRepository) : nameof(AppSettings.LocalRepository);
        if (!isValid)
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.LocalRepository, false), Enumerable.Repeat(settingsName, 1));
        DirectoryInfo directoryInfo;
        try { options.LocalRepository = (directoryInfo = new(options.LocalRepository)).FullName; }
        catch (System.Security.SecurityException error)
        {
            return new ValidationResult(_logger.LogRepositorySecurityException(options.LocalRepository, false, error), Enumerable.Repeat(settingsName, 1));
        }
        catch (PathTooLongException error)
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.LocalRepository, false, error), Enumerable.Repeat(settingsName, 1));
        }
        catch (ArgumentException error)
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.LocalRepository, false, error), Enumerable.Repeat(settingsName, 1));
        }
        if (!directoryInfo.Exists)
        {
            if (directoryInfo.Parent is not null && directoryInfo.Parent.Exists && !File.Exists(directoryInfo.FullName))
                try { directoryInfo.Create(); }
                catch (DirectoryNotFoundException exception)
                {
                    return new ValidationResult(_logger.LogRepositoryPathNotFound(options.LocalRepository, false, exception), Enumerable.Repeat(settingsName, 1));
                }
                catch (IOException exception)
                {
                    return new ValidationResult(_logger.LogLocalRepositoryIOException(options.LocalRepository, exception), Enumerable.Repeat(settingsName, 1));
                }
                catch (System.Security.SecurityException exception)
                {
                    return new ValidationResult(_logger.LogRepositorySecurityException(options.LocalRepository, false, exception), Enumerable.Repeat(settingsName, 1));
                }
            else
            {
                return new ValidationResult(_logger.LogRepositoryPathNotFound(options.LocalRepository, false), Enumerable.Repeat(settingsName, 1));
            }
        }
        return null;
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
