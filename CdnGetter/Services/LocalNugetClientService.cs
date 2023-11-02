using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace CdnGetter.Services;

public sealed class LocalNugetClientService : NugetClientService
{
    private readonly Task<SourceRepository> _getRepositoryAsync;

    protected override Task<SourceRepository> GetRepositoryAsync() => _getRepositoryAsync;

    public LocalNugetClientService(IOptions<Config.NuGetSettings> options, IHostEnvironment hostingEnvironment, ILogger<LocalNugetClientService> logger) : base(options, logger)
    {
        string repositoryPath = options.Value.LocalRepository.DefaultIfWhiteSpace(() => Config.NuGetSettings.DEFAULT_LOCAL_REPOSITORY);
        _getRepositoryAsync = Task.Run(() =>
        {
            DirectoryInfo directoryInfo;
            using (var scope1 = Logger.BeginRepositoryUrlScope(repositoryPath))
            {
                try { repositoryPath = (directoryInfo = new(Path.IsPathFullyQualified(repositoryPath) ? repositoryPath : Path.Combine(hostingEnvironment.ContentRootPath, repositoryPath))).FullName; }
                catch (System.Security.SecurityException error)
                {
                    throw LocalNugetRepositorySecurityException.LogAndCreate(logger, repositoryPath, error);
                }
                catch (PathTooLongException error)
                {
                    throw InvalidLocalNugetRepositoryPathException.LogAndCreate(logger, repositoryPath, error);
                }
                catch (ArgumentException error)
                {
                    throw InvalidLocalNugetRepositoryPathException.LogAndCreate(logger, repositoryPath, error);
                }
            }
            using var scope2 = Logger.BeginRepositoryUrlScope(repositoryPath);
            if (!directoryInfo.Exists)
                try
                {
                    if (directoryInfo.Parent is null || !directoryInfo.Parent.Exists)
                    {
                        if (File.Exists(repositoryPath))
                            throw InvalidLocalNugetRepositoryPathException.LogAndCreate(logger, repositoryPath);
                        directoryInfo.Create();
                    }
                }
                catch (System.Security.SecurityException error)
                {
                    throw LocalNugetRepositorySecurityException.LogAndCreate(logger, repositoryPath, error);
                }
                catch (IOException error)
                {
                    throw LocalNugetRepositoryCreationException.LogAndCreate(logger, repositoryPath, error);
                }
            return Repository.Factory.GetCoreV3(new PackageSource(directoryInfo.FullName));
        });
    }
}