using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using IsolatedPackageFeeds.Shared;

namespace CdnGetter;

internal static class LoggerMessages
{
    #region UnexpectedService Error (0x0001)

    public const int EVENT_ID_UnexpectedServiceError = 0x0001;

    public static readonly EventId UnexpectedServiceError = new(EVENT_ID_UnexpectedServiceError, nameof(UnexpectedServiceError));

    private static readonly Action<ILogger, string, Exception?> _unexpectedServiceError = LoggerMessage.Define<string>(LogLevel.Error,
        UnexpectedServiceError, "Error executing {TypeFullName}.");
    /// <summary>
    /// Logs an UnexpectedService event with event code 0x0001.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="type">The type of service that threw the exception.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogUnexpectedServiceError(this ILogger logger, Type type, Exception error) => _unexpectedServiceError(logger, type.FullName ?? type.Name, error);

    /// <summary>
    /// Logs an UnexpectedService event with event code 0x0001.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="error">The exception that caused the event.</param>
    /// <typeparam name="T">The type of service that threw the exception.</typeparam>
    public static void LogUnexpectedServiceError<T>(this ILogger logger, Exception error) => LogUnexpectedServiceError(logger, typeof(T), error);

    #endregion

    #region InvalidContentRoot Error (0x0002)

    public const int EVENT_ID_InvalidContentRootError = 0x0002;

    public static readonly EventId InvalidContentRootError = new(EVENT_ID_InvalidContentRootError, nameof(InvalidContentRootError));

    private static readonly Action<ILogger, string, Exception?> _invalidContentRootError = LoggerMessage.Define<string>(LogLevel.Error, InvalidContentRootError,
        "Invalid path for CDN Content root: {Path}.");
    /// <summary>
    /// Logs an InvalidContentRoot event with event code 0x0002.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The invalid path string.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogInvalidContentRootError(this ILogger logger, string path, Exception error) => _invalidContentRootError(logger, path, error);

    #endregion

    #region EVENT_ID_CannotCreateCdnContentRoot Error (0x0003)

    public const int EVENT_ID_CannotCreateCdnContentRootError = 0x0003;

    public static readonly EventId CannotCreateCdnContentRootError = new(EVENT_ID_CannotCreateCdnContentRootError, nameof(CannotCreateCdnContentRootError));

    private static readonly Action<ILogger, string, Exception?> _cannotCreateCdnContentRootError = LoggerMessage.Define<string>(LogLevel.Error, CannotCreateCdnContentRootError,
        "Canont create CDN content root folder: {Path}");
    /// <summary>
    /// Logs an CannotCreateCdnContentRoot event with event code 0x0003.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path that could not be created.</param>
    /// <param name="error">The optional exception that caused the event.</param>
    public static void LogCannotCreateCdnContentRootError(this ILogger logger, string path, Exception? error = null) => _cannotCreateCdnContentRootError(logger, path, error);

    #endregion

    #region UpstreamCdnNotFound Error (0x0004)

    public const int EVENT_ID_UpstreamCdnNotFoundError = 0x0004;

    public static readonly EventId UpstreamCdnNotFoundError = new(EVENT_ID_UpstreamCdnNotFoundError, nameof(UpstreamCdnNotFoundError));

    private static readonly Action<ILogger, string, Exception?> _upstreamCdnNotFoundError = LoggerMessage.Define<string>(LogLevel.Error, UpstreamCdnNotFoundError,
        "Upstream CDN Service with the name \"{UpstreamCdnName}\" was not found.");
    /// <summary>
    /// Logs an UpstreamCdnNotFound event with event code 0x0004.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="upstreamCdnName">The name of the upstream CDN that could not be found in the database.</param>
    /// <param name="error">The optional exception that caused the event.</param>
    public static void LogUpstreamCdnNotFoundError(this ILogger logger, string upstreamCdnName, Exception? error = null) => _upstreamCdnNotFoundError(logger, upstreamCdnName, error);

    #endregion

    #region UpstreamCdnNotSupported Error (0x0005)

