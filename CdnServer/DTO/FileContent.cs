using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CdnServer.DTO;

public class FileContent
{
    public Guid Id { get; set; }
    
    public LibFile? File { get; set; }

    public byte[] Content { get; set; } = [];

    public string ContentType { get; set; } = string.Empty;

    public Encoding? Encoding { get; set; }

    internal static void OnBuildEntity(EntityTypeBuilder<FileContent> builder)
    {
        _ = builder.HasKey(nameof(Id));
    }
}