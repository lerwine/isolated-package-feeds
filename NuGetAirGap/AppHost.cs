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
    private static readonly object _syncRoot = new();

    public static IHost Host { get; private set; } = null!;

    public static IHost Initialize(string contentRootPath, params string[] args)
    {
        lock (_syncRoot)
        {
            if (Host is not null)
                throw new InvalidOperationException();
            HostApplicationBuilder builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder();
            builder.Environment.ContentRootPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
            builder.Logging.ClearProviders();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();
            builder.Logging.AddSerilog();
            AppSettings.Configure(args, builder.Configuration);
            builder.Services
                .AddOptions<AppSettings>()
                .Bind(builder.Configuration.GetSection(nameof(NuGetAirGap)))
                .ValidateDataAnnotations();
            builder.Services.AddHostedService<MainService>()
                .AddSingleton<LocalClientService>()
                .AddSingleton<UpstreamClientService>()
                .AddSingleton<IValidateOptions<AppSettings>, ValidateAppSettings>()
                .PostConfigure<AppSettings>(settings =>
            {
                if (string.IsNullOrWhiteSpace(settings.GlobalPackagesFolder))
                    settings.GlobalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(Settings.LoadDefaultSettings(root: null));
                if (string.IsNullOrWhiteSpace(settings.UpstreamServiceIndex))
                    settings.UpstreamServiceIndex = AppSettings.DEFAULT_UPSTREAM_SERVICE_INDEX;
                if (string.IsNullOrWhiteSpace(settings.LocalRepository))
                    settings.LocalRepository = Path.Combine(builder.Environment.ContentRootPath, AppSettings.DEFAULT_LOCAL_REPOSITORY);
            });
            Host = builder.Build();
        }
        return Host;
    }
}
