# NuGetPuller CLI

- [About NuGetPuller CLI](#about-nugetpuller-cli)
- [Links](#links)
- [Command Line Options](#command-line-options)
  - [Show All Packages](#show-all-packages)
  - [Download Packages](#download-packages)
  - [Add Package Files](#add-package-files)
  - [Remove Packages](#remove-packages)
  - [Check Package Dependencies](#check-package-dependencies)
  - [Export Local NuGet Feed Metadata](#export-local-nuget-feed-metadata)
  - [Create File Transfer Bundle](#create-file-transfer-bundle)
  - [Override Application Settings](#override-application-settings)
- [Application Settings](#application-settings)

## About NuGetPuller CLI

The purpose of this application is to synchronize packages between an authoritative [NuGet](https://www.nuget.org/) repository and Offline NuGet Package Feeds.

An example use-case would be for a locally-hosted NuGet feed on a machine that does not have direct access to the internet. You can use this to download NuGet packages, including any dependencies, and then create a bundle of packages (a ZIP file) which can then be transferred to that disconnected host and imported into the Local NuGet Feed.

See [Key Concepts](../NuGetPuller/README.md#key-concepts) for a conceptual overview of terms used by this application.

## Links

- [Isolate Pacakge Feeds Home](../README.md)
- [NuGetPuller Class Library](../NuGetPuller/README.md) - Base class library for User Interface Applications.
- [NuGetPuller Unit Tests](../NuGetPuller.UnitTests/README.md)

## Command Line Options

### Show All Packages

Command line options for showing all packages that have been downloaded.

- `--list` **Required** *(stand-alone switch)*: List all [Locally Downloaded Packages](../NuGetPuller/README.md#locally-downloaded-packages-folder).
- `--include-versions` *(Optional - stand-alone switch)*: Show all version numbers of each downloaded package.

Example:

```bash
NuGetPuller --list --include-versions
```

### Download Packages

Download packages from the [Upstream NuGet Repository](../NuGetPuller/README.md#upstream-nuget-repository) and add them to the [Downloaded Packages Feed](../NuGetPuller/README.md#locally-downloaded-packages-folder).

- `--download` **Required**: Identifier(s) of package to download.
  
  Multiple package identifiers are to be separated by commas.
- `--version` *(Optional)*: Specific package version to be downloaded. You can use the keyword `all` to download all versions.
  
  Multiple package identifiers are to be separated by commas.
  
  If no version is specified, the latest version of each package will be downloaded.
- `--no-dependencies` *(Optional - stand-alone switch)*: Do not download dependencies.
  If this switch is not used, then all dependencies will be downloaded and added to the [Downloaded Packages Feed](../NuGetPuller/README.md#locally-downloaded-packages-folder) as well.

Example:

```bash
NuGetPuller --download=Microsoft.Extensions.Logging,Microsoft.Extensions.Configuration --version=8.0.0,7.0.0 --no-dependencies
```

### Add Package Files

Add package file(s) to the [Downloaded Packages Feed](../NuGetPuller/README.md#locally-downloaded-packages-folder).

- `--add-file` **Required**: Path to an individual `.nupkg` file, a `.zip` file containing package files, or to a subdirectory of `.nupkg` files. This will not recursively search sub-folders.
  
  Multiple paths can be specified, separated by semi-colons.

Example #1: Add Single Package Files

```bash
NuGetPuller --add-file="microsoft.extensions.logging.8.0.0.nupkg;microsoft.extensions.logging.7.0.0.nupkg"
```

Example #2: Add Package Files From Subdirectory

```bash
NuGetPuller --add-file=john.doe/downloads
```

### Remove Packages

Remove packages from the [Downloaded Packages Feed](../NuGetPuller/README.md#locally-downloaded-packages-folder).

- `--remove` **Required**: Identifier of package to remove.
  
  Multiple package identifiers can be specified, separated by commas.
- `--version` *(Optional)*: Specific version(s) of package(s) to be removed.
  
  Multiple versions can be specified, separated by commas.
  
  If the version is not specified, all versions of the specified packages will be removed.
- `--save-to` *(Optional)*: Subdirectory to save packages to before removing them.

```bash
NuGetPuller --remove=Microsoft.Extensions.Logging,Microsoft.Extensions.Configuration --version=8.0.0,7.0.0 --save-to=john.doe/downloads
```

### Check Package Dependencies

Verify that all dependiencies have been downloaded for all packages in the [Downloaded Packages Feed](../NuGetPuller/README.md#locally-downloaded-packages-folder).

- `--check-depencencies` **Required** *(stand-alone switch)*: Check locally downloaded packages for dependencies.
- `--package-id` *(Optional)*: Identifier of pecific package to check.
  
  Multiple package identifiers can be specified, separated by commas.
  
  If the package identifer is not specified, then all packages in the Local NuGet Feed will be checked.
- `--version` *(Optional)*: Specific version of package to check.
  
  Multiple versions can be specified, separated by commas.
  
  If the version is not specified, all package versions will be checked.

  This argument will be ignored if the `--package-id` argument is not present.
- `--no-download` *(Optional - stand-alone switch)*: Do not download missing dependencies.
  
  If this switch is not specified, any missing dependencies will be downloaded from the Upstream NuGet Feed, if they are found.

Example:

```bash
NuGetPuller --check-depencencies=System.Text.Json,Microsoft.Extensions.Hosting --no-download
```

### Export Local NuGet Feed Metadata

Create a [NuGet Feed Metadata File](../NuGetPuller/README.md#nuget-feed-metadata-file) that represents all versions of all packages in the [Downloaded Packages Feed](../NuGetPuller/README.md#locally-downloaded-packages-folder).

- `--export-metadata` **Required:**: Path of the metadata file to create.

Example:

```bash
NuGetPuller --export-metadata=MyLocaRepo.json
```

### Create File Transfer Bundle

This creates a ZIP file which contains NuGet packages to be transferred to an [Offline NuGet Feed](../NuGetPuller/README.md#offline-nuget-feed).
To efficiently synchronize content with another NuGet feed, this can reference a [NuGet Feed Metadata File](../NuGetPuller/README.md#nuget-feed-metadata-file)
to know which packages already exist in the target feed.

Command line arguments are as follows:

- `--create-bundle` **Required:**: Path of bundle file to create.
  
  If no extension is specified, this will use the `.zip` file extension.
- `--create-from` *(Optional)*: Path of a [NuGet Feed Metadata File](../NuGetPuller/README.md#nuget-feed-metadata-file) representing the target [Offline NuGet Feed](../NuGetPuller/README.md#offline-nuget-feed).
  
  If this option is used, the output bundle will not contain any packages that are represented in the metadata file.
- `--save-metadata-to` *(Optional)*: Path of file to save the updated [NuGet Feed Metadata File](../NuGetPuller/README.md#nuget-feed-metadata-file) to.
  
  If the `--create-from` was used, the new metadata file will reflect the contents of the associated [Offline NuGet Feed](../NuGetPuller/README.md#offline-nuget-feed) after the packages from output bundle have been added; otherwise, the metadata file will contain all packages that were exported to the `.zip` file.

  It is okay if the `--create-from` and `--save-metadata-to` options refer to the same file.
- `--package-id` *(Optional)*: Comma-separated list of specific package IDs to include in the bundle.
  
  If this option is not specified, then all [locally downloaded](../NuGetPuller/README.md#locally-downloaded-packages-folder) packages not represented in the `--create-from` [metadata file](../NuGetPuller/README.md#nuget-feed-metadata-file) will be included in the bundle.
- `--version` *(Optional)*: Comma-separated list of specific versions to include in the bundle.
  
  If this option is not specified, then all [locally downloaded](../NuGetPuller/README.md#locally-downloaded-packages-folder) versions not represented in the `--create-from` [metadata file](../NuGetPuller/README.md#nuget-feed-metadata-file) will be included in the bundle.
  
```bash
NuGetPuller --create-bundle=Updates.zip --create-from=DisconnectedServer.nuget.metadata.json --save-metadata-to=DisconnectedServer.nuget.metadata.json
```

### Override Application Settings

The following optional command line arguments can be used to override [Applicaton Settings](../NuGetPuller/README.md#application-settings),
and can be used in combination with other command line arguments:

- `--local-feed-path`: Override the [LocalFeedPath](../NuGetPuller/README.md#local-nuget-feed-path) setting.
  
  Example:

  ```bash
  NuGetPuller --local-feed-path=john.doe/downloads --check-depencencies="System.Text.Json" --no-download
  ```

- `--upstream-service-index`: Override the [UpstreamServiceIndexUrl](../NuGetPuller/README.md#upstream-service-index-url) setting.
  
  Example:

  ```bash
  NuGetPuller --upstream-service-index=file://myserver/myshare --download="System.Text.Json" --version=8.0.0
  ```
  
- `--global-packages-folder`: Override the [GlobalPackagesFolder](../NuGetPuller/README.md#global-packages-folder-path) setting.
  Example:

  ```bash
  NuGetPuller --global-packages-folder=ohn.doe/Downloads/MyNuGetGpf --check-depencencies
  ```

## Application Settings

See [NuGet Common Class Library Application Settings](../NuGetPuller/README.md#application-settings) for the  configuration options for the `NuGetPuller` section of [appsettings.json](./appsettings.json).
