using Microsoft.Extensions.Logging;
using IsolatedPackageFeeds.Shared;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace NuGetPuller;

public static class AppLoggerExtensions
{
    #region Nuget Debug (0x0001)

    public const int EVENT_ID_NugetDebug = 0x0001;
    public static readonly EventId NugetDebug = new(EVENT_ID_NugetDebug, nameof(NugetDebug));

    private static readonly Action<ILogger, string, Exception?> _nugetDebugMessage1 = LoggerMessage.Define<string>(LogLevel.Debug, NugetDebug, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _nugetDebugMessage2 = LoggerMessage.Define<string, string, int>(LogLevel.Debug, NugetDebug, "NuGet {NugetID} ({Code}) Message: {Message}");

    private static readonly Action<ILogger, string, Exception?> _nugetVerboseMessage1 = LoggerMessage.Define<string>(LogLevel.Trace, NugetDebug, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _nugetVerboseMessage2 = LoggerMessage.Define<string, string, int>(LogLevel.Trace, NugetDebug, "NuGet {NugetID} ({Code}) Message: {Message}");

    /// <summary>
    /// Logs a <see cref="NuGet.Common.LogLevel.Debug"/> NugetDebug event with event code 0x0001.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    /// <param name="code">The optional NuGet log code.</param>
    public static void LogNugetDebugMessage(this ILogger logger, string message, NuGet.Common.NuGetLogCode? code = null)
    {
        if (code.HasValue)
            _nugetDebugMessage2(logger, message, code.Value.ToString("F"), (int)code.Value, null);
        else
            _nugetDebugMessage1(logger, message, null);
    }

    /// <summary>
    /// Logs a <see cref="NuGet.Common.LogLevel.Verbose"/> NugetDebug event with event code 0x0001.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    /// <param name="code">The optional NuGet log code.</param>
    public static void LogNugetVerboseMessage(this ILogger logger, string message, NuGet.Common.NuGetLogCode? code = null)
    {
        if (code.HasValue)
            _nugetVerboseMessage2(logger, message, code.Value.ToString("F"), (int)code.Value, null);
        else
            _nugetVerboseMessage1(logger, message, null);
    }

    #endregion

    #region Nuget Message (0x0002)

    public const int EVENT_ID_NugetMessage = 0x0002;

    public static readonly EventId NugetMessage = new(EVENT_ID_NugetMessage, nameof(NugetMessage));

    private static readonly Action<ILogger, string, Exception?> _nugetInformationMessage1 = LoggerMessage.Define<string>(LogLevel.Information, NugetMessage, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _nugetInformationMessage2 = LoggerMessage.Define<string, string, int>(LogLevel.Information, NugetMessage, "NuGet {NugetID} ({Code}) Message: {Message}");

    private static readonly Action<ILogger, string, Exception?> _nugetMinimalMessage1 = LoggerMessage.Define<string>(LogLevel.Information, NugetMessage, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _nugetMinimalMessage2 = LoggerMessage.Define<string, string, int>(LogLevel.Information, NugetMessage, "NuGet {NugetID} ({Code}) Message: {Message}");

    /// <summary>
    /// Logs an <see cref="NuGet.Common.LogLevel.Information"/> NugetMessage event with event code 0x0002.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    /// <param name="code">The optional NuGet log code.</param>
    public static void LogNugetInformationMessage(this ILogger logger, string message, NuGet.Common.NuGetLogCode? code = null)
    {
        if (code.HasValue)
            _nugetInformationMessage2(logger, message, code.Value.ToString("F"), (int)code.Value, null);
        else
            _nugetInformationMessage1(logger, message, null);
    }

    /// <summary>
    /// Logs an <see cref="NuGet.Common.LogLevel.Minimal"/> NugetMessage event with event code 0x0002.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet log message.</param>
    /// <param name="code">The optional NuGet log code.</param>
    public static void LogNugetMinimalMessage(this ILogger logger, string message, NuGet.Common.NuGetLogCode? code = null)
    {
        if (code.HasValue)
            _nugetMinimalMessage2(logger, message, code.Value.ToString("F"), (int)code.Value, null);
        else
            _nugetMinimalMessage1(logger, message, null);
    }

    #endregion

    #region Nuget Warning Message (0x0003)

    public const int EVENT_ID_NugetWarning = 0x0003;

    public static readonly EventId NugetWarning = new(EVENT_ID_NugetWarning, nameof(NugetWarning));

    private static readonly Action<ILogger, string, Exception?> _nugetWarning1 = LoggerMessage.Define<string>(LogLevel.Warning, NugetWarning, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _nugetWarning2 = LoggerMessage.Define<string, string, int>(LogLevel.Warning, NugetWarning, "NuGet {NugetID} ({Code}) Message: {Message}");

    /// <summary>
    /// Logs an NugetWarning event with event code 0x0003.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet warning message.</param>
    /// <param name="code">The optional NuGet log code.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogNugetWarning(this ILogger logger, string message, NuGet.Common.NuGetLogCode? code = null, Exception? exception = null)
    {
        if (code.HasValue)
            _nugetWarning2(logger, message, code.Value.ToString("F"), (int)code.Value, exception);
        else
            _nugetWarning1(logger, message, exception);
    }

    #endregion

    #region NuGet Error Message (0x0004)

    public const int EVENT_ID_NuGetError = 0x0004;

    public static readonly EventId NuGetError = new(EVENT_ID_NuGetError, nameof(NuGetError));

    private static readonly Action<ILogger, string, Exception?> _nuGetError1 = LoggerMessage.Define<string>(LogLevel.Error, NuGetError, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _nuGetError2 = LoggerMessage.Define<string, string, int>(LogLevel.Error, NuGetError, "NuGet {NugetID} ({Code}) Message: {Message}");

    /// <summary>
    /// Logs a NuGetError event with event code 0x0004.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet error message.</param>
    /// <param name="code">The optional NuGet log code.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogNuGetError(this ILogger logger, string message, NuGet.Common.NuGetLogCode? code = null, Exception? error = null)
    {
        if (code.HasValue)
            _nuGetError2(logger, message, code.Value.ToString("F"), (int)code.Value, error);
        else
            _nuGetError1(logger, message, error);
    }

    #endregion

    #region Critical Nuget Error (0x0005)

    public const int EVENT_ID_CriticalNugetError = 0x0005;
    public static readonly EventId CriticalNugetError = new(EVENT_ID_CriticalNugetError, nameof(CriticalNugetError));

    private static readonly Action<ILogger, string, Exception?> _criticalNugetError1 = LoggerMessage.Define<string>(LogLevel.Critical, CriticalNugetError, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _criticalNugetError2 = LoggerMessage.Define<string, string, int>(LogLevel.Critical, CriticalNugetError, "NuGet {NugetID} ({Code}) Message: {Message}");

    /// <summary>
    /// Logs a CriticalNugetError event with event code 0x0005.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="message">The NuGet error message.</param>
    /// <param name="code">The optional NuGet log code.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogCriticalNugetError(this ILogger logger, string message, NuGet.Common.NuGetLogCode? code = null, Exception? error = null)
    {
        if (code.HasValue)
            _criticalNugetError2(logger, message, code.Value.ToString("F"), (int)code.Value, error);
        else
            _criticalNugetError1(logger, message, error);
    }

    #endregion

    #region InvalidRepositoryUrl event logger message (0x0006)

    public const int EVENT_ID_InvalidRepositoryUrl = 0x0006;

    public static readonly EventId InvalidRepositoryUrl = new(EVENT_ID_InvalidRepositoryUrl, nameof(InvalidRepositoryUrl));

    private const string MESSAGE_LocalRepositoryUrlIsNotLocal = "Local NuGet repository URL does not reference a local path";

    private static readonly Action<ILogger, string, Exception?> _localRepositoryUrlIsNotLocal = LoggerMessage.Define<string>(LogLevel.Critical, InvalidRepositoryUrl,
        $"{MESSAGE_LocalRepositoryUrlIsNotLocal} ({{URL}}).");

    private const string MESSAGE_UpstreamRepositoryUrlIsNotAbsolute = "Upstream NuGet repository URL cannot be relative";

    private static readonly Action<ILogger, string, Exception?> _upstreamRepositoryUrlIsNotAbsolute = LoggerMessage.Define<string>(LogLevel.Critical, InvalidRepositoryUrl,
        $"{MESSAGE_UpstreamRepositoryUrlIsNotAbsolute} ({{URL}}).");

    private const string MESSAGE_UpstreamRepositoryPathTooLong = "Upstream NuGet repository path is too long";

    private static readonly Action<ILogger, string, Exception?> _upstreamRepositoryPathTooLong = LoggerMessage.Define<string>(LogLevel.Critical, InvalidRepositoryUrl,
        $"{MESSAGE_UpstreamRepositoryPathTooLong} ({{Path}}).");

    private const string MESSAGE_LocalRepositoryPathTooLong = "Local NuGet repository path is too long";

    private static readonly Action<ILogger, string, Exception?> _localRepositoryPathTooLong = LoggerMessage.Define<string>(LogLevel.Critical, InvalidRepositoryUrl,
        $"{MESSAGE_LocalRepositoryPathTooLong} ({{Path}}).");

    private const string MESSAGE_InvalidLocalRepositoryUrl = "Local NuGet repository URL is invalid";

    private static readonly Action<ILogger, string, Exception?> _invalidLocalRepositoryUrl = LoggerMessage.Define<string>(LogLevel.Critical, InvalidRepositoryUrl,
        $"{MESSAGE_InvalidLocalRepositoryUrl} ({{URL}}).");

    private const string MESSAGE_InvalidUpstreamRepositoryUrl = "Upstream NuGet repository URL is invalid";

    private static readonly Action<ILogger, string, Exception?> _invalidUpstreamRepositoryUrl = LoggerMessage.Define<string>(LogLevel.Critical, InvalidRepositoryUrl,
        $"{MESSAGE_InvalidUpstreamRepositoryUrl} ({{URL}}).");

    private const string MESSAGE_UnsupportedUpstreamRepositoryUrlScheme = "Invalid scheme or Upstream NuGet repository URL";

    private static readonly Action<ILogger, string, Exception?> _unsupportedUpstreamRepositoryUrlScheme = LoggerMessage.Define<string>(LogLevel.Critical, InvalidRepositoryUrl,
        $"{MESSAGE_UnsupportedUpstreamRepositoryUrlScheme} ({{URL}}).");

    private const string MESSAGE_UnsupportedLocalRepositoryUrlScheme = "Invalid scheme or Local NuGet repository URL";

    private static readonly Action<ILogger, string, Exception?> _unsupportedLocalRepositoryUrlScheme = LoggerMessage.Define<string>(LogLevel.Critical, InvalidRepositoryUrl,
        $"{MESSAGE_UnsupportedLocalRepositoryUrlScheme} ({{URL}}).");

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="InvalidRepositoryUrl"/> event with event code 0x0006.
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
                _upstreamRepositoryPathTooLong(logger, url, exception);
                return MESSAGE_UpstreamRepositoryPathTooLong;
            }
            _localRepositoryPathTooLong(logger, url, exception);
            return MESSAGE_LocalRepositoryPathTooLong;
        }
        if (isUpstream)
        {
            _invalidUpstreamRepositoryUrl(logger, url, exception);
            return $"{MESSAGE_InvalidUpstreamRepositoryUrl}.";
        }
        _invalidLocalRepositoryUrl(logger, url, exception);
        return $"{MESSAGE_InvalidLocalRepositoryUrl}.";
    }

    public static T LogInvalidRepositoryUrl<T>(this ILogger logger, string url, bool isUpstream, Func<string, T> factory, Exception? exception = null)
        where T : LoggedException
    {
        if (exception is PathTooLongException)
        {
            if (isUpstream)
            {
                _upstreamRepositoryPathTooLong(logger, url, exception);
                return factory(MESSAGE_UpstreamRepositoryPathTooLong);
            }
            _localRepositoryPathTooLong(logger, url, exception);
            return factory(MESSAGE_LocalRepositoryPathTooLong);
        }
        if (isUpstream)
        {
            _invalidUpstreamRepositoryUrl(logger, url, exception);
            return factory(MESSAGE_InvalidUpstreamRepositoryUrl);
        }
        _invalidLocalRepositoryUrl(logger, url, exception);
        return factory(MESSAGE_InvalidLocalRepositoryUrl);
    }

    public static string LogInvalidRepositoryUrl(this ILogger logger, Uri url, bool isUpstream, Exception? exception = null)
    {
        if (url.IsAbsoluteUri)
        {
            if (isUpstream)
            {
                _invalidUpstreamRepositoryUrl(logger, url.OriginalString, exception);
                return $"{MESSAGE_InvalidUpstreamRepositoryUrl}.";
            }
            if (!url.IsFile)
            {
                _localRepositoryUrlIsNotLocal(logger, url.OriginalString, exception);
                return $"{MESSAGE_LocalRepositoryUrlIsNotLocal}.";
            }
        }
        else if (isUpstream)
        {
            _upstreamRepositoryUrlIsNotAbsolute(logger, url.OriginalString, exception);
            return $"{MESSAGE_UpstreamRepositoryUrlIsNotAbsolute}.";
        }

        _invalidLocalRepositoryUrl(logger, url.OriginalString, exception);
        return $"{MESSAGE_InvalidLocalRepositoryUrl}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="UrlSchemeNotSupported"/> event with code 0x0006.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uriString">The invalid URI.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T LogUnsupportedRepositoryUrlScheme<T>(this ILogger logger, string uriString, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        _unsupportedUpstreamRepositoryUrlScheme(logger, uriString, exception);
        return factory(MESSAGE_UnsupportedUpstreamRepositoryUrlScheme);
    }
    
    public static string LogUnsupportedRepositoryUrlScheme(this ILogger logger, string uriString, bool isUpstream, Exception? exception = null)
    {
        if (isUpstream)
        {
            _unsupportedUpstreamRepositoryUrlScheme(logger, uriString, exception);
            return $"{MESSAGE_UnsupportedUpstreamRepositoryUrlScheme}.";
        }
        _unsupportedLocalRepositoryUrlScheme(logger, uriString, exception);
        return $"{MESSAGE_UnsupportedLocalRepositoryUrlScheme}.";
    }

    #endregion

    #region RepositorySecurityException event logger message (0x0007)

    public const int EVENT_ID_RepositorySecurityException = 0x0007;

    public static readonly EventId RepositorySecurityException = new(EVENT_ID_RepositorySecurityException, nameof(RepositorySecurityException));

    private const string MESSAGE_UpstreamRepositorySecurityException = "Access denied while accessing upstream NuGet repository path";

    private const string MESSAGE_LocalRepositorySecurityException = "Access denied while accessing local NuGet repository path";

    private static readonly Action<ILogger, string, Exception?> _upstreamRepositorySecurityException = LoggerMessage.Define<string>(LogLevel.Critical, RepositorySecurityException,
        $"{MESSAGE_UpstreamRepositorySecurityException} ({{Path}}).");

    private static readonly Action<ILogger, string, Exception?> _localRepositorySecurityException = LoggerMessage.Define<string>(LogLevel.Critical, RepositorySecurityException,
        $"{MESSAGE_LocalRepositorySecurityException} ({{Path}}).");

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="RepositorySecurityException"/> event with code 0x0007.
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
            _upstreamRepositorySecurityException(logger, path, exception);
            return MESSAGE_UpstreamRepositorySecurityException;
        }
        _localRepositorySecurityException(logger, path, exception);
        return MESSAGE_LocalRepositorySecurityException;
    }

    #endregion

    #region LocalRepositoryIOException event logger message (0x0008)

    public const int EVENT_ID_LocalRepositoryIOException = 0x0008;

    public static readonly EventId LocalRepositoryIOException = new(EVENT_ID_LocalRepositoryIOException, nameof(LocalRepositoryIOException));

    private const string MESSAGE_LocalRepositoryIOException = "I/O error while creating local repository folder";

    private static readonly Action<ILogger, string, Exception?> _localRepositoryIOException = LoggerMessage.Define<string>(LogLevel.Critical, LocalRepositoryIOException,
        $"{MESSAGE_LocalRepositoryIOException} {{Path}}.");

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="LocalRepositoryIOException"/> event with code 0x0008.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The local repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogLocalRepositoryIOException(this ILogger logger, string path, Exception? exception = null)
    {
        _localRepositoryIOException(logger, path, exception);
        return MESSAGE_LocalRepositoryIOException;
    }

    #endregion

    #region RepositoryPathNotFound event logger message (0x0009)

    public const int EVENT_ID_RepositoryPathNotFound = 0x0009;

    public static readonly EventId RepositoryPathNotFound = new(EVENT_ID_RepositoryPathNotFound, nameof(RepositoryPathNotFound));

    private const string MESSAGE_UpstreamRepositoryPathNotFound = "Upstream repository path not found";

    private static readonly Action<ILogger, string, Exception?> _upstreamRepositoryPathNotFound = LoggerMessage.Define<string>(LogLevel.Critical, RepositoryPathNotFound,
        $"{MESSAGE_UpstreamRepositoryPathNotFound} ({{Path}}).");

    private const string MESSAGE_LocalRepositoryPathNotFound = "Local repository path not found";

    private static readonly Action<ILogger, string, Exception?> _localRepositoryPathNotFound = LoggerMessage.Define<string>(LogLevel.Critical, RepositoryPathNotFound,
        $"{MESSAGE_UpstreamRepositoryPathNotFound} ({{Path}}).");

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="RepositoryPathNotFound"/> event with code 0x0009.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogRepositoryPathNotFound(this ILogger logger, string path, bool isUpstream, Exception? exception = null)
    {
        if (isUpstream)
        {
            _upstreamRepositoryPathNotFound(logger, path, exception);
            return MESSAGE_UpstreamRepositoryPathNotFound;
        }
        _localRepositoryPathNotFound(logger, path, exception);
        return MESSAGE_LocalRepositoryPathNotFound;
    }

    public static T LogRepositoryPathNotFound<T>(this ILogger logger, string path, bool isUpstream, Func<string, T> factory, Exception? exception = null)
        where T : LoggedException
    {
        if (isUpstream)
        {
            _upstreamRepositoryPathNotFound(logger, path, exception);
            return factory(MESSAGE_UpstreamRepositoryPathNotFound);
        }
        _localRepositoryPathNotFound(logger, path, exception);
        return factory(MESSAGE_LocalRepositoryPathNotFound);
    }

    #endregion

    #region InvalidExportLocalMetaData event logger message (0x000a)

    public const int EVENT_ID_InvalidExportLocalMetaData = 0x000a;

    public static readonly EventId InvalidExportLocalMetaData = new(EVENT_ID_InvalidExportLocalMetaData, nameof(InvalidExportLocalMetaData));

    private const string MESSAGE_InvalidExportLocalMetaData = "Package metadata export path is invalid";

    private const string MESSAGE_ExportLocalMetaDataDirectoryNotFound = "Parent subdirectory of package metadata export path not found";

    private const string MESSAGE_ExportLocalMetaDataPathTooLong = "Package metadata export path is too long";

    private static readonly Action<ILogger, string, Exception?> _invalidExportLocalMetaData = LoggerMessage.Define<string>(LogLevel.Critical, InvalidExportLocalMetaData,
        $"{MESSAGE_InvalidExportLocalMetaData} ({{Path}}).");

    private static readonly Action<ILogger, string, Exception?> _exportLocalMetaDataDirectoryNotFound = LoggerMessage.Define<string>(LogLevel.Critical, InvalidExportLocalMetaData,
        $"{MESSAGE_ExportLocalMetaDataDirectoryNotFound} ({{Path}}).");

    private static readonly Action<ILogger, string, Exception?> _exportLocalMetaDataPathTooLong = LoggerMessage.Define<string>(LogLevel.Critical, InvalidExportLocalMetaData,
        $"{MESSAGE_ExportLocalMetaDataPathTooLong} ({{Path}}).");

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="InvalidMetaDataExportPath"/> event with code 0x000a.
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
                _exportLocalMetaDataDirectoryNotFound(logger, path, exception);
                return factory(MESSAGE_ExportLocalMetaDataDirectoryNotFound);
            }
            if (exception is PathTooLongException)
            {
                _exportLocalMetaDataPathTooLong(logger, path, exception);
                return factory(MESSAGE_ExportLocalMetaDataPathTooLong);
            }
        }
        _invalidExportLocalMetaData(logger, path, exception);
        return factory(MESSAGE_InvalidExportLocalMetaData);
    }

