using System.Diagnostics.CodeAnalysis;

namespace IsolatedPackageFeeds.Shared.LazyInit;

/// <summary>
/// Represents a deferred value creation process that does not create the value until the <typeparamref name="TResult"/> value is actually retrieved.
/// </summary>
/// <typeparam name="TResult">The type of result value.</typeparam>
public interface ILazyProducer<TResult>
{
    /// <summary>
    /// Gets a value indicating the status of the result value creation.
    /// </summary>
    /// <value>A value indicating whether the underlying <typeparamref name="TResult"/> value creation process ran to completion, threw an exception, or has not yet been invoked.</value>
    LazyInitState State { get; }
    
    /// <summary>
    /// Gets a value that indicates whether the underlying process that creates the <typeparamref name="TResult"/> value has already been invoked.
    /// </summary>
    /// <value><see langword="true"/> if the underlying process that creates the <typeparamref name="TResult"/> value has already been invoked; otherwise <see langword="false"/>.</value>
    bool WasInvoked { get; }

    /// <summary>
    /// Gets the exception, if any, that was thrown by the underlying <typeparamref name="TResult"/> value creation process.
    /// </summary>
    /// <returns>The exception thrown by the underlying <typeparamref name="TResult"/> value creation process or <see langword="null"/> if that process ran to completion.</returns>
    Exception? GetFault();

    /// <summary>
    /// Gets the result value.
    /// </summary>
    /// <returns>The <typeparamref name="TResult"/> value.</returns>
    /// <exception cref="Exception">The exception thrown by the underlying <typeparamref name="TResult"/> value creation process.</exception>
    /// <remarks>If the underlying <typeparamref name="TResult"/> value creation process has not already been invoked, this will cause it to be invoked.
    /// <para>Any exception that was thrown during the <typeparamref name="TResult"/> value creation process will be re-thrown when this method is subsequently invoked.</para></remarks>
    TResult GetResult();

    /// <summary>
    /// Gets the result value.
    /// </summary>
    /// <param name="onSuccess">Delegate to invoke if the <typeparamref name="TResult"/> value was successfully created.</param>
    /// <param name="whenFaulted">Delegate to invoke if the underlying <typeparamref name="TResult"/> value creation process threw an exception.</param>
    /// <remarks>The <paramref name="onSuccess"/> or <paramref name="whenFaulted"/> delegate will be invoked regardless of whether this caused the underlying <typeparamref name="TResult"/>
    /// value creation process to be invoked.</remarks>
    void GetResult(Action<TResult> onSuccess, Action<Exception> whenFaulted);

    /// <summary>
    /// Ensures that the underlying <typeparamref name="TResult"/> value creation process has been invoked.
    /// </summary>
    /// <returns><see langword="true"/> if this caused the underlying <typeparamref name="TResult"/> value creation process to be invoked; otherwise, <see langword="false"/> if it had already been invoked.</returns>
    bool Invoke();

    /// <summary>
    /// Ensures that the underlying <typeparamref name="TResult"/> value creation process has been invoked.
    /// </summary>
    /// <param name="ranToCompletion"><see langword="true"/> if the underlying <typeparamref name="TResult"/> value creation process ran to completion;
    /// otherwise, <see langword="false"/> if that process threw an exception.</param>
    /// <returns><see langword="true"/> if this caused the underlying <typeparamref name="TResult"/> value creation process to be invoked; otherwise, <see langword="false"/> if it had already been invoked.</returns>
    /// <remarks>The <paramref name="isFaulted"/> will indicate whether the underlying <typeparamref name="TResult"/> value creation process threw an exception, regardless of whether
    /// this caused that process to be invoked.</remarks>
    bool Invoke(out bool ranToCompletion);

    /// <summary>
    /// Ensures that the underlying <typeparamref name="TResult"/> value creation process has been invoked.
    /// </summary>
    /// <param name="onSuccess">Delegate to invoke if the <typeparamref name="TResult"/> value was successfully created.</param>
    /// <param name="whenFaulted">Delegate to invoke if the underlying <typeparamref name="TResult"/> value creation process threw an exception.</param>
    /// <returns><see langword="true"/> if this caused the underlying <typeparamref name="TResult"/> value creation process to be invoked; otherwise, <see langword="false"/> if it had already been invoked.</returns>
    /// <remarks>The <paramref name="onSuccess"/> or <paramref name="whenFaulted"/> delegate will be invoked regardless of whether this caused the underlying <typeparamref name="TResult"/>
    /// value creation process to be invoked.</remarks>
    bool Invoke(Action<TResult> onSuccess, Action<Exception> whenFaulted);

    /// <summary>
    /// Gets a value indicating whether a <typeparamref name="TResult"/> value was successfully created.
    /// </summary>
    /// <returns><see langword="true"/> if a <typeparamref name="TResult"/> value was successfully created; otherwise, <see langword="false"/>.</returns>
    /// <remarks>If the underlying <typeparamref name="TResult"/> value creation process has not already been invoked, this will cause it to be invoked.</remarks>
    bool Test();

    /// <summary>
    /// Gets a value indicating whether the underlying <typeparamref name="TResult"/> value creation process completed successfully.
    /// </summary>
    /// <param name="invokedValueCreationProcess"><see langword="true"/> if this caused the underlying <typeparamref name="TResult"/> value creation process to be invoked;
    /// otherwise, <see langword="false"/> if it had already been invoked.</param>
    /// <returns><see langword="true"/> if a <typeparamref name="TResult"/> value was successfully created; otherwise, <see langword="false"/>.</returns>
    /// <remarks>If the underlying <typeparamref name="TResult"/> value creation process has not already been invoked, this will cause it to be invoked.</remarks>
    bool Test(out bool invokedValueCreationProcess);

    /// <summary>
    /// Attempts to get the result value.
    /// </summary>
    /// <param name="result">The result value, which may be <see langword="null"/> if this returns <see langword="false"/>.</param>
    /// <returns><see langword="true"/> if the <typeparamref name="TResult"/> value was successfully created; otherwise, <see langword="true"/>.</returns>
    /// <remarks>If the underlying <typeparamref name="TResult"/> value creation process has not already been invoked, this will cause it to be invoked.</remarks>
    bool TryGetResult([MaybeNullWhen(false)] out TResult result);
}