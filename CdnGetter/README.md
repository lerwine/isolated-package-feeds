# CdnGetter Project

## Content Retrieval Service Classes

For a class to function as middleware for a specifice remote CDN, it must inerit from [CdnGetter.Services.ContentGetterService](./Services/ContentGetterService.cs) and it must also have the
[CdnGetter.Services.ContentGetterAttribute](./Services/ContentGetterAttribute.cs) attribute, which specifies a unique GUID that identifies the corresponding [UpstreamCdn](./Model/UpstreamCdn.cs)
database entity and a name that describes the upstream CDN. The static `UpstreamCdnServices` property of that attribute contains all the upstream CDN service types that were found.
Each `ContentGetterService` implementation should have its own settings section under the main [Application Settings](./Config/AppSettings.cs)
where the upstream CDN URL and other CDN-specific configuration can be specified.

## Command Line Arguments

### Displaying information

#### Show CDNs

- `--Show=CDNs` - Show the upstream CDN names in the database.

#### Show Libraries

- `--Show=Libraries` - Show the upstream CDN names in the database.

Optional Parameter

- `--Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to show local libraries for.

#### Show Versions

- `--Show=Versions` - Show the upstream CDN names in the database.
- `--Library=[name]`*[,name,...]* - The library name(s) to show versions for.

Optional Parameter

- `--Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to show local libraries for.

#### Show Files

- `--Show=[Files]` - Show the upstream CDN file names in the database.
- `--Library=[name]`*[,name,...]* - The library name(s) to show files for.
- `--Version=[string]`*[,string,...]* - The library versions(s) to show files for.
- `--Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to show files for.

## Add Libraries

- `--AddLibrary=[name]`*[,name,...]* - The library name(s) on the upstream CDN to be added to the database.
- `--Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to retrieve libraries from.

Optional Switch

- `--Version=[string]`*[,string,...]* - The specific version(s) to add. If this is not specified, then all versions will be added.

## Get New Versions

- `--GetNewVersions=[name]`*[,name,...]* - The library name(s) on the upstream CDN to be added to the database.

Optional Switch

- `--Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to retrieve libraries from. If this is not specified, then new versions will be retrieved from all CDNs.

## Remove Libraries

- `--RemoveLibrary==[name]`*[,name,...]* - The library name(s) to remove from the database.</description>

Optional Switches

- `--Upstream=[name]`*[,name,...]* - The explicit upstream CDN name(s) to remove local libraries from. If this is not specified, then all matching libraries will be removed.
- `--Version=[string]`*[,string,...]* - The specific version(s) to remove. If this is not specified, then all versions of matching libraries will be removed.

## Reload Libraries

### Reload Libraries by CDN

- `--ReloadLibrary=[name]`*[,name,...]* - The library name(s) on the upstream CDN to be reloaded.
- `--Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to retrieve libraries from.

Optional Switch

- `--Version=[string]`*[,string,...]* - The specific version(s) to reload. If this is not specified, then all versions of matching libraries will be reloaded.

### Reload Libraries by Version

- `--ReloadLibrary=[name]`*[,name,...]* - The library name(s) to be reloaded.
- `--Version=[string]`*[,string,...]* - The specific version(s) to reload.

## Reload Existing Versions

### Reload Existing Versions by CDN

- `--ReloadExistingVersions=[name]`*[,name,...]* - The library name(s) on the upstream CDN to be reloaded.
- `--Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to retrieve libraries from.

Optional Switch

- `--Version=[string]`*[,string,...]* - The specific version(s) to reload. If this is not specified, then all versions of matching libraries will be reloaded.

### Reload Specific Existing Versions

- `--ReloadExistingVersions=[name]`*[,name,...]* - The library name(s) to be reloaded.
- `--Version=[string]`*[,string,...]* - The specific version(s) to reload.

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

#### Example Projects

- [NugetDownloader](https://github.com/paraspatidar/NugetDownloader)
- [NetCoreNugetServer](https://github.com/emresenturk/NetCoreNugetServer)
- [MinimalNugetServer](https://github.com/TanukiSharp/MinimalNugetServer)
