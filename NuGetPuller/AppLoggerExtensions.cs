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
    #region Nuget Logger Event Methods

    #region NuGetDebug Logger Event Methods

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.NuGetDebug"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    [LoggerMessage(EventId = (int)NuGetPullerEventId.NuGetDebug, Level = LogLevel.Debug, Message = "NuGet API Debug: {Message})")]
    public static partial void NugetDebugMessage(this ILogger logger, string message);

    [LoggerMessage(EventId = (int)NuGetPullerEventId.NuGetDebug, Level = LogLevel.Debug, Message = "NuGet API Debug {NugetID} ({Code}): {Message}")]
    private static partial void NugetDebugCode(ILogger logger, string nuGetId, int code, string message);

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.NuGetDebug"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    /// <param name="code">The NuGet log code.</param>
    public static void NugetDebugMessage(this ILogger logger, string message, NuGet.Common.NuGetLogCode code) =>
        NugetDebugCode(logger, code.ToString("F"), (int)code, message);

    /// <summary>
    /// Logs a <see cref="NuGet.Common.LogLevel.Verbose"/> <see cref="NuGetPullerEventId.NuGetDebug"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    /// <param name="code">The NuGet log code.</param>
    [LoggerMessage(EventId = (int)NuGetPullerEventId.NuGetDebug, Level = LogLevel.Trace, Message = "NuGet API Verbose: {Message})")]
    public static partial void NugetVerboseMessage(this ILogger logger, string message);

    [LoggerMessage(EventId = (int)NuGetPullerEventId.NuGetDebug, Level = LogLevel.Trace, Message = "NuGet API Verbose {NugetID} ({Code}): {Message}")]
    private static partial void NugetVerboseCode(ILogger logger, string nuGetId, int code, string message);

    /// <summary>
    /// Logs a <see cref="NuGet.Common.LogLevel.Verbose"/> <see cref="NuGetPullerEventId.NuGetDebug"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    /// <param name="code">The NuGet log code.</param>
    public static void NugetVerboseMessage(this ILogger logger, string message, NuGet.Common.NuGetLogCode code) =>
        NugetVerboseCode(logger, code.ToString("F"), (int)code, message);

    #endregion

    #region NugetMessage Logger Event Methods

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.NugetMessage"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    [LoggerMessage(EventId = (int)NuGetPullerEventId.NugetMessage, Level = LogLevel.Information, Message = "NuGet API Information: {Message}")]
    public static partial void NugetInformationMessage(this ILogger logger, string message);

    [LoggerMessage(EventId = (int)NuGetPullerEventId.NugetMessage, Level = LogLevel.Information, Message = "NuGet API Information {NugetID} ({Code}): {Message}")]
    private static partial void NugetInformationCode(ILogger logger, string nuGetId, int code, string message);

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.NugetMessage"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    /// <param name="code">The NuGet log code.</param>
    public static void NugetInformationMessage(this ILogger logger, string message, NuGet.Common.NuGetLogCode code) =>
        NugetInformationCode(logger, code.ToString("F"), (int)code, message);

    /// <summary>
    /// Logs a <see cref="NuGet.Common.LogLevel.Minimal"/> <see cref="NuGetPullerEventId.NugetMessage"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    [LoggerMessage(EventId = (int)NuGetPullerEventId.NugetMessage, Level = LogLevel.Information, Message = "NuGet API Minimal: {Message}")]
    public static partial void NugetMinimalMessage(this ILogger logger, string message);

    [LoggerMessage(EventId = (int)NuGetPullerEventId.NugetMessage, Level = LogLevel.Information, Message = "NuGet API Minimal {NugetID} ({Code}): {Message}")]
    private static partial void NugetMinimalCode(ILogger logger, string nuGetId, int code, string message);

    /// <summary>
    /// Logs a <see cref="NuGet.Common.LogLevel.Minimal"/> <see cref="NuGetPullerEventId.NugetMessage"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    /// <param name="code">The NuGet log code.</param>
    public static void NugetMinimalMessage(this ILogger logger, string message, NuGet.Common.NuGetLogCode code) =>
        NugetMinimalCode(logger, code.ToString("F"), (int)code, message);

    #endregion

    #region NugetWarning Logger Event Methods

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.NugetWarning"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet warning message.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)NuGetPullerEventId.NugetWarning, Level = LogLevel.Warning, Message = "NuGet API Warning: {Message}")]
    public static partial void NugetWarningMessage(this ILogger logger, string message, Exception? exception = null);

    [LoggerMessage(EventId = (int)NuGetPullerEventId.NugetWarning, Level = LogLevel.Warning, Message = "NuGet API Warning {NugetID} ({Code}): {Message}")]
    private static partial void NugetWarningCode(ILogger logger, string nuGetId, int code, string message, Exception? exception);

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.NugetWarning"/> event message relayed from the NuGet API.
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
    /// Logs a <see cref="NuGetPullerEventId.NuGetError"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet error message.</param>
    /// <param name="code">The NuGet log code.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)NuGetPullerEventId.NuGetError, Level = LogLevel.Error, Message = "NuGet API Error: {Message}")]
    public static partial void NuGetErrorMessage(this ILogger logger, string message, Exception? error = null);

    [LoggerMessage(EventId = (int)NuGetPullerEventId.NuGetError, Level = LogLevel.Error, Message = "NuGet API Error {NugetID} ({Code}): {Message}")]
    private static partial void NuGetErrorCode(ILogger logger, string nuGetId, int code, string message, Exception? error);

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.NuGetError"/> event message relayed from the NuGet API.
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
    /// Logs a <see cref="NuGetPullerEventId.CriticalNugetError"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet error message.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)NuGetPullerEventId.CriticalNugetError, Level = LogLevel.Critical, Message = "NuGet API Critical: {Message}")]
    public static partial void CriticalNugetErrorMessage(this ILogger logger, string message, Exception? error = null);

    [LoggerMessage(EventId = (int)NuGetPullerEventId.CriticalNugetError, Level = LogLevel.Critical, Message = "NuGet API Critical {NugetID} ({Code}): {Message}")]
    private static partial void CriticalNugetErrorCode(ILogger logger, string nuGetId, int code, string message, Exception? error);

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.CriticalNugetError"/> event message relayed from the NuGet API.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet error message.</param>
    /// <param name="code">The NuGet log code.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void CriticalNugetErrorMessage(this ILogger logger, string message, NuGet.Common.NuGetLogCode code, Exception? error = null) =>
        CriticalNugetErrorCode(logger, code.ToString("F"), (int)code, message, error);

    #endregion

    #endregion

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.Cri"/> <see cref="LogLevel.Debug"/> message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="myParam">Dooha</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)NuGetPullerEventId.CriticalNugetError, Level = LogLevel.Debug,
        Message = "Message (MyParam={MyParam})")]
    public static partial void Cri(this ILogger logger, string myParam, Exception? exception = null);

    #region InvalidRepositoryUrl Logger Event Methods

    private const string MESSAGE_UpstreamRepositoryPathTooLong = "Upstream NuGet repository path is too long";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{MESSAGE_UpstreamRepositoryPathTooLong} ({{Path}}).")]
    private static partial void UpstreamRepositoryPathTooLong(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_LocalRepositoryPathTooLong = "Local NuGet repository path is too long";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{MESSAGE_LocalRepositoryPathTooLong} ({{Path}}).")]
    private static partial void LocalRepositoryPathTooLong(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_InvalidUpstreamRepositoryUrl = "Upstream NuGet repository URL is invalid";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{MESSAGE_InvalidUpstreamRepositoryUrl} ({{URL}}).")]
    private static partial void InvalidUpstreamRepositoryUrl(ILogger logger, string url, Exception? exception);

    private const string MESSAGE_InvalidLocalRepositoryUrl = "Local NuGet repository URL is invalid";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{MESSAGE_InvalidLocalRepositoryUrl} ({{URL}}).")]
    private static partial void InvalidLocalRepositoryUrl(ILogger logger, string url, Exception? exception);

    private const string MESSAGE_LocalRepositoryUrlIsNotLocal = "Local NuGet repository URL does not reference a local path";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{MESSAGE_LocalRepositoryUrlIsNotLocal} ({{URL}}).")]
    private static partial void LocalRepositoryUrlIsNotLocal(ILogger logger, string url, Exception? exception);

    private const string MESSAGE_UpstreamRepositoryUrlIsNotAbsolute = "Upstream NuGet repository URL cannot be relative";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{MESSAGE_UpstreamRepositoryUrlIsNotAbsolute} ({{URL}}).")]
    private static partial void UpstreamRepositoryUrlIsNotAbsolute(ILogger logger, string url, Exception? exception);

    private const string MESSAGE_UnsupportedUpstreamRepositoryUrlScheme = "Invalid scheme or Upstream NuGet repository URL";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{MESSAGE_UnsupportedUpstreamRepositoryUrlScheme} ({{URL}}).")]
    private static partial void UnsupportedUpstreamRepositoryUrlScheme(ILogger logger, string url, Exception? exception);

    private const string MESSAGE_UnsupportedLocalRepositoryUrlScheme = "Invalid scheme or Local NuGet repository URL";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidRepositoryUrl, Level = LogLevel.Critical, Message = $"{MESSAGE_UnsupportedLocalRepositoryUrlScheme} ({{URL}}).")]
    private static partial void UnsupportedLocalRepositoryUrlScheme(ILogger logger, string url, Exception? exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.InvalidRepositoryUrl"/> event error message for an invalid repository URL or path.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The invalid NuGet repository URL.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory URL.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogInvalidRepositoryUrl(this ILogger logger, string url, bool isUpstream, Exception? exception = null)
    {
        if (exception is PathTooLongException)
        {
            if (isUpstream)
            {
                UpstreamRepositoryPathTooLong(logger, url, exception);
                return $"{MESSAGE_UpstreamRepositoryPathTooLong}.";
            }
            LocalRepositoryPathTooLong(logger, url, exception);
            return $"{MESSAGE_LocalRepositoryPathTooLong}.";
        }
        if (isUpstream)
        {
            InvalidUpstreamRepositoryUrl(logger, url, exception);
            return $"{MESSAGE_InvalidUpstreamRepositoryUrl}.";
        }
        InvalidLocalRepositoryUrl(logger, url, exception);
        return $"{MESSAGE_InvalidLocalRepositoryUrl}.";
    }

    public static T LogInvalidRepositoryUrl<T>(this ILogger logger, string url, bool isUpstream, Func<string, T> factory, Exception? exception = null)
        where T : LoggedException
    {
        if (exception is PathTooLongException)
        {
            if (isUpstream)
            {
                UpstreamRepositoryPathTooLong(logger, url, exception);
                return factory(MESSAGE_UpstreamRepositoryPathTooLong);
            }
            LocalRepositoryPathTooLong(logger, url, exception);
            return factory(MESSAGE_LocalRepositoryPathTooLong);
        }
        if (isUpstream)
        {
            InvalidUpstreamRepositoryUrl(logger, url, exception);
            return factory(MESSAGE_InvalidUpstreamRepositoryUrl);
        }
        InvalidLocalRepositoryUrl(logger, url, exception);
        return factory(MESSAGE_InvalidLocalRepositoryUrl);
    }

    public static string LogInvalidRepositoryUrl(this ILogger logger, Uri url, bool isUpstream, Exception? exception = null)
    {
        if (url.IsAbsoluteUri)
        {
            if (isUpstream)
            {
                InvalidUpstreamRepositoryUrl(logger, url.OriginalString, exception);
                return $"{MESSAGE_InvalidUpstreamRepositoryUrl}.";
            }
            if (!url.IsFile)
            {
                LocalRepositoryUrlIsNotLocal(logger, url.OriginalString, exception);
                return $"{MESSAGE_LocalRepositoryUrlIsNotLocal}.";
            }
        }
        else if (isUpstream)
        {
            UpstreamRepositoryUrlIsNotAbsolute(logger, url.OriginalString, exception);
            return $"{MESSAGE_UpstreamRepositoryUrlIsNotAbsolute}.";
        }

        InvalidLocalRepositoryUrl(logger, url.OriginalString, exception);
        return $"{MESSAGE_InvalidLocalRepositoryUrl}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.UrlSchemeNotSupported"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uriString">The invalid URI.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T LogUnsupportedRepositoryUrlScheme<T>(this ILogger logger, string uriString, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        UnsupportedUpstreamRepositoryUrlScheme(logger, uriString, exception);
        return factory(MESSAGE_UnsupportedUpstreamRepositoryUrlScheme);
    }

    public static string LogUnsupportedRepositoryUrlScheme(this ILogger logger, string uriString, bool isUpstream, Exception? exception = null)
    {
        if (isUpstream)
        {
            UnsupportedUpstreamRepositoryUrlScheme(logger, uriString, exception);
            return $"{MESSAGE_UnsupportedUpstreamRepositoryUrlScheme}.";
        }
        UnsupportedLocalRepositoryUrlScheme(logger, uriString, exception);
        return $"{MESSAGE_UnsupportedLocalRepositoryUrlScheme}.";
    }

    #endregion

    #region RepositorySecurityException Logger Event Methods

    private const string MESSAGE_UpstreamRepositorySecurityException = "Access denied while accessing upstream NuGet repository path";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.RepositorySecurityException, Level = LogLevel.Critical, Message = $"{MESSAGE_UpstreamRepositorySecurityException} ({{Path}}).")]
    private static partial void UpstreamRepositorySecurityException(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_LocalRepositorySecurityException = "Access denied while accessing local NuGet repository path";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.RepositorySecurityException, Level = LogLevel.Critical, Message = $"{MESSAGE_LocalRepositorySecurityException} ({{Path}}).")]
    private static partial void LocalRepositorySecurityException(ILogger logger, string path, Exception? exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.RepositorySecurityException"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogRepositorySecurityException(this ILogger logger, string path, bool isUpstream, Exception? exception = null)
    {
        if (isUpstream)
        {
            UpstreamRepositorySecurityException(logger, path, exception);
            return $"{MESSAGE_UpstreamRepositorySecurityException}.";
        }
        LocalRepositorySecurityException(logger, path, exception);
        return $"{MESSAGE_LocalRepositorySecurityException}.";
    }

    #endregion

    #region LocalRepositoryIOException Logger Event Methods

    private const string MESSAGE_LocalRepositoryIOException = "I/O error while creating local repository folder";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.LocalRepositoryIOException, Level = LogLevel.Critical, Message = $"{MESSAGE_LocalRepositoryIOException} {{Path}}.")]
    private static partial void LocalRepositoryIOExceptionPrivate(this ILogger logger, string path, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.LocalRepositoryIOException"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The local repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogLocalRepositoryIOException(this ILogger logger, string path, Exception? exception = null)
    {
        LocalRepositoryIOExceptionPrivate(logger, path, exception);
        return $"{MESSAGE_LocalRepositoryIOException}.";
    }

    #endregion

    #region RepositoryPathNotFound Logger Event Methods

    private const string MESSAGE_UpstreamRepositoryPathNotFound = "Upstream repository path not found";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.RepositoryPathNotFound, Level = LogLevel.Critical, Message = $"{MESSAGE_UpstreamRepositoryPathNotFound} ({{Path}})")]
    private static partial void UpstreamRepositoryPathNotFound(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_LocalRepositoryPathNotFound = "Local repository path not found";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.RepositoryPathNotFound, Level = LogLevel.Critical, Message = $"{MESSAGE_LocalRepositoryPathNotFound} ({{Path}})")]
    private static partial void LocalRepositoryPathNotFound(ILogger logger, string path, Exception? exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.RepositoryPathNotFound"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogRepositoryPathNotFound(this ILogger logger, string path, bool isUpstream, Exception? exception = null)
    {
        if (isUpstream)
        {
            UpstreamRepositoryPathNotFound(logger, path, exception);
            return $"{MESSAGE_UpstreamRepositoryPathNotFound}.";
        }
        LocalRepositoryPathNotFound(logger, path, exception);
        return $"{MESSAGE_LocalRepositoryPathNotFound}.";
    }

    public static T LogRepositoryPathNotFound<T>(this ILogger logger, string path, bool isUpstream, Func<string, T> factory, Exception? exception = null)
        where T : LoggedException
    {
        if (isUpstream)
        {
            UpstreamRepositoryPathNotFound(logger, path, exception);
            return factory(MESSAGE_UpstreamRepositoryPathNotFound);
        }
        LocalRepositoryPathNotFound(logger, path, exception);
        return factory(MESSAGE_LocalRepositoryPathNotFound);
    }

    #endregion

    #region InvalidExportLocalMetaData Logger Event Methods

    private const string MESSAGE_ExportLocalMetaDataDirectoryNotFound = "Parent subdirectory of package metadata export path not found";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidExportLocalMetaData, Level = LogLevel.Critical, Message = $"{MESSAGE_ExportLocalMetaDataDirectoryNotFound} ({{Path}})")]
    private static partial void ExportLocalMetaDataDirectoryNotFound(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_ExportLocalMetaDataPathTooLong = "Package metadata export path is too long";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidExportLocalMetaData, Level = LogLevel.Critical, Message = $"{MESSAGE_ExportLocalMetaDataPathTooLong} ({{Path}})")]
    private static partial void ExportLocalMetaDataPathTooLong(ILogger logger, string path, Exception exception);

    private const string MESSAGE_InvalidExportLocalMetaData = "Package metadata export path is invalid";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidExportLocalMetaData, Level = LogLevel.Critical, Message = $"{MESSAGE_InvalidExportLocalMetaData} ({{Path}})")]
    private static partial void InvalidExportLocalMetaData(ILogger logger, string path, Exception? exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.InvalidMetaDataExportPath"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The package metadata export path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T LogInvalidExportLocalMetaData<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        if (exception is not null)
        {
            if (exception is DirectoryNotFoundException)
            {
                ExportLocalMetaDataDirectoryNotFound(logger, path, exception);
                return factory(MESSAGE_ExportLocalMetaDataDirectoryNotFound);
            }
            if (exception is PathTooLongException)
            {
                ExportLocalMetaDataPathTooLong(logger, path, exception);
                return factory(MESSAGE_ExportLocalMetaDataPathTooLong);
            }
        }
        InvalidExportLocalMetaData(logger, path, exception);
        return factory(MESSAGE_InvalidExportLocalMetaData);
    }

    /// <summary>
    /// </summary>
    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.InvalidExportLocalMetaData"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The invalid NuGet repository URL.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogInvalidExportLocalMetaData(this ILogger logger, string url, Exception? exception = null)
    {
        if (exception is PathTooLongException)
        {
            ExportLocalMetaDataPathTooLong(logger, url, exception);
            return $"{MESSAGE_ExportLocalMetaDataPathTooLong}.";
        }
        InvalidExportLocalMetaData(logger, url, exception);
        return $"{MESSAGE_InvalidExportLocalMetaData}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.InvalidExportLocalMetaData"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogExportLocalMetaDataDirectoryNotFound(this ILogger logger, string path, Exception? exception = null)
    {
        ExportLocalMetaDataDirectoryNotFound(logger, path, exception);
        return $"{MESSAGE_ExportLocalMetaDataDirectoryNotFound}.";
    }

    public static T LogExportLocalMetaDataDirectoryNotFound<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        ExportLocalMetaDataDirectoryNotFound(logger, path, exception);
        return factory(MESSAGE_ExportLocalMetaDataDirectoryNotFound);
    }

    #endregion

    #region MetaDataExportPathAccessDenied Logger Event Methods

    private const string MESSAGE_MetaDataExportPathAccessDenied = "Access to package metadata export path is denied";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.MetaDataExportPathAccessDenied, Level = LogLevel.Critical, EventName = nameof(MetaDataExportPathAccessDenied), Message = $"{MESSAGE_MetaDataExportPathAccessDenied} ({{Path}})")]
    private static partial void LogMetaDataExportPathAccessDenied(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_InsufficientPermissionsForMetaDataExportPath = "Caller has insufficient permissions to package metadata export path";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.MetaDataExportPathAccessDenied, Level = LogLevel.Critical, Message = $"{MESSAGE_InsufficientPermissionsForMetaDataExportPath} ({{Path}})")]
    private static partial void InsufficientPermissionsForMetaDataExportPath(ILogger logger, string path, Exception exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.MetaDataExportPathAccessDenied"/> event error message.
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
            return factory(MESSAGE_InsufficientPermissionsForMetaDataExportPath);
        }
        LogMetaDataExportPathAccessDenied(logger, path, exception);
        return factory(MESSAGE_MetaDataExportPathAccessDenied);
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.MetaDataExportPathAccessDenied"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string MetaDataExportPathAccessDenied(this ILogger logger, string path, Exception? exception = null)
    {
        if (exception is System.Security.SecurityException)
        {
            InsufficientPermissionsForMetaDataExportPath(logger, path, exception);
            return $"{MESSAGE_InsufficientPermissionsForMetaDataExportPath}.";
        }
        LogMetaDataExportPathAccessDenied(logger, path, exception);
        return $"{MESSAGE_MetaDataExportPathAccessDenied}.";
    }

    #endregion

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.PackageDeleted"/> <see cref="LogLevel.Warning"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the package that was deleted.</param>
    /// <param name="repositoryPath">The path of the local NuGet repository.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)NuGetPullerEventId.PackageDeleted, Level = LogLevel.Warning, Message = "Package {PackageId} has been deleted from local repository ({RepositoryPath}).")]
    public static partial void PackageDeleted(this ILogger logger, string packageId, string repositoryPath, Exception? exception = null);

    #region PackageNotFound Logger Event Methods

    [LoggerMessage(EventId = (int)NuGetPullerEventId.PackageNotFound, Level = LogLevel.Warning, Message = "Package {PackageId} not found in upstream NuGet source ({RepositoryPath}).")]
    private static partial void UpstreamDirPackageNotFound(this ILogger logger, string packageId, string repositoryPath, Exception? exception);

    [LoggerMessage(EventId = (int)NuGetPullerEventId.PackageNotFound, Level = LogLevel.Warning, Message = "Package {PackageId} not found in local NuGet source ({RepositoryPath}).")]
    private static partial void LocalPackageNotFound(this ILogger logger, string packageId, string repositoryPath, Exception? exception);

    [LoggerMessage(EventId = (int)NuGetPullerEventId.PackageNotFound, Level = LogLevel.Warning, Message = "Package {PackageId} not found in upstream NuGet source ({URL}).")]
    private static partial void RemotePackageNotFound(this ILogger logger, string packageId, string url, Exception? exception);

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.PackageNotFound"/> <see cref="LogLevel.Warning"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the package that was not found.</param>
    /// <param name="clientService">The client service.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void PackageNotFound(this ILogger logger, string packageId, IClientService clientService, Exception? exception = null)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                UpstreamDirPackageNotFound(logger, packageId, clientService.PackageSourceLocation, exception);
            else
                LocalPackageNotFound(logger, packageId, clientService.PackageSourceLocation, exception);
        }
        else
            RemotePackageNotFound(logger, packageId, clientService.PackageSourceLocation, exception);
    }


    [LoggerMessage(EventId = (int)NuGetPullerEventId.PackageNotFound, Level = LogLevel.Warning,
        Message = "Version {Version} of package {PackageId} not found in upstream NuGet source ({URL}).")]
    private static partial void RemotePackageVersionNotFound(ILogger logger, string packageId, NuGetVersion version, string url, Exception? exception);

    [LoggerMessage(EventId = (int)NuGetPullerEventId.PackageNotFound, Level = LogLevel.Warning,
        Message = "Version {Version} of package {PackageId} not found in local NuGet source ({PackageSourceLocation}).")]
    private static partial void LocalPackageVersionNotFound(ILogger logger, string packageId, NuGetVersion version, string packageSourceLocation, Exception? exception);

    [LoggerMessage(EventId = (int)NuGetPullerEventId.PackageNotFound, Level = LogLevel.Warning,
        Message = "Version {Version} of package {PackageId} not found in upstream NuGet source ({PackageSourceLocation}).")]
    private static partial void UstreamDirPackageVersionNotFound(ILogger logger, NuGetVersion version, string packageId, string packageSourceLocation, Exception? exception);

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.PackageNotFound"/> <see cref="LogLevel.Warning"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the package that was not found.</param>
    /// <param name="version">The version of the package that was not found.</param>
    /// <param name="clientService">The client service.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void PackageNotFound(this ILogger logger, string packageId, NuGetVersion version, IClientService clientService, Exception? exception = null)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                UstreamDirPackageVersionNotFound(logger, version, packageId, clientService.PackageSourceLocation, exception);
            else
                LocalPackageVersionNotFound(logger, packageId, version, clientService.PackageSourceLocation, exception);
        }
        else
            RemotePackageVersionNotFound(logger, packageId, version, clientService.PackageSourceLocation, exception);
    }

    #endregion

    #region GlobalPackagesFolderNotFound Logger Event Methods

    private const string MESSAGE_GlobalPackagesFolderNotFound = "Global packages folder not found";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.GlobalPackagesFolderNotFound, EventName = nameof(GlobalPackagesFolderNotFound), Level = LogLevel.Critical,
        Message = $"{MESSAGE_GlobalPackagesFolderNotFound} ({{Path}}).")]
    private static partial void LogGlobalPackagesFolderNotFound(ILogger logger, string path, Exception? exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.GlobalPackagesFolderNotFound"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The Global packages folder path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string GlobalPackagesFolderNotFound(this ILogger logger, string path, Exception? exception = null)
    {
        LogGlobalPackagesFolderNotFound(logger, path, exception);
        return $"{MESSAGE_GlobalPackagesFolderNotFound}.";
    }

    public static T GlobalPackagesFolderNotFound<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        LogGlobalPackagesFolderNotFound(logger, path, exception);
        return factory(MESSAGE_GlobalPackagesFolderNotFound);
    }

    #endregion

    #region GlobalPackagesFolderSecurityException Logger Event Methods

    private const string MESSAGE_GlobalPackagesFolderSecurityException = "Access denied while accessing global packages folder";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.GlobalPackagesFolderSecurityException, Level = LogLevel.Critical, EventName = nameof(GlobalPackagesFolderSecurityException), Message = $"{MESSAGE_GlobalPackagesFolderSecurityException} ({{Path}}).")]
    private static partial void LogGlobalPackagesFolderSecurityException(ILogger logger, string path, Exception? exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.GlobalPackagesFolderSecurityException"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The Global packages folder path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string GlobalPackagesFolderSecurityException(this ILogger logger, string path, Exception? exception = null)
    {
        LogGlobalPackagesFolderSecurityException(logger, path, exception);
        return $"{MESSAGE_GlobalPackagesFolderSecurityException}.";
    }

    #endregion

    #region InvalidGlobalPackagesFolder Logger Event Methods

    private const string MESSAGE_GlobalPackagesFolderPathTooLong = "NuGet Global Packages Folder path is too long";


    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidGlobalPackagesFolder, Level = LogLevel.Critical, Message = $"{MESSAGE_GlobalPackagesFolderPathTooLong} ({{Path}}).")]
    private static partial void GlobalPackagesFolderPathTooLong(ILogger logger, string path, Exception exception);

    private const string MESSAGE_InvalidGlobalPackagesFolder = "NuGet Global Packages Folder path is invalid";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidGlobalPackagesFolder, Level = LogLevel.Critical, EventName = nameof(InvalidGlobalPackagesFolder), Message = $"{MESSAGE_InvalidGlobalPackagesFolder} ({{Path}}).")]
    private static partial void LogInvalidGlobalPackagesFolder(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_GlobalPackagesFolderNotFileUri = "NuGet Global Packages Folder must refer to a filesystem subdirectory";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidGlobalPackagesFolder, Level = LogLevel.Critical, EventName = nameof(GlobalPackagesFolderNotFileUri), Message = $"{MESSAGE_GlobalPackagesFolderNotFileUri} ({{URL}}).")]
    private static partial void LogGlobalPackagesFolderNotFileUri(ILogger logger, string url, Exception? exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.InvalidGlobalPackagesFolder"/> event error message.
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
            return $"{MESSAGE_GlobalPackagesFolderPathTooLong}.";
        }
        LogInvalidGlobalPackagesFolder(logger, path, exception);
        return $"{MESSAGE_InvalidGlobalPackagesFolder}.";
    }

    public static T InvalidGlobalPackagesFolder<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        if (exception is PathTooLongException)
        {
            GlobalPackagesFolderPathTooLong(logger, path, exception);
            return factory(MESSAGE_GlobalPackagesFolderPathTooLong);
        }
        LogInvalidGlobalPackagesFolder(logger, path, exception);
        return factory(MESSAGE_InvalidGlobalPackagesFolder);
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.InvalidGlobalPackagesFolder"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The invalid global packages folder url.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string GlobalPackagesFolderNotFileUri(this ILogger logger, string url, Exception? exception = null)
    {
        LogGlobalPackagesFolderNotFileUri(logger, url, exception);
        return $"{MESSAGE_GlobalPackagesFolderNotFileUri}.";
    }

    #endregion

    #region MultipleSettingsWithSameRepositoryLocation Logger Event Methods

    private const string MESSAGE_LocalSameAsUpstreamNugetRepository = "Local NuGet repository path cannot be the same as the upstream NuGet repository path";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.MultipleSettingsWithSameRepositoryLocation, Level = LogLevel.Critical, EventName = nameof(LocalSameAsUpstreamNugetRepository), Message = $"{MESSAGE_LocalSameAsUpstreamNugetRepository} ({{Path}}).")]
    private static partial void LogLocalSameAsUpstreamNugetRepository(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_LocalRepositorySameAsGlobalPackagesFolder = "Local NuGet repository path cannot be the same as the upstream NuGet repository path";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.MultipleSettingsWithSameRepositoryLocation, Level = LogLevel.Critical, EventName = nameof(LocalRepositorySameAsGlobalPackagesFolder), Message = $"{MESSAGE_LocalRepositorySameAsGlobalPackagesFolder} ({{Path}}).")]
    private static partial void LogLocalRepositorySameAsGlobalPackagesFolder(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_UpstreamRepositorySameAsGlobalPackagesFolder = "Local NuGet repository path cannot be the same as the upstream NuGet repository path";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.MultipleSettingsWithSameRepositoryLocation, Level = LogLevel.Critical, EventName = nameof(UpstreamRepositorySameAsGlobalPackagesFolder), Message = $"{MESSAGE_UpstreamRepositorySameAsGlobalPackagesFolder} ({{Path}}).")]
    private static partial void LogUpstreamRepositorySameAsGlobalPackagesFolder(ILogger logger, string path, Exception? exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.MultipleSettingsWithSameRepositoryLocation"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static string LocalSameAsUpstreamNugetRepository(this ILogger logger, string path, Exception? exception = null)
    {
        LogLocalSameAsUpstreamNugetRepository(logger, path, exception);
        return $"{MESSAGE_LocalSameAsUpstreamNugetRepository}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.MultipleSettingsWithSameRepositoryLocation"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static string LocalRepositorySameAsGlobalPackagesFolder(this ILogger logger, string path, Exception? exception = null)
    {
        LogLocalRepositorySameAsGlobalPackagesFolder(logger, path, exception);
        return $"{MESSAGE_LocalRepositorySameAsGlobalPackagesFolder}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.MultipleSettingsWithSameRepositoryLocation"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static string UpstreamRepositorySameAsGlobalPackagesFolder(this ILogger logger, string path, Exception? exception = null)
    {
        LogUpstreamRepositorySameAsGlobalPackagesFolder(logger, path, exception);
        return $"{MESSAGE_UpstreamRepositorySameAsGlobalPackagesFolder}.";
    }

    #endregion

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.PackageFileNotFound"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="fileName">The path of the file that was not found.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)NuGetPullerEventId.PackageFileNotFound, Level = LogLevel.Error, Message = "Package file not found: {FileName}")]
    public static partial void PackageFileNotFound(this ILogger logger, string fileName, Exception? exception = null);

    #region InvalidPackageFile Logger Event Methods

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.InvalidPackageFile"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="fileName">The path of the invalid package file.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidPackageFile, Level = LogLevel.Error, Message = "Package file is not a ZIP archive: {FileName}")]
    public static partial void PackageFileNotZipArchive(this ILogger logger, string fileName, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.InvalidPackageFile"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="fileName">The path of the invalid package file.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidPackageFile, Level = LogLevel.Error, Message = "Package file has invalid content: {FileName}")]
    public static partial void InvalidPackageFileContent(this ILogger logger, string fileName, Exception? exception = null);

    #endregion

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.PackageAlreadyAdded"/> <see cref="LogLevel.Warning"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The identifier of the existing package.</param>
    /// <param name="exception">The optional exception that caused the event.</param>;
    [LoggerMessage(EventId = (int)NuGetPullerEventId.PackageAlreadyAdded, Level = LogLevel.Warning, Message = "Package {PackageId} has already been added.")]
    public static partial void PackageAlreadyAdded(this ILogger logger, string packageId, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.PackageVersionDeleteFailure"/> <see cref="LogLevel.Warning"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)NuGetPullerEventId.PackageVersionDeleteFailure, Level = LogLevel.Warning, Message = "Unexpected error deleting Package {PackageId}, Version {Version}.")]
    public static partial void PackageVersionDeleteFailure(this ILogger logger, string packageId, NuGetVersion version, Exception? exception = null);

    #region UnexpectedPackageDownloadFailure Logger Event Methods

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.UnexpectedPackageDownloadFailure"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="exception">The exception that caused the event.</param>
    [LoggerMessage(EventId = (int)NuGetPullerEventId.UnexpectedPackageDownloadFailure, Level = LogLevel.Error, Message = "Unexpected error while downloading package {PackageId}, Version {Version}.")]
    public static partial void UnexpectedPackageDownloadFailure(this ILogger logger, string packageId, NuGetVersion version, Exception exception);

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.UnexpectedPackageDownloadFailure"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    [LoggerMessage(EventId = (int)NuGetPullerEventId.UnexpectedPackageDownloadFailure, Level = LogLevel.Error, Message = "Package download of {PackageId}, Version {Version} was unexpectedly empty.")]
    public static partial void DownloadPackageIsEmpty(this ILogger logger, string packageId, NuGetVersion version);

    #endregion

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.UnexpectedAddFailure"/> <see cref="LogLevel.Error"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)NuGetPullerEventId.UnexpectedAddFailure, Level = LogLevel.Error, Message = "Unexpected error while adding Package {PackageId}, Version {Version}.")]
    public static partial void UnexpectedAddPackageFailure(this ILogger logger, string packageId, NuGetVersion version, Exception? exception = null);

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.NoLocalPackagesExist"/> <see cref="LogLevel.Warning"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    [LoggerMessage(EventId = (int)NuGetPullerEventId.NoLocalPackagesExist, Level = LogLevel.Warning, Message = "Local repository has no packages.")]
    public static partial void NoLocalPackagesExist(this ILogger logger, Exception? exception = null);

    #region InvalidExportBundle Logger Event Methods

    private const string MESSAGE_InvalidExportBundle = "Package metadata export path is invalid";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidExportBundle, Level = LogLevel.Critical, EventName = nameof(InvalidExportBundle), Message = $"{MESSAGE_InvalidExportBundle} ({{Path}}).")]
    private static partial void LogInvalidExportBundle(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_ExportBundleDirectoryNotFound = "Parent subdirectory of package metadata export path not found";


    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidExportBundle, Level = LogLevel.Critical, Message = $"{MESSAGE_ExportBundleDirectoryNotFound} ({{Path}}).")]
    private static partial void ExportBundleDirectoryNotFound(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_ExportBundlePathTooLong = "Package metadata export path is too long";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidExportBundle, Level = LogLevel.Critical, Message = $"{MESSAGE_ExportBundlePathTooLong} ({{Path}}).")]
    private static partial void ExportBundlePathTooLong(ILogger logger, string path, Exception exception);

    private const string MESSAGE_ExportBundlePathAccessDenied = "Access to export bundle path is denied";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidExportBundle, Level = LogLevel.Critical, Message = $"{MESSAGE_ExportBundlePathAccessDenied} ({{Path}}).")]
    private static partial void ExportBundlePathAccessDenied(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_InsufficientPermissionsForExportBundlePath = "Caller has insufficient permissions to export bundle path";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidExportBundle, Level = LogLevel.Critical, Message = $"{MESSAGE_InsufficientPermissionsForExportBundlePath} ({{Path}}).")]
    private static partial void InsufficientPermissionsForExportBundlePath(ILogger logger, string path, Exception exception);

    /// <summary>
    /// </summary>
    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.InvalidExportBundle"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The invalid NuGet repository URL.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string InvalidExportBundle(this ILogger logger, string url, Exception? exception = null)
    {
        if (exception is PathTooLongException)
        {
            ExportBundlePathTooLong(logger, url, exception);
            return $"{MESSAGE_ExportBundlePathTooLong}.";
        }
        LogInvalidExportBundle(logger, url, exception);
        return $"{MESSAGE_InvalidExportBundle}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.InvalidExportBundle"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogExportBundleDirectoryNotFound(this ILogger logger, string path, Exception? exception = null)
    {
        ExportBundleDirectoryNotFound(logger, path, exception);
        return $"{MESSAGE_ExportBundleDirectoryNotFound}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.MetaDataExportPathAccessDenied"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogExportBundlePathAccessDenied(this ILogger logger, string path, Exception? exception = null)
    {
        if (exception is System.Security.SecurityException)
        {
            InsufficientPermissionsForExportBundlePath(logger, path, exception);
            return $"{MESSAGE_InsufficientPermissionsForExportBundlePath}.";
        }
        ExportBundlePathAccessDenied(logger, path, exception);
        return $"{MESSAGE_ExportBundlePathAccessDenied}.";
    }

    #endregion

    #region InvalidTargetManifestFile Logger Event Methods

    private const string MESSAGE_TargetManifestFilePathTooLong = "Package metadata export path is too long";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidTargetManifestFile, Level = LogLevel.Critical, Message = $"{MESSAGE_TargetManifestFilePathTooLong} ({{Path}}).")]
    private static partial void TargetManifestFilePathTooLong(ILogger logger, string path, Exception exception);

    private const string MESSAGE_InvalidTargetManifestFile = "Package metadata export path is invalid";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidTargetManifestFile, Level = LogLevel.Critical, EventName = nameof(InvalidTargetManifestFile), Message = $"{MESSAGE_InvalidTargetManifestFile} ({{Path}}).")]
    private static partial void LogInvalidTargetManifestFile(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_TargetManifestFileDirectoryNotFound = "Parent subdirectory of package metadata export path not found";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidTargetManifestFile, Level = LogLevel.Critical, EventName = nameof(TargetManifestFileDirectoryNotFound), Message = $"{MESSAGE_TargetManifestFileDirectoryNotFound} ({{Path}}).")]
    private static partial void LogTargetManifestFileDirectoryNotFound(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_InsufficientPermissionsForTargetManifestFilePath = "Caller has insufficient permissions to access the target manifest file path";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidTargetManifestFile, Level = LogLevel.Critical, Message = $"{MESSAGE_InsufficientPermissionsForTargetManifestFilePath} ({{Path}}).")]
    private static partial void InsufficientPermissionsForTargetManifestFilePath(ILogger logger, string path, Exception exception);

    private const string MESSAGE_TargetManifestFilePathAccessDenied = "Access to target manifest file path is denied";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidTargetManifestFile, Level = LogLevel.Critical, EventName = nameof(TargetManifestFilePathAccessDenied), Message = $"{MESSAGE_TargetManifestFilePathAccessDenied} ({{Path}}).")]
    private static partial void LogTargetManifestFilePathAccessDenied(ILogger logger, string path, Exception? exception);

    /// <summary>
    /// </summary>
    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.InvalidTargetManifestFile"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The invalid NuGet repository URL.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string InvalidTargetManifestFile(this ILogger logger, string url, Exception? exception = null)
    {
        if (exception is PathTooLongException)
        {
            TargetManifestFilePathTooLong(logger, url, exception);
            return $"{MESSAGE_TargetManifestFilePathTooLong}.";
        }
        LogInvalidTargetManifestFile(logger, url, exception);
        return $"{MESSAGE_InvalidTargetManifestFile}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.InvalidTargetManifestFile"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string TargetManifestFileDirectoryNotFound(this ILogger logger, string path, Exception? exception = null)
    {
        LogTargetManifestFileDirectoryNotFound(logger, path, exception);
        return $"{MESSAGE_TargetManifestFileDirectoryNotFound}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.MetaDataExportPathAccessDenied"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string TargetManifestFilePathAccessDenied(this ILogger logger, string path, Exception? exception = null)
    {
        if (exception is System.Security.SecurityException)
        {
            InsufficientPermissionsForTargetManifestFilePath(logger, path, exception);
            return $"{MESSAGE_InsufficientPermissionsForTargetManifestFilePath}.";
        }
        LogTargetManifestFilePathAccessDenied(logger, path, exception);
        return $"{MESSAGE_TargetManifestFilePathAccessDenied}.";
    }

    #endregion

    #region InvalidSaveTargetManifestAs Logger Event Methods

    private const string MESSAGE_SaveTargetManifestAsPathTooLong = "Package metadata export path is too long";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidSaveTargetManifestAs, Level = LogLevel.Critical, Message = $"{MESSAGE_SaveTargetManifestAsPathTooLong} ({{Path}}).")]
    private static partial void SaveTargetManifestAsPathTooLong(ILogger logger, string path, Exception exception);

    private const string MESSAGE_InvalidSaveTargetManifestAs = "Package metadata export path is invalid";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidSaveTargetManifestAs, Level = LogLevel.Critical, EventName = nameof(InvalidSaveTargetManifestAs), Message = $"{MESSAGE_InvalidSaveTargetManifestAs} ({{Path}}).")]
    private static partial void LogInvalidSaveTargetManifestAs(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_SaveTargetManifestAsDirectoryNotFound = "Parent subdirectory of package metadata export path not found";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidSaveTargetManifestAs, Level = LogLevel.Critical, Message = $"{MESSAGE_SaveTargetManifestAsDirectoryNotFound} ({{Path}}).")]
    private static partial void SaveTargetManifestAsDirectoryNotFound(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_InsufficientPermissionsForSaveTargetManifestAsPath = "Caller has insufficient permissions to access the manifest file output path";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidSaveTargetManifestAs, Level = LogLevel.Critical, Message = $"{MESSAGE_InsufficientPermissionsForSaveTargetManifestAsPath} ({{Path}}).")]
    private static partial void InsufficientPermissionsForSaveTargetManifestAsPath(ILogger logger, string path, Exception exception);

    private const string MESSAGE_SaveTargetManifestAsPathAccessDenied = "Access to manifest file output path is denied";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidSaveTargetManifestAs, Level = LogLevel.Critical, Message = $"{MESSAGE_SaveTargetManifestAsPathAccessDenied} ({{Path}}).")]
    private static partial void SaveTargetManifestAsPathAAccessDenied(ILogger logger, string path, Exception? exception);

    /// <summary>
    /// </summary>
    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.InvalidSaveTargetManifestAs"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The invalid NuGet repository URL.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string InvalidSaveTargetManifestAs(this ILogger logger, string url, Exception? exception = null)
    {
        if (exception is PathTooLongException)
        {
            SaveTargetManifestAsPathTooLong(logger, url, exception);
            return $"{MESSAGE_SaveTargetManifestAsPathTooLong}.";
        }
        LogInvalidSaveTargetManifestAs(logger, url, exception);
        return $"{MESSAGE_InvalidSaveTargetManifestAs}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.InvalidSaveTargetManifestAs"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogSaveTargetManifestAsDirectoryNotFound(this ILogger logger, string path, Exception? exception = null)
    {
        SaveTargetManifestAsDirectoryNotFound(logger, path, exception);
        return $"{MESSAGE_SaveTargetManifestAsDirectoryNotFound}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.MetaDataExportPathAccessDenied"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogSaveTargetManifestAsPathAccessDenied(this ILogger logger, string path, Exception? exception = null)
    {
        if (exception is System.Security.SecurityException)
        {
            InsufficientPermissionsForSaveTargetManifestAsPath(logger, path, exception);
            return $"{MESSAGE_InsufficientPermissionsForSaveTargetManifestAsPath}.";
        }
        SaveTargetManifestAsPathAAccessDenied(logger, path, exception);
        return $"{MESSAGE_SaveTargetManifestAsPathAccessDenied}.";
    }

    #endregion

    #region InvalidImportPath Logger Event Methods
    private const string MESSAGE_ImportPathTooLong = "Package metadata export path is too long";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidImportPath, Level = LogLevel.Critical, Message = $"{MESSAGE_ImportPathTooLong} ({{Path}}).")]
    private static partial void ImportPathTooLong(ILogger logger, string path, Exception exception);

    private const string MESSAGE_InvalidImportPath = "Package metadata export path is invalid";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidImportPath, Level = LogLevel.Critical, EventName = nameof(InvalidImportPath), Message = $"{MESSAGE_InvalidImportPath} ({{Path}}).")]
    private static partial void LogInvalidImportPath(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_ImportDirectoryNotFound = "Parent subdirectory of package metadata export path not found";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidImportPath, Level = LogLevel.Critical, Message = $"{MESSAGE_ImportDirectoryNotFound} ({{Path}}).")]
    private static partial void ImportDirectoryNotFound(ILogger logger, string path, Exception? exception);

    private const string MESSAGE_InsufficientPermissionsForImportPath = "Caller has insufficient permissions to access the manifest file output path";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidImportPath, Level = LogLevel.Critical, Message = $"{MESSAGE_InsufficientPermissionsForImportPath} ({{Path}}).")]
    private static partial void InsufficientPermissionsForImportPath(ILogger logger, string path, Exception exception);

    private const string MESSAGE_ImportPathAccessDenied = "Access to manifest file output path is denied";

    [LoggerMessage(EventId = (int)NuGetPullerEventId.InvalidImportPath, Level = LogLevel.Critical, Message = $"{MESSAGE_ImportPathAccessDenied} ({{Path}}).")]
    private static partial void ImportPathAccessDenied(ILogger logger, string path, Exception? exception);

    /// <summary>
    /// </summary>
    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.InvalidImport"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The invalid NuGet repository URL.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string InvalidImportPath(this ILogger logger, string url, Exception? exception = null)
    {
        if (exception is PathTooLongException)
        {
            ImportPathTooLong(logger, url, exception);
            return $"{MESSAGE_ImportPathTooLong}.";
        }
        LogInvalidImportPath(logger, url, exception);
        return $"{MESSAGE_InvalidImportPath}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.InvalidImport"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogImportFileOrDirectoryNotFound(this ILogger logger, string path, Exception? exception = null)
    {
        ImportDirectoryNotFound(logger, path, exception);
        return $"{MESSAGE_ImportDirectoryNotFound}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> <see cref="NuGetPullerEventId.MetaDataExportPathAccessDenied"/> event error message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogImportPathAccessDenied(this ILogger logger, string path, Exception? exception = null)
    {
        if (exception is System.Security.SecurityException)
        {
            InsufficientPermissionsForImportPath(logger, path, exception);
            return $"{MESSAGE_InsufficientPermissionsForMetaDataExportPath}.";
        }
        ImportPathAccessDenied(logger, path, exception);
        return $"{MESSAGE_MetaDataExportPathAccessDenied}.";
    }

    #endregion

    #region DownloadingNuGetPackage Logger Event Methods

    [LoggerMessage(EventId = (int)NuGetPullerEventId.DownloadingNuGetPackage, Level = LogLevel.Warning, Message = "Downloading package {PackageId}, version {Version} from local {PackageSourceLocation}.")]
    private static partial void DownloadingLocalNuGetPackageVersion(ILogger logger, string packageId, NuGetVersion version, string packageSourceLocation, Exception? exception);

    [LoggerMessage(EventId = (int)NuGetPullerEventId.DownloadingNuGetPackage, Level = LogLevel.Warning, Message = "Downloading package {PackageId} from local {PackageSourceLocation}.")]
    private static partial void DownloadingLocalNuGetPackage(ILogger logger, string packageId, string packageSourceLocation, Exception? exception);

    [LoggerMessage(EventId = (int)NuGetPullerEventId.DownloadingNuGetPackage, Level = LogLevel.Warning, Message = "Downloading package {PackageId}, version {Version} from upstream {PackageSourceUri}.")]
    private static partial void DownloadingRemoteNuGetPackageVersion(ILogger logger, string packageId, NuGetVersion version, Uri packageSourceUri, Exception? exception);

    [LoggerMessage(EventId = (int)NuGetPullerEventId.DownloadingNuGetPackage, Level = LogLevel.Warning, EventName = nameof(DownloadingNuGetPackage), Message = "Downloading package {PackageId} from upstream {PackageSourceUri}.")]
    private static partial void LogDownloadingRemoteNuGetPackage(ILogger logger, string packageId, Uri packageSourceUri, Exception? exception);

    /// <summary>
    /// Logs a <see cref="NuGetPullerEventId.DownloadingNuGetPackage"/> <see cref="LogLevel.Information"/> event message.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="identity">The package identifier and version.</param>
    /// <param name="clientService">The source repository.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void DownloadingNuGetPackage(this ILogger logger, PackageIdentity identity, IClientService clientService, Exception? exception = null)
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


    #region GetDownloadResource Scope

    private static readonly Func<ILogger, string, NuGetVersion?, string, IDisposable?> _getRemoteDownloadResourceResultScope = LoggerMessage.DefineScope<string, NuGetVersion?, string>(
        "Getting Download Resource for package {PackageId}, version {Version} from upstream NuGet repository at {RepositoryUrl}."
    );

    private static readonly Func<ILogger, string, NuGetVersion?, string, IDisposable?> _getUpstreamDirDownloadResourceResultScope = LoggerMessage.DefineScope<string, NuGetVersion?, string>(
        "Getting Download Resource for package {PackageId}, version {Version} from upstream NuGet repository at {Path}."
    );

    private static readonly Func<ILogger, string, NuGetVersion?, string, IDisposable?> _getLocalDownloadResourceResultScope = LoggerMessage.DefineScope<string, NuGetVersion?, string>(
        "Getting Download Resource for package {PackageId}, version {Version} from local NuGet repository at {Path}."
    );

    /// <summary>
    /// Formats the GetDownloadResource message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetDownloadResourceResultScope(this ILogger logger, PackageIdentity identity, IClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _getUpstreamDirDownloadResourceResultScope(logger, identity.Id, identity.HasVersion ? identity.Version : null, clientService.PackageSourceLocation);
            return _getLocalDownloadResourceResultScope(logger, identity.Id, identity.HasVersion ? identity.Version : null, clientService.PackageSourceLocation);
        }
        return _getRemoteDownloadResourceResultScope(logger, identity.Id, identity.HasVersion ? identity.Version : null, clientService.PackageSourceLocation);
    }

    #endregion

    #region DownloadNupkg Scope

    private static readonly Func<ILogger, string, NuGetVersion?, string?, IDisposable?> _downloadRemoteNupkgScope = LoggerMessage.DefineScope<string, NuGetVersion?, string?>(
       "Download NuGet package from upstream (PackageId={PackageId}; Version={Version}; URL={RepositoryUrl}).");

    private static readonly Func<ILogger, string, NuGetVersion?, string, IDisposable?> _downloadUpstreamDirNupkgScope = LoggerMessage.DefineScope<string, NuGetVersion?, string>(
       "Download NuGet package from upstream (PackageId={PackageId}; Version={Version}; Path={RepositoryPath}).");

    private static readonly Func<ILogger, string, NuGetVersion?, string, IDisposable?> _downloadLocalNupkgScope = LoggerMessage.DefineScope<string, NuGetVersion?, string>(
       "Download NuGet package from local (PackageId={PackageId}; Version={Version}; Path={RepositoryPath}).");

    /// <summary>
    /// Formats the DownloadNupkg message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginDownloadNupkgScope(this ILogger logger, string packageId, NuGetVersion version, IClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _downloadUpstreamDirNupkgScope(logger, packageId, version, clientService.PackageSourceLocation);
            return _downloadLocalNupkgScope(logger, packageId, version, clientService.PackageSourceLocation);
        }
        return _downloadRemoteNupkgScope(logger, packageId, version, clientService.PackageSourceLocation);
    }

    #endregion

    #region GetMetadata Scope

    private static readonly Func<ILogger, string, bool, bool, string, IDisposable?> _getRemoteMetadataScope1 = LoggerMessage.DefineScope<string, bool, bool, string>(
       "Get NuGet package metadata from upstream (PackageId={PackageId}; IncludePreRelease={IncludePreRelease}; IncludeUnlisted={IncludeUnlisted}; URL={RepositoryUrl})."
    );

    private static readonly Func<ILogger, string, bool, bool, string, IDisposable?> _getLocalMetadataScope1 = LoggerMessage.DefineScope<string, bool, bool, string>(
       "Get NuGet package metadata from local (PackageId={PackageId}; IncludePreRelease={IncludePreRelease}; IncludeUnlisted={IncludeUnlisted}; Path={RepositoryPath})."
    );

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _getRemoteMetadataScope2 = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get NuGet package metadata from upstream (PackageId={PackageId}; Version={Version}; URL={RepositoryUrl})."
    );

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _getLocalMetadataScope2 = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get NuGet package metadata from local (PackageId={PackageId}; Version={Version}; Path={RepositoryPath})."
    );

    private static readonly Func<ILogger, string, bool, bool, string, IDisposable?> _getUpstreamDirMetadataScope1 = LoggerMessage.DefineScope<string, bool, bool, string>(
       "Get NuGet package metadata from upstream (PackageId={PackageId}; IncludePreRelease={IncludePreRelease}; IncludeUnlisted={IncludeUnlisted}; Path={RepositoryPath})."
    );

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _getUpstreamDirMetadataScope2 = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get NuGet package metadata from upstream (PackageId={PackageId}; Version={Version}; Path={RepositoryPath})."
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
    public static IDisposable? BeginGetMetadataScope(this ILogger logger, string packageId, bool includePreRelease, bool includeUnlisted, IClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _getUpstreamDirMetadataScope1(logger, packageId, includePreRelease, includeUnlisted, clientService.PackageSourceLocation);
            return _getLocalMetadataScope1(logger, packageId, includePreRelease, includeUnlisted, clientService.PackageSourceLocation);
        }
        return _getRemoteMetadataScope1(logger, packageId, includePreRelease, includeUnlisted, clientService.PackageSourceLocation);
    }

    public static IDisposable? BeginGetMetadataScope(this ILogger logger, string packageId, NuGetVersion version, IClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _getUpstreamDirMetadataScope2(logger, packageId, version, clientService.PackageSourceLocation);
            return _getLocalMetadataScope2(logger, packageId, version, clientService.PackageSourceLocation);
        }
        return _getRemoteMetadataScope2(logger, packageId, version, clientService.PackageSourceLocation);
    }

    #endregion

    #region GetAllVersions Scope

    private static readonly Func<ILogger, string, string, IDisposable?> _getAllRemoteVersionsScope = LoggerMessage.DefineScope<string, string>(
       "Get all NuGet package versions from upstream (PackageId={PackageId}; RepositoryUrl={RepositoryUrl})."
    );

    private static readonly Func<ILogger, string, string, IDisposable?> _getAllUpstreamDirVersionsScope = LoggerMessage.DefineScope<string, string>(
       "Get all NuGet package versions from upstream (PackageId={PackageId}; RepositoryUrl={RepositoryUrl})."
    );

    private static readonly Func<ILogger, string, string, IDisposable?> _getAllLocalVersionsScope = LoggerMessage.DefineScope<string, string>(
       "Get all NuGet package versions from local (PackageId={PackageId}; RepositoryUrl={RepositoryUrl})."
    );

    /// <summary>
    /// Formats the GetAllVersions message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetAllVersionsScope(this ILogger logger, string packageId, IClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _getAllUpstreamDirVersionsScope(logger, packageId, clientService.PackageSourceLocation);
            return _getAllLocalVersionsScope(logger, packageId, clientService.PackageSourceLocation);
        }
        return _getAllRemoteVersionsScope(logger, packageId, clientService.PackageSourceLocation);
    }

    #endregion

    #region ResolvePackage Scope

    private static readonly Func<ILogger, string, NuGetVersion, NuGetFramework, string, IDisposable?> _resolveRemotePackageScope = LoggerMessage.DefineScope<string, NuGetVersion, NuGetFramework, string>(
       "Getting upstream package dependencies (PackageId={PackageId}; Version={Version}; Framework={Framework}; URL={RepositoryUrl})."
    );

    private static readonly Func<ILogger, string, NuGetVersion, NuGetFramework, string, IDisposable?> _resolveUpstreamDirPackageScope = LoggerMessage.DefineScope<string, NuGetVersion, NuGetFramework, string>(
       "Getting upstream package dependencies (PackageId={PackageId}; Version={Version}; Framework={Framework}; Path={RepositoryPath})."
    );

    private static readonly Func<ILogger, string, NuGetVersion, NuGetFramework, string, IDisposable?> _resolveLocalPackageScope = LoggerMessage.DefineScope<string, NuGetVersion, NuGetFramework, string>(
       "Getting local package dependencies (PackageId={PackageId}; Version={Version}; Framework={Framework}; Path={RepositoryPath})."
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
    public static IDisposable? BeginResolvePackageScope(this ILogger logger, string packageId, NuGetVersion version, NuGetFramework framework, IClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _resolveUpstreamDirPackageScope(logger, packageId, version, framework, clientService.PackageSourceLocation);
            return _resolveLocalPackageScope(logger, packageId, version, framework, clientService.PackageSourceLocation);
        }
        return _resolveRemotePackageScope(logger, packageId, version, framework, clientService.PackageSourceLocation);
    }

    #endregion

    #region ResolvePackages Scope

    private static readonly Func<ILogger, string, string, IDisposable?> _resolveRemotePackagesScope1 = LoggerMessage.DefineScope<string, string>(
       "Getting upstream package dependencies (PackageId={PackageId}; URL={RepositoryUrl})."
    );

    private static readonly Func<ILogger, string, string, IDisposable?> _resolveUpstreamDirPackagesScope1 = LoggerMessage.DefineScope<string, string>(
       "Getting upstream package dependencies (PackageId={PackageId}; Path={RepositoryPath})."
    );

    private static readonly Func<ILogger, string, string, IDisposable?> _resolveLocalPackagesScope1 = LoggerMessage.DefineScope<string, string>(
       "Getting local package dependencies (PackageId={PackageId}; Path={RepositoryPath})."
    );

    private static readonly Func<ILogger, string, NuGetFramework, string, IDisposable?> _resolveRemotePackagesScope2 = LoggerMessage.DefineScope<string, NuGetFramework, string>(
       "Getting upstream package dependencies (PackageId={PackageId}; Framework={Framework}; URL={RepositoryUrl})."
    );

    private static readonly Func<ILogger, string, NuGetFramework, string, IDisposable?> _resolveUpstreamDirPackagesScope2 = LoggerMessage.DefineScope<string, NuGetFramework, string>(
       "Getting upstream package dependencies (PackageId={PackageId}; Framework={Framework}; Path={RepositoryPath})."
    );

    private static readonly Func<ILogger, string, NuGetFramework, string, IDisposable?> _resolveLocalPackagesScope2 = LoggerMessage.DefineScope<string, NuGetFramework, string>(
       "Getting local package dependencies (PackageId={PackageId}; Framework={Framework}; Path={RepositoryPath})."
    );

    /// <summary>
    /// Formats the ResolvePackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package ID.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginResolvePackagesScope(this ILogger logger, string packageId, IClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _resolveUpstreamDirPackagesScope1(logger, packageId, clientService.PackageSourceLocation);
            return _resolveLocalPackagesScope1(logger, packageId, clientService.PackageSourceLocation);
        }
        return _resolveRemotePackagesScope1(logger, packageId, clientService.PackageSourceLocation);
    }

    /// <summary>
    /// Formats the ResolvePackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package ID.</param>
    /// <param name="framework">The package target framework.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginResolvePackagesScope(this ILogger logger, string packageId, NuGetFramework framework, IClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _resolveUpstreamDirPackagesScope2(logger, packageId, framework, clientService.PackageSourceLocation);
            return _resolveLocalPackagesScope2(logger, packageId, framework, clientService.PackageSourceLocation);
        }
        return _resolveRemotePackagesScope2(logger, packageId, framework, clientService.PackageSourceLocation);
    }

    #endregion

    #region GetDependencyInfo Scope

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _getRemoteDependencyInfoScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get dependency information from upstream (PackageId={PackageId}; Version={Version}; RepositoryUrl={RepositoryUrl})."
    );

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _getUpstreamDirDependencyInfoScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get dependency information from upstream (PackageId={PackageId}; Version={Version}; RepositoryUrl={RepositoryUrl})."
    );

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _getLocalDependencyInfoScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get dependency information from local (PackageId={PackageId}; Version={Version}; RepositoryUrl={RepositoryUrl})."
    );

    /// <summary>
    /// Formats the GetDependencyInfo message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetDependencyInfoScope(this ILogger logger, string packageId, NuGetVersion version, IClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _getUpstreamDirDependencyInfoScope(logger, packageId, version, clientService.PackageSourceLocation);
            return _getLocalDependencyInfoScope(logger, packageId, version, clientService.PackageSourceLocation);
        }
        return _getRemoteDependencyInfoScope(logger, packageId, version, clientService.PackageSourceLocation);
    }

    #endregion

    #region DoesPackageExist Scope

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _doesRemotePackageExistScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get dependency information from upstream (PackageId={PackageId}; Version={Version}; RepositoryUrl={RepositoryUrl})."
    );

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _doesUpstreamDirPackageExistScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get dependency information from upstream (PackageId={PackageId}; Version={Version}; RepositoryUrl={RepositoryUrl})."
    );

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _doesLocalPackageExistScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get dependency information from local (PackageId={PackageId}; Version={Version}; RepositoryUrl={RepositoryUrl})."
    );

    /// <summary>
    /// Formats the DoesPackageExist message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginDoesPackageExistScope(this ILogger logger, string packageId, NuGetVersion version, IClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _doesUpstreamDirPackageExistScope(logger, packageId, version, clientService.PackageSourceLocation);
            return _doesLocalPackageExistScope(logger, packageId, version, clientService.PackageSourceLocation);
        }
        return _doesRemotePackageExistScope(logger, packageId, version, clientService.PackageSourceLocation);
    }

    #endregion

    #region GetPackageDependencies Scope

    private static readonly Func<ILogger, string, NuGetVersion, NuGetFramework, string, IDisposable?> _getRemotePackageDependenciesScope = LoggerMessage.DefineScope<string, NuGetVersion, NuGetFramework, string>(
       "Get package dependencies from upstream (PackageId={PackageId}; Version={Version}; Framework={Framework}; URL={RepositoryUrl})."
    );

    private static readonly Func<ILogger, string, NuGetVersion, NuGetFramework, string, IDisposable?> _getUpstreamDirPackageDependenciesScope = LoggerMessage.DefineScope<string, NuGetVersion, NuGetFramework, string>(
       "Get package dependencies from upstream (PackageId={PackageId}; Version={Version}; Framework={Framework}; Path={RepositoryPath})."
    );

    private static readonly Func<ILogger, string, NuGetVersion, NuGetFramework, string, IDisposable?> _getLocalPackageDependenciesScope = LoggerMessage.DefineScope<string, NuGetVersion, NuGetFramework, string>(
       "Get package dependencies from local (PackageId={PackageId}; Version={Version}; Framework={Framework}; Path={RepositoryPath})."
    );

    private static readonly Func<ILogger, string, string, IDisposable?> _getAllRemotePackageDependenciesScope = LoggerMessage.DefineScope<string, string>(
       "Get package dependencies from upstream (PackageId={PackageId}; URL={RepositoryUrl})."
    );

    private static readonly Func<ILogger, string, string, IDisposable?> _getAllUpstreamDirPackageDependenciesScope = LoggerMessage.DefineScope<string, string>(
       "Get package dependencies from upstream (PackageId={PackageId}; Path={RepositoryPath})."
    );

    private static readonly Func<ILogger, string, string, IDisposable?> _getAllLocalPackageDependenciesScope = LoggerMessage.DefineScope<string, string>(
       "Get package dependencies from local (PackageId={PackageId}; Path={RepositoryPath})."
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
    public static IDisposable? BeginGetPackageDependenciesScope(this ILogger logger, string packageId, NuGetVersion version, NuGetFramework framework, IClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _getUpstreamDirPackageDependenciesScope(logger, packageId, version, framework, clientService.PackageSourceLocation);
            return _getLocalPackageDependenciesScope(logger, packageId, version, framework, clientService.PackageSourceLocation);
        }
        return _getRemotePackageDependenciesScope(logger, packageId, version, framework, clientService.PackageSourceLocation);
    }

    /// <summary>
    /// Formats the GetPackageDependencies message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="clientService">The client service.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetPackageDependenciesScope(this ILogger logger, string packageId, IClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _getAllUpstreamDirPackageDependenciesScope(logger, packageId, clientService.PackageSourceLocation);
            return _getAllLocalPackageDependenciesScope(logger, packageId, clientService.PackageSourceLocation);
        }
        return _getAllRemotePackageDependenciesScope(logger, packageId, clientService.PackageSourceLocation);
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
        "Delete local package {PackageId} from {RepositoryPath}."
    );

    /// <summary>
    /// Formats the DeleteLocalPackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the local package to delete.</param>
    /// <param name="repositoryPath">The local repository URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginDeleteLocalPackageScope(this ILogger logger, string packageId, string repositoryPath) => _deleteLocalPackageScope(logger, packageId, repositoryPath);

    #endregion

    #region DeleteLocalPackageVersion Scope

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _deleteLocalPackageVersionScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
        "Delete local package {PackageId}, version {Version} from {RepositoryPath}."
    );

    /// <summary>
    /// Formats the DeleteLocalPackageVersion message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the local package to delete.</param>
    /// <param name="version">The package version.</param>
    /// <param name="repositoryPath">The local repository URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginDeleteLocalPackageVersionScope(this ILogger logger, string packageId, NuGetVersion version, string repositoryPath) => _deleteLocalPackageVersionScope(logger, packageId, version, repositoryPath);

    #endregion

    #region AddLocalPackage Scope

    private static readonly Func<ILogger, string, string, IDisposable?> _addLocalPackageScope = LoggerMessage.DefineScope<string, string>(
        "Add local package {PackageId} to {RepositoryPath}."
    );

    /// <summary>
    /// Formats the AddLocalPackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the upstream package to add locally.</param>
    /// <param name="repositoryPath">The local repository URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginAddLocalPackageScope(this ILogger logger, string packageId, string repositoryPath) => _addLocalPackageScope(logger, packageId, repositoryPath);

    #endregion

    #region UpdateLocalPackage Scope

    private static readonly Func<ILogger, string, string, IDisposable?> _updateLocalPackageScope = LoggerMessage.DefineScope<string, string>(
        "Update local package {PackageId} to {RepositoryPath}."
    );

    /// <summary>
    /// Formats the UpdateLocalPackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the upstream package to update locally.</param>
    /// <param name="repositoryPath">The local repository URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginUpdateLocalPackageScope(this ILogger logger, string packageId, string repositoryPath) => _updateLocalPackageScope(logger, packageId, repositoryPath);

    #endregion

    #region GetAllLocalPackages Scope

    private static readonly Func<ILogger, string, IDisposable?> _getAllLocalPackagesScope = LoggerMessage.DefineScope<string>(
        "Getting all packages from {URL}."
    );

    /// <summary>
    /// Formats the GetAllLocalPackages message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The local repository URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetAllLocalPackagesScope(this ILogger logger, string url) => _getAllLocalPackagesScope(logger, url);

    #endregion
}