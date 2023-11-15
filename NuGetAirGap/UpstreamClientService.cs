using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace NuGetAirGap;

public sealed class UpstreamClientService : ClientService
{
    public override bool IsUpstream => true;
    
    public UpstreamClientService(IOptions<AppSettings> appSettings, IHostEnvironment hostingEnvironment, ILogger<UpstreamClientService> logger) : base(logger, Task.Run(() =>
    {
        var uriString = appSettings.Value.UpstreamServiceIndex.DefaultIfWhiteSpace(() => AppSettings.DEFAULT_SERVICE_INDEX_URL);
        using var scope = logger.BeginValidateUpstreamURLScope(uriString);
        Uri? uri;
        try
        {
            if (!Uri.TryCreate(uriString, UriKind.Absolute, out uri))
                uri = new Uri(uriString, UriKind.Absolute);
        }
        catch (UriFormatException error)
        {
            throw logger.LogInvalidRepositoryUrl(uriString, true, message => new InvalidRepositoryUrlException(uriString, true, error), error);
        }
        catch (ArgumentException error)
        {
            throw logger.LogInvalidRepositoryUrl(uriString, true, message => new InvalidRepositoryUrlException(uriString, true, error), error);
        }
        DirectoryInfo directoryInfo;
        if (uri.IsAbsoluteUri)
        {
            if (!uri.IsFile)
                return Repository.Factory.GetCoreV3(uri.AbsoluteUri);
            uriString = uri.LocalPath;
            try { uriString = (directoryInfo = new(uriString)).FullName; }
            catch (System.Security.SecurityException error)
            {
                throw logger.LogRepositorySecurityException(uriString, true, message => new RepositorySecurityException(uriString, true, message, error), error);
            }
            catch (PathTooLongException error)
            {
                throw logger.LogInvalidRepositoryUrl(uriString, true, message => new InvalidRepositoryUrlException(uriString, true, error), error);
            }
            catch (ArgumentException error)
            {
                throw logger.LogInvalidRepositoryUrl(uriString, true, message => new InvalidRepositoryUrlException(uriString, true, error), error);
            }
        }
        else
        {
            if (Path.GetInvalidPathChars().Any(c => uriString.Contains(c)))
                throw logger.LogInvalidRepositoryUrl(uriString, true, message => new Exception(message));
            try { uriString = (directoryInfo = new(Path.IsPathFullyQualified(uriString) ? uriString : Path.Combine(hostingEnvironment.ContentRootPath, uriString))).FullName; }
            catch (System.Security.SecurityException error)
            {
                throw logger.LogRepositorySecurityException(uriString, true, message => new RepositorySecurityException(uriString, true, message, error), error);
            }
            catch (PathTooLongException error)
            {
                throw logger.LogInvalidRepositoryUrl(uriString, true, message => new InvalidRepositoryUrlException(uriString, true, error), error);
            }
            catch (ArgumentException error)
            {
                throw logger.LogInvalidRepositoryUrl(uriString, true, message => new InvalidRepositoryUrlException(uriString, true, error), error);
            }
        }
        if (directoryInfo.Exists)
            return Repository.Factory.GetCoreV3(uriString);
        if (uri.IsAbsoluteUri && uri.IsFile)
            throw logger.LogRepositoryPathNotFound(uriString, true, message => new RepositoryPathNotFoundException(uriString, true, message));
        throw logger.LogInvalidRepositoryUrl(uriString, true, message => new InvalidRepositoryUrlException(uriString, true));
    })) { }
}
