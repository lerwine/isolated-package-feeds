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
        var target = _hosting.Host.Services.GetRequiredService<IUpstreamNuGetClientService>();
        string packageId = "System.Text.Json";
        bool includePrerelease = false;
        bool includeUnlisted = false;
        var result = await target.GetMetadataAsync(packageId, includePrerelease, includeUnlisted, CancellationToken.None);
        Assert.That(result, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Select(m => m.Identity.Id), Is.All.EqualTo(packageId));
        // var json = new ResultWithType<IEnumerable<NuGet.Protocol.Core.Types.IPackageSearchMetadata>, NuGet.Protocol.Core.Types.IPackageSearchMetadata>(result).ToJson(Newtonsoft.Json.Formatting.Indented);
        // File.WriteAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../../Resources/System.Text.Json-Metadata.json"), json,
        //     new System.Text.UTF8Encoding(false, true));
    }

    [Test]
    public async Task GetMetadataTest2()
    {
        var target = _hosting.Host.Services.GetRequiredService<IUpstreamNuGetClientService>();
        string packageId = "System.Text.Json";
        var version = NuGetVersion.Parse("7.0.0");
        var result = await target.GetMetadataAsync(packageId, version, CancellationToken.None);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Identity.Id, Is.EqualTo(packageId));
            Assert.That(result.Identity.HasVersion, Is.True);
            Assert.That(result.Identity.Version, Is.Not.Null);
        });
        Assert.That(result.Identity.Version, Is.EqualTo(version));
    }

    [Test]
    public async Task GetAllVersionsTest()
    {
        var target = _hosting.Host.Services.GetRequiredService<IUpstreamNuGetClientService>();
        string packageId = "System.Text.Json";
        var result = await target.GetAllVersionsAsync(packageId, CancellationToken.None);
        Assert.That(result, Is.Not.Null.And.Not.Empty);
        // var json = new ResultWithType<IEnumerable<NuGetVersion>, NuGetVersion>(result).ToJson(Newtonsoft.Json.Formatting.Indented);
        // File.WriteAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../../Resources/System.Text.Json-AllVersions.json"), json,
        //     new System.Text.UTF8Encoding(false, true));
    }

    [Test]
    public async Task GetDependencyInfoTest()
    {
        var target = _hosting.Host.Services.GetRequiredService<IUpstreamNuGetClientService>();
        string packageId = "System.Text.Json";
        var version = NuGetVersion.Parse("7.0.0");
        var result = await target.GetDependencyInfoAsync(packageId, version, CancellationToken.None);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.PackageIdentity, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.PackageIdentity.Id, Is.EqualTo(packageId));
            Assert.That(result.PackageIdentity.HasVersion, Is.True);
            Assert.That(result.PackageIdentity.Version, Is.Not.Null);
        });
        Assert.That(result.PackageIdentity.Version, Is.EqualTo(version));
    }

    [Test]
    public async Task CopyNupkgToStreamAsyncTest()
    {
        var target = _hosting.Host.Services.GetRequiredService<IUpstreamNuGetClientService>();
        string packageId = "System.Text.Json";
        var version = NuGetVersion.Parse("7.0.0");
        MemoryStream destination = new();
        await target.CopyNupkgToStreamAsync(packageId, version, destination, CancellationToken.None);
        Assert.That(destination.Length, Is.GreaterThan(0L));
    }

    [Test]
    public async Task DoesPackageExistTest()
    {
        var target = _hosting.Host.Services.GetRequiredService<IUpstreamNuGetClientService>();
        string packageId = "System.Text.Json";
        var version = NuGetVersion.Parse("7.0.0");
        var result = await target.DoesPackageExistAsync(packageId, version, CancellationToken.None);
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ResolvePackageTest()
    {
        var target = _hosting.Host.Services.GetRequiredService<IUpstreamNuGetClientService>();
        string packageId = "System.Text.Json";
        var version = NuGetVersion.Parse("7.0.0");
        var framework = NuGetFramework.Parse("net8.0");
        var result = await target.ResolvePackageAsync(packageId, version, framework, CancellationToken.None);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(packageId));
            Assert.That(result.HasVersion, Is.True);
            Assert.That(result.Version, Is.Not.Null);
        });
        Assert.That(result.Version, Is.EqualTo(version));
    }

    [Test]
    public async Task ResolvePackagesTest1()
    {
        var target = _hosting.Host.Services.GetRequiredService<IUpstreamNuGetClientService>();
        string packageId = "System.Text.Json";
        var framework = NuGetFramework.Parse("net8.0");
        var result = await target.ResolvePackagesAsync(packageId, framework, CancellationToken.None);
        Assert.That(result, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Select(d => d.Id), Is.All.EqualTo(packageId));
        // var json = new ResultWithType<IEnumerable<NuGet.Protocol.Core.Types.SourcePackageDependencyInfo>, NuGet.Protocol.Core.Types.SourcePackageDependencyInfo>(result).ToJson(Newtonsoft.Json.Formatting.Indented);
        // File.WriteAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../../Resources/System.Text.Json_net8.0-ResolvedPackages.json"), json,
        //     new System.Text.UTF8Encoding(false, true));
    }

    [Test]
    public async Task ResolvePackagesTest2()
    {
        var target = _hosting.Host.Services.GetRequiredService<IUpstreamNuGetClientService>();
        string packageId = "System.Text.Json";
        var result = await target.ResolvePackagesAsync(packageId, CancellationToken.None);
        Assert.That(result, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Select(d => d.Identity.Id), Is.All.EqualTo(packageId));
        // var json = new ResultWithType<IEnumerable<NuGet.Protocol.Core.Types.RemoteSourceDependencyInfo>, NuGet.Protocol.Core.Types.RemoteSourceDependencyInfo>(result).ToJson(Newtonsoft.Json.Formatting.Indented);
        // File.WriteAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../../Resources/System.Text.Json-ResolvedPackages.json"), json,
        //     new System.Text.UTF8Encoding(false, true));
    }
}
