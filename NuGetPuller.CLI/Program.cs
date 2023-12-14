using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            .AddSingleton<IValidatedRepositoryPathsService, ValidatedRepositoryPathsService<AppSettings>>()
            .AddSingleton<ILocalNuGetFeedService, LocalNuGetFeedService>()
            .AddSingleton<IUpstreamNuGetClientService, UpstreamNuGetClientService>()
            .AddTransient<PackageUpdateService>();
        builder.Services.AddHostedService<MainService>();
        (Host = builder.Build()).Run();
    }
}
