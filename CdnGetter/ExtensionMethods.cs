using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace CdnGetter;

public static class ExtensionMethods
{
    public static readonly Regex NonNormalizedWhiteSpaceRegex = new(@"( |(?! ))[\r\n\s]+", RegexOptions.Compiled);

    public static readonly Regex LineBreakRegex = new(@"\r?\n|\n", RegexOptions.Compiled);

    public static readonly ValueConverter<JsonNode?, string?> JsonValueConverter = new(
        v => (v == null) ? null : v.ToJsonString(JsonSerializerOptions.Default),
        s => s.ConvertToJsonNode()
    );

    public static Uri? ForceCreateUri(string? uriString)
    {
        if (uriString is null)
            return null;
        if (uriString.Length > 0 && Uri.TryCreate(uriString, UriKind.Absolute, out Uri? uri))
            return uri;
        if (Uri.IsWellFormedUriString(uriString, UriKind.Relative))
            return new Uri(uriString, UriKind.Relative);
        try
        {
            int i = uriString.IndexOf('#');
            UriBuilder ub = new() { Host = null, Scheme = null };
            if (i > 0)
            {
                ub.Fragment = uriString[(i + 1)..];
                string path = uriString[..i];
                if ((i = path.IndexOf('?')) > 0)
                {
                    ub.Query = path[(i + 1)..];
                    ub.Path = path[..i];
                }
                else
                    ub.Path = path;
            }
            else if ((i = uriString.IndexOf('?')) > 0)
            {
                ub.Query = uriString[(i + 1)..];
                ub.Path = uriString[..i];
            }
            else
                ub.Path = uriString;
            return new Uri(ub.ToString(), UriKind.Relative);
        }
        catch { return new Uri(Uri.EscapeDataString(uriString), UriKind.Relative); }
    }
    
    public static readonly ValueConverter<Uri?, string?> UriConverter = new(
        u => (u == null) ? null : u.IsAbsoluteUri ? u.AbsoluteUri : u.OriginalString,
        s => ForceCreateUri(s)
    );

    public static Model.ErrorLevel ToErrorLevel(this LogLevel level) => level switch
    {
        LogLevel.Warning => Model.ErrorLevel.Warning,
        LogLevel.Error => Model.ErrorLevel.Error,
        LogLevel.Critical => Model.ErrorLevel.Critical,
        _ => Model.ErrorLevel.Information,
    };

    private static readonly Model.ErrorLevel[] _allErrorLevels = Enum.GetValues<Model.ErrorLevel>();
    public static readonly ValueConverter<Model.ErrorLevel, byte> ErrorLevelConverter = new(
        a => (byte)a,
        b => _allErrorLevels.FirstOrDefault(a => (byte)a == b)
    );

    private static readonly Model.LibraryAction[] _allLibraryActions = Enum.GetValues<Model.LibraryAction>();
    public static readonly ValueConverter<Model.LibraryAction, byte> LibraryActionConverter = new(
        a => (byte)a,
        b => _allLibraryActions.FirstOrDefault(a => (byte)a == b)
    );

    public static string? ConvertFromJsonNode(this JsonNode? value) => value?.ToJsonString(JsonSerializerOptions.Default);

    public static JsonNode? ConvertToJsonNode(this string? value)
    {
        string? normalized = value.ToTrimmedOrNullIfEmpty();
        if (normalized is null)
            return null;
        try { return JsonNode.Parse(normalized); }
        catch
        {
            try { return JsonValue.Create(value); }
            catch { return null; }
        }
    }
    
    public static bool TryConvertToJsonNode(this string? value, out JsonNode? result)
    {
        string? normalized = value.ToTrimmedOrNullIfEmpty();
        if (normalized is not null)
            try
            {
                result = JsonNode.Parse(normalized)!;
                return true;
            }
            catch
            {
                try
                {
                    result = JsonValue.Create(value!)!;
                    return true;
                }
                catch { /* Okay to ignore */ }
            }
        result = null;
        return false;
    }
    
    public static JsonObject? ToJsonObject(this Dictionary<string, JsonElement>? source)
    {
        if (source is null)
            return null;
        JsonObject result = new();
        foreach (KeyValuePair<string, JsonElement> kvp in source)
            switch (kvp.Value.ValueKind)
            {
                case JsonValueKind.Object:
                    result.Add(kvp.Key, JsonObject.Create(kvp.Value));
                    break;
                case JsonValueKind.Array:
                    result.Add(kvp.Key, JsonArray.Create(kvp.Value));
                    break;
                default:
                    result.Add(kvp.Key, JsonValue.Create(kvp.Value));
                    break;
            }
        return result;
    }

