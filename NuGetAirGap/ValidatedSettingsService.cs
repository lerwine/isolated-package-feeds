using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Protocol.Core.Types;

namespace NuGetAirGap;

public class ValidatedSettingsService
{
    private readonly IOptions<AppSettings> _settingsOptions;
    private readonly ILogger<AppSettingsValidatorService> _logger;
    private readonly HostingEnvironment _hostingEnvironment;
    private readonly object _syncRoot = new();
    private Task<bool>? _validateAsync;

    public Uri UpstreamRepositoryUrl { get; private set; } = null!;
    
    public DirectoryInfo? UpstreamRepository { get; private set; }

    public DirectoryInfo LocalRepository { get; private set; } = null!;

    public FileInfo ExportLocalMetaData { get; private set; } = null!;

    public DirectoryInfo GlobalPackagesFolder { get; private set; } = null!;

    public ValidatedSettingsService(IOptions<AppSettings> settingsOptions, ILogger<AppSettingsValidatorService> logger, HostingEnvironment hostingEnvironment) =>
        (_settingsOptions, _logger, _hostingEnvironment) = (settingsOptions, logger, hostingEnvironment);

    private bool ValidateUpstreamServiceIndex(AppSettings options)
    {
        string settingsValue = options.OverrideUpstreamServiceIndex.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() =>
            (nameof(AppSettings.OverrideUpstreamServiceIndex), Environment.CurrentDirectory), options.UpstreamServiceIndex, () =>
                (nameof(AppSettings.UpstreamServiceIndex), _hostingEnvironment.ContentRootPath), out var opt);
        try
        {
            if (ResourceLocatorUtil.TryParseHttpOrFileAsDirectoryInfo(opt.BasePath, settingsValue, out Uri UpstreamRepositoryUrl, out DirectoryInfo? directory) && !(UpstreamRepository = directory).Exists)
            {
                _logger.LogRepositoryPathNotFound(directory.FullName, true);
                return false;
            }
            return true;
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            _logger.LogRepositorySecurityException(settingsValue, true, error);
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            _logger.LogInvalidRepositoryUrl(settingsValue, true, error);
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            _logger.LogInvalidRepositoryUrl(settingsValue, true, error);
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            _logger.LogInvalidRepositoryUrl(settingsValue, true, error);
        }
        return false;
    }
    
    private bool ValidateLocalRepository(AppSettings options)
    {
        string settingsValue = options.OverrideLocalRepository.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(AppSettings.OverrideLocalRepository), Environment.CurrentDirectory), options.LocalRepository, () => (nameof(AppSettings.LocalRepository), _hostingEnvironment.ContentRootPath), out var opt);
        try
        {
            settingsValue = (LocalRepository = ResourceLocatorUtil.GetDirectoryInfo(opt.BasePath, settingsValue)).FullName;
            if (LocalRepository.Exists)
                return true;
            if (LocalRepository.Parent is null || File.Exists(settingsValue) || !LocalRepository.Parent.Exists)
                _logger.LogRepositoryPathNotFound(settingsValue, false);
            else
            {
                LocalRepository.Create();
                return true;
            }
        }
        catch (DirectoryNotFoundException exception) // Parent directory not found.
        {
            _logger.LogRepositoryPathNotFound(settingsValue, false, exception);
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            _logger.LogUnsupportedRepositoryUrlScheme(settingsValue, false, error);
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            _logger.LogRepositorySecurityException(settingsValue, false, error);
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            _logger.LogInvalidRepositoryUrl(settingsValue, true, error);
        }
        catch (IOException exception) // Error creating folder.
        {
            _logger.LogLocalRepositoryIOException(settingsValue, exception);
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            _logger.LogInvalidRepositoryUrl(settingsValue, false, error);
        }
        return false;
    }

    private bool ValidateExportLocalMetaData(AppSettings options)
    {
        string? settingsValue = options.ExportLocalMetaData;
        if (string.IsNullOrWhiteSpace(settingsValue))
            return true;
        try
        {
            ExportLocalMetaData = ResourceLocatorUtil.GetFileInfo(Environment.CurrentDirectory, settingsValue);
            if (ExportLocalMetaData.Exists || (ExportLocalMetaData.Directory is not null && ExportLocalMetaData.Directory.Exists))
                return true;
            _logger.LogExportLocalMetaDataDirectoryNotFound(settingsValue);
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            _logger.LogMetaDataExportPathAccessDenied(settingsValue, error);
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            _logger.LogInvalidExportLocalMetaData(settingsValue, error);
        }
        catch (DirectoryNotFoundException error) // Path is a URI, but not an https, http or file.
        {
            _logger.LogExportLocalMetaDataDirectoryNotFound(settingsValue, error);
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            _logger.LogInvalidExportLocalMetaData(settingsValue, error);
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            _logger.LogInvalidExportLocalMetaData(settingsValue, error);
        }
        return false;
    }

    private bool ValidateGlobalPackagesFolder(AppSettings options)
    {
        string settingsValue = options.OverrideGlobalPackagesFolder.DefaultIfWhiteSpace<(string SettingName, string BasePath)>(() => (nameof(AppSettings.OverrideGlobalPackagesFolder), Environment.CurrentDirectory), options.GlobalPackagesFolder, () => (nameof(AppSettings.GlobalPackagesFolder), _hostingEnvironment.ContentRootPath), out var opt);
        try
        {
            settingsValue = (GlobalPackagesFolder = ResourceLocatorUtil.GetDirectoryInfo(opt.BasePath, settingsValue)).FullName;
            if (GlobalPackagesFolder.Exists)
                return true;
            _logger.LogGlobalPackagesFolderNotFound(settingsValue);
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            _logger.LogGlobalPackagesFolderSecurityException(settingsValue, error);
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            _logger.LogGlobalPackagesFolderNotFileUri(settingsValue, error);
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            _logger.LogInvalidGlobalPackagesFolder(settingsValue, error);
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            _logger.LogInvalidGlobalPackagesFolder(settingsValue, error);
        }
        return false;
    }

    private bool Validate(CancellationToken cancellationToken)
    {
        AppSettings options = _settingsOptions.Value;
        bool isValid = ValidateUpstreamServiceIndex(options);
        if (!ValidateLocalRepository(options))
            isValid = false;
        if (!ValidateExportLocalMetaData(options))
            isValid = false;
        if (!ValidateGlobalPackagesFolder(options))
            isValid = false;
        if (isValid)
        {
            if (StringComparer.CurrentCultureIgnoreCase.Equals(options.LocalRepository, options.UpstreamServiceIndex))
                _logger.LogLocalSameAsUpstreamNugetRepository(options.LocalRepository);
            else if (StringComparer.CurrentCultureIgnoreCase.Equals(options.LocalRepository, options.GlobalPackagesFolder))
                _logger.LogLocalRepositorySameAsGlobalPackagesFolder(options.LocalRepository);
            else if (StringComparer.CurrentCultureIgnoreCase.Equals(options.UpstreamServiceIndex, options.GlobalPackagesFolder))
                _logger.LogUpstreamRepositorySameAsGlobalPackagesFolder(options.LocalRepository);
            else
                return true;
        }
        return false;
    }
    
    public async Task<bool> IsValidAsync(CancellationToken cancellationToken)
    {
        lock (_syncRoot)
            _validateAsync ??= Task.Factory.StartNew(() => Validate(cancellationToken));
        return await _validateAsync;
    }
}