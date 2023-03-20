using System.Collections.ObjectModel;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CdnGet.Model;

/// <summary>
/// Represents a content library.
/// </summary>
public class ContentLibrary
{
    private readonly object _syncRoot = new();

    private Guid? _id;
    /// <summary>
    /// The unique identifier for the content library.
    /// </summary>
    public Guid Id
    {
        get => _id.EnsureGuid(_syncRoot);
        set => _id = value;
    }
    
    private string _name = "";
    /// <summary>
    /// The name of the content library.
    /// </summary>
    public string Name
    {
        get => _name;
        set => _name = value.ToWsNormalizedOrEmptyIfNull();
    }

    private string _description = "";
    public string Description
    {
        get => _description;
        set => _description = value.ToTrimmedOrEmptyIfNull();
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

    private Collection<LibraryVersion> _versions = new();
    /// <summary>
    /// Library versions for this content library.
    /// </summary>
    public Collection<LibraryVersion> Versions
    {
        get => _versions;
        set => _versions = value ?? new();
    }
    
    /// <summary>
    /// Performs configuration of the <see cref="ContentLibrary" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<ContentLibrary> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(f => f.Id)
            .UseCollation("NOCASE");
        _ = builder.HasIndex(nameof(Name));
        _ = builder.HasIndex(nameof(Name), nameof(RemoteServiceId)).IsUnique();
        _ = builder.Property(f => f.Name)
            .IsRequired()
            .UseCollation("NOCASE");
        _ = builder.Property(f => f.ProviderData).HasConversion(ExtensionMethods.JsonValueConverter);
        _ = builder.HasOne(f => f.RemoteService).WithMany(v => v.Libraries).HasForeignKey(f => f.RemoteServiceId).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
        _ = builder.Property(f => f.RemoteServiceId)
            .UseCollation("NOCASE");
    }

    internal async Task RemoveAsync(Services.ContentDb dbContext, CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;
        Guid id = Id;
        LibraryVersion[] toRemove = await dbContext.Versions.Where(v => v.LibraryId == id).ToArrayAsync(stoppingToken);
        if (stoppingToken.IsCancellationRequested)
            return;
        if (toRemove.Length > 0)
        {
            foreach (LibraryVersion lv in toRemove)
            {
                await lv.RemoveAsync(dbContext, stoppingToken);
                if (stoppingToken.IsCancellationRequested)
                    return;
            }
            await dbContext.SaveChangesAsync(true, stoppingToken);
            if (stoppingToken.IsCancellationRequested)
                return;
        }
        dbContext.Libraries.Remove(this);
    }
}