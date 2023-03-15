using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace CdnSync;

public class ContentLibrary
{
    private readonly object _syncRoot = new();

    public Guid Id { get; set; }
    
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
    
    private JsonObject? _providerData;
    // BUG: JsonObject can't be serialized to DB
    public JsonObject? ProviderData
    {
        get { return _providerData; }
        set { _providerData = value; }
    }
    
    private Guid _providerId;
    public Guid ProviderId
    {
        get { return _providerId; }
        set { value.SetNavigation(_syncRoot, p => p.Id, ref _providerId, ref _provider); }
    }
    
    private LibraryProvider? _provider;
    public LibraryProvider? Provider
    {
        get { return _provider; }
        set { value.SetNavigation(_syncRoot, p => p.Id, ref _providerId, ref _provider); }
    }

    private Collection<LibraryVersion> _versions = new();
    public Collection<LibraryVersion> Versions
    {
        get { return _versions; }
        set { _versions = value ?? new(); }
    }

    internal static void OnBuildEntity(EntityTypeBuilder<ContentLibrary> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.HasIndex(nameof(Name));
        _ = builder.Property(c => c.Name).UseCollation("SQL_Latin1_General_CP1_CI_AS");
        _ = builder.HasOne(l => l.Provider).WithMany(p => p.Libraries).HasForeignKey(l => l.ProviderId).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
    }
}
