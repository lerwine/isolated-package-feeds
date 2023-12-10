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

    #region Nuget Debug (0x001b)

    public const int EVENT_ID_NugetDebug = 0x001b;

    public static readonly EventId NugetDebug = new(EVENT_ID_NugetDebug, nameof(NugetDebug));

    private static readonly Action<ILogger, string, Exception?> _nugetDebugMessage1 = LoggerMessage.Define<string>(LogLevel.Debug, NugetDebug, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _nugetDebugMessage2 = LoggerMessage.Define<string, string, int>(LogLevel.Debug, NugetDebug, "NuGet {NugetID} ({Code}) Message: {Message}");

    private static readonly Action<ILogger, string, Exception?> _nugetVerboseMessage1 = LoggerMessage.Define<string>(LogLevel.Trace, NugetDebug, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _nugetVerboseMessage2 = LoggerMessage.Define<string, string, int>(LogLevel.Trace, NugetDebug, "NuGet {NugetID} ({Code}) Message: {Message}");

    /// <summary>
    /// Logs a <see cref="NuGet.Common.LogLevel.Debug"/> NugetDebug event with event code 0x001b.
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
    /// Logs a <see cref="NuGet.Common.LogLevel.Verbose"/> NugetDebug event with event code 0x001b.
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

    #region Nuget Message (0x001c)

    public const int EVENT_ID_NugetMessage = 0x001c;

    public static readonly EventId NugetMessage = new(EVENT_ID_NugetMessage, nameof(NugetMessage));

    private static readonly Action<ILogger, string, Exception?> _nugetInformationMessage1 = LoggerMessage.Define<string>(LogLevel.Information, NugetMessage, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _nugetInformationMessage2 = LoggerMessage.Define<string, string, int>(LogLevel.Information, NugetMessage, "NuGet {NugetID} ({Code}) Message: {Message}");

    private static readonly Action<ILogger, string, Exception?> _nugetMinimalMessage1 = LoggerMessage.Define<string>(LogLevel.Information, NugetMessage, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _nugetMinimalMessage2 = LoggerMessage.Define<string, string, int>(LogLevel.Information, NugetMessage, "NuGet {NugetID} ({Code}) Message: {Message}");

    /// <summary>
    /// Logs an <see cref="NuGet.Common.LogLevel.Information"/> NugetMessage event with event code 0x001b.
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
    /// Logs an <see cref="NuGet.Common.LogLevel.Minimal"/> NugetMessage event with event code 0x001b.
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

    #region Nuget Warning Message (0x001d)

    public const int EVENT_ID_NugetWarning = 0x001d;

    public static readonly EventId NugetWarning = new(EVENT_ID_NugetWarning, nameof(NugetWarning));

    private static readonly Action<ILogger, string, Exception?> _nugetWarning1 = LoggerMessage.Define<string>(LogLevel.Warning, NugetWarning, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _nugetWarning2 = LoggerMessage.Define<string, string, int>(LogLevel.Warning, NugetWarning, "NuGet {NugetID} ({Code}) Message: {Message}");

    /// <summary>
    /// Logs an NugetWarning event with event code 0x001d.
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

    #region NuGet Error Message (0x001e)

    public const int EVENT_ID_NuGetError = 0x001e;

    public static readonly EventId NuGetError = new(EVENT_ID_NuGetError, nameof(NuGetError));

    private static readonly Action<ILogger, string, Exception?> _nuGetError1 = LoggerMessage.Define<string>(LogLevel.Error, NuGetError, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _nuGetError2 = LoggerMessage.Define<string, string, int>(LogLevel.Error, NuGetError, "NuGet {NugetID} ({Code}) Message: {Message}");

    /// <summary>
    /// Logs a NuGetError event with event code 0x001e.
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

    #region Critical Nuget Error (0x001f)

    public const int EVENT_ID_CriticalNugetError = 0x001f;

    public static readonly EventId CriticalNugetError = new(EVENT_ID_CriticalNugetError, nameof(CriticalNugetError));

    private static readonly Action<ILogger, string, Exception?> _criticalNugetError1 = LoggerMessage.Define<string>(LogLevel.Critical, CriticalNugetError, "NuGet Message: {Message}");

    private static readonly Action<ILogger, string, string, int, Exception?> _criticalNugetError2 = LoggerMessage.Define<string, string, int>(LogLevel.Critical, CriticalNugetError, "NuGet {NugetID} ({Code}) Message: {Message}");

    /// <summary>
    /// Logs a CriticalNugetError event with event code 0x001f.
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

    #region Critical LocalNugetRepositorySecurity Error (0x0020)

    public const int EVENT_ID_LocalNugetRepositorySecurityError = 0x0020;

    public static readonly EventId LocalNugetRepositorySecurityError = new(EVENT_ID_LocalNugetRepositorySecurityError, nameof(LocalNugetRepositorySecurityError));

    private static readonly Action<ILogger, string, Exception?> _localNugetRepositorySecurityError = LoggerMessage.Define<string>(LogLevel.Critical, LocalNugetRepositorySecurityError,
        "Access denied while accessing local NuGet repository path {Path}");

    /// <summary>
    /// Logs an LocalNugetRepositorySecurityError event with event code 0x0020.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the local NuGet repository.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogLocalNugetRepositorySecurityError(this ILogger logger, string path, Exception? error) => _localNugetRepositorySecurityError(logger, path, error);

    #endregion

    #region Critical InvalidLocalNugetRepositoryPath Error (0x0021)

    public const int EVENT_ID_InvalidLocalNugetRepositoryPath = 0x0021;

    public static readonly EventId InvalidLocalNugetRepositoryPath = new(EVENT_ID_InvalidLocalNugetRepositoryPath, nameof(InvalidLocalNugetRepositoryPath));

    private static readonly Action<ILogger, string, Exception?> _invalidLocalNugetRepositoryPath = LoggerMessage.Define<string>(LogLevel.Critical, InvalidLocalNugetRepositoryPath,
        "Local NuGet repository path {Path} is invalid.");

    private static readonly Action<ILogger, string, Exception?> _localNugetRepositoryPathTooLong = LoggerMessage.Define<string>(LogLevel.Critical, InvalidLocalNugetRepositoryPath,
        "Local NuGet repository path {Path} is too long.");

    private static readonly Action<ILogger, string, Exception?> _localNugetRepositoryPathNotSubdirectory = LoggerMessage.Define<string>(LogLevel.Critical, InvalidLocalNugetRepositoryPath,
        "Local NuGet repository path {Path} does not refer to a subdirectory.");

    /// <summary>
    /// Logs an InvalidLocalNugetRepositoryPath event with event code 0x0021.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the local NuGet repository.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogInvalidLocalNugetRepositoryPath(this ILogger logger, string path, Exception error) => _invalidLocalNugetRepositoryPath(logger, path, error);

    /// <summary>
    /// Logs an InvalidLocalNugetRepositoryPath event with event code 0x0021.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the local NuGet repository.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogLocalNugetRepositoryPathTooLong(this ILogger logger, string path, PathTooLongException error) => _localNugetRepositoryPathTooLong(logger, path, error);

    /// <summary>
    /// Logs an InvalidLocalNugetRepositoryPath event with event code 0x0021.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the local NuGet repository.</param>
    public static void LogLocalNugetRepositoryPathNotSubdirectory(this ILogger logger, string path) => _localNugetRepositoryPathNotSubdirectory(logger, path, null);

    #endregion

    #region Critical LocalNugetRepositoryCreation Error (0x0022)

    public const int EVENT_ID_LocalNugetRepositoryCreationError = 0x0022;

    public static readonly EventId LocalNugetRepositoryCreationError = new(EVENT_ID_LocalNugetRepositoryCreationError, nameof(LocalNugetRepositoryCreationError));

    private static readonly Action<ILogger, string, Exception?> _localNugetRepositoryCreationError = LoggerMessage.Define<string>(LogLevel.Critical, LocalNugetRepositoryCreationError,
        "Unable to create local NuGet repository subdirectory {Path}");

    /// <summary>
    /// Logs an LocalNugetRepositoryCreationError event with event code 0x0022.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the local NuGet repository.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogLocalNugetRepositoryCreationError(this ILogger logger, string path, Exception? error) => _localNugetRepositoryCreationError(logger, path, error);

    #endregion

    #region Critical InvalidRemoteRepositoryUrl Error (0x0023)

    public const int EVENT_ID_InvalidRemoteRepositoryUrl = 0x0023;

    public static readonly EventId InvalidRemoteRepositoryUrl = new(EVENT_ID_InvalidRemoteRepositoryUrl, nameof(InvalidRemoteRepositoryUrl));

    private static readonly Action<ILogger, string, Exception?> _invalidRemoteRepositoryUrl = LoggerMessage.Define<string>(LogLevel.Critical, InvalidRemoteRepositoryUrl,
        "Remote NuGet repository URL {UriString} is invalid.");

    private static readonly Action<ILogger, string, Exception?> _remoteRepositoryIsLocal = LoggerMessage.Define<string>(LogLevel.Critical, InvalidRemoteRepositoryUrl,
        "Remote NuGet repository URL {URL} must not refer to a local path.");

    /// <summary>
    /// Logs an InvalidRemoteRepositoryUrl event with event code 0x0023.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uriString">The URI string for the remote NuGet repository.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogInvalidRemoteRepositoryUrl(this ILogger logger, string uriString, Exception error) => _invalidRemoteRepositoryUrl(logger, uriString, error);

    /// <summary>
    /// Logs an InvalidRemoteRepositoryUrl event with event code 0x0023.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The URI for the remote NuGet repository.</param>
    public static void LogRemoteRepositoryUrlIsLocal(this ILogger logger, string uri) => _remoteRepositoryIsLocal(logger, uri, null);

    #endregion

    #region GetNugetResourceFailure Error (0x0024)

    public const int EVENT_ID_GetNugetResourceFailure = 0x0024;

    public static readonly EventId GetNugetResourceFailure = new(EVENT_ID_GetNugetResourceFailure, nameof(GetNugetResourceFailure));

    private static readonly Action<ILogger, Type, Exception?> _getNugetResourceFailure = LoggerMessage.Define<Type>(LogLevel.Error, GetNugetResourceFailure,
        "Failed to get NuGet resource type {Type}");

    /// <summary>
    /// Logs an GetNugetResourceFailure event with event code 0x0024.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="type">The type of resource that was not found.</param>
    public static void LogGetNugetResourceFailure(this ILogger logger, Type type) => _getNugetResourceFailure(logger, type, null);

    #endregion

    #region GetNuGetMetaDataFailure Error (0x0025)

    public const int EVENT_ID_GetNuGetMetaDataFailure = 0x0025;

    public static readonly EventId GetNuGetMetaDataFailure = new(EVENT_ID_GetNuGetMetaDataFailure, nameof(GetNuGetMetaDataFailure));

    private static readonly Action<ILogger, string, string, bool, bool, Exception?> _getNuGetMetaDataFailure1 = LoggerMessage.Define<string, string, bool, bool>(LogLevel.Error, GetNuGetMetaDataFailure,
        "Failed to get NuGet MetaData for package with ID {PackageID} from {URL}: IncludePreRelease={IncludePreRelease}; IncludeUnlisted={IncludeUnlisted}");

    private static readonly Action<ILogger, string, string, NuGetVersion, Exception?> _getNuGetMetaDataFailure2 = LoggerMessage.Define<string, string, NuGetVersion>(LogLevel.Error, GetNuGetMetaDataFailure,
        "Failed to get NuGet MetaData for package with ID {PackageID} from {URL}: Version={Version}");

    /// <summary>
    /// Logs an GetNuGetMetaDataFailure event with event code 0x0025.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageID">The package identifier.</param>
    /// <param name="url">The source repository URL.</param>
    /// <param name="includePreRelease">Whether pre-release version were to be includd.</param>
    /// <param name="includeUnlisted">Whether unlisted version were to be included.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogGetNuGetMetaDataFailure(this ILogger logger, string packageID, string url, bool includePreRelease, bool includeUnlisted, Exception error) => _getNuGetMetaDataFailure1(logger, packageID, url, includePreRelease, includeUnlisted, error);

    /// <summary>
    /// Logs an GetNuGetMetaDataFailure event with event code 0x0025.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageID">The package identifier.</param>
    /// <param name="url">The source repository URL.</param>
    /// <param name="version">The package version.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogGetNuGetMetaDataFailure(this ILogger logger, string packageID, string url, NuGetVersion version, Exception error) => _getNuGetMetaDataFailure2(logger, packageID, url, version, error);

    #endregion

    #region GetAllNuGetVersionsFailure Error (0x0026)

    public const int EVENT_ID_GetAllNuGetVersionsFailure = 0x0026;

    public static readonly EventId GetAllNuGetVersionsFailure = new(EVENT_ID_GetAllNuGetVersionsFailure, nameof(GetAllNuGetVersionsFailure));

    private static readonly Action<ILogger, string, string, Exception?> _getAllNuGetVersionsFailure = LoggerMessage.Define<string, string>(LogLevel.Error, GetAllNuGetVersionsFailure, "Failed to get all NuGet version for package with ID {PackageID} from {URL}");

    /// <summary>
    /// Logs an GetAllNuGetVersionsFailure event with event code 0x0026.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageID">The package identifier.</param>
    /// <param name="url">The source repository URL.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogGetAllNuGetVersionsFailure(this ILogger logger, string packageID, string url, Exception error) => _getAllNuGetVersionsFailure(logger, packageID, url, error);

    #endregion

    #region GetNuGetDependencyInfoFailure Error (0x0027)

    public const int EVENT_ID_GetNuGetDependencyInfoFailure = 0x0027;

    public static readonly EventId GetNuGetDependencyInfoFailure = new(EVENT_ID_GetNuGetDependencyInfoFailure, nameof(GetNuGetDependencyInfoFailure));

    private static readonly Action<ILogger, string, NuGetVersion, string, Exception?> _getNuGetDependencyInfoFailure = LoggerMessage.Define<string, NuGetVersion, string>(LogLevel.Error, GetNuGetDependencyInfoFailure,
        "Failed to get NuGet dependency information for package with ID {PackageID} and version {Version} from {URL}");

    /// <summary>
    /// Logs an GetNuGetDependencyInfoFailure event with event code 0x0027.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageID">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="url">The source repository URL.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogGetNuGetDependencyInfoFailure(this ILogger logger, string packageID, NuGetVersion version, string url, Exception error) => _getNuGetDependencyInfoFailure(logger, packageID, version, url, error);

    #endregion

    #region DoesNuGetPackageExistFailure Error (0x0027)

    public const int EVENT_ID_DoesNuGetPackageExistFailure = 0x0027;

    public static readonly EventId DoesNuGetPackageExistFailure = new(EVENT_ID_DoesNuGetPackageExistFailure, nameof(DoesNuGetPackageExistFailure));

    private static readonly Action<ILogger, string, NuGetVersion, string, Exception?> _doesNuGetPackageExistFailure = LoggerMessage.Define<string, NuGetVersion, string>(LogLevel.Error, DoesNuGetPackageExistFailure,
        "Failed to get test whether NuGet package with ID {PackageID} and version {Version} exists from {URL}");

    /// <summary>
    /// Logs an DoesNuGetPackageExistFailure event with event code 0x0027.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageID">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="url">The source repository URL.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogDoesNuGetPackageExistFailure(this ILogger logger, string packageID, NuGetVersion version, string url, Exception error) => _doesNuGetPackageExistFailure(logger, packageID, version, url, error);

    #endregion

    #region NuGetPackageSearchFailure event logger message (0x0028)

    public const int EVENT_ID_NuGetPackageSearchFailure = 0x0028;

    public static readonly EventId NuGetPackageSearchFailure = new(EVENT_ID_NuGetPackageSearchFailure, nameof(NuGetPackageSearchFailure));

    private static readonly Action<ILogger, string, string, SearchFilter, int, int, Exception?> _nuGetPackageSearchFailure = LoggerMessage.Define<string, string, SearchFilter, int, int>(LogLevel.Error, NuGetPackageSearchFailure,
        "Message {SearchTerm} {URL} {Filters} {SkipCount} {TakeCount}");

    /// <summary>
    /// Logs a <see cref="LogLevel.Error"/> message for a NuGetPackageSearchFailure event with event code 0x0028.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="searchTerm">The first event parameter.</param>
    /// <param name="url">The second event parameter.</param>
    /// <param name="filters">The third event parameter.</param>
    /// <param name="skipCount">The fourth event parameter.</param>
    /// <param name="takeCount">The fifth event parameter.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogNuGetPackageSearchFailure(this ILogger logger, string searchTerm, string url, SearchFilter filters, int skipCount, int takeCount, Exception? exception = null) => _nuGetPackageSearchFailure(logger, searchTerm, url, filters, skipCount, takeCount, exception);

    #endregion

    #region GetNuGetDownloadResourceResultsFailure event logger message (0x0029)

    public const int EVENT_ID_GetNuGetDownloadResourceResultsFailure = 0x0029;

    public static readonly EventId GetNuGetDownloadResourceResultsFailure = new(EVENT_ID_GetNuGetDownloadResourceResultsFailure, nameof(GetNuGetDownloadResourceResultsFailure));

    private static readonly Action<ILogger, PackageIdentity, string, PackageDownloadContext, string, Exception?> _getNuGetDownloadResourceResultsFailure = LoggerMessage.Define<PackageIdentity, string, PackageDownloadContext, string>(LogLevel.Error, GetNuGetDownloadResourceResultsFailure,
        "Message {Identity} {URL} {DownloadContext} {GlobalPackagesFolder}");

    /// <summary>
    /// Logs a <see cref="LogLevel.Error"/> message for a GetNuGetDownloadResourceResultsFailure event with event code 0x0029.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="identity">The first event parameter.</param>
    /// <param name="url">The second event parameter.</param>
    /// <param name="downloadContext">The third event parameter.</param>
    /// <param name="globalPackagesFolder">The fourth event parameter.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogGetNuGetDownloadResourceResultsFailure(this ILogger logger, PackageIdentity identity, string url, PackageDownloadContext downloadContext, string globalPackagesFolder, Exception? exception = null) => _getNuGetDownloadResourceResultsFailure(logger, identity, url, downloadContext, globalPackagesFolder, exception);

    #endregion

    #region ListNuGetPackagesFailure event logger message (0x002a)

    public const int EVENT_ID_ListNuGetPackagesFailure = 0x002a;

    public static readonly EventId ListNuGetPackagesFailure = new(EVENT_ID_ListNuGetPackagesFailure, nameof(ListNuGetPackagesFailure));

    private static readonly Action<ILogger, string, string, bool, bool, bool, Exception?> _listNuGetPackagesFailure = LoggerMessage.Define<string, string, bool, bool, bool>(LogLevel.Error, ListNuGetPackagesFailure,
        "Failed to list NuGet packages ({SearchTerm}; {URL}; {PreRelease}; {AllVersions}; {IncludeDelisted}).");

    /// <summary>
    /// Logs a <see cref="LogLevel.Error"/> message for a ListNuGetPackagesFailure event with event code 0x002a.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="searchTerm">The first event parameter.</param>
    /// <param name="url">The second event parameter.</param>
    /// <param name="preRelease">The third event parameter.</param>
    /// <param name="allVersions">The fourth event parameter.</param>
    /// <param name="includeDelisted">The fifth event parameter.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogListNuGetPackagesFailure(this ILogger logger, string searchTerm, string url, bool preRelease, bool allVersions, bool includeDelisted, Exception? exception = null) => _listNuGetPackagesFailure(logger, searchTerm, url, preRelease, allVersions, includeDelisted, exception);

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

    #region RepositoryUrl

    private static readonly Func<ILogger, string, bool, IDisposable?> _repositoryUrlScope = LoggerMessage.DefineScope<string, bool>("Nuget Repossitory URL: {URL}; Is Local {IsLocal}");

    /// <summary>
    /// Formats the RepositoryUrl message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The name of the method.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginRepositoryUrlScope(this ILogger logger, string url, bool isLocal = false) => _repositoryUrlScope(logger, url, isLocal);

    #endregion

    #region GetNugetMetaData

    private static readonly Func<ILogger, string, string, bool, bool, IDisposable?> _getNugetMetaDataScope1 = LoggerMessage.DefineScope<string, string, bool, bool>("Get NuGet MetaData from {URL}: ID={PackageID}; IncludePrerelease={IncludePrerelease}; IncludeUnlisted={IncludeUnlisted}");

    private static readonly Func<ILogger, string, string, NuGetVersion, IDisposable?> _getNugetMetaDataScope2 = LoggerMessage.DefineScope<string, string, NuGetVersion>("Get NuGet MetaData from {URL}: ID={PackageID}; Version={Version}");

    /// <summary>
    /// Formats the GetNugetMetaData message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The source URL.</param>
    /// <param name="packageId">The package ID.</param>
    /// <param name="includePrerelease">Whether include pre-release packages.</param>
    /// <param name="includeUnlisted">Whether to include unlisted packages.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetNugetMetaDataScope(this ILogger logger, string url, string packageId, bool includePrerelease, bool includeUnlisted) => _getNugetMetaDataScope1(logger, url, packageId, includePrerelease, includeUnlisted);

    /// <summary>
    /// Formats the GetNugetMetaData message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The source URL.</param>
    /// <param name="packageId">The package ID.</param>
    /// <param name="version">The package version.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetNugetMetaDataScope(this ILogger logger, string url, string packageId, NuGetVersion version) => _getNugetMetaDataScope2(logger, url, packageId, version);

    #endregion

    #region GetNuGetPackageVersions

    private static readonly Func<ILogger, string, string, IDisposable?> _getNuGetPackageVersionsScope = LoggerMessage.DefineScope<string, string>("Get NuGet Package versions from {URL}: ID={PackageID}");

    /// <summary>
    /// Formats the GetNuGetPackageVersions message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package ID.</param>
    /// <param name="url">The source URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetNuGetPackageVersionsScope(this ILogger logger, string url, string packageId) => _getNuGetPackageVersionsScope(logger, url, packageId);

    #endregion

    #region FindNuGetPackageById

    private static readonly Func<ILogger, string, string, IDisposable?> _findNuGetPackageByIdScope = LoggerMessage.DefineScope<string, string>("Find NuGet Package by ID from {URL}: ID={PackageID}");

    /// <summary>
    /// Formats the FindNuGetPackageById message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="packageId">The package ID.</param>
    /// <param name="url">The source URL.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginFindNuGetPackageByIdScope(this ILogger logger, string url, string packageId) => _findNuGetPackageByIdScope(logger, url, packageId);

    #endregion

    #region ListNuGetPackages Scope

    private static readonly Func<ILogger, string, string, bool, bool, bool, IDisposable?> _listNuGetPackagesScope = LoggerMessage.DefineScope<string, string, bool, bool, bool>("List Packages {SearchTerm} {URL} {PreRelease} {AllVersions} {IncludeDelisted}");

    /// <summary>
    /// Formats the ListNuGetPackages message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="searchTerm">description</param>
    /// <param name="url">description</param>
    /// <param name="preRelease">description</param>
    /// <param name="allVersions">description</param>
    /// <param name="includeDelisted">description</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginListNuGetPackagesScope(this ILogger logger, string searchTerm, string url, bool preRelease, bool allVersions, bool includeDelisted) => _listNuGetPackagesScope(logger, searchTerm, url, preRelease, allVersions, includeDelisted);

    #endregion

    #region GetNuGetDownloadResourceResult Scope

    private static readonly Func<ILogger, PackageIdentity, string, PackageDownloadContext, string, IDisposable?> _getDownloadResourceResultScope = LoggerMessage.DefineScope<PackageIdentity, string, PackageDownloadContext, string>("Message {Identity} {URL} {DownloadContext} {GlobalPackageFolder}");

    /// <summary>
    /// Formats the GetNuGetDownloadResourceResult message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="identity">The first scope context parameter.</param>
    /// <param name="url">The second scope context parameter.</param>
    /// <param name="downloadContext">The third scope context parameter.</param>
    /// <param name="globalPackageFolder">The fourth scope context parameter.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginGetNuGetDownloadResourceResultScope(this ILogger logger, PackageIdentity identity, string url, PackageDownloadContext downloadContext, string globalPackageFolder) => _getDownloadResourceResultScope(logger, identity, url, downloadContext, globalPackageFolder);

    #endregion

    #region SearchNuGetPackage Scope

    private static readonly Func<ILogger, string, string, SearchFilter, int, int, IDisposable?> _searchNuGetPackageScope = LoggerMessage.DefineScope<string, string, SearchFilter, int, int>("Message {SearchTerm} {URL} {Filters} {Skip} {Take}");

    /// <summary>
    /// Formats the SearchNuGetPackage message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="searchTerm">The first scope context parameter.</param>
    /// <param name="url">The second scope context parameter.</param>
    /// <param name="filters">The third scope context parameter.</param>
    /// <param name="skip">The fourth scope context parameter.</param>
    /// <param name="take">The fifth scope context parameter.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginSearchNuGetPackageScope(this ILogger logger, string searchTerm, string url, SearchFilter filters, int skip, int take) => _searchNuGetPackageScope(logger, searchTerm, url, filters, skip, take);

    #endregion

    #endregion
}
