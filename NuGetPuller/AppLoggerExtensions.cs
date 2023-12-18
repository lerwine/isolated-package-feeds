using Microsoft.Extensions.Logging;
using IsolatedPackageFeeds.Shared;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace NuGetPuller;

/// <summary>
/// Extension methods for high performance logging.
/// </summary>
/// <remarks></remarks>
/// <seealso href="https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator"/>
/// <seealso href="https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging"/>
public static partial class AppLoggerExtensions
{
    private const string Message_Upstream_Feed_Path = "Upstream NuGet Feed path";

    private const string Message_Local_Feed_Path = "Local NuGet Feed path";

    private const string Message_Is_Too_Long = "is too long";

    private const string Message_Local_NuGet_Feed_URL = "Local NuGet Feed URL";

    private const string Message_NuGet_Service_Index_URL = "NuGet Service Index URL";

    private const string Message_Is_Invalid = "is invalid";

    private const string Message_Not_Local_Path = "does not reference a local path";

    private const string Message_Is_Not_Absolute = "is not absolute";

    private const string Message_Is_Denied = "is denied";

    private const string Message_Not_Found = "not found";

    private const string Message_Not_Filesystem_Subdirectory = "does not refer to a filesystem subdirectory";

    private const string MESSAGE_Global_Packages_Folder = "Global NuGet Packages Folder";

    #region Nuget Logger Event Methods

    #region NuGetDebug Logger Event Methods

    /// <summary>
    /// Logs a <see cref="AppEventId.NuGetDebug"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    [LoggerMessage(EventId = (int)AppEventId.NuGetDebug, Level = LogLevel.Debug, Message = "NuGet API Debug: {Message})")]
    public static partial void NugetDebugMessage(this ILogger logger, string message);

    [LoggerMessage(EventId = (int)AppEventId.NuGetDebug, Level = LogLevel.Debug, Message = "NuGet API Debug {NugetID} ({Code}): {Message}")]
    private static partial void NugetDebugCode(ILogger logger, string nuGetId, int code, string message);

    /// <summary>
    /// Logs a <see cref="AppEventId.NuGetDebug"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    /// <param name="code">The NuGet log code.</param>
    public static void NugetDebugMessage(this ILogger logger, string message, NuGet.Common.NuGetLogCode code) =>
        NugetDebugCode(logger, code.ToString("F"), (int)code, message);

    /// <summary>
    /// Logs a <see cref="NuGet.Common.LogLevel.Verbose"/> <see cref="AppEventId.NuGetDebug"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    /// <param name="code">The NuGet log code.</param>
    [LoggerMessage(EventId = (int)AppEventId.NuGetDebug, Level = LogLevel.Trace, Message = "NuGet API Verbose: {Message})")]
    public static partial void NugetVerboseMessage(this ILogger logger, string message);

    [LoggerMessage(EventId = (int)AppEventId.NuGetDebug, Level = LogLevel.Trace, Message = "NuGet API Verbose {NugetID} ({Code}): {Message}")]
    private static partial void NugetVerboseCode(ILogger logger, string nuGetId, int code, string message);

    /// <summary>
    /// Logs a <see cref="NuGet.Common.LogLevel.Verbose"/> <see cref="AppEventId.NuGetDebug"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    /// <param name="code">The NuGet log code.</param>
    public static void NugetVerboseMessage(this ILogger logger, string message, NuGet.Common.NuGetLogCode code) =>
        NugetVerboseCode(logger, code.ToString("F"), (int)code, message);

    #endregion

    #region NugetMessage Logger Event Methods

    /// <summary>
    /// Logs a <see cref="AppEventId.NugetMessage"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    [LoggerMessage(EventId = (int)AppEventId.NugetMessage, Level = LogLevel.Information, Message = "NuGet API Information: {Message}")]
    public static partial void NugetInformationMessage(this ILogger logger, string message);

    [LoggerMessage(EventId = (int)AppEventId.NugetMessage, Level = LogLevel.Information, Message = "NuGet API Information {NugetID} ({Code}): {Message}")]
    private static partial void NugetInformationCode(ILogger logger, string nuGetId, int code, string message);

    /// <summary>
    /// Logs a <see cref="AppEventId.NugetMessage"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    /// <param name="code">The NuGet log code.</param>
    public static void NugetInformationMessage(this ILogger logger, string message, NuGet.Common.NuGetLogCode code) =>
        NugetInformationCode(logger, code.ToString("F"), (int)code, message);

    /// <summary>
    /// Logs a <see cref="NuGet.Common.LogLevel.Minimal"/> <see cref="AppEventId.NugetMessage"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    [LoggerMessage(EventId = (int)AppEventId.NugetMessage, Level = LogLevel.Information, Message = "NuGet API Minimal: {Message}")]
    public static partial void NugetMinimalMessage(this ILogger logger, string message);

    [LoggerMessage(EventId = (int)AppEventId.NugetMessage, Level = LogLevel.Information, Message = "NuGet API Minimal {NugetID} ({Code}): {Message}")]
    private static partial void NugetMinimalCode(ILogger logger, string nuGetId, int code, string message);

    /// <summary>
    /// Logs a <see cref="NuGet.Common.LogLevel.Minimal"/> <see cref="AppEventId.NugetMessage"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    /// <param name="code">The NuGet log code.</param>
    public static void NugetMinimalMessage(this ILogger logger, string message, NuGet.Common.NuGetLogCode code) =>
        NugetMinimalCode(logger, code.ToString("F"), (int)code, message);

    #endregion

    #region NugetWarning Logger Event Methods

    /// <summary>
    /// Logs a <see cref="AppEventId.NugetWarning"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet warning message.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.NugetWarning, Level = LogLevel.Warning, Message = "NuGet API Warning: {Message}")]
    public static partial void NugetWarningMessage(this ILogger logger, string message, Exception? exception = null);

    [LoggerMessage(EventId = (int)AppEventId.NugetWarning, Level = LogLevel.Warning, Message = "NuGet API Warning {NugetID} ({Code}): {Message}")]
    private static partial void NugetWarningCode(ILogger logger, string nuGetId, int code, string message, Exception? exception);

    /// <summary>
    /// Logs a <see cref="AppEventId.NugetWarning"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet warning message.</param>
    /// <param name="code">The NuGet log code.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void NugetWarningMessage(this ILogger logger, string message, NuGet.Common.NuGetLogCode code, Exception? exception = null) =>
        NugetWarningCode(logger, code.ToString("F"), (int)code, message, exception);

    #endregion

    #region NuGetError Logger Event Methods

    /// <summary>
    /// Logs a <see cref="AppEventId.NuGetError"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet error message.</param>
    /// <param name="code">The NuGet log code.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.NuGetError, Level = LogLevel.Error, Message = "NuGet API Error: {Message}")]
    public static partial void NuGetErrorMessage(this ILogger logger, string message, Exception? error = null);

    [LoggerMessage(EventId = (int)AppEventId.NuGetError, Level = LogLevel.Error, Message = "NuGet API Error {NugetID} ({Code}): {Message}")]
    private static partial void NuGetErrorCode(ILogger logger, string nuGetId, int code, string message, Exception? error);

    /// <summary>
    /// Logs a <see cref="AppEventId.NuGetError"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet error message.</param>
    /// <param name="code">The NuGet log code.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void NuGetErrorMessage(this ILogger logger, string message, NuGet.Common.NuGetLogCode code, Exception? error = null) =>
        NuGetErrorCode(logger, code.ToString("F"), (int)code, message, error);

    #endregion

    #region CriticalNugetError Logger Event Methods

    /// <summary>
    /// Logs a <see cref="AppEventId.CriticalNugetError"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet error message.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.CriticalNugetError, Level = LogLevel.Critical, Message = "NuGet API Critical: {Message}")]
    public static partial void CriticalNugetErrorMessage(this ILogger logger, string message, Exception? error = null);

    [LoggerMessage(EventId = (int)AppEventId.CriticalNugetError, Level = LogLevel.Critical, Message = "NuGet API Critical {NugetID} ({Code}): {Message}")]
    private static partial void CriticalNugetErrorCode(ILogger logger, string nuGetId, int code, string message, Exception? error);

