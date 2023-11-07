using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NuGet.Frameworks;
using NuGet.Versioning;
using Xunit;
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
    public void GetMetadataTest1()
    {
        var service = _host.Services.GetRequiredService<Services.RemoteNugetClientService>();
        string packageId = "Microsoft.Extensions.DependencyInjection";
        bool includePrerelease = false;
        bool includeUnlisted = false;
        var result = service.GetMetadataAsync(packageId, includePrerelease, includeUnlisted, CancellationToken.None).Result;
        Assert.NotNull(result);
    }

    [Fact]
    public void GetMetadataTest2()
    {
        var service = _host.Services.GetRequiredService<Services.RemoteNugetClientService>();
        string packageId = "Microsoft.Extensions.DependencyInjection";
        var version = NuGetVersion.Parse("7.0.13");
        var result = service.GetMetadataAsync(packageId, version, CancellationToken.None).Result;
        Assert.NotNull(result);
    }

    [Fact]
    public void GetAllVersionsTest()
    {
        var service = _host.Services.GetRequiredService<Services.RemoteNugetClientService>();
        string packageId = "Microsoft.Extensions.DependencyInjection";
        var result = service.GetAllVersionsAsync(packageId, CancellationToken.None).Result;
        Assert.NotNull(result);
    }

    [Fact]
    public void GetDependencyInfoTest()
    {
        var service = _host.Services.GetRequiredService<Services.RemoteNugetClientService>();
        string packageId = "Microsoft.Extensions.DependencyInjection";
        var version = NuGetVersion.Parse("7.0.13");
        var result = service.GetDependencyInfoAsync(packageId, version, CancellationToken.None).Result;
        Assert.NotNull(result);
    }

    [Fact]
    public void DoesPackageExistTest()
    {
        var service = _host.Services.GetRequiredService<Services.RemoteNugetClientService>();
        string packageId = "Microsoft.Extensions.DependencyInjection";
        var version = NuGetVersion.Parse("7.0.13");
        var result = service.DoesPackageExistAsync(packageId, version, CancellationToken.None).Result;
        Assert.True(result);
    }
}