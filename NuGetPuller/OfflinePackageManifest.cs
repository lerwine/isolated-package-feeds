using IsolatedPackageFeeds.Shared;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using static NuGetPuller.MainServiceStatic;

namespace NuGetPuller;

public class OfflinePackageManifest : IComparable<OfflinePackageManifest>, IEquatable<OfflinePackageManifest>, IEquatable<PackageIdentity>, IEquatable<string>
{
    public string Identifier { get; set; }

    public List<NuGetVersion> Versions { get; set; }

    public string? Title { get; set; }

    public string? Summary { get; set; }

    public string? Description { get; set; }

    public OfflinePackageManifest() => (Identifier, Versions) = (string.Empty, []);

    public OfflinePackageManifest(IPackageSearchMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        Identifier = metadata.Identity.Id;
        Versions = metadata.Identity.HasVersion ? [metadata.Identity.Version] : [];
        if (metadata.Title.TryGetNonWhitesSpace(out string? text))
            Title = text;
        if (metadata.Summary.TryGetNonWhitesSpace(out text))
            Summary = text;
        if (metadata.Description.TryGetNonWhitesSpace(out text))
            Description = text;
    }

    internal OfflinePackageManifest(string identifer, params NuGetVersion[] versions) => (Identifier, Versions) = (identifer, versions?.ToList() ?? []);

    public int CompareTo(OfflinePackageManifest? other) => (other is null) ? 1 : ReferenceEquals(this, other) ? 0 : PackageIdentitifierComparer.Compare(Identifier, other.Identifier);

    public bool Equals(OfflinePackageManifest? other) => other is not null && (ReferenceEquals(this, other) || PackageIdentitifierComparer.Equals(Identifier, other.Identifier));

    public bool Equals(PackageIdentity? other) => other is not null && PackageIdentitifierComparer.Equals(Identifier, other.Id) && (!other.HasVersion || Versions.Contains(other.Version, VersionComparer.VersionReleaseMetadata));

    public bool Equals(string id, NuGetVersion version) => id is not null && PackageIdentitifierComparer.Equals(Identifier, id) && (version is null || Versions.Contains(version, VersionComparer.VersionReleaseMetadata));

    public bool Equals(string? id) => id is not null && PackageIdentitifierComparer.Equals(Identifier, id);

    public override bool Equals(object? obj) => obj is OfflinePackageManifest other && (ReferenceEquals(this, other) || PackageIdentitifierComparer.Equals(Identifier, other.Identifier));

    public override int GetHashCode() => PackageIdentitifierComparer.GetHashCode(Identifier);

    public override string ToString() => this.ToJson(Newtonsoft.Json.Formatting.None);
}
