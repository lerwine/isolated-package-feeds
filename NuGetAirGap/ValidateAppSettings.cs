using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NuGetAirGap;

public partial class ValidateAppSettings : IValidateOptions<AppSettings>
{
    private readonly ILogger<ValidateAppSettings> _logger;

    public ValidateAppSettings(ILogger<ValidateAppSettings> logger) => _logger = logger;

    private ValidateOptionsResult ValidateUpstreamServiceIndexOld(AppSettings options)
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
            return ValidateOptionsResult.Fail(_logger.LogInvalidRepositoryUrl(options.UpstreamServiceIndex, true, error));
        }
        catch (ArgumentException error)
        {
            return ValidateOptionsResult.Fail(_logger.LogInvalidRepositoryUrl(options.UpstreamServiceIndex, true, error));
        }
        if (uri.IsAbsoluteUri)
        {
            if (!uri.IsFile)
            {
                options.UpstreamServiceIndex = uri.AbsoluteUri;
                return ValidateOptionsResult.Success;
            }
            options.UpstreamServiceIndex = uri.LocalPath;
            try { options.UpstreamServiceIndex = (directoryInfo = new(options.UpstreamServiceIndex)).FullName; }
            catch (System.Security.SecurityException error)
            {
                return ValidateOptionsResult.Fail(_logger.LogRepositorySecurityException(options.UpstreamServiceIndex, true, error));
            }
            catch (PathTooLongException error)
            {
                return ValidateOptionsResult.Fail(_logger.LogInvalidRepositoryUrl(uri, true, error));
            }
            catch (ArgumentException error)
            {
                return ValidateOptionsResult.Fail(_logger.LogInvalidRepositoryUrl(uri, true, error));
            }
        }
        else
        {
            if (Path.GetInvalidPathChars().Any(c => options.UpstreamServiceIndex.Contains(c)))
                return ValidateOptionsResult.Fail(_logger.LogInvalidRepositoryUrl(uri, true));
            try { options.UpstreamServiceIndex = (directoryInfo = new(options.UpstreamServiceIndex)).FullName; }
            catch (System.Security.SecurityException error)
            {
                return ValidateOptionsResult.Fail(_logger.LogRepositorySecurityException(options.UpstreamServiceIndex, true, error));
            }
            catch (PathTooLongException error)
            {
                return ValidateOptionsResult.Fail(_logger.LogInvalidRepositoryUrl(uri, true, error));
            }
            catch (ArgumentException error)
            {
                return ValidateOptionsResult.Fail(_logger.LogInvalidRepositoryUrl(uri, true, error));
            }
        }
        if (directoryInfo.Exists)
            return ValidateOptionsResult.Success;
        return ValidateOptionsResult.Fail((uri.IsAbsoluteUri && uri.IsFile) ? _logger.LogRepositoryPathNotFound(options.UpstreamServiceIndex, true) :
            _logger.LogInvalidRepositoryUrl(uri, true));
    }

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

    private ValidateOptionsResult ValidateLocalRepositoryOld(AppSettings options)
    {
        Uri? uri;
        DirectoryInfo directoryInfo;
        try
        {
            if (!Uri.TryCreate(options.LocalRepository, UriKind.Absolute, out uri))
                uri = new Uri(options.LocalRepository, UriKind.Relative);
        }
        catch (UriFormatException error)
        {
            return ValidateOptionsResult.Fail(_logger.LogInvalidRepositoryUrl(options.LocalRepository, false, error));
        }
        catch (ArgumentException error)
        {
            return ValidateOptionsResult.Fail(_logger.LogInvalidRepositoryUrl(options.LocalRepository, false, error));
        }
        if (uri.IsAbsoluteUri)
        {
            if (!uri.IsFile)
                return ValidateOptionsResult.Fail(_logger.LogInvalidRepositoryUrl(uri, false));
            options.LocalRepository = uri.LocalPath;
        }
        try { options.LocalRepository = (directoryInfo = new(options.LocalRepository)).FullName; }
        catch (System.Security.SecurityException error)
        {
            return ValidateOptionsResult.Fail(_logger.LogRepositorySecurityException(options.LocalRepository, false, error));
        }
        catch (PathTooLongException error)
        {
            return ValidateOptionsResult.Fail(_logger.LogInvalidRepositoryUrl(options.LocalRepository, false, error));
        }
        catch (ArgumentException error)
        {
            return ValidateOptionsResult.Fail(_logger.LogInvalidRepositoryUrl(options.LocalRepository, false, error));
        }
        if (!directoryInfo.Exists)
        {
            if (directoryInfo.Parent is not null && directoryInfo.Parent.Exists && !File.Exists(directoryInfo.FullName))
                try { directoryInfo.Create(); }
                catch (DirectoryNotFoundException exception)
                {
                    return ValidateOptionsResult.Fail(_logger.LogRepositoryPathNotFound(options.LocalRepository, false, exception));
                }
                catch (IOException exception)
                {
                    return ValidateOptionsResult.Fail(_logger.LogLocalRepositoryIOException(options.LocalRepository, exception));
                }
                catch (System.Security.SecurityException exception)
                {
                    return ValidateOptionsResult.Fail(_logger.LogRepositorySecurityException(options.LocalRepository, false, exception));
                }
            else
            {
                return ValidateOptionsResult.Fail(_logger.LogRepositoryPathNotFound(options.LocalRepository, false));
            }
        }
        return ValidateOptionsResult.Success;
    }
    
    private ValidationResult? ValidateLocalRepository(AppSettings options)
    {
        Uri? uri;
        DirectoryInfo directoryInfo;
        try
        {
            if (!Uri.TryCreate(options.LocalRepository, UriKind.Absolute, out uri))
                uri = new Uri(options.LocalRepository, UriKind.Relative);
        }
        catch (UriFormatException error)
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.LocalRepository, false, error), Enumerable.Repeat(nameof(AppSettings.LocalRepository), 1));
        }
        catch (ArgumentException error)
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.LocalRepository, false, error), Enumerable.Repeat(nameof(AppSettings.LocalRepository), 1));
        }
        if (uri.IsAbsoluteUri)
        {
            if (!uri.IsFile)
                return new ValidationResult(_logger.LogInvalidRepositoryUrl(uri, false), Enumerable.Repeat(nameof(AppSettings.LocalRepository), 1));
            options.LocalRepository = uri.LocalPath;
        }
        try { options.LocalRepository = (directoryInfo = new(options.LocalRepository)).FullName; }
        catch (System.Security.SecurityException error)
        {
            return new ValidationResult(_logger.LogRepositorySecurityException(options.LocalRepository, false, error), Enumerable.Repeat(nameof(AppSettings.LocalRepository), 1));
        }
        catch (PathTooLongException error)
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.LocalRepository, false, error), Enumerable.Repeat(nameof(AppSettings.LocalRepository), 1));
        }
        catch (ArgumentException error)
        {
            return new ValidationResult(_logger.LogInvalidRepositoryUrl(options.LocalRepository, false, error), Enumerable.Repeat(nameof(AppSettings.LocalRepository), 1));
        }
        if (!directoryInfo.Exists)
        {
            if (directoryInfo.Parent is not null && directoryInfo.Parent.Exists && !File.Exists(directoryInfo.FullName))
                try { directoryInfo.Create(); }
                catch (DirectoryNotFoundException exception)
                {
                    return new ValidationResult(_logger.LogRepositoryPathNotFound(options.LocalRepository, false, exception), Enumerable.Repeat(nameof(AppSettings.LocalRepository), 1));
                }
                catch (IOException exception)
                {
                    return new ValidationResult(_logger.LogLocalRepositoryIOException(options.LocalRepository, exception), Enumerable.Repeat(nameof(AppSettings.LocalRepository), 1));
                }
                catch (System.Security.SecurityException exception)
                {
                    return new ValidationResult(_logger.LogRepositorySecurityException(options.LocalRepository, false, exception), Enumerable.Repeat(nameof(AppSettings.LocalRepository), 1));
                }
            else
            {
                return new ValidationResult(_logger.LogRepositoryPathNotFound(options.LocalRepository, false), Enumerable.Repeat(nameof(AppSettings.LocalRepository), 1));
            }
        }
        return null;
    }
    
    private ValidateOptionsResult ValidateExportLocalMetaDataOld(AppSettings options)
    {
        if (string.IsNullOrWhiteSpace(options.ExportLocalMetaData))
        {
            options.ExportLocalMetaData = null;
            return ValidateOptionsResult.Success;
        }
        FileInfo fileInfo;
        try { options.ExportLocalMetaData = (fileInfo = new(options.ExportLocalMetaData)).FullName; }
        catch (System.Security.SecurityException error)
        {
            return ValidateOptionsResult.Fail(_logger.LogMetaDataExportPathAccessDenied(options.ExportLocalMetaData, error));
        }
        catch (PathTooLongException error)
        {
            return ValidateOptionsResult.Fail(_logger.LogInvalidExportLocalMetaData(options.ExportLocalMetaData, error));
        }
        catch (ArgumentException error)
        {
            return ValidateOptionsResult.Fail(_logger.LogInvalidExportLocalMetaData(options.ExportLocalMetaData, error));
        }
        if (fileInfo.Exists || (fileInfo.Directory is not null && fileInfo.Directory.Exists))
            return ValidateOptionsResult.Success;
        return ValidateOptionsResult.Fail(_logger.LogExportLocalMetaDataDirectoryNotFound(options.ExportLocalMetaData));
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

    private ValidateOptionsResult ValidateGlobalPackagesFolderOld(AppSettings options)
    {
        DirectoryInfo directoryInfo;
        try { options.GlobalPackagesFolder = (directoryInfo = new(options.GlobalPackagesFolder)).FullName; }
        catch (System.Security.SecurityException error)
        {
            return ValidateOptionsResult.Fail(_logger.LogGlobalPackagesFolderSecurityException(options.GlobalPackagesFolder, error));
        }
        catch (PathTooLongException error)
        {
            return ValidateOptionsResult.Fail(_logger.LogInvalidGlobalPackagesFolder(options.GlobalPackagesFolder, error));
        }
        catch (ArgumentException error)
        {
            return ValidateOptionsResult.Fail(_logger.LogInvalidGlobalPackagesFolder(options.GlobalPackagesFolder, error));
        }
        if (directoryInfo.Exists)
            return ValidateOptionsResult.Success;
        return ValidateOptionsResult.Fail(_logger.LogGlobalPackagesFolderNotFound(options.GlobalPackagesFolder));
    }

    private ValidationResult? ValidateGlobalPackagesFolder(AppSettings options)
    {
        DirectoryInfo directoryInfo;
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
