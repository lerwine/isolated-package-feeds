using System.Diagnostics.CodeAnalysis;

namespace IsolatedPackageFeeds.Shared.LazyInit;

/// <summary>
/// Represents a deferred invocation of a delegate that optionally converts a <typeparamref name="TInput"/> value to the <typeparamref name="TResult"/> type.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="TryFunc{T, TResult}"/>
public class LazyOptionalDelegatedConversion<TInput, TResult>(TInput input, TryFunc<TInput, TResult?> tryFunc) : LazyOptionalConversion<TInput, TResult>(input)
{
    private readonly TryFunc<TInput, TResult?> _tryFunc = tryFunc;

    protected override bool TryConvert([NotNullWhen(true)] out TResult? result) => _tryFunc(Input, out result);
}

public class LazyOptionalReDelegatedInitializer<TSource, TResult>(TryFunc<TSource?> tryGetSource, TryFunc<TSource, TResult?> tryConvert) :
    LazyOptionalConversion<LazyOptionalDelegatedInitializer<TSource>, TResult>(new LazyOptionalDelegatedInitializer<TSource>(tryGetSource))
{
    private readonly TryFunc<TSource, TResult?> _tryConvert = tryConvert;

    protected override bool TryConvert([NotNullWhen(true)] out TResult? result)
    {
        if (Input.TryGetResult(out TSource? source))
            return _tryConvert(source, out result);
        result = default;
        return false;
    }
}

/// <summary>
/// Represents a deferred invocation of a delegate that optionally converts a <typeparamref name="TInput"/> value an additional conversion argument to the <typeparamref name="TResult"/> type.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="TryFunc{T1, T2, TResult}"/>
public class LazyOptionalDelegatedConversion<TInput, TArg1, TResult>(TInput input, TArg1 arg1, TryFunc<TInput, TArg1, TResult?> tryFunc) : LazyOptionalConversion<TInput, TArg1, TResult>(input, arg1)
{
    private readonly TryFunc<TInput, TArg1, TResult?> _tryFunc = tryFunc;

    protected override bool TryConvert([NotNullWhen(true)] out TResult? result) => _tryFunc(Input, Arg1, out result);
}

/// <summary>
/// Represents a deferred invocation of a delegate that optionally converts a <typeparamref name="TInput"/> value and two additional conversion arguments to the <typeparamref name="TResult"/> type.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="TryFunc{T1, T2, T3, TResult}"/>
public class LazyOptionalDelegatedConversion<TInput, TArg1, TArg2, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TryFunc<TInput, TArg1, TArg2, TResult?> tryFunc) :
    LazyOptionalConversion<TInput, TArg1, TArg2, TResult>(input, arg1, arg2)
{
    private readonly TryFunc<TInput, TArg1, TArg2, TResult?> _tryFunc = tryFunc;

    protected override bool TryConvert([NotNullWhen(true)] out TResult? result) => _tryFunc(Input, Arg1, Arg2, out result);
}

/// <summary>
/// Represents a deferred invocation of a delegate that optionally converts a <typeparamref name="TInput"/> value and three additional conversion arguments to the <typeparamref name="TResult"/> type.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="TryFunc{T1, T2, T3, T4, TResult}"/>
public class LazyOptionalDelegatedConversion<TInput, TArg1, TArg2, TArg3, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3, TryFunc<TInput, TArg1, TArg2, TArg3, TResult?> tryFunc) :
    LazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TResult>(input, arg1, arg2, arg3)
{
    private readonly TryFunc<TInput, TArg1, TArg2, TArg3, TResult?> _tryFunc = tryFunc;

    protected override bool TryConvert([NotNullWhen(true)] out TResult? result) => _tryFunc(Input, Arg1, Arg2, Arg3, out result);
}

/// <summary>
/// Represents a deferred invocation of a delegate that optionally converts a <typeparamref name="TInput"/> value and four additional conversion arguments to the <typeparamref name="TResult"/> type.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TArg4">The type of the fourth conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="TryFunc{T1, T2, T3, T4, T5, TResult}"/>
public class LazyOptionalDelegatedConversion<TInput, TArg1, TArg2, TArg3, TArg4, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TryFunc<TInput, TArg1, TArg2, TArg3, TArg4, TResult?> tryFunc) :
    LazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TResult>(input, arg1, arg2, arg3, arg4)
{
    private readonly TryFunc<TInput, TArg1, TArg2, TArg3, TArg4, TResult?> _tryFunc = tryFunc;

    protected override bool TryConvert([NotNullWhen(true)] out TResult? result) => _tryFunc(Input, Arg1, Arg2, Arg3, Arg4, out result);
}

/// <summary>
/// Represents a deferred invocation of a delegate that optionally converts a <typeparamref name="TInput"/> value and five additional conversion arguments to the <typeparamref name="TResult"/> type.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TArg4">The type of the fourth conversion argument.</typeparam>
/// <typeparam name="TArg5">The type of the fifth conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="TryFunc{T1, T2, T3, T4, T5, T6, TResult}"/>
public class LazyOptionalDelegatedConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5,
    TryFunc<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult?> tryFunc) :
    LazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(input, arg1, arg2, arg3, arg4, arg5)
{
    private readonly TryFunc<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult?> _tryFunc = tryFunc;

    protected override bool TryConvert([NotNullWhen(true)] out TResult? result) => _tryFunc(Input, Arg1, Arg2, Arg3, Arg4, Arg5, out result);
}

/// <summary>
/// Represents a deferred invocation of a delegate that optionally converts a <typeparamref name="TInput"/> value and six additional conversion arguments to the <typeparamref name="TResult"/> type.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TArg4">The type of the fourth conversion argument.</typeparam>
/// <typeparam name="TArg5">The type of the fifth conversion argument.</typeparam>
/// <typeparam name="TArg6">The type of the sixth conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
/// <seealso cref="TryFunc{T1, T2, T3, T4, T5, T6, T7, TResult}"/>
public class LazyOptionalDelegatedConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6,
    TryFunc<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult?> tryFunc) :
    LazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(input, arg1, arg2, arg3, arg4, arg5, arg6)
{
    private readonly TryFunc<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult?> _tryFunc = tryFunc;

    protected override bool TryConvert([NotNullWhen(true)] out TResult? result) => _tryFunc(Input, Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, out result);
}

/// <summary>
/// Represents a deferred invocation of a delegate that optionally converts a <typeparamref name="TInput"/> value and seven additional conversion arguments to the <typeparamref name="TResult"/> type.
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
/// <seealso cref="TryFunc{T1, T2, T3, T4, T5, T6, T7, T8, TResult}"/>
public class LazyOptionalDelegatedConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(TInput input, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7,
    TryFunc<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult?> tryFunc) :
    LazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(input, arg1, arg2, arg3, arg4, arg5, arg6, arg7)
{
    private readonly TryFunc<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult?> _tryFunc = tryFunc;

    protected override bool TryConvert([NotNullWhen(true)] out TResult? result) => _tryFunc(Input, Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, out result);
}
