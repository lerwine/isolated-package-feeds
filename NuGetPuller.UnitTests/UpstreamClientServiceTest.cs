using Microsoft.Extensions.DependencyInjection;
using NuGet.Frameworks;
using NuGet.Versioning;
using NuGetPuller.UnitTests.Helpers;

namespace NuGetPuller.UnitTests;

[TestFixture]
public class UpstreamClientServiceTest
{
    private HostingFake _hosting;

    [SetUp]
    public void Setup() => _hosting = HostingFake.Setup<UpstreamClientServiceTest>();

    [TearDown]
    public async Task TearDown() => await _hosting.TearDownAsync();

    [Test]
    public async Task GetMetadataTest1()
    {
        var target = _hosting.Host.Services.GetRequiredService<UpstreamClientService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        bool includePrerelease = false;
        bool includeUnlisted = false;
        var result = await target.GetMetadataAsync(packageId, includePrerelease, includeUnlisted, CancellationToken.None);
        Assert.That(result, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task GetMetadataTest2()
    {
        var target = _hosting.Host.Services.GetRequiredService<UpstreamClientService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        var version = NuGetVersion.Parse("7.0.0");
        var result = await target.GetMetadataAsync(packageId, version, CancellationToken.None);
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetAllVersionsTest()
    {
        var target = _hosting.Host.Services.GetRequiredService<UpstreamClientService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        var result = await target.GetAllVersionsAsync(packageId, CancellationToken.None);
        Assert.That(result, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task GetDependencyInfoTest()
    {
        var target = _hosting.Host.Services.GetRequiredService<UpstreamClientService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        var version = NuGetVersion.Parse("7.0.0");
        var result = await target.GetDependencyInfoAsync(packageId, version, CancellationToken.None);
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task CopyNupkgToStreamAsyncTest()
    {
        var target = _hosting.Host.Services.GetRequiredService<UpstreamClientService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        var version = NuGetVersion.Parse("7.0.0");
        MemoryStream destination = new MemoryStream();
        await target.CopyNupkgToStreamAsync(packageId, version, destination, CancellationToken.None);
        Assert.That(destination.Length, Is.GreaterThan(0L));
    }

    [Test]
    public async Task DoesPackageExistTest()
    {
        var target = _hosting.Host.Services.GetRequiredService<UpstreamClientService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        var version = NuGetVersion.Parse("7.0.0");
        var result = await target.DoesPackageExistAsync(packageId, version, CancellationToken.None);
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ResolvePackageTest()
    {
        var target = _hosting.Host.Services.GetRequiredService<UpstreamClientService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        var version = NuGetVersion.Parse("7.0.0");
        var framework = NuGetFramework.Parse("net8.0");
        var result = await target.ResolvePackage(packageId, version, framework, CancellationToken.None);
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task ResolvePackagesTest1()
    {
        var target = _hosting.Host.Services.GetRequiredService<UpstreamClientService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        var framework = NuGetFramework.Parse("net8.0");
        var result = await target.ResolvePackages(packageId, framework, CancellationToken.None);
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task ResolvePackagesTest2()
    {
        var target = _hosting.Host.Services.GetRequiredService<UpstreamClientService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        var result = await target.ResolvePackages(packageId, CancellationToken.None);
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetGetAllDependenciesAsyncAsync()
    {
        var target = _hosting.Host.Services.GetRequiredService<UpstreamClientService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        var version = NuGetVersion.Parse("7.0.0");
        var framework = NuGetFramework.Parse("net8.0");
        var result = target.GetAllDependenciesAsync(packageId, version, framework, CancellationToken.None);
        Assert.That(result, Is.Not.Null);
        await foreach (var item in result)
        {
            Assert.That(item, Is.Not.Null);
            Assert.That(item.Id, Is.Not.EqualTo(packageId));
        }
    }

    [Test]
    public async Task GetAllVersionsWithDependenciesAsync()
    {
        var target = _hosting.Host.Services.GetRequiredService<UpstreamClientService>();
        var childPackageId = "Microsoft.Extensions.Logging.Abstractions";
        var parentPackageId = "Microsoft.Extensions.Logging";
        string[] packageIds = [childPackageId, parentPackageId];
        var result = target.GetAllVersionsWithDependenciesAsync(packageIds, CancellationToken.None);
        Assert.That(result, Is.Not.Null);
        bool foundParent = false;
        bool foundChild = false;
        await foreach (var item in result)
        {
            Assert.That(item, Is.Not.Null);
            if (item.Id == parentPackageId)
                foundParent = true;
            else if (item.Id == childPackageId)
                foundChild = true;
        }
        Assert.That(foundParent, Is.True);
        Assert.That(foundChild, Is.True);
    }
}