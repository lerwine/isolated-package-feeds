using Microsoft.Extensions.Logging;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGetAirGap;

public static class AppLoggerExtensions
{
    #region InvalidRepositoryUrl event logger message (0x0002)
    
    public const int EVENT_ID_InvalidRepositoryUrl = 0x0002;
    
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

    /// <summary>
    /// </summary>
    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="InvalidRepositoryUrl"/> event with event code 0x0002.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The invalid NuGet repository URL.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory URL.</param>
    /// <param name="createException">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by <paramref name="createException"/>.</returns>
    public static T LogInvalidRepositoryUrl<T>(this ILogger logger, string url, bool isUpstream, Func<string, T> createException, Exception? exception = null)
        where T : Exception
    {
        if (exception is PathTooLongException)
        {
            if (isUpstream)
            {
                _upstreamRepositoryPathTooLong(logger, url, exception);
                return createException(MESSAGE_UpstreamRepositoryPathTooLong);
            }
            _localRepositoryPathTooLong(logger, url, exception);
            return createException(MESSAGE_LocalRepositoryPathTooLong);
        }
        if (isUpstream)
        {
            _invalidUpstreamRepositoryUrl(logger, url, exception);
            return createException($"{MESSAGE_InvalidUpstreamRepositoryUrl}.");
        }
        _invalidLocalRepositoryUrl(logger, url, exception);
        return createException($"{MESSAGE_InvalidLocalRepositoryUrl}.");
    }

    public static T LogInvalidRepositoryUrl<T>(this ILogger logger, Uri url, bool isUpstream, Func<string, T> createException, Exception? exception = null)
        where T : Exception
    {
        if (url.IsAbsoluteUri)
        {
            if (isUpstream)
            {
                _invalidUpstreamRepositoryUrl(logger, url.OriginalString, exception);
                return createException($"{MESSAGE_InvalidUpstreamRepositoryUrl}.");
            }
            if (!url.IsFile)
            {
                _localRepositoryUrlIsNotLocal(logger, url.OriginalString, exception);
                return createException($"{MESSAGE_LocalRepositoryUrlIsNotLocal}.");
            }
        }
        else if (isUpstream)
        {
            _upstreamRepositoryUrlIsNotAbsolute(logger, url.OriginalString, exception);
            return createException($"{MESSAGE_UpstreamRepositoryUrlIsNotAbsolute}.");
        }
            
        _invalidLocalRepositoryUrl(logger, url.OriginalString, exception);
        return createException($"{MESSAGE_InvalidLocalRepositoryUrl}.");
    }

    #endregion

    #region RepositorySecurityException event logger message (0x0003)
    
    public const int EVENT_ID_RepositorySecurityException = 0x0003;
    
    public static readonly EventId RepositorySecurityException = new(EVENT_ID_RepositorySecurityException, nameof(RepositorySecurityException));
    
    private const string MESSAGE_UpstreamRepositorySecurityException = "Access denied while accessing upstream NuGet repository path";
    
    private const string MESSAGE_LocalRepositorySecurityException = "Access denied while accessing local NuGet repository path";
    
    private static readonly Action<ILogger, string, Exception?> _upstreamRepositorySecurityException = LoggerMessage.Define<string>(LogLevel.Critical, RepositorySecurityException,
        $"{MESSAGE_UpstreamRepositorySecurityException} ({{Path}}).");
    
    private static readonly Action<ILogger, string, Exception?> _localRepositorySecurityException = LoggerMessage.Define<string>(LogLevel.Critical, RepositorySecurityException,
        $"{MESSAGE_LocalRepositorySecurityException} ({{Path}}).");
    
    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="RepositorySecurityException"/> event with code 0x0003.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T LogRepositorySecurityException<T>(this ILogger logger, string path, bool isUpstream, Func<string, T> factory, Exception? exception = null) where T : Exception
    {
        if (isUpstream)
        {
            _upstreamRepositorySecurityException(logger, path, exception);
            return factory(MESSAGE_UpstreamRepositorySecurityException);
        }
        _localRepositorySecurityException(logger, path, exception);
        return factory(MESSAGE_LocalRepositorySecurityException);
    }
    
