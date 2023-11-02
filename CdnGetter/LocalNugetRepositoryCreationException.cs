using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace CdnGetter;

[Serializable]
public class LocalNugetRepositoryCreationException : LoggedException
{
    public string Path { get; private set; } = null!;

    public LocalNugetRepositoryCreationException() { }

    private LocalNugetRepositoryCreationException(string message, string path, IOException inner) : base(message, inner) => Path = path;
    
    protected LocalNugetRepositoryCreationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    internal static LocalNugetRepositoryCreationException LogAndCreate(ILogger logger, string path, IOException error) =>
        LogAndCreate(logger, () => new LocalNugetRepositoryCreationException($"Unable to create local NuGet repository subdirectory {JsonValue.Create(path)?.ToJsonString() ?? path}", path, error));

    protected override void Log(ILogger logger) => logger.LogLocalNugetRepositoryCreationError(Path, InnerException);
}

[Serializable]
public class GetNugetResourceException : LoggedException
{
    public Type Type { get; private set; } = null!;

    public GetNugetResourceException() { }

    private GetNugetResourceException(string message, Type type) : base(message) => Type = type;
    
    protected GetNugetResourceException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    internal static GetNugetResourceException LogAndCreate<T>(ILogger logger) => LogAndCreate(logger, typeof(T));

    internal static GetNugetResourceException LogAndCreate(ILogger logger, Type type) => LogAndCreate(logger, () => new GetNugetResourceException($"Failed to get NuGet resource type {type.FullName}", type));

    protected override void Log(ILogger logger) => logger.LogGetNugetResourceFailure(Type);
}

[Serializable]
public class GetNuGetMetaDataException : LoggedException
{
    public string PackageID { get; private set; } = null!;

    public Uri Url { get; private set; } = null!;
    
    public bool IncludePreRelease { get; private set; }
    
    public bool IncludeUnlisted { get; private set; }

    public GetNuGetMetaDataException() { }

    private GetNuGetMetaDataException(string message, string packageID, Uri url, bool includePreRelease, bool includeUnlisted, Exception inner) : base(message, inner) => (PackageID, Url, IncludePreRelease, IncludeUnlisted) = (packageID, url, includePreRelease, includeUnlisted);
    
    protected GetNuGetMetaDataException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    internal static GetNuGetMetaDataException LogAndCreate(ILogger logger, string packageID, Uri url, bool includePreRelease, bool includeUnlisted, Exception inner) => LogAndCreate(logger, () =>
        new GetNuGetMetaDataException($"Failed to get NuGet MetaData for package with ID {JsonValue.Create(packageID)?.ToJsonString() ?? packageID} from {url}: IncludePreRelease={includePreRelease}; IncludeUnlisted={includeUnlisted}", packageID, url, includePreRelease, includeUnlisted, inner));

    protected override void Log(ILogger logger) => logger.LogGetNuGetMetaDataFailure(PackageID, Url, IncludePreRelease, IncludeUnlisted, InnerException!);
}

[Serializable]
public class GetAllNuGetVersionsException : LoggedException
{
    public string PackageID { get; private set; } = null!;

    public Uri Url { get; private set; } = null!;
    
    public GetAllNuGetVersionsException() { }

    private GetAllNuGetVersionsException(string message, string packageID, Uri url, Exception inner) : base(message, inner) => (PackageID, Url) = (packageID, url);
    
    protected GetAllNuGetVersionsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    internal static GetAllNuGetVersionsException LogAndCreate(ILogger logger, string packageID, Uri url, Exception inner) => LogAndCreate(logger, () =>
        new GetAllNuGetVersionsException($"Failed to get NuGet versoins for package with ID {JsonValue.Create(packageID)?.ToJsonString() ?? packageID} from {url}", packageID, url, inner));

    protected override void Log(ILogger logger) => logger.LogGetAllNuGetVersionsFailure(PackageID, Url, InnerException!);
}

[Serializable]
public class GetNuGetDependencyInfoException : LoggedException
{
    public string PackageID { get; private set; } = null!;

    public NuGetVersion Version { get; private set; } = null!;
    
    public Uri Url { get; private set; } = null!;
    
    public GetNuGetDependencyInfoException() { }

    private GetNuGetDependencyInfoException(string message, string packageID, NuGetVersion version, Uri url, Exception inner) : base(message, inner) => (PackageID, Url, Version) = (packageID, url, version);
    
    protected GetNuGetDependencyInfoException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    internal static GetNuGetDependencyInfoException LogAndCreate(ILogger logger, string packageID, NuGetVersion version, Uri url, Exception inner) => LogAndCreate(logger, () =>
        new GetNuGetDependencyInfoException($"Failed to get NuGet dependency information for package with ID {JsonValue.Create(packageID)?.ToJsonString() ?? packageID} and version {version} from {url}", packageID, version, url, inner));

    protected override void Log(ILogger logger) => logger.LogGetNuGetDependencyInfoFailure(PackageID, Version, Url, InnerException!);
}

[Serializable]
public class DoesNuGetPackageExistException : LoggedException
{
    public string PackageID { get; private set; } = null!;

    public NuGetVersion Version { get; private set; } = null!;
    
    public Uri Url { get; private set; } = null!;
    
    public DoesNuGetPackageExistException() { }

    private DoesNuGetPackageExistException(string message, string packageID, NuGetVersion version, Uri url, Exception inner) : base(message, inner) => (PackageID, Url, Version) = (packageID, url, version);
    
    protected DoesNuGetPackageExistException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    internal static DoesNuGetPackageExistException LogAndCreate(ILogger logger, string packageID, NuGetVersion version, Uri url, Exception inner) => LogAndCreate(logger, () =>
        new DoesNuGetPackageExistException($"Failed to get test whether NuGet package with ID {JsonValue.Create(packageID)?.ToJsonString() ?? packageID} and version {version} exists from {url}", packageID, version, url, inner));

    protected override void Log(ILogger logger) => logger.LogDoesNuGetPackageExistFailure(PackageID, Version, Url, InnerException!);
}