    /// <summary>
    /// </summary>
    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="InvalidExportLocalMetaData"/> event with event code 0x000a.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The invalid NuGet repository URL.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogInvalidExportLocalMetaData(this ILogger logger, string url, Exception? exception = null)
    {
        if (exception is PathTooLongException)
        {
            _exportLocalMetaDataPathTooLong(logger, url, exception);
            return MESSAGE_ExportLocalMetaDataPathTooLong;
        }
        _invalidExportLocalMetaData(logger, url, exception);
        return $"{MESSAGE_InvalidExportLocalMetaData}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="InvalidExportLocalMetaData"/> event with code 0x000a.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogExportLocalMetaDataDirectoryNotFound(this ILogger logger, string path, Exception? exception = null)
    {
        _exportLocalMetaDataDirectoryNotFound(logger, path, exception);
        return MESSAGE_ExportLocalMetaDataDirectoryNotFound;
    }

    public static T LogExportLocalMetaDataDirectoryNotFound<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        _exportLocalMetaDataDirectoryNotFound(logger, path, exception);
        return factory(MESSAGE_ExportLocalMetaDataDirectoryNotFound);
    }

    #endregion

    #region MetaDataExportPathAccessDenied event logger message (0x000b)

    public const int EVENT_ID_MetaDataExportPathAccessDenied = 0x000b;

