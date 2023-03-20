using Microsoft.EntityFrameworkCore;
using CdnGet.Model;

namespace CdnGet.Services;

/// <summary>
/// Database context for content retrieved from remote content delivery networks.
/// </summary>
public class ContentDb : DbContext
{
    public ContentDb(DbContextOptions<ContentDb> options) : base(options) { }

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
    /// Lcoally stored library versions.
    /// </summary>
    public DbSet<LocalVersion> LocalVersions { get; set; } = null!;

    /// <summary>
    /// Specific versions of content libraries retrieved from remote content delivery networks.
    /// </summary>
    public DbSet<RemoteVersion> RemoteVersions { get; set; } = null!;

    /// <summary>
    /// Locally stored library files.
    /// </summary>
    public DbSet<LocalFile> LocalFiles { get; set; } = null!;

    /// <summary>
    /// Files retrieved from remote content delivery networks.
    /// </summary>
    public DbSet<RemoteFile> RemoteFiles { get; set; } = null!;
    
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
            .Entity<LocalVersion>(LocalVersion.OnBuildEntity)
            .Entity<RemoteVersion>(RemoteVersion.OnBuildEntity)
            .Entity<LocalFile>(LocalFile.OnBuildEntity)
            .Entity<RemoteFile>(RemoteFile.OnBuildEntity);
    }
}
