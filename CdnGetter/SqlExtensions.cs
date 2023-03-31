using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CdnGetter;

/// <summary>
/// SQL-related extension methods.
/// </summary>
public static class SqlExtensions
{
    /// <summary>
    /// The Sqlite collation for case-insensitive matching.
    /// </summary>
    public const string COLLATION_NOCASE = "NOCASE";

    /// <summary>
    /// The Sqlite code for the current date and time.
    /// </summary>
    /// <returns></returns>
    public const string DEFAULT_SQL_NOW = "(datetime('now','localtime'))";

    /// <summary>
    /// The maximum length for URL properties.
    /// </summary>
    public const int MAXLENGTH_Url = 4096;

    /// <summary>
    /// The maximum length for file and subdirectory names.
    /// </summary>
    public const int MAXLENGTH_FileName = 256;

    /// <summary>
    /// THe maximum lenght for SRI properties.
    /// </summary>
    public const int MAXLENGTH_SRI = 256;

    /// <summary>
    /// The maximum length for content type properties.
    /// </summary>
    public const int MAXLENGTH_ContentType = 512;

    /// <summary>
    /// The minimum length for content type properties.
    /// </summary>
    public const int MINLENGTH_ContentType = 3;

    /// <summary>
    /// The maximum length for content encoding properties.
    /// </summary>
    public const int MAXLENGTH_Encoding = 32;
    
    /// <summary>
    /// Ensures that an entity property has a random <see cref="Guid" /> assigned when being added.
    /// </summary>
    /// <param name="entry">The target entity entry.</param>
    /// <param name="propertyName">The name of the primary key property.</param>
    public static void EnsurePrimaryKey(this EntityEntry entry, string propertyName)
    {
        PropertyEntry propertyEntry = entry.Property(propertyName);
        if (typeof(Guid).Equals(propertyEntry.Metadata.ClrType) && entry.State == EntityState.Added)
        {
            Guid? pk = propertyEntry.CurrentValue as Guid?;
            if (!pk.HasValue || pk.Value.Equals(Guid.Empty))
                propertyEntry.CurrentValue = Guid.NewGuid();
        }
    }

    /// <summary>
    /// Indicates whether the entry exists in the target database.
    /// </summary>
    /// <param name="entry">The <see cref="EntityEntry"/> to test.</param>
    /// <returns><see langword="true"/> if the <paramref name="entry"/> is not <see langword="null"/> and its <see cref="EntityEntry.State"/>
    /// is <see cref="EntityState.Unchanged"/> or <see cref="EntityState.Modified"/>; otherwise, <see langword="false"/>.</returns>
    public static bool ExistsInDb(this EntityEntry? entry)
    {
        if (entry is null)
            return false;
        return entry.State switch
        {
            EntityState.Unchanged or EntityState.Modified => true,
            _ => false,
        };
    }

    /// <summary>
    /// Asyncrhonously loads and gets the related items for a navigation property.
    /// </summary>
    /// <param name="entry">The parent entity entry.</param>
    /// <param name="propertyExpression">The expression for the nagivation property.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <typeparam name="TEntity">The parent entity type.</typeparam>
    /// <typeparam name="TProperty">The related entity type.</typeparam>
    /// <returns>The related entity objects.</returns>
    public static async Task<IEnumerable<TProperty>> GetRelatedCollectionAsync<TEntity, TProperty>(this EntityEntry<TEntity>? entry, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        if (entry is null)
            return Enumerable.Empty<TProperty>();
        CollectionEntry<TEntity, TProperty> collectionEntry = entry.Collection(propertyExpression);
        if (!collectionEntry.IsLoaded && entry.ExistsInDb())
            await collectionEntry.LoadAsync(cancellationToken);
        return collectionEntry.CurrentValue ?? Enumerable.Empty<TProperty>();
    }

