namespace NuGetPuller.UnitTests.Helpers;

public class TestAppSettings : ISharedAppSettings
{
    public string UpstreamServiceIndexUrl { get; set; } = string.Empty;

    public string? OverrideUpstreamServiceIndex { get; set; }

    public string DownloadedPackagesFolder { get; set; } = string.Empty;

    public string? OverrideDownloadedPackagesFolder { get; set; }

    public string GlobalPackagesFolder { get; set; } = string.Empty;

    public string? OverrideGlobalPackagesFolder { get; set; }
}
