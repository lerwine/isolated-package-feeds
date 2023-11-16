using NuGet.Protocol.Core.Types;

namespace NuGetAirGap;
public interface IRepositoryProvider : IUrlProvider
{
    SourceRepository GetSourceRepository();
}

