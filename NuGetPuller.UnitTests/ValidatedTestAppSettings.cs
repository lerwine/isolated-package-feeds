namespace NuGetPuller.UnitTests;

public class ValidatedTestAppSettings : IValidatedSharedAppSettings
{
    public Uri UpstreamServiceIndex { get; set; } = null!;
    public DirectoryInfo LocalRepository { get; set; } = null!;
    public DirectoryInfo GlobalPackagesFolder { get; set; } = null!;
}
