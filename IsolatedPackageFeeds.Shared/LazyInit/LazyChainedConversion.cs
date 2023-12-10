namespace IsolatedPackageFeeds.Shared.LazyInit;

public class LazyChainedConversion<TInput, TResult> : LazyConversion<ILazyInitializer<TInput>, TResult>
{
    public LazyChainedConversion(ILazyInitializer<TInput> source, Func<TInput, TResult> convert) : base(source) => _convert = convert;
    
    public LazyChainedConversion(Func<TInput> getSource, Func<TInput, TResult> convert) : base(new LazyDelegatedInitializer<TInput>(getSource)) => _convert = convert;

    public LazyChainedConversion(TInput input, Func<TInput, TInput> normalize, Func<TInput, TResult> convert) : base(new LazyDelegatedConversion<TInput, TInput>(input, normalize)) => _convert = convert;
    
    private readonly Func<TInput, TResult> _convert;

    protected override TResult Convert() => _convert(Input.GetResult());
}

public class LazyChainedConversion<TInput, TIntermediate, TResult> : LazyConversion<ILazyInitializer<TIntermediate>, TResult>
{
    public LazyChainedConversion(ILazyInitializer<TInput> source, Func<TInput, TIntermediate> convertSource, Func<TIntermediate, TResult> convertIntermediate) :
        base(new LazyChainedConversion<TInput, TIntermediate>(source, convertSource)) => _convert = convertIntermediate;
    
    public LazyChainedConversion(Func<TInput> getSource, Func<TInput, TIntermediate> convertSource, Func<TIntermediate, TResult> convertIntermediate) :
        base(new LazyChainedConversion<TInput, TIntermediate>(new LazyDelegatedInitializer<TInput>(getSource), convertSource)) => _convert = convertIntermediate;
    
    public LazyChainedConversion(TInput source, Func<TInput, TIntermediate> convertSource, Func<TIntermediate, TResult> convertIntermediate) :
        base(new LazyDelegatedConversion<TInput, TIntermediate>(source, convertSource)) => _convert = convertIntermediate;
    
    private readonly Func<TIntermediate, TResult> _convert;

    protected override TResult Convert() => _convert(Input.GetResult());
}
