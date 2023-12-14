namespace NuGetPuller;

public static partial class ServiceDefaults
{
    /// <summary>
    /// Gets the default value for <see cref="ISharedAppSettings.UpstreamServiceIndexUrl"/>.
    /// </summary>
    public const string Default_Service_Index_URL = "https://api.nuget.org/v3/index.json";

    /// <summary>
    /// The default path of the local feed, relative to the <see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />.
    /// </summary>
    public const string Default_Local_Feed_Folder_Name = "LocalFeed";
}
