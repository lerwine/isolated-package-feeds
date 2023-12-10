namespace IsolatedPackageFeeds.Shared.LazyInit;

public class LazyChainedConversion<TInput, TResult> : LazyConversion<ILazyProducer<TInput>, TResult>
{
    public LazyChainedConversion(ILazyProducer<TInput> source, Func<TInput, TResult> convert) : base(source) => _convert = convert;
    
    public LazyChainedConversion(Func<TInput> getSource, Func<TInput, TResult> convert) : base(new LazyDelegatedProducer<TInput>(getSource)) => _convert = convert;

    public LazyChainedConversion(TInput input, Func<TInput, TInput> normalize, Func<TInput, TResult> convert) : base(new LazyDelegatedConversion<TInput, TInput>(input, normalize)) => _convert = convert;
    
    private readonly Func<TInput, TResult> _convert;

    protected override TResult Convert() => _convert(Input.GetResult());
}

public class LazyChainedConversion<TInput, TIntermediate, TResult> : LazyConversion<ILazyProducer<TIntermediate>, TResult>
{
    public LazyChainedConversion(ILazyProducer<TInput> source, Func<TInput, TIntermediate> convertSource, Func<TIntermediate, TResult> convertIntermediate) :
        base(new LazyChainedConversion<TInput, TIntermediate>(source, convertSource)) => _convert = convertIntermediate;
    
    public LazyChainedConversion(Func<TInput> getSource, Func<TInput, TIntermediate> convertSource, Func<TIntermediate, TResult> convertIntermediate) :
        base(new LazyChainedConversion<TInput, TIntermediate>(new LazyDelegatedProducer<TInput>(getSource), convertSource)) => _convert = convertIntermediate;
    
    public LazyChainedConversion(TInput source, Func<TInput, TIntermediate> convertSource, Func<TIntermediate, TResult> convertIntermediate) :
        base(new LazyDelegatedConversion<TInput, TIntermediate>(source, convertSource)) => _convert = convertIntermediate;
    
    private readonly Func<TIntermediate, TResult> _convert;

    protected override TResult Convert() => _convert(Input.GetResult());
}
