using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static NuGetPuller.CommonStatic;

namespace NuGetPuller;

public abstract class SharedAppSettingsValidatorService<TSettings, TValidated> : IValidateOptions<TSettings>
    where TSettings : class, ISharedAppSettings
    where TValidated : class, IValidatedSharedAppSettings, new()
{
    public TValidated Validated { get; } = new();

    protected ILogger<SharedAppSettingsValidatorService<TSettings, TValidated>> Logger { get; }

    protected IHostEnvironment HostEnvironment { get; }

    protected SharedAppSettingsValidatorService(ILogger<SharedAppSettingsValidatorService<TSettings, TValidated>> logger, IHostEnvironment hostEnvironment) => (Logger, HostEnvironment) = (logger, hostEnvironment);

    private bool CheckUpstreamServiceIndex(TSettings options, [NotNullWhen(true)] out ValidationResult? validationResult)
    {
        var path = options.OverrideUpstreamServiceIndex.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(ISharedAppSettings.OverrideUpstreamServiceIndex), Directory.GetCurrentDirectory()), options.UpstreamServiceIndex, () => (nameof(ISharedAppSettings.UpstreamServiceIndex), HostEnvironment.ContentRootPath), out var opt);
        try
        {
            if (ResourceLocatorUtil.TryParseHttpOrFileAsDirectoryInfo(opt.BasePath, path, out Uri absoluteUri, out DirectoryInfo? directory))
            {
                if (directory is not null && !directory.Exists)
                {
                    Validated.UpstreamServiceIndex = absoluteUri;
                    validationResult = new ValidationResult(Logger.LogRepositoryPathNotFound(Validated.UpstreamServiceIndex.AbsoluteUri, true));
                    return true;
                }
            }
            Validated.UpstreamServiceIndex = absoluteUri;
            validationResult = null;
            return false;
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(Logger.LogRepositorySecurityException(Validated.UpstreamServiceIndex.AbsoluteUri, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(Logger.LogInvalidRepositoryUrl(Validated.UpstreamServiceIndex.AbsoluteUri, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(Logger.LogInvalidRepositoryUrl(Validated.UpstreamServiceIndex.AbsoluteUri, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(Logger.LogInvalidRepositoryUrl(Validated.UpstreamServiceIndex.AbsoluteUri, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        return true;

    }

    private bool CheckLocalRepository(TSettings options, [NotNullWhen(true)] out ValidationResult? validationResult)
    {
        var path = options.OverrideLocalRepository.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(ISharedAppSettings.OverrideLocalRepository), Directory.GetCurrentDirectory()), options.LocalRepository, () => (nameof(ISharedAppSettings.LocalRepository), HostEnvironment.ContentRootPath), out var opt);
        try
        {
            var directoryInfo = ResourceLocatorUtil.GetDirectoryInfo(opt.BasePath, path);
            path = (Validated.LocalRepository = directoryInfo).FullName;
            if (Validated.LocalRepository.Exists)
            {
                if (Validated.LocalRepository.Parent is null || File.Exists(path) || !Validated.LocalRepository.Parent.Exists)
                {
                    validationResult = new ValidationResult(Logger.LogRepositoryPathNotFound(path, false), Enumerable.Repeat(opt.SettingName, 1));
                    return true;
                }
            }
            else
                Validated.LocalRepository.Create();
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
            path = (Validated.GlobalPackagesFolder = ResourceLocatorUtil.GetDirectoryInfo(opt.BasePath, path)).FullName;
            if (Validated.GlobalPackagesFolder.Exists)
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

    protected abstract void Validate(TSettings settings, List<ValidationResult> validationResults);

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
            if (Validated.UpstreamServiceIndex.IsFile && NoCaseComparer.Equals(Validated.LocalRepository.FullName, Validated.UpstreamServiceIndex.LocalPath))
                validationResults.Add(new ValidationResult(Logger.LogLocalSameAsUpstreamNugetRepository(Validated.LocalRepository.FullName), new string[] { nameof(ISharedAppSettings.LocalRepository), nameof(ISharedAppSettings.UpstreamServiceIndex) }));
            else if (NoCaseComparer.Equals(Validated.LocalRepository.FullName, Validated.GlobalPackagesFolder.FullName))
                validationResults.Add(new ValidationResult(Logger.LogLocalRepositorySameAsGlobalPackagesFolder(Validated.LocalRepository.FullName), new string[] { nameof(ISharedAppSettings.LocalRepository), nameof(ISharedAppSettings.GlobalPackagesFolder) }));
            else if (Validated.UpstreamServiceIndex.IsFile && NoCaseComparer.Equals(Validated.UpstreamServiceIndex.LocalPath, Validated.GlobalPackagesFolder))
                validationResults.Add(new ValidationResult(Logger.LogUpstreamRepositorySameAsGlobalPackagesFolder(Validated.GlobalPackagesFolder.FullName), new string[] { nameof(ISharedAppSettings.UpstreamServiceIndex), nameof(ISharedAppSettings.GlobalPackagesFolder) }));
        }
        Validate(settings, validationResults);
        return (validationResults.Count == 0) ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(validationResults.Select(r => r.ToString()));
    }
}
