using System.Diagnostics.CodeAnalysis;

namespace IsolatedPackageFeeds.Shared.LazyInit;

/// <summary>
/// Deferred optional value conversion where the value is not converted until the converted <typeparamref name="TResult"/> value is actually retrieved.
/// </summary>
/// <typeparam name="TInput">The input value type.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="Func{T, TResult}"/>
public abstract class LazyOptionalConversion<TInput, TResult>(TInput input) : LazyProducer<TResult?, LazyOptionalInitState>(LazyOptionalInitState.NotInvoked), ILazyOptionalConversion<TInput, TResult>
{
    /// <summary>
    /// The input value.
    /// </summary>
    public TInput Input { get; } = input;
    
    private bool _hasValue;

    LazyInitState ILazyProducer<TResult?>.State => State.ToLazyInitState();

    protected override LazyOptionalInitState GetFaultedState() => LazyOptionalInitState.Faulted;

    /// <summary>
    /// Gets the result value.
    /// </summary>
    /// <param name="onSuccess">Delegate to invoke if the <typeparamref name="TResult"/> value was successfully converted from the <see cref="input"/> value.</param>
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
        _hasValue = TryConvert(out result);
        return _hasValue ? LazyOptionalInitState.ValueProduced : LazyOptionalInitState.NoValue;
    }

    /// <summary>
    /// Ensures that the underlying <typeparamref name="TResult"/> value creation process has been invoked.
    /// </summary>
    /// <param name="onSuccess">Delegate to invoke if the <typeparamref name="TResult"/> value was successfully converted from the <see cref="input"/> value.</param>
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
    /// Gets a value indicating whether a <typeparamref name="TResult"/> value was successfully converted from the <see cref="input"/> value.
    /// </summary>
    /// <returns><see langword="true"/> if a <typeparamref name="TResult"/> value was successfully converted from the <see cref="input"/> value; otherwise, <see langword="false"/>.</returns>
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
    /// <returns><see langword="true"/> if a <typeparamref name="TResult"/> value was successfully converted from the <see cref="input"/> value; otherwise, <see langword="false"/>.</returns>
    /// <remarks>If the underlying <typeparamref name="TResult"/> value creation process has not already been invoked, this will cause it to be invoked.</remarks>
    public bool Test(out bool invokedValueCreationProcess)
    {
        invokedValueCreationProcess = Invoke();
        return _hasValue;
    }

    /// <summary>
    /// This gets called when <see cref="Input"/> value needs to be converted to a <typeparamref name="TResult"/> value.
    /// </summary>
    /// <param name="result">The converted value, which should be <see langword="null"/> or the <see langword="default"/> value if this returns <see langword="false"/>.</param>
    /// <returns><see langword="true"/> to indicate a <typeparamref name="TResult"/> value is being produced; otherwise, <see langword="false"/> to indicate there is no result value.</returns>
    protected abstract bool TryConvert([NotNullWhen(true)] out TResult? result);
    
    /// <summary>
    /// Attempts to get the result value.
    /// </summary>
    /// <param name="result">The result value, which may be <see langword="null"/> if this returns <see langword="false"/>.</param>
    /// <returns><see langword="true"/> if the <typeparamref name="TResult"/> value was successfully converted from the <see cref="input"/> value; otherwise, <see langword="true"/>.</returns>
    /// <remarks>If the underlying <typeparamref name="TResult"/> value creation process has not already been invoked, this will cause it to be invoked.</remarks>
    public bool TryGetResult([NotNullWhen(true)] out TResult? result)
    {
        Exec();
        result = Result;
        return _hasValue;
    }
}

/// <summary>
/// Deferred optional value conversion that takes an additional argument, where the value is not converted until the converted <typeparamref name="TResult"/> value is actually retrieved.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="Func{T1, T2, TResult}"/>
public abstract class LazyOptionalConversion<TInput, TArg1, TResult>(TInput input, TArg1 arg1) :
    LazyOptionalConversion<TInput, TResult>(input),
    ILazyOptionalConversion<TInput, TArg1, TResult>
{
    public TArg1 Arg1 => arg1;
}

/// <summary>
/// Deferred optional value conversion that takes two additional arguments, where the value is not converted until the converted <typeparamref name="TResult"/> value is actually retrieved.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="Func{T1, T2, T3, TResult}"/>
public abstract class LazyOptionalConversion<TInput, TArg1, TArg2, TResult>(TInput input, TArg1 arg1, TArg2 arg2) :
    LazyOptionalConversion<TInput, TArg1, TResult>(input, arg1),
    ILazyOptionalConversion<TInput, TArg1, TArg2, TResult>
{
    public TArg2 Arg2 => arg2;
}

/// <summary>
/// Deferred optional value conversion that takes three additional arguments, where the value is not converted until the converted <typeparamref name="TResult"/> value is actually retrieved.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="Func{T1, T2, T3, T4, TResult}"/>
public abstract class LazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3) :
    LazyOptionalConversion<TInput, TArg1, TArg2, TResult>(input, arg1, arg2),
    ILazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TResult>
{
    public TArg3 Arg3 => arg3;
}

/// <summary>
/// Deferred optional value conversion that takes four additional arguments, where the value is not converted until the converted <typeparamref name="TResult"/> value is actually retrieved.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TArg4">The type of the fourth conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="Func{T1, T2, T3, T4, T5, TResult}"/>
public abstract class LazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) :
    LazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TResult>(input, arg1, arg2, arg3),
    ILazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TResult>
{
    public TArg4 Arg4 => arg4;
}

/// <summary>
/// Deferred optional value conversion that takes five additional arguments, where the value is not converted until the converted <typeparamref name="TResult"/> value is actually retrieved.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TArg4">The type of the fourth conversion argument.</typeparam>
/// <typeparam name="TArg5">The type of the fifth conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="Func{T1, T2, T3, T4, T5, T6, TResult}"/>
public abstract class LazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5) :
    LazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TResult>(input, arg1, arg2, arg3, arg4),
    ILazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>
{
    public TArg5 Arg5 => arg5;
}

/// <summary>
/// Deferred optional value conversion that takes six additional arguments, where the value is not converted until the converted <typeparamref name="TResult"/> value is actually retrieved.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TArg4">The type of the fourth conversion argument.</typeparam>
/// <typeparam name="TArg5">The type of the fifth conversion argument.</typeparam>
/// <typeparam name="TArg6">The type of the sixth conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="Func{T1, T2, T3, T4, T5, T6, T7, TResult}"/>
public abstract class LazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6) :
    LazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(input, arg1, arg2, arg3, arg4, arg5),
    ILazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>
{
    public TArg6 Arg6 => arg6;
}

/// <summary>
/// Deferred optional value conversion that takes seven additional arguments, where the value is not converted until the converted <typeparamref name="TResult"/> value is actually retrieved.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TArg4">The type of the fourth conversion argument.</typeparam>
/// <typeparam name="TArg5">The type of the fifth conversion argument.</typeparam>
/// <typeparam name="TArg6">The type of the sixth conversion argument.</typeparam>
/// <typeparam name="TArg7">The type of the seventh conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, TResult}"/>
public abstract class LazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7) :
    LazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(input, arg1, arg2, arg3, arg4, arg5, arg6),
    ILazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>
{
    public TArg7 Arg7 => arg7;
}
