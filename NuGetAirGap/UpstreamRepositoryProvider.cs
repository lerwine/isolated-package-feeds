using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace NuGetAirGap;

public sealed class UpstreamRepositoryProvider : IRepositoryProvider
{
    private readonly object _syncRoot = new();
    private readonly ILogger<LocalRepositoryProvider> _logger;
    private Exception? _error;
    private SourceRepository? _sourceRepository;
    private Uri? _uri;

    public string OriginalString { get; }

    private bool TryParseUri()
    {
        var uriString = OriginalString;
        using var scope = _logger.BeginValidateUpstreamURLScope(uriString);
        Uri? uri;
        try
        {
            if (!Uri.TryCreate(uriString, UriKind.Absolute, out uri))
                uri = new Uri(uriString, UriKind.Absolute);
        }
        catch (UriFormatException error)
        {
            _error = _logger.LogInvalidRepositoryUrl(uriString, true, message => new InvalidRepositoryUrlException(uriString, true, error), error);
            return false;
        }
        catch (ArgumentException error)
        {
            _error = _logger.LogInvalidRepositoryUrl(uriString, true, message => new InvalidRepositoryUrlException(uriString, true, error), error);
            return false;
        }
        DirectoryInfo directoryInfo;
        if (uri.IsAbsoluteUri)
        {
            if (!uri.IsFile)
            {
                _uri = uri;
                return true;
            }
            uriString = uri.LocalPath;
            try { uriString = (directoryInfo = new(uriString)).FullName; }
            catch (System.Security.SecurityException error)
            {
                _error = _logger.LogRepositorySecurityException(uriString, true, message => new RepositorySecurityException(uriString, true, message, error), error);
                return false;
            }
            catch (PathTooLongException error)
            {
                _error = _logger.LogInvalidRepositoryUrl(uri, true, message => new InvalidRepositoryUrlException(uriString, true, error), error);
                return false;
            }
            catch (ArgumentException error)
            {
                _error = _logger.LogInvalidRepositoryUrl(uri, true, message => new InvalidRepositoryUrlException(uriString, true, error), error);
                return false;
            }
        }
        else
        {
            if (Path.GetInvalidPathChars().Any(c => uriString.Contains(c)))
            {
                _error = _logger.LogInvalidRepositoryUrl(uri, true, message => new Exception(message));
                return false;
            }
            try { uriString = (directoryInfo = new(uriString)).FullName; }
            catch (System.Security.SecurityException error)
            {
                _error = _logger.LogRepositorySecurityException(uriString, true, message => new RepositorySecurityException(uriString, true, message, error), error);
                return false;
            }
            catch (PathTooLongException error)
            {
                _error = _logger.LogInvalidRepositoryUrl(uri, true, message => new InvalidRepositoryUrlException(uriString, true, error), error);
                return false;
            }
            catch (ArgumentException error)
            {
                _error = _logger.LogInvalidRepositoryUrl(uri, true, message => new InvalidRepositoryUrlException(uriString, true, error), error);
                return false;
            }
        }
        if (directoryInfo.Exists)
        {
            _uri = uri;
            return true;
        }
        _error = (uri.IsAbsoluteUri && uri.IsFile) ? _logger.LogRepositoryPathNotFound(uriString, true, message => new RepositoryPathNotFoundException(uriString, true, message)) :
            _logger.LogInvalidRepositoryUrl(uri, true, message => new InvalidRepositoryUrlException(uriString, true));
        return false;
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
        return _uri.IsFile ? _uri.LocalPath : _uri.AbsoluteUri;
    }

    public bool GetPath(out string path)
    {
        Monitor.Enter(_syncRoot);
        try
        {
            if (_uri is null || _error is not null || !TryParseUri())
            {
                path = OriginalString;
                return false;
            }
        }
        finally { Monitor.Exit(_syncRoot); }
        if (_uri.IsFile)
        {
            path = _uri.LocalPath;
            return true;
        }
        path = _uri.AbsoluteUri;
        return false;
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

    public UpstreamRepositoryProvider(IOptions<AppSettings> options, IHostEnvironment hostingEnvironment, ILogger<LocalRepositoryProvider> logger)
    {
        OriginalString = options.Value.UpstreamServiceIndex.DefaultIfWhiteSpace(() => AppSettings.DEFAULT_UPSTREAM_SERVICE_INDEX);
        _logger = logger;
    }
}