    #endregion

    #region RepositoryPathNotFound event logger message (0x0004)
    
    public const int EVENT_ID_RepositoryPathNotFound = 0x0004;
    
    public static readonly EventId RepositoryPathNotFound = new(EVENT_ID_RepositoryPathNotFound, nameof(RepositoryPathNotFound));
    
    private const string MESSAGE_UpstreamRepositoryPathNotFound = "Upstream repository path not found";
    
    private static readonly Action<ILogger, string, Exception?> _upstreamRepositoryPathNotFound = LoggerMessage.Define<string>(LogLevel.Critical, RepositoryPathNotFound,
        $"{MESSAGE_UpstreamRepositoryPathNotFound} ({{Path}}).");
    
    private const string MESSAGE_LocalRepositoryPathNotFound = "Local repository path not found";
    
    private static readonly Action<ILogger, string, Exception?> _localRepositoryPathNotFound = LoggerMessage.Define<string>(LogLevel.Critical, RepositoryPathNotFound,
        $"{MESSAGE_UpstreamRepositoryPathNotFound} ({{Path}}).");
    
    /// <summary>
    /// Logs a <see cref="LogLevel.Critical"/> message for a <see cref="RepositoryPathNotFound"/> event with code 0x0004.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The NuGet repository path.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory path.</param>
    /// <param name="factory">Factory method to create the exception to be returned (and subsequently thrown).</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of exception to be created.</typeparam>
    /// <returns>The exception that was created by the <paramref name="factory"/> function.</returns>
    public static T LogRepositoryPathNotFound<T>(this ILogger logger, string path, bool isUpstream, Func<string, T> factory, Exception? exception = null) where T : Exception
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

    #region Nuget Debug (0x0005)

    public const int EVENT_ID_NugetDebug = 0x0005;
    public static readonly EventId NugetDebug = new(EVENT_ID_NugetDebug, nameof(NugetDebug));

