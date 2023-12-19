using IsolatedPackageFeeds.Shared;
using IsolatedPackageFeeds.Shared.LazyInit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static NuGetPuller.NuGetPullerStatic;

namespace NuGetPuller;

/// <summary>
/// Service for validation of repository paths.
/// </summary>
public sealed class ValidatedRepositoryPathsService<T>(IOptions<T> options, IHostEnvironment hostEnvironment, ILogger logger) : IValidatedRepositoryPathsService
    where T : class, ISharedAppSettings
{
    /// <summary>
    /// Gets the <see cref="ISharedAppSettings"/> that contains the original settings values.
    /// </summary>
    public T Settings { get; } = options.Value;

    ISharedAppSettings IValidatedRepositoryPathsService.Settings => Settings;

    /// <summary>
    /// Lazy validation for the <see cref="ISharedAppSettings.UpstreamServiceIndexUrl"/> setting.
    /// </summary>
    public LazyChainedConversion<string, Uri> UpstreamServiceIndexUrl { get; } = new LazyChainedConversion<string, Uri>(
        () => options.Value.OverrideUpstreamServiceIndex.TryGetNonWhitesSpace(options.Value.UpstreamServiceIndexUrl, out string result) ? result : Default_Service_Index_URL,
        value =>
        {
            Uri result;
            try { result = new Uri(value, UriKind.Absolute); }
            catch (Exception exception)
            {
                throw logger.InvalidRepositoryUrl(value, true, message => new InvalidRepositoryUriException(value, message, exception), exception);
            }
            if (result.Scheme == Uri.UriSchemeFile)
                try
                {
                    if (!File.Exists(result.LocalPath))
                        logger.NuGetFeedPathNotFound(result.LocalPath, true, message => new NuGetFeedPathNotFoundException(result.LocalPath, message));
                }
                catch (Exception exception)
                {
                    throw logger.NuGetFeedPathNotFound(value, true, message => new NuGetFeedPathNotFoundException(value, message, exception), exception);
                }
            else if (result.Scheme != Uri.UriSchemeHttps && result.Scheme != Uri.UriSchemeHttp)
                throw logger.UnsupportedNuGetServiceIndexUrlScheme(result.OriginalString, message => new UriSchemeNotSupportedException(result, message));
            return result;
        });

    /// <summary>
    /// Lazy validation for the <see cref="ISharedAppSettings.LocalFeedPath"/> setting.
    /// </summary>
    public LazyChainedConversion<string, DirectoryInfo> LocalFeedPath { get; } = new LazyChainedConversion<string, DirectoryInfo>(
        () => options.Value.OverrideLocalFeed.TryGetNonWhitesSpace(options.Value.LocalFeedPath, out string result) ? result :
            Path.Combine(hostEnvironment.ContentRootPath, Default_Local_Feed_Folder_Name),
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
                throw logger.InvalidRepositoryUrl(path, false, message => new InvalidRepositoryUriException(path, message, exception), exception);
            }
            throw logger.NuGetFeedPathNotFound(path, false, message => new NuGetFeedPathNotFoundException(path, message));
        });

    /// <summary>
    /// Lazy validation for the <see cref="ISharedAppSettings.UpstreamServicGlobalPackagesFoldereIndex"/> setting.
    /// </summary>
    public LazyChainedConversion<string, DirectoryInfo> GlobalPackagesFolder { get; } = new LazyChainedConversion<string, DirectoryInfo>(
        () => options.Value.OverrideGlobalPackagesFolder.TryGetNonWhitesSpace(options.Value.GlobalPackagesFolder, out string result) ? result :
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
                throw logger.InvalidGlobalPackagesFolder(value, message => new InvalidGlobalPackagesPathException(value, message, exception), exception);
            }
            throw logger.GlobalPackagesFolderNotFound(directoryInfo.FullName, message => new GlobalPackagesPathNotFoundException(directoryInfo.FullName, message));
        });
}
