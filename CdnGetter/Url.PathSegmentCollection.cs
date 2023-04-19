using System.Collections;
using System.Collections.ObjectModel;

namespace CdnGetter;

public partial class Url
{
    /// <summary>
    /// URI path segments.
    /// </summary>
    public class PathSegmentCollection : IList<string>, IEquatable<PathSegmentCollection>, IComparable<PathSegmentCollection>
    {
        private readonly Url _owner;
        private readonly Collection<string> _backingCollection = new();

        /// <summary>
        /// Gets or sets a value that indicates whether the URI path will be rooted if the parent <see cref="Url" /> is relative.
        /// </summary>
        public bool ForceRooted { get; set; }

        /// <summary>
        /// Indicates whether the URL path is rooted.
        /// </summary>
        /// <remarks>This will always be <see langword="true" /> if the parent <see cref="Url" /> is absolute.</remarks>
        public bool IsRooted => ForceRooted || _owner._scheme.Length > 0 || _owner.Authority is not null;

        /// <summary>
        /// Gets the number of path segments.
        /// </summary>
        public int Count => _backingCollection.Count;

        bool ICollection<string>.IsReadOnly => false;

        /// <summary>
        /// Gets or sets the name of the path segment (NOT uri-encoded) at the specified index.
        /// </summary>
        public string this[int index]
        {
            get => _backingCollection[index];
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException($"Path segment {nameof(value)} cannot be empty.", nameof(value));
                if (PathSeparatorRegex.IsMatch(value))
                    throw new ArgumentException($"Path segment {nameof(value)} cannot contain path separator characters.", nameof(value));
                lock (_backingCollection)
                    _backingCollection[index] = value;
            }
        }

        internal PathSegmentCollection(Url owner, IEnumerable<string>? segments, bool forceRooted)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            if (segments is not null)
                foreach (string p in segments)
                    if (!string.IsNullOrEmpty(p))
                        _backingCollection.Add(p);
            ForceRooted = forceRooted;
        }

        public int IndexOf(string item) => string.IsNullOrEmpty(item) ? -1 : _backingCollection.IndexOf(item);

        public void Insert(int index, string item)
        {
            if (string.IsNullOrEmpty(item))
                throw new ArgumentException($"'{nameof(item)}' cannot be null or empty.", nameof(item));
            if (PathSeparatorRegex.IsMatch(item))
                throw new ArgumentException($"Path segments cannot contain path separator characters.", nameof(item));
            lock (_backingCollection)
                _backingCollection.Insert(index, item);
        }

        public void RemoveAt(int index) => _backingCollection.RemoveAt(index);

        public void Add(string item)
        {
            if (string.IsNullOrEmpty(item))
                throw new ArgumentException($"'{nameof(item)}' cannot be null or empty.", nameof(item));
            if (PathSeparatorRegex.IsMatch(item))
                throw new ArgumentException($"Path segments cannot contain path separator characters.", nameof(item));
            lock (_backingCollection)
                _backingCollection.Add(item);
        }

        public void Clear()
        {
            lock (_backingCollection)
                _backingCollection.Clear();
        }

        public bool Contains(string item) => !string.IsNullOrEmpty(item) && _backingCollection.Contains(item);

        public void CopyTo(string[] array, int arrayIndex) => _backingCollection.CopyTo(array, arrayIndex);

        public bool Remove(string item)
        {
            if (string.IsNullOrEmpty(item))
                return false;
            lock (_backingCollection)
                return _backingCollection.Remove(item);
        }

        public IEnumerator<string> GetEnumerator() => _backingCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_backingCollection).GetEnumerator();

        public int CompareTo(PathSegmentCollection? other)
        {
            if (other is null)
                return 1;
            if (ReferenceEquals(this, other))
                return 0;
            lock (_backingCollection)
            {
                lock (other._backingCollection)
                {
                    using IEnumerator<string> x = _backingCollection.GetEnumerator();
                    using IEnumerator<string> y = other._backingCollection.GetEnumerator();
                    if (IsRooted)
                    {
                        if (!other.IsRooted)
                            return y.MoveNext() ? Comparer.Compare(PRIMARY_PATH_SEPARATOR, y.Current) : 1;
                    }
                    else if (other.IsRooted)
                        return x.MoveNext() ? Comparer.Compare(x.Current, PRIMARY_PATH_SEPARATOR) : -1;
                    while (x.MoveNext())
                    {
                        if (y.MoveNext())
                        {
                            int result = Comparer.Compare(x.Current, y.Current);
                            if (result != 0)
                                return result;
                        }
                        else
                            return 1;
                    }
                    return y.MoveNext() ? -1 : 0;
                }
            }
        }

        public bool Equals(PathSegmentCollection? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            lock (_backingCollection)
            {
                lock (other._backingCollection)
                {
                    if (IsRooted != other.IsRooted || _backingCollection.Count != other._backingCollection.Count)
                        return false;
                    using IEnumerator<string> x = _backingCollection.GetEnumerator();
                    using IEnumerator<string> y = other._backingCollection.GetEnumerator();
                    while (x.MoveNext() && y.MoveNext())
                        if (!Comparer.Equals(x.Current, y.Current))
                            return false;
                }
            }
            return true;
        }

        public override bool Equals(object? obj) => obj is PathSegmentCollection other && Equals(other);

        public override int GetHashCode()
        {
            int result = 3;
            unchecked
            {
                result = (result * 7) + IsRooted.GetHashCode();
                foreach (string s in _backingCollection)
                    result = (result * 7) + Comparer.GetHashCode(s);
            }
            return result;
        }

        public override string ToString()
        {
            lock (_backingCollection)
            {
                if (IsRooted)
                {
                    if (_backingCollection.Count > 0)
                        return string.Join(PRIMARY_PATH_SEPARATOR, new string[] { string.Empty }.Concat(_backingCollection.Select(s => PathSegmentEncodeRegex.Replace(s, m => Uri.HexEscape(m.Value[0])))));
                    return PRIMARY_PATH_SEPARATOR;
                }
                return _backingCollection.Count switch
                {
                    0 => string.Empty,
                    1 => PathSegmentEncodeRegex.Replace(_backingCollection[0], m => Uri.HexEscape(m.Value[0])),
                    _ => string.Join(PRIMARY_PATH_SEPARATOR, _backingCollection.Select(s => PathSegmentEncodeRegex.Replace(s, m => Uri.HexEscape(m.Value[0]))))
                };
            }
        }
    }
}
