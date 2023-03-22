using System.Collections.ObjectModel;
using CdnGetter.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using static CdnGetter.SqlDefinitions;

namespace CdnGetter.Model;

public class LocalLibrary
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
    
    public const int MAXLENGTH_Name = 1024;
    private string _name = string.Empty;
    /// <summary>
    /// The name of the content library.
    /// </summary>
    public string Name
    {
        get => _name;
        set => _name = value.ToWsNormalizedOrEmptyIfNull();
    }

    private string _description = string.Empty;
    /// <summary>
    /// Verbose description of the content library.
    /// </summary>
    public string? Description
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
    /// Library versions for this content library.
    /// </summary>
    public Collection<LocalVersion> Versions { get; set; } = new();
    
    public Collection<RemoteLibrary> Remotes { get; set; } = new();
    
    /// <summary>
    /// Performs configuration of the <see cref="LocalLibrary" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<LocalLibrary> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(nameof(Id)).UseCollation(COLLATION_NOCASE);
        _ = builder.HasIndex(nameof(Name)).IsUnique();
        _ = builder.Property(nameof(Name)).IsRequired().UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(Description)).IsRequired();
        _ = builder.Property(nameof(CreatedOn)).IsRequired().HasDefaultValueSql(DEFAULT_SQL_NOW);
        _ = builder.Property(nameof(ModifiedOn)).IsRequired().HasDefaultValueSql(DEFAULT_SQL_NOW);
    }

    internal static void CreateTable(Action<string> executeNonQuery, ILogger logger)
    {
        executeNonQuery(@$"CREATE TABLE ""{nameof(Services.ContentDb.LocalLibraries)}"" (
    ""{nameof(Id)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(Name)}"" NVARCHAR({MAXLENGTH_Name}) NOT NULL CHECK(length(trim(""{nameof(Name)}""))=length(""{nameof(Name)}"") AND length(""{nameof(Name)}"")>0) UNIQUE COLLATE NOCASE,
    ""{nameof(Description)}"" TEXT NOT NULL CHECK(length(trim(""{nameof(Description)}""))=length(""{nameof(Description)}"")),
    ""{nameof(CreatedOn)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(ModifiedOn)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    PRIMARY KEY(""{nameof(Id)}""),
    CHECK(""{nameof(CreatedOn)}""<=""{nameof(ModifiedOn)}"")
)");
        executeNonQuery($"CREATE UNIQUE INDEX \"IDX_LocalLibraries_Name\" ON \"{nameof(Services.ContentDb.LocalLibraries)}\" (\"{nameof(Name)}\" COLLATE NOCASE ASC)");
    }

    internal async Task ClearRemotesAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        Guid id = Id;
        foreach (RemoteLibrary toRemove in await dbContext.RemoteLibraries.Where(l => l.LocalId == id).ToArrayAsync(cancellationToken))
        {
            await toRemove.RemoveAsync(dbContext, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
        }
        Remotes.Clear();
    }

    internal async Task ClearVersionsAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        Guid id = Id;
        foreach (LocalVersion toRemove in await dbContext.LocalVersions.Where(l => l.LibraryId == id).ToArrayAsync(cancellationToken))
        {
            await toRemove.RemoveAsync(dbContext, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
        }
        Versions.Clear();
    }

    internal async Task RemoveAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        await ClearRemotesAsync(dbContext, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        await ClearVersionsAsync(dbContext, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        dbContext.LocalLibraries.Remove(this);
        await dbContext.SaveChangesAsync(true, cancellationToken);
    }

    internal async Task ReloadAsync(Services.ContentDb dbContext, CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }

    internal async Task GetNewVersionsAsync(Services.ContentDb dbContext, CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }

    internal async Task GetNewVersionsPreferredAsync(Services.ContentDb dbContext, CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }

    internal static async Task AddAsync(string libraryName, Services.ContentDb dbContext, AppSettings appSettings, CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}