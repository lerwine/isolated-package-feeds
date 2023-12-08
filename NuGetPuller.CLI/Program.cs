using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGetPuller;
using NuGetPuller.CLI;
using Serilog;

internal class Program
{
    internal static IHost Host { get; private set; } = null!;

    private static void Main(string[] args)
    {
        HostApplicationBuilder builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(new HostApplicationBuilderSettings()
        {
            ContentRootPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!,
            Args = args
        });
        builder.Logging.ClearProviders();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        builder.Logging.AddSerilog();
        AppSettings.Configure(args, builder.Configuration);
        builder.Services
            .AddOptions<AppSettings>()
            .Bind(builder.Configuration.GetSection(nameof(NuGetPuller)))
            .ValidateDataAnnotations();
        builder.Services
            .AddSingleton<IValidateOptions<AppSettings>, AppSettingsValidatorService>()
            .AddSingleton<LocalClientService>()
            .AddSingleton<UpstreamClientService>()
            .PostConfigure<AppSettings>(settings =>
            {
                if (string.IsNullOrWhiteSpace(settings.GlobalPackagesFolder))
                    settings.GlobalPackagesFolder = NuGet.Configuration.SettingsUtility.GetGlobalPackagesFolder(NuGet.Configuration.Settings.LoadDefaultSettings(root: null));
                if (string.IsNullOrWhiteSpace(settings.UpstreamServiceIndex))
                    settings.UpstreamServiceIndex = CommonStatic.DEFAULT_UPSTREAM_SERVICE_INDEX;
                if (string.IsNullOrWhiteSpace(settings.LocalRepository))
                    settings.LocalRepository = Path.Combine(builder.Environment.ContentRootPath, CommonStatic.DEFAULT_LOCAL_REPOSITORY);
            });
        builder.Services.AddHostedService<MainService>();
        (Host = builder.Build()).Run();
    }
}