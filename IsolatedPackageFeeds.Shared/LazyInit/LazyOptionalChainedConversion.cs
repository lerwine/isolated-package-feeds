using System.Diagnostics.CodeAnalysis;

namespace IsolatedPackageFeeds.Shared.LazyInit;

public class LazyOptionalChainedConversion<TInput, TResult> : LazyOptionalConversion<ILazyOptionalInitializer<TInput>, TResult>
{
    public LazyOptionalChainedConversion(ILazyOptionalInitializer<TInput> source, TryFunc<TInput, TResult?> tryConvert) : base(source) => _tryConvert = tryConvert;
    
    public LazyOptionalChainedConversion(TryFunc<TInput?> tryGetSource, TryFunc<TInput, TResult?> tryConvert) : base(new LazyOptionalDelegatedInitializer<TInput>(tryGetSource)) => _tryConvert = tryConvert;
    
    private readonly TryFunc<TInput, TResult?> _tryConvert;
    protected override bool TryConvert([NotNullWhen(true)] out TResult? result)
    {
        if (Input.TryGetResult(out TInput? input))
            return _tryConvert(input, out result);
        result = default;
        return false;
    }
}
