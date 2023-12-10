namespace IsolatedPackageFeeds.Shared.LazyInit;

/// <summary>
/// Represents a deferred invocation of a delegate that converts a <typeparamref name="TInput"/> valueto the <typeparamref name="TResult"/> type.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="Func{T, TResult}"/>
public class LazyDelegatedConversion<TInput, TResult>(TInput input, Func<TInput, TResult> func) : LazyConversion<TInput, TResult>(input)
{
    private readonly Func<TInput, TResult> _func = func;

    protected override TResult Convert() => _func(Input);
}

/// <summary>
/// Represents a deferred invocation of a delegate that converts a <typeparamref name="TInput"/> value and an additional conversion argument to the <typeparamref name="TResult"/> type.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="Func{T1, T2, TResult}"/>
public class LazyDelegatedConversion<TInput, TArg1, TResult>(TInput input, TArg1 arg1, Func<TInput, TArg1, TResult> func) :
    LazyConversion<TInput, TArg1, TResult>(input, arg1)
{
    private readonly Func<TInput, TArg1, TResult> _func = func;

    protected override TResult Convert() => _func(Input, Arg1);
}

/// <summary>
/// Represents a deferred invocation of a delegate that converts a <typeparamref name="TInput"/> value and two additional conversion arguments to the <typeparamref name="TResult"/> type.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="Func{T1, T2, T3, TResult}"/>
public class LazyDelegatedConversion<TInput, TArg1, TArg2, TResult>(TInput input, TArg1 arg1, TArg2 arg2, Func<TInput, TArg1, TArg2, TResult> func) :
    LazyConversion<TInput, TArg1, TArg2, TResult>(input, arg1, arg2)
{
    private readonly Func<TInput, TArg1, TArg2, TResult> _func = func;

    protected override TResult Convert() => _func(Input, Arg1, Arg2);
}

/// <summary>
/// Represents a deferred invocation of a delegate that converts a <typeparamref name="TInput"/> value and three additional conversion arguments to the <typeparamref name="TResult"/> type.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="Func{T1, T2, T3, T4, TResult}"/>
public class LazyDelegatedConversion<TInput, TArg1, TArg2, TArg3, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3, Func<TInput, TArg1, TArg2, TArg3, TResult> func) :
    LazyConversion<TInput, TArg1, TArg2, TArg3, TResult>(input, arg1, arg2, arg3)
{
    private readonly Func<TInput, TArg1, TArg2, TArg3, TResult> _func = func;

    protected override TResult Convert() => _func(Input, Arg1, Arg2, Arg3);
}

/// <summary>
/// Represents a deferred invocation of a delegate that converts a <typeparamref name="TInput"/> value and four additional conversion arguments to the <typeparamref name="TResult"/> type.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TArg4">The type of the fourth conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="Func{T1, T2, T3, T4, T5, TResult}"/>
public class LazyDelegatedConversion<TInput, TArg1, TArg2, TArg3, TArg4, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, Func<TInput, TArg1, TArg2, TArg3, TArg4, TResult> func) :
    LazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TResult>(input, arg1, arg2, arg3, arg4)
{
    private readonly Func<TInput, TArg1, TArg2, TArg3, TArg4, TResult> _func = func;

    protected override TResult Convert() => _func(Input, Arg1, Arg2, Arg3, Arg4);
}

/// <summary>
/// Represents a deferred invocation of a delegate that converts a <typeparamref name="TInput"/> value and five additional conversion arguments to the <typeparamref name="TResult"/> type.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TArg4">The type of the fourth conversion argument.</typeparam>
/// <typeparam name="TArg5">The type of the fifth conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="Func{T1, T2, T3, T4, T5, T6, TResult}"/>
public class LazyDelegatedConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5,
    Func<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult> func) : LazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(input, arg1, arg2, arg3, arg4, arg5)
{
    private readonly Func<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult> _func = func;

    protected override TResult Convert() => _func(Input, Arg1, Arg2, Arg3, Arg4, Arg5);
}

/// <summary>
/// Represents a deferred invocation of a delegate that converts a <typeparamref name="TInput"/> value and six additional conversion arguments to the <typeparamref name="TResult"/> type.
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
public class LazyDelegatedConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6,
    Func<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> func) : LazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(input, arg1, arg2, arg3, arg4, arg5, arg6)
{
    private readonly Func<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> _func = func;

    protected override TResult Convert() => _func(Input, Arg1, Arg2, Arg3, Arg4, Arg5, Arg6);
}

/// <summary>
/// Represents a deferred invocation of a delegate that converts a <typeparamref name="TInput"/> value and seven additional conversion arguments to the <typeparamref name="TResult"/> type.
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
public class LazyDelegatedConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7,
    Func<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> func) : LazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(input, arg1, arg2, arg3, arg4, arg5, arg6, arg7)
{
    private readonly Func<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> _func = func;

    protected override TResult Convert() => _func(Input, Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7);
}
