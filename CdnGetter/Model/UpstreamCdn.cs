using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static CdnGetter.SqlExtensions;

namespace CdnGetter.Model;

/// <summary>
/// Represents a registered upstream content delivery service.
/// </summary>
public class UpstreamCdn : ModificationTrackingModelBase
{
    private readonly object _syncRoot = new();

    /// <summary>
    /// The unique identifier of the registered upstream service.
    /// </summary>
    public Guid Id { get; set; }
    
    public const int MAXLENGTH_Name = 1024;
    private string _name = string.Empty;
    /// <summary>
    /// The display name of the registered registered upstream content delivery service.
    /// </summary>
    [MaxLength(MAXLENGTH_Name)]
    [MinLength(1)]
    public string Name
    {
        get => _name;
        set => _name = value.ToWsNormalizedOrEmptyIfNull();
    }

    private string _dirName = string.Empty;
    /// <summary>
    /// The name of the local subdirectory where the content for this library is stored.
    /// </summary>
    [MaxLength(MAXLENGTH_FileName)]
    [MinLength(1)]
    public string DirName
    {
        get => _dirName;
        set => _dirName = value.ToTrimmedOrEmptyIfNull();
    }

    public const ushort DEFAULTVALUE_Priority = ushort.MaxValue;
    /// <summary>
    /// The preferential order for the upstream CDN.
    /// </summary>
    public ushort Priority { get; set; } = DEFAULTVALUE_Priority;

    private string _description = string.Empty;
    /// <summary>
    /// The verbose description of the upstream content delivery service.
    /// </summary>
    public string Description
    {
        get => _description;
        set => _description = value.ToTrimmedOrEmptyIfNull();
    }

    /// <summary>
    /// The content libraries that have been retrieved from the upstream content delivery service.
    /// </summary>
    public Collection<CdnLibrary> Libraries { get; set; } = new();
    
    /// <summary>
    /// CDN acess logs for this content library.
    /// </summary>
    public Collection<CdnLog> Logs { get; set; } = new();

    protected override void Validate(ValidationContext validationContext, EntityState state, List<ValidationResult> results)
    {
        if (validationContext.GetService(typeof(EntityEntry)) is EntityEntry entry)
            entry.EnsurePrimaryKey(nameof(Id));
        Validator.TryValidateProperty(Name, new ValidationContext(this, null, null) { MemberName = nameof(Name) }, results);
        Validator.TryValidateProperty(DirName, new ValidationContext(this, null, null) { MemberName = nameof(DirName) }, results);
        base.Validate(validationContext, state, results);
    }

    /// <summary>
    /// Performs configuration of the <see cref="UpstreamCdn" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<UpstreamCdn> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(nameof(Id)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(Priority)).IsRequired().HasDefaultValue(DEFAULTVALUE_Priority);
        _ = builder.HasIndex(nameof(Priority));
        _ = builder.Property(nameof(Name)).IsRequired().HasMaxLength(MAXLENGTH_Name).UseCollation(COLLATION_NOCASE);
        _ = builder.HasIndex(nameof(Name)).IsUnique();
        _ = builder.Property(nameof(DirName)).HasMaxLength(MAXLENGTH_FileName).IsRequired().UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(Description)).IsRequired();
        ModificationTrackingModelBase.OnBuildModificationTrackingModel(builder);
    }
    
    internal static void CreateTable(Action<string> executeNonQuery)
    {
        executeNonQuery(@$"CREATE TABLE ""{nameof(Services.ContentDb.UpstreamCdns)}"" (
    ""{nameof(Id)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(Name)}"" NVARCHAR({MAXLENGTH_Name}) NOT NULL CHECK(length(trim(""{nameof(Name)}""))=length(""{nameof(Name)}"") AND length(""{nameof(Name)}"")>0) UNIQUE COLLATE NOCASE,
    ""{nameof(DirName)}"" NVARCHAR({MAXLENGTH_FileName}) NOT NULL CHECK(length(trim(""{nameof(DirName)}""))=length(""{nameof(DirName)}"") AND length(""{nameof(DirName)}"")>0) UNIQUE COLLATE NOCASE,
    ""{nameof(Priority)}"" UNSIGNED SMALLINT NOT NULL DEFAULT {DEFAULTVALUE_Priority},
    ""{nameof(Description)}"" TEXT NOT NULL CHECK(length(trim(""{nameof(Description)}""))=length(""{nameof(Description)}"")),
    ""{nameof(CreatedOn)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(ModifiedOn)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    PRIMARY KEY(""{nameof(Id)}""),
    CHECK(""{nameof(CreatedOn)}""<=""{nameof(ModifiedOn)}"")
)");
        executeNonQuery($"CREATE INDEX \"IDX_UpstreamCdns_Priority\" ON \"{nameof(Services.ContentDb.UpstreamCdns)}\" (\"{nameof(Priority)}\" ASC)");
        executeNonQuery($"CREATE UNIQUE INDEX \"IDX_UpstreamCdns_Name\" ON \"{nameof(Services.ContentDb.UpstreamCdns)}\" (\"{nameof(Name)}\" COLLATE NOCASE ASC)");
    }

    internal async static Task ShowCDNsAsync(Services.ContentDb dbContext, ILogger logger, IServiceScopeFactory scopeFactory, CancellationToken cancellationToken)
    {
            using IServiceScope scope = scopeFactory.CreateScope();
            int count = 0;
            foreach (KeyValuePair<Guid, (Type Type, string Name, string Description)> item in Services.ContentGetterAttribute.UpstreamCdnServices)
            {
                Guid id = item.Key;
                UpstreamCdn? rsvc = await dbContext.UpstreamCdns.FirstOrDefaultAsync(r => r.Id == id, cancellationToken: cancellationToken);
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
                    logger.LogServiceTypeNotFoundError(type);
            }
            if (count > 0)
            {
                Console.WriteLine("");
                Console.WriteLine("{0:d} CDNs total.", count);
            }
            else
                logger.LogNoUpstreamCdnsFoundWarning();
    }
}
