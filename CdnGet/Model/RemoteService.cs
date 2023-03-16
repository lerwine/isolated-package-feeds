using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CdnGet.Model;

/// <summary>
/// Represents a registered remote content delivery service.
/// </summary>
public class RemoteService
{
    private readonly object _syncRoot = new();

    private Guid? _id;
    /// <summary>
    /// The unique identifier of the registered remote service.
    /// </summary>
    public Guid Id
    {
        get => _id.EnsureGuid(_syncRoot);
        set => _id = value;
    }
    
    private string _name = "";
    /// <summary>
    /// The display name of the registered registered remote content delivery service.
    /// </summary>
    public string Name
    {
        get => _name;
        set => _name = value.ToWsNormalizedOrEmptyIfNull();
    }

    private string _description = "";
    /// <summary>
    /// The verbose description of the remote content delivery service.
    /// </summary>
    public string Description
    {
        get => _description;
        set => _description = value.ToTrimmedOrEmptyIfNull();
    }

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
    /// The date and time when the remote service was last checked for changes.
    /// </summary>
    public DateTime LastChecked
    {
        get => _lastChecked.EnsureLastChecked(ref _createdOn, ref _modifiedOn, _syncRoot);
        set => value.SetLastChecked(ref _createdOn, ref _modifiedOn, ref _lastChecked, _syncRoot);
    }

    private Collection<ContentLibrary> _libraries = new();
    /// <summary>
    /// The content libraries that have been retrieved from the remote content delivery service.
    /// </summary>
    public Collection<ContentLibrary> Libraries
    {
        get => _libraries;
        set => _libraries = value ?? new();
    }

    /// <summary>
    /// Performs configuration of the <see cref="RemoteService" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<RemoteService> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(c => c.Id)
            .UseCollation("NOCASE");
        _ = builder.HasIndex(nameof(Name)).IsUnique();
        _ = builder.Property(c => c.Name)
            .IsRequired()
            .UseCollation("NOCASE");
            // .UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }
}
