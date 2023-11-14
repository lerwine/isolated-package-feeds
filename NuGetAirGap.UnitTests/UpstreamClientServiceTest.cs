using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace NuGetAirGap.UnitTests;

[TestFixture]
public class UpstreamClientServiceTest
{
    private IHost _host;

    [SetUp]
    public void Setup()
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();
        builder.Logging.AddDebug();
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(NuGetAirGap)));
        builder.Services.AddSingleton<UpstreamClientService>();
        _host = builder.Build();
        _host.Start();
    }

    [TearDown]
    public void TearDown() => _host.Dispose();

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