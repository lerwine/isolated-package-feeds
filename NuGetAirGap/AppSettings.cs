using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Configuration;

namespace NuGetAirGap;

/// <summary>
/// Binds to application settings and command line options.
/// </summary>
public class AppSettings
{
    #region UpdateAll

    public const string COMMAND_LINE_SWITCH_update_2D_all = "--update-all";

    /// <summary>
    /// Update all existing packages in the local NuGet source.
    /// </summary>
    /// <remarks>This can be set with the <see cref="COMMAND_LINE_SWITCH_update_2D_all"/> command line option.</remarks>
    public bool UpdateAll { get; set; }

    #endregion

    #region Update

    public const string COMMAND_LINE_SWITCH_u = "-u";

    /// <summary>
    /// Comma-separated list of package IDs for packages in the local repository to update.
    /// </summary>
    /// <remarks>If a package is referenced in this option as well as the <see cref="Delete" /> option, it will be deleted and not updated.
    /// This can be specified using the <see cref="COMMAND_LINE_SWITCH_u"/> command line switch.</remarks>
    public string? Update { get; set; }

    #endregion

    #region Add

    public const string COMMAND_LINE_SWITCH_a = "-a";

    /// <summary>
    /// Comma-separated list of package IDs for packages to be added to the local repository.
    /// </summary>
    /// <remarks>If a package is referenced in this option as well as the <see cref="Delete" />, it will be deleted and then re-added.
    /// This can be specified using the <see cref="COMMAND_LINE_SWITCH_a"/> command line switch.</remarks>
    public string? Add { get; set; }

    #endregion

    #region Delete

    public const string COMMAND_LINE_SWITCH_d = "-d";

    /// <summary>
    /// Comma-separated list of package IDs for packages to be deleted from the local reository.
    /// </summary>
    /// <remarks>If a package is referenced in this option as well as the <see cref="Add" />, it will be deleted and then re-added.
    /// This can be specified using the <see cref="COMMAND_LINE_SWITCH_d"/> command line switch.</remarks>
    public string? Delete { get; set; }

    #endregion

    #region ListLocal

    public const string COMMAND_LINE_SWITCH_l = "-l";

    /// <summary>
    /// List packages in the local NuGet source.
    /// </summary>
    /// <remarks>This can be set with the <see cref="COMMAND_LINE_SWITCH_l"/> command line option.</remarks>
    public bool ListLocal { get; set; }

    #endregion

    #region ExportLocalPackageListing

    public const string COMMAND_LINE_SWITCH_export_2D_package_2D_listing = "--export-package-listing";

    /// <summary>
    /// Specifies the relative or absolute path of the local package list export.
    /// </summary>
    /// <remarks>The package listing is exported as a JSON array. If this path is not absolute, it will be resolved relative to the current working directory.
    /// This can be specified using the <see cref="COMMAND_LINE_SWITCH_export_2D_package_2D_listing"/> command line switch.</remarks>
    public string? ExportLocalPackageListing { get; }

    #endregion

    #region UpstreamServiceIndex

    /// <summary>
    /// The default remote endpoint URL for the V3 NGet API.
    /// </summary>
    public const string DEFAULT_UPSTREAM_SERVICE_INDEX = "https://api.nuget.org/v3/index.json";

    public const string COMMAND_LINE_SWITCH_upstream_2D_service_2D_index = "--upstream-service-index";

    /// <summary>
    /// Specifies the remote endpoint URL for the V3 NGet API.
    /// </summary>
    /// <remarks>The default value of this setting is defined in the <see cref="DEFAULT_UPSTREAM_SERVICE_INDEX" /> constant.
    /// This can be overridden using the <see cref="COMMAND_LINE_SWITCH_upstream_2D_service_2D_index"/> command line switch.</remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/overview#service-index"/>
    public string? UpstreamServiceIndex { get; set; }

    #endregion

    #region LocalRepository

