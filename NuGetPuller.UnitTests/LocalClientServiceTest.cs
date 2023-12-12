using Microsoft.Extensions.DependencyInjection;
using IsolatedPackageFeeds.Shared;
using NuGet.Versioning;
using NuGetPuller.UnitTests.Helpers;

namespace NuGetPuller.UnitTests;

public class LocalClientServiceTest
{
    private HostingFake _hosting;

    [SetUp]
    public void Setup() => _hosting = HostingFake.Setup<LocalClientServiceTest>();

    [TearDown]
    public async Task TearDown() => await _hosting.TearDownAsync();

    [Test]
    public async Task AddPackageAsyncTest()
    {
        using TempStagingFolder tempStagingFolder = new();
        var target = _hosting.Host.Services.GetRequiredService<LocalClientService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        var version = NuGetVersion.Parse("7.0.0");
        FileInfo fileInfo;
        try
        {
            var upstreamService = _hosting.Host.Services.GetRequiredService<UpstreamClientService>();
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
}
