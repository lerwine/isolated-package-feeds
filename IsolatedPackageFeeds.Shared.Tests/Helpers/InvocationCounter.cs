namespace IsolatedPackageFeeds.Shared.Tests.Helpers
{
    public class InvocationCounter<T>
    {
        public int Count { get; private set; }

        public T Current => _values.Current;

        private readonly IEnumerator<T> _values;

        public InvocationCounter(IEnumerable<T> values)
        {
            if (!(_values = values.GetEnumerator()).MoveNext())
                Assert.Inconclusive("Invocation counter had no values");
        }

        public InvocationCounter(params T[] values) : this((IEnumerable<T>)values) { }

        public T Invoke()
        {
            Count++;
            T result = _values.Current;
            if (!_values.MoveNext())
                _values.Reset();
            return result;
        }
    }
}