    public const int EVENT_ID_UpstreamCdnNotSupportedError = 0x0005;

    public static readonly EventId UpstreamCdnNotSupportedError = new(EVENT_ID_UpstreamCdnNotSupportedError, nameof(UpstreamCdnNotSupportedError));

    private static readonly Action<ILogger, string, Exception?> _upstreamCdnNotSupportedError = LoggerMessage.Define<string>(LogLevel.Error, UpstreamCdnNotSupportedError,
        "Upstream CDN Service with the name \"{UpstreamCdnName}\" is not supported.");
    /// <summary>
    /// Logs an UpstreamCdnNotSupported event with event code 0x0005.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="upstreamCdnName">The name of the upstream CDN that is not supported.</param>
    /// <param name="error">The optional exception that caused the event.</param>
    public static void LogUpstreamCdnNotSupportedError(this ILogger logger, string upstreamCdnName, Exception? error = null) => _upstreamCdnNotSupportedError(logger, upstreamCdnName, error);

    #endregion

    #region ServiceTypeNotFound Error (0x0006)

    public const int EVENT_ID_ServiceTypeNotFoundError = 0x0006;

    public static readonly EventId ServiceTypeNotFoundError = new(EVENT_ID_ServiceTypeNotFoundError, nameof(ServiceTypeNotFoundError));

    private static readonly Action<ILogger, string, Exception?> _serviceTypeNotFoundError = LoggerMessage.Define<string>(LogLevel.Error, ServiceTypeNotFoundError,
        "Could not find service for {TypeFullName}.");
    /// <summary>
    /// Logs an ServiceTypeNotFound event with event code 0x0006.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="type">The type of service that could not be found.</param>
    /// <param name="error">The optional exception that caused the event.</param>
    public static void LogServiceTypeNotFoundError(this ILogger logger, Type type, Exception? error = null) => _serviceTypeNotFoundError(logger, type.FullName ?? type.Name, error);

    #endregion

    #region NoUpstreamCdnsFound Warning (0x0007)

    public const int EVENT_ID_NoUpstreamCdnsFoundWarning = 0x0007;

    public static readonly EventId NoUpstreamCdnsFoundWarning = new(EVENT_ID_NoUpstreamCdnsFoundWarning, nameof(NoUpstreamCdnsFoundWarning));

    private static readonly Action<ILogger, Exception?> _noUpstreamCdnsFoundWarning = LoggerMessage.Define(LogLevel.Warning, NoUpstreamCdnsFoundWarning,
        "No CDNs found.");
    /// <summary>
    /// Logs an NoUpstreamCdnsFound event with event code 0x0007.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    public static void LogNoUpstreamCdnsFoundWarning(this ILogger logger) => _noUpstreamCdnsFoundWarning(logger, null);

    #endregion

    #region UpstreamCdnNoActions Warning (0x0008)

    public const int EVENT_ID_UpstreamCdnNoActionsWarning = 0x0008;

    public static readonly EventId UpstreamCdnNoActionsWarning = new(EVENT_ID_UpstreamCdnNoActionsWarning, nameof(UpstreamCdnNoActionsWarning));

    private static readonly Action<ILogger, string, Exception?> _upstreamCdnNoActionsWarning = LoggerMessage.Define<string>(LogLevel.Warning, UpstreamCdnNoActionsWarning,
        "Nothing to do with upstream CDN service \"{UpstreamCdnName}\".");
    /// <summary>
    /// Logs an UpstreamCdnNoActions event with event code 0x0008.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="upstreamCdnName">The name of the upstream CDN.</param>
    public static void LogUpstreamCdnNoActionsWarning(this ILogger logger, string upstreamCdnName) => _upstreamCdnNoActionsWarning(logger, upstreamCdnName, null);

    #endregion

    #region NothingToDo Warning (0x0009)

    public const int EVENT_ID_NothingToDoWarning = 0x0009;

    public static readonly EventId NothingToDoWarning = new(EVENT_ID_NothingToDoWarning, nameof(NothingToDoWarning));

