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
    /// Gets the command line option for the <c><see cref="Config.AppSettings.Upstream" /></c> setting.
    /// </summary>
    public const char SHORTHAND_u = 'u';

    /// <summary>
    /// Gets the command line option for the <c><see cref="Config.AppSettings.Version" /></c> setting.
    /// </summary>
    public const char SHORTHAND_v = 'v';

    /// <summary>
    /// Gets the command line option for the <c><see cref="Config.AppSettings.AddLibrary" /></c> setting.
    /// </summary>
    public const char SHORTHAND_a = 'a';

    /// <summary>
    /// Gets the command line option for the <c><see cref="Config.AppSettings.GetNewVersions" /></c> setting.
    /// </summary>
    public const char SHORTHAND_n = 'n';

    /// <summary>
    /// Gets the command line option for the <c><see cref="Config.AppSettings.RemoveLibrary" /></c> setting.
    /// </summary>
    public const char SHORTHAND_d = 'd';

    /// <summary>
    /// Gets the command line option for the <c><see cref="Config.AppSettings.ReloadLibrary" /></c> setting.
    /// </summary>
    public const char SHORTHAND_r = 'r';

    /// <summary>
    /// Gets the command line option for the <c><see cref="Config.AppSettings.ReloadExistingVersions" /></c> setting.
    /// </summary>
    public const char SHORTHAND_e = 'e';

    /// <summary>
    /// Gets the command line option for the <c><see cref="Config.AppSettings.Library" /></c> setting.
    /// </summary>
    public const char SHORTHAND_l = 'l';

    /// <summary>
    /// Gets the command line option for the <c><see cref="Config.AppSettings.Show" /></c> setting.
    /// </summary>
    public const char SHORTHAND_s = 's';

    /// <summary>
    /// Gets the command line option for the <c><see cref="Config.AppSettings.Help" /></c> setting.
    /// </summary>
    public const char SHORTHAND_h = 'h';

    /// <summary>
    /// Gets the command line switch for the <see cref="Config.AppSettings.Help" /> application option option.
    /// </summary>
    public const char SHORTHAND__3F_ = '?';

    public const string SHOW_CDNs = "CDNs";

    public const string SHOW_Libraries = "Libraries";

    public const string SHOW_Versions = "Versions";

    public const string SHOW_Files = "Files";

    public static string GetSwitchName(string propertyName)
    {
        Type t = typeof(AppSettings);
        var propertyInfo = t.GetProperty(propertyName);
        if (propertyInfo is null)
            throw new InvalidOperationException("Property not found.");
        var attribute = propertyInfo.GetCustomAttribute<CommandLineShorthandAttribute>(false);
        return (attribute is null) ? $"--{nameof(CdnGetter)}:{propertyInfo.Name}" : $"-{attribute.SwitchCharacter}";
    }

    public static string GetSwitchNameForLog(string propertyName)
    {
        Type t = typeof(AppSettings);
        var propertyInfo = t.GetProperty(propertyName);
        if (propertyInfo is null)
            throw new InvalidOperationException("Property not found.");
        var attribute = propertyInfo.GetCustomAttribute<CommandLineShorthandAttribute>(false);
        return (attribute is null) ? $"--{nameof(CdnGetter)}:{propertyInfo.Name}" : $"-{attribute.SwitchCharacter} (--{nameof(CdnGetter)}:{propertyInfo.Name})";
    }
}
