using System.Collections.ObjectModel;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CdnGet.Model;

/// <summary>
/// Represents a specific version of a content library.
/// </summary>
public class LibraryVersion
{
    private readonly object _syncRoot = new();

    private Guid? _id;
    /// <summary>
    /// The unique identifier for the library version.
    /// </summary>
    public Guid Id
    {
        get => _id.EnsureGuid(_syncRoot);
        set => _id = value;
    }
    
    /// <summary>
    /// The library version.
    /// </summary>
    public SwVersion Version { get; set; }

    /// <summary>
    /// The release order for the library version.
    /// </summary>
    public ushort Order { get; set; }

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
    /// The date and time when the library version was last checked for changes.
    /// </summary>
    public DateTime LastChecked
    {
        get => _lastChecked.EnsureLastChecked(ref _createdOn, ref _modifiedOn, _syncRoot);
        set => value.SetLastChecked(ref _createdOn, ref _modifiedOn, ref _lastChecked, _syncRoot);
    }

    private Guid _libraryId;
    /// <summary>
    /// The unique identifier of the parent <see cref="ContentLibrary" />.
    /// </summary>
    public Guid LibraryId
    {
        get => _libraryId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _libraryId, ref _library);
    }

    private ContentLibrary? _library;
    /// <summary>
    /// The parent content library.
    /// </summary>
    public ContentLibrary? Library
    {
        get => _library;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _libraryId, ref _library);
    }

    private Collection<LibraryFile> _file = new();
    /// <summary>
    /// The files that belong to the current version of the content library.
    /// </summary>
    public Collection<LibraryFile> Files
    {
        get => _file;
        set => _file = value ?? new();
    }

    /// <summary>
    /// Performs configuration of the <see cref="LibraryVersion" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<LibraryVersion> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(v => v.Id)
            .UseCollation("NOCASE");
        _ = builder.HasIndex(nameof(Version));
        _ = builder.HasIndex(nameof(Version), nameof(LibraryId)).IsUnique();
        _ = builder.Property(v => v.Version).HasConversion(SwVersion.Converter)
            .IsRequired()
            .UseCollation("NOCASE");
            // .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        _ = builder.Property(c => c.Order).IsRequired();
        _ = builder.HasIndex(nameof(Order));
        _ = builder.HasIndex(nameof(Order), nameof(LibraryId)).IsUnique();
        _ = builder.Property(v => v.ProviderData).HasConversion(ExtensionMethods.JsonValueConverter);
        _ = builder.HasOne(v => v.Library).WithMany(l => l.Versions).HasForeignKey(l => l.LibraryId).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
        _ = builder.Property(f => f.LibraryId)
            .UseCollation("NOCASE");
    }

    internal async Task RemoveAsync(Services.ContentDb dbContext, CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;
        Guid id = Id;
        LibraryFile[] toRemove = await dbContext.Files.Where(v => v.VersionId == id).ToArrayAsync(stoppingToken);
        if (stoppingToken.IsCancellationRequested)
            return;
        if (toRemove.Length > 0)
        {
            dbContext.Files.RemoveRange(toRemove);
            await dbContext.SaveChangesAsync(true, stoppingToken);
            if (stoppingToken.IsCancellationRequested)
                return;
        }
        dbContext.Versions.Remove(this);
    }
}
