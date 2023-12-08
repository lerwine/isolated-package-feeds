namespace NuGetPuller;

public interface IValidatedSharedAppSettings
{
    /// <summary>
    /// Specifies the remote endpoint URL for the V3 NGet API or a subdirectory path that contains the upstream NuGet repository.
    /// </summary>
    Uri UpstreamServiceIndex { get; set; }

    string GetUpstreamServiceIndex();

    /// <summary>
    /// Specifies the relative or absolute path of a subdirectory for a local Nuget repository.
    /// </summary>
    DirectoryInfo LocalRepository { get; set; }

    string GetLocalRepository();

    /// <summary>
    /// Specifies the relative or absolute path of the NuGet global packages folder.
    /// </summary>
    DirectoryInfo GlobalPackagesFolder { get; set; }

    string GetGlobalPackagesFolder();
}
