namespace CdnGetter.Services;
using static CdnGetter.Constants;

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
