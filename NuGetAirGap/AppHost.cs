using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Configuration;
using Serilog;

namespace NuGetAirGap;

public static class AppHost
{
    public static HostApplicationBuilder CreateBuilder(string contentRootPath)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        builder.Environment.ContentRootPath = contentRootPath;
        return builder;
    }

    public static void ConfigureSettings(HostApplicationBuilder builder, params string[] args)
    {
        AppSettings.Configure(args, builder.Configuration);
        builder.Services
            .AddOptions<AppSettings>()
            .Bind(builder.Configuration.GetSection(nameof(NuGetAirGap)))
            .ValidateDataAnnotations();
    }

    public static void ConfigureLogging(HostApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        builder.Logging.AddSerilog();
    }

    public static void ConfigureServices(HostApplicationBuilder builder, Action<AppSettings> onPostConfigure) => builder.Services.AddSingleton<LocalClientService>()
        .AddSingleton<UpstreamClientService>()
        .AddSingleton<IValidateOptions<AppSettings>, ValidateAppSettings>()
        .PostConfigure(onPostConfigure);

    public static void DefaultPostConfigure(AppSettings settings, HostApplicationBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(settings.GlobalPackagesFolder))
            settings.GlobalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(Settings.LoadDefaultSettings(root: null));
        if (string.IsNullOrWhiteSpace(settings.UpstreamServiceIndex))
            settings.UpstreamServiceIndex = AppSettings.DEFAULT_UPSTREAM_SERVICE_INDEX;
        if (string.IsNullOrWhiteSpace(settings.LocalRepository))
            settings.LocalRepository = Path.Combine(builder.Environment.ContentRootPath, AppSettings.DEFAULT_LOCAL_REPOSITORY);
    }
}
