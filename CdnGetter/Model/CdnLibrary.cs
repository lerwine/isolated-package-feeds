using System.Collections.ObjectModel;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using static CdnGetter.SqlExtensions;

namespace CdnGetter.Model;

/// <summary>
/// Represents an upstream content library.
/// </summary>
public class CdnLibrary : ModificationTrackingModelBase
{
    private readonly object _syncRoot = new();

    private Guid _localId;
    /// <summary>
    /// Gets or sets the unique identifier of the parent <see cref="LocalLibrary" />.
    /// </summary>
    public Guid LocalId
    {
        get => _cdnId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _cdnId, ref _cdn);
    }

    private LocalLibrary? _local;
    /// <summary>
    /// Gets or sets the upstream service for this content library.
    /// </summary>
    public LocalLibrary? Local
    {
        get => _local;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _localId, ref _local);
    }

    private Guid _cdnId;
    /// <summary>
    /// Gets or sets the unique identifier of the parent <see cref="Cdn" />.
    /// </summary>
    public Guid CdnId
    {
        get => _cdnId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _cdnId, ref _cdn);
    }

    private UpstreamCdn? _cdn;
    /// <summary>
    /// Gets or sets the upstream service for this content library.
    /// </summary>
    public UpstreamCdn? Cdn
    {
        get => _cdn;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _cdnId, ref _cdn);
    }
    
    /// <summary>
    /// Gets or sets the preferential order override for the upstream CDN or <see langword="null" /> to use <see cref="UpstreamCdn.Priority" />.
    /// </summary>
    public ushort? Priority { get; set; }

    private string? _description;
    public string? Description
    {
        get => _description;
        set => _description = value.ToTrimmedOrNullIfEmpty();
    }

    /// <summary>
    /// Optional provider-specific data for <see cref="Cdn" />.
    /// </summary>
    public JsonNode? ProviderData { get; set; }

    /// <summary>
    /// Library versions for this content library.
    /// </summary>
    public Collection<CdnVersion> Versions { get; set; } = new();
    
    /// <summary>
    /// CDN acess logs for this content library.
    /// </summary>
    public Collection<LibraryLog> Logs { get; set; } = new();
    
    /// <summary>
    /// Performs configuration of the <see cref="CdnLibrary" /> entity type in the model for the <see cref="Services.ContentDb" />.
    /// </summary>
    /// <param name="builder">The builder being used to configure the current entity type.</param>
    internal static void OnBuildEntity(EntityTypeBuilder<CdnLibrary> builder)
    {
        _ = builder.HasKey(nameof(LocalId), nameof(CdnId));
        _ = builder.Property(nameof(LocalId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(CdnId)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(ProviderData)).HasConversion(ValueConverters.JsonValueConverter);
        ModificationTrackingModelBase.OnBuildModificationTrackingModel(builder);
        _ = builder.HasOne(f => f.Cdn).WithMany(v => v.Libraries).HasForeignKey(f => f.CdnId).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(r => r.Local).WithMany(l => l.Upstream).HasForeignKey(r => r.LocalId).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }

    internal static void CreateTable(Action<string> executeNonQuery)
    {
        executeNonQuery(@$"CREATE TABLE ""{nameof(Services.ContentDb.CdnLibraries)}"" (
    ""{nameof(LocalId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(CdnId)}"" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    ""{nameof(Priority)}"" UNSIGNED SMALLINT DEFAULT NULL,
    ""{nameof(Description)}"" TEXT DEFAULT NULL CHECK(""{nameof(Description)}"" IS NULL OR length(trim(""{nameof(Description)}""))=length(""{nameof(Description)}"")),
    ""{nameof(ProviderData)}"" TEXT DEFAULT NULL,
    ""{nameof(CreatedOn)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(ModifiedOn)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    PRIMARY KEY(""{nameof(LocalId)}"",""{nameof(CdnId)}""),
    FOREIGN KEY(""{nameof(CdnId)}"") REFERENCES ""{nameof(Services.ContentDb.UpstreamCdns)}""(""{nameof(Model.UpstreamCdn.Id)}"") ON DELETE RESTRICT,
    FOREIGN KEY(""{nameof(LocalId)}"") REFERENCES ""{nameof(Services.ContentDb.LocalLibraries)}""(""{nameof(LocalLibrary.Id)}"") ON DELETE RESTRICT,
    CHECK(""{nameof(CreatedOn)}""<=""{nameof(ModifiedOn)}"")
)");
    }

    internal async Task ClearVersionsAsync(Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        Guid id = _localId;
        Guid upstreamCdnId = _cdnId;
        foreach (CdnVersion toRemove in await dbContext.CdnVersions.Where(l => l.LibraryId == id && l.UpstreamCdnId == upstreamCdnId).ToArrayAsync(cancellationToken))
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
        dbContext.CdnLibraries.Remove(this);
        await dbContext.SaveChangesAsync(true, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        LocalLibrary? local = await this.GetReferencedEntityAsync(dbContext.CdnLibraries, l => l.Local, cancellationToken);
        if (local is not null)
        {
            Guid id = local.Id;
            if (!await dbContext.CdnLibraries.AnyAsync(r => r.LocalId == id, cancellationToken: cancellationToken))
                await local.RemoveAsync(dbContext, cancellationToken);
        }
    }

    internal static async Task ShowAsync(IEnumerable<string> cdnNames, Services.ContentDb dbContext, ILogger logger, CancellationToken cancellationToken)
    {
        if (cdnNames.Any())
        {
            LinkedList<UpstreamCdn> cdns = await UpstreamCdn.GetByNamesAsync(cdnNames, dbContext, logger, cancellationToken);
            if (cdns.Count == 1)
            {
                Guid id = cdns.First!.Value.Id;
                await dbContext.CdnLibraries.Where(l => l.LocalId == id).Include(l => l.Local).Select(l => l.Local!.Name).ForEachAsync(n => Console.WriteLine(n), cancellationToken);
            }
            else
                foreach (UpstreamCdn cdn in cdns)
                {
                    Console.WriteLine(cdn.Name);
                    Guid id = cdn.Id;
                    await dbContext.CdnLibraries.Where(l => l.LocalId == id).Include(l => l.Local).Select(l => l.Local!.Name).ForEachAsync(ln => Console.WriteLine($"\t{ln}"), cancellationToken);
                }
        }
        else
            await dbContext.LocalLibraries.ForEachAsync(l => Console.WriteLine(l.Name), cancellationToken);
    }

#pragma warning disable CS1998
    internal static async Task AddAsync(IEnumerable<string> cdnNames, IEnumerable<string> versionStrings, Services.ContentDb dbContext, ILogger<Services.MainService> logger, CancellationToken cancellationToken)
#pragma warning restore CS1998
    {
            if (cdnNames.Any())
            {
                throw new NotImplementedException("Add libraries functionality not implemented.");
            }
            else
                logger.LogRequiredDependentParameterWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Upstream)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.AddLibrary)}");
    }

#pragma warning disable CS1998
    internal static async Task RemoveAsync(IEnumerable<string> cdnNames, IEnumerable<string> versionStrings, Services.ContentDb dbContext, ILogger<Services.MainService> logger, CancellationToken cancellationToken)
#pragma warning restore CS1998
    {
    }

#pragma warning disable CS1998
    internal static async Task ReloadAsync(IEnumerable<string> cdnNames, IEnumerable<string> versionStrings, Services.ContentDb dbContext, ILogger<Services.MainService> logger, CancellationToken cancellationToken)
#pragma warning restore CS1998
    {
        if (cdnNames.Any())
        {
            throw new NotImplementedException("Reload libraries for specific CDNs functionality not implemented.");
        }
        else if (versionStrings.Any())
        {
            throw new NotImplementedException("Reload libraries for specific versions functionality not implemented.");
        }
        else
            logger.LogRequiredAltDependentParameterWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Upstream)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Version)}",
                $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadLibrary)}");
    }

#pragma warning disable CS1998
    internal static async Task ReloadExistingAsync(IEnumerable<string> cdnNames, IEnumerable<string> versionStrings, Services.ContentDb dbContext, ILogger<Services.MainService> logger, CancellationToken cancellationToken)
#pragma warning restore CS1998
    {
        if (cdnNames.Any())
        {
            throw new NotImplementedException("Reload existing versions for specific CDNs functionality not implemented.");
        }
        else if (versionStrings.Any())
        {
            throw new NotImplementedException("Reload libraries for specific existing versions functionality not implemented.");
        }
        else
            logger.LogRequiredAltDependentParameterWarning($"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Upstream)}", $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.Version)}",
                $"{nameof(CdnGetter)}:{nameof(Config.CommandSettings.ReloadExistingVersions)}");
    }
}