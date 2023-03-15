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
        get { return _name; }
        set { _name = value.ToWsNormalizedOrEmptyIfNull(); }
    }
    
    public ushort Order { get; set; }

    private string _contentType = System.Net.Mime.MediaTypeNames.Application.Octet;
    public string ContentType
    {
        get { return _contentType; }
        set { _contentType = value.ToTrimmedOrDefaultIfEmpty(() => System.Net.Mime.MediaTypeNames.Application.Octet); }
    }
    
    private string? _encoding;
    public string? Encoding
    {
        get { return _encoding; }
        set { _encoding = value.ToTrimmedOrNullIfEmpty(); }
    }
    
    private byte[] _data = Array.Empty<byte>();
    public byte[] Data
    {
        get { return _data; }
        set { _data = value ?? Array.Empty<byte>(); }
    }

    private JsonObject? _providerData;
    // BUG: JsonObject can't be serialized to DB
    public JsonObject? ProviderData
    {
        get { return _providerData; }
        set { _providerData = value; }
    }
    
    private Guid _versionId;
    public Guid VersionId
    {
        get { return _versionId; }
        set { value.SetNavigation(_syncRoot, p => p.Id, ref _versionId, ref _version); }
    }
    
    private LibraryVersion? _version;
    public LibraryVersion? Version
    {
        get { return _version; }
        set { value.SetNavigation(_syncRoot, p => p.Id, ref _versionId, ref _version); }
    }
    
    internal static void OnBuildEntity(EntityTypeBuilder<LibraryFile> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(c => c.Name).UseCollation("SQL_Latin1_General_CP1_CI_AS");
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
