using System.Diagnostics.CodeAnalysis;

namespace IsolatedPackageFeeds.Shared;

/// <summary>
/// Encapsulates a method that has no input parameters, produces an output value, and returns a <see cref="bool"/> value indicating success or failure.
/// </summary>
/// <param name="result">The output value of the method that this delegate encapsulates, which may be <see langword="null"/> if this method returns <see langword="false"/>.</param>
/// <typeparam name="TOut">The output parameter type.</typeparam>
/// <returns><see langword="true"/> if the <paramref name="result"/> value was successfully produced; otherwise, <see langword="false"/> if the <paramref name="result"/> value should be disregarded.</returns>
/// <remarks>This can be likened to an optional <see cref="Func{TResult}"/> delegate.</remarks>
public delegate bool TryFunc<TOut>([MaybeNullWhen(false)] out TOut result);

/// <summary>
/// Encapsulates a method that one one input parameter, produces an output value, and returns a <see cref="bool"/> value indicating success or failure.
/// </summary>
/// <param name="arg">The input parameter of the method that this delegate encapsulates.</param>
/// <param name="result">The output value of the method that this delegate encapsulates.</param>
/// <typeparam name="T">The input parameter type.</typeparam>
/// <typeparam name="TOut">The output parameter type.</typeparam>
/// <returns><see langword="true"/> if the <paramref name="result"/>value  was successfully produced; otherwise, <see langword="false"/> if the <paramref name="result"/> value should be disregarded.</returns>
/// <remarks>This can be likened to an optional <see cref="Func{T, TResult}"/> delegate.</remarks>
public delegate bool TryFunc<in T, TOut>(T arg, [MaybeNullWhen(false)] out TOut result);

/// <summary>
/// Encapsulates a method that has two input parameters, produces an output value, and returns a <see cref="bool"/> value indicating success or failure.
/// </summary>
/// <param name="arg1">The first input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="result">The output value of the method that this delegate encapsulates.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="TOut">The output parameter type.</typeparam>
/// <returns><see langword="true"/> if the <paramref name="result"/> value was successfully produced; otherwise, <see langword="false"/> if the <paramref name="result"/> value should be disregarded.</returns>
/// <remarks>This can be likened to an optional <see cref="Func{T1, T2, TResult}"/> delegate.</remarks>
public delegate bool TryFunc<in T1, in T2, TOut>(T1 arg1, T2 arg2, [MaybeNullWhen(false)] out TOut result);

/// <summary>
/// Encapsulates a method that has three input parameters, produces an output value, and returns a <see cref="bool"/> value indicating success or failure.
/// </summary>
/// <param name="arg1">The first input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="result">The output value of the method that this delegate encapsulates.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="TOut">The output parameter type.</typeparam>
/// <returns><see langword="true"/> if the <paramref name="result"/> value was successfully produced; otherwise, <see langword="false"/> if the <paramref name="result"/> value should be disregarded.</returns>
/// <remarks>This can be likened to an optional <see cref="Func{T1, T2, T3, TResult}"/> delegate.</remarks>
public delegate bool TryFunc<in T1, in T2, in T3, TOut>(T1 arg1, T2 arg2, T3 arg3, [MaybeNullWhen(false)] out TOut result);

/// <summary>
/// Encapsulates a method that has four input parameters, produces an output value, and returns a <see cref="bool"/> value indicating success or failure.
/// </summary>
/// <param name="arg1">The first input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="result">The output value of the method that this delegate encapsulates.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="TOut">The output parameter type.</typeparam>
/// <returns><see langword="true"/> if the <paramref name="result"/> value was successfully produced; otherwise, <see langword="false"/> if the <paramref name="result"/> value should be disregarded.</returns>
/// <remarks>This can be likened to an optional <see cref="Func{T1, T2, T3, T4, TResult}"/> delegate.</remarks>
public delegate bool TryFunc<in T1, in T2, in T3, in T4, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, [MaybeNullWhen(false)] out TOut result);

/// <summary>
/// Encapsulates a method that has five input parameters, produces an output value, and returns a <see cref="bool"/> value indicating success or failure.
/// </summary>
/// <param name="arg1">The first input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="result">The output value of the method that this delegate encapsulates.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="T5">The fifth input parameter type.</typeparam>
/// <typeparam name="TOut">The output parameter type.</typeparam>
/// <returns><see langword="true"/> if the <paramref name="result"/> value was successfully produced; otherwise, <see langword="false"/> if the <paramref name="result"/> value should be disregarded.</returns>
/// <remarks>This can be likened to an optional <see cref="Func{T1, T2, T3, T4, T5, TResult}"/> delegate.</remarks>
public delegate bool TryFunc<in T1, in T2, in T3, in T4, in T5, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, [MaybeNullWhen(false)] out TOut result);

