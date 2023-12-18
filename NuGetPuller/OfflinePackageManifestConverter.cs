using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NuGetPuller;

public class OfflinePackageManifestConverter : JsonConverter
{
    public const string LocalManfestFieldId = "id";

    private const string LocalManfestFieldHeadingId = $"        \"{LocalManfestFieldId}\": ";

    public const string LocalManfestFieldTitle = "title";

    private const string LocalManfestFieldHeadingTitle = $"        \"{LocalManfestFieldTitle}\": ";

    public const string LocalManfestFieldSummary = "summary";

    private const string LocalManfestFieldHeadingSummary = $"        \"{LocalManfestFieldSummary}\": ";

    public const string LocalManfestFieldDescription = "description";

    private const string LocalManfestFieldHeadingDescription = $"        \"{LocalManfestFieldDescription}\": ";

    public const string LocalManfestFieldVersions = "versions";

    private const string LocalManfestFieldHeadingVersions = $"        \"{LocalManfestFieldVersions}\": ";

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
        if (value is OfflinePackageManifest offlinePackageManifest)
        {
            var obj = new JObject
            {
                { LocalManfestFieldId, JToken.FromObject(offlinePackageManifest.Identifier) }
            };
            if (!string.IsNullOrWhiteSpace(offlinePackageManifest.Title))
                obj.Add(LocalManfestFieldTitle, JToken.FromObject(offlinePackageManifest.Title));
            if (!string.IsNullOrWhiteSpace(offlinePackageManifest.Summary))
                obj.Add(LocalManfestFieldSummary, JToken.FromObject(offlinePackageManifest.Summary));
            if (!string.IsNullOrWhiteSpace(offlinePackageManifest.Description))
                obj.Add(LocalManfestFieldDescription, JToken.FromObject(offlinePackageManifest.Description));
            obj.Add(LocalManfestFieldVersions, JToken.FromObject(offlinePackageManifest.Versions.Select(v => v.ToString()).ToArray()));
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
        string[]? versionStrings = jObject.Value<string[]>(LocalManfestFieldVersions);
        OfflinePackageManifest result;
        if (versionStrings is null)
            result = new(jObject.Value<string>(LocalManfestFieldId)!, []);
        else
        {
            var versions = new NuGetVersion[(versionStrings = versionStrings.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray()).Length];
            for (var i = 0; i < versionStrings.Length; i++)
                versions[i] = NuGetVersion.Parse(versionStrings[i]);
            result = new(jObject.Value<string>(LocalManfestFieldId)!, versions.Distinct<NuGetVersion>(VersionComparer.VersionReleaseMetadata).ToArray());
        }
        result.Title = jObject.Value<string>(LocalManfestFieldTitle);
        result.Summary = jObject.Value<string>(LocalManfestFieldSummary);
        result.Description = jObject.Value<string>(LocalManfestFieldDescription);
        return result;
    }

    public override bool CanConvert(Type objectType) => objectType == typeof(OfflinePackageManifest);
    public static async Task ExportLocalManifestAsync(IEnumerable<IPackageSearchMetadata> packages, StreamWriter writer, CancellationToken cancellationToken)
    {
        var pkgArr = packages.ToArray();
        if (pkgArr.Length > 0)
        {
            static async Task writeFields(IGrouping<string, IPackageSearchMetadata> group, StreamWriter writer)
            {
                var hasVersion = group.Where(p => p.Identity.HasVersion).OrderByDescending(p => p.Identity.Version, VersionComparer.VersionReleaseMetadata);
                var ordered = hasVersion.Concat(group.Where(p => !p.Identity.HasVersion));
                await writer.WriteAsync(LocalManfestFieldHeadingId);
                var precedingLine = JsonConvert.SerializeObject(group.Key, MetadataSerializationSettings);
                var text = ordered.Select(p => p.Title).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));
                if (text is not null)
                {
                    await writer.WriteAsync(precedingLine);
                    await writer.WriteLineAsync(',');
                    await writer.WriteAsync(LocalManfestFieldHeadingTitle);
                    precedingLine = JsonConvert.SerializeObject(text, MetadataSerializationSettings);
                }
                if ((text = ordered.Select(p => p.Summary).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t))) is not null)
                {
                    await writer.WriteAsync(precedingLine);
                    await writer.WriteLineAsync(',');
                    await writer.WriteAsync(LocalManfestFieldHeadingSummary);
                    precedingLine = JsonConvert.SerializeObject(text, MetadataSerializationSettings);
                }
                if ((text = ordered.Select(p => p.Description).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t))) is not null)
                {
                    await writer.WriteAsync(precedingLine);
                    await writer.WriteLineAsync(',');
                    await writer.WriteAsync(LocalManfestFieldHeadingDescription);
                    precedingLine = JsonConvert.SerializeObject(text, MetadataSerializationSettings);
                }
                var versions = hasVersion.Select(p => p.Identity.Version).ToArray();
                if (versions.Length > 0)
                {
                    await writer.WriteAsync(precedingLine);
                    await writer.WriteLineAsync(',');
                    await writer.WriteAsync(LocalManfestFieldHeadingVersions);
                    precedingLine = JsonConvert.SerializeObject(versions, MetadataSerializationSettings);
                }
                await writer.WriteLineAsync(precedingLine);
            }
            var grouped = pkgArr.GroupBy(p => p.Identity.Id, MainServiceStatic.PackageIdentityComparer).OrderBy(g => g.Key, MainServiceStatic.PackageIdentityComparer);
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
