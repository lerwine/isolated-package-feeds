namespace CdnGet.Config;

/// <summary>
/// Indicates a database library modification action.
/// </summary>
public enum LibraryAction
{
    /// <summary>
    /// Adds a new <see cref="Model.ContentLibrary" /> to the current <see cref="Model.RemoteService" />.
    /// </summary>
    Add,
    
    /// <summary>
    /// Adds new versions of the <see cref="Model.ContentLibrary" /> that matches the specified <see cref="Model.ContentLibrary.Name" /> of the current <see cref="Model.RemoteService" />.
    /// </summary>
    GetNew,
    
    /// <summary>
    /// Reloads all versions of the <see cref="Model.ContentLibrary" /> that matches the specified <see cref="Model.ContentLibrary.Name" /> of the current <see cref="Model.RemoteService" />, including adding new versions from the remote service.
    /// </summary>
    Reload,
    
    /// <summary>
    /// Reloads existing <see cref="Model.LibraryVersion" /> records for the <see cref="Model.ContentLibrary" /> that matches the specified <see cref="Model.ContentLibrary.Name" /> of the current <see cref="Model.RemoteService" />.
    /// </summary>
    ReloadExisting,
    
    /// <summary>
    /// Removes the <see cref="Model.ContentLibrary" /> from the <see cref="Services.ContentDb" /> that matches the current <see cref="Model.RemoteService" /> and the specified <see cref="Model.ContentLibrary.Name" />.
    /// </summary>
    Remove
}