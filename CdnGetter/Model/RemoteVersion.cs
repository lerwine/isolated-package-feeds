using System.Collections.ObjectModel;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using static CdnGetter.SqlDefinitions;

namespace CdnGetter.Model;

/// <summary>
/// Represents a specific version of a content library.
/// </summary>
public class RemoteVersion
{
    private readonly object _syncRoot = new();

    private Guid _localId;
    /// <summary>
    /// The unique identifier of the parent <see cref="LocalVersion" />.
    /// </summary>
    public Guid LocalId
    {
        get => _localId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _localId, ref _local);
    }

    private LocalVersion? _local;
    /// <summary>
    /// The content library version that the current file belongs to.
    /// </summary>
    public LocalVersion? Local
    {
        get => _local;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _localId, ref _local);
    }

    private Guid _libraryId;
    /// <summary>
    /// The unique identifier of the parent <see cref="RemoteLibrary" />.
    /// </summary>
    public Guid LibraryId
    {
        get => _libraryId;
        set => value.SetNavigation(_remoteServiceId, _syncRoot, p => (p.LocalId, p.RemoteServiceId), ref _libraryId, ref _remoteServiceId, ref _library);
    }

    private Guid _remoteServiceId;
    /// <summary>
    /// The unique identifier of the parent <see cref="RemoteService" />.
    /// </summary>
    public Guid RemoteServiceId
    {
        get => _remoteServiceId;
        set => _libraryId.SetNavigation(value, _syncRoot, p => (p.LocalId, p.RemoteServiceId), ref _libraryId, ref _remoteServiceId, ref _library);
    }

    private RemoteLibrary? _library;
    /// <summary>
    /// The parent content library.
    /// </summary>
    public RemoteLibrary? Library
    {
        get => _library;
        set => value.SetNavigation(_syncRoot, p => (p.LocalId, p.RemoteServiceId), ref _libraryId, ref _remoteServiceId, ref _library);
    }
    
    /// <summary>
    /// The preferential order override for the remote CDN or <see langword="null" /> to use <see cref="RemoteLibrary.Priority" /> or <see cref="RemoteService.Priority" />.
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
    /// The files that belong to the current version of the content library.
    /// </summary>
    public Collection<RemoteFile> Files { get; set; } = new();
    
    /// <summary>
    /// Remote acess logs for this content library version.
    /// </summary>
    public Collection<VersionLog> Logs { get; set; } = new();

    /// <summary>
    /// Performs configuration of the <see cref="RemoteVersion" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<RemoteVersion> builder)
    {
        _ = builder.HasKey(nameof(LocalId), nameof(LibraryId), nameof(RemoteServiceId));
        _ = builder.Property(nameof(LocalId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(LibraryId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(RemoteServiceId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(ProviderData)).HasConversion(ExtensionMethods.JsonValueConverter);
        _ = builder.HasOne(f => f.Local).WithMany(f => f.Remotes).HasForeignKey(nameof(LocalId)).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
        _ = builder.HasOne(v => v.Library).WithMany(l => l.Versions)
            .HasForeignKey(nameof(LibraryId), nameof(RemoteServiceId))
            .HasPrincipalKey(nameof(LocalId), nameof(RemoteServiceId))
            .IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
    }

    internal static void CreateTable(Action<string> executeNonQuery, ILogger logger)
    {
        throw new NotImplementedException();
    }

    internal async Task ClearFilesAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        Guid id = _localId;
        Guid libraryId = _libraryId;
        Guid remoteServiceId = _remoteServiceId;
        RemoteFile[] toRemove = await dbContext.RemoteFiles.Where(f => f.VersionId == id && f.LibraryId == libraryId && f.RemoteServiceId == remoteServiceId).ToArrayAsync(cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        if (toRemove.Length > 0)
        {
            dbContext.RemoteFiles.RemoveRange(toRemove);
            await dbContext.SaveChangesAsync(true, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
            Files.Clear();
        }
    }
    
    internal async Task RemoveAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        await ClearFilesAsync(dbContext, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        dbContext.RemoteVersions.Remove(this);
        await dbContext.SaveChangesAsync(true, cancellationToken);
    }
}
