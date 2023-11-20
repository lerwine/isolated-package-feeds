using System.Collections.Immutable;
using Microsoft.Extensions.Configuration;

namespace NuGetAirGap;

/// <summary>
/// Based upon the <see cref="Microsoft.Extensions.Configuration.CommandLine.CommandLineConfigurationSource"/>, adding support for stand-alone command line switches are not followed by a value. 
/// </summary>
public class FlagSwitchCommandLineConfigSource : IConfigurationSource
{
    /// <summary>
    /// Gets a dictionary that maps stand-alone command line switches (not followed by a value) to boolean application settings.
    /// </summary>
    public IDictionary<string, string>? SwitchValueMappings { get; set; }

    /// <summary>
    /// Gets a dictionary that maps command line switch values to application settings.
    /// </summary>
    public IDictionary<string, string> BooleanSwitchMappings { get; set; }

    public ImmutableArray<string> Args { get; set; }

    /// <summary>
    /// Creates a new command-line configuration source.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <param name="booleanSwitchMappings">Maps stand-alone command line switches (not followed by a value) to boolean application settings.</param>
    /// <param name="switchValueMappings">Maps command line switch values to application settings.</param>
    public FlagSwitchCommandLineConfigSource(ImmutableArray<string> args, IDictionary<string, string> booleanSwitchMappings, IDictionary<string, string>? switchValueMappings = null) =>
        (Args, BooleanSwitchMappings, SwitchValueMappings) = (args, booleanSwitchMappings, switchValueMappings);

    public IConfigurationProvider Build(IConfigurationBuilder builder) => new FlagSwitchCommandLineConfigProvider(Args, BooleanSwitchMappings, SwitchValueMappings);
}
