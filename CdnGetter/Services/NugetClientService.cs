using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace CdnGetter.Services
{
    public class NugetClientService
    {
        private readonly ILogger<NugetClientService> _logger;
        private readonly NugetLogWrapper _nugetLogger;
        private readonly PackageSource _packageSource;
        private readonly SourceRepository _sourceRepository;

        public NugetClientService(IOptions<Config.NuGetSettings> options, ILogger<NugetClientService> logger)
        {
            _nugetLogger = new(_logger = logger);
            string endpointUri = options.Value.V3ApiUrl.DefaultIfWhiteSpace(() => Config.NuGetSettings.DEFAULT_V3_API_URI);
            List<Lazy<INuGetResourceProvider>> providers = new();
            providers.AddRange(Repository.Provider.GetCoreV3());
            _packageSource = new(endpointUri);
            _sourceRepository = new SourceRepository(_packageSource, providers);
        }

        public async Task<PackageMetadataResource> GetPackageMetadataResourceAsync(CancellationToken cancellationToken) => await _sourceRepository.GetResourceAsync<PackageMetadataResource>(cancellationToken);

        public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, CancellationToken cancellationToken) =>
            GetMetadataAsync(packageId, false, false, null, cancellationToken);
            
        public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, CancellationToken cancellationToken) =>
            GetMetadataAsync(packageId, includePrerelease, false, null, cancellationToken);
            
        public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, PackageMetadataResource packageMetadataResource, CancellationToken cancellationToken) =>
            GetMetadataAsync(packageId, false, false, packageMetadataResource, cancellationToken);
            
        public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, bool includeUnlisted, CancellationToken cancellationToken) =>
            GetMetadataAsync(packageId, includePrerelease, includeUnlisted, null, cancellationToken);
            
        public Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, PackageMetadataResource packageMetadataResource, CancellationToken cancellationToken) =>
            GetMetadataAsync(packageId, includePrerelease, false, packageMetadataResource, cancellationToken);
            
        public async Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId, bool includePrerelease, bool includeUnlisted, PackageMetadataResource? packageMetadataResource, CancellationToken cancellationToken)
        {
            packageMetadataResource ??= await _sourceRepository.GetResourceAsync<PackageMetadataResource>(cancellationToken);
            return await packageMetadataResource.GetMetadataAsync(packageId, includePrerelease, includeUnlisted, _nugetLogger, cancellationToken);
        }
    }
}