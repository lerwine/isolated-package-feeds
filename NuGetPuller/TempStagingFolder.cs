using System.Collections.Immutable;
using System.Text;

namespace NuGetPuller;

public sealed class TempStagingFolder : IDisposable
{
    private static readonly ImmutableArray<char> _invalidFileNameChars = [.. Path.GetInvalidFileNameChars()];

    private DirectoryInfo _directory;

    public DirectoryInfo Directory => _directory ?? throw new ObjectDisposedException(nameof(TempStagingFolder));

    public FileInfo WriteNewFileInfo(string fileName, Action<StreamWriter> writeContent, Encoding? encoding = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        if (fileName.Any(c => _invalidFileNameChars.Contains(c)))
            throw new ArgumentException("Invalid file name", nameof(fileName));
        FileInfo result = new(Path.Combine(_directory.FullName, fileName));
        if (result.Exists)
            throw new InvalidOperationException($"File already exists: {result.FullName}");
        if (encoding is null)
            using (var writer = result.CreateText())
            {
                writeContent(writer);
                writer.Flush();
            }
        else
            using (var stream = result.Create())
            {
                using var writer = new StreamWriter(stream, encoding);
                writeContent(writer);
                writer.Flush();
            }
        result.Refresh();
        return result;
    }

    public Task<FileInfo> WriteNewFileInfoAsync(string fileName, Func<StreamWriter, CancellationToken, Task> writeContentAsync, CancellationToken cancellationToken) => WriteNewFileInfoAsync(fileName, writeContentAsync, null, cancellationToken);

    public async Task<FileInfo> WriteNewFileInfoAsync(string fileName, Func<StreamWriter, CancellationToken, Task> writeContentAsync, Encoding? encoding, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        if (fileName.Any(c => _invalidFileNameChars.Contains(c)))
            throw new ArgumentException("Invalid file name", nameof(fileName));
        FileInfo result = new(Path.Combine(_directory.FullName, fileName));
        if (result.Exists)
            throw new InvalidOperationException($"File already exists: {result.FullName}");
        if (encoding is null)
            using (var writer = result.CreateText())
            {
                await writeContentAsync(writer, cancellationToken);
                await writer.FlushAsync(cancellationToken);
            }
        else
            using (var stream = result.Create())
            {
                using var writer = new StreamWriter(stream, encoding);
                await writeContentAsync(writer, cancellationToken);
                await writer.FlushAsync(cancellationToken);
            }
        result.Refresh();
        return result;
    }

    public FileInfo NewFileInfo(string fileName, Action<FileStream>? writeContent = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        if (fileName.Any(c => _invalidFileNameChars.Contains(c)))
            throw new ArgumentException("Invalid file name", nameof(fileName));
        FileInfo result = new(Path.Combine(_directory.FullName, fileName));
        if (result.Exists)
            throw new InvalidOperationException($"File already exists: {result.FullName}");
        using (var stream = result.Create())
        {
            if (writeContent is not null)
            {
                writeContent(stream);
                stream.Flush();
            }
        }
        result.Refresh();
        return result;
    }

    public Task<FileInfo> NewFileInfoAsync(string fileName, CancellationToken cancellationToken) => NewFileInfoAsync(fileName, null, cancellationToken);