    private static readonly Action<ILogger, Exception?> _nothingToDoWarning = LoggerMessage.Define(LogLevel.Warning, NothingToDoWarning,
        "Nothing to do.");
    /// <summary>
    /// Logs an NothingToDo event with event code 0x0009.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    public static void LogNothingToDoWarning(this ILogger logger) => _nothingToDoWarning(logger, null);

    #endregion

    #region InvalidBaseUrl Error (0x000a)

    public const int EVENT_ID_InvalidBaseUrlError = 0x000a;

    public static readonly EventId InvalidBaseUrlError = new(EVENT_ID_InvalidBaseUrlError, nameof(InvalidBaseUrlError));

    private static readonly Action<ILogger, string, string, string, Exception?> _invalidBaseUrlError = LoggerMessage.Define<string, string, string>(LogLevel.Error, InvalidBaseUrlError,
        "Invalid base URL for {UpstreamCdnType} ({UpstreamCdnName}): \"{URIstring}\" is not an absolute http or https URL.");
    /// <summary>
    /// Logs an InvalidBaseUrl event with event code 0x000a.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="upstreamCdnType">The type of the upstream CDN.</param>
    /// <param name="upstreamCdnName">The identifier name of the upstream CDN.</param>
    /// <param name="uriString">The invalid URI string.</param>
    /// <param name="error">The optional exception that caused the event.</param>
    public static void LogInvalidBaseUrlError(this ILogger logger, Type upstreamCdnType, string upstreamCdnName, string uriString, Exception? error = null) => _invalidBaseUrlError(logger, upstreamCdnType.FullName ?? upstreamCdnType.Name, upstreamCdnName, uriString, error);

    /// <summary>
    /// Logs an InvalidBaseUrl event with event code 0x000a.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="upstreamCdnName">The identifier name of the upstream CDN.</param>
    /// <param name="uriString">The invalid URI string.</param>
    /// <param name="error">The optional exception that caused the event.</param>
    /// <typeparam name="T">The type of the upstream CDN.</typeparam>
    public static void LogInvalidBaseUrlError<T>(this ILogger logger, string upstreamCdnName, string uriString, Exception? error = null) => LogInvalidBaseUrlError(logger, typeof(T), upstreamCdnName, uriString, error);

    #endregion

    #region LocalLibraryNotFound Warning (0x000b)

    public const int EVENT_ID_LocalLibraryNotFoundWarning = 0x000b;

    public static readonly EventId LocalLibraryNotFoundWarning = new(EVENT_ID_LocalLibraryNotFoundWarning, nameof(LocalLibraryNotFoundWarning));

    private static readonly Action<ILogger, string, Exception?> _localLibraryNotFoundWarning = LoggerMessage.Define<string>(LogLevel.Warning, LocalLibraryNotFoundWarning,
        "Local library named \"{LibraryName}\" not found.");
    /// <summary>
    /// Logs an LocalLibraryNotFound event with event code 0x000b.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="libraryName">The name of the local library that could not be found.</param>
    public static void LogLocalLibraryNotFoundWarning(this ILogger logger, string libraryName) => _localLibraryNotFoundWarning(logger, libraryName, null);

    #endregion

    #region CdnLibraryNotFound Warning (0x000c)

    public const int EVENT_ID_CdnLibraryNotFoundWarning = 0x000c;

    public static readonly EventId CdnLibraryNotFoundWarning = new(EVENT_ID_CdnLibraryNotFoundWarning, nameof(CdnLibraryNotFoundWarning));

    private static readonly Action<ILogger, string, string, Exception?> _cdnLibraryNotFoundWarning = LoggerMessage.Define<string, string>(LogLevel.Warning, CdnLibraryNotFoundWarning,
        "No local library named \"{LibraryName}\" exists from upstream CDN service \"{UpstreamCdnName}\".");
    /// <summary>
    /// Logs an CdnLibraryNotFound event with event code 0x000c.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="libraryName">The name of the library that could not be found.</param>
    /// <param name="upstreamCdnName">The name of the upstream CDN.</param>
    public static void LogCdnLibraryNotFoundWarning(this ILogger logger, string libraryName, string upstreamCdnName) => _cdnLibraryNotFoundWarning(logger, libraryName, upstreamCdnName, null);

