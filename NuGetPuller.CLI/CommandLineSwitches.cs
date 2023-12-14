namespace NuGetPuller.CLI;

public static class CommandLineSwitches
{
    /// <summary>
    /// Used with the <see cref="COMMAND_LINE_SWITCH_version"/> to denote all versions.
    /// </summary>
    public const string COMMAND_LINE_VALUE_all = "all";

    public const string EXTENSION_nupkg = ".nupkg";

    public const string METADATA_EXTENSION_nuget_metadata_json = ".nuget.metadata.json";
    
    /// <summary>
    /// List all packages in the local repository.
    /// </summary>
    /// <remarks>This is mapped as a boolean switch to <see cref="AppSettings.ListLocal"/>.</remarks>
    public const string COMMAND_LINE_SWITCH_list = "--list";

    /// <summary>
    /// This is used with the <c>--list</c> (<see cref="COMMAND_LINE_SWITCH_list"/>) command line switch to show all version numbers of each package in the local repository.
    /// </summary>
    /// <remarks>This is mapped as a boolean switch to <see cref="AppSettings.IncludeVersions"/>.</remarks>
    public const string COMMAND_LINE_SWITCH_include_versions = "--include-versions";

    /// <summary>
    /// Comma-separated list of package IDs to be downloaded.
    /// </summary>
    /// <remarks>This is mapped as a string value to <see cref="AppSettings.Download"/>.</remarks>
    public const string COMMAND_LINE_SWITCH_download = "--download";

    /// <summary>
    /// Comma-separated list of package versions. This is used with the <c>--download</c> (<see cref="COMMAND_LINE_SWITCH_download"/>),
    /// <c>--remove</c> (<see cref="COMMAND_LINE_SWITCH_remove"/>), <c>--check-depencencies</c> (<see cref="COMMAND_LINE_SWITCH_check_depencencies"/>)
    /// and <c>--create-bundle</c> (<see cref="COMMAND_LINE_SWITCH_create_bundle"/>) command line switches.
    /// </summary>
    /// <remarks>This is mapped as a string value to <see cref="AppSettings.Version"/>.</remarks>
    public const string COMMAND_LINE_SWITCH_version = "--version";

    /// <summary>
    /// This is used with the <c>--download</c> (<see cref="COMMAND_LICOMMAND_LINE_SWITCH_downloadE_SWITCH_list"/>) command line switch to refrain from downloading missing dependencies if <see cref="true"/>;
    /// otherwise, missing dependencies will be downloaded from the upstream repository, if present.
    /// </summary>
    /// <remarks>This is mapped as a boolean switch to <see cref="AppSettings.NoDependencies"/>.</remarks>
    public const string COMMAND_LINE_SWITCH_no_dependencies = "--no-dependencies";

    /// <summary>
    /// Semi-colon-separated list of package file paths or subdirectory paths.
    /// </summary>
    /// <remarks>This is mapped as a string value to <see cref="AppSettings.AddPackageFiles"/>.</remarks>
    public const string COMMAND_LINE_SWITCH_add_file = "--add-file";

    /// <summary>
    /// Comma-separated list of package IDs to remove from the local repository.
    /// </summary>
    /// <remarks>This is mapped as a string value to <see cref="AppSettings.Remove"/>.</remarks>
    public const string COMMAND_LINE_SWITCH_remove = "--remove";

    /// <summary>
    /// This is used with the <c>--download</c> (<see cref="COMMAND_LINE_SWITCH_download"/>) command line to specify the subdirectory to save packages to before they are removed.
    /// </summary>
    /// <remarks>This is mapped as a string value to <see cref="AppSettings.SaveTo"/>.</remarks>
    public const string COMMAND_LINE_SWITCH_save_to = "--save-to";

    /// <summary>
    /// Check to see if there are any missing dependencies for packages in the local repository.
    /// </summary>
    /// <remarks>This is mapped as a boolean switch to <see cref="AppSettings.CheckDependencies"/>.</remarks>
    public const string COMMAND_LINE_SWITCH_check_depencencies = "--check-depencencies";

    /// <summary>
    /// Comma-separated list of package IDs to check for missing dependencies.
    /// </summary>
    /// <remarks>This is mapped as a string value to <see cref="AppSettings.PackageId"/>.</remarks>
    public const string COMMAND_LINE_SWITCH_package_id = "--package-id";

    /// <summary>
    /// This is used with the <c>--check-depencencies</c> (<see cref="COMMAND_LINE_SWITCH_check_depencencies"/>) to prevent missing dependencies from being downloaded.
    /// </summary>
    /// <remarks>This is mapped as a boolean switch to <see cref="AppSettings.NoDownload"/>.</remarks>
    public const string COMMAND_LINE_SWITCH_no_download = "--no-download";

    /// <summary>
    /// Export path of package metadata from the local NuGet rpository.
    /// </summary>
    /// <remarks>This is mapped as a string value to <see cref="AppSettings.ExportMetaData"/>.</remarks>
    public const string COMMAND_LINE_SWITCH_export_metadata = "--export-metadata";

    /// <summary>
    /// Path of package bundle to create.
    /// </summary>
    /// <remarks>This is mapped as a string value to <see cref="AppSettings.CreateBundle"/>.</remarks>
    public const string COMMAND_LINE_SWITCH_create_bundle = "--create-bundle";

    /// <summary>
    /// Path of package to package metadata json of another local NuGet repository.
    /// </summary>
    /// <remarks>This is mapped as a string value to <see cref="AppSettings.CreateFrom"/>.</remarks>
    public const string COMMAND_LINE_SWITCH_create_from = "--create-from";

    /// <summary>
    /// Path of to save updated metadata to.
    /// </summary>
    /// <remarks>This is mapped as a string value to <see cref="AppSettings.SaveMetaDataTo"/>.</remarks>
    public const string COMMAND_LINE_SWITCH_save_metadata_to = "--save-metadata-to";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.OverrideUpstreamServiceIndex" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_upstream_2D_service_2D_index = "--upstream-service-index";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.OverrideLocalRepository" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_local_2D_repository = "--local-repository";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.OverrideGlobalPackagesFolder" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_global_2D_packages_2D_folder = "--global-packages-folder";

    /// <summary>
    /// Gets the command line switch that sets <see cref="AppSettings.Help" /> to <see langword="true"/>.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_help = "--help";

    /// <summary>
    /// Gets the command line switch that sets <see cref="AppSettings.Help" /> to <see langword="true"/>.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_h = "-h";

    /// <summary>
    /// Gets the command line switch that sets <see cref="AppSettings.Help" /> to <see langword="true"/>.
    /// </summary>
    public const string COMMAND_LINE_SWITCH__3F_ = "-?";

}