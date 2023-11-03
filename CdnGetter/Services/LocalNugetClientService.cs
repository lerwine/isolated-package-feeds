using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace CdnGetter.Services;

public sealed class LocalNugetClientService : NugetClientService
{
    private readonly LazyTransform<string, SourceRepository> _sourceRepo;

    protected override LazyTransform<string, SourceRepository> SourceRepo => _sourceRepo;

    public LocalNugetClientService(IOptions<Config.NuGetSettings> options, IHostEnvironment hostingEnvironment, ILogger<LocalNugetClientService> logger) : base(logger)
    {
        _sourceRepo = new(options.Value.LocalRepository.DefaultIfWhiteSpace(() => Config.NuGetSettings.DEFAULT_LOCAL_REPOSITORY), (path, setPath) =>
        {
            DirectoryInfo directoryInfo;
            using (var scope1 = Logger.BeginRepositoryUrlScope(path))
            {
                try { path = (directoryInfo = new(Path.IsPathFullyQualified(path) ? path : Path.Combine(hostingEnvironment.ContentRootPath, path))).FullName; }
                catch (System.Security.SecurityException error)
                {
                    throw LocalNugetRepositorySecurityException.LogAndCreate(logger, path, error);
                }
                catch (PathTooLongException error)
                {
                    throw InvalidLocalNugetRepositoryPathException.LogAndCreate(logger, path, error);
                }
                catch (ArgumentException error)
                {
                    throw InvalidLocalNugetRepositoryPathException.LogAndCreate(logger, path, error);
                }
            }
            setPath(path);
            using var scope2 = Logger.BeginRepositoryUrlScope(path);
            if (!directoryInfo.Exists)
                try
                {
                    if (directoryInfo.Parent is null || !directoryInfo.Parent.Exists)
                    {
                        if (File.Exists(path))
                            throw InvalidLocalNugetRepositoryPathException.LogAndCreate(logger, path);
                        directoryInfo.Create();
                    }
                }
                catch (System.Security.SecurityException error)
                {
                    throw LocalNugetRepositorySecurityException.LogAndCreate(logger, path, error);
                }
                catch (IOException error)
                {
                    throw LocalNugetRepositoryCreationException.LogAndCreate(logger, path, error);
                }
            return Repository.Factory.GetCoreV3(new PackageSource(directoryInfo.FullName));
        });
    }
}