    public static readonly EventId MetaDataExportPathAccessDenied = new(EVENT_ID_MetaDataExportPathAccessDenied, nameof(MetaDataExportPathAccessDenied));

    private const string MESSAGE_MetaDataExportPathAccessDenied1 = "Access to package metadata export path is denied";

    private const string MESSAGE_MetaDataExportPathAccessDenied2 = "Caller has insufficient permissions to package metadata export path";

    private static readonly Action<ILogger, string, Exception?> _metaDataExportPathAccessDenied1 = LoggerMessage.Define<string>(LogLevel.Critical, MetaDataExportPathAccessDenied,
        $"{MESSAGE_MetaDataExportPathAccessDenied1} ({{Path}}).");

    private static readonly Action<ILogger, string, Exception?> _metaDataExportPathAccessDenied2 = LoggerMessage.Define<string>(LogLevel.Critical, MetaDataExportPathAccessDenied,
        $"{MESSAGE_MetaDataExportPathAccessDenied2} ({{Path}}).");

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="MetaDataExportPathAccessDenied"/> event with code 0x000b.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The package metadata export path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T LogMetaDataExportPathAccessDenied<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        if (exception is System.Security.SecurityException)
        {
            _metaDataExportPathAccessDenied2(logger, path, exception);
            return factory(MESSAGE_MetaDataExportPathAccessDenied2);
        }
        _metaDataExportPathAccessDenied1(logger, path, exception);
        return factory(MESSAGE_MetaDataExportPathAccessDenied1);
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="MetaDataExportPathAccessDenied"/> event with code 0x000b.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogMetaDataExportPathAccessDenied(this ILogger logger, string path, Exception? exception = null)
    {
        if (exception is System.Security.SecurityException)
        {
            _metaDataExportPathAccessDenied2(logger, path, exception);
            return MESSAGE_MetaDataExportPathAccessDenied2;
        }
        _metaDataExportPathAccessDenied1(logger, path, exception);
        return MESSAGE_MetaDataExportPathAccessDenied1;
    }

