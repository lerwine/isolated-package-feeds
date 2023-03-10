using Microsoft.EntityFrameworkCore;

namespace CdnSync;

public class CdnSyncDb : DbContext
{
    // private readonly ILogger<CdnSyncDb> _logger;

    // public CdnSyncDb(DbContextOptions<CdnSyncDb> options, ILogger<CdnSyncDb> logger)
    //     : base(options)
    // {
    //     _logger = logger;
    // }

    public CdnSyncDb(DbContextOptions<CdnSyncDb> options) : base(options) { }

    public DbSet<LibraryProvider> Providers { get; set; } = null!;

    public DbSet<ContentLibrary> Libraries { get; set; } = null!;
    
    public DbSet<LibraryVersion> Versions { get; set; } = null!;
    
    public DbSet<LibraryFile> Files { get; set; } = null!;
    
    /// <summary>
    /// Configures the data model.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<LibraryProvider>(LibraryProvider.OnBuildEntity)
            .Entity<ContentLibrary>(ContentLibrary.OnBuildEntity)
            .Entity<LibraryVersion>(LibraryVersion.OnBuildEntity)
            .Entity<LibraryFile>(LibraryFile.OnBuildEntity);
    }
}