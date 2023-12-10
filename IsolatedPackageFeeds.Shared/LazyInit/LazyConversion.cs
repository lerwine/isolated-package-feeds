using System.Diagnostics.CodeAnalysis;

namespace IsolatedPackageFeeds.Shared.LazyInit;

/// <summary>
/// Deferred value conversion where the value is not converted until the converted <typeparamref name="TResult"/> value is actually retrieved.
/// </summary>
/// <typeparam name="TInput">The input value type.</typeparam>
/// <typeparam name="TResult">The type of converted value.</typeparam>
public abstract class LazyConversion<TInput, TResult>(TInput input) : LazyProducer<TResult, LazyInitState>(LazyInitState.NotInvoked), ILazyConversion<TInput, TResult>
{
    /// <summary>
    /// The input value.
    /// </summary>
    public TInput Input { get; } = input;

    /// <summary>
    /// This gets called when <see cref="Input"/> value needs to be converted to a <typeparamref name="TResult"/> value.
    /// </summary>
    /// <returns>The <see cref="Input"/> value that was converted to a <typeparamref name="TResult"/> value</returns>
    protected abstract TResult Convert();

    protected override LazyInitState GetFaultedState() => LazyInitState.Faulted;

    /// <summary>
    /// Gets the result value.
    /// </summary>
    /// <param name="onSuccess">Delegate to invoke if the <typeparamref name="TResult"/> value was successfully converted from the <see cref="Input"/> value.</param>
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

    protected override LazyInitState Initialize(out TResult result)
    {
        result = Convert();
        return LazyInitState.SuccessfullyCompleted;
    }

    /// <summary>
    /// Ensures that the underlying <typeparamref name="TResult"/> value creation process has been invoked.
    /// </summary>
    /// <param name="onSuccess">Delegate to invoke if the <typeparamref name="TResult"/> value was successfully converted from the <see cref="input"/> value.</param>
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
    /// Gets a value indicating whether a <typeparamref name="TResult"/> value was successfully converted from the <see cref="input"/> value.
    /// </summary>
    /// <returns><see langword="true"/> if a <typeparamref name="TResult"/> value was successfully converted from the <see cref="input"/> value; otherwise, <see langword="false"/>.</returns>
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
    /// <returns><see langword="true"/> if a <typeparamref name="TResult"/> value was successfully converted from the <see cref="input"/> value; otherwise, <see langword="false"/>.</returns>
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
    /// <returns><see langword="true"/> if the <typeparamref name="TResult"/> value was successfully converted from the <see cref="input"/> value; otherwise, <see langword="true"/>.</returns>
    /// <remarks>If the underlying <typeparamref name="TResult"/> value creation process has not already been invoked, this will cause it to be invoked.</remarks>
    public bool TryGetResult([MaybeNullWhen(false)] out TResult result)
    {
        Exec();
        result = Result;
        return Fault is null;
    }
}

/// <summary>
/// Deferred value conversion that takes one additional argument, where the value is not converted until the converted <typeparamref name="TResult"/> value is actually retrieved.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public abstract class LazyConversion<TInput, TArg1, TResult>(TInput input, TArg1 arg1) : LazyConversion<TInput, TResult>(input), ILazyConversion<TInput, TArg1, TResult>
{
    /// <summary>
    /// The first conversion argument.
    /// </summary>
    public TArg1 Arg1 => arg1;
}

/// <summary>
/// Deferred value conversion that takes two additional arguments, where the value is not converted until the converted <typeparamref name="TResult"/> value is actually retrieved.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public abstract class LazyConversion<TInput, TArg1, TArg2, TResult>(TInput input, TArg1 arg1, TArg2 arg2) : LazyConversion<TInput, TArg1, TResult>(input, arg1), ILazyConversion<TInput, TArg1, TArg2, TResult>
{
    /// <summary>
    /// The second conversion argument.
    /// </summary>
    public TArg2 Arg2 => arg2;
}

/// <summary>
/// Deferred value conversion that takes three additional arguments, where the value is not converted until the converted <typeparamref name="TResult"/> value is actually retrieved.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public abstract class LazyConversion<TInput, TArg1, TArg2, TArg3, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3) : LazyConversion<TInput, TArg1, TArg2, TResult>(input, arg1, arg2),
    ILazyConversion<TInput, TArg1, TArg2, TArg3, TResult>
{
    /// <summary>
    /// The third conversion argument.
    /// </summary>
    public TArg3 Arg3 => arg3;
}

/// <summary>
/// Deferred value conversion that takes four additional arguments, where the value is not converted until the converted <typeparamref name="TResult"/> value is actually retrieved.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TArg4">The type of the fourth conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public abstract class LazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) :
    LazyConversion<TInput, TArg1, TArg2, TArg3, TResult>(input, arg1, arg2, arg3), ILazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TResult>
{
    /// <summary>
    /// The fourth conversion argument.
    /// </summary>
    public TArg4 Arg4 => arg4;
}

/// <summary>
/// Deferred value conversion that takes five additional arguments, where the value is not converted until the converted <typeparamref name="TResult"/> value is actually retrieved.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TArg4">The type of the fourth conversion argument.</typeparam>
/// <typeparam name="TArg5">The type of the fifth conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public abstract class LazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5) :
    LazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TResult>(input, arg1, arg2, arg3, arg4), ILazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>
{
    /// <summary>
    /// The fifth conversion argument.
    /// </summary>
    public TArg5 Arg5 => arg5;
}

/// <summary>
/// Deferred value conversion that takes six additional arguments, where the value is not converted until the converted <typeparamref name="TResult"/> value is actually retrieved.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TArg4">The type of the fourth conversion argument.</typeparam>
/// <typeparam name="TArg5">The type of the fifth conversion argument.</typeparam>
/// <typeparam name="TArg6">The type of the sixth conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public abstract class LazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6) :
    LazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(input, arg1, arg2, arg3, arg4, arg5), ILazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>
{
    /// <summary>
    /// The sixth conversion argument.
    /// </summary>
    public TArg6 Arg6 => arg6;
}

/// <summary>
/// Deferred value conversion that takes seven additional arguments, where the value is not converted until the converted <typeparamref name="TResult"/> value is actually retrieved.
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
public abstract class LazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7) :
    LazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(input, arg1, arg2, arg3, arg4, arg5, arg6),
    ILazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>
{
    /// <summary>
    /// The seventh conversion argument.
    /// </summary>
    public TArg7 Arg7 => arg7;
}
