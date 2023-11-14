using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace NuGetAirGap;

public sealed class LocalClientService : ClientService
{
    public override bool IsUpstream => false;

    public LocalClientService(IOptions<AppSettings> appSettings, IHostEnvironment hostingEnvironment, ILogger<UpstreamClientService> logger) : base(logger, Task.Run(() =>
    {
        var uriString = appSettings.Value.ServiceIndexUrl.DefaultIfWhiteSpace(() => AppSettings.DEFAULT_SERVICE_INDEX_URL);
        using var scope = logger.BeginValidateUpstreamURLScope(uriString);
        Uri? uri;
        try
        {
            if (!Uri.TryCreate(uriString, UriKind.Absolute, out uri))
                uri = new Uri(uriString, UriKind.Absolute);
        }
        catch (UriFormatException error)
        {
            throw logger.LogInvalidRepositoryUrl(uriString, false, message => new InvalidRepositoryUrlException(uriString, false, message, error), error);
        }
        catch (ArgumentException error)
        {
            throw logger.LogInvalidRepositoryUrl(uriString, false, message => new InvalidRepositoryUrlException(uriString, false, error), error);
        }
        DirectoryInfo directoryInfo;
        if (uri.IsAbsoluteUri)
        {
            if (!uri.IsFile)
                throw logger.LogInvalidRepositoryUrl(uri, false, message => new InvalidRepositoryUrlException(uri.AbsoluteUri, false, message));
            uriString = uri.LocalPath;
            try { uriString = (directoryInfo = new(uriString)).FullName; }
            catch (System.Security.SecurityException error)
            {
                throw logger.LogRepositorySecurityException(uriString, false, message => new RepositorySecurityException(uriString, false, message, error), error);
            }
            catch (PathTooLongException error)
            {
                throw logger.LogInvalidRepositoryUrl(uriString, false, message => new InvalidRepositoryUrlException(uriString, false, error), error);
            }
            catch (ArgumentException error)
            {
                throw logger.LogInvalidRepositoryUrl(uriString, false, message => new InvalidRepositoryUrlException(uriString, false, error), error);
            }
        }
        else
            try { uriString = (directoryInfo = new(Path.IsPathFullyQualified(uriString) ? uriString : Path.Combine(hostingEnvironment.ContentRootPath, uriString))).FullName; }
            catch (System.Security.SecurityException error)
            {
                throw logger.LogRepositorySecurityException(uriString, false, message => new RepositorySecurityException(uriString, false, message, error), error);
            }
            catch (PathTooLongException error)
            {
                throw logger.LogInvalidRepositoryUrl(uriString, false, message => new InvalidRepositoryUrlException(uriString, false, error), error);
            }
            catch (ArgumentException error)
            {
                throw logger.LogInvalidRepositoryUrl(uriString, false, message => new InvalidRepositoryUrlException(uriString, false, error), error);
            }
        if (directoryInfo.Exists)
            return Repository.Factory.GetCoreV3(uriString);
        throw logger.LogRepositoryPathNotFound(uriString, false, message => new RepositoryPathNotFoundException(uriString, false, message));
    })) { }

    public async Task DeleteAsync(IEnumerable<string> packageIds, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    public async Task AddAsync(IEnumerable<string> packageIds, UpstreamClientService upstreamClientService, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

