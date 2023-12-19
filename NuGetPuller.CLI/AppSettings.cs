using System.Collections.Immutable;
using System.Collections.ObjectModel;
using IsolatedPackageFeeds.Shared;
using Microsoft.Extensions.Configuration;
using static NuGetPuller.CLI.CommandLineSwitches;

namespace NuGetPuller.CLI;

public partial class AppSettings : ISharedAppSettings
{
    /// <summary>
    /// If true, then all packages in the Downloaded NuGet Packages Folder will be displayed in the output.
    /// </summary>
    /// <remarks>This is mapped from the <c>--list</c> (<see cref="COMMAND_LINE_SWITCH_list"/>) command line switch.</remarks>
    public bool ListDownloaded { get; set; }

    /// <summary>
    /// If true, then all packages in the Downloaded NuGet Packages Folder will be displayed in the output.
    /// </summary>
    /// <remarks>This is mapped from the <c>--include-versions</c> (<see cref="COMMAND_LINE_SWITCH_include_versions"/>) command line argument.</remarks>
    public bool IncludeVersions { get; set; }

    /// <summary>
    /// Comma-separated list of package IDs to be downloaded.
    /// </summary>
    /// <remarks>This is mapped from the <c>--download</c> (<see cref="COMMAND_LINE_SWITCH_download"/>) command line argument.</remarks>
    public string? Download { get; set; }

    /// <summary>
    /// Comma-separated list of package versions. This is used with the <c>--download</c> (<see cref="COMMAND_LINE_SWITCH_download"/>),
    /// <c>--remove</c> (<see cref="COMMAND_LINE_SWITCH_remove"/>), <c>--check-depencencies</c> (<see cref="COMMAND_LINE_SWITCH_check_depencencies"/>)
    /// and <c>--create-bundle</c> (<see cref="COMMAND_LINE_SWITCH_create_bundle"/>) command line argumentes.
    /// </summary>
    /// <remarks>This is mapped from the <c>--version</c> (<see cref="COMMAND_LINE_SWITCH_version"/>) command line argument.
    /// <para>In addition to version number strings, you can use the key word <c>all</c> to ensure that all versions are downloaded.</para></remarks>
    public string? Version { get; set; }

    /// <summary>
    /// This is used with the (<see cref="Download"/>) setting to refrain from downloading missing dependencies if <see cref="true"/>;
    /// otherwise, missing dependencies will be downloaded from the upstream repository, if present.
    /// </summary>
    /// <remarks>This is mapped from the <c>--no-dependencies</c> (<see cref="COMMAND_LINE_SWITCH_no_dependencies"/>) command line switch.</remarks>
    public bool NoDependencies { get; set; }

    /// <summary>
    /// Semi-colon-separated list of package file paths or subdirectory paths to add to the Downloaded NuGet Packages Folder.
    /// </summary>
    /// <remarks>This is mapped from the <c>--add-file</c> (<see cref="COMMAND_LINE_SWITCH_add_file"/>) command line argument.</remarks>
    public string? AddPackageFiles { get; set; }

    /// <summary>
    /// Comma-separated list of package IDs for packages to be deleted from the Downloaded NuGet Packages Folder.
    /// </summary>
    /// <remarks>This is mapped from the <c>--remove</c> (<see cref="COMMAND_LINE_SWITCH_remove"/>) command line argument.</remarks>
    public string? Remove { get; set; }

    /// <summary>
    /// This is used with the <see cref="Remove"/>) setting to specify the subdirectory to save packages to before they are removed.
    /// </summary>
    /// <remarks>This is mapped from the <c>--save-to</c> (<see cref="COMMAND_LINE_SWITCH_save_to"/>) command line argument.</remarks>
    public string? SaveTo { get; set; }

    /// <summary>
    /// Check to see if there are any missing dependencies for packages in the Downloaded NuGet Packages Folder.
    /// </summary>
    /// <remarks>This is mapped from the <c>--check-dependencies</c> (<see cref="COMMAND_LINE_SWITCH_check_depencencies"/>) command line switch.</remarks>
    public bool CheckDependencies { get; set; }

    /// <summary>
    /// Used along with <see cref="CheckDependencies"/> and <see cref="CreateBundle"/> to indicate a specific list of comma-separated packages IDs to check, rather than referencing all packages in the Downloaded NuGet Packages Folder.
    /// </summary>
    /// <remarks>This is mapped from the <c>--package-id</c> (<see cref="COMMAND_LINE_SWITCH_package_id"/>) command line argument.</remarks>
    public string? PackageId { get; set; }

