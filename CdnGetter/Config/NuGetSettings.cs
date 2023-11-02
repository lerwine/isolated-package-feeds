using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CdnGetter.Config;

public class NuGetSettings
{
    public const string DEFAULT_V3_API_URI = "https://api.nuget.org/v3/index.json";
    
    public const string DEFAULT_LOCAL_REPOSITORY = "LocalSource";
    
    /// <summary>
    /// Specifies the remote NuGet endpoint URL.
    /// </summary>
    /// <remarks>The default value of this setting is defined in the <see cref="DEFAULT_V3_API_URI" /> constant.</remarks>
    public string? V3ApiUrl { get; set; }
    
    /// <summary>
    /// Specifies the relative or absolute path of the local repository.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the <see cref="Microsoft.Extensions.Hosting.ContentRootPath" />. The default value of this setting is defined in the <see cref="DEFAULT_LOCAL_REPOSITORY" /> constant.</remarks>
    public string? LocalRepository { get; set; }
}