    /// <summary>
    /// Asyncrhonously loads and gets the related items for a navigation property.
    /// </summary>
    /// <param name="entity">The parent entity.</param>
    /// <param name="dbSet">The database context property for the parent entity's table.</param>
    /// <param name="propertyExpression">The expression for the nagivation property.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <typeparam name="TEntity">The parent entity type.</typeparam>
    /// <typeparam name="TProperty">The related entity type.</typeparam>
    /// <returns>The related entity objects.</returns>
    public static async Task<IEnumerable<TProperty>> GetRelatedCollectionAsync<TEntity, TProperty>(this TEntity? entity, DbSet<TEntity> dbSet, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        if (entity is null)
            return Enumerable.Empty<TProperty>();
        return await dbSet.Entry(entity).GetRelatedCollectionAsync(propertyExpression, cancellationToken);
    }

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.CdnLibrary" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entity">The source <see cref="Model.UpstreamCdn" /> entity.</param>
    /// <param name="dbContext">The current database context.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.UpstreamCdn.Upstream" /> property.</returns>
    public static async Task<IEnumerable<Model.CdnLibrary>> GetLibrariesAsync(this Model.UpstreamCdn? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetRelatedCollectionAsync(dbContext.UpstreamCdns, u => u.Libraries, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.CdnLibrary" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entry">The source <see cref="Model.UpstreamCdn" /> <see cref="EntityEntry{TEntity}" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.UpstreamCdn.Upstream" /> property.</returns>
    public static async Task<IEnumerable<Model.CdnLibrary>> GetLibrariesAsync(this EntityEntry<Model.UpstreamCdn>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Libraries, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.CdnLibrary" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entity">The source <see cref="Model.LocalLibrary" /> entity.</param>
    /// <param name="dbContext">The current database context.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.LocalLibrary.Upstream" /> property.</returns>
    public static async Task<IEnumerable<Model.CdnLibrary>> GetUpstreamAsync(this Model.LocalLibrary? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetRelatedCollectionAsync(dbContext.LocalLibraries, u => u.Upstream, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.CdnLibrary" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entry">The source <see cref="Model.LocalLibrary" /> <see cref="EntityEntry{TEntity}" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.LocalLibrary.Upstream" /> property.</returns>
    public static async Task<IEnumerable<Model.CdnLibrary>> GetUpstreamAsync(this EntityEntry<Model.LocalLibrary>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Upstream, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.CdnLog" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entity">The source <see cref="Model.UpstreamCdn" /> entity.</param>
    /// <param name="dbContext">The current database context.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.UpstreamCdn.Logs" /> property.</returns>
    public static async Task<IEnumerable<Model.CdnLog>> GetLogsAsync(this Model.UpstreamCdn? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetRelatedCollectionAsync(dbContext.UpstreamCdns, u => u.Logs, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.CdnLog" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entry">The source <see cref="Model.UpstreamCdn" /> <see cref="EntityEntry{TEntity}" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.UpstreamCdn.Logs" /> property.</returns>
    public static async Task<IEnumerable<Model.CdnLog>> GetLogsAsync(this EntityEntry<Model.UpstreamCdn>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Logs, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.CdnVersion" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entity">The source <see cref="Model.CdnLibrary" /> entity.</param>
    /// <param name="dbContext">The current database context.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.CdnLibrary.Versions" /> property.</returns>
    public static async Task<IEnumerable<Model.CdnVersion>> GetVersionsAsync(this Model.CdnLibrary? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetRelatedCollectionAsync(dbContext.CdnLibraries, u => u.Versions, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.CdnVersion" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entry">The source <see cref="Model.CdnLibrary" /> <see cref="EntityEntry{TEntity}" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.CdnLibrary.Versions" /> property.</returns>
    public static async Task<IEnumerable<Model.CdnVersion>> GetVersionsAsync(this EntityEntry<Model.CdnLibrary>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Versions, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.CdnVersion" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entity">The source <see cref="Model.LocalVersion" /> entity.</param>
    /// <param name="dbContext">The current database context.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.LocalVersion.Upstream" /> property.</returns>
    public static async Task<IEnumerable<Model.CdnVersion>> GetUpstreamAsync(this Model.LocalVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetRelatedCollectionAsync(dbContext.LocalVersions, u => u.Upstream, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.CdnVersion" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entry">The source <see cref="Model.LocalVersion" /> <see cref="EntityEntry{TEntity}" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.LocalVersion.Upstream" /> property.</returns>
    public static async Task<IEnumerable<Model.CdnVersion>> GetUpstreamAsync(this EntityEntry<Model.LocalVersion>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Upstream, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.LibraryLog" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entity">The source <see cref="Model.CdnLibrary" /> entity.</param>
    /// <param name="dbContext">The current database context.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.CdnLibrary.Logs" /> property.</returns>
    public static async Task<IEnumerable<Model.LibraryLog>> GetLogsAsync(this Model.CdnLibrary? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetRelatedCollectionAsync(dbContext.CdnLibraries, u => u.Logs, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.LibraryLog" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entry">The source <see cref="Model.CdnLibrary" /> <see cref="EntityEntry{TEntity}" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.CdnLibrary.Logs" /> property.</returns>
    public static async Task<IEnumerable<Model.LibraryLog>> GetLogsAsync(this EntityEntry<Model.CdnLibrary>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Logs, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.CdnFile" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entity">The source <see cref="Model.CdnVersion" /> entity.</param>
    /// <param name="dbContext">The current database context.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.CdnVersion.Files" /> property.</returns>
    public static async Task<IEnumerable<Model.CdnFile>> GetFilesAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetRelatedCollectionAsync(dbContext.CdnVersions, u => u.Files, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.CdnFile" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entry">The source <see cref="Model.CdnVersion" /> <see cref="EntityEntry{TEntity}" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.CdnVersion.Files" /> property.</returns>
    public static async Task<IEnumerable<Model.CdnFile>> GetFilesAsync(this EntityEntry<Model.CdnVersion>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Files, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.CdnFile" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entity">The source <see cref="Model.LocalFile" /> entity.</param>
    /// <param name="dbContext">The current database context.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.LocalFile.Upstream" /> property.</returns>
    public static async Task<IEnumerable<Model.CdnFile>> GetUpstreamAsync(this Model.LocalFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetRelatedCollectionAsync(dbContext.LocalFiles, u => u.Upstream, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.CdnFile" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entry">The source <see cref="Model.LocalFile" /> <see cref="EntityEntry{TEntity}" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.LocalFile.Upstream" /> property.</returns>
    public static async Task<IEnumerable<Model.CdnFile>> GetUpstreamAsync(this EntityEntry<Model.LocalFile>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Upstream, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.VersionLog" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entity">The source <see cref="Model.CdnVersion" /> entity.</param>
    /// <param name="dbContext">The current database context.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.CdnVersion.Logs" /> property.</returns>
    public static async Task<IEnumerable<Model.VersionLog>> GetLogsAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetRelatedCollectionAsync(dbContext.CdnVersions, u => u.Logs, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.VersionLog" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entry">The source <see cref="Model.CdnVersion" /> <see cref="EntityEntry{TEntity}" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.CdnVersion.Logs" /> property.</returns>
    public static async Task<IEnumerable<Model.VersionLog>> GetLogsAsync(this EntityEntry<Model.CdnVersion>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Logs, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.FileLog" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entity">The source <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.CdnFile.Logs" /> property.</returns>
    public static async Task<IEnumerable<Model.FileLog>> GetLogsAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetRelatedCollectionAsync(dbContext.CdnFiles, u => u.Logs, cancellationToken);

    /// <summary>
    /// Asynchronously gets the associated <see cref="Model.FileLog" /> entities, loading them from the database, if necessary.
    /// </summary>
    /// <param name="entry">The source <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The entities from the <see cref="Model.CdnFile.Logs" /> property.</returns>
    public static async Task<IEnumerable<Model.FileLog>> GetLogsAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Logs, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related entity for a navigation property.
    /// </summary>
    /// <param name="entry">The parent entity entry.</param>
    /// <param name="propertyExpression">The expression for the nagivation property.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <typeparam name="TEntity">The parent entity type.</typeparam>
    /// <typeparam name="TProperty">The related entity type.</typeparam>
    /// <returns>The entry related entity or <see langword="null" /> if there is no related entity.</returns>
    public static async Task<EntityEntry<TProperty>?> GetReferencedEntryAsync<TEntity, TProperty>(this EntityEntry<TEntity>? entry, Expression<Func<TEntity, TProperty?>> propertyExpression, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        if (entry is null)
            return null;
        ReferenceEntry<TEntity, TProperty> referenceEntry = entry.Reference(propertyExpression);
        if (!referenceEntry.IsLoaded && entry.ExistsInDb())
            await referenceEntry.LoadAsync(cancellationToken);
        return referenceEntry.TargetEntry;
    }

    /// <summary>
    /// Asyncrhonously loads and gets the related entity for a navigation property.
    /// </summary>
    /// <param name="entity">The parent entity object.</param>
    /// <param name="dbSet">The database context property for the parent entity's table.</param>
    /// <param name="propertyExpression">The expression for the nagivation property.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <typeparam name="TEntity">The parent entity type.</typeparam>
    /// <typeparam name="TProperty">The related entity type.</typeparam>
    /// <returns>The entry related entity or <see langword="null" /> if there is no related entity.</returns>
    public static async Task<EntityEntry<TProperty>?> GetReferencedEntryAsync<TEntity, TProperty>(this TEntity? entity, DbSet<TEntity> dbSet, Expression<Func<TEntity, TProperty?>> propertyExpression, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        if (entity is null)
            return null;
        return await dbSet.Entry(entity).GetReferencedEntryAsync(propertyExpression, cancellationToken);
    }

    /// <summary>
    /// Asyncrhonously loads and gets the related entity for a navigation property.
    /// </summary>
    /// <param name="entityEntry">The parent entity entry.</param>
    /// <param name="propertyExpression">The expression for the nagivation property.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <typeparam name="TEntity">The parent entity type.</typeparam>
    /// <typeparam name="TProperty">The related entity type.</typeparam>
    /// <returns>The related entity object or <see langword="null" /> if there is no related entity.</returns>
    public static async Task<TProperty?> GetReferencedEntityAsync<TEntity, TProperty>(this EntityEntry<TEntity>? entityEntry, Expression<Func<TEntity, TProperty?>> propertyExpression, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        if (entityEntry is null)
            return null;
        ReferenceEntry<TEntity, TProperty> referenceEntry = entityEntry.Reference(propertyExpression);
        if (!referenceEntry.IsLoaded && entityEntry.ExistsInDb())
            await referenceEntry.LoadAsync(cancellationToken);
        return referenceEntry.CurrentValue;
    }

    /// <summary>
    /// Asyncrhonously loads and gets the related entity for a navigation property.
    /// </summary>
    /// <param name="entity">The parent entity object.</param>
    /// <param name="dbSet">The database context property for the parent entity's table.</param>
    /// <param name="propertyExpression">The expression for the nagivation property.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <typeparam name="TEntity">The parent entity type.</typeparam>
    /// <typeparam name="TProperty">The related entity type.</typeparam>
    /// <returns>The related entity object or <see langword="null" /> if there is no related entity.</returns>
    public static async Task<TProperty?> GetReferencedEntityAsync<TEntity, TProperty>(this TEntity? entity, DbSet<TEntity> dbSet, Func<TEntity, TProperty?> propertyAccessor, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        if (entity is null)
            return null;
        TProperty? result = propertyAccessor(entity);
        return (result is null) ? (await dbSet.Entry(entity).GetReferencedEntryAsync(Expression.Lambda<Func<TEntity, TProperty?>>(Expression.Call(propertyAccessor.Method)), cancellationToken))?.Entity : result;
    }

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnLog" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnLog" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnLog.Cdn" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<Model.UpstreamCdn?> GetCdnAsync(this Model.CdnLog? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntityAsync(dbContext.CdnLogs, l => l.Cdn, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnLog" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnLog" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnLog.Cdn" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<EntityEntry<Model.UpstreamCdn>?> GetCdnEntryAsync(this Model.CdnLog? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntryAsync(dbContext.CdnLogs, l => l.Cdn, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnLog" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnLog" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnLog.Cdn" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<Model.UpstreamCdn?> GetCdnAsync(this EntityEntry<Model.CdnLog>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntityAsync(l => l.Cdn, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnLog" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnLog" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnLog.Cdn" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<EntityEntry<Model.UpstreamCdn>?> GetCdnEntryAsync(this EntityEntry<Model.CdnLog>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntryAsync(l => l.Cdn, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnVersion" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnVersion" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the nested loaded <see cref="Model.CdnVersion.Library" /> <c>=&gt;</c> <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" />
    /// if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<Model.UpstreamCdn?> GetCdnAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await (await entity.GetLibraryEntryAsync(dbContext, cancellationToken)).GetCdnAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnVersion" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnVersion" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnVersion.Library" /> <c>=&gt;</c> <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" />
    /// if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<EntityEntry<Model.UpstreamCdn>?> GetCdnEntryAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await (await entity.GetLibraryEntryAsync(dbContext, cancellationToken)).GetCdnEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnVersion" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnVersion" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnVersion.Library" /> <c>=&gt;</c> <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<Model.UpstreamCdn?> GetCdnAsync(this EntityEntry<Model.CdnVersion>? entry, CancellationToken cancellationToken) => await (await entry.GetLibraryEntryAsync(cancellationToken)).GetCdnAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnVersion" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnVersion" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnVersion.Library" /> <c>=&gt;</c> <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" />
    /// if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<EntityEntry<Model.UpstreamCdn>?> GetCdnEntryAsync(this EntityEntry<Model.CdnVersion>? entry, CancellationToken cancellationToken)
        => await (await entry.GetLibraryEntryAsync(cancellationToken)).GetCdnEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> <c>=&gt;</c> <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" />
    /// if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<Model.UpstreamCdn?> GetCdnAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await (await entity.GetLibraryEntryAsync(dbContext, cancellationToken)).GetCdnAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> <c>=&gt;</c> <see cref="Model.CdnLibrary.Cdn" /> property,
    /// which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<EntityEntry<Model.UpstreamCdn>?> GetCdnEntryAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await (await entity.GetLibraryEntryAsync(dbContext, cancellationToken)).GetCdnEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> <c>=&gt;</c> <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" />
    /// if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<Model.UpstreamCdn?> GetCdnAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken) => await (await entry.GetLibraryEntryAsync(cancellationToken)).GetCdnAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> <c>=&gt;</c> <see cref="Model.CdnLibrary.Cdn" /> property,
    /// which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<EntityEntry<Model.UpstreamCdn>?> GetCdnEntryAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken)
        => await (await entry.GetLibraryEntryAsync(cancellationToken)).GetCdnEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnLibrary" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnLibrary" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<Model.UpstreamCdn?> GetCdnAsync(this Model.CdnLibrary? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntityAsync(dbContext.CdnLibraries, l => l.Cdn, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnLibrary" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnLibrary" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<EntityEntry<Model.UpstreamCdn>?> GetCdnEntryAsync(this Model.CdnLibrary? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntryAsync(dbContext.CdnLibraries, l => l.Cdn, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnLibrary" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnLibrary" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<Model.UpstreamCdn?> GetCdnAsync(this EntityEntry<Model.CdnLibrary>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntityAsync(l => l.Cdn, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnLibrary" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnLibrary" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<EntityEntry<Model.UpstreamCdn>?> GetCdnEntryAsync(this EntityEntry<Model.CdnLibrary>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntryAsync(l => l.Cdn, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.CdnLibrary" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnLibrary" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnLibrary.Local" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<Model.LocalLibrary?> GetLocalAsync(this Model.CdnLibrary? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntityAsync(dbContext.CdnLibraries, l => l.Local, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.CdnLibrary" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnLibrary" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnLibrary.Local" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<EntityEntry<Model.LocalLibrary>?> GetLocalEntryAsync(this Model.CdnLibrary? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntryAsync(dbContext.CdnLibraries, l => l.Local, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.CdnLibrary" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnLibrary" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnLibrary.Local" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<Model.LocalLibrary?> GetLocalAsync(this EntityEntry<Model.CdnLibrary>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntityAsync(l => l.Local, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.CdnLibrary" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnLibrary" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnLibrary.Local" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<EntityEntry<Model.LocalLibrary>?> GetLocalEntryAsync(this EntityEntry<Model.CdnLibrary>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntryAsync(l => l.Local, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.CdnVersion" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnVersion" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property <see cref="Model.CdnVersion.Local" /> property, which may be <see langword="null" />
    /// if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<Model.LocalLibrary?> GetLocalLibraryAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await (await entity.GetLocalEntryAsync(dbContext, cancellationToken)).GetLibraryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.CdnVersion" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnVersion" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" />
    /// if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<EntityEntry<Model.LocalLibrary>?> GetLocalLibraryEntryAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await (await entity.GetLocalEntryAsync(dbContext, cancellationToken)).GetLibraryEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.CdnVersion" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnVersion" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<Model.LocalLibrary?> GetLocalLibraryAsync(this EntityEntry<Model.CdnVersion>? entry, CancellationToken cancellationToken) => await (await entry.GetLocalEntryAsync(cancellationToken)).GetLibraryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.CdnVersion" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnVersion" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" />
    /// if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<EntityEntry<Model.LocalLibrary>?> GetLocalLibraryEntryAsync(this EntityEntry<Model.CdnVersion>? entry, CancellationToken cancellationToken)
        => await (await entry.GetLocalEntryAsync(cancellationToken)).GetLibraryEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Local" /> <c>=&gt;</c> <see cref="Model.LocalFile.Version" /> <c>=&gt;</c> <see cref="Model.LocalVersion.Library" /> property, which may be <see langword="null" />
    /// if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<Model.LocalLibrary?> GetLocalLibraryAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await (await entity.GetLocalEntryAsync(dbContext, cancellationToken)).GetLibraryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Local" /> <c>=&gt;</c> <see cref="Model.LocalFile.Version" /> <c>=&gt;</c> <see cref="Model.LocalVersion.Library" /> property,
    /// which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<EntityEntry<Model.LocalLibrary>?> GetLocalLibraryEntryAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await (await entity.GetLocalEntryAsync(dbContext, cancellationToken)).GetLibraryEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Local" /> <c>=&gt;</c> <see cref="Model.LocalFile.Version" /> <c>=&gt;</c> <see cref="Model.LocalVersion.Library" /> property, which may be <see langword="null" />
    /// if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<Model.LocalLibrary?> GetLocalLibraryAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken) => await (await entry.GetLocalEntryAsync(cancellationToken)).GetLibraryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Local" /> <c>=&gt;</c> <see cref="Model.LocalFile.Version" /> <c>=&gt;</c> <see cref="Model.LocalVersion.Library" /> property,
    /// which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<EntityEntry<Model.LocalLibrary>?> GetLocalLibraryEntryAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken)
        => await (await entry.GetLocalEntryAsync(cancellationToken)).GetLibraryEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.LocalVersion" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.LocalVersion" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.LocalVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<Model.LocalLibrary?> GetLibraryAsync(this Model.LocalVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntityAsync(dbContext.LocalVersions, l => l.Library, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.LocalVersion" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.LocalVersion" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.LocalVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<EntityEntry<Model.LocalLibrary>?> GetLibraryEntryAsync(this Model.LocalVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntryAsync(dbContext.LocalVersions, l => l.Library, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.LocalVersion" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.LocalVersion" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.LocalVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<Model.LocalLibrary?> GetLibraryAsync(this EntityEntry<Model.LocalVersion>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntityAsync(l => l.Library, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.LocalVersion" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.LocalVersion" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.LocalVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<EntityEntry<Model.LocalLibrary>?> GetLibraryEntryAsync(this EntityEntry<Model.LocalVersion>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntryAsync(l => l.Library, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.LibraryLog" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.LibraryLog" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.LibraryLog.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<Model.CdnLibrary?> GetLibraryAsync(this Model.LibraryLog? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntityAsync(dbContext.LibraryLogs, l => l.Library, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.LibraryLog" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.LibraryLog" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.LibraryLog.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<EntityEntry<Model.CdnLibrary>?> GetLibraryEntryAsync(this Model.LibraryLog? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntryAsync(dbContext.LibraryLogs, l => l.Library, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.LibraryLog" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.LibraryLog" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.LibraryLog.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<Model.CdnLibrary?> GetLibraryAsync(this EntityEntry<Model.LibraryLog>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntityAsync(l => l.Library, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.LibraryLog" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.LibraryLog" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.LibraryLog.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<EntityEntry<Model.CdnLibrary>?> GetLibraryEntryAsync(this EntityEntry<Model.LibraryLog>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntryAsync(l => l.Library, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<Model.CdnLibrary?> GetLibraryAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await (await entity.GetVersionEntryAsync(dbContext, cancellationToken)).GetLibraryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" />
    /// if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<EntityEntry<Model.CdnLibrary>?> GetLibraryEntryAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await (await entity.GetVersionEntryAsync(dbContext, cancellationToken)).GetLibraryEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<Model.CdnLibrary?> GetLibraryAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken) => await (await entry.GetVersionEntryAsync(cancellationToken)).GetLibraryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property which may be <see langword="null" />
    /// if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<EntityEntry<Model.CdnLibrary>?> GetLibraryEntryAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken)
        => await (await entry.GetVersionEntryAsync(cancellationToken)).GetLibraryEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<Model.LocalLibrary?> GetLibraryAsync(this Model.LocalFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await (await entity.GetVersionEntryAsync(dbContext, cancellationToken)).GetLibraryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" />
    /// if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<EntityEntry<Model.LocalLibrary>?> GetLibraryEntryAsync(this Model.LocalFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await (await entity.GetVersionEntryAsync(dbContext, cancellationToken)).GetLibraryEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<Model.LocalLibrary?> GetLibraryAsync(this EntityEntry<Model.LocalFile>? entry, CancellationToken cancellationToken) => await (await entry.GetVersionEntryAsync(cancellationToken)).GetLibraryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property which may be <see langword="null" />
    /// if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<EntityEntry<Model.LocalLibrary>?> GetLibraryEntryAsync(this EntityEntry<Model.LocalFile>? entry, CancellationToken cancellationToken)
        => await (await entry.GetVersionEntryAsync(cancellationToken)).GetLibraryEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.CdnVersion" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnVersion" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<Model.CdnLibrary?> GetLibraryAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntityAsync(dbContext.CdnVersions, l => l.Library, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.CdnVersion" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnVersion" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<EntityEntry<Model.CdnLibrary>?> GetLibraryEntryAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntryAsync(dbContext.CdnVersions, l => l.Library, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.CdnVersion" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnVersion" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<Model.CdnLibrary?> GetLibraryAsync(this EntityEntry<Model.CdnVersion>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntityAsync(l => l.Library, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.CdnVersion" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnVersion" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<EntityEntry<Model.CdnLibrary>?> GetLibraryEntryAsync(this EntityEntry<Model.CdnVersion>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntryAsync(l => l.Library, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalVersion" /> for the specified <see cref="Model.CdnVersion" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnVersion" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnVersion.Local" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalVersion" />.</returns>
    public static async Task<Model.LocalVersion?> GetLocalAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntityAsync(dbContext.CdnVersions, l => l.Local, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalVersion" /> for the specified <see cref="Model.CdnVersion" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnVersion" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnVersion.Local" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalVersion" />.</returns>
    public static async Task<EntityEntry<Model.LocalVersion>?> GetLocalEntryAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntryAsync(dbContext.CdnVersions, l => l.Local, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalVersion" /> for the specified <see cref="Model.CdnVersion" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnVersion" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnVersion.Local" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalVersion" />.</returns>
    public static async Task<Model.LocalVersion?> GetLocalAsync(this EntityEntry<Model.CdnVersion>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntityAsync(l => l.Local, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalVersion" /> for the specified <see cref="Model.CdnVersion" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnVersion" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnVersion.Local" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalVersion" />.</returns>
    public static async Task<EntityEntry<Model.LocalVersion>?> GetLocalEntryAsync(this EntityEntry<Model.CdnVersion>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntryAsync(l => l.Local, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalVersion" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Local" /> <c>=&gt;</c> <see cref="Model.LocalFile.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalVersion" />.</returns>
    public static async Task<Model.LocalVersion?> GetLocalVersionAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await (await entity.GetLocalEntryAsync(dbContext, cancellationToken)).GetVersionAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalVersion" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Local" /> <c>=&gt;</c> <see cref="Model.LocalFile.Version" /> property, which may be <see langword="null" />
    /// if there is no related <see cref="Model.LocalVersion" />.</returns>
    public static async Task<EntityEntry<Model.LocalVersion>?> GetLocalVersionEntryAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await (await entity.GetLocalEntryAsync(dbContext, cancellationToken)).GetVersionEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalVersion" /> for the specified <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Local" /> <c>=&gt;</c> <see cref="Model.LocalFile.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalVersion" />.</returns>
    public static async Task<Model.LocalVersion?> GetLocalVersionAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken) => await (await entry.GetLocalEntryAsync(cancellationToken)).GetVersionAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalVersion" /> for the specified <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Local" /> <c>=&gt;</c> <see cref="Model.LocalFile.Version" /> property, which may be <see langword="null" />
    /// if there is no related <see cref="Model.LocalVersion" />.</returns>
    public static async Task<EntityEntry<Model.LocalVersion>?> GetLocalVersionEntryAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken)
        => await (await entry.GetLocalEntryAsync(cancellationToken)).GetVersionEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalVersion" /> for the specified <see cref="Model.LocalFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.LocalFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.LocalFile.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalVersion" />.</returns>
    public static async Task<Model.LocalVersion?> GetVersionAsync(this Model.LocalFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntityAsync(dbContext.LocalFiles, l => l.Version, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalVersion" /> for the specified <see cref="Model.LocalFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.LocalFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.LocalFile.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalVersion" />.</returns>
    public static async Task<EntityEntry<Model.LocalVersion>?> GetVersionEntryAsync(this Model.LocalFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntryAsync(dbContext.LocalFiles, l => l.Version, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalVersion" /> for the specified <see cref="Model.LocalFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.LocalFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.LocalFile.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalVersion" />.</returns>
    public static async Task<Model.LocalVersion?> GetVersionAsync(this EntityEntry<Model.LocalFile>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntityAsync(l => l.Version, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalVersion" /> for the specified <see cref="Model.LocalFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.LocalFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.LocalFile.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalVersion" />.</returns>
    public static async Task<EntityEntry<Model.LocalVersion>?> GetVersionEntryAsync(this EntityEntry<Model.LocalFile>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntryAsync(l => l.Version, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.CdnVersion" /> for the specified <see cref="Model.VersionLog" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.VersionLog" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.VersionLog.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnVersion" />.</returns>
    public static async Task<Model.CdnVersion?> GetVersionAsync(this Model.VersionLog? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntityAsync(dbContext.VersionLogs, l => l.Version, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnVersion" /> for the specified <see cref="Model.VersionLog" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.VersionLog" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.VersionLog.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnVersion" />.</returns>
    public static async Task<EntityEntry<Model.CdnVersion>?> GetVersionEntryAsync(this Model.VersionLog? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntryAsync(dbContext.VersionLogs, l => l.Version, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.CdnVersion" /> for the specified <see cref="Model.VersionLog" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.VersionLog" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.VersionLog.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnVersion" />.</returns>
    public static async Task<Model.CdnVersion?> GetVersionAsync(this EntityEntry<Model.VersionLog>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntityAsync(l => l.Version, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnVersion" /> for the specified <see cref="Model.VersionLog" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.VersionLog" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.VersionLog.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnVersion" />.</returns>
    public static async Task<EntityEntry<Model.CdnVersion>?> GetVersionEntryAsync(this EntityEntry<Model.VersionLog>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntryAsync(l => l.Version, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.CdnVersion" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnVersion" />.</returns>
    public static async Task<Model.CdnVersion?> GetVersionAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntityAsync(dbContext.CdnFiles, l => l.Version, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnVersion" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnVersion" />.</returns>
    public static async Task<EntityEntry<Model.CdnVersion>?> GetVersionEntryAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntryAsync(dbContext.CdnFiles, l => l.Version, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.CdnVersion" /> for the specified <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnVersion" />.</returns>
    public static async Task<Model.CdnVersion?> GetVersionAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntityAsync(l => l.Version, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnVersion" /> for the specified <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnVersion" />.</returns>
    public static async Task<EntityEntry<Model.CdnVersion>?> GetVersionEntryAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntryAsync(l => l.Version, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalFile" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Local" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalFile" />.</returns>
    public static async Task<Model.LocalFile?> GetLocalAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntityAsync(dbContext.CdnFiles, l => l.Local, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalFile" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Local" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalFile" />.</returns>
    public static async Task<EntityEntry<Model.LocalFile>?> GetLocalEntryAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntryAsync(dbContext.CdnFiles, l => l.Local, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalFile" /> for the specified <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Local" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalFile" />.</returns>
    public static async Task<Model.LocalFile?> GetLocalAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntityAsync(l => l.Local, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalFile" /> for the specified <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Local" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalFile" />.</returns>
    public static async Task<EntityEntry<Model.LocalFile>?> GetLocalEntryAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntryAsync(l => l.Local, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.CdnFile" /> for the specified <see cref="Model.FileLog" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.FileLog" /> entity.</param>*/1`
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.FileLog.File" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnFile" />.</returns>
    public static async Task<Model.CdnFile?> GetFileAsync(this Model.FileLog? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntityAsync(dbContext.FileLogs, l => l.File, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnFile" /> for the specified <see cref="Model.FileLog" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.FileLog" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.FileLog.File" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnFile" />.</returns>
    public static async Task<EntityEntry<Model.CdnFile>?> GetFileEntryAsync(this Model.FileLog? entity, Services.ContentDb dbContext, CancellationToken cancellationToken)
        => await entity.GetReferencedEntryAsync(dbContext.FileLogs, l => l.File, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.CdnFile" /> for the specified <see cref="Model.FileLog" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.FileLog" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.FileLog.File" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnFile" />.</returns>
    public static async Task<Model.CdnFile?> GetFileAsync(this EntityEntry<Model.FileLog>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntityAsync(l => l.File, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnFile" /> for the specified <see cref="Model.FileLog" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.FileLog" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.FileLog.File" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnFile" />.</returns>
    public static async Task<EntityEntry<Model.CdnFile>?> GetFileEntryAsync(this EntityEntry<Model.FileLog>? entry, CancellationToken cancellationToken) => await entry.GetReferencedEntryAsync(l => l.File, cancellationToken);

    /// <summary>
    /// Gets the full directory name for files associated with a specific <see cref="Model.LocalVersion" />.
    /// </summary>
    /// <param name="entity">he source <see cref="Model.LocalVersion" /> entity.</param>
    /// <param name="contentDirectory">The root subdirectory for downloaded CDN content.</param>
    /// <param name="dbContext">The current database context.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The full directory name for files associated with a specific <see cref="Model.LocalVersion" /> or <see langword="null" /> if any of the associated parameters or entities are invalid.</returns>
    public static async Task<string?> GetDirNameAsync(this Model.LocalVersion? entity, DirectoryInfo contentDirectory, Services.ContentDb dbContext, CancellationToken cancellationToken)
    {
        string dirName;
        if (entity is null || contentDirectory is null || (dirName = entity.DirName).Length == 0)
            return null;
        Model.LocalLibrary? library = await entity.GetLibraryAsync(dbContext, cancellationToken);
        if (library is null || library.DirName.Length == 0)
            return null;
        return Path.Combine(contentDirectory.FullName, library.DirName, dirName);
    }
    
    /// <summary>
    /// Gets the full directory name for files associated with a specific <see cref="Model.LocalVersion" />.
    /// </summary>
    /// <param name="entry">The source <see cref="Model.LocalVersion" /> <see cref="EntityEntry{TEntity}" />.</param>
    /// <param name="contentDirectory">The root subdirectory for downloaded CDN content.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The full directory name for files associated with a specific <see cref="Model.LocalVersion" /> or <see langword="null" /> if any of the associated parameters or entities are invalid.</returns>
    public static async Task<string?> GetDirNameAsync(this EntityEntry<Model.LocalVersion>? entry, DirectoryInfo contentDirectory, CancellationToken cancellationToken)
    {
        string dirName;
        if (entry is null || contentDirectory is null || (dirName = entry.Entity.DirName).Length == 0)
            return null;
        Model.LocalLibrary? library = await entry.GetLibraryAsync(cancellationToken);
        if (library is null || library.DirName.Length == 0)
            return null;
        return Path.Combine(contentDirectory.FullName, library.DirName, dirName);
    }
    
    /// <summary>
    /// Gets the full directory name for files associated with a specific <see cref="Model.CdnVersion" />.
    /// </summary>
    /// <param name="entry">The source <see cref="Model.CdnVersion" /> <see cref="EntityEntry{TEntity}" />.</param>
    /// <param name="contentDirectory">The root subdirectory for downloaded CDN content.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The full directory name for files associated with a specific <see cref="Model.CdnVersion" /> or <see langword="null" /> if any of the associated parameters or entities are invalid.</returns>
    public static async Task<string?> GetDirNameAsync(this EntityEntry<Model.CdnVersion>? entry, DirectoryInfo contentDirectory, CancellationToken cancellationToken)
    {
        if (entry is null || contentDirectory is null)
            return null;
        string? dirName = await (await entry.GetLocalEntryAsync(cancellationToken)).GetDirNameAsync(contentDirectory, cancellationToken);
        if (dirName is null)
            return null;
        Model.UpstreamCdn? cdn = await entry.GetCdnAsync(cancellationToken);
        if (cdn is null || cdn.DirName.Length == 0)
            return null;
        return Path.Combine(dirName, cdn.DirName);
    }
    
    /// <summary>
    /// Creates a <see cref="FileProperties" /> record from a <see cref="Model.LocalFile" />.
    /// </summary>
    /// <param name="entry">The source <see cref="Model.LocalFile" /> <see cref="EntityEntry{TEntity}" />.</param>
    /// <param name="contentDirectory">The root subdirectory for downloaded CDN content.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="FileProperties" /> record or <see langword="null" /> if the <paramref name="entry" /> or <paramref name="contentDirectory" /> is null or does not have a valid <see cref="Model.LocalFile.FileName" />.</returns>
    public static async Task<FileProperties?> GetFilePropertiesAsync(this EntityEntry<Model.LocalFile>? entry, DirectoryInfo contentDirectory, CancellationToken cancellationToken)
    {
        string fileName;
        if (entry is null || contentDirectory is null || (fileName = entry.Entity.FileName).Length == 0)
            return null;
        string? dirName = await (await entry.GetVersionEntryAsync(cancellationToken)).GetDirNameAsync(contentDirectory, cancellationToken);
        if (dirName is null)
            return null;
        Model.LocalFile entity = entry.Entity;
        return new(Name: entity.Name, SRI: entity.SRI, ContentType: entity.ContentType, Encoding: entity.Encoding, DirName: dirName, FileName: fileName, Order: entity.Order);
    }
    
    /// <summary>
    /// Creates a <see cref="FileProperties" /> record from a <see cref="Model.CdnFile" /> and its associated <see cref="Model.LocalFile" />.
    /// </summary>
    /// <param name="entry">The source <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.</param>
    /// <param name="contentDirectory">The root subdirectory for downloaded CDN content.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="FileProperties" /> record or <see langword="null" /> if the <paramref name="entry" /> or <paramref name="contentDirectory" /> is null or does not have a valid <see cref="Model.LocalFile.FileName" />.</returns>
    public static async Task<FileProperties?> GetFilePropertiesAsync(this EntityEntry<Model.CdnFile>? entry, DirectoryInfo contentDirectory, CancellationToken cancellationToken)
    {
        if (entry is null || contentDirectory is null)
            return null;
        string? fileName = entry.Entity.FileName;
        if (fileName is null)
            return await (await entry.GetLocalEntryAsync(cancellationToken)).GetFilePropertiesAsync(contentDirectory, cancellationToken);

        string? dirName = await (await entry.GetVersionEntryAsync(cancellationToken)).GetDirNameAsync(contentDirectory, cancellationToken);
        if (dirName is null)
            return null;
        Model.LocalFile? local = await entry.GetLocalAsync(cancellationToken);
        if (local is null)
            return null;
        Model.CdnFile entity = entry.Entity;
        return new(Name: local.Name, SRI: entity.SRI ?? local.SRI, ContentType: local.ContentType, Encoding: entity.Encoding ?? local.Encoding, DirName: dirName, FileName: fileName, Order: local.Order);
    }
}