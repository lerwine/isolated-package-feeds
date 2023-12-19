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

    private const string Message_Downloaded_Packages_Folder_Path = "Downloaded NuGet Packages Folder path";

    private const string Message_Is_Too_Long = "is too long";

    private const string Message_Downloaded_Packages_Folder_URL = "Downloaded NuGet Packages Folder URL";

    private const string Message_NuGet_Service_Index_URL = "NuGet Service Index URL";

    private const string Message_Is_Invalid = "is invalid";

    private const string Message_Not_Local_Path = "does not reference a local path";

    private const string Message_Is_Not_Absolute = "is not absolute";

    private const string Message_Is_Denied = "is denied";

    private const string Message_Not_Found = "not found";

    private const string Message_Not_Filesystem_Subdirectory = "does not refer to a filesystem subdirectory";

    private const string MESSAGE_Global_Packages_Folder = "Global NuGet Packages Folder";

    private const string Message_Not_File = "does not refer to a file";
    
    private const string Message_Does_Not_Exist = "does not exist";
    
    #region Nuget Logger Event Methods

    #region NuGetDebug Logger Event Methods

    /// <summary>
    /// Logs a <see cref="AppEventId.NuGetDebug"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    [LoggerMessage(EventId = (int)AppEventId.NuGetDebug, Level = LogLevel.Debug, Message = "NuGet API Debug: {Message})")]
    public static partial void NugetDebugMessage(this ILogger logger, string message);

    [LoggerMessage(EventId = (int)AppEventId.NuGetDebug, Level = LogLevel.Debug, Message = "NuGet API Debug {NuGetID} ({Code}): {Message}")]
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

    [LoggerMessage(EventId = (int)AppEventId.NuGetDebug, Level = LogLevel.Trace, Message = "NuGet API Verbose {NuGetID} ({Code}): {Message}")]
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

    [LoggerMessage(EventId = (int)AppEventId.NugetMessage, Level = LogLevel.Information, Message = "NuGet API Information {NuGetID} ({Code}): {Message}")]
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

    [LoggerMessage(EventId = (int)AppEventId.NugetMessage, Level = LogLevel.Information, Message = "NuGet API Minimal {NuGetID} ({Code}): {Message}")]
    private static partial void NugetMinimalCode(ILogger logger, string nuGetId, int code, string message);

    /// <summary>
    /// Logs a <see cref="NuGet.Common.LogLevel.Minimal"/> <see cref="AppEventId.NugetMessage"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    /// <param name="code">The NuGet log code.</param>
    public static void NugetMinimalMessage(this ILogger logger, string message, NuGet.Common.NuGetLogCode code) => NugetMinimalCode(logger, code.ToString("F"), (int)code, message);

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

    [LoggerMessage(EventId = (int)AppEventId.NugetWarning, Level = LogLevel.Warning, Message = "NuGet API Warning {NuGetID} ({Code}): {Message}")]
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

    [LoggerMessage(EventId = (int)AppEventId.NuGetError, Level = LogLevel.Error, Message = "NuGet API Error {NuGetID} ({Code}): {Message}")]
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

    [LoggerMessage(EventId = (int)AppEventId.CriticalNugetError, Level = LogLevel.Critical, Message = "NuGet API Critical {NuGetID} ({Code}): {Message}")]
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

    public static readonly EventId EventId_InvalidRepositoryUrl = new((int)AppEventId.InvalidRepositoryUrl, nameof(InvalidRepositoryUrl));

    private static readonly Action<ILogger, string, Exception?> UpstreamFeedPathTooLong = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_InvalidRepositoryUrl, $"{Message_Upstream_Feed_Path} \"{{Path}}\" {Message_Is_Too_Long}.");

    // [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = "Upstream NuGet Feed path \"{Path}\" is too long.")]
    // [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{Message_Upstream_Feed_Path} \"{{Path}}\" {Message_Is_Too_Long}.")]
    // private static partial void UpstreamFeedPathTooLong(ILogger logger, string path, Exception? exception);

    private static readonly Action<ILogger, string, Exception?> DownloadedPackagesFolderTooLong = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_InvalidRepositoryUrl, $"{Message_Downloaded_Packages_Folder_Path} \"{{Path}}\" {Message_Is_Too_Long}.");

    // [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = "Downloaded NuGet Packages Folder path \"{Path}\" is too long.")]
    // [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{Message_Downloaded_Packages_Folder_Path} \"{{Path}}\" {Message_Is_Too_Long}.")]
    // private static partial void DownloadedPackagesFolderTooLong(ILogger logger, string path, Exception? exception);

    private static readonly Action<ILogger, string, Exception?> InvalidNuGetServiceIndexUrl = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_InvalidRepositoryUrl, $"{Message_NuGet_Service_Index_URL} \"{{URL}}\" {Message_Is_Invalid}.");

    // [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = "NuGet Service Index URL \"{Url}\" is invalid.")]
    // [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{Message_NuGet_Service_Index_URL} \"{{URL}}\" {Message_Is_Invalid}.")]
    // private static partial void InvalidNuGetServiceIndexUrl(ILogger logger, string url, Exception? exception);

    private static readonly Action<ILogger, string, Exception?> InvalidDownloadedPackagesFolderUrl = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_InvalidRepositoryUrl, $"{Message_Downloaded_Packages_Folder_URL} \"{{URL}}\" {Message_Is_Invalid}.");

    // [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = "Downloaded NuGet Packages Folder URL \"{Url}\" is invalid.")]
    // [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{Message_Downloaded_Packages_Folder_URL} \"{{URL}}\" {Message_Is_Invalid}.")]
    // private static partial void InvalidDownloadedPackagesFolderUrl(ILogger logger, string url, Exception? exception);

    private static readonly Action<ILogger, string, Exception?> DownloadedPackagesFolderUrlIsNotLocal = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_InvalidRepositoryUrl, $"{Message_Downloaded_Packages_Folder_URL} \"{{URL}}\" {Message_Not_Local_Path}.");

    // [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = "Downloaded NuGet Packages Folder URL \"{Url}\" does not reference a local path.")]
    // [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{Message_Downloaded_Packages_Folder_URL} \"{{URL}}\" {Message_Not_Local_Path}.")]
    // private static partial void DownloadedPackagesFolderUrlIsNotLocal(ILogger logger, string url, Exception? exception);

    private static readonly Action<ILogger, string, Exception?> NuGetServiceIndexUrlIsNotAbsolute = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_InvalidRepositoryUrl, $"{Message_NuGet_Service_Index_URL} \"{{URL}}\" {Message_Is_Not_Absolute}.");

    // [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = "NuGet Service Index URL \"{Url}\" is not absolute.")]
    // [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{Message_NuGet_Service_Index_URL} \"{{URL}}\" {Message_Is_Not_Absolute}.")]
    // private static partial void NuGetServiceIndexUrlIsNotAbsolute(ILogger logger, string url, Exception? exception);

    private const string Message_UnsupportedNuGetServiceIndexUrlScheme = "Invalid URI scheme for NuGet Service Index URL";

    private static readonly Action<ILogger, string, Exception?> LogUnsupportedNuGetServiceIndexUrlScheme = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_InvalidRepositoryUrl, $"{Message_UnsupportedNuGetServiceIndexUrlScheme} {{URL}}.");

    // [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = "Invalid URI scheme for NuGet Service Index URL {Url}.")]
    // [LoggerMessage(EventId = (int)AppEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{Message_UnsupportedNuGetServiceIndexUrlScheme} {{URL}}.")]
    // private static partial void LogUnsupportedNuGetServiceIndexUrlScheme(ILogger logger, string url, Exception? exception);

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
            DownloadedPackagesFolderTooLong(logger, url, exception);
            return factory($"{Message_Downloaded_Packages_Folder_Path} \"{url}\" {Message_Is_Too_Long}.");
        }
        if (isUpstream)
        {
            InvalidNuGetServiceIndexUrl(logger, url, exception);
            return factory($"{Message_NuGet_Service_Index_URL} \"{url}\" {Message_Is_Invalid}.");
        }
        InvalidDownloadedPackagesFolderUrl(logger, url, exception);
        return factory($"{Message_Downloaded_Packages_Folder_URL} \"{url}\" {Message_Is_Invalid}.");
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
        LogUnsupportedNuGetServiceIndexUrlScheme(logger, uriString, exception);
        return factory($"{Message_UnsupportedNuGetServiceIndexUrlScheme} {uriString}.");
    }

    #endregion

    #region NugetFeedSecurityException Logger Event Methods

    public static readonly EventId EventId_NugetFeedSecurityException = new((int)AppEventId.NugetFeedSecurityException, nameof(NugetFeedSecurityException));

    private const string Message_Access_To_Upstream_NuGet_Feed_Path = "Access to Upstream NuGet Feed path";

    private static readonly Action<ILogger, string, Exception?> UpstreamNugetFeedSecurityException = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_NugetFeedSecurityException, $"{Message_Access_To_Upstream_NuGet_Feed_Path} \"{{Path}}\" {Message_Is_Denied}.");

    // [LoggerMessage(EventId = (int)AppEventId.NugetFeedSecurityException, Level = LogLevel.Critical, Message = $"{Message_Access_To_Upstream_NuGet_Feed_Path} \"{{Path}}\" {Message_Is_Denied}.")]
    // private static partial void UpstreamNugetFeedSecurityException(ILogger logger, string path, Exception? exception);

    private static readonly Action<ILogger, string, Exception?> DownloadedPackagesFolderSecurityException = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_NugetFeedSecurityException, $"{Message_Access_To_Downloaded_Packages_Folder_Path} \"{{Path}}\" {Message_Is_Denied}.");

    private const string Message_Access_To_Downloaded_Packages_Folder_Path = "Access to Downloaded NuGet Packages Folder path";

    // [LoggerMessage(EventId = (int)AppEventId.NugetFeedSecurityException, Level = LogLevel.Critical, Message = $"{Message_Access_To_Downloaded_Packages_Folder_Path} \"{{Path}}\" {Message_Is_Denied}.")]
    // private static partial void DownloadedPackagesFolderSecurityException(ILogger logger, string path, Exception? exception);

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
        DownloadedPackagesFolderSecurityException(logger, path, exception);
        return $"{Message_Access_To_Downloaded_Packages_Folder_Path} \"{path}\" {Message_Is_Denied}.";
    }

    #endregion

    #region DownloadedPackagesFolderIOException Logger Event Methods

    public static readonly EventId EventId_DownloadedPackagesFolderIOException = new((int)AppEventId.DownloadedPackagesFolderIOException, nameof(DownloadedPackagesFolderIOException));

    private const string Message_DownloadedPackagesFolderIOException = "I/O error while creating Downloaded NuGet Packages Folder";

    private static readonly Action<ILogger, string, Exception?> LogDownloadedPackagesFolderIOException = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_DownloadedPackagesFolderIOException, $"{Message_DownloadedPackagesFolderIOException} \"{{Path}}\".");

    // [LoggerMessage(EventId = (int)AppEventId.DownloadedPackagesFolderIOException, Level = LogLevel.Critical, EventName = nameof(AppEventId.DownloadedPackagesFolderIOException),
    //     Message = $"{Message_DownloadedPackagesFolderIOException} \"{{Path}}\".")]
    // private static partial void LogDownloadedPackagesFolderIOException(this ILogger logger, string path, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.DownloadedPackagesFolderIOException"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The Downloaded NuGet Packages Folder path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string DownloadedPackagesFolderIOException(this ILogger logger, string path, Exception? exception = null)
    {
        LogDownloadedPackagesFolderIOException(logger, path, exception);
        return $"{Message_DownloadedPackagesFolderIOException} \"{path}\".";
    }

    #endregion

    #region NuGetFeedPathNotFound Logger Event Methods

    public static readonly EventId EventId_NuGetFeedPathNotFound = new((int)AppEventId.NuGetFeedPathNotFound, nameof(NuGetFeedPathNotFound));

    private static readonly Action<ILogger, string, Exception?> UpstreamNuGetFeedPathNotFound = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_NuGetFeedPathNotFound, $"{Message_Upstream_Feed_Path} \"{{Path}}\" {Message_Not_Found}.");

    // [LoggerMessage(EventId = (int)AppEventId.NuGetFeedPathNotFound, Level = LogLevel.Critical, Message = $"{Message_Upstream_Feed_Path} \"{{Path}}\" {Message_Not_Found}.")]
    // private static partial void UpstreamNuGetFeedPathNotFound(ILogger logger, string path, Exception? exception);

    private static readonly Action<ILogger, string, Exception?> DownloadedPackagesFolderPathNotFound = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_NuGetFeedPathNotFound, $"{Message_Downloaded_Packages_Folder_Path} \"{{Path}}\" {Message_Not_Found}.");


    // [LoggerMessage(EventId = (int)AppEventId.NuGetFeedPathNotFound, Level = LogLevel.Critical, Message = $"{Message_Downloaded_Packages_Folder_Path} \"{{Path}}\" {Message_Not_Found}.")]
    // private static partial void DownloadedPackagesFolderPathNotFound(ILogger logger, string path, Exception? exception);

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
        DownloadedPackagesFolderPathNotFound(logger, path, exception);
        return factory($"{Message_Downloaded_Packages_Folder_Path} \"{path}\" {Message_Not_Found}.");
    }

    #endregion

    #region InvalidMetaDataExportPath Logger Event Methods

    public static readonly EventId EventId_InvalidMetaDataExportPath = new((int)AppEventId.InvalidMetaDataExportPath, nameof(InvalidMetaDataExportPath));

    private const string MESSAGE_Downloaded_Nuget_Package_MetaData_Export_Path = "Downloaded NuGet Package metadata export path";

    private const string MESSAGE_Downloaded_MetaData_Export_Parent_Not_found = "Parent subdirectory not found for package metadata export path";

    private static readonly Action<ILogger, string, Exception?> LogExportDownloadedMetaDataDirectoryNotFound = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_InvalidMetaDataExportPath, $"{MESSAGE_Downloaded_MetaData_Export_Parent_Not_found} \"{{Path}}\".");

    // [LoggerMessage(EventId = (int)AppEventId.InvalidMetaDataExportPath, Level = LogLevel.Critical, Message = $"{MESSAGE_Downloaded_MetaData_Export_Parent_Not_found} \"{{Path}}\".")]
    // private static partial void LogExportDownloadedMetaDataDirectoryNotFound(ILogger logger, string path, Exception? exception);

    private static readonly Action<ILogger, string, Exception?> ExportDownloadedMetaDataPathTooLong = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_InvalidMetaDataExportPath, $"{MESSAGE_Downloaded_Nuget_Package_MetaData_Export_Path} \"{{Path}}\" {Message_Is_Too_Long}.");


    // [LoggerMessage(EventId = (int)AppEventId.InvalidMetaDataExportPath, Level = LogLevel.Critical, Message = $"{MESSAGE_Downloaded_Nuget_Package_MetaData_Export_Path} \"{{Path}}\" {Message_Is_Too_Long}.")]
    // private static partial void ExportDownloadedMetaDataPathTooLong(ILogger logger, string path, Exception exception);

    private static readonly Action<ILogger, string, Exception?> LogInvalidMetaDataExportPath = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_InvalidMetaDataExportPath, $"{MESSAGE_Downloaded_Nuget_Package_MetaData_Export_Path} \"{{Path}}\" {Message_Is_Invalid}.");

    // [LoggerMessage(EventId = (int)AppEventId.InvalidMetaDataExportPath, Level = LogLevel.Critical, Message = $"{MESSAGE_Downloaded_Nuget_Package_MetaData_Export_Path} \"{{Path}}\" {Message_Is_Invalid}.")]
    // private static partial void LogInvalidMetaDataExportPath(ILogger logger, string path, Exception? exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidMetaDataExportPath"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The package metadata export path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T InvalidMetaDataExportPath<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        if (exception is not null)
        {
            if (exception is DirectoryNotFoundException)
            {
                LogExportDownloadedMetaDataDirectoryNotFound(logger, path, exception);
                return factory($"{MESSAGE_Downloaded_MetaData_Export_Parent_Not_found} \"{path}\".");
            }
            if (exception is PathTooLongException)
            {
                ExportDownloadedMetaDataPathTooLong(logger, path, exception);
                return factory($"{MESSAGE_Downloaded_Nuget_Package_MetaData_Export_Path} \"{path}\" {Message_Is_Too_Long}.");
            }
        }
        LogInvalidMetaDataExportPath(logger, path, exception);
        return factory($"{MESSAGE_Downloaded_Nuget_Package_MetaData_Export_Path} \"{path}\" {Message_Is_Invalid}.");
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidMetaDataExportPath"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet feed path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T ExportDownloadedMetaDataDirectoryNotFound<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        LogExportDownloadedMetaDataDirectoryNotFound(logger, path, exception);
        return factory(MESSAGE_Downloaded_MetaData_Export_Parent_Not_found);
    }

    #endregion

    #region MetaDataExportPathAccessDenied Logger Event Methods

    public static readonly EventId EventId_MetaDataExportPathAccessDenied = new((int)AppEventId.MetaDataExportPathAccessDenied, nameof(MetaDataExportPathAccessDenied));

    private const string MESSAGE_Access_To_MetaData_Export_Path = "Access denied to NuGet Feed metadata export path";

    private static readonly Action<ILogger, string, Exception?> LogMetaDataExportPathAccessDenied = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_MetaDataExportPathAccessDenied, $"{MESSAGE_Access_To_MetaData_Export_Path} \"{{Path}}\" {Message_Is_Denied}.");

    // [LoggerMessage(EventId = (int)AppEventId.MetaDataExportPathAccessDenied, Level = LogLevel.Critical, EventName = nameof(AppEventId.MetaDataExportPathAccessDenied),
    //     Message = $"{MESSAGE_Access_To_MetaData_Export_Path} \"{{Path}}\" {Message_Is_Denied}.")]
    // private static partial void LogMetaDataExportPathAccessDenied(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_InsufficientPermissionsForMetaDataExportPath = "Caller has insufficient permissions to the metadata export path";

    private static readonly Action<ILogger, string, Exception?> InsufficientPermissionsForMetaDataExportPath = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_MetaDataExportPathAccessDenied, $"{MESSAGE_InsufficientPermissionsForMetaDataExportPath} \"{{Path}}\".");

    // [LoggerMessage(EventId = (int)AppEventId.MetaDataExportPathAccessDenied, Level = LogLevel.Critical, Message = $"{MESSAGE_InsufficientPermissionsForMetaDataExportPath} \"{{Path}}\".")]
    // private static partial void InsufficientPermissionsForMetaDataExportPath(ILogger logger, string path, Exception exception);

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
    /// <param name="folderPath">The path of the Downloaded Package Folder.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.NuGetPackageDeleted, Level = LogLevel.Warning,
        Message = "NuGet Package {PackageId} has been deleted from Downloaded NuGet Packages Folder {FolderPath}.")]
    public static partial void NuGetPackageDeleted(this ILogger logger, string packageId, string folderPath, Exception? exception = null);

    #region NuGetPackageNotFound Logger Event Methods

    [LoggerMessage(EventId = (int)AppEventId.NuGetPackageNotFound, Level = LogLevel.Warning, Message = "NuGet Package {PackageId} not found in Upstream NuGet Feed {FolderPath}.")]
    private static partial void NuGetPackageNotFoundInUpstreamFeed(this ILogger logger, string packageId, string folderPath, Exception? exception);

    [LoggerMessage(EventId = (int)AppEventId.NuGetPackageNotFound, Level = LogLevel.Warning, Message = "NuGet Package {PackageId} not found in Downloaded Package Folder {FolderPath}.")]
    private static partial void NuGetPackageNotFoundInDownloadedPackageFolder(this ILogger logger, string packageId, string folderPath, Exception? exception);

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
                NuGetPackageNotFoundInDownloadedPackageFolder(logger, packageId, clientService.PackageSourceLocation, exception);
        }
        else
            NuGetPackageNotFoundInRemoteServer(logger, packageId, clientService.PackageSourceLocation, exception);
    }


    [LoggerMessage(EventId = (int)AppEventId.NuGetPackageNotFound, Level = LogLevel.Warning,
        Message = "Version {Version} of package {PackageId} not found in NuGet Server (Service Index URL = {URL}).")]
    private static partial void NuGetPackageNotFoundInRemoteServer(ILogger logger, NuGetVersion version, string packageId, string url, Exception? exception);

    [Obsolete("Put version argument before packageId")]
    private static void NuGetPackageNotFoundInRemoteServer(ILogger logger, string packageId, NuGetVersion version, string url, Exception? exception) => NuGetPackageNotFoundInRemoteServer(logger, version, packageId, url, exception);
    
    [LoggerMessage(EventId = (int)AppEventId.NuGetPackageNotFound, Level = LogLevel.Warning,
        Message = "Version {Version} of package {PackageId} not found in Downloaded Package Folder {PackageSourceLocation}.")]
    private static partial void NuGetPackageNotFoundInDownloadedPackageFolder(ILogger logger, NuGetVersion version, string packageId, string packageSourceLocation, Exception? exception);
    
    [Obsolete("Put version argument before packageId")]
    private static void NuGetPackageNotFoundInDownloadedPackageFolder(ILogger logger, string packageId, NuGetVersion version, string packageSourceLocation, Exception? exception) => NuGetPackageNotFoundInDownloadedPackageFolder(logger, version, packageId, packageSourceLocation, exception);

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
                NuGetPackageNotFoundInDownloadedPackageFolder(logger, version, packageId, clientService.PackageSourceLocation, exception);
        }
        else
            NuGetPackageNotFoundInRemoteServer(logger, version, packageId, clientService.PackageSourceLocation, exception);
    }

    #endregion

    #region GlobalPackagesFolderNotFound Logger Event Methods

    public static readonly EventId EventId_GlobalPackagesFolderNotFound = new((int)AppEventId.GlobalPackagesFolderNotFound, nameof(GlobalPackagesFolderNotFound));

    private static readonly Action<ILogger, string, Exception?> LogGlobalPackagesFolderNotFound = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_GlobalPackagesFolderNotFound, $"{MESSAGE_Global_Packages_Folder} \"{{Path}}\" {Message_Not_Found}.");

    // [LoggerMessage(EventId = (int)AppEventId.GlobalPackagesFolderNotFound, EventName = nameof(AppEventId.GlobalPackagesFolderNotFound), Level = LogLevel.Critical,
    //     Message = $"{MESSAGE_Global_Packages_Folder} \"{{Path}}\" {Message_Not_Found}.")]
    // private static partial void LogGlobalPackagesFolderNotFound(ILogger logger, string path, Exception? exception);

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

    public static readonly EventId EventId_GlobalPackagesFolderSecurityException = new((int)AppEventId.GlobalPackagesFolderSecurityException, nameof(GlobalPackagesFolderSecurityException));

    private const string Message_Access_To_Global_Packages_Folder = "Access to Global NuGet Packages Folder";

    private static readonly Action<ILogger, string, Exception?> LogGlobalPackagesFolderSecurityException = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_GlobalPackagesFolderSecurityException, $"{Message_Access_To_Global_Packages_Folder} \"{{Path}})\" {Message_Is_Denied}.");

    // [LoggerMessage(EventId = (int)AppEventId.GlobalPackagesFolderSecurityException, Level = LogLevel.Critical, EventName = nameof(AppEventId.GlobalPackagesFolderSecurityException),
    //     Message = $"{Message_Access_To_Global_Packages_Folder} \"{{Path}})\" {Message_Is_Denied}.")]
    // private static partial void LogGlobalPackagesFolderSecurityException(ILogger logger, string path, Exception? exception);

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

    public static readonly EventId EventId_InvalidGlobalPackagesFolder = new((int)AppEventId.InvalidGlobalPackagesFolder, nameof(InvalidGlobalPackagesFolder));

    private const string MESSAGE_Global_Packages_Folder_Path = "Global NuGet Packages Folder path";

    private static readonly Action<ILogger, string, Exception?> GlobalPackagesFolderPathTooLong = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_InvalidGlobalPackagesFolder, $"{MESSAGE_Global_Packages_Folder_Path} \"{{Path}}\" {Message_Is_Too_Long}.");

    // [LoggerMessage(EventId = (int)AppEventId.InvalidGlobalPackagesFolder, Level = LogLevel.Critical,
    //     Message = $"{MESSAGE_Global_Packages_Folder_Path} \"{{Path}}\" {Message_Is_Too_Long}.")]
    // private static partial void GlobalPackagesFolderPathTooLong(ILogger logger, string path, Exception exception);

    private static readonly Action<ILogger, string, Exception?> LogInvalidGlobalPackagesFolder = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_InvalidGlobalPackagesFolder, $"{MESSAGE_Global_Packages_Folder_Path} \"{{Path}}\" {Message_Is_Invalid}.");

    // [LoggerMessage(EventId = (int)AppEventId.InvalidGlobalPackagesFolder, Level = LogLevel.Critical, EventName = nameof(AppEventId.InvalidGlobalPackagesFolder),
    //     Message = $"{MESSAGE_Global_Packages_Folder_Path} \"{{Path}}\" {Message_Is_Invalid}.")]
    // private static partial void LogInvalidGlobalPackagesFolder(ILogger logger, string path, Exception? exception);

    private static readonly Action<ILogger, string, Exception?> LogGlobalPackagesFolderNotFileUri = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_InvalidGlobalPackagesFolder, $"{MESSAGE_Global_Packages_Folder_Path} \"{{URL}}\" {Message_Not_Filesystem_Subdirectory}.");

    // [LoggerMessage(EventId = (int)AppEventId.InvalidGlobalPackagesFolder, Level = LogLevel.Critical, EventName = nameof(GlobalPackagesFolderNotFileUri),
    //     Message = $"{MESSAGE_Global_Packages_Folder_Path} \"{{URL}}\" {Message_Not_Filesystem_Subdirectory}.")]
    // private static partial void LogGlobalPackagesFolderNotFileUri(ILogger logger, string url, Exception? exception);

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

    public static readonly EventId EventId_MultipleSettingsWithSameRepositoryLocation = new((int)AppEventId.MultipleSettingsWithSameRepositoryLocation, nameof(AppEventId.MultipleSettingsWithSameRepositoryLocation));

    private const string MESSAGE_Cannot_Be_Same_As = "cannot be the same as the";

    private static readonly Action<ILogger, string, Exception?> LogDownloadedFolderSameAsUpstreamNugetFeed = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_MultipleSettingsWithSameRepositoryLocation, $"{Message_Downloaded_Packages_Folder_Path} \"{{Path}}\" {MESSAGE_Cannot_Be_Same_As} {Message_Upstream_Feed_Path}.");

    // [LoggerMessage(EventId = (int)AppEventId.MultipleSettingsWithSameRepositoryLocation, Level = LogLevel.Critical, EventName = nameof(DownloadedFolderSameAsUpstreamNugetFeed),
    //     Message = $"{Message_Downloaded_Packages_Folder_Path} \"{{Path}}\" {MESSAGE_Cannot_Be_Same_As} {Message_Upstream_Feed_Path}.")]
    // private static partial void LogDownloadedFolderSameAsUpstreamNugetFeed(ILogger logger, string path, Exception? exception);

    private static readonly Action<ILogger, string, Exception?> LogDownloadedFolderSameAsGlobalPackagesFolder = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_MultipleSettingsWithSameRepositoryLocation, $"{Message_Downloaded_Packages_Folder_Path} \"{{Path}}\" {MESSAGE_Cannot_Be_Same_As} {MESSAGE_Global_Packages_Folder}.");

    // [LoggerMessage(EventId = (int)AppEventId.MultipleSettingsWithSameRepositoryLocation, Level = LogLevel.Critical, EventName = nameof(DownloadedFolderSameAsGlobalPackagesFolder),
    //     Message = $"{Message_Downloaded_Packages_Folder_Path} \"{{Path}}\" {MESSAGE_Cannot_Be_Same_As} {MESSAGE_Global_Packages_Folder}.")]
    // private static partial void LogDownloadedFolderSameAsGlobalPackagesFolder(ILogger logger, string path, Exception? exception);

    private static readonly Action<ILogger, string, Exception?> LogUpstreamNuGetFeedSameAsGlobalPackagesFolder = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_MultipleSettingsWithSameRepositoryLocation, $"{Message_Upstream_Feed_Path} \"{{Path}}\" {MESSAGE_Cannot_Be_Same_As} {MESSAGE_Global_Packages_Folder}.");

    // [LoggerMessage(EventId = (int)AppEventId.MultipleSettingsWithSameRepositoryLocation, Level = LogLevel.Critical, EventName = nameof(UpstreamNuGetFeedSameAsGlobalPackagesFolder),
    //     Message = $"{Message_Upstream_Feed_Path} \"{{Path}}\" {MESSAGE_Cannot_Be_Same_As} {MESSAGE_Global_Packages_Folder}.")]
    // private static partial void LogUpstreamNuGetFeedSameAsGlobalPackagesFolder(ILogger logger, string path, Exception? exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.MultipleSettingsWithSameRepositoryLocation"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static string DownloadedFolderSameAsUpstreamNugetFeed(this ILogger logger, string path, Exception? exception = null)
    {
        LogDownloadedFolderSameAsUpstreamNugetFeed(logger, path, exception);
        return $"{Message_Downloaded_Packages_Folder_Path} \"{path}\" {MESSAGE_Cannot_Be_Same_As} {Message_Upstream_Feed_Path}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.MultipleSettingsWithSameRepositoryLocation"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static string DownloadedFolderSameAsGlobalPackagesFolder(this ILogger logger, string path, Exception? exception = null)
    {
        LogDownloadedFolderSameAsGlobalPackagesFolder(logger, path, exception);
        return $"{Message_Downloaded_Packages_Folder_Path} \"{path}\" {MESSAGE_Cannot_Be_Same_As} {MESSAGE_Global_Packages_Folder}.";
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
    [LoggerMessage(EventId = (int)AppEventId.PackageAlreadyAdded, Level = LogLevel.Warning, Message = "NuGet Package {PackageId} has already been added to the Downloaded NuGet Packages Folder.")]
    public static partial void NuGetPackageAlreadyAdded(this ILogger logger, string packageId, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="AppEventId.PackageVersionDeleteFailure"/> <see cref="LogLevel.Warning"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.PackageVersionDeleteFailure, Level = LogLevel.Warning,
        Message = "Unexpected error deleting NuGet Package {PackageId}, Version {Version} from the Downloaded NuGet Packages Folder.")]
    public static partial void PackageVersionDeleteFailure(this ILogger logger, string packageId, NuGetVersion version, Exception? exception = null);

    #region UnexpectedPackageDownloadFailure Logger Event Methods

    /// <summary>
    /// Logs a <see cref="AppEventId.UnexpectedPackageDownloadFailure"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="exception">The exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.UnexpectedPackageDownloadFailure, Level = LogLevel.Error,
        Message = "Unexpected error while downloading package {PackageId}, Version {Version} from the upstream Nuget repository.")]
    public static partial void UnexpectedPackageDownloadFailure(this ILogger logger, string packageId, NuGetVersion version, Exception exception);

    /// <summary>
    /// Logs a <see cref="AppEventId.UnexpectedPackageDownloadFailure"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    [LoggerMessage(EventId = (int)AppEventId.UnexpectedPackageDownloadFailure, Level = LogLevel.Error,
        Message = "NuGet Package download of {PackageId}, Version {Version} from the upstream Nuget repository was unexpectedly empty.")]
    public static partial void DownloadPackageIsEmpty(this ILogger logger, string packageId, NuGetVersion version);

    #endregion

    /// <summary>
    /// Logs a <see cref="AppEventId.UnexpectedAddFailure"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.UnexpectedAddFailure, Level = LogLevel.Error,
        Message = "Unexpected error while adding Package {PackageId}, Version {Version} to the Downloaded NuGet Packages Folder.")]
    public static partial void UnexpectedAddNuGetPackageFailure(this ILogger logger, string packageId, NuGetVersion version, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="AppEventId.NoDownloadedPackagesExist"/> <see cref="LogLevel.Warning"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.NoDownloadedPackagesExist, Level = LogLevel.Warning, Message = "Downloaded NuGet Packages Folder has no packages.")]
    public static partial void NoDownloadedPackagesPackagesExist(this ILogger logger, Exception? exception = null);

    public const string Message_Is_Not_A_File = "is not a file";

    #region InvalidExportBundle Logger Event Methods

    public static readonly EventId EventId_InvalidExportBundle = new((int)AppEventId.InvalidExportBundle, nameof(InvalidExportBundle));

    private const string Message_Export_Bundle_Path = "Export Bundle path";
    
    private static readonly Action<ILogger, string, Exception?> LogInvalidExportBundle = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_InvalidExportBundle, $"{Message_Export_Bundle_Path} {{Path}} {Message_Is_Invalid}.");

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidExportBundle"/> error event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The export bundle path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void InvalidExportBundle(this ILogger logger, string path, Exception? exception = null) => LogInvalidExportBundle(logger, path, exception);

    // [LoggerMessage(EventId = (int)AppEventId.InvalidExportBundle, Level = LogLevel.Critical, Message = $"{Message_Export_Bundle_Path} {{Path}} {Message_Is_Invalid}.")]
    // public static partial void InvalidExportBundle(this ILogger logger, string path, Exception? exception = null);

    public static T InvalidExportBundle<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : Exception
    {
        InvalidExportBundle(logger, path, exception);
        return factory($"{Message_Export_Bundle_Path} {path} {Message_Is_Invalid}.");
    }

    private static readonly Action<ILogger, string, Exception?> LogExportBundlePathNotAFile = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_InvalidExportBundle, $"{Message_Export_Bundle_Path} {{Path}} {Message_Not_File}.");

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidExportBundle"/> error event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The export bundle path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void ExportBundlePathNotAFile(this ILogger logger, string path, Exception? exception = null) => LogExportBundlePathNotAFile(logger, path, exception);
    // [LoggerMessage(EventId = (int)AppEventId.InvalidExportBundle, Level = LogLevel.Critical, Message = $"{Message_Export_Bundle_Path} {{Path}} {Message_Not_File}.")]
    // public static partial void ExportBundlePathNotAFile(this ILogger logger, string path, Exception? exception = null);

    public static T ExportBundlePathNotAFile<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : Exception
    {
        ExportBundlePathNotAFile(logger, path, exception);
        return factory($"{Message_Export_Bundle_Path} {path} {Message_Not_File}.");
    }

    private const string Message_Export_Bundle_Path_Parent_Directory = "Parent directory of Export Bundle path";
    
    private static readonly Action<ILogger, string, Exception?> LogExportBundleDirectoryNotFound = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_InvalidExportBundle, $"{Message_Export_Bundle_Path_Parent_Directory} {{Path}} {Message_Does_Not_Exist}.");

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidExportBundle"/> error event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The export bundle path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void ExportBundleDirectoryNotFound(this ILogger logger, string path, Exception? exception = null) => LogExportBundleDirectoryNotFound(logger, path, exception);
    // [LoggerMessage(EventId = (int)AppEventId.InvalidExportBundle, Level = LogLevel.Critical, Message = $"{Message_Export_Bundle_Path_Parent_Directory} {{Path}} {Message_Does_Not_Exist}.")]
    // public static partial void ExportBundleDirectoryNotFound(this ILogger logger, string path, Exception? exception = null);

    public static T ExportBundleDirectoryNotFound<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : Exception
    {
        ExportBundlePathNotAFile(logger, path, exception);
        return factory($"{Message_Export_Bundle_Path_Parent_Directory} {path} {Message_Does_Not_Exist}.");
    }

    private const string Message_ExportBundleAccessError = "Unexpected error accessing export bundle path";

    private static readonly Action<ILogger, string, Exception?> LogExportBundleAccessError = LoggerMessage.Define<string>(LogLevel.Critical,
        EventId_InvalidExportBundle, $"{Message_ExportBundleAccessError} {{Path}}.");

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidExportBundle"/> error event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The export bundle path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void ExportBundleAccessError(this ILogger logger, string path, Exception? exception = null) => LogExportBundleAccessError(logger, path, exception);
    // [LoggerMessage(EventId = (int)AppEventId.InvalidExportBundle, Level = LogLevel.Critical, Message = $"{Message_ExportBundleAccessError} {{Path}}.")]
    // public static partial void ExportBundleAccessError(this ILogger logger, string path, Exception? exception = null);

    public static T ExportBundleAccessError<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : Exception
    {
        ExportBundleAccessError(logger, path, exception);
        return factory($"{Message_ExportBundleAccessError} {path}.");
    }

    #endregion

    #region DownloadingNuGetPackage Logger Event Methods

    [LoggerMessage(EventId = (int)AppEventId.DownloadingNuGetPackage, Level = LogLevel.Warning, Message = "Downloading package {PackageId}, version {Version} from NuGet Packages Folder {PackageSourceLocation}.")]
    private static partial void CopyingDownloadedNuGetPackageVersion(ILogger logger, string packageId, NuGetVersion version, string packageSourceLocation, Exception? exception);

    [LoggerMessage(EventId = (int)AppEventId.DownloadingNuGetPackage, Level = LogLevel.Warning, Message = "Downloading package {PackageId} from NuGet Packages Folder {PackageSourceLocation}.")]
    private static partial void CopyingDownloadedNuGetPackage(ILogger logger, string packageId, string packageSourceLocation, Exception? exception);

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
                CopyingDownloadedNuGetPackageVersion(logger, identity.Id, identity.Version, clientService.PackageSourceLocation, exception);
            else
                CopyingDownloadedNuGetPackage(logger, identity.Id, clientService.PackageSourceLocation, exception);
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
    [LoggerMessage(EventId = (int)AppEventId.InvalidCreateFromPath, Level = LogLevel.Critical, Message = "Create-from path {Path} is invalid.")]
    public static partial void InvalidCreateFromPath(this ILogger logger, string path, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidCreateFromPath"/> error event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The Create-From path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.InvalidCreateFromPath, Level = LogLevel.Critical, Message = "Create-from path {Path} does not exist.")]
    public static partial void CreateFromFileNotFound(this ILogger logger, string path, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidCreateFromPath"/> error event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The Create-From path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.InvalidCreateFromPath, Level = LogLevel.Critical, Message = "Create-from path {Path} is not a file.")]
    public static partial void CreateFromNotAFile(this ILogger logger, string path, Exception? exception = null);

    #endregion

    #region InvalidSaveMetadataToPath Logger Event Methods

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidSaveMetadataToPath"/> error event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The filesystem path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.InvalidSaveMetadataToPath, Level = LogLevel.Critical, Message = "Save-to path {Path} is invalid")]
    public static partial void InvalidSaveMetadataToPath(this ILogger logger, string path, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidSaveMetadataToPath"/> error event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The filesystem path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.InvalidSaveMetadataToPath, Level = LogLevel.Critical, Message = "Save-to path {Path} is invalid")]
    public static partial void SaveMetadataToFileNotFound(this ILogger logger, string path, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="AppEventId.InvalidSaveMetadataToPath"/> error event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The filesystem path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)AppEventId.InvalidSaveMetadataToPath, Level = LogLevel.Critical, Message = "Save-to path {Path} is invalid")]
    public static partial void SaveMetadataToPathNotAFile(this ILogger logger, string path, Exception? exception = null);

    #endregion

    #region PackageMetadataOpenError Logger Event Methods
    
    public static readonly EventId EventId_PackageMetadataOpenError = new((int)AppEventId.PackageMetadataOpenError, nameof(PackageMetadataOpenError));

    private const string MESSAGE_Package_Metadata_File = "Offline package metadata file";
    
    private const string MESSAGE_Package_Metadata_Path = "Offline package metadata file path";
    
    private const string MESSAGE_Package_Metadata_Parent_Directory = "Parent directgory of offline package metadata file path";
    
    private static readonly Action<ILogger, string, Exception?> LogPackageMetadataOpenError = LoggerMessage.Define<string>(LogLevel.Error,
        EventId_PackageMetadataOpenError, $"{MESSAGE_Package_Metadata_Path} {{Path}} {Message_Is_Invalid}.");

    // [LoggerMessage(EventId = (int)AppEventId.PackageMetadataOpenError, Level = LogLevel.Error, EventName = nameof(PackageMetadataOpenError),
    //     Message = $"{MESSAGE_Package_Metadata_Path} {{Path}} {Message_Is_Invalid}.")]
    // private static partial void LogPackageMetadataOpenError(ILogger logger, string path, Exception? exception);
    
    private static readonly Action<ILogger, string, Exception?> OfflinePackageMetadataFileNotFound = LoggerMessage.Define<string>(LogLevel.Error,
        EventId_PackageMetadataOpenError, $"{MESSAGE_Package_Metadata_File} {{Path}} {Message_Not_Found}.");

    // [LoggerMessage(EventId = (int)AppEventId.PackageMetadataOpenError, Level = LogLevel.Error,
    //     Message = $"{MESSAGE_Package_Metadata_File} {{Path}} {Message_Not_Found}.")]
    // private static partial void OfflinePackageMetadataFileNotFound(ILogger logger, string path, Exception? exception);
    
    private static readonly Action<ILogger, string, Exception?> LogOfflinePackageMetadataDirectoryNotFound = LoggerMessage.Define<string>(LogLevel.Error,
        EventId_PackageMetadataOpenError, $"{MESSAGE_Package_Metadata_Parent_Directory} {{Path}} {Message_Does_Not_Exist}.");

    // [LoggerMessage(EventId = (int)AppEventId.PackageMetadataOpenError, Level = LogLevel.Error,
    //     Message = $"{MESSAGE_Package_Metadata_Parent_Directory} {{Path}} {Message_Does_Not_Exist}.")]
    // private static partial void LogOfflinePackageMetadataDirectoryNotFound(ILogger logger, string path, Exception? exception);
    
    private static readonly Action<ILogger, string, Exception?> LogOfflinePackageMetadataNotAFile = LoggerMessage.Define<string>(LogLevel.Error,
        EventId_PackageMetadataOpenError,  $"{MESSAGE_Package_Metadata_Path} {{Path}} {Message_Not_File}.");

    // [LoggerMessage(EventId = (int)AppEventId.PackageMetadataOpenError, Level = LogLevel.Error,
    //     Message = $"{MESSAGE_Package_Metadata_Path} {{Path}} {Message_Not_File}.")]
    // private static partial void LogOfflinePackageMetadataNotAFile(ILogger logger, string path, Exception? exception);
    
    private static readonly Action<ILogger, string, Exception?> OfflinePackageMetadataPathTooLong = LoggerMessage.Define<string>(LogLevel.Error,
        EventId_PackageMetadataOpenError, $"{MESSAGE_Package_Metadata_Path} {{Path}} {Message_Is_Too_Long}.");

    // [LoggerMessage(EventId = (int)AppEventId.PackageMetadataOpenError, Level = LogLevel.Error,
    //     Message = $"{MESSAGE_Package_Metadata_Path} {{Path}} {Message_Is_Too_Long}.")]
    // private static partial void OfflinePackageMetadataPathTooLong(ILogger logger, string path, Exception? exception);
    
    /// <summary>
    /// Logs a <see cref="AppEventId.PackageMetadataOpenError"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the offline package metadata file.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T PackageMetadataOpenError<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        if (exception is not null)
        {
            if (exception is FileNotFoundException || exception is DirectoryNotFoundException)
            {
                OfflinePackageMetadataFileNotFound(logger, path, exception);
                return factory($"{MESSAGE_Package_Metadata_File} \"{path}\" {Message_Not_Found}.");
            }
            if (exception is PathTooLongException)
            {
                OfflinePackageMetadataPathTooLong(logger, path, exception);
                return factory($"{MESSAGE_Package_Metadata_Path} \"{path}\" {Message_Is_Too_Long}.");
            }
        }
        LogPackageMetadataOpenError(logger, path, exception);
        return factory($"{MESSAGE_Package_Metadata_Path} \"{path}\" {Message_Is_Invalid}.");
    }
    
    public static T OfflinePackageMetadataNotAFile<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        LogOfflinePackageMetadataNotAFile(logger, path, exception);
        return factory($"{MESSAGE_Package_Metadata_Path} \"{path}\" {Message_Not_File}.");
    }
    
    public static T OfflinePackageMetadataDirectoryNotFound<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        LogOfflinePackageMetadataDirectoryNotFound(logger, path, exception);
        return factory($"{MESSAGE_Package_Metadata_Parent_Directory} \"{path}\" {Message_Does_Not_Exist}.");
    }
    
    #endregion

    #region PackageMetadataFileAccessDenied Logger Event Methods
    
    public static readonly EventId EventId_PackageMetadataFileAccessDenied = new((int)AppEventId.PackageMetadataFileAccessDenied, nameof(PackageMetadataFileAccessDenied));

    private const string MESSAGE_Access_To_Package_MetaData_File = "Access to the Offline NuGet Feed metadata file";

    private const string MESSAGE_InsufficientPermissionsForPackageMetadataFile = "Caller has insufficient permissions to the Offline NuGet Feed metadata file";

    private static readonly Action<ILogger, string, Exception?> LogPackageMetadataFileAccessDenied = LoggerMessage.Define<string>(LogLevel.Error,
        EventId_PackageMetadataFileAccessDenied, $"{MESSAGE_Access_To_Package_MetaData_File} {{Path}} {Message_Is_Denied}.");

    // [LoggerMessage(EventId = (int)AppEventId.PackageMetadataFileAccessDenied, Level = LogLevel.Error, EventName = nameof(PackageMetadataFileAccessDenied),
    //     Message = $"{MESSAGE_Access_To_Package_MetaData_File} {{Path}} {Message_Is_Denied}.")]
    // private static partial void LogPackageMetadataFileAccessDenied(ILogger logger, string path, Exception? exception);

    private static readonly Action<ILogger, string, Exception?> InsufficientPermissionsForPackageMetadataFile = LoggerMessage.Define<string>(LogLevel.Error,
        EventId_PackageMetadataFileAccessDenied, $"{MESSAGE_InsufficientPermissionsForPackageMetadataFile} {{Path}}.");

    // [LoggerMessage(EventId = (int)AppEventId.PackageMetadataFileAccessDenied, Level = LogLevel.Error,
    //     Message = $"{MESSAGE_InsufficientPermissionsForPackageMetadataFile} {{Path}}.")]
    // private static partial void InsufficientPermissionsForPackageMetadataFile(ILogger logger, string path, Exception? exception);
    
    /// <summary>
    /// Logs a <see cref="AppEventId.PackageMetadataFileAccessDenied"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The package metadata path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T PackageMetadataFileAccessDenied<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        if (exception is System.Security.SecurityException)
        {
            InsufficientPermissionsForPackageMetadataFile(logger, path, exception);
            return factory($"{MESSAGE_InsufficientPermissionsForPackageMetadataFile} \"{path}\".");
        }
        LogPackageMetadataFileAccessDenied(logger, path, exception);
        return factory($"{MESSAGE_Access_To_Package_MetaData_File} \"{path}\" {Message_Is_Denied}.");
    }
    
    #endregion

    #region PackageMetadataFileReadError Logger Event Methods
    
    public static readonly EventId EventId_PackageMetadataFileReadError = new((int)AppEventId.PackageMetadataFileReadError, nameof(PackageMetadataFileReadError));

    private const string MESSAGE_PackageMetadataFileReadError = "Error parsing JSON data from offline package metadata file";
    
    private static readonly Action<ILogger, string, Exception?> LogPackageMetadataFileReadError = LoggerMessage.Define<string>(LogLevel.Error,
        EventId_PackageMetadataFileReadError, $"{MESSAGE_PackageMetadataFileReadError} {{Path}}.");

    // [LoggerMessage(EventId = (int)AppEventId.PackageMetadataFileReadError, Level = LogLevel.Error, EventName = nameof(PackageMetadataFileReadError),
    //     Message = $"{MESSAGE_PackageMetadataFileReadError} {{Path}}.")]
    // private static partial void LogPackageMetadataFileReadError(ILogger logger, string path, Exception? exception);
    
    /// <summary>
    /// Logs a <see cref="AppEventId.PackageMetadataFileReadError"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The offline package metadata file path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T PackageMetadataFileReadError<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        LogPackageMetadataFileReadError(logger, path, exception);
        return factory($"{MESSAGE_PackageMetadataFileReadError} {path}.");
    }
    
    #endregion

    #region PackageExportAccessDenied Logger Event Methods
    
    public static readonly EventId EventId_PackageExportAccessDenied = new((int)AppEventId.PackageExportAccessDenied, nameof(PackageExportAccessDenied));

    private const string MESSAGE_Access_To_Package_File = "Access to the Offline NuGet Feed metadata file";

    private const string MESSAGE_InsufficientPermissionsForPackageExport = "Caller has insufficient permissions to the Offline NuGet Feed metadata file";
    
    private static readonly Action<ILogger, string, Exception?> LogPackageExportAccessDenied = LoggerMessage.Define<string>(LogLevel.Error,
        EventId_PackageExportAccessDenied, $"{MESSAGE_Access_To_Package_File} {{Path}} {Message_Is_Denied}.");

    private static readonly Action<ILogger, string, Exception?> InsufficientPermissionsForPackageExport = LoggerMessage.Define<string>(LogLevel.Error,
        EventId_PackageExportAccessDenied, $"{MESSAGE_InsufficientPermissionsForPackageExport} {{Path}}.");

    // [LoggerMessage(EventId = (int)AppEventId.PackageExportAccessDenied, Level = LogLevel.Error, EventName = nameof(PackageExportAccessDenied),
    //     Message = $"{MESSAGE_Access_To_Package_File} {{Path}} {Message_Is_Denied}.")]
    // private static partial void LogPackageExportAccessDenied(ILogger logger, string path, Exception? exception);
    
    // [LoggerMessage(EventId = (int)AppEventId.PackageExportAccessDenied, Level = LogLevel.Error,
    //     Message = $"{MESSAGE_InsufficientPermissionsForPackageExport} {{Path}}.")]
    // private static partial void InsufficientPermissionsForPackageExport(ILogger logger, string path, Exception? exception);
    
    /// <summary>
    /// Logs a <see cref="AppEventId.PackageExportAccessDenied"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The package metadata path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T PackageExportAccessDenied<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        if (exception is System.Security.SecurityException)
        {
            InsufficientPermissionsForPackageExport(logger, path, exception);
            return factory($"{MESSAGE_InsufficientPermissionsForPackageExport} \"{path}\".");
        }
        LogPackageExportAccessDenied(logger, path, exception);
        return factory($"{MESSAGE_Access_To_Package_File} \"{path}\" {Message_Is_Denied}.");
    }
    
    #endregion

    #region PackageExportWriteError Logger Event Methods
    
    public static readonly EventId EventId_PackageExportWriteError = new((int)AppEventId.PackageExportWriteError, nameof(PackageExportWriteError));

    private static readonly Action<ILogger, string, Exception?> LogPackageExportWriteError = LoggerMessage.Define<string>(LogLevel.Error,
        EventId_PackageExportWriteError, $"{MESSAGE_PackageExportWriteError} {{Path}}.");

    private const string MESSAGE_PackageExportWriteError = "Error writing package contents to file";
    
    // [LoggerMessage(EventId = (int)AppEventId.PackageExportWriteError, Level = LogLevel.Error, EventName = nameof(PackageExportWriteError),
    //     Message = $"{MESSAGE_PackageExportWriteError} {{Path}}.")]
    // private static partial void LogPackageExportWriteError(ILogger logger, string path, Exception? exception);
    
    /// <summary>
    /// Logs a <see cref="AppEventId.PackageExportWriteError"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The offline package metadata file path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T PackageExportWriteError<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        LogPackageExportWriteError(logger, path, exception);
        return factory($"{MESSAGE_PackageExportWriteError} {path}.");
    }
    
    #endregion

    #region GetDownloadResource Scope

    private static readonly Func<ILogger, string, NuGetVersion?, string, IDisposable?> _getNugetServerDownloadResourceResultScope = LoggerMessage.DefineScope<string, NuGetVersion?, string>(
        "Getting Download Resource for package {PackageId}, version {Version} from upstream NuGet Server at {Url}."
    );

    private static readonly Func<ILogger, string, NuGetVersion?, string, IDisposable?> _getUpstreamFeedDownloadResourceResultScope = LoggerMessage.DefineScope<string, NuGetVersion?, string>(
        "Getting Download Resource for package {PackageId}, version {Version} from Upstream NuGet Feed at {Path}."
    );

    private static readonly Func<ILogger, string, NuGetVersion?, string, IDisposable?> _getDownloadedPackagesDownloadResourceResultScope = LoggerMessage.DefineScope<string, NuGetVersion?, string>(
        "Getting Download Resource for package {PackageId}, version {Version} from Downloaded NuGet Packages Folder at {Path}."
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
            return _getDownloadedPackagesDownloadResourceResultScope(logger, identity.Id, identity.HasVersion ? identity.Version : null, clientService.PackageSourceLocation);
        }
        return _getNugetServerDownloadResourceResultScope(logger, identity.Id, identity.HasVersion ? identity.Version : null, clientService.PackageSourceLocation);
    }

    #endregion

    #region DownloadNupkg Scope

    private static readonly Func<ILogger, string, NuGetVersion?, string?, IDisposable?> _downloadRemoteNupkgScope = LoggerMessage.DefineScope<string, NuGetVersion?, string?>(
       "Download NuGet package {PackageId}, version {Version} from server {Url}).");

    private static readonly Func<ILogger, string, NuGetVersion?, string, IDisposable?> _downloadUpstreamFeedNupkgScope = LoggerMessage.DefineScope<string, NuGetVersion?, string>(
       "Download NuGet package {PackageId}, version {Version} from Upstream NuGet Feed {Path}.");

    private static readonly Func<ILogger, string, NuGetVersion?, string, IDisposable?> _downloadDownloadedNupkgScope = LoggerMessage.DefineScope<string, NuGetVersion?, string>(
       "Download NuGet package {PackageId}, version {Version} from Downloaded NuGet Packages Folder {Path}.");

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
            return _downloadDownloadedNupkgScope(logger, packageId, version, clientService.PackageSourceLocation);
        }
        return _downloadRemoteNupkgScope(logger, packageId, version, clientService.PackageSourceLocation);
    }

    #endregion

    #region GetMetadata Scope

    private static readonly Func<ILogger, string, bool, bool, string, IDisposable?> _getNugetServerMetadataScope1 = LoggerMessage.DefineScope<string, bool, bool, string>(
       "Get metadata for package {PackageId} (IncludePreRelease={IncludePreRelease}; IncludeUnlisted={IncludeUnlisted}) from NuGet Server {Url}."
    );

    private static readonly Func<ILogger, string, bool, bool, string, IDisposable?> _getDownloadedPackagesMetadataScope1 = LoggerMessage.DefineScope<string, bool, bool, string>(
       "Get metadata for package {PackageId} (IncludePreRelease={IncludePreRelease}; IncludeUnlisted={IncludeUnlisted}) from Downloaded NuGet Packages Folder {Path}."
    );

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _getNugetServerMetadataScope2 = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get metadata for package {PackageId}, version {Version} from NuGet Server {Url}."
    );

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _getDownloadedPackagesMetadataScope2 = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get metadata for package {PackageId}, version {Version} from Downloaded NuGet Packages Folder {Path}."
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
            return _getDownloadedPackagesMetadataScope1(logger, packageId, includePreRelease, includeUnlisted, clientService.PackageSourceLocation);
        }
        return _getNugetServerMetadataScope1(logger, packageId, includePreRelease, includeUnlisted, clientService.PackageSourceLocation);
    }

    public static IDisposable? BeginGetMetadataScope(this ILogger logger, string packageId, NuGetVersion version, INuGetClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _getUpstreamFeedMetadataScope2(logger, packageId, version, clientService.PackageSourceLocation);
            return _getDownloadedPackagesMetadataScope2(logger, packageId, version, clientService.PackageSourceLocation);
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

    private static readonly Func<ILogger, string, string, IDisposable?> _getAllDownloadedPackageVersionsScope = LoggerMessage.DefineScope<string, string>(
       "Get all versions for package {PackageId} from Downloaded NuGet Packages Folder {Path}."
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
            return _getAllDownloadedPackageVersionsScope(logger, packageId, clientService.PackageSourceLocation);
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

    private static readonly Func<ILogger, string, NuGetVersion, NuGetFramework, string, IDisposable?> _resolveDownloadedPackageScope = LoggerMessage.DefineScope<string, NuGetVersion, NuGetFramework, string>(
       "Getting dependencies for package {PackageId}, version {Version}, framework {Framework} from Downloaded NuGet Packages Folder {Path}."
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
            return _resolveDownloadedPackageScope(logger, packageId, version, framework, clientService.PackageSourceLocation);
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

    private static readonly Func<ILogger, string, string, IDisposable?> _resolveDownloadedPackagesScope1 = LoggerMessage.DefineScope<string, string>(
       "Getting dependencies for package {PackageId} from Downloaded NuGet Packages Folder {Path}."
    );

    private static readonly Func<ILogger, string, NuGetFramework, string, IDisposable?> _resolveNugetServerPackagesScope2 = LoggerMessage.DefineScope<string, NuGetFramework, string>(
       "Getting dependencies for package {PackageId}, framework {Framework} from NuGet Server {URL}."
    );

    private static readonly Func<ILogger, string, NuGetFramework, string, IDisposable?> _resolveUpstreamFeedPackagesScope2 = LoggerMessage.DefineScope<string, NuGetFramework, string>(
       "Getting dependencies for package {PackageId}, framework {Framework} from Upstream NuGet Feed {Path})."
    );

    private static readonly Func<ILogger, string, NuGetFramework, string, IDisposable?> _resolveDownloadedPackagesScope2 = LoggerMessage.DefineScope<string, NuGetFramework, string>(
       "Getting dependencies for package {PackageId}, framework {Framework} from Downloaded NuGet Packages Folder {Path})."
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
            return _resolveDownloadedPackagesScope1(logger, packageId, clientService.PackageSourceLocation);
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
            return _resolveDownloadedPackagesScope2(logger, packageId, framework, clientService.PackageSourceLocation);
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

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _getDownloadedPackagesDependencyInfoScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get dependency information for package {PackageId}, version {Version} from Downloaded NuGet Packages Folder {Path})."
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
            return _getDownloadedPackagesDependencyInfoScope(logger, packageId, version, clientService.PackageSourceLocation);
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

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _doesDownloadedPackageExistScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get dependency information for package {PackageId}, version {Version} from Downloaded NuGet Packages Folder {Path}."
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
            return _doesDownloadedPackageExistScope(logger, packageId, version, clientService.PackageSourceLocation);
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

    private static readonly Func<ILogger, string, NuGetVersion, NuGetFramework, string, IDisposable?> _getDownloadedPackageDependenciesScope = LoggerMessage.DefineScope<string, NuGetVersion, NuGetFramework, string>(
       "Get dependencies for package {PackageId}, version {Version}, framework {Framework} from Downloaded NuGet Packages Folder {Path})."
    );

    private static readonly Func<ILogger, string, string, IDisposable?> _getAllNugetServerPackageDependenciesScope = LoggerMessage.DefineScope<string, string>(
       "Get dependencies for package {PackageId} from NuGet Server {URL}."
    );

    private static readonly Func<ILogger, string, string, IDisposable?> _getAllUpstreamFeedPackageDependenciesScope = LoggerMessage.DefineScope<string, string>(
       "Get dependencies for package {PackageId} from Upstream NuGet Feed {Path})."
    );

    private static readonly Func<ILogger, string, string, IDisposable?> _getAllDownloadedPackageDependenciesScope = LoggerMessage.DefineScope<string, string>(
       "Get dependencies for package {PackageId} from Downloaded NuGet Packages Folder {Path})."
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
            return _getDownloadedPackageDependenciesScope(logger, packageId, version, framework, clientService.PackageSourceLocation);
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
            return _getAllDownloadedPackageDependenciesScope(logger, packageId, clientService.PackageSourceLocation);
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

    #region DeleteDownloadedPackage Scope

    private static readonly Func<ILogger, string, string, IDisposable?> _deleteDownloadedPackageScope = LoggerMessage.DefineScope<string, string>(
        "Delete package {PackageId} from Downloaded NuGet Packages Folder {Path}."
    );

    /// <summary>
    /// Formats the DeleteDownloadedPackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the downloaded package to delete.</param>
    /// <param name="path">The Downloaded NuGet Packages Folder path.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginDeleteDownloadedPackageScope(this ILogger logger, string packageId, string path) => _deleteDownloadedPackageScope(logger, packageId, path);

    #endregion

    #region DeleteDownloadedPackageVersion Scope

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _deleteDownloadedlPackageVersionScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
        "Delete package {PackageId}, version {Version} from Downloaded NuGet Packages Folder {Path}."
    );

    /// <summary>
    /// Formats the DeleteDownloadedPackageVersion message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the downloaded package to delete.</param>
    /// <param name="version">The package version.</param>
    /// <param name="path">The Downloaded NuGet Packages Folder path.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginDeleteDownloadedPackageVersionScope(this ILogger logger, string packageId, NuGetVersion version, string path) => _deleteDownloadedlPackageVersionScope(logger, packageId, version, path);

    #endregion

    #region AddDownloadedPackage Scope

    private static readonly Func<ILogger, string, string, IDisposable?> _addDownloadedPackageScope = LoggerMessage.DefineScope<string, string>(
        "Add package {PackageId} to Downloaded NuGet Packages Folder {Path}."
    );

    /// <summary>
    /// Formats the AddDownloadedPackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the upstream package to add.</param>
    /// <param name="path">The Downloaded NuGet Packages Folder path.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginAddDownloadedPackageScope(this ILogger logger, string packageId, string path) => _addDownloadedPackageScope(logger, packageId, path);

    #endregion

    #region UpdateDownloadedPackage Scope

    private static readonly Func<ILogger, string, string, IDisposable?> _updateDownloadedPackageScope = LoggerMessage.DefineScope<string, string>(
        "Update package {PackageId} in Downloaded NuGet Packages Folder {Path}."
    );

    /// <summary>
    /// Formats the UpdateDownloadedPackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the upstream package to update.</param>
    /// <param name="path">The Downloaded NuGet Packages Folder path.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginUpdateDownloadedPackageScope(this ILogger logger, string packageId, string path) => _updateDownloadedPackageScope(logger, packageId, path);

    #endregion

    #region GetAllDownloadedPackages Scope

    private static readonly Func<ILogger, string, IDisposable?> _getAllDownloadedPackagesScope = LoggerMessage.DefineScope<string>(
        "Getting all packages from {Path}."
    );

    /// <summary>
    /// Formats the GetAllDownloadedPackages message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The Downloaded NuGet Packages Folder path.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetAllDownloadedPackagesScope(this ILogger logger, string path) => _getAllDownloadedPackagesScope(logger, path);

    #endregion
}