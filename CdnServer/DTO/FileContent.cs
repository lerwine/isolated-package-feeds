using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CdnServer.DTO;

public class FileContent
{
    public Guid Id { get; set; }
    
    public LibFile File { get; set; }

    public byte[] Content { get; set; }

    public string ContentType { get; set; }

    public Encoding? Encoding { get; set; }

    internal static void OnBuildEntity(EntityTypeBuilder<FileContent> builder)
    {
        _ = builder.HasKey(nameof(Id));
    }
}