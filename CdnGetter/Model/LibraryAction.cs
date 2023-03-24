namespace CdnGetter.Model;

/// <summary>
/// Indicates a database library modification action.
/// </summary>
public enum LibraryAction : byte
{
    /// <summary>
    /// Adds new versions of the <see cref="Model.CdnLibrary" /> that matches the specified <see cref="Model.CdnLibrary.Name" /> of the current <see cref="Model.UpstreamCdn" />.
    /// </summary>
    GetNewVersions = 0x00,
    
    /// <summary>
    /// Adds a new <see cref="Model.CdnLibrary" /> to the current <see cref="Model.UpstreamCdn" />.
    /// </summary>
    Add = 0x01,
    
    /// <summary>
    /// Reloads all versions of the <see cref="Model.CdnLibrary" /> that matches the specified <see cref="Model.CdnLibrary.Name" /> of the current <see cref="Model.UpstreamCdn" />, including adding new versions from the remote service.
    /// </summary>
    Reload = 0x02,
    
    /// <summary>
    /// Reloads existing <see cref="Model.CdnVersion" /> records for the <see cref="Model.CdnLibrary" /> that matches the specified <see cref="Model.CdnLibrary.Name" /> of the current <see cref="Model.UpstreamCdn" />.
    /// </summary>
    ReloadExistingVersions = 0x03,
    
    /// <summary>
    /// Removes the <see cref="Model.CdnLibrary" /> from the <see cref="Services.ContentDb" /> that matches the current <see cref="Model.UpstreamCdn" /> and the specified <see cref="Model.CdnLibrary.Name" />.
    /// </summary>
    Remove = 0x04
}