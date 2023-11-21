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
        HostApplicationBuilder builder = AppHost.CreateBuilder(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!, args);
        AppHost.ConfigureLogging(builder);
        AppHost.ConfigureSettings(builder, args);
        builder.Services.AddHostedService<MainService>();
        AppHost.ConfigureServices(builder, settings => AppHost.DefaultPostConfigure(settings, builder));
        (Host = builder.Build()).Run();
    }
}
