using System.Collections;

namespace IsolatedPackageFeeds.Shared;

public sealed class PeekableEnumerator<T>(IEnumerable<T>? source) : IEnumerator<T>
{
    private readonly IEnumerator<T> _backingEnumerator = (source ?? Enumerable.Empty<T>()).GetEnumerator();

    private bool? _hasPeekedValue;

    private T _peekedValue = default!;

    public T Current
    {
        get
        {
            lock (_backingEnumerator)
                return (_hasPeekedValue.HasValue && _hasPeekedValue.Value) ? _peekedValue : _backingEnumerator.Current;
        }
    }

    object IEnumerator.Current => ((IEnumerator)_backingEnumerator).Current;

    public bool MoveNext()
    {
        lock (_backingEnumerator)
        {
            if (_hasPeekedValue.HasValue)
            {
                if (_hasPeekedValue.Value)
                {
                    _hasPeekedValue = null;
                    return true;
                }
                _hasPeekedValue = null;
                return false;
            }
            return _backingEnumerator.MoveNext();
        }
    }

    public void Reset()
    {
        _backingEnumerator.Reset();
        _hasPeekedValue = null;
    }

    public bool TryPeek(out T result)
    {
        lock (_backingEnumerator)
        {
            if (_hasPeekedValue.HasValue)
            {
                if (_hasPeekedValue.Value)
                {
                    result = _peekedValue;
                    return true;
                }
            }
            else if (_backingEnumerator.MoveNext())
            {
                _hasPeekedValue = true;
                result = _peekedValue = _backingEnumerator.Current;
                return true;
            }
            else
                _hasPeekedValue = false;
        }
        result = default!;
        return false;
    }

    public void Dispose() => _backingEnumerator.Dispose();
}
