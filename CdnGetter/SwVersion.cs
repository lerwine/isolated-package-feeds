using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CdnGetter;

public readonly struct SwVersion : IEquatable<SwVersion>, IComparable<SwVersion>
{
    public enum VersionStringFormat
    {
        Standard,
        Alt,
        NonStandard
    }

    public record ExtraVersionComponent(char? Lead, string Value);

    private static readonly StringComparer _comparer = StringComparer.OrdinalIgnoreCase;
    public static readonly ValueConverter<SwVersion, string> Converter = new(
        v => v.ToString(),
        s => Parse(s)
    );

    public const char LEAD_CHAR_PreRelease = '-';
    
    private static readonly char[] PreReleaseSeparators = new[] { '-', '.' };

    public const char LEAD_CHAR_Build = '+';
    
    private static readonly char[] BuildSeparators = new[] { '-', '.', '+' };

    public const string GROUP_NAME_prefix = "prefix";

    public const string GROUP_NAME_major = "major";

    public const string GROUP_NAME_minor = "minor";
    
    public const string GROUP_NAME_patch = "patch";
    
    public const string GROUP_NAME_revision = "revision";
    
    public const string GROUP_NAME_minorRevision = "minorRevision";
    
    public const string GROUP_NAME_preReleaseValue = "preRelease";
    
    public const string GROUP_NAME_buildValue = "build";
    
    public static readonly Regex StandardFormatRegex = new(@$"^(?<{GROUP_NAME_prefix}>([^\d-]+|-\D)+)?(?<{GROUP_NAME_major}>-?\d+)(\.(?<{GROUP_NAME_minor}>\d+)(\.(?<{GROUP_NAME_patch}>\d+)(\.(?<{GROUP_NAME_revision}>\d+)(\.(?<{GROUP_NAME_minorRevision}>\d+))?)?)?)?(-(?<{GROUP_NAME_preReleaseValue}>[^\s+]+))?(\+(?<{GROUP_NAME_buildValue}>\S+))?$", RegexOptions.Compiled);
    
    public static readonly Regex AltFormatRegex = new(@$"^(?<{GROUP_NAME_prefix}>([^\d-]+|-\D)+)?(?<{GROUP_NAME_major}>-?\d+)(\.(?<{GROUP_NAME_minor}>\d+)(\.(?<{GROUP_NAME_patch}>\d+)(\.(?<{GROUP_NAME_revision}>\d+)(\.(?<{GROUP_NAME_minorRevision}>\d+))?)?)?)?(?<{GROUP_NAME_preReleaseValue}>[^+\s-]\S*)?$", RegexOptions.Compiled);
    
    public static readonly Regex ExtraneousLeadingZeroRegex = new(@"^-0+(?=0($|\D))|((?<=^-)|^)0+(?=[1-9])", RegexOptions.Compiled);

    public string Prefix { get; }

    public int Major { get; }
    
    public int? Minor { get; }
    
    public int? Patch { get; }
    
    public int? Revision { get; }
    
    public int? MinorRevision { get; }
    
    public ReadOnlyCollection<ExtraVersionComponent> PreRelease { get; }
    
    public ReadOnlyCollection<ExtraVersionComponent> Build { get; }

    public VersionStringFormat Format { get; }

    private SwVersion(Match match, bool isAltFormat)
    {
        VersionStringFormat format = isAltFormat ? VersionStringFormat.Alt : VersionStringFormat.Standard;
        if (int.TryParse(match.Groups[GROUP_NAME_major].Value, out int value))
            Major = value;
        else
        {
            Major = 0;
            format = VersionStringFormat.NonStandard;
        }
        Group mg = match.Groups[GROUP_NAME_minor];
        if (mg.Success)
        {
            if (int.TryParse(mg.Value, out value))
                Minor = value;
            else
            {
                Minor = 0;
                format = VersionStringFormat.NonStandard;
            }
            if ((mg = match.Groups[GROUP_NAME_patch]).Success)
            {
                if (int.TryParse(mg.Value, out value))
                    Patch = value;
                else
                {
                    Patch = 0;
                    format = VersionStringFormat.NonStandard;
                }
                if ((mg = match.Groups[GROUP_NAME_revision]).Success)
                {
                    if (int.TryParse(mg.Value, out value))
                        Revision = value;
                    else
                    {
                        Revision = 0;
                        format = VersionStringFormat.NonStandard;
                    }
                    if ((mg = match.Groups[GROUP_NAME_minorRevision]).Success)
                    {
                        if (int.TryParse(mg.Value, out value))
                            MinorRevision = value;
                        else
                        {
                            MinorRevision = 0;
                            format = VersionStringFormat.NonStandard;
                        }
                    }
                    else
                        MinorRevision = null;
                }
                else
                    Revision = MinorRevision = null;
            }
            else
                Patch = Revision = MinorRevision = null;
        }
        else
            Minor = Patch = Revision = MinorRevision = null;
        if ((Format = format) == VersionStringFormat.NonStandard)
        {
            Prefix = match.Value;
            PreRelease = new(Array.Empty<ExtraVersionComponent>());
            Build = new(Array.Empty<ExtraVersionComponent>());
        }
        else
        {
            Prefix = (mg = match.Groups[GROUP_NAME_prefix]).Success ? mg.Value : "";
            if ((mg = match.Groups[GROUP_NAME_preReleaseValue]).Success)
                PreRelease = new(ParseComponents(LEAD_CHAR_PreRelease, mg.Value, PreReleaseSeparators).ToArray());
            else
                PreRelease = new(Array.Empty<ExtraVersionComponent>());
            if (format == VersionStringFormat.Standard && (mg = match.Groups[GROUP_NAME_buildValue]).Success)
                Build = new(ParseComponents(LEAD_CHAR_Build, mg.Value, BuildSeparators).ToArray());
            else
                Build = new(Array.Empty<ExtraVersionComponent>());
        }
    }

    private static IEnumerable<ExtraVersionComponent> ParseComponents(char initialLead, string source, char[] separators)
    {
        int index = source.IndexOfAny(separators);
        if (index < 0)
            yield return new(initialLead, source);
        else
        {
            yield return new(initialLead, (index == 0) ? "" : source[..index]);
            int startIndex = index + 1;
            while (startIndex < source.Length)
            {
                char c = source[index];
                if ((index = source.IndexOfAny(separators, startIndex)) < 0)
                {
                    yield return new(c, source[startIndex..]);
                    yield break;
                }
                yield return new(source[index], (startIndex < index) ? source[startIndex..index] :  "");
                startIndex = index + 1;
            }
            yield return new(source[index], "");
        }
    }
    
    private SwVersion(string nonStandard)
    {
        Prefix = nonStandard;
        Format = VersionStringFormat.NonStandard;
        Major = 0;
        Minor = Patch = Revision = MinorRevision = null;
        PreRelease = new(Array.Empty<ExtraVersionComponent>());
        Build = new(Array.Empty<ExtraVersionComponent>());
    }
    
    private SwVersion(ExtraVersionComponent[]? preRelease, ExtraVersionComponent[]? build, int major, int? minor = null, int? patch = null, int? revision = null, int? minorRevision = null)
    {
        Prefix = "";
        Major = major;
        if ((Minor = minor).HasValue && minor!.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(minor));
        if ((Patch = patch).HasValue && patch!.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(patch));
        if ((Revision = revision).HasValue && revision!.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(revision));
        if ((MinorRevision = minorRevision).HasValue && minorRevision!.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(minorRevision));
        if (preRelease is not null && preRelease.Length > 0)
        {
            char? c = preRelease[0].Lead;
            if (c.HasValue)
            {
                if (c.Value != LEAD_CHAR_PreRelease || preRelease.Skip(1).Any(p => !(p.Lead.HasValue && PreReleaseSeparators.Contains(p.Lead.Value))))
                    throw new ArgumentOutOfRangeException(nameof(preRelease));
                Format = VersionStringFormat.Standard;
            }
            else
            {
                if (preRelease[0].Value.Length == 0 || preRelease.Skip(1).Any(p => !(p.Lead.HasValue && PreReleaseSeparators.Contains(p.Lead.Value))))
                    throw new ArgumentOutOfRangeException(nameof(preRelease));
                Format = VersionStringFormat.Alt;
            }
            PreRelease = new(preRelease);
        }
        else
        {
            Format = VersionStringFormat.Standard;
            PreRelease = new(Array.Empty<ExtraVersionComponent>());
        }
        if (build is not null && build.Length > 0)
        {
            char? c = build[0].Lead;
            if (!c.HasValue || c.Value != LEAD_CHAR_Build || build.Skip(1).Any(p => !(p.Lead.HasValue && BuildSeparators.Contains(p.Lead.Value))))
                throw new ArgumentOutOfRangeException(nameof(build));
            Build = new(build);
        }
        else
            Build = new(Array.Empty<ExtraVersionComponent>());
    }
    
    public SwVersion(int major, int minor, int patch, int revision, int minorRevision, ExtraVersionComponent[]? preRelease, params ExtraVersionComponent[] build) : this(preRelease, build, major, minor, patch, revision, minorRevision) { }
    
    public SwVersion(int major, int minor, int patch, int revision, ExtraVersionComponent[]? preRelease, params ExtraVersionComponent[] build) : this(preRelease, build, major, minor, patch, revision) { }
    
    public SwVersion(int major, int minor, int patch, ExtraVersionComponent[]? preRelease, params ExtraVersionComponent[] build) : this(preRelease, build, major, minor, patch) { }
    
    public SwVersion(int major, int minor, ExtraVersionComponent[]? preRelease, params ExtraVersionComponent[] build) : this(preRelease, build, major, minor) { }
    
    public SwVersion(int major, ExtraVersionComponent[]? preRelease, params ExtraVersionComponent[] build) : this(preRelease, build, major) { }
    
    public SwVersion(int major, int minor, int patch, int revision, int minorRevision, params ExtraVersionComponent[] preRelease) : this(preRelease, null, major, minor, patch, revision, minorRevision) { }
    
    public SwVersion(int major, int minor, int patch, int revision, params ExtraVersionComponent[] preRelease) : this(preRelease, null, major, minor, patch, revision) { }
    
    public SwVersion(int major, int minor, int patch, params ExtraVersionComponent[] preRelease) : this(preRelease, null, major, minor, patch) { }
    
    public SwVersion(int major, int minor, params ExtraVersionComponent[] preRelease) : this(preRelease, null, major, minor) { }
    
    public SwVersion(int major, params ExtraVersionComponent[] preRelease) : this(preRelease, null, major) { }
    
    public static bool TryParse(string? versionString, [NotNullWhen(true)] out SwVersion? result)
    {
        if ((versionString = versionString.ToTrimmedOrNullIfEmpty()) is not null)
        {
            Match match = StandardFormatRegex.Match(versionString);
            result = match.Success ? new(match, false) : (match = AltFormatRegex.Match(versionString)).Success ? new(match, true) : new(versionString);
            return true;
        }
        result = null;
        return false;
    }

    public static SwVersion Parse(string versionString)
    {
        if ((versionString = versionString.ToTrimmedOrNullIfEmpty()!) is null)
            throw new ArgumentException($"'{nameof(versionString)}' cannot be null or whitespace.", nameof(versionString));

        Match match = StandardFormatRegex.Match(versionString);
        return match.Success ? new(match, false) : (match = AltFormatRegex.Match(versionString)).Success ? new(match, true) : new(versionString);
    }

    public bool Equals(SwVersion other)
    {
        if (Format == VersionStringFormat.NonStandard)
            return other.Format == VersionStringFormat.NonStandard && Prefix == other.Prefix;
        if (Format != other.Format || Major != other.Major)
            return false;
        static bool nullablesNotEqual(int? x, int? y)
        {
            return x.HasValue ? x.Value != (y ?? 0) : y.HasValue && y.Value != 0;
        }
        if (nullablesNotEqual(Minor, other.Minor) || nullablesNotEqual(Patch, other.Patch) || nullablesNotEqual(Revision, other.MinorRevision) || nullablesNotEqual(Revision, other.MinorRevision) || Format != other.Format || Prefix != other.Prefix)
            return false;
        static bool sequencesEqual(IEnumerable<ExtraVersionComponent> x, IEnumerable<ExtraVersionComponent> y)
        {
            using IEnumerator<ExtraVersionComponent> enumerator1 = x.GetEnumerator();
            using IEnumerator<ExtraVersionComponent> enumerator2 = y.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                if (!(enumerator2.MoveNext() && enumerator1.Current.Lead == enumerator2.Current.Lead && _comparer.Equals(enumerator1.Current.Value, enumerator2.Current.Value)))
                    return false;
            }
            return !enumerator2.MoveNext();
        }
        return Prefix == other.Prefix && sequencesEqual(PreRelease, other.PreRelease) && sequencesEqual(Build, other.Build);
    }

    public override bool Equals(object? obj) => obj is not null && obj is SwVersion s && Equals(s);
    
    public int CompareTo(SwVersion other)
    {
        if (Format == VersionStringFormat.NonStandard && other.Format == VersionStringFormat.NonStandard)
            return Prefix.CompareTo(other.Prefix);
        int result = Major - other.Major;
        if (result == 0)
        {
            static int compareNullables(int? x, int? y)
            {
                return x.HasValue ? (y.HasValue ? x.Value - y.Value : x.Value) : y.HasValue ? 0 - y.Value : 0;
            }
            if ((result = compareNullables(Minor, other.Minor)) == 0 && (result = compareNullables(Patch, other.Patch)) == 0 && (result = compareNullables(Revision, other.Revision)) == 0 &&
                (result = compareNullables(MinorRevision, other.MinorRevision)) == 0)
            {
                static int compareSequences(IEnumerable<ExtraVersionComponent> x, IEnumerable<ExtraVersionComponent> y)
                {
                    using IEnumerator<ExtraVersionComponent> enumerator1 = x.GetEnumerator();
                    using IEnumerator<ExtraVersionComponent> enumerator2 = y.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        if (!enumerator2.MoveNext())
                            return -1;
                        char? c1 = enumerator1.Current.Lead;
                        char? c2 = enumerator2.Current.Lead;
                        int r = c1.HasValue ? (c2.HasValue ? c1.Value.CompareTo(c2.Value) : -1) : c2.HasValue ? 1 : 0;
                        if (r != 0 || (r = _comparer.Compare(enumerator1.Current.Value, enumerator2.Current.Value)) != 0)
                            return r;
                    }
                    return enumerator2.MoveNext() ? 1 : 0;
                }
                if ((result = compareSequences(PreRelease, other.PreRelease)) == 0 && (result = compareSequences(PreRelease, other.PreRelease)) == 0 && (result = Format.CompareTo(other.Format)) == 0)
                    return Prefix.CompareTo(other.Prefix);
            }
        }
        return result;
    }

    public override int GetHashCode()
    {
        if (Format == VersionStringFormat.NonStandard)
            return Prefix.GetHashCode();
        int hash;
        unchecked
        {
            hash = (Major == 0) ? 0 : 1376 + Major;
            if (MinorRevision.HasValue && MinorRevision.Value != 0)
            {
                hash = hash * 43 + Minor!.Value;
                hash = hash * 43 + Patch!.Value;
                hash = hash * 43 + Revision!.Value;
                hash = hash * 43 + MinorRevision.Value;
            }
            else
            {
                hash *= 43;
                if (Revision.HasValue && Revision.Value != 0)
                {
                    hash = hash * 43 + Minor!.Value;
                    hash = hash * 43 + Patch!.Value;
                    hash = hash * 43 + Revision!.Value;
                }
                else
                {
                    hash *= 43;
                    if (Patch.HasValue && Patch.Value != 0)
                    {
                        hash = hash * 43 + Minor!.Value;
                        hash = hash * 43 + Patch!.Value;
                    }
                    else
                    {
                        hash *= 43;
                        if (Minor.HasValue && Minor.Value != 0)
                            hash = hash * 43 + Minor!.Value;
                        else
                            hash *= 43;
                    }
                }
            }
            int ch = 11;
            foreach (ExtraVersionComponent s in PreRelease)
            {
                char? c = s.Lead;
                if (c.HasValue)
                    ch = ch * 17 + c.Value.GetHashCode();
                else
                    ch *= 17;
                ch = ch * 17 + _comparer.GetHashCode(s.Value);
            }
            hash = hash * 43 + ch;
            ch = 11;
            foreach (ExtraVersionComponent s in Build)
            {
                char? c = s.Lead;
                if (c.HasValue)
                    ch = ch * 17 + c.Value.GetHashCode();
                else
                    ch *= 17;
                ch = ch * 17 + _comparer.GetHashCode(s.Value);
            }
            hash = hash * 43 + ch;
            hash = hash * 43 + Format.GetHashCode();
            hash = hash * 43 + Prefix.GetHashCode();
        }
        return hash;
    }

    public override string ToString() => ToString(0);

    public IEnumerable<int> GetNumericalComponents(int minSegments)
    {
        yield return Major;
        if (Minor.HasValue)
        {
            yield return Minor.Value;
            if (Patch.HasValue)
            {
                yield return Patch.Value;
                if (Revision.HasValue)
                {
                    yield return Revision.Value;
                    if (MinorRevision.HasValue)
                        yield return MinorRevision.Value;
                    else if (minSegments > 4)
                        yield return 0;
                }
                else
                {
                    if (minSegments > 5)
                        minSegments = 5;
                    for (int i = 3; i < minSegments; i++)
                        yield return 0;
                }
            }
            else
            {
                if (minSegments > 5)
                    minSegments = 5;
                for (int i = 2; i < minSegments; i++)
                    yield return 0;
            }
        }
        else
        {
            if (minSegments > 5)
                minSegments = 5;
            for (int i = 1; i < minSegments; i++)
                yield return 0;
        }
    }

    public string ToString(int minSegments, bool omitBuild = false, bool omitPreRelease = false)
    {
        StringBuilder stringBuilder;
        IEnumerable<int> numericalComponents;
        switch (Format)
        {
            case VersionStringFormat.NonStandard:
                return Prefix;
            case VersionStringFormat.Alt:
                numericalComponents = GetNumericalComponents(minSegments);
                if (omitPreRelease || PreRelease.Count == 0)
                    return Prefix + string.Join(".", numericalComponents.Select(n => n.ToString()));
                using (IEnumerator<int> enumerator = numericalComponents.GetEnumerator())
                {
                    stringBuilder = new StringBuilder(Prefix).Append(enumerator.Current);
                    while (enumerator.MoveNext())
                        stringBuilder.Append('.').Append(enumerator.Current);
                    
                    foreach (ExtraVersionComponent vc in PreRelease)
                    {
                        char? c = vc.Lead;
                        if (c.HasValue)
                        {
                            if (vc.Value.Length > 0)
                                stringBuilder.Append(c.Value).Append(vc.Value);
                            else
                                stringBuilder.Append(c.Value);
                        }
                        else
                            stringBuilder.Append(vc.Value);
                    }
                    return stringBuilder.ToString();
                }
            default:
                numericalComponents = GetNumericalComponents(minSegments);
                if (omitPreRelease || PreRelease.Count == 0)
                {
                    if (omitBuild || Build.Count == 0)
                        return Prefix + string.Join(".", numericalComponents.Select(n => n.ToString()));
                    using IEnumerator<int> enumerator = numericalComponents.GetEnumerator();
                    enumerator.MoveNext();
                    stringBuilder = new StringBuilder(Prefix).Append(enumerator.Current);
                    while (enumerator.MoveNext())
                        stringBuilder.Append('.').Append(enumerator.Current);
                    foreach (ExtraVersionComponent vc in Build)
                        if (vc.Value.Length > 0)
                            stringBuilder.Append(vc.Lead!.Value).Append(vc.Value);
                        else
                            stringBuilder.Append(vc.Lead!.Value);
                }
                else
                {
                    using IEnumerator<int> enumerator = numericalComponents.GetEnumerator();
                    enumerator.MoveNext();
                    stringBuilder = new StringBuilder(Prefix).Append(enumerator.Current);
                    while (enumerator.MoveNext())
                        stringBuilder.Append('.').Append(enumerator.Current);
                    foreach (ExtraVersionComponent vc in PreRelease)
                    {
                        char? c = vc.Lead;
                        if (c.HasValue)
                        {
                            if (vc.Value.Length > 0)
                                stringBuilder.Append(c.Value).Append(vc.Value);
                            else
                                stringBuilder.Append(c.Value);
                        }
                        else
                            stringBuilder.Append(vc.Value);
                    }
                    if (omitBuild || Build.Count == 0)
                        return stringBuilder.ToString();
                }
                foreach (ExtraVersionComponent vc in Build)
                    if (vc.Value.Length > 0)
                        stringBuilder.Append(vc.Lead!.Value).Append(vc.Value);
                    else
                        stringBuilder.Append(vc.Lead!.Value);
                return stringBuilder.ToString();
        }
    }
    
    public static bool operator ==(SwVersion left, SwVersion right) => left.Equals(right);

    public static bool operator !=(SwVersion left, SwVersion right) => !left.Equals(right);

    public static bool operator <(SwVersion left, SwVersion right) => left.CompareTo(right) < 0;

    public static bool operator <=(SwVersion left, SwVersion right) => left.CompareTo(right) <= 0;

    public static bool operator >(SwVersion left, SwVersion right) => left.CompareTo(right) > 0;

    public static bool operator >=(SwVersion left, SwVersion right) => left.CompareTo(right) >= 0;
}