using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CdnGet.Model;

/// <summary>
/// Represents an individual file in a specific version of a content library.
/// </summary>
public class LibraryFile
{
    private readonly object _syncRoot = new();

    private Guid? _id;
    /// <summary>
    /// The unique identifier for the library file.
    /// </summary>
    public Guid Id
    {
        get => _id.EnsureGuid(_syncRoot);
        set => _id = value;
    }
    
    private string _name = "";
    /// <summary>
    /// The name of the library file.
    /// </summary>
    public string Name
    {
        get => _name;
        set => _name = value.ToWsNormalizedOrEmptyIfNull();
    }

    /// <summary>
    /// The display order for the library file.
    /// </summary>
    public ushort Order { get; set; }

    private string _contentType = System.Net.Mime.MediaTypeNames.Application.Octet;
    /// <summary>
    /// The library file MIME content type.
    /// </summary>
    public string ContentType
    {
        get => _contentType;
        set => _contentType = value.ToTrimmedOrDefaultIfEmpty(() => System.Net.Mime.MediaTypeNames.Application.Octet);
    }

    private string? _encoding;
    /// <summary>
    /// The content encoding for library file or <cref langword="null" /> for binary files.
    /// </summary>
    public string? Encoding
    {
        get => _encoding;
        set => _encoding = value.ToTrimmedOrNullIfEmpty();
    }

    private byte[] _data = Array.Empty<byte>();
    /// <summary>
    /// The contents of the library file.
    /// </summary>
    public byte[] Data
    {
        get => _data;
        set => _data = value ?? Array.Empty<byte>();
    }

    /// <summary>
    /// Optional provider-specific data for <see cref="RemoteService" />.
    /// </summary>
    public JsonNode? ProviderData { get; set; }

    private DateTime? _createdOn;
    /// <summary>
    /// The date and time that the record was created.
    /// </summary>
    public DateTime CreatedOn
    {
        get => _createdOn.EnsureCreatedOn(ref _modifiedOn, ref _lastChecked, _syncRoot);
        set => value.SetCreatedOn(ref _createdOn, ref _modifiedOn, ref _lastChecked, _syncRoot);
    }

    private DateTime? _modifiedOn;
    /// <summary>
    /// The date and time that the record was last modified.
    /// </summary>
    public DateTime ModifiedOn
    {
        get => _modifiedOn.EnsureModifiedOn(ref _createdOn, ref _lastChecked, _syncRoot);
        set => value.SetModifiedOn(ref _createdOn, ref _modifiedOn, ref _lastChecked, _syncRoot);
    }

    private DateTime? _lastChecked;
    /// <summary>
    /// The date and time when the library file was last checked for changes.
    /// </summary>
    public DateTime LastChecked
    {
        get => _lastChecked.EnsureLastChecked(ref _createdOn, ref _modifiedOn, _syncRoot);
        set => value.SetLastChecked(ref _createdOn, ref _modifiedOn, ref _lastChecked, _syncRoot);
    }

    private Guid _versionId;
    /// <summary>
    /// The unique identifier of the parent <see cref="LibraryVersion" />.
    /// </summary>
    public Guid VersionId
    {
        get => _versionId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _versionId, ref _version);
    }

    private LibraryVersion? _version;
    /// <summary>
    /// The content library version that the current file belongs to.
    /// </summary>
    public LibraryVersion? Version
    {
        get => _version;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _versionId, ref _version);
    }
    
    /// <summary>
    /// Performs configuration of the <see cref="LibraryFile" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<LibraryFile> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(f => f.Id)
            .UseCollation("NOCASE");
        _ = builder.HasIndex(nameof(Name));
        _ = builder.HasIndex(nameof(Name), nameof(VersionId)).IsUnique();
        _ = builder.Property(f => f.Name)
            .IsRequired()
            .UseCollation("NOCASE");
        _ = builder.Property(c => c.Order).IsRequired();
        _ = builder.Property(c => c.Data).IsRequired();
        _ = builder.Property(c => c.ContentType).IsRequired();
        _ = builder.HasIndex(nameof(Order));
        _ = builder.HasIndex(nameof(Order), nameof(VersionId)).IsUnique();
        _ = builder.Property(f => f.ProviderData).HasConversion(ExtensionMethods.JsonValueConverter);
        _ = builder.HasOne(f => f.Version).WithMany(v => v.Files).HasForeignKey(f => f.VersionId).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
        _ = builder.Property(f => f.VersionId)
            .UseCollation("NOCASE");
    }
}