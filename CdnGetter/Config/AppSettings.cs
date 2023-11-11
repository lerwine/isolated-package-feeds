using Microsoft.Extensions.Configuration;
using static CdnGetter.Constants;

namespace CdnGetter.Config;
/// <summary>
/// Top-level section for custom app settings.
/// </summary>
public class AppSettings
{

    /// <summary>
    /// Specifies the relative or absolute path of the database file.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the <see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />. The default value of this setting is defined in the <see cref="DEFAULT_DbFile" /> constant.</remarks>
    public string? DbFile { get; set; }

    /// <summary>
    /// Specifies the path for local storage of CDN content files.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the <see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />. The default value of this setting is defined in the <see cref="DEFAULT_LocalStoragePath" /> constant.</remarks>
    public string? LocalStoragePath { get; set; }

    /// <summary>
    /// Configuration settings for the <see cref="CdnJsUpstreamCdn" />.
    /// </summary>
    public CdnJsSettings? CdnJs { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Model.UpstreamCdn.Name" /> of the upstream content delivery service.
    /// </summary>
    /// <remarks>This can be set with the -<see cref="SHORTHAND_u"/> command line option.</remarks>
    [CommandLineShorthand(SHORTHAND_u)]
    public string? Upstream { get; set; }

    /// <summary>
    /// Gets or sets the library version(s) strings that matches the string value of <see cref="Model.LocalVersion.Version"/>.
    /// </summary>
    /// <remarks>This can be set with the -<see cref="SHORTHAND_v"/> command line option.</remarks>
    [CommandLineShorthand(SHORTHAND_v)]
    public string? Version { get; set; }

    /// <summary>
    /// Gets the names of libraries on the upstream CDN to be added to the database.
    /// </summary>
    /// <remarks>
    ///     Mandatory Switches
    ///     <list type="bullet">
    ///         <item>
    ///             <term>-<see cref="SHORTHAND_a"/></term>
    ///             <description>The library name(s) on the upstream CDN to be added to the database.</description>
    ///         </item>
    ///         <item>
    ///             <term>-<see cref="SHORTHAND_u"/>=<c>name[,name,...]</c></term>
    ///             <description>The upstream CDN name(s) to retrieve libraries from.</description>
    ///         </item>
    ///     </list>
    ///     Optional Switch
    ///     <list type="bullet">
    ///         <item>
    ///             <term>-<see cref="SHORTHAND_v"/>=<c>string[,string,...]</c></term>
    ///             <description>The specific version(s) to add. If this is not specified, then all versions will be added.</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    [CommandLineShorthand(SHORTHAND_l)]
    public string? AddLibrary { get; set; }

    /// <summary>
    /// Gets names of libraries in the database that are to be checked for new versions on the upstream CDN.
    /// </summary>
    /// <remarks>
    ///     Mandatory Switches
    ///     <list type="bullet">
    ///         <item>
    ///             <term>-<see cref="SHORTHAND_n"/>=<c>library_name[,library_name,...]</c></term>
    ///             <description>The library name(s) on the upstream CDN to be added to the database.</description>
    ///         </item>
    ///     </list>
    ///     Optional Switch
    ///     <list type="bullet">
    ///         <item>
    ///             <term>-<see cref="SHORTHAND_u"/>=<c>name[,name,...]</c></term>
    ///             <description>The upstream CDN name(s) to retrieve libraries from. If this is not specified, then new versions will be retrieved from all CDNs.</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    [CommandLineShorthand(SHORTHAND_n)]
    public string? GetNewVersions { get; set; }

    /// <summary>
    /// Gets names of libraries to remove from the database.
    /// </summary>
    /// <remarks>
    ///     Mandatory Switch
    ///     <list type="bullet">
    ///         <item>
    ///             <term>-<see cref="SHORTHAND_d"/>y=<c>name[,name,...]</c></term>
    ///             <description>The library name(s) to remove from the database.</description>
    ///         </item>
    ///     </list>
    ///     Optional Switches
    ///     <list type="bullet">
    ///         <item>
    ///             <term>-<see cref="SHORTHAND_u"/>=<c>name[,name,...]</c></term>
    ///             <description>The explicit upstream CDN name(s) to remove local libraries from. If this is not specified, then all matching libraries will be removed.</description>
    ///         </item>
    ///         <item>
    ///             <term>-<see cref="SHORTHAND_v"/>=<c>string[,string,...]</c></term>
    ///             <description>The specific version(s) to remove. If this is not specified, then all versions of matching libraries will be removed.</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    [CommandLineShorthand(SHORTHAND_d)]
    public string? RemoveLibrary { get; set; }

    /// <summary>
    /// Gets names of libraries in the database that are to be reloaded from the upstream CDN.
    /// </summary>
    /// <remarks>
    ///     Mandatory Switch
    ///     <list type="bullet">
    ///         <item>
    ///             <term>-<see cref="SHORTHAND_r"/>=<c>name[,name,...]</c></term>
    ///             <description>The library name(s) on the upstream CDN to be reloaded.</description>
    ///         </item>
    ///     </list>
    ///     Optional Switch
    ///     <list type="bullet">
    ///         <item>
    ///             <term>-<see cref="SHORTHAND_u"/>=<c>string[,string,...]</c></term>
    ///             <description>The specific CDN(s) to reload. If this is not specified, then libraries will be reloaded regardless of the CDN.</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    [CommandLineShorthand(SHORTHAND_r)]
    public string? ReloadLibrary { get; set; }

    /// <summary>
    /// Gets names of libraries in the database whose existing library versions are to be reloaded from the upstream CDN.
    /// </summary>
    /// <remarks>
    ///     Parameter Set #1: Mandatory Switches
    ///     <list type="bullet">
    ///         <item>
    ///             <term>-<see cref="SHORTHAND_e"/>=<c>name[,name,...]</c></term>
    ///             <description>The library name(s) on the upstream CDN to be reloaded.</description>
    ///         </item>
    ///         <item>
    ///             <term>-<see cref="SHORTHAND_u"/>=<c>name[,name,...]</c></term>
    ///             <description>The upstream CDN name(s) to retrieve libraries from.</description>
    ///         </item>
    ///     </list>
    ///     Optional Switch
    ///     <list type="bullet">
    ///         <item>
    ///             <term>-<see cref="SHORTHAND_v"/>=<c>string[,string,...]</c></term>
    ///             <description>The specific version(s) to reload. If this is not specified, then all versions of matching libraries will be reloaded.</description>
    ///         </item>
    ///     </list>
    ///     <para>
    ///         Parameter Set #2: Mandatory Switches
    ///         <list type="bullet">
    ///             <item>
    ///                 <term>-<see cref="SHORTHAND_e"/>=<c>name[,name,...]</c></term>
    ///                 <description>The library name(s) to be reloaded.</description>
    ///             </item>
    ///             <item>
    ///                 <term>-<see cref="SHORTHAND_v"/>=<c>string[,string,...]</c></term>
    ///                 <description>The specific version(s) to reload.</description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    [CommandLineShorthand(SHORTHAND_e)]
    public string? ReloadExistingVersions { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Model.LocalLibrary.Name" />(s) of local librares.
    /// </summary>
    /// <remarks>This can be set with the -<see cref="SHORTHAND_l"/> command line option.</remarks>
    [CommandLineShorthand(SHORTHAND_l)]
    public string? Library { get; set; }

    /// <summary>
    /// Display information.
    /// </summary>
    /// <remarks>
    ///     Show CDNs
    ///     <list type="bullet">
    ///         <item>
    ///             <term>-<see cref="SHORTHAND_s"/>=<see cref="SHOW_CDNs"/></term>
    ///             <description>Show the upstream CDN names in the database.</description>
    ///         </item>
    ///     </list>
    ///     <para>
    ///         Show Libraries
    ///         <list type="bullet">
    ///             <item>
    ///                 <term>-<see cref="SHORTHAND_s"/>=<see cref="SHOW_Libraries"/></term>
    ///                 <description>Show the library names in the database.</description>
    ///             </item>
    ///         </list>
    ///         Optional Parameter
    ///         <list type="bullet">
    ///             <item>
    ///                 <term>-<see cref="SHORTHAND_u"/>=<c>name[,name,...]</c></term>
    ///                 <description>The upstream CDN name(s) to show local libraries for.</description>
    ///             </item>
    ///         </list>
    ///     </para>
    ///     <para>
    ///         Show Versions
    ///         <list type="bullet">
    ///             <item>
    ///                 <term>-<see cref="SHORTHAND_s"/>=<see cref="SHOW_Versions"/></term>
    ///                 <description>Show the versions in the database.</description>
    ///             </item>
    ///             <item>
    ///                 <term>-<see cref="SHORTHAND_l"/>=<c>name[,name,...]</c></term>
    ///                 <description>The library name(s) to show versions for.</description>
    ///             </item>
    ///         </list>
    ///         Optional Parameter
    ///         <list type="bullet">
    ///             <item>
    ///                 <term>-<see cref="SHORTHAND_u"/>=<c>name[,name,...]</c></term>
    ///                 <description>The upstream CDN name(s) to show local libraries for.</description>
    ///             </item>
    ///         </list>
    ///     </para>
    ///     <para>
    ///         Show Files
    ///         <list type="bullet">
    ///             <item>
    ///                 <term>-<see cref="SHORTHAND_s"/>=<see cref="SHOW_Files"/></term>
    ///                 <description>Show the upstream CDN file names in the database.</description>
    ///             </item>
    ///             <item>
    ///                 <term>-<see cref="SHORTHAND_l"/>=<c>name[,name,...]</c></term>
    ///                 <description>The library name(s) to show files for.</description>
    ///             </item>
    ///             <item>
    ///                 <term>-<see cref="SHORTHAND_v"/>=<c>string[,string,...]</c></term>
    ///                 <description>The library version(s) to show files for.</description>
    ///             </item>
    ///             <item>
    ///                 <term>-<see cref="SHORTHAND_u"/>=<c>name[,name,...]</c></term>
    ///                 <description>The upstream CDN name(s) to show files for.</description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    [CommandLineShorthand(SHORTHAND_s)]
    public string? Show { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether to write help information to the console.
    /// </summary>
    /// <remarks>If this option is used, then all other options are ignored.</remarks>
    /// <remarks>This can be set with the -<see cref="SHORTHAND_h"/> or -<see cref="SHORTHAND__3F_"/> command line option.</remarks>
    public bool? Help { get; set; }

    public bool ShowHelp() => Help ?? false;
    
    private static readonly Dictionary<string, string> _switchMappings = new()
    {
        { $"-{SHORTHAND_u}", $"{nameof(CdnGetter)}:{nameof(Upstream)}" },
        { $"-{SHORTHAND_v}", $"{nameof(CdnGetter)}:{nameof(Version)}" },
        { $"-{SHORTHAND_a}", $"{nameof(CdnGetter)}:{nameof(AddLibrary)}" },
        { $"-{SHORTHAND_n}", $"{nameof(CdnGetter)}:{nameof(GetNewVersions)}" },
        { $"-{SHORTHAND_d}", $"{nameof(CdnGetter)}:{nameof(RemoveLibrary)}" },
        { $"-{SHORTHAND_r}", $"{nameof(CdnGetter)}:{nameof(ReloadLibrary)}" },
        { $"-{SHORTHAND_e}", $"{nameof(CdnGetter)}:{nameof(ReloadExistingVersions)}" },
        { $"-{SHORTHAND_l}", $"{nameof(CdnGetter)}:{nameof(Library)}" },
        { $"-{SHORTHAND_s}", $"{nameof(CdnGetter)}:{nameof(Show)}" },
        { $"-{SHORTHAND_h}", $"{nameof(CdnGetter)}:{nameof(Help)}" },
        { $"-{SHORTHAND__3F_}", $"{nameof(CdnGetter)}:{nameof(Help)}" }
    };

    internal static void Configure(string[] args, IConfigurationBuilder configuration) => configuration.AddCommandLine(args, _switchMappings);

    public static string GetLocalStoragePath(AppSettings? settings) { return (settings?.LocalStoragePath).ToTrimmedOrDefaultIfEmpty(DEFAULT_LocalStoragePath); }
}
