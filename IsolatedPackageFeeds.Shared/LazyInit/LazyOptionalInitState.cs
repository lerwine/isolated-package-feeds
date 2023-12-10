namespace IsolatedPackageFeeds.Shared.LazyInit;

public enum LazyOptionalInitState
{
    /// <summary>
    /// A result value creation process has not been invoked.
    /// </summary>
    NotInvoked,

    /// <summary>
    /// A result value creation process threw an exception.
    /// </summary>
    Faulted,

    /// <summary>
    /// A result value creation process successfully ran to completion, but produced no value.
    /// </summary>
    NoValue,
    
    /// <summary>
    /// A result value creation process successfully ran to completion and produced a result value.
    /// </summary>
    ValueProduced
}