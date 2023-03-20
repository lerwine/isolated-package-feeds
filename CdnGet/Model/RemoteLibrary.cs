using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CdnGet.Model;

/// <summary>
/// Represents a content library.
/// </summary>
public class RemoteLibrary
{
    private readonly object _syncRoot = new();

    private Guid _localId;
    /// <summary>
    /// The unique identifier of the parent <see cref="LocalLibrary" />.
    /// </summary>
    public Guid LocalId
    {
        get => _remoteServiceId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _remoteServiceId, ref _remoteService);
    }

    private LocalLibrary? _local;
    /// <summary>
    /// The remote service for this content library.
    /// </summary>
    public LocalLibrary? Local
    {
        get => _local;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _localId, ref _local);
    }

    private Guid _remoteServiceId;
    /// <summary>
    /// The unique identifier of the parent <see cref="RemoteService" />.
    /// </summary>
    public Guid RemoteServiceId
    {
        get => _remoteServiceId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _remoteServiceId, ref _remoteService);
    }

    private RemoteService? _remoteService;
    /// <summary>
    /// The remote service for this content library.
    /// </summary>
    public RemoteService? RemoteService
    {
        get => _remoteService;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _remoteServiceId, ref _remoteService);
    }
    
    /// <summary>
    /// The preferential order override for the remote CDN or <see langword="null" /> to use <see cref="RemoteService.Priority" />.
    /// </summary>
    public ushort? Priority { get; set; }

    private string? _description;
    public string? Description
    {
        get => _description;
        set => _description = value.ToTrimmedOrNullIfEmpty();
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
    /// The date and time when the library was last checked for changes.
    /// </summary>
    public DateTime LastChecked
    {
        get => _lastChecked.EnsureLastChecked(ref _createdOn, ref _modifiedOn, _syncRoot);
        set => value.SetLastChecked(ref _createdOn, ref _modifiedOn, ref _lastChecked, _syncRoot);
    }

    /// <summary>
    /// Library versions for this content library.
    /// </summary>
    public Collection<RemoteVersion> Versions { get; set; } = new();
    
    /// <summary>
    /// Performs configuration of the <see cref="RemoteLibrary" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<RemoteLibrary> builder)
    {
        _ = builder.HasKey(nameof(LocalId), nameof(RemoteServiceId));
        _ = builder.Property(f => f.LocalId)
            .UseCollation("NOCASE");
        _ = builder.Property(f => f.RemoteServiceId)
            .UseCollation("NOCASE");
            // .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        _ = builder.Property(f => f.ProviderData).HasConversion(ExtensionMethods.JsonValueConverter);
        _ = builder.HasOne(f => f.RemoteService).WithMany(v => v.Libraries).HasForeignKey(f => f.RemoteServiceId).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
        _ = builder.HasOne(r => r.Local).WithMany(l => l.Remotes).HasForeignKey(r => r.LocalId).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
    }

    internal async Task ClearVersionsAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        Guid id = _localId;
        Guid remoteServiceId = _remoteServiceId;
        foreach (RemoteVersion toRemove in await dbContext.RemoteVersions.Where(l => l.LibraryId == id && l.RemoteServiceId == remoteServiceId).ToArrayAsync(cancellationToken))
        {
            await toRemove.RemoveAsync(dbContext, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
        }
        Versions.Clear();
    }

    internal async Task RemoveAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        await ClearVersionsAsync(dbContext, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        dbContext.RemoteLibraries.Remove(this);
        await dbContext.SaveChangesAsync(true, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        LocalLibrary? local = await dbContext.RemoteLibraries.Entry(this).EnsureRelatedAsync(l => l.Local, cancellationToken);
        if (local is not null)
        {
            Guid id = local.Id;
            if (!(await dbContext.RemoteLibraries.AnyAsync(r => r.LocalId == id)))
                await local.RemoveAsync(dbContext, cancellationToken);
        }
    }
}