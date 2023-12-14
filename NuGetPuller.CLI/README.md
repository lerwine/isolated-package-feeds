# NuGetPuller CLI

The purpose of this application is to synchronize packages between [NuGet](https://www.nuget.org/) feeds.

This application runs in the context of a Local NuGet Feed and an Upstream NuGet Feed or server.
> For the purposes of this application and documentation, a NuGet "Feed" refers to a filesystem subdirectory that contains properly named NuGet packages.
> and a Nuget "Repository" refers generically to a NuGet server or filesystem-based feed.

An example use-case would be for a locally-hosted NuGet feed on a machine that does not have direct access to the internet. You can use this to download NuGet packages, including any dependencies, and then create a bundle of packages (a ZIP file) which can then be transferred to that disconnected host and imported into the Local NuGet Feed.

This references package manifest files to represent which packages already exist in other locally-hosted NuGet feeds. This minimizes bundle sizes, because it doesn't have to include packages that already exist in the target NuGet feed.

## Application Settings

See [NuGet Common Class Library Application Settings](../NuGetPuller/README.md#application-settings) for the  configuration options for the `NuGetPuller` section of [appsettings.json](./appsettings.json).

## Command Line Options

### Show All Packages

Command line options for showing all packages in the local feed.

- `--list` **Required** *(stand-alone switch)*: List all packages in the local feed.
- `--include-versions` *(Optional - stand-alone switch)*: Show all version numbers of each package in the local feed.

Example:

```bash
NuGetPuller --list --include-versions
```

### Download Packages

Download packages from the upstream NuGet repository and add them to the local feed.

- `--download` **Required**: Identifier of package to download.
  
  Multiple package identifiers can be specified, separated by commas.
- `--version` *(Optional)*: Specific version of package to be downloaded. You can use the keyword `all` to download all packages.
  
  Multiple versions can be specified, separated by commas.
  
  If the version is not specified, the latest version will be downloaded.
- `--no-dependencies` *(Optional - stand-alone switch)*: Do not download dependencies.
  If this switch is not used, then all dependencies will be downloaded and added to the local feed as well.

Example:

```bash
NuGetPuller --download=Microsoft.Extensions.Logging,Microsoft.Extensions.Configuration --version=8.0.0,7.0.0 --no-dependencies
```

### Add Package Files

Add package files to the Local NuGet Feed.

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

Remove packages from the Local NuGet Feed.

- `--remove` **Required**: Identifier of package to remove.
  
  Multiple package identifiers can be specified, separated by commas.
- `--version` *(Optional)*: Specific version of package to be downloaded.
  
  Multiple versions can be specified, separated by commas.
  
  If the version is not specified, all versions will be removed.
- `--save-to` *(Optional)*: Subdirectory to save packages to before removing them.

```bash
NuGetPuller --remove=Microsoft.Extensions.Logging,Microsoft.Extensions.Configuration --version=8.0.0,7.0.0 --save-to=john.doe/downloads
```

### Check Package Dependencies

- `--check-depencencies` **Required** *(stand-alone switch)*: Identifier of package to check. Use the keyword `all` to check dependencies of all packages.
- `--package-id` *(Optional)*: Identifier of pecific package to check.
  
  Multiple package identifiers can be specified, separated by commas.
  
  If the package identifer is not specified, all packages in the local feed will be checked.
- `--version` *(Optional)*: Specific version of package to check.
  
  Multiple versions can be specified, separated by commas.
  
  If the version is not specified, all package versions will be checked.
- `--no-download` *(Optional - stand-alone switch)*: Do not download missing dependencies.
  
  If this switch is not specified, any missing dependencies will be downloaded from the Upstream NuGet Feed, if they are found

Example:

```bash
NuGetPuller --check-depencencies=System.Text.Json,Microsoft.Extensions.Hosting --no-download
```

### Export Local NuGet Feed Metadata

Create a metadata file that represnts the contents of the Local NuGet Feed.

- `--export-metadata` **Required:**: Path metadata file to create.

Example:

```bash
NuGetPuller --export-metadata=MyLocaRepo.json
```

### Create File Transfer Bundle

- `--create-bundle` **Required:**: Path of bundle file to create.
- If no extension is specified, this will use the `.zip` file extension.
- `--create-from` *(Optional)*: Path of a Local NuGet Feed metadata file.
  If this option is used, the bundle will not contain any packages that are represented in the nuget feed metadata file.
- `--save-metadata-to` *(Optional)*: Path of file to save the updated metadata to. If the `--create-from` was used, the metadata file will
  reflect the contents of the associated Local NuGet Feed after the bundle is added; otherwise, the metadata file will contain packages
  that were exported to the `.zip` file.

  It is okay if the `--create-from` and `--save-metadata-to` options refer to the same file.
- `--package-id` *(Optional)*: Comma-separated list of specific package IDs to include in the bundle. If this option is not specified, then all packages will be included in the bundle.
- `--version` *(Optional)*: Comma-separated list of specific versions to include in the bundle. If this option is not specified, then all packages will be included in the bundle.
  
```bash
NuGetPuller --create-bundle=Updates.zip --create-from=DisconnectedServer.nuget.metadata.json --save-metadata-to=DisconnectedServer.nuget.metadata.json
```

### Override Application Settings

The following optional command line arguments can be used to override [Applicaton Settings](../NuGetPuller/README.md#application-settings),
and can be used in combination with other command line arguments:

- `--local-feed-path`: Override the [LocalPath](../NuGetPuller/README.md#local-nuget-feed-path) setting.
  
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
