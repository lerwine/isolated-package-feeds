namespace IsolatedPackageFeeds.Shared.LazyInit;

/// <summary>
/// Represents a deferred optional value conversion process that doesn't do a value conversion until a result value accessor is invoked.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public interface ILazyOptionalConversion<TInput, TResult> : ILazyConversion<TInput, TResult?>, ILazyOptionalInitializer<TResult>
{
}

/// <summary>
/// Represents a deferred optional value conversion process, that takes one conversion argument, and doesn't do a value conversion until a result value accessor is invoked.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the conversion argument value.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public interface ILazyOptionalConversion<TInput, TArg1, TResult> : ILazyConversion<TInput, TArg1, TResult?>, ILazyOptionalConversion<TInput, TResult>
{
}

/// <summary>
/// Represents a deferred optional value conversion process, that takes two conversion arguments, and doesn't do a value conversion until a result value accessor is invoked.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public interface ILazyOptionalConversion<TInput, TArg1, TArg2, TResult> : ILazyConversion<TInput, TArg1, TArg2, TResult?>, ILazyOptionalConversion<TInput, TArg1, TResult>
{
}

/// <summary>
/// Represents a deferred optional value conversion process, that takes three conversion arguments, and doesn't do a value conversion until a result value accessor is invoked.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public interface ILazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TResult> : ILazyConversion<TInput,  TArg1, TArg2, TArg3, TResult?>, ILazyOptionalConversion<TInput, TArg1, TArg2, TResult>
{
}

/// <summary>
/// Represents a deferred optional value conversion process, that takes four conversion arguments, and doesn't do a value conversion until a result value accessor is invoked.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TArg4">The type of the fourth conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public interface ILazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TResult> : ILazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TResult?>, ILazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TResult>
{
}

/// <summary>
/// Represents a deferred optional value conversion process, that takes five conversion arguments, and doesn't do a value conversion until a result value accessor is invoked.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TArg4">The type of the fourth conversion argument.</typeparam>
/// <typeparam name="TArg5">The type of the fifth conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public interface ILazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult> : ILazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult?>,
    ILazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TResult>
{
}

/// <summary>
/// Represents a deferred optional value conversion process, that takes sex conversion arguments, and doesn't do a value conversion until a result value accessor is invoked.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TArg1">The type of the first conversion argument.</typeparam>
/// <typeparam name="TArg2">The type of the second conversion argument.</typeparam>
/// <typeparam name="TArg3">The type of the third conversion argument.</typeparam>
/// <typeparam name="TArg4">The type of the fourth conversion argument.</typeparam>
/// <typeparam name="TArg5">The type of the fifth conversion argument.</typeparam>
/// <typeparam name="TArg6">The type of the sixth conversion argument.</typeparam>
/// <typeparam name="TResult">The type of result value.</typeparam>
public interface ILazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> : ILazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult?>,
    ILazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>
{
}

/// <summary>
/// Represents a deferred optional value conversion process, that takes seven conversion arguments, and doesn't do a value conversion until a result value accessor is invoked.
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
public interface ILazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> : ILazyConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult?>,
    ILazyOptionalConversion<TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>
{
}
