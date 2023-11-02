using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace CdnGetter.Services;

public sealed class RemoteNugetClientService : NugetClientService
{
    private readonly Task<SourceRepository> _getRepositoryAsync;

    protected override Task<SourceRepository> GetRepositoryAsync() => _getRepositoryAsync;
    
    public RemoteNugetClientService(IOptions<Config.NuGetSettings> options, ILogger<RemoteNugetClientService> logger) : base(options, logger)
    {
        string endpointUri = options.Value.V3ApiUrl.DefaultIfWhiteSpace(() => Config.NuGetSettings.DEFAULT_V3_API_URI);
        _getRepositoryAsync = Task.Run(() =>
        {
            using var scope = Logger.BeginRepositoryUrlScope(endpointUri);
            Uri uri;
            try { uri = new(endpointUri, UriKind.Absolute); }
            catch (UriFormatException error)
            {
                throw InvalidRemoteNugetRepositoryUrlException.LogAndCreate(logger, endpointUri, error);
            }
            catch (ArgumentException error)
            {
                throw InvalidRemoteNugetRepositoryUrlException.LogAndCreate(logger, endpointUri, error);
            }
            if (uri.IsFile)
                throw InvalidRemoteNugetRepositoryUrlException.LogAndCreate(logger, endpointUri);
            return Repository.Factory.GetCoreV3(endpointUri);
        });
    }
}