using System.Collections.Immutable;
using System.Collections.ObjectModel;
using IsolatedPackageFeeds.Shared;
using Microsoft.Extensions.Configuration;
using static NuGetPuller.CLI.CommandLineSwitches;

namespace NuGetPuller.CLI;

public partial class AppSettings : ISharedAppSettings
{
    /// <summary>
    /// If true, then all packages in the local NuGet source will be displayed in the output.
    /// </summary>
    /// <remarks>This is mapped from the <c>--list</c> (<see cref="COMMAND_LINE_SWITCH_list"/>) command line switch.</remarks>
    public bool ListLocal { get; set; }

    /// <summary>
    /// If true, then all packages in the local NuGet source will be displayed in the output.
    /// </summary>
    /// <remarks>This is mapped from the <c>-l</c> (<see cref="COMMAND_LINE_SWITCH_include_versions"/>) command line switch.</remarks>
    public string? IncludeVersions { get; set; }

    /// <summary>
    /// Comma-separated list of package IDs to be downloaded.
    /// </summary>
    /// <remarks>This is mapped from the <c>--download</c> (<see cref="COMMAND_LINE_SWITCH_download"/>) command line switch.</remarks>
    public string? Download { get; set; }

    /// <summary>
    /// Comma-separated list of package versions. This is used with the <c>--download</c> (<see cref="COMMAND_LINE_SWITCH_download"/>),
    /// <c>--remove</c> (<see cref="COMMAND_LINE_SWITCH_remove"/>), <c>--check-depencencies</c> (<see cref="COMMAND_LINE_SWITCH_check_depencencies"/>)
    /// and <c>--create-bundle</c> (<see cref="COMMAND_LINE_SWITCH_create_bundle"/>) command line switches.
    /// </summary>
    /// <remarks>This is mapped from the <c>--version</c> (<see cref="COMMAND_LINE_SWITCH_version"/>) command line switch.
    /// <para>In addition to version number strings, you can use the key word <c>all</c> to ensure that all versions are downloaded.</para></remarks>
    public string? Version { get; set; }

    /// <summary>
    /// This is used with the (<see cref="Download"/>) setting to refrain from downloading missing dependencies if <see cref="true"/>;
    /// otherwise, missing dependencies will be downloaded from the upstream repository, if present.
    /// </summary>
    /// <remarks>This is mapped from the <c>--no-dependencies</c> (<see cref="COMMAND_LINE_SWITCH_no_dependencies"/>) command line switch.</remarks>
    public bool NoDependencies { get; set; }

    /// <summary>
    /// Semi-colon-separated list of package file paths or subdirectory paths to add to the local NuGet repository.
    /// </summary>
    /// <remarks>This is mapped from the <c>--add</c> (<see cref="COMMAND_LINE_SWITCH_add"/>) command line switch.</remarks>
    public string? AddPackageFiles { get; set; }

    /// <summary>
    /// Comma-separated list of package IDs for packages to be deleted from the local reository.
    /// </summary>
    /// <remarks>This is mapped from the <c>-d</c> (<see cref="COMMAND_LINE_SWITCH_remove"/>) command line switch.</remarks>
    public string? Remove { get; set; }

    /// <summary>
    /// This is used with the <see cref="Remove"/>) setting to specify the subdirectory to save packages to before they are removed.
    /// </summary>
    /// <remarks>This is mapped from the <c>-d</c> (<see cref="COMMAND_LINE_SWITCH_save_to"/>) command line switch.</remarks>
    public string? SaveTo { get; set; }

    /// <summary>
    /// Gets the path of the new manifest file for the target NuGet feed that includes the packages bundled in <see cref="TargetManifestFile"/>.
    /// </summary>
    /// <remarks>If this option is not specified, then <see cref="TargetManifestFile"/> will be overwritten to include the bundled package information.
    /// <para>This is mapped from the <c>--save-target-manifest-as</c> (<see cref="COMMAND_LINE_SWITCH_check_depencencies"/>) command line switch.</para></remarks>
    public string? CheckDependencies { get; set; }

