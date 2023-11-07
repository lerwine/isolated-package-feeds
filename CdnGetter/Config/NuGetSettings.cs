namespace CdnGetter.Config;

public class NuGetSettings
{
    /// <summary>
    /// The default remote endpoint URL for the V3 NGet API.
    /// </summary>
    public const string DEFAULT_SERVICE_INDEX_URL = "https://api.nuget.org/v3/index.json";

    /// <summary>
    /// The default path of the local repository, relative to the <see cref="Microsoft.Extensions.Hosting.ContentRootPath" />.
    /// </summary>
    public const string DEFAULT_LOCAL_REPOSITORY = "LocalSource";

    /// <summary>
    /// Specifies the remote endpoint URL for the V3 NGet API.
    /// </summary>
    /// <remarks>The default value of this setting is defined in the <see cref="DEFAULT_SERVICE_INDEX_URL" /> constant.</remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/overview#service-index"/>
    public string? ServiceIndexUrl { get; set; }

    /// <summary>
    /// Specifies the relative or absolute path of the local repository.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the <see cref="Microsoft.Extensions.Hosting.ContentRootPath" />. The default value of this setting is defined in the <see cref="DEFAULT_LOCAL_REPOSITORY" /> constant.</remarks>
    public string? LocalRepository { get; set; }
}