    #endregion

    #region PackageDeleted event logger message (0x000c)

    public const int EVENT_ID_PackageDeleted = 0x000c;

    public static readonly EventId PackageDeleted = new(EVENT_ID_PackageDeleted, nameof(PackageDeleted));

    private static readonly Action<ILogger, string, string, Exception?> _packageDeleted = LoggerMessage.Define<string, string>(LogLevel.Warning, PackageDeleted,
        "Package {PackageId} has been deleted from local repository ({RepositoryPath}).");

    /// <summary>
    /// Logs a <see cref="LogLevel.Warning"/> message for a <see cref="PackageDeleted"/> event with event code 0x000c.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the package that was deleted.</param>
    /// <param name="repositoryPath">The path of the local NuGet repository.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogPackageDeleted(this ILogger logger, string packageId, string repositoryPath, Exception? exception = null) => _packageDeleted(logger, packageId, repositoryPath, exception);

    #endregion

    #region PackageNotFound event logger message (0x000d)

    public const int EVENT_ID_PackageNotFound = 0x000d;

    public static readonly EventId PackageNotFound = new(EVENT_ID_PackageNotFound, nameof(PackageNotFound));

    private static readonly Action<ILogger, string, string, Exception?> _remotePackageNotFound1 = LoggerMessage.Define<string, string>(LogLevel.Warning, PackageNotFound,
        "Package {PackageId} not found in upstream NuGet source ({URL}).");

    private static readonly Action<ILogger, string, string, Exception?> _localPackageNotFound1 = LoggerMessage.Define<string, string>(LogLevel.Warning, PackageNotFound,
        "Package {PackageId} not found in local NuGet source ({RepositoryPath}).");

    private static readonly Action<ILogger, string, string, Exception?> _upstreamDirPackageNotFound1 = LoggerMessage.Define<string, string>(LogLevel.Warning, PackageNotFound,
        "Package {PackageId} not found in upstream NuGet source ({RepositoryPath}).");

    private static readonly Action<ILogger, string, NuGetVersion, string, Exception?> _remotePackageNotFound2 = LoggerMessage.Define<string, NuGetVersion, string>(LogLevel.Warning, PackageNotFound,
        "Version {Version} of package {PackageId} not found in upstream NuGet source ({URL}).");

    private static readonly Action<ILogger, string, NuGetVersion, string, Exception?> _localPackageNotFound2 = LoggerMessage.Define<string, NuGetVersion, string>(LogLevel.Warning, PackageNotFound,
        "Version {Version} of package {PackageId} not found in upstream NuGet source ({URL}).");

    private static readonly Action<ILogger, string, NuGetVersion, string, Exception?> _upstreamDirPackageNotFound2 = LoggerMessage.Define<string, NuGetVersion, string>(LogLevel.Warning, PackageNotFound,
        "Version {Version} of package {PackageId} not found in upstream NuGet source ({URL}).");

