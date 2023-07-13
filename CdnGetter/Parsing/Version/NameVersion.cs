using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CdnGetter.Parsing.Parsing;
using static CdnGetter.Parsing.Version.Version;

namespace CdnGetter.Parsing.Version;

/// <summary>
/// Version string to be treated as an alphabetical name.
/// </summary>
#pragma warning disable CA2231
public readonly partial struct NameVersion : ISoftwareVersion
#pragma warning restore CA2231
{
    public static readonly NameVersion Empty = new();

    public IToken this[int index]
    {
        get
        {
            int i;
            if (Prefix is null)
            {
                if (Patch is not null)
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
                else if (Minor is not null)
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
            else if (Patch is not null)
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
            else if (Minor is not null)
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

    public IStringToken? Prefix { get; }

    IToken? ISoftwareVersion.Prefix => Prefix;

    public IStringToken Major { get; }

    IToken ISoftwareVersion.Major => Major;

    public DelimitedToken? Minor { get; }

    IDelimitedToken? ISoftwareVersion.Minor => Minor;

    public DelimitedToken? Patch { get; }

    IDelimitedToken? ISoftwareVersion.Patch => Patch;

    private readonly MicroList _micro;
    public IDelimitedTokenList Micro => _micro;

    public PreReleaseList PreRelease { get; }

    IDelimitedTokenList ISoftwareVersion.PreRelease => PreRelease;

    public BuildList Build { get; }
    
    IDelimitedTokenList ISoftwareVersion.Build => Build;

    int IReadOnlyCollection<IToken>.Count => (Patch.HasValue ? 3 + Micro.Count : Minor.HasValue ? 2 : 1) + PreRelease.Count + Build.Count;

    public NameVersion(IStringToken major)
    {
        Prefix = null;
        Major = major;
        Minor = null;
        Patch = null;
        _micro = new();
        PreRelease = new PreReleaseList();
        Build = new BuildList();
    }

    public NameVersion(IStringToken major, DelimitedToken minor)
    {
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = null;
        _micro = new();
        PreRelease = new PreReleaseList();
        Build = new BuildList();
    }
    
    public NameVersion(IStringToken major, DelimitedToken minor, DelimitedToken patch)
    {
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = patch;
        _micro = new();
        PreRelease = new PreReleaseList();
        Build = new BuildList();
    }
    
    public NameVersion(IStringToken major, DelimitedToken minor, DelimitedToken patch, params DelimitedToken[] micro)
    {
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = patch;
        _micro = new(micro);
        PreRelease = new PreReleaseList();
        Build = new BuildList();
    }
    
    public NameVersion(IStringToken major, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build)
    {
        Prefix = null;
        Major = major;
        Minor = null;
        Patch = null;
        _micro = new();
        PreRelease = new PreReleaseList(preRelease);
        Build = new BuildList(build);
    }
    
    public NameVersion(IStringToken major, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build) : this(major, preRelease, (IEnumerable<IDelimitedToken>)build) { }
    
    public NameVersion(IStringToken major, DelimitedToken minor, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build)
    {
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = null;
        _micro = new();
        PreRelease = new PreReleaseList(preRelease);
        Build = new BuildList(build);
    }
    
    public NameVersion(IStringToken major, DelimitedToken minor, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build) : this(major, minor, preRelease, (IEnumerable<IDelimitedToken>)build) { }
    
    public NameVersion(IStringToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build)
    {
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = patch;
        _micro = new();
        PreRelease = new PreReleaseList(preRelease);
        Build = new BuildList(build);
    }
    
    public NameVersion(IStringToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<DelimitedToken> micro, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build)
    {
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = patch;
        _micro = new(micro);
        PreRelease = new PreReleaseList(preRelease);
        Build = new BuildList(build);
    }
    
    public NameVersion(IStringToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<DelimitedToken> micro, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build)
        : this(major, minor, patch, micro, preRelease, (IEnumerable<IDelimitedToken>)build) { }
    
    private NameVersion(IStringToken prefix, DelimitedToken major, DelimitedToken? minor, DelimitedToken? patch, IEnumerable<DelimitedToken> micro, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build)
    {
        Prefix = prefix;
        Major = major;
        Minor = minor;
        Patch = patch;
        _micro = new(micro);
        PreRelease = new PreReleaseList(preRelease);
        Build = new BuildList(build);
    }

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<DelimitedToken> micro, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build) =>
        new(prefix, major, minor, patch, micro, preRelease, build);

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<DelimitedToken> micro, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build) =>
        new(prefix, major, minor, patch, micro, preRelease, build);

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build) =>
        new(prefix, major, minor, patch, Enumerable.Empty<DelimitedToken>(), preRelease, build);

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build) =>
        new(prefix, major, minor, patch, Enumerable.Empty<DelimitedToken>(), preRelease, build);

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, DelimitedToken minor, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build) =>
        new(prefix, major, minor, null, Enumerable.Empty<DelimitedToken>(), preRelease, build);

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, DelimitedToken minor, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build) =>
        new(prefix, major, minor, null, Enumerable.Empty<DelimitedToken>(), preRelease, build);

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, params DelimitedToken[] micro) =>
        new(prefix, major, minor, patch, micro, Enumerable.Empty<IDelimitedToken>(), Enumerable.Empty<IDelimitedToken>());

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build) =>
        new(prefix, major, null, null, Enumerable.Empty<DelimitedToken>(), preRelease, build);

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build) =>
        new(prefix, major, null, null, Enumerable.Empty<DelimitedToken>(), preRelease, build);

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, DelimitedToken minor) =>
        new(prefix, major, minor, null, Enumerable.Empty<DelimitedToken>(), Enumerable.Empty<IDelimitedToken>(), Enumerable.Empty<IDelimitedToken>());

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major) =>
        new(prefix, major, null, null, Enumerable.Empty<DelimitedToken>(), Enumerable.Empty<IDelimitedToken>(), Enumerable.Empty<IDelimitedToken>());

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
                    foreach (DelimitedToken t in _micro)
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
                foreach (DelimitedToken t in _micro)
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

    public int GetLength(bool allParsedValues = false)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<char> GetSourceValues()
    {
        throw new NotImplementedException();
    }

    public string GetValue()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        throw new NotImplementedException();
    }
}