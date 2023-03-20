using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CdnGet.Model;

/// <summary>
/// Represents an individual file in a specific version of a content library.
/// </summary>
public class RemoteFile
{
    private readonly object _syncRoot = new();

    private Guid _localId;
    /// <summary>
    /// The unique identifier of the parent <see cref="LocalFile" />.
    /// </summary>
    public Guid LocalId
    {
        get => _localId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _localId, ref _local);
    }

    private LocalFile? _local;
    /// <summary>
    /// The content library version that the current file belongs to.
    /// </summary>
    public LocalFile? Local
    {
        get => _local;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _localId, ref _local);
    }

    private Guid _versionId;
    /// <summary>
    /// The unique identifier of the parent <see cref="RemoteVersion" />.
    /// </summary>
    public Guid VersionId
    {
        get => _versionId;
        set => value.SetNavigation(_libraryId, _remoteServiceId, _syncRoot, p => (p.LocalId, p.LibraryId, p.RemoteServiceId), ref _versionId, ref _libraryId, ref _remoteServiceId, ref _version);
    }

    private Guid _libraryId;
    /// <summary>
    /// The unique identifier of the parent <see cref="RemoteLibrary" />.
    /// </summary>
    public Guid LibraryId
    {
        get => _libraryId;
        set => _versionId.SetNavigation(value, _remoteServiceId, _syncRoot, p => (p.LocalId, p.LibraryId, p.RemoteServiceId), ref _versionId, ref _libraryId, ref _remoteServiceId, ref _version);
    }

    private Guid _remoteServiceId;
    /// <summary>
    /// The unique identifier of the parent <see cref="RemoteService" />.
    /// </summary>
    public Guid RemoteServiceId
    {
        get => _remoteServiceId;
        set => _versionId.SetNavigation(_libraryId, value, _syncRoot, p => (p.LocalId, p.LibraryId, p.RemoteServiceId), ref _versionId, ref _libraryId, ref _remoteServiceId, ref _version);
    }

    private RemoteVersion? _version;
    /// <summary>
    /// The content library version that the current file belongs to.
    /// </summary>
    public RemoteVersion? Version
    {
        get => _version;
        set => value.SetNavigation(_syncRoot, p => (p.LocalId, p.LibraryId, p.RemoteServiceId), ref _versionId, ref _libraryId, ref _remoteServiceId, ref _version);
    }

    private string? _encoding;
    /// <summary>
    /// The content encoding override for the library file or <see langword="null" /> for no override.
    /// </summary>
    public string? Encoding
    {
        get => _encoding;
        set => _encoding = value.ToTrimmedOrNullIfEmpty();
    }

    private string? _sri;
    /// <summary>
    /// The cryptographic hash override or <see langword="null" /> for no override.
    /// </summary>
    public string? SRI
    {
        get => _sri;
        set => _sri = value.ToTrimmedOrNullIfEmpty();
    }

    private byte[]? _data;
    /// <summary>
    /// The library file content override or <see langword="null" /> for no override.
    /// </summary>
    public byte[]? Data
    {
        get => _data;
        set => _data = (value is null || value.Length == 0) ? null : value;
    }

    /// <summary>
    /// The preferential order override for the remote CDN or <see langword="null" /> to use <see cref="RemoteVersion.Priority" />, <see cref="RemoteLibrary.Priority" /> or <see cref="RemoteService.Priority" />.
    /// </summary>
    public ushort? Priority { get; set; }

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
    
    /// <summary>
    /// Performs configuration of the <see cref="RemoteFile" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<RemoteFile> builder)
    {
        _ = builder.HasKey(nameof(LocalId), nameof(VersionId), nameof(LibraryId), nameof(RemoteServiceId));
        _ = builder.Property(f => f.LocalId)
            .UseCollation("NOCASE");
        _ = builder.Property(f => f.VersionId)
            .UseCollation("NOCASE");
        _ = builder.Property(f => f.LibraryId)
            .UseCollation("NOCASE");
        _ = builder.Property(f => f.RemoteServiceId)
            .UseCollation("NOCASE");
            // .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        _ = builder.Property(f => f.ProviderData).HasConversion(ExtensionMethods.JsonValueConverter);
        _ = builder.HasOne(f => f.Local).WithMany(f => f.Remotes).HasForeignKey(nameof(LocalId)).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
        _ = builder.HasOne(f => f.Version).WithMany(v => v.Files)
            .HasForeignKey(nameof(VersionId), nameof(LibraryId), nameof(RemoteServiceId))
            .HasPrincipalKey(nameof(RemoteVersion.LocalId), nameof(RemoteVersion.LibraryId), nameof(RemoteVersion.RemoteServiceId))
            .IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
    }
}