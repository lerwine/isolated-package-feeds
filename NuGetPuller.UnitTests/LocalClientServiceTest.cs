using Microsoft.Extensions.DependencyInjection;
using IsolatedPackageFeeds.Shared;
using NuGet.Versioning;
using NuGetPuller.UnitTests.Helpers;

namespace NuGetPuller.UnitTests;

public class DownloadedPackagesServiceTest
{
    private HostingFake _hosting;

    [SetUp]
    public void Setup() => _hosting = HostingFake.Setup<DownloadedPackagesServiceTest>();

    [TearDown]
    public async Task TearDown() => await _hosting.TearDownAsync();

    [Test]
    public async Task AddPackageAsyncTest()
    {
        using TempStagingFolder tempStagingFolder = new();
        var target = _hosting.Host.Services.GetRequiredService<IDownloadedPackagesService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        var version = NuGetVersion.Parse("7.0.0");
        Assert.That(await target.DoesPackageExistAsync(packageId, version, CancellationToken.None), Is.False);
        FileInfo fileInfo;
        try
        {
            var upstreamService = _hosting.Host.Services.GetRequiredService<IUpstreamNuGetClientService>();
            fileInfo = await tempStagingFolder.NewRandomFileInfoAsync(async (stream, token) =>
            {
                await upstreamService.CopyNupkgToStreamAsync(packageId, version, stream, token);
            }, ".nupkg", CancellationToken.None);
        }
        catch (Exception exception)
        {
            Assert.Inconclusive($"Failed to download NuGet package: {exception}");
            return;
        }
        if (!fileInfo.Exists)
            Assert.Inconclusive("NuGet package failed to download.");
        if (fileInfo.Length == 0)
            Assert.Inconclusive("Downloaded NuGet package is empty.");
        await target.AddPackageAsync(fileInfo.FullName, false, CancellationToken.None);
        Assert.That(await target.DoesPackageExistAsync(packageId, version, CancellationToken.None), Is.True);
    }

    [Test]
    public async Task DeleteAsyncTest()
    {
        using TempStagingFolder tempStagingFolder = new();
        var target = _hosting.Host.Services.GetRequiredService<IDownloadedPackagesService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        var version1 = NuGetVersion.Parse("7.0.0");
        var version2 = NuGetVersion.Parse("6.0.4");
        FileInfo fileInfo1, fileInfo2;
        try
        {
            var upstreamService = _hosting.Host.Services.GetRequiredService<IUpstreamNuGetClientService>();
            fileInfo1 = await tempStagingFolder.NewRandomFileInfoAsync(async (stream, token) =>
            {
                await upstreamService.CopyNupkgToStreamAsync(packageId, version1, stream, token);
            }, ".nupkg", CancellationToken.None);
            fileInfo2 = await tempStagingFolder.NewRandomFileInfoAsync(async (stream, token) =>
            {
                await upstreamService.CopyNupkgToStreamAsync(packageId, version2, stream, token);
            }, ".nupkg", CancellationToken.None);
        }
        catch (Exception exception)
        {
            Assert.Inconclusive($"Failed to download NuGet package: {exception}");
            return;
        }
        if (!fileInfo1.Exists)
            Assert.Inconclusive($"NuGet package {packageId}, version {version1} failed to download.");
        if (fileInfo1.Length == 0)
            Assert.Inconclusive($"Downloaded NuGet package {packageId}, version {version1} is empty.");
        if (!fileInfo2.Exists)
            Assert.Inconclusive($"NuGet package {packageId}, version {version2} failed to download.");
        if (fileInfo2.Length == 0)
            Assert.Inconclusive($"Downloaded NuGet package {packageId}, version {version2} is empty.");
        try { await target.AddPackageAsync(fileInfo1.FullName, false, CancellationToken.None); }
        catch (Exception exception)
        {
            Assert.Inconclusive($"Failed to add NuGet package {packageId}, version {version1}: {exception}");
            return;
        }
        try { await target.AddPackageAsync(fileInfo2.FullName, false, CancellationToken.None); }
        catch (Exception exception)
        {
            Assert.Inconclusive($"Failed to add NuGet package {packageId}, version {version2}: {exception}");
            return;
        }
        bool exists;
        try { exists = await target.DoesPackageExistAsync(packageId, version1, CancellationToken.None); }
        catch (Exception exception)
        {
            Assert.Inconclusive($"Failed to verify added NuGet package {packageId}, version {version1}: {exception}");
            return;
        }
        if (!exists)
            Assert.Inconclusive($"Failed to add NuGet package {packageId}, version {version1}");
        try { exists = await target.DoesPackageExistAsync(packageId, version2, CancellationToken.None); }
        catch (Exception exception)
        {
            Assert.Inconclusive($"Failed to verify added NuGet package {packageId}, version {version2}: {exception}");
            return;
        }
        if (!exists)
            Assert.Inconclusive($"Failed to add NuGet package {packageId}, version {version2}");
        var deletedPackages = target.DeleteAsync(packageId, CancellationToken.None).ToBlockingEnumerable(CancellationToken.None).ToArray();
        Assert.That(deletedPackages, Is.Not.Null);
        Assert.That(deletedPackages, Has.Length.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(deletedPackages.Any(a => a.Version.Equals(version1) && a.Success), Is.True);
            Assert.That(deletedPackages.Any(a => a.Version.Equals(version2) && a.Success), Is.True);
        });
    }
}
