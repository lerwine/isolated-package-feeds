namespace NuGetPuller;

public static partial class ServiceDefaults
{
    /// <summary>
    /// Gets the default value for <see cref="ISharedAppSettings.UpstreamServiceIndex"/>.
    /// </summary>
    public const string DEFAULT_UPSTREAM_SERVICE_INDEX = "https://api.nuget.org/v3/index.json";

    /// <summary>
    /// The default path of the local repository, relative to the <see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />.
    /// </summary>
    public const string DEFAULT_LOCAL_REPOSITORY = "LocalSource";
}
