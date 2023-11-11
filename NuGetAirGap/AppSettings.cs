using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Configuration;

namespace NuGetAirGap;

public class AppSettings
{
    #region ServiceIndexUrl

    /// <summary>
    /// The default remote endpoint URL for the V3 NGet API.
    /// </summary>
    public const string DEFAULT_SERVICE_INDEX_URL = "https://api.nuget.org/v3/index.json";

    public const char COMMAND_LINE_SWITCH_s = 's';

    /// <summary>
    /// Specifies the remote endpoint URL for the V3 NGet API.
    /// </summary>
    /// <remarks>The default value of this setting is defined in the <see cref="DEFAULT_SERVICE_INDEX_URL" /> constant.</remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/nuget/api/overview#service-index"/>
    public string? ServiceIndexUrl { get; set; }

    #endregion

    #region LocalRepository

    /// <summary>
    /// The default path of the local repository, relative to the <see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />.
    /// </summary>
    public const string DEFAULT_LOCAL_REPOSITORY = "LocalSource";

    public const char COMMAND_LINE_SWITCH_l = 'l';

    /// <summary>
    /// Specifies the relative or absolute path of the local repository.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the <see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />. The default value of this setting is defined in the <see cref="DEFAULT_LOCAL_REPOSITORY" /> constant.</remarks>
    public string? LocalRepository { get; set; }

    #endregion

    #region Help

    /// <summary>
    /// Gets the command line option for the <c><see cref="Config.AppSettings.Help" /></c> setting.
    /// </summary>
    public const char COMMAND_LINE_SWITCH_h = 'h';

    /// <summary>
    /// Gets the command line switch for the <see cref="Config.AppSettings.Help" /> application option option.
    /// </summary>
    public const char COMMAND_LINE_SWITCH__3F_ = '?';

    /// <summary>
    /// Gets or sets the value indicating whether to write help information to the console.
    /// </summary>
    /// <remarks>If this option is used, then all other options are ignored.</remarks>
    /// <remarks>This can be set with the -<see cref="SHORTHAND_h"/> or -<see cref="SHORTHAND__3F_"/> command line option.</remarks>
    public bool Help { get; set; }

    #endregion

    private static readonly ReadOnlyDictionary<string, string> _booleanSwitchMappings = new(new Dictionary<string, string>()
    {
        { $"-{COMMAND_LINE_SWITCH_h}", $"{nameof(NuGetAirGap)}:{nameof(Help)}" },
        { $"-{COMMAND_LINE_SWITCH__3F_}", $"{nameof(NuGetAirGap)}:{nameof(Help)}" }
    });

    private static readonly ReadOnlyDictionary<string, string> _valueSwitchMappings = new(new Dictionary<string, string>()
    {
        { $"-{COMMAND_LINE_SWITCH_s}", $"{nameof(NuGetAirGap)}:{nameof(LocalRepository)}" },
        { $"-{COMMAND_LINE_SWITCH_l}", $"{nameof(NuGetAirGap)}:{nameof(ServiceIndexUrl)}" }
    });

    internal static void Configure(string[] args, IConfigurationBuilder builder)
    {
        builder.Add(new FlagSwitchCommandLineConfigSource(args?.ToImmutableArray() ?? ImmutableArray<string>.Empty, _booleanSwitchMappings, _valueSwitchMappings));
    }
}