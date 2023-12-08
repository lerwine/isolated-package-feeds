using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static NuGetPuller.CommonStatic;

namespace NuGetPuller;

public abstract class SharedAppSettingsValidatorService<TSettings, TValidated> : IValidateOptions<TSettings>
    where TSettings : class, ISharedAppSettings
    where TValidated : class, IValidatedSharedAppSettings
{
    private readonly TValidated _validatedSettings;

    protected ILogger<SharedAppSettingsValidatorService<TSettings, TValidated>> Logger { get; }

    protected IHostEnvironment HostEnvironment { get; }

    protected SharedAppSettingsValidatorService(ILogger<SharedAppSettingsValidatorService<TSettings, TValidated>> logger, TValidated validatedSettings, IHostEnvironment hostEnvironment) => (Logger, HostEnvironment, _validatedSettings) = (logger, hostEnvironment, validatedSettings);

    private bool CheckUpstreamServiceIndex(TSettings options, [NotNullWhen(true)] out ValidationResult? validationResult)
    {
        var path = options.OverrideUpstreamServiceIndex.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(ISharedAppSettings.OverrideUpstreamServiceIndex), Directory.GetCurrentDirectory()), options.UpstreamServiceIndex, () => (nameof(ISharedAppSettings.UpstreamServiceIndex), HostEnvironment.ContentRootPath), out var opt);
        try
        {
            if (ResourceLocatorUtil.TryParseHttpOrFileAsDirectoryInfo(opt.BasePath, path, out Uri absoluteUri, out DirectoryInfo? directory))
            {
                if (directory is not null && !directory.Exists)
                {
                    _validatedSettings.UpstreamServiceIndex = absoluteUri;
                    validationResult = new ValidationResult(Logger.LogRepositoryPathNotFound(_validatedSettings.UpstreamServiceIndex.AbsoluteUri, true));
                    return true;
                }
            }
            _validatedSettings.UpstreamServiceIndex = absoluteUri;
            validationResult = null;
            return false;
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(Logger.LogRepositorySecurityException(_validatedSettings.UpstreamServiceIndex.AbsoluteUri, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(Logger.LogInvalidRepositoryUrl(_validatedSettings.UpstreamServiceIndex.AbsoluteUri, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(Logger.LogInvalidRepositoryUrl(_validatedSettings.UpstreamServiceIndex.AbsoluteUri, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(Logger.LogInvalidRepositoryUrl(_validatedSettings.UpstreamServiceIndex.AbsoluteUri, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        return true;

    }

    private bool CheckLocalRepository(TSettings options, [NotNullWhen(true)] out ValidationResult? validationResult)
    {
        var path = options.OverrideLocalRepository.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(ISharedAppSettings.OverrideLocalRepository), Directory.GetCurrentDirectory()), options.LocalRepository, () => (nameof(ISharedAppSettings.LocalRepository), HostEnvironment.ContentRootPath), out var opt);
        try
        {
            var directoryInfo = ResourceLocatorUtil.GetDirectoryInfo(opt.BasePath, path);
            path = (_validatedSettings.LocalRepository = directoryInfo).FullName;
            if (_validatedSettings.LocalRepository.Exists)
            {
                if (_validatedSettings.LocalRepository.Parent is null || File.Exists(path) || !_validatedSettings.LocalRepository.Parent.Exists)
                {
                    validationResult = new ValidationResult(Logger.LogRepositoryPathNotFound(path, false), Enumerable.Repeat(opt.SettingName, 1));
                    return true;
                }
            }
            else
                _validatedSettings.LocalRepository.Create();
            validationResult = null;
            return false;
        }
        catch (DirectoryNotFoundException exception) // Parent directory not found.
        {
            validationResult = new ValidationResult(Logger.LogRepositoryPathNotFound(path, false, exception), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(Logger.LogUnsupportedRepositoryUrlScheme(path, false, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(Logger.LogRepositorySecurityException(path, false, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(Logger.LogInvalidRepositoryUrl(path, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (IOException exception) // Error creating folder.
        {
            validationResult = new ValidationResult(Logger.LogLocalRepositoryIOException(path, exception), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(Logger.LogInvalidRepositoryUrl(path, false, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        return true;
    }

    private bool CheckGlobalPackagesFolder(TSettings options, [NotNullWhen(true)] out ValidationResult? validationResult)
    {
        var path = options.OverrideGlobalPackagesFolder.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(ISharedAppSettings.OverrideGlobalPackagesFolder), Directory.GetCurrentDirectory()), options.GlobalPackagesFolder, () => (nameof(ISharedAppSettings.GlobalPackagesFolder), HostEnvironment.ContentRootPath), out var opt);
        try
        {
            path = (_validatedSettings.GlobalPackagesFolder = ResourceLocatorUtil.GetDirectoryInfo(opt.BasePath, path)).FullName;
            if (_validatedSettings.GlobalPackagesFolder.Exists)
            {
                validationResult = null;
                return false;
            }
            validationResult = new ValidationResult(Logger.LogGlobalPackagesFolderNotFound(path), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(Logger.LogGlobalPackagesFolderSecurityException(path, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(Logger.LogGlobalPackagesFolderNotFileUri(path, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(Logger.LogInvalidGlobalPackagesFolder(path, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(Logger.LogInvalidGlobalPackagesFolder(path, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        return true;
    }

    protected abstract void Validate(TSettings settings, TValidated validatedSettings, List<ValidationResult> validationResults);

    public ValidateOptionsResult Validate(string? name, TSettings settings)
    {
        var validationResults = new List<ValidationResult>();
        if (CheckUpstreamServiceIndex(settings, out ValidationResult? validationResult))
            validationResults.Add(validationResult);
        if (CheckLocalRepository(settings, out validationResult))
            validationResults.Add(validationResult);
        if (CheckGlobalPackagesFolder(settings, out validationResult))
            validationResults.Add(validationResult);
        if (validationResults.Count == 0)
        {
            if (_validatedSettings.UpstreamServiceIndex.IsFile && NoCaseComparer.Equals(_validatedSettings.LocalRepository.FullName, _validatedSettings.UpstreamServiceIndex.LocalPath))
                validationResults.Add(new ValidationResult(Logger.LogLocalSameAsUpstreamNugetRepository(_validatedSettings.LocalRepository.FullName), new string[] { nameof(ISharedAppSettings.LocalRepository), nameof(ISharedAppSettings.UpstreamServiceIndex) }));
            else if (NoCaseComparer.Equals(_validatedSettings.LocalRepository.FullName, _validatedSettings.GlobalPackagesFolder.FullName))
                validationResults.Add(new ValidationResult(Logger.LogLocalRepositorySameAsGlobalPackagesFolder(_validatedSettings.LocalRepository.FullName), new string[] { nameof(ISharedAppSettings.LocalRepository), nameof(ISharedAppSettings.GlobalPackagesFolder) }));
            else if (_validatedSettings.UpstreamServiceIndex.IsFile && NoCaseComparer.Equals(_validatedSettings.UpstreamServiceIndex.LocalPath, _validatedSettings.GlobalPackagesFolder))
                validationResults.Add(new ValidationResult(Logger.LogUpstreamRepositorySameAsGlobalPackagesFolder(_validatedSettings.GlobalPackagesFolder.FullName), new string[] { nameof(ISharedAppSettings.UpstreamServiceIndex), nameof(ISharedAppSettings.GlobalPackagesFolder) }));
        }
        Validate(settings, _validatedSettings, validationResults);
        return (validationResults.Count == 0) ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(validationResults.Select(r => r.ToString()));
    }
}
