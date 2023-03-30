using Microsoft.EntityFrameworkCore;
using CdnGetter.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;

namespace CdnGetter.Services;

/// <summary>
/// Database context for content retrieved from upstream content delivery networks.
/// </summary>
public class ContentDb : DbContext
{
    private readonly ILogger<ContentDb> _logger;
    private static readonly object _syncRoot = new();
    private static bool _connectionStringValidated;

    public ContentDb(DbContextOptions<ContentDb> options, ILogger<ContentDb> logger) : base(options)
    {
        _logger = logger;
        lock (_syncRoot)
        {
            if (!_connectionStringValidated)
            {
                _connectionStringValidated = true;
                SqliteConnectionStringBuilder builder = new(Database.GetConnectionString());
                string connectionString = builder.ConnectionString;
                _logger.LogInformation($"Using {nameof(SqliteConnectionStringBuilder.ConnectionString)} {{{nameof(SqliteConnectionStringBuilder.ConnectionString)}}}", connectionString);
                if (!File.Exists(builder.DataSource))
                {
                    builder.Mode = SqliteOpenMode.ReadWriteCreate;
                    _logger.LogInformation("Initializing new database");
                    using SqliteConnection connection = new(builder.ConnectionString);
                    connection.Open();
                    void executeNonQuery(string text)
                    {
                        using SqliteCommand command = connection.CreateCommand();
                        command.CommandText = text;
                        command.CommandType = System.Data.CommandType.Text;
                        try { _ = command.ExecuteNonQuery(); }
                        catch (Exception exception)
                        {
#pragma warning disable CA2201 // Exception type System.Exception is not sufficiently specific
                            throw new Exception($"Error executing query '{text}': {exception.Message}");
#pragma warning restore CA2201 // Exception type System.Exception is not sufficiently specific
                        }
                    }
                    UpstreamCdn.CreateTable(executeNonQuery);
                    CdnLog.CreateTable(executeNonQuery);
                    LocalLibrary.CreateTable(executeNonQuery);
                    CdnLibrary.CreateTable(executeNonQuery);
                    LibraryLog.CreateTable(executeNonQuery);
                    LocalVersion.CreateTable(executeNonQuery);
                    CdnVersion.CreateTable(executeNonQuery);
                    VersionLog.CreateTable(executeNonQuery);
                    LocalFile.CreateTable(executeNonQuery);
                    CdnFile.CreateTable(executeNonQuery);
                    FileLog.CreateTable(executeNonQuery);
                }
            }
        }
    }

    /// <summary>
    /// Registered upstream content delivery services.
    /// </summary>
    public DbSet<UpstreamCdn> UpstreamCdns { get; set; } = null!;

    /// <summary>
    /// Activity logs for libraries retrieved from upstream content delivery networks.
    /// </summary>
    public DbSet<CdnLog> CdnLogs { get; set; } = null!;

    /// <summary>
    /// Locally stored content libraries.
    /// </summary>
    public DbSet<LocalLibrary> LocalLibraries { get; set; } = null!;

    /// <summary>
    /// Content libraries retrieved from upstream content delivery networks.
    /// </summary>
    public DbSet<CdnLibrary> CdnLibraries { get; set; } = null!;

    /// <summary>
    /// Activity logs for libraries retrieved from upstream content delivery networks.
    /// </summary>
    public DbSet<LibraryLog> LibraryLogs { get; set; } = null!;

    /// <summary>
    /// Lcoally stored library versions.
    /// </summary>
    public DbSet<LocalVersion> LocalVersions { get; set; } = null!;

    /// <summary>
    /// Specific versions of content libraries retrieved from upstream content delivery networks.
    /// </summary>
    public DbSet<CdnVersion> CdnVersions { get; set; } = null!;

    /// <summary>
    /// Activity logs for specific versions of content libraries retrieved from upstream content delivery networks.
    /// </summary>
    public DbSet<VersionLog> VersionLogs { get; set; } = null!;

    /// <summary>
    /// Locally stored library files.
    /// </summary>
    public DbSet<LocalFile> LocalFiles { get; set; } = null!;

    /// <summary>
    /// Files retrieved from upstream content delivery networks.
    /// </summary>
    public DbSet<CdnFile> CdnFiles { get; set; } = null!;
    
    /// <summary>
    /// Activity logs for files retrieved from upstream content delivery networks.
    /// </summary>
    public DbSet<FileLog> FileLogs { get; set; } = null!;

