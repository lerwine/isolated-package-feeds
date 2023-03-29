using Microsoft.Extensions.Logging;

namespace CdnGetter;

internal static class LoggerMessages
{
    public const int EVENT_ID_UnexpectedServiceError = 0x0001;
    public static readonly EventId UnexpectedServiceError = new(EVENT_ID_UnexpectedServiceError, nameof(UnexpectedServiceError));
    private static readonly Action<ILogger, string, Exception?> _unexpectedServiceError = LoggerMessage.Define<string>(LogLevel.Error,
        UnexpectedServiceError, "Error executing {TypeFullName}.");
    public static void LogUnexpectedServiceError(this ILogger logger, Type type, Exception error) => _unexpectedServiceError(logger, type.FullName ?? type.Name, error);
    public static void LogUnexpectedServiceError<T>(this ILogger logger, Exception error) => LogUnexpectedServiceError(logger, typeof(T), error);
    
    public const int EVENT_ID_InvalidCdnContentRoot = 0x0002;
    public static readonly EventId InvalidCdnContentRoot = new(EVENT_ID_InvalidCdnContentRoot, nameof(InvalidCdnContentRoot));
    private static readonly Action<ILogger, string, Exception?> _invalidCdnContentRoot = LoggerMessage.Define<string>(LogLevel.Error,
        InvalidCdnContentRoot, "Invalid path for CDN Content root: {Path}.");
    public static void LogInvalidCdnContentRoot(this ILogger logger, string path, Exception error) => _invalidCdnContentRoot(logger, path, error);
    
    public const int EVENT_ID_CannotCreateCdnContentRoot = 0x0003;
    public static readonly EventId CannotCreateCdnContentRoot = new(EVENT_ID_CannotCreateCdnContentRoot, nameof(CannotCreateCdnContentRoot));
    private static readonly Action<ILogger, string, Exception?> _cannotCreateCdnContentRoot = LoggerMessage.Define<string>(LogLevel.Error,
        CannotCreateCdnContentRoot, "Invalid path for CDN Content root: {Path}.");
    public static void LogCannotCreateCdnContentRoot(this ILogger logger, string path, Exception? error = null) => _cannotCreateCdnContentRoot(logger, path, error);
    
    public const int EVENT_ID_UpstreamCdnNotFound = 0x0004;
    public static readonly EventId UpstreamCdnNotFound = new(EVENT_ID_UpstreamCdnNotFound, nameof(UpstreamCdnNotFound));
    private static readonly Action<ILogger, string, Exception?> _upstreamCdnNotFound = LoggerMessage.Define<string>(LogLevel.Error,
        UpstreamCdnNotFound, "Upstream CDN Service with the name \"{UpstreamCdnName}\" was not found.");
    public static void LogUpstreamCdnNotFound(this ILogger logger, string name, Exception? error = null) => _upstreamCdnNotFound(logger, name, error);
    
    public const int EVENT_ID_UpstreamCdnNotSupported = 0x0005;
    public static readonly EventId UpstreamCdnNotSupported = new(EVENT_ID_UpstreamCdnNotSupported, nameof(UpstreamCdnNotSupported));
    private static readonly Action<ILogger, string, Exception?> _upstreamCdnNotSupported = LoggerMessage.Define<string>(LogLevel.Error,
        UpstreamCdnNotSupported, "Upstream CDN Service with the name \"{UpstreamCdnName}\" is not supported.");
    public static void LogUpstreamCdnNotSupported(this ILogger logger, string name, Exception? error = null) => _upstreamCdnNotSupported(logger, name, error);
    
    public const int EVENT_ID_ServiceTypeNotFound = 0x0006;
    public static readonly EventId ServiceTypeNotFound = new(EVENT_ID_ServiceTypeNotFound, nameof(ServiceTypeNotFound));
    private static readonly Action<ILogger, string, Exception?> _serviceTypeNotFound = LoggerMessage.Define<string>(LogLevel.Error,
        ServiceTypeNotFound, "Could not find service for {TypeFullName}.");
    public static void LogServiceTypeNotFound(this ILogger logger, Type type, Exception? error = null) => _serviceTypeNotFound(logger, type.FullName ?? type.Name, error);
    
