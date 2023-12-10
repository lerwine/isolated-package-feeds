namespace IsolatedPackageFeeds.Shared.LazyInit;

/// <summary>
/// Represents a deferred value conversion process that doesn't do a value conversion until a result value accessor is invoked.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public interface ILazyConversion<TInput, TResult> : ILazyProducer<TResult>
{
    /// <summary>
    /// The input value.
    /// </summary>
    TInput Input { get; }
}

/// <summary>
/// Represents a deferred value conversion process, with one conversion argument, that doesn't do a value conversion until a result value accessor is invoked.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the conversion argument value.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public interface ILazyConversion<TInput, TArg1, TResult> : ILazyConversion<TInput, TResult>
{
    /// <summary>
    /// The first conversion argument.
    /// </summary>
    TArg1 Arg1 { get; }
}

/// <summary>
/// Represents a deferred value conversion process, with two conversion arguments, that doesn't do a value conversion until a result value accessor is invoked.
/// </summary>
/// <typeparam name="TInput">The type of the first input value</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public interface ILazyConversion<TInput, TArg1, TArg2, TResult> : ILazyConversion<TInput, TArg1, TResult>
{
    /// <summary>
    /// The second conversion argument.
    /// </summary>
    TArg2 Arg2 { get; }
}

/// <summary>
/// Represents a deferred value conversion process, with three conversion arguments, that doesn't do a value conversion until a result value accessor is invoked.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public interface ILazyConversion<TInput, TArg1, TArg2, TArg3, TResult> : ILazyConversion<TInput, TArg1, TArg2, TResult>
{
    /// <summary>
    /// The third conversion argument.
    /// </summary>
    TArg3 Arg3 { get; }
}

/// <summary>
/// Represents a deferred value conversion process, with four conversion arguments, that doesn't do a value conversion until a result value accessor is invoked.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TArg4">The type of the fourth conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public interface ILazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TResult> : ILazyConversion<TInput, TArg1, TArg2, TArg3, TResult>
{
    /// <summary>
    /// The fourth conversion argument.
    /// </summary>
    TArg4 Arg4 { get; }
}

/// <summary>
/// Represents a deferred value conversion process, with five conversion arguments, that doesn't do a value conversion until a result value accessor is invoked.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TArg4">The type of the fourth conversion argument.</typeparam>
/// <typeparam name="TArg5">The type of the fifth conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public interface ILazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult> : ILazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TResult>
{
    /// <summary>
    /// The fifth conversion argument.
    /// </summary>
    TArg5 Arg5 { get; }
}

/// <summary>
/// Represents a deferred value conversion process, with six conversion arguments, that doesn't do a value conversion until a result value accessor is invoked.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TArg4">The type of the fourth conversion argument.</typeparam>
/// <typeparam name="TArg5">The type of the fifth conversion argument.</typeparam>
/// <typeparam name="TArg6">The type of the sixth conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public interface ILazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> : ILazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>
{
    /// <summary>
    /// The sixth conversion argument.
    /// </summary>
    TArg6 Arg6 { get; }
}

/// <summary>
/// Represents a deferred value conversion process, with seven conversion arguments, that doesn't do a value conversion until a result value accessor is invoked.
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
public interface ILazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> : ILazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>
{
    /// <summary>
    /// The seventh conversion argument.
    /// </summary>
    TArg7 Arg7 { get; }
}
