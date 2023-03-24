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
    
    public const int EVENT_ID_RemoteServiceNotFound = 0x0002;
    public static readonly EventId RemoteServiceNotFound = new(EVENT_ID_RemoteServiceNotFound, nameof(RemoteServiceNotFound));
    private static readonly Action<ILogger, string, Exception?> _remoteServiceNotFound = LoggerMessage.Define<string>(LogLevel.Error,
        RemoteServiceNotFound, "Remote service with the name \"{RemoteServiceName}\" was not found.");
    public static void LogRemoteServiceNotFound(this ILogger logger, string name, Exception? error = null) => _remoteServiceNotFound(logger, name, error);
    
    public const int EVENT_ID_RemoteServiceNotSupported = 0x0003;
    public static readonly EventId RemoteServiceNotSupported = new(EVENT_ID_RemoteServiceNotSupported, nameof(RemoteServiceNotSupported));
    private static readonly Action<ILogger, string, Exception?> _remoteServiceNotSupported = LoggerMessage.Define<string>(LogLevel.Error,
        RemoteServiceNotSupported, "Remote service with the name \"{RemoteServiceName}\" is not supported.");
    public static void LogRemoteServiceNotSupported(this ILogger logger, string name, Exception? error = null) => _remoteServiceNotSupported(logger, name, error);
    
    public const int EVENT_ID_ServiceTypeNotFound = 0x0004;
    public static readonly EventId ServiceTypeNotFound = new(EVENT_ID_ServiceTypeNotFound, nameof(ServiceTypeNotFound));
    private static readonly Action<ILogger, string, Exception?> _serviceTypeNotFound = LoggerMessage.Define<string>(LogLevel.Error,
        ServiceTypeNotFound, "Could not find service for {TypeFullName}.");
    public static void LogServiceTypeNotFound(this ILogger logger, Type type, Exception? error = null) => _serviceTypeNotFound(logger, type.FullName ?? type.Name, error);
    
    public const int EVENT_ID_NoRemotesFound = 0x0005;
    public static readonly EventId NoRemotesFound = new(EVENT_ID_NoRemotesFound, nameof(NoRemotesFound));
    private static readonly Action<ILogger, Exception?> _noRemotesFound = LoggerMessage.Define(LogLevel.Warning,
        NoRemotesFound, "No remotes found.");
    public static void LogNoRemotesFound(this ILogger logger) => _noRemotesFound(logger, null);
    
    public const int EVENT_ID_RemoteServiceNoActions = 0x0006;
    public static readonly EventId RemoteServiceNoActions = new(EVENT_ID_RemoteServiceNoActions, nameof(RemoteServiceNoActions));
    private static readonly Action<ILogger, string, Exception?> _remoteServiceNoActions = LoggerMessage.Define<string>(LogLevel.Warning,
        RemoteServiceNoActions, "Nothing to do with remote service \"{RemoteServiceName}\".");
    public static void LogRemoteServiceNoActions(this ILogger logger, string name) => _remoteServiceNoActions(logger, name, null);
    
    public const int EVENT_ID_NothingToDo = 0x0007;
    public static readonly EventId NothingToDo = new(EVENT_ID_NothingToDo, nameof(NothingToDo));
    private static readonly Action<ILogger, Exception?> _nothingToDo = LoggerMessage.Define(LogLevel.Warning,
        NothingToDo, "Nothing to do.");
    public static void LogNothingToDo(this ILogger logger) => _nothingToDo(logger, null);
    
    public const int EVENT_ID_InvalidBaseUrl = 0x0008;
    public static readonly EventId InvalidBaseUrl = new(EVENT_ID_InvalidBaseUrl, nameof(InvalidBaseUrl));
    private static readonly Action<ILogger, string, string, Exception?> _invalidBaseUrl = LoggerMessage.Define<string, string>(LogLevel.Error,
        InvalidBaseUrl, "Invalid base URL for {RemoteServiceType} ({RemoteServiceName}).");
    public static void LogInvalidBaseUrl(this ILogger logger, Type remoteSvcType, string remoteSvcName, Exception? error = null) => _invalidBaseUrl(logger, remoteSvcType.FullName ?? remoteSvcType.Name, remoteSvcName, error);
    public static void LogInvalidBaseUrl<T>(this ILogger logger, string remoteSvcName, Exception? error = null) => LogInvalidBaseUrl(logger, typeof(T), remoteSvcName, error);
    
    public const int EVENT_ID_LocalLibraryNotFound = 0x0009;
    public static readonly EventId LocalLibraryNotFound = new(EVENT_ID_LocalLibraryNotFound, nameof(LocalLibraryNotFound));
    private static readonly Action<ILogger, string, Exception?> _localLibraryNotFound = LoggerMessage.Define<string>(LogLevel.Warning,
        LocalLibraryNotFound, "Local library named \"{LibraryName}\" not found.");
    public static void LogLocalLibraryNotFound(this ILogger logger, string libraryName) => _localLibraryNotFound(logger, libraryName, null);
    
    public const int EVENT_ID_RemoteLibraryNotFound = 0x000a;
    public static readonly EventId RemoteLibraryNotFound = new(EVENT_ID_RemoteLibraryNotFound, nameof(RemoteLibraryNotFound));
    private static readonly Action<ILogger, string, string, Exception?> _remoteLibraryNotFound = LoggerMessage.Define<string, string>(LogLevel.Warning,
        RemoteLibraryNotFound, "No local library named \"{LibraryName}\" exists from remote service \"{RemoteServiceName}\".");
    public static void LogRemoteLibraryNotFound(this ILogger logger, string libraryName, string remoteSvcName) => _remoteLibraryNotFound(logger, libraryName, remoteSvcName, null);
    
    public const int EVENT_ID_LocalLibraryAlreadyExists = 0x000b;
    public static readonly EventId LocalLibraryAlreadyExists = new(EVENT_ID_LocalLibraryAlreadyExists, nameof(LocalLibraryAlreadyExists));
    private static readonly Action<ILogger, string, Exception?> _localLibraryAlreadyExists = LoggerMessage.Define<string>(LogLevel.Warning,
        LocalLibraryAlreadyExists, "Local Library \"{LibraryName}\" has already been retreived.");
    public static void LogLocalLibraryAlreadyExists(this ILogger logger, string libraryName) => _localLibraryAlreadyExists(logger, libraryName, null);
    
    public const int EVENT_ID_RemoteLibraryAlreadyExists = 0x000c;
    public static readonly EventId RemoteLibraryAlreadyExists = new(EVENT_ID_RemoteLibraryAlreadyExists, nameof(RemoteLibraryAlreadyExists));
    private static readonly Action<ILogger, string, string, Exception?> _remoteLibraryAlreadyExists = LoggerMessage.Define<string, string>(LogLevel.Warning,
        RemoteLibraryAlreadyExists, "Library \"{LibraryName}\" has already been retreived from \"{RemoteServiceName}\".");
    public static void LogRemoteLibraryAlreadyExists(this ILogger logger, string libraryName, string remoteSvcName) => _remoteLibraryAlreadyExists(logger, libraryName, remoteSvcName, null);
    
    public const int EVENT_ID_MutuallyExclusiveSwitchError = 0x000d;
    public static readonly EventId MutuallyExclusiveSwitchError = new(EVENT_ID_MutuallyExclusiveSwitchError, nameof(MutuallyExclusiveSwitchError));
    private static readonly Action<ILogger, string, string, Exception?> _mutuallyExclusiveSwitchError = LoggerMessage.Define<string, string>(LogLevel.Warning,
        MutuallyExclusiveSwitchError, "The --{SwitchName} switch cannot be used with the --{MutuallyExclusive} switch.");
    public static void LogMutuallyExclusiveSwitchError(this ILogger logger, string switchName, string mutuallyExclusiveSwitch) => _mutuallyExclusiveSwitchError(logger, switchName, mutuallyExclusiveSwitch, null);

    public const int EVENT_ID_RequiredDependentParameter1 = 0x000e;
    public static readonly EventId RequiredDependentParameter1 = new(EVENT_ID_RequiredDependentParameter1, nameof(RequiredDependentParameter1));
    private static readonly Action<ILogger, string, string, Exception?> _requiredDependentParameter1 = LoggerMessage.Define<string, string>(LogLevel.Warning,
        RequiredDependentParameter1, "The --{DependentSwitch} switch is required when the --{SwitchName} switch is present.");
    public static void LogRequiredDependentParameter1(this ILogger logger, string dependentSwitch, string switchName) => _requiredDependentParameter1(logger, dependentSwitch, switchName, null);

    public const int EVENT_ID_RequiredAltDependentParameter = 0x000f;
    public static readonly EventId RequiredAltDependentParameter = new(EVENT_ID_RequiredAltDependentParameter, nameof(RequiredAltDependentParameter));
    private static readonly Action<ILogger, string, string, string, Exception?> _requiredAltDependentParameter = LoggerMessage.Define<string, string, string>(LogLevel.Warning,
        RequiredDependentParameter1, "The --{DependentSwitch1} or --{DependentSwitch2} switch is required when the --{DependentSwitch} switch is present.");
    public static void LogRequiredAltDependentParameter(this ILogger logger, string dependentSwitch1, string dependentSwitch2, string switchName) => _requiredAltDependentParameter(logger, dependentSwitch1, dependentSwitch2, switchName, null);


    public const int EVENT_ID_InvalidParameterValue = 0x0010;
    public static readonly EventId InvalidParameterValue = new(EVENT_ID_InvalidParameterValue, nameof(InvalidParameterValue));
    private static readonly Action<ILogger, string, string, Exception?> _invalidParameterValue = LoggerMessage.Define<string, string>(LogLevel.Warning,
        RequiredDependentParameter1, "Invalid value for the --{SwitchName} switch ({Value}).");
    public static void LogInvalidParameterValue(this ILogger logger, string switchName, string value) => _invalidParameterValue(logger, switchName, value, null);
}
