using Microsoft.EntityFrameworkCore;
using CdnGetter.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;

namespace CdnGetter.Services;

/// <summary>
/// Database context for content retrieved from upstream content delivery networks.
/// </summary>
public class ContentDb : DbContext
{
    private readonly ILogger<ContentDb> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private static readonly object _syncRoot = new();
    private static bool _connectionStringValidated;

    public ContentDb(DbContextOptions<ContentDb> options, ILogger<ContentDb> logger, IServiceScopeFactory scopeFactory) : base(options)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        lock (_syncRoot)
        {
            if (!_connectionStringValidated)
            {
                _connectionStringValidated = true;
                SqliteConnectionStringBuilder builder = new(Database.GetConnectionString());
                string connectionString = builder.ConnectionString;
                _logger.LogUsingConnectionStringTrace(connectionString);
                if (!File.Exists(builder.DataSource))
                {
                    builder.Mode = SqliteOpenMode.ReadWriteCreate;
                    _logger.LogInitializingNewDatabase(builder.DataSource);
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
                            _logger.LogCriticalQueryExecutionError(text, exception);
                            throw;
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
    /// Finds the upstream CDN by its name.
    /// </summary>
    /// <param name="name">The name of the CDN to find.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The matching <see cref="UpstreamCdn" /> or <see langword="null" /> if none matched the specified <paramref name="name" />.</returns>
    internal async Task<UpstreamCdn?> FindCdnByNameAsync(string name, CancellationToken cancellationToken) => await UpstreamCdns.FirstOrDefaultAsync(cdn => cdn.Name == name, cancellationToken);

    /// <summary>
    /// Activity logs for libraries retrieved from upstream content delivery networks.
    /// </summary>
    public DbSet<CdnLog> CdnLogs { get; set; } = null!;

    /// <summary>
    /// Locally stored content libraries.
    /// </summary>
    public DbSet<LocalLibrary> LocalLibraries { get; set; } = null!;

    /// <summary>
    /// Finds the local library by its name.
    /// </summary>
    /// <param name="name">The name of the library to find.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The matching <see cref="LocalLibrary" /> or <see langword="null" /> if none matched the specified <paramref name="name" />.</returns>
    internal async Task<LocalLibrary?> FindLibraryByNameAsync(string name, CancellationToken cancellationToken) => await LocalLibraries.FirstOrDefaultAsync(cdn => cdn.Name == name, cancellationToken);

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
        using IDisposable? scope = _logger.BeginExecuteMethodScope(nameof(OnBeforeSaveAsync));
        using IServiceScope serviceScope = _scopeFactory.CreateScope();
        foreach (EntityEntry e in ChangeTracker.Entries())
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            if (e.Entity is IValidatableObject entity)
                switch (e.State)
                {
                    case EntityState.Added:
                    case EntityState.Modified:
                        await ValidateEntryAsync(e, entity, serviceScope.ServiceProvider);
                        break;
                }
        }
    }

    internal async Task ValidateEntryAsync(EntityEntry e, IValidatableObject entity, IServiceProvider backingServiceProvider)
    {
        DbContextServiceProvider serviceProvider = new(this, backingServiceProvider, e);
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
        using IDisposable? scope = _logger.BeginExecuteMethodScope(nameof(SaveChanges));
        OnBeforeSaveAsync().Wait();
        int returnValue = base.SaveChanges();
        _logger.LogDbSaveChangeCompletedTrace(false, null, returnValue);
        return returnValue;
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        using IDisposable? scope = _logger.BeginExecuteMethodScope(nameof(SaveChanges), nameof(acceptAllChangesOnSuccess), acceptAllChangesOnSuccess);
        OnBeforeSaveAsync().Wait();
        int returnValue = base.SaveChanges(acceptAllChangesOnSuccess);
        _logger.LogDbSaveChangeCompletedTrace(false, acceptAllChangesOnSuccess, returnValue);
        return returnValue;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        using IDisposable? scope = _logger.BeginExecuteMethodScope(nameof(SaveChangesAsync), nameof(acceptAllChangesOnSuccess), acceptAllChangesOnSuccess);
        await OnBeforeSaveAsync(cancellationToken);
        int returnValue = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        _logger.LogDbSaveChangeCompletedTrace(true, acceptAllChangesOnSuccess, returnValue);
        return returnValue;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        using IDisposable? scope = _logger.BeginExecuteMethodScope(nameof(SaveChangesAsync));
        await OnBeforeSaveAsync(cancellationToken);
        int returnValue = await base.SaveChangesAsync(cancellationToken);
        _logger.LogDbSaveChangeCompletedTrace(true, null, returnValue);
        return returnValue;
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
        private readonly IServiceProvider _backingServiceProvider;

        internal DbContextServiceProvider(ContentDb dbContext, IServiceProvider backingServiceProvider, object entity)
        {
            _dbContext = dbContext;
            _entity = entity;
            _backingServiceProvider = backingServiceProvider;
        }

        public object? GetService(Type serviceType)
        {
            if (serviceType is null)
                return null;
            if (serviceType.IsInstanceOfType(_entity))
                return _entity;
            if (serviceType.IsInstanceOfType(_dbContext._logger))
                return _dbContext._logger;
            return _backingServiceProvider.GetService(serviceType);
        }
    }
}
