using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace CdnSync;

/// <summary>
/// Represents a registered library provider that retrieves content from a remote CDN.
/// </summary>
public class LibraryProvider
{
    /// <summary>
    /// The unique identifier fo the registered library provider.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The name of the registered library provider.
    /// </summary>
    private string _name = "";
    public string Name
    {
        get => _name;
        set => _name = value.ToWsNormalizedOrEmptyIfNull();
    }

    private string _description = "";
    public string Description
    {
        get => _description;
        set => _description = value.ToTrimmedOrEmptyIfNull();
    }

    private Collection<ContentLibrary> _libraries = new();
    public Collection<ContentLibrary> Libraries
    {
        get => _libraries;
        set => _libraries = value ?? new();
    }

    internal static void OnBuildEntity(EntityTypeBuilder<LibraryProvider> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.HasIndex(nameof(Name));
        _ = builder.Property(c => c.Name).IsRequired().UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }
}