    public const int EVENT_ID_NoUpstreamCdnsFound = 0x0007;
    public static readonly EventId NoUpstreamCdnsFound = new(EVENT_ID_NoUpstreamCdnsFound, nameof(NoUpstreamCdnsFound));
    private static readonly Action<ILogger, Exception?> _noUpstreamCdnsFound = LoggerMessage.Define(LogLevel.Warning,
        NoUpstreamCdnsFound, "No CDNs found.");
    public static void LogNoUpstreamCdnsFound(this ILogger logger) => _noUpstreamCdnsFound(logger, null);
    
    public const int EVENT_ID_UpstreamCdnNoActions = 0x0008;
    public static readonly EventId UpstreamCdnNoActions = new(EVENT_ID_UpstreamCdnNoActions, nameof(UpstreamCdnNoActions));
    private static readonly Action<ILogger, string, Exception?> _upstreamCdnNoActions = LoggerMessage.Define<string>(LogLevel.Warning,
        UpstreamCdnNoActions, "Nothing to do with upstream CDN service \"{UpstreamCdnName}\".");
    public static void LogUpstreamCdnNoActions(this ILogger logger, string name) => _upstreamCdnNoActions(logger, name, null);
    
    public const int EVENT_ID_NothingToDo = 0x0009;
    public static readonly EventId NothingToDo = new(EVENT_ID_NothingToDo, nameof(NothingToDo));
    private static readonly Action<ILogger, Exception?> _nothingToDo = LoggerMessage.Define(LogLevel.Warning,
        NothingToDo, "Nothing to do.");
    public static void LogNothingToDo(this ILogger logger) => _nothingToDo(logger, null);
    
    public const int EVENT_ID_InvalidBaseUrl = 0x000a;
    public static readonly EventId InvalidBaseUrl = new(EVENT_ID_InvalidBaseUrl, nameof(InvalidBaseUrl));
    private static readonly Action<ILogger, string, string, Exception?> _invalidBaseUrl = LoggerMessage.Define<string, string>(LogLevel.Error,
        InvalidBaseUrl, "Invalid base URL for {UpstreamCdnType} ({UpstreamCdnName}).");
    public static void LogInvalidBaseUrl(this ILogger logger, Type cdnSvcType, string cdnSvcName, Exception? error = null) => _invalidBaseUrl(logger, cdnSvcType.FullName ?? cdnSvcType.Name, cdnSvcName, error);
    public static void LogInvalidBaseUrl<T>(this ILogger logger, string cdnSvcName, Exception? error = null) => LogInvalidBaseUrl(logger, typeof(T), cdnSvcName, error);
    
    public const int EVENT_ID_LocalLibraryNotFound = 0x000b;
    public static readonly EventId LocalLibraryNotFound = new(EVENT_ID_LocalLibraryNotFound, nameof(LocalLibraryNotFound));
    private static readonly Action<ILogger, string, Exception?> _localLibraryNotFound = LoggerMessage.Define<string>(LogLevel.Warning,
        LocalLibraryNotFound, "Local library named \"{LibraryName}\" not found.");
    public static void LogLocalLibraryNotFound(this ILogger logger, string libraryName) => _localLibraryNotFound(logger, libraryName, null);
    
    public const int EVENT_ID_CdnLibraryNotFound = 0x000c;
    public static readonly EventId CdnLibraryNotFound = new(EVENT_ID_CdnLibraryNotFound, nameof(CdnLibraryNotFound));
    private static readonly Action<ILogger, string, string, Exception?> _cdnLibraryNotFound = LoggerMessage.Define<string, string>(LogLevel.Warning,
        CdnLibraryNotFound, "No local library named \"{LibraryName}\" exists from upstream CDN service \"{UpstreamCdnName}\".");
    public static void LogCdnLibraryNotFound(this ILogger logger, string libraryName, string cdnSvcName) => _cdnLibraryNotFound(logger, libraryName, cdnSvcName, null);
    
