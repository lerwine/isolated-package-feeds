using System.Text.RegularExpressions;

namespace NuGetPuller;

public static partial class Constants
{
    public static readonly StringComparer NoCaseComparer = StringComparer.OrdinalIgnoreCase;
    
    public static readonly Regex NonNormalizedWhiteSpaceRegex = CreateNonNormalizedWhiteSpaceRegex();

    public static readonly Regex LineBreakRegex = CreateLineBreakRegex();

    /// <summary>
    /// Gets the command line switch that sets <see cref="UpdateAll" /> to <see langword="true"/>.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_update_2D_all = "--update-all";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="Update" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_u = "-u";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="Add" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_a = "-a";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="Delete" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_d = "-d";

    /// <summary>
    /// Gets the command line switch that sets <see cref="ListLocal" /> to <see langword="true"/>.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_l = "-l";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="ExportLocalMetaData" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_export_2D_local_2D_metadata = "--export-local-metadata";

    /// <summary>
    /// Gets the default value for <see cref="UpstreamServiceIndex"/>.
    /// </summary>
    public const string DEFAULT_UPSTREAM_SERVICE_INDEX = "https://api.nuget.org/v3/index.json";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="OverrideUpstreamServiceIndex" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_upstream_2D_service_2D_index = "--upstream-service-index";

    /// <summary>
    /// The default path of the local repository, relative to the <see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />.
    /// </summary>
    public const string DEFAULT_LOCAL_REPOSITORY = "LocalSource";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="OverrideLocalRepository" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_local_2D_repository = "--local-repository";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="OverrideGlobalPackagesFolder" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_global_2D_packages_2D_folder = "--global-packages-folder";

    /// <summary>
    /// Gets the command line switch that sets <see cref="Help" /> to <see langword="true"/>.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_help = "--help";

    /// <summary>
    /// Gets the command line switch that sets <see cref="Help" /> to <see langword="true"/>.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_h = "-h";

    /// <summary>
    /// Gets the command line switch that sets <see cref="Help" /> to <see langword="true"/>.
    /// </summary>
    public const string COMMAND_LINE_SWITCH__3F_ = "-?";

    [GeneratedRegex(@"( |(?! ))[\r\n\s]+", RegexOptions.Compiled)]
    private static partial Regex CreateNonNormalizedWhiteSpaceRegex();

    [GeneratedRegex(@"\r?\n|\n", RegexOptions.Compiled)]
    private static partial Regex CreateLineBreakRegex();
}