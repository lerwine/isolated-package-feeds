namespace IsolatedPackageFeeds.Shared;

/// <summary>
/// Encapsulates an asynchronous method that only has a <see cref="cancellationtoken" /> parameter and asynchronously returns a value.
/// </summary>
/// <param name="cancellationToken">The cancellation token to be observed during asynchronous execution.</param>
/// <typeparam name="TResult">The result value of the <see cref="Task{TResult}"/> that this encapsulated delegate returns.</typeparam>
/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous process that will produce the <typeparamref name="TResult"/> value.</returns>
/// <remarks>This can be likened to an asynchronous <see cref="Func{T, TResult}"/> delegate.</remarks>
public delegate Task<TResult> AsyncFunc<TResult>(CancellationToken cancellationToken);

/// <summary>
/// Encapsulates an asynchronous method that has one input parameter, plus a <see cref="cancellationtoken" /> parameter, and asynchronously returns a value.
/// </summary>
/// <param name="arg">The input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="cancellationToken">The cancellation token to be observed during asynchronous execution.</param>
/// <typeparam name="T">The input parameter type.</typeparam>
/// <typeparam name="TResult">The result value of the <see cref="Task{TResult}"/> that this encapsulated delegate returns.</typeparam>
/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous process that will produce the <typeparamref name="TResult"/> value.</returns>
/// <remarks>This can be likened to an asynchronous <see cref="Func{T, TResult}"/> delegate.</remarks>
public delegate Task<TResult> AsyncFunc<in T, TResult>(T arg, CancellationToken cancellationToken);

/// <summary>
/// Encapsulates an asynchronous method that has two input parameters, plus a <see cref="cancellationtoken" /> parameter, and asynchronously returns a value.
/// </summary>
/// <param name="arg1">The first input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="cancellationToken">The cancellation token to be observed during asynchronous execution.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="TResult">The result value of the <see cref="Task{TResult}"/> that this encapsulated delegate returns.</typeparam>
/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous process that will produce the <typeparamref name="TResult"/> value.</returns>
/// <remarks>This can be likened to an asynchronous <see cref="Func{T1, T2, TResult}"/> delegate.</remarks>
public delegate Task<TResult> AsyncFunc<in T1, in T2, TResult>(T1 arg1, T2 arg2, CancellationToken cancellationToken);

/// <summary>
/// Encapsulates an asynchronous method that has three input parameters, plus a <see cref="cancellationtoken" /> parameter, and asynchronously returns a value.
/// </summary>
/// <param name="arg1">The first input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="cancellationToken">The cancellation token to be observed during asynchronous execution.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="TResult">The result value of the <see cref="Task{TResult}"/> that this encapsulated delegate returns.</typeparam>
/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous process that will produce the <typeparamref name="TResult"/> value.</returns>
/// <remarks>This can be likened to an asynchronous <see cref="Func{T1, T2, T3, TResult}"/> delegate.</remarks>
public delegate Task<TResult> AsyncFunc<in T1, in T2, in T3, TResult>(T1 arg1, T2 arg2, T3 arg3, CancellationToken cancellationToken);

/// <summary>
/// Encapsulates an asynchronous method that has four input parameters, plus a <see cref="cancellationtoken" /> parameter, and asynchronously returns a value.
/// </summary>
/// <param name="arg1">The first input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="cancellationToken">The cancellation token to be observed during asynchronous execution.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="TResult">The result value of the <see cref="Task{TResult}"/> that this encapsulated delegate returns.</typeparam>
/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous process that will produce the <typeparamref name="TResult"/> value.</returns>
/// <remarks>This can be likened to an asynchronous <see cref="Func{T1, T2, T3, T4, TResult}"/> delegate.</remarks>
public delegate Task<TResult> AsyncFunc<in T1, in T2, in T3, in T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken cancellationToken);

/// <summary>
/// Encapsulates an asynchronous method that has five input parameters, plus a <see cref="cancellationtoken" /> parameter, and asynchronously returns a value.
/// </summary>
/// <param name="arg1">The first input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="cancellationToken">The cancellation token to be observed during asynchronous execution.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="T5">The fifth input parameter type.</typeparam>
/// <typeparam name="TResult">The result value of the <see cref="Task{TResult}"/> that this encapsulated delegate returns.</typeparam>
/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous process that will produce the <typeparamref name="TResult"/> value.</returns>
/// <remarks>This can be likened to an asynchronous <see cref="Func{T1, T2, T3, T4, T5, TResult}"/> delegate.</remarks>
public delegate Task<TResult> AsyncFunc<in T1, in T2, in T3, in T4, in T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken cancellationToken);

