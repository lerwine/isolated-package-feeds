using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CdnGetter;

/// <summary>
/// Represents a comparable software version.
/// </summary>
public readonly partial struct SwVersion : IEquatable<SwVersion>, IComparable<SwVersion>
{
    [Obsolete("Use SemanticLikeRegex")]
    private const string GRP_p = "p";

    [Obsolete("Use SemanticLikeRegex")]
    private const string GRP_n = "n";

    [Obsolete("Use SemanticLikeRegex")]
    private const string GRP_r = "r";

    [Obsolete("Use SemanticLikeRegex")]
    private const string GRP_b = "b";

    [Obsolete("Use Tokens")]
    public const string REGEX_GROUP_pfx = "pfx";

    [Obsolete("Use Tokens")]
    public const string REGEX_GROUP_major = "major";

    [Obsolete("Use Tokens")]
    public const string REGEX_GROUP_minor = "minor";

    [Obsolete("Use Tokens")]
    public const string REGEX_GROUP_rel = "rel";

    [Obsolete("Use Tokens")]
    public const string REGEX_GROUP_patch = "patch";

    [Obsolete("Use Tokens")]
    public const string REGEX_GROUP_rev = "rev";

    [Obsolete("Use Tokens")]
    public const string REGEX_GROUP_xnum = "xnum";

    [Obsolete("Use Tokens")]
    public const string REGEX_GROUP_endday = "endday";

    [Obsolete("Use Tokens")]
    public const string REGEX_GROUP_endyr = "endyr";

    [Obsolete("Use Tokens")]
    public const string REGEX_GROUP_modname = "modname";

    [Obsolete("Use Tokens")]
    public const string REGEX_GROUP_modnum = "modnum";

    [Obsolete("Use Tokens")]
    public const string REGEX_GROUP_DELIM = "delim";

    [Obsolete("Use Tokens")]
    public const string REGEX_GROUP_PRE = "pre";

    [Obsolete("Use Tokens")]
    public const string REGEX_GROUP_BUILD = "build";

    [Obsolete("Use Tokens")]
    public const string REGEX_GROUP_epoch = "epoch";

    /// <summary>
    /// Matches a version string similar to the PEP 440 format.
    /// </summary>
    /// <see href="https://peps.python.org/pep-0440/" />
    [Obsolete("Use Tokens")]
    public static readonly Regex Pep440Regex = new(@$"^
(?<{REGEX_GROUP_pfx}>\D+)?
((?<{REGEX_GROUP_epoch}>\d+)!)?
(?<{REGEX_GROUP_major}>\d+)(\.(?<{REGEX_GROUP_minor}>\d+)(\.(?<{REGEX_GROUP_patch}>\d+)(\.(?<{REGEX_GROUP_rev}>\d+)(\.(?<{REGEX_GROUP_xnum}>\d+(\.\d+)*))?)?)?)?
(
    (?<{REGEX_GROUP_DELIM}>[-_\.])?
    (?<{REGEX_GROUP_PRE}>
        (?<{REGEX_GROUP_modname}>[a-z]+)
        [-_\.]?(?<{REGEX_GROUP_modnum}>\d+)?
    )
    |
    (?<{REGEX_GROUP_DELIM}>-)(?<{REGEX_GROUP_PRE}>\d+) # post
)?
(
    [+.-](?<{REGEX_GROUP_BUILD}>.*)
    |
    (?<{REGEX_GROUP_BUILD}>[^\d+_.-].*)
)?
$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
    
    /// <summary>
    /// Matches a date-based version string
    /// </summary>
    [Obsolete("Use Tokens")]
    public static readonly Regex DatedVersionRegex = new(@$"^
(?<{REGEX_GROUP_pfx}>\D+)?
(
    (?<{REGEX_GROUP_minor}>1[012]|0?\d)-(?<{REGEX_GROUP_rel}>3[01]|[012]?\d)-(?<{REGEX_GROUP_major}>\d{{4}})
    |
    (?<{REGEX_GROUP_major}>\d{{2}}|\d{{4}})-(?<{REGEX_GROUP_minor}>1[012]|0?\d)-(?<{REGEX_GROUP_rel}>3[01]|[012]?\d)
)
(
    (?<{REGEX_GROUP_DELIM}>(\.|_+)([^\d._]\D*)?|[^\d._]\D*)
    (
        (?<{REGEX_GROUP_patch}>\d+)
        [_+-]*
    )?
    (
        (?<{REGEX_GROUP_xnum}>(1[012]|0?\d)-(3[01]|[012]?\d))-(?<{REGEX_GROUP_rev}>\d{{4}})
        |
        (?<{REGEX_GROUP_rev}>\d{{2}}|\d{{4}})-(?<{REGEX_GROUP_xnum}>(1[012]|0?\d)-(3[01]|[012]?\d))
    )
    (
        [_-]+
        (
            (?<{REGEX_GROUP_endday}>(1[012]|0?\d)-(3[01]|[012]?\d))-(?<{REGEX_GROUP_endyr}>\d{{4}})
            |
            (?<{REGEX_GROUP_endyr}>\d{{2}}|\d{{4}})-(?<{REGEX_GROUP_endday}>(1[012]|0?\d)-(3[01]|[012]?\d))
        )
    )
)?
(
    [_+-](?<{REGEX_GROUP_BUILD}>.*)
    |
    (?<{REGEX_GROUP_BUILD}>\D.*)
)?
$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
    
    /// <summary>
    /// Matches a version string similar to the SemVer format.
    /// </summary>
    /// <see href="https://semver.org/" />
    [Obsolete("Use Tokens")]
    public static readonly Regex SemanticLikeRegex = new(@$"^(?<{REGEX_GROUP_pfx}>[+-]*[^\d+-]+([+-]+[^\d+-]+)*-*)?(?<{REGEX_GROUP_major}>-?\d+)(\.(?<{REGEX_GROUP_minor}>-?\d+)(\.(?<{REGEX_GROUP_patch}>-?\d+)(\.(?<{REGEX_GROUP_rev}>-?\d+)(\.(?<{REGEX_GROUP_xnum}>-?\d+(\.-?\d+)*))?)?)?)?((?<{REGEX_GROUP_DELIM}>\.)(?<{REGEX_GROUP_PRE}>([^\d+][^+]*)?)|(?<{REGEX_GROUP_DELIM}>-)(?<{REGEX_GROUP_PRE}>[^+]*)|(?<{REGEX_GROUP_PRE}>[^\d.+-][^+]*))?(\+(?<{REGEX_GROUP_BUILD}>.*))?$", RegexOptions.Compiled);

    [Obsolete("Use Tokens")]
    private static readonly Regex _versionRegex = new(@$"^(?<{GRP_p}>(-?[^\d.+-]+)+)?(?<{GRP_n}>-?\d+(\.\d+)*)(?<{GRP_r}>-[^+]*|[^+]+)?(?<{GRP_b}>\+.*)?$", RegexOptions.Compiled);

    [Obsolete("Use SEPARATOR_DASH")]
    public const char DELIMITER_PRERELEASE = '-';

    [Obsolete("Use SEPARATOR_PLUS")]
    public const char DELIMITER_BUILD = '+';

    [Obsolete("Use Tokens")]
    private static readonly char[] FIRST_VERSION_TOKEN_CHARS = new char[] { SEPARATOR_DOT, '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    [Obsolete("Use Tokens")]
    private static readonly char[] SEPARATOR_PreRelease = new char[] { DELIMITER_PRERELEASE, SEPARATOR_DOT };

    [Obsolete("Use Tokens")]
    private static readonly char[] SEPARATOR_Build = new char[] { DELIMITER_BUILD, SEPARATOR_DOT, DELIMITER_PRERELEASE };

    public static readonly StringComparer TextComparer = StringComparer.OrdinalIgnoreCase;

    private readonly ReadOnlyCollection<IToken> _tokens;

    /// <summary>
    /// Gets the text version component that precedes other version components.
    /// A <see langword="null" /> value indicates there is no prefixed version component.
    /// </summary>
    /// <remarks>This will never be a zero-length string.</remarks>
    [Obsolete("Use Prefix property")]
    public string? Prefix_obs { get; }

    public ReadOnlyCollection<IToken>? Prefix { get; }

    /// <summary>
    /// Gets the value of first numerical version component indicating the major version number.
    /// </summary>
    /// <remarks>This will always be <c>0</c> when <see cref="Format" /> is <see cref="VersionStringFormat.NonNumerical" /> since non-standard version strings do not have parsable numerical components..</remarks>
    [Obsolete("Use Major property")]
    public int Major_obs { get; }

    public INumericToken Major { get; }

    /// <summary>
    /// Gets the value of the second numerical version component indicating the minor version number.
    /// A <see langword="null" /> value indicates an implied <c>0</c> value.
    /// </summary>
    /// <remarks>This will always be <see langword="null" /> when <see cref="Format" /> is <see cref="VersionStringFormat.NonNumerical" />.
    /// This will never have a negative value.</remarks>
    [Obsolete("Use Minor property")]
    public int? Minor_obs { get; }

    public DelimitedToken<INumericToken>? Minor { get; }

    /// <summary>
    /// Gets the value of the third numerical version component indicating the patch number.
    /// A <see langword="null" /> value indicates an implied <c>0</c> value.
    /// </summary>
    /// <remarks>This will always be <see langword="null" /> when <see cref="Format" /> is <see cref="VersionStringFormat.NonNumerical" />.
    /// This will never have a negative value.</remarks>
    [Obsolete("Use Micro property")]
    public int? Patch { get; }
    
    public DelimitedToken<INumericToken>? Micro { get; }

    /// <summary>
    /// Gets the value of the fourth numerical version component indicating the revision number.
    /// A <see langword="null" /> value indicates an implied <c>0</c> value.
    /// </summary>
    /// <remarks>This will always be <see langword="null" /> when <see cref="Format" /> is <see cref="VersionStringFormat.NonNumerical" />.
    /// This will never have a negative value.</remarks>
    [Obsolete("This will roll up into the AdditionalNumerical property")]
    public int? Revision { get; }

    /// <summary>
    /// Gets the numerical version components that follow the <see cref="Revision" /> components.
    /// A <see langword="null" /> value indicates there are no additinal numerical component.
    /// </summary>
    /// <remarks>This will never contain an empty collection; It will always have at least 1 element if it is not <see cref="null" />.</remarks>
    [Obsolete("Use AdditionalNumerical property")]
    public ReadOnlyCollection<int>? AdditionalNumerical_obs { get; }

    public ReadOnlyCollection<DelimitedToken<INumericToken>>? AdditionalNumerical { get; }

    /// <summary>
    /// Gets the segments of the pre-release component, which follows the numerical components.
    /// A <see langword="null" /> value indicates there is no pre-release component.
    /// </summary>
    /// <remarks>This will never contain an empty collection; It will always have at least 1 element if it is not <see cref="null" />.</remarks>
    [Obsolete("Use Modifiers property")]
    public ReadOnlyCollection<PreReleaseSegment>? PreRelease { get; }

    public ReadOnlyCollection<IDelimitedTokenList<IToken>>? Modifiers { get; }

    /// <summary>
    /// Gets the segments of the build component, which follows the numerical components.
    /// A <see langword="null" /> value indicates there is no build component.
    /// </summary>
    /// <remarks>This will never contain an empty collection; It will always have at least 1 element if it is not <see cref="null" />.</remarks>
    [Obsolete("This will roll up into the Modifiers property")]
    public ReadOnlyCollection<BuildSegment>? Build { get; }
    
    /// <summary>
    /// Gets the format of the version string.
    /// </summary>
    public VersionStringFormat Format { get; }

    public static readonly ValueConverter<SwVersion, string> Converter = new(
        v => v.ToString(),
        s => new(s)
    );

    private SwVersion(int major, int? minor, int? patch, int? revision, IEnumerable<int>? additionalNumerical, IEnumerable<PreReleaseSegment>? preRelease, IEnumerable<BuildSegment>? build, string? prefix)
    {
        IList<IToken> tokens = Tokenize(prefix);
        if (tokens is not List<IToken>)
            tokens = tokens.ToList();
        Prefix = string.IsNullOrEmpty(prefix) ? null : new(Tokenize(prefix));
        Major = NumericTokenFromInt32(major);
        tokens.Add(Major);
        if (minor.HasValue)
        {
            tokens.Add(SeparatorToken.Dot);
            tokens.Add((Minor = new(SeparatorToken.Dot, NumericTokenFromInt32(minor.Value))).Value);
            if (patch.HasValue)
            {
                tokens.Add(SeparatorToken.Dot);
                tokens.Add((Micro = new(SeparatorToken.Dot, NumericTokenFromInt32(minor.Value))).Value);
                if (revision.HasValue)
                {
                    DelimitedToken<INumericToken> dt = new(SeparatorToken.Dot, NumericTokenFromInt32(minor.Value));
                    Collection<DelimitedToken<INumericToken>> additional = new() { dt };
                    tokens.Add(SeparatorToken.Dot);
                    tokens.Add(dt.Value);
                    if (additionalNumerical is not null)
                        foreach (int a in additionalNumerical)
                        {
                            tokens.Add(SeparatorToken.Dot);
                            tokens.Add((dt = new(SeparatorToken.Dot, NumericTokenFromInt32(minor.Value))).Value);
                            additional.Add(dt);
                        }
                    AdditionalNumerical = new(additional);
                }
                else
                    AdditionalNumerical = null;
            }
            else
            {
                 Micro = null;
                AdditionalNumerical = null;
            }
        }
        else
        {
            Minor = Micro = null;
            AdditionalNumerical = null;
        }
        if (additionalNumerical is null)
            AdditionalNumerical_obs = null;
        else
        {
            int[] an = additionalNumerical.ToArray();
            if (an.Length > 0)
            {
                if (an.Any(nextSparator => nextSparator < 0))
                    throw new ArgumentOutOfRangeException(nameof(additionalNumerical));
                AdditionalNumerical_obs = new(an);
            }
            else
                AdditionalNumerical_obs = null;
        }
        if (preRelease.TryGetFirst(out PreReleaseSegment prs))
        {
            if (prs.AltSeparator && prs.Value.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(preRelease));
            if (prs.AltSeparator)
                ((List<IToken>)tokens).AddRange(Tokenize(prs.Value));
            else
            {
                tokens.Add(SeparatorToken.Dash);
                if (prs.Value.Length > 0)
                    ((List<IToken>)tokens).AddRange(Tokenize(prs.Value));
            }
            foreach (PreReleaseSegment p in preRelease.Skip(1))
            {
                tokens.Add(p.AltSeparator ? SeparatorToken.Dot : SeparatorToken.Dash);
                if (p.Value.Length > 0)
                    ((List<IToken>)tokens).AddRange(Tokenize(p.Value));
            }
            Format = prs.AltSeparator ? VersionStringFormat.Alt : VersionStringFormat.Standard;
            PreRelease = new(preRelease.ToArray());
        }
        else
        {
            Format = VersionStringFormat.Standard;
            PreRelease = null;
        }
        if (build.TryGetFirst(out BuildSegment bs))
        {
            if (bs.Separator != BuildSeparator.Plus)
                throw new ArgumentException($"The {nameof(BuildSegment.Separator)} property of the first element of {nameof(build)} must be {nameof(BuildSeparator)}.{nameof(BuildSeparator.Plus)}.", nameof(build));
            tokens.Add(SeparatorToken.Plus);
            if (bs.Value.Length > 0)
                ((List<IToken>)tokens).AddRange(Tokenize(bs.Value));
            foreach (BuildSegment b in build.Skip(1))
            {
                tokens.Add(b.Separator switch
                {
                    BuildSeparator.Dot => SeparatorToken.Dot,
                    BuildSeparator.Dash => SeparatorToken.Dash,
                    _ => SeparatorToken.Plus
                });
                if (b.Value.Length > 0)
                    ((List<IToken>)tokens).AddRange(Tokenize(b.Value));
            }
            Build = new(build.ToArray());
        }
        else
            Build = null;
        _tokens = new(tokens);
        Prefix_obs = (prefix is null || prefix.Length == 0) ? null : prefix;
        Major_obs = major;
        Minor_obs = minor;
        Patch = patch;
        Revision = revision;
        throw new NotImplementedException();
    }

    public SwVersion(string prefix, int major, int minor, int patch, int revision, IEnumerable<int> additionalNumerical, IEnumerable<PreReleaseSegment> preRelease, BuildSegment build, params BuildSegment[] additionalBuild)
        : this(major, minor, patch, revision, additionalNumerical, preRelease, additionalBuild.PrependValue(build), prefix) { }

    public SwVersion(int major, int minor, int patch, int revision, IEnumerable<int> additionalNumerical, IEnumerable<PreReleaseSegment> preRelease, BuildSegment build, params BuildSegment[] additionalBuild)
        : this(major, minor, patch, revision, additionalNumerical, preRelease, additionalBuild.PrependValue(build), null) { }

    public SwVersion(string prefix, int major, int minor, int patch, int revision, IEnumerable<int> additionalNumerical, PreReleaseSegment preRelease, params PreReleaseSegment[] additionalPreRelease)
        : this(major, minor, patch, revision, additionalNumerical, additionalPreRelease.PrependValue(preRelease), null, prefix) { }

    public SwVersion(int major, int minor, int patch, int revision, IEnumerable<int> additionalNumerical, PreReleaseSegment preRelease, params PreReleaseSegment[] additionalPreRelease)
        : this(major, minor, patch, revision, additionalNumerical, additionalPreRelease.PrependValue(preRelease), null, null) { }

    public SwVersion(string prefix, int major, int minor, int patch, int revision, IEnumerable<int> additionalNumerical, BuildSegment build, params BuildSegment[] additionalBuild)
        : this(major, minor, patch, revision, additionalNumerical, null, additionalBuild.PrependValue(build), prefix) { }

    public SwVersion(int major, int minor, int patch, int revision, IEnumerable<int> additionalNumerical, BuildSegment build, params BuildSegment[] additionalBuild)
        : this(major, minor, patch, revision, additionalNumerical, null, additionalBuild.PrependValue(build), null) { }

    public SwVersion(string prefix, int major, int minor, int patch, IEnumerable<PreReleaseSegment> preRelease, BuildSegment build, params BuildSegment[] additionalBuild)
        : this(major, minor, patch, null, null, preRelease, additionalBuild.PrependValue(build), prefix) { }

    public SwVersion(string prefix, int major, int minor, int patch, int revision, params int[] additionalNumerical) : this(major, minor, patch, revision, additionalNumerical, null, null, prefix) { }

    public SwVersion(int major, int minor, int patch, IEnumerable<PreReleaseSegment> preRelease, BuildSegment build, params BuildSegment[] additionalBuild)
        : this(major, minor, patch, null, null, preRelease, additionalBuild.PrependValue(build), null) { }

    public SwVersion(string prefix, int major, int minor, int patch, PreReleaseSegment preRelease, params PreReleaseSegment[] additionalPreRelease)
        : this(major, minor, patch, null, null, additionalPreRelease.PrependValue(preRelease), null, prefix) { }

    public SwVersion(int major, int minor, int patch, PreReleaseSegment preRelease, params PreReleaseSegment[] additionalPreRelease)
        : this(major, minor, patch, null, null, additionalPreRelease.PrependValue(preRelease), null, null) { }

    public SwVersion(string prefix, int major, int minor, int patch, BuildSegment build, params BuildSegment[] additionalBuild)
        : this(major, minor, patch, null, null, null, additionalBuild.PrependValue(build), prefix) { }

    public SwVersion(string prefix, int major, int minor, IEnumerable<PreReleaseSegment> preRelease, BuildSegment build, params BuildSegment[] additionalBuild)
        : this(major, minor, null, null, null, preRelease, additionalBuild.PrependValue(build), prefix) { }

    public SwVersion(int major, int minor, int patch, int revision, params int[] additionalNumerical) : this(major, minor, patch, revision, additionalNumerical, null, null, null) { }

    public SwVersion(int major, int minor, int patch, BuildSegment build, params BuildSegment[] additionalBuild)
        : this(major, minor, patch, null, null, null, additionalBuild.PrependValue(build), null) { }

    public SwVersion(int major, int minor, IEnumerable<PreReleaseSegment> preRelease, BuildSegment build, params BuildSegment[] additionalBuild)
        : this(major, minor, null, null, null, preRelease, additionalBuild.PrependValue(build), null) { }

    public SwVersion(string prefix, int major, int minor, PreReleaseSegment preRelease, params PreReleaseSegment[] additionalPreRelease)
        : this(major, minor, null, null, null, additionalPreRelease.PrependValue(preRelease), null, prefix) { }

    public SwVersion(int major, int minor, PreReleaseSegment preRelease, params PreReleaseSegment[] additionalPreRelease)
        : this(major, minor, null, null, null, additionalPreRelease.PrependValue(preRelease), null, null) { }

    public SwVersion(string prefix, int major, int minor, BuildSegment build, params BuildSegment[] additionalBuild)
        : this(major, minor, null, null, null, null, additionalBuild.PrependValue(build), prefix) { }

    public SwVersion(string prefix, int major, IEnumerable<PreReleaseSegment> preRelease, BuildSegment build, params BuildSegment[] additionalBuild)
        : this(major, null, null, null, null, preRelease, additionalBuild.PrependValue(build), prefix) { }

    public SwVersion(int major, int minor, BuildSegment build, params BuildSegment[] additionalBuild) : this(major, minor, null, null, null, null, additionalBuild.PrependValue(build), null) { }

    public SwVersion(int major, IEnumerable<PreReleaseSegment> preRelease, BuildSegment build, params BuildSegment[] additionalBuild)
        : this(major, null, null, null, null, preRelease, additionalBuild.PrependValue(build), null) { }

    public SwVersion(string prefix, int major, PreReleaseSegment preRelease, params PreReleaseSegment[] additionalPreRelease)
        : this(major, null, null, null, null, additionalPreRelease.PrependValue(preRelease), null, prefix) { }

    public SwVersion(int major, PreReleaseSegment preRelease, params PreReleaseSegment[] additionalPreRelease)
        : this(major, null, null, null, null, additionalPreRelease.PrependValue(preRelease), null, null) { }

    public SwVersion(string prefix, int major, BuildSegment build, params BuildSegment[] additionalBuild) : this(major, null, null, null, null, null, additionalBuild.PrependValue(build), prefix) { }

    public SwVersion(string prefix, int major, int minor, int patch) : this(major, minor, patch, null, null, null, null, prefix) { }

    public SwVersion(int major, int minor, int patch) : this(major, minor, patch, null, null, null, null, null) { }

    public SwVersion(int major, BuildSegment build, params BuildSegment[] additionalBuild) : this(major, null, null, null, null, null, additionalBuild.PrependValue(build), null) { }

    public SwVersion(string prefix, int major, int minor) : this(major, minor, null, null, null, null, null, prefix) { }

    public SwVersion(int major, int minor) : this(major, minor, null, null, null, null, null, null) { }

    public SwVersion(string prefix, int major) : this(major, null, null, null, null, null, null, prefix) { }

    public SwVersion(int major) : this(major, null, null, null, null, null, null, null) { }

    public SwVersion(string? versionString)
    {
        // https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/manifest.json/version/format
        // https://github.com/dotnet/aspnet-api-versioning/wiki/Version-Format
        // using IEnumerator<IToken> enumerator = (_tokens = new(Tokenize(versionString))).GetEnumerator();
        // if (!enumerator.MoveNext())
        // {
        //     Format = VersionStringFormat.NonNumerical;
        //     Major_obs = 0;
        //     Minor_obs = Patch = Revision = null;
        //     AdditionalNumerical_obs = null;
        //     Build = null;
        //     Prefix_obs = string.Empty;
        //     PreRelease = null;
        //     return;
        // }
        // IToken token = enumerator.Current;
        // List<IToken> prefix = new();
        // while (token is not INumericToken)
        // {
        //     prefix.Add(token);
        //     if (!enumerator.MoveNext())
        //     {
        //         Format = VersionStringFormat.NonNumerical;
        //         Major_obs = 0;
        //         Minor_obs = Patch = Revision = null;
        //         AdditionalNumerical_obs = null;
        //         Build = null;
        //         Prefix_obs = GetString(prefix);
        //         PreRelease = null;
        //         return;
        //     }
        //     token = enumerator.Current;
        // }
        // Prefix_obs = GetString(prefix);
        // INumericToken major = (INumericToken)token;
        throw new NotImplementedException();
    }

    private IEnumerable<int> GetSubVersionNumberValues()
    {
        if (Minor_obs.HasValue)
        {
            yield return Minor_obs.Value;
            if (Patch.HasValue)
            {
                yield return Patch.Value;
                if (Revision.HasValue)
                {
                    yield return Revision.Value;
                    if (AdditionalNumerical_obs is not null)
                        foreach (int v in AdditionalNumerical_obs)
                            yield return v;
                }
            }
        }
    }
    private static IEnumerable<BuildSegment> ParseBuildSegments(string text)
    {
        if (text.Length == 1)
        {
            yield return new BuildSegment(BuildSeparator.Plus, string.Empty);
            yield break;
        }
        int previousSeparator = 0;
        int startIndex = 1;
        int nextSeparator = text.IndexOfAny(SEPARATOR_Build, 1);
        while (nextSeparator > 0)
        {
            yield return new BuildSegment(text[previousSeparator] switch
            {
                SEPARATOR_DOT => BuildSeparator.Dot,
                DELIMITER_PRERELEASE => BuildSeparator.Dash,
                _ => BuildSeparator.Plus,
            }, (startIndex < nextSeparator) ? text[startIndex..nextSeparator] : string.Empty);
            if ((startIndex = (previousSeparator = nextSeparator) + 1) == text.Length)
            {
                yield return new BuildSegment(text[previousSeparator] switch
                {
                    SEPARATOR_DOT => BuildSeparator.Dot,
                    DELIMITER_PRERELEASE => BuildSeparator.Dash,
                    _ => BuildSeparator.Plus,
                }, string.Empty);
                yield break;
            }
            nextSeparator = text.IndexOfAny(SEPARATOR_Build, startIndex);
        }

        yield return new BuildSegment(text[previousSeparator] switch
        {
            SEPARATOR_DOT => BuildSeparator.Dot,
            DELIMITER_PRERELEASE => BuildSeparator.Dash,
            _ => BuildSeparator.Plus,
        }, text[startIndex..]);
    }

    private static IEnumerable<PreReleaseSegment> ParsePreReleaseSegments(string text)
    {
        int previousSeparator = text.IndexOfAny(SEPARATOR_PreRelease);
        if (previousSeparator < 0)
        {
            yield return new PreReleaseSegment(true, text);
            yield break;
        }
        if (previousSeparator == 0)
        {
            if (text.Length == 1)
            {
                if (text[0] == DELIMITER_PRERELEASE)
                    yield return new PreReleaseSegment(true, text);
                else
                    yield return new PreReleaseSegment(false, string.Empty);
                yield break;
            }
            if (text[0] != DELIMITER_PRERELEASE)
            {
                if ((previousSeparator = text.IndexOfAny(SEPARATOR_PreRelease)) < 0)
                {
                    yield return new PreReleaseSegment(true, text);
                    yield break;
                }
                yield return new PreReleaseSegment(true, text[..previousSeparator]);
            }
        }
        else
            yield return new PreReleaseSegment(true, text[..previousSeparator]);
        int startIndex = previousSeparator + 1;
        while (startIndex < text.Length)
        {
            int nextSeparator = text.IndexOfAny(SEPARATOR_PreRelease, startIndex);
            if (nextSeparator < 0)
            {
                yield return new PreReleaseSegment(text[previousSeparator] != DELIMITER_PRERELEASE, text[startIndex..]);
                yield break;
            }
            yield return new PreReleaseSegment(text[previousSeparator] != DELIMITER_PRERELEASE, (startIndex < nextSeparator) ? text[startIndex..nextSeparator] : string.Empty);
            startIndex = nextSeparator + 1;
            previousSeparator = nextSeparator;
        }
        yield return new PreReleaseSegment(text[previousSeparator] != DELIMITER_PRERELEASE, string.Empty);
    }

    private static int FirstNonZero(IEnumerable<int> source) => source.Where(i => i != 0).DefaultIfEmpty(0).First();

    private static int CompareVersionNumbers(IEnumerable<int> x, IEnumerable<int> y)
    {
        using IEnumerator<int> a = x.GetEnumerator();
        using IEnumerator<int> b = y.GetEnumerator();
        int result;
        while (a.MoveNext())
        {
            if (b.MoveNext())
                if ((result = a.Current - b.Current) != 0)
                    return result;
            else
            {
                do if (a.Current != 0) return 1; while (a.MoveNext());
                return 0;
            }
        }
        while (b.MoveNext())
            if (b.Current != 0)
                return -1;
        return 0;
    }

    private static bool VersionNumbersEqual(IEnumerable<int> x, IEnumerable<int> y)
    {
        using IEnumerator<int> a = x.GetEnumerator();
        using IEnumerator<int> b = y.GetEnumerator();
        while (a.MoveNext())
        {
            if (b.MoveNext())
                if (a.Current != b.Current)
                    return false;
            else
            {
                do if (a.Current != 0) return false; while (a.MoveNext());
                return true;
            }
        }
        while (b.MoveNext())
            if (b.Current != 0)
                return false;
        return true;
    }

    private static int CompareValues<T>(IEnumerable<T>? x, IEnumerable<T>? y) where T : struct, IComparable<T>
    {
        if (x is null)
            return (y is null) ? 0 : -1;
        if (y is null)
            return 1;
        using IEnumerator<T> a = x.GetEnumerator();
        using IEnumerator<T> b = y.GetEnumerator();
        while (a.MoveNext())
        {
            if (b.MoveNext())
            {
                int result = a.Current.CompareTo(b.Current);
                if (result != 0)
                    return result;
            }
            else
                return 1;
        }
        return b.MoveNext() ? -1 : 0;
    }

    private static bool ValuesEqual<T>(IEnumerable<T>? x, IEnumerable<T>? y) where T : struct, IEquatable<T>
    {
        if (x is null)
            return y is null;
        if (y is null)
            return false;
        using IEnumerator<T> a = x.GetEnumerator();
        using IEnumerator<T> b = y.GetEnumerator();
        while (a.MoveNext())
        {
            if (b.MoveNext())
                if (!a.Current.Equals(b.Current))
                    return false;
            else
                return false;
        }
        return !b.MoveNext();
    }

    public int CompareTo(SwVersion other)
    {
        // int diff = Major_obs - other.Major_obs;
        // if (diff != 0 || (diff = CompareVersionNumbers(GetSubVersionNumberValues(), other.GetSubVersionNumberValues())) != 0 ||
        //         (diff = CompareValues(PreRelease, other.PreRelease)) != 0 || (diff = CompareValues(Build, other.Build)) != 0)
        //     return diff;
        // if (Prefix_obs is null)
        //     return (other.Prefix_obs is null) ? 0 : -1;
        // return (other.Prefix_obs is null) ? 1 : TextComparer.Compare(Prefix_obs, other.Prefix_obs);
        throw new NotImplementedException();
    }

    public bool Equals(SwVersion other)
    {
        // if (Major_obs == other.Major_obs && VersionNumbersEqual(GetSubVersionNumberValues(), other.GetSubVersionNumberValues()) && ValuesEqual(PreRelease, other.PreRelease) && ValuesEqual(Build, other.Build))
        // {
        //     if (Prefix_obs is null)
        //         return other.Prefix_obs is null;
        //     return other.Prefix_obs is not null && TextComparer.Equals(Prefix_obs, other.Prefix_obs);
        // }
        // return false;
        throw new NotImplementedException();
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is SwVersion other && Equals(other);

    public override int GetHashCode()
    {
        // 1  2  3  4   5   6   7   8
        // 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59
        int hash = 23;
        unchecked
        {
            hash = hash * 31 + Major_obs;
            if (Minor_obs.HasValue)
            {
                hash = hash * 31 + Minor_obs.Value;
                if (Patch.HasValue)
                {
                    hash = hash * 31 + Patch.Value;
                    if (Revision.HasValue)
                    {
                        hash = hash * 31 + Revision.Value;
                        if (AdditionalNumerical_obs is not null)
                        {
                            int h = 3;
                            foreach (int n in AdditionalNumerical_obs)
                                h = (h * 7) + n;
                            hash = hash * 31 + h;
                        }
                    }
                }
            }
            if (PreRelease is not null)
            {
                int h = 3;
                foreach (PreReleaseSegment s in PreRelease)
                    h = (h * 7) + s.GetHashCode();
                hash = hash * 31 + h;
            }
            if (Build is not null)
            {
                int h = 3;
                foreach (BuildSegment s in Build)
                    h = (h * 7) + s.GetHashCode();
                hash = hash * 31 + h;
            }
            return (Prefix_obs is null) ? hash : hash * 31 + TextComparer.GetHashCode(Prefix_obs);
        }
    }

    /// <summary>
    /// Converts the current version object to a formatted version string.
    /// </summary>
    /// <param name="minDigitCount">The minimum number of numerical components to display or <c>0</c> to only display the defined numerical components.</param>
    /// <param name="omitBuild">If <see langword="true" />, the <see cref="Build" /> components are omitted. The default is <see langword="false" />./param>
    /// <param name="omitPreRelease">If <see langword="true" />, the <see cref="PreRelease" /> components are omitted. The default is <see langword="false" />.</param>
    /// <param name="omitPrefix">If <see langword="true" />, the <see cref="Prefix_obs" /> component is omitted. The default is <see langword="false" />.</param>
    /// <returns>A formatted version string according to the current <see cref="Format" />.</returns>
    public string ToString(int minDigitCount, bool omitBuild = false, bool omitPreRelease = false, bool omitPrefix = false)
    {
        StringBuilder sb;
        PreReleaseSegment prs;
        if (omitBuild || Build is null)
        {
            if (omitPreRelease || PreRelease is null)
            {
                if (AdditionalNumerical_obs is null)
                {
                    if (Revision.HasValue)
                    {
                        if (minDigitCount < 5)
                        {
                            if (Prefix_obs is null)
                                return $"{Major_obs}.{Minor_obs!.Value}.{Patch!.Value}.{Revision.Value}";
                            return $"{Prefix_obs}{Major_obs}.{Minor_obs!.Value}.{Patch!.Value}.{Revision.Value}";
                        }
                        sb = ((Prefix_obs is null) ? new StringBuilder() : new StringBuilder(Prefix_obs)).Append(Major_obs).Append('.').Append(Minor_obs!.Value).Append('.').Append(Patch!.Value).Append('.').Append(Revision.Value);
                        for (int i = 4; i < minDigitCount; i++)
                            sb.Append(".0");
                        return sb.ToString();
                    }
                    if (Patch.HasValue)
                    {
                        if (minDigitCount < 4)
                        {
                            if (Prefix_obs is null)
                                return $"{Major_obs}.{Minor_obs!.Value}.{Patch.Value}";
                            return $"{Prefix_obs}{Major_obs}.{Minor_obs!.Value}.{Patch.Value}";
                        }
                        sb = ((Prefix_obs is null) ? new StringBuilder() : new StringBuilder(Prefix_obs)).Append(Major_obs).Append('.').Append(Minor_obs!.Value).Append('.').Append(Patch.Value);
                        for (int i = 3; i < minDigitCount; i++)
                            sb.Append(".0");
                        return sb.ToString();
                    }
                    if (Minor_obs.HasValue)
                    {
                        if (minDigitCount < 3)
                        {
                            if (Prefix_obs is null)
                                return $"{Major_obs}.{Minor_obs.Value}";
                            return $"{Prefix_obs}{Major_obs}.{Minor_obs.Value}";
                        }
                        sb = ((Prefix_obs is null) ? new StringBuilder() : new StringBuilder(Prefix_obs)).Append(Major_obs).Append('.').Append(Minor_obs!.Value);
                        for (int i = 2; i < minDigitCount; i++)
                            sb.Append(".0");
                        return sb.ToString();
                    }
                    if (minDigitCount < 2)
                    {
                        if (Prefix_obs is null)
                            return Major_obs.ToString();
                        return $"{Prefix_obs}{Major_obs}";
                    }
                    sb = ((Prefix_obs is null) ? new StringBuilder() : new StringBuilder(Prefix_obs)).Append(Major_obs);
                    for (int i = 2; i < minDigitCount; i++)
                        sb.Append(".0");
                    return sb.ToString();
                }
            }
            // if (prs.AltSeparator != PreRelease[0].AltSeparator)
            //     sb.Append('-');
            // sb.Append(prs.Value);
            // foreach (PreReleaseSegment p in PreRelease.Skip(1))
            // {
            //     sb.Append(p.AltSeparator ? '.' : '-');
            //     if (p.Value.Length > 0)
            //         sb.Append(p);
            // }
        }
        else
        {
            // if (omitPreRelease || PreRelease is null)
            // {

            // }
            // else
            // {
            //     if (!prs = PreRelease[0].AltSeparator)
            //         sb.Append('-');
            //     sb.Append(prs.Value);
            //     foreach (PreReleaseSegment p in PreRelease.Skip(1))
            //     {
            //         sb.Append(p.AltSeparator ? '.' : '-');
            //         if (p.Value.Length > 0)
            //             sb.Append(p);
            //     }
            // }
        }
        // foreach (BuildSegment bs in Build)
        // {
        //     sb.Append(bs.Separator switch
        //     {
        //         BuildSeparator.Dot => '.',
        //         BuildSeparator.Dash => '-',
        //         _ => '+',
        //     });
        //     if (bs.Value.Length > 0)
        //         sb.Append(bs.Value);
        // }
        // return sb.ToString();
        throw new NotImplementedException();
    }

    public override string ToString() => ToString(0);

    public static bool operator ==(SwVersion left, SwVersion right) => left.Equals(right);

    public static bool operator !=(SwVersion left, SwVersion right) => !left.Equals(right);

    public static bool operator <(SwVersion left, SwVersion right) => left.CompareTo(right) < 0;

    public static bool operator <=(SwVersion left, SwVersion right) => left.CompareTo(right) <= 0;

    public static bool operator >(SwVersion left, SwVersion right) => left.CompareTo(right) > 0;

    public static bool operator >=(SwVersion left, SwVersion right) => left.CompareTo(right) >= 0;

}
