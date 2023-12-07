namespace NuGetPuller;

public partial class AppSettings
{
    internal class ValidatedAppSettings
    {
        /// <summary>
        /// Gets the URI for the upstream service index or full local path to the upstream repository subdirectory.
        /// </summary>
        /// <value>The validated value of <see cref="OverrideUpstreamServiceIndex"/> or <see cref="UpstreamServiceIndex"/>.</value>
        internal string UpstreamServiceLocation { get; set; } = string.Empty;

        /// <summary>
        /// Gets the upstream service index or path for upstream repository subdirectory as an absolute URL.
        /// </summary>
        /// <value>The validated absolute URL of <see cref="OverrideUpstreamServiceIndex"/> or <see cref="UpstreamServiceIndex"/>.</value>
        internal Uri UpstreamServiceUri { get; set; } = null!;

        /// <summary>
        /// Gets the full local path to the local repository subdirectory.
        /// </summary>
        /// <value>The validated value of <see cref="OverrideLocalRepository"/> or <see cref="LocalRepository"/>.</value>
        internal string LocalRepositoryPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets the optional full local path of the local repository metadata export file.
        /// </summary>
        /// <value>The validated value of <see cref="ExportLocalManifest"/>.</value>
        internal string? ExportLocalMetaDataPath { get; set; }

        /// <summary>
        /// Gets the full local path of the Nuget package or folder of NuGet packages to import.
        /// </summary>
        /// <value>The validated value of <see cref="Import"/>.</value>
        internal string? ImportPath { get; set; }

        /// <summary>
        /// Gets the full local path to the NuGet global packages folder.
        /// </summary>
        /// <value>The validated value of <see cref="OverrideGlobalPackagesFolder"/> or <see cref="GlobalPackagesFolder"/>.</value>
        internal string GlobalPackagesFolderPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets the full local path of the bundle export file.
        /// </summary>
        /// <value>The validated value of <see cref="ExportBundle"/>.</value>
        internal string? ExportBundlePath { get; set; }

        /// <summary>
        /// Gets the full local path of the manifest file for the target NuGet feed.
        /// </summary>
        /// <value>The validated value of <see cref="TargetManifestFile"/>.</value>
        internal string TargetManifestFilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets the full local path to save the updated target manifest file to.
        /// </summary>
        /// <value>The validated value of <see cref="SaveTargetManifestAs"/>.</value>
        internal string TargetManifestSaveAsPath { get; set; } = string.Empty;
    }
}