/// <summary>
/// Encapsulates a method that has six input parameters, produces an output value, and returns a <see cref="bool"/> value indicating success or failure.
/// </summary>
/// <param name="arg1">The first input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="result">The output value of the method that this delegate encapsulates.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="T5">The fifth input parameter type.</typeparam>
/// <typeparam name="T6">The sixth input parameter type.</typeparam>
/// <typeparam name="TOut">The output parameter type.</typeparam>
/// <returns><see langword="true"/> if the <paramref name="result"/> value was successfully produced; otherwise, <see langword="false"/> if the <paramref name="result"/> value should be disregarded.</returns>
/// <remarks>This can be likened to an optional <see cref="Func{T1, T2, T3, T4, T5, T6, TResult}"/> delegate.</remarks>
public delegate bool TryFunc<in T1, in T2, in T3, in T4, in T5, in T6, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, [MaybeNullWhen(false)] out TOut result);

/// <summary>
/// Encapsulates a method that has seven input parameters, produces an output value, and returns a <see cref="bool"/> value indicating success or failure.
/// </summary>
/// <param name="arg1">The first input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="result">The output value of the method that this delegate encapsulates.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="T5">The fifth input parameter type.</typeparam>
/// <typeparam name="T6">The sixth input parameter type.</typeparam>
/// <typeparam name="T7">The seventh input parameter type.</typeparam>
/// <typeparam name="TOut">The output parameter type.</typeparam>
/// <returns><see langword="true"/> if the <paramref name="result"/> value was successfully produced; otherwise, <see langword="false"/> if the <paramref name="result"/> value should be disregarded.</returns>
/// <remarks>This can be likened to an optional <see cref="Func{T1, T2, T3, T4, T5, T6, T7, TResult}"/> delegate.</remarks>
public delegate bool TryFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, [MaybeNullWhen(false)] out TOut result);

/// <summary>
/// Encapsulates a method that has eight input parameters, produces an output value, and returns a <see cref="bool"/> value indicating success or failure.
/// </summary>
/// <param name="arg1">The first input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg8">The eighth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="result">The output value of the method that this delegate encapsulates.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="T5">The fifth input parameter type.</typeparam>
/// <typeparam name="T6">The sixth input parameter type.</typeparam>
/// <typeparam name="T7">The seventh input parameter type.</typeparam>
/// <typeparam name="T8">The eighth input parameter type.</typeparam>
/// <typeparam name="TOut">The output parameter type.</typeparam>
/// <returns><see langword="true"/> if the <paramref name="result"/> value was successfully produced; otherwise, <see langword="false"/> if the <paramref name="result"/> value should be disregarded.</returns>
/// <remarks>This can be likened to an optional <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, TResult}"/> delegate.</remarks>
public delegate bool TryFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, [MaybeNullWhen(false)] out TOut result);

/// <summary>
/// Encapsulates a method that has nine input parameters, produces an output value, and returns a <see cref="bool"/> value indicating success or failure.
/// </summary>
/// <param name="arg1">The first input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg8">The eighth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg9">The ninth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="result">The output value of the method that this delegate encapsulates.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="T5">The fifth input parameter type.</typeparam>
/// <typeparam name="T6">The sixth input parameter type.</typeparam>
/// <typeparam name="T7">The seventh input parameter type.</typeparam>
/// <typeparam name="T8">The eighth input parameter type.</typeparam>
/// <typeparam name="T9">The ninth input parameter type.</typeparam>
/// <typeparam name="TOut">The output parameter type.</typeparam>
/// <returns><see langword="true"/> if the <paramref name="result"/> value was successfully produced; otherwise, <see langword="false"/> if the <paramref name="result"/> value should be disregarded.</returns>
/// <remarks>This can be likened to an optional <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult}"/> delegate.</remarks>
public delegate bool TryFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, [MaybeNullWhen(false)] out TOut result);

/// <summary>
/// Encapsulates a method that has ten input parameters, produces an output value, and returns a <see cref="bool"/> value indicating success or failure.
/// </summary>
/// <param name="arg1">The first input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg8">The eighth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg9">The ninth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg10">The tenth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="result">The output value of the method that this delegate encapsulates.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="T5">The fifth input parameter type.</typeparam>
/// <typeparam name="T6">The sixth input parameter type.</typeparam>
/// <typeparam name="T7">The seventh input parameter type.</typeparam>
/// <typeparam name="T8">The eighth input parameter type.</typeparam>
/// <typeparam name="T9">The ninth input parameter type.</typeparam>
/// <typeparam name="T10">The tenth input parameter type.</typeparam>
/// <typeparam name="TOut">The output parameter type.</typeparam>
/// <returns><see langword="true"/> if the <paramref name="result"/> value was successfully produced; otherwise, <see langword="false"/> if the <paramref name="result"/> value should be disregarded.</returns>
/// <remarks>This can be likened to an optional <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult}"/> delegate.</remarks>
public delegate bool TryFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10,
    [MaybeNullWhen(false)] out TOut result);