    /// <summary>
    /// Used along with <see cref="CheckDependencies"/> to indicate that missing dependencies are not to be downloaded.
    /// </summary>This is mapped from the <c>--no-download</c> (<see cref="COMMAND_LINE_SWITCH_no_download"/>) command line switch.</remarks>
    public bool NoDownload { get; set; }

    /// <summary>
    /// Gets the path to export the metadata for all packages in the local feed. This refers to a relative or absolute file path.
    /// </summary>
    /// <remarks>The package listing is exported as a JSON array. If this path is not absolute, it will be resolved relative to the current working directory (<see cref="Directory.GetCurrentDirectory"/>).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This is mapped from the <c>--export-metadata</c> (<see cref="COMMAND_LINE_SWITCH_export_metadata"/>) command line argument.</para></remarks>
    public string? ExportMetaData { get; set; }

    /// <summary>
    /// Gets the path of the package transfer bundle to create.
    /// </summary>
    /// <remarks>This is mapped from the <c>--create-bundle</c> (<see cref="COMMAND_LINE_SWITCH_create_bundle"/>) command line argument.</remarks>
    public string? CreateBundle { get; set; }

    /// <summary>
    /// Optional path of package to package metadata json of another Local NuGet Feed.
    /// </summary>
    /// <remarks>This is mapped from the <c>--create-from</c> (<see cref="COMMAND_LINE_SWITCH_create_from"/>) command line argument.</remarks>
    public string? CreateFrom { get; set; }

    /// <summary>
    /// Optional path of to save updated metadata to.
    /// </summary>
    /// <remarks>If this option is not specified, then <see cref="CreateBundle"/> will be overwritten to include the bundled package information.
    /// If <see cref="CreateBundle"/> is not specified, then it will save the metadata to a file named after the current machine
    /// (incrementally adding a number to the end to avoid overwriting existing files), with an extension <c>.nuget.metadata.json</c>.
    /// <para>This is mapped from the <c>--save-metadata-to</c> (<see cref="COMMAND_LINE_SWITCH_save_metadata_to"/>) command line argument.</para></remarks>
    public string? SaveMetaDataTo { get; set; }

    /// <summary>
    /// Gets the override value for the <see cref="GlobalPackagesFolder" /> setting.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the current working directory (<see cref="Directory.GetCurrentDirectory"/>).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This is mapped from the <c>--global-packages-folder</c> (<see cref="COMMAND_LINE_SWITCH_global_2D_packages_2D_folder"/>) command line argument.</para></remarks>
    public string? OverrideGlobalPackagesFolder { get; set; }

    /// <summary>
    /// Specifies the remote endpoint URL for the V3 NGet API or a subdirectory path that contains the Upstream NuGet Feed.
    /// </summary>
    /// <remarks>The default value of this setting is defined in the <see cref="Default_Upstream_Service_Index_URL" /> constant.
    /// If this refers to a subdirectory and is not absolute, it will be resolved relative to the application assembly directory (<see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This can be overridden using the <c>--upstream-service-index</c> (<see cref="COMMAND_LINE_SWITCH_upstream_2D_service_2D_index"/>) command line argument.</para></remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/overview#service-index"/>
    public string UpstreamServiceIndexUrl { get; set; } = null!;

    /// <summary>
    /// Gets the override value for the <see cref="UpstreamServiceIndexUrl" /> setting.
    /// </summary>
    /// <remarks>If this refers to a subdirectory and is not absolute, it will be resolved relative to the current working directory (<see cref="Directory.GetCurrentDirectory"/>).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This is mapped from the <c>--upstream-service-index</c> (<see cref="COMMAND_LINE_SWITCH_upstream_2D_service_2D_index"/>) command line argument.</para></remarks>
    public string? OverrideUpstreamServiceIndex { get; set; }

    /// <summary>
    /// Specifies the relative or absolute path of a subdirectory for a Downloaded NuGet Packages Folder.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the application assembly directory (<see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />). The default value of this setting is defined in the <see cref="Default_Downloaded_Packages_Folder_Name" /> constant.
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This can be overridden using the <c>--downloaded-packages-folder</c> (<see cref="COMMAND_LINE_SWITCH_downloaded_2D_packages_2D_folder"/>) command line argument.</para></remarks>
    public string DownloadedPackagesFolder { get; set; } = null!;

    /// <summary>
    /// Gets the override value for the <see cref="DownloadedPackagesFolder" /> setting.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the current working directory (<see cref="Directory.GetCurrentDirectory"/>).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This is mapped from the <c>--downloaded-packages-folder</c> (<see cref="COMMAND_LINE_SWITCH_downloaded_2D_packages_2D_folder"/>) command line argument.</para></remarks>
    public string? OverrideDownloadedPackagesFolder { get; set; }

