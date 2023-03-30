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

- `--CdnGetter:Show=CDNs` - Show the upstream CDN names in the database.

#### Show Libraries

- `--CdnGetter:Show=Libraries` - Show the upstream CDN names in the database.

Optional Parameter

- `--CdnGetter:Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to show local libraries for.

#### Show Versions

- `--CdnGetter:Show=Versions` - Show the upstream CDN names in the database.
- `--CdnGetter:Library=[name]`*[,name,...]* - The library name(s) to show versions for.

Optional Parameter

- `--CdnGetter:Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to show local libraries for.

#### Show Files

- `--CdnGetter:Show=[Files]` - Show the upstream CDN file names in the database.
- `--CdnGetter:Library=[name]`*[,name,...]* - The library name(s) to show files for.
- `--CdnGetter:Version=[string]`*[,string,...]* - The library versions(s) to show files for.
- `--CdnGetter:Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to show files for.

## Add Libraries

- `--CdnGetter:AddLibrary=[name]`*[,name,...]* - The library name(s) on the upstream CDN to be added to the database.
- `--CdnGetter:Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to retrieve libraries from.

Optional Switch

- `--CdnGetter:Version=[string]`*[,string,...]* - The specific version(s) to add. If this is not specified, then all versions will be added.

## Get New Versions

- `--CdnGetter:GetNewVersions=[name]`*[,name,...]* - The library name(s) on the upstream CDN to be added to the database.

Optional Switch

- `--CdnGetter:Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to retrieve libraries from. If this is not specified, then new versions will be retrieved from all CDNs.

## Remove Libraries

- `--CdnGetter:RemoveLibrary==[name]`*[,name,...]* - The library name(s) to remove from the database.</description>

Optional Switches

- `--CdnGetter:Upstream=[name]`*[,name,...]* - The explicit upstream CDN name(s) to remove local libraries from. If this is not specified, then all matching libraries will be removed.
- `--CdnGetter:Version=[string]`*[,string,...]* - The specific version(s) to remove. If this is not specified, then all versions of matching libraries will be removed.

## Reload Libraries

### Reload Libraries by CDN

- `--CdnGetter:ReloadLibrary=[name]`*[,name,...]* - The library name(s) on the upstream CDN to be reloaded.
- `--CdnGetter:Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to retrieve libraries from.

Optional Switch

- `--CdnGetter:Version=[string]`*[,string,...]* - The specific version(s) to reload. If this is not specified, then all versions of matching libraries will be reloaded.

### Reload Libraries by Version

- `--CdnGetter:ReloadLibrary=[name]`*[,name,...]* - The library name(s) to be reloaded.
- `--CdnGetter:Version=[string]`*[,string,...]* - The specific version(s) to reload.

## Reload Existing Versions

### Reload Existing Versions by CDN

- `--CdnGetter:ReloadExistingVersions=[name]`*[,name,...]* - The library name(s) on the upstream CDN to be reloaded.
- `--CdnGetter:Upstream=[name]`*[,name,...]* - The upstream CDN name(s) to retrieve libraries from.

Optional Switch

- `--CdnGetter:Version=[string]`*[,string,...]* - The specific version(s) to reload. If this is not specified, then all versions of matching libraries will be reloaded.

### Reload Specific Existing Versions

- `--CdnGetter:ReloadExistingVersions=[name]`*[,name,...]* - The library name(s) to be reloaded.
- `--CdnGetter:Version=[string]`*[,string,...]* - The specific version(s) to reload.