/// <summary>
/// Encapsulates a method that has eleven input parameters, produces an output value, and returns a <see cref="bool"/> value indicating success or failure.
/// </summary>
/// <param name="arg1">The first input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg8">The eighth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg9">The ninth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg10">The tenth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg11">The eleventh input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="result">The output value of the method that this delegate encapsulates.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="T5">The fifth input parameter type.</typeparam>
/// <typeparam name="T6">The sixth input parameter type.</typeparam>
/// <typeparam name="T7">The seventh input parameter type.</typeparam>
/// <typeparam name="T8">The eighth input parameter type.</typeparam>
/// <typeparam name="T9">The ninth input parameter type.</typeparam>
/// <typeparam name="T10">The tenth input parameter type.</typeparam>
/// <typeparam name="T11">The eleventh input parameter type.</typeparam>
/// <typeparam name="TOut">The output parameter type.</typeparam>
/// <returns><see langword="true"/> if the <paramref name="result"/> value was successfully produced; otherwise, <see langword="false"/> if the <paramref name="result"/> value should be disregarded.</returns>
/// <remarks>This can be likened to an optional <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult}"/> delegate.</remarks>
public delegate bool TryFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11,
    [MaybeNullWhen(false)] out TOut result);

/// <summary>
/// Encapsulates a method that has twelve input parameters, produces an output value, and returns a <see cref="bool"/> value indicating success or failure.
/// </summary>
/// <param name="arg1">The first input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg8">The eighth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg9">The ninth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg10">The tenth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg11">The eleventh input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg12">The twelfth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="result">The output value of the method that this delegate encapsulates.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="T5">The fifth input parameter type.</typeparam>
/// <typeparam name="T6">The sixth input parameter type.</typeparam>
/// <typeparam name="T7">The seventh input parameter type.</typeparam>
/// <typeparam name="T8">The eighth input parameter type.</typeparam>
/// <typeparam name="T9">The ninth input parameter type.</typeparam>
/// <typeparam name="T10">The tenth input parameter type.</typeparam>
/// <typeparam name="T11">The eleventh input parameter type.</typeparam>
/// <typeparam name="T12">The twelfth input parameter type.</typeparam>
/// <typeparam name="TOut">The output parameter type.</typeparam>
/// <returns><see langword="true"/> if the <paramref name="result"/> value was successfully produced; otherwise, <see langword="false"/> if the <paramref name="result"/> value should be disregarded.</returns>
/// <remarks>This can be likened to an optional <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult}"/> delegate.</remarks>
public delegate bool TryFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10,
    T11 arg11, T12 arg12, [MaybeNullWhen(false)] out TOut result);

/// <summary>
/// Encapsulates a method that has thirteen input parameters, produces an output value, and returns a <see cref="bool"/> value indicating success or failure.
/// </summary>
/// <param name="arg1">The first input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg8">The eighth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg9">The ninth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg10">The tenth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg11">The eleventh input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg12">The twelfth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg13">The thirteenth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="result">The output value of the method that this delegate encapsulates.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="T5">The fifth input parameter type.</typeparam>
/// <typeparam name="T6">The sixth input parameter type.</typeparam>
/// <typeparam name="T7">The seventh input parameter type.</typeparam>
/// <typeparam name="T8">The eighth input parameter type.</typeparam>
/// <typeparam name="T9">The ninth input parameter type.</typeparam>
/// <typeparam name="T10">The tenth input parameter type.</typeparam>
/// <typeparam name="T11">The eleventh input parameter type.</typeparam>
/// <typeparam name="T12">The twelfth input parameter type.</typeparam>
/// <typeparam name="T13">The thirteenth input parameter type.</typeparam>
/// <typeparam name="TOut">The output parameter type.</typeparam>
/// <returns><see langword="true"/> if the <paramref name="result"/> value was successfully produced; otherwise, <see langword="false"/> if the <paramref name="result"/> value should be disregarded.</returns>
/// <remarks>This can be likened to an optional <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult}"/> delegate.</remarks>
public delegate bool TryFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9,
    T10 arg10, T11 arg11, T12 arg12, T13 arg13, [MaybeNullWhen(false)] out TOut result);