    #endregion

    #region LocalLibraryAlreadyExists Warning (0x000d)

    public const int EVENT_ID_LocalLibraryAlreadyExistsWarning = 0x000d;

    public static readonly EventId LocalLibraryAlreadyExistsWarning = new(EVENT_ID_LocalLibraryAlreadyExistsWarning, nameof(LocalLibraryAlreadyExistsWarning));

    private static readonly Action<ILogger, string, Exception?> _localLibraryAlreadyExistsWarning = LoggerMessage.Define<string>(LogLevel.Warning, LocalLibraryAlreadyExistsWarning,
        "Local Library \"{LibraryName}\" has already been retreived.");
    /// <summary>
    /// Logs an LocalLibraryAlreadyExists event with event code 0x000d.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="libraryName">The name of the local library.</param>
    public static void LogLocalLibraryAlreadyExistsWarning(this ILogger logger, string libraryName) => _localLibraryAlreadyExistsWarning(logger, libraryName, null);

    #endregion

    #region CdnLibraryAlreadyExists Warning (0x000e)

    public const int EVENT_ID_CdnLibraryAlreadyExistsWarning = 0x000e;

    public static readonly EventId CdnLibraryAlreadyExistsWarning = new(EVENT_ID_CdnLibraryAlreadyExistsWarning, nameof(CdnLibraryAlreadyExistsWarning));

    private static readonly Action<ILogger, string, string, Exception?> _cdnLibraryAlreadyExistsWarning = LoggerMessage.Define<string, string>(LogLevel.Warning, CdnLibraryAlreadyExistsWarning,
        "Library \"{LibraryName}\" has already been retreived from \"{UpstreamCdnName}\".");
    /// <summary>
    /// Logs an CdnLibraryAlreadyExists event with event code 0x000e.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="libraryName">The name of the upstream library record that already exists.</param>
    /// <param name="upstreamCdnName">The name of the upstream CDN.</param>
    public static void LogCdnLibraryAlreadyExistsWarning(this ILogger logger, string libraryName, string upstreamCdnName) => _cdnLibraryAlreadyExistsWarning(logger, libraryName, upstreamCdnName, null);

    #endregion

    #region UnsupportedSwitch Warning (0x000f)

    public const int EVENT_ID_UnsupportedSwitchWarning = 0x000f;

    public static readonly EventId UnsupportedSwitchWarning = new(EVENT_ID_UnsupportedSwitchWarning, nameof(UnsupportedSwitchWarning));

    private static readonly Action<ILogger, string, string, Exception?> _mutuallyExclusiveSwitchWarning1 = LoggerMessage.Define<string, string>(LogLevel.Warning, UnsupportedSwitchWarning,
        "The {SwitchName} switch cannot be used with the {MutuallyExclusive} switch.");
    private static readonly Action<ILogger, string, string, string, Exception?> _mutuallyExclusiveSwitchWarning2 = LoggerMessage.Define<string, string, string>(LogLevel.Warning, UnsupportedSwitchWarning,
        "The {SwitchName} switch cannot be used with the {MutuallyExclusive} switch is set to {Value}.");
    /// <summary>
    /// Logs an UnsupportedSwitch event with event code 0x000f.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="switchName">The name of the target command line switch.</param>
    /// <param name="mutuallyExclusiveSwitch">The name of the command line switch that can't be used with the target switch.</param>
    public static void LogMutuallyExclusiveSwitchWarning(this ILogger logger, string switchName, string mutuallyExclusiveSwitch) => _mutuallyExclusiveSwitchWarning1(logger, switchName, mutuallyExclusiveSwitch, null);

    public static void LogMutuallyExclusiveSwitchWarning(this ILogger logger, string switchName, string mutuallyExclusiveSwitch, string switchValue) => _mutuallyExclusiveSwitchWarning2(logger, switchName, mutuallyExclusiveSwitch, switchValue, null);

