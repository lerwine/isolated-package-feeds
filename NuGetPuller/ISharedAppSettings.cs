namespace NuGetPuller;

/// <summary>
/// Common interface for NuGetPuller settings.
/// </summary>
public interface ISharedAppSettings
{
    /// <summary>
    /// Specifies the remote endpoint URL for the V3 NGet API or a subdirectory path that refers to the Upstream NuGet Feed.
    /// </summary>
    /// <remarks>The default value of this setting is defined in the <see cref="NuGetPullerStatic.Default_Service_Index_URL" /> constant.
    /// If this refers to a subdirectory and is not absolute, it will be resolved relative to the current working directory.
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para></remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/overview#service-index"/>
    string UpstreamServiceIndexUrl { get; set; }

    /// <summary>
    /// Override for the <see cref="UpstreamServiceIndexUrl"/> setting.
    /// </summary>
    string? OverrideUpstreamServiceIndex { get; }

    /// <summary>
    /// Specifies the relative or absolute path of a subdirectory for a Local Nuget Feed.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the current working directory.
    /// The default value of this setting is defined in the <see cref="NuGetPullerStatic.Default_Local_Feed_Folder_Name" /> constant.
    /// <para>You can use environment variables (<see cref="Environment.ExpandEnvironmentVariables(string)"/>) for specifying this option.</para></remarks>
    string LocalFeedPath { get; set; }

    /// <summary>
    /// Override for the <see cref="LocalFeedPath"/> setting.
    /// </summary>
    string? OverrideLocalFeed { get; }

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
