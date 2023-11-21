using System.Collections.Immutable;
using CdnGetter.Config;
using CdnGetter.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static CdnGetter.Constants;

namespace CdnGetter.Services;

public partial class MainService : BackgroundService
{
    internal static readonly StringComparer NameComparer = StringComparer.InvariantCultureIgnoreCase;
    private readonly ILogger<MainService> _logger;
    private readonly IServiceScope _scope;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly OperationType? _operation;
    private readonly ImmutableArray<string> _libraryNames;
    private readonly ImmutableArray<string> _versions;
    private readonly ImmutableArray<string> _cdnNames;

    private static void WriteDescription(string description)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(description);
    }

    private static void WriteShowCDNsHelpToConsole()
    {
        string exe = Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;
        Console.BackgroundColor = ConsoleColor.Black;
        try
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine($"{exe} {GetDefaultSwitchName(nameof(AppSettings.Show))}={SHOW_CDNs}");
            WriteDescription("Show the upstream CDN names in the database.");
        }
        finally { Console.ResetColor(); }
    }

    private static void WriteShowLibrariesHelpToConsole()
    {
        string exe = Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;
        Console.BackgroundColor = ConsoleColor.Black;
        try
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine($"{exe} {GetDefaultSwitchName(nameof(AppSettings.Show))}={SHOW_Libraries}");
            WriteDescription("Show the libraries in the database.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Optional switch:");
            Console.Write($"\t{GetDefaultSwitchName(nameof(AppSettings.Upstream))}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("cdn_name");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(",cdn_name,...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - The CDN name(s) to show libraries for.");
        }
        finally { Console.ResetColor(); }
    }

    private static void WriteShowVersionsHelpToConsole()
    {
        string exe = Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;
        Console.BackgroundColor = ConsoleColor.Black;
        try
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.Write($"{exe} {GetDefaultSwitchName(nameof(AppSettings.Show))}={SHOW_Versions} {GetDefaultSwitchName(nameof(AppSettings.Library))}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("library_name");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(",library_name,...");
            WriteDescription("Show the library versions in the database.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Optional switch:");
            Console.Write($"\t{GetDefaultSwitchName(nameof(AppSettings.Upstream))}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("cdn_name");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(",cdn_name,...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - The CDN name(s) to show library versions for.");
        }
        finally { Console.ResetColor(); }
    }

    private static void WriteShowFilesHelpToConsole()
    {
        string exe = Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;
        Console.BackgroundColor = ConsoleColor.Black;
        try
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.Write($"{exe} {GetDefaultSwitchName(nameof(AppSettings.Show))}={SHOW_Files} {GetDefaultSwitchName(nameof(AppSettings.Library))}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("library_name");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(",library_name,...");
            WriteDescription("Show the library files in the database.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Optional switches:");
            Console.Write($"\t{GetDefaultSwitchName(nameof(AppSettings.Version))}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("version_string");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(",version_string,...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - The library version(s) to show files for.");
            Console.Write($"\t{GetDefaultSwitchName(nameof(AppSettings.Upstream))}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("cdn_name");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(",cdn_name,...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - The CDN name(s) to show files for.");
        }
        finally { Console.ResetColor(); }
    }

    private static void WriteAddLibrariesHelpToConsole()
    {
        string exe = Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;
        Console.BackgroundColor = ConsoleColor.Black;
        try
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.Write($"{exe} {GetDefaultSwitchName(nameof(AppSettings.AddLibrary))}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("library_name");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(",library_name,...");
            Console.Write($" {GetDefaultSwitchName(nameof(AppSettings.Upstream))}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("cdn_name");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(",cdn_name,...");
            WriteDescription("Add libraries to the database.");
            Console.WriteLine("Optional switch:");
            Console.Write($"\t{GetDefaultSwitchName(nameof(AppSettings.Version))}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("version_string");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(",version_string,...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - The library version(s) to add.");
        }
        finally { Console.ResetColor(); }
    }

    private static void WriteGetNewVersionsHelpToConsole()
    {
        string exe = Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;
        Console.BackgroundColor = ConsoleColor.Black;
        try
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine($"{exe} {GetDefaultSwitchName(nameof(AppSettings.GetNewVersions))}=");
            WriteDescription("Show the upstream CDN names in the database.");
        }
        finally { Console.ResetColor(); }
    }

    private static void WriteRemoveLibrariesHelpToConsole()
    {
        string exe = Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;
        Console.BackgroundColor = ConsoleColor.Black;
        try
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine($"{exe} {GetDefaultSwitchName(nameof(AppSettings.RemoveLibrary))}");
            WriteDescription("Show the upstream CDN names in the database.");
        }
        finally { Console.ResetColor(); }
    }

    private static void WriteReloadLibrariesHelpToConsole()
    {
        string exe = Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;
        Console.BackgroundColor = ConsoleColor.Black;
        try
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine($"{exe} {GetDefaultSwitchName(nameof(AppSettings.ReloadLibrary))}");
            WriteDescription("Show the upstream CDN names in the database.");
        }
        finally { Console.ResetColor(); }
    }

    private static void WriteReloadExistingHelpToConsole()
    {
        string exe = Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;
        Console.BackgroundColor = ConsoleColor.Black;
        try
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine($"{exe} {GetDefaultSwitchName(nameof(AppSettings.ReloadExistingVersions))}");
            WriteDescription("Show the upstream CDN names in the database.");
        }
        finally { Console.ResetColor(); }
    }

    private static void WriteHelpToConsole()
    {
        string exe = Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;
        Console.BackgroundColor = ConsoleColor.Black;
        try
        {
            static void writeOptionSet(int optionSet)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                Console.WriteLine($"Option set #{optionSet}:");
            }

            writeOptionSet(1);
            Console.Write(exe);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{exe} {GetDefaultSwitchName(nameof(AppSettings.Show))}=");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(SHOW_CDNs);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("|");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(SHOW_Libraries);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("|");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(SHOW_Versions);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("|");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(SHOW_Files);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("]");
            WriteDescription("Show database information.");

            void writeSyntaxStart(int optionSet, string shorthand)
            {
                writeOptionSet(optionSet);
                Console.Write($"{exe} {shorthand}=");
            }
            
            static void writeSyntaxEnd(string value)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(value);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($",{value},...");
            }
            
            void writeCommandSyntax2(int optionSet, string shorthand1, string value1, string shorthand2, string value2)
            {
                writeSyntaxStart(optionSet, shorthand1);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(value1);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($",{value1},...");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" {shorthand2}=");
                writeSyntaxEnd(value2);
            }

            writeCommandSyntax2(2, SHORTHAND_a, "library_name", SHORTHAND_u, "cdn_name");
            WriteDescription("Add libraries from an upstream CDN to the database.");

            void writeCommandSyntax(int optionSet, string shorthand, string value)
            {
                writeSyntaxStart(optionSet, shorthand);
                writeSyntaxEnd(value);
            }

            writeCommandSyntax(3, SHORTHAND_n, "library_name");
            WriteDescription("Add new versions of libraries to the database.");

            writeCommandSyntax(4, SHORTHAND_d, "library_name");
            WriteDescription("Removes libraries from the database.");

            writeCommandSyntax(5, SHORTHAND_r, "library_name");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  or");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{exe} {GetDefaultSwitchName(nameof(AppSettings.Upstream))}=");
            writeSyntaxEnd("library_name");
            WriteDescription("Reloads libraries in the database.");

            writeCommandSyntax2(6, SHORTHAND_e, "library_name", SHORTHAND_u, "cdn_name");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  or");
            Console.Write($"{exe} {GetDefaultSwitchName(nameof(AppSettings.ReloadExistingVersions))}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("library_name");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(",libary_name,...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" {GetDefaultSwitchName(nameof(AppSettings.Version))}=");
            writeSyntaxEnd("version_string");
            WriteDescription("Reloads existing library versions in the database.");

            writeOptionSet(7);
            Console.Write(SHORTHAND__3F_);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(SHORTHAND_h);
            WriteDescription("Displays this help information.");
            Console.WriteLine("If this option is used, then all other options are ignored.");
        }
        finally { Console.ResetColor(); }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Show CDNs
    /// <list type="bullet">
    ///     <item>
    ///         <term><see cref="SHORTHAND_s"/>=<see cref="SHOW_CDNs"/> or no args</term>
    ///         <description>
    ///             <list type="bullet">
    ///                 <item>
    ///                     <term>Method:</term><description><see cref="UpstreamCdn.ShowAsync(ContentDb, ILogger, IServiceScopeFactory, CancellationToken)"/></description>
    ///                 </item>
    ///             </list>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="SHORTHAND_s"/>=<see cref="SHOW_Libraries"/></term>
    ///         <description>
    ///             <list type="bullet">
    ///                 <item>
    ///                     <term><see cref="SHORTHAND_u"/>:</term><description>Optional <see cref="_cdnNames"/></description>
    ///                     <term>Method:</term><description><see cref="CdnLibrary.ShowAsync(ImmutableArray{string}, ContentDb, ILogger, CancellationToken)"/></description>
    ///                 </item>
    ///             </list>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="SHORTHAND_s"/>=<see cref="SHOW_Versions"/></term>
    ///         <description>
    ///             <list type="bullet">
    ///                 <item>
    ///                     <term><see cref="SHORTHAND_l"/>:</term><description>Required <see cref="_libraryNames"/></description>
    ///                     <term><see cref="SHORTHAND_u"/>:</term><description>Optional <see cref="_cdnNames"/></description>
    ///                     <term>Method:</term><description><see cref="CdnVersion.ShowAsync(ImmutableArray{string}, ImmutableArray{string}, ContentDb, ILogger, CancellationToken)"/></description>
    ///                 </item>
    ///             </list>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="SHORTHAND_s"/>=<see cref="SHOW_Files"/></term>
    ///         <description>
    ///             <list type="bullet">
    ///                 <item>
    ///                     <term><see cref="SHORTHAND_l"/>:</term><description>Required <see cref="_libraryNames"/></description>
    ///                     <term><see cref="SHORTHAND_v"/>:</term><description>Optional <see cref="_versions"/></description>
    ///                     <term><see cref="SHORTHAND_u"/>:</term><description>Optional <see cref="_cdnNames"/></description>
    ///                     <term>Method:</term><description><see cref="CdnFile.ShowAsync(ImmutableArray{string}, ImmutableArray{string}, ImmutableArray{string}, ContentDb, ILogger, CancellationToken)"/></description>
    ///                 </item>
    ///             </list>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="SHORTHAND_a"/>=</term><description>Required <see cref="_libraryNames"/></term>
    ///         <description>
    ///             <list type="bullet">
    ///                 <item>
    ///                     <term><see cref="SHORTHAND_u"/>:</term><description>Required <see cref="_cdnNames"/></description>
    ///                     <term><see cref="SHORTHAND_v"/>:</term><description>Optional <see cref="_versions"/></description>
    ///                     <term>Method:</term><description><see cref="CdnLibrary.AddAsync(ImmutableArray{string}, ImmutableArray{string}, ImmutableArray{string}, ContentDb, ILogger, CancellationToken)"/></description>
    ///                 </item>
    ///             </list>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="SHORTHAND_d"/>=</term><description>Required <see cref="_libraryNames"/></term>
    ///         <description>
    ///             <list type="bullet">
    ///                 <item>
    ///                     <term><see cref="SHORTHAND_u"/>:</term><description>Optional <see cref="_cdnNames"/></description>
    ///                     <term><see cref="SHORTHAND_v"/>:</term><description>Optional <see cref="_versions"/></description>
    ///                     <term>Method:</term><description><see cref="CdnLibrary.RemoveAsync(ImmutableArray{string}, ImmutableArray{string}, ImmutableArray{string}, ContentDb, ILogger, CancellationToken)"/></description>
    ///                 </item>
    ///             </list>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="SHORTHAND_r"/>=</term><description>Required <see cref="_libraryNames"/></term>
    ///         <description>
    ///             <list type="bullet">
    ///                 <item>
    ///                     <term><see cref="SHORTHAND_u"/>:</term><description>Optional <see cref="_cdnNames"/></description>
    ///                     <term>Method:</term><description><see cref="CdnLibrary.ReloadAsync(ImmutableArray{string}, ImmutableArray{string}, ContentDb, ILogger, CancellationToken)"/></description>
    ///                 </item>
    ///             </list>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="SHORTHAND_e"/>=</term><description>Required <see cref="_libraryNames"/></term>
    ///         <description>
    ///             <list type="bullet">
    ///                 <item>
    ///                     <term><see cref="SHORTHAND_u"/>:</term><description>Required Alt <see cref="_cdnNames"/></description>
    ///                     <term><see cref="SHORTHAND_v"/>:</term><description>Required Alt <see cref="_versions"/></description>
    ///                     <term>Method:</term><description><see cref="CdnLibrary.ReloadExistingAsync(ImmutableArray{string}, ImmutableArray{string}, ImmutableArray{string}, ContentDb, ILogger, CancellationToken)"/></description>
    ///                 </item>
    ///             </list>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="SHORTHAND_n"/>=</term><description>Required <see cref="_libraryNames"/></term>
    ///         <description>
    ///             <list type="bullet">
    ///                 <item>
    ///                     <term><see cref="SHORTHAND_u"/>:</term><description>Required <see cref="_cdnNames"/></description>
    ///                     <term><see cref="SHORTHAND_v"/>:</term><description>Optional <see cref="_versions"/></description>
    ///                     <term>Method:</term><description><see cref="CdnVersion.AddNewAsync(ImmutableArray{string}, ImmutableArray{string}, ImmutableArray{string}, ContentDb, ILogger, CancellationToken)"/></description>
    ///                 </item>
    ///             </list>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="SHORTHAND_u"/>=</term><description>Required <see cref="_cdnNames"/></term>
    ///         <description>
    ///             <list type="bullet">
    ///                 <item>
    ///                     <term>Method:</term><description><see cref="LocalLibrary.GetNewVersionsPreferredAsync(ImmutableArray{string}, ContentDb, ILogger, CancellationToken)"/></description>
    ///                 </item>
    ///             </list>
    ///         </description>
    ///     </item>
    /// </list>
    /// </remarks>
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (stoppingToken.IsCancellationRequested || !_operation.HasValue)
                return;
            using var _dbContext = _scope.ServiceProvider.GetRequiredService<ContentDb>();
            switch (_operation.Value)
            {
                case OperationType.ShowCDNs:
                    await UpstreamCdn.ShowAsync(_dbContext, _logger, _scopeFactory, stoppingToken);
                    break;
                case OperationType.ShowLibraries:
                    await CdnLibrary.ShowAsync(_cdnNames, _dbContext, _logger, stoppingToken);
                    break;
                case OperationType.ShowVersions:
                    await CdnVersion.ShowAsync(_libraryNames, _cdnNames, _dbContext, _logger, stoppingToken);
                    break;
                case OperationType.ShowFiles:
                    await CdnFile.ShowAsync(_libraryNames, _versions, _cdnNames, _dbContext, _logger, stoppingToken);
                    break;
                case OperationType.AddLibraries:
                    await CdnLibrary.AddAsync(_libraryNames, _cdnNames, _versions, _dbContext, _logger, stoppingToken);
                    break;
                case OperationType.RemoveLibraries:
                    await CdnLibrary.RemoveAsync(_libraryNames, _cdnNames, _versions, _dbContext, _logger, stoppingToken);
                    break;
                case OperationType.ReloadLibraries:
                    await CdnLibrary.ReloadAsync(_libraryNames, _cdnNames, _dbContext, _logger, stoppingToken);
                    break;
                case OperationType.ReloadExisting:
                    await CdnLibrary.ReloadExistingAsync(_libraryNames, _cdnNames, _versions, _dbContext, _logger, stoppingToken);
                    break;
                default:
                    if (_libraryNames.Length > 0)
                        await CdnVersion.AddNewAsync(_libraryNames, _cdnNames, _versions, _dbContext, _logger, stoppingToken);
                    else
                        await LocalLibrary.GetNewVersionsPreferredAsync(_cdnNames, _dbContext, _logger, stoppingToken);
                    break;
            }
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception error)
        {
            _logger.LogUnexpectedServiceError<MainService>(error);
        }
        finally
        {
            if (!stoppingToken.IsCancellationRequested)
                _applicationLifetime.StopApplication();
        }
    }
    
    public MainService(ILogger<MainService> logger, IServiceProvider services, IOptions<Config.AppSettings> options, IHostApplicationLifetime applicationLifetime, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scope = services.CreateScope();
        _applicationLifetime = applicationLifetime;
        _scopeFactory = scopeFactory;
        var appSettings = options.Value;
        
        if (appSettings.Show.IsTrimmedNotEmpty(out string? show))
        {
            if (appSettings.AddLibrary.FromCsv().Any())
                _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Show)), GetSwitchNameForLog(nameof(Config.AppSettings.AddLibrary)));
            else if (appSettings.GetNewVersions.FromCsv().Any())
                _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Show)), GetSwitchNameForLog(nameof(Config.AppSettings.GetNewVersions)));
            else if (appSettings.ReloadExistingVersions.FromCsv().Any())
                _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Show)), GetSwitchNameForLog(nameof(Config.AppSettings.ReloadExistingVersions)));
            else if (appSettings.ReloadLibrary.FromCsv().Any())
                _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Show)), GetSwitchNameForLog(nameof(Config.AppSettings.ReloadLibrary)));
            else if (appSettings.RemoveLibrary.FromCsv().Any())
                _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Show)), GetSwitchNameForLog(nameof(Config.AppSettings.RemoveLibrary)));
            else
                switch (show.ToLower())
                {
                    case SHOW_CDNs:
                        if (appSettings.Library.FromCsv().Any())
                            _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Library)), GetSwitchNameForLog(nameof(Config.AppSettings.Show)), SHOW_CDNs);
                        else if (appSettings.Version.FromCsv().Any())
                            _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Version)), GetSwitchNameForLog(nameof(Config.AppSettings.Show)), SHOW_CDNs);
                        else if (appSettings.Upstream.FromCsv().Any())
                            _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Upstream)), GetSwitchNameForLog(nameof(Config.AppSettings.Show)), SHOW_CDNs);
                        else
                        {
                            if (appSettings.ShowHelp)
                                WriteShowCDNsHelpToConsole();
                            else
                                _operation = OperationType.ShowCDNs;
                            _libraryNames = ImmutableArray<string>.Empty;
                            _versions = ImmutableArray<string>.Empty;
                            _cdnNames = ImmutableArray<string>.Empty;
                            return;
                        }
                        break;
                    case SHOW_Libraries:
                        if (appSettings.Library.FromCsv().Any())
                            _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Library)), GetSwitchNameForLog(nameof(Config.AppSettings.Show)), SHOW_Libraries);
                        else if (appSettings.Version.FromCsv().Any())
                            _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Version)), GetSwitchNameForLog(nameof(Config.AppSettings.Show)), SHOW_Libraries);
                        else if (!appSettings.ShowHelp)
                        {
                            _operation = OperationType.ShowCDNs;
                            _cdnNames = appSettings.Upstream.FromCsv().Distinct().ToImmutableArray();
                            _libraryNames = ImmutableArray<string>.Empty;
                            _versions = ImmutableArray<string>.Empty;
                            return;
                        }
                        WriteShowLibrariesHelpToConsole();
                        break;
                    case SHOW_Versions:
                        if (appSettings.Version.FromCsv().Any())
                            _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Version)), GetSwitchNameForLog(nameof(Config.AppSettings.Show)), SHOW_Versions);
                        else if (!appSettings.ShowHelp)
                        {
                            if ((_libraryNames = appSettings.Library.FromCsv().Distinct().ToImmutableArray()).Length > 0)
                            {
                                _cdnNames = appSettings.Upstream.FromCsv().Distinct().ToImmutableArray();
                                _versions = ImmutableArray<string>.Empty;
                                return;
                            }
                            else
                                _logger.LogRequiredDependentParameterWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Library)), GetSwitchNameForLog(nameof(Config.AppSettings.Show)), SHOW_Versions);
                        }
                        WriteShowVersionsHelpToConsole();
                        break;
                    case SHOW_Files:
                        if (!appSettings.ShowHelp)
                        {
                            if ((_libraryNames = appSettings.Library.FromCsv().Distinct().ToImmutableArray()).Length > 0)
                            {
                                _cdnNames = appSettings.Upstream.FromCsv().Distinct().ToImmutableArray();
                                _versions = appSettings.Version.FromCsv().Distinct().ToImmutableArray();
                                return;
                            }
                            else
                                _logger.LogRequiredDependentParameterWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Library)), GetSwitchNameForLog(nameof(Config.AppSettings.Show)), SHOW_Versions);
                        }
                        WriteShowFilesHelpToConsole();
                        break;
                    default:
                        _logger.LogInvalidParameterValueWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Show)), show);
                        break;
                }
        }
        else
        {
            var libraries = appSettings.AddLibrary.FromCsv().ToImmutableArray();
            if (libraries.Length > 0)
            {
                if (appSettings.GetNewVersions.FromCsv().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.GetNewVersions)), GetSwitchNameForLog(nameof(Config.AppSettings.AddLibrary)));
                else if (appSettings.RemoveLibrary.FromCsv().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.RemoveLibrary)), GetSwitchNameForLog(nameof(Config.AppSettings.AddLibrary)));
                else if (appSettings.ReloadLibrary.FromCsv().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.ReloadLibrary)), GetSwitchNameForLog(nameof(Config.AppSettings.AddLibrary)));
                else if (appSettings.ReloadExistingVersions.FromCsv().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.ReloadExistingVersions)), GetSwitchNameForLog(nameof(Config.AppSettings.AddLibrary)));
                else if (appSettings.Library.FromCsv().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Library)), GetSwitchNameForLog(nameof(Config.AppSettings.AddLibrary)));
                else if (!appSettings.ShowHelp)
                {
                    if ((_cdnNames = appSettings.Upstream.FromCsv().ToImmutableArray()).Length > 0)
                    {
                        _libraryNames = libraries;
                        _versions = appSettings.Version.FromCsv().ToImmutableArray();
                        _operation = OperationType.AddLibraries;
                        return;
                    }
                    else
                        logger.LogRequiredDependentParameterWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Upstream)), GetSwitchNameForLog(nameof(Config.AppSettings.AddLibrary)));
                }
                WriteAddLibrariesHelpToConsole();
            }
            else if ((libraries = appSettings.GetNewVersions.FromCsv().ToImmutableArray()).Length > 0)
            {
                if (appSettings.RemoveLibrary.FromCsv().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.RemoveLibrary)), GetSwitchNameForLog(nameof(Config.AppSettings.GetNewVersions)));
                else if (appSettings.ReloadLibrary.FromCsv().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.ReloadLibrary)), GetSwitchNameForLog(nameof(Config.AppSettings.GetNewVersions)));
                else if (appSettings.ReloadExistingVersions.FromCsv().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.ReloadExistingVersions)), GetSwitchNameForLog(nameof(Config.AppSettings.GetNewVersions)));
                else if (appSettings.Library.FromCsv().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Library)), GetSwitchNameForLog(nameof(Config.AppSettings.GetNewVersions)));
                else if (!appSettings.ShowHelp)
                {
                    if ((_cdnNames = appSettings.Upstream.FromCsv().ToImmutableArray()).Length > 0)
                    {
                        _libraryNames = libraries;
                        _versions = appSettings.Version.FromCsv().ToImmutableArray();
                        _operation = OperationType.GetNewVersions;
                        return;
                    }
                    else
                        logger.LogRequiredDependentParameterWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Upstream)), GetSwitchNameForLog(nameof(Config.AppSettings.AddLibrary)));
                }
                WriteGetNewVersionsHelpToConsole();
            }
            else if ((libraries = appSettings.ReloadExistingVersions.FromCsv().ToImmutableArray()).Length > 0)
            {
                if (appSettings.RemoveLibrary.FromCsv().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.RemoveLibrary)), GetSwitchNameForLog(nameof(Config.AppSettings.ReloadExistingVersions)));
                else if (appSettings.ReloadLibrary.FromCsv().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.ReloadLibrary)), GetSwitchNameForLog(nameof(Config.AppSettings.ReloadExistingVersions)));
                else if (appSettings.Library.FromCsv().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Library)), GetSwitchNameForLog(nameof(Config.AppSettings.ReloadExistingVersions)));
                else if (!appSettings.ShowHelp)
                {
                    _versions = appSettings.Version.FromCsv().ToImmutableArray();
                    if ((_cdnNames = appSettings.Upstream.FromCsv().ToImmutableArray()).Length > 0 || _versions.Length > 0)
                    {
                        _libraryNames = libraries;
                        _operation = OperationType.ReloadExisting;
                        return;
                    }
                    else
                        _logger.LogRequiredAltDependentParameterWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Upstream)), GetSwitchNameForLog(nameof(Config.AppSettings.Version)), GetSwitchNameForLog(nameof(Config.AppSettings.ReloadExistingVersions)));
                }
                WriteReloadExistingHelpToConsole();
            }
            else if ((libraries = appSettings.ReloadLibrary.FromCsv().ToImmutableArray()).Length > 0)
            {
                if (appSettings.RemoveLibrary.FromCsv().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.RemoveLibrary)), GetSwitchNameForLog(nameof(Config.AppSettings.ReloadLibrary)));
                else if (appSettings.Library.FromCsv().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Library)), GetSwitchNameForLog(nameof(Config.AppSettings.ReloadLibrary)));
                else if (appSettings.Version.FromCsv().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Version)), GetSwitchNameForLog(nameof(Config.AppSettings.ReloadLibrary)));
                else if (!appSettings.ShowHelp)
                {
                    _libraryNames = libraries;
                    _cdnNames = appSettings.Upstream.FromCsv().ToImmutableArray();
                    _versions = ImmutableArray<string>.Empty;
                    _operation = OperationType.ReloadLibraries;
                    return;
                }
                WriteReloadLibrariesHelpToConsole();
            }
            else if ((libraries = appSettings.RemoveLibrary.FromCsv().ToImmutableArray()).Length > 0)
            {
                if (appSettings.Library.FromCsv().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Library)), GetSwitchNameForLog(nameof(Config.AppSettings.RemoveLibrary)));
                else if (!appSettings.ShowHelp)
                {
                    _libraryNames = libraries;
                    _cdnNames = appSettings.Upstream.FromCsv().ToImmutableArray();
                    _versions = appSettings.Version.FromCsv().ToImmutableArray();
                    _operation = OperationType.RemoveLibraries;
                    return;
                }
                WriteRemoveLibrariesHelpToConsole();
            }
            else if ((libraries = appSettings.Upstream.FromCsv().ToImmutableArray()).Length > 0)
            {
                if (appSettings.Library.FromCsv().Any())
                    _logger.LogMutuallyExclusiveSwitchWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Library)), GetSwitchNameForLog(nameof(Config.AppSettings.Upstream)));
                else if (!appSettings.ShowHelp)
                {
                    _libraryNames = ImmutableArray<string>.Empty;
                    _cdnNames = libraries;
                    _versions = appSettings.Version.FromCsv().ToImmutableArray();
                    _operation = OperationType.GetNewVersions;
                    return;
                }
                WriteGetNewVersionsHelpToConsole();
            }
            else
            {
                if (!appSettings.ShowHelp)
                {
                    if (appSettings.Library.FromCsv().Any())
                        _logger.LogRequiredAltDependentParameterWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Show)), SHOW_Libraries, SHOW_Versions, GetSwitchNameForLog(nameof(Config.AppSettings.Library)));
                    else if (appSettings.Version.FromCsv().Any())
                        _logger.LogRequiredAltDependentParameterWarning(GetSwitchNameForLog(nameof(Config.AppSettings.Show)), SHOW_Files, GetSwitchNameForLog(nameof(Config.AppSettings.Show)), SHOW_CDNs,
                            GetSwitchNameForLog(nameof(Config.AppSettings.Version)), GetSwitchNameForLog(nameof(Config.AppSettings.AddLibrary)), GetSwitchNameForLog(nameof(Config.AppSettings.GetNewVersions)),
                            GetSwitchNameForLog(nameof(Config.AppSettings.ReloadExistingVersions)), GetSwitchNameForLog(nameof(Config.AppSettings.RemoveLibrary)), GetSwitchNameForLog(nameof(Config.AppSettings.Upstream)));
                    else
                    {
                        _operation = OperationType.ShowCDNs;
                        _libraryNames = ImmutableArray<string>.Empty;
                        _versions = ImmutableArray<string>.Empty;
                        _cdnNames = ImmutableArray<string>.Empty;
                        return;
                    }
                }
                WriteHelpToConsole();
            }
        }
        _libraryNames = ImmutableArray<string>.Empty;
        _versions = ImmutableArray<string>.Empty;
        _cdnNames = ImmutableArray<string>.Empty;
    }
}
