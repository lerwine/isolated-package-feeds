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
        get => _name;
        set => _name = value.ToWsNormalizedOrEmptyIfNull();
    }

    private string _description = "";
    public string Description
    {
        get => _description;
        set => _description = value.ToTrimmedOrEmptyIfNull();
    }

    public JsonNode? ProviderData { get; set; }

    private string? ProviderJson
    {
        get => ProviderData?.ToJsonString();
        set
        {
            string? text = value.ToTrimmedOrNullIfEmpty();
            if (text is null)
                ProviderData = null;
            else
                try { ProviderData = JsonNode.Parse(text); }
                catch { ProviderData = JsonValue.Create(text); }
        }
    }
    
    private Guid _providerId;
    public Guid ProviderId
    {
        get => _providerId;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _providerId, ref _provider);
    }

    private LibraryProvider? _provider;
    public LibraryProvider? Provider
    {
        get => _provider;
        set => value.SetNavigation(_syncRoot, p => p.Id, ref _providerId, ref _provider);
    }

    private Collection<LibraryVersion> _versions = new();
    public Collection<LibraryVersion> Versions
    {
        get => _versions;
        set => _versions = value ?? new();
    }

    internal static void OnBuildEntity(EntityTypeBuilder<ContentLibrary> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.HasIndex(nameof(Name));
        _ = builder.Property(c => c.Name).IsRequired().UseCollation("SQL_Latin1_General_CP1_CI_AS");
        _ = builder.HasOne(l => l.Provider).WithMany(p => p.Libraries).HasForeignKey(l => l.ProviderId).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.Property(c => c.ProviderJson);
        _ = builder.Ignore(l => l.ProviderData);
    }
}
