namespace IsolatedPackageFeeds.Shared.LazyInit;

public class LazyChainedConversion<TInput, TResult> : LazyConversion<ILazyInitializer<TInput>, TResult>
{
    public LazyChainedConversion(ILazyInitializer<TInput> source, Func<TInput, TResult> convert) : base(source) => _convert = convert;
    
    public LazyChainedConversion(Func<TInput> getSource, Func<TInput, TResult> convert) : base(new LazyDelegatedInitializer<TInput>(getSource)) => _convert = convert;

    private readonly Func<TInput, TResult> _convert;

    protected override TResult Convert() => _convert(Input.GetResult());
}
