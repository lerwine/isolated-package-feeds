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
    public static HostApplicationBuilder CreateBuilder(string contentRootPath, params string[] args)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(contentRootPath);
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings()
        {
            ContentRootPath = contentRootPath,
            Args = args
        });
        // var fp = builder.Configuration.GetFileProvider();
        // builder.Configuration.SetBasePath(contentRootPath);
        // if (builder.Environment.IsDevelopment())
        //     builder.Configuration.AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly(), true);
        // fp = builder.Configuration.GetFileProvider();
        return builder;
    }

    public static void ConfigureLogging(HostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Logging.ClearProviders();
        // var section = builder.Configuration.GetSection("Serilog:MinimumLevel:Default");
        // section = builder.Configuration.GetSection("Logging:LogLevel:Default");
        // section = builder.Configuration.GetRequiredSection("Serilog");
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        builder.Logging.AddSerilog();
    }

    public static void ConfigureSettings(HostApplicationBuilder builder, params string[] args)
    {
        ArgumentNullException.ThrowIfNull(builder);
        AppSettings.Configure(args, builder.Configuration);
        builder.Services
            .AddOptions<AppSettings>()
            .Bind(builder.Configuration.GetSection(nameof(NuGetAirGap)))
            .ValidateDataAnnotations();
    }

    public static void ConfigureServices(HostApplicationBuilder builder, Action<AppSettings> onPostConfigure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services
        .AddSingleton<IValidateOptions<AppSettings>, AppSettingsValidatorService>()
        .AddSingleton<LocalClientService>()
        .AddSingleton<UpstreamClientService>()
        .PostConfigure(onPostConfigure);
    }

    public static void DefaultPostConfigure(AppSettings settings, HostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(builder);
        if (string.IsNullOrWhiteSpace(settings.GlobalPackagesFolder))
            settings.GlobalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(Settings.LoadDefaultSettings(root: null));
        if (string.IsNullOrWhiteSpace(settings.UpstreamServiceIndex))
            settings.UpstreamServiceIndex = AppSettings.DEFAULT_UPSTREAM_SERVICE_INDEX;
        if (string.IsNullOrWhiteSpace(settings.LocalRepository))
            settings.LocalRepository = Path.Combine(builder.Environment.ContentRootPath, AppSettings.DEFAULT_LOCAL_REPOSITORY);
    }
}
