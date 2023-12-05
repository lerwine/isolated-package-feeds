namespace CdnGetter.Services;

public partial class MainService
{
    enum OperationType
    {
        ShowCDNs,
        ShowLibraries,
        ShowVersions,
        ShowFiles,
        AddLibraries,
        GetNewVersions,
        RemoveLibraries,
        ReloadLibraries,
        ReloadExisting
    }
}