    /// <summary>
    /// Logs a <see cref="AppEventId.CriticalNugetError"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet error message.</param>
    /// <param name="code">The NuGet log code.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void CriticalNugetErrorMessage(this ILogger logger, string message, NuGet.Common.NuGetLogCode code, Exception? error = null) =>
        CriticalNugetErrorCode(logger, code.ToString("F"), (int)code, message, error);

    #endregion

    #endregion

    #region InvalidRepositoryUrl Logger Event Methods

    [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{Message_Upstream_Feed_Path} \"{{Path}}\" {Message_Is_Too_Long}.")]
    private static partial void UpstreamFeedPathTooLong(ILogger logger, string path, Exception? exception);

    [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{Message_Local_Feed_Path} \"{{Path}}\" {Message_Is_Too_Long}.")]
    private static partial void LocalFeedPathTooLong(ILogger logger, string path, Exception? exception);

    [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{Message_NuGet_Service_Index_URL} \"{{URL}}\" {Message_Is_Invalid}.")]
    private static partial void InvalidNuGetServiceIndexUrl(ILogger logger, string url, Exception? exception);

    [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{Message_Local_NuGet_Feed_URL} \"{{URL}}\" {Message_Is_Invalid}.")]
    private static partial void InvalidLocalNuGetFeedUrl(ILogger logger, string url, Exception? exception);

    [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{Message_Local_NuGet_Feed_URL} \"{{URL}}\" {Message_Not_Local_Path}.")]
    private static partial void LocalNugetFeedUrlIsNotLocal(ILogger logger, string url, Exception? exception);

    [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{Message_NuGet_Service_Index_URL} \"{{URL}}\" {Message_Is_Not_Absolute}.")]
    private static partial void NuGetServiceIndexUrlIsNotAbsolute(ILogger logger, string url, Exception? exception);

    private const string Message_UnsupportedNuGetServiceIndexUrlScheme = "Invalid URI scheme for NuGet Service Index URL";

    [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{Message_UnsupportedNuGetServiceIndexUrlScheme} {{URL}}.")]
    private static partial void UnsupportedNuGetServiceIndexUrlScheme(ILogger logger, string url, Exception? exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidRepositoryUrl"/> event error message for an invalid repository URL or path.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The invalid NuGet repository URL.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory URL.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T InvalidRepositoryUrl<T>(this ILogger logger, string url, bool isUpstream, Func<string, T> factory, Exception? exception = null)
        where T : LoggedException
    {
        if (exception is PathTooLongException)
        {
            if (isUpstream)
            {
                UpstreamFeedPathTooLong(logger, url, exception);
                return factory($"{Message_Upstream_Feed_Path} \"{url}\" {Message_Is_Too_Long}.");
            }
            LocalFeedPathTooLong(logger, url, exception);
            return factory($"{Message_Local_Feed_Path} \"{url}\" {Message_Is_Too_Long}.");
        }
        if (isUpstream)
        {
            InvalidNuGetServiceIndexUrl(logger, url, exception);
            return factory($"{Message_NuGet_Service_Index_URL} \"{url}\" {Message_Is_Invalid}.");
        }
        InvalidLocalNuGetFeedUrl(logger, url, exception);
        return factory($"{Message_Local_NuGet_Feed_URL} \"{url}\" {Message_Is_Invalid}.");
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.UrlSchemeNotSupported"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uriString">The invalid URI.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T UnsupportedNuGetServiceIndexUrlScheme<T>(this ILogger logger, string uriString, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        UnsupportedNuGetServiceIndexUrlScheme(logger, uriString, exception);
        return factory($"{Message_UnsupportedNuGetServiceIndexUrlScheme} {uriString}.");
    }

    #endregion

    #region NugetFeedSecurityException Logger Event Methods

    private const string Message_Access_To_Upstream_NuGet_Feed_Path = "Access to Upstream NuGet Feed path";

    [LoggerMessage(EventId = (int)AppEventId.NugetFeedSecurityException, Level = LogLevel.Critical, Message = $"{Message_Access_To_Upstream_NuGet_Feed_Path} \"{{Path}}\" {Message_Is_Denied}.")]
    private static partial void UpstreamNugetFeedSecurityException(ILogger logger, string path, Exception? exception);

    private const string Message_Access_To_Local_NuGet_Feed_Path = "Access to Local NuGet Feed path";

    [LoggerMessage(EventId = (int)AppEventId.NugetFeedSecurityException, Level = LogLevel.Critical, Message = $"{Message_Access_To_Local_NuGet_Feed_Path} \"{{Path}}\" {Message_Is_Denied}.")]
    private static partial void LocalNugetFeedSecurityException(ILogger logger, string path, Exception? exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.NugetFeedSecurityException"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet feed path.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string NugetFeedSecurityException(this ILogger logger, string path, bool isUpstream, Exception? exception = null)
    {
        if (isUpstream)
        {
            UpstreamNugetFeedSecurityException(logger, path, exception);
            return $"{Message_Access_To_Upstream_NuGet_Feed_Path} \"{path}\" {Message_Is_Denied}.";
        }
        LocalNugetFeedSecurityException(logger, path, exception);
        return $"{Message_Access_To_Local_NuGet_Feed_Path} \"{path}\" {Message_Is_Denied}.";
    }

    #endregion

    #region LocalFeedIOException Logger Event Methods

    private const string Message_LocalFeedIOException = "I/O error while creating Local NuGet Feed folder";

    [LoggerMessage(EventId = (int)AppEventId.LocalFeedIOException, Level = LogLevel.Critical, EventName = nameof(AppEventId.LocalFeedIOException), Message = $"{Message_LocalFeedIOException} \"{{Path}}\".")]
    private static partial void LogLocalFeedIOException(this ILogger logger, string path, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.LocalFeedIOException"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The Local NuGet Feed path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LocalFeedIOException(this ILogger logger, string path, Exception? exception = null)
    {
        LogLocalFeedIOException(logger, path, exception);
        return $"{Message_LocalFeedIOException} \"{path}\".";
    }

    #endregion

    #region NuGetFeedPathNotFound Logger Event Methods


    [LoggerMessage(EventId = (int)AppEventId.NuGetFeedPathNotFound, Level = LogLevel.Critical, Message = $"{Message_Upstream_Feed_Path} \"{{Path}}\" {Message_Not_Found}.")]
    private static partial void UpstreamNuGetFeedPathNotFound(ILogger logger, string path, Exception? exception);


    [LoggerMessage(EventId = (int)AppEventId.NuGetFeedPathNotFound, Level = LogLevel.Critical, Message = $"{Message_Local_Feed_Path} \"{{Path}}\" {Message_Not_Found}.")]
    private static partial void LocalNuGetFeedPathNotFound(ILogger logger, string path, Exception? exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.NuGetFeedPathNotFound"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet feed path.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T NuGetFeedPathNotFound<T>(this ILogger logger, string path, bool isUpstream, Func<string, T> factory, Exception? exception = null)
        where T : LoggedException
    {
        if (isUpstream)
        {
            UpstreamNuGetFeedPathNotFound(logger, path, exception);
            return factory($"{Message_Upstream_Feed_Path} \"{path}\" {Message_Not_Found}.");
        }
        LocalNuGetFeedPathNotFound(logger, path, exception);
        return factory($"{Message_Local_Feed_Path} \"{path}\" {Message_Not_Found}.");
    }

    #endregion

    #region InvalidLocalMetaDataExportPath Logger Event Methods

    private const string MESSAGE_Local_Nuget_Feed_MetaData_Export_Path = "Local NuGet Feed metadata export path";

    private const string MESSAGE_ExportLocalMetaDataDirectoryNotFound = "Parent subdirectory not found for package metadata export path";

    [LoggerMessage(EventId = (int)AppEventId.InvalidLocalMetaDataExportPath, Level = LogLevel.Critical, Message = $"{MESSAGE_ExportLocalMetaDataDirectoryNotFound} \"{{Path}}\".")]
    private static partial void ExportLocalMetaDataDirectoryNotFound(ILogger logger, string path, Exception? exception);


    [LoggerMessage(EventId = (int)AppEventId.InvalidLocalMetaDataExportPath, Level = LogLevel.Critical, Message = $"{MESSAGE_Local_Nuget_Feed_MetaData_Export_Path} \"{{Path}}\" {Message_Is_Too_Long}.")]
    private static partial void ExportLocalMetaDataPathTooLong(ILogger logger, string path, Exception exception);


    [LoggerMessage(EventId = (int)AppEventId.InvalidLocalMetaDataExportPath, Level = LogLevel.Critical, Message = $"{MESSAGE_Local_Nuget_Feed_MetaData_Export_Path} \"{{Path}}\" {Message_Is_Invalid}.")]
    private static partial void InvalidLocalMetaDataExportPath(ILogger logger, string path, Exception? exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidMetaDataExportPath"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The package metadata export path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T InvalidLocalMetaDataExportPath<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        if (exception is not null)
        {
            if (exception is DirectoryNotFoundException)
            {
                ExportLocalMetaDataDirectoryNotFound(logger, path, exception);
                return factory($"{MESSAGE_ExportLocalMetaDataDirectoryNotFound} \"{path}\".");
            }
            if (exception is PathTooLongException)
            {
                ExportLocalMetaDataPathTooLong(logger, path, exception);
                return factory($"{MESSAGE_Local_Nuget_Feed_MetaData_Export_Path} \"{path}\" {Message_Is_Too_Long}.");
            }
        }
        InvalidLocalMetaDataExportPath(logger, path, exception);
        return factory($"{MESSAGE_Local_Nuget_Feed_MetaData_Export_Path} \"{path}\" {Message_Is_Invalid}.");
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidLocalMetaDataExportPath"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet feed path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T ExportLocalMetaDataDirectoryNotFound<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        ExportLocalMetaDataDirectoryNotFound(logger, path, exception);
        return factory(MESSAGE_ExportLocalMetaDataDirectoryNotFound);
    }

    #endregion

    #region MetaDataExportPathAccessDenied Logger Event Methods

    private const string MESSAGE_Access_To_MetaData_Export_Path = "Access denied to NuGet Feed metadata export path";

    [LoggerMessage(EventId = (int)AppEventId.MetaDataExportPathAccessDenied, Level = LogLevel.Critical, EventName = nameof(AppEventId.MetaDataExportPathAccessDenied),
        Message = $"{MESSAGE_Access_To_MetaData_Export_Path} \"{{Path}}\" {Message_Is_Denied}.")]
    private static partial void LogMetaDataExportPathAccessDenied(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_InsufficientPermissionsForMetaDataExportPath = "Caller has insufficient permissions to the Local NuGet Feed metadata export path";

    [LoggerMessage(EventId = (int)AppEventId.MetaDataExportPathAccessDenied, Level = LogLevel.Critical, Message = $"{MESSAGE_InsufficientPermissionsForMetaDataExportPath} \"{{Path}}\".")]
    private static partial void InsufficientPermissionsForMetaDataExportPath(ILogger logger, string path, Exception exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.MetaDataExportPathAccessDenied"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The package metadata export path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T MetaDataExportPathAccessDenied<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        if (exception is System.Security.SecurityException)
        {
            InsufficientPermissionsForMetaDataExportPath(logger, path, exception);
            return factory($"{MESSAGE_InsufficientPermissionsForMetaDataExportPath} \"{path}\".");
        }
        LogMetaDataExportPathAccessDenied(logger, path, exception);
        return factory($"{MESSAGE_Access_To_MetaData_Export_Path} \"{path}\" {Message_Is_Denied}.");
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.MetaDataExportPathAccessDenied"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet feed path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string MetaDataExportPathAccessDenied(this ILogger logger, string path, Exception? exception = null)
    {
        if (exception is System.Security.SecurityException)
        {
            InsufficientPermissionsForMetaDataExportPath(logger, path, exception);
            return $"{MESSAGE_InsufficientPermissionsForMetaDataExportPath} \"{path}\".";
        }
        LogMetaDataExportPathAccessDenied(logger, path, exception);
        return $"{MESSAGE_Access_To_MetaData_Export_Path} \"{path}\" {Message_Is_Denied}.";
    }

    #endregion

    /// <summary>
    /// Logs a <see cref="AppEventId.NuGetPackageDeleted"/> <see cref="LogLevel.Warning"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the package that was deleted.</param>
    /// <param name="feedPath">The path of the Local NuGet Feed.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.NuGetPackageDeleted, Level = LogLevel.Warning,
        Message = "NuGet Package {PackageId} has been deleted from Local NuGet Feed {FeedPath}.")]
    public static partial void NuGetPackageDeleted(this ILogger logger, string packageId, string feedPath, Exception? exception = null);

    #region NuGetPackageNotFound Logger Event Methods

    [LoggerMessage(EventId = (int)AppEventId.NuGetPackageNotFound, Level = LogLevel.Warning, Message = "NuGet Package {PackageId} not found in Upstream NuGet Feed {FeedPath}.")]
    private static partial void NuGetPackageNotFoundInUpstreamFeed(this ILogger logger, string packageId, string feedPath, Exception? exception);

    [LoggerMessage(EventId = (int)AppEventId.NuGetPackageNotFound, Level = LogLevel.Warning, Message = "NuGet Package {PackageId} not found in Local NuGet Feed {FeedPath}.")]
    private static partial void NuGetPackageNotFoundInLocalFeed(this ILogger logger, string packageId, string feedPath, Exception? exception);

    [LoggerMessage(EventId = (int)AppEventId.NuGetPackageNotFound, Level = LogLevel.Warning, Message = "NuGet Package {PackageId} not found in NuGet Server (Service Index URL = {URL}).")]
    private static partial void NuGetPackageNotFoundInRemoteServer(this ILogger logger, string packageId, string url, Exception? exception);

    /// <summary>
    /// Logs a <see cref="AppEventId.NuGetPackageNotFound"/> <see cref="LogLevel.Warning"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the package that was not found.</param>
    /// <param name="clientService">The client service.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void NuGetPackageNotFound(this ILogger logger, string packageId, INuGetClientService clientService, Exception? exception = null)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                NuGetPackageNotFoundInUpstreamFeed(logger, packageId, clientService.PackageSourceLocation, exception);
            else
                NuGetPackageNotFoundInLocalFeed(logger, packageId, clientService.PackageSourceLocation, exception);
        }
        else
            NuGetPackageNotFoundInRemoteServer(logger, packageId, clientService.PackageSourceLocation, exception);
    }


    [LoggerMessage(EventId = (int)AppEventId.NuGetPackageNotFound, Level = LogLevel.Warning,
        Message = "Version {Version} of package {PackageId} not found in NuGet Server (Service Index URL = {URL}).")]
    private static partial void NuGetPackageNotFoundInRemoteServer(ILogger logger, string packageId, NuGetVersion version, string url, Exception? exception);

    [LoggerMessage(EventId = (int)AppEventId.NuGetPackageNotFound, Level = LogLevel.Warning,
        Message = "Version {Version} of package {PackageId} not found in Local NuGet Feed {PackageSourceLocation}.")]
    private static partial void NuGetPackageNotFoundInLocalFeed(ILogger logger, string packageId, NuGetVersion version, string packageSourceLocation, Exception? exception);

    [LoggerMessage(EventId = (int)AppEventId.NuGetPackageNotFound, Level = LogLevel.Warning,
        Message = "Version {Version} of package {PackageId} not found in Upstream NuGet Feed {PackageSourceLocation}.")]
    private static partial void NuGetPackageNotFoundInUpstreamFeed(ILogger logger, NuGetVersion version, string packageId, string packageSourceLocation, Exception? exception);

    /// <summary>
    /// Logs a <see cref="AppEventId.NuGetPackageNotFound"/> <see cref="LogLevel.Warning"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the package that was not found.</param>
    /// <param name="version">The version of the package that was not found.</param>
    /// <param name="clientService">The client service.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void NuGetPackageNotFound(this ILogger logger, string packageId, NuGetVersion version, INuGetClientService clientService, Exception? exception = null)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                NuGetPackageNotFoundInUpstreamFeed(logger, version, packageId, clientService.PackageSourceLocation, exception);
            else
                NuGetPackageNotFoundInLocalFeed(logger, packageId, version, clientService.PackageSourceLocation, exception);
        }
        else
            NuGetPackageNotFoundInRemoteServer(logger, packageId, version, clientService.PackageSourceLocation, exception);
    }

    #endregion

    #region GlobalPackagesFolderNotFound Logger Event Methods

    [LoggerMessage(EventId = (int)AppEventId.GlobalPackagesFolderNotFound, EventName = nameof(AppEventId.GlobalPackagesFolderNotFound), Level = LogLevel.Critical,
        Message = $"{MESSAGE_Global_Packages_Folder} \"{{Path}}\" {Message_Not_Found}.")]
    private static partial void LogGlobalPackagesFolderNotFound(ILogger logger, string path, Exception? exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.GlobalPackagesFolderNotFound"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The Global packages folder path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string GlobalPackagesFolderNotFound(this ILogger logger, string path, Exception? exception = null)
    {
        LogGlobalPackagesFolderNotFound(logger, path, exception);
        return $"{MESSAGE_Global_Packages_Folder} \"{path}\" {Message_Not_Found}.";
    }

    public static T GlobalPackagesFolderNotFound<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        LogGlobalPackagesFolderNotFound(logger, path, exception);
        return factory($"{MESSAGE_Global_Packages_Folder} \"{path}\" {Message_Not_Found}.");
    }

    #endregion

    #region GlobalPackagesFolderSecurityException Logger Event Methods

    private const string Message_Access_To_Global_Packages_Folder = "Access to Global NuGet Packages Folder";

    [LoggerMessage(EventId = (int)AppEventId.GlobalPackagesFolderSecurityException, Level = LogLevel.Critical, EventName = nameof(AppEventId.GlobalPackagesFolderSecurityException),
        Message = $"{Message_Access_To_Global_Packages_Folder} \"{{Path}})\" {Message_Is_Denied}.")]
    private static partial void LogGlobalPackagesFolderSecurityException(ILogger logger, string path, Exception? exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.GlobalPackagesFolderSecurityException"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The Global packages folder path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string GlobalPackagesFolderSecurityException(this ILogger logger, string path, Exception? exception = null)
    {
        LogGlobalPackagesFolderSecurityException(logger, path, exception);
        return $"{Message_Access_To_Global_Packages_Folder} \"{path})\" {Message_Is_Denied}.";
    }

    #endregion

    #region InvalidGlobalPackagesFolder Logger Event Methods

    private const string MESSAGE_Global_Packages_Folder_Path = "Global NuGet Packages Folder path";

    [LoggerMessage(EventId = (int)AppEventId.InvalidGlobalPackagesFolder, Level = LogLevel.Critical,
        Message = $"{MESSAGE_Global_Packages_Folder_Path} \"{{Path}}\" {Message_Is_Too_Long}.")]
    private static partial void GlobalPackagesFolderPathTooLong(ILogger logger, string path, Exception exception);

    [LoggerMessage(EventId = (int)AppEventId.InvalidGlobalPackagesFolder, Level = LogLevel.Critical, EventName = nameof(AppEventId.InvalidGlobalPackagesFolder),
        Message = $"{MESSAGE_Global_Packages_Folder_Path} \"{{Path}}\" {Message_Is_Invalid}.")]
    private static partial void LogInvalidGlobalPackagesFolder(ILogger logger, string path, Exception? exception);

    [LoggerMessage(EventId = (int)AppEventId.InvalidGlobalPackagesFolder, Level = LogLevel.Critical, EventName = nameof(GlobalPackagesFolderNotFileUri),
        Message = $"{MESSAGE_Global_Packages_Folder_Path} \"{{URL}}\" {Message_Not_Filesystem_Subdirectory}.")]
    private static partial void LogGlobalPackagesFolderNotFileUri(ILogger logger, string url, Exception? exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidGlobalPackagesFolder"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The invalid global packages folder path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string InvalidGlobalPackagesFolder(this ILogger logger, string path, Exception? exception = null)
    {
        if (exception is PathTooLongException)
        {
            GlobalPackagesFolderPathTooLong(logger, path, exception);
            return $"{MESSAGE_Global_Packages_Folder_Path} \"{path}\" {Message_Is_Too_Long}.";
        }
        LogInvalidGlobalPackagesFolder(logger, path, exception);
        return $"{MESSAGE_Global_Packages_Folder_Path} \"{path}\" {Message_Is_Invalid}.";
    }

    public static T InvalidGlobalPackagesFolder<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        if (exception is PathTooLongException)
        {
            GlobalPackagesFolderPathTooLong(logger, path, exception);
            return factory($"{MESSAGE_Global_Packages_Folder_Path} \"{path}\" {Message_Is_Too_Long}.");
        }
        LogInvalidGlobalPackagesFolder(logger, path, exception);
        return factory($"{MESSAGE_Global_Packages_Folder_Path} \"{path}\" {Message_Is_Invalid}.");
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidGlobalPackagesFolder"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The invalid global packages folder url.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string GlobalPackagesFolderNotFileUri(this ILogger logger, string url, Exception? exception = null)
    {
        LogGlobalPackagesFolderNotFileUri(logger, url, exception);
        return $"{MESSAGE_Global_Packages_Folder_Path} \"{url}\" {Message_Not_Filesystem_Subdirectory}.";
    }

    #endregion

    #region MultipleSettingsWithSameRepositoryLocation Logger Event Methods

    private const string MESSAGE_Cannot_Be_Same_As = "cannot be the same as the";

    [LoggerMessage(EventId = (int)AppEventId.MultipleSettingsWithSameRepositoryLocation, Level = LogLevel.Critical, EventName = nameof(LocalSameAsUpstreamNugetFeed),
        Message = $"{Message_Local_Feed_Path} \"{{Path}}\" {MESSAGE_Cannot_Be_Same_As} {Message_Upstream_Feed_Path}.")]
    private static partial void LogLocalSameAsUpstreamNugetFeed(ILogger logger, string path, Exception? exception);

    [LoggerMessage(EventId = (int)AppEventId.MultipleSettingsWithSameRepositoryLocation, Level = LogLevel.Critical, EventName = nameof(LocalNuGetFeedSameAsGlobalPackagesFolder),
        Message = $"{Message_Local_Feed_Path} \"{{Path}}\" {MESSAGE_Cannot_Be_Same_As} {MESSAGE_Global_Packages_Folder}.")]
    private static partial void LogLocalNuGetFeedSameAsGlobalPackagesFolder(ILogger logger, string path, Exception? exception);

    [LoggerMessage(EventId = (int)AppEventId.MultipleSettingsWithSameRepositoryLocation, Level = LogLevel.Critical, EventName = nameof(UpstreamNuGetFeedSameAsGlobalPackagesFolder),
        Message = $"{Message_Upstream_Feed_Path} \"{{Path}}\" {MESSAGE_Cannot_Be_Same_As} {MESSAGE_Global_Packages_Folder}.")]
    private static partial void LogUpstreamNuGetFeedSameAsGlobalPackagesFolder(ILogger logger, string path, Exception? exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.MultipleSettingsWithSameRepositoryLocation"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static string LocalSameAsUpstreamNugetFeed(this ILogger logger, string path, Exception? exception = null)
    {
        LogLocalSameAsUpstreamNugetFeed(logger, path, exception);
        return $"{Message_Local_Feed_Path} \"{path}\" {MESSAGE_Cannot_Be_Same_As} {Message_Upstream_Feed_Path}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.MultipleSettingsWithSameRepositoryLocation"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static string LocalNuGetFeedSameAsGlobalPackagesFolder(this ILogger logger, string path, Exception? exception = null)
    {
        LogLocalNuGetFeedSameAsGlobalPackagesFolder(logger, path, exception);
        return $"{Message_Local_Feed_Path} \"{path}\" {MESSAGE_Cannot_Be_Same_As} {MESSAGE_Global_Packages_Folder}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.MultipleSettingsWithSameRepositoryLocation"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static string UpstreamNuGetFeedSameAsGlobalPackagesFolder(this ILogger logger, string path, Exception? exception = null)
    {
        LogUpstreamNuGetFeedSameAsGlobalPackagesFolder(logger, path, exception);
        return $"{Message_Upstream_Feed_Path} \"{path}\" {MESSAGE_Cannot_Be_Same_As} {MESSAGE_Global_Packages_Folder}.";
    }

    #endregion

    /// <summary>
    /// Logs a <see cref="AppEventId.PackageFileNotFound"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="fileName">The path of the file that was not found.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.PackageFileNotFound, Level = LogLevel.Error, Message = "NuGet Package file \"{FileName}\" not found.")]
    public static partial void NupkgFileNotFound(this ILogger logger, string fileName, Exception? exception = null);

    #region InvalidNupkgFile Logger Event Methods

    /// <summary>
    /// Logs a <see cref="AppEventId.InvalidNupkgFile"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="fileName">The path of the invalid package file.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.InvalidNupkgFile, Level = LogLevel.Error, Message = "NuGet Package file \"{FileName}\" is not a ZIP archive.")]
    public static partial void NupkgFileNotZipArchive(this ILogger logger, string fileName, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="AppEventId.InvalidNupkgFile"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="fileName">The path of the invalid package file.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.InvalidNupkgFile, Level = LogLevel.Error, Message = "NuGet Package file \"{FileName}\" has invalid content.")]
    public static partial void InvalidNupkgFileContent(this ILogger logger, string fileName, Exception? exception = null);

    #endregion

    /// <summary>
    /// Logs a <see cref="AppEventId.PackageAlreadyAdded"/> <see cref="LogLevel.Warning"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The identifier of the existing package.</param>
    /// <param name="exception">The optional exception that caused the event.</param>;
    [LoggerMessage(EventId = (int)AppEventId.PackageAlreadyAdded, Level = LogLevel.Warning, Message = "NuGet Package {PackageId} has already been added to the Local NuGet Feed.")]
    public static partial void NuGetPackageAlreadyAdded(this ILogger logger, string packageId, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="AppEventId.PackageVersionDeleteFailure"/> <see cref="LogLevel.Warning"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.PackageVersionDeleteFailure, Level = LogLevel.Warning, Message = "Unexpected error deleting NuGet Package {PackageId}, Version {Version} from the Local NuGet Feed.")]
    public static partial void PackageVersionDeleteFailure(this ILogger logger, string packageId, NuGetVersion version, Exception? exception = null);

    #region UnexpectedPackageDownloadFailure Logger Event Methods

    /// <summary>
    /// Logs a <see cref="AppEventId.UnexpectedPackageDownloadFailure"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="exception">The exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.UnexpectedPackageDownloadFailure, Level = LogLevel.Error, Message = "Unexpected error while downloading package {PackageId}, Version {Version} from the upstream Nuget repository.")]
    public static partial void UnexpectedPackageDownloadFailure(this ILogger logger, string packageId, NuGetVersion version, Exception exception);

    /// <summary>
    /// Logs a <see cref="AppEventId.UnexpectedPackageDownloadFailure"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    [LoggerMessage(EventId = (int)AppEventId.UnexpectedPackageDownloadFailure, Level = LogLevel.Error, Message = "NuGet Package download of {PackageId}, Version {Version} from the upstream Nuget repository was unexpectedly empty.")]
    public static partial void DownloadPackageIsEmpty(this ILogger logger, string packageId, NuGetVersion version);

    #endregion

    /// <summary>
    /// Logs a <see cref="AppEventId.UnexpectedAddFailure"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.UnexpectedAddFailure, Level = LogLevel.Error, Message = "Unexpected error while adding Package {PackageId}, Version {Version} to the Local NuGet Feed.")]
    public static partial void UnexpectedAddNuGetPackageFailure(this ILogger logger, string packageId, NuGetVersion version, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="AppEventId.NoLocalPackagesExist"/> <see cref="LogLevel.Warning"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.NoLocalPackagesExist, Level = LogLevel.Warning, Message = "Local NuGet Feed has no packages.")]
    public static partial void NoLocalNuGetPackagesExist(this ILogger logger, Exception? exception = null);

    public const string Message_Is_Not_A_File = "is not a file";

    #region InvalidExportBundle Logger Event Methods

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidExportBundle"/> error event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The export bundle path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.InvalidExportBundle, Level = LogLevel.Critical,
        Message = "Export Bundle path {Path} is invalid.")]
    public static partial void InvalidExportBundle(this ILogger logger, string path, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidExportBundle"/> error event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The export bundle path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.InvalidExportBundle, Level = LogLevel.Critical,
        Message = "Export Bundle path {Path} does not refer to a file.")]
    public static partial void ExportBundlePathNotAFile(this ILogger logger, string path, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidExportBundle"/> error event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The export bundle path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.InvalidExportBundle, Level = LogLevel.Critical,
        Message = "Parent directory of Export Bundle path {Path} does not exist.")]
    public static partial void ExportBundleDirectoryNotFound(this ILogger logger, string path, Exception? exception = null);

    #endregion

    #region DownloadingNuGetPackage Logger Event Methods

    [LoggerMessage(EventId = (int)AppEventId.DownloadingNuGetPackage, Level = LogLevel.Warning, Message = "Downloading package {PackageId}, version {Version} from local {PackageSourceLocation}.")]
    private static partial void DownloadingLocalNuGetPackageVersion(ILogger logger, string packageId, NuGetVersion version, string packageSourceLocation, Exception? exception);

    [LoggerMessage(EventId = (int)AppEventId.DownloadingNuGetPackage, Level = LogLevel.Warning, Message = "Downloading package {PackageId} from local {PackageSourceLocation}.")]
    private static partial void DownloadingLocalNuGetPackage(ILogger logger, string packageId, string packageSourceLocation, Exception? exception);

    [LoggerMessage(EventId = (int)AppEventId.DownloadingNuGetPackage, Level = LogLevel.Warning, Message = "Downloading package {PackageId}, version {Version} from upstream {PackageSourceUri}.")]
    private static partial void DownloadingRemoteNuGetPackageVersion(ILogger logger, string packageId, NuGetVersion version, Uri packageSourceUri, Exception? exception);

    [LoggerMessage(EventId = (int)AppEventId.DownloadingNuGetPackage, Level = LogLevel.Warning, EventName = nameof(AppEventId.DownloadingNuGetPackage), Message = "Downloading package {PackageId} from upstream {PackageSourceUri}.")]
    private static partial void LogDownloadingRemoteNuGetPackage(ILogger logger, string packageId, Uri packageSourceUri, Exception? exception);

    /// <summary>
    /// Logs a <see cref="AppEventId.DownloadingNuGetPackage"/> <see cref="LogLevel.Information"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="identity">The package identifier and version.</param>
    /// <param name="clientService">The source repository service.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void DownloadingNuGetPackage(this ILogger logger, PackageIdentity identity, INuGetClientService clientService, Exception? exception = null)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (identity.HasVersion)
                DownloadingLocalNuGetPackageVersion(logger, identity.Id, identity.Version, clientService.PackageSourceLocation, exception);
            else
                DownloadingLocalNuGetPackage(logger, identity.Id, clientService.PackageSourceLocation, exception);
        }
        else if (identity.HasVersion)
            DownloadingRemoteNuGetPackageVersion(logger, identity.Id, identity.Version, clientService.PackageSourceUri, exception);
        else
            LogDownloadingRemoteNuGetPackage(logger, identity.Id, clientService.PackageSourceUri, exception);
    }

    #endregion

    #region InvalidCreateFromPath Logger Event Methods

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidCreateFromPath"/> error event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The Create-From path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.InvalidCreateFromPath, Level = LogLevel.Critical,
        Message = "Create-from path {Path} is invalid.")]
    public static partial void InvalidCreateFromPath(this ILogger logger, string path, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidCreateFromPath"/> error event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The Create-From path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.InvalidCreateFromPath, Level = LogLevel.Critical,
        Message = "Create-from path {Path} does not exist.")]
    public static partial void CreateFromFileNotFound(this ILogger logger, string path, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidCreateFromPath"/> error event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The Create-From path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.InvalidCreateFromPath, Level = LogLevel.Critical,
        Message = "Create-from path {Path} is not a file.")]
    public static partial void CreateFromNotAFile(this ILogger logger, string path, Exception? exception = null);

    #endregion

    #region IgnoredDependentCommandLineArgument Logger Event Methods

    /// <summary>
    /// Logs a <see cref="AppEventId.IgnoredDependentCommandLineArgument"/> <see cref="LogLevel.Warning"/> message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dependentSwitch">The dependent command line switch.</param>
    /// <param name="switchName">The primary command line switch.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.IgnoredDependentCommandLineArgument, Level = LogLevel.Warning,
        Message = "Command line switch {DependentSwitch} is ignnored if {SwitchName} is not specified.")]
    [Obsolete("Use NuGetPuller.CLI.MainService.CheckIgnoredDependentCommandLineArgument")]
    public static partial void IgnoredDependentCommandLineArgument(this ILogger logger, string dependentSwitch, string switchName, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="AppEventId.IgnoredDependentCommandLineArgument"/> <see cref="LogLevel.Warning"/> message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dependentSwitch">The dependent command line switch.</param>
    /// <param name="switchName1">The first primary command line switch.</param>
    /// <param name="switchName2">The second primary command line switch.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.IgnoredDependentCommandLineArgument, Level = LogLevel.Warning,
        Message = "Command line switch {DependentSwitch} is ignnored if {SwitchName1} or {SwitchName2} is not specified.")]
    [Obsolete("Use NuGetPuller.CLI.MainService.CheckIgnoredDependentCommandLineArgument")]
    public static partial void IgnoredDependentCommandLineArgument(this ILogger logger, string dependentSwitch, string switchName1, string switchName2, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="AppEventId.IgnoredDependentCommandLineArgument"/> <see cref="LogLevel.Warning"/> message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dependentSwitch">The dependent command line switch.</param>
    /// <param name="switchName1">The first primary command line switch.</param>
    /// <param name="switchName2">The second primary command line switch.</param>
    /// <param name="switchName3">The third primary command line switch.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.IgnoredDependentCommandLineArgument, Level = LogLevel.Warning,
        Message = "Command line switch {DependentSwitch} is ignnored if {SwitchName1}, {SwitchName2} or {SwitchName3} is not specified.")]
    [Obsolete("Use NuGetPuller.CLI.MainService.CheckIgnoredDependentCommandLineArgument")]
    public static partial void IgnoredDependentCommandLineArgument(this ILogger logger, string dependentSwitch, string switchName1, string switchName2, string switchName3, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="AppEventId.IgnoredDependentCommandLineArgument"/> <see cref="LogLevel.Warning"/> message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dependentSwitch">The dependent command line switch.</param>
    /// <param name="switchName1">The first primary command line switch.</param>
    /// <param name="switchName2">The second primary command line switch.</param>
    /// <param name="switchName3">The third primary command line switch.</param>
    /// <param name="switchName4">The fourth primary command line switch.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.IgnoredDependentCommandLineArgument, Level = LogLevel.Warning,
        Message = "Command line switch {DependentSwitch} is ignnored if {SwitchName1}, {SwitchName2}, {SwitchName3} or {SwitchName4} is not specified.")]
    [Obsolete("Use NuGetPuller.CLI.MainService.CheckIgnoredDependentCommandLineArgument")]
    public static partial void IgnoredDependentCommandLineArgument(this ILogger logger, string dependentSwitch, string switchName1, string switchName2, string switchName3, string switchName4, Exception? exception = null);

    #endregion

    #region InvalidSaveManifestToPath Logger Event Methods

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidSaveManifestToPath"/> error event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The filesystem path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.InvalidSaveManifestToPath, Level = LogLevel.Critical,
        Message = "Save-to path {Path} is invalid")]
    public static partial void InvalidSaveManifestToPath(this ILogger logger, string path, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidSaveManifestToPath"/> error event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The filesystem path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.InvalidSaveManifestToPath, Level = LogLevel.Critical,
        Message = "Save-to path {Path} is invalid")]
    public static partial void SaveManifestToFileNotFound(this ILogger logger, string path, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidSaveManifestToPath"/> error event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The filesystem path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.InvalidSaveManifestToPath, Level = LogLevel.Critical,
        Message = "Save-to path {Path} is invalid")]
    public static partial void SaveManifestToPathNotAFile(this ILogger logger, string path, Exception? exception = null);

    #endregion

    #region GetDownloadResource Scope

    private static readonly Func<ILogger, string, NuGetVersion?, string, IDisposable?> _getNugetServerDownloadResourceResultScope = LoggerMessage.DefineScope<string, NuGetVersion?, string>(
        "Getting Download Resource for package {PackageId}, version {Version} from upstream NuGet Server at {Url}."
    );

    private static readonly Func<ILogger, string, NuGetVersion?, string, IDisposable?> _getUpstreamFeedDownloadResourceResultScope = LoggerMessage.DefineScope<string, NuGetVersion?, string>(
        "Getting Download Resource for package {PackageId}, version {Version} from Upstream NuGet Feed at {Path}."
    );

    private static readonly Func<ILogger, string, NuGetVersion?, string, IDisposable?> _getLocalFeedDownloadResourceResultScope = LoggerMessage.DefineScope<string, NuGetVersion?, string>(
        "Getting Download Resource for package {PackageId}, version {Version} from Local NuGet Feed at {Path}."
    );

    /// <summary>
    /// Formats the GetDownloadResource message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetDownloadResourceResultScope(this ILogger logger, PackageIdentity identity, INuGetClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _getUpstreamFeedDownloadResourceResultScope(logger, identity.Id, identity.HasVersion ? identity.Version : null, clientService.PackageSourceLocation);
            return _getLocalFeedDownloadResourceResultScope(logger, identity.Id, identity.HasVersion ? identity.Version : null, clientService.PackageSourceLocation);
        }
        return _getNugetServerDownloadResourceResultScope(logger, identity.Id, identity.HasVersion ? identity.Version : null, clientService.PackageSourceLocation);
    }

    #endregion

    #region DownloadNupkg Scope

    private static readonly Func<ILogger, string, NuGetVersion?, string?, IDisposable?> _downloadRemoteNupkgScope = LoggerMessage.DefineScope<string, NuGetVersion?, string?>(
       "Download NuGet package {PackageId}, version {Version} from server {Url}).");

    private static readonly Func<ILogger, string, NuGetVersion?, string, IDisposable?> _downloadUpstreamFeedNupkgScope = LoggerMessage.DefineScope<string, NuGetVersion?, string>(
       "Download NuGet package {PackageId}, version {Version} from Upstream NuGet Feed {Path}.");

    private static readonly Func<ILogger, string, NuGetVersion?, string, IDisposable?> _downloadLocalFeedNupkgScope = LoggerMessage.DefineScope<string, NuGetVersion?, string>(
       "Download NuGet package {PackageId}, version {Version} from Local NuGet Feed {Path}.");

    /// <summary>
    /// Formats the DownloadNupkg message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginDownloadNupkgScope(this ILogger logger, string packageId, NuGetVersion version, INuGetClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _downloadUpstreamFeedNupkgScope(logger, packageId, version, clientService.PackageSourceLocation);
            return _downloadLocalFeedNupkgScope(logger, packageId, version, clientService.PackageSourceLocation);
        }
        return _downloadRemoteNupkgScope(logger, packageId, version, clientService.PackageSourceLocation);
    }

    #endregion

    #region GetMetadata Scope

    private static readonly Func<ILogger, string, bool, bool, string, IDisposable?> _getNugetServerMetadataScope1 = LoggerMessage.DefineScope<string, bool, bool, string>(
       "Get metadata for package {PackageId} (IncludePreRelease={IncludePreRelease}; IncludeUnlisted={IncludeUnlisted}) from NuGet Server {Url}."
    );

    private static readonly Func<ILogger, string, bool, bool, string, IDisposable?> _getLocalFeedMetadataScope1 = LoggerMessage.DefineScope<string, bool, bool, string>(
       "Get metadata for package {PackageId} (IncludePreRelease={IncludePreRelease}; IncludeUnlisted={IncludeUnlisted}) from Local NuGet Feed {Path}."
    );

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _getNugetServerMetadataScope2 = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get metadata for package {PackageId}, version {Version} from NuGet Server {Url}."
    );

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _getLocalFeedMetadataScope2 = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get metadata for package {PackageId}, version {Version} from Local NuGet Feed {Path}."
    );

    private static readonly Func<ILogger, string, bool, bool, string, IDisposable?> _getUpstreamFeedMetadataScope1 = LoggerMessage.DefineScope<string, bool, bool, string>(
       "Get metadata for package {PackageId} (IncludePreRelease={IncludePreRelease}; IncludeUnlisted={IncludeUnlisted}) from Upstream NuGet Feed {Path}."
    );

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _getUpstreamFeedMetadataScope2 = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get metadata for package {PackageId}, version {Version} from Upstream NuGet Feed {Path}."
    );

    /// <summary>
    /// Formats the GetMetadata message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package ID.</param>
    /// <param name="includePreRelease">Whether to include pre-release packages.</param>
    /// <param name="includeUnlisted">Whether to include unlisted packages.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetMetadataScope(this ILogger logger, string packageId, bool includePreRelease, bool includeUnlisted, INuGetClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _getUpstreamFeedMetadataScope1(logger, packageId, includePreRelease, includeUnlisted, clientService.PackageSourceLocation);
            return _getLocalFeedMetadataScope1(logger, packageId, includePreRelease, includeUnlisted, clientService.PackageSourceLocation);
        }
        return _getNugetServerMetadataScope1(logger, packageId, includePreRelease, includeUnlisted, clientService.PackageSourceLocation);
    }

    public static IDisposable? BeginGetMetadataScope(this ILogger logger, string packageId, NuGetVersion version, INuGetClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _getUpstreamFeedMetadataScope2(logger, packageId, version, clientService.PackageSourceLocation);
            return _getLocalFeedMetadataScope2(logger, packageId, version, clientService.PackageSourceLocation);
        }
        return _getNugetServerMetadataScope2(logger, packageId, version, clientService.PackageSourceLocation);
    }

    #endregion

    #region GetAllVersions Scope

    private static readonly Func<ILogger, string, string, IDisposable?> _getAllNugetServerVersionsScope = LoggerMessage.DefineScope<string, string>(
       "Get all versions for package {PackageId} from NuGet Server {Url}."
    );

    private static readonly Func<ILogger, string, string, IDisposable?> _getAllUpstreamFeedVersionsScope = LoggerMessage.DefineScope<string, string>(
       "Get all versions for package {PackageId} from Upstream NuGet Feed {Path}."
    );

    private static readonly Func<ILogger, string, string, IDisposable?> _getAllLocalFeedVersionsScope = LoggerMessage.DefineScope<string, string>(
       "Get all versions for package {PackageId} from Local NuGet Feed {Path}."
    );

    /// <summary>
    /// Formats the GetAllVersions message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetAllVersionsScope(this ILogger logger, string packageId, INuGetClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _getAllUpstreamFeedVersionsScope(logger, packageId, clientService.PackageSourceLocation);
            return _getAllLocalFeedVersionsScope(logger, packageId, clientService.PackageSourceLocation);
        }
        return _getAllNugetServerVersionsScope(logger, packageId, clientService.PackageSourceLocation);
    }

    #endregion

    #region ResolvePackage Scope

    private static readonly Func<ILogger, string, NuGetVersion, NuGetFramework, string, IDisposable?> _resolveNugetServerPackageScope = LoggerMessage.DefineScope<string, NuGetVersion, NuGetFramework, string>(
       "Getting dependencies for package {PackageId}, version {Version}, framework {Framework} from NuGet Server {URL}."
    );

    private static readonly Func<ILogger, string, NuGetVersion, NuGetFramework, string, IDisposable?> _resolveUpstreamFeedPackageScope = LoggerMessage.DefineScope<string, NuGetVersion, NuGetFramework, string>(
       "Getting dependencies for package {PackageId}, version {Version}, framework {Framework} from Upstream NuGet Feed {Path}."
    );

    private static readonly Func<ILogger, string, NuGetVersion, NuGetFramework, string, IDisposable?> _resolveLocalFeedPackageScope = LoggerMessage.DefineScope<string, NuGetVersion, NuGetFramework, string>(
       "Getting dependencies for package {PackageId}, version {Version}, framework {Framework} from Local NuGet Feed {Path}."
    );

    /// <summary>
    /// Formats the ResolvePackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="framework">The package target framework.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginResolvePackageScope(this ILogger logger, string packageId, NuGetVersion version, NuGetFramework framework, INuGetClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _resolveUpstreamFeedPackageScope(logger, packageId, version, framework, clientService.PackageSourceLocation);
            return _resolveLocalFeedPackageScope(logger, packageId, version, framework, clientService.PackageSourceLocation);
        }
        return _resolveNugetServerPackageScope(logger, packageId, version, framework, clientService.PackageSourceLocation);
    }

    #endregion

    #region ResolvePackages Scope

    private static readonly Func<ILogger, string, string, IDisposable?> _resolveNugetServerPackagesScope1 = LoggerMessage.DefineScope<string, string>(
       "Getting dependencies for package {PackageId} from NuGet Server {URL}."
    );

    private static readonly Func<ILogger, string, string, IDisposable?> _resolveUpstreamFeedPackagesScope1 = LoggerMessage.DefineScope<string, string>(
       "Getting dependencies for package {PackageId} from Upstream NuGet Feed {Path}."
    );

    private static readonly Func<ILogger, string, string, IDisposable?> _resolveLocalFeedPackagesScope1 = LoggerMessage.DefineScope<string, string>(
       "Getting dependencies for package {PackageId} from Local NuGet Feed {Path}."
    );

    private static readonly Func<ILogger, string, NuGetFramework, string, IDisposable?> _resolveNugetServerPackagesScope2 = LoggerMessage.DefineScope<string, NuGetFramework, string>(
       "Getting dependencies for package {PackageId}, framework {Framework} from NuGet Server {URL}."
    );

    private static readonly Func<ILogger, string, NuGetFramework, string, IDisposable?> _resolveUpstreamFeedPackagesScope2 = LoggerMessage.DefineScope<string, NuGetFramework, string>(
       "Getting dependencies for package {PackageId}, framework {Framework} from Upstream NuGet Feed {Path})."
    );

    private static readonly Func<ILogger, string, NuGetFramework, string, IDisposable?> _resolveLocalFeedPackagesScope2 = LoggerMessage.DefineScope<string, NuGetFramework, string>(
       "Getting dependencies for package {PackageId}, framework {Framework} from Local NuGet Feed {Path})."
    );

    /// <summary>
    /// Formats the ResolvePackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package ID.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginResolvePackagesScope(this ILogger logger, string packageId, INuGetClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _resolveUpstreamFeedPackagesScope1(logger, packageId, clientService.PackageSourceLocation);
            return _resolveLocalFeedPackagesScope1(logger, packageId, clientService.PackageSourceLocation);
        }
        return _resolveNugetServerPackagesScope1(logger, packageId, clientService.PackageSourceLocation);
    }

    /// <summary>
    /// Formats the ResolvePackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package ID.</param>
    /// <param name="framework">The package target framework.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginResolvePackagesScope(this ILogger logger, string packageId, NuGetFramework framework, INuGetClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _resolveUpstreamFeedPackagesScope2(logger, packageId, framework, clientService.PackageSourceLocation);
            return _resolveLocalFeedPackagesScope2(logger, packageId, framework, clientService.PackageSourceLocation);
        }
        return _resolveNugetServerPackagesScope2(logger, packageId, framework, clientService.PackageSourceLocation);
    }

    #endregion

    #region GetDependencyInfo Scope

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _getNugetServerDependencyInfoScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get dependency information for package {PackageId}, version {Version} from NuGet Server {URL}."
    );

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _getUpstreamFeedDependencyInfoScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get dependency information for package {PackageId}, version {Version} from Upstream NuGet Feed {Path})."
    );

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _getLocalFeedDependencyInfoScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get dependency information for package {PackageId}, version {Version} from Local NuGet Feed {Path})."
    );

    /// <summary>
    /// Formats the GetDependencyInfo message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetDependencyInfoScope(this ILogger logger, string packageId, NuGetVersion version, INuGetClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _getUpstreamFeedDependencyInfoScope(logger, packageId, version, clientService.PackageSourceLocation);
            return _getLocalFeedDependencyInfoScope(logger, packageId, version, clientService.PackageSourceLocation);
        }
        return _getNugetServerDependencyInfoScope(logger, packageId, version, clientService.PackageSourceLocation);
    }

    #endregion

    #region DoesPackageExist Scope

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _doesNugetServerPackageExistScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get dependency information for package {PackageId}, version {Version} from NuGet Server {URL}."
    );

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _doesUpstreamFeedPackageExistScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get dependency information for package {PackageId}, version {Version} from Upstream NuGet Feed {Path}."
    );

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _doesLocalFeedPackageExistScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get dependency information for package {PackageId}, version {Version} from Local NuGet Feed {Path}."
    );

    /// <summary>
    /// Formats the DoesPackageExist message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginDoesPackageExistScope(this ILogger logger, string packageId, NuGetVersion version, INuGetClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _doesUpstreamFeedPackageExistScope(logger, packageId, version, clientService.PackageSourceLocation);
            return _doesLocalFeedPackageExistScope(logger, packageId, version, clientService.PackageSourceLocation);
        }
        return _doesNugetServerPackageExistScope(logger, packageId, version, clientService.PackageSourceLocation);
    }

    #endregion

    #region GetPackageDependencies Scope

    private static readonly Func<ILogger, string, NuGetVersion, NuGetFramework, string, IDisposable?> _getNugetServerPackageDependenciesScope = LoggerMessage.DefineScope<string, NuGetVersion, NuGetFramework, string>(
       "Get dependencies for package {PackageId}, version {Version}, framework {Framework} from NuGet Server {URL}."
    );

    private static readonly Func<ILogger, string, NuGetVersion, NuGetFramework, string, IDisposable?> _getUpstreamFeedPackageDependenciesScope = LoggerMessage.DefineScope<string, NuGetVersion, NuGetFramework, string>(
       "Get dependencies for package {PackageId}, version {Version}, framework {Framework} from Upstream NuGet Feed {Path})."
    );

    private static readonly Func<ILogger, string, NuGetVersion, NuGetFramework, string, IDisposable?> _getLocalFeedPackageDependenciesScope = LoggerMessage.DefineScope<string, NuGetVersion, NuGetFramework, string>(
       "Get dependencies for package {PackageId}, version {Version}, framework {Framework} from Local NuGet Feed {Path})."
    );

    private static readonly Func<ILogger, string, string, IDisposable?> _getAllNugetServerPackageDependenciesScope = LoggerMessage.DefineScope<string, string>(
       "Get dependencies for package {PackageId} from NuGet Server {URL}."
    );

    private static readonly Func<ILogger, string, string, IDisposable?> _getAllUpstreamFeedPackageDependenciesScope = LoggerMessage.DefineScope<string, string>(
       "Get dependencies for package {PackageId} from Upstream NuGet Feed {Path})."
    );

    private static readonly Func<ILogger, string, string, IDisposable?> _getAllLocalFeedPackageDependenciesScope = LoggerMessage.DefineScope<string, string>(
       "Get dependencies for package {PackageId} from Local NuGet Feed {Path})."
    );

    /// <summary>
    /// Formats the GetPackageDependencies message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="framework">The package framework.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetPackageDependenciesScope(this ILogger logger, string packageId, NuGetVersion version, NuGetFramework framework, INuGetClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _getUpstreamFeedPackageDependenciesScope(logger, packageId, version, framework, clientService.PackageSourceLocation);
            return _getLocalFeedPackageDependenciesScope(logger, packageId, version, framework, clientService.PackageSourceLocation);
        }
        return _getNugetServerPackageDependenciesScope(logger, packageId, version, framework, clientService.PackageSourceLocation);
    }

    /// <summary>
    /// Formats the GetPackageDependencies message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetPackageDependenciesScope(this ILogger logger, string packageId, INuGetClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _getAllUpstreamFeedPackageDependenciesScope(logger, packageId, clientService.PackageSourceLocation);
            return _getAllLocalFeedPackageDependenciesScope(logger, packageId, clientService.PackageSourceLocation);
        }
        return _getAllNugetServerPackageDependenciesScope(logger, packageId, clientService.PackageSourceLocation);
    }

    #endregion

    #region NugetSource Scope

    private static readonly Func<ILogger, string, bool, IDisposable?> _nugetSourceScope = LoggerMessage.DefineScope<string, bool>(
       "Using NuGet source {URL} (IsUpstream={IsUpstream})."
    );

    /// <summary>
    /// Formats the NugetSource message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The url of the NuGet respository.</param>
    /// <param name="isUpstream"><see langword="true"/>  if the repository is upstream; otherwise, <see langword="false"/>.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginNugetSourceScope(this ILogger logger, string url, bool isUpstream) => _nugetSourceScope(logger, url, isUpstream);

    #endregion

    #region DeleteLocalPackage Scope

    private static readonly Func<ILogger, string, string, IDisposable?> _deleteLocalPackageScope = LoggerMessage.DefineScope<string, string>(
        "Delete package {PackageId} from Local NuGet Feed {Path}."
    );

    /// <summary>
    /// Formats the DeleteLocalPackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the local package to delete.</param>
    /// <param name="path">The Local NuGet Feed URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginDeleteLocalPackageScope(this ILogger logger, string packageId, string path) => _deleteLocalPackageScope(logger, packageId, path);

    #endregion

    #region DeleteLocalPackageVersion Scope

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _deleteLocalPackageVersionScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
        "Delete local package {PackageId}, version {Version} from Local NuGet Feed {Path}."
    );

    /// <summary>
    /// Formats the DeleteLocalPackageVersion message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the local package to delete.</param>
    /// <param name="version">The package version.</param>
    /// <param name="path">The Local NuGet Feed URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginDeleteLocalPackageVersionScope(this ILogger logger, string packageId, NuGetVersion version, string path) => _deleteLocalPackageVersionScope(logger, packageId, version, path);

    #endregion

    #region AddLocalPackage Scope

    private static readonly Func<ILogger, string, string, IDisposable?> _addLocalPackageScope = LoggerMessage.DefineScope<string, string>(
        "Add package {PackageId} to Local NuGet Feed {Path}."
    );

    /// <summary>
    /// Formats the AddLocalPackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the upstream package to add locally.</param>
    /// <param name="path">The Local NuGet Feed path.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginAddLocalPackageScope(this ILogger logger, string packageId, string path) => _addLocalPackageScope(logger, packageId, path);

    #endregion

    #region UpdateLocalPackage Scope

    private static readonly Func<ILogger, string, string, IDisposable?> _updateLocalPackageScope = LoggerMessage.DefineScope<string, string>(
        "Update package {PackageId} in Local NuGet Feed {Path}."
    );

    /// <summary>
    /// Formats the UpdateLocalPackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the upstream package to update locally.</param>
    /// <param name="path">The Local NuGet Feed URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginUpdateLocalPackageScope(this ILogger logger, string packageId, string path) => _updateLocalPackageScope(logger, packageId, path);

    #endregion

    #region GetAllLocalPackages Scope

    private static readonly Func<ILogger, string, IDisposable?> _getAllLocalPackagesScope = LoggerMessage.DefineScope<string>(
        "Getting all packages from {Path}."
    );

    /// <summary>
    /// Formats the GetAllLocalPackages message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The Local Nuget Feed path.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetAllLocalPackagesScope(this ILogger logger, string path) => _getAllLocalPackagesScope(logger, path);

    #endregion
}