    private async Task OnBeforeSaveAsync(CancellationToken cancellationToken = default)
{
        if (cancellationToken.IsCancellationRequested)
            return;
        using IDisposable? scope = _logger.BeginScope(nameof(OnBeforeSaveAsync));
        foreach (EntityEntry e in ChangeTracker.Entries())
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            if (e.Entity is IValidatableObject entity)
                switch (e.State)
                {
                    case EntityState.Added:
                    case EntityState.Modified:
                        await ValidateEntryAsync(e, entity);
                        break;
                }
        }
    }

    internal async Task ValidateEntryAsync(EntityEntry e, IValidatableObject entity)
    {
        DbContextServiceProvider serviceProvider = new(this, e);
        ValidationContext validationContext = new(entity, serviceProvider, null);
        if (entity is INotifyValidatingAsync notifyValidatingAsync)
            await notifyValidatingAsync.OnValidatingAsync(validationContext, e.State, serviceProvider);
        _logger.LogValidatingEntityTrace(e.State, e.Metadata, entity);
        try { Validator.ValidateObject(entity, validationContext, true); }
        catch (ValidationException validationException)
        {
            _logger.LogEntityValidationFailure(validationException, e.Metadata, entity);
            throw;
        }
        _logger.LogValidationSucceededTrace(e.State, e.Metadata, entity);
    }

    public override int SaveChanges()
    {
        using (_logger.BeginScope("{MethodName}()", nameof(SaveChanges)))
        {
            OnBeforeSaveAsync().Wait();
            int returnValue = base.SaveChanges();
            _logger.LogDbSaveChangeCompletedTrace(false, null, returnValue);
            return returnValue;
        }
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        using (_logger.BeginScope($"{{MethodName}}({nameof(acceptAllChangesOnSuccess)}: {{{nameof(acceptAllChangesOnSuccess)}}})", nameof(SaveChanges),
            acceptAllChangesOnSuccess))
        {
            OnBeforeSaveAsync().Wait();;
            int returnValue = base.SaveChanges(acceptAllChangesOnSuccess);
            _logger.LogDbSaveChangeCompletedTrace(false, acceptAllChangesOnSuccess, returnValue);
            return returnValue;
        }
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope($"{{MethodName}}({nameof(acceptAllChangesOnSuccess)}: {{{nameof(acceptAllChangesOnSuccess)}}})", nameof(SaveChangesAsync),
            acceptAllChangesOnSuccess))
        {
            await OnBeforeSaveAsync(cancellationToken);
            int returnValue = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            _logger.LogDbSaveChangeCompletedTrace(true, acceptAllChangesOnSuccess, returnValue);
            return returnValue;
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("{MethodName}()", nameof(SaveChangesAsync)))
        {
            await OnBeforeSaveAsync(cancellationToken);
            int returnValue = await base.SaveChangesAsync(cancellationToken);
            _logger.LogDbSaveChangeCompletedTrace(true, null, returnValue);
            return returnValue;
        }
    }

    /// <summary>
    /// Configures the data model.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<UpstreamCdn>(UpstreamCdn.OnBuildEntity)
            .Entity<CdnLog>(CdnLog.OnBuildEntity)
            .Entity<LocalLibrary>(LocalLibrary.OnBuildEntity)
            .Entity<CdnLibrary>(CdnLibrary.OnBuildEntity)
            .Entity<LibraryLog>(LibraryLog.OnBuildEntity)
            .Entity<LocalVersion>(LocalVersion.OnBuildEntity)
            .Entity<CdnVersion>(CdnVersion.OnBuildEntity)
            .Entity<VersionLog>(VersionLog.OnBuildEntity)
            .Entity<LocalFile>(LocalFile.OnBuildEntity)
            .Entity<CdnFile>(CdnFile.OnBuildEntity)
            .Entity<FileLog>(FileLog.OnBuildEntity);
    }

    internal class DbContextServiceProvider : IServiceProvider
    {
        private readonly object _entity;
        private readonly ContentDb _dbContext;

        internal DbContextServiceProvider(ContentDb dbContext, object entity)
        {
            _dbContext = dbContext;
            _entity = entity;
        }

        public object? GetService(Type serviceType)
        {
            if (serviceType is not null)
            {
                if (serviceType.IsInstanceOfType(_dbContext))
                    return _dbContext;
                if (serviceType.IsInstanceOfType(_entity))
                    return _entity;
                if (serviceType.IsInstanceOfType(_dbContext._logger))
                    return _dbContext._logger;
            }
            return null;
        }
    }
}
