using System.Collections;

namespace CdnGetter.UnitTests;

public sealed class RotatingEnumerable<T> : IEnumerable<T>
{
    private readonly T[] _values;

    public RotatingEnumerable(params T[] values) => _values = values ?? Array.Empty<T>();

    public IEnumerator<T> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public IEnumerable<T> GetValues() => _values.AsEnumerable();

    class Enumerator : IEnumerator<T>
    {
        private readonly RotatingEnumerable<T> _target;
        private int _index = -1;
        private bool _isDisposed;

        public T Current => (_index < 0) ? default! : _target._values[_index];

        object IEnumerator.Current => Current!;

        internal Enumerator(RotatingEnumerable<T> target) => _target = target;

        public bool MoveNext()
        {
            if (_isDisposed || _target._values.Length == 0)
                return false;
            if (++_index >= _target._values.Length)
                _index = 0;
            return true;
        }

        public void Reset() { _index = -1; }

        public void Dispose() => _isDisposed = true;
    }
}