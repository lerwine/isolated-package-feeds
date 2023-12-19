# NuGetPuller Common Library

- [About NuGetPuller](#about-nugetpuller)
- [Key Concepts](#key-concepts)
  - [NuGet Server](#nuget-server)
  - [Local NuGet Package Feed](#local-nuget-package-feed)
  - [NuGet Repository](#nuget-repository)
  - [Offline NuGet Feed](#offline-nuget-feed)
  - [Upstream NuGet Repository](#upstream-nuget-repository)
  - [Locally Downloaded Packages Folder](#locally-downloaded-packages-folder)
  - [File Transfer Bundle](#file-transfer-bundle)
- [NuGet Feed Metadata File](#nuget-feed-metadata-file)
- [Links](#links)
- [Application Settings](#application-settings)
  - [Upstream Service Index URL](#upstream-service-index-url)
  - [Local NuGet Feed Path](#local-nuget-feed-path)
  - [Global Packages Folder Path](#global-packages-folder-path)
  - [Example application settings](#example-application-settings)
- [Development](#development)
  - [High Performance Logging](#high-performance-logging)
  - [NuGet](#nuget)
    - [Example Projects](#example-projects)
    - [NuGet Alternatives](#nuget-alternatives)

## About NuGetPuller

The purpose of this library is the basae class for applications that synchronize Local NuGet Feeds with an upstream NuGet repository.

An example use-case would be for a locally-hosted NuGet feed on a machine that does not have direct access to the internet. You can use this to download NuGet packages, including any dependencies, and then create a bundle of packages (a ZIP file) which can then be transferred to that disconnected host and imported into the Local NuGet Feed.

This uses package metadata files to represent which packages already exist in other locally-hosted NuGet feeds. This minimizes bundle sizes, because it doesn't have to include packages that already exist in the target NuGet feed.

## Key Concepts

### NuGet Server

A *"NuGet Server"* is a web-based service implementing the the [NuGet Server API](https://learn.microsoft.com/en-us/nuget/api/overview).

### Local NuGet Package Feed

A *"Local NuGet Package Feed"* is a subdirectory that contains specially-named `.nupkg` files representing individual versions of NuGet packages.

### NuGet Repository

This is a generic term for either a [NuGet Server](#nuget-server) or a [Local NuGet Feed](#local-nuget-package-feed).

### Offline NuGet Feed

An *"Offline NuGet Feed"* is a [Local NuGet Package Feed](#local-nuget-package-feed) for an environment that does not have internet access.
It is also conceptually assumed that the user interface application may not have direct access to this feed.

### Upstream NuGet Repository

This is the source [NuGet Repository](#nuget-repository) that packages will be downloaded from.

This can specified using the [UpstreamServiceIndexUrl application setting](#upstream-service-index-url).

Typically, this will be a [NuGet Server](#nuget-server) using `https://api.nuget.org/v3/index.json` as the URL.

### Locally Downloaded Packages Folder

This application saves locally-downloaded packages in a [Local NuGet Package Feed](#local-nuget-package-feed), which can be specified using the [DownloadedPackagesFolder application setting](#local-nuget-feed-path).

Typically, this is a folder named `LocalFeed` in the same subdirectory as the `NuGetPuller.CLI` executable.

### File Transfer Bundle

A ZIP file containing [locally-downloaded](#locally-downloaded-packages-folder) `.nupkg` files to be transferred to an [Offline NuGet Feed](#offline-nuget-feed).

## NuGet Feed Metadata File

This is a `JSON` file which contains metadata indicating what packages already exist in an [Offline NuGet Feed](#offline-nuget-feed).

## Links

- [Isolate Pacakge Feeds Home](../README.md)
- [NuGetPuller CLI](../NuGetPuller.CLI/README.md)
- [Unit Tests](../NuGetPuller.UnitTests/README.md)

## Application Settings

Following are the configuration options for the `NuGetPuller` configuration section:

### Upstream Service Index URL

- `UpstreamServiceIndexUrl` - The URL of the upstream NuGet server.
  - The default value is `https://api.nuget.org/v3/index.json`.
  - This can also be the path to a subdirectory, if you want to use a local folder as the upstream NuGet repository.

### Local NuGet Feed Path

- `DownloadedPackagesFolder` - The path of Local NuGet Feed.
  - The default location of the Local NuGet Feed is a folder named `LocalFeed` in the same subdirectory as the `NuGetPuller.CLI` executable.

### Global Packages Folder Path

- `GlobalPackagesFolder` - Explicitly specifies the location of the global NuGet packages folder.

### Example application settings

```json
{
    "NuGetPuller": {
        "UpstreamServiceIndexUrl": "file://myserver/myshare",
        "DownloadedPackagesFolder": "C:/users/john.doe/Documents/MyLocalFeed",
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
- [Overview of Hosting Your Own NuGet Repositories | Microsoft Learn](https://learn.microsoft.com/en-au/nuget/hosting-packages/overview)
  - [Using NuGet.Server to Host NuGet Repositories | Microsoft Learn](https://learn.microsoft.com/en-au/nuget/hosting-packages/overview)
  - [Setting up Local NuGet Repositories | Microsoft Learn](https://learn.microsoft.com/en-au/nuget/hosting-packages/local-Repositories)
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
