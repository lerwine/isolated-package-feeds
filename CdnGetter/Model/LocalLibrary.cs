using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using static CdnGetter.SqlExtensions;
using IsolatedPackageFeeds.Shared;

namespace CdnGetter.Model;

public class LocalLibrary : ModificationTrackingModelBase
{
    /// <summary>
    /// Gets or sets the unique identifier for the content library.
    /// </summary>
    public Guid Id { get; set; }
    
    public const int MAXLENGTH_Name = 1024;
    private string _name = string.Empty;
    /// <summary>
    /// Gets or sets the name of the content library.
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
    /// Gets or sets the name of the local subdirectory where the content for this library is stored.
    /// </summary>
    /// <remarks>The 
    /// </remarks>
    [MaxLength(MAXLENGTH_FileName)]
    [MinLength(1)]
    public string DirName
    {
        get => _dirName;
        set => _dirName = value.ToTrimmedOrEmptyIfNull();
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
    /// Library versions for this content library.
    /// </summary>
    public Collection<LocalVersion> Versions { get; set; } = [];
    
    public Collection<CdnLibrary> Upstream { get; set; } = [];
    
    protected override void Validate(ValidationContext validationContext, EntityState state, List<ValidationResult> results)
    {
        if (validationContext.GetService(typeof(EntityEntry)) is EntityEntry entry)
            entry.EnsurePrimaryKey(nameof(Id));
        Validator.TryValidateProperty(Name, new ValidationContext(this, null, null) { MemberName = nameof(Name) }, results);
        Validator.TryValidateProperty(DirName, new ValidationContext(this, null, null) { MemberName = nameof(DirName) }, results);
        base.Validate(validationContext, state, results);
    }

    /// <summary>
    /// Performs configuration of the <see cref="LocalLibrary" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<LocalLibrary> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(nameof(Id)).UseCollation(COLLATION_NOCASE);
        _ = builder.HasIndex(nameof(Name)).IsUnique();
        _ = builder.Property(nameof(Name)).HasMaxLength(MAXLENGTH_Name).IsRequired().UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(DirName)).HasMaxLength(MAXLENGTH_FileName).IsRequired().UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(Description)).IsRequired();
        ModificationTrackingModelBase.OnBuildModificationTrackingModel(builder);
    }

    internal static void CreateTable(Action<string> executeNonQuery)
    {
        executeNonQuery(@$"CREATE TABLE ""{nameof(Services.ContentDb.LocalLibraries)}"" (
    ""{nameof(Id)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(Name)}"" NVARCHAR({MAXLENGTH_Name}) NOT NULL CHECK(length(trim(""{nameof(Name)}""))=length(""{nameof(Name)}"") AND length(""{nameof(Name)}"")>0) UNIQUE COLLATE NOCASE,
    ""{nameof(DirName)}"" NVARCHAR({MAXLENGTH_FileName}) NOT NULL CHECK(length(trim(""{nameof(DirName)}""))=length(""{nameof(DirName)}"") AND length(""{nameof(DirName)}"")>0) UNIQUE COLLATE NOCASE,
    ""{nameof(Description)}"" TEXT NOT NULL CHECK(length(trim(""{nameof(Description)}""))=length(""{nameof(Description)}"")),
    ""{nameof(CreatedOn)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(ModifiedOn)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    PRIMARY KEY(""{nameof(Id)}""),
    CHECK(""{nameof(CreatedOn)}""<=""{nameof(ModifiedOn)}"")
)");
        executeNonQuery($"CREATE UNIQUE INDEX \"IDX_LocalLibraries_Name\" ON \"{nameof(Services.ContentDb.LocalLibraries)}\" (\"{nameof(Name)}\" COLLATE NOCASE ASC)");
    }

    internal async Task ClearCDNsAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        Guid id = Id;
        foreach (CdnLibrary toRemove in await dbContext.CdnLibraries.Where(l => l.LocalId == id).ToArrayAsync(cancellationToken))
        {
            await toRemove.RemoveAsync(dbContext, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
        }
        Upstream.Clear();
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
        await ClearCDNsAsync(dbContext, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        await ClearVersionsAsync(dbContext, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        dbContext.LocalLibraries.Remove(this);
        await dbContext.SaveChangesAsync(true, cancellationToken);
    }

    /// <summary>
    /// Gets new versions for the specified libraries.
    /// </summary>
    /// <param name="libraryNames">The explicit library names to get new versions for. This should not be empty.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The current logger.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The asyncrhonous task.</returns>
    internal static async Task GetNewVersionsPreferredAsync(ImmutableArray<string> cdnNames, Services.ContentDb dbContext, ILogger logger, CancellationToken cancellationToken)
    {
        LocalLibrary[] libraries = await dbContext.LocalLibraries.ToArrayAsync(cancellationToken);
        if (libraries.Length == 0)
            logger.LogNothingToDoWarning();
        else
            throw new NotImplementedException("Get New Versions for Preferred not implemented.");
            // foreach (LocalLibrary l in libraries)
    }
}