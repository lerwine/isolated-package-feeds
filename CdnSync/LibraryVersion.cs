using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace CdnSync;

public class LibraryVersion
{
    private readonly object _syncRoot = new object();

    public Guid Id { get; set; }
    
    public ushort Order { get; set; }

    private string _name = "";
    public string Name
    {
        get { return _name; }
        set { _name = value.ToWsNormalizedOrEmptyIfNull(); }
    }
    
    private JsonObject? _providerData;
    public JsonObject? ProviderData
    {
        get { return _providerData; }
        set { _providerData = value; }
    }
    
    private Guid _libraryId;
    public Guid LibraryId
    {
        get { return _libraryId; }
        set { value.SetNavigation(_syncRoot, p => p.Id, ref _libraryId, ref _library); }
    }
    
    private ContentLibrary? _library;
    public ContentLibrary? Library
    {
        get { return _library; }
        set { value.SetNavigation(_syncRoot, p => p.Id, ref _libraryId, ref _library); }
    }

    private Collection<LibraryFile> _files = new();
    public Collection<LibraryFile> Files
    {
        get { return _files; }
        set { _files = value ?? new(); }
    }

    internal static void OnBuildEntity(EntityTypeBuilder<LibraryVersion> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(c => c.Name).UseCollation("SQL_Latin1_General_CP1_CI_AS");
        _ = builder.HasIndex(nameof(Order), nameof(LibraryId));
        _ = builder.HasIndex(nameof(Name), nameof(LibraryId));
        _ = builder.HasOne(l => l.Library).WithMany(p => p.Versions).HasForeignKey(l => l.LibraryId).IsRequired().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
    }

    internal static async Task<bool> DeleteAsync(CdnSyncDb dbContext, Guid libraryId, CancellationToken stoppingToken)
    {
        LibraryVersion[] toDelete = await dbContext.Versions.Where(v => v.LibraryId == libraryId).ToArrayAsync(stoppingToken);
        if (toDelete.Length == 0)
            return false;
        foreach (LibraryVersion lv in toDelete)
            await LibraryFile.DeleteAsync(dbContext, lv.Id, stoppingToken);
        dbContext.Versions.RemoveRange(toDelete);
        await dbContext.SaveChangesAsync(true, stoppingToken);
        return true;
    }
}
