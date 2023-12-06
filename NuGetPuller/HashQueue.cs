using System.Diagnostics.CodeAnalysis;

namespace NuGetPuller;

/// <summary>
/// First-in, first-out queue of unique items.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public class HashQueue<T> : IEnumerable<T>, System.Collections.IEnumerable, IReadOnlyCollection<T>, System.Collections.ICollection
{
    /// <summary>
    /// Initializes a new <c>HashQueue</c>.
    /// </summary>
    /// <param name="capacity">The initial capacity of the buffer.</param>
    /// <param name="source">The items to include in the <c>HashQueue</c>.</param>
    /// <param name="comparer">The optional element comparer.</param>
    public HashQueue(int capacity, IEnumerable<T> source, IEqualityComparer<T>? comparer = null)
    {
        _comparer = comparer ?? EqualityComparer<T>.Default;
        _head = 0;
        if (source is null)
        {
            _tail = Count = 0;
            _backingArray = (capacity < 1) ? [] : new T[capacity];
        }
        else if ((Count = (_backingArray = source.Distinct(_comparer).ToArray()).Length) < capacity)
        {
            _tail = Count;
            SetCapacity(capacity);
        }
        else
            _tail = 0;
    }

    /// <summary>
    /// Initializes a new <c>HashQueue</c>.
    /// </summary>
    /// <param name="source">The items to include in the <c>HashQueue</c>.</param>
    /// <param name="comparer">The optional element comparer.</param>
    public HashQueue(IEnumerable<T> source, IEqualityComparer<T>? comparer = null)
    {
        _comparer = comparer ?? EqualityComparer<T>.Default;
        _head = 0;
        if (source is null)
        {
            _tail = Count = 0;
            _backingArray = new T[_DefaultCapacity];
        }
        else if ((Count = (_backingArray = source.Distinct(_comparer).ToArray()).Length) < _DefaultCapacity)
        {
            _tail = Count;
            SetCapacity(_DefaultCapacity);
        }
        else
            _tail = 0;
    }

    /// <summary>
    /// Initializes a new <c>HashQueue</c>.
    /// </summary>
    /// <param name="capacity">The initial capacity of the buffer.</param>
    /// <param name="comparer">The optional element comparer.</param>
    public HashQueue(int capacity, IEqualityComparer<T>? comparer = null)
    {
        _comparer = comparer ?? EqualityComparer<T>.Default;
        _backingArray = (capacity < 1) ? [] : new T[capacity];
        _head = _tail = Count = 0;
    }

    /// <summary>
    /// Initializes a new <c>HashQueue</c>.
    /// </summary>
    /// <param name="comparer">The optional element comparer.</param>
    public HashQueue(IEqualityComparer<T>? comparer = null)
    {
        _comparer = comparer ?? EqualityComparer<T>.Default;
        _backingArray = new T[_DefaultCapacity];
        _head = _tail = Count = 0;
    }

    private readonly IEqualityComparer<T> _comparer;
    private T[] _backingArray;
    private int _head;
    private int _tail;
    private const int _MinimumGrow = 4;
    private const int _ShrinkThreshold = 32;
    private const int _GrowFactor = 200;
    private const int _DefaultCapacity = 4;
    private object _modificationKey = new();

    /// <summary>
    /// Gets the number of elements in the current <c>HashQueue</c>.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Gets an object that can be used to synchronize access to the  <c>HashQueue</c>.
    /// </summary>
    public object SyncRoot { get; } = new();

    bool System.Collections.ICollection.IsSynchronized => true;

    /// <summary>
    /// Attempts to add an object to the end of the <c>HashQueue</c>.
    /// </summary>
    /// <param name="item">The item to be added.</param>
    /// <returns><see langword="true"/> if there wasn't already a matching element and the <paramref name="item"/> was added; otherwise, <see langword="false"/>.</returns>
    public bool TryEnqueue(T item)
    {
        lock (SyncRoot)
        {
            int index = _head;
            int count = Count;

            if (item is null)
                while (count-- > 0)
                {
                    if (_backingArray[index] == null)
                        return false;
                    index = (index + 1) % _backingArray.Length;
                }
            else
                while (count-- > 0)
                {
                    if (_comparer.Equals(_backingArray[index], item))
                        return false;
                    index = (index + 1) % _backingArray.Length;
                }

            if (Count == _backingArray.Length)
            {
                int newcapacity = (int)(_backingArray.Length * (long)_GrowFactor / 100L);
                if (newcapacity < _backingArray.Length + _MinimumGrow)
                    newcapacity = _backingArray.Length + _MinimumGrow;
                SetCapacity(newcapacity);
            }

            _backingArray[_tail] = item;
            _tail = (_tail + 1) % _backingArray.Length;
            Count++;
            _modificationKey = new();
        }
        return true;
    }

    /// <summary>
    /// Ensures that the size of the backing array for this queue is at least the specified capacity.
    /// </summary>
    /// <param name="capacity">The minimum size of the backing array.</param>
    /// <returns>The current size of the backing array.</returns>
    public int EnsureCapacity(int capacity)
    {
        lock (SyncRoot)
        {
            if (capacity > _backingArray.Length)
                SetCapacity(capacity);
        }
        return _backingArray.Length;
    }

    private void SetCapacity(int capacity)
    {
        T[] newarray = new T[capacity];
        if (Count > 0)
        {
            if (_head < _tail)
                Array.Copy(_backingArray, _head, newarray, 0, Count);
            else
            {
                Array.Copy(_backingArray, _head, newarray, 0, _backingArray.Length - _head);
                Array.Copy(_backingArray, 0, newarray, _backingArray.Length - _head, _tail);
            }
        }

        _backingArray = newarray;
        _head = 0;
        _tail = (Count == capacity) ? 0 : Count;
        _modificationKey = new();
    }

    /// <summary>
    /// Sets the capacity to the actual number of elements.
    /// </summary>
    public void TrimExcess()
    {
        lock (SyncRoot)
        {
            int threshold = (int)(_backingArray.Length * 0.9);
            if (Count < threshold)
                SetCapacity(Count);
        }
    }

    /// <summary>
    /// If the current <c>HashQueue</c> is not empty, removes and returns the value at the beginning of the current <c>HashQueue</c>.
    /// </summary>
    /// <param name="result">The value removed from the current <c>HashQueue</c>.</param>
    /// <returns><see langword="true"/> if an element was removed from the beginning of the current <c>HashQueue</c>; otherwise, <see langword="false"/>.</returns>
    public bool TryDequeue([MaybeNullWhen(false)] out T result)
    {
        lock (SyncRoot)
        {
            if (Count == 0)
            {
                result = default;
                return false;
            }

            result = _backingArray[_head];
            _backingArray[_head] = default!;
            _head = (_head + 1) % _backingArray.Length;
            Count--;
            _modificationKey = new();
        }
        return true;
    }

    /// <summary>
    /// If the current <c>HashQueue</c> is not empty, returns the value at the beginning of the current <c>HashQueue</c> without removing it.
    /// </summary>
    /// <param name="result">The value at the beginning of the current <c>HashQueue</c>.</param>
    /// <returns><see langword="true"/> if the current <c>HashQueue</c> was not empty and the first element was returned; otherwise, <see langword="false"/>.</returns>
    public bool TryPeek([MaybeNullWhen(false)] out T result)
    {
        lock (SyncRoot)
        {
            if (Count == 0)
            {
                result = default;
                return false;
            }
            result = _backingArray[_head];
        }
        return true;
    }

    /// <summary>
    /// Removes all Objects from the queue.
    /// </summary>
    public void Clear()
    {
        lock (SyncRoot)
        {
            if (Count == 0)
                return;
            if (_head < _tail)
                Array.Clear(_backingArray, _head, Count);
            else
            {
                Array.Clear(_backingArray, _head, _backingArray.Length - _head);
                Array.Clear(_backingArray, 0, _tail);
            }

            _head = _tail = Count = 0;
            _modificationKey = new();
        }
    }

    /// <summary>
    /// Tests whether a matching value exists in the current <c>HashQueue</c>.
    /// </summary>
    /// <param name="item">The value to search for</param>
    /// <returns><see langword="true"/> if a matching value exists in the current <c>HashQueue</c>; otherwise, <see langword="false"/>.</returns>
    public bool Contains(T item)
    {
        lock (SyncRoot)
        {
            int index = _head;
            int count = Count;

            if (item is null)
                while (count-- > 0)
                {
                    if (_backingArray[index] == null)
                        return true;
                    index = (index + 1) % _backingArray.Length;
                }
            else
                while (count-- > 0)
                {
                    if (_comparer.Equals(_backingArray[index], item))
                        return true;
                    index = (index + 1) % _backingArray.Length;
                }
        }

        return false;
    }

    public void CopyTo(Array array, int index)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (array.Rank != 1)
            throw new ArgumentException("Multidimensional array not supported", nameof(array));

        if (array.GetLowerBound(0) != 0)
            throw new ArgumentException("Non-zero lower bound not supported", nameof(array));

        int arrayLen = array.Length;
        if (index < 0 || index > arrayLen || arrayLen - index < Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        lock (SyncRoot)
        {
            int numToCopy = (arrayLen - index < Count) ? arrayLen - index : Count;
            if (numToCopy == 0) return;
            try
            {
                int firstPart = (_backingArray.Length - _head < numToCopy) ? _backingArray.Length - _head : numToCopy;
                Array.Copy(_backingArray, _head, array, index, firstPart);
                numToCopy -= firstPart;

                if (numToCopy > 0)
                {
                    Array.Copy(_backingArray, 0, array, index + _backingArray.Length - _head, numToCopy);
                }
            }
            catch (ArrayTypeMismatchException error)
            {
                throw new ArgumentException("Invalid array type", nameof(array), error);
            }
        }
    }

    public IEnumerator<T> GetEnumerator() => new Enumerator(this);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => new Enumerator(this);

    public T[] ToArray()
    {
        lock (SyncRoot)
        {
            if (Count == 0)
                return [];
            T[] arr = new T[Count];

            if (_head < _tail)
                Array.Copy(_backingArray, _head, arr, 0, Count);
            else
            {
                Array.Copy(_backingArray, _head, arr, 0, _backingArray.Length - _head);
                Array.Copy(_backingArray, 0, arr, _backingArray.Length - _head, _tail);
            }
            return arr;
        }
    }

    public sealed class Enumerator : IEnumerator<T>, System.Collections.IEnumerator
    {
        private HashQueue<T> _target;
        private int _index;
        private object _modificationKey;
        private T _currentElement = default!;

        internal Enumerator(HashQueue<T> q)
        {
            _target = q;
            _modificationKey = _target._modificationKey;
            _index = -1;
        }

        public void Dispose()
        {
            lock (_target.SyncRoot)
            {
                _index = -2;
            }
        }

        public bool MoveNext()
        {
            lock (_target.SyncRoot)
            {
                if (!ReferenceEquals(_modificationKey, _target._modificationKey))
                    throw new InvalidOperationException();

                if (_index == -2)
                    return false;

                _index++;

                if (_index == _target.Count)
                {
                    _index = -2;
                    _currentElement = default!;
                    return false;
                }

                _currentElement = _target._backingArray[(_target._head + _index) % _target._backingArray.Length];
            }
            return true;
        }

        public T Current
        {
            get
            {
                if (_index < 0)
                    throw new InvalidOperationException((_index == -1) ? "Enumeration not started." : "End of enumeration");
                return _currentElement;
            }
        }

        object System.Collections.IEnumerator.Current => Current!;

        void System.Collections.IEnumerator.Reset()
        {
            lock (_target.SyncRoot)
            {
                _modificationKey = _target._modificationKey;
                _index = -1;
                _currentElement = default!;
            }
        }
    }
}
