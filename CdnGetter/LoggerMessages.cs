using Microsoft.Extensions.Logging;

namespace CdnGetter;

internal static class LoggerMessages
{
    public const int EVENT_ID_UnexpectedServiceError = 0x0001;
    public static EventId UnexpectedServiceError = new EventId(EVENT_ID_UnexpectedServiceError, nameof(UnexpectedServiceError));
    private static readonly Action<ILogger, string, Exception?> _unexpectedServiceError = LoggerMessage.Define<string>(LogLevel.Error,
        UnexpectedServiceError, "Error executing {TypeFullName}.");
    public static void LogUnexpectedServiceError(this ILogger logger, Type type, Exception error) => _unexpectedServiceError(logger, type.FullName ?? type.Name, error);
    public static void LogUnexpectedServiceError<T>(this ILogger logger, Exception error) => LogUnexpectedServiceError(logger, typeof(T), error);
    
    public const int EVENT_ID_RemoteServiceNotFound = 0x0002;
    public static EventId RemoteServiceNotFound = new EventId(EVENT_ID_RemoteServiceNotFound, nameof(RemoteServiceNotFound));
    private static readonly Action<ILogger, string, Exception?> _remoteServiceNotFound = LoggerMessage.Define<string>(LogLevel.Error,
        RemoteServiceNotFound, "Remote service with the name \"{RemoteServiceName}\" was not found.");
    public static void LogRemoteServiceNotFound(this ILogger logger, string name, Exception? error = null) => _remoteServiceNotFound(logger, name, error);
    
    public const int EVENT_ID_RemoteServiceNotSupported = 0x0003;
    public static EventId RemoteServiceNotSupported = new EventId(EVENT_ID_RemoteServiceNotSupported, nameof(RemoteServiceNotSupported));
    private static readonly Action<ILogger, string, Exception?> _remoteServiceNotSupported = LoggerMessage.Define<string>(LogLevel.Error,
        RemoteServiceNotSupported, "Remote service with the name \"{RemoteServiceName}\" is not supported.");
    public static void LogRemoteServiceNotSupported(this ILogger logger, string name, Exception? error = null) => _remoteServiceNotSupported(logger, name, error);
    
    public const int EVENT_ID_ServiceTypeNotFound = 0x0004;
    public static EventId ServiceTypeNotFound = new EventId(EVENT_ID_ServiceTypeNotFound, nameof(ServiceTypeNotFound));
    private static readonly Action<ILogger, string, Exception?> _serviceTypeNotFound = LoggerMessage.Define<string>(LogLevel.Error,
        ServiceTypeNotFound, "Could not find service for {TypeFullName}.");
    public static void LogServiceTypeNotFound(this ILogger logger, Type type, Exception? error = null) => _serviceTypeNotFound(logger, type.FullName ?? type.Name, error);
    
    public const int EVENT_ID_NoRemotesFound = 0x0005;
    public static EventId NoRemotesFound = new EventId(EVENT_ID_NoRemotesFound, nameof(NoRemotesFound));
    private static readonly Action<ILogger, Exception?> _noRemotesFound = LoggerMessage.Define(LogLevel.Warning,
        NoRemotesFound, "No remotes found.");
    public static void LogNoRemotesFound(this ILogger logger) => _noRemotesFound(logger, null);
    
    public const int EVENT_ID_RemoteServiceNoActions = 0x0006;
    public static EventId RemoteServiceNoActions = new EventId(EVENT_ID_RemoteServiceNoActions, nameof(RemoteServiceNoActions));
    private static readonly Action<ILogger, string, Exception?> _remoteServiceNoActions = LoggerMessage.Define<string>(LogLevel.Warning,
        RemoteServiceNoActions, "Nothing to do with remote service \"{RemoteServiceName}\".");
    public static void LogRemoteServiceNoActions(this ILogger logger, string name) => _remoteServiceNoActions(logger, name, null);
    
