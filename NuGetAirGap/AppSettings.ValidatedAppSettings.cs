namespace NuGetAirGap;

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
        /// <value>The validated value of <see cref="ExportLocalMetaData"/>.</value>
        internal string? ExportLocalMetaDataPath { get; set; }

        /// <summary>
        /// Gets the full local path to the NuGet global packages folder.
        /// </summary>
        /// <value>The validated value of <see cref="OverrideGlobalPackagesFolder"/> or <see cref="GlobalPackagesFolder"/>.</value>
        internal string GlobalPackagesFolderPath { get; set; } = string.Empty;
    }
}