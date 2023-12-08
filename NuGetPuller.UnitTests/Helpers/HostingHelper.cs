using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NuGetPuller.UnitTests.Helpers;

record HostingFake(IHost Host, string PreviousCwd, DirectoryInfo BaseDirectory, DirectoryInfo Cwd)
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
        HostApplicationBuilder builder = AppHost.CreateBuilder(testContext.TestDirectory);
        AppHost.ConfigureSettings<TestAppSettings>(builder);
        AppHost.ConfigureLogging(builder);
        builder.Logging.AddDebug();
        AppHost.ConfigureServices<TestAppSettings, TestAppSettingsValidatorService, LocalClientService, UpstreamClientService>(builder, settings => AppHost.DefaultPostConfigure(settings, builder));
        var host = builder.Build();
        host.Start();
        return new(host, previousCwd, baseDirectory, cwd);
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