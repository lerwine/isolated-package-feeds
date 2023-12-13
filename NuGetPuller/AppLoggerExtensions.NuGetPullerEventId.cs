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
        /// Event ID for security errors when accessing repository paths.
        /// </summary>
        RepositorySecurityException = 0x7,

        /// <summary>
        /// Event ID for I/O errors when accessing repository paths.
        /// </summary>
        LocalRepositoryIOException = 0x8,

        /// <summary>
        /// Event ID for I/O errors for no-existent repository paths.
        /// </summary>
        RepositoryPathNotFound = 0x9,

        /// <summary>
        /// Event ID for invalid NuGet package metadata.
        /// </summary>
        InvalidExportLocalMetaData = 0xA,

        /// <summary>
        /// Event ID for security errors when saving or loading metadata files.
        /// </summary>
        MetaDataExportPathAccessDenied = 0xB,

        /// <summary>
        /// Event ID for normal package deletion.
        /// </summary>
        PackageDeleted = 0xC,

        /// <summary>
        /// Event ID for a NuGet package that could not be found in a repository.
        /// </summary>
        PackageNotFound = 0xD,

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
        InvalidPackageFile = 0x13,

        /// <summary>
        /// Event ID when a package already exists while being added.
        /// </summary>
        PackageAlreadyAdded = 0x14,

        /// <summary>
        /// Event ID when a specific package version could not be removed from the local repository.
        /// </summary>
        PackageVersionDeleteFailure = 0x15,

        /// <summary>
        /// Event ID for unexpected error while downloading package file from upstream repository.
        /// </summary>
        UnexpectedPackageDownloadFailure = 0x16,

        /// <summary>
        /// Event ID for unexpected error while adding a pacakge to the local repository.
        /// </summary>
        UnexpectedAddFailure = 0x17,

        /// <summary>
        /// Event ID when there are no packages in the local repository.
        /// </summary>
        NoLocalPackagesExist = 0x18,

        /// <summary>
        /// Event ID when an export bundle file content is invalid.
        /// </summary>
        InvalidExportBundle = 0x19,

        /// <summary>
        /// Event when a target manifest file content is invalid.
        /// </summary>
        InvalidTargetManifestFile = 0x1A,

        /// <summary>
        /// Event ID when the save-as path for the updated manifest file is invalid.
        /// </summary>
        InvalidSaveTargetManifestAs = 0x1B,

        /// <summary>
        /// Event ID for an invalid package import source path.
        /// </summary>
        InvalidImportPath = 0x1C,

        /// <summary>
        /// Event ID when a package is being downloaded from the upstream repository.
        /// </summary>
        DownloadingNuGetPackage = 0x1D,
        CommandLineArgumentsAreExclusive = 0x1E,
        IgnoredDependentCommandLineArgument = 0x1F
    }
}