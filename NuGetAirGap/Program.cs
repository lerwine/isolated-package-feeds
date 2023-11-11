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
        builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();
        builder.Logging.ClearProviders();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        builder.Logging.AddSerilog();
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(NuGetAirGap)));
        AppSettings.Configure(args, builder.Configuration);
        builder.Services.AddHostedService<MainService>()
            .AddSingleton<LocalClientService>()
            .AddSingleton<UpstreamClientService>();
        (Host = builder.Build()).Run();
    }
}
