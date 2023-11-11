using Microsoft.Extensions.Logging;

namespace NuGetAirGap;

public class NugetLogWrapper : NuGet.Common.ILogger
{
    private readonly ILogger _underlyingLogger;

    public NugetLogWrapper(ILogger logger) => _underlyingLogger = logger;

    public void Log(NuGet.Common.LogLevel level, string data)
    {
        switch (level)
        {
            case NuGet.Common.LogLevel.Debug:
                _underlyingLogger.LogNugetDebugMessage(data);
                break;
            case NuGet.Common.LogLevel.Verbose:
                _underlyingLogger.LogNugetVerboseMessage(data);
                break;
            case NuGet.Common.LogLevel.Information:
                _underlyingLogger.LogNugetInformationMessage(data);
                break;
            case NuGet.Common.LogLevel.Minimal:
                _underlyingLogger.LogNugetMinimalMessage(data);
                break;
            case NuGet.Common.LogLevel.Error:
                _underlyingLogger.LogNuGetError(data);
                break;
            default:
                _underlyingLogger.LogNugetWarning(data);
                break;
        }
    }

    public void Log(NuGet.Common.ILogMessage message)
    {
        switch (message.Level)
        {
            case NuGet.Common.LogLevel.Debug:
                _underlyingLogger.LogNugetDebugMessage($"{message.Message} ({message.Time})", message.Code);
                break;
            case NuGet.Common.LogLevel.Verbose:
                _underlyingLogger.LogNugetVerboseMessage($"{message.Message} ({message.Time})", message.Code);
                break;
            case NuGet.Common.LogLevel.Information:
                _underlyingLogger.LogNugetInformationMessage($"{message.Message} ({message.Time})", message.Code);
                break;
            case NuGet.Common.LogLevel.Minimal:
                _underlyingLogger.LogNugetMinimalMessage($"{message.Message} ({message.Time})", message.Code);
                break;
            case NuGet.Common.LogLevel.Error:
                switch (message.WarningLevel)
                {
                    case NuGet.Common.WarningLevel.Important:
                        _underlyingLogger.LogNuGetError($"[{message.WarningLevel:F}] {message.Message} ({message.Time})", message.Code);
                        break;
                    case NuGet.Common.WarningLevel.Severe:
                        _underlyingLogger.LogCriticalNugetError($"[{message.WarningLevel:F}] {message.Message} ({message.Time})", message.Code);
                        break;
                    default:
                        _underlyingLogger.LogNuGetError($"{message.Message} ({message.Time})", message.Code);
                        break;
                }
                break;
            default:
                switch (message.WarningLevel)
                {
                    case NuGet.Common.WarningLevel.Important:
                    case NuGet.Common.WarningLevel.Severe:
                        _underlyingLogger.LogNugetWarning($"[{message.WarningLevel:F}] {message.Message} ({message.Time})", message.Code);
                        break;
                    default:
                        _underlyingLogger.LogNugetWarning($"{message.Message} ({message.Time})", message.Code);
                        break;
                }
                break;
        }
    }

    public Task LogAsync(NuGet.Common.LogLevel level, string data) => Task.Run(() => Log(level, data));

    public Task LogAsync(NuGet.Common.ILogMessage message) => Task.Run(() => Log(message));

    public void LogDebug(string data) => _underlyingLogger.LogNugetDebugMessage(data);

    public void LogError(string data) => _underlyingLogger.LogNuGetError(data);

    public void LogInformation(string data) => _underlyingLogger.LogNugetInformationMessage(data);

    public void LogInformationSummary(string data) => _underlyingLogger.LogNugetInformationMessage(data);

    public void LogMinimal(string data) => _underlyingLogger.LogNugetMinimalMessage(data);

    public void LogVerbose(string data) => _underlyingLogger.LogNugetVerboseMessage(data);

    public void LogWarning(string data) => _underlyingLogger.LogNugetWarning(data);
}
