namespace NuGetPuller;

public sealed class TempStagingFolder : IDisposable
{
    private DirectoryInfo _directory;

    public DirectoryInfo Directory => _directory ?? throw new ObjectDisposedException(nameof(TempStagingFolder));

    public TempStagingFolder()
    {
        while ((_directory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("n")))).Exists)
            _directory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("n")));
        _directory.Create();
    }

    ~TempStagingFolder()
    {
        if (_directory is not null)
            try
            {
                _directory.Refresh();
                if (_directory.Exists)
                    _directory.Delete(true);
            }
            catch { /* We did the best we could */ }
    }

    public void Dispose()
    {
        DirectoryInfo directory = _directory;
        _directory = null!;
        if (directory is not null)
            try
            {
                directory.Refresh();
                if (directory.Exists)
                    directory.Delete(true);
            }
            catch { /* We did the best we could */ }
        GC.SuppressFinalize(this);
    }
}