    /// <summary>
    /// Gets the path of the new manifest file for the target NuGet feed that includes the packages bundled in <see cref="TargetManifestFile"/>.
    /// </summary>
    /// <remarks>If this option is not specified, then <see cref="TargetManifestFile"/> will be overwritten to include the bundled package information.
    /// <para>This is mapped from the <c>--save-target-manifest-as</c> (<see cref="COMMAND_LINE_SWITCH_no_download"/>) command line switch.</para></remarks>
    public bool NoDownload { get; set; }

    /// <summary>
    /// Gets the path of the package transfer bundle to create.
    /// </summary>
    /// <remarks>This is mapped from the <c>--list</c> (<see cref="COMMAND_LINE_SWITCH_create_bundle"/>) command line switch.</remarks>
    public string? CreateBundle { get; set; }

    /// <summary>
    /// Gets the path of the package transfer bundle to create.
    /// </summary>
    /// <remarks>This is mapped from the <c>--create-from</c> (<see cref="COMMAND_LINE_SWITCH_create_from"/>) command line switch.</remarks>
    public string? CreateFrom { get; set; }

    /// <summary>
    /// Gets the path of the new manifest file for the target NuGet feed that includes the packages bundled in <see cref="TargetManifestFile"/>.
    /// </summary>
    /// <remarks>If this option is not specified, then <see cref="TargetManifestFile"/> will be overwritten to include the bundled package information.
    /// <para>This is mapped from the <c>--save-target-manifest-as</c> (<see cref="COMMAND_LINE_SWITCH_save_metadata_to"/>) command line switch.</para></remarks>
    public string? SaveMetaDataTo { get; set; }

    /// <summary>
    /// Gets the path to export the metadata for all packages in the local repository. This refers to a relative or absolute file path.
    /// </summary>
    /// <remarks>The package listing is exported as a JSON array. If this path is not absolute, it will be resolved relative to the current working directory (<see cref="Directory.GetCurrentDirectory"/>).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This is mapped from the <c>--export-local-manifest</c> (<see cref="COMMAND_LINE_SWITCH_export_metadata"/>) command line switch.</para></remarks>
    public string? ExportMetaData { get; set; }

    /// <summary>
    /// Gets the path to export the metadata for all packages in the local repository. This refers to a relative or absolute file path.
    /// </summary>
    /// <remarks>The package listing is exported as a JSON array. If this path is not absolute, it will be resolved relative to the current working directory (<see cref="Directory.GetCurrentDirectory"/>).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This is mapped from the <c>--export-local-manifest</c> (<see cref="COMMAND_LINE_SWITCH_include"/>) command line switch.</para></remarks>
    public string? Include { get; set; }


    /// <summary>
    /// If true, then all existing packages in the local NuGet repository should be checked and updated to their latest versions.
    /// </summary>
    /// <remarks>This is mapped from the <c>--update-all</c> (<see cref="COMMAND_LINE_SWITCH_update_2D_all"/>) command line switch.</remarks>
    // BUG: This doesn't make sense. Packages aren't updated - newer version are downloaded.
    [Obsolete("Use Add with Version set to 'all'")]
    public bool UpdateAll { get; set; }

    /// <summary>
    /// Gets a semi-colon or comma-separated list of package IDs for packages in the local repository that should be checked and updated to their latest versions.
    /// </summary>
    /// <remarks>If a package is referenced in this option as well as the <c>-d</c> (<see cref="COMMAND_LINE_SWITCH_d"/>) command line switch, it will result in a warning stating that the package was not found in the local repository.
    /// <para>This is mapped from the <c>-u</c> (<see cref="COMMAND_LINE_SWITCH_u"/>) command line switch.</para></remarks>
    // BUG: This doesn't make sense. Packages aren't updated - newer version are downloaded.
    [Obsolete("Use Add with Version")]
    public string? Update { get; set; }

