using Microsoft.Extensions.Logging;

namespace NuGetPuller;

/// <summary>
/// Writes NuGet log events to a <see cref="ILogger"/>.
/// </summary>
/// <param name="logger">The underlying <see cref="ILogger"/>.</param>
public class NugetLogWrapper(ILogger logger) : NuGet.Common.ILogger
{
    private readonly ILogger _underlyingLogger = logger;

    public void Log(NuGet.Common.LogLevel level, string data)
    {
        switch (level)
        {
            case NuGet.Common.LogLevel.Debug:
                _underlyingLogger.NugetDebugMessage(data);
                break;
            case NuGet.Common.LogLevel.Verbose:
                _underlyingLogger.NugetVerboseMessage(data);
                break;
            case NuGet.Common.LogLevel.Information:
                _underlyingLogger.NugetInformationMessage(data);
                break;
            case NuGet.Common.LogLevel.Minimal:
                _underlyingLogger.NugetMinimalMessage(data);
                break;
            case NuGet.Common.LogLevel.Error:
                _underlyingLogger.NuGetErrorMessage(data);
                break;
            default:
                _underlyingLogger.NugetWarningMessage(data);
                break;
        }
    }

    public void Log(NuGet.Common.ILogMessage message)
    {
        switch (message.Level)
        {
            case NuGet.Common.LogLevel.Debug:
                _underlyingLogger.NugetDebugMessage($"{message.Message} ({message.Time})", message.Code);
                break;
            case NuGet.Common.LogLevel.Verbose:
                _underlyingLogger.NugetVerboseMessage($"{message.Message} ({message.Time})", message.Code);
                break;
            case NuGet.Common.LogLevel.Information:
                _underlyingLogger.NugetInformationMessage($"{message.Message} ({message.Time})", message.Code);
                break;
            case NuGet.Common.LogLevel.Minimal:
                _underlyingLogger.NugetMinimalMessage($"{message.Message} ({message.Time})", message.Code);
                break;
            case NuGet.Common.LogLevel.Error:
                switch (message.WarningLevel)
                {
                    case NuGet.Common.WarningLevel.Important:
                        _underlyingLogger.NuGetErrorMessage($"[{message.WarningLevel:F}] {message.Message} ({message.Time})", message.Code);
                        break;
                    case NuGet.Common.WarningLevel.Severe:
                        _underlyingLogger.CriticalNugetErrorMessage($"[{message.WarningLevel:F}] {message.Message} ({message.Time})", message.Code);
                        break;
                    default:
                        _underlyingLogger.NuGetErrorMessage($"{message.Message} ({message.Time})", message.Code);
                        break;
                }
                break;
            default:
                switch (message.WarningLevel)
                {
                    case NuGet.Common.WarningLevel.Important:
                    case NuGet.Common.WarningLevel.Severe:
                        _underlyingLogger.NugetWarningMessage($"[{message.WarningLevel:F}] {message.Message} ({message.Time})", message.Code);
                        break;
                    default:
                        _underlyingLogger.NugetWarningMessage($"{message.Message} ({message.Time})", message.Code);
                        break;
                }
                break;
        }
    }

    public Task LogAsync(NuGet.Common.LogLevel level, string data) => Task.Run(() => Log(level, data));

    public Task LogAsync(NuGet.Common.ILogMessage message) => Task.Run(() => Log(message));

    public void LogDebug(string data) => _underlyingLogger.NugetDebugMessage(data);

    public void LogError(string data) => _underlyingLogger.NuGetErrorMessage(data);

    public void LogInformation(string data) => _underlyingLogger.NugetInformationMessage(data);

    public void LogInformationSummary(string data) => _underlyingLogger.NugetInformationMessage(data);

    public void LogMinimal(string data) => _underlyingLogger.NugetMinimalMessage(data);

    public void LogVerbose(string data) => _underlyingLogger.NugetVerboseMessage(data);

    public void LogWarning(string data) => _underlyingLogger.NugetWarningMessage(data);
}
