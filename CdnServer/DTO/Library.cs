using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CdnServer.DTO;

public class Library
{
    public Guid Id { get; set; }
    
    public Guid GroupId { get; set; }
    
    public LibGroup? Group { get; set; }

    public string Version { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? HomePage { get; set; }

    public string? RepositryType { get; set; }

    public string? RepositoryUrl{ get; set; }

    public string? License{ get; set; }

    public Collection<LibAuthor> Authors { get; set; } = null!;
    
    internal static void OnBuildEntity(EntityTypeBuilder<Library> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.HasOne(i => i.Group).WithMany(t => t.Libraries).HasForeignKey(nameof(GroupId)).OnDelete(DeleteBehavior.Restrict);
        _ = builder.Property(c => c.Version).UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }
}
