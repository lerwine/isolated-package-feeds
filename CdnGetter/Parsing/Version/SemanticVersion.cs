using System.Collections;
using static CdnGetter.Parsing.Parsing;

namespace CdnGetter.Parsing.Version;

/// <summary>
/// Represents a SemVer format version string
/// </summary>
/// <see href="https://semver.org/" />
#pragma warning disable CA2231
public readonly partial struct SemanticVersion : INumericalSoftwareVersion
#pragma warning restore CA2231
{
    public static readonly SemanticVersion Empty = new();

    IToken IReadOnlyList<IToken>.this[int index]
    {
        get
        {
            int i;
            if (Prefix is null)
            {
                if (Patch.HasValue)
                    switch (index)
                    {
                        case 0:
                            return Major;
                        case 1:
                            return Minor!;
                        case 2:
                            return Patch!;
                        default:
                            i = index - 3;
                            break;
                    }
                else if (Minor.HasValue)
                    switch (index)
                    {
                        case 0:
                            return Major;
                        case 1:
                            return Minor!;
                        default:
                            i = index - 2;
                            break;
                    }
                else if (index == 0)
                    return Major;
                else
                    i = index - 2;
            }
            else if (Patch.HasValue)
                switch (index)
                {
                    case 0:
                        return Patch!;
                    case 1:
                        return Major;
                    case 2:
                        return Minor!;
                    case 3:
                        return Patch!;
                    default:
                        i = index - 4;
                        break;
                }
            else if (Minor.HasValue)
                switch (index)
                {
                    case 0:
                        return Patch!;
                    case 1:
                        return Major;
                    case 2:
                        return Minor!;
                    default:
                        i = index - 3;
                        break;
                }
            else
                switch (index)
                {
                    case 0:
                        return Patch!;
                    case 1:
                        return Major;
                    default:
                        i = index - 2;
                        break;
                }
            int c = PreRelease.Count;
            return (i < c) ? PreRelease[i] : Build[i - c];
        }
    }

    int IReadOnlyCollection<IToken>.Count => (Patch.HasValue ? 3 + Micro.Count : Minor.HasValue ? 2 : 1) + PreRelease.Count + Build.Count;

    public readonly IStringToken? Prefix { get; }

    IToken? ISoftwareVersion.Prefix => Prefix;

    public readonly INumericalToken Major { get; }

    IToken ISoftwareVersion.Major => Major;

    public readonly DelimitedNumericalToken? Minor { get; }

    IDelimitedToken? ISoftwareVersion.Minor => Minor;

    public readonly DelimitedNumericalToken? Patch { get; }

    IDelimitedToken? ISoftwareVersion.Patch => Patch;

    public readonly NumericalTokenList Micro { get; }

    IDelimitedTokenList ISoftwareVersion.Micro => Micro;

    public readonly PreReleaseList PreRelease { get; }

    IDelimitedTokenList ISoftwareVersion.PreRelease => PreRelease;

    public readonly BuildList Build { get; }

    IDelimitedTokenList ISoftwareVersion.Build => Build;

    public SemanticVersion(int major)
    {
        Prefix = null;
        Major = ToNumericalToken(major);
        Minor = null;
        Patch = null;
        Micro = new();
        PreRelease = new();
        Build = new();
    }
    
    public SemanticVersion(int major, int minor)
    {
        Prefix = null;
        Major = ToNumericalToken(major);
        Minor = ToDelimitedNumericalToken(minor);
        Patch = null;
        Micro = new();
        PreRelease = new();
        Build = new();
    }
    
    public SemanticVersion(int major, int minor, int patch, params int[] micro)
    {
        Prefix = null;
        Major = ToNumericalToken(major);
        Minor = ToDelimitedNumericalToken(minor);
        Patch = ToDelimitedNumericalToken(patch);
        Micro = new();
        PreRelease = new();
        Build = new();
    }
    
    public SemanticVersion(INumericalToken major)
    {
        if (!major.IsNamedOrSimpleNumericToken())
            throw new ArgumentException($"{nameof(Major)} must be a simple or named numeric token.", nameof(major));
        Prefix = null;
        Major = major;
        Minor = null;
        Patch = null;
        Micro = new();
        PreRelease = new();
        Build = new();
    }

    public SemanticVersion(INumericalToken major, DelimitedNumericalToken minor)
    {
        if (!major.IsNamedOrSimpleNumericToken())
            throw new ArgumentException($"{nameof(Major)} must be a simple or named numeric token.", nameof(major));
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = null;
        Micro = new();
        PreRelease = new();
        Build = new();
    }

    public SemanticVersion(INumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, params DelimitedNumericalToken[] micro)
    {
        if (!major.IsNamedOrSimpleNumericToken())
            throw new ArgumentException($"{nameof(Major)} must be a simple or named numeric token.", nameof(major));
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = patch;
        Micro = new(micro);
        PreRelease = new();
        Build = new();
    }

    public SemanticVersion(INumericalToken major, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build)
    {
        if (!major.IsNamedOrSimpleNumericToken())
            throw new ArgumentException($"{nameof(Major)} must be a simple or named numeric token.", nameof(major));
        Prefix = null;
        Major = major;
        Minor = null;
        Patch = null;
        Micro = new();
        PreRelease = new(preRelease);
        Build = new(build);
    }

    public SemanticVersion(INumericalToken major, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build)
        : this(major, preRelease, (IEnumerable<IDelimitedToken>)build) { }

    public SemanticVersion(INumericalToken major, DelimitedNumericalToken minor, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build)
    {
        if (!major.IsNamedOrSimpleNumericToken())
            throw new ArgumentException($"{nameof(Major)} must be a simple or named numeric token.", nameof(major));
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = null;
        Micro = new();
        PreRelease = new(preRelease);
        Build = new(build);
    }

    public SemanticVersion(INumericalToken major, DelimitedNumericalToken minor, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build)
        : this(major, minor, preRelease, (IEnumerable<IDelimitedToken>)build) { }

    public SemanticVersion(INumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build)
    {
        if (!major.IsNamedOrSimpleNumericToken())
            throw new ArgumentException($"{nameof(Major)} must be a simple or named numeric token.", nameof(major));
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = patch;
        Micro = new();
        PreRelease = new(preRelease);
        Build = new(build);
    }

    public SemanticVersion(INumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build)
        : this(major, minor, patch, preRelease, (IEnumerable<IDelimitedToken>)build) { }

    public SemanticVersion(INumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, IEnumerable<DelimitedNumericalToken> micro, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build)
    {
        if (!major.IsNamedOrSimpleNumericToken())
            throw new ArgumentException($"{nameof(Major)} must be a simple or named numeric token.", nameof(major));
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = patch;
        Micro = new(micro);
        PreRelease = new(preRelease);
        Build = new(build);
    }

    public SemanticVersion(INumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, IEnumerable<DelimitedNumericalToken> micro, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build)
        : this(major, minor, patch, micro, preRelease, (IEnumerable<IDelimitedToken>)build) { }

    private SemanticVersion(IStringToken prefix, DelimitedNumericalToken major, DelimitedNumericalToken? minor, DelimitedNumericalToken? patch, IEnumerable<DelimitedNumericalToken> micro, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build)
    {
        if (!prefix.IsSimpleStringToken())
            throw new ArgumentException($"{nameof(Prefix)} must be a simple string token.", nameof(prefix));
        Prefix = prefix;
        Major = major;
        Minor = minor;
        Patch = patch;
        Micro = new(micro);
        PreRelease = new(preRelease);
        Build = new(build);
    }

    public static SemanticVersion WithPrefix(IStringToken prefix, DelimitedNumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, IEnumerable<DelimitedNumericalToken> micro, IEnumerable<IDelimitedToken> preRelease,
        IEnumerable<IDelimitedToken> build) => new(prefix, major, minor, patch, micro, preRelease, build);

    public static SemanticVersion WithPrefix(IStringToken prefix, DelimitedNumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, IEnumerable<DelimitedNumericalToken> micro, IEnumerable<IDelimitedToken> preRelease,
        params IDelimitedToken[] build) => new(prefix, major, minor, patch, micro, preRelease, build);

    public static SemanticVersion WithPrefix(IStringToken prefix, DelimitedNumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build) =>
        new(prefix, major, minor, patch, Enumerable.Empty<DelimitedNumericalToken>(), preRelease, build);

    public static SemanticVersion WithPrefix(IStringToken prefix, DelimitedNumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build) =>
        new(prefix, major, minor, patch, Enumerable.Empty<DelimitedNumericalToken>(), preRelease, build);

    public static SemanticVersion WithPrefix(IStringToken prefix, DelimitedNumericalToken major, DelimitedNumericalToken minor, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build) =>
        new(prefix, major, minor, null, Enumerable.Empty<DelimitedNumericalToken>(), preRelease, build);

    public static SemanticVersion WithPrefix(IStringToken prefix, DelimitedNumericalToken major, DelimitedNumericalToken minor, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build) =>
        new(prefix, major, minor, null, Enumerable.Empty<DelimitedNumericalToken>(), preRelease, build);

    public static SemanticVersion WithPrefix(IStringToken prefix, DelimitedNumericalToken major, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build) =>
        new(prefix, major, null, null, Enumerable.Empty<DelimitedNumericalToken>(), preRelease, build);

    public static SemanticVersion WithPrefix(IStringToken prefix, DelimitedNumericalToken major, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build) =>
        new(prefix, major, null, null, Enumerable.Empty<DelimitedNumericalToken>(), preRelease, build);

    public static SemanticVersion WithPrefix(IStringToken prefix, DelimitedNumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, params DelimitedNumericalToken[] micro) =>
        new(prefix, major, minor, patch, micro, Enumerable.Empty<IDelimitedToken>(), Enumerable.Empty<IDelimitedToken>());

    public static SemanticVersion WithPrefix(IStringToken prefix, DelimitedNumericalToken major, DelimitedNumericalToken minor) =>
        new(prefix, major, minor, null, Enumerable.Empty<DelimitedNumericalToken>(), Enumerable.Empty<IDelimitedToken>(), Enumerable.Empty<IDelimitedToken>());

    public static SemanticVersion WithPrefix(IStringToken prefix, DelimitedNumericalToken major) =>
        new(prefix, major, null, null, Enumerable.Empty<DelimitedNumericalToken>(), Enumerable.Empty<IDelimitedToken>(), Enumerable.Empty<IDelimitedToken>());

    public int CompareTo(IToken? other)
    {
        throw new NotImplementedException();
    }

    public bool Equals(IToken? other)
    {
        throw new NotImplementedException();
    }

    public override bool Equals(object? obj) => Equals(obj as IToken);

    public IEnumerator<IToken> GetEnumerator() => ((Prefix is null) ? Patch.HasValue ? new IToken[] { Major, Minor!.Value, Patch.Value }.Concat(Micro) : Minor.HasValue ? new IToken[] { Major, Minor.Value } : new IToken[] { Major } :
            Patch.HasValue ? new IToken[] { Major, Minor!.Value, Patch.Value } : Minor.HasValue ? new IToken[] { Major, Minor.Value } : new IToken[] { Major }).Concat(PreRelease).Concat(Build).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)((Prefix is null) ? Patch.HasValue ? new IToken[] { Major, Minor!.Value, Patch.Value }.Concat(Micro) : Minor.HasValue ? new IToken[] { Major, Minor.Value } : new IToken[] { Major } :
            Patch.HasValue ? new IToken[] { Major, Minor!.Value, Patch.Value } : Minor.HasValue ? new IToken[] { Major, Minor.Value } : new IToken[] { Major }).Concat(PreRelease).Concat(Build)).GetEnumerator();

    public override int GetHashCode()
    {
        unchecked
        {
            int hash;
            if (Prefix is null)
            {
                if (Patch.HasValue)
                {
                    hash = (((24 + Micro.Count + PreRelease.Count + Build.Count) * 7 + Major.GetHashCode()) * 7 + Minor!.Value.GetHashCode()) * 7 + Patch.Value.GetHashCode();
                    foreach (DelimitedNumericalToken t in Micro)
                        hash = hash * 7 + t.GetHashCode();
                }
                else if (Minor.HasValue)
                    hash = ((23 + PreRelease.Count + Build.Count) * 7 + Major.GetHashCode()) * 7 + Minor.Value.GetHashCode();
                else
                    hash = (22 + PreRelease.Count + Build.Count) * 7 + Major.GetHashCode();
            }
            else if (Patch.HasValue)
            {
                hash = ((((25 + Micro.Count + PreRelease.Count + Build.Count) * 7 + Prefix.GetHashCode()) * 7 + Major.GetHashCode()) * 7 + Minor!.Value.GetHashCode()) * 7 + Patch.Value.GetHashCode();
                foreach (DelimitedNumericalToken t in Micro)
                    hash = hash * 7 + t.GetHashCode();
            }
            else if (Minor.HasValue)
                hash = (((24 + PreRelease.Count + Build.Count) * 7 + Prefix.GetHashCode()) * 7 + Major.GetHashCode()) * 7 + Minor.Value.GetHashCode();
            else
                hash = ((23 + PreRelease.Count + Build.Count) * 7 + Prefix.GetHashCode()) * 7 + Major.GetHashCode();
            foreach (IDelimitedToken t in PreRelease)
                hash = hash * 7 + t.GetHashCode();
            foreach (IDelimitedToken t in Build)
                hash = hash * 7 + t.GetHashCode();
            return hash;
        }
    }

    int IToken.GetLength(bool allChars)
    {
        throw new NotImplementedException();
    }

    string IToken.GetValue()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<char> GetSourceValues()
    {
        throw new NotImplementedException();
    }
}