    #endregion

    #region RequiredDependentParameter Warning (0x0010)

    public const int EVENT_ID_RequiredDependentParameterWarning = 0x0010;

    public static readonly EventId RequiredDependentParameterWarning = new(EVENT_ID_RequiredDependentParameterWarning, nameof(RequiredDependentParameterWarning));

    private static readonly Action<ILogger, string, string, Exception?> _requiredDependentParameterWarning1 = LoggerMessage.Define<string, string>(LogLevel.Warning, RequiredDependentParameterWarning,
        "The {DependentSwitch} switch is required when the {SwitchName} switch is present.");
    private static readonly Action<ILogger, string, string, string, Exception?> _requiredDependentParameterWarning2 = LoggerMessage.Define<string, string, string>(LogLevel.Warning, RequiredDependentParameterWarning,
        "The {DependentSwitch} switch is required when the {SwitchName} switch is set to {Value}.");
    /// <summary>
    /// Logs an RequiredDependentParameter event with event code 0x0010.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dependentSwitch">The dependent switch name.</param>
    /// <param name="switchName">The target switch name.</param>
    public static void LogRequiredDependentParameterWarning(this ILogger logger, string dependentSwitch, string switchName) => _requiredDependentParameterWarning1(logger, dependentSwitch, switchName, null);

    public static void LogRequiredDependentParameterWarning(this ILogger logger, string dependentSwitch, string switchName, string switchValue) => _requiredDependentParameterWarning2(logger, dependentSwitch, switchName, switchValue, null);

    #endregion

    #region RequiredAltDependentParameter Warning (0x0011)

    public const int EVENT_ID_RequiredAltDependentParameterWarning = 0x0011;

    public static readonly EventId RequiredAltDependentParameterWarning = new(EVENT_ID_RequiredAltDependentParameterWarning, nameof(RequiredAltDependentParameterWarning));

    private static readonly Action<ILogger, string, string, string, Exception?> _requiredAltDependentParameterWarning1 = LoggerMessage.Define<string, string, string>(LogLevel.Warning, RequiredAltDependentParameterWarning,
        "The {DependentSwitch1} or {DependentSwitch2} switch is required when the {SwitchName} switch is present.");

    private static readonly Action<ILogger, string, string, string, string, Exception?> _requiredAltDependentParameterWarning2 = LoggerMessage.Define<string, string, string, string>(LogLevel.Warning, RequiredAltDependentParameterWarning,
        "The {DependentSwitch} is required with the value {Value1} or {Value2} switch is required when the {SwitchName} switch is present.");

    private static readonly Action<ILogger, string, string, string, string, Exception?> _requiredAltDependentParameterWarning3 = LoggerMessage.Define<string, string, string, string>(LogLevel.Warning, RequiredAltDependentParameterWarning,
        "The {SwitchValues} switch or the {LastDependendSwitch} with the value {DependendValue} required when the {SwitchName} switch is present.");

    /// <summary>
    /// Logs an RequiredAltDependentParameter event with event code 0x0011.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dependentSwitch1">The first depenent switch name.</param>
    /// <param name="dependentSwitch2">The second dependent switch name.</param>
    /// <param name="switchName">The target switch name.</param>
    public static void LogRequiredAltDependentParameterWarning(this ILogger logger, string dependentSwitch1, string dependentSwitch2, string switchName) => _requiredAltDependentParameterWarning1(logger, dependentSwitch1, dependentSwitch2, switchName, null);

    // Upstream, RemoveLibrary, ReloadExistingVersions, GetNewVersions, AddLibrary, Show, SHOW_Files
    public static void LogRequiredAltDependentParameterWarning(this ILogger logger, string dependentSwitch, string dependendValue1, string dependendValue2, string switchName) => _requiredAltDependentParameterWarning2(logger, dependentSwitch, dependendValue1, dependendValue2, switchName, null);

