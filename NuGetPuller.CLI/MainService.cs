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

    private void WriteHelpToConsole()
    {
        throw new NotImplementedException();
    }

    private void WriteListLocalHelpToConsole()
    {
        throw new NotImplementedException();
    }

    private void WriteCheckDependenciesHelpToConsole()
    {
        throw new NotImplementedException();
    }

    private void WriteCreateBundleHelpToConsole()
    {
        throw new NotImplementedException();
    }

    private void WriteDownloadHelpToConsole()
    {
        throw new NotImplementedException();
    }

    private void WriteRemoveHelpToConsole()
    {
        throw new NotImplementedException();
    }

    private void WriteAddPackageFilesHelpToConsole()
    {
        throw new NotImplementedException();
    }

    private void WriteExportMetaDataHelpToConsole()
    {
        throw new NotImplementedException();
    }

    private static async Task WriteLocalPackagesToConsoleAsync(ILocalNuGetFeedService localClientService, bool includeVersions, CancellationToken cancellationToken)
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

    private static bool ArePackageIdsValid(IEnumerable<string> packageIds)
    {
        using var enumerator = packageIds.GetEnumerator();
        while (enumerator.MoveNext())
        {
            string id = enumerator.Current;
            if (id.Length == 0)
            {
                WriteConsoleError("Package identifier cannot be empty.");
                while (enumerator.MoveNext())
                    if ((id = enumerator.Current).Length > 0 && !NuGet.Packaging.PackageIdValidator.IsValidPackageId(id))
                        WriteConsoleError("\"{0}\" is not a valid package identifier.", id);
                return false;
            }
            if (!NuGet.Packaging.PackageIdValidator.IsValidPackageId(id))
            {
                WriteConsoleError("\"{0}\" is not a valid package identifier.", id);
                while (enumerator.MoveNext())
                    if ((id = enumerator.Current).Length == 0)
                        WriteConsoleError("Package identifier cannot be empty.");
                    else if (!NuGet.Packaging.PackageIdValidator.IsValidPackageId(id))
                        WriteConsoleError("\"{0}\" is not a valid package identifier.", id);
                return false;
            }
        }
        return true;
    }

    private static bool TryParseVersionStrings(string[] versionStrings, out NuGetVersion[] versions)
    {
        int count = versionStrings.Length;
        versions = new NuGetVersion[count];
        for (int i = 0; i < count; i++)
        {
            string s = versionStrings[i];
            if (s.Length == 0)
            {
                WriteConsoleError("Version string cannot be empty.");
                while (++i < count)
                    if ((s = versionStrings[i]).Length > 0 && !NuGetVersion.TryParse(s, out _))
                        WriteConsoleError("\"{0}\" is not a valid version string.", s);
                return false;
            }
            if (NuGetVersion.TryParse(s, out NuGetVersion? v))
                versions[i] = v;
            else
            {
                WriteConsoleError("\"{0}\" is not a valid version string.", s);
                while (++i < count)
                    if ((s = versionStrings[i]).Length == 0)
                        WriteConsoleError("Version string cannot be empty.");
                    else if (!NuGetVersion.TryParse(s, out _))
                        WriteConsoleError("\"{0}\" is not a valid version string.", s);
            }
        }
        return true;
    }

    private static async Task OnCheckDependenciesAsync(string[]? packageIds, string[]? versionStrings, bool noDownload, IServiceProvider serviceProvider, ILogger logger, CancellationToken cancellationToken)
    {
        if (packageIds is null)
        {
            if (versionStrings is not null)
                WriteConsoleWarning("Command line switch {0} is ignored because {1} is not specified.", CommandLineSwitches.COMMAND_LINE_SWITCH_version,
                    CommandLineSwitches.COMMAND_LINE_SWITCH_package_id);
            var localService = serviceProvider.GetRequiredService<ILocalNuGetFeedService>();
            if (noDownload)
                await CheckAllDependenciesAsync(localService, logger, cancellationToken);
            else
                await DownloadAllMissingDependenciesAsync(localService, serviceProvider.GetRequiredService<IUpstreamNuGetClientService>(), logger, cancellationToken);
        }
        else
        {
            packageIds = packageIds.Distinct(PackageIdentitifierComparer).ToArray();
            if (ArePackageIdsValid(packageIds))
            {
                var localService = serviceProvider.GetRequiredService<ILocalNuGetFeedService>();
                if (versionStrings is null)
                {
                    if (noDownload)
                        await CheckDependenciesAsync(localService, logger, packageIds, cancellationToken);
                    else
                        await DownloadMissingDependenciesAsync(localService, serviceProvider.GetRequiredService<IUpstreamNuGetClientService>(), logger, packageIds, cancellationToken);
                }
                else if (TryParseVersionStrings(versionStrings.Distinct(PackageIdentitifierComparer).ToArray(), out NuGetVersion[] versions))
                {
                    if (noDownload)
                        await CheckDependenciesAsync(localService, logger, packageIds, versions, cancellationToken);
                    else
                        await DownloadMissingDependenciesAsync(localService, serviceProvider.GetRequiredService<IUpstreamNuGetClientService>(), logger, packageIds, versions, cancellationToken);
                }
            }
            else if (versionStrings is not null)
                _ = TryParseVersionStrings(versionStrings.Distinct(PackageIdentitifierComparer).ToArray(), out _);
        }
    }

    private async static Task OnCreateBundle(string path, string? createFrom, string? saveTo, string[]? packageIds, string[]? versionStrings, IServiceProvider serviceProvider, ILogger logger, CancellationToken cancellationToken)
    {
        if (packageIds is null)
        {
            if (versionStrings is not null)
                WriteConsoleWarning("Command line switch {0} is ignored because {1} is not specified.", CommandLineSwitches.COMMAND_LINE_SWITCH_version,
                    CommandLineSwitches.COMMAND_LINE_SWITCH_package_id);
            await ExportBundleAsync(path, createFrom, saveTo, serviceProvider.GetRequiredService<ILocalNuGetFeedService>(), logger, cancellationToken);
        }
        else if (versionStrings is null)
            await ExportBundleAsync(path, createFrom, saveTo, packageIds, serviceProvider.GetRequiredService<ILocalNuGetFeedService>(), logger, cancellationToken);
        else if (TryParseVersionStrings(versionStrings.Distinct(PackageIdentitifierComparer).ToArray(), out NuGetVersion[] versions))
            await ExportBundleAsync(path, createFrom, saveTo, packageIds, versions, serviceProvider.GetRequiredService<ILocalNuGetFeedService>(), logger, cancellationToken);
    }

    private static Task OnDownload(string[] packageIds, string[]? versionStrings, bool noDependencies, string? saveTo, IServiceProvider serviceProvider, ILogger logger, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("--download not implemented.");
    }

    private static Task OnRemove(string[] packageIds, string[]? versionStrings, string? saveTo, IServiceProvider serviceProvide, ILogger loggerr, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("--remove not implemented.");
    }

    private static Task<Task> OnAddPackageFiles(string[] paths, IServiceProvider serviceProvider, ILogger logger, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("--add-file not implemented.");
    }

    private Task<Task> OnExportMetaData(string path, IServiceProvider serviceProvider, ILogger logger, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("--export-metadata not implemented.");
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
                        WriteCheckDependenciesHelpToConsole();
                    }
                    else
                        await OnCheckDependenciesAsync(_settings.PackageId.SplitIfNotWhiteSpace(','), _settings.Version.SplitIfNotWhiteSpace(','), _settings.NoDownload, scope.ServiceProvider, _logger, stoppingToken);
                }
            }
            else
            {
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
                            WriteListLocalHelpToConsole();
                        else
                            await WriteLocalPackagesToConsoleAsync(scope.ServiceProvider.GetRequiredService<ILocalNuGetFeedService>(), _settings.IncludeVersions, stoppingToken);
                    }
                }
                else
                {
                    CheckIgnoredDependentCommandLineArgument(_settings.IncludeVersions, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_include_versions, CommandLineSwitches.COMMAND_LINE_SWITCH_list));
                    if (string.IsNullOrWhiteSpace(_settings.CreateBundle))
                    {
                        CheckIgnoredDependentCommandLineArgument(_settings.CreateFrom, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_create_from, CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                        CheckIgnoredDependentCommandLineArgument(_settings.SaveMetaDataTo, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_save_metadata_to, CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                        if (string.IsNullOrWhiteSpace(_settings.Download))
                        {
                            CheckIgnoredDependentCommandLineArgument(_settings.NoDependencies, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_no_dependencies, CommandLineSwitches.COMMAND_LINE_SWITCH_download));
                            if (string.IsNullOrWhiteSpace(_settings.Remove))
                            {
                                CheckIgnoredDependentCommandLineArgument(_settings.Version, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_version, CommandLineSwitches.COMMAND_LINE_SWITCH_download,
                                    CommandLineSwitches.COMMAND_LINE_SWITCH_remove, CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies, CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                                CheckIgnoredDependentCommandLineArgument(_settings.PackageId, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_package_id, CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies,
                                    CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                                CheckIgnoredDependentCommandLineArgument(_settings.SaveTo, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_save_to, CommandLineSwitches.COMMAND_LINE_SWITCH_remove));
                                if (string.IsNullOrWhiteSpace(_settings.AddPackageFiles))
                                {
                                    if (string.IsNullOrWhiteSpace(_settings.ExportMetaData))
                                    {
                                        if (!_settings.Help)
                                            WriteConsoleWarning("No valid command line arguments provided.");
                                        WriteHelpToConsole();
                                    }
                                    else if (_settings.Help)
                                        WriteExportMetaDataHelpToConsole();
                                    else
                                        await OnExportMetaData(_settings.ExportMetaData, scope.ServiceProvider, _logger, stoppingToken);
                                }
                                else if (!string.IsNullOrWhiteSpace(_settings.ExportMetaData))
                                    WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_add_file,
                                        CommandLineSwitches.COMMAND_LINE_SWITCH_export_metadata);
                                else if (_settings.Help)
                                    WriteAddPackageFilesHelpToConsole();
                                else
                                    await OnAddPackageFiles(_settings.AddPackageFiles.Split(';'), scope.ServiceProvider, _logger, stoppingToken);
                            }
                            else if (!string.IsNullOrWhiteSpace(_settings.AddPackageFiles))
                                WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_remove, CommandLineSwitches.COMMAND_LINE_SWITCH_add_file);
                            else if (!string.IsNullOrWhiteSpace(_settings.ExportMetaData))
                                WriteConsoleError("Command line switch {0} cannot be used with {1}.", CommandLineSwitches.COMMAND_LINE_SWITCH_remove, CommandLineSwitches.COMMAND_LINE_SWITCH_export_metadata);
                            else
                            {
                                CheckIgnoredDependentCommandLineArgument(_settings.PackageId, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_package_id, CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies,
                                    CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                                if (_settings.Help)
                                    WriteRemoveHelpToConsole();
                                else
                                    await OnRemove(_settings.Remove.Split(','), _settings.Version.SplitIfNotWhiteSpace(','), _settings.SaveTo.NullIfWhiteSpace(), scope.ServiceProvider, _logger, stoppingToken);
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
                            CheckIgnoredDependentCommandLineArgument(_settings.SaveTo, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_save_to, CommandLineSwitches.COMMAND_LINE_SWITCH_remove));
                            CheckIgnoredDependentCommandLineArgument(_settings.PackageId, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_package_id, CommandLineSwitches.COMMAND_LINE_SWITCH_check_depencencies,
                                CommandLineSwitches.COMMAND_LINE_SWITCH_create_bundle));
                            if (_settings.Help)
                                WriteDownloadHelpToConsole();
                            else
                                await OnDownload(_settings.Download.Split(','), _settings.Version.SplitIfNotWhiteSpace(','), _settings.NoDependencies, _settings.SaveTo, scope.ServiceProvider, _logger, stoppingToken);
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
                        CheckIgnoredDependentCommandLineArgument(_settings.NoDependencies, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_no_dependencies, CommandLineSwitches.COMMAND_LINE_SWITCH_download));
                        CheckIgnoredDependentCommandLineArgument(_settings.SaveTo, () => (CommandLineSwitches.COMMAND_LINE_SWITCH_save_to, CommandLineSwitches.COMMAND_LINE_SWITCH_remove));
                        if (_settings.Help)
                            WriteCreateBundleHelpToConsole();
                        else
                            await OnCreateBundle(_settings.CreateBundle, _settings.CreateFrom.NullIfWhiteSpace(), _settings.SaveMetaDataTo.NullIfWhiteSpace(), _settings.PackageId.SplitIfNotWhiteSpace(','),
                                _settings.Version.SplitIfNotWhiteSpace(','), scope.ServiceProvider, _logger, stoppingToken);
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
