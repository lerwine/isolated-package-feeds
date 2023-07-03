using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CdnGetter.Parsing;

/// <summary>
/// Represents a SemVer format version string
/// </summary>
/// <see href="https://semver.org/" />
public readonly struct SemanticVersion : INumericalSoftwareVersion
{
    IToken IReadOnlyList<IToken>.this[int index] => throw new NotImplementedException();

    int IReadOnlyCollection<IToken>.Count => throw new NotImplementedException();

    public IToken? Prefix => throw new NotImplementedException();

    public INumericalToken Major => throw new NotImplementedException();

    IToken ISoftwareVersion.Major => Major;

    public INumericalToken? Minor => throw new NotImplementedException();

    IToken? ISoftwareVersion.Minor => Minor;

    public INumericalToken? Patch => throw new NotImplementedException();

    IToken? ISoftwareVersion.Patch => Patch;

    public INumericalTokenList Micro => throw new NotImplementedException();

    ITokenList ISoftwareVersion.Micro => Micro;

    public ITokenList PreRelease => throw new NotImplementedException();

    public ITokenList Build => throw new NotImplementedException();

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
}