    /// <summary>
    /// The default path of the local repository, relative to the <see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />.
    /// </summary>
    public const string DEFAULT_LOCAL_REPOSITORY = "LocalSource";

    public const string COMMAND_LINE_SWITCH_local_2D_repository = "--local-repository";

    /// <summary>
    /// Specifies the relative or absolute path of the local repository.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the current working directory. The default value of this setting is defined in the <see cref="DEFAULT_LOCAL_REPOSITORY" /> constant.
    /// This can be overridden using the <see cref="COMMAND_LINE_SWITCH_local_2D_repository"/> command line switch.</remarks>
    public string? LocalRepository { get; set; }

    #endregion

    #region GlobalPackagesFolder

    public const string COMMAND_LINE_SWITCH_global_2D_packages_2D_folder = "--global-packages-folder";

    /// <summary>
    /// Specifies the relative or absolute path of the NuGet global packages folder.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the current working directory.
    /// This can be overridden using the <see cref="COMMAND_LINE_SWITCH_global_2D_packages_2D_folder"/> command line switch.</remarks>
    public string? GlobalPackagesFolder { get; set; }

    #endregion

    #region Help

    /// <summary>
    /// Gets the command line option for the <c><see cref="Config.AppSettings.Help" /></c> setting.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_h = "-h";

    /// <summary>
    /// Gets the command line switch for the <see cref="Config.AppSettings.Help" /> application option option.
    /// </summary>
    public const string COMMAND_LINE_SWITCH__3F_ = "-?";

    /// <summary>
    /// Gets or sets the value indicating whether to write help information to the console.
    /// </summary>
    /// <remarks>If this option is used, then all other options are ignored.</remarks>
    /// <remarks>This can be set with the <see cref="SHORTHAND_h"/> or <see cref="SHORTHAND__3F_"/> command line option.</remarks>
    public bool Help { get; set; }

    #endregion

    private static readonly ReadOnlyDictionary<string, string> _booleanSwitchMappings = new(new Dictionary<string, string>()
    {
        { COMMAND_LINE_SWITCH_update_2D_all, $"{nameof(NuGetAirGap)}:{nameof(UpdateAll)}" },
        { COMMAND_LINE_SWITCH_l, $"{nameof(NuGetAirGap)}:{nameof(ListLocal)}" },
        { COMMAND_LINE_SWITCH_h, $"{nameof(NuGetAirGap)}:{nameof(Help)}" },
        { COMMAND_LINE_SWITCH__3F_, $"{nameof(NuGetAirGap)}:{nameof(Help)}" }
    });

    private static readonly ReadOnlyDictionary<string, string> _valueSwitchMappings = new(new Dictionary<string, string>()
    {
        { COMMAND_LINE_SWITCH_a, $"{nameof(NuGetAirGap)}:{nameof(Add)}" },
        { COMMAND_LINE_SWITCH_d, $"{nameof(NuGetAirGap)}:{nameof(Delete)}" },
        { COMMAND_LINE_SWITCH_export_2D_package_2D_listing, $"{nameof(NuGetAirGap)}:{nameof(ExportLocalPackageListing)}" },
        { COMMAND_LINE_SWITCH_local_2D_repository, $"{nameof(NuGetAirGap)}:{nameof(LocalRepository)}" },
        { COMMAND_LINE_SWITCH_upstream_2D_service_2D_index, $"{nameof(NuGetAirGap)}:{nameof(UpstreamServiceIndex)}" },
        { COMMAND_LINE_SWITCH_global_2D_packages_2D_folder, $"{nameof(NuGetAirGap)}:{nameof(GlobalPackagesFolder)}" }
    });

    internal static void Configure(string[] args, IConfigurationBuilder builder)
    {
        builder.Add(new FlagSwitchCommandLineConfigSource(args?.ToImmutableArray() ?? ImmutableArray<string>.Empty, _booleanSwitchMappings, _valueSwitchMappings));
    }
}