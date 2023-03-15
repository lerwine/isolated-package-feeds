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
    /// Content libraries retrieved from remote content delivery networks.
    /// </summary>
    public DbSet<ContentLibrary> Libraries { get; set; } = null!;

    /// <summary>
    /// Specific versions of content libraries retrieved from remote content delivery networks.
    /// </summary>
    public DbSet<LibraryVersion> Versions { get; set; } = null!;

    /// <summary>
    /// Files retrieved from remote content delivery networks.
    /// </summary>
    public DbSet<LibraryFile> Files { get; set; } = null!;
    
    /// <summary>
    /// Configures the data model.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<RemoteService>(RemoteService.OnBuildEntity)
            .Entity<ContentLibrary>(ContentLibrary.OnBuildEntity)
            .Entity<LibraryVersion>(LibraryVersion.OnBuildEntity)
            .Entity<LibraryFile>(LibraryFile.OnBuildEntity);
    }
}