    /// <summary>
    /// Specifies the relative or absolute path of the NuGet global packages folder.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the application assembly directory (<see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This can be overridden using the <c>--global-packages-folder</c> (<see cref="COMMAND_LINE_SWITCH_global_2D_packages_2D_folder"/>) command line argument.</para></remarks>
    public string GlobalPackagesFolder { get; set; } = null!;

    /// <summary>
    /// Gets or sets the value indicating whether to write help information to the console.
    /// </summary>
    /// <remarks>If this option is used, then all other options are ignored.
    /// <para>This is mapped from the <c>-?</c> (<see cref="COMMAND_LINE_SWITCH__3F_"/>), <c>-h</c> (<see cref="COMMAND_LINE_SWITCH_h"/>), and <c>--help</c> (<see cref="COMMAND_LINE_SWITCH_help"/>) command line switches.</para></remarks>
    public bool Help { get; set; }

    private static readonly ReadOnlyDictionary<string, string> _booleanSwitchMappings = new(new Dictionary<string, string>()
    {
        { COMMAND_LINE_SWITCH_list, $"{nameof(NuGetPuller)}:{nameof(ListDownloaded)}" },
        { COMMAND_LINE_SWITCH_include_versions, $"{nameof(NuGetPuller)}:{nameof(IncludeVersions)}" },
        { COMMAND_LINE_SWITCH_no_dependencies, $"{nameof(NuGetPuller)}:{nameof(NoDependencies)}" },
        { COMMAND_LINE_SWITCH_check_depencencies, $"{nameof(NuGetPuller)}:{nameof(CheckDependencies)}" },
        { COMMAND_LINE_SWITCH_no_download, $"{nameof(NuGetPuller)}:{nameof(NoDownload)}" },
        { COMMAND_LINE_SWITCH_h, $"{nameof(NuGetPuller)}:{nameof(Help)}" },
        { COMMAND_LINE_SWITCH__3F_, $"{nameof(NuGetPuller)}:{nameof(Help)}" },
        { COMMAND_LINE_SWITCH_help, $"{nameof(NuGetPuller)}:{nameof(Help)}" }
    });

    private static readonly ReadOnlyDictionary<string, string> _valueSwitchMappings = new(new Dictionary<string, string>()
    {
        { COMMAND_LINE_SWITCH_download, $"{nameof(NuGetPuller)}:{nameof(Download)}" },
        { COMMAND_LINE_SWITCH_version, $"{nameof(NuGetPuller)}:{nameof(Version)}" },
        { COMMAND_LINE_SWITCH_add_file, $"{nameof(NuGetPuller)}:{nameof(AddPackageFiles)}" },
        { COMMAND_LINE_SWITCH_remove, $"{nameof(NuGetPuller)}:{nameof(Remove)}" },
        { COMMAND_LINE_SWITCH_save_to, $"{nameof(NuGetPuller)}:{nameof(SaveTo)}" },
        { COMMAND_LINE_SWITCH_package_id, $"{nameof(NuGetPuller)}:{nameof(PackageId)}" },
        { COMMAND_LINE_SWITCH_export_metadata, $"{nameof(NuGetPuller)}:{nameof(ExportMetaData)}" },
        { COMMAND_LINE_SWITCH_create_bundle, $"{nameof(NuGetPuller)}:{nameof(CreateBundle)}" },
        { COMMAND_LINE_SWITCH_create_from, $"{nameof(NuGetPuller)}:{nameof(CreateFrom)}" },
        { COMMAND_LINE_SWITCH_save_metadata_to, $"{nameof(NuGetPuller)}:{nameof(SaveMetaDataTo)}" },
        { COMMAND_LINE_SWITCH_downloaded_2D_packages_2D_folder, $"{nameof(NuGetPuller)}:{nameof(OverrideDownloadedPackagesFolder)}" },
        { COMMAND_LINE_SWITCH_upstream_2D_service_2D_index, $"{nameof(NuGetPuller)}:{nameof(OverrideDownloadedPackagesFolder)}" },
        { COMMAND_LINE_SWITCH_global_2D_packages_2D_folder, $"{nameof(NuGetPuller)}:{nameof(OverrideGlobalPackagesFolder)}" }
    });

    internal static void Configure(string[] args, IConfigurationBuilder builder)
    {
        builder.Add(new FlagSwitchCommandLineConfigSource(args?.ToImmutableArray() ?? [], _booleanSwitchMappings, _valueSwitchMappings));
    }
}