namespace NuGetPuller;

public class FileSystemInfoComparer : IEqualityComparer<FileInfo>, IEqualityComparer<DirectoryInfo>, IEqualityComparer<FileSystemInfo>, IComparer<FileInfo>, IComparer<DirectoryInfo>, IComparer<FileSystemInfo>
{
    private static readonly StringComparer _backingComparer = StringComparer.InvariantCultureIgnoreCase;

    public static FileSystemInfoComparer Instance { get; } = new();

    public int Compare(FileInfo? x, FileInfo? y) => (x is null) ? ((y is null) ? 0 : -1) : (y is null) ? 1 : ReferenceEquals(x, y) ? 0 : _backingComparer.Compare(x.FullName, y.FullName);

    public int Compare(DirectoryInfo? x, DirectoryInfo? y) => (x is null) ? ((y is null) ? 0 : -1) : (y is null) ? 1 : ReferenceEquals(x, y) ? 0 : _backingComparer.Compare(x.FullName, y.FullName);

    public int Compare(FileSystemInfo? x, FileSystemInfo? y) => (x is null) ? ((y is null) ? 0 : -1) : (y is null) ? 1 : ReferenceEquals(x, y) ? 0 :
        (x is FileInfo) ? ((y is FileInfo) ? _backingComparer.Compare(x.FullName, y.FullName) : -1) : (y is FileInfo) ? 1 : _backingComparer.Compare(x.FullName, y.FullName);

    public bool Equals(FileInfo? x, FileInfo? y) => (x is null) ? y is null : y is not null && (ReferenceEquals(x, y) || _backingComparer.Equals(x.FullName, y.FullName));

    public bool Equals(DirectoryInfo? x, DirectoryInfo? y) => (x is null) ? y is null : y is not null && (ReferenceEquals(x, y) || _backingComparer.Equals(x.FullName, y.FullName));

    public bool Equals(FileSystemInfo? x, FileSystemInfo? y) => (x is null) ? y is null : y is not null && (ReferenceEquals(x, y) || ((x is FileInfo) == (y is FileInfo)) && _backingComparer.Equals(x.FullName, y.FullName));

    int IEqualityComparer<FileInfo>.GetHashCode(FileInfo obj) => (obj is null) ? 0 : _backingComparer.GetHashCode(obj.FullName);

    int IEqualityComparer<DirectoryInfo>.GetHashCode(DirectoryInfo obj) => (obj is null) ? 0 : _backingComparer.GetHashCode(obj.FullName);

    int IEqualityComparer<FileSystemInfo>.GetHashCode(FileSystemInfo obj)
    {
        if (obj is null)
            return 0;
        unchecked
        {
            return ((obj is FileInfo) ? 22 : 21) * 7 + _backingComparer.GetHashCode(obj.FullName);
        }
    }
}
