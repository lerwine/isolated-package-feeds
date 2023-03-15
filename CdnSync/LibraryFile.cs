using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CdnSync;

public class LibraryFile
{
    private readonly object _syncRoot = new();

    public Guid Id { get; set; }
    
    private string _name = "";
    public string Name
    {
        get => _name;
        set => _name = value.ToWsNormalizedOrEmptyIfNull();
    }

    public ushort Order { get; set; }

    private string _contentType = System.Net.Mime.MediaTypeNames.Application.Octet;
    public string ContentType
    {
        get => _contentType;
        set => _contentType = value.ToTrimmedOrDefaultIfEmpty(() => System.Net.Mime.MediaTypeNames.Application.Octet);
    }

    private string? _encoding;
    public string? Encoding
    {
        get => _encoding;
        set => _encoding = value.ToTrimmedOrNullIfEmpty();
    }

    private byte[] _data = Array.Empty<byte>();
    public byte[] Data
    {
        get => _data;
        set => _data = value ?? Array.Empty<byte>();
    }

    public JsonNode? ProviderData { get; set; }

    private string? ProviderJson
    {
        get => ProviderData?.ToJsonString();
        set
        {
            string? text = value.ToTrimmedOrNullIfEmpty();
            if (text is null)
                ProviderData = null;
            else
                try { ProviderData = JsonNode.Parse(text); }
                catch { ProviderData = JsonValue.Create(text); }
        }
    }
    
    private Guid _versionId;
    public Guid VersionId
    {
        get => _versionId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _versionId, ref _version);
    }

    private LibraryVersion? _version;
    public LibraryVersion? Version
    {
        get => _version;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _versionId, ref _version);
    }

    internal static void OnBuildEntity(EntityTypeBuilder<LibraryFile> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(c => c.Name).IsRequired().UseCollation("SQL_Latin1_General_CP1_CI_AS");
        _ = builder.Property(c => c.Order).IsRequired();
        _ = builder.Property(c => c.Data).IsRequired();
        _ = builder.Property(c => c.ContentType).IsRequired();
        _ = builder.Property(c => c.ProviderJson);
        _ = builder.Ignore(c => c.ProviderData);
        _ = builder.HasIndex(nameof(Order), nameof(VersionId));
        _ = builder.HasIndex(nameof(Name), nameof(VersionId));
        _ = builder.HasOne(l => l.Version).WithMany(p => p.Files).HasForeignKey(l => l.VersionId).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
    }

    internal static async Task<bool> DeleteAsync(CdnSyncDb dbContext, Guid versionId, CancellationToken stoppingToken)
    {
        LibraryFile[] toDelete = await dbContext.Files.Where(f => f.VersionId == versionId).ToArrayAsync(stoppingToken);
        if (toDelete.Length == 0)
            return false;
        dbContext.Files.RemoveRange(toDelete);
        await dbContext.SaveChangesAsync(true, stoppingToken);
        return true;
    }
}
