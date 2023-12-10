using System.Diagnostics.CodeAnalysis;

namespace IsolatedPackageFeeds.Shared.LazyInit;

/// <summary>
/// Represents a deferred optional value creation process that does not create the value until the result value accessor is invoked.
/// </summary>
/// <typeparam name="TResult">The type of result value.</typeparam>
public interface ILazyOptionalInitializer<TResult> : ILazyInitializer<TResult?>
{
    /// <summary>
    /// Gets a value indicating the status of the result value creation.
    /// </summary>
    /// <value>A value indicating whether the underlying <typeparamref name="TResult"/> value creation process successfully created a value, ran to completion but produced no value,
    /// threw an exception, or has not yet been invoked.</value>
    new LazyOptionalInitState State { get; }

    /// <summary>
    /// Gets the result value.
    /// </summary>
    /// <returns>The <typeparamref name="TResult"/> value, which may be <see langword="null"/> or the <see langword="default"/> value if the underlyng <typeparamref name="TResult"/>
    /// value creation process did not create a value.</returns>
    /// <exception cref="Exception">The exception thrown by the underlying <typeparamref name="TResult"/> value creation process.</exception>
    /// <remarks>If the underlying <typeparamref name="TResult"/> value creation process has not already been invoked, this will cause it to be invoked.
    /// <para>Any exception that was thrown during the <typeparamref name="TResult"/> value creation process will be re-thrown when this method is subsequently invoked.</para></remarks>
    new TResult? GetResult();

    /// <summary>
    /// Gets the result value.
    /// </summary>
    /// <param name="onSuccess">Delegate to invoke if the <typeparamref name="TResult"/> value was successfully created.</param>
    /// <param name="whenNoValue">Delegate to invoke if the underlying <typeparamref name="TResult"/> value creation process ran to completion, but did not produce a value.</param>
    /// <param name="whenFaulted">Delegate to invoke if the underlying <typeparamref name="TResult"/> value creation process threw an exception.</param>
    /// <remarks>The <paramref name="onSuccess"/>, <paramref name="whenNoValue"/>, or <paramref name="whenFaulted"/> delegate will be invoked
    /// regardless of whether this caused the underlying <typeparamref name="TResult"/> value creation process to be invoked.</remarks>
    void GetResult(Action<TResult> onSuccess, Action whenNoValue, Action<Exception>? whenFaulted = null);

    /// <summary>
    /// Ensures that the underlying <typeparamref name="TResult"/> value creation process has been invoked.
    /// </summary>
    /// <param name="onSuccess">Delegate to invoke if the <typeparamref name="TResult"/> value was successfully created.</param>
    /// <param name="whenNoValue">Delegate to invoke if the underlying <typeparamref name="TResult"/> value creation process ran to completion, but did not produce a value.</param>
    /// <param name="whenFaulted">Delegate to invoke if the underlying <typeparamref name="TResult"/> value creation process threw an exception.</param>
    /// <returns><see langword="true"/> if this caused the underlying <typeparamref name="TResult"/> value creation process to be invoked; otherwise, <see langword="false"/> if it had already been invoked.</returns>
    /// <remarks>The <paramref name="onSuccess"/>, <paramref name="whenNoValue"/>, or <paramref name="whenFaulted"/> delegate will be invoked
    /// regardless of whether this caused the underlying <typeparamref name="TResult"/> value creation process to be invoked.</remarks>
    bool Invoke(Action<TResult> onSuccess, Action whenNoValue, Action<Exception> whenFaulted);

    /// <summary>
    /// Attempts to get the result value.
    /// </summary>
    /// <param name="result">The result value, which may be <see langword="null"/> if this returns <see langword="false"/>.</param>
    /// <returns><see langword="true"/> if the <typeparamref name="TResult"/> value was successfully created; otherwise, <see langword="true"/>.</returns>
    /// <remarks>If the underlying <typeparamref name="TResult"/> value creation process has not already been invoked, this will cause it to be invoked.</remarks>
    new bool TryGetResult([NotNullWhen(true)] out TResult? result);
}