    public static void LogRequiredAltDependentParameterWarning(this ILogger logger, string lastDependentSwitch, string dependendValue, string switchName, params string[] otherDependendSwitches) =>
        _requiredAltDependentParameterWarning3(logger, $"{string.Join(", ", otherDependendSwitches.SkipLast(1))} or {otherDependendSwitches[^1]}", lastDependentSwitch, dependendValue, switchName, null);

    #endregion

    #region InvalidParameterValue Warning (0x0012)

    public const int EVENT_ID_InvalidParameterValueWarning = 0x0012;

    public static readonly EventId InvalidParameterValueWarning = new(EVENT_ID_InvalidParameterValueWarning, nameof(InvalidParameterValueWarning));

    private static readonly Action<ILogger, string, string, Exception?> _invalidParameterValueWarning = LoggerMessage.Define<string, string>(LogLevel.Warning, InvalidParameterValueWarning,
        "Invalid value for the {SwitchName} switch ({ParameterValue}).");

    /// <summary>
    /// Logs an InvalidParameterValue event with event code 0x0012.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="switchName">The name of the command line switch.</param>
    /// <param name="parameterValue">The value assigned to the command line switch.</param>
    public static void LogInvalidParameterValueWarning(this ILogger logger, string switchName, string parameterValue) => _invalidParameterValueWarning(logger, switchName, parameterValue, null);

    #endregion

    #region ValidatingEntity Trace (0x0013)

    public const int EVENT_ID_ValidatingEntityTrace = 0x0013;

    public static readonly EventId ValidatingEntityTrace = new(EVENT_ID_ValidatingEntityTrace, nameof(ValidatingEntityTrace));

    private static readonly Action<ILogger, EntityState, string, object, Exception?> _validatingEntityTrace = LoggerMessage.Define<EntityState, string, object>(LogLevel.Trace, ValidatingEntityTrace,
        "Validating {State} {Name}: {Entity}");

    /// <summary>
    /// Logs an ValidatingEntity event with event code 0x0013.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="state">The entity state while being validated.</param>
    /// <param name="metadata">The entity metadata.</param>
    /// <param name="entity">The entity object.</param>
    public static void LogValidatingEntityTrace(this ILogger logger, EntityState state, IEntityType metadata, object entity) => _validatingEntityTrace(logger, state,
        metadata.DisplayName().ToTrimmedOrDefaultIfEmpty(() => metadata.Name.ToTrimmedOrDefaultIfEmpty(() => entity.GetType().FullName!)), entity, null);

    #endregion

    #region EntityValidation Error (0x0014)

    public const int EVENT_ID_EntityValidationFailure = 0x0014;

    public static readonly EventId EntityValidationFailure = new(EVENT_ID_EntityValidationFailure, nameof(EntityValidationFailure));

    private static readonly Action<ILogger, string, string, object, Exception?> _entityValidationFailure1 = LoggerMessage.Define<string, string, object>(LogLevel.Error, EntityValidationFailure,
        "Error Validating {Name} ({ValidationMessage}) {Entity}");

    private static readonly Action<ILogger, string, string, string, object, Exception?> _entityValidationFailure2 = LoggerMessage.Define<string, string, string, object>(LogLevel.Error, EntityValidationFailure,
        "Error Validating {Name} [{Properties}] ({ValidationMessage}) {Entity}");

    /// <summary>
    /// Logs an EntityValidation event with event code 0x0014.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="validationException">The exception that caused the event.</param>
    /// <param name="metadata">The entity metadata.</param>
    /// <param name="entity">The entity object.</param>
    public static void LogEntityValidationFailure(this ILogger logger, ValidationException validationException, IEntityType metadata, object entity)
    {
        IEnumerable<string> memberNames = validationException.ValidationResult.MemberNames.TrimmedNotEmptyValues();
        string name = metadata.DisplayName().ToTrimmedOrDefaultIfEmpty(() => metadata.Name.ToTrimmedOrDefaultIfEmpty(() => entity.GetType().FullName!));
        string message = validationException.ValidationResult.ErrorMessage.ToTrimmedOrDefaultIfEmpty(() => validationException.Message.ToTrimmedOrEmptyIfNull());
        if (message.Length == 0)
            _entityValidationFailure1(logger, name, memberNames.Any() ? $"Validation error on {string.Join(", ", memberNames)}" : "Validation Failure", entity, validationException);
        else if (memberNames.Any())
            _entityValidationFailure1(logger, name, message, entity, validationException);
        else
            _entityValidationFailure2(logger, name, string.Join(", ", memberNames), message, entity, validationException);
    }

