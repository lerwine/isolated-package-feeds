namespace CdnGetter.Model;

/// <summary>
/// Indicates a database library modification action.
/// </summary>
public enum LibraryAction : byte
{
    /// <summary>
    /// Adds new versions of the <see cref="CdnLibrary" /> that matches the specified <see cref="CdnLibrary.Name" /> of the current <see cref="UpstreamCdn" />.
    /// </summary>
    GetNewVersions = 0x00,
    
    /// <summary>
    /// Adds a new <see cref="CdnLibrary" /> to the current <see cref="UpstreamCdn" />.
    /// </summary>
    Add = 0x01,
    
    /// <summary>
    /// Reloads all versions of the <see cref="CdnLibrary" /> that matches the specified <see cref="CdnLibrary.Name" /> of the current <see cref="UpstreamCdn" />, including adding new versions from the upstream service.
    /// </summary>
    Reload = 0x02,
    
    /// <summary>
    /// Reloads existing <see cref="CdnVersion" /> records for the <see cref="CdnLibrary" /> that matches the specified <see cref="CdnLibrary.Name" /> of the current <see cref="UpstreamCdn" />.
    /// </summary>
    ReloadExistingVersions = 0x03,
    
    /// <summary>
    /// Removes the <see cref="CdnLibrary" /> from the <see cref="Services.ContentDb" /> that matches the current <see cref="UpstreamCdn" /> and the specified <see cref="CdnLibrary.Name" />.
    /// </summary>
    Remove = 0x04
}