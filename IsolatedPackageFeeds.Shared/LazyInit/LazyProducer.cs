using System.Diagnostics.CodeAnalysis;

namespace IsolatedPackageFeeds.Shared.LazyInit;

/// <summary>
/// Base class for deferred value creation.
/// </summary>
/// <typeparam name="TResult">The result value type.</typeparam>
/// <typeparam name="TState">The result value creation status type.</typeparam>
public abstract class LazyProducer<TResult, TState>(TState notInvoked)
    where TState : struct, Enum
{
    private const string CreationInProgressErrorMessage = "Cannot invoke value accessor method while value creation process is being executed";

    private readonly object _syncRoot = new();
    
    private bool _invoking;

    /// <summary>
    /// Gets the current exception value.
    /// </summary>
    /// <value>The current exception value, or <see langword="null"/> if <see cref="Initialize(out TResult)"/> hasn't been invoked or it did not throw an exception.</value>
    protected Exception? Fault { get; private set; }

    /// <summary>
    /// Gets the current result value
    /// </summary>
    /// <value>The current result value, which may be <see langword="null"/> or the defalt value if <see cref="Initialize(out TResult)"/> hasn't been invoked, didn't return a value, or it threw an exception.</value>
    protected TResult Result { get; private set; } = default!;

    /// <summary>
    /// Gets a value indicating the status of the result value creation.
    /// </summary>
    /// <value>A value indicating whether the underlying <typeparamref name="TResult"/> value creation process ran to completion, threw an exception, or has not yet been invoked.</value>
    public TState State { get; private set; } = notInvoked;

    /// <summary>
    /// Gets a value that indicates whether the underlying process that creates the <typeparamref name="TResult"/> value has already been invoked.
    /// </summary>
    /// <value><see langword="true"/> if the underlying process that creates the <typeparamref name="TResult"/> value has already been invoked; otherwise <see langword="false"/>.</value>
    public bool WasInvoked { get; private set; }

    /// <summary>
    /// Ensures that <see cref="Initialize(out TResult)"/> has been invoked.
    /// </summary>
    protected void Exec()
    {
        Monitor.Enter(_syncRoot);
        try
        {
            if (!WasInvoked)
            {
                _invoking = true;
                try
                {
                    State = Initialize(out TResult result);
                    Result = result;
                    if (!_invoking)
                        throw new InvalidOperationException(CreationInProgressErrorMessage);
                }
                catch (Exception fault)
                {
                    State = GetFaultedState();
                    Fault = fault;
                }
                finally
                {
                    WasInvoked = true;
                    _invoking = false;
                }
            }
        }
        finally { Monitor.Exit(_syncRoot); }
    }

    /// <summary>
    /// Gets the exception, if any, that was thrown by the underlying <typeparamref name="TResult"/> value creation process.
    /// </summary>
    /// <returns>The exception thrown by the underlying <typeparamref name="TResult"/> value creation process or <see langword="null"/> if that process ran to completion.</returns>
    public Exception? GetFault()
    {
        Exec();
        return Fault;
    }

    /// <summary>
    /// Gets the faulted status value.
    /// </summary>
    /// <returns>The <typeparamref name="TState"/> value indicating that the conversion process threw an exception.</returns>
    protected abstract TState GetFaultedState();

    /// <summary>
    /// Gets the result value.
    /// </summary>
    /// <returns>The <typeparamref name="TResult"/> value.</returns>
    /// <exception cref="Exception">The exception thrown by the underlying <typeparamref name="TResult"/> value creation process.</exception>
    /// <remarks>If the underlying <typeparamref name="TResult"/> value creation process has not already been invoked, this will cause it to be invoked.
    /// <para>Any exception that was thrown during the <typeparamref name="TResult"/> value creation process will be re-thrown when this method is subsequently invoked.</para></remarks>
    public TResult GetResult()
    {
        Monitor.Enter(_syncRoot);
        try
        {
            if (!WasInvoked)
            {
                _invoking = true;
                try
                {
                    State = Initialize(out TResult result);
                    Result = result;
                    if (!_invoking)
                        throw new InvalidOperationException(CreationInProgressErrorMessage);
                }
                catch (Exception fault)
                {
                    State = GetFaultedState();
                    Fault = fault;
                    throw;
                }
                finally
                {
                    WasInvoked = true;
                    _invoking = false;
                }
            }
        }
        finally { Monitor.Exit(_syncRoot); }
        return Result;
    }

    /// <summary>
    /// This gets called once to initialize the value.
    /// </summary>
    /// <param name="result">The initialized value.</param>
    /// <returns>The <typeparamref name="TState"/> value indicating the conversion process ran to completion.</returns>
    protected abstract TState Initialize(out TResult result);

    /// <summary>
    /// Ensures that the underlying <typeparamref name="TResult"/> value creation process has been invoked.
    /// </summary>
    /// <returns><see langword="true"/> if this caused the underlying <typeparamref name="TResult"/> value creation process to be invoked; otherwise, <see langword="false"/> if it had already been invoked.</returns>
    public bool Invoke()
    {
        Monitor.Enter(_syncRoot);
        try
        {
            if (WasInvoked)
                return false;
            _invoking = true;
            try
            {
                State = Initialize(out TResult result);
                Result = result;
                if (!_invoking)
                    throw new InvalidOperationException(CreationInProgressErrorMessage);
            }
            catch (Exception fault)
            {
                State = GetFaultedState();
                Fault = fault;
            }
            finally
            {
                WasInvoked = true;
                _invoking = false;
            }
        }
        finally { Monitor.Exit(_syncRoot); }
        return true;
    }

    /// <summary>
    /// Ensures that the underlying <typeparamref name="TResult"/> value creation process has been invoked.
    /// </summary>
    /// <param name="ranToCompletion"><see langword="true"/> if the underlying <typeparamref name="TResult"/> value creation process ran to completion;
    /// otherwise, <see langword="false"/> if that process threw an exception.</param>
    /// <returns><see langword="true"/> if this caused the underlying <typeparamref name="TResult"/> value creation process to be invoked; otherwise, <see langword="false"/> if it had already been invoked.</returns>
    /// <remarks>The <paramref name="isFaulted"/> will indicate whether the underlying <typeparamref name="TResult"/> value creation process threw an exception, regardless of whether
    /// this caused that process to be invoked.</remarks>
    public bool Invoke(out bool ranToCompletion)
    {
        bool wasInvoked;
        Monitor.Enter(_syncRoot);
        try
        {
            wasInvoked = !WasInvoked; 
            if (wasInvoked)
            {
                _invoking = true;
                try
                {
                    State = Initialize(out TResult result);
                    Result = result;
                    ranToCompletion = true;
                    if (!_invoking)
                        throw new InvalidOperationException(CreationInProgressErrorMessage);
                }
                catch (Exception fault)
                {
                    State = GetFaultedState();
                    Fault = fault;
                    ranToCompletion = false;
                }
                finally
                {
                    WasInvoked = true;
                    _invoking = false;
                }
            }
            else 
                ranToCompletion = Fault is null;
        }
        finally { Monitor.Exit(_syncRoot); }
        return wasInvoked;
    }
}

