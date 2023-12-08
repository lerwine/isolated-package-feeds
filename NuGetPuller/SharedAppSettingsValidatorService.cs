using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static NuGetPuller.CommonStatic;

namespace NuGetPuller;

public abstract class SharedAppSettingsValidatorService<T> : IValidateOptions<T> where T : class, ISharedAppSettings
{
    protected ILogger<SharedAppSettingsValidatorService<T>> Logger { get; }

    protected IHostEnvironment HostEnvironment { get; }

    protected SharedAppSettingsValidatorService(ILogger<SharedAppSettingsValidatorService<T>> logger, IHostEnvironment hostEnvironment) => (Logger, HostEnvironment) = (logger, hostEnvironment);

    private bool CheckUpstreamServiceIndex(T options, [NotNullWhen(true)] out ValidationResult? validationResult)
    {
        var value = options.OverrideUpstreamServiceIndex.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(ISharedAppSettings.OverrideUpstreamServiceIndex), Directory.GetCurrentDirectory()), options.UpstreamServiceIndex, () => (nameof(ISharedAppSettings.UpstreamServiceIndex), HostEnvironment.ContentRootPath), out var opt);
        try
        {
            if (ResourceLocatorUtil.TryParseHttpOrFileAsDirectoryInfo(opt.BasePath, value, out Uri absoluteUri, out DirectoryInfo? directory))
            {
                if (directory is not null && !directory.Exists)
                {
                    options.Validated.UpstreamServiceIndex = absoluteUri;
                    validationResult = new ValidationResult(Logger.LogRepositoryPathNotFound(options.Validated.UpstreamServiceIndex.AbsoluteUri, true));
                    return true;
                }
            }
            options.Validated.UpstreamServiceIndex = absoluteUri;
            validationResult = null;
            return false;
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(Logger.LogRepositorySecurityException(options.Validated.UpstreamServiceIndex.AbsoluteUri, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(Logger.LogInvalidRepositoryUrl(options.Validated.UpstreamServiceIndex.AbsoluteUri, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(Logger.LogInvalidRepositoryUrl(options.Validated.UpstreamServiceIndex.AbsoluteUri, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(Logger.LogInvalidRepositoryUrl(options.Validated.UpstreamServiceIndex.AbsoluteUri, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        return true;

    }

    private bool CheckLocalRepository(T options, [NotNullWhen(true)] out ValidationResult? validationResult)
    {
        var path = options.OverrideLocalRepository.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(ISharedAppSettings.OverrideLocalRepository), Directory.GetCurrentDirectory()), options.LocalRepository, () => (nameof(ISharedAppSettings.LocalRepository), HostEnvironment.ContentRootPath), out var opt);
        try
        {
            options.Validated.LocalRepository = ResourceLocatorUtil.GetDirectoryInfo(opt.BasePath, path);
            if (options.Validated.LocalRepository.Exists)
            {
                if (options.Validated.LocalRepository.Parent is null || File.Exists(options.Validated.LocalRepository.FullName) || !options.Validated.LocalRepository.Parent.Exists)
                {
                    validationResult = new ValidationResult(Logger.LogRepositoryPathNotFound(options.Validated.LocalRepository.FullName, false), Enumerable.Repeat(opt.SettingName, 1));
                    return true;
                }
            }
            else
                options.Validated.LocalRepository.Create();
            validationResult = null;
            return false;
        }
        catch (DirectoryNotFoundException exception) // Parent directory not found.
        {
            validationResult = new ValidationResult(Logger.LogRepositoryPathNotFound(options.Validated.LocalRepository.FullName, false, exception), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(Logger.LogUnsupportedRepositoryUrlScheme(options.Validated.LocalRepository.FullName, false, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(Logger.LogRepositorySecurityException(options.Validated.LocalRepository.FullName, false, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(Logger.LogInvalidRepositoryUrl(options.Validated.LocalRepository.FullName, true, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (IOException exception) // Error creating folder.
        {
            validationResult = new ValidationResult(Logger.LogLocalRepositoryIOException(options.Validated.LocalRepository.FullName, exception), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(Logger.LogInvalidRepositoryUrl(options.Validated.LocalRepository.FullName, false, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        return true;
    }

    private bool CheckGlobalPackagesFolder(T options, [NotNullWhen(true)] out ValidationResult? validationResult)
    {
        var path = options.OverrideGlobalPackagesFolder.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(ISharedAppSettings.OverrideGlobalPackagesFolder), Directory.GetCurrentDirectory()), options.GlobalPackagesFolder, () => (nameof(ISharedAppSettings.GlobalPackagesFolder), HostEnvironment.ContentRootPath), out var opt);
        try
        {
            options.Validated.GlobalPackagesFolder = ResourceLocatorUtil.GetDirectoryInfo(opt.BasePath, path);
            if (options.Validated.GlobalPackagesFolder.Exists)
            {
                validationResult = null;
                return false;
            }
            validationResult = new ValidationResult(Logger.LogGlobalPackagesFolderNotFound(options.Validated.GlobalPackagesFolder.FullName), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(Logger.LogGlobalPackagesFolderSecurityException(options.Validated.GlobalPackagesFolder.FullName, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(Logger.LogGlobalPackagesFolderNotFileUri(options.Validated.GlobalPackagesFolder.FullName, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(Logger.LogInvalidGlobalPackagesFolder(options.Validated.GlobalPackagesFolder.FullName, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(Logger.LogInvalidGlobalPackagesFolder(options.Validated.GlobalPackagesFolder.FullName, error), Enumerable.Repeat(opt.SettingName, 1));
        }
        return true;
    }

    protected abstract void Validate(T options, List<ValidationResult> validationResults);

    public ValidateOptionsResult Validate(string? name, T options)
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
            if (options.Validated.UpstreamServiceIndex.IsFile && NoCaseComparer.Equals(options.Validated.LocalRepository.FullName, options.Validated.UpstreamServiceIndex.LocalPath))
                validationResults.Add(new ValidationResult(Logger.LogLocalSameAsUpstreamNugetRepository(options.Validated.LocalRepository.FullName), new string[] { nameof(ISharedAppSettings.LocalRepository), nameof(ISharedAppSettings.UpstreamServiceIndex) }));
            else if (NoCaseComparer.Equals(options.Validated.LocalRepository.FullName, options.Validated.GlobalPackagesFolder.FullName))
                validationResults.Add(new ValidationResult(Logger.LogLocalRepositorySameAsGlobalPackagesFolder(options.Validated.LocalRepository.FullName), new string[] { nameof(ISharedAppSettings.LocalRepository), nameof(ISharedAppSettings.GlobalPackagesFolder) }));
            else if (options.Validated.UpstreamServiceIndex.IsFile && NoCaseComparer.Equals(options.Validated.UpstreamServiceIndex.LocalPath, options.Validated.GlobalPackagesFolder))
                validationResults.Add(new ValidationResult(Logger.LogUpstreamRepositorySameAsGlobalPackagesFolder(options.Validated.GlobalPackagesFolder.FullName), new string[] { nameof(ISharedAppSettings.UpstreamServiceIndex), nameof(ISharedAppSettings.GlobalPackagesFolder) }));
        }
        Validate(options, validationResults);
        return (validationResults.Count == 0) ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(validationResults.Select(r => r.ToString()));
    }
}
