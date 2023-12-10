using IsolatedPackageFeeds.Shared;

namespace NuGetPuller;

public static class ExtensionMethods
{
    public static string GetUpstreamServiceIndex(this ISharedAppSettings settings) => settings.OverrideUpstreamServiceIndex.DefaultIfWhiteSpace(settings.UpstreamServiceIndex);

    public static string GetLocalRepository(this ISharedAppSettings settings) => settings.OverrideLocalRepository.DefaultIfWhiteSpace(settings.LocalRepository);

    public static string GetGlobalPackagesFolder(this ISharedAppSettings settings) => settings.OverrideGlobalPackagesFolder.DefaultIfWhiteSpace(settings.GlobalPackagesFolder);
}
