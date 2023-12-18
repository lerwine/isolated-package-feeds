using IsolatedPackageFeeds.Shared;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGetPuller;

public class OfflinePackageManifest
{
    public string Identifier { get; set; }

    public NuGetVersion[] Versions { get; set; }

    public string? Title { get; set; }

    public string? Summary { get; set; }

    public string? Description { get; set; }

    public OfflinePackageManifest() => (Identifier, Versions) = (string.Empty, []);

    public OfflinePackageManifest(string identifer, NuGetVersion[] versions) => (Identifier, Versions) = (identifer, versions);

    public async static Task<OfflinePackageManifest> CreateAsync(IPackageSearchMetadata pkg)
    {
        OfflinePackageManifest result = new(pkg.Identity.Id, (await pkg.GetVersionsAsync())?.Select(v => v.Version).ToArray() ?? []);
        if (pkg.Title.TryGetNonWhitesSpace(out string? text))
            result.Title = text;
        if (pkg.Summary.TryGetNonWhitesSpace(out text))
            result.Summary = text;
        if (pkg.Description.TryGetNonWhitesSpace(out text))
            result.Description = text;
        return result;
    }
}
