using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace CdnGetter.Services;

public sealed class RemoteNugetClientService : NugetClientService
{
    private readonly LazyTransform<string, SourceRepository> _sourceRepo;

    protected override LazyTransform<string, SourceRepository> SourceRepo => _sourceRepo;

    public RemoteNugetClientService(IOptions<Config.NuGetSettings> options, ILogger<RemoteNugetClientService> logger) : base(logger)
    {
        _sourceRepo = new(options.Value.V3ApiUrl.DefaultIfWhiteSpace(() => Config.NuGetSettings.DEFAULT_V3_API_URI), (uriString, setUri) =>
        {
            using var scope = Logger.BeginRepositoryUrlScope(uriString);
            Uri uri;
            try { uriString = (uri = new(uriString, UriKind.Absolute)).AbsoluteUri; }
            catch (UriFormatException error)
            {
                throw InvalidRemoteNugetRepositoryUrlException.LogAndCreate(logger, uriString, error);
            }
            catch (ArgumentException error)
            {
                throw InvalidRemoteNugetRepositoryUrlException.LogAndCreate(logger, uriString, error);
            }
            setUri(uriString);
            if (uri.IsFile)
                throw InvalidRemoteNugetRepositoryUrlException.LogAndCreate(logger, uriString);
            return Repository.Factory.GetCoreV3(uriString);
        });
    }
}