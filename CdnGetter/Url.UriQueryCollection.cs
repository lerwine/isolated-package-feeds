using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace CdnGetter;

public partial class Url
{
    public class UriQueryCollection : IList<UriQueryElement>, IDictionary<string, string?>, IEquatable<UriQueryCollection>, IComparable<UriQueryCollection>
    {
        private readonly Dictionary<string, string?> _backingDictionary = new(Comparer);
        private readonly Collection<UriQueryElement> _backingCollection = new();

        public string? this[string key]
        {
            get => _backingDictionary[key];
            set
            {
                if (key is null)
                    throw new ArgumentNullException(nameof(value));
                lock (_backingCollection)
                {
                    if (_backingDictionary.TryGetValue(key, out string? existingValue))
                    {
                        int index = 0;
                        UriQueryElement e = _backingCollection[index];
                        if (value is null)
                        {
                            if (existingValue is null)
                                return;
                            while (!Comparer.Equals(e.Key) || e.Value is not null)
                            {
                                index++;
                                e = _backingCollection[index];
                            }
                        }
                        else
                        {
                            if (existingValue is not null && existingValue == value)
                                return;
                            while (!Comparer.Equals(e.Key) || e.Value is null || !Comparer.Equals(e.Value, existingValue))
                            {
                                index++;
                                e = _backingCollection[index];
                            }
                        }
                        _backingCollection[index] = new(key, value);
                        _backingDictionary[key] = value;
                        int i = index + 1;
                        while (i < _backingCollection.Count)
                        {
                            if (Comparer.Equals(_backingCollection[i].Key, key))
                                _backingCollection.RemoveAt(i);
                            else
                                i++;
                        }
                        for (i = index - 1; i >= 0; i--)
                        {
                            if (Comparer.Equals(_backingCollection[i].Key, key))
                                _backingCollection.RemoveAt(i);
                        }
                    }
                    else
                    {
                        _backingCollection.Add(new(key, value));
                        _backingDictionary.Add(key, value);
                    }
                }
            }
        }

        public UriQueryElement this[int index] { get => _backingCollection[index]; set => _backingCollection[index] = value; }

        public ICollection<string> Keys => _backingDictionary.Keys;

        public ICollection<string?> Values => _backingDictionary.Values;

        public int Count => _backingCollection.Count;

        bool ICollection<KeyValuePair<string, string?>>.IsReadOnly => false;

        bool ICollection<UriQueryElement>.IsReadOnly => false;

        public UriQueryCollection(IEnumerable<UriQueryElement>? elements = null)
        {
            if (elements is not null)
                foreach (UriQueryElement e in elements)
                {
                    if (!_backingDictionary.ContainsKey(e.Key))
                        _backingDictionary.Add(e.Key, e.Value);
                    _backingCollection.Add(e);
                }
        }

