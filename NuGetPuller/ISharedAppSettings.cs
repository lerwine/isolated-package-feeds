namespace NuGetPuller;

/// <summary>
/// Common interface for NuGetPuller settings.
/// </summary>
public interface ISharedAppSettings
{
    /// <summary>
    /// Specifies the remote endpoint URL for the V3 NGet API or a subdirectory path that contains the upstream NuGet repository.
    /// </summary>
    /// <remarks>The default value of this setting is defined in the <see cref="ServiceDefaults.DEFAULT_UPSTREAM_SERVICE_INDEX" /> constant.
    /// If this refers to a subdirectory and is not absolute, it will be resolved relative to the current working directory.
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para></remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/overview#service-index"/>
    string UpstreamServiceIndex { get; set; }

    /// <summary>
    /// Override for the <see cref="UpstreamServiceIndex"/> setting.
    /// </summary>
    string? OverrideUpstreamServiceIndex { get; }

    /// <summary>
    /// Specifies the relative or absolute path of a subdirectory for a local Nuget repository.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the current working directory.
    /// The default value of this setting is defined in the <see cref="ServiceDefaults.DEFAULT_LOCAL_REPOSITORY" /> constant.
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para></remarks>
    string LocalRepository { get; set; }

    /// <summary>
    /// Override for the <see cref="LocalRepository"/> setting.
    /// </summary>
    string? OverrideLocalRepository { get; }

    /// <summary>
    /// Specifies the relative or absolute path of the NuGet global packages folder.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the current working directory.
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para></remarks>
    string GlobalPackagesFolder { get; set; }

    /// <summary>
    /// Override for the <see cref="GlobalPackagesFolder"/> setting.
    /// </summary>
    string? OverrideGlobalPackagesFolder { get; }
}
