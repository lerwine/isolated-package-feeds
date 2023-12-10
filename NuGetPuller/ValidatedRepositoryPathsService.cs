using IsolatedPackageFeeds.Shared;
using IsolatedPackageFeeds.Shared.LazyInit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NuGetPuller;

public abstract class ValidatedRepositoryPathsService<T> : IValidatedRepositoryPathsService
    where T : ISharedAppSettings
{
    public T Settings { get; }

    public LazyChainedConversion<string, Uri> UpstreamServiceIndex { get; }

    public LazyChainedConversion<string, DirectoryInfo> LocalRepository { get; }

    public LazyChainedConversion<string, DirectoryInfo> GlobalPackagesFolder { get; }

    public ValidatedRepositoryPathsService(T settings, IHostEnvironment hostEnvironment, ILogger logger)
    {
        Settings = settings;
        UpstreamServiceIndex = new LazyChainedConversion<string, Uri>(
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
        LocalRepository = new LazyChainedConversion<string, DirectoryInfo>(
            () => settings.OverrideLocalRepository.TryGetNonWhitesSpace(settings.LocalRepository, out string result) ? result : Path.Combine(hostEnvironment.ContentRootPath, ServiceDefaults.DEFAULT_LOCAL_REPOSITORY),
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
                throw logger.LogInvalidRepositoryUrl(value, false, message => new InvalidRepositoryUriException(value, message, exception), exception);
            }
            throw logger.LogRepositoryPathNotFound(directoryInfo.FullName, false, message => new RepositoryPathNotFoundException(directoryInfo.FullName, message));
        });
        GlobalPackagesFolder = new LazyChainedConversion<string, DirectoryInfo>(
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
}
