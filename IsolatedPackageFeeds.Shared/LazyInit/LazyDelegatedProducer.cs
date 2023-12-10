namespace IsolatedPackageFeeds.Shared.LazyInit;

/// <summary>
/// Deferred invocation of a delegate that produces a <typeparamref name="TResult"/> value.
/// </summary>
/// <typeparam name="TResult">The result value type.</typeparam>
/// <seealso cref="Func{TResult}"/>
public class LazyDelegatedProducer<TResult>(Func<TResult> func) : LazyProducer<TResult>
{
    private readonly Func<TResult> _func = func;

    protected override TResult Initialize() => _func();
}
