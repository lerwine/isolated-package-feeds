using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            [$"{nameof(NuGetAirGap)}:{nameof(AppSettings.GlobalPackagesFolder)}"] = ClientService.GetDefaultGlobalPackagesFolder()
        });
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(NuGetAirGap)));
        AppSettings.Configure(args, builder.Configuration);
        builder.Services.AddHostedService<MainService>()
            .AddSingleton<LocalRepositoryProvider>()
            .AddSingleton<UpstreamRepositoryProvider>()
            .AddSingleton<LocalClientService>()
            .AddSingleton<UpstreamClientService>();
        (Host = builder.Build()).Run();
    }
}