    #endregion

    #region ValidationSucceeded Trace (0x0015)

    public const int EVENT_ID_ValidationSucceededTrace = 0x0015;

    public static readonly EventId ValidationSucceededTrace = new(EVENT_ID_ValidationSucceededTrace, nameof(ValidationSucceededTrace));

    private static readonly Action<ILogger, EntityState, string, object, Exception?> _validationSucceededTrace = LoggerMessage.Define<EntityState, string, object>(LogLevel.Trace, ValidationSucceededTrace,
        "Validation for {State} {Name} succeeded: {Entity}");

    /// <summary>
    /// Logs an ValidationSucceeded event with event code 0x0015.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="state">The entity state during validation.</param>
    /// <param name="metadata">The entity metadata.</param>
    /// <param name="entity">The entity object.</param>
    public static void LogValidationSucceededTrace(this ILogger logger, EntityState state, IEntityType metadata, object entity) => _validationSucceededTrace(logger, state,
        metadata.DisplayName().ToTrimmedOrDefaultIfEmpty(() => metadata.Name.ToTrimmedOrDefaultIfEmpty(() => entity.GetType().FullName!)), entity, null);

    #endregion

    #region DbSaveChangeCompleted Trace (0x0016)

    public const int EVENT_ID_DbSaveChangeCompletedTrace = 0x0016;

    public static readonly EventId DbSaveChangeCompletedTrace = new(EVENT_ID_DbSaveChangeCompletedTrace, nameof(DbSaveChangeCompletedTrace));

    private static readonly Action<ILogger, string, int, Exception?> _dbSaveChangeCompletedTrace = LoggerMessage.Define<string, int>(LogLevel.Trace, DbSaveChangeCompletedTrace,
        "{MethodSignature} completed. Returning {ReturnValue}.");

    /// <summary>
    /// Logs an DbSaveChangeCompleted event with event code 0x0016.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="isAsync">Indicates whether the save method was asyncrhonous.</param>
    /// <param name="acceptAllChangesOnSuccess">The value of the <c>acceptAllChangesOnSuccess</c> parameter or <see langword="null" /> if the save method did not include that parameter.</param>
    /// <param name="returnValue">The value being returned.</param>
    public static void LogDbSaveChangeCompletedTrace(this ILogger logger, bool isAsync, bool? acceptAllChangesOnSuccess, int returnValue) => _dbSaveChangeCompletedTrace(logger, isAsync ?
        (acceptAllChangesOnSuccess.HasValue ? $"SaveChangesAsync({acceptAllChangesOnSuccess.Value})" : "SaveChangesAsync()") :
        acceptAllChangesOnSuccess.HasValue ? $"SaveChanges({acceptAllChangesOnSuccess.Value})" : "SaveChanges()", returnValue, null);

    #endregion

    #region UsingConnectionString Trace (0x0017)

    public const int EVENT_ID_UsingConnectionStringTrace = 0x0017;

    public static readonly EventId UsingConnectionStringTrace = new(EVENT_ID_UsingConnectionStringTrace, nameof(UsingConnectionStringTrace));

    private static readonly Action<ILogger, string, Exception?> _usingConnectionStringTrace = LoggerMessage.Define<string>(LogLevel.Trace, UsingConnectionStringTrace,
        "Using connection string \"{ConnectionString}\".");

    /// <summary>
    /// Logs an UsingConnectionString event with event code 0x0017.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="connectionString">The connection string being used for the database connection.</param>
    public static void LogUsingConnectionStringTrace(this ILogger logger, string connectionString) => _usingConnectionStringTrace(logger, connectionString, null);

