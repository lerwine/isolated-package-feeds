using IsolatedPackageFeeds.Shared;

namespace CdnGetter.Services;

sealed class AsyncLookupEnumerator<TInput, TResult> : IDisposable
    where TResult : class
{
    private readonly IEnumerator<TInput> _backingEnumerator;
    private readonly AsyncFunc<TInput, TResult?> _getResult;

    internal int IterationCount { get; private set; }

    internal TResult Current { get; private set; } = null!;

    internal AsyncLookupEnumerator(IEnumerable<TInput> source, AsyncFunc<TInput, TResult?> getResult)
    {
        _backingEnumerator = source.GetEnumerator();
        _getResult = getResult;
    }

    public async Task<bool> MoveNextAsync(CancellationToken cancellationToken)
    {
        while (_backingEnumerator.MoveNext())
        {
            IterationCount++;
            TResult? result = await _getResult(_backingEnumerator.Current, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                break;
            if (result is not null)
            {
                Current = result;
                return true;
            }
        }
        return false;
    }

    public void Dispose() => _backingEnumerator.Dispose();
}