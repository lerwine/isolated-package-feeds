using System.Diagnostics.CodeAnalysis;

namespace IsolatedPackageFeeds.Shared.LazyInit;

/// <summary>
/// Lazy optional initializer combined with laxy optional conversion.
/// </summary>
/// <typeparam name="TInput">The input value type.</typeparam>
/// <typeparam name="TResult">The output value type</typeparam>
public class LazyOptionalChainedConversion<TInput, TResult> : LazyOptionalConversion<ILazyOptionalProducer<TInput>, TResult>
{
    public LazyOptionalChainedConversion(ILazyOptionalProducer<TInput> source, TryFunc<TInput, TResult?> tryConvert) : base(source) => _tryConvert = tryConvert;
    
    public LazyOptionalChainedConversion(TryFunc<TInput?> tryGetSource, TryFunc<TInput, TResult?> tryConvert) : base(new LazyOptionalDelegatedProducer<TInput>(tryGetSource)) => _tryConvert = tryConvert;
    
    public LazyOptionalChainedConversion(TInput? input, TryFunc<TInput?, TInput?> tryNormalize, TryFunc<TInput, TResult?> tryConvert) : base(new LazyOptionalDelegatedConversion<TInput?, TInput>(input, tryNormalize)) => _tryConvert = tryConvert;
    
    private readonly TryFunc<TInput, TResult?> _tryConvert;

    protected override bool TryConvert([NotNullWhen(true)] out TResult? result)
    {
        if (Input.TryGetResult(out TInput? input))
            return _tryConvert(input, out result);
        result = default;
        return false;
    }
}

/// <summary>
/// Lazy optional initializer combined with intermediary and final optional conversions.
/// </summary>
/// <typeparam name="TInput">The input value type.</typeparam>
/// <typeparam name="TIntermediate">The intermediate value type.</typeparam>
/// <typeparam name="TResult">The result value type.</typeparam>
public class LazyOptionalChainedConversion<TInput, TIntermediate, TResult> : LazyOptionalConversion<ILazyOptionalProducer<TIntermediate>, TResult>
{
    public LazyOptionalChainedConversion(ILazyOptionalProducer<TInput> source, TryFunc<TInput, TIntermediate?> tryConvertSource, TryFunc<TIntermediate, TResult?> tryConvertIntermediate) :
        base(new LazyOptionalChainedConversion<TInput, TIntermediate>(source, tryConvertSource)) => _tryConvert = tryConvertIntermediate;
    
    public LazyOptionalChainedConversion(TryFunc<TInput?> tryGetSource, TryFunc<TInput, TIntermediate?> tryConvertSource, TryFunc<TIntermediate, TResult?> tryConvertIntermediate) :
        base(new LazyOptionalChainedConversion<TInput, TIntermediate>(new LazyOptionalDelegatedProducer<TInput>(tryGetSource), tryConvertSource)) => _tryConvert = tryConvertIntermediate;
    
    public LazyOptionalChainedConversion(TInput? source, TryFunc<TInput?, TIntermediate?> tryConvertSource, TryFunc<TIntermediate, TResult?> tryConvertIntermediate) :
        base(new LazyOptionalDelegatedConversion<TInput?, TIntermediate>(source, tryConvertSource)) => _tryConvert = tryConvertIntermediate;
    
    private readonly TryFunc<TIntermediate, TResult?> _tryConvert;

    protected override bool TryConvert([NotNullWhen(true)] out TResult? result)
    {
        if (Input.TryGetResult(out TIntermediate? input))
            return _tryConvert(input, out result);
        result = default;
        return false;
    }
}
