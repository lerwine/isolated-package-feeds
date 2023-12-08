namespace NuGetPuller;

public interface ISharedAppSettings
{
    /// <summary>
    /// Specifies the remote endpoint URL for the V3 NGet API or a subdirectory path that contains the upstream NuGet repository.
    /// </summary>
    /// <remarks>The default value of this setting is defined in the <see cref="DEFAULT_UPSTREAM_SERVICE_INDEX" /> constant.
    /// If this refers to a subdirectory and is not absolute, it will be resolved relative to the application assembly directory (<see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This can be overridden using the <c>--upstream-service-index</c> (<see cref="COMMAND_LINE_SWITCH_upstream_2D_service_2D_index"/>) command line switch.</para></remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/overview#service-index"/>
    string UpstreamServiceIndex { get; set; }

    string? OverrideUpstreamServiceIndex { get; }

    /// <summary>
    /// Specifies the relative or absolute path of a subdirectory for a local Nuget repository.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the application assembly directory (<see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />). The default value of this setting is defined in the <see cref="DEFAULT_LOCAL_REPOSITORY" /> constant.
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This can be overridden using the <c>--local-repository</c> (<see cref="COMMAND_LINE_SWITCH_local_2D_repository"/>) command line switch.</para></remarks>
    string LocalRepository { get; set; }

    string? OverrideLocalRepository { get; }

    /// <summary>
    /// Specifies the relative or absolute path of the NuGet global packages folder.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the application assembly directory (<see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />).
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para>
    /// <para>This can be overridden using the <c>--global-packages-folder</c> (<see cref="COMMAND_LINE_SWITCH_global_2D_packages_2D_folder"/>) command line switch.</para></remarks>
    string GlobalPackagesFolder { get; set; }

    string? OverrideGlobalPackagesFolder { get; }

    IValidatedSharedAppSettings Validated { get; }
}
