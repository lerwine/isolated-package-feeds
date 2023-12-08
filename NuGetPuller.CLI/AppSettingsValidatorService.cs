using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NuGetPuller.CLI;

public partial class AppSettingsValidatorService(ILogger<AppSettingsValidatorService> logger, ValidatedAppSettings validatedAppSettings, IHostEnvironment hostEnvironment) : SharedAppSettingsValidatorService<AppSettings, ValidatedAppSettings>(logger, validatedAppSettings, hostEnvironment)
{
    private bool CheckExportLocalManifest(AppSettings options, ValidatedAppSettings validatedAppSettings, [NotNullWhen(true)] out ValidationResult? validationResult)
    {
        string? path = options.ExportLocalManifest;
        if (string.IsNullOrWhiteSpace(path))
        {
            validationResult = null;
            return false;
        }
        try
        {
            path = (validatedAppSettings.ExportLocalManifest = ResourceLocatorUtil.GetFileInfo(Directory.GetCurrentDirectory(), path)).FullName;
            if (validatedAppSettings.ExportLocalManifest.Exists || (validatedAppSettings.ExportLocalManifest.Directory is not null && validatedAppSettings.ExportLocalManifest.Directory.Exists))
            {
                validationResult = null;
                return false;
            }
            validationResult = new ValidationResult(Logger.LogExportLocalMetaDataDirectoryNotFound(path), Enumerable.Repeat(nameof(AppSettings.ExportLocalManifest), 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(Logger.LogMetaDataExportPathAccessDenied(path, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalManifest), 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(Logger.LogInvalidExportLocalMetaData(path, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalManifest), 1));
        }
        catch (DirectoryNotFoundException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(Logger.LogExportLocalMetaDataDirectoryNotFound(path, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalManifest), 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(Logger.LogInvalidExportLocalMetaData(path, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalManifest), 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(Logger.LogInvalidExportLocalMetaData(path, error), Enumerable.Repeat(nameof(AppSettings.ExportLocalManifest), 1));
        }
        return true;
    }

    private bool CheckExportBundle(AppSettings options, ValidatedAppSettings validatedAppSettings, [NotNullWhen(true)] out ValidationResult? validationResult)
    {
        string? path = options.ExportBundle;
        if (string.IsNullOrWhiteSpace(path))
        {
            validationResult = null;
            return false;
        }
        try
        {
            path = (validatedAppSettings.ExportBundle = ResourceLocatorUtil.GetFileInfo(Directory.GetCurrentDirectory(), path)).FullName;
            if (!validatedAppSettings.ExportBundle.Exists && (validatedAppSettings.ExportBundle.Directory is null || !validatedAppSettings.ExportBundle.Directory.Exists))
            {
                validationResult = new ValidationResult(Logger.LogExportBundleDirectoryNotFound(path), Enumerable.Repeat(nameof(AppSettings.ExportBundle), 1));
                return true;
            }
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(Logger.LogExportBundlePathAccessDenied(path, error), Enumerable.Repeat(nameof(AppSettings.ExportBundle), 1));
            return true;
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(Logger.LogInvalidExportBundle(path, error), Enumerable.Repeat(nameof(AppSettings.ExportBundle), 1));
            return true;
        }
        catch (DirectoryNotFoundException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(Logger.LogExportBundleDirectoryNotFound(path, error), Enumerable.Repeat(nameof(AppSettings.ExportBundle), 1));
            return true;
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(Logger.LogInvalidExportBundle(path, error), Enumerable.Repeat(nameof(AppSettings.ExportBundle), 1));
            return true;
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(Logger.LogInvalidExportBundle(path, error), Enumerable.Repeat(nameof(AppSettings.ExportBundle), 1));
            return true;
        }
        path = options.TargetManifestFile!;
        try
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = $"{Path.GetFileNameWithoutExtension(validatedAppSettings.ExportBundle.Name)}.json";
                if (validatedAppSettings.ExportBundle.Directory is not null)
                    path = Path.Combine(validatedAppSettings.ExportBundle.DirectoryName!, path);
            }
            else
            {
                FileInfo f = ResourceLocatorUtil.GetFileInfo(Directory.GetCurrentDirectory(), path);
                path = f.FullName;
                if (!f.Exists && (f.Directory is null || !f.Directory.Exists))
                {
                    validationResult = new ValidationResult(Logger.LogTargetManifestFileDirectoryNotFound(path), Enumerable.Repeat(nameof(AppSettings.TargetManifestFile), 1));
                    return true;
                }
            }
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(Logger.LogTargetManifestFilePathAccessDenied(path, error), Enumerable.Repeat(nameof(AppSettings.TargetManifestFile), 1));
            return true;
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(Logger.LogInvalidTargetManifestFile(path, error), Enumerable.Repeat(nameof(AppSettings.TargetManifestFile), 1));
            return true;
        }
        catch (DirectoryNotFoundException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(Logger.LogTargetManifestFileDirectoryNotFound(path, error), Enumerable.Repeat(nameof(AppSettings.TargetManifestFile), 1));
            return true;
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(Logger.LogInvalidTargetManifestFile(path, error), Enumerable.Repeat(nameof(AppSettings.TargetManifestFile), 1));
            return true;
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(Logger.LogInvalidTargetManifestFile(path, error), Enumerable.Repeat(nameof(AppSettings.TargetManifestFile), 1));
            return true;
        }
        path = options.SaveTargetManifestAs;
        if (string.IsNullOrWhiteSpace(path))
        {
            validatedAppSettings.SaveTargetManifestAs = validatedAppSettings.TargetManifestFile;
            validationResult = null;
            return false;
        }
        try
        {

            FileInfo f = ResourceLocatorUtil.GetFileInfo(Directory.GetCurrentDirectory(), path);
            path = f.FullName;
            if (f.Exists || (f.Directory is not null && f.Directory.Exists))
            {
                validationResult = null;
                return false;
            }
            validationResult = new ValidationResult(Logger.LogSaveTargetManifestAsDirectoryNotFound(path), Enumerable.Repeat(nameof(AppSettings.SaveTargetManifestAs), 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(Logger.LogSaveTargetManifestAsPathAccessDenied(path, error), Enumerable.Repeat(nameof(AppSettings.SaveTargetManifestAs), 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(Logger.LogInvalidSaveTargetManifestAs(path, error), Enumerable.Repeat(nameof(AppSettings.SaveTargetManifestAs), 1));
        }
        catch (DirectoryNotFoundException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(Logger.LogSaveTargetManifestAsDirectoryNotFound(path, error), Enumerable.Repeat(nameof(AppSettings.SaveTargetManifestAs), 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(Logger.LogInvalidSaveTargetManifestAs(path, error), Enumerable.Repeat(nameof(AppSettings.SaveTargetManifestAs), 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(Logger.LogInvalidSaveTargetManifestAs(path, error), Enumerable.Repeat(nameof(AppSettings.SaveTargetManifestAs), 1));
        }
        return true;
    }

    private bool CheckImport(AppSettings options, ValidatedAppSettings validatedAppSettings, [NotNullWhen(true)] out ValidationResult? validationResult)
    {
        string? path = options.Import;
        if (string.IsNullOrWhiteSpace(path))
        {
            validationResult = null;
            return false;
        }
        try
        {
            path = (validatedAppSettings.Import = ResourceLocatorUtil.GetFileOrDirectory(Directory.GetCurrentDirectory(), path)).FullName;
            if (validatedAppSettings.Import.Exists)
            {
                validationResult = null;
                return false;
            }
            validationResult = new ValidationResult(Logger.LogImportFileOrDirectoryNotFound(path), Enumerable.Repeat(nameof(AppSettings.Import), 1));
        }
        catch (System.Security.SecurityException error) // The caller does not have the required permissions to the path
        {
            validationResult = new ValidationResult(Logger.LogImportPathAccessDenied(path, error), Enumerable.Repeat(nameof(AppSettings.Import), 1));
        }
        catch (UriSchemeNotSupportedException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(Logger.LogInvalidImportPath(path, error), Enumerable.Repeat(nameof(AppSettings.Import), 1));
        }
        catch (DirectoryNotFoundException error) // Path is a URI, but not an https, http or file.
        {
            validationResult = new ValidationResult(Logger.LogImportFileOrDirectoryNotFound(path, error), Enumerable.Repeat(nameof(AppSettings.Import), 1));
        }
        catch (PathTooLongException error) // Path, file name, or both exceed the system-defined maximum length
        {
            validationResult = new ValidationResult(Logger.LogInvalidImportPath(path, error), Enumerable.Repeat(nameof(AppSettings.Import), 1));
        }
        catch (ArgumentException error) // Path contains invalid characters or system could not retrieve the absolute path
        {
            validationResult = new ValidationResult(Logger.LogInvalidImportPath(path, error), Enumerable.Repeat(nameof(AppSettings.Import), 1));
        }
        return true;
    }

    protected override void Validate(AppSettings options, ValidatedAppSettings validatedAppSettings, List<ValidationResult> validationResults)
    {
        if (CheckExportLocalManifest(options, validatedAppSettings, out ValidationResult? validationResult))
            validationResults.Add(validationResult);
        if (CheckExportBundle(options, validatedAppSettings, out validationResult))
            validationResults.Add(validationResult);
        if (CheckImport(options, validatedAppSettings, out validationResult))
            validationResults.Add(validationResult);
    }
}