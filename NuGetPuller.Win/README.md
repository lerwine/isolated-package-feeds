# NuGetPuller WIn

- [About NuGetPuller CLI](#about-nugetpuller-cli)
- [Links](#links)
- [Application Settings](#application-settings)

## About NuGetPuller CLI

The purpose of this application is to synchronize packages between an authoritative [NuGet](https://www.nuget.org/) repository and Offline NuGet Package Feeds.

An example use-case would be for a locally-hosted NuGet feed on a machine that does not have direct access to the internet. You can use this to download NuGet packages, including any dependencies, and then create a bundle of packages (a ZIP file) which can then be transferred to that disconnected host and imported into the Local NuGet Feed.

See [Key Concepts](../NuGetPuller/README.md#key-concepts) for a conceptual overview of terms used by this application.

## Links

- [Isolate Pacakge Feeds Home](../README.md)
- [NuGetPuller Class Library](../NuGetPuller/README.md) - Base class library for User Interface Applications.
- [NuGetPuller Class Library](../NuGetPuller.CLI/README.md) - Command line interface.
- [NuGetPuller Unit Tests](../NuGetPuller.UnitTests/README.md)

## Application Settings

See [NuGet Common Class Library Application Settings](../NuGetPuller/README.md#application-settings) for the  configuration options for the `NuGetPuller` section of [appsettings.json](./appsettings.json).
