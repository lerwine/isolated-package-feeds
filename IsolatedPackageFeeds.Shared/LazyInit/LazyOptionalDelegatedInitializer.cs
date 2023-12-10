using System.Diagnostics.CodeAnalysis;

namespace IsolatedPackageFeeds.Shared.LazyInit;

/// <summary>
/// Deferred invocation of a delegate that optionally produces a <typeparamref name="TResult"/> value.
/// </summary>
/// <typeparam name="TResult">The result value type.</typeparam>
/// <seealso cref="TryFunc{TResult}"/>
public class LazyOptionalDelegatedInitializer<TResult>(TryFunc<TResult?> tryFunc) : LazyOptionalInitializer<TResult>
{
    private readonly TryFunc<TResult?> _tryFunc = tryFunc;

    protected override bool TryInitialize([NotNullWhen(true)] out TResult? result) => _tryFunc(out result);
}
