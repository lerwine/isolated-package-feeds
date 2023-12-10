using System.Text.RegularExpressions;
//using static IsolatedPackageFeeds.Shared.CommonStatic

namespace IsolatedPackageFeeds.Shared;

public static partial class CommonStatic
{
    public static readonly StringComparer NoCaseComparer = StringComparer.CurrentCultureIgnoreCase;

    public static readonly Regex NonNormalizedWhiteSpaceRegex = CreateNonNormalizedWhiteSpaceRegex();

    public static readonly Regex LineBreakRegex = CreateLineBreakRegex();

    public static readonly Uri EmptyURI = new(string.Empty, UriKind.Relative);

    [GeneratedRegex(@"( |(?! ))[\r\n\s]+", RegexOptions.Compiled)]
    private static partial Regex CreateNonNormalizedWhiteSpaceRegex();

    [GeneratedRegex(@"\r?\n|\n", RegexOptions.Compiled)]
    private static partial Regex CreateLineBreakRegex();

}
