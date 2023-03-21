namespace CdnGetter.Model;

/// <summary>
/// Indicates a database library modification action.
/// </summary>
public enum LibraryAction : byte
{
    /// <summary>
    /// Adds new versions of the <see cref="Model.RemoteLibrary" /> that matches the specified <see cref="Model.RemoteLibrary.Name" /> of the current <see cref="Model.RemoteService" />.
    /// </summary>
    GetNewVersions = 0x00,
    
    /// <summary>
    /// Adds a new <see cref="Model.RemoteLibrary" /> to the current <see cref="Model.RemoteService" />.
    /// </summary>
    Add = 0x01,
    
    /// <summary>
    /// Reloads all versions of the <see cref="Model.RemoteLibrary" /> that matches the specified <see cref="Model.RemoteLibrary.Name" /> of the current <see cref="Model.RemoteService" />, including adding new versions from the remote service.
    /// </summary>
    Reload = 0x02,
    
    /// <summary>
    /// Reloads existing <see cref="Model.RemoteVersion" /> records for the <see cref="Model.RemoteLibrary" /> that matches the specified <see cref="Model.RemoteLibrary.Name" /> of the current <see cref="Model.RemoteService" />.
    /// </summary>
    ReloadExistingVersions = 0x03,
    
    /// <summary>
    /// Removes the <see cref="Model.RemoteLibrary" /> from the <see cref="Services.ContentDb" /> that matches the current <see cref="Model.RemoteService" /> and the specified <see cref="Model.RemoteLibrary.Name" />.
    /// </summary>
    Remove = 0x04
}