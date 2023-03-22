using System.Collections.ObjectModel;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using static CdnGetter.SqlDefinitions;

namespace CdnGetter.Model;

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

    /// <summary>
    /// The date and time that the record was created.
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.Now;


    /// <summary>
    /// The date and time that the record was last modified.
    /// </summary>
    public DateTime ModifiedOn { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Remote acess logs for this content library file.
    /// </summary>
    public Collection<FileLog> Logs { get; set; } = new();

    /// <summary>
    /// Performs configuration of the <see cref="RemoteFile" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<RemoteFile> builder)
    {
        _ = builder.HasKey(nameof(LocalId), nameof(VersionId), nameof(LibraryId), nameof(RemoteServiceId));
        _ = builder.Property(nameof(LocalId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(VersionId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(LibraryId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(RemoteServiceId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(SRI)).HasMaxLength(LocalFile.MAXLENGTH_SRI).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(Encoding)).HasMaxLength(LocalFile.MAXLENGTH_Encoding).IsRequired();
        _ = builder.Property(nameof(ProviderData)).HasConversion(ExtensionMethods.JsonValueConverter);
        _ = builder.HasOne(f => f.Local).WithMany(f => f.Remotes).HasForeignKey(nameof(LocalId)).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
        _ = builder.HasOne(f => f.Version).WithMany(v => v.Files)
            .HasForeignKey(nameof(VersionId), nameof(LibraryId), nameof(RemoteServiceId))
            .HasPrincipalKey(nameof(RemoteVersion.LocalId), nameof(RemoteVersion.LibraryId), nameof(RemoteVersion.RemoteServiceId))
            .IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
    }

    internal static void CreateTable(Action<string> executeNonQuery, ILogger logger)
    {
        throw new NotImplementedException();
    }
}