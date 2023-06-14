using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CdnGetter.Versioning;

public interface ISoftwareVersion : IComparable<ISoftwareVersion>, IEquatable<ISoftwareVersion>
{
    IToken? Prefix { get; }

    INumericalToken Major { get; }

    INumericalToken? Minor { get; }

    INumericalToken? Patch { get; }

    IEnumerable<INumericalToken> Micro { get; }

    IEnumerable<IToken> PreRelease { get; }

    IEnumerable<IToken> Build { get; }
}