    private static readonly Action<ILogger, string, Exception?> _nugetDebugMessage1 = LoggerMessage.Define<string>(LogLevel.Debug, NugetDebug, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _nugetDebugMessage2 = LoggerMessage.Define<string, string, int>(LogLevel.Debug, NugetDebug, "NuGet {NugetID} ({Code}) Message: {Message}");

    private static readonly Action<ILogger, string, Exception?> _nugetVerboseMessage1 = LoggerMessage.Define<string>(LogLevel.Trace, NugetDebug, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _nugetVerboseMessage2 = LoggerMessage.Define<string, string, int>(LogLevel.Trace, NugetDebug, "NuGet {NugetID} ({Code}) Message: {Message}");

    /// <summary>
    /// Logs a <see cref="NuGet.Common.LogLevel.Debug"/> NugetDebug event with event code 0x0005.
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
    /// Logs a <see cref="NuGet.Common.LogLevel.Verbose"/> NugetDebug event with event code 0x0005.
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

    #region Nuget Message (0x0006)

    public const int EVENT_ID_NugetMessage = 0x0006;
    public static readonly EventId NugetMessage = new(EVENT_ID_NugetMessage, nameof(NugetMessage));

    private static readonly Action<ILogger, string, Exception?> _nugetInformationMessage1 = LoggerMessage.Define<string>(LogLevel.Information, NugetMessage, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _nugetInformationMessage2 = LoggerMessage.Define<string, string, int>(LogLevel.Information, NugetMessage, "NuGet {NugetID} ({Code}) Message: {Message}");

    private static readonly Action<ILogger, string, Exception?> _nugetMinimalMessage1 = LoggerMessage.Define<string>(LogLevel.Information, NugetMessage, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _nugetMinimalMessage2 = LoggerMessage.Define<string, string, int>(LogLevel.Information, NugetMessage, "NuGet {NugetID} ({Code}) Message: {Message}");

    /// <summary>
    /// Logs an <see cref="NuGet.Common.LogLevel.Information"/> NugetMessage event with event code 0x0005.
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
    /// Logs an <see cref="NuGet.Common.LogLevel.Minimal"/> NugetMessage event with event code 0x0005.
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

    #region Nuget Warning Message (0x0007)

    public const int EVENT_ID_NugetWarning = 0x0007;

    public static readonly EventId NugetWarning = new(EVENT_ID_NugetWarning, nameof(NugetWarning));

    private static readonly Action<ILogger, string, Exception?> _nugetWarning1 = LoggerMessage.Define<string>(LogLevel.Warning, NugetWarning, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _nugetWarning2 = LoggerMessage.Define<string, string, int>(LogLevel.Warning, NugetWarning, "NuGet {NugetID} ({Code}) Message: {Message}");

    /// <summary>
    /// Logs an NugetWarning event with event code 0x0007.
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

    #region NuGet Error Message (0x0008)

    public const int EVENT_ID_NuGetError = 0x0008;

    public static readonly EventId NuGetError = new(EVENT_ID_NuGetError, nameof(NuGetError));

    private static readonly Action<ILogger, string, Exception?> _nuGetError1 = LoggerMessage.Define<string>(LogLevel.Error, NuGetError, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _nuGetError2 = LoggerMessage.Define<string, string, int>(LogLevel.Error, NuGetError, "NuGet {NugetID} ({Code}) Message: {Message}");

    /// <summary>
    /// Logs a NuGetError event with event code 0x0008.
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

    #region Critical Nuget Error (0x0009)

    public const int EVENT_ID_CriticalNugetError = 0x0009;
    public static readonly EventId CriticalNugetError = new(EVENT_ID_CriticalNugetError, nameof(CriticalNugetError));

    private static readonly Action<ILogger, string, Exception?> _criticalNugetError1 = LoggerMessage.Define<string>(LogLevel.Critical, CriticalNugetError, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _criticalNugetError2 = LoggerMessage.Define<string, string, int>(LogLevel.Critical, CriticalNugetError, "NuGet {NugetID} ({Code}) Message: {Message}");

    /// <summary>
    /// Logs a CriticalNugetError event with event code 0x0009.
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

    #region ValidateUpstreamURL Scope

    private static readonly Func<ILogger, string, IDisposable?> _validateUpstreamURLScope = LoggerMessage.DefineScope<string>(
        "Validating upstream NuGet URL {URL}."
    );
    
    /// <summary>
    /// Formats the ValidateUpstreamURL message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The upstream NuGet repository URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginValidateUpstreamURLScope(this ILogger logger, string url) => _validateUpstreamURLScope(logger, url);
    
    #endregion

    #region ValidateLocalPath Scope
    
    private static readonly Func<ILogger, string, IDisposable?> _validateLocalPathScope = LoggerMessage.DefineScope<string>(
        "Validating local NuGet path ({Path})."
    );
    
    /// <summary>
    /// Formats the ValidateLocalPath message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the local NuGet repository.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginValidateLocalPathScope(this ILogger logger, string path) => _validateLocalPathScope(logger, path);
    
    #endregion
    
    #region GetDownloadResourceResult Scope
    
    private static readonly Func<ILogger, string, NuGetVersion?, string?, Guid?, string?, string, IDisposable?> _getUpstreamDownloadResourceResultScope = LoggerMessage.DefineScope<string, NuGetVersion?, string?, Guid?, string?, string>(
       "Get NuGet package download resource result from upstream (PackageId={PackageId}; Version={Version}; DownloadDirectory={DownloadDirectory}; ParentId={ParentId}; GlobalPackagesFolder={GlobalPackagesFolder}; RepositoryUrl={RepositoryUrl}).");

    private static readonly Func<ILogger, string, NuGetVersion?, string?, Guid?, string?, string, IDisposable?> _getLocalDownloadResourceResultScope = LoggerMessage.DefineScope<string, NuGetVersion?, string?, Guid?, string?, string>(
       "Get NuGet package download resource result from local (PackageId={PackageId}; Version={Version}; DownloadDirectory={DownloadDirectory}; ParentId={ParentId}; GlobalPackagesFolder={GlobalPackagesFolder}; RepositoryUrl={RepositoryUrl}).");

    /// <summary>
    /// Formats the GetDownloadResourceResult message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="identity">The NuGet Package identity.</param>
    /// <param name="downloadContext">The package download context.</param>
    /// <param name="globalPackagesFolder">The global packages folder.</param>
    /// <param name="repositoryUrl">The NuGet repository URL.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetDownloadResourceResultScope(this ILogger logger, PackageIdentity identity, PackageDownloadContext downloadContext, string? globalPackagesFolder,
        string repositoryUrl, bool isUpstream)
    {
        if (isUpstream)
            return _getUpstreamDownloadResourceResultScope(logger, identity.Id, identity.Version, downloadContext.DirectDownload ? downloadContext.DirectDownloadDirectory : null, downloadContext.ParentId, globalPackagesFolder, repositoryUrl);
        return _getLocalDownloadResourceResultScope(logger, identity.Id, identity.Version, downloadContext.DirectDownload ? downloadContext.DirectDownloadDirectory : null, downloadContext.ParentId, globalPackagesFolder, repositoryUrl);
    }

    #endregion

    /// string packageId, bool includePrerelease, bool includeUnlisted, string repositoryUrl
    #region GetMetadataAsync Scope
    
    private static readonly Func<ILogger, string, bool, bool, string, IDisposable?> _getUpstreamMetadataAsyncScope1 = LoggerMessage.DefineScope<string, bool, bool, string>(
       "Get NuGet package metadata from upstream (PackageId={PackageId}; IncludePreRelease={IncludePreRelease}; IncludeUnlisted={IncludeUnlisted}; RepositoryUrl={RepositoryUrl})."
    );

    private static readonly Func<ILogger, string, bool, bool, string, IDisposable?> _getLocalMetadataAsyncScope1 = LoggerMessage.DefineScope<string, bool, bool, string>(
       "Get NuGet package metadata from local (PackageId={PackageId}; IncludePreRelease={IncludePreRelease}; IncludeUnlisted={IncludeUnlisted}; RepositoryUrl={RepositoryUrl})."
    );
    
    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _getUpstreamMetadataAsyncScope2 = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get NuGet package metadata from upstream (PackageId={PackageId}; Version={Version}; RepositoryUrl={RepositoryUrl})."
    );

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _getLocalMetadataAsyncScope2 = LoggerMessage.DefineScope<string, NuGetVersion, string>(
       "Get NuGet package metadata from local (PackageId={PackageId}; Version={Version}; RepositoryUrl={RepositoryUrl})."
    );

    /// <summary>
    /// Formats the GetMetadataAsync message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package ID.</param>
    /// <param name="includePreRelease">Whether to include pre-release packages.</param>
    /// <param name="includeUnlisted">Whether to include unlisted packages.</param>
    /// <param name="repositoryUrl">The NuGet repository URL.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetMetadataAsyncScope(this ILogger logger, string packageId, bool includePreRelease, bool includeUnlisted, string repositoryUrl, bool isUpstream)
    {
        if (isUpstream)
            return _getUpstreamMetadataAsyncScope1(logger, packageId, includePreRelease, includeUnlisted, repositoryUrl);
        return _getLocalMetadataAsyncScope1(logger, packageId, includePreRelease, includeUnlisted, repositoryUrl);
    }

    public static IDisposable? BeginGetMetadataAsyncScope(this ILogger logger, string packageId, NuGetVersion version, string repositoryUrl, bool isUpstream)
    {
        if (isUpstream)
            return _getUpstreamMetadataAsyncScope2(logger, packageId, version, repositoryUrl);
        return _getLocalMetadataAsyncScope2(logger, packageId, version, repositoryUrl);
    }

    #endregion

    #region GetAllVersions Scope
    
    private static readonly Func<ILogger, string, string, IDisposable?> _getAllUpstreamVersionsScope = LoggerMessage.DefineScope<string, string>(
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
    /// <param name="repositoryUrl">The NuGet repository URL.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetAllVersionsScope(this ILogger logger, string packageId, string repositoryUrl, bool isUpstream)
    {
        if (isUpstream)
            return _getAllUpstreamVersionsScope(logger, packageId, repositoryUrl);
        return _getAllLocalVersionsScope(logger, packageId, repositoryUrl);
    }

    #endregion

    #region ResolvePackage Scope
    
    private static readonly Func<ILogger, string, NuGetVersion, NuGetFramework, string, IDisposable?> _resolveUpstreamPackageScope = LoggerMessage.DefineScope<string, NuGetVersion, NuGetFramework, string>(
       "Getting upstream package dependencies (PackageId={PackageId}; Version={Version}; Framework={Framework}; RepositoryUrl={RepositoryUrl})."
    );
    
    private static readonly Func<ILogger, string, NuGetVersion, NuGetFramework, string, IDisposable?> _resolveLocalPackageScope = LoggerMessage.DefineScope<string, NuGetVersion, NuGetFramework, string>(
       "Getting local package dependencies (PackageId={PackageId}; Version={Version}; Framework={Framework}; RepositoryUrl={RepositoryUrl})."
    );

    /// <summary>
    /// Formats the ResolvePackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <param name="framework">The package target framework.</param>
    /// <param name="repositoryUrl">The NuGet repository URL.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginResolvePackageScope(this ILogger logger, string packageId, NuGetVersion version, NuGetFramework framework, string repositoryUrl, bool isUpstream)
    {
        if (isUpstream)
            return _resolveUpstreamPackageScope(logger, packageId, version, framework, repositoryUrl);
        return _resolveLocalPackageScope(logger, packageId, version, framework, repositoryUrl);
    }

    #endregion

    #region ResolvePackages Scope
    
    private static readonly Func<ILogger, string, string, IDisposable?> _resolveUpstreamPackagesScope1 = LoggerMessage.DefineScope<string, string>(
       "Getting upstream package dependencies (PackageId={PackageId}; RepositoryUrl={RepositoryUrl})."
    );
    
    private static readonly Func<ILogger, string, string, IDisposable?> _resolveLocalPackagesScope1 = LoggerMessage.DefineScope<string, string>(
       "Getting local package dependencies (PackageId={PackageId}; RepositoryUrl={RepositoryUrl})."
    );

    private static readonly Func<ILogger, string, NuGetFramework, string, IDisposable?> _resolveUpstreamPackagesScope2 = LoggerMessage.DefineScope<string, NuGetFramework, string>(
       "Getting upstream package dependencies (PackageId={PackageId}; Framework={Framework}; RepositoryUrl={RepositoryUrl})."
    );
    
    private static readonly Func<ILogger, string, NuGetFramework, string, IDisposable?> _resolveLocalPackagesScope2 = LoggerMessage.DefineScope<string, NuGetFramework, string>(
       "Getting local package dependencies (PackageId={PackageId}; Framework={Framework}; RepositoryUrl={RepositoryUrl})."
    );

    /// <summary>
    /// Formats the ResolvePackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package ID.</param>
    /// <param name="repositoryUrl">The NuGet repository URL.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginResolvePackagesScope(this ILogger logger, string packageId, string repositoryUrl, bool isUpstream)
    {
        if (isUpstream)
            return _resolveUpstreamPackagesScope1(logger, packageId, repositoryUrl);
        return _resolveLocalPackagesScope1(logger, packageId, repositoryUrl);
    }

    /// <summary>
    /// Formats the ResolvePackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package ID.</param>
    /// <param name="framework">The package target framework.</param>
    /// <param name="repositoryUrl">The NuGet repository URL.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginResolvePackagesScope(this ILogger logger, string packageId, NuGetFramework framework, string repositoryUrl, bool isUpstream)
    {
        if (isUpstream)
            return _resolveUpstreamPackagesScope2(logger, packageId, framework, repositoryUrl);
        return _resolveLocalPackagesScope2(logger, packageId, framework, repositoryUrl);
    }

    #endregion

    #region GetDependencyInfo Scope

    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _getUpstreamDependencyInfoScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
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
    /// <param name="repositoryUrl">The NuGet repository URL.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetDependencyInfoScope(this ILogger logger, string packageId, NuGetVersion version, string repositoryUrl, bool isUpstream)
    {
        if (isUpstream)
            return _getUpstreamDependencyInfoScope(logger, packageId, version, repositoryUrl);
        return _getLocalDependencyInfoScope(logger, packageId, version, repositoryUrl);
    }

    #endregion

    #region DoesPackageExist Scope
    
    private static readonly Func<ILogger, string, NuGetVersion, string, IDisposable?> _doesUpstreamPackageExistScope = LoggerMessage.DefineScope<string, NuGetVersion, string>(
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
    /// <param name="repositoryUrl">The NuGet repository URL.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginDoesPackageExistScope(this ILogger logger, string packageId, NuGetVersion version, string repositoryUrl, bool isUpstream)
    {
        if (isUpstream)
            return _doesUpstreamPackageExistScope(logger, packageId, version, repositoryUrl);
        return _doesLocalPackageExistScope(logger, packageId, version, repositoryUrl);
    }

    #endregion

    #region GetPackageDependencies Scope
    
    private static readonly Func<ILogger, string, NuGetVersion, NuGetFramework, string, IDisposable?> _getUpstreamPackageDependenciesScope = LoggerMessage.DefineScope<string, NuGetVersion, NuGetFramework, string>(
       "Get package dependencies from upstream (PackageId={PackageId}; Version={Version}; Framework={Framework}; RepositoryUrl={RepositoryUrl})."
    );

    private static readonly Func<ILogger, string, NuGetVersion, NuGetFramework, string, IDisposable?> _getLocalPackageDependenciesScope = LoggerMessage.DefineScope<string, NuGetVersion, NuGetFramework, string>(
       "Get package dependencies from local (PackageId={PackageId}; Version={Version}; Framework={Framework}; RepositoryUrl={RepositoryUrl})."
    );

    /// <summary>
    /// Formats the GetPackageDependencies message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="framework">The package framework.</param>
    /// <param name="repositoryUrl">The NuGet repository URL.</param>
    /// <param name="isUpstream">Whether the error refers to an upstream NuGet repostitory URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetPackageDependenciesScope(this ILogger logger, string packageId, NuGetVersion version, NuGetFramework framework, string repositoryUrl, bool isUpstream)
    {
        if (isUpstream)
            return _getUpstreamPackageDependenciesScope(logger, packageId, version, framework, repositoryUrl);
        return _getLocalPackageDependenciesScope(logger, packageId, version, framework, repositoryUrl);
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
    
    private static readonly Func<ILogger, string, IDisposable?> _deleteLocalPackageScope = LoggerMessage.DefineScope<string>(
        "Delete local package {PackageId}."
    );
    
    /// <summary>
    /// Formats the DeleteLocalPackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the local package to delete.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginDeleteLocalPackageScope(this ILogger logger, string packageId) => _deleteLocalPackageScope(logger, packageId);
    
    #endregion

    #region DeleteLocalPackageVersion Scope
    
    private static readonly Func<ILogger, string, NuGetVersion, IDisposable?> _deleteLocalPackageVersionScope = LoggerMessage.DefineScope<string, NuGetVersion>(
        "Delete local package (PackageId={PackageId}; Version={Version})."
    );
    
    /// <summary>
    /// Formats the DeleteLocalPackageVersion message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the local package to delete.</param>
    /// <param name="version">The package version.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginDeleteLocalPackageVersionScope(this ILogger logger, string packageId, NuGetVersion version) => _deleteLocalPackageVersionScope(logger, packageId, version);
    
    #endregion

    #region AddLocalPackage Scope
    
    private static readonly Func<ILogger, string, IDisposable?> _addLocalPackageScope = LoggerMessage.DefineScope<string>(
        "Add local package {PackageId}."
    );
    
    /// <summary>
    /// Formats the AddLocalPackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The ID of the upstream package to add locally.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginAddLocalPackageScope(this ILogger logger, string packageId) => _addLocalPackageScope(logger, packageId);
    
    #endregion
}
