using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CdnServer.DTO;

public class LibGroup
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = "";

    public Collection<Library> Libraries { get; set; }

    internal static void OnBuildEntity(EntityTypeBuilder<LibGroup> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(c => c.Name).UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }
}
