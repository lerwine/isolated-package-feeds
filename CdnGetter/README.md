# CdnGetter Project

## Database Entities

## Content Retrieval Service Classes

Classes that are used for retrieving data from a specifc upstream CDN are derived from the [ContentGetterService](./Services/ContentGetterService.cs) class and
also have the [ContentGetter](./Services/ContentGetterAttribute.cs) attribute, which specifies a unique GUID that identifies the corresponding [UpstreamCdn](./Model/UpstreamCdn.cs)
database entity and a name that describes the upstream CDN. Each `ContentGetterService` implementation should have its own settings section under the main [Application Settings](./Config/AppSettings.cs)
where the upstream CDN URL and other CDN-specific configuration can be specified.
