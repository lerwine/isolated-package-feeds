using Microsoft.EntityFrameworkCore;
using CdnGetter.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;

namespace CdnGetter.Services;

/// <summary>
/// Database context for content retrieved from remote content delivery networks.
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
                _logger.LogInformation($"Using {nameof(SqliteConnectionStringBuilder.ConnectionString)} {{{nameof(SqliteConnectionStringBuilder.ConnectionString)}}}",
                    connectionString);
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
                    RemoteService.CreateTable(executeNonQuery, (ILogger)logger);
                    LocalLibrary.CreateTable(executeNonQuery, (ILogger)logger);
                    RemoteLibrary.CreateTable(executeNonQuery, (ILogger)logger);
                    LibraryLog.CreateTable(executeNonQuery, (ILogger)logger);
                    LocalVersion.CreateTable(executeNonQuery, (ILogger)logger);
                    RemoteVersion.CreateTable(executeNonQuery, (ILogger)logger);
                    VersionLog.CreateTable(executeNonQuery, (ILogger)logger);
                    LocalFile.CreateTable(executeNonQuery, (ILogger)logger);
                    RemoteFile.CreateTable(executeNonQuery, (ILogger)logger);
                    FileLog.CreateTable(executeNonQuery, (ILogger)logger);
                }
            }
        }
    }

    /// <summary>
    /// Registered remote content delivery services.
    /// </summary>
    public DbSet<RemoteService> RemoteServices { get; set; } = null!;

    /// <summary>
    /// Locally stored content libraries.
    /// </summary>
    public DbSet<LocalLibrary> LocalLibraries { get; set; } = null!;

    /// <summary>
    /// Content libraries retrieved from remote content delivery networks.
    /// </summary>
    public DbSet<RemoteLibrary> RemoteLibraries { get; set; } = null!;

    /// <summary>
    /// Activity logs for libraries retrieved from remote content delivery networks.
    /// </summary>
    public DbSet<LibraryLog> LibraryLogs { get; set; } = null!;

    /// <summary>
    /// Lcoally stored library versions.
    /// </summary>
    public DbSet<LocalVersion> LocalVersions { get; set; } = null!;

    /// <summary>
    /// Specific versions of content libraries retrieved from remote content delivery networks.
    /// </summary>
    public DbSet<RemoteVersion> RemoteVersions { get; set; } = null!;

    /// <summary>
    /// Activity logs for specific versions of content libraries retrieved from remote content delivery networks.
    /// </summary>
    public DbSet<VersionLog> VersionLogs { get; set; } = null!;

    /// <summary>
    /// Locally stored library files.
    /// </summary>
    public DbSet<LocalFile> LocalFiles { get; set; } = null!;

    /// <summary>
    /// Files retrieved from remote content delivery networks.
    /// </summary>
    public DbSet<RemoteFile> RemoteFiles { get; set; } = null!;
    
    /// <summary>
    /// Activity logs for files retrieved from remote content delivery networks.
    /// </summary>
    public DbSet<FileLog> FileLogs { get; set; } = null!;

    /// <summary>
    /// Configures the data model.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<RemoteService>(RemoteService.OnBuildEntity)
            .Entity<LocalLibrary>(LocalLibrary.OnBuildEntity)
            .Entity<RemoteLibrary>(RemoteLibrary.OnBuildEntity)
            .Entity<LibraryLog>(LibraryLog.OnBuildEntity)
            .Entity<LocalVersion>(LocalVersion.OnBuildEntity)
            .Entity<RemoteVersion>(RemoteVersion.OnBuildEntity)
            .Entity<VersionLog>(VersionLog.OnBuildEntity)
            .Entity<LocalFile>(LocalFile.OnBuildEntity)
            .Entity<RemoteFile>(RemoteFile.OnBuildEntity)
            .Entity<FileLog>(FileLog.OnBuildEntity);
    }
}
