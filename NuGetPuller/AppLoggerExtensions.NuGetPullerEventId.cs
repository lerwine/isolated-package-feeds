namespace NuGetPuller;

public static partial class AppLoggerExtensions
{
    /// <summary>
    /// Logger event IDs.
    /// </summary>
    public enum NuGetPullerEventId : int
    {
        /// <summary>
        /// Event ID for <see cref="NuGet.Common.LogLevel.Debug"/> and <see cref="NuGet.Common.LogLevel.Verbose"/> messages relayed from the NuGet API.
        /// </summary>
        NuGetDebug = 0x1,

        /// <summary>
        /// Event ID for <see cref="NuGet.Common.LogLevel.Information"/> and <see cref="NuGet.Common.LogLevel.Minimal"/> messages relayed from the NuGet API.
        /// </summary>
        NugetMessage = 0x2,

        /// <summary>
        /// Event ID for <see cref="NuGet.Common.LogLevel.Warning"/> messages relayed from the NuGet API.
        /// </summary>
        NugetWarning = 0x3,

        /// <summary>
        /// Event ID for <see cref="NuGet.Common.LogLevel.Error"/> messages relayed from the NuGet API.
        /// </summary>
        NuGetError = 0x4,

        /// <summary>
        /// Event ID for <see cref="NuGet.Common.WarningLevel.Severe"/> messages relayed from the NuGet API.
        /// </summary>
        CriticalNugetError = 0x5,

        /// <summary>
        /// Event ID for invalid repository URLs.
        /// </summary>
        InvalidRepositoryUrl = 0x6,

        /// <summary>
        /// Event ID for security errors when accessing NuGet Feed paths.
        /// </summary>
        NugetFeedSecurityException = 0x7,

        /// <summary>
        /// Event ID for I/O errors when accessing NuGet Feed paths.
        /// </summary>
        LocalFeedIOException = 0x8,

        /// <summary>
        /// Event ID for I/O errors for no-existent NuGet Feed paths.
        /// </summary>
        NuGetFeedPathNotFound = 0x9,

        /// <summary>
        /// Event ID for invalid NuGet package metadata.
        /// </summary>
        InvalidLocalMetaDataExportPath = 0xA,

        /// <summary>
        /// Event ID for security errors when saving or loading metadata files.
        /// </summary>
        MetaDataExportPathAccessDenied = 0xB,

        /// <summary>
        /// Event ID for normal package deletion.
        /// </summary>
        NuGetPackageDeleted = 0xC,

        /// <summary>
        /// Event ID for a NuGet package that could not be found in a repository.
        /// </summary>
        NuGetPackageNotFound = 0xD,

        /// <summary>
        /// Event ID when the global packages folder could not be found.
        /// </summary>
        GlobalPackagesFolderNotFound = 0xE,

        /// <summary>
        /// Event ID for security errors when accessing the global packages folder.
        /// </summary>
        GlobalPackagesFolderSecurityException = 0xF,

        /// <summary>
        /// Event ID for invalid global packages folder path.
        /// </summary>
        InvalidGlobalPackagesFolder = 0x10,

        /// <summary>
        /// Event ID when more than one path setting, which should be exclusive, refers to the same location.
        /// </summary>
        MultipleSettingsWithSameRepositoryLocation = 0x11,

        /// <summary>
        /// Event ID when a single package file could not be found.
        /// </summary>
        PackageFileNotFound = 0x12,

        /// <summary>
        /// Event ID for invalid package file.
        /// </summary>
        InvalidNupkgFile = 0x13,

        /// <summary>
        /// Event ID when a package already exists while being added.
        /// </summary>
        PackageAlreadyAdded = 0x14,

        /// <summary>
        /// Event ID when a specific package version could not be removed from the Local NuGet Feed.
        /// </summary>
        PackageVersionDeleteFailure = 0x15,

        /// <summary>
        /// Event ID for unexpected error while downloading package file from Upstream NuGet Feed.
        /// </summary>
        UnexpectedPackageDownloadFailure = 0x16,

        /// <summary>
        /// Event ID for unexpected error while adding a pacakge to the Local NuGet Feed.
        /// </summary>
        UnexpectedAddFailure = 0x17,

        /// <summary>
        /// Event ID when there are no packages in the Local NuGet Feed.
        /// </summary>
        NoLocalPackagesExist = 0x18,

        /// <summary>
        /// Event ID when an export bundle file content is invalid.
        /// </summary>
        InvalidExportBundle = 0x19,

        /// <summary>
        /// Event ID when a package is being downloaded from the upstream NuGet repository.
        /// </summary>
        DownloadingNuGetPackage = 0x1A,
        InvalidCreateFromPath = 0x1B,
        IgnoredDependentCommandLineArgument = 0x1C,
        InvalidSaveManifestToPath = 0x1D
    }
}