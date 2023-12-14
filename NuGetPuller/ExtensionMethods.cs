using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using IsolatedPackageFeeds.Shared;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace NuGetPuller;

public static class ExtensionMethods
{
    public static bool TryGetNuGetVersionList(this string? value, [NotNullWhen(true)] out NuGetVersion[]? result)
    {
        if (value is not null && (value = value.Trim()).Length > 0)
        {
            if (value.Contains(','))
            {
                string[] arr = value.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray(); 
                var len = arr.Length;
                if (len > 0)
                {
                    result = new NuGetVersion[len];
                    for (var i = 0; i < len; i++)
                    {
                        if (!NuGetVersion.TryParse(value, out NuGetVersion? version))
                            return false;
                        result[i] = version;
                    }
                    return true;
                }
            }
            else if (NuGetVersion.TryParse(value, out NuGetVersion? version))
            {
                result = [version];
                return true;
            }
        }
        result = null;
        return false;
    }
    
    public static string GetUpstreamServiceIndex(this ISharedAppSettings settings) => settings.OverrideUpstreamServiceIndex.DefaultIfWhiteSpace(settings.UpstreamServiceIndex);

    public static string GetLocalRepository(this ISharedAppSettings settings) => settings.OverrideLocalRepository.DefaultIfWhiteSpace(settings.LocalRepository);

    public static string GetGlobalPackagesFolder(this ISharedAppSettings settings) => settings.OverrideGlobalPackagesFolder.DefaultIfWhiteSpace(settings.GlobalPackagesFolder);
}
