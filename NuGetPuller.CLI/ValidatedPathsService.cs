using IsolatedPackageFeeds.Shared;
using IsolatedPackageFeeds.Shared.LazyInit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NuGetPuller.CLI;

public class ValidatedPathsService : ValidatedRepositoryPathsService<AppSettings>
{
    public LazyOptionalChainedConversion<string, FileInfo> ExportLocalManifest { get; }

    public LazyOptionalChainedConversion<string, FileSystemInfo> Import { get; }

    public LazyOptionalChainedConversion<string, FileInfo> ExportBundle { get; }

    public LazyOptionalChainedConversion<string, FileInfo> TargetManifestFile { get; }

    public LazyOptionalChainedConversion<string, FileInfo> SaveTargetManifestAs { get; }

    public ValidatedPathsService(IOptions<AppSettings> options, IHostEnvironment hostEnvironment, ILogger<ValidatedPathsService> logger) : base(options.Value, hostEnvironment, logger)
    {
        ExportLocalManifest = new LazyOptionalChainedConversion<string, FileInfo>(
            (out string? path) =>
            {
                if (Settings.ExportLocalManifest.TryGetNonWhitesSpace(out path))
                    return true;
                path = null;
                return false;
            },
            (string value, out FileInfo? result) =>
            {
                try
                {
                    if ((result = new(value)).Exists || (result.Directory is not null && result.Directory.Exists))
                        return true;
                }
                catch (Exception exception)
                {
                    throw logger.LogInvalidExportLocalMetaData(value, message => new MetaDataExportPathException(value, message, exception), exception);
                }
                value = result.FullName;
                throw logger.LogExportLocalMetaDataDirectoryNotFound(value, message => new MetaDataExportPathException(value, message));
            });
        Import = new LazyOptionalChainedConversion<string, FileSystemInfo>(
            (out string? path) =>
            {
                if (Settings.Import.TryGetNonWhitesSpace(out path))
                    return true;
                path = null;
                return false;

            },
            (string value, out FileSystemInfo? result) =>
            {
                try
                {
                    FileInfo fileInfo = new(value);
                    if ((result = fileInfo).Exists)
                    {
                        if (fileInfo.Directory is not null && fileInfo.Directory.Exists)
                            return true;
                    }
                    else if ((result = new DirectoryInfo(value)).Exists)
                        return true;
                }
                catch (Exception exception)
                {
                    throw logger.LogInvalidExportLocalMetaData(value, message => new MetaDataExportPathException(value, message, exception), exception);
                }
                value = result.FullName;
                throw logger.LogExportLocalMetaDataDirectoryNotFound(value, message => new MetaDataExportPathException(value, message));
            });
        ExportBundle = new LazyOptionalChainedConversion<string, FileInfo>(
            (out string? path) =>
            {
                if (Settings.ExportBundle.TryGetNonWhitesSpace(out path))
                    return true;
                path = null;
                return false;
            },
            (string value, out FileInfo? result) =>
            {
                try
                {
                    if ((result = new(value)).Exists || (result.Directory is not null && result.Directory.Exists))
                        return true;
                }
                catch (Exception exception)
                {
                    throw logger.LogInvalidExportLocalMetaData(value, message => new MetaDataExportPathException(value, message, exception), exception);
                }
                value = result.FullName;
                throw logger.LogExportLocalMetaDataDirectoryNotFound(value, message => new MetaDataExportPathException(value, message));
            });
        TargetManifestFile = new LazyOptionalChainedConversion<string, FileInfo>(
            (out string? path) =>
            {
                if (Settings.TargetManifestFile.TryGetNonWhitesSpace(out path))
                    return true;
                path = null;
                return false;
            },
            (string value, out FileInfo? result) =>
            {
                try
                {
                    if ((result = new(value)).Exists || (result.Directory is not null && result.Directory.Exists))
                        return true;
                }
                catch (Exception exception)
                {
                    throw logger.LogInvalidExportLocalMetaData(value, message => new MetaDataExportPathException(value, message, exception), exception);
                }
                value = result.FullName;
                throw logger.LogExportLocalMetaDataDirectoryNotFound(value, message => new MetaDataExportPathException(value, message));
            });
        SaveTargetManifestAs = new LazyOptionalChainedConversion<string, FileInfo>(
            (out string? path) =>
            {
                if (Settings.SaveTargetManifestAs.TryGetNonWhitesSpace(out path))
                    return true;
                path = null;
                return false;
            },
            (string value, out FileInfo? result) =>
            {
                try
                {
                    if ((result = new(value)).Exists || (result.Directory is not null && result.Directory.Exists))
                        return true;
                }
                catch (Exception exception)
                {
                    throw logger.LogInvalidExportLocalMetaData(value, message => new MetaDataExportPathException(value, message, exception), exception);
                }
                value = result.FullName;
                throw logger.LogExportLocalMetaDataDirectoryNotFound(value, message => new MetaDataExportPathException(value, message));
            });
    }
}

