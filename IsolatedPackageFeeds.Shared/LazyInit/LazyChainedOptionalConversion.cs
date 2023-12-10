using System.Diagnostics.CodeAnalysis;

namespace IsolatedPackageFeeds.Shared.LazyInit;

public class LazyChainedOptionalConversion<TInput, TResult> : LazyOptionalConversion<ILazyInitializer<TInput>, TResult>
{
    public LazyChainedOptionalConversion(ILazyInitializer<TInput> source, TryFunc<TInput, TResult?> tryConvert) : base(source) => _tryConvert = tryConvert;
    
    public LazyChainedOptionalConversion(Func<TInput> getSource, TryFunc<TInput, TResult?> tryConvert) : base(new LazyDelegatedInitializer<TInput>(getSource)) => _tryConvert = tryConvert;
    
    public LazyChainedOptionalConversion(TInput input, Func<TInput, TInput> normalize, TryFunc<TInput, TResult?> tryConvert) : base(new LazyDelegatedConversion<TInput, TInput>(input, normalize)) => _tryConvert = tryConvert;
    
    private readonly TryFunc<TInput, TResult?> _tryConvert;

    protected override bool TryConvert([NotNullWhen(true)] out TResult? result) => _tryConvert(Input.GetResult(), out result);
}

public class LazyChainedOptionalConversion<TInput, TIntermediate, TResult> : LazyOptionalConversion<ILazyOptionalInitializer<TIntermediate>, TResult>
{
    public LazyChainedOptionalConversion(ILazyInitializer<TInput> source, TryFunc<TInput, TIntermediate?> tryConvertSource, TryFunc<TIntermediate, TResult?> tryConvertIntermediate) :
        base(new LazyChainedOptionalConversion<TInput, TIntermediate>(source, tryConvertSource)) => _tryConvert = tryConvertIntermediate;
    
    public LazyChainedOptionalConversion(Func<TInput> getSource, TryFunc<TInput, TIntermediate?> tryConvertSource, TryFunc<TIntermediate, TResult?> tryConvertIntermediate) :
        base(new LazyChainedOptionalConversion<TInput, TIntermediate>(new LazyDelegatedInitializer<TInput>(getSource), tryConvertSource)) => _tryConvert = tryConvertIntermediate;
    
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
