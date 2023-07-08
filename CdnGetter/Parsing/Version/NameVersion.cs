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
public readonly struct NameVersion : ISoftwareVersion
#pragma warning restore CA2231
{
    public static readonly NameVersion Empty = new();

    public IToken this[int index] => throw new NotImplementedException();

    public IStringToken? Prefix { get; }

    IToken? ISoftwareVersion.Prefix => Prefix;

    public IStringToken Major { get; }

    IToken ISoftwareVersion.Major => Major;

    public IDelimitedToken? Minor { get; }

    public IDelimitedToken? Patch { get; }

    public IDelimitedTokenList Micro { get; }

    public IDelimitedTokenList PreRelease { get; }

    public IDelimitedTokenList Build { get; }

    public int Count => throw new NotImplementedException();


    // IStringToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<DelimitedToken> micro, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build
    // IStringToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<DelimitedToken> micro, IEnumerable<IDelimitedToken> preRelease
    // IStringToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<DelimitedToken> micro, IEnumerable<IDelimitedToken> build
    // IStringToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<DelimitedToken> micro

    // IStringToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<IDelimitedToken> build
    // IStringToken major, DelimitedToken minor, IEnumerable<IDelimitedToken> build
    // IStringToken major, IEnumerable<IDelimitedToken> build

    public NameVersion(IStringToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<DelimitedToken> micro, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build)
    {
        if (!major.IsSimpleStringToken())
            throw new ArgumentException($"{nameof(Major)} version must be a {nameof(CharacterToken)} or {nameof(StringToken)}.", nameof(major));
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = patch;
        throw new NotImplementedException();
    }
    
    public NameVersion(IStringToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<DelimitedToken> micro, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build)
        : this(major, minor, patch, micro, preRelease, (IEnumerable<IDelimitedToken>)build) { }
    
    public NameVersion(IStringToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build)
    {
        if (!major.IsSimpleStringToken())
            throw new ArgumentException($"{nameof(Major)} version must be a {nameof(CharacterToken)} or {nameof(StringToken)}.", nameof(major));
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = patch;
        throw new NotImplementedException();
    }
    
    public NameVersion(IStringToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build)
        : this(major, minor, patch, preRelease, (IEnumerable<IDelimitedToken>)build) { }
    
    public NameVersion(IStringToken major, DelimitedToken minor, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build)
    {
        if (!major.IsSimpleStringToken())
            throw new ArgumentException($"{nameof(Major)} version must be a {nameof(CharacterToken)} or {nameof(StringToken)}.", nameof(major));
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = null;
        throw new NotImplementedException();
    }
    
    public NameVersion(IStringToken major, DelimitedToken minor, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build)
        : this(major, minor, preRelease, (IEnumerable<IDelimitedToken>)build) { }

    public NameVersion(IStringToken major, DelimitedToken minor, DelimitedToken patch, params DelimitedToken[] micro)
    {
        if (!major.IsSimpleStringToken())
            throw new ArgumentException($"{nameof(Major)} version must be a {nameof(CharacterToken)} or {nameof(StringToken)}.", nameof(major));
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = patch;
        throw new NotImplementedException();
    }
    
    public NameVersion(IStringToken major, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build)
    {
        if (!major.IsSimpleStringToken())
            throw new ArgumentException($"{nameof(Major)} version must be a {nameof(CharacterToken)} or {nameof(StringToken)}.", nameof(major));
        Prefix = null;
        Major = major;
        Minor = null;
        Patch = null;
        throw new NotImplementedException();
    }
    
    public NameVersion(IStringToken major, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build)
        : this(major, preRelease, (IEnumerable<IDelimitedToken>)build) { }
    
    private NameVersion(IStringToken prefix, DelimitedToken major, DelimitedToken? minor, DelimitedToken? patch, IEnumerable<DelimitedToken>? micro, IEnumerable<IDelimitedToken>? preRelease, IEnumerable<IDelimitedToken>? build)
    {
        Prefix = prefix;
        Major = major;
        Minor = minor;
        Patch = patch;
        throw new NotImplementedException();
    }
    
    // IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<DelimitedToken> micro, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build
    // IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<DelimitedToken> micro, IEnumerable<IDelimitedToken> preRelease
    // IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<DelimitedToken> micro, IEnumerable<IDelimitedToken> build
    // IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<DelimitedToken> micro
    // IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build
    // IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<IDelimitedToken> preRelease
    // IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<IDelimitedToken> build
    // IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch
    // IStringToken prefix, DelimitedToken major, DelimitedToken minor, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build
    // IStringToken prefix, DelimitedToken major, DelimitedToken minor, IEnumerable<IDelimitedToken> preRelease
    // IStringToken prefix, DelimitedToken major, DelimitedToken minor, IEnumerable<IDelimitedToken> build
    // IStringToken prefix, DelimitedToken major, DelimitedToken minor
    // IStringToken prefix, DelimitedToken major, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build
    // IStringToken prefix, DelimitedToken major, IEnumerable<IDelimitedToken> preRelease
    // IStringToken prefix, DelimitedToken major, IEnumerable<IDelimitedToken> build
    // IStringToken prefix, DelimitedToken major

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<DelimitedToken> micro, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build) =>
        new(prefix, major, minor, patch, micro, preRelease, build);

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<DelimitedToken> micro, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build) =>
        new(prefix, major, minor, patch, micro, preRelease, build);

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build) =>
        new(prefix, major, minor, patch, null, preRelease, build);

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build) =>
        new(prefix, major, minor, patch, null, preRelease, build);

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<DelimitedToken> micro, IEnumerable<IDelimitedToken> build) =>
        new(prefix, major, minor, patch, micro, null, build);

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, IEnumerable<DelimitedToken> micro, params IDelimitedToken[] build) =>
        new(prefix, major, minor, patch, micro, null, build);

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, DelimitedToken minor, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build) =>
        new(prefix, major, minor, null, null, preRelease, build);

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, DelimitedToken minor, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build) =>
        new(prefix, major, minor, null, null, preRelease, build);

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, DelimitedToken minor, DelimitedToken patch, params DelimitedToken[] micro) =>
        new(prefix, major, minor, patch, micro, null, null);

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, IEnumerable<IDelimitedToken> preRelease, IEnumerable<IDelimitedToken> build) =>
        new(prefix, major, null, null, null, preRelease, build);

    public static NameVersion WithPrefix(IStringToken prefix, DelimitedToken major, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build) =>
        new(prefix, major, null, null, null, preRelease, build);

    public int CompareTo(IToken? other)
    {
        throw new NotImplementedException();
    }

    public bool Equals(IToken? other)
    {
        throw new NotImplementedException();
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return base.Equals(obj);
    }

    public IEnumerator<IToken> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
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