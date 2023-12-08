# NuGetPuller CLI

The purpose of this application is to synchronize a local NuGet repository with an upstream NuGet repository as well as other locally-hosted NuGet repositories.

An example use-case would be for a locally-hosted NuGet repository on a machine that does not have direct access to the internet. You can use this to download NuGet packages, including any dependencies, and then create a bundle of packages (a ZIP file) which can then be transferred to that disconnected host and imported into the local NuGet repository.

This references package manifest files to represent which packages already exist in other locally-hosted NuGet repositorys. This minimizes bundle sizes, because it doesn't have to include packages that already exist in the target NuGet repository.

## Application Settings

See [NuGet Common Class Library Application Settings](../NuGetPuller/README.md#application-settings) for the  configuration options for the `NuGetPuller` section of [appsettings.json](./appsettings.json).

## Command Line Arguments

### Get Latest Updates From Upstream NuGet Repository

- `--update-all` - **Required:** *This command line switch does not have any associated value.*

*Example:* `NuGetPuller.CLI --update-all`

### Update Specific Packages From Upstream NuGet Repository

- `-u` - **Required:** Identifier(s) of packages to be updated.
  - Multiple package identifiers are separated by commas.

*Example:* `NuGetPuller.CLI -u Microsoft.Extensions.Configuration.Abstractions,Microsoft.Extensions.DependencyInjection.Abstractions`

### Add New Packages From Upstream NuGet Repository

- `-a` - **Required:** Identifier(s) of packages to be downloaded and added to the local repository.
  - Multiple package identifiers are separated by commas.

*Example:* `NuGetPuller.CLI -a System.Text.Json,Microsoft.CodeAnalysis.CSharp.Workspaces`

### Import Package Files Into Local NuGet Repository

- `-i` - **Required:** Import package file(s) into the local NuGet repository.
  - This can either refer to a single `.nupkg` file or to a subdirectory containing `.nupkg` files.
  - If this refers to a subdirectory, it will not recursively search nested sub-directories.

*Example #1:* `NuGetPuller.CLI -i C:\users\john.doe\Downloads\microsoft.extensions.logging.8.0.0.nupkg`

*Example #2:* `NuGetPuller.CLI -i C:\users\john.doe\Downloads\MyPackageFolder`

### Delete Packages From Local NuGet Repository

- `-d` - **Required:** Identifier(s) of packages to be removed from the local repository.
  - Multiple package identifiers are separated by commas.

*Example:* `NuGetPuller.CLI -d PackageA,PackageB`

### List Packages Stored In Local NuGet Repository

- `-l` - **Required:** *This command line switch does not have any associated value.*

*Example:* `NuGetPuller.CLI -l`

### Create File Transfer Bundle

- `-b` - **Required:** Path of bundle file to create.
  - If no extension is specified, this will use the `.zip` file extension.
  
  *Example:* `NuGetPuller.CLI -b MyAirgapped_2023-12-06.zip`
- `-t` - *Optional:* Path of the package manifest file for the locally-hosted NuGet repository that this bundle is being created for.
  - If this is not specified, then this will look for a `.json` file with the same base file name and location as the path specified by the `-b` argument.
  - If the package manifest file for the target NuGet repository does not exist, one it will be created.
  
  *Example:* `NuGetPuller.CLI -b MyAirgapped_2023-12-06.zip -t MyAirgappedFeed.json`
- `--save-target-manifest-as` - *Optional:* Alternate path to save updated package manifest file to.
  - If this is not specified, then the package manifest file for the target NuGet repository will be updated to include the packages that are contained in the bundle file specified by the `-b` argument.
  
  *Example:* `NuGetPuller.CLI -b MyAirgapped_2023-12-06.zip -t MyAirgappedFeed.json --save-target-manifest-as=MyAirgappedFeed-New.json`

### Create Package Fanifest File From Local NuGet Repository

- `--export-local-manifest` - **Required:** Path of `.json` file to create which will be a manifest of all packages in the local NuGet repository.
  
  *Example:* `NuGetPuller.CLI --export-local-manifest=MyLocalRepoManifest.json`

### Override Application Settings

The following optional command line arguments can be used to override [Applicaton Settings](../NuGetPuller/README.md#application-settings),
and can be used in combination with other command line arguments:

- `--local-repository` - Override the [LocalPath](../NuGetPuller/README.md#local-nuget-repository-path) setting.
  
  *Example:* `NuGetPuller.CLI --local-repository=C:\users\john.doe\Documents\MyLocalRepository -i mycustom.nupkg`
- `--upstream-service-index` - Override the [UpstreamServiceIndex](../NuGetPuller/README.md#upstream-service-index-url) setting.
  
  *Example:* `NuGetPuller.CLI --upstream-service-index=file://myserver/myshare --update-all`
- `--global-packages-folder` - Override the [GlobalPackagesFolder](../NuGetPuller/README.md#global-packages-folder-path) setting.
  
  *Example:* `NuGetPuller.CLI --global-packages-folder=C:\users\john.doe\Downloads\MyNuGetGpf -u Microsoft.EntityFrameworkCore`
