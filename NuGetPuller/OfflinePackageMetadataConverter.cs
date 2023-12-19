using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static NuGetPuller.NuGetPullerStatic;

namespace NuGetPuller;

public class OfflinePackageMetadataConverter : JsonConverter
{
    public const string OfflinePkgMetadataFieldId = "id";

    private const string OfflinePkgMetadataFieldHeadingId = $"        \"{OfflinePkgMetadataFieldId}\": ";

    public const string OfflinePkgMetadataFieldTitle = "title";

    private const string OfflinePkgMetadataFieldHeadingTitle = $"        \"{OfflinePkgMetadataFieldTitle}\": ";

    public const string OfflinePkgMetadataFieldSummary = "summary";

    private const string OfflinePkgMetadataFieldHeadingSummary = $"        \"{OfflinePkgMetadataFieldSummary}\": ";

    public const string OfflinePkgMetadataFieldDescription = "description";

    private const string OfflinePkgMetadataFieldHeadingDescription = $"        \"{OfflinePkgMetadataFieldDescription}\": ";

    public const string OfflinePkgMetadataFieldVersions = "versions";

    private const string OfflinePkgMetadataFieldHeadingVersions = $"        \"{OfflinePkgMetadataFieldVersions}\": ";

    private static readonly JsonSerializerSettings MetadataSerializationSettings = new()
    {
        MaxDepth = 512,
        NullValueHandling = NullValueHandling.Ignore,
        TypeNameHandling = TypeNameHandling.None,
        Converters = [new NuGetVersionConverter()],
        Formatting = Formatting.None
    };

    private static readonly JsonLoadSettings MetadataLoadSettings = new()
    {
        LineInfoHandling = LineInfoHandling.Ignore,
        CommentHandling = CommentHandling.Ignore
    };

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is OfflinePackageMetadata offlineMetadata)
        {
            var obj = new JObject
            {
                { OfflinePkgMetadataFieldId, JToken.FromObject(offlineMetadata.Identifier) }
            };
            if (!string.IsNullOrWhiteSpace(offlineMetadata.Title))
                obj.Add(OfflinePkgMetadataFieldTitle, JToken.FromObject(offlineMetadata.Title));
            if (!string.IsNullOrWhiteSpace(offlineMetadata.Summary))
                obj.Add(OfflinePkgMetadataFieldSummary, JToken.FromObject(offlineMetadata.Summary));
            if (!string.IsNullOrWhiteSpace(offlineMetadata.Description))
                obj.Add(OfflinePkgMetadataFieldDescription, JToken.FromObject(offlineMetadata.Description));
            obj.Add(OfflinePkgMetadataFieldVersions, JToken.FromObject(offlineMetadata.Versions.Select(v => v.ToString()).ToArray()));
            serializer.Serialize(writer, obj);
        }
        else
            serializer.Serialize(writer, null);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;
        JObject jObject = JObject.Load(reader, MetadataLoadSettings);
        string[]? versionStrings = jObject.Value<string[]>(OfflinePkgMetadataFieldVersions);
        OfflinePackageMetadata result;
        if (versionStrings is null)
            result = new(jObject.Value<string>(OfflinePkgMetadataFieldId)!, []);
        else
        {
            var versions = new NuGetVersion[(versionStrings = versionStrings.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray()).Length];
            for (var i = 0; i < versionStrings.Length; i++)
                versions[i] = NuGetVersion.Parse(versionStrings[i]);
            result = new(jObject.Value<string>(OfflinePkgMetadataFieldId)!, versions.Distinct<NuGetVersion>(VersionComparer.VersionReleaseMetadata).ToArray());
        }
        result.Title = jObject.Value<string>(OfflinePkgMetadataFieldTitle);
        result.Summary = jObject.Value<string>(OfflinePkgMetadataFieldSummary);
        result.Description = jObject.Value<string>(OfflinePkgMetadataFieldDescription);
        return result;
    }

    public override bool CanConvert(Type objectType) => objectType == typeof(OfflinePackageMetadata);
    public static async Task ExportDownloadedPackageManifestAsync(IEnumerable<IPackageSearchMetadata> packages, StreamWriter writer, CancellationToken cancellationToken)
    {
        var pkgArr = packages.ToArray();
        if (pkgArr.Length > 0)
        {
            static async Task writeFields(IGrouping<string, IPackageSearchMetadata> group, StreamWriter writer)
            {
                var hasVersion = group.Where(p => p.Identity.HasVersion).OrderByDescending(p => p.Identity.Version, VersionComparer.VersionReleaseMetadata);
                var ordered = hasVersion.Concat(group.Where(p => !p.Identity.HasVersion));
                await writer.WriteAsync(OfflinePkgMetadataFieldHeadingId);
                var precedingLine = JsonConvert.SerializeObject(group.Key, MetadataSerializationSettings);
                var text = ordered.Select(p => p.Title).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));
                if (text is not null)
                {
                    await writer.WriteAsync(precedingLine);
                    await writer.WriteLineAsync(',');
                    await writer.WriteAsync(OfflinePkgMetadataFieldHeadingTitle);
                    precedingLine = JsonConvert.SerializeObject(text, MetadataSerializationSettings);
                }
                if ((text = ordered.Select(p => p.Summary).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t))) is not null)
                {
                    await writer.WriteAsync(precedingLine);
                    await writer.WriteLineAsync(',');
                    await writer.WriteAsync(OfflinePkgMetadataFieldHeadingSummary);
                    precedingLine = JsonConvert.SerializeObject(text, MetadataSerializationSettings);
                }
                if ((text = ordered.Select(p => p.Description).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t))) is not null)
                {
                    await writer.WriteAsync(precedingLine);
                    await writer.WriteLineAsync(',');
                    await writer.WriteAsync(OfflinePkgMetadataFieldHeadingDescription);
                    precedingLine = JsonConvert.SerializeObject(text, MetadataSerializationSettings);
                }
                var versions = hasVersion.Select(p => p.Identity.Version).ToArray();
                if (versions.Length > 0)
                {
                    await writer.WriteAsync(precedingLine);
                    await writer.WriteLineAsync(',');
                    await writer.WriteAsync(OfflinePkgMetadataFieldHeadingVersions);
                    precedingLine = JsonConvert.SerializeObject(versions, MetadataSerializationSettings);
                }
                await writer.WriteLineAsync(precedingLine);
            }
            var grouped = pkgArr.GroupBy(p => p.Identity.Id, PackageIdentitifierComparer).OrderBy(g => g.Key, PackageIdentitifierComparer);
            await writer.WriteLineAsync('[');
            foreach (var group in grouped.SkipLast(1))
            {
                cancellationToken.ThrowIfCancellationRequested();
                await writer.WriteLineAsync("    {");
                await writeFields(group, writer);
                await writer.WriteLineAsync("    },");
            }
            cancellationToken.ThrowIfCancellationRequested();
            await writer.WriteLineAsync("    {");
            await writeFields(grouped.Last(), writer);
            await writer.WriteLineAsync("    }");
            await writer.WriteLineAsync(']');
        }
        else
            await writer.WriteLineAsync("[]");
    }
}
