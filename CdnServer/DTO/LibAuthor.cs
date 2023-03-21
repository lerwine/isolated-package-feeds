using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CdnServer.DTO;

public class LibAuthor
{
    public Guid Id { get; set; }
    
    public Guid LibraryId { get; set; }
    
    public Library? Library { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Url { get; set; }

    internal static void OnBuildEntity(EntityTypeBuilder<LibAuthor> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.HasOne(i => i.Library).WithMany(t => t.Authors).HasForeignKey(nameof(Id)).OnDelete(DeleteBehavior.Restrict);
        _ = builder.Property(c => c.Name).UseCollation("SQL_Latin1_General_CP1_CI_AS");
        _ = builder.Property(c => c.Url).UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }
}
