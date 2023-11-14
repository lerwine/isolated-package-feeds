
using System.Diagnostics.CodeAnalysis;
using CdnGetter.Versioning;

namespace CdnGetter
{
    public static class NoCaseComparer
    {
        private static readonly Impl _instance = new();
        public static IEqualityComparer<ITextComponent> EqualityComparer => _instance;
        public static IComparer<ITextComponent> Comparer => _instance;
        public static IEqualityComparer<char> EqualityCharComparer => _instance;
        public static IComparer<char> CharComparer => _instance;
        public static StringComparer StringComparer => _instance;
        
        public static int Compare(string? x, string? y)
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
                int result = Compare(a.Current, b.Current);
                if (result != 0)
                    return result;
            }
            return b.MoveNext() ? -1 : 0;
        }
        
        public static int Compare(char x, char y) => char.IsUpper(x) ? (char.IsUpper(y) ? x.CompareTo(y) : char.ToLower(x).CompareTo(y)) : x.CompareTo(char.IsUpper(y) ? char.ToLower(y) : y);

        public static int Compare(char x, string? y)
        {
            if (string.IsNullOrEmpty(y))
                return 1;
            int result = Compare(x, y[0]);
            return (result != 0) ? result : (y.Length > 1) ? -1 : 0;
        }

        public static int Compare(string? x, char y)
        {
            if (string.IsNullOrEmpty(x))
                return -1;
            int result = Compare(x[0], y);
            return (result != 0) ? result : x.Length - 1;
        }

        public static int Compare(char x, ITextComponent? y)
        {
            if (y is null)
                return 1;
            if (y is CharacterComponent charComponent)
                return Compare(x, charComponent.Value);
            if (y is StringComponent stringComponent)
                return Compare(x, stringComponent.Text);
            
            IEnumerator<char> b = y.GetEnumerator();
            if (!b.MoveNext())
                return 1;
            int result = Compare(x, b.Current);
            return (result != 0) ? result : b.MoveNext() ? -1 : 0;
        }

        public static int Compare(string x, ITextComponent? y)
        {
            if (y is null)
                return 1;
            if (y is CharacterComponent charComponent)
                return Compare(x, charComponent.Value);
            if (y is StringComponent stringComponent)
                return Compare(x, stringComponent.Text);
            IEnumerator<char> a = x.GetEnumerator();
            IEnumerator<char> b = y.GetEnumerator();
            while (a.MoveNext())
            {
                if (!b.MoveNext())
                    return 1;
                int result = Compare(a.Current, b.Current);
                if (result != 0)
                    return result;
            }
            return b.MoveNext() ? -1 : 0;
        }

        public static bool Equals(string? x, string? y)
        {
            if (x is null)
                return y is null;
            if (y is null)
                return false;
            IEnumerator<char> a = x.GetEnumerator();
            IEnumerator<char> b = y.GetEnumerator();
            while (a.MoveNext())
            {
                if (!(b.MoveNext() && Equals(a.Current, b.Current)))
                    return false;
            }
            return !b.MoveNext();
        }

        public static bool Equals(char x, ITextComponent? y)
        {
            if (y is null)
                return false;
            if (y is CharacterComponent charComponent)
                return Equals(x, charComponent.Value);
            if (y is StringComponent)
                return false;
            IEnumerator<char> b = y.GetEnumerator();
            return b.MoveNext() && Equals(x, b.Current) && !b.MoveNext();
        }

        public static bool Equals(string? x, ITextComponent? y)
        {
            if (x is null)
                return y is null;
            if (y is null)
                return false;
            if (y is CharacterComponent charComponent)
                return x.Length == 1 && Equals(x[0], charComponent.Value);
            if (y is StringComponent stringComponent)
                return Equals(x, stringComponent.Text);
            IEnumerator<char> a = x.GetEnumerator();
            IEnumerator<char> b = y.GetEnumerator();
            while (a.MoveNext())
            {
                if (!(b.MoveNext() && Equals(a.Current, b.Current)))
                    return false;
            }
            return !b.MoveNext();
        }

        public static bool Equals(char x, char y) => char.IsUpper(x) ? (char.IsUpper(y) ? x.Equals(y) : char.ToLower(x).Equals(y)) : x.Equals(char.IsUpper(y) ? char.ToLower(y) : y);

        public static bool Equals(char x, string? y) => y is not null && y.Length == 1 && Equals(x, y[0]);

        public static bool Equals(string? x, char y) => x is not null && x.Length == 1 && Equals(x[0], y);

        public static int GetHashCode(string? obj)
        {
            if (string.IsNullOrEmpty(obj))
                return 0;
            int result = 3;
            unchecked
            {
                foreach (char c in obj)
                    result = (result * 7) + GetHashCode(c);
            }
            return result;
        }

        public static int GetHashCode([DisallowNull] char obj) => (char.IsUpper(obj) ? char.ToLower(obj) : obj).GetHashCode();

        class Impl : StringComparer, IEqualityComparer<char>, IComparer<char>, IEqualityComparer<ITextComponent>, IComparer<ITextComponent>
        {
            public override int Compare(string? x, string? y) => NoCaseComparer.Compare(x, y);

            public int Compare(char x, char y) => NoCaseComparer.Compare(x, y);

            public int Compare(ITextComponent? x, ITextComponent? y)
            {
                if (x is null)
                    return (y is null) ? 0 : -1;
                if (y is null)
                    return 1;
                if (x is CharacterComponent xc && y is CharacterComponent yc)
                    return Compare(xc.Value, yc.Value);
                IEnumerator<char> a = x.GetEnumerator();
                IEnumerator<char> b = y.GetEnumerator();
                while (a.MoveNext())
                {
                    if (!b.MoveNext())
                        return 1;
                    int result = Compare(a.Current, b.Current);
                    if (result != 0)
                        return result;
                }
                return b.MoveNext() ? -1 : 0;
            }

            public override bool Equals(string? x, string? y) => NoCaseComparer.Equals(x, y);

            public bool Equals(char x, char y) => NoCaseComparer.Equals(x, y);

            public bool Equals(ITextComponent? x, ITextComponent? y)
            {
                if (x is null)
                    return y is null;
                if (y is null)
                    return false;
                if (x is CharacterComponent xc && y is CharacterComponent yc)
                    return Equals(xc.Value, yc.Value);
                IEnumerator<char> a = x.GetEnumerator();
                IEnumerator<char> b = y.GetEnumerator();
                while (a.MoveNext())
                {
                    if (!(b.MoveNext() && Equals(a.Current, b.Current)))
                        return false;
                }
                return !b.MoveNext();
            }

            public override int GetHashCode(string obj) => NoCaseComparer.GetHashCode(obj);

            public int GetHashCode([DisallowNull] char obj) => NoCaseComparer.GetHashCode(obj);

            public int GetHashCode([DisallowNull] ITextComponent obj)
            {
                if (obj is null)
                    return 0;
                if (obj is CharacterComponent charComponent)
                    return GetHashCode(charComponent.Value);
                if (obj is StringComponent stringComponent)
                    return GetHashCode(stringComponent.Text);
                IEnumerator<char> enumerator = obj.GetEnumerator();
                if (!enumerator.MoveNext())
                    return 0;
                int result = 3;
                unchecked
                {
                    do
                    {
                        result = (result * 7) + GetHashCode(enumerator.Current);
                    }
                    while (enumerator.MoveNext());
                }
                return result;
            }
        }
    }
}