# CdnGetter Project

## Command Line Arguments

- `--update-all` - Update all existing packages in the local NuGet repository.
- `-u [package-id]`*[,package-id,...]* - Update specified packages in the local NuGet repository.
- `-a [package-id]`*[,package-id,...]* - Add packages from remote repository to local repository.
- `-d [package-id]`*[,package-id,...]* - Delete packages from local repository.
- `-l` - Lists packages stored in the local NuGet repository.
- `--export-package-listing=[path.json]` - Export package metadata for packages stored in local repository.
- `--local-repository=[subdirectory]` - Override path of local NuGet repository subdirectory.
- `--upstream-service-index=[URL_or_subdirectory]` - Override URL of upstream NuGet server. This can also be the path to a subdirectory to use a local folder as the upstream repository.
- `--global-packages-folder=[subdirectory]` - Override path of the global NuGet packages folder.

## App Settings

Following are the configuration options for the `NuGetAirGap` section of [appsettings.json](./appsettings.json):

- `UpstreamServiceIndex` - The URL of the upstream NuGet server. The default is `https://api.nuget.org/v3/index.json`. This can also be the path to a subdirectory.
- `LocalRepository` - The path of local NuGet repository subdirectory, relative to the current working directory. The default value is a folder named `LocalSource`, relative to the assembly location.
- `GlobalPackagesFolder` - Overrides the path of the global NuGet packages folder.

## Development

### NuGet

- [NuGet Server API](https://learn.microsoft.com/en-us/nuget/api/overview)
  - [Exploring the NuGet v3 Libraries, Part 1: Introduction and concepts](http://daveaglick.com/posts/exploring-the-nuget-v3-libraries-part-1)
  - [Exploring the NuGet v3 Libraries, Part 2](https://www.daveaglick.com/posts/exploring-the-nuget-v3-libraries-part-2)
  - [Exploring the NuGet v3 Libraries, Part 3: Installing packages](http://daveaglick.com/posts/exploring-the-nuget-v3-libraries-part-3)
  - [Revisiting the NuGet v3 Libraries](https://martinbjorkstrom.com/posts/2018-09-19-revisiting-nuget-client-libraries)
- [Sources](https://github.com/NuGet)
  - [NuGet.Protocol](https://www.nuget.org/packages/NuGet.Protocol)
    - [Repository](https://github.com/NuGet/NuGet.Client/tree/dev/src/NuGet.Core/NuGet.Protocol)
  - [NuGet.Packaging](https://www.nuget.org/packages/NuGet.Packaging/)
    - [Repository](https://github.com/NuGet/NuGet.Client/tree/dev/src/NuGet.Core/NuGet.Packaging)
  - [NuGetGallery](https://github.com/NuGet/NuGetGallery)
  - [NuGet.Server](https://github.com/NuGet/NuGet.Server)
  - [NuGet.Client](https://github.com/NuGet/NuGet.Client)
- [Overview of Hosting Your Own NuGet Feeds | Microsoft Learn](https://learn.microsoft.com/en-au/nuget/hosting-packages/overview)
  - [Using NuGet.Server to Host NuGet Feeds | Microsoft Learn](https://learn.microsoft.com/en-au/nuget/hosting-packages/overview)
  - [Setting up Local NuGet Feeds | Microsoft Learn](https://learn.microsoft.com/en-au/nuget/hosting-packages/local-feeds)
- [NuGet Command-Line Interface (CLI) Reference | Microsoft Learn](https://learn.microsoft.com/en-us/nuget/reference/nuget-exe-cli-reference?source=recommendations)
- [Creating a local NuGet repository for offline development | Codurance](https://www.codurance.com/publications/2015/05/04/creating-a-local-nuget-repository)
- [Installing a .nupkg on an offline machine | Nathan Manzi's Blog](https://nmanzi.com/blog/installing-nupkg-offline)

#### Example Projects

- [NugetDownloader](https://github.com/paraspatidar/NugetDownloader)
- [NetCoreNugetServer](https://github.com/emresenturk/NetCoreNugetServer)
- [MinimalNugetServer](https://github.com/TanukiSharp/MinimalNugetServer)

#### NuGet Alternatives

- [NuGet packages in the Package Registry | GitLab](https://docs.gitlab.com/ee/user/packages/nuget_repository/): May require licensing.
- [svenkle/nuget-server: A stand-alone hosted wrapper of the NuGet.Server package](https://github.com/lerwine/nuget-server): Uses Nuget.Server and has an MSI package. Developer is not a U.S. citizen and does not show prior history in GitHub.
- [NSubstitute](https://nsubstitute.github.io/) ([Creating a local NuGet repository for offline development | Codurance](https://www.codurance.com/publications/2015/05/04/creating-a-local-nuget-repository)): May not work in environments that are never routable to the public internet.
