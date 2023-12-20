using System.Collections.Immutable;
using System.Collections.ObjectModel;
using IsolatedPackageFeeds.Shared;
using Microsoft.Extensions.Configuration;
using static NuGetPuller.Win.CommandLineSwitches;

namespace NuGetPuller.Win;

public class AppSettings : ISharedAppSettings
{
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
    /// Gets the override value for the <see cref="GlobalPackagesFolder" /> setting.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the current working directory (<see cref="Directory.GetCurrentDirectory"/>).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This is mapped from the <c>--global-packages-folder</c> (<see cref="COMMAND_LINE_SWITCH_global_2D_packages_2D_folder"/>) command line argument.</para></remarks>
    public string? OverrideGlobalPackagesFolder { get; set; }

    private static readonly ReadOnlyDictionary<string, string> _booleanSwitchMappings = new(new Dictionary<string, string>());

    private static readonly ReadOnlyDictionary<string, string> _valueSwitchMappings = new(new Dictionary<string, string>()
    {
        { COMMAND_LINE_SWITCH_downloaded_2D_packages_2D_folder, $"{nameof(NuGetPuller)}:{nameof(OverrideDownloadedPackagesFolder)}" },
        { COMMAND_LINE_SWITCH_upstream_2D_service_2D_index, $"{nameof(NuGetPuller)}:{nameof(OverrideDownloadedPackagesFolder)}" },
        { COMMAND_LINE_SWITCH_global_2D_packages_2D_folder, $"{nameof(NuGetPuller)}:{nameof(OverrideGlobalPackagesFolder)}" }
    });

    internal static void Configure(string[] args, IConfigurationBuilder builder)
    {
        builder.Add(new FlagSwitchCommandLineConfigSource(args?.ToImmutableArray() ?? [], _booleanSwitchMappings, _valueSwitchMappings));
    }
}
