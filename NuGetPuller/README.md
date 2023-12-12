# NuGetPuller Common Library

The purpose of this library is the basae class for applications that synchronize a local NuGet repository with an upstream NuGet repository as well as other locally-hosted NuGet repositories.

An example use-case would be for a locally-hosted NuGet repository on a machine that does not have direct access to the internet. You can use this to download NuGet packages, including any dependencies, and then create a bundle of packages (a ZIP file) which can then be transferred to that disconnected host and imported into the local NuGet repository.

This references package manifest files to represent which packages already exist in other locally-hosted NuGet repositorys. This minimizes bundle sizes, because it doesn't have to include packages that already exist in the target NuGet repository.

## Application Settings

Following are the configuration options for the `NuGetPuller` configuration section:

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

### High Performance Logging

You can use the [CS Code Snippets](../.vscode/cs.code-snippets) included with this project to create extension methods for high-performance logging.
The extension methods should be created on the [AppLoggerExtensions](./AppLoggerExtensions.cs) class.
The event name will be the same as the [AppLoggerExtensions.NuGetPullerEventId enum](./AppLoggerExtensions.NuGetPullerEventId.cs) field as well as the extension method.

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
