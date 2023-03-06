using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
        get { return _name; }
        set { _name = value.ToWsNormalizedOrEmptyIfNull(); }
    }
    
    private string _description = "";
    public string Description
    {
        get { return _description; }
        set { _description = value.ToTrimmedOrEmptyIfNull(); }
    }

    private Collection<ContentLibrary> _libraries = new();
    public Collection<ContentLibrary> Libraries
    {
        get { return _libraries; }
        set { _libraries = value ?? new(); }
    }
    
    internal static void OnBuildEntity(EntityTypeBuilder<LibraryProvider> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.HasIndex(nameof(Name));
        _ = builder.Property(c => c.Name).UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }
}