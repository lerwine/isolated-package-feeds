using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NuGetPuller;
using NuGetPuller.CLI;

internal class Program
{
    internal static IHost Host { get; private set; } = null!;

    private static void Main(string[] args)
    {
        HostApplicationBuilder builder = AppHost.CreateBuilder(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!, args);
        AppHost.ConfigureLogging(builder);
        AppHost.ConfigureSettings<AppSettings>(builder, configuration => AppSettings.Configure(args, configuration));
        builder.Services.AddHostedService<MainService>();
        AppHost.ConfigureServices<AppSettings, AppSettingsValidatorService, LocalClientService, UpstreamClientService>(builder, settings => AppHost.DefaultPostConfigure(settings, builder));
        (Host = builder.Build()).Run();
    }
}