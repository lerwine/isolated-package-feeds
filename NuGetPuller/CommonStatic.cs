using System.Text.RegularExpressions;

namespace NuGetPuller;

public static partial class CommonStatic
{
    public static readonly StringComparer NoCaseComparer = StringComparer.CurrentCultureIgnoreCase;

    public static readonly Regex NonNormalizedWhiteSpaceRegex = CreateNonNormalizedWhiteSpaceRegex();

    public static readonly Regex LineBreakRegex = CreateLineBreakRegex();

    public static readonly Uri EmptyURI = new(string.Empty, UriKind.Relative);

    /// <summary>
    /// Gets the default value for <see cref="SharedAppSettings.UpstreamServiceIndex"/>.
    /// </summary>
    public const string DEFAULT_UPSTREAM_SERVICE_INDEX = "https://api.nuget.org/v3/index.json";

    /// <summary>
    /// The default path of the local repository, relative to the <see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />.
    /// </summary>
    public const string DEFAULT_LOCAL_REPOSITORY = "LocalSource";

    [GeneratedRegex(@"( |(?! ))[\r\n\s]+", RegexOptions.Compiled)]
    private static partial Regex CreateNonNormalizedWhiteSpaceRegex();

    [GeneratedRegex(@"\r?\n|\n", RegexOptions.Compiled)]
    private static partial Regex CreateLineBreakRegex();
}
