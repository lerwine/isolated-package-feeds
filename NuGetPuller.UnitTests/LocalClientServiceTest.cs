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
        var upstreamService = _hosting.Host.Services.GetRequiredService<UpstreamClientService>();
        var target = _hosting.Host.Services.GetRequiredService<LocalClientService>();
        string packageId = "Microsoft.Extensions.Logging.Abstractions";
        var version = NuGetVersion.Parse("7.0.0");
        var fileInfo = await tempStagingFolder.NewRandomFileInfoAsync(async (stream, token) =>
        {
            await target.CopyNupkgToStreamAsync(packageId, version, stream, token);
        }, ".nupkg", CancellationToken.None);
        await target.AddPackageAsync(fileInfo.FullName, false, CancellationToken.None);
        Assert.That(await target.DoesPackageExistAsync(packageId, version, CancellationToken.None), Is.True);
    }
}
