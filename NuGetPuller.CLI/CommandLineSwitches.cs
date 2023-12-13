namespace NuGetPuller.CLI;

public static class CommandLineSwitches
{
    /// <summary>
    /// Used with the <see cref="COMMAND_LINE_SWITCH_version"/> to denote all versions.
    /// </summary>
    public const string COMMAND_LINE_VALUE_all = "all";

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
    public const string COMMAND_LINE_SWITCH_remove = "--remove";

    /// <summary>
    /// This is used with the <c>--download</c> (<see cref="COMMAND_LINE_SWITCH_download"/>) command line to specify the subdirectory to save packages to before they are removed.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_save_to = "--save-to";

    /// <summary>
    /// Comma-separated list of package IDs to check for missing dependencies.
    /// </summary>
    /// <remarks>Use the keyword `all` to check dependencies of all packages.
    /// <para>This is mapped as a string value to <see cref="AppSettings.CheckDependencies"/>.</para></remarks>
    public const string COMMAND_LINE_SWITCH_check_depencencies = "--check-depencencies";

    /// <summary>
    /// This is used with the <c>--check-depencencies</c> (<see cref="COMMAND_LINE_SWITCH_check_depencencies"/>) to prevent missing dependencies from being downloaded.
    /// </summary>
    /// <remarks>This is mapped as a boolean switch to <see cref="AppSettings.NoDownload"/>.</remarks>
    public const string COMMAND_LINE_SWITCH_no_download = "--no-download";

    /// <summary>
    /// Path of package metadata for the local NuGet rpository.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_export_metadata = "--export-metadata";

    /// <summary>
    /// Path of package bundle to create.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_create_bundle = "--create-bundle";

    public const string COMMAND_LINE_SWITCH_create_from = "--create-from";

    public const string COMMAND_LINE_SWITCH_save_metadata_to = "--save-metadata-to";

    public const string COMMAND_LINE_SWITCH_include = "--include";

    /// <summary>
    /// Gets the command line switch that sets <see cref="AppSettings.UpdateAll" /> to <see langword="true"/>.
    /// </summary>
    // BUG: This doesn't make sense. Packages aren't updated - newer version are downloaded.
    [Obsolete("Use COMMAND_LINE_SWITCH_download with COMMAND_LINE_SWITCH_version set to 'all'")]
    public const string COMMAND_LINE_SWITCH_update_2D_all = "--update-all";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.Update" /> value.
    /// </summary>
    // BUG: This doesn't make sense. Packages aren't updated - newer version are downloaded.
    [Obsolete("Use COMMAND_LINE_SWITCH_download with COMMAND_LINE_SWITCH_version")]
    public const string COMMAND_LINE_SWITCH_u = "-u";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.Add" /> value.
    /// </summary>
    [Obsolete("Use COMMAND_LINE_SWITCH_download")]
    public const string COMMAND_LINE_SWITCH_a = "-a";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.Version" /> value.
    /// </summary>
    [Obsolete("Use COMMAND_LINE_SWITCH_download, COMMAND_LINE_SWITCH_remove, etc")]
    public const string COMMAND_LINE_SWITCH_v = "-v";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.Import" /> value.
    /// </summary>
    [Obsolete("Use COMMAND_LINE_SWITCH_add")]
    public const string COMMAND_LINE_SWITCH_i = "-i";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.Remove" /> value.
    /// </summary>
    [Obsolete("Use COMMAND_LINE_SWITCH_remove")]
    public const string COMMAND_LINE_SWITCH_d = "-d";

    /// <summary>
    /// Gets the command line switch that sets <see cref="AppSettings.ListLocal" /> to <see langword="true"/>.
    /// </summary>
    [Obsolete("Use COMMAND_LINE_SWITCH_list")]
    public const string COMMAND_LINE_SWITCH_l = "-l";

    /// <summary>
    /// Gets the command line switch that sets <see cref="AppSettings.CreateBundle" /> to <see langword="true"/>.
    /// </summary>
    [Obsolete("Use COMMAND_LINE_SWITCH_create_bundle")]
    public const string COMMAND_LINE_SWITCH_b = "-b";

    /// <summary>
    /// Gets the command line switch that sets <see cref="AppSettings.TargetManifestFile" /> to <see langword="true"/>.
    /// </summary>
    [Obsolete("Use COMMAND_LINE_SWITCH_create_from")]
    public const string COMMAND_LINE_SWITCH_t = "-t";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.SaveMetaDataTo" /> value.
    /// </summary>
    [Obsolete("Use COMMAND_LINE_SWITCH_save_metadata_to")]
    public const string COMMAND_LINE_SWITCH_save_2D_target_2D_manifest_2D_as = "--save-target-manifest-as";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.ExportMetaData" /> value.
    /// </summary>
    [Obsolete("Use COMMAND_LINE_SWITCH_export_metadata")]
    public const string COMMAND_LINE_SWITCH_export_2D_local_2D_manifest = "--export-local-manifest";

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