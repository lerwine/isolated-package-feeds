using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace NuGetAirGap.UnitTests;

[TestFixture]
public class UpstreamClientServiceTest
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
    public async Task GetMetadataTest1()
    {
        var service = _host.Services.GetRequiredService<UpstreamClientService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        bool includePrerelease = false;
        bool includeUnlisted = false;
        var result = await service.GetMetadataAsync(packageId, includePrerelease, includeUnlisted, CancellationToken.None);
        Assert.That(result, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task GetMetadataTest2()
    {
        var service = _host.Services.GetRequiredService<UpstreamClientService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        var version = NuGetVersion.Parse("7.0.0");
        var result = await service.GetMetadataAsync(packageId, version, CancellationToken.None);
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetAllVersionsTest()
    {
        var service = _host.Services.GetRequiredService<UpstreamClientService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        var result = await service.GetAllVersionsAsync(packageId, CancellationToken.None);
        Assert.That(result, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task GetDependencyInfoTest()
    {
        var service = _host.Services.GetRequiredService<UpstreamClientService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        var version = NuGetVersion.Parse("7.0.0");
        var result = await service.GetDependencyInfoAsync(packageId, version, CancellationToken.None);
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task DoesPackageExistTest()
    {
        var service = _host.Services.GetRequiredService<UpstreamClientService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        var version = NuGetVersion.Parse("7.0.0");
        var result = await service.DoesPackageExistAsync(packageId, version, CancellationToken.None);
        Assert.True(result);
    }
}