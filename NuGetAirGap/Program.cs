using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Configuration;
using Serilog;

namespace NuGetAirGap;

class Program
{
    internal static IHost Host { get; private set; } = null!;

    static void Main(string[] args)
    {
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
        (Host = builder.Build()).Run();
    }
}
