using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CdnServer.DTO;
public class LibFile
{
    public Guid Id { get; set; }
    
    public FileContent Content { get; set; }

    public Guid LibraryId { get; set; }
    
    public Library Library { get; set; }

    public string Name { get; set; } = "";

    public string SriHash { get; set; } = "";

    public bool NotAvailable { get; set; }

    internal static void OnBuildEntity(EntityTypeBuilder<LibFile> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.HasOne(i => i.Content).WithOne(t => t.File).HasForeignKey(nameof(Id)).OnDelete(DeleteBehavior.Restrict);
        _ = builder.Property(c => c.Name).UseCollation("SQL_Latin1_General_CP1_CI_AS");
        _ = builder.Property(c => c.SriHash).UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }
}
