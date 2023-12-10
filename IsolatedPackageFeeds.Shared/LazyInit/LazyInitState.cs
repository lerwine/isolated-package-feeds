namespace IsolatedPackageFeeds.Shared.LazyInit;

/// <summary>
/// Indicates the status of lazy result value creation.
/// </summary>
public enum LazyInitState
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
    /// A result value creation process successfully ran to completion.
    /// </summary>
    SuccessfullyCompleted
}
