namespace IsolatedPackageFeeds.Shared
{
    public class InvertedComparer<T>(IComparer<T> backingComparer) : IComparer<T>
    {
        private readonly IComparer<T> _backingComparer = backingComparer;

        public int Compare(T? x, T? y) => 0 - _backingComparer.Compare(x, y);

        public static readonly InvertedComparer<T> Default = new(Comparer<T>.Default);
    }

    public static class InvertedComparer
    {
        public static readonly InvertedComparer<string> StringInvariantCultureIgnoreCase = new(StringComparer.InvariantCultureIgnoreCase);
        public static readonly InvertedComparer<string> StringOrdinalIgnoreCase = new(StringComparer.OrdinalIgnoreCase);
        public static readonly InvertedComparer<string> StringCurrentCultureIgnoreCase = new(StringComparer.CurrentCultureIgnoreCase);
        public static readonly InvertedComparer<string> StringCurrentCulture = new(StringComparer.CurrentCulture);
        public static readonly InvertedComparer<string> StringInvariantCulture = new(StringComparer.InvariantCulture);
    }
}