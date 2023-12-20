namespace NuGetPuller.Win;

public static class CommandLineSwitches
{
    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.OverrideUpstreamServiceIndex" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_upstream_2D_service_2D_index = "--upstream-service-index";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.OverrideDownloadedPackagesFolder" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_downloaded_2D_packages_2D_folder = "--downloaded-packages-folder";

    /// <summary>
    /// Gets the command line switch that sets the <see cref="AppSettings.OverrideGlobalPackagesFolder" /> value.
    /// </summary>
    public const string COMMAND_LINE_SWITCH_global_2D_packages_2D_folder = "--global-packages-folder";

}