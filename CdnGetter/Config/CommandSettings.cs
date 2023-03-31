namespace CdnGetter.Config;

public class CommandSettings
{
    /// <summary>
    /// The <see cref="Model.UpstreamCdn.Name" /> of the upstream content delivery service.
    /// </summary>
    public List<string>? Upstream { get; set; }

    /// <summary>
    /// Gets the library versions.
    /// </summary>
    public List<string>? Version { get; set; }

    /// <summary>
    /// Gets the names of libraries on the upstream CDN to be added to the database.
    /// </summary>
    /// <remarks>
    ///     Mandatory Switches
    ///     <list type="bullet">
    ///         <item>
    ///             <term>--AddLibrary=<c>name[,name,...]</c></term>
    ///             <description>The library name(s) on the upstream CDN to be added to the database.</description>
    ///         </item>
    ///         <item>
    ///             <term>--Upstream=<c>name[,name,...]</c></term>
    ///             <description>The upstream CDN name(s) to retrieve libraries from.</description>
    ///         </item>
    ///     </list>
    ///     Optional Switch
    ///     <list type="bullet">
    ///         <item>
    ///             <term>--Version=<c>string[,string,...]</c></term>
    ///             <description>The specific version(s) to add. If this is not specified, then all versions will be added.</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public List<string>? AddLibrary { get; set; }

    /// <summary>
    /// Gets names of libraries in the database that are to be checked for new versions on the upstream CDN.
    /// </summary>
    /// <remarks>
    ///     Mandatory Switches
    ///     <list type="bullet">
    ///         <item>
    ///             <term>--GetNewVersions=<c>library_name[,library_name,...]</c></term>
    ///             <description>The library name(s) on the upstream CDN to be added to the database.</description>
    ///         </item>
    ///     </list>
    ///     Optional Switch
    ///     <list type="bullet">
    ///         <item>
    ///             <term>--Upstream=<c>name[,name,...]</c></term>
    ///             <description>The upstream CDN name(s) to retrieve libraries from. If this is not specified, then new versions will be retrieved from all CDNs.</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public List<string>? GetNewVersions { get; set; }

    /// <summary>
    /// Gets names of libraries to remove from the database.
    /// </summary>
    /// <remarks>
    ///     Mandatory Switch
    ///     <list type="bullet">
    ///         <item>
    ///             <term>--RemoveLibrary=<c>name[,name,...]</c></term>
    ///             <description>The library name(s) to remove from the database.</description>
    ///         </item>
    ///     </list>
    ///     Optional Switches
    ///     <list type="bullet">
    ///         <item>
    ///             <term>--Upstream=<c>name[,name,...]</c></term>
    ///             <description>The explicit upstream CDN name(s) to remove local libraries from. If this is not specified, then all matching libraries will be removed.</description>
    ///         </item>
    ///         <item>
    ///             <term>--Version=<c>string[,string,...]</c></term>
    ///             <description>The specific version(s) to remove. If this is not specified, then all versions of matching libraries will be removed.</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public List<string>? RemoveLibrary { get; set; }

    /// <summary>
    /// Gets names of libraries in the database that are to be reloaded from the upstream CDN.
    /// </summary>
    /// <remarks>
    ///     Parameter Set #1: Mandatory Switches
    ///     <list type="bullet">
    ///         <item>
    ///             <term>---ReloadLibrary=<c>name[,name,...]</c></term>
    ///             <description>The library name(s) on the upstream CDN to be reloaded.</description>
    ///         </item>
    ///         <item>
    ///             <term>--Upstream=<c>name[,name,...]</c></term>
    ///             <description>The upstream CDN name(s) to retrieve libraries from.</description>
    ///         </item>
    ///     </list>
    ///     Optional Switch
    ///     <list type="bullet">
    ///         <item>
    ///             <term>--Version=<c>string[,string,...]</c></term>
    ///             <description>The specific version(s) to reload. If this is not specified, then all versions of matching libraries will be reloaded.</description>
    ///         </item>
    ///     </list>
    ///     <para>
    ///         Parameter Set #2: Mandatory Switches
    ///         <list type="bullet">
    ///             <item>
    ///                 <term>--ReloadLibrary=<c>name[,name,...]</c></term>
    ///                 <description>The library name(s) to be reloaded.</description>
    ///             </item>
    ///             <item>
    ///                 <term>--Version=<c>string[,string,...]</c></term>
    ///                 <description>The specific version(s) to reload.</description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    public List<string>? ReloadLibrary { get; set; }

    /// <summary>
    /// Gets names of libraries in the database whose existing library versions are to be reloaded from the upstream CDN.
    /// </summary>
    /// <remarks>
    ///     Parameter Set #1: Mandatory Switches
    ///     <list type="bullet">
    ///         <item>
    ///             <term>--ReloadExistingVersions=<c>name[,name,...]</c></term>
    ///             <description>The library name(s) on the upstream CDN to be reloaded.</description>
    ///         </item>
    ///         <item>
    ///             <term>--Upstream=<c>name[,name,...]</c></term>
    ///             <description>The upstream CDN name(s) to retrieve libraries from.</description>
    ///         </item>
    ///     </list>
    ///     Optional Switch
    ///     <list type="bullet">
    ///         <item>
    ///             <term>--Version=<c>string[,string,...]</c></term>
    ///             <description>The specific version(s) to reload. If this is not specified, then all versions of matching libraries will be reloaded.</description>
    ///         </item>
    ///     </list>
    ///     <para>
    ///         Parameter Set #2: Mandatory Switches
    ///         <list type="bullet">
    ///             <item>
    ///                 <term>--ReloadExistingVersions=<c>name[,name,...]</c></term>
    ///                 <description>The library name(s) to be reloaded.</description>
    ///             </item>
    ///             <item>
    ///                 <term>--Version=<c>string[,string,...]</c></term>
    ///                 <description>The specific version(s) to reload.</description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    public List<string>? ReloadExistingVersions { get; set; }

    /// <summary>
    /// The <see cref="Model.LocalLibrary.Name" />(s) of the local librares.
    /// </summary>
    public List<string>? Library { get; set; }
    
    public const string SHOW_CDNs = "CDNs";
    
    public const string SHOW_Libraries = "Libraries";
    
    public const string SHOW_Versions = "Versions";
    
    public const string SHOW_Files = "Files";
    
    /// <summary>
    /// Display information.
    /// </summary>
    /// <remarks>
    ///     Show CDNs
    ///     <list type="bullet">
    ///         <item>
    ///             <term>--Show=CDNs</term>
    ///             <description>Show the upstream CDN names in the database.</description>
    ///         </item>
    ///     </list>
    ///     <para>
    ///         Show Libraries
    ///         <list type="bullet">
    ///             <item>
    ///                 <term>--Show=Libraries</term>
    ///                 <description>Show the upstream CDN names in the database.</description>
    ///             </item>
    ///         </list>
    ///         Optional Parameter
    ///         <list type="bullet">
    ///             <item>
    ///                 <term>--Upstream=<c>name[,name,...]</c></term>
    ///                 <description>The upstream CDN name(s) to show local libraries for.</description>
    ///             </item>
    ///         </list>
    ///     </para>
    ///     <para>
    ///         Show Versions
    ///         <list type="bullet">
    ///             <item>
    ///                 <term>--Show=Versions</term>
    ///                 <description>Show the upstream CDN names in the database.</description>
    ///             </item>
    ///             <item>
    ///                 <term>--Library=<c>name[,name,...]</c></term>
    ///                 <description>The library name(s) to show versions for.</description>
    ///             </item>
    ///         </list>
    ///         Optional Parameter
    ///         <list type="bullet">
    ///             <item>
    ///                 <term>--Upstream=<c>name[,name,...]</c></term>
    ///                 <description>The upstream CDN name(s) to show local libraries for.</description>
    ///             </item>
    ///         </list>
    ///     </para>
    ///     <para>
    ///         Show Files
    ///         <list type="bullet">
    ///             <item>
    ///                 <term>--Show=Files</term>
    ///                 <description>Show the upstream CDN file names in the database.</description>
    ///             </item>
    ///             <item>
    ///                 <term>--Library=<c>name[,name,...]</c></term>
    ///                 <description>The library name(s) to show files for.</description>
    ///             </item>
    ///             <item>
    ///                 <term>--Version=<c>string[,string,...]</c></term>
    ///                 <description>The library version(s) to show files for.</description>
    ///             </item>
    ///             <item>
    ///                 <term>--Upstream=<c>name[,name,...]</c></term>
    ///                 <description>The upstream CDN name(s) to show files for.</description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    public string? Show { get; set; }
}