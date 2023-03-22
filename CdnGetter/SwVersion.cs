using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CdnGetter;

public readonly struct SwVersion : IEquatable<SwVersion>, IComparable<SwVersion>
{
    private static readonly StringComparer _comparer = StringComparer.OrdinalIgnoreCase;
    public static readonly ValueConverter<SwVersion, string> Converter = new(
        v => v.ToString(),
        s => Parse(s)
    );

    public const string GROUP_NAME_major = "major";
    public const string GROUP_NAME_minor = "minor";
    public const string GROUP_NAME_patch = "patch";
    public const string GROUP_NAME_revision = "revision";
    public const string GROUP_NAME_minorRevision = "minorRevision";
    public const string GROUP_NAME_preRelease = "preRelease";
    public const string GROUP_NAME_build = "build";
    public static readonly Regex ParsingRegex = new(@$"$(?<{GROUP_NAME_major}>-?\d+)(\.(?<{GROUP_NAME_minor}>\d+)(\.(?<{GROUP_NAME_patch}>\d+)(\.(?<{GROUP_NAME_revision}>\d+)(\.(?<{GROUP_NAME_minorRevision}>\d+))?)?)?)?(-(?<{GROUP_NAME_preRelease}>[\w-]+(\.[\w-]+)*))?(\+(?<{GROUP_NAME_build}>[\w-]+(\.[\w-]+)*))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    public static readonly Regex ExtraneousLeadingZeroRegex = new(@"^-0+(?=0($|\D))|((?<=^-)|^)0+(?=[1-9])", RegexOptions.Compiled);

    public int Major { get; }
    
    public int? Minor { get; }
    
    public int? Patch { get; }
    
    public int? Revision { get; }
    
    public int? MinorRevision { get; }
    
    public ReadOnlyCollection<string> PreRelease { get; }
    
    public ReadOnlyCollection<string> Build { get; }

    private SwVersion(Match match)
    {
        Major = int.Parse(match.Groups[GROUP_NAME_major].Value);
        Group mg = match.Groups[GROUP_NAME_minorRevision];
        if (mg.Success)
        {
            MinorRevision = int.Parse(mg.Value);
            Revision = int.Parse(match.Groups[GROUP_NAME_revision].Value);
            Patch = int.Parse(match.Groups[GROUP_NAME_patch].Value);
            Minor = int.Parse(match.Groups[GROUP_NAME_minor].Value);
        }
        else
        {
            MinorRevision = null;
            if ((mg = match.Groups[GROUP_NAME_revision]).Success)
            {
                Revision = int.Parse(mg.Value);
                Patch = int.Parse(match.Groups[GROUP_NAME_patch].Value);
                Minor = int.Parse(match.Groups[GROUP_NAME_minor].Value);
            }
            else
            {
                Revision = null;
                if ((mg = match.Groups[GROUP_NAME_patch]).Success)
                {
                    Patch = int.Parse(mg.Value);
                    Minor = int.Parse(match.Groups[GROUP_NAME_minor].Value);
                }
                else
                {
                    Patch = null;
                    if ((mg = match.Groups[GROUP_NAME_minor]).Success)
                        Minor = int.Parse(mg.Value);
                    else
                        Minor = null;
                }
            }
        }
        if ((mg = match.Groups[GROUP_NAME_preRelease]).Success)
            PreRelease = new(mg.Value.Split('.').Select(s => ExtraneousLeadingZeroRegex.Replace(s, "")).ToArray());
        else
            PreRelease = new(Array.Empty<string>());
        if ((mg = match.Groups[GROUP_NAME_build]).Success)
            Build = new(mg.Value.Split('.').Select(s => ExtraneousLeadingZeroRegex.Replace(s, "")).ToArray());
        else
            Build = new(Array.Empty<string>());
    }
    
    private SwVersion(string[]? preRelease, string[]? build, int major, int? minor = null, int? patch = null, int? revision = null, int? minorRevision = null)
    {
        Major = major;
        if ((Minor = minor).HasValue && minor!.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(minor));
        if ((Patch = patch).HasValue && patch!.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(patch));
        if ((Revision = revision).HasValue && revision!.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(revision));
        if ((MinorRevision = minorRevision).HasValue && minorRevision!.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(minorRevision));
        PreRelease = new((preRelease is null) ? Array.Empty<string>() : preRelease.Select(s => (s is not null && (s = s.Trim()).Length > 0) ? ExtraneousLeadingZeroRegex.Replace(s, "") : "").ToArray());
        Build = new((build is null) ? Array.Empty<string>() : build.Select(s => (s is not null && (s = s.Trim()).Length > 0) ? ExtraneousLeadingZeroRegex.Replace(s, "") : "").ToArray());
    }
    
    public SwVersion(int major, int minor, int patch, int revision, int minorRevision, string[]? preRelease, params string[] build) : this(preRelease, build, major, minor, patch, revision, minorRevision) { }
    
    public SwVersion(int major, int minor, int patch, int revision, string[]? preRelease, params string[] build) : this(preRelease, build, major, minor, patch, revision) { }
    
    public SwVersion(int major, int minor, int patch, string[]? preRelease, params string[] build) : this(preRelease, build, major, minor, patch) { }
    
    public SwVersion(int major, int minor, string[]? preRelease, params string[] build) : this(preRelease, build, major, minor) { }
    
    public SwVersion(int major, string[]? preRelease, params string[] build) : this(preRelease, build, major) { }
    
    public SwVersion(int major, int minor, int patch, int revision, int minorRevision, params string[] preRelease) : this(preRelease, null, major, minor, patch, revision, minorRevision) { }
    
    public SwVersion(int major, int minor, int patch, int revision, params string[] preRelease) : this(preRelease, null, major, minor, patch, revision) { }
    
    public SwVersion(int major, int minor, int patch, params string[] preRelease) : this(preRelease, null, major, minor, patch) { }
    
    public SwVersion(int major, int minor, params string[] preRelease) : this(preRelease, null, major, minor) { }
    
    public SwVersion(int major, params string[] preRelease) : this(preRelease, null, major) { }
    
    public static bool TryParse(string? versionString, [NotNullWhen(true)] out SwVersion? result)
    {
        if ((versionString = versionString.ToTrimmedOrNullIfEmpty()) is not null)
        {
            Match match = ParsingRegex.Match(versionString);
            if (match.Success)
            {
                result = new(match);
                return true;
            }
        }
        result = null;
        return false;
    }

    public static SwVersion Parse(string versionString)
    {
        if ((versionString = versionString.ToTrimmedOrNullIfEmpty()!) is null)
            throw new ArgumentException($"'{nameof(versionString)}' cannot be null or whitespace.", nameof(versionString));

        Match match = ParsingRegex.Match(versionString);
        if (match.Success)
            return new(match);
        throw new ArgumentException($"'{nameof(versionString)}' is not a valid version format.", nameof(versionString));
    }

    public bool Equals(SwVersion other)
    {
        if (Major != other.Major || (Minor ?? 0) != (other.Minor ?? 0) || (Patch ?? 0) != (other.Patch ?? 0) || (Revision ?? 0) != (other.Revision ?? 0) || (MinorRevision ?? 0) != (other.MinorRevision ?? 0))
            return false;
        using IEnumerator<string> enumerator1 = PreRelease.GetEnumerator();
        using IEnumerator<string> enumerator2 = other.PreRelease.GetEnumerator();
        while (enumerator1.MoveNext())
        {
            if (!(enumerator2.MoveNext() && _comparer.Equals(enumerator1.Current, enumerator2.Current)))
                return false;
        }
        if (enumerator2.MoveNext())
            return false;
        using IEnumerator<string> enumerator3 = Build.GetEnumerator();
        using IEnumerator<string> enumerator4 = other.Build.GetEnumerator();
        while (enumerator3.MoveNext())
        {
            if (!(enumerator4.MoveNext() && _comparer.Equals(enumerator3.Current, enumerator4.Current)))
                return false;
        }
        return !enumerator4.MoveNext();
    }

    public override bool Equals(object? obj) => obj is not null && obj is SwVersion s && Equals(s);
    
    public int CompareTo(SwVersion other)
    {
        int result = Major - other.Major;
        if (result == 0 && (result = (Minor ?? 0) - (other.Minor ?? 0)) == 0 && (result = (Patch ?? 0) - (other.Patch ?? 0)) == 0 && (result = (Revision ?? 0) - (other.Revision ?? 0)) == 0 &&
            (result = (MinorRevision ?? 0) - (other.MinorRevision ?? 0)) == 0)
        {
            using IEnumerator<string> enumerator1 = PreRelease.GetEnumerator();
            using IEnumerator<string> enumerator2 = other.PreRelease.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                if (enumerator2.MoveNext())
                {
                    if ((result = _comparer.Compare(enumerator1.Current, enumerator2.Current)) != 0)
                        return result;
                }
                else
                    return -1;
            }
            if (enumerator2.MoveNext())
                return 1;
            using IEnumerator<string> enumerator3 = Build.GetEnumerator();
            using IEnumerator<string> enumerator4 = other.Build.GetEnumerator();
            while (enumerator3.MoveNext())
            {
                if (enumerator4.MoveNext())
                {
                    if ((result = _comparer.Compare(enumerator3.Current, enumerator4.Current)) != 0)
                        return result;
                }
                else
                    return -1;
            }
            if (enumerator4.MoveNext())
                return 1;
        }
        return result;
    }

    public override int GetHashCode()
    {
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
            foreach (string s in PreRelease)
                hash = hash * 43 + _comparer.GetHashCode(s);
            foreach (string s in Build)
                hash = hash * 43 + _comparer.GetHashCode(s);
        }
        return hash;
    }

    public override string ToString() => ToString(0);

    public string ToString(int minSegments)
    {
        if (minSegments < 1)
        {
            if (PreRelease.Count > 0)
            {
                if (Build.Count > 0)
                {
                    if (MinorRevision.HasValue && MinorRevision.Value > 0)
                        return $"{Major}.{Minor!.Value}.{Patch!.Value}.{Revision!.Value}.{MinorRevision.Value}-{string.Join('.', PreRelease)}+{string.Join('.', Build)}";
                    if (Revision.HasValue && Revision.Value > 0)
                        return $"{Major}.{Minor!.Value}.{Patch!.Value}.{Revision.Value}-{string.Join('.', PreRelease)}+{string.Join('.', Build)}";
                    if (Patch.HasValue && Patch.Value > 0)
                        return $"{Major}.{Minor!.Value}.{Patch.Value}-{string.Join('.', PreRelease)}+{string.Join('.', Build)}";
                    return $"{Major}.{Minor ?? 0}-{string.Join('.', PreRelease)}+{string.Join('.', Build)}";
                }
                
                if (MinorRevision.HasValue && MinorRevision.Value > 0)
                    return $"{Major}.{Minor!.Value}.{Patch!.Value}.{Revision!.Value}.{MinorRevision.Value}";
                if (Revision.HasValue && Revision.Value > 0)
                    return $"{Major}.{Minor!.Value}.{Patch!.Value}.{Revision.Value}";
                if (Patch.HasValue && Patch.Value > 0)
                    return $"{Major}.{Minor!.Value}.{Patch.Value}";
                return $"{Major}.{Minor ?? 0}-{string.Join('.', PreRelease)}";
            }
            if (Build.Count > 0)
            {
                if (MinorRevision.HasValue && MinorRevision.Value > 0)
                    return $"{Major}.{Minor!.Value}.{Patch!.Value}.{Revision!.Value}.{MinorRevision.Value}";
                if (Revision.HasValue && Revision.Value > 0)
                    return $"{Major}.{Minor!.Value}.{Patch!.Value}.{Revision.Value}";
                if (Patch.HasValue && Patch.Value > 0)
                    return $"{Major}.{Minor!.Value}.{Patch.Value}";
                return $"{Major}.{Minor ?? 0}+{string.Join('.', Build)}";
            }
            if (MinorRevision.HasValue && MinorRevision.Value > 0)
                return $"{Major}.{Minor!.Value}.{Patch!.Value}.{Revision!.Value}.{MinorRevision.Value}";
            if (Revision.HasValue && Revision.Value > 0)
                return $"{Major}.{Minor!.Value}.{Patch!.Value}.{Revision.Value}";
            if (Patch.HasValue && Patch.Value > 0)
                return $"{Major}.{Minor!.Value}.{Patch.Value}";
            return $"{Major}.{Minor ?? 0}";
        }
        if (PreRelease.Count > 0)
        {
            if (Build.Count > 0)
            {
                if (minSegments > 4 || MinorRevision.HasValue)
                    return $"{Major}.{Minor ?? 0}.{Patch ?? 0}.{Revision ?? 0}.{MinorRevision ?? 0}-{string.Join('.', PreRelease)}+{string.Join('.', Build)}";
                if (minSegments == 4 || Revision.HasValue)
                    return $"{Major}.{Minor ?? 0}.{Patch ?? 0}.{Revision ?? 0}-{string.Join('.', PreRelease)}+{string.Join('.', Build)}";
                if (minSegments == 3 || Patch.HasValue)
                    return $"{Major}.{Minor ?? 0}.{Patch ?? 0}-{string.Join('.', PreRelease)}+{string.Join('.', Build)}";
                if (minSegments == 2 || Minor.HasValue)
                    return $"{Major}.{Minor ?? 0}-{string.Join('.', PreRelease)}+{string.Join('.', Build)}";
                return $"{Major}-{string.Join('.', PreRelease)}+{string.Join('.', Build)}";
            }
            if (minSegments > 4 || MinorRevision.HasValue)
                return $"{Major}.{Minor ?? 0}.{Patch ?? 0}.{Revision ?? 0}.{MinorRevision ?? 0}-{string.Join('.', PreRelease)}";
            if (minSegments == 4 || Revision.HasValue)
                return $"{Major}.{Minor ?? 0}.{Patch ?? 0}.{Revision ?? 0}-{string.Join('.', PreRelease)}";
            if (minSegments == 3 || Patch.HasValue)
                return $"{Major}.{Minor ?? 0}.{Patch ?? 0}-{string.Join('.', PreRelease)}";
            if (minSegments == 2 || Minor.HasValue)
                return $"{Major}.{Minor ?? 0}-{string.Join('.', PreRelease)}";
            return $"{Major}-{string.Join('.', PreRelease)}";
        }
        if (Build.Count > 0)
        {
            if (minSegments > 4 || MinorRevision.HasValue)
                return $"{Major}.{Minor ?? 0}.{Patch ?? 0}.{Revision ?? 0}.{MinorRevision ?? 0}+{string.Join('.', Build)}";
            if (minSegments == 4 || Revision.HasValue)
                return $"{Major}.{Minor ?? 0}.{Patch ?? 0}.{Revision ?? 0}+{string.Join('.', Build)}";
            if (minSegments == 3 || Patch.HasValue)
                return $"{Major}.{Minor ?? 0}.{Patch ?? 0}+{string.Join('.', Build)}";
            if (minSegments == 2 || Minor.HasValue)
                return $"{Major}.{Minor ?? 0}+{string.Join('.', Build)}";
            return $"{Major}+{string.Join('.', Build)}";
        }
        if (minSegments > 4 || MinorRevision.HasValue)
            return $"{Major}.{Minor ?? 0}.{Patch ?? 0}.{Revision ?? 0}.{MinorRevision ?? 0}";
        if (minSegments == 4 || Revision.HasValue)
            return $"{Major}.{Minor ?? 0}.{Patch ?? 0}.{Revision ?? 0}";
        if (minSegments == 3 || Patch.HasValue)
            return $"{Major}.{Minor ?? 0}.{Patch ?? 0}";
        if (minSegments == 2 || Minor.HasValue)
            return $"{Major}.{Minor ?? 0}";
        return Major.ToString();
    }
    
    public static bool operator ==(SwVersion left, SwVersion right) => left.Equals(right);

    public static bool operator !=(SwVersion left, SwVersion right) => !left.Equals(right);

    public static bool operator <(SwVersion left, SwVersion right) => left.CompareTo(right) < 0;

    public static bool operator <=(SwVersion left, SwVersion right) => left.CompareTo(right) <= 0;

    public static bool operator >(SwVersion left, SwVersion right) => left.CompareTo(right) > 0;

    public static bool operator >=(SwVersion left, SwVersion right) => left.CompareTo(right) >= 0;
}