/// <summary>
/// Deferred value creation where the <typeparamref name="TResult"/> value is not created until it is actually retrieved.
/// </summary>
/// <typeparam name="TResult">The result value type.</typeparam>
public abstract class LazyProducer<TResult> : LazyProducer<TResult, LazyInitState>, ILazyProducer<TResult>
{
    protected LazyProducer() : base(LazyInitState.NotInvoked) { }

    /// <summary>
    /// This gets called when the result value needs to be created.
    /// </summary>
    /// <returns>The initialized result value.</returns>
    protected abstract TResult Initialize();

    protected override LazyInitState GetFaultedState() => LazyInitState.Faulted;

    protected override LazyInitState Initialize(out TResult result)
    {
        result = Initialize();
        return LazyInitState.SuccessfullyCompleted;
    }

    /// <summary>
    /// Gets the result value.
    /// </summary>
    /// <param name="onSuccess">Delegate to invoke if the <typeparamref name="TResult"/> value was successfully created.</param>
    /// <param name="whenFaulted">Delegate to invoke if the underlying <typeparamref name="TResult"/> value creation process threw an exception.</param>
    /// <remarks>The <paramref name="onSuccess"/> or <paramref name="whenFaulted"/> delegate will be invoked regardless of whether this caused the underlying <typeparamref name="TResult"/>
    /// value creation process to be invoked.</remarks>
    public void GetResult(Action<TResult> onSuccess, Action<Exception> whenFaulted)
    {
        Exec();
        if (Fault is null)
            onSuccess(Result);
        else
            whenFaulted(Fault);
    }

    /// <summary>
    /// Ensures that the underlying <typeparamref name="TResult"/> value creation process has been invoked.
    /// </summary>
    /// <param name="onSuccess">Delegate to invoke if the <typeparamref name="TResult"/> value was successfully created.</param>
    /// <param name="whenFaulted">Delegate to invoke if the underlying <typeparamref name="TResult"/> value creation process threw an exception.</param>
    /// <returns><see langword="true"/> if this caused the underlying <typeparamref name="TResult"/> value creation process to be invoked; otherwise, <see langword="false"/> if it had already been invoked.</returns>
    /// <remarks>The <paramref name="onSuccess"/> or <paramref name="whenFaulted"/> delegate will be invoked regardless of whether this caused the underlying <typeparamref name="TResult"/>
    /// value creation process to be invoked.</remarks>
    public bool Invoke(Action<TResult> onSuccess, Action<Exception> whenFaulted)
    {
        bool wasInvoked = Invoke();
        if (Fault is null)
            onSuccess(Result);
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
        return Fault is not null;
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
        return Fault is null;
    }

    /// <summary>
    /// Attempts to get the result value.
    /// </summary>
    /// <param name="result">The result value, which may be <see langword="null"/> if this returns <see langword="false"/>.</param>
    /// <returns><see langword="true"/> if the <typeparamref name="TResult"/> value was successfully created; otherwise, <see langword="true"/>.</returns>
    /// <remarks>If the underlying <typeparamref name="TResult"/> value creation process has not already been invoked, this will cause it to be invoked.</remarks>
    public bool TryGetResult([MaybeNullWhen(false)] out TResult result)
    {
        Exec();
        result = Result;
        return Fault is null;
    }
}