        public void Add(string key, string? value)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));
            lock (_backingCollection)
            {
                if (_backingDictionary.ContainsKey(key))
                {
                    if ((value is null) ? _backingCollection.Any(e => Comparer.Equals(e.Key, key) && e.Value is null) : _backingCollection.Any(e => Comparer.Equals(e.Key, key) && e.Value is not null && Comparer.Equals(e.Value, value)))
                        return;
                }
                else
                    _backingDictionary.Add(key, value);
                _backingCollection.Add(new(key, value));
            }
        }

        public void Add(UriQueryElement item)
        {
            lock (_backingCollection)
            {
                if (_backingCollection.Contains(item))
                    return;
                if (!_backingDictionary.ContainsKey(item.Key))
                    _backingDictionary.Add(item.Key, item.Value);
                _backingCollection.Add(item);
            }
        }

        void ICollection<KeyValuePair<string, string?>>.Add(KeyValuePair<string, string?> item)
        {
            lock (_backingCollection)
            {
                string key = item.Key;
                    string? value  = item.Value;
                if (_backingDictionary.ContainsKey(key))
                {
                    if ((value is null) ? _backingCollection.Any(e => Comparer.Equals(e.Key, key) && e.Value is null) : _backingCollection.Any(e => Comparer.Equals(e.Key, key) && e.Value is not null && Comparer.Equals(e.Value, value)))
                        return;
                }
                else
                    _backingDictionary.Add(key, value);
                _backingCollection.Add(new(key, value));
            }
        }

        public void Clear()
        {
            lock (_backingCollection)
            {
                _backingDictionary.Clear();
                _backingCollection.Clear();
            }
        }

        public int CompareTo(UriQueryCollection? other)
        {
            throw new NotImplementedException();
        }

        public bool Contains(UriQueryElement item) => _backingCollection.Contains(item);

        bool ICollection<KeyValuePair<string, string?>>.Contains(KeyValuePair<string, string?> item)
        {
            string key = item.Key;
            lock (_backingCollection)
            {
                if (!_backingDictionary.ContainsKey(key))
                    return false;
                string? value = item.Value;
                if (value is null)
                    return _backingCollection.Any(e => Comparer.Equals(e.Key, key) && value is null);
                return _backingCollection.Any(e => Comparer.Equals(e.Key, key) && value is not null && Comparer.Equals(e.Value, value));
            }
        }

        public bool ContainsKey(string key) => key is not null && _backingDictionary.ContainsKey(key);

        public void CopyTo(UriQueryElement[] array, int arrayIndex) => _backingCollection.CopyTo(array, arrayIndex);

        void ICollection<KeyValuePair<string, string?>>.CopyTo(KeyValuePair<string, string?>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Equals(UriQueryCollection? other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object? obj) => obj is UriQueryCollection other && Equals(other);

        public IEnumerator<UriQueryElement> GetEnumerator() => _backingCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_backingCollection).GetEnumerator();

        IEnumerator<KeyValuePair<string, string?>> IEnumerable<KeyValuePair<string, string?>>.GetEnumerator() => _backingDictionary.GetEnumerator();

        public int IndexOf(UriQueryElement item) => _backingCollection.IndexOf(item);

        public void Insert(int index, UriQueryElement item)
        {
            lock (_backingCollection)
            {
                int oldIndex = _backingCollection.IndexOf(item);
                if (oldIndex < 0)
                {
                    if (!_backingDictionary.ContainsKey(item.Key))
                        _backingDictionary.Add(item.Key, item.Value);
                    _backingCollection.Insert(index, item);
                }
                else if (oldIndex > index)
                {
                    _backingCollection.Insert(index, item);
                    _backingCollection.RemoveAt(index + 1);
                }
                else if (index > oldIndex + 1)
                {
                    _backingCollection.Insert(index, item);
                    _backingCollection.RemoveAt(oldIndex);
                }
            }
        }

        public bool Remove(string key)
        {
            if (key is null)
                return false;
            lock (_backingCollection)
            {
                if (!_backingDictionary.Remove(key))
                    return false;
                if (_backingDictionary.Count == 0)
                    _backingCollection.Clear();
                else
                    foreach (UriQueryElement item in _backingCollection.Where(k => Comparer.Equals(k.Key, key)).ToArray())
                        _backingCollection.Remove(item);
            }
            return true;
        }

        public bool Remove(UriQueryElement item)
        {
            lock (_backingCollection)
            {
                if (!_backingCollection.Remove(item))
                    return false;
                if (_backingCollection.Count == 0)
                    _backingDictionary.Clear();
                else
                {
                    string key = item.Key;
                    if (!_backingCollection.Any(e => Comparer.Equals(e.Key, key)))
                        _backingDictionary.Remove(key);
                }
            }
            return true;
        }

        bool ICollection<KeyValuePair<string, string?>>.Remove(KeyValuePair<string, string?> item)
        {
            lock (_backingCollection)
            {
                string key = item.Key;
                string? value = item.Value;
                if (!_backingDictionary.ContainsKey(key))
                    return false;
                if (value is null)
                    for (int i = 0; i < _backingCollection.Count; i++)
                    {
                        UriQueryElement e = _backingCollection[i];
                        if (Comparer.Equals(e.Key, key) && e.Value is null)
                        {
                            _backingCollection.RemoveAt(i);
                            if (!_backingCollection.Any(e => Comparer.Equals(e.Key, key)))
                                _backingDictionary.Remove(key);
                            return true;
                        }
                    }
                else
                    for (int i = 0; i < _backingCollection.Count; i++)
                    {
                        UriQueryElement e = _backingCollection[i];
                        if (Comparer.Equals(e.Key, key) && e.Value is not null && Comparer.Equals(e.Value, value))
                        {
                            _backingCollection.RemoveAt(i);
                            if (!_backingCollection.Any(e => Comparer.Equals(e.Key, key)))
                                _backingDictionary.Remove(key);
                            return true;
                        }
                    }
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            lock (_backingCollection)
            {
                string key = _backingCollection[index].Key;
                _backingCollection.RemoveAt(index);
                if (!_backingCollection.Any(e => Comparer.Equals(e.Key, key)))
                    _backingDictionary.Remove(key);
            }
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out string? value) => _backingDictionary.TryGetValue(key, out value);

        public override int GetHashCode()
        {
            int result = 3;
            unchecked
            {
                foreach (UriQueryElement e in _backingCollection)
                    result = (result * 7) + e.GetHashCode();
            }
            return result;
        }

        public string ToString(bool noLeadingQueryChar)
        {
            lock (_backingCollection)
                return _backingCollection.Count switch
                {
                    0 => noLeadingQueryChar ? string.Empty : QUERY_LEAD_CHAR,
                    1 => noLeadingQueryChar ? _backingCollection[0].ToString() : QUERY_LEAD_CHAR + _backingCollection[0].ToString(),
                    _ => noLeadingQueryChar ? string.Join(QUERY_SEPARATOR, _backingCollection.Select(e => e.ToString())) :
                                                QUERY_LEAD_CHAR + string.Join(QUERY_SEPARATOR, _backingCollection.Select(e => e.ToString())),
                };
        }
        public override string ToString() => ToString(false);
    }
}
