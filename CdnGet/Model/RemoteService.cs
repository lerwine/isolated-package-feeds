using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

    /// <summary>
    /// The preferential order for the remote CDN.
    /// </summary>
    public ushort Priority { get; set; }

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

    /// <summary>
    /// The content libraries that have been retrieved from the remote content delivery service.
    /// </summary>
    public Collection<RemoteLibrary> Libraries { get; set; } = new();

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
        _ = builder.Property(c => c.Priority)
            .IsRequired();
    }

    internal async static Task ShowRemotesAsync(Services.ContentDb dbContext, ILogger logger, IServiceScopeFactory scopeFactory, CancellationToken cancellationToken)
    {
            using IServiceScope scope = scopeFactory.CreateScope();
            int count = 0;
            foreach (KeyValuePair<Guid, (Type Type, string Name, string Description)> item in Services.ContentGetterAttribute.RemoteUpdateServices)
            {
                Guid id = item.Key;
                RemoteService? rsvc = await dbContext.RemoteServices.FirstOrDefaultAsync(r => r.Id == id);
                (Type type, string name, string description) = item.Value;
                if (scope.ServiceProvider.GetService(type) is Services.ContentGetterService)
                {
                    count++;
                    if (rsvc is not null)
                    {
                        if (name.Length > 0 && !Services.MainService.NameComparer.Equals(rsvc.Name, name))
                            Console.WriteLine("{0}; {1}", rsvc.Name, name);
                        else
                            Console.WriteLine(rsvc.Name);
                    }
                    else if (name.Length > 0)
                        Console.WriteLine(name);
                    if (description.Length > 0)
                        foreach (string d in description.SplitLines().Select(l => l.ToWsNormalizedOrEmptyIfNull()!))
                            Console.WriteLine((d.Length > 0) ? $"\t{d}" : d);
                }
                else
                    logger.LogServiceTypeNotFound(type);
            }
            if (count > 0)
            {
                Console.WriteLine("");
                Console.WriteLine("{0:d} remotes total.", count);
            }
            else
                logger.LogNoRemotesFound();
    }
}
