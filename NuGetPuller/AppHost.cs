using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Configuration;
using Serilog;
using static NuGetPuller.CommonStatic;

namespace NuGetPuller;

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

    public static void ConfigureSettings<T>(HostApplicationBuilder builder, Action<ConfigurationManager>? configure = null)
        where T : class, ISharedAppSettings
    {
        ArgumentNullException.ThrowIfNull(builder);
        configure?.Invoke(builder.Configuration);
        builder.Services
            .AddOptions<T>()
            .Bind(builder.Configuration.GetSection(nameof(NuGetPuller)))
            .ValidateDataAnnotations();
    }

    public static void ConfigureServices<TSettings, TValidator, TLocalClientSvc, TUpstreamClientSvc>(HostApplicationBuilder builder, Action<TSettings> onPostConfigure)
        where TSettings : class, ISharedAppSettings
        where TValidator : SharedAppSettingsValidatorService<TSettings>
        where TLocalClientSvc : LocalClientService<TSettings>
        where TUpstreamClientSvc : UpstreamClientService<TSettings>
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services
        .AddSingleton<IValidateOptions<TSettings>, TValidator>()
        .AddSingleton<TLocalClientSvc>()
        .AddSingleton<TUpstreamClientSvc>()
        .PostConfigure(onPostConfigure);
    }

    public static void DefaultPostConfigure(ISharedAppSettings settings, HostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(builder);
        if (string.IsNullOrWhiteSpace(settings.GlobalPackagesFolder))
            settings.GlobalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(Settings.LoadDefaultSettings(root: null));
        if (string.IsNullOrWhiteSpace(settings.UpstreamServiceIndex))
            settings.UpstreamServiceIndex = DEFAULT_UPSTREAM_SERVICE_INDEX;
        if (string.IsNullOrWhiteSpace(settings.LocalRepository))
            settings.LocalRepository = Path.Combine(builder.Environment.ContentRootPath, DEFAULT_LOCAL_REPOSITORY);
    }
}
