using System.Diagnostics.CodeAnalysis;

namespace CdnSync
{
    public struct TernaryOption<T1, T2>
    {
        private readonly bool _isAlternate;
        private readonly T1 _primary;
        private readonly T2 _alternate;

        public readonly bool IsAlternate { get { return _isAlternate; } }
        public readonly T1 Primary { get { return _primary; } }
        public readonly T2 Alternate { get { return _alternate; } }

        public void Apply(Action<T1> ifPrimary, Action<T2> ifAlternate)
        {
            if (_isAlternate)
                ifAlternate(_alternate);
            else
                ifPrimary(_primary);
        }

        public TernaryOption<TPrimaryResult, TAltResult> Map<TPrimaryResult, TAltResult>(Func<T1, TernaryOption<TPrimaryResult, TAltResult>> ifPrimary, Func<T2, TernaryOption<TPrimaryResult, TAltResult>> ifAlternate) =>
            _isAlternate ? ifAlternate(_alternate) : ifPrimary(_primary);

        public TResult FlatMap<TResult>(Func<T1, TResult> ifPrimary, Func<T2, TResult> ifAlternate) => _isAlternate ? ifAlternate(_alternate) : ifPrimary(_primary);

        public TResult FlatMap<TResult>(Func<T1, TResult> ifPrimary, TResult ifAlternate) => _isAlternate ? ifAlternate : ifPrimary(_primary);

        public TResult FlatMap<TResult>(TResult ifPrimary, Func<T2, TResult> ifAlternate) => _isAlternate ? ifAlternate(_alternate) : ifPrimary;

        public TResult FlatMap<TResult>(TResult ifPrimary, TResult ifAlternate) => _isAlternate ? ifAlternate : ifPrimary;

        public bool TryPrimary<TResult>(Func<T1, TResult> ifPrimary, [NotNullWhen(true)] out TResult? result)
        {
            if (_isAlternate)
            {
                result = default;
                return false;
            }
            result = ifPrimary(_primary)!;
            return true;
        }
        
        public bool TryPrimary<TResult>(Func<T1, TResult> ifPrimary, Func<T2, TResult> ifAlternate, out TResult result)
        {
            if (_isAlternate)
            {
                result = ifAlternate(_alternate);
                return false;
            }
            result = ifPrimary(_primary);
            return true;
        }
        
        public bool TryAlternate<TResult>(Func<T2, TResult> ifAlternate, [NotNullWhen(true)] out TResult? result)
        {
            if (_isAlternate)
            {
                result = ifAlternate(_alternate)!;
                return true;
            }
            result = default;
            return false;
        }
        
        public TernaryOption(T1 value)
        {
            _isAlternate = false;
            _primary = value;
            _alternate = default!;
        }

        public TernaryOption(bool isAlternate, T1 primary, T2 alternate)
        {
            _isAlternate = isAlternate;
            _primary = primary;
            _alternate = alternate;
        }

        public static TernaryOption<T1, T2> AsAlternate(T2 alternate) => new(true, default!, alternate);
    }
}