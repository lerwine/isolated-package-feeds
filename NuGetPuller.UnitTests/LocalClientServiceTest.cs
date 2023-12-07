using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;
namespace NuGetPuller.UnitTests;

public class LocalClientServiceTest
{
    private IHost _host;
    private DirectoryInfo _baseDirectory = null!;
    private DirectoryInfo _cwd = null!;
    private string _previousCwd = null!;

    private const string DIRNAME_CWD = "CWD";

    private const string DIRNAME_ContentRoot = "ContentRoot";

    [SetUp]
    public void Setup()
    {
        var testContext = TestContext.CurrentContext;
        if (!(_baseDirectory = new DirectoryInfo(Path.Combine(testContext.WorkDirectory, nameof(UpstreamClientServiceTest)))).Exists)
            _baseDirectory.Create();
        if (!(_cwd = new DirectoryInfo(Path.Combine(_baseDirectory.FullName, DIRNAME_CWD))).Exists)
            _cwd.Create();
        _previousCwd = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(_cwd.FullName);
        HostApplicationBuilder builder = AppHost.CreateBuilder(testContext.TestDirectory);
        AppHost.ConfigureSettings(builder);
        AppHost.ConfigureLogging(builder);
        builder.Logging.AddDebug();
        AppHost.ConfigureServices(builder, settings => AppHost.DefaultPostConfigure(settings, builder));
        _host = builder.Build();
        _host.Start();
    }

    [TearDown]
    public async Task TearDown()
    {
        try { await _host.StopAsync(); }
        finally
        {
            try { _host.Dispose(); }
            finally { Directory.SetCurrentDirectory(_previousCwd); }
        }
    }

    [Test]
    public async Task AddPackageAsyncTest()
    {
        using TempStagingFolder tempStagingFolder = new();
        var upstreamService = _host.Services.GetRequiredService<UpstreamClientService>();
        var target = _host.Services.GetRequiredService<LocalClientService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        var version = NuGetVersion.Parse("7.0.0");
        var fileInfo = await tempStagingFolder.NewRandomFileInfoAsync(async (stream, token) =>
        {
            await target.CopyNupkgToStreamAsync(packageId, version, stream, token);
        }, ".nupkg", CancellationToken.None);
        var result = await target.AddPackageAsync(fileInfo.FullName, false, CancellationToken.None);
        Assert.That(result, Is.True);
    }
}