    /// <summary>
    /// Gets a semi-colon or comma-separated list of package IDs for packages that should be downloaded from the upstream repository and added to the local repository.
    /// </summary>
    /// <remarks>Packages that are referenced in this option as well as the <c>-d</c> (<see cref="COMMAND_LINE_SWITCH_d"/>) command line switch, will effectively be deleted and then re-added.
    /// <para>This is mapped from the <c>-a</c> (<see cref="COMMAND_LINE_SWITCH_a"/>) command line switch.</para></remarks>
    [Obsolete()]
    public string? Add { get; set; }

    /// <summary>
    /// Gets the path to a <c>.nupkg</c> file to import or the path to a subdirectory containing <c>.nupkg</c> files to import.
    /// </summary>
    /// <para>This is mapped from the <c>-i</c> (<see cref="COMMAND_LINE_SWITCH_i"/>) command line switch.</para></remarks>
    [Obsolete()]
    public string? Import { get; set; }

    /// <summary>
    /// Gets the path of the manifest file for the target NuGet feed that <see cref="CreateBundle"/> is intended for.
    /// </summary>
    /// <remarks>If this refers to a file that doesn't exist, then a new file will be created.
    /// <para>If this is not specified, then this will default to a file at the same location and base name as <see cref="CreateBundle"/>, but with a <c>.json</c> extension.</para>
    /// <para>This is mapped from the <c>-t</c> (<see cref="COMMAND_LINE_SWITCH_t"/>) command line switch.</para></remarks>
    [Obsolete()]
    public string? TargetManifestFile { get; set; }

    /// <summary>
    /// Gets the override value for the <see cref="UpstreamServiceIndex" /> setting.
    /// </summary>
    /// <remarks>If this refers to a subdirectory and is not absolute, it will be resolved relative to the current working directory (<see cref="Directory.GetCurrentDirectory"/>).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This is mapped from the <c>--upstream-service-index</c> (<see cref="COMMAND_LINE_SWITCH_upstream_2D_service_2D_index"/>) command line switch.</para></remarks>
    public string? OverrideUpstreamServiceIndex { get; set; }

    /// <summary>
    /// Gets the override value for the <see cref="LocalRepository" /> setting.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the current working directory (<see cref="Directory.GetCurrentDirectory"/>).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This is mapped from the <c>--local-repository</c> (<see cref="COMMAND_LINE_SWITCH_local_2D_repository"/>) command line switch.</para></remarks>
    public string? OverrideLocalRepository { get; set; }

    /// <summary>
    /// Gets the override value for the <see cref="GlobalPackagesFolder" /> setting.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the current working directory (<see cref="Directory.GetCurrentDirectory"/>).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This is mapped from the <c>--global-packages-folder</c> (<see cref="COMMAND_LINE_SWITCH_global_2D_packages_2D_folder"/>) command line switch.</para></remarks>
    public string? OverrideGlobalPackagesFolder { get; set; }

    /// <summary>
    /// Specifies the remote endpoint URL for the V3 NGet API or a subdirectory path that contains the upstream NuGet repository.
    /// </summary>
    /// <remarks>The default value of this setting is defined in the <see cref="DEFAULT_UPSTREAM_SERVICE_INDEX" /> constant.
    /// If this refers to a subdirectory and is not absolute, it will be resolved relative to the application assembly directory (<see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This can be overridden using the <c>--upstream-service-index</c> (<see cref="COMMAND_LINE_SWITCH_upstream_2D_service_2D_index"/>) command line switch.</para></remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/overview#service-index"/>
    public string UpstreamServiceIndex { get; set; } = null!;

