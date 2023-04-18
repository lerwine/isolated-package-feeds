using System.Collections;
using System.Collections.ObjectModel;

namespace CdnGetter;

public partial class Url
{
    public class PathSegmentCollection : IList<string>, IEquatable<PathSegmentCollection>, IComparable<PathSegmentCollection>
    {
        private readonly Url _owner;
        private readonly Collection<string> _backingCollection = new();

        public bool ForceRooted { get; set; }

        public bool IsRooted => ForceRooted || _owner._scheme.Length > 0 || _owner.Authority is not null;

        public int Count => _backingCollection.Count;

        bool ICollection<string>.IsReadOnly => false;

        public string this[int index] { get => _backingCollection[index]; set => _backingCollection[index] = value; }

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
            _backingCollection.Insert(index, item);
        }

        public void RemoveAt(int index) => _backingCollection.RemoveAt(index);

        public void Add(string item)
        {
            if (string.IsNullOrEmpty(item))
                throw new ArgumentException($"'{nameof(item)}' cannot be null or empty.", nameof(item));
            _backingCollection.Add(item);
        }

        public void Clear() => _backingCollection.Clear();

        public bool Contains(string item) => !string.IsNullOrEmpty(item) && _backingCollection.Contains(item);

        public void CopyTo(string[] array, int arrayIndex) => _backingCollection.CopyTo(array, arrayIndex);

        public bool Remove(string item) => !string.IsNullOrEmpty(item) && _backingCollection.Remove(item);

        public IEnumerator<string> GetEnumerator() => _backingCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_backingCollection).GetEnumerator();

        public int CompareTo(PathSegmentCollection? other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(PathSegmentCollection? other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object? obj) => obj is PathSegmentCollection other && Equals(other);

        public override int GetHashCode()
        {
            int result = 3;
            unchecked
            {
                foreach (string s in _backingCollection)
                    result = (result * 7) + Comparer.GetHashCode(s);
            }
            return result;
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
