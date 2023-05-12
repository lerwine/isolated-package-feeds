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
    // Semantic Versioning: https://semver.org/

    private const string GRP_p = "p";

    private const string GRP_n = "n";

    private const string GRP_r = "r";

    private const string GRP_b = "b";

    public const string REGEX_GROUP_pfx = "pfx";

    public const string REGEX_GROUP_major = "major";

    public const string REGEX_GROUP_minor = "minor";

    public const string REGEX_GROUP_rel = "rel";

    public const string REGEX_GROUP_patch = "patch";

    public const string REGEX_GROUP_rev = "rev";

    public const string REGEX_GROUP_xnum = "xnum";

    public const string REGEX_GROUP_endday = "endday";

    public const string REGEX_GROUP_endyr = "endyr";

    public const string REGEX_GROUP_modname = "modname";

    public const string REGEX_GROUP_modnum = "modnum";

    public const string REGEX_GROUP_DELIM = "delim";

    public const string REGEX_GROUP_PRE = "pre";

    public const string REGEX_GROUP_BUILD = "build";

    public const string REGEX_GROUP_epoch = "epoch";

    /// <summary>
    /// Matches a version string similar to the PEP 440 format.
    /// </summary>
    /// <see href="https://peps.python.org/pep-0440/" />
    public static readonly Regex Pep440Regex = new(@$"^
(?<{REGEX_GROUP_pfx}>\D+)?
((?<{REGEX_GROUP_epoch}>\d+)!)?
(?<{REGEX_GROUP_major}>\d+)(\.(?<{REGEX_GROUP_minor}>\d+)(\.(?<{REGEX_GROUP_patch}>\d+)(\.(?<{REGEX_GROUP_rev}>\d+)(\.(?<{REGEX_GROUP_xnum}>\d+(\.\d+)*))?)?)?)?
(
    (?<{REGEX_GROUP_DELIM}>[-_\.])?
    (?<{REGEX_GROUP_PRE}>
        (?<{REGEX_GROUP_modname}>a|b|c|rc|alpha|beta|pre|preview|post|rev|r|dev)
        [-_\.]?(?<{REGEX_GROUP_modnum}>\d+)?
    )
    |
    (?<{REGEX_GROUP_DELIM}>-)(?<{REGEX_GROUP_PRE}>\d+) # post
)?
(
    +(?<{REGEX_GROUP_BUILD}>.*)
    |
    (?<{REGEX_GROUP_BUILD}>[^\d+_.-].*)
)?
$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
    
    /// <summary>
    /// Matches a date-based version string
    /// </summary>
    /// <see href="https://peps.python.org/pep-0440/" />
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
    public static readonly Regex SemanticLikeRegex = new(@$"^(?<{REGEX_GROUP_pfx}>[+-]*[^\d+-]+([+-]+[^\d+-]+)*-*)?(?<{REGEX_GROUP_major}>-?\d+)(\.(?<{REGEX_GROUP_minor}>-?\d+)(\.(?<{REGEX_GROUP_patch}>-?\d+)(\.(?<{REGEX_GROUP_rev}>-?\d+)(\.(?<{REGEX_GROUP_xnum}>-?\d+(\.-?\d+)*))?)?)?)?((?<{REGEX_GROUP_DELIM}>\.)(?<{REGEX_GROUP_PRE}>([^\d+][^+]*)?)|(?<{REGEX_GROUP_DELIM}>-)(?<{REGEX_GROUP_PRE}>[^+]*)|(?<{REGEX_GROUP_PRE}>[^\d.+-][^+]*))?(\+(?<{REGEX_GROUP_BUILD}>.*))?$", RegexOptions.Compiled);

    [Obsolete("Use SemanticLikeRegex")]
    private static readonly Regex _versionRegex = new(@$"^(?<{GRP_p}>(-?[^\d.+-]+)+)?(?<{GRP_n}>-?\d+(\.\d+)*)(?<{GRP_r}>-[^+]*|[^+]+)?(?<{GRP_b}>\+.*)?$", RegexOptions.Compiled);

    public const char SEPARATOR_DOT = '.';

    public const char DELIMITER_PRERELEASE = '-';

    public const char DELIMITER_BUILD = '+';

    private static readonly char[] FIRST_VERSION_TOKEN_CHARS = new char[] { SEPARATOR_DOT, '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    private static readonly char[] SEPARATOR_PreRelease = new char[] { DELIMITER_PRERELEASE, SEPARATOR_DOT };

    private static readonly char[] SEPARATOR_Build = new char[] { DELIMITER_BUILD, SEPARATOR_DOT, DELIMITER_PRERELEASE };

    public static readonly StringComparer TextComparer = StringComparer.OrdinalIgnoreCase;

    /// <summary>
    /// Gets the text version component that precedes other version components.
    /// A <see langword="null" /> value indicates there is no prefixed version component.
    /// </summary>
    /// <remarks>This will never be a zero-length string.</remarks>
    public string? Prefix { get; }

    /// <summary>
    /// Gets the value of first numerical version component indicating the major version number.
    /// </summary>
    /// <remarks>This will always be <c>0</c> when <see cref="Format" /> is <see cref="VersionStringFormat.NonNumerical" /> since non-standard version strings do not have parsable numerical components..</remarks>
    public int Major { get; }

    /// <summary>
    /// Gets the value of the second numerical version component indicating the minor version number.
    /// A <see langword="null" /> value indicates an implied <c>0</c> value.
    /// </summary>
    /// <remarks>This will always be <see langword="null" /> when <see cref="Format" /> is <see cref="VersionStringFormat.NonNumerical" />.
    /// This will never have a negative value.</remarks>
    public int? Minor { get; }

    /// <summary>
    /// Gets the value of the third numerical version component indicating the patch number.
    /// A <see langword="null" /> value indicates an implied <c>0</c> value.
    /// </summary>
    /// <remarks>This will always be <see langword="null" /> when <see cref="Format" /> is <see cref="VersionStringFormat.NonNumerical" />.
    /// This will never have a negative value.</remarks>
    public int? Patch { get; }

    /// <summary>
    /// Gets the value of the fourth numerical version component indicating the revision number.
    /// A <see langword="null" /> value indicates an implied <c>0</c> value.
    /// </summary>
    /// <remarks>This will always be <see langword="null" /> when <see cref="Format" /> is <see cref="VersionStringFormat.NonNumerical" />.
    /// This will never have a negative value.</remarks>
    public int? Revision { get; }

    /// <summary>
    /// Gets the numerical version components that follow the <see cref="Revision" /> components.
    /// A <see langword="null" /> value indicates there are no additinal numerical component.
    /// </summary>
    /// <remarks>This will never contain an empty collection; It will always have at least 1 element if it is not <see cref="null" />.</remarks>
    public ReadOnlyCollection<int>? AdditionalNumerical { get; }

    /// <summary>
    /// Gets the segments of the pre-release component, which follows the numerical components.
    /// A <see langword="null" /> value indicates there is no pre-release component.
    /// </summary>
    /// <remarks>This will never contain an empty collection; It will always have at least 1 element if it is not <see cref="null" />.</remarks>
    public ReadOnlyCollection<PreReleaseSegment>? PreRelease { get; }

    /// <summary>
    /// Gets the segments of the build component, which follows the numerical components.
    /// A <see langword="null" /> value indicates there is no build component.
    /// </summary>
    /// <remarks>This will never contain an empty collection; It will always have at least 1 element if it is not <see cref="null" />.</remarks>
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
        if (minor.HasValue && minor.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(minor));
        if (patch.HasValue && patch.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(patch));
        if (revision.HasValue && revision.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(revision));
        if (additionalNumerical is null)
            AdditionalNumerical = null;
        else
        {
            int[] an = additionalNumerical.ToArray();
            if (an.Length > 0)
            {
                if (an.Any(nextSparator => nextSparator < 0))
                    throw new ArgumentOutOfRangeException(nameof(additionalNumerical));
                AdditionalNumerical = new(an);
            }
            else
                AdditionalNumerical = null;
        }
        if (preRelease.TryGetFirst(out PreReleaseSegment prs))
        {
            if (prs.AltSeparator && prs.Value.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(preRelease));
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
            Build = new(build.ToArray());
        }
        else
            Build = null;
        Prefix = (prefix is null || prefix.Length == 0) ? null : prefix;
        Major = major;
        Minor = minor;
        Patch = patch;
        Revision = revision;
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
        if (versionString is null)
        {
            Format = VersionStringFormat.NonNumerical;
            Major = 0;
            Minor = Patch = Revision = null;
            AdditionalNumerical = null;
            Build = null;
            Prefix = string.Empty;
            PreRelease = null;
            return;
        }
        bool tryParseIntAt(int index, out string? nonNumeric, out int value, out int zeroPadCount, out int nextIndex)
        {
            int start = index;
            nextIndex = versionString!.Length;
            while (start < nextIndex)
            {
                if (char.IsNumber(versionString![start]))
                {
                    int end = start + 1;
                    while (end < versionString!.Length)
                    {
                        if (!char.IsNumber(versionString![end]))
                            break;
                    }
                    int nzi = start;
                    string v;
                    if (end < versionString!.Length)
                    {
                        while (versionString![nzi] == '0')
                        {
                            nzi++;
                            if (nzi == end)
                            {
                                zeroPadCount = (end - start) + 1;
                                value = 0;
                                nonNumeric = (start > index) ? versionString[index..start] : null;
                                nextIndex = end;
                                return true;
                            }
                        }
                        v = versionString[start..end];
                        if (int.TryParse(v, out value) && v.ToString() == v)
                        {
                            zeroPadCount = (end - start) + 1;
                            nonNumeric = (start > index) ? versionString[index..start] : null;
                            nextIndex = end;
                            return true;
                        }
                        start = end;
                    }
                    else
                    {
                        end = versionString!.Length;
                        while (versionString![nzi] == '0')
                        {
                            nzi++;
                            if (nzi == end)
                            {
                                zeroPadCount = (end - start) + 1;
                                value = 0;
                                nonNumeric = (start > index) ? versionString[index..start] : null;
                                return true;
                            }
                        }
                        v = (start == 0) ? versionString : versionString[start..];
                        if (int.TryParse(v, out value) && v.ToString() == v)
                        {
                            zeroPadCount = (end - start) + 1;
                            nonNumeric = (start > index) ? versionString[index..start] : null;
                            return true;
                        }
                        break;
                    }
                }
                else
                    start++;
            }
            nonNumeric = (start > index) ? versionString[index..start] : null;
            zeroPadCount = 0;
            value = 0;
            return false;
        }
        for (int startIndex = 0; startIndex < versionString.Length; startIndex++)
        {
            if (char.IsNumber(versionString[startIndex]))
            {
                int endIndex = startIndex + 1;
                if (endIndex < versionString.Length)
                {
                    while (char.IsNumber(versionString[endIndex]))
                    {
                        endIndex++;
                        if (endIndex == versionString.Length)
                            break;
                    }

                }
            }
        }
        Format = VersionStringFormat.NonNumerical;
        Major = 0;
        Minor = Patch = Revision = null;
        AdditionalNumerical = null;
        Build = null;
        Prefix = versionString;
        PreRelease = null;
        Match m = _versionRegex.Match(versionString);
        if (m.Success)
        {
            Collection<int> nv = new();
            string[] sv = m.Groups[GRP_n].Value.Split(SEPARATOR_DOT);
            foreach (string s in sv)
            {
                if (int.TryParse(s, out int n))
                    nv.Add(n);
                else
                {
                    nv.Clear();
                    break;
                }
            }
            var grp = m.Groups[GRP_p];
            string? preRelease;
            if (nv.Count == 0 || (nv[0] == 0 && sv[0][0] == DELIMITER_PRERELEASE))
            {
                Prefix = grp.Success ? grp.Value + m.Groups[GRP_n].Value : m.Groups[GRP_n].Value;
                Major = 0;
                Minor = Patch = Revision = null;
                AdditionalNumerical = null;
                Format = VersionStringFormat.NonNumerical;
                preRelease = (grp = m.Groups[GRP_r]).Success ? grp.Value : null;
            }
            else
            {
                if (grp.Success)
                    Prefix = grp.Value;
                else
                    Prefix = null;
                using IEnumerator<int> enumerator = nv.GetEnumerator();
                enumerator.MoveNext();
                Major = enumerator.Current;
                if (enumerator.MoveNext())
                {
                    Minor = enumerator.Current;
                    if (enumerator.MoveNext())
                    {
                        Patch = enumerator.Current;
                        if (enumerator.MoveNext())
                        {
                            Revision = enumerator.Current;
                            if (enumerator.MoveNext())
                            {
                                nv = new Collection<int>();
                                do { nv.Add(enumerator.Current); }
                                while (enumerator.MoveNext());
                                AdditionalNumerical = new(nv);
                            }
                            else
                                AdditionalNumerical = null;
                        }
                        else
                        {
                            Revision = null;
                            AdditionalNumerical = null;
                        }
                    }
                    else
                    {
                        Patch = Revision = null;
                        AdditionalNumerical = null;
                    }
                }
                else
                {
                    Minor = Patch = Revision = null;
                    AdditionalNumerical = null;
                }
                if ((grp = m.Groups[GRP_r]).Success)
                {
                    preRelease = grp.Value;
                    Format = (preRelease[0] == DELIMITER_PRERELEASE) ? VersionStringFormat.Standard : VersionStringFormat.Alt;
                }
                else
                {
                    preRelease = null;
                    Format = VersionStringFormat.Standard;
                }
            }
            if (preRelease is null)
                PreRelease = null;
            else
                PreRelease = new(ParsePreReleaseSegments(preRelease).ToArray());
            if ((grp = m.Groups[GRP_b]).Success)
                Build = new(ParseBuildSegments(grp.Value).ToArray());
            else
                Build = null;
        }
        else
        {
            Format = VersionStringFormat.NonNumerical;
            Major = 0;
            Minor = Patch = Revision = null;
            AdditionalNumerical = null;
            int index = versionString.IndexOf(DELIMITER_BUILD);
            if (index < 0)
                Build = null;
            else
            {
                Build = new(ParseBuildSegments(versionString[index..]).ToArray());
                versionString = versionString[..index];
            }
            if ((index = versionString.IndexOf(DELIMITER_PRERELEASE)) < 1)
            {
                Prefix = versionString;
                PreRelease = null;
            }
            else
            {
                Prefix = versionString[..index];
                PreRelease = new(ParsePreReleaseSegments(versionString[index..]).ToArray());
            }
        }
    }

    private IEnumerable<int> GetSubVersionNumberValues()
    {
        if (Minor.HasValue)
        {
            yield return Minor.Value;
            if (Patch.HasValue)
            {
                yield return Patch.Value;
                if (Revision.HasValue)
                {
                    yield return Revision.Value;
                    if (AdditionalNumerical is not null)
                        foreach (int v in AdditionalNumerical)
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
        int diff = Major - other.Major;
        if (diff != 0 || (diff = CompareVersionNumbers(GetSubVersionNumberValues(), other.GetSubVersionNumberValues())) != 0 ||
                (diff = CompareValues(PreRelease, other.PreRelease)) != 0 || (diff = CompareValues(Build, other.Build)) != 0)
            return diff;
        if (Prefix is null)
            return (other.Prefix is null) ? 0 : -1;
        return (other.Prefix is null) ? 1 : TextComparer.Compare(Prefix, other.Prefix);
    }

    public bool Equals(SwVersion other)
    {
        if (Major == other.Major && VersionNumbersEqual(GetSubVersionNumberValues(), other.GetSubVersionNumberValues()) && ValuesEqual(PreRelease, other.PreRelease) && ValuesEqual(Build, other.Build))
        {
            if (Prefix is null)
                return other.Prefix is null;
            return other.Prefix is not null && TextComparer.Equals(Prefix, other.Prefix);
        }
        return false;
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is SwVersion other && Equals(other);

    public override int GetHashCode()
    {
        // 1  2  3  4   5   6   7   8
        // 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59
        int hash = 23;
        unchecked
        {
            hash = (hash * 31) + Major;
            if (Minor.HasValue)
            {
                hash = (hash * 31) + Minor.Value;
                if (Patch.HasValue)
                {
                    hash = (hash * 31) + Patch.Value;
                    if (Revision.HasValue)
                    {
                        hash = (hash * 31) + Revision.Value;
                        if (AdditionalNumerical is not null)
                        {
                            int h = 3;
                            foreach (int n in AdditionalNumerical)
                                h = (h * 7) + n;
                            hash = (hash * 31) + h;
                        }
                    }
                }
            }
            if (PreRelease is not null)
            {
                int h = 3;
                foreach (PreReleaseSegment s in PreRelease)
                    h = (h * 7) + s.GetHashCode();
                hash = (hash * 31) + h;
            }
            if (Build is not null)
            {
                int h = 3;
                foreach (BuildSegment s in Build)
                    h = (h * 7) + s.GetHashCode();
                hash = (hash * 31) + h;
            }
            return (Prefix is null) ? hash : (hash * 31) + TextComparer.GetHashCode(Prefix);
        }
    }

    /// <summary>
    /// Converts the current version object to a formatted version string.
    /// </summary>
    /// <param name="minDigitCount">The minimum number of numerical components to display or <c>0</c> to only display the defined numerical components.</param>
    /// <param name="omitBuild">If <see langword="true" />, the <see cref="Build" /> components are omitted. The default is <see langword="false" />./param>
    /// <param name="omitPreRelease">If <see langword="true" />, the <see cref="PreRelease" /> components are omitted. The default is <see langword="false" />.</param>
    /// <param name="omitPrefix">If <see langword="true" />, the <see cref="Prefix" /> component is omitted. The default is <see langword="false" />.</param>
    /// <returns>A formatted version string according to the current <see cref="Format" />.</returns>
    public string ToString(int minDigitCount, bool omitBuild = false, bool omitPreRelease = false, bool omitPrefix = false)
    {
        StringBuilder sb;
        PreReleaseSegment prs;
        if (omitBuild || Build is null)
        {
            if (omitPreRelease || PreRelease is null)
            {
                if (AdditionalNumerical is null)
                {
                    if (Revision.HasValue)
                    {
                        if (minDigitCount < 5)
                        {
                            if (Prefix is null)
                                return $"{Major}.{Minor!.Value}.{Patch!.Value}.{Revision.Value}";
                            return $"{Prefix}{Major}.{Minor!.Value}.{Patch!.Value}.{Revision.Value}";
                        }
                        sb = ((Prefix is null) ? new StringBuilder() : new StringBuilder(Prefix)).Append(Major).Append('.').Append(Minor!.Value).Append('.').Append(Patch!.Value).Append('.').Append(Revision.Value);
                        for (int i = 4; i < minDigitCount; i++)
                            sb.Append(".0");
                        return sb.ToString();
                    }
                    if (Patch.HasValue)
                    {
                        if (minDigitCount < 4)
                        {
                            if (Prefix is null)
                                return $"{Major}.{Minor!.Value}.{Patch.Value}";
                            return $"{Prefix}{Major}.{Minor!.Value}.{Patch.Value}";
                        }
                        sb = ((Prefix is null) ? new StringBuilder() : new StringBuilder(Prefix)).Append(Major).Append('.').Append(Minor!.Value).Append('.').Append(Patch.Value);
                        for (int i = 3; i < minDigitCount; i++)
                            sb.Append(".0");
                        return sb.ToString();
                    }
                    if (Minor.HasValue)
                    {
                        if (minDigitCount < 3)
                        {
                            if (Prefix is null)
                                return $"{Major}.{Minor.Value}";
                            return $"{Prefix}{Major}.{Minor.Value}";
                        }
                        sb = ((Prefix is null) ? new StringBuilder() : new StringBuilder(Prefix)).Append(Major).Append('.').Append(Minor!.Value);
                        for (int i = 2; i < minDigitCount; i++)
                            sb.Append(".0");
                        return sb.ToString();
                    }
                    if (minDigitCount < 2)
                    {
                        if (Prefix is null)
                            return Major.ToString();
                        return $"{Prefix}{Major}";
                    }
                    sb = ((Prefix is null) ? new StringBuilder() : new StringBuilder(Prefix)).Append(Major);
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
