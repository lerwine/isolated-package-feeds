
using System.Diagnostics.CodeAnalysis;
using CdnGetter.Versioning;

namespace CdnGetter;

public class VersionComponentComparer : StringComparer, IEqualityComparer<char>, IComparer<char>, IEqualityComparer<ITextComponent>, IComparer<ITextComponent>, IEqualityComparer<IVersionNumber>,
    IComparer<IVersionNumber>, IEqualityComparer<IVersionComponent>, IComparer<IVersionComponent>
{
    public static readonly VersionComponentComparer Instance = new();
    
    private VersionComponentComparer() { }

    public static int CompareTo(string? x, string? y)
    {
        if (x is null)
            return (y is null) ? 0 : -1;
        if (y is null)
            return 1;
        IEnumerator<char> a = x.GetEnumerator();
        IEnumerator<char> b = y.GetEnumerator();
        while (a.MoveNext())
        {
            if (!b.MoveNext())
                return 1;
            int result = CompareTo(a.Current, b.Current);
            if (result != 0)
                return result;
        }
        return b.MoveNext() ? -1 : 0;
    }
    
    public override int Compare(string? x, string? y) => CompareTo(x, y);

    public static int CompareTo(char x, char y) => char.IsUpper(x) ? (char.IsUpper(y) ? x.CompareTo(y) : char.ToLower(x).CompareTo(y)) : x.CompareTo(char.IsUpper(y) ? char.ToLower(y) : y);

    public int Compare(char x, char y) => CompareTo(x, y);

    public static int CompareTo(char x, string? y)
    {
        if (string.IsNullOrEmpty(y))
            return 1;
        int result = CompareTo(x, y[0]);
        return (result != 0) ? result : (y.Length > 1) ? -1 : 0;
    }

    public static int CompareTo(string? x, char y)
    {
        if (string.IsNullOrEmpty(x))
            return -1;
        int result = CompareTo(x[0], y);
        return (result != 0) ? result : x.Length - 1;
    }

    public int Compare(ITextComponent? x, ITextComponent? y) => (x is null) ? ((y is null) ? 0 : -1) : x.CompareTo(y);

    public int Compare(IVersionNumber? x, IVersionNumber? y) => (x is null) ? ((y is null) ? 0 : -1) : x.CompareTo(y);

    public int Compare(IVersionComponent? x, IVersionComponent? y) => (x is null) ? ((y is null) ? 0 : -1) : x.CompareTo(y);

    public static bool AreEqual(string? x, string? y)
    {
        if (x is null)
            return y is null;
        if (y is null)
            return false;
        IEnumerator<char> a = x.GetEnumerator();
        IEnumerator<char> b = y.GetEnumerator();
        while (a.MoveNext())
        {
            if (!(b.MoveNext() && AreEqual(a.Current, b.Current)))
                return false;
        }
        return !b.MoveNext();
    }

    public override bool Equals(string? x, string? y) => AreEqual(x, y);

    public bool Equals(ITextComponent? x, ITextComponent? y) => (x is null) ? y is null : x.Equals(y);

    public bool Equals(IVersionNumber? x, IVersionNumber? y) => (x is null) ? y is null : x.Equals(y);

    public bool Equals(IVersionComponent? x, IVersionComponent? y) => (x is null) ? y is null : x.Equals(y);

    public static bool AreEqual(char x, char y) => char.IsUpper(x) ? (char.IsUpper(y) ? x.Equals(y) : char.ToLower(x).Equals(y)) : x.Equals(char.IsUpper(y) ? char.ToLower(y) : y);

    public bool Equals(char x, char y) => AreEqual(x, y);

    public static bool AreEqual(char x, string? y) => y is not null && y.Length == 1 && AreEqual(x, y[0]);

    public static bool AreEqual(string? x, char y) => x is not null && x.Length == 1 && AreEqual(x[0], y);

    public static int GetHashCodeOf(string? obj)
    {
        if (string.IsNullOrEmpty(obj))
            return 0;
        int result = 3;
        unchecked
        {
            foreach (char c in obj)
                result = (result * 7) + GetHashCodeOf(c);
        }
        return result;
    }
    
    public override int GetHashCode(string obj) => GetHashCodeOf(obj);

    public static int GetHashCodeOf([DisallowNull] char obj) => (char.IsUpper(obj) ? char.ToLower(obj) : obj).GetHashCode();

    public int GetHashCode([DisallowNull] char obj) => GetHashCodeOf(obj);

    public int GetHashCode([DisallowNull] ITextComponent obj) => obj?.GetHashCode() ?? 0;

    public int GetHashCode([DisallowNull] IVersionNumber obj) => obj?.GetHashCode() ?? 0;

    public int GetHashCode([DisallowNull] IVersionComponent obj) => obj?.GetHashCode() ?? 0;
}