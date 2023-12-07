using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Configuration;
using static NuGetPuller.Constants;

namespace NuGetPuller;

public partial class AppSettings
{
    /// <summary>
    /// If true, then all existing packages in the local NuGet repository should be checked and updated to their latest versions.
    /// </summary>
    /// <remarks>This is mapped from the <c>--update-all</c> (<see cref="COMMAND_LINE_SWITCH_update_2D_all"/>) command line switch.</remarks>
    public bool UpdateAll { get; set; }

    /// <summary>
    /// Gets a comma-separated list of package IDs for packages in the local repository that should be checked and updated to their latest versions.
    /// </summary>
    /// <remarks>If a package is referenced in this option as well as the <c>-d</c> (<see cref="COMMAND_LINE_SWITCH_d"/>) command line switch, it will result in a warning stating that the package was not found in the local repository.
    /// <para>This is mapped from the <c>-u</c> (<see cref="COMMAND_LINE_SWITCH_u"/>) command line switch.</para></remarks>
    public string? Update { get; set; }

    /// <summary>
    /// Gets a comma-separated list of package IDs for packages that should be downloaded from the upstream repository and added to the local repository.
    /// </summary>
    /// <remarks>Packages that are referenced in this option as well as the <c>-d</c> (<see cref="COMMAND_LINE_SWITCH_d"/>) command line switch, will effectively be deleted and then re-added.
    /// <para>This is mapped from the <c>-a</c> (<see cref="COMMAND_LINE_SWITCH_a"/>) command line switch.</para></remarks>
    public string? Add { get; set; }

    /// <summary>
    /// Gets the path to a <c>.nupkg</c> file to import or the path to a subdirectory containing <c>.nupkg</c> files to import.
    /// </summary>
    /// <para>This is mapped from the <c>-i</c> (<see cref="COMMAND_LINE_SWITCH_i"/>) command line switch.</para></remarks>
    public string? Import { get; set; }

    /// <summary>
    /// Comma-separated list of package IDs for packages to be deleted from the local reository.
    /// </summary>
    /// <remarks>Packages that are referenced in this option as well as the <c>-a</c> (<see cref="COMMAND_LINE_SWITCH_a"/>) command line switch, will effectively be deleted and then re-added.
    /// <para>This is mapped from the <c>-d</c> (<see cref="COMMAND_LINE_SWITCH_d"/>) command line switch.</para></remarks>
    public string? Delete { get; set; }

    /// <summary>
    /// If true, then all packages in the local NuGet source will be displayed in the output.
    /// </summary>
    /// <remarks>This is mapped from the <c>-l</c> (<see cref="COMMAND_LINE_SWITCH_l"/>) command line switch.</remarks>
    public bool ListLocal { get; set; }

    /// <summary>
    /// Gets the path of the package transfer bundle to create.
    /// </summary>
    /// <remarks>This is mapped from the <c>-b</c> (<see cref="COMMAND_LINE_SWITCH_b"/>) command line switch.</remarks>
    public string? ExportBundle { get; set; }

    /// <summary>
    /// Gets the path of the manifest file for the target NuGet feed that <see cref="ExportBundle"/> is intended for.
    /// </summary>
    /// <remarks>If this refers to a file that doesn't exist, then a new file will be created.
    /// <para>If this is not specified, then this will default to a file at the same location and base name as <see cref="ExportBundle"/>, but with a <c>.json</c> extension.</para>
    /// <para>This is mapped from the <c>-t</c> (<see cref="COMMAND_LINE_SWITCH_t"/>) command line switch.</para></remarks>
    public string? TargetManifestFile { get; set; }

    /// <summary>
    /// Gets the path of the new manifest file for the target NuGet feed that includes the packages bundled in <see cref="TargetManifestFile"/>.
    /// </summary>
    /// <remarks>If this option is not specified, then <see cref="TargetManifestFile"/> will be overwritten to include the bundled package information.
    /// <para>This is mapped from the <c>--save-target-manifest-as</c> (<see cref="COMMAND_LINE_SWITCH_save_2D_target_2D_manifest_2D_as"/>) command line switch.</para></remarks>
    public string? SaveTargetManifestAs { get; set; }

