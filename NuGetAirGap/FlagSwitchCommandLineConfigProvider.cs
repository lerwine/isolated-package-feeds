using System.Collections.Immutable;
using Microsoft.Extensions.Configuration;

namespace NuGetAirGap;

/// <summary>
/// Based upon the <see cref="Microsoft.Extensions.Configuration.CommandLine.CommandLineConfigurationProvider"/>, adding support for stand-alone command line switches are not followed by a value. 
/// </summary>
public class FlagSwitchCommandLineConfigProvider : ConfigurationProvider
{
    private readonly Dictionary<string, string> _valueSwitchMappings = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _booleanwitchMappings = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// The command line arguments.
    /// </summary>
    protected ImmutableArray<string> Args { get; private set; }

    /// <summary>
    /// Creates a new command-line configuration provider.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <param name="booleanSwitchMappings">Maps stand-alone command line switches (not followed by a value) to boolean application settings.</param>
    /// <param name="switchValueMappings">Maps command line switch values to application settings.</param>
    public FlagSwitchCommandLineConfigProvider(ImmutableArray<string> args, IDictionary<string, string> booleanSwitchMappings, IDictionary<string, string>? switchValueMappings = null)
    {
        Args = args;
        ArgumentNullException.ThrowIfNull(booleanSwitchMappings);
        foreach (var mapping in booleanSwitchMappings)
        {
            // Only keys start with "--" or "-" are acceptable
            if (!mapping.Key.StartsWith("-") && !mapping.Key.StartsWith("--"))
                throw new ArgumentException("Invalid switch mappings key", nameof(switchValueMappings));

            if (_booleanwitchMappings.ContainsKey(mapping.Key))
                throw new ArgumentException("Duplicate switch mappings key", nameof(switchValueMappings));

            _booleanwitchMappings.Add(mapping.Key, mapping.Value);
        }

        if (switchValueMappings is not null)
            foreach (var mapping in switchValueMappings)
            {
                // Only keys start with "--" or "-" are acceptable
                if (!mapping.Key.StartsWith("-") && !mapping.Key.StartsWith("--"))
                    throw new ArgumentException("Invalid switch mappings key", nameof(switchValueMappings));

                if (_booleanwitchMappings.ContainsKey(mapping.Key) || _valueSwitchMappings.ContainsKey(mapping.Key))
                    throw new ArgumentException("Duplicate switch mappings key", nameof(switchValueMappings));

                _valueSwitchMappings.Add(mapping.Key, mapping.Value);
            }
    }

    /// <summary>
    /// Loads the configuration data from the command line args.
    /// </summary>
    public override void Load()
    {
        var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        var enumerator = Args.GetEnumerator();
        string value;
        string? key;
        if (_valueSwitchMappings is null || _valueSwitchMappings.Count == 0)
            while (enumerator.MoveNext())
            {
                var currentArg = enumerator.Current;
                var keyStartIndex = 0;

                if (currentArg.StartsWith("--"))
                    keyStartIndex = 2;
                else if (currentArg.StartsWith("-"))
                    keyStartIndex = 1;
                else if (currentArg.StartsWith("/"))
                {
                    // "/SomeSwitch" is equivalent to "--SomeSwitch" when interpreting switch mappings
                    // So we do a conversion to simplify later processing
                    currentArg = $"--{currentArg[1..]}";
                    keyStartIndex = 2;
                }

                var separator = currentArg.IndexOf('=');

                if (separator < 0)
                {
                    // If there is neither equal sign nor prefix in current arugment, it is an invalid format
                    if (keyStartIndex == 0)
                        throw new FormatException($"{enumerator.Current} is an invalid argument.");
                    if (_booleanwitchMappings.TryGetValue(currentArg, out key))
                        value = "true";
                    else
                    {
                        if (keyStartIndex == 1)
                            throw new FormatException($"Shortcut {enumerator.Current} not defined.");
                        // Otherwise, use the switch name directly as a key
                        key = currentArg[keyStartIndex..];
                        var previousKey = enumerator.Current;
                        if (!enumerator.MoveNext())
                            throw new FormatException($"{previousKey} is missing a value");

                        value = enumerator.Current;
                    }
                }
                else
                {
                    if (keyStartIndex == 1)
                        throw new FormatException($"Shortcut {enumerator.Current} not defined.");
                    key = currentArg[keyStartIndex..separator];
                    value = currentArg[(separator + 1)..];
                }

                // Override value when key is duplicated. So we always have the last argument win.
                data[key] = value;
            }
        else
            while (enumerator.MoveNext())
            {
                var currentArg = enumerator.Current;
                var keyStartIndex = 0;

                if (currentArg.StartsWith("--"))
                    keyStartIndex = 2;
                else if (currentArg.StartsWith("-"))
                    keyStartIndex = 1;
                else if (currentArg.StartsWith("/"))
                {
                    // "/SomeSwitch" is equivalent to "--SomeSwitch" when interpreting switch mappings
                    // So we do a conversion to simplify later processing
                    currentArg = string.Format("--{0}", currentArg.Substring(1));
                    keyStartIndex = 2;
                }

                var separator = currentArg.IndexOf('=');

                if (separator < 0)
                {
                    // If there is neither equal sign nor prefix in current arugment, it is an invalid format
                    if (keyStartIndex == 0)
                        throw new FormatException($"{enumerator.Current} is an invalid argument.");
                    if (_booleanwitchMappings.TryGetValue(currentArg, out key))
                        value = "true";
                    else
                    {
                        // If the switch is a key in given switch mappings, interpret it
                        if (!_valueSwitchMappings.TryGetValue(currentArg, out key))
                        {
                            // If the switch starts with a single "-" and it isn't in given mappings , it is an invalid usage
                            if (keyStartIndex == 1)
                                throw new FormatException($"Shortcut {enumerator.Current} not defined.");
                            // Otherwise, use the switch name directly as a key
                            key = currentArg[keyStartIndex..];
                        }

                        var previousKey = enumerator.Current;
                        if (!enumerator.MoveNext())
                            throw new FormatException($"{previousKey} is missing a value");

                        value = enumerator.Current;
                    }
                }
                else
                {
                    var keySegment = currentArg[..separator];
                    if (!_valueSwitchMappings.TryGetValue(keySegment, out key))
                    {
                        if (keyStartIndex == 1)
                            throw new FormatException($"Shortcut {enumerator.Current} not defined.");
                        key = currentArg[keyStartIndex..separator];
                    }
                    value = currentArg[(separator + 1)..];
                }

                // Override value when key is duplicated. So we always have the last argument win.
                data[key] = value;
            }

        Data = data;
    }
}
