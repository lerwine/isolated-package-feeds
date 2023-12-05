using CdnServer.DTO;
using Microsoft.EntityFrameworkCore;

namespace CdnServer.Services;

public class CdnAppDbContext : DbContext
{
    private readonly ILogger<CdnAppDbContext> _logger;

    public CdnAppDbContext(DbContextOptions<CdnAppDbContext> options, ILogger<CdnAppDbContext> logger)
        : base(options)
    {
        _logger = logger;
    }

    public DbSet<LibAuthor> Authors { get; set; } = null!;

    public DbSet<LibFile> Files { get; set; } = null!;

    public DbSet<FileContent> FileContents { get; set; } = null!;

    public DbSet<LibGroup> Groups { get; set; } = null!;

    public DbSet<Library> Libraries { get; set; } = null!;

    /// <summary>
    /// Configures the data model.
    /// </summary>
    /// <param name="modelBuilder"> The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<LibAuthor>(LibAuthor.OnBuildEntity)
            .Entity<LibFile>(LibFile.OnBuildEntity)
            .Entity<FileContent>(FileContent.OnBuildEntity)
            .Entity<LibGroup>(LibGroup.OnBuildEntity)
            .Entity<Library>(Library.OnBuildEntity);
    }
}