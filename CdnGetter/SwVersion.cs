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
    private const string GRP_p = "p";

    private const string GRP_n = "n";

    private const string GRP_r = "r";

    private const string GRP_b = "b";

    private static readonly Regex _versionRegex = new(@$"^(?<{GRP_p}>(-?[^\d.+-]+)+)?(?<{GRP_n}>-?\d+(\.\d+)*)(?<{GRP_r}>-[^+]*|[^+]+)?(?<{GRP_b}>\+.*)?", RegexOptions.Compiled);

    public const char SEPARATOR_Dot = '.';

    public const char LEADCHAR_PreRelease = '-';

    public const char LEADCHAR_Build = '+';

    private static readonly char[] SEPARATOR_PreRelease = new char[] { LEADCHAR_PreRelease, SEPARATOR_Dot };

    private static readonly char[] SEPARATOR_Build = new char[] { LEADCHAR_Build, SEPARATOR_Dot, LEADCHAR_PreRelease };

    public readonly StringComparer TextComparer = StringComparer.OrdinalIgnoreCase;

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
        if (string.IsNullOrEmpty(versionString))
        {
            Format = VersionStringFormat.NonNumerical;
            Major = 0;
            Minor = Patch = Revision = null;
            AdditionalNumerical = null;
            Build = null;
            Prefix = null;
            PreRelease = null;
            return;
        }
        Match m = _versionRegex.Match(versionString);
        if (m.Success)
        {
            Collection<int> nv = new();
            string[] sv = m.Groups[GRP_n].Value.Split(SEPARATOR_Dot);
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
            if (nv.Count == 0 || (nv[0] == 0 && sv[0][0] == LEADCHAR_PreRelease))
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
                    Format = (preRelease[0] == LEADCHAR_PreRelease) ? VersionStringFormat.Standard : VersionStringFormat.Alt;
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
            int index = versionString.IndexOf(LEADCHAR_Build);
            if (index < 0 || index == versionString.Length - 1)
                Build = null;
            else
            {
                Build = new(ParseBuildSegments(versionString[..index]).ToArray());
                versionString = versionString[index..];
            }
            if ((index = versionString.IndexOf(LEADCHAR_PreRelease)) < 0)
            {
                Prefix = versionString;
                PreRelease = null;
            }
            else
            {
                Prefix = (index > 0) ? versionString[..index] : null;
                PreRelease = new(ParsePreReleaseSegments(versionString[index..]).ToArray());
            }
        }
    }

    private static IEnumerable<BuildSegment> ParseBuildSegments(string text)
    {
        int previousSeparator = text.IndexOfAny(SEPARATOR_Build, 1);
        if (previousSeparator < 0)
        {
            yield return new BuildSegment(BuildSeparator.Plus, text[1..]);
            yield break;
        }
        yield return new BuildSegment(BuildSeparator.Plus, (previousSeparator > 1) ? text[1..previousSeparator] : "");
        int startIndex = previousSeparator + 1;
        while (startIndex < text.Length)
        {
            int nextSeparator = text.IndexOfAny(SEPARATOR_PreRelease, startIndex);
            if (nextSeparator < 0)
                switch (text[previousSeparator])
                {
                    case SEPARATOR_Dot:
                        yield return new BuildSegment(BuildSeparator.Dot, (startIndex < text.Length - 1) ? text[startIndex..] : "");
                        yield break;
                    case LEADCHAR_PreRelease:
                        yield return new BuildSegment(BuildSeparator.Dash, (startIndex < text.Length - 1) ? text[startIndex..] : "");
                        yield break;
                    default:
                        yield return new BuildSegment(BuildSeparator.Plus, (startIndex < text.Length - 1) ? text[startIndex..] : "");
                        yield break;
                }

            switch (text[previousSeparator])
            {
                case SEPARATOR_Dot:
                    yield return new BuildSegment(BuildSeparator.Dot, (startIndex < nextSeparator - 1) ? text[startIndex..nextSeparator] : "");
                    break;
                case LEADCHAR_PreRelease:
                    yield return new BuildSegment(BuildSeparator.Dash, (startIndex < nextSeparator - 1) ? text[startIndex..nextSeparator] : "");
                    break;
                default:
                    yield return new BuildSegment(BuildSeparator.Plus, (startIndex < nextSeparator - 1) ? text[startIndex..nextSeparator] : "");
                    break;
            }
            startIndex = nextSeparator + 1;
        }
    }

    private static IEnumerable<PreReleaseSegment> ParsePreReleaseSegments(string text)
    {
        int previousSeparator;
        if (text[0] == LEADCHAR_PreRelease)
            previousSeparator = 0;
        else if ((previousSeparator = text.IndexOfAny(SEPARATOR_PreRelease)) < 0)
        {
            yield return new PreReleaseSegment(true, text);
            yield break;
        }
        int startIndex = previousSeparator + 1;
        while (startIndex < text.Length)
        {
            int nextSeparator = text.IndexOfAny(SEPARATOR_PreRelease, startIndex);
            if (nextSeparator < 0)
            {
                yield return new PreReleaseSegment(text[previousSeparator] != LEADCHAR_PreRelease, (startIndex < text.Length - 1) ? text[startIndex..] : "");
                yield break;
            }
            yield return new PreReleaseSegment(text[previousSeparator] != LEADCHAR_PreRelease, (startIndex < nextSeparator - 1) ? text[startIndex..nextSeparator] : "");
            startIndex = nextSeparator + 1;
        }
    }

    public int CompareTo(SwVersion other)
    {
        throw new NotImplementedException();
    }

    public bool Equals(SwVersion other)
    {
        throw new NotImplementedException();
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        // TODO: Implement Equals(object?)
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        // TODO: Implement GetHashCode()
        return base.GetHashCode();
    }

    /// <summary>
    /// Converts the current version object to a formatted version string.
    /// </summary>
    /// <param name="minDigitCount">The minimum number of numerical components to display or <c>0</c> to only display the defined numerical components.</param>
    /// <param name="omitBuild">If <see langword="true" />, the <see cref="Build" /> components are omitted. The default is <see langword="false" />./param>
    /// <param name="omitPreRelease">If <see langword="true" />, the <see cref="PreRelease" /> components are omitted. The default is <see langword="false" />.</param>
    /// <returns>A formatted version string according to the current <see cref="Format" />.</returns>
    public string ToString(int minDigitCount, bool omitBuild = false, bool omitPreRelease = false)
    {
        // TODO: Implement ToString(int, bool, bool)
        return base.ToString()!;
    }

    public override string ToString() => ToString(0);

    public static bool operator ==(SwVersion left, SwVersion right) => left.Equals(right);

    public static bool operator !=(SwVersion left, SwVersion right) => !left.Equals(right);

    public static bool operator <(SwVersion left, SwVersion right) => left.CompareTo(right) < 0;

    public static bool operator <=(SwVersion left, SwVersion right) => left.CompareTo(right) <= 0;

    public static bool operator >(SwVersion left, SwVersion right) => left.CompareTo(right) > 0;

    public static bool operator >=(SwVersion left, SwVersion right) => left.CompareTo(right) >= 0;
}
