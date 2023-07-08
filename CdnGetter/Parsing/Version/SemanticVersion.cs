using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CdnGetter.Parsing.Parsing;
using static CdnGetter.Parsing.Version.Version;

namespace CdnGetter.Parsing.Version;

/// <summary>
/// Represents a SemVer format version string
/// </summary>
/// <see href="https://semver.org/" />
public readonly struct SemanticVersion : INumericalSoftwareVersion
{
    public static readonly SemanticVersion Empty = new();

    IToken IReadOnlyList<IToken>.this[int index] => throw new NotImplementedException();

    int IReadOnlyCollection<IToken>.Count => (Patch.HasValue ? 3 + Micro.Count : Minor.HasValue ? 2 : 1) + PreRelease.Count + Build.Count;

    public readonly IToken? Prefix { get; }

    public readonly INumericalToken Major { get; }

    IToken ISoftwareVersion.Major => Major;

    public readonly DelimitedNumericalToken? Minor { get; }

    IDelimitedToken? ISoftwareVersion.Minor => Minor;

    public readonly DelimitedNumericalToken? Patch { get; }

    IDelimitedToken? ISoftwareVersion.Patch => Patch;

    public readonly NumericalTokenList Micro { get; }

    IDelimitedTokenList ISoftwareVersion.Micro => Micro;

    public readonly IDelimitedTokenList PreRelease { get; }

    public readonly IDelimitedTokenList Build { get; }

    public SemanticVersion(IToken prefix, DelimitedNumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, IEnumerable<DelimitedNumericalToken> micro, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build)
    {
        Prefix = prefix;
        Major = major;
        Minor = minor;
        Patch = patch;
        Micro = new(micro);
        throw new NotImplementedException();
    }
    
    public SemanticVersion(INumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, IEnumerable<DelimitedNumericalToken> micro, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build)
    {
        if (!major.IsNamedOrSimpleNumericToken())
            throw new ArgumentException($"{nameof(Major)} version must be a {nameof(Digits8Bit)}, {nameof(Digits16Bit)}, {nameof(Digits32Bit)}, {nameof(Digits64Bit)}, {nameof(Digits8Bit)}, {nameof(DigitsNBit)} or {nameof(NamedNumericalToken)}.",
                nameof(major));
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = patch;
        Micro = new(micro);
        throw new NotImplementedException();
    }
    
    public SemanticVersion(IToken prefix, DelimitedNumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, IEnumerable<DelimitedNumericalToken> micro, IEnumerable<IDelimitedToken>? build = null)
    {
        Prefix = prefix;
        Major = major;
        Minor = minor;
        Patch = patch;
        Micro = new(micro);
        throw new NotImplementedException();
    }
    
    public SemanticVersion(IToken prefix, DelimitedNumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build)
    {
        Prefix = prefix;
        Major = major;
        Minor = minor;
        Patch = patch;
        Micro = new();
        throw new NotImplementedException();
    }

    public SemanticVersion(INumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, IEnumerable<DelimitedNumericalToken> micro, IEnumerable<IDelimitedToken>? build = null)
    {
        if (!major.IsNamedOrSimpleNumericToken())
            throw new ArgumentException($"{nameof(Major)} version must be a {nameof(Digits8Bit)}, {nameof(Digits16Bit)}, {nameof(Digits32Bit)}, {nameof(Digits64Bit)}, {nameof(Digits8Bit)}, {nameof(DigitsNBit)} or {nameof(NamedNumericalToken)}.",
                nameof(major));
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = patch;
        Micro = new(micro);
        throw new NotImplementedException();
    }
    
    public SemanticVersion(INumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build)
    {
        if (!major.IsNamedOrSimpleNumericToken())
            throw new ArgumentException($"{nameof(Major)} version must be a {nameof(Digits8Bit)}, {nameof(Digits16Bit)}, {nameof(Digits32Bit)}, {nameof(Digits64Bit)}, {nameof(Digits8Bit)}, {nameof(DigitsNBit)} or {nameof(NamedNumericalToken)}.",
                nameof(major));
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = patch;
        Micro = new();
        throw new NotImplementedException();
    }
    
    public SemanticVersion(IToken prefix, DelimitedNumericalToken major, DelimitedNumericalToken minor, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build)
    {
        Prefix = prefix;
        Major = major;
        Minor = minor;
        Patch = null;
        Micro = new();
        throw new NotImplementedException();
    }
    
    public SemanticVersion(IToken prefix, DelimitedNumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, params DelimitedNumericalToken[] micro)
    {
        Prefix = prefix;
        Major = major;
        Minor = minor;
        Patch = patch;
        Micro = new(micro);
        throw new NotImplementedException();
    }
    
    public SemanticVersion(IToken prefix, DelimitedNumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, IEnumerable<IDelimitedToken> build)
    {
        Prefix = prefix;
        Major = major;
        Minor = minor;
        Patch = patch;
        Micro = new();
        throw new NotImplementedException();
    }
    
    public SemanticVersion(INumericalToken major, DelimitedNumericalToken minor, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build)
    {
        if (!major.IsNamedOrSimpleNumericToken())
            throw new ArgumentException($"{nameof(Major)} version must be a {nameof(Digits8Bit)}, {nameof(Digits16Bit)}, {nameof(Digits32Bit)}, {nameof(Digits64Bit)}, {nameof(Digits8Bit)}, {nameof(DigitsNBit)} or {nameof(NamedNumericalToken)}.",
                nameof(major));
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = null;
        Micro = new();
        throw new NotImplementedException();
    }
    
    public SemanticVersion(INumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, params DelimitedNumericalToken[] micro)
    {
        if (!major.IsNamedOrSimpleNumericToken())
            throw new ArgumentException($"{nameof(Major)} version must be a {nameof(Digits8Bit)}, {nameof(Digits16Bit)}, {nameof(Digits32Bit)}, {nameof(Digits64Bit)}, {nameof(Digits8Bit)}, {nameof(DigitsNBit)} or {nameof(NamedNumericalToken)}.",
                nameof(major));
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = patch;
        Micro = new(micro);
        throw new NotImplementedException();
    }
    
    public SemanticVersion(INumericalToken major, DelimitedNumericalToken minor, DelimitedNumericalToken patch, IEnumerable<IDelimitedToken> build)
    {
        if (!major.IsNamedOrSimpleNumericToken())
            throw new ArgumentException($"{nameof(Major)} version must be a {nameof(Digits8Bit)}, {nameof(Digits16Bit)}, {nameof(Digits32Bit)}, {nameof(Digits64Bit)}, {nameof(Digits8Bit)}, {nameof(DigitsNBit)} or {nameof(NamedNumericalToken)}.",
                nameof(major));
        Prefix = null;
        Major = major;
        Minor = minor;
        Patch = patch;
        Micro = new();
        throw new NotImplementedException();
    }
    
    public SemanticVersion(IToken prefix, DelimitedNumericalToken major, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build)
    {
        Prefix = prefix;
        Major = major;
        Patch = null;
        Micro = new();
        throw new NotImplementedException();
    }
    
    public SemanticVersion(IToken prefix, DelimitedNumericalToken major, DelimitedNumericalToken minor, IEnumerable<IDelimitedToken>? build = null)
    {
        Prefix = prefix;
        Major = major;
        Minor = minor;
        Patch = null;
        Micro = new();
        throw new NotImplementedException();
    }
    
    public SemanticVersion(INumericalToken major, IEnumerable<IDelimitedToken> preRelease, params IDelimitedToken[] build)
    {
        if (!major.IsNamedOrSimpleNumericToken())
            throw new ArgumentException($"{nameof(Major)} version must be a {nameof(Digits8Bit)}, {nameof(Digits16Bit)}, {nameof(Digits32Bit)}, {nameof(Digits64Bit)}, {nameof(Digits8Bit)}, {nameof(DigitsNBit)} or {nameof(NamedNumericalToken)}.",
                nameof(major));
        Major = major;
        Minor = Patch = null;
        Micro = new();
        throw new NotImplementedException();
    }
    
    public SemanticVersion(INumericalToken major, DelimitedNumericalToken minor, IEnumerable<IDelimitedToken>? build = null)
    {
        if (!major.IsNamedOrSimpleNumericToken())
            throw new ArgumentException($"{nameof(Major)} version must be a {nameof(Digits8Bit)}, {nameof(Digits16Bit)}, {nameof(Digits32Bit)}, {nameof(Digits64Bit)}, {nameof(Digits8Bit)}, {nameof(DigitsNBit)} or {nameof(NamedNumericalToken)}.",
                nameof(major));
        Prefix = null;
        Major = major;
        Minor = minor;
        Micro = new();
        throw new NotImplementedException();
    }
    
    public SemanticVersion(IToken prefix, DelimitedNumericalToken major, IEnumerable<IDelimitedToken>? build = null)
    {
        Prefix = prefix;
        Major = major;
        Minor = Patch = null;
        Micro = new();
        throw new NotImplementedException();
    }
    
    public SemanticVersion(INumericalToken major, IEnumerable<IDelimitedToken>? build = null)
    {
        if (!major.IsNamedOrSimpleNumericToken())
            throw new ArgumentException($"{nameof(Major)} version must be a {nameof(Digits8Bit)}, {nameof(Digits16Bit)}, {nameof(Digits32Bit)}, {nameof(Digits64Bit)}, {nameof(Digits8Bit)}, {nameof(DigitsNBit)} or {nameof(NamedNumericalToken)}.",
                nameof(major));
        Prefix = null;
        Major = major;
        Minor = Patch = null;
        Micro = new();
        throw new NotImplementedException();
    }
    
    public int CompareTo(IToken? other)
    {
        throw new NotImplementedException();
    }

    public bool Equals(IToken? other)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<IToken> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
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
