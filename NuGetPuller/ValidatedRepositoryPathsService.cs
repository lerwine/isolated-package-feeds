using IsolatedPackageFeeds.Shared;
using IsolatedPackageFeeds.Shared.LazyInit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NuGetPuller;

/// <summary>
/// Service for validation of repository paths.
/// </summary>
public abstract class ValidatedRepositoryPathsService<T>(T settings, IHostEnvironment hostEnvironment, ILogger logger) : IValidatedRepositoryPathsService
    where T : ISharedAppSettings
{
    /// <summary>
    /// Gets the <see cref="ISharedAppSettings"/> that contains the original settings values.
    /// </summary>
    public T Settings { get; } = settings;

    ISharedAppSettings IValidatedRepositoryPathsService.Settings => Settings;

    /// <summary>
    /// Lazy validation for the <see cref="ISharedAppSettings.UpstreamServiceIndex"/> setting.
    /// </summary>
    public LazyChainedConversion<string, Uri> UpstreamServiceIndex { get; } = new LazyChainedConversion<string, Uri>(
        () => settings.OverrideUpstreamServiceIndex.TryGetNonWhitesSpace(settings.UpstreamServiceIndex, out string result) ? result : ServiceDefaults.DEFAULT_UPSTREAM_SERVICE_INDEX,
        value =>
        {
            Uri result;
            try { result = new Uri(value, UriKind.Absolute); }
            catch (Exception exception)
            {
                throw logger.LogInvalidRepositoryUrl(value, true, message => new InvalidRepositoryUriException(value, message, exception), exception);
            }
            if (result.Scheme == Uri.UriSchemeFile)
                try
                {
                    if (!File.Exists(result.LocalPath))
                        logger.LogRepositoryPathNotFound(result.LocalPath, true, message => new RepositoryPathNotFoundException(result.LocalPath, message));
                }
                catch (Exception exception)
                {
                    throw logger.LogRepositoryPathNotFound(value, true, message => new RepositoryPathNotFoundException(value, message, exception), exception);
                }
            else if (result.Scheme != Uri.UriSchemeHttps && result.Scheme != Uri.UriSchemeHttp)
                throw logger.LogUnsupportedRepositoryUrlScheme(result.OriginalString, message => new UriSchemeNotSupportedException(result, message));
            return result;
        });

    /// <summary>
    /// Lazy validation for the <see cref="ISharedAppSettings.LocalRepository"/> setting.
    /// </summary>
    public LazyChainedConversion<string, DirectoryInfo> LocalRepository { get; } = new LazyChainedConversion<string, DirectoryInfo>(
        () => settings.OverrideLocalRepository.TryGetNonWhitesSpace(settings.LocalRepository, out string result) ? result :
            Path.Combine(hostEnvironment.ContentRootPath, ServiceDefaults.DEFAULT_LOCAL_REPOSITORY),
        value =>
        {
            string path = value;
            DirectoryInfo directoryInfo;
            try
            {
                if ((directoryInfo = new(value)).Exists)
                    return directoryInfo;
                path = directoryInfo.FullName;
                if (directoryInfo.Parent is not null && directoryInfo.Parent.Exists && !File.Exists(path))
                {
                    directoryInfo.Create();
                    directoryInfo.Refresh();
                    if (directoryInfo.Exists)
                        return directoryInfo;
                }
            }
            catch (Exception exception)
            {
                throw logger.LogInvalidRepositoryUrl(path, false, message => new InvalidRepositoryUriException(path, message, exception), exception);
            }
            throw logger.LogRepositoryPathNotFound(path, false, message => new RepositoryPathNotFoundException(path, message));
        });

    /// <summary>
    /// Lazy validation for the <see cref="ISharedAppSettings.UpstreamServicGlobalPackagesFoldereIndex"/> setting.
    /// </summary>
    public LazyChainedConversion<string, DirectoryInfo> GlobalPackagesFolder { get; } = new LazyChainedConversion<string, DirectoryInfo>(
        () => settings.OverrideGlobalPackagesFolder.TryGetNonWhitesSpace(settings.GlobalPackagesFolder, out string result) ? result :
            NuGet.Configuration.SettingsUtility.GetGlobalPackagesFolder(NuGet.Configuration.Settings.LoadDefaultSettings(root: null)),
        value =>
        {
            DirectoryInfo directoryInfo;
            try
            {
                if ((directoryInfo = new(value)).Exists)
                    return directoryInfo;
            }
            catch (Exception exception)
            {
                throw logger.LogInvalidGlobalPackagesFolder(value, message => new InvalidGlobalPackagesPathException(value, message, exception), exception);
            }
            throw logger.LogGlobalPackagesFolderNotFound(directoryInfo.FullName, message => new GlobalPackagesPathNotFoundException(directoryInfo.FullName, message));
        });
}
