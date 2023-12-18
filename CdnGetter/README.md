# CdnGetter Application

- [About CdnGetter](#about-cdngetter)
- [Links](#links)
- [Command Line Options](#command-line-options)
  - [Show CDNs](#show-cdns)
  - [Show Libraries](#show-libraries)
  - [Show Versions](#show-versions)
  - [Show Files](#show-files)
  - [Add Libraries](#add-libraries)
  - [Get New Versions](#get-new-versions)
  - [Remove Libraries](#remove-libraries)
  - [Reload Libraries by CDN](#reload-libraries-by-cdn)
  - [Reload Libraries by Version](#reload-libraries-by-version)
  - [Reload Existing Versions by CDN](#reload-existing-versions-by-cdn)
  - [Reload Specific Existing Versions](#reload-specific-existing-versions)
- [Content Retrieval Service Classes](#content-retrieval-service-classes)
- [References](#references)

## About CdnGetter

The purpose of this application is to retrieve content from internet CDNs to be used by the [CDN Server Web Application](../CdnServer/README.md).

## Links

- [Isolate Pacakge Feeds Home](../README.md)
- [CdnGetter Unit Tests](../CdnGetter.UnitTests/README.md)
- [CDN Server Web Application](../CdnServer/README.md)

## Command Line Options

### Show CDNs

- `--Show=CDNs` - Show the upstream CDN names in the database.

### Show Libraries

- `--Show=Libraries` - Show the upstream CDN names in the database.

Optional Parameter

- `--Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to show local libraries for.

### Show Versions

- `--Show=Versions` - Show the upstream CDN names in the database.
- `--Library=[name]`*[,name,...]* - The library name(s) to show versions for.

Optional Parameter

- `--Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to show local libraries for.

### Show Files

- `--Show=[Files]` - Show the upstream CDN file names in the database.
- `--Library=[name]`*[,name,...]* - The library name(s) to show files for.
- `--Version=[string]`*[,string,...]* - The library versions(s) to show files for.
- `--Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to show files for.

### Add Libraries

- `--AddLibrary=[name]`*[,name,...]* - The library name(s) on the upstream CDN to be added to the database.
- `--Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to retrieve libraries from.

Optional Switch

- `--Version=[string]`*[,string,...]* - The specific version(s) to add. If this is not specified, then all versions will be added.

### Get New Versions

- `--GetNewVersions=[name]`*[,name,...]* - The library name(s) on the upstream CDN to be added to the database.

Optional Switch

- `--Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to retrieve libraries from. If this is not specified, then new versions will be retrieved from all CDNs.

### Remove Libraries

- `--RemoveLibrary==[name]`*[,name,...]* - The library name(s) to remove from the database.</description>

Optional Switches

- `--Upstream=[name]`*[,name,...]* - The explicit upstream CDN name(s) to remove local libraries from. If this is not specified, then all matching libraries will be removed.
- `--Version=[string]`*[,string,...]* - The specific version(s) to remove. If this is not specified, then all versions of matching libraries will be removed.

### Reload Libraries by CDN

- `--ReloadLibrary=[name]`*[,name,...]* - The library name(s) on the upstream CDN to be reloaded.
- `--Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to retrieve libraries from.

Optional Switch

- `--Version=[string]`*[,string,...]* - The specific version(s) to reload. If this is not specified, then all versions of matching libraries will be reloaded.

### Reload Libraries by Version

- `--ReloadLibrary=[name]`*[,name,...]* - The library name(s) to be reloaded.
- `--Version=[string]`*[,string,...]* - The specific version(s) to reload.

### Reload Existing Versions by CDN

- `--ReloadExistingVersions=[name]`*[,name,...]* - The library name(s) on the upstream CDN to be reloaded.
- `--Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to retrieve libraries from.

Optional Switch

- `--Version=[string]`*[,string,...]* - The specific version(s) to reload. If this is not specified, then all versions of matching libraries will be reloaded.

### Reload Specific Existing Versions

- `--ReloadExistingVersions=[name]`*[,name,...]* - The library name(s) to be reloaded.
- `--Version=[string]`*[,string,...]* - The specific version(s) to reload.

## Content Retrieval Service Classes

For a class to function as middleware for a specifice remote CDN, it must inerit from [CdnGetter.Services.ContentGetterService](./Services/ContentGetterService.cs) and it must also have the
[CdnGetter.Services.ContentGetterAttribute](./Services/ContentGetterAttribute.cs) attribute, which specifies a unique GUID that identifies the corresponding [UpstreamCdn](./Model/UpstreamCdn.cs)
database entity and a name that describes the upstream CDN. The static `UpstreamCdnServices` property of that attribute contains all the upstream CDN service types that were found.
Each `ContentGetterService` implementation should have its own settings section under the main [Application Settings](./Config/AppSettings.cs)
where the upstream CDN URL and other CDN-specific configuration can be specified.

## References

- [cdnjs](https://cdnjs.com/)
  - [API Documentation](https://cdnjs.com/api)
  - [Main Git Repository](https://github.com/lerwine/cdnjs)
    - [Vue/Nuxt website for cdnjs.com](https://github.com/lerwine/static-website)
    - [API server for api.cdnjs.com](https://github.com/lerwine/api-server)
    - [Package configurations](https://github.com/lerwine/packages)
    - [Brand and design assets for cdnjs](https://github.com/lerwine/brand)
- [jsdelivr](https://www.jsdelivr.com/)
  - [API Documentation](https://www.jsdelivr.com/docs/data.jsdelivr.com#overview)