    /// <summary>
    /// Logs a <see cref="LogLevel.Warning"/> message for a <see cref="PackageNotFound"/> event with event code 0x000d.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the package that was not found.</param>
    /// <param name="clientService">The client service.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogPackageNotFound(this ILogger logger, string packageId, ClientService clientService, Exception? exception = null)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                _upstreamDirPackageNotFound1(logger, packageId, clientService.PackageSourceLocation, exception);
            else
                _localPackageNotFound1(logger, packageId, clientService.PackageSourceLocation, exception);
        }
        else
            _remotePackageNotFound1(logger, packageId, clientService.PackageSourceLocation, exception);
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Warning"/> message for a <see cref="PackageNotFound"/> event with event code 0x000d.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the package that was not found.</param>
    /// <param name="version">The version of the package that was not found.</param>
    /// <param name="clientService">The client service.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogPackageNotFound(this ILogger logger, string packageId, NuGetVersion version, ClientService clientService, Exception? exception = null)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                _upstreamDirPackageNotFound2(logger, packageId, version, clientService.PackageSourceLocation, exception);
            else
                _localPackageNotFound2(logger, packageId, version, clientService.PackageSourceLocation, exception);
        }
        else
            _remotePackageNotFound2(logger, packageId, version, clientService.PackageSourceLocation, exception);
    }

    #endregion

    #region GlobalPackagesFolderNotFound event logger message (0x000e)

    public const int EVENT_ID_GlobalPackagesFolderNotFound = 0x000e;

    public static readonly EventId GlobalPackagesFolderNotFound = new(EVENT_ID_GlobalPackagesFolderNotFound, nameof(GlobalPackagesFolderNotFound));

    private const string MESSAGE_GlobalPackagesFolderNotFound = "Global packages folder not found";

    private static readonly Action<ILogger, string, Exception?> _globalPackagesFolderNotFound = LoggerMessage.Define<string>(LogLevel.Critical, GlobalPackagesFolderNotFound,
        $"{MESSAGE_GlobalPackagesFolderNotFound} ({{Path}}).");

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="GlobalPackagesFolderNotFound"/> event with code 0x000e.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The Global packages folder path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogGlobalPackagesFolderNotFound(this ILogger logger, string path, Exception? exception = null)
    {
        _globalPackagesFolderNotFound(logger, path, exception);
        return MESSAGE_GlobalPackagesFolderNotFound;
    }

    public static T LogGlobalPackagesFolderNotFound<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        _globalPackagesFolderNotFound(logger, path, exception);
        return factory(MESSAGE_GlobalPackagesFolderNotFound);
    }

    #endregion

    #region GlobalPackagesFolderSecurityException event logger message (0x000f)

    public const int EVENT_ID_GlobalPackagesFolderSecurityException = 0x000f;

    public static readonly EventId GlobalPackagesFolderSecurityException = new(EVENT_ID_GlobalPackagesFolderSecurityException, nameof(GlobalPackagesFolderSecurityException));

    private const string MESSAGE_GlobalPackagesFolderSecurityException = "Access denied while accessing global packages folder";

    private static readonly Action<ILogger, string, Exception?> _globalPackagesFolderSecurityException = LoggerMessage.Define<string>(LogLevel.Critical, GlobalPackagesFolderSecurityException,
        $"{MESSAGE_GlobalPackagesFolderSecurityException} ({{Path}}).");

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="GlobalPackagesFolderSecurityException"/> event with code 0x000f.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The Global packages folder path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogGlobalPackagesFolderSecurityException(this ILogger logger, string path, Exception? exception = null)
    {
        _globalPackagesFolderSecurityException(logger, path, exception);
        return MESSAGE_GlobalPackagesFolderSecurityException;
    }

    #endregion

    #region InvalidGlobalPackagesFolder event logger message (0x0010)

    public const int EVENT_ID_InvalidGlobalPackagesFolder = 0x0010;

    public static readonly EventId InvalidGlobalPackagesFolder = new(EVENT_ID_InvalidGlobalPackagesFolder, nameof(InvalidGlobalPackagesFolder));

    private const string MESSAGE_GlobalPackagesFolderPathTooLong = "NuGet Global Packages Folder path is too long";

    private static readonly Action<ILogger, string, Exception?> _globalPackagesFolderPathTooLong = LoggerMessage.Define<string>(LogLevel.Critical, InvalidGlobalPackagesFolder,
        $"{MESSAGE_GlobalPackagesFolderPathTooLong} ({{Path}}).");

    private const string MESSAGE_InvalidGlobalPackagesFolder = "NuGet Global Packages Folder path is invalid";

    private static readonly Action<ILogger, string, Exception?> _invalidGlobalPackagesFolder = LoggerMessage.Define<string>(LogLevel.Critical, InvalidGlobalPackagesFolder,
        $"{MESSAGE_InvalidGlobalPackagesFolder} ({{Path}}).");

    private const string MESSAGE_GlobalPackagesFolderNotFileUri = "NuGet Global Packages Folder must refer to a filesystem subdirectory";

    private static readonly Action<ILogger, string, Exception?> _globalPackagesFolderNotFileUri = LoggerMessage.Define<string>(LogLevel.Critical, InvalidGlobalPackagesFolder,
        $"{MESSAGE_GlobalPackagesFolderNotFileUri} ({{URI}}).");

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="InvalidGlobalPackagesFolder"/> event with event code 0x0010.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The invalid global packages folder path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogInvalidGlobalPackagesFolder(this ILogger logger, string path, Exception? exception = null)
    {
        if (exception is PathTooLongException)
        {
            _globalPackagesFolderPathTooLong(logger, path, exception);
            return MESSAGE_GlobalPackagesFolderPathTooLong;
        }
        _invalidGlobalPackagesFolder(logger, path, exception);
        return $"{MESSAGE_InvalidGlobalPackagesFolder}.";
    }

    public static T LogInvalidGlobalPackagesFolder<T>(this ILogger logger, string path, Func<string, T> factory, Exception? exception = null) where T : LoggedException
    {
        if (exception is PathTooLongException)
        {
            _globalPackagesFolderPathTooLong(logger, path, exception);
            return factory(MESSAGE_GlobalPackagesFolderPathTooLong);
        }
        _invalidGlobalPackagesFolder(logger, path, exception);
        return factory(MESSAGE_InvalidGlobalPackagesFolder);
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="InvalidGlobalPackagesFolder"/> event with event code 0x0010.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The invalid global packages folder url.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogGlobalPackagesFolderNotFileUri(this ILogger logger, string url, Exception? exception = null)
    {
        _globalPackagesFolderNotFileUri(logger, url, exception);
        return $"{MESSAGE_GlobalPackagesFolderNotFileUri}.";
    }

    #endregion

    #region MultipleSettingsWithSameRepositoryLocation event logger message (0x0011)

    public const int EVENT_ID_MultipleSettingsWithSameRepositoryLocation = 0x0011;

    public static readonly EventId MultipleSettingsWithSameRepositoryLocation = new(EVENT_ID_MultipleSettingsWithSameRepositoryLocation, nameof(MultipleSettingsWithSameRepositoryLocation));

    private const string MESSAGE_LocalSameAsUpstreamNugetRepository = "Local NuGet repository path cannot be the same as the upstream NuGet repository path";

    private const string MESSAGE_LocalRepositorySameAsGlobalPackagesFolder = "Local NuGet repository path cannot be the same as the upstream NuGet repository path";

    private const string MESSAGE_UpstreamRepositorySameAsGlobalPackagesFolder = "Local NuGet repository path cannot be the same as the upstream NuGet repository path";

    private static readonly Action<ILogger, string, Exception?> _localSameAsUpstreamNugetRepository = LoggerMessage.Define<string>(LogLevel.Critical, MultipleSettingsWithSameRepositoryLocation,
        $"{MESSAGE_LocalSameAsUpstreamNugetRepository} ({{Path}}).");

    private static readonly Action<ILogger, string, Exception?> _localRepositorySameAsGlobalPackagesFolder = LoggerMessage.Define<string>(LogLevel.Critical, MultipleSettingsWithSameRepositoryLocation,
        $"{MESSAGE_LocalRepositorySameAsGlobalPackagesFolder} ({{Path}}).");

    private static readonly Action<ILogger, string, Exception?> _upstreamRepositorySameAsGlobalPackagesFolder = LoggerMessage.Define<string>(LogLevel.Critical, MultipleSettingsWithSameRepositoryLocation,
        $"{MESSAGE_UpstreamRepositorySameAsGlobalPackagesFolder} ({{Path}}).");

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="MultipleSettingsWithSameRepositoryLocation"/> event with event code 0x0011.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static string LogLocalSameAsUpstreamNugetRepository(this ILogger logger, string path, Exception? exception = null)
    {
        _localSameAsUpstreamNugetRepository(logger, path, exception);
        return MESSAGE_LocalSameAsUpstreamNugetRepository;
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="MultipleSettingsWithSameRepositoryLocation"/> event with event code 0x0011.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static string LogLocalRepositorySameAsGlobalPackagesFolder(this ILogger logger, string path, Exception? exception = null)
    {
        _localRepositorySameAsGlobalPackagesFolder(logger, path, exception);
        return MESSAGE_LocalRepositorySameAsGlobalPackagesFolder;
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="MultipleSettingsWithSameRepositoryLocation"/> event with event code 0x0011.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static string LogUpstreamRepositorySameAsGlobalPackagesFolder(this ILogger logger, string path, Exception? exception = null)
    {
        _upstreamRepositorySameAsGlobalPackagesFolder(logger, path, exception);
        return MESSAGE_UpstreamRepositorySameAsGlobalPackagesFolder;
    }

    #endregion

    #region PackageFileNotFound event logger message (0x0012)

    public const int EVENT_ID_PackageFileNotFound = 0x0012;

    public static readonly EventId PackageFileNotFound = new(EVENT_ID_PackageFileNotFound, nameof(PackageFileNotFound));

    private static readonly Action<ILogger, string, Exception?> _packageFileNotFound = LoggerMessage.Define<string>(LogLevel.Error, PackageFileNotFound,
        "Package file not found: {FileName}");

    /// <summary>
    /// Logs a <see cref="LogLevel.Error"/> message for a <see cref="PackageFileNotFound"/> event with event code 0x0012.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="fileName">The path of the file that was not found.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogPackageFileNotFound(this ILogger logger, string fileName, Exception? exception = null) => _packageFileNotFound(logger, fileName, exception);

    #endregion

    #region InvalidPackageFile event logger message (0x0013)

    public const int EVENT_ID_InvalidPackageFile = 0x0013;

    public static readonly EventId InvalidPackageFile = new(EVENT_ID_InvalidPackageFile, nameof(InvalidPackageFile));

    private static readonly Action<ILogger, string, Exception?> _packageFileNotZipArchive = LoggerMessage.Define<string>(LogLevel.Error, InvalidPackageFile,
        "Package file is not a ZIP archive: {FileName}");

    private static readonly Action<ILogger, string, Exception?> _packageFileInvalidContent = LoggerMessage.Define<string>(LogLevel.Error, InvalidPackageFile,
        "Package file has invalid content: {FileName}");

    /// <summary>
    /// Logs a <see cref="LogLevel.Error"/> message for a <see cref="InvalidPackageFile"/> event with event code 0x0013.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="fileName">The path of the invalid package file.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogPackageFileNotZipArchive(this ILogger logger, string fileName, Exception? exception = null) => _packageFileNotZipArchive(logger, fileName, exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Error"/> message for a <see cref="InvalidPackageFile"/> event with event code 0x0013.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="fileName">The path of the invalid package file.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogPackageFileInvalidContent(this ILogger logger, string fileName, Exception? exception = null) => _packageFileInvalidContent(logger, fileName, exception);

    #endregion

    #region PackageAlreadyAdded event logger message (0x0014)

    public const int EVENT_ID_PackageAlreadyAdded = 0x0014;

    public static readonly EventId PackageAlreadyAdded = new(EVENT_ID_PackageAlreadyAdded, nameof(PackageAlreadyAdded));

    private static readonly Action<ILogger, string, Exception?> _packageAlreadyAdded = LoggerMessage.Define<string>(LogLevel.Warning, PackageAlreadyAdded,
        "Package {PackageId} has already been added.");

    /// <summary>
    /// Logs a <see cref="LogLevel.Warning"/> message for a <see cref="PackageAlreadyAdded"/> event with event code 0x0014.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The identifier of the existing package.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogPackageAlreadyAdded(this ILogger logger, string packageId, Exception? exception = null) => _packageAlreadyAdded(logger, packageId, exception);

    #endregion

    #region PackageVersionDeleteFailure event logger message (0x0015)

    public const int EVENT_ID_PackageVersionDeleteFailure = 0x0015;

    public static readonly EventId PackageVersionDeleteFailure = new(EVENT_ID_PackageVersionDeleteFailure, nameof(PackageVersionDeleteFailure));

    private static readonly Action<ILogger, string, NuGetVersion, Exception?> _packageVersionDeleteFailure = LoggerMessage.Define<string, NuGetVersion>(LogLevel.Warning, PackageVersionDeleteFailure,
        "Unexpected error deleting Package {PackageId}, Version {Version}.");

    /// <summary>
    /// Logs a <see cref="LogLevel.Warning"/> message for a <see cref="PackageVersionDeleteFailure"/> event with event code 0x0015.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogPackageVersionDeleteFailure(this ILogger logger, string packageId, NuGetVersion version, Exception? exception = null) => _packageVersionDeleteFailure(logger, packageId, version, exception);

    #endregion

    #region UnexpectedPackageDownloadFailure event logger message (0x0016)

    public const int EVENT_ID_UnexpectedPackageDownloadFailure = 0x0016;

    public static readonly EventId UnexpectedPackageDownloadFailure = new(EVENT_ID_UnexpectedPackageDownloadFailure, nameof(UnexpectedPackageDownloadFailure));

    private static readonly Action<ILogger, string, NuGetVersion, Exception?> _unexpectedPackageDownloadFailure = LoggerMessage.Define<string, NuGetVersion>(LogLevel.Error, UnexpectedPackageDownloadFailure,
        "Unexpected error while downloading package {PackageId}, Version {Version}.");

    private static readonly Action<ILogger, string, NuGetVersion, Exception?> _emptyPackageDownload = LoggerMessage.Define<string, NuGetVersion>(LogLevel.Error, UnexpectedPackageDownloadFailure,
        "Package download of {PackageId}, Version {Version} was unexpectedly empty.");

    /// <summary>
    /// Logs a <see cref="LogLevel.Error"/> message for a <see cref="UnexpectedPackageDownloadFailure"/> event with event code 0x0016.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="exception">The exception that caused the event.</param>
    public static void LogUnexpectedPackageDownloadFailure(this ILogger logger, string packageId, NuGetVersion version, Exception exception) => _unexpectedPackageDownloadFailure(logger, packageId, version, exception);

    /// <summary>
    /// Logs a <see cref="LogLevel.Error"/> message for a <see cref="UnexpectedPackageDownloadFailure"/> event with event code 0x0016.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    public static void LogEmptyPackageDownload(this ILogger logger, string packageId, NuGetVersion version) => _emptyPackageDownload(logger, packageId, version, null);

    #endregion

    #region UnexpectedAddFailure event logger message (0x0017)

    public const int EVENT_ID_UnexpectedAddFailure = 0x0017;

    public static readonly EventId UnexpectedAddFailure = new(EVENT_ID_UnexpectedAddFailure, nameof(UnexpectedAddFailure));

    private static readonly Action<ILogger, string, NuGetVersion, Exception?> _unexpectedAddFailure = LoggerMessage.Define<string, NuGetVersion>(LogLevel.Error, UnexpectedAddFailure,
        "Unexpected error while adding Package {PackageId}, Version {Version}.");

    /// <summary>
    /// Logs a <see cref="LogLevel.Error"/> message for a <see cref="UnexpectedAddFailure"/> event with event code 0x0017.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogUnexpectedAddFailure(this ILogger logger, string packageId, NuGetVersion version, Exception? exception = null) => _unexpectedAddFailure(logger, packageId, version, exception);

    #endregion

    #region NoLocalPackagesExist event logger message (0x0018)

    public const int EVENT_ID_NoLocalPackagesExist = 0x0018;

    public static readonly EventId NoLocalPackagesExist = new(EVENT_ID_NoLocalPackagesExist, nameof(NoLocalPackagesExist));

    private static readonly Action<ILogger, Exception?> _nolocalPackagesExist = LoggerMessage.Define(LogLevel.Warning, NoLocalPackagesExist,
        "Local repository has no packages.");

    /// <summary>
    /// Logs a <see cref="LogLevel.Warning"/> message for a <see cref="NoLocalPackagesExist"/> event with code 0x0018.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogNoLocalPackagesExist(this ILogger logger, Exception? exception = null) => _nolocalPackagesExist(logger, exception);

    #endregion

    #region InvalidExportBundle event logger message (0x0019)

    public const int EVENT_ID_InvalidExportBundle = 0x0019;

    public static readonly EventId InvalidExportBundle = new(EVENT_ID_InvalidExportBundle, nameof(InvalidExportBundle));

    private const string MESSAGE_InvalidExportBundle = "Package metadata export path is invalid";

    private const string MESSAGE_ExportBundleDirectoryNotFound = "Parent subdirectory of package metadata export path not found";

    private const string MESSAGE_ExportBundlePathTooLong = "Package metadata export path is too long";

    private static readonly Action<ILogger, string, Exception?> _invalidExportBundle = LoggerMessage.Define<string>(LogLevel.Critical, InvalidExportBundle,
        $"{MESSAGE_InvalidExportBundle} ({{Path}}).");

    private static readonly Action<ILogger, string, Exception?> _exportBundleDirectoryNotFound = LoggerMessage.Define<string>(LogLevel.Critical, InvalidExportBundle,
        $"{MESSAGE_ExportBundleDirectoryNotFound} ({{Path}}).");

    private static readonly Action<ILogger, string, Exception?> _exportBundlePathTooLong = LoggerMessage.Define<string>(LogLevel.Critical, InvalidExportBundle,
        $"{MESSAGE_ExportBundlePathTooLong} ({{Path}}).");

    /// <summary>
    /// </summary>
    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="InvalidExportBundle"/> event with event code 0x0019.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The invalid NuGet repository URL.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogInvalidExportBundle(this ILogger logger, string url, Exception? exception = null)
    {
        if (exception is PathTooLongException)
        {
            _exportBundlePathTooLong(logger, url, exception);
            return MESSAGE_ExportBundlePathTooLong;
        }
        _invalidExportBundle(logger, url, exception);
        return $"{MESSAGE_InvalidExportBundle}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="InvalidExportBundle"/> event with code 0x0019.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogExportBundleDirectoryNotFound(this ILogger logger, string path, Exception? exception = null)
    {
        _exportBundleDirectoryNotFound(logger, path, exception);
        return MESSAGE_ExportBundleDirectoryNotFound;
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="MetaDataExportPathAccessDenied"/> event with code 0x000b.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogExportBundlePathAccessDenied(this ILogger logger, string path, Exception? exception = null)
    {
        if (exception is System.Security.SecurityException)
        {
            _metaDataExportPathAccessDenied2(logger, path, exception);
            return MESSAGE_MetaDataExportPathAccessDenied2;
        }
        _metaDataExportPathAccessDenied1(logger, path, exception);
        return MESSAGE_MetaDataExportPathAccessDenied1;
    }

    #endregion

    #region InvalidTargetManifestFile event logger message (0x001a)

    public const int EVENT_ID_InvalidTargetManifestFile = 0x001a;

    public static readonly EventId InvalidTargetManifestFile = new(EVENT_ID_InvalidTargetManifestFile, nameof(InvalidTargetManifestFile));

    private const string MESSAGE_InvalidTargetManifestFile = "Package metadata export path is invalid";

    private const string MESSAGE_TargetManifestFileDirectoryNotFound = "Parent subdirectory of package metadata export path not found";

    private const string MESSAGE_TargetManifestFilePathTooLong = "Package metadata export path is too long";

    private static readonly Action<ILogger, string, Exception?> _invalidTargetManifestFile = LoggerMessage.Define<string>(LogLevel.Critical, InvalidTargetManifestFile,
        $"{MESSAGE_InvalidTargetManifestFile} ({{Path}}).");

    private static readonly Action<ILogger, string, Exception?> _targetManifestFileDirectoryNotFound = LoggerMessage.Define<string>(LogLevel.Critical, InvalidTargetManifestFile,
        $"{MESSAGE_TargetManifestFileDirectoryNotFound} ({{Path}}).");

    private static readonly Action<ILogger, string, Exception?> _targetManifestFilePathTooLong = LoggerMessage.Define<string>(LogLevel.Critical, InvalidTargetManifestFile,
        $"{MESSAGE_TargetManifestFilePathTooLong} ({{Path}}).");

    /// <summary>
    /// </summary>
    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="InvalidTargetManifestFile"/> event with event code 0x001a.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The invalid NuGet repository URL.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogInvalidTargetManifestFile(this ILogger logger, string url, Exception? exception = null)
    {
        if (exception is PathTooLongException)
        {
            _targetManifestFilePathTooLong(logger, url, exception);
            return MESSAGE_TargetManifestFilePathTooLong;
        }
        _invalidTargetManifestFile(logger, url, exception);
        return $"{MESSAGE_InvalidTargetManifestFile}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="InvalidTargetManifestFile"/> event with code 0x001a.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogTargetManifestFileDirectoryNotFound(this ILogger logger, string path, Exception? exception = null)
    {
        _targetManifestFileDirectoryNotFound(logger, path, exception);
        return MESSAGE_TargetManifestFileDirectoryNotFound;
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="MetaDataExportPathAccessDenied"/> event with code 0x000b.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogTargetManifestFilePathAccessDenied(this ILogger logger, string path, Exception? exception = null)
    {
        if (exception is System.Security.SecurityException)
        {
            _metaDataExportPathAccessDenied2(logger, path, exception);
            return MESSAGE_MetaDataExportPathAccessDenied2;
        }
        _metaDataExportPathAccessDenied1(logger, path, exception);
        return MESSAGE_MetaDataExportPathAccessDenied1;
    }

    #endregion

    #region InvalidSaveTargetManifestAs event logger message (0x001b)

    public const int EVENT_ID_InvalidSaveTargetManifestAs = 0x001b;

    public static readonly EventId InvalidSaveTargetManifestAs = new(EVENT_ID_InvalidSaveTargetManifestAs, nameof(InvalidSaveTargetManifestAs));

    private const string MESSAGE_InvalidSaveTargetManifestAs = "Package metadata export path is invalid";

    private const string MESSAGE_SaveTargetManifestAsDirectoryNotFound = "Parent subdirectory of package metadata export path not found";

    private const string MESSAGE_SaveTargetManifestAsPathTooLong = "Package metadata export path is too long";

    private static readonly Action<ILogger, string, Exception?> _invalidSaveTargetManifestAs = LoggerMessage.Define<string>(LogLevel.Critical, InvalidSaveTargetManifestAs,
        $"{MESSAGE_InvalidSaveTargetManifestAs} ({{Path}}).");

    private static readonly Action<ILogger, string, Exception?> _saveTargetManifestAsDirectoryNotFound = LoggerMessage.Define<string>(LogLevel.Critical, InvalidSaveTargetManifestAs,
        $"{MESSAGE_SaveTargetManifestAsDirectoryNotFound} ({{Path}}).");

    private static readonly Action<ILogger, string, Exception?> _saveTargetManifestAsPathTooLong = LoggerMessage.Define<string>(LogLevel.Critical, InvalidSaveTargetManifestAs,
        $"{MESSAGE_SaveTargetManifestAsPathTooLong} ({{Path}}).");

    /// <summary>
    /// </summary>
    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="InvalidSaveTargetManifestAs"/> event with event code 0x001b.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The invalid NuGet repository URL.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogInvalidSaveTargetManifestAs(this ILogger logger, string url, Exception? exception = null)
    {
        if (exception is PathTooLongException)
        {
            _saveTargetManifestAsPathTooLong(logger, url, exception);
            return MESSAGE_SaveTargetManifestAsPathTooLong;
        }
        _invalidSaveTargetManifestAs(logger, url, exception);
        return $"{MESSAGE_InvalidSaveTargetManifestAs}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="InvalidSaveTargetManifestAs"/> event with code 0x001b.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogSaveTargetManifestAsDirectoryNotFound(this ILogger logger, string path, Exception? exception = null)
    {
        _saveTargetManifestAsDirectoryNotFound(logger, path, exception);
        return MESSAGE_SaveTargetManifestAsDirectoryNotFound;
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="MetaDataExportPathAccessDenied"/> event with code 0x000b.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogSaveTargetManifestAsPathAccessDenied(this ILogger logger, string path, Exception? exception = null)
    {
        if (exception is System.Security.SecurityException)
        {
            _metaDataExportPathAccessDenied2(logger, path, exception);
            return MESSAGE_MetaDataExportPathAccessDenied2;
        }
        _metaDataExportPathAccessDenied1(logger, path, exception);
        return MESSAGE_MetaDataExportPathAccessDenied1;
    }

    #endregion

    #region InvalidImport event logger message (0x001c)

    public const int EVENT_ID_InvalidImport = 0x001c;

    public static readonly EventId InvalidImport = new(EVENT_ID_InvalidImport, nameof(InvalidImport));

    private const string MESSAGE_InvalidImport = "Package metadata export path is invalid";

    private const string MESSAGE_ImportDirectoryNotFound = "Parent subdirectory of package metadata export path not found";

    private const string MESSAGE_ImportPathTooLong = "Package metadata export path is too long";

    private static readonly Action<ILogger, string, Exception?> _invalidImport = LoggerMessage.Define<string>(LogLevel.Critical, InvalidImport,
        $"{MESSAGE_InvalidImport} ({{Path}}).");

    private static readonly Action<ILogger, string, Exception?> _importDirectoryNotFound = LoggerMessage.Define<string>(LogLevel.Critical, InvalidImport,
        $"{MESSAGE_ImportDirectoryNotFound} ({{Path}}).");

    private static readonly Action<ILogger, string, Exception?> _importPathTooLong = LoggerMessage.Define<string>(LogLevel.Critical, InvalidImport,
        $"{MESSAGE_ImportPathTooLong} ({{Path}}).");

    /// <summary>
    /// </summary>
    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="InvalidImport"/> event with event code 0x001c.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The invalid NuGet repository URL.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogInvalidImportPath(this ILogger logger, string url, Exception? exception = null)
    {
        if (exception is PathTooLongException)
        {
            _importPathTooLong(logger, url, exception);
            return MESSAGE_ImportPathTooLong;
        }
        _invalidImport(logger, url, exception);
        return $"{MESSAGE_InvalidImport}.";
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="InvalidImport"/> event with code 0x001c.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogImportFileOrDirectoryNotFound(this ILogger logger, string path, Exception? exception = null)
    {
        _importDirectoryNotFound(logger, path, exception);
        return MESSAGE_ImportDirectoryNotFound;
    }

    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="MetaDataExportPathAccessDenied"/> event with code 0x000b.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <returns>The validation message.</returns>
    public static string LogImportPathAccessDenied(this ILogger logger, string path, Exception? exception = null)
    {
        if (exception is System.Security.SecurityException)
        {
            _metaDataExportPathAccessDenied2(logger, path, exception);
            return MESSAGE_MetaDataExportPathAccessDenied2;
        }
        _metaDataExportPathAccessDenied1(logger, path, exception);
        return MESSAGE_MetaDataExportPathAccessDenied1;
    }

    #endregion

    #region DownloadingNuGetPackage event logger message (0x001d)

    public const int EVENT_ID_DownloadingNuGetPackage = 0x001d;

    public static readonly EventId DownloadingNuGetPackage = new(EVENT_ID_DownloadingNuGetPackage, nameof(DownloadingNuGetPackage));

    private static readonly Action<ILogger, string, Uri, Exception?> _downloadingRemoteNuGetPackage1 = LoggerMessage.Define<string, Uri>(LogLevel.Information, DownloadingNuGetPackage,
        "Downloading package {PackageId} from {URL}.");

    private static readonly Action<ILogger, string, NuGetVersion, Uri, Exception?> _downloadingRemoteNuGetPackage2 = LoggerMessage.Define<string, NuGetVersion, Uri>(LogLevel.Information, DownloadingNuGetPackage,
        "Downloading package {PackageId}, version {Version} from {URL}.");

    private static readonly Action<ILogger, string, string, Exception?> _downloadingLocalNuGetPackage1 = LoggerMessage.Define<string, string>(LogLevel.Information, DownloadingNuGetPackage,
        "Downloading package {PackageId} from {Path}.");

    private static readonly Action<ILogger, string, NuGetVersion, string, Exception?> _downloadingLocalNuGetPackage2 = LoggerMessage.Define<string, NuGetVersion, string>(LogLevel.Information, DownloadingNuGetPackage,
        "Downloading package {PackageId}, version {Version} from {Path}.");

    /// <summary>
    /// Logs a <see cref="LogLevel.Information"/> message for a <see cref="DownloadingNuGetPackage"/> event with event code 0x001d.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="identity">The package identifier and version.</param>
    /// <param name="clientService">The source repository.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogDownloadingNuGetPackage(this ILogger logger, PackageIdentity identity, ClientService clientService, Exception? exception = null)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (identity.HasVersion)
                _downloadingLocalNuGetPackage2(logger, identity.Id, identity.Version, clientService.PackageSourceLocation, exception);
            else
                _downloadingLocalNuGetPackage1(logger, identity.Id, clientService.PackageSourceLocation, exception);
        }
        else if (identity.HasVersion)
            _downloadingRemoteNuGetPackage2(logger, identity.Id, identity.Version, clientService.PackageSourceUri, exception);
        else
            _downloadingRemoteNuGetPackage1(logger, identity.Id, clientService.PackageSourceUri, exception);
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
    public static IDisposable? BeginGetDownloadResourceResultScope(this ILogger logger, PackageIdentity identity, ClientService clientService)
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
    public static IDisposable? BeginDownloadNupkgScope(this ILogger logger, string packageId, NuGetVersion version, ClientService clientService)
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
    public static IDisposable? BeginGetMetadataScope(this ILogger logger, string packageId, bool includePreRelease, bool includeUnlisted, ClientService clientService)
    {
        if (clientService.PackageSourceUri.IsFile)
        {
            if (clientService.IsUpstream)
                return _getUpstreamDirMetadataScope1(logger, packageId, includePreRelease, includeUnlisted, clientService.PackageSourceLocation);
            return _getLocalMetadataScope1(logger, packageId, includePreRelease, includeUnlisted, clientService.PackageSourceLocation);
        }
        return _getRemoteMetadataScope1(logger, packageId, includePreRelease, includeUnlisted, clientService.PackageSourceLocation);
    }

    public static IDisposable? BeginGetMetadataScope(this ILogger logger, string packageId, NuGetVersion version, ClientService clientService)
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
    public static IDisposable? BeginGetAllVersionsScope(this ILogger logger, string packageId, ClientService clientService)
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
    public static IDisposable? BeginResolvePackageScope(this ILogger logger, string packageId, NuGetVersion version, NuGetFramework framework, ClientService clientService)
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
    public static IDisposable? BeginResolvePackagesScope(this ILogger logger, string packageId, ClientService clientService)
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
    public static IDisposable? BeginResolvePackagesScope(this ILogger logger, string packageId, NuGetFramework framework, ClientService clientService)
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
    public static IDisposable? BeginGetDependencyInfoScope(this ILogger logger, string packageId, NuGetVersion version, ClientService clientService)
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
    public static IDisposable? BeginDoesPackageExistScope(this ILogger logger, string packageId, NuGetVersion version, ClientService clientService)
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
    public static IDisposable? BeginGetPackageDependenciesScope(this ILogger logger, string packageId, NuGetVersion version, NuGetFramework framework, ClientService clientService)
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
    public static IDisposable? BeginGetPackageDependenciesScope(this ILogger logger, string packageId, ClientService clientService)
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