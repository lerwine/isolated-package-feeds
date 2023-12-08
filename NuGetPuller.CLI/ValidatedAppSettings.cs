using Microsoft.Extensions.Options;

namespace NuGetPuller.CLI;

public class ValidatedAppSettings(IOptions<AppSettings> options) : ValidatedSharedAppSettings<AppSettings>(options.Value)
{

    /// <summary>
    /// Gets the optional full local path of the local repository metadata export file.
    /// </summary>
    /// <value>The validated value of <see cref="AppSettings.ExportLocalManifest"/>.</value>
    public FileInfo? ExportLocalManifest { get; set; }

    public string? GetExportLocalManifest() => ExportLocalManifest?.FullName ?? AppSettings.ExportLocalManifest;

    /// <summary>
    /// Gets the full local path of the Nuget package or folder of NuGet packages to import.
    /// </summary>
    /// <value>The validated value of <see cref="AppSettings.Import"/>.</value>
    public FileSystemInfo? Import { get; set; }

    public string? GetImport() => Import?.FullName ?? AppSettings.Import;

    /// <summary>
    /// Gets the full local path of the bundle export file.
    /// </summary>
    /// <value>The validated value of <see cref="AppSettings.ExportBundle"/>.</value>
    internal FileInfo? ExportBundle { get; set; }

    public string? GetExportBundle() => ExportBundle?.FullName ?? AppSettings.ExportBundle;

    /// <summary>
    /// Gets the full local path of the manifest file for the target NuGet feed.
    /// </summary>
    /// <value>The validated value of <see cref="AppSettings.TargetManifestFile"/>.</value>
    public FileInfo TargetManifestFile { get; set; } = null!;

    public string? GetTargetManifestFile() => TargetManifestFile?.FullName ?? AppSettings.TargetManifestFile;

    /// <summary>
    /// Gets the full local path to save the updated target manifest file to.
    /// </summary>
    /// <value>The validated value of <see cref="AppSettings.SaveTargetManifestAs"/>.</value>
    public FileInfo SaveTargetManifestAs { get; set; } = null!;

    public string? GetSaveTargetManifestAs() => SaveTargetManifestAs?.FullName ?? AppSettings.SaveTargetManifestAs;
}