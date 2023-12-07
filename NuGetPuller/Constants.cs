using System.Text.RegularExpressions;

namespace NuGetPuller;

public static partial class Constants
{
    public static readonly StringComparer NoCaseComparer = StringComparer.CurrentCultureIgnoreCase;

    public static readonly Regex NonNormalizedWhiteSpaceRegex = CreateNonNormalizedWhiteSpaceRegex();

    public static readonly Regex LineBreakRegex = CreateLineBreakRegex();

    /// <summary>
    /// Gets the command line switch that sets <see cref="AppSettings.UpdateAll" /> to <see langword="true"/>.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_update_2D_all = "--update-all";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.Update" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_u = "-u";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.Add" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_a = "-a";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.Import" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_i = "-i";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.Delete" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_d = "-d";

    /// <summary>
    /// Gets the command line switch that sets <see cref="AppSettings.ListLocal" /> to <see langword="true"/>.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_l = "-l";

    /// <summary>
    /// Gets the command line switch that sets <see cref="AppSettings.ExportBundle" /> to <see langword="true"/>.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_b = "-b";

    /// <summary>
    /// Gets the command line switch that sets <see cref="AppSettings.TargetManifestFile" /> to <see langword="true"/>.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_t = "-t";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.SaveTargetManifestAs" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_save_2D_target_2D_manifest_2D_as = "--save-target-manifest-as";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.ExportLocalManifest" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_export_2D_local_2D_manifest = "--export-local-manifest";

    /// <summary>
    /// Gets the default value for <see cref="AppSettings.UpstreamServiceIndex"/>.
    /// </summary>
    public const string DEFAULT_UPSTREAM_SERVICE_INDEX = "https://api.nuget.org/v3/index.json";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.OverrideUpstreamServiceIndex" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_upstream_2D_service_2D_index = "--upstream-service-index";

    /// <summary>
    /// The default path of the local repository, relative to the <see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />.
    /// </summary>
    public const string DEFAULT_LOCAL_REPOSITORY = "LocalSource";

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

    [GeneratedRegex(@"( |(?! ))[\r\n\s]+", RegexOptions.Compiled)]
    private static partial Regex CreateNonNormalizedWhiteSpaceRegex();

    [GeneratedRegex(@"\r?\n|\n", RegexOptions.Compiled)]
    private static partial Regex CreateLineBreakRegex();
}