    /// <summary>
    /// Specifies the relative or absolute path of a subdirectory for a local Nuget repository.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the application assembly directory (<see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />). The default value of this setting is defined in the <see cref="DEFAULT_LOCAL_REPOSITORY" /> constant.
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This can be overridden using the <c>--local-repository</c> (<see cref="COMMAND_LINE_SWITCH_local_2D_repository"/>) command line switch.</para></remarks>
    public string LocalRepository { get; set; } = null!;

    /// <summary>
    /// Specifies the relative or absolute path of the NuGet global packages folder.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the application assembly directory (<see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This can be overridden using the <c>--global-packages-folder</c> (<see cref="COMMAND_LINE_SWITCH_global_2D_packages_2D_folder"/>) command line switch.</para></remarks>
    public string GlobalPackagesFolder { get; set; } = null!;

    /// <summary>
    /// Gets or sets the value indicating whether to write help information to the console.
    /// </summary>
    /// <remarks>If this option is used, then all other options are ignored.
    /// <para>This is mapped from the <c>-?</c> (<see cref="COMMAND_LINE_SWITCH__3F_"/>), <c>-h</c> (<see cref="COMMAND_LINE_SWITCH_h"/>), and <c>--help</c> (<see cref="COMMAND_LINE_SWITCH_help"/>) command line switches.</para></remarks>
    public bool Help { get; set; }

    private static readonly ReadOnlyDictionary<string, string> _booleanSwitchMappings = new(new Dictionary<string, string>()
    {
        { COMMAND_LINE_SWITCH_update_2D_all, $"{nameof(NuGetPuller)}:{nameof(UpdateAll)}" },
        { COMMAND_LINE_SWITCH_l, $"{nameof(NuGetPuller)}:{nameof(ListLocal)}" },
        { COMMAND_LINE_SWITCH_h, $"{nameof(NuGetPuller)}:{nameof(Help)}" },
        { COMMAND_LINE_SWITCH__3F_, $"{nameof(NuGetPuller)}:{nameof(Help)}" },
        { COMMAND_LINE_SWITCH_help, $"{nameof(NuGetPuller)}:{nameof(Help)}" }
    });

    private static readonly ReadOnlyDictionary<string, string> _valueSwitchMappings = new(new Dictionary<string, string>()
    {
        { COMMAND_LINE_SWITCH_a, $"{nameof(NuGetPuller)}:{nameof(Add)}" },
        { COMMAND_LINE_SWITCH_d, $"{nameof(NuGetPuller)}:{nameof(Remove)}" },
        { COMMAND_LINE_SWITCH_i, $"{nameof(NuGetPuller)}:{nameof(Import)}" },
        { COMMAND_LINE_SWITCH_b, $"{nameof(NuGetPuller)}:{nameof(CreateBundle)}" },
        { COMMAND_LINE_SWITCH_t, $"{nameof(NuGetPuller)}:{nameof(TargetManifestFile)}" },
        { COMMAND_LINE_SWITCH_save_2D_target_2D_manifest_2D_as, $"{nameof(NuGetPuller)}:{nameof(SaveMetaDataTo)}" },
        { COMMAND_LINE_SWITCH_export_2D_local_2D_manifest, $"{nameof(NuGetPuller)}:{nameof(ExportMetaData)}" },
        { COMMAND_LINE_SWITCH_local_2D_repository, $"{nameof(NuGetPuller)}:{nameof(OverrideLocalRepository)}" },
        { COMMAND_LINE_SWITCH_upstream_2D_service_2D_index, $"{nameof(NuGetPuller)}:{nameof(OverrideLocalRepository)}" },
        { COMMAND_LINE_SWITCH_global_2D_packages_2D_folder, $"{nameof(NuGetPuller)}:{nameof(OverrideGlobalPackagesFolder)}" }
    });

    internal static void Configure(string[] args, IConfigurationBuilder builder)
    {
        builder.Add(new FlagSwitchCommandLineConfigSource(args?.ToImmutableArray() ?? [], _booleanSwitchMappings, _valueSwitchMappings));
    }
}