    public static Guid EnsureGuid(this ref Guid? target, object syncRoot)
    {
        Guid? guid;
        Monitor.Enter(syncRoot);
        try
        {
            guid = target;
            if (!guid.HasValue)
                target = guid = Guid.NewGuid();
        }
        finally { Monitor.Exit(syncRoot); }
        return guid.Value;
    }

    public static T[] EmptyIfNull<T>(this T[]? source) { return (source is null) ? Array.Empty<T>() : source; }

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) { return (source is null) ? Enumerable.Empty<T>() : source; }

    public static T[]? NullIfEmpty<T>(this T[]? source) { return (source is null || source.Length == 0) ? null : source; }

    public static IEnumerable<T> NonNullValues<T>(this IEnumerable<T?>? source) where T : class
    {
        if (source is null)
            return Enumerable.Empty<T>();
        return source.Where(t => t is not null)!;
    }
    
    public static IEnumerable<string> TrimmedNotEmptyValues(this IEnumerable<string?>? source)
    {
        if (source is null)
            return Enumerable.Empty<string>();
        return source.Select(ToTrimmedOrNullIfEmpty).Where(t => t is not null)!;
    }
    
    public static string[] SplitLines(this string? value)
    {
        if (value is null)
            return Array.Empty<string>();
        return LineBreakRegex.Split(value);
    }

    public static bool IsWsNormalizedNotEmpty(this string? value, [NotNullWhen(true)] out string? wsNormalized)
    {
        return (wsNormalized = value.ToWsNormalizedOrNullIfEmpty()) is not null;
    }
    
    public static bool IsTrimmedNotEmpty(this string? value, [NotNullWhen(true)] out string? wsNormalized)
    {
        return (wsNormalized = value.ToTrimmedOrNullIfEmpty()) is not null;
    }
    
    public static string ToWsNormalizedOrEmptyIfNull(this string? value)
    {
        return (value is not null && (value = value.Trim()).Length > 0) ? NonNormalizedWhiteSpaceRegex.Replace(value, " ") : "";
    }

    public static string? ToWsNormalizedOrNullIfEmpty(this string? value)
    {
        return (value is not null && (value = value.Trim()).Length > 0) ? NonNormalizedWhiteSpaceRegex.Replace(value, " ") : null;
    }

    public static string ToWsNormalizedOrDefaultIfEmpty(this string? value, Func<string> getDefaultValue)
    {
        return (value is not null && (value = value.Trim()).Length > 0) ? NonNormalizedWhiteSpaceRegex.Replace(value, " ") : getDefaultValue();
    }

    public static string ToWsNormalizedOrDefaultIfEmpty(this string? value, string defaultValue)
    {
        return (value is not null && (value = value.Trim()).Length > 0) ? NonNormalizedWhiteSpaceRegex.Replace(value, " ") : defaultValue;
    }

    public static string ToTrimmedOrEmptyIfNull(this string? value) { return (value is null) ? "" : value.Trim(); }

    public static string? ToTrimmedOrNullIfEmpty(this string? value) { return (value is not null && (value = value.Trim()).Length > 0) ? value : null; }

    public static string ToTrimmedOrDefaultIfEmpty(this string? value, Func<string> getDefault) { return (value is not null && (value = value.Trim()).Length > 0) ? value : getDefault(); }

    public static string ToTrimmedOrDefaultIfEmpty(this string? value, string defaultValue) { return (value is not null && (value = value.Trim()).Length > 0) ? value : defaultValue; }

    public static void SetNavigation<T>(this Guid newValue, object syncRoot, Func<T, Guid> keyAcessor, ref Guid foreignKey, ref T? target)
        where T : class
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (target is not null)
            {
                if (newValue.Equals(keyAcessor(target)))
                    return;
                target = null;
            }
            foreignKey = newValue;
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static void SetNavigation<T>(this Guid newValue1, Guid newValue2, object syncRoot, Func<T, (Guid, Guid)> keyAcessor, ref Guid foreignKey1, ref Guid foreignKey2, ref T? target)
        where T : class
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (target is not null)
            {
                (Guid fk1, Guid fk2) = keyAcessor(target);
                if (newValue1.Equals(fk1) && newValue2.Equals(fk2))
                    return;
                target = null;
            }
            foreignKey1 = newValue1;
            foreignKey2 = newValue2;
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static void SetNavigation<T>(this Guid newValue1, Guid newValue2, Guid newValue3, object syncRoot, Func<T, (Guid, Guid, Guid)> keyAcessor, ref Guid foreignKey1, ref Guid foreignKey2, ref Guid foreignKey3, ref T? target)
        where T : class
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (target is not null)
            {
                (Guid fk1, Guid fk2, Guid fk3) = keyAcessor(target);
                if (newValue1.Equals(fk1) && newValue2.Equals(fk2) && newValue3.Equals(fk3))
                    return;
                target = null;
            }
            foreignKey1 = newValue1;
            foreignKey2 = newValue2;
            foreignKey3 = newValue3;
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static void SetNavigation<T>(this Guid newValue1, Guid newValue2, Guid newValue3, Guid newValue4, object syncRoot, Func<T, (Guid, Guid, Guid, Guid)> keyAcessor, ref Guid foreignKey1, ref Guid foreignKey2, ref Guid foreignKey3, ref Guid foreignKey4, ref T? target)
        where T : class
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (target is not null)
            {
                (Guid fk1, Guid fk2, Guid fk3, Guid fk4) = keyAcessor(target);
                if (newValue1.Equals(fk1) && newValue2.Equals(fk2) && newValue3.Equals(fk3) && newValue4.Equals(fk4))
                    return;
                target = null;
            }
            foreignKey1 = newValue1;
            foreignKey2 = newValue2;
            foreignKey3 = newValue3;
            foreignKey4 = newValue4;
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static void SetNavigation<T>(this T? newValue, object syncRoot, Func<T, Guid> keyAcessor, ref Guid foreignKey, ref T? target)
        where T : class
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (newValue is null)
                target = null;
            else if (target is null || ReferenceEquals(newValue, target))
                foreignKey = keyAcessor(target = newValue);
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static void SetNavigation<T>(this T? newValue, object syncRoot, Func<T, (Guid, Guid)> keyAcessor, ref Guid foreignKey1, ref Guid foreignKey2, ref T? target)
        where T : class
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (newValue is null)
                target = null;
            else if (target is null || !ReferenceEquals(newValue, target))
                (foreignKey1, foreignKey2) = keyAcessor(target = newValue);
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static void SetNavigation<T>(this T? newValue, object syncRoot, Func<T, (Guid, Guid, Guid)> keyAcessor, ref Guid foreignKey1, ref Guid foreignKey2, ref Guid foreignKey3, ref T? target)
        where T : class
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (newValue is null)
                target = null;
            else if (target is null || !ReferenceEquals(newValue, target))
                (foreignKey1, foreignKey2, foreignKey3) = keyAcessor(target = newValue);
        }
        finally { Monitor.Exit(syncRoot); }
    }

    public static void SetNavigation<T>(this T? newValue, object syncRoot, Func<T, (Guid, Guid, Guid, Guid)> keyAcessor, ref Guid foreignKey1, ref Guid foreignKey2, ref Guid foreignKey3, ref Guid foreignKey4, ref T? target)
        where T : class
    {
        Monitor.Enter(syncRoot);
        try
        {
            if (newValue is null)
                target = null;
            else if (target is null || !ReferenceEquals(newValue, target))
                (foreignKey1, foreignKey2, foreignKey3, foreignKey4) = keyAcessor(target = newValue);
        }
        finally { Monitor.Exit(syncRoot); }
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
        return (await dbSet.Entry(entity).GetRelatedCollectionAsync(propertyExpression, cancellationToken));
    }

    public static async Task<IEnumerable<Model.CdnLibrary>> GetLibrariesAsync(this Model.UpstreamCdn? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetRelatedCollectionAsync(dbContext.UpstreamCdns, u => u.Libraries, cancellationToken);

    public static async Task<IEnumerable<Model.CdnLibrary>> GetLibrariesAsync(this EntityEntry<Model.UpstreamCdn>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Libraries, cancellationToken);

    public static async Task<IEnumerable<Model.CdnLibrary>> GetUpstreamAsync(this Model.LocalLibrary? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetRelatedCollectionAsync(dbContext.LocalLibraries, u => u.Upstream, cancellationToken);

    public static async Task<IEnumerable<Model.CdnLibrary>> GetUpstreamAsync(this EntityEntry<Model.LocalLibrary>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Upstream, cancellationToken);

    public static async Task<IEnumerable<Model.CdnLog>> GetLogsAsync(this Model.UpstreamCdn? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetRelatedCollectionAsync(dbContext.UpstreamCdns, u => u.Logs, cancellationToken);

    public static async Task<IEnumerable<Model.CdnLog>> GetLogsAsync(this EntityEntry<Model.UpstreamCdn>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Logs, cancellationToken);

    public static async Task<IEnumerable<Model.CdnVersion>> GetVersionsAsync(this Model.CdnLibrary? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetRelatedCollectionAsync(dbContext.CdnLibraries, u => u.Versions, cancellationToken);

    public static async Task<IEnumerable<Model.CdnVersion>> GetVersionsAsync(this EntityEntry<Model.CdnLibrary>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Versions, cancellationToken);

    public static async Task<IEnumerable<Model.CdnVersion>> GetUpstreamAsync(this Model.LocalVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetRelatedCollectionAsync(dbContext.LocalVersions, u => u.Upstream, cancellationToken);

    public static async Task<IEnumerable<Model.CdnVersion>> GetUpstreamAsync(this EntityEntry<Model.LocalVersion>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Upstream, cancellationToken);

    public static async Task<IEnumerable<Model.LibraryLog>> GetLogsAsync(this Model.CdnLibrary? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetRelatedCollectionAsync(dbContext.CdnLibraries, u => u.Logs, cancellationToken);

    public static async Task<IEnumerable<Model.LibraryLog>> GetLogsAsync(this EntityEntry<Model.CdnLibrary>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Logs, cancellationToken);

    public static async Task<IEnumerable<Model.CdnFile>> GetFilesAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetRelatedCollectionAsync(dbContext.CdnVersions, u => u.Files, cancellationToken);

    public static async Task<IEnumerable<Model.CdnFile>> GetFilesAsync(this EntityEntry<Model.CdnVersion>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Files, cancellationToken);

    public static async Task<IEnumerable<Model.CdnFile>> GetUpstreamAsync(this Model.LocalFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetRelatedCollectionAsync(dbContext.LocalFiles, u => u.Upstream, cancellationToken);

    public static async Task<IEnumerable<Model.CdnFile>> GetUpstreamAsync(this EntityEntry<Model.LocalFile>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Upstream, cancellationToken);

    public static async Task<IEnumerable<Model.VersionLog>> GetLogsAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetRelatedCollectionAsync(dbContext.CdnVersions, u => u.Logs, cancellationToken);

    public static async Task<IEnumerable<Model.VersionLog>> GetLogsAsync(this EntityEntry<Model.CdnVersion>? entry, CancellationToken cancellationToken) => await entry.GetRelatedCollectionAsync(u => u.Logs, cancellationToken);

    public static async Task<IEnumerable<Model.FileLog>> GetLogsAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetRelatedCollectionAsync(dbContext.CdnFiles, u => u.Logs, cancellationToken);

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
    public static async Task<EntityEntry<Model.UpstreamCdn>?> GetCdnEntryAsync(this Model.CdnLog? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntryAsync(dbContext.CdnLogs, l => l.Cdn, cancellationToken);

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
    /// <returns>The value of the nested loaded <see cref="Model.CdnVersion.Library" /> <c>=&gt;</c> <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<Model.UpstreamCdn?> GetCdnAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await (await entity.GetLibraryEntryAsync(dbContext, cancellationToken)).GetCdnAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnVersion" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnVersion" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnVersion.Library" /> <c>=&gt;</c> <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<EntityEntry<Model.UpstreamCdn>?> GetCdnEntryAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await (await entity.GetLibraryEntryAsync(dbContext, cancellationToken)).GetCdnEntryAsync(cancellationToken);

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
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnVersion.Library" /> <c>=&gt;</c> <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<EntityEntry<Model.UpstreamCdn>?> GetCdnEntryAsync(this EntityEntry<Model.CdnVersion>? entry, CancellationToken cancellationToken) => await (await entry.GetLibraryEntryAsync(cancellationToken)).GetCdnEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> <c>=&gt;</c> <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<Model.UpstreamCdn?> GetCdnAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await (await entity.GetLibraryEntryAsync(dbContext, cancellationToken)).GetCdnAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> <c>=&gt;</c> <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<EntityEntry<Model.UpstreamCdn>?> GetCdnEntryAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await (await entity.GetLibraryEntryAsync(dbContext, cancellationToken)).GetCdnEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> <c>=&gt;</c> <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<Model.UpstreamCdn?> GetCdnAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken) => await (await entry.GetLibraryEntryAsync(cancellationToken)).GetCdnAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> <c>=&gt;</c> <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<EntityEntry<Model.UpstreamCdn>?> GetCdnEntryAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken) => await (await entry.GetLibraryEntryAsync(cancellationToken)).GetCdnEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnLibrary" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnLibrary" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<Model.UpstreamCdn?> GetCdnAsync(this Model.CdnLibrary? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntityAsync(dbContext.CdnLibraries, l => l.Cdn, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.UpstreamCdn" /> for the specified <see cref="Model.CdnLibrary" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnLibrary" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnLibrary.Cdn" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.UpstreamCdn" />.</returns>
    public static async Task<EntityEntry<Model.UpstreamCdn>?> GetCdnEntryAsync(this Model.CdnLibrary? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntryAsync(dbContext.CdnLibraries, l => l.Cdn, cancellationToken);

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
    public static async Task<Model.LocalLibrary?> GetLocalAsync(this Model.CdnLibrary? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntityAsync(dbContext.CdnLibraries, l => l.Local, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.CdnLibrary" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnLibrary" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnLibrary.Local" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<EntityEntry<Model.LocalLibrary>?> GetLocalEntryAsync(this Model.CdnLibrary? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntryAsync(dbContext.CdnLibraries, l => l.Local, cancellationToken);

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
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property <see cref="Model.CdnVersion.Local" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<Model.LocalLibrary?> GetLocalLibraryAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await (await entity.GetLocalEntryAsync(dbContext, cancellationToken)).GetLibraryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.CdnVersion" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnVersion" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<EntityEntry<Model.LocalLibrary>?> GetLocalLibraryEntryAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await (await entity.GetLocalEntryAsync(dbContext, cancellationToken)).GetLibraryEntryAsync(cancellationToken);

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
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<EntityEntry<Model.LocalLibrary>?> GetLocalLibraryEntryAsync(this EntityEntry<Model.CdnVersion>? entry, CancellationToken cancellationToken) => await (await entry.GetLocalEntryAsync(cancellationToken)).GetLibraryEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Local" /> <c>=&gt;</c> <see cref="Model.LocalFile.Version" /> <c>=&gt;</c> <see cref="Model.LocalVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<Model.LocalLibrary?> GetLocalLibraryAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await (await entity.GetLocalEntryAsync(dbContext, cancellationToken)).GetLibraryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Local" /> <c>=&gt;</c> <see cref="Model.LocalFile.Version" /> <c>=&gt;</c> <see cref="Model.LocalVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<EntityEntry<Model.LocalLibrary>?> GetLocalLibraryEntryAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await (await entity.GetLocalEntryAsync(dbContext, cancellationToken)).GetLibraryEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Local" /> <c>=&gt;</c> <see cref="Model.LocalFile.Version" /> <c>=&gt;</c> <see cref="Model.LocalVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<Model.LocalLibrary?> GetLocalLibraryAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken) => await (await entry.GetLocalEntryAsync(cancellationToken)).GetLibraryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.CdnFile" /> <see cref="EntityEntry{TEntity}" />.
    /// </summary>
    /// <param name="entity">The <see cref="EntityEntry{TEntity}" /> of the target <see cref="Model.CdnFile" />.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Local" /> <c>=&gt;</c> <see cref="Model.LocalFile.Version" /> <c>=&gt;</c> <see cref="Model.LocalVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<EntityEntry<Model.LocalLibrary>?> GetLocalLibraryEntryAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken) => await (await entry.GetLocalEntryAsync(cancellationToken)).GetLibraryEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.LocalVersion" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.LocalVersion" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.LocalVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<Model.LocalLibrary?> GetLibraryAsync(this Model.LocalVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntityAsync(dbContext.LocalVersions, l => l.Library, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalLibrary" /> for the specified <see cref="Model.LocalVersion" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.LocalVersion" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.LocalVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalLibrary" />.</returns>
    public static async Task<EntityEntry<Model.LocalLibrary>?> GetLibraryEntryAsync(this Model.LocalVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntryAsync(dbContext.LocalVersions, l => l.Library, cancellationToken);

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
    public static async Task<Model.CdnLibrary?> GetLibraryAsync(this Model.LibraryLog? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntityAsync(dbContext.LibraryLogs, l => l.Library, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.LibraryLog" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.LibraryLog" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.LibraryLog.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<EntityEntry<Model.CdnLibrary>?> GetLibraryEntryAsync(this Model.LibraryLog? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntryAsync(dbContext.LibraryLogs, l => l.Library, cancellationToken);

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
    public static async Task<Model.CdnLibrary?> GetLibraryAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await (await entity.GetVersionEntryAsync(dbContext, cancellationToken)).GetLibraryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<EntityEntry<Model.CdnLibrary>?> GetLibraryEntryAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await (await entity.GetVersionEntryAsync(dbContext, cancellationToken)).GetLibraryEntryAsync(cancellationToken);

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
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<EntityEntry<Model.CdnLibrary>?> GetLibraryEntryAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken) => await (await entry.GetVersionEntryAsync(cancellationToken)).GetLibraryEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<Model.LocalLibrary?> GetLibraryAsync(this Model.LocalFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await (await entity.GetVersionEntryAsync(dbContext, cancellationToken)).GetLibraryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<EntityEntry<Model.LocalLibrary>?> GetLibraryEntryAsync(this Model.LocalFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await (await entity.GetVersionEntryAsync(dbContext, cancellationToken)).GetLibraryEntryAsync(cancellationToken);

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
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> <c>=&gt;</c> <see cref="Model.CdnVersion.Library" /> property which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<EntityEntry<Model.LocalLibrary>?> GetLibraryEntryAsync(this EntityEntry<Model.LocalFile>? entry, CancellationToken cancellationToken) => await (await entry.GetVersionEntryAsync(cancellationToken)).GetLibraryEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.CdnVersion" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnVersion" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<Model.CdnLibrary?> GetLibraryAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntityAsync(dbContext.CdnVersions, l => l.Library, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnLibrary" /> for the specified <see cref="Model.CdnVersion" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnVersion" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnVersion.Library" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnLibrary" />.</returns>
    public static async Task<EntityEntry<Model.CdnLibrary>?> GetLibraryEntryAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntryAsync(dbContext.CdnVersions, l => l.Library, cancellationToken);

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
    public static async Task<Model.LocalVersion?> GetLocalAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntityAsync(dbContext.CdnVersions, l => l.Local, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalVersion" /> for the specified <see cref="Model.CdnVersion" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnVersion" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnVersion.Local" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalVersion" />.</returns>
    public static async Task<EntityEntry<Model.LocalVersion>?> GetLocalEntryAsync(this Model.CdnVersion? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntryAsync(dbContext.CdnVersions, l => l.Local, cancellationToken);

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
    public static async Task<Model.LocalVersion?> GetLocalVersionAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await (await entity.GetLocalEntryAsync(dbContext, cancellationToken)).GetVersionAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalVersion" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Local" /> <c>=&gt;</c> <see cref="Model.LocalFile.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalVersion" />.</returns>
    public static async Task<EntityEntry<Model.LocalVersion>?> GetLocalVersionEntryAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await (await entity.GetLocalEntryAsync(dbContext, cancellationToken)).GetVersionEntryAsync(cancellationToken);

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
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Local" /> <c>=&gt;</c> <see cref="Model.LocalFile.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalVersion" />.</returns>
    public static async Task<EntityEntry<Model.LocalVersion>?> GetLocalVersionEntryAsync(this EntityEntry<Model.CdnFile>? entry, CancellationToken cancellationToken) => await (await entry.GetLocalEntryAsync(cancellationToken)).GetVersionEntryAsync(cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the related <see cref="Model.LocalVersion" /> for the specified <see cref="Model.LocalFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.LocalFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The value of the loaded <see cref="Model.LocalFile.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalVersion" />.</returns>
    public static async Task<Model.LocalVersion?> GetVersionAsync(this Model.LocalFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntityAsync(dbContext.LocalFiles, l => l.Version, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.LocalVersion" /> for the specified <see cref="Model.LocalFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.LocalFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.LocalFile.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.LocalVersion" />.</returns>
    public static async Task<EntityEntry<Model.LocalVersion>?> GetVersionEntryAsync(this Model.LocalFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntryAsync(dbContext.LocalFiles, l => l.Version, cancellationToken);

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
    public static async Task<Model.CdnVersion?> GetVersionAsync(this Model.VersionLog? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntityAsync(dbContext.VersionLogs, l => l.Version, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnVersion" /> for the specified <see cref="Model.VersionLog" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.VersionLog" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.VersionLog.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnVersion" />.</returns>
    public static async Task<EntityEntry<Model.CdnVersion>?> GetVersionEntryAsync(this Model.VersionLog? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntryAsync(dbContext.VersionLogs, l => l.Version, cancellationToken);

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
    public static async Task<Model.CdnVersion?> GetVersionAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntityAsync(dbContext.CdnFiles, l => l.Version, cancellationToken);

    /// <summary>
    /// Asyncrhonously loads and gets the <see cref="EntityEntry{TEntity}" /> of the related <see cref="Model.CdnVersion" /> for the specified <see cref="Model.CdnFile" /> entity.
    /// </summary>
    /// <param name="entity">The target <see cref="Model.CdnFile" /> entity.</param>
    /// <param name="dbContext">The current database context object.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}" /> of the loaded <see cref="Model.CdnFile.Version" /> property, which may be <see langword="null" /> if there is no related <see cref="Model.CdnVersion" />.</returns>
    public static async Task<EntityEntry<Model.CdnVersion>?> GetVersionEntryAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntryAsync(dbContext.CdnFiles, l => l.Version, cancellationToken);

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
    public static async Task<EntityEntry<Model.LocalFile>?> GetLocalEntryAsync(this Model.CdnFile? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntryAsync(dbContext.CdnFiles, l => l.Local, cancellationToken);

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
    public static async Task<EntityEntry<Model.CdnFile>?> GetFileEntryAsync(this Model.FileLog? entity, Services.ContentDb dbContext, CancellationToken cancellationToken) => await entity.GetReferencedEntryAsync(dbContext.FileLogs, l => l.File, cancellationToken);

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
    
    public static string ToStatusMessage(this int statusCode) => statusCode switch
    {
        100 => "Continue",
        101 => "Switching Protocols",
        102 => "Processing",
        103 => "Early Hints",

        200 => "OK",
        201 => "Created",
        202 => "Accepted",
        203 => "Non-Authoritative Information",
        204 => "No Content",
        205 => "Reset Content",
        206 => "Partial Content",
        207 => "Multi-Status",
        208 => "Already Reported",
        226 => "IM Used",

        300 => "Multiple Choices",
        301 => "Moved Permanently",
        302 => "Found",
        303 => "See Other",
        304 => "Not Modified",
        305 => "Use Proxy",
        307 => "Temporary Redirect",
        308 => "Permanent Redirect",

        400 => "Bad Request",
        401 => "Unauthorized",
        402 => "Payment Required",
        403 => "Forbidden",
        404 => "Not Found",
        405 => "Method Not Allowed",
        406 => "Not Acceptable",
        407 => "Proxy Authentication Required",
        408 => "Request Timeout",
        409 => "Conflict",
        410 => "Gone",
        411 => "Length Required",
        412 => "Precondition Failed",
        413 => "Request Entity Too Large",
        414 => "Request-Uri Too Long",
        415 => "Unsupported Media Type",
        416 => "Requested Range Not Satisfiable",
        417 => "Expectation Failed",
        421 => "Misdirected Request",
        422 => "Unprocessable Entity",
        423 => "Locked",
        424 => "Failed Dependency",
        426 => "Upgrade Required", // RFC 2817
        428 => "Precondition Required",
        429 => "Too Many Requests",
        431 => "Request Header Fields Too Large",
        451 => "Unavailable For Legal Reasons",

        500 => "Internal Server Error",
        501 => "Not Implemented",
        502 => "Bad Gateway",
        503 => "Service Unavailable",
        504 => "Gateway Timeout",
        505 => "Http Version Not Supported",
        506 => "Variant Also Negotiates",
        507 => "Insufficient Storage",
        508 => "Loop Detected",
        510 => "Not Extended",
        511 => "Network Authentication Required",
        _ => $"Status Code {statusCode}"
    };
}
