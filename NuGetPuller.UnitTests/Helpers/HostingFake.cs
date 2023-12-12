using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace NuGetPuller.UnitTests.Helpers;

record HostingFake(IHost Host, string PreviousCwd, DirectoryInfo BaseDirectory, DirectoryInfo Cwd, DirectoryInfo LocalRepo)
{
    private const string DIRNAME_CWD = "CWD";

    private const string DIRNAME_ContentRoot = "ContentRoot";

    internal static HostingFake Setup<T>()
    {
        var testContext = TestContext.CurrentContext;
        DirectoryInfo baseDirectory = new(Path.Combine(testContext.WorkDirectory, nameof(T)));
        if (!baseDirectory.Exists)
            baseDirectory.Create();
        DirectoryInfo cwd = new(Path.Combine(baseDirectory.FullName, DIRNAME_CWD));
        if (!cwd.Exists)
            cwd.Create();
        var previousCwd = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(cwd.FullName);

        DirectoryInfo localRepo = new(Path.Combine(testContext.TestDirectory, ServiceDefaults.DEFAULT_LOCAL_REPOSITORY));
        if (localRepo.Exists)
            localRepo.Delete(true);

        HostApplicationBuilder builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(new HostApplicationBuilderSettings()
        {
            ContentRootPath = testContext.TestDirectory,
            Args = []
        });
        builder.Logging.ClearProviders();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        builder.Logging.AddSerilog();
        builder.Services
            .AddOptions<TestAppSettings>()
            .Bind(builder.Configuration.GetSection(nameof(NuGetPuller)))
            .ValidateDataAnnotations();
        builder.Services
            .AddSingleton<IValidatedRepositoryPathsService, ValidatedRepositoryPathsService<TestAppSettings>>()
            .AddSingleton<ILocalClientService, LocalClientService>()
            .AddSingleton<IUpstreamClientService, UpstreamClientService>();
        var host = builder.Build();
        host.Start();
        return new(Host: host, PreviousCwd: previousCwd, BaseDirectory: baseDirectory, Cwd: cwd, LocalRepo: localRepo);
    }

    internal async Task TearDownAsync()
    {
        try { await Host.StopAsync(); }
        finally
        {
            try { Host.Dispose(); }
            finally { Directory.SetCurrentDirectory(PreviousCwd); }
        }
    }
}
