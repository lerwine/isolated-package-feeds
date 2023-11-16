using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace NuGetAirGap;

public sealed class LocalRepositoryProvider : IRepositoryProvider
{
    private readonly object _syncRoot = new();
    private readonly ILogger<LocalRepositoryProvider> _logger;
    private Exception? _error;
    private SourceRepository? _sourceRepository;
    private Uri? _uri;

    public string OriginalString { get; }

    private bool TryParseUri()
    {
        var path = OriginalString;
        using var scope = _logger.BeginValidateLocalPathScope(path);
        Uri? uri;
        try
        {
            if (!Uri.TryCreate(path, UriKind.Absolute, out uri))
                uri = new Uri(path, UriKind.Relative);
        }
        catch (UriFormatException error)
        {
            _error = _logger.LogInvalidRepositoryUrl(path, false, message => new InvalidRepositoryUrlException(path, false, message, error), error);
            return false;
        }
        catch (ArgumentException error)
        {
            _error = _logger.LogInvalidRepositoryUrl(path, false, message => new InvalidRepositoryUrlException(path, false, error), error);
            return false;
        }
        DirectoryInfo directoryInfo;
        if (uri.IsAbsoluteUri && !uri.IsFile)
        {
            if (!uri.IsFile)
            {
                _error = _logger.LogInvalidRepositoryUrl(uri, false, message => new InvalidRepositoryUrlException(uri.AbsoluteUri, false, message));
                return false;
            }
            path = uri.LocalPath;
        }
        try { path = (directoryInfo = new(path)).FullName; }
        catch (System.Security.SecurityException error)
        {
            _error = _logger.LogRepositorySecurityException(path, false, message => new RepositorySecurityException(path, false, message, error), error);
            return false;
        }
        catch (PathTooLongException error)
        {
            _error = _logger.LogInvalidRepositoryUrl(path, false, message => new InvalidRepositoryUrlException(path, false, error), error);
            return false;
        }
        catch (ArgumentException error)
        {
            _error = _logger.LogInvalidRepositoryUrl(path, false, message => new InvalidRepositoryUrlException(path, false, error), error);
            return false;
        }
        if (!directoryInfo.Exists)
        {
            if (directoryInfo.Parent is not null && directoryInfo.Parent.Exists && !File.Exists(directoryInfo.FullName))
                try { directoryInfo.Create(); }
                catch (DirectoryNotFoundException exception)
                {
                    _error = _logger.LogRepositoryPathNotFound(path, false, message => new RepositoryPathNotFoundException(path, false, message, exception), exception);
                    return false;
                }
                catch (IOException exception)
                {
                    _error = _logger.LogLocalRepositoryIOException(path, message => new LocalRepositoryIOExceptionException(path, message, exception), exception);
                    return false;
                }
                catch (System.Security.SecurityException exception)
                {
                    _error = _logger.LogRepositorySecurityException(path, false, message => new RepositorySecurityException(path, false, message, exception), exception);
                    return false;
                }
            else
            {
                _error = _logger.LogRepositoryPathNotFound(path, false, message => new RepositoryPathNotFoundException(path, false, message));
                return false;
            }
        }
        _uri = uri;
        return true;
    }

    public string GetPath()
    {
        Monitor.Enter(_syncRoot);
        try
        {
            if (_uri is null || _error is not null || !TryParseUri())
                return OriginalString;
        }
        finally { Monitor.Exit(_syncRoot); }
        return _uri.LocalPath;
    }

    public bool GetPath(out string path)
    {
        Monitor.Enter(_syncRoot);
        try
        {
            path = (_uri is null || _error is not null || !TryParseUri()) ? OriginalString : _uri.LocalPath;
        }
        finally { Monitor.Exit(_syncRoot); }
        return true;
    }

    public Uri GetUri()
    {
        Monitor.Enter(_syncRoot);
        try
        {
            if (_uri is not null)
                return _uri;
            if (_error != null || !TryParseUri())
                throw _error!;
        }
        finally { Monitor.Exit(_syncRoot); }
        return _uri!;
    }

    public SourceRepository GetSourceRepository() => _sourceRepository ??= Repository.Factory.GetCoreV3(GetUri().LocalPath);

    public LocalRepositoryProvider(IOptions<AppSettings> options, IHostEnvironment hostingEnvironment, ILogger<LocalRepositoryProvider> logger)
    {
        OriginalString = options.Value.UpstreamServiceIndex.DefaultIfWhiteSpace(() => Path.Combine(hostingEnvironment.ContentRootPath, AppSettings.DEFAULT_LOCAL_REPOSITORY)); ;
        _logger = logger;
    }
}

