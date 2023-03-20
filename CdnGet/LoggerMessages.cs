using Microsoft.Extensions.Logging;

namespace CdnGet;

internal static class LoggerMessages
{
    public static EventId UnexpectedServiceError = new EventId(0x0001, nameof(UnexpectedServiceError));
    private static readonly Action<ILogger, string, Exception?> _unexpectedServiceError = LoggerMessage.Define<string>(Microsoft.Extensions.Logging.LogLevel.Error,
        UnexpectedServiceError, "Error executing {TypeFullName}.");
    public static void LogUnexpectedServiceError(this ILogger logger, Type type, Exception error) => _unexpectedServiceError(logger, type.FullName ?? type.Name, error);
    public static void LogUnexpectedServiceError<T>(this ILogger logger, Exception error) => LogUnexpectedServiceError(logger, typeof(T), error);
    
    public static EventId RemoteServiceNotFound = new EventId(0x0002, nameof(RemoteServiceNotFound));
    private static readonly Action<ILogger, string, Exception?> _remoteServiceNotFound = LoggerMessage.Define<string>(Microsoft.Extensions.Logging.LogLevel.Error,
        RemoteServiceNotFound, "Remote service with the name \"{RemoteServiceName}\" was not found.");
    public static void LogRemoteServiceNotFound(this ILogger logger, string name, Exception? error = null) => _remoteServiceNotFound(logger, name, error);
    
    public static EventId RemoteServiceNotSupported = new EventId(0x0003, nameof(RemoteServiceNotSupported));
    private static readonly Action<ILogger, string, Exception?> _remoteServiceNotSupported = LoggerMessage.Define<string>(Microsoft.Extensions.Logging.LogLevel.Error,
        RemoteServiceNotSupported, "Remote service with the name \"{RemoteServiceName}\" is not supported.");
    public static void LogRemoteServiceNotSupported(this ILogger logger, string name, Exception? error = null) => _remoteServiceNotSupported(logger, name, error);
    
    public static EventId ServiceTypeNotFound = new EventId(0x0004, nameof(ServiceTypeNotFound));
    private static readonly Action<ILogger, string, Exception?> _serviceTypeNotFound = LoggerMessage.Define<string>(Microsoft.Extensions.Logging.LogLevel.Error,
        ServiceTypeNotFound, "Could not find service for {TypeFullName}.");
    public static void LogServiceTypeNotFound(this ILogger logger, Type type, Exception? error = null) => _serviceTypeNotFound(logger, type.FullName ?? type.Name, error);
    
    public static EventId NoRemotesFound = new EventId(0x0005, nameof(NoRemotesFound));
    private static readonly Action<ILogger, Exception?> _noRemotesFound = LoggerMessage.Define(Microsoft.Extensions.Logging.LogLevel.Warning,
        NoRemotesFound, "No remotes found.");
    public static void LogNoRemotesFound(this ILogger logger) => _noRemotesFound(logger, null);
    
    public static EventId RemoteServiceNoActions = new EventId(0x0006, nameof(RemoteServiceNoActions));
    private static readonly Action<ILogger, string, Exception?> _remoteServiceNoActions = LoggerMessage.Define<string>(Microsoft.Extensions.Logging.LogLevel.Warning,
        RemoteServiceNoActions, "Nothing to do with remote service \"{RemoteServiceName}\".");
    public static void LogRemoteServiceNoActions(this ILogger logger, string name) => _remoteServiceNoActions(logger, name, null);
    
    public static EventId NothingToDo = new EventId(0x0007, nameof(NothingToDo));
    private static readonly Action<ILogger, Exception?> _nothingToDo = LoggerMessage.Define(Microsoft.Extensions.Logging.LogLevel.Warning,
        NothingToDo, "Nothing to do.");
    public static void LogNothingToDo(this ILogger logger) => _nothingToDo(logger, null);
    
    public static EventId InvalidBaseUrl = new EventId(0x0008, nameof(InvalidBaseUrl));
    private static readonly Action<ILogger, string, string, Exception?> _invalidBaseUrl = LoggerMessage.Define<string, string>(Microsoft.Extensions.Logging.LogLevel.Error,
        InvalidBaseUrl, "Invalid base URL for {RemoteServiceType} ({RemoteServiceName}).");
    public static void LogInvalidBaseUrl(this ILogger logger, Type remoteSvcType, string remoteSvcName, Exception? error = null) => _invalidBaseUrl(logger, remoteSvcType.FullName ?? remoteSvcType.Name, remoteSvcName, error);
    public static void LogInvalidBaseUrl<T>(this ILogger logger, string remoteSvcName, Exception? error = null) => LogInvalidBaseUrl(logger, typeof(T), remoteSvcName, error);
    
    public static EventId LocalLibraryNotFound = new EventId(0x0009, nameof(LocalLibraryNotFound));
    private static readonly Action<ILogger, string, string, Exception?> _localLibraryNotFound = LoggerMessage.Define<string, string>(Microsoft.Extensions.Logging.LogLevel.Warning,
        LocalLibraryNotFound, "No local library named \"{LibraryName}\" exists from remote service \"{RemoteServiceName}\".");
    public static void LogLocalLibraryNotFound(this ILogger logger, string libraryName, string remoteSvcName) => _invalidBaseUrl(logger, libraryName, remoteSvcName, null);
    
    public static EventId LibraryAlreadyExistsLocal = new EventId(0x000a, nameof(LibraryAlreadyExistsLocal));
    private static readonly Action<ILogger, string, string, Exception?> _libraryAlreadyExistsLocal = LoggerMessage.Define<string, string>(Microsoft.Extensions.Logging.LogLevel.Warning,
        LibraryAlreadyExistsLocal, "Library \"{LibraryName}\" has already been retreived from \"{RemoteServiceName}\".");
    public static void LogLibraryAlreadyExistsLocal(this ILogger logger, string libraryName, string remoteSvcName) => _libraryAlreadyExistsLocal(logger, libraryName, remoteSvcName, null);
}