    public const int EVENT_ID_LocalLibraryAlreadyExists = 0x000d;
    public static readonly EventId LocalLibraryAlreadyExists = new(EVENT_ID_LocalLibraryAlreadyExists, nameof(LocalLibraryAlreadyExists));
    private static readonly Action<ILogger, string, Exception?> _localLibraryAlreadyExists = LoggerMessage.Define<string>(LogLevel.Warning,
        LocalLibraryAlreadyExists, "Local Library \"{LibraryName}\" has already been retreived.");
    public static void LogLocalLibraryAlreadyExists(this ILogger logger, string libraryName) => _localLibraryAlreadyExists(logger, libraryName, null);
    
    public const int EVENT_ID_CdnLibraryAlreadyExists = 0x000e;
    public static readonly EventId CdnLibraryAlreadyExists = new(EVENT_ID_CdnLibraryAlreadyExists, nameof(CdnLibraryAlreadyExists));
    private static readonly Action<ILogger, string, string, Exception?> _cdnLibraryAlreadyExists = LoggerMessage.Define<string, string>(LogLevel.Warning,
        CdnLibraryAlreadyExists, "Library \"{LibraryName}\" has already been retreived from \"{UpstreamCdnName}\".");
    public static void LogCdnLibraryAlreadyExists(this ILogger logger, string libraryName, string cdnSvcName) => _cdnLibraryAlreadyExists(logger, libraryName, cdnSvcName, null);
    
    public const int EVENT_ID_MutuallyExclusiveSwitchError = 0x000f;
    public static readonly EventId MutuallyExclusiveSwitchError = new(EVENT_ID_MutuallyExclusiveSwitchError, nameof(MutuallyExclusiveSwitchError));
    private static readonly Action<ILogger, string, string, Exception?> _mutuallyExclusiveSwitchError = LoggerMessage.Define<string, string>(LogLevel.Warning,
        MutuallyExclusiveSwitchError, "The --{SwitchName} switch cannot be used with the --{MutuallyExclusive} switch.");
    public static void LogMutuallyExclusiveSwitchError(this ILogger logger, string switchName, string mutuallyExclusiveSwitch) => _mutuallyExclusiveSwitchError(logger, switchName, mutuallyExclusiveSwitch, null);

    public const int EVENT_ID_RequiredDependentParameter1 = 0x0010;
    public static readonly EventId RequiredDependentParameter1 = new(EVENT_ID_RequiredDependentParameter1, nameof(RequiredDependentParameter1));
    private static readonly Action<ILogger, string, string, Exception?> _requiredDependentParameter1 = LoggerMessage.Define<string, string>(LogLevel.Warning,
        RequiredDependentParameter1, "The --{DependentSwitch} switch is required when the --{SwitchName} switch is present.");
    public static void LogRequiredDependentParameter1(this ILogger logger, string dependentSwitch, string switchName) => _requiredDependentParameter1(logger, dependentSwitch, switchName, null);

    public const int EVENT_ID_RequiredAltDependentParameter = 0x0011;
    public static readonly EventId RequiredAltDependentParameter = new(EVENT_ID_RequiredAltDependentParameter, nameof(RequiredAltDependentParameter));
    private static readonly Action<ILogger, string, string, string, Exception?> _requiredAltDependentParameter = LoggerMessage.Define<string, string, string>(LogLevel.Warning,
        RequiredDependentParameter1, "The --{DependentSwitch1} or --{DependentSwitch2} switch is required when the --{DependentSwitch} switch is present.");
    public static void LogRequiredAltDependentParameter(this ILogger logger, string dependentSwitch1, string dependentSwitch2, string switchName) => _requiredAltDependentParameter(logger, dependentSwitch1, dependentSwitch2, switchName, null);

    public const int EVENT_ID_InvalidParameterValue = 0x0012;
    public static readonly EventId InvalidParameterValue = new(EVENT_ID_InvalidParameterValue, nameof(InvalidParameterValue));
    private static readonly Action<ILogger, string, string, Exception?> _invalidParameterValue = LoggerMessage.Define<string, string>(LogLevel.Warning,
        RequiredDependentParameter1, "Invalid value for the --{SwitchName} switch ({Value}).");
    public static void LogInvalidParameterValue(this ILogger logger, string switchName, string value) => _invalidParameterValue(logger, switchName, value, null);
}