    #endregion

    #region InitializingNewDatabase Information (0x0018)

    public const int EVENT_ID_InitializingNewDatabase = 0x0018;

    public static readonly EventId InitializingNewDatabase = new(EVENT_ID_InitializingNewDatabase, nameof(InitializingNewDatabase));

    private static readonly Action<ILogger, string, Exception?> _initializingNewDatabase = LoggerMessage.Define<string>(LogLevel.Information, InitializingNewDatabase,
        "Initializing new database \"{DataSource}\".");

    /// <summary>
    /// Logs an InitializingNewDatabase event with event code 0x0018.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dataSource">The path for the database file.</param>
    public static void LogInitializingNewDatabase(this ILogger logger, string dataSource) => _initializingNewDatabase(logger, dataSource, null);

    #endregion

    #region CriticalQueryExecution Error (0x0019)

    public const int EVENT_ID_CriticalQueryExecutionError = 0x0019;

    public static readonly EventId CriticalQueryExecutionError = new(EVENT_ID_CriticalQueryExecutionError, nameof(CriticalQueryExecutionError));

    private static readonly Action<ILogger, string, string, Exception?> _criticalQueryExecutionError = LoggerMessage.Define<string, string>(LogLevel.Critical, CriticalQueryExecutionError,
        "Error executing query '{SqlQuery}': {ErrorMessage}");

    /// <summary>
    /// Logs an QueryExecution event with event code 0x0019.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="sqlQuery">The SQL query that failed.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogCriticalQueryExecutionError(this ILogger logger, string sqlQuery, Exception error) => _criticalQueryExecutionError(logger, sqlQuery,
        error.Message.ToTrimmedOrDefaultIfEmpty(() =>
        {
            Type t = error.GetType();
            return t.FullName.ToTrimmedOrDefaultIfEmpty(() => t.Name);
        }), error);

    #endregion

    #region NoLibraryNameSpecified Warning (0x001a)

    public const int EVENT_ID_NoLibraryNameSpecifiedWarning = 0x001a;

    public static readonly EventId NoLibraryNameSpecifiedWarning = new(EVENT_ID_NoLibraryNameSpecifiedWarning, nameof(NoLibraryNameSpecifiedWarning));

    private static readonly Action<ILogger, string, string, string, Exception?> _noLibraryNameSpecifiedWarning = LoggerMessage.Define<string, string, string>(LogLevel.Warning, NoLibraryNameSpecifiedWarning,
        "At least one library name must be specified with the --{LibrarySwitchName} switch when --{ShowSwitchName}={ShowSwitchValue} is used.");

    /// <summary>
    /// Logs an NoLibraryNameSpecified event with event code 0x001a.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="showSwitchValue">The name of the show value.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogNoLibraryNameSpecifiedWarning(this ILogger logger, string showSwitchValue) => _noLibraryNameSpecifiedWarning(logger, nameof(Config.AppSettings.Library), nameof(Config.AppSettings.Show), showSwitchValue, null);

    #endregion

    #region Scopes

    #region ExecuteMethod

    private static readonly Func<ILogger, string, IDisposable?> _executeMethodScope = LoggerMessage.DefineScope<string>("Execute method {MethodName}()");

    /// <summary>
    /// Formats the ExecuteMethod message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginExecuteMethodScope(this ILogger logger, string methodName) => _executeMethodScope(logger, methodName);

    private static readonly Func<ILogger, string, string, object?, IDisposable?> _executeMethodScope1 = LoggerMessage.DefineScope<string, string, object?>("Execute method {MethodName}({ParamName}: {ParamValue})");

    /// <summary>
    /// Formats the ExecuteMethod message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="paramName">The name of the method parameter.</param>
    /// <param name="paramValue">The value of the method parameter.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginExecuteMethodScope(this ILogger logger, string methodName, string paramName, object? paramValue) => _executeMethodScope1(logger, methodName, paramName, paramValue);

    #endregion

    #endregion
}
