namespace IsolatedPackageFeeds.Shared.LazyInit;

public static class ExtensionMethods
{
    public static LazyInitState ToLazyInitState(this LazyOptionalInitState value) => value switch
    {
        LazyOptionalInitState.NotInvoked => LazyInitState.NotInvoked,
        LazyOptionalInitState.Faulted => LazyInitState.Faulted,
        _ => LazyInitState.SuccessfullyCompleted,
    };
}