    /// <summary>
    /// Gets the path to export the metadata for all packages in the local repository. This refers to a relative or absolute file path.
    /// </summary>
    /// <remarks>The package listing is exported as a JSON array. If this path is not absolute, it will be resolved relative to the current working directory (<see cref="Directory.GetCurrentDirectory"/>).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This is mapped from the <c>--export-local-manifest</c> (<see cref="COMMAND_LINE_SWITCH_export_2D_local_2D_manifest"/>) command line switch.</para></remarks>
    public string? ExportLocalManifest { get; set; }

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
    /// Gets the override value for the <see cref="UpstreamServiceIndex" /> setting.
    /// </summary>
    /// <remarks>If this refers to a subdirectory and is not absolute, it will be resolved relative to the current working directory (<see cref="Directory.GetCurrentDirectory"/>).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This is mapped from the <c>--upstream-service-index</c> (<see cref="COMMAND_LINE_SWITCH_upstream_2D_service_2D_index"/>) command line switch.</para></remarks>
    public string? OverrideUpstreamServiceIndex { get; set; }

    /// <summary>
    /// Specifies the relative or absolute path of a subdirectory for a local Nuget repository.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the application assembly directory (<see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />). The default value of this setting is defined in the <see cref="DEFAULT_LOCAL_REPOSITORY" /> constant.
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This can be overridden using the <c>--local-repository</c> (<see cref="COMMAND_LINE_SWITCH_local_2D_repository"/>) command line switch.</para></remarks>
    public string LocalRepository { get; set; } = null!;

    /// <summary>
    /// Gets the override value for the <see cref="LocalRepository" /> setting.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the current working directory (<see cref="Directory.GetCurrentDirectory"/>).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This is mapped from the <c>--local-repository</c> (<see cref="COMMAND_LINE_SWITCH_local_2D_repository"/>) command line switch.</para></remarks>
    public string? OverrideLocalRepository { get; set; }

    /// <summary>
    /// Specifies the relative or absolute path of the NuGet global packages folder.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the application assembly directory (<see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This can be overridden using the <c>--global-packages-folder</c> (<see cref="COMMAND_LINE_SWITCH_global_2D_packages_2D_folder"/>) command line switch.</para></remarks>
    public string GlobalPackagesFolder { get; set; } = null!;

    /// <summary>
    /// Gets the override value for the <see cref="GlobalPackagesFolder" /> setting.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the current working directory (<see cref="Directory.GetCurrentDirectory"/>).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This is mapped from the <c>--global-packages-folder</c> (<see cref="COMMAND_LINE_SWITCH_global_2D_packages_2D_folder"/>) command line switch.</para></remarks>
    public string? OverrideGlobalPackagesFolder { get; set; }

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
        { COMMAND_LINE_SWITCH_d, $"{nameof(NuGetPuller)}:{nameof(Delete)}" },
        { COMMAND_LINE_SWITCH_i, $"{nameof(NuGetPuller)}:{nameof(Import)}" },
        { COMMAND_LINE_SWITCH_b, $"{nameof(NuGetPuller)}:{nameof(ExportBundle)}" },
        { COMMAND_LINE_SWITCH_t, $"{nameof(NuGetPuller)}:{nameof(TargetManifestFile)}" },
        { COMMAND_LINE_SWITCH_save_2D_target_2D_manifest_2D_as, $"{nameof(NuGetPuller)}:{nameof(SaveTargetManifestAs)}" },
        { COMMAND_LINE_SWITCH_export_2D_local_2D_manifest, $"{nameof(NuGetPuller)}:{nameof(ExportLocalManifest)}" },
        { COMMAND_LINE_SWITCH_local_2D_repository, $"{nameof(NuGetPuller)}:{nameof(OverrideLocalRepository)}" },
        { COMMAND_LINE_SWITCH_upstream_2D_service_2D_index, $"{nameof(NuGetPuller)}:{nameof(OverrideLocalRepository)}" },
        { COMMAND_LINE_SWITCH_global_2D_packages_2D_folder, $"{nameof(NuGetPuller)}:{nameof(OverrideGlobalPackagesFolder)}" }
    });

    internal static void Configure(string[] args, IConfigurationBuilder builder)
    {
        builder.Add(new FlagSwitchCommandLineConfigSource(args?.ToImmutableArray() ?? [], _booleanSwitchMappings, _valueSwitchMappings));
    }

    internal ValidatedAppSettings Validated { get; } = new();
}