/// <summary>
/// Encapsulates an asynchronous method that has six input parameters, plus a <see cref="cancellationtoken" /> parameter, and asynchronously returns a value.
/// </summary>
/// <param name="arg1">The first input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="cancellationToken">The cancellation token to be observed during asynchronous execution.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="T5">The fifth input parameter type.</typeparam>
/// <typeparam name="T6">The sixth input parameter type.</typeparam>
/// <typeparam name="TResult">The result value of the <see cref="Task{TResult}"/> that this encapsulated delegate returns.</typeparam>
/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous process that will produce the <typeparamref name="TResult"/> value.</returns>
/// <remarks>This can be likened to an asynchronous <see cref="Func{T1, T2, T3, T4, T5, T6, TResult}"/> delegate.</remarks>
public delegate Task<TResult> AsyncFunc<in T1, in T2, in T3, in T4, in T5, in T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, CancellationToken cancellationToken);

/// <summary>
/// Encapsulates an asynchronous method that has seven input parameters, plus a <see cref="cancellationtoken" /> parameter, and asynchronously returns a value.
/// </summary>
/// <param name="arg1">The first input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="cancellationToken">The cancellation token to be observed during asynchronous execution.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="T5">The fifth input parameter type.</typeparam>
/// <typeparam name="T6">The sixth input parameter type.</typeparam>
/// <typeparam name="T7">The seventh input parameter type.</typeparam>
/// <typeparam name="TResult">The result value of the <see cref="Task{TResult}"/> that this encapsulated delegate returns.</typeparam>
/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous process that will produce the <typeparamref name="TResult"/> value.</returns>
/// <remarks>This can be likened to an asynchronous <see cref="Func{T1, T2, T3, T4, T5, T6, T7, TResult}"/> delegate.</remarks>
public delegate Task<TResult> AsyncFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, CancellationToken cancellationToken);

/// <summary>
/// Encapsulates an asynchronous method that has eight input parameters, plus a <see cref="cancellationtoken" /> parameter, and asynchronously returns a value.
/// </summary>
/// <param name="arg1">The first input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg8">The eighth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="cancellationToken">The cancellation token to be observed during asynchronous execution.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="T5">The fifth input parameter type.</typeparam>
/// <typeparam name="T6">The sixth input parameter type.</typeparam>
/// <typeparam name="T7">The seventh input parameter type.</typeparam>
/// <typeparam name="T8">The eighth input parameter type.</typeparam>
/// <typeparam name="TResult">The result value of the <see cref="Task{TResult}"/> that this encapsulated delegate returns.</typeparam>
/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous process that will produce the <typeparamref name="TResult"/> value.</returns>
/// <remarks>This can be likened to an asynchronous <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, TResult}"/> delegate.</remarks>
public delegate Task<TResult> AsyncFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, CancellationToken cancellationToken);

/// <summary>
/// Encapsulates an asynchronous method that has nine input parameters, plus a <see cref="cancellationtoken" /> parameter, and asynchronously returns a value.
/// </summary>
/// <param name="arg1">The first input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg8">The eighth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg9">The ninth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="cancellationToken">The cancellation token to be observed during asynchronous execution.</param>
/// <typeparam name="T1">The first input parameter type.</typeparam>
/// <typeparam name="T2">The second input parameter type.</typeparam>
/// <typeparam name="T3">The third input parameter type.</typeparam>
/// <typeparam name="T4">The fourth input parameter type.</typeparam>
/// <typeparam name="T5">The fifth input parameter type.</typeparam>
/// <typeparam name="T6">The sixth input parameter type.</typeparam>
/// <typeparam name="T7">The seventh input parameter type.</typeparam>
/// <typeparam name="T8">The eighth input parameter type.</typeparam>
/// <typeparam name="T9">The ninth input parameter type.</typeparam>
/// <typeparam name="TResult">The result value of the <see cref="Task{TResult}"/> that this encapsulated delegate returns.</typeparam>
/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous process that will produce the <typeparamref name="TResult"/> value.</returns>
/// <remarks>This can be likened to an asynchronous <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult}"/> delegate.</remarks>
public delegate Task<TResult> AsyncFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9,
    CancellationToken cancellationToken);

/// <summary>
/// Encapsulates an asynchronous method that has ten input parameters, plus a <see cref="cancellationtoken" /> parameter, and asynchronously returns a value.
/// </summary>
/// <param name="arg1">The first input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg8">The eighth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg9">The ninth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg10">The tenth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="cancellationToken">The cancellation token to be observed during asynchronous execution.</param>
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
/// <typeparam name="TResult">The result value of the <see cref="Task{TResult}"/> that this encapsulated delegate returns.</typeparam>
/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous process that will produce the <typeparamref name="TResult"/> value.</returns>
/// <remarks>This can be likened to an asynchronous <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult}"/> delegate.</remarks>
public delegate Task<TResult> AsyncFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10,
    CancellationToken cancellationToken);

