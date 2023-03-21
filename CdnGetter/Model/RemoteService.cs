using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static CdnGetter.SqlDefinitions;

namespace CdnGetter.Model;

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
    
    public const int MAXLENGTH_Name = 1024;
    private string _name = string.Empty;
    /// <summary>
    /// The display name of the registered registered remote content delivery service.
    /// </summary>
    public string Name
    {
        get => _name;
        set => _name = value.ToWsNormalizedOrEmptyIfNull();
    }

    public const ushort DEFAULTVALUE_Priority = ushort.MaxValue;
    /// <summary>
    /// The preferential order for the remote CDN.
    /// </summary>
    public ushort Priority { get; set; } = DEFAULTVALUE_Priority;

    private string _description = string.Empty;
    /// <summary>
    /// The verbose description of the remote content delivery service.
    /// </summary>
    public string Description
    {
        get => _description;
        set => _description = value.ToTrimmedOrEmptyIfNull();
    }

    /// <summary>
    /// The date and time that the record was created.
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    /// <summary>
    /// The date and time that the record was last modified.
    /// </summary>
    public DateTime ModifiedOn { get; set; } = DateTime.Now;

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
        _ = builder.Property(nameof(Id)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(Priority)).IsRequired().HasDefaultValue(ushort.MaxValue);
        _ = builder.HasIndex(nameof(Priority));
        _ = builder.Property(nameof(Name)).IsRequired().HasMaxLength(MAXLENGTH_Name).UseCollation(COLLATION_NOCASE);
        _ = builder.HasIndex(nameof(Name)).IsUnique();
        _ = builder.Property(nameof(Description)).IsRequired();
        _ = builder.Property(nameof(CreatedOn)).HasDefaultValueSql("(datetime('now','localtime'))");
        _ = builder.Property(nameof(ModifiedOn)).HasDefaultValueSql("(datetime('now','localtime'))");
    }
    
    internal static void CreateTable(Action<string> executeNonQuery, ILogger logger)
    {
        /*
        CREATE TABLE IF NOT EXISTS "RemoteServices" (
            "Id" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
            "Name" NVARCHAR(1024) NOT NULL CHECK(length(trim("Name"))=length("Name") AND length("Name")>0) COLLATE NOCASE,
            "Priority" UNSIGNED SMALLINT NOT NULL DEFAULT 65535,
            "Description" TEXT NOT NULL CHECK(length(trim("Description"))=length("Description")),
            "CreatedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
            "ModifiedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
            CONSTRAINT "PK_RemoteServices" PRIMARY KEY("Id"),
            CONSTRAINT "UK_RemoteService_Name" UNIQUE("Name"),
            CHECK("CreatedOn"<="ModifiedOn")
        );
        */
        executeNonQuery(@$"CREATE TABLE IF NOT EXISTS ""{nameof(Services.ContentDb.RemoteServices)}"" (
    {SqlUniqueIdentifier(nameof(Id))},
    {VarCharTrimmedNotEmptyNoCase(nameof(Name), MAXLENGTH_Name)},
    {SqlSmallUInt(nameof(Priority), DEFAULTVALUE_Priority)},
    {SqlTextTrimmed(nameof(Description))},
    {SqlDateTime(nameof(CreatedOn))},
    {SqlDateTime(nameof(ModifiedOn))},
    {SqlPkConstraint(nameof(Services.ContentDb.RemoteServices), nameof(Id))},
    {SqlUniqueConstraint(nameof(RemoteService), nameof(Name))},
    CHECK(""{nameof(CreatedOn)}""<=""{nameof(ModifiedOn)}"")
)");
        // CREATE INDEX "IDX_RemoteServices_Priority" ON "RemoteServices" ("Priority");
        executeNonQuery(SqlIndex(nameof(Services.ContentDb.RemoteServices), nameof(Priority)));
        // CREATE INDEX "IDX_RemoteServices_Name" ON "RemoteServices" ("Name" COLLATE NOCASE);
        executeNonQuery(SqlIndex(nameof(Services.ContentDb.RemoteServices), nameof(Name), true));
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