    public const int EVENT_ID_NothingToDo = 0x0007;
    public static EventId NothingToDo = new EventId(EVENT_ID_NothingToDo, nameof(NothingToDo));
    private static readonly Action<ILogger, Exception?> _nothingToDo = LoggerMessage.Define(LogLevel.Warning,
        NothingToDo, "Nothing to do.");
    public static void LogNothingToDo(this ILogger logger) => _nothingToDo(logger, null);
    
    public const int EVENT_ID_InvalidBaseUrl = 0x0008;
    public static EventId InvalidBaseUrl = new EventId(EVENT_ID_InvalidBaseUrl, nameof(InvalidBaseUrl));
    private static readonly Action<ILogger, string, string, Exception?> _invalidBaseUrl = LoggerMessage.Define<string, string>(LogLevel.Error,
        InvalidBaseUrl, "Invalid base URL for {RemoteServiceType} ({RemoteServiceName}).");
    public static void LogInvalidBaseUrl(this ILogger logger, Type remoteSvcType, string remoteSvcName, Exception? error = null) => _invalidBaseUrl(logger, remoteSvcType.FullName ?? remoteSvcType.Name, remoteSvcName, error);
    public static void LogInvalidBaseUrl<T>(this ILogger logger, string remoteSvcName, Exception? error = null) => LogInvalidBaseUrl(logger, typeof(T), remoteSvcName, error);
    
    public const int EVENT_ID_LocalLibraryNotFound = 0x0009;
    public static EventId LocalLibraryNotFound = new EventId(EVENT_ID_LocalLibraryNotFound, nameof(LocalLibraryNotFound));
    private static readonly Action<ILogger, string, Exception?> _localLibraryNotFound = LoggerMessage.Define<string>(LogLevel.Warning,
        LocalLibraryNotFound, "Local library named \"{LibraryName}\" not found.");
    public static void LogLocalLibraryNotFound(this ILogger logger, string libraryName) => _localLibraryNotFound(logger, libraryName, null);
    
    public const int EVENT_ID_RemoteLibraryNotFound = 0x000a;
    public static EventId RemoteLibraryNotFound = new EventId(EVENT_ID_RemoteLibraryNotFound, nameof(RemoteLibraryNotFound));
    private static readonly Action<ILogger, string, string, Exception?> _remoteLibraryNotFound = LoggerMessage.Define<string, string>(LogLevel.Warning,
        RemoteLibraryNotFound, "No local library named \"{LibraryName}\" exists from remote service \"{RemoteServiceName}\".");
    public static void LogRemoteLibraryNotFound(this ILogger logger, string libraryName, string remoteSvcName) => _remoteLibraryNotFound(logger, libraryName, remoteSvcName, null);
    
    public const int EVENT_ID_LocalLibraryAlreadyExists = 0x000b;
    public static EventId LocalLibraryAlreadyExists = new EventId(EVENT_ID_LocalLibraryAlreadyExists, nameof(LocalLibraryAlreadyExists));
    private static readonly Action<ILogger, string, Exception?> _localLibraryAlreadyExists = LoggerMessage.Define<string>(LogLevel.Warning,
        LocalLibraryAlreadyExists, "Local Library \"{LibraryName}\" has already been retreived.");
    public static void LogLocalLibraryAlreadyExists(this ILogger logger, string libraryName) => _localLibraryAlreadyExists(logger, libraryName, null);
    
    public const int EVENT_ID_RemoteLibraryAlreadyExists = 0x000c;
    public static EventId RemoteLibraryAlreadyExists = new EventId(EVENT_ID_RemoteLibraryAlreadyExists, nameof(RemoteLibraryAlreadyExists));
    private static readonly Action<ILogger, string, string, Exception?> _remoteLibraryAlreadyExists = LoggerMessage.Define<string, string>(LogLevel.Warning,
        RemoteLibraryAlreadyExists, "Library \"{LibraryName}\" has already been retreived from \"{RemoteServiceName}\".");
    public static void LogRemoteLibraryAlreadyExists(this ILogger logger, string libraryName, string remoteSvcName) => _remoteLibraryAlreadyExists(logger, libraryName, remoteSvcName, null);
}