/// <summary>
/// Encapsulates an asynchronous method that has eleven input parameters, plus a <see cref="cancellationtoken" /> parameter, and asynchronously returns a value.
/// </summary>
/// <param name="arg1">The first input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg8">The eighth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg9">The ninth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg10">The tenth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg11">The eleventh input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="cancellationToken">The cancellation token to be observed during asynchronous execution.</param>
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
/// <typeparam name="TResult">The result value of the <see cref="Task{TResult}"/> that this encapsulated delegate returns.</typeparam>
/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous process that will produce the <typeparamref name="TResult"/> value.</returns>
/// <remarks>This can be likened to an asynchronous <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult}"/> delegate.</remarks>
public delegate Task<TResult> AsyncFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10,
    T11 arg11, CancellationToken cancellationToken);

/// <summary>
/// Encapsulates an asynchronous method that has twelve input parameters, plus a <see cref="cancellationtoken" /> parameter, and asynchronously returns a value.
/// </summary>
/// <param name="arg1">The first input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg8">The eighth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg9">The ninth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg10">The tenth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg11">The eleventh input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg12">The twelfth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="cancellationToken">The cancellation token to be observed during asynchronous execution.</param>
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
/// <typeparam name="TResult">The result value of the <see cref="Task{TResult}"/> that this encapsulated delegate returns.</typeparam>
/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous process that will produce the <typeparamref name="TResult"/> value.</returns>
/// <remarks>This can be likened to an asynchronous <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult}"/> delegate.</remarks>
public delegate Task<TResult> AsyncFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9,
    T10 arg10, T11 arg11, T12 arg12, CancellationToken cancellationToken);

/// <summary>
/// Encapsulates an asynchronous method that has thirteen input parameters, plus a <see cref="cancellationtoken" /> parameter, and asynchronously returns a value.
/// </summary>
/// <param name="arg1">The first input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg8">The eighth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg9">The ninth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg10">The tenth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg11">The eleventh input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg12">The twelfth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg13">The thirteenth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="cancellationToken">The cancellation token to be observed during asynchronous execution.</param>
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
/// <typeparam name="TResult">The result value of the <see cref="Task{TResult}"/> that this encapsulated delegate returns.</typeparam>
/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous process that will produce the <typeparamref name="TResult"/> value.</returns>
/// <remarks>This can be likened to an asynchronous <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult}"/> delegate.</remarks>
public delegate Task<TResult> AsyncFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8,
    T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, CancellationToken cancellationToken);

/// <summary>
/// Encapsulates an asynchronous method that has fourteen input parameters, plus a <see cref="cancellationtoken" /> parameter, and asynchronously returns a value.
/// </summary>
/// <param name="arg1">The first input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg8">The eighth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg9">The ninth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg10">The tenth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg11">The eleventh input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg12">The twelfth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg13">The thirteenth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg14">The fourteenth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="cancellationToken">The cancellation token to be observed during asynchronous execution.</param>
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
/// <typeparam name="TResult">The result value of the <see cref="Task{TResult}"/> that this encapsulated delegate returns.</typeparam>
/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous process that will produce the <typeparamref name="TResult"/> value.</returns>
/// <remarks>This can be likened to an asynchronous <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult}"/> delegate.</remarks>
public delegate Task<TResult> AsyncFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7,
    T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, CancellationToken cancellationToken);

/// <summary>
/// Encapsulates an asynchronous method that has fifteen input parameters, plus a <see cref="cancellationtoken" /> parameter, and asynchronously returns a value.
/// </summary>
/// <param name="arg1">The first input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg8">The eighth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg9">The ninth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg10">The tenth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg11">The eleventh input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg12">The twelfth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg13">The thirteenth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg14">The fourteenth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg15">The fifteenth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="cancellationToken">The cancellation token to be observed during asynchronous execution.</param>
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
/// <typeparam name="TResult">The result value of the <see cref="Task{TResult}"/> that this encapsulated delegate returns.</typeparam>
/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous process that will produce the <typeparamref name="TResult"/> value.</returns>
/// <remarks>This can be likened to an asynchronous <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult}"/> delegate.</remarks>
public delegate Task<TResult> AsyncFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6,
    T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, CancellationToken cancellationToken);

/// <summary>
/// Encapsulates an asynchronous method that has sixteen input parameters, plus a <see cref="cancellationtoken" /> parameter, and asynchronously returns a value.
/// </summary>
/// <param name="arg1">The first input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg2">The second input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg3">The third input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg4">The fourth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg5">The fifth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg6">The sixth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg7">The seventh input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg8">The eighth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg9">The ninth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg10">The tenth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg11">The eleventh input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg12">The twelfth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg13">The thirteenth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg14">The fourteenth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg15">The fifteenth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="arg16">The sixteenth input parameter of the asynchronous method that this delegate encapsulates.</typeparam>
/// <param name="cancellationToken">The cancellation token to be observed during asynchronous execution.</param>
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
/// <typeparam name="TResult">The result value of the <see cref="Task{TResult}"/> that this encapsulated delegate returns.</typeparam>
/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous process that will produce the <typeparamref name="TResult"/> value.</returns>
/// <remarks>This can be likened to an asynchronous <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult}"/> delegate.</remarks>
public delegate Task<TResult> AsyncFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
    T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, CancellationToken cancellationToken);
