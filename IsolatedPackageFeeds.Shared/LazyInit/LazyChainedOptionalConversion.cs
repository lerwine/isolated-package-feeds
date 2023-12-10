using System.Diagnostics.CodeAnalysis;

namespace IsolatedPackageFeeds.Shared.LazyInit;

public class LazyChainedOptionalConversion<TInput, TResult> : LazyOptionalConversion<ILazyProducer<TInput>, TResult>
{
    public LazyChainedOptionalConversion(ILazyProducer<TInput> source, TryFunc<TInput, TResult?> tryConvert) : base(source) => _tryConvert = tryConvert;
    
    public LazyChainedOptionalConversion(Func<TInput> getSource, TryFunc<TInput, TResult?> tryConvert) : base(new LazyDelegatedProducer<TInput>(getSource)) => _tryConvert = tryConvert;
    
    public LazyChainedOptionalConversion(TInput input, Func<TInput, TInput> normalize, TryFunc<TInput, TResult?> tryConvert) : base(new LazyDelegatedConversion<TInput, TInput>(input, normalize)) => _tryConvert = tryConvert;
    
    private readonly TryFunc<TInput, TResult?> _tryConvert;

    protected override bool TryConvert([NotNullWhen(true)] out TResult? result) => _tryConvert(Input.GetResult(), out result);
}

public class LazyChainedOptionalConversion<TInput, TIntermediate, TResult> : LazyOptionalConversion<ILazyOptionalProducer<TIntermediate>, TResult>
{
    public LazyChainedOptionalConversion(ILazyProducer<TInput> source, TryFunc<TInput, TIntermediate?> tryConvertSource, TryFunc<TIntermediate, TResult?> tryConvertIntermediate) :
        base(new LazyChainedOptionalConversion<TInput, TIntermediate>(source, tryConvertSource)) => _tryConvert = tryConvertIntermediate;
    
    public LazyChainedOptionalConversion(Func<TInput> getSource, TryFunc<TInput, TIntermediate?> tryConvertSource, TryFunc<TIntermediate, TResult?> tryConvertIntermediate) :
        base(new LazyChainedOptionalConversion<TInput, TIntermediate>(new LazyDelegatedProducer<TInput>(getSource), tryConvertSource)) => _tryConvert = tryConvertIntermediate;
    
    public LazyChainedOptionalConversion(TInput source, TryFunc<TInput, TIntermediate?> tryConvertSource, TryFunc<TIntermediate, TResult?> tryConvertIntermediate) :
        base(new LazyOptionalDelegatedConversion<TInput, TIntermediate>(source, tryConvertSource)) => _tryConvert = tryConvertIntermediate;
    
    private readonly TryFunc<TIntermediate, TResult?> _tryConvert;

    protected override bool TryConvert([NotNullWhen(true)] out TResult? result)
    {
        if (Input.TryGetResult(out TIntermediate? input))
            return _tryConvert(input, out result);
        result = default;
        return false;
    }
}
