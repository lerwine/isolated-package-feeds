namespace NuGetPuller.UnitTests.Helpers;

public class TestAppSettings : ISharedAppSettings
{
    public string UpstreamServiceIndex { get; set; } = string.Empty;

    public string? OverrideUpstreamServiceIndex { get; set; }

    public string LocalRepository { get; set; } = string.Empty;

    public string? OverrideLocalRepository { get; set; }

    public string GlobalPackagesFolder { get; set; } = string.Empty;

    public string? OverrideGlobalPackagesFolder { get; set; }

    public ValidatedTestAppSettings Validated => new();

    IValidatedSharedAppSettings ISharedAppSettings.Validated => Validated;
}
