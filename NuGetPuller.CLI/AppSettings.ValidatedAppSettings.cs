namespace NuGetPuller.CLI;

public partial class AppSettings
{
    public class ValidatedAppSettings : IValidatedSharedAppSettings
    {
        public Uri UpstreamServiceIndex { get; set; } = CommonStatic.EmptyURI;

        /// <summary>
        /// Gets the optional full local path of the local repository metadata export file.
        /// </summary>
        /// <value>The validated value of <see cref="AppSettings.LocalRepository"/>.</value>
        public DirectoryInfo LocalRepository { get; set; } = null!;

        /// <summary>
        /// Gets the optional full local path of the local repository metadata export file.
        /// </summary>
        /// <value>The validated value of <see cref="AppSettings.GlobalPackagesFolder"/>.</value>
        public DirectoryInfo GlobalPackagesFolder { get; set; } = null!;

        /// <summary>
        /// Gets the optional full local path of the local repository metadata export file.
        /// </summary>
        /// <value>The validated value of <see cref="AppSettings.ExportLocalManifest"/>.</value>
        public FileInfo? ExportLocalManifest { get; set; }

        /// <summary>
        /// Gets the full local path of the Nuget package or folder of NuGet packages to import.
        /// </summary>
        /// <value>The validated value of <see cref="AppSettings.Import"/>.</value>
        public FileSystemInfo? Import { get; set; }

        /// <summary>
        /// Gets the full local path of the bundle export file.
        /// </summary>
        /// <value>The validated value of <see cref="AppSettings.ExportBundle"/>.</value>
        internal FileInfo? ExportBundle { get; set; }

        /// <summary>
        /// Gets the full local path of the manifest file for the target NuGet feed.
        /// </summary>
        /// <value>The validated value of <see cref="AppSettings.TargetManifestFile"/>.</value>
        public FileInfo TargetManifestFile { get; set; } = null!;

        /// <summary>
        /// Gets the full local path to save the updated target manifest file to.
        /// </summary>
        /// <value>The validated value of <see cref="AppSettings.SaveTargetManifestAs"/>.</value>
        public FileInfo SaveTargetManifestAs { get; set; } = null!;
    }
}