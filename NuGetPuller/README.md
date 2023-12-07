# NuGetPuller Project

The purpose of this application is to synchronize a local NuGet repository with an upstream NuGet repository as well as other locally-hosted NuGet repositories.

An example use-case would be for a locally-hosted NuGet repository on a machine that does not have direct access to the internet. You can use this to download NuGet packages, including any dependencies, and then create a bundle of packages (a ZIP file) which can then be transferred to that disconnected host and imported into the local NuGet repository.

This references package manifest files to represent which packages already exist in other locally-hosted NuGet repositorys. This minimizes bundle sizes, because it doesn't have to include packages that already exist in the target NuGet repository.

## Command Line Arguments

### Get Latest Updates From Upstream NuGet Repository

- `--update-all` - **Required:** *This command line switch does not have any associated value.*

Example: `NuGetPuller --update-all`

### Update Specific Packages From Upstream NuGet Repository

- `-u` - **Required:** Identifier(s) of packages to be updated.
  - Multiple package identifiers are separated by commas.

Example: `NuGetPuller -u Microsoft.Extensions.Configuration.Abstractions,Microsoft.Extensions.DependencyInjection.Abstractions`

### Add New Packages From Upstream NuGet Repository

- `-a` - **Required:** Identifier(s) of packages to be downloaded and added to the local repository.
  - Multiple package identifiers are separated by commas.

### Import Package Files Into Local NuGet Repository

- `-i` - **Required:** Import package file(s) into the local NuGet repository.
  - This can either refer to a single `.nupkg` file or to a subdirectory containing `.nupkg` files.
  - If this refers to a subdirectory, it will not recursively search nested sub-directories.

Example: `NuGetPuller -i C:\users\john.doe\Downloads\MyPackageFolder`

### Delete Packages From Local NuGet Repository

- `-d` - **Required:** Identifier(s) of packages to be removed from the local repository.
  - Multiple package identifiers are separated by commas.

Example: `NuGetPuller -d PackageA,PackageB`

### List Packages Stored In Local NuGet Repository

- `-l` - **Required:** *This command line switch does not have any associated value.*

Example: `NuGetPuller -l`

### Create File Transfer Bundle

- `-b` - **Required:** Path of bundle file to create.
  - If no extension is specified, this will use the `.zip` file extension.
  - *Example:* `NuGetPuller -b MyAirgapped_2023-12-06.zip`
- `-t` - *Optional:* Path of the package manifest file for the locally-hosted NuGet repository that this bundle is being created for.
  - If this is not specified, then this will look for a `.json` file with the same base file name and location as the path specified by the `-b` argument.
  - If the package manifest file for the target NuGet repository does not exist, one it will be created.
  - *Example:* `NuGetPuller -b MyAirgapped_2023-12-06.zip -t MyAirgappedFeed.json`
- `--save-target-manifest-as` - *Optional:* Alternate path to save updated package manifest file to.
  - If this is not specified, then the package manifest file for the target NuGet repository will be updated to include the packages that are contained in the bundle file specified by the `-b` argument.
  - *Example:* `NuGetPuller -b MyAirgapped_2023-12-06.zip -t MyAirgappedFeed.json --save-target-manifest-as=MyAirgappedFeed-New.json`

### Create Package Fanifest File From Local NuGet Repository

- `--export-local-manifest` - **Required:** Path of `.json` file to create which will be a manifest of all packages in the local NuGet repository.
  - *Example:* `NuGetPuller --export-local-manifest=MyLocalRepoManifest.json`

### Override Application Settings

The following optional command line arguments can be used to override [Applicaton Settings](#application-settings),
and can be used in combination with other command line arguments:

- `--local-repository` - Override the [LocalPath](#local-nuget-repository-path) setting.
  - *Example:* `NuGetPuller --local-repository=C:\users\john.doe\Documents\MyLocalRepository -i mycustom.nupkg`
- `--upstream-service-index` - Override the [UpstreamServiceIndex](#upstream-service-index-url) setting.
  - *Example:* `NuGetPuller --upstream-service-index=file://myserver/myshare --update-all`
- `--global-packages-folder` - Override the [GlobalPackagesFolder](#global-packages-folder-path) setting.
  - *Example:* `NuGetPuller --global-packages-folder=C:\users\john.doe\Downloads\MyNuGetGpf -u Microsoft.EntityFrameworkCore`

## Application Settings

Following are the configuration options for the `NuGetPuller` section of [appsettings.json](./appsettings.json):

### Upstream Service Index URL

- `UpstreamServiceIndex` - The URL of the upstream NuGet server.
  - The default value is `https://api.nuget.org/v3/index.json`.
  - This can also be the path to a subdirectory, if you want to use a local folder as the upstream NuGet repository.

### Local NuGet Repository Path

- `LocalRepository` - The path of local NuGet repository.
  - The default location of the local NuGet repository is a folder named `LocalSource` in the same subdirectory as the `NuGetPuller` executable.

### Global Packages Folder Path

- `GlobalPackagesFolder` - Explicitly specifies the location of the global NuGet packages folder.

### Example application settings

```json
{
    "NuGetPuller": {
        "UpstreamServiceIndex": "file://myserver/myshare",
        "LocalRepository": "C:/users/john.doe/Documents/MyLocalRepository",
        "GlobalPackagesFolder": "C:/users/john.doe/Downloads/MyNuGetGpf"
    }
}
```

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
- [Overview of Hosting Your Own NuGet Repositorys | Microsoft Learn](https://learn.microsoft.com/en-au/nuget/hosting-packages/overview)
  - [Using NuGet.Server to Host NuGet Repositorys | Microsoft Learn](https://learn.microsoft.com/en-au/nuget/hosting-packages/overview)
  - [Setting up Local NuGet Repositorys | Microsoft Learn](https://learn.microsoft.com/en-au/nuget/hosting-packages/local-repositorys)
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
