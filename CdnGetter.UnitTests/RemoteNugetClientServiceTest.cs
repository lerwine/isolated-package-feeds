using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;
using Xunit.Abstractions;

namespace CdnGetter.UnitTests;

public class RemoteNugetClientServiceTest : IDisposable
{
    private readonly ITestOutputHelper _output;
    private readonly IHost _host;

    public RemoteNugetClientServiceTest(ITestOutputHelper output)
    {
        _output = output;
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();
        builder.Logging.AddConsole();
        builder.Services.Configure<Config.AppSettings>(builder.Configuration.GetSection(nameof(CdnGetter)));
        builder.Services.AddSingleton<Services.RemoteNugetClientService>();
        _host = builder.Build();
        _host.Start();
    }

    public void Dispose()
    {
        _host.Dispose();
    }

    [Fact]
    public async void GetMetadataTest1()
    {
        var service = _host.Services.GetRequiredService<Services.RemoteNugetClientService>();
        string packageId = "Microsoft.Extensions.DependencyInjection";
        bool includePrerelease = false;
        bool includeUnlisted = false;
        var result = await service.GetMetadataAsync(packageId, includePrerelease, includeUnlisted, CancellationToken.None);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async void GetMetadataTest2()
    {
        var service = _host.Services.GetRequiredService<Services.RemoteNugetClientService>();
        string packageId = "Microsoft.Extensions.DependencyInjection";
        var version = NuGetVersion.Parse("7.0.0");
        var result = await service.GetMetadataAsync(packageId, version, CancellationToken.None);
        Assert.NotNull(result);
    }

    [Fact]
    public async void GetAllVersionsTest()
    {
        var service = _host.Services.GetRequiredService<Services.RemoteNugetClientService>();
        string packageId = "Microsoft.Extensions.DependencyInjection";
        var result = await service.GetAllVersionsAsync(packageId, CancellationToken.None);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async void GetDependencyInfoTest()
    {
        var service = _host.Services.GetRequiredService<Services.RemoteNugetClientService>();
        string packageId = "Microsoft.Extensions.DependencyInjection";
        var version = NuGetVersion.Parse("7.0.0");
        var result = await service.GetDependencyInfoAsync(packageId, version, CancellationToken.None);
        Assert.NotNull(result);
    }

    [Fact]
    public async void DoesPackageExistTest()
    {
        var service = _host.Services.GetRequiredService<Services.RemoteNugetClientService>();
        string packageId = "Microsoft.Extensions.DependencyInjection";
        var version = NuGetVersion.Parse("7.0.0");
        var result = await service.DoesPackageExistAsync(packageId, version, CancellationToken.None);
        Assert.True(result);
    }
}