    public async Task<FileInfo> NewFileInfoAsync(string fileName, Func<FileStream, CancellationToken, Task>? writeContentAsync, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        if (fileName.Any(c => _invalidFileNameChars.Contains(c)))
            throw new ArgumentException("Invalid file name", nameof(fileName));
        FileInfo result = new(Path.Combine(_directory.FullName, fileName));
        if (result.Exists)
            throw new InvalidOperationException($"File already exists: {result.FullName}");
        using (var stream = result.Create())
        {
            if (writeContentAsync is not null)
            {
                await writeContentAsync(stream, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }
        }
        result.Refresh();
        return result;
    }

    public FileInfo WriteNewRandomFileInfo(Action<StreamWriter> writeContent, Encoding? encoding) => WriteNewRandomFileInfo(writeContent, null, encoding);

    public FileInfo WriteNewRandomFileInfo(Action<StreamWriter> writeContent, string? extension = null, Encoding? encoding = null)
    {
        FileInfo result;
        if (string.IsNullOrWhiteSpace(extension))
        {
            result = new(Path.Combine(_directory.FullName, Guid.NewGuid().ToString("n")));
            while (result.Exists)
                result = new(Path.Combine(_directory.FullName, Guid.NewGuid().ToString("n")));
        }
        else
        {
            if (extension[0] != '.') extension = $".{extension}";
            result = new(Path.Combine(_directory.FullName, $"{Guid.NewGuid():n}{extension}"));
            while (result.Exists)
                result = new(Path.Combine(_directory.FullName, $"{Guid.NewGuid():n}{extension}"));
        }
        if (encoding is null)
            using (var writer = result.CreateText())
            {
                writeContent(writer);
                writer.Flush();
            }
        else
            using (var stream = result.Create())
            {
                using var writer = new StreamWriter(stream, encoding);
                writeContent(writer);
                writer.Flush();
            }
        result.Refresh();
        return result;
    }

    public FileInfo NewRandomFileInfo(string? extension) => NewRandomFileInfo(null, extension);

    public FileInfo NewRandomFileInfo(Action<FileStream>? writeContent = null, string? extension = null)
    {
        FileInfo result;
        if (string.IsNullOrWhiteSpace(extension))
        {
            result = new(Path.Combine(_directory.FullName, Guid.NewGuid().ToString("n")));
            while (result.Exists)
                result = new(Path.Combine(_directory.FullName, Guid.NewGuid().ToString("n")));
        }
        else
        {
            if (extension[0] != '.') extension = $".{extension}";
            result = new(Path.Combine(_directory.FullName, $"{Guid.NewGuid():n}{extension}"));
            while (result.Exists)
                result = new(Path.Combine(_directory.FullName, $"{Guid.NewGuid():n}{extension}"));
        }
        using (var stream = result.Create())
        {
            if (writeContent is not null)
            {
                writeContent(stream);
                stream.Flush();
            }
        }
        result.Refresh();
        return result;
    }

    public Task<FileInfo> WriteNewRandomFileInfoAsync(Func<StreamWriter, CancellationToken, Task> writeContentAsync, CancellationToken cancellationToken) => WriteNewRandomFileInfoAsync(writeContentAsync, null, null, cancellationToken);

    public Task<FileInfo> WriteNewRandomFileInfoAsync(Func<StreamWriter, CancellationToken, Task> writeContentAsync, string? extension, CancellationToken cancellationToken) => WriteNewRandomFileInfoAsync(writeContentAsync, extension, null, cancellationToken);

    public Task<FileInfo> WriteNewRandomFileInfoAsync(Func<StreamWriter, CancellationToken, Task> writeContentAsync, Encoding? encoding, CancellationToken cancellationToken) => WriteNewRandomFileInfoAsync(writeContentAsync, null, encoding, cancellationToken);

    public async Task<FileInfo> WriteNewRandomFileInfoAsync(Func<StreamWriter, CancellationToken, Task> writeContentAsync, string? extension, Encoding? encoding, CancellationToken cancellationToken)
    {
        FileInfo result;
        if (string.IsNullOrWhiteSpace(extension))
        {
            result = new(Path.Combine(_directory.FullName, Guid.NewGuid().ToString("n")));
            while (result.Exists)
                result = new(Path.Combine(_directory.FullName, Guid.NewGuid().ToString("n")));
        }
        else
        {
            if (extension[0] != '.') extension = $".{extension}";
            result = new(Path.Combine(_directory.FullName, $"{Guid.NewGuid():n}{extension}"));
            while (result.Exists)
                result = new(Path.Combine(_directory.FullName, $"{Guid.NewGuid():n}{extension}"));
        }
        cancellationToken.ThrowIfCancellationRequested();
        if (encoding is null)
            using (var writer = result.CreateText())
            {
                await writeContentAsync(writer, cancellationToken);
                await writer.FlushAsync(cancellationToken);
            }
        else
            using (var stream = result.Create())
            {
                using var writer = new StreamWriter(stream, encoding);
                await writeContentAsync(writer, cancellationToken);
                await writer.FlushAsync(cancellationToken);
            }
        result.Refresh();
        return result;
    }

    public Task<FileInfo> NewRandomFileInfoAsync(Func<FileStream, CancellationToken, Task> writeContentAsync, CancellationToken cancellationToken) => NewRandomFileInfoAsync(writeContentAsync, null, cancellationToken);

    public async Task<FileInfo> NewRandomFileInfoAsync(Func<FileStream, CancellationToken, Task> writeContentAsync, string? extension, CancellationToken cancellationToken)
    {
        FileInfo result;
        if (string.IsNullOrWhiteSpace(extension))
        {
            result = new(Path.Combine(_directory.FullName, Guid.NewGuid().ToString("n")));
            while (result.Exists)
                result = new(Path.Combine(_directory.FullName, Guid.NewGuid().ToString("n")));
        }
        else
        {
            if (extension[0] != '.') extension = $".{extension}";
            result = new(Path.Combine(_directory.FullName, $"{Guid.NewGuid():n}{extension}"));
            while (result.Exists)
                result = new(Path.Combine(_directory.FullName, $"{Guid.NewGuid():n}{extension}"));
        }
        using (var stream = result.Create())
        {
            await writeContentAsync(stream, cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }
        result.Refresh();
        return result;
    }

    public DirectoryInfo NewRandomDirectoryInfo()
    {
        DirectoryInfo result = new(Path.Combine(_directory.FullName, Guid.NewGuid().ToString("n")));
        while (result.Exists)
            result = new(Path.Combine(_directory.FullName, Guid.NewGuid().ToString("n")));
        result.Create();
        result.Refresh();
        return result;
    }

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