using System.Reflection;
using CdnGetter.Config;

namespace CdnGetter;

public static class Constants
{
    /// <summary>
    /// Default name of database file.
    /// </summary>
    public const string DEFAULT_DbFile = $"{nameof(CdnGetter)}.db";

    /// <summary>
    /// Default name of database file.
    /// </summary>
    public const string DEFAULT_LocalStoragePath = "Content";

    /// <summary>
    /// Gets the command line option for the <see cref="AppSettings.Upstream" /> setting.
    /// </summary>
    public const string SHORTHAND_u = "-u";

    /// <summary>
    /// Gets the command line option for the <see cref="AppSettings.Version" /> setting.
    /// </summary>
    public const string SHORTHAND_v = "-v";

    /// <summary>
    /// Gets the command line option for the <see cref="AppSettings.AddLibrary" /> setting.
    /// </summary>
    public const string SHORTHAND_a = "-a";

    /// <summary>
    /// Gets the command line option for the <see cref="AppSettings.GetNewVersions" /> setting.
    /// </summary>
    public const string SHORTHAND_n = "-n";

    /// <summary>
    /// Gets the command line option for the <see cref="AppSettings.RemoveLibrary" /> setting.
    /// </summary>
    public const string SHORTHAND_d = "-d";

    /// <summary>
    /// Gets the command line option for the <see cref="AppSettings.ReloadLibrary" /> setting.
    /// </summary>
    public const string SHORTHAND_r = "-r";

    /// <summary>
    /// Gets the command line option for the <see cref="AppSettings.ReloadExistingVersions" /> setting.
    /// </summary>
    public const string SHORTHAND_e = "-e";

    /// <summary>
    /// Gets the command line option for the <see cref="AppSettings.Library" /> setting.
    /// </summary>
    public const string SHORTHAND_l = "-l";

    /// <summary>
    /// Gets the command line option for the <see cref="AppSettings.Show" /> setting.
    /// </summary>
    public const string SHORTHAND_s = "-s";

    /// <summary>
    /// Gets the command line option for the <see cref="AppSettings.ShowHelp" /> setting.
    /// </summary>
    public const string SHORTHAND_h = "-h";

    /// <summary>
    /// Gets the command line switch for the <see cref="AppSettings.ShowHelp" /> application option option.
    /// </summary>
    public const string SHORTHAND__3F_ = "-?";

    /// <summary>
    /// Gets the command line switch for the <see cref="AppSettings.ShowHelp" /> application option option.
    /// </summary>
    public const string SHORTHAND_help = "--help";

    public const string SHOW_CDNs = "CDNs";

    public const string SHOW_Libraries = "Libraries";

    public const string SHOW_Versions = "Versions";

    public const string SHOW_Files = "Files";

    public static string GetDefaultSwitchName(string propertyName)
    {
        Type t = typeof(AppSettings);
        var propertyInfo = t.GetProperty(propertyName) ?? throw new InvalidOperationException("Property not found.");
        var attribute = propertyInfo.GetCustomAttribute<CommandLineShorthandAttribute>(false);
        return GetSwitchNames(propertyName).DefaultIfEmpty($"--{nameof(CdnGetter)}:{propertyInfo.Name}").First();
    }

    public static IEnumerable<string> GetSwitchNames(string propertyName)
    {
        Type t = typeof(AppSettings);
        var propertyInfo = t.GetProperty(propertyName);
        if (propertyInfo is null)
            throw new InvalidOperationException("Property not found.");
        var attribute = propertyInfo.GetCustomAttribute<CommandLineShorthandAttribute>(false);
        return (attribute is null) ? Enumerable.Empty<string>() : attribute.Switches;
    }

    public static string GetSwitchNameForLog(string propertyName) => GetSwitchNames(propertyName).Take(1).Select(s => $"{s} (--{nameof(CdnGetter)}:{propertyName})").DefaultIfEmpty($"--{nameof(CdnGetter)}:{propertyName}").First();
}
