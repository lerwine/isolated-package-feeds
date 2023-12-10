using System.Diagnostics.CodeAnalysis;

namespace IsolatedPackageFeeds.Shared.LazyInit;

/// <summary>
/// Deferred optional value creation where the <typeparamref name="TResult"/> value is not created until it is actually retrieved.
/// </summary>
/// <typeparam name="TResult">The result value type.</typeparam>
public abstract class LazyOptionalProducer<TResult> : LazyProducer<TResult?, LazyOptionalInitState>, ILazyOptionalProducer<TResult>
{
    protected LazyOptionalProducer() : base(LazyOptionalInitState.NotInvoked) { }

    private bool _hasValue;
     
    LazyInitState ILazyProducer<TResult?>.State => State.ToLazyInitState();

    protected override LazyOptionalInitState GetFaultedState() => LazyOptionalInitState.Faulted;

    /// <summary>
    /// Gets the result value.
    /// </summary>
    /// <param name="onSuccess">Delegate to invoke if the <typeparamref name="TResult"/> value was successfully created.</param>
    /// <param name="whenNoValue">Delegate to invoke if the underlying <typeparamref name="TResult"/> value creation process ran to completion, but did not produce a value.</param>
    /// <param name="whenFaulted">Delegate to invoke if the underlying <typeparamref name="TResult"/> value creation process threw an exception.</param>
    /// <remarks>The <paramref name="onSuccess"/>, <paramref name="whenNoValue"/>, or <paramref name="whenFaulted"/> delegate will be invoked
    /// regardless of whether this caused the underlying <typeparamref name="TResult"/> value creation process to be invoked.</remarks>
    public void GetResult(Action<TResult> onSuccess, Action whenNoValue, Action<Exception>? whenFaulted = null)
    {
        Exec();
        if (_hasValue)
            onSuccess(Result!);
        else if (Fault is null)
            whenNoValue();
        else if (whenFaulted is not null)
            whenFaulted(Fault);
    }

    void ILazyProducer<TResult?>.GetResult(Action<TResult?> onSuccess, Action<Exception> whenFaulted)
    {
        Exec();
        if (Fault is null)
            onSuccess(Result);
        else
            whenFaulted(Fault);
    }

    protected override LazyOptionalInitState Initialize(out TResult? result)
    {
        _hasValue = TryInitialize(out result);
        return _hasValue ? LazyOptionalInitState.ValueProduced : LazyOptionalInitState.NoValue;
    }

    bool ILazyProducer<TResult?>.Invoke(Action<TResult?> onSuccess, Action<Exception> whenFaulted)
    {
        bool wasInvoked = Invoke();
        if (Fault is null)
            onSuccess(Result);
        else
            whenFaulted(Fault);
        return wasInvoked;
    }

    /// <summary>
    /// Ensures that the underlying <typeparamref name="TResult"/> value creation process has been invoked.
    /// </summary>
    /// <param name="onSuccess">Delegate to invoke if the <typeparamref name="TResult"/> value was successfully created.</param>
    /// <param name="whenNoValue">Delegate to invoke if the underlying <typeparamref name="TResult"/> value creation process ran to completion, but did not produce a value.</param>
    /// <param name="whenFaulted">Delegate to invoke if the underlying <typeparamref name="TResult"/> value creation process threw an exception.</param>
    /// <returns><see langword="true"/> if this caused the underlying <typeparamref name="TResult"/> value creation process to be invoked; otherwise, <see langword="false"/> if it had already been invoked.</returns>
    /// <remarks>The <paramref name="onSuccess"/>, <paramref name="whenNoValue"/>, or <paramref name="whenFaulted"/> delegate will be invoked
    /// regardless of whether this caused the underlying <typeparamref name="TResult"/> value creation process to be invoked.</remarks>
    public bool Invoke(Action<TResult> onSuccess, Action whenNoValue, Action<Exception> whenFaulted)
    {
        bool wasInvoked = Invoke();
        if (_hasValue)
            onSuccess(Result!);
        else if (Fault is null)
            whenNoValue();
        else
            whenFaulted(Fault);
        return wasInvoked;
    }

    /// <summary>
    /// Gets a value indicating whether a <typeparamref name="TResult"/> value was successfully created.
    /// </summary>
    /// <returns><see langword="true"/> if a <typeparamref name="TResult"/> value was successfully created; otherwise, <see langword="false"/>.</returns>
    /// <remarks>If the underlying <typeparamref name="TResult"/> value creation process has not already been invoked, this will cause it to be invoked.</remarks>
    public bool Test()
    {
        Exec();
        return _hasValue;
    }

    /// <summary>
    /// Gets a value indicating whether the underlying <typeparamref name="TResult"/> value creation process completed successfully.
    /// </summary>
    /// <param name="invokedValueCreationProcess"><see langword="true"/> if this caused the underlying <typeparamref name="TResult"/> value creation process to be invoked;
    /// otherwise, <see langword="false"/> if it had already been invoked.</param>
    /// <returns><see langword="true"/> if a <typeparamref name="TResult"/> value was successfully created; otherwise, <see langword="false"/>.</returns>
    /// <remarks>If the underlying <typeparamref name="TResult"/> value creation process has not already been invoked, this will cause it to be invoked.</remarks>
    public bool Test(out bool invokedValueCreationProcess)
    {
        invokedValueCreationProcess = Invoke();
        return _hasValue;
    }

    /// <summary>
    /// Attempts to get the result value.
    /// </summary>
    /// <param name="result">The result value, which may be <see langword="null"/> if this returns <see langword="false"/>.</param>
    /// <returns><see langword="true"/> if the <typeparamref name="TResult"/> value was successfully created; otherwise, <see langword="true"/>.</returns>
    /// <remarks>If the underlying <typeparamref name="TResult"/> value creation process has not already been invoked, this will cause it to be invoked.</remarks>
    public bool TryGetResult([NotNullWhen(true)] out TResult? result)
    {
        Exec();
        result = Result;
        return _hasValue;
    }

    /// <summary>
    /// This gets called when the result value needs to be created.
    /// </summary>
    /// <param name="result">The result value, which should be <see langword="null"/> or the <see langword="default"/> value if this returns <see langword="false"/>.</param>
    /// <returns><see langword="true"/> to indicate a <typeparamref name="TResult"/> value is being produced; otherwise, <see langword="false"/> to indicate there is no result value.</returns>
    protected abstract bool TryInitialize([NotNullWhen(true)] out TResult? result);
}
