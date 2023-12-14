using IsolatedPackageFeeds.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Versioning;
using static NuGetPuller.MainServiceStatic;
using static NuGetPuller.CLI.Defaults;

namespace NuGetPuller.CLI;
public class MainService : BackgroundService
{
    private readonly AppSettings _settings;
    private readonly ILogger<MainService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public MainService(IOptions<AppSettings> options, ILogger<MainService> logger, IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime) =>
        (_settings, _logger, _serviceProvider, _applicationLifetime) = (options.Value, logger, serviceProvider, applicationLifetime);
 
    public static void CheckIgnoredDependentCommandLineArgument(bool value, Func<(string DependentSwitch, string SwitchName)> getSwitchNames)
    {
        if (!value)
            return;
        (string dependentSwitch, string switchName) = getSwitchNames();
        WriteConsoleWarning("Command line switch {0} is ignored because {1} is not specified.", dependentSwitch, switchName);
    }
    
    public static void CheckIgnoredDependentCommandLineArgument(string? value, Func<(string DependentSwitch, string SwitchName)> getSwitchNames)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        (string dependentSwitch, string switchName) = getSwitchNames();
        WriteConsoleWarning("Command line switch {0} is ignored because {1} is not specified.", dependentSwitch, switchName);
    }
    
    public static void CheckIgnoredDependentCommandLineArgument(string? value, Func<(string DependentSwitch, string SwitchName1, string SwitchName2)> getSwitchNames)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        (string dependentSwitch, string switchName1, string switchName2) = getSwitchNames();
        WriteConsoleWarning("Command line switch {0} is ignored because neither {1}, nor {2} is specified.", dependentSwitch, switchName1, switchName2);
    }
    
    public static void CheckIgnoredDependentCommandLineArgument(string? value, Func<(string DependentSwitch, string SwitchName1, string SwitchName2, string SwitchName3, string SwitchName4)> getSwitchNames)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        (string dependentSwitch, string switchName1, string switchName2, string switchName3, string switchName4) = getSwitchNames();
        WriteConsoleWarning("Command line switch {0} is ignored because neither {1}, {2}, {3}, nor {4} is specified.", dependentSwitch, switchName1, switchName2, switchName3, switchName4);
    }
    
    private Task WriteHelpToConsoleAsync(CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException());
    }

    private Task WriteListLocalHelpToConsoleAsync(CancellationToken cancellationToken)
    {
        return Task.FromException(new NotImplementedException());
    }

    private Task WriteCheckDependenciesHelpToConsoleAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }

    private Task WriteCreateBundleHelpToConsoleAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }

    private Task WriteDownloadHelpToConsoleAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }

    private Task WriteRemoveHelpToConsoleAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }

    private Task WriteAddPackageFilesHelpToConsoleAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }

    private Task WriteExportMetaDataHelpToConsoleAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }

    private async Task WriteLocalPackagesToConsoleAsync(ILocalNuGetFeedService localClientService, bool includeVersions, CancellationToken cancellationToken)
    {
        var allPackages = localClientService.GetAllPackagesAsync(cancellationToken);
        var count = 0;
        if (includeVersions)
            await foreach (var package in allPackages)
            {
                Console.WriteLine("{0}: {1}", package.Identity.Id, package.Title);
                count++;
                var pkgVersions = await package.GetVersionsAsync();
                if (pkgVersions is null || !pkgVersions.Any())
                    Console.WriteLine("    (no versions reported)");
                else
                    foreach (var version in pkgVersions)
                        Console.WriteLine("    {0}", version);
            }
        else
            await foreach (var package in allPackages)
            {
                Console.WriteLine("{0}: {1}", package.Identity.Id, package.Title);
                count++;
            }
        switch (count)
        {
            case 0:
                Console.WriteLine("0 packages in local NuGet source.");
                return;
            case 1:
                Console.WriteLine("1 package in local NuGet source.");
                break;
            default:
                Console.WriteLine("{0} packages in local NuGet source.", count);
                break;
        }
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var originalForegroundColor = Console.ForegroundColor;
        var originalBackgroundColor = Console.BackgroundColor;
        try
        {
            Console.BackgroundColor = BackgroundColor;
            Console.ForegroundColor = InfoColor;
            using var scope = _serviceProvider.CreateScope();
            if (_settings.CheckDependencies)
            {
                if (_settings.ListLocal)
                    WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies, CommandLineSwitches.COMMAND_LINE_SWITCH_list);
                else if (!string.IsNullOrWhiteSpace(_settings.CreateBundle))
                    WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies, CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle);
                else if (!string.IsNullOrWhiteSpace(_settings.Download))
                    WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies, CommandLineSwitches.COMMAND_LINE_SWITCH_download);
                else if (!string.IsNullOrWhiteSpace(_settings.Remove))
                    WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies, CommandLineSwitches.COMMAND_LINE_SWITCH_remove);
                else if (!string.IsNullOrWhiteSpace(_settings.AddPackageFiles))
                    WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies, CommandLineSwitches.COMMAND_LINE_SWITCH_add_file);
                else if (!string.IsNullOrWhiteSpace(_settings.ExportMetaData))
                    WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies, CommandLineSwitches.COMMAND_LINE_SWITCH_export_metadata);
                else
                {
                    CheckIgnoredDependentCommandLineArgument(_settings.IncludeVersions, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_include_versions, CommandLineSwitches.COMMAND_LINE_SWITCH_list));
                    CheckIgnoredDependentCommandLineArgument(_settings.NoDependencies, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_no_dependencies, CommandLineSwitches.COMMAND_LINE_SWITCH_download));
                    CheckIgnoredDependentCommandLineArgument(_settings.SaveTo, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_save_to, CommandLineSwitches.COMMAND_LINE_SWITCH_remove));
                    CheckIgnoredDependentCommandLineArgument(_settings.CreateFrom, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_create_from, CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                    CheckIgnoredDependentCommandLineArgument(_settings.SaveMetaDataTo, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_save_metadata_to, CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                    if (_settings.Help)
                    {
                        if (string.IsNullOrWhiteSpace(_settings.PackageId) && !string.IsNullOrWhiteSpace(_settings.Version))
                            WriteConsoleWarning("Command line switch {0} is ignored because {1} is not specified.", CommandLineSwitches.COMMAND_LINE_SWITCH_version,
                                CommandLineSwitches.COMMAND_LINE_SWITCH_package_id);
                        await WriteCheckDependenciesHelpToConsoleAsync(stoppingToken);
                    }
                    else
                    {
                        bool? hasPackageIdentifiers = _settings.PackageId.TryGetValidNuGetPackageIdentifierList(out string[] packageIds);
                        bool? hasVersions = _settings.Version.TryParseNuGetVersionList(out NuGetVersion[] versions);
                        if (hasPackageIdentifiers.HasValue)
                        {
                            if (hasPackageIdentifiers.Value)
                            {
                                if (hasVersions.HasValue)
                                {
                                    if (hasVersions.Value)
                                    {
                                        var localService = _serviceProvider.GetRequiredService<ILocalNuGetFeedService>();
                                        if (_settings.NoDownload)
                                            await CheckDependenciesAsync(localService, _logger, packageIds, versions, stoppingToken);
                                        else
                                            await DownloadMissingDependenciesAsync(localService, _serviceProvider.GetRequiredService<IUpstreamNuGetClientService>(), _logger, packageIds, versions, stoppingToken);
                                    }
                                    else
                                        WriteConsoleError("An invalid version string was specified.");
                                }
                                else
                                {
                                    var localService = _serviceProvider.GetRequiredService<ILocalNuGetFeedService>();
                                    if (_settings.NoDownload)
                                        await CheckDependenciesAsync(localService, _logger, packageIds, stoppingToken);
                                    else
                                        await DownloadMissingDependenciesAsync(localService, _serviceProvider.GetRequiredService<IUpstreamNuGetClientService>(), _logger, packageIds, stoppingToken);
                                }
                            }
                            else
                            {
                                WriteConsoleError("An invalid package identifier was specified.");
                                if (hasVersions.HasValue && !hasVersions.Value)
                                    WriteConsoleError("An invalid version string was specified.");
                            }
                        }
                        else if (hasVersions.HasValue)
                        {
                            if (hasVersions.Value)
                                WriteConsoleWarning("Command line switch {0} is ignored because {1} is not specified.", CommandLineSwitches.COMMAND_LINE_SWITCH_version,
                                    CommandLineSwitches.COMMAND_LINE_SWITCH_package_id);
                            else
                                WriteConsoleError("An invalid version string was specified.");
                        }
                        else
                        {
                            var localService = _serviceProvider.GetRequiredService<ILocalNuGetFeedService>();
                            if (_settings.NoDownload)
                                await CheckAllDependenciesAsync(localService, _logger, stoppingToken);
                            else
                                await DownloadAllMissingDependenciesAsync(localService, _serviceProvider.GetRequiredService<IUpstreamNuGetClientService>(), _logger, stoppingToken);
                        }
                    }
                }
            }
            else
            {
                CheckIgnoredDependentCommandLineArgument(_settings.NoDownload, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_no_download, CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies));
                if (_settings.ListLocal)
                {
                    if (!string.IsNullOrWhiteSpace(_settings.CreateBundle))
                        WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_list, CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle);
                    else if (!string.IsNullOrWhiteSpace(_settings.Download))
                        WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_list, CommandLineSwitches.COMMAND_LINE_SWITCH_download);
                    else if (!string.IsNullOrWhiteSpace(_settings.Remove))
                        WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_list, CommandLineSwitches.COMMAND_LINE_SWITCH_remove);
                    else if (!string.IsNullOrWhiteSpace(_settings.AddPackageFiles))
                        WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_list, CommandLineSwitches.COMMAND_LINE_SWITCH_add_file);
                    else if (!string.IsNullOrWhiteSpace(_settings.ExportMetaData))
                        WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_list, CommandLineSwitches.COMMAND_LINE_SWITCH_export_metadata);
                    else
                    {
                        CheckIgnoredDependentCommandLineArgument(_settings.Version, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_version, CommandLineSwitches.COMMAND_LINE_SWITCH_download,
                            CommandLineSwitches.COMMAND_LINE_SWITCH_remove, CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies, CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                        CheckIgnoredDependentCommandLineArgument(_settings.NoDependencies, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_no_dependencies, CommandLineSwitches.COMMAND_LINE_SWITCH_download));
                        CheckIgnoredDependentCommandLineArgument(_settings.SaveTo, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_save_to, CommandLineSwitches.COMMAND_LINE_SWITCH_remove));
                        CheckIgnoredDependentCommandLineArgument(_settings.PackageId, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_package_id, CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies,
                            CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                        CheckIgnoredDependentCommandLineArgument(_settings.CreateFrom, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_create_from, CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                        CheckIgnoredDependentCommandLineArgument(_settings.SaveMetaDataTo, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_save_metadata_to, CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                        if (_settings.Help)
                            await WriteListLocalHelpToConsoleAsync(stoppingToken);
                        else
                            await WriteLocalPackagesToConsoleAsync(scope.ServiceProvider.GetRequiredService<ILocalNuGetFeedService>(), _settings.IncludeVersions, stoppingToken);
                    }
                }
                else
                {
                    CheckIgnoredDependentCommandLineArgument(_settings.IncludeVersions, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_include_versions, CommandLineSwitches.COMMAND_LINE_SWITCH_list));
                    if (_settings.CreateBundle.TryGetExistingFileInfo(out Exception? error, out FileInfo? fileInfo))
                    {

                    }
                    else if (error is not null)
                    {
                        // TODO: Log error
                    }
                    else if (fileInfo is not null)
                    {
                        // TODO: Does not exist
                    }
                    else
                    {
                        CheckIgnoredDependentCommandLineArgument(_settings.CreateFrom, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_create_from, CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                        CheckIgnoredDependentCommandLineArgument(_settings.SaveMetaDataTo, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_save_metadata_to, CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                        bool? hasPackageIds = _settings.Download.TryGetValidNuGetPackageIdentifierList(out string[] packageIds);
                        if (hasPackageIds.HasValue)
                        {
                            if (hasPackageIds.Value)
                            {

                            }
                            else
                            {
                                // TODO: Invalid package ID
                            }
                        }
                        else
                        {
                            CheckIgnoredDependentCommandLineArgument(_settings.NoDependencies, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_no_dependencies, CommandLineSwitches.COMMAND_LINE_SWITCH_download));
                            if ((hasPackageIds = _settings.Remove.TryGetValidNuGetPackageIdentifierList(out packageIds)).HasValue)
                            {
                                if (hasPackageIds.Value)
                                {

                                }
                                else
                                {
                                    // TODO: Invalid package ID
                                }
                            }
                            else
                            {
                                CheckIgnoredDependentCommandLineArgument(_settings.SaveTo, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_save_to, CommandLineSwitches.COMMAND_LINE_SWITCH_remove));
                                CheckIgnoredDependentCommandLineArgument(_settings.IncludeVersions, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_include_versions, CommandLineSwitches.COMMAND_LINE_SWITCH_list));
                                CheckIgnoredDependentCommandLineArgument(_settings.Version, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_version, CommandLineSwitches.COMMAND_LINE_SWITCH_download,
                                    CommandLineSwitches.COMMAND_LINE_SWITCH_remove, CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies, CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                                CheckIgnoredDependentCommandLineArgument(_settings.PackageId, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_package_id,
                                    CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies, CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                                if (_settings.ExportMetaData.TryGetExistingFileInfo(out error, out fileInfo))
                                {

                                }
                                else if (error is not null)
                                {
                                    // TODO: Log error
                                }
                                else if (fileInfo is not null)
                                {
                                    // TODO: Does not exist
                                }
                                else
                                {
                                    // AddPackageFiles = may be split by ';'
                                }
                            }
                        }
                    }
                }
            }
            if (!_settings.CheckDependencies)
            {
                if (!_settings.ListLocal)
                {
                    if (string.IsNullOrWhiteSpace(_settings.CreateBundle))
                    {
                        if (string.IsNullOrWhiteSpace(_settings.Download))
                        {
                            if (string.IsNullOrWhiteSpace(_settings.Remove))
                            {
                                if (string.IsNullOrWhiteSpace(_settings.AddPackageFiles))
                                {
                                    if (string.IsNullOrWhiteSpace(_settings.ExportMetaData))
                                    {
                                        if (!_settings.Help)
                                        {
                                            Console.ForegroundColor = WarningColor;
                                            Console.WriteLine("No valid command line arguments provided.");
                                            Console.ForegroundColor = InfoColor;
                                        }
                                        await WriteHelpToConsoleAsync(stoppingToken);
                                    }
                                    else if (_settings.Help)
                                        await WriteExportMetaDataHelpToConsoleAsync(stoppingToken);
                                    else
                                        throw new NotImplementedException("--export-metadata not implemented.");
                                }
                                else if (!string.IsNullOrWhiteSpace(_settings.ExportMetaData))
                                    WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_add_file,
                                        CommandLineSwitches.COMMAND_LINE_SWITCH_export_metadata);
                                else if (_settings.Help)
                                    await WriteAddPackageFilesHelpToConsoleAsync(stoppingToken);
                                else
                                    throw new NotImplementedException("--add-file not implemented.");
                            }
                            else if (!string.IsNullOrWhiteSpace(_settings.AddPackageFiles))
                                WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_remove, CommandLineSwitches.COMMAND_LINE_SWITCH_add_file);
                            else if (!string.IsNullOrWhiteSpace(_settings.ExportMetaData))
                                WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_remove, CommandLineSwitches.COMMAND_LINE_SWITCH_export_metadata);
                            else
                            {
                                CheckIgnoredDependentCommandLineArgument(_settings.IncludeVersions, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_include_versions, CommandLineSwitches.COMMAND_LINE_SWITCH_list));
                                CheckIgnoredDependentCommandLineArgument(_settings.PackageId, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_package_id,
                                    CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies, CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                                if (_settings.Help)
                                    await WriteRemoveHelpToConsoleAsync(stoppingToken);
                                else
                                    throw new NotImplementedException("--remove not implemented.");
                                // _settings.Version, _settings.SaveTo
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(_settings.Remove))
                            WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_download, CommandLineSwitches.COMMAND_LINE_SWITCH_remove);
                        else if (!string.IsNullOrWhiteSpace(_settings.AddPackageFiles))
                            WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_download, CommandLineSwitches.COMMAND_LINE_SWITCH_add_file);
                        else if (!string.IsNullOrWhiteSpace(_settings.ExportMetaData))
                            WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_download, CommandLineSwitches.COMMAND_LINE_SWITCH_export_metadata);
                        else
                        {
                            CheckIgnoredDependentCommandLineArgument(_settings.IncludeVersions, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_include_versions, CommandLineSwitches.COMMAND_LINE_SWITCH_list));
                            CheckIgnoredDependentCommandLineArgument(_settings.SaveTo, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_save_to, CommandLineSwitches.COMMAND_LINE_SWITCH_remove));
                            CheckIgnoredDependentCommandLineArgument(_settings.PackageId, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_package_id, CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies,
                                CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                            if (_settings.Help)
                                await WriteDownloadHelpToConsoleAsync(stoppingToken);
                            else
                                throw new NotImplementedException("--download not implemented.");
                            // _settings.Version, _settings.NoDependencies
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(_settings.Download))
                        WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle, CommandLineSwitches.COMMAND_LINE_SWITCH_download);
                    else if (!string.IsNullOrWhiteSpace(_settings.Remove))
                        WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle, CommandLineSwitches.COMMAND_LINE_SWITCH_remove);
                    else if (!string.IsNullOrWhiteSpace(_settings.AddPackageFiles))
                        WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle, CommandLineSwitches.COMMAND_LINE_SWITCH_add_file);
                    else if (!string.IsNullOrWhiteSpace(_settings.ExportMetaData))
                        WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle, CommandLineSwitches.COMMAND_LINE_SWITCH_export_metadata);
                    else
                    {
                        CheckIgnoredDependentCommandLineArgument(_settings.IncludeVersions, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_include_versions, CommandLineSwitches.COMMAND_LINE_SWITCH_list));
                        CheckIgnoredDependentCommandLineArgument(_settings.Version, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_version, CommandLineSwitches.COMMAND_LINE_SWITCH_download,
                            CommandLineSwitches.COMMAND_LINE_SWITCH_remove, CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies, CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                        CheckIgnoredDependentCommandLineArgument(_settings.NoDependencies, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_no_dependencies, CommandLineSwitches.COMMAND_LINE_SWITCH_download));
                        CheckIgnoredDependentCommandLineArgument(_settings.SaveTo, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_save_to, CommandLineSwitches.COMMAND_LINE_SWITCH_remove));
                        CheckIgnoredDependentCommandLineArgument(_settings.PackageId, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_package_id, CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies,
                            CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                        if (_settings.Help)
                        {
                            if (string.IsNullOrWhiteSpace(_settings.PackageId) && !string.IsNullOrWhiteSpace(_settings.Version))
                                WriteConsoleWarning("Command line switch {0} is ignored because {1} is not specified.", CommandLineSwitches.COMMAND_LINE_SWITCH_version,
                                    CommandLineSwitches.COMMAND_LINE_SWITCH_package_id);
                            await WriteCreateBundleHelpToConsoleAsync(stoppingToken);
                        }
                        else
                        {
                            string? createFrom = _settings.CreateFrom.NullIfWhiteSpace();
                            string? saveMetaDataTo = _settings.SaveMetaDataTo.NullIfWhiteSpace();
                            if (saveMetaDataTo is null)
                            {
                                if (createFrom is null)
                                {
                                    saveMetaDataTo = Environment.MachineName + CommandLineSwitches.METADATA_EXTENSION_nuget_metadata_json;
                                    if (File.Exists(saveMetaDataTo) || Directory.Exists(saveMetaDataTo))
                                    {
                                        int index = 0;
                                        do
                                        {
                                            index++;
                                            saveMetaDataTo = $"{Environment.MachineName}{index}{CommandLineSwitches.METADATA_EXTENSION_nuget_metadata_json}";
                                        }
                                        while (File.Exists(saveMetaDataTo) || Directory.Exists(saveMetaDataTo));
                                    }
                                }
                                else
                                    saveMetaDataTo  = createFrom;
                            }
                            if (_settings.PackageId.TrySplitToNonWhiteSpaceTrimmed(',', out string[]? packageIds))
                            {
                                bool? hasVersions = _settings.Version.TryParseNuGetVersionList(out NuGetVersion[] versions);
                                if (hasVersions.HasValue)
                                {
                                    if (hasVersions.Value)
                                    {

                                    }
                                    else
                                        WriteConsoleError("An invalid version string was specified.");
                                }
                                else
                                {

                                }
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(_settings.Version))
                                    WriteConsoleWarning("Command line switch {0} is ignored because {1} is not specified.", CommandLineSwitches.COMMAND_LINE_SWITCH_version,
                                        CommandLineSwitches.COMMAND_LINE_SWITCH_package_id);
                            }
                        }
                    }
                }
            }

            // var packageIds = _settings.Remove?.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).Distinct(NoCaseComparer);
            // HashSet<PackageIdentity> deletedPackages = new(PackageIdentityComparer.Default);
            // if (packageIds is not null && packageIds.Any())
            //     await foreach (var (package, success) in scope.ServiceProvider.GetRequiredService<ILocalNuGetFeedService>().DeleteAsync(packageIds, stoppingToken))
            //         if (success)
            //             deletedPackages.Add(package);
            // if ((packageIds = _settings.Add?.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).Distinct(NoCaseComparer)) is not null && packageIds.Any())
            // {
            //     Dictionary<string, HashSet<NuGetVersion>> packagesAdded = new(NoCaseComparer);
            //     var localClientService = scope.ServiceProvider.GetRequiredService<ILocalNuGetFeedService>();
            //     foreach (string id in packageIds)
            //         await AddToLocalFromRemote(id, packagesAdded, localClientService, scope.ServiceProvider.GetRequiredService<IUpstreamNuGetClientService>(), _logger, stoppingToken);
            // }
            // if (validatedSettings.Import.TryGetResult(out FileSystemInfo? importFrom))
            //     throw new NotImplementedException();
            // // await ImportAsync(validatedSettings.ImportPath, localClientService, _logger, stoppingToken);
            // if (_settings.UpdateAll)
            // {
            //     var asyncEn = scope.ServiceProvider.GetRequiredService<ILocalNuGetFeedService>().GetAllPackagesAsync(stoppingToken);
            //     if (!(packageIds = asyncEn.ToBlockingEnumerable(stoppingToken).Select(p => p.Identity.Id)).Any())
            //         _logger.NoLocalPackagesExist();
            //     else
            //         await scope.ServiceProvider.GetRequiredService<PackageUpdateService>().UpdatePackagesAsync(packageIds, stoppingToken);
            // }
            // else if ((packageIds = _settings.Update?.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).Distinct(NoCaseComparer)) is not null && packageIds.Any())
            //     await scope.ServiceProvider.GetRequiredService<PackageUpdateService>().UpdatePackagesAsync(packageIds, stoppingToken);
            // if (_settings.ListLocal)
            //     await ListLocalPackagesAsync(scope.ServiceProvider.GetRequiredService<ILocalNuGetFeedService>().GetAllPackagesAsync(stoppingToken).ToBlockingEnumerable(stoppingToken), validatedSettings.ExportLocalManifest.TryGetResult(out FileInfo? exportLocalManifest) ? exportLocalManifest.FullName : null, _logger, stoppingToken);
            // if (validatedSettings.ExportLocalManifest.TryGetResult(out FileInfo? fileInfo))
            //     await ExportLocalManifestAsync(scope.ServiceProvider.GetRequiredService<ILocalNuGetFeedService>().GetAllPackagesAsync(stoppingToken).ToBlockingEnumerable(stoppingToken), fileInfo.FullName, _logger, stoppingToken);
            // if (validatedSettings.ExportBundle.TryGetResult(out fileInfo))
            //     throw new NotImplementedException();
            // // await ExportBundleAsync(validatedSettings.ExportBundlePath, validatedSettings.TargetManifestFilePath, validatedSettings.TargetManifestSaveAsPath, localClientService, _logger, stoppingToken);
        }
        catch (OperationCanceledException) { throw; }
        finally
        {
            try
            {
                if (!stoppingToken.IsCancellationRequested)
                    _applicationLifetime.StopApplication();
            }
            finally
            {
                Console.ForegroundColor = originalForegroundColor;
                Console.BackgroundColor = originalBackgroundColor;
            }
        }
    }
}