/// <summary>
/// Encapsulates a method that has fourteen input parameters, produces an output value, and returns a <see cref="bool"/> value indicating success or failure.
/// </summary>
/// <param name="arg1">The first input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg8">The eighth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg9">The ninth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg10">The tenth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg11">The eleventh input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg12">The twelfth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg13">The thirteenth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg14">The fourteenth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="result">The output value of the method that this delegate encapsulates.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="T5">The fifth input parameter type.</typeparam>
/// <typeparam name="T6">The sixth input parameter type.</typeparam>
/// <typeparam name="T7">The seventh input parameter type.</typeparam>
/// <typeparam name="T8">The eighth input parameter type.</typeparam>
/// <typeparam name="T9">The ninth input parameter type.</typeparam>
/// <typeparam name="T10">The tenth input parameter type.</typeparam>
/// <typeparam name="T11">The eleventh input parameter type.</typeparam>
/// <typeparam name="T12">The twelfth input parameter type.</typeparam>
/// <typeparam name="T13">The thirteenth input parameter type.</typeparam>
/// <typeparam name="T14">The fourteenth input parameter type.</typeparam>
/// <typeparam name="TOut">The output parameter type.</typeparam>
/// <returns><see langword="true"/> if the <paramref name="result"/> value was successfully produced; otherwise, <see langword="false"/> if the <paramref name="result"/> value should be disregarded.</returns>
/// <remarks>This can be likened to an optional <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult}"/> delegate.</remarks>
public delegate bool TryFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9,
    T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, [MaybeNullWhen(false)] out TOut result);

/// <summary>
/// Encapsulates a method that has fifteen input parameters, produces an output value, and returns a <see cref="bool"/> value indicating success or failure.
/// </summary>
/// <param name="arg1">The first input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg8">The eighth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg9">The ninth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg10">The tenth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg11">The eleventh input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg12">The twelfth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg13">The thirteenth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg14">The fourteenth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg15">The fifteenth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="result">The output value of the method that this delegate encapsulates.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="T5">The fifth input parameter type.</typeparam>
/// <typeparam name="T6">The sixth input parameter type.</typeparam>
/// <typeparam name="T7">The seventh input parameter type.</typeparam>
/// <typeparam name="T8">The eighth input parameter type.</typeparam>
/// <typeparam name="T9">The ninth input parameter type.</typeparam>
/// <typeparam name="T10">The tenth input parameter type.</typeparam>
/// <typeparam name="T11">The eleventh input parameter type.</typeparam>
/// <typeparam name="T12">The twelfth input parameter type.</typeparam>
/// <typeparam name="T13">The thirteenth input parameter type.</typeparam>
/// <typeparam name="T14">The fourteenth input parameter type.</typeparam>
/// <typeparam name="T15">The fifteenth input parameter type.</typeparam>
/// <typeparam name="TOut">The output parameter type.</typeparam>
/// <returns><see langword="true"/> if the <paramref name="result"/> value was successfully produced; otherwise, <see langword="false"/> if the <paramref name="result"/> value should be disregarded.</returns>
/// <remarks>This can be likened to an optional <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult}"/> delegate.</remarks>
public delegate bool TryFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8,
    T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, [MaybeNullWhen(false)] out TOut result);

/// <summary>
/// Encapsulates a method that has sixteen input parameters, produces an output value, and returns a <see cref="bool"/> value indicating success or failure.
/// </summary>
/// <param name="arg1">The first input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg8">The eighth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg9">The ninth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg10">The tenth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg11">The eleventh input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg12">The twelfth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg13">The thirteenth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg14">The fourteenth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg15">The fifteenth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg16">The sixteenth input parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="result">The output value of the method that this delegate encapsulates.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="T5">The fifth input parameter type.</typeparam>
/// <typeparam name="T6">The sixth input parameter type.</typeparam>
/// <typeparam name="T7">The seventh input parameter type.</typeparam>
/// <typeparam name="T8">The eighth input parameter type.</typeparam>
/// <typeparam name="T9">The ninth input parameter type.</typeparam>
/// <typeparam name="T10">The tenth input parameter type.</typeparam>
/// <typeparam name="T11">The eleventh input parameter type.</typeparam>
/// <typeparam name="T12">The twelfth input parameter type.</typeparam>
/// <typeparam name="T13">The thirteenth input parameter type.</typeparam>
/// <typeparam name="T14">The fourteenth input parameter type.</typeparam>
/// <typeparam name="T15">The fifteenth input parameter type.</typeparam>
/// <typeparam name="T16">The sixteenth input parameter type.</typeparam>
/// <typeparam name="TOut">The output parameter type.</typeparam>
/// <returns><see langword="true"/> if the <paramref name="result"/> value was successfully produced; otherwise, <see langword="false"/> if the <paramref name="result"/> value should be disregarded.</returns>
/// <remarks>This can be likened to an optional <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult}"/> delegate.</remarks>
public delegate bool TryFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7,
    T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, [MaybeNullWhen(false)] out TOut result);
