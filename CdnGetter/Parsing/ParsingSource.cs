using System.Collections;

namespace CdnGetter.Parsing;

public class ParsingSource : IReadOnlyList<char>
{
    public static readonly ParsingSource Empty = new(string.Empty);

    private readonly string _source;

    public char this[int index] => _source[index];

    public int Count { get; }

    public ParsingSource(string source) => Count = (_source = source ?? "").Length;

    public IEnumerator<char> GetEnumerator() => _source.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_source).GetEnumerator();

    public int IndexOf(char c) => _source.IndexOf(c);

    public int IndexOf(char c, bool ignoreCase)
    {
        if (Count == 0)
            return -1;
        if (ignoreCase)
        {
            if (char.IsLower(c))
            {
                if (_source.Any(v => char.IsUpper(v)))
                {
                    int index = -1;
                    foreach (char v in _source)
                    {
                        index++;
                        if (c.Equals(char.IsUpper(v) ? char.ToLower(v) : v))
                            return index;
                    }
                    return -1;
                }
            }
            if (char.IsUpper(c) && _source.Any(v => char.IsLower(v)))
            {
                int index = -1;
                foreach (char v in _source)
                {
                    index++;
                    if (c.Equals(char.IsLower(v) ? char.ToUpper(v) : v))
                        return index;
                }
                return -1;
            }
        }
        return _source.IndexOf(c);
    }

    public int IndexOf(char c, int startIndex) => _source.IndexOf(c, startIndex);

    public int IndexOf(char c, int startIndex, bool ignoreCase)
    {
        if (startIndex == Count)
            return -1;
        if (ignoreCase)
        {
            IEnumerable<char> values = GetCharacters(startIndex);
            if (char.IsLower(c))
            {
                if (values.Any(v => char.IsUpper(v)))
                {
                    int index = startIndex - 1;
                    foreach (char v in values)
                    {
                        index++;
                        if (c.Equals(char.IsUpper(v) ? char.ToLower(v) : v))
                            return index;
                    }
                    return -1;
                }
            }
            if (char.IsUpper(c) && values.Any(v => char.IsLower(v)))
            {
                int index = startIndex - 1;
                foreach (char v in values)
                {
                    index++;
                    if (c.Equals(char.IsLower(v) ? char.ToUpper(v) : v))
                        return index;
                }
                return -1;
            }
        }
        return (startIndex == 0) ? _source.IndexOf(c) : _source.IndexOf(c, startIndex);
    }

    public int IndexOf(char c, int startIndex, int count) => _source.IndexOf(c, startIndex, count);

    public int IndexOf(char c, int startIndex, int count, bool ignoreCase)
    {
        if (count == 0)
            return -1;
        if (ignoreCase)
        {
            IEnumerable<char> values = GetCharacters(startIndex, count);
            if (char.IsLower(c))
            {
                if (values.Any(v => char.IsUpper(v)))
                {
                    int index = startIndex - 1;
                    foreach (char v in values)
                    {
                        index++;
                        if (c.Equals(char.IsUpper(v) ? char.ToLower(v) : v))
                            return index;
                    }
                    return -1;
                }
            }
            if (char.IsUpper(c) && values.Any(v => char.IsLower(v)))
            {
                int index = startIndex - 1;
                foreach (char v in values)
                {
                    index++;
                    if (c.Equals(char.IsLower(v) ? char.ToUpper(v) : v))
                        return index;
                }
                return -1;
            }
        }
        return _source.IndexOf(c, startIndex, count);
    }

    public int IndexOfAny(params char[] options) => IndexOfAny((IEnumerable<char>)options);
    
    public int IndexOfAny(IEnumerable<char> options)
    {
        if (Count == 0 || options is null || !options.Any())
            return -1;
        options = options.Distinct();
        int index = -1;
        foreach (char v in _source)
        {
            index++;
            if (options.Any(c => c.Equals(v)))
                return index;
        }
        return -1;
    }

    public int IndexOfAny(IEnumerable<char> options, bool ignoreCase)
    {
        if (Count == 0 || options is null || !options.Any())
            return -1;
        IEnumerable<char> values = _source;
        if (ignoreCase)
        {
            if (values.Any(v => char.IsLower(v)))
            {
                if (values.Any(v => char.IsUpper(v)))
                {
                    if (options.Any(c => char.IsLower(c) || char.IsUpper(c)))
                    {
                        options = options.Select(c => char.IsUpper(c) ? char.ToLower(c): c).Distinct();
                        values = values.Select(c => char.IsUpper(c) ? char.ToLower(c): c).Distinct();
                    }
                    else
                        options = options.Distinct();
                }
                else if (options.Any(c => char.IsUpper(c)))
                    options = options.Select(c => char.IsUpper(c) ? char.ToLower(c): c).Distinct();
                else
                    options = options.Distinct();
            }
            else if (values.Any(v => char.IsUpper(v)) && options.Any(c => char.IsLower(c)))
                options = options.Select(c => char.IsLower(c) ? char.ToUpper(c): c).Distinct();
            else
                options = options.Distinct();
        }
        else
            options = options.Distinct();
        int index = -1;
        foreach (char v in values)
        {
            index++;
            if (options.Any(c => c.Equals(v)))
                return index;
        }
        return -1;
    }

    public int IndexOfAny(bool ignoreCase, params char[] options) => IndexOfAny(options, ignoreCase);
    
    public int IndexOfAny(IEnumerable<char> options, int startIndex)
    {
        if (startIndex < 0 || startIndex > Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the source string.");
        if (startIndex == Count || options is null || !options.Any())
            return -1;
        IEnumerable<char> values = GetCharacters(startIndex);
        options = options.Distinct();
        int index = startIndex - 1;
        foreach (char v in values)
        {
            index++;
            if (options.Any(c => c.Equals(v)))
                return index;
        }
        return -1;
    }

    public int IndexOfAny(int startIndex, params char[] options) => IndexOfAny(options, startIndex);
    
    public int IndexOfAny(IEnumerable<char> options, int startIndex, bool ignoreCase)
    {
        if (startIndex < 0 || startIndex > Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the source string.");
        if (startIndex == Count || options is null || !options.Any())
            return -1;
        IEnumerable<char> values = GetCharacters(startIndex);
        if (ignoreCase)
        {
            if (values.Any(v => char.IsLower(v)))
            {
                if (values.Any(v => char.IsUpper(v)))
                {
                    if (options.Any(c => char.IsLower(c) || char.IsUpper(c)))
                    {
                        options = options.Select(c => char.IsUpper(c) ? char.ToLower(c): c).Distinct();
                        values = values.Select(c => char.IsUpper(c) ? char.ToLower(c): c).Distinct();
                    }
                    else
                        options = options.Distinct();
                }
                else if (options.Any(c => char.IsUpper(c)))
                    options = options.Select(c => char.IsUpper(c) ? char.ToLower(c): c).Distinct();
                else
                    options = options.Distinct();
            }
            else if (values.Any(v => char.IsUpper(v)) && options.Any(c => char.IsLower(c)))
                options = options.Select(c => char.IsLower(c) ? char.ToUpper(c): c).Distinct();
            else
                options = options.Distinct();
        }
        else
            options = options.Distinct();
        int index = startIndex - 1;
        foreach (char v in values)
        {
            index++;
            if (options.Any(c => c.Equals(v)))
                return index;
        }
        return -1;
    }

    public int IndexOfAny(int startIndex, bool ignoreCase, params char[] options) => IndexOfAny(options, startIndex, ignoreCase);
    
    public int IndexOfAny(IEnumerable<char> options, int startIndex, int count)
    {
        if (startIndex < 0 || startIndex > Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the source string.");
        if (count < 0 || count + startIndex > Count)
            throw new ArgumentOutOfRangeException(nameof(Count), "Count must be positive and count must refer to a location within the source string.");
        if (count == 0 || options is null || !options.Any())
            return -1;
        IEnumerable<char> values = GetCharacters(startIndex, count);
        options = options.Distinct();
        int index = startIndex - 1;
        foreach (char v in values)
        {
            index++;
            if (options.Any(c => c.Equals(v)))
                return index;
        }
        return -1;
    }

    public int IndexOfAny(int startIndex, int count, params char[] options) => IndexOfAny(options, startIndex, count);
    
    public int IndexOfAny(IEnumerable<char> options, int startIndex, int count, bool ignoreCase)
    {
        if (startIndex < 0 || startIndex > Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the source string.");
        if (count < 0 || count + startIndex > Count)
            throw new ArgumentOutOfRangeException(nameof(Count), "Count must be positive and count must refer to a location within the source string.");
        if (count == 0 || options is null || !options.Any())
            return -1;
        IEnumerable<char> values = GetCharacters(startIndex, count);
        if (ignoreCase)
        {
            if (values.Any(v => char.IsLower(v)))
            {
                if (values.Any(v => char.IsUpper(v)))
                {
                    if (options.Any(c => char.IsLower(c) || char.IsUpper(c)))
                    {
                        options = options.Select(c => char.IsUpper(c) ? char.ToLower(c): c).Distinct();
                        values = values.Select(c => char.IsUpper(c) ? char.ToLower(c): c).Distinct();
                    }
                    else
                        options = options.Distinct();
                }
                else if (options.Any(c => char.IsUpper(c)))
                    options = options.Select(c => char.IsUpper(c) ? char.ToLower(c): c).Distinct();
                else
                    options = options.Distinct();
            }
            else if (values.Any(v => char.IsUpper(v)) && options.Any(c => char.IsLower(c)))
                options = options.Select(c => char.IsLower(c) ? char.ToUpper(c): c).Distinct();
            else
                options = options.Distinct();
        }
        else
            options = options.Distinct();
        int index = startIndex - 1;
        foreach (char v in values)
        {
            index++;
            if (options.Any(c => c.Equals(v)))
                return index;
        }
        return -1;
    }

    public int IndexOfAny(int startIndex, int count, bool ignoreCase, params char[] options) => IndexOfAny(options, startIndex, count, ignoreCase);
    
    public ReadOnlySpan<char> AsSpan(int startIndex, int count)
    {
        if (startIndex < 0 || startIndex > Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the source string.");
        if (count < 0 || count + startIndex > Count)
            throw new ArgumentOutOfRangeException(nameof(Count), "Count must be positive and count must refer to a location within the source string.");
        return (count > 0) ? ((startIndex > 0 && count == Count) ? _source.AsSpan() : _source.AsSpan(startIndex, count)) : []; 
    }

    public IEnumerable<char> GetCharacters(int startIndex, int count)
    {
        if (startIndex < 0 || startIndex > Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the source string.");
        if (count < 0 || count + startIndex > Count)
            throw new ArgumentOutOfRangeException(nameof(Count), "Count must be positive and count must refer to a location within the source string.");
        return (count > 0) ? ((startIndex > 0) ? _source.Skip(startIndex).Take(count) : (count == Count) ? _source : _source.Take(count)) : Enumerable.Empty<char>(); 
    }

    public IEnumerable<char> GetCharacters(int startIndex)
    {
        if (startIndex < 0 || startIndex > Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the source string.");
        return (startIndex > 0) ? ((startIndex < Count) ? string.Empty : _source[startIndex..]) : _source;
    }

    public string ToString(int startIndex, int count)
    {
        if (startIndex < 0 || startIndex > Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the source string.");
        if (count < 0 || count + startIndex > Count)
            throw new ArgumentOutOfRangeException(nameof(Count), "Count must be positive and count must refer to a location within the source string.");
        return (count > 0) ? ((startIndex > 0) ? _source.Substring(startIndex, count) : (count == Count) ? _source : _source[..count]) : string.Empty;
    }

    public string ToString(int startIndex)
    {
        if (startIndex < 0 || startIndex > Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the source string.");
        return (startIndex > 0) ? ((startIndex < Count) ? string.Empty : _source[startIndex..]) : _source;
    }

    public override string ToString() => _source;
}
