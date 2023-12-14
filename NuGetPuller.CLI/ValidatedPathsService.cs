using IsolatedPackageFeeds.Shared;
using IsolatedPackageFeeds.Shared.LazyInit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NuGetPuller.CLI;

[Obsolete("Validate when used")]
public class ValidatedPathsService
{
    public LazyOptionalChainedConversion<string, FileInfo> ExportLocalManifest { get; }

    public LazyOptionalChainedConversion<string, FileSystemInfo> Import { get; }

    public LazyOptionalChainedConversion<string, FileInfo> ExportBundle { get; }

    public LazyOptionalChainedConversion<string, FileInfo> TargetManifestFile { get; }

    public LazyOptionalChainedConversion<string, FileInfo> SaveTargetManifestAs { get; }

    public ValidatedPathsService(IOptions<AppSettings> options, IHostEnvironment hostEnvironment, ILogger<ValidatedPathsService> logger)
    {
        ExportLocalManifest = new LazyOptionalChainedConversion<string, FileInfo>(
            (out string? path) =>
            {
                if (options.Value.ExportMetaData.TryGetNonWhitesSpace(out path))
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
                    throw logger.InvalidLocalMetaDataExportPath(value, message => new MetaDataExportPathException(value, message, exception), exception);
                }
                value = result.FullName;
                throw logger.ExportLocalMetaDataDirectoryNotFound(value, message => new MetaDataExportPathException(value, message));
            });
        Import = new LazyOptionalChainedConversion<string, FileSystemInfo>(
            (out string? path) =>
            {
                if (options.Value.Import.TryGetNonWhitesSpace(out path))
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
                    throw logger.InvalidLocalMetaDataExportPath(value, message => new MetaDataExportPathException(value, message, exception), exception);
                }
                value = result.FullName;
                throw logger.ExportLocalMetaDataDirectoryNotFound(value, message => new MetaDataExportPathException(value, message));
            });
        ExportBundle = new LazyOptionalChainedConversion<string, FileInfo>(
            (out string? path) =>
            {
                if (options.Value.CreateBundle.TryGetNonWhitesSpace(out path))
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
                    throw logger.InvalidLocalMetaDataExportPath(value, message => new MetaDataExportPathException(value, message, exception), exception);
                }
                value = result.FullName;
                throw logger.ExportLocalMetaDataDirectoryNotFound(value, message => new MetaDataExportPathException(value, message));
            });
        TargetManifestFile = new LazyOptionalChainedConversion<string, FileInfo>(
            (out string? path) =>
            {
                if (options.Value.TargetManifestFile.TryGetNonWhitesSpace(out path))
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
                    throw logger.InvalidLocalMetaDataExportPath(value, message => new MetaDataExportPathException(value, message, exception), exception);
                }
                value = result.FullName;
                throw logger.ExportLocalMetaDataDirectoryNotFound(value, message => new MetaDataExportPathException(value, message));
            });
        SaveTargetManifestAs = new LazyOptionalChainedConversion<string, FileInfo>(
            (out string? path) =>
            {
                if (options.Value.SaveMetaDataTo.TryGetNonWhitesSpace(out path))
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
                    throw logger.InvalidLocalMetaDataExportPath(value, message => new MetaDataExportPathException(value, message, exception), exception);
                }
                value = result.FullName;
                throw logger.ExportLocalMetaDataDirectoryNotFound(value, message => new MetaDataExportPathException(value, message));
            });
    }
}

