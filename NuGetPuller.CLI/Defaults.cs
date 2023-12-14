using System.Diagnostics.CodeAnalysis;

namespace NuGetPuller.CLI;

public static class Defaults
{
    public const ConsoleColor BackgroundColor = ConsoleColor.Black;
    public const ConsoleColor ErrorColor = ConsoleColor.Red;
    public const ConsoleColor WarningColor = ConsoleColor.Yellow;
    public const ConsoleColor InfoColor = ConsoleColor.White;
    public const ConsoleColor VerboseColor = ConsoleColor.Cyan;

    internal static void ConsoleWriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string message)
    {
        if (Console.ForegroundColor != foregroundColor) Console.ForegroundColor = foregroundColor;
        if (Console.BackgroundColor != backgroundColor) Console.BackgroundColor = backgroundColor;
        Console.WriteLine(message);
    }

    internal static void ConsoleWriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, [StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1)
    {
        if (Console.ForegroundColor != foregroundColor) Console.ForegroundColor = foregroundColor;
        if (Console.BackgroundColor != backgroundColor) Console.BackgroundColor = backgroundColor;
        Console.WriteLine(format, arg0, arg1);
    }
    internal static void ConsoleWriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, [StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1, object? arg2)
    {
        if (Console.ForegroundColor != foregroundColor) Console.ForegroundColor = foregroundColor;
        if (Console.BackgroundColor != backgroundColor) Console.BackgroundColor = backgroundColor;
        Console.WriteLine(format, arg0, arg1, arg2);
    }
    internal static void ConsoleWriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, [StringSyntax("CompositeFormat")] string format, params object?[]? arg)
    {
        if (Console.ForegroundColor != foregroundColor) Console.ForegroundColor = foregroundColor;
        if (Console.BackgroundColor != backgroundColor) Console.BackgroundColor = backgroundColor;
        Console.WriteLine(format, arg);
    }

    internal static void ConsoleWriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, [StringSyntax("CompositeFormat")] string format, object? arg0)
    {
        if (Console.ForegroundColor != foregroundColor) Console.ForegroundColor = foregroundColor;
        if (Console.BackgroundColor != backgroundColor) Console.BackgroundColor = backgroundColor;
        Console.WriteLine(format, arg0);
    }

    internal static void WriteConsoleError(string message) => ConsoleWriteLine(ErrorColor, BackgroundColor, message);

    internal static void WriteConsoleError([StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1) =>
        ConsoleWriteLine(ErrorColor, BackgroundColor, format, arg0, arg1);

    internal static void WriteConsoleError([StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1, object? arg2) =>
        ConsoleWriteLine(ErrorColor, BackgroundColor, format, arg0, arg1, arg2);

    internal static void WriteConsoleError([StringSyntax("CompositeFormat")] string format, params object?[]? arg) =>
        ConsoleWriteLine(ErrorColor, BackgroundColor, format, arg);

    internal static void WriteConsoleError([StringSyntax("CompositeFormat")] string format, object? arg0) =>
        ConsoleWriteLine(ErrorColor, BackgroundColor, format, arg0);

    internal static void WriteConsoleWarning(string message) => ConsoleWriteLine(WarningColor, BackgroundColor, message);

    internal static void WriteConsoleWarning([StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1) =>
        ConsoleWriteLine(WarningColor, BackgroundColor, format, arg0, arg1);

    internal static void WriteConsoleWarning([StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1, object? arg2) =>
        ConsoleWriteLine(WarningColor, BackgroundColor, format, arg0, arg1, arg2);

    internal static void WriteConsoleWarning([StringSyntax("CompositeFormat")] string format, params object?[]? arg) =>
        ConsoleWriteLine(WarningColor, BackgroundColor, format, arg);

    internal static void WriteConsoleWarning([StringSyntax("CompositeFormat")] string format, object? arg0) =>
        ConsoleWriteLine(WarningColor, BackgroundColor, format, arg0);

    internal static void WriteConsoleInfo(string message) => ConsoleWriteLine(InfoColor, BackgroundColor, message);

    internal static void WriteConsoleInfo([StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1) =>
        ConsoleWriteLine(InfoColor, BackgroundColor, format, arg0, arg1);

    internal static void WriteConsoleInfo([StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1, object? arg2) =>
        ConsoleWriteLine(InfoColor, BackgroundColor, format, arg0, arg1, arg2);

    internal static void WriteConsoleInfo([StringSyntax("CompositeFormat")] string format, params object?[]? arg) =>
        ConsoleWriteLine(InfoColor, BackgroundColor, format, arg);

    internal static void WriteConsoleInfo([StringSyntax("CompositeFormat")] string format, object? arg0) =>
        ConsoleWriteLine(InfoColor, BackgroundColor, format, arg0);

    internal static void WriteConsoleVerbose(string message) => ConsoleWriteLine(InfoColor, VerboseColor, message);

    internal static void WriteConsoleVerbose([StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1) =>
        ConsoleWriteLine(InfoColor, VerboseColor, format, arg0, arg1);

    internal static void WriteConsoleVerbose([StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1, object? arg2) =>
        ConsoleWriteLine(InfoColor, VerboseColor, format, arg0, arg1, arg2);

    internal static void WriteConsoleVerbose([StringSyntax("CompositeFormat")] string format, params object?[]? arg) =>
        ConsoleWriteLine(InfoColor, VerboseColor, format, arg);

    internal static void WriteConsoleVerbose([StringSyntax("CompositeFormat")] string format, object? arg0) =>
        ConsoleWriteLine(InfoColor, VerboseColor, format, arg0);
}
