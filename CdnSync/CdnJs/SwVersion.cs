using System.Collections.ObjectModel;

namespace CdnSync.CdnJs
{
    public struct SwVersion : IEquatable<SwVersion>, IComparable<SwVersion>
    {
        /*
        Comparison of development stage indicators
Stage	Semver	Num. Status	Num 90+
Alpha	1.2.0-a.1	1.2.0.1	1.1.90
Beta	1.2.0-b.2	1.2.1.2	1.1.93
Release candidate	1.2.0-rc.3	1.2.2.3	1.1.97
Release	1.2.0	1.2.3.0	1.2.0
Post-release fixes	1.2.5	1.2.3.5	1.2.5


<valid semver> ::= <version core>
                 | <version core> "-" <pre-release>
                 | <version core> "+" <build>
                 | <version core> "-" <pre-release> "+" <build>

<version core> ::= <major> "." <minor> "." <patch>

<major> ::= <numeric identifier>

<minor> ::= <numeric identifier>

<patch> ::= <numeric identifier>

<pre-release> ::= <dot-separated pre-release identifiers>

<dot-separated pre-release identifiers> ::= <pre-release identifier>
                                          | <pre-release identifier> "." <dot-separated pre-release identifiers>

<build> ::= <dot-separated build identifiers>

<dot-separated build identifiers> ::= <build identifier>
                                    | <build identifier> "." <dot-separated build identifiers>

<pre-release identifier> ::= <alphanumeric identifier>
                           | <numeric identifier>

<build identifier> ::= <alphanumeric identifier>
                     | <digits>

<alphanumeric identifier> ::= <non-digit>
                            | <non-digit> <identifier characters>
                            | <identifier characters> <non-digit>
                            | <identifier characters> <non-digit> <identifier characters>

<numeric identifier> ::= "0"
                       | <positive digit>
                       | <positive digit> <digits>

<identifier characters> ::= <identifier character>
                          | <identifier character> <identifier characters>

<identifier character> ::= <digit>
                         | <non-digit>

<non-digit> ::= <letter>
              | "-"

<digits> ::= <digit>
           | <digit> <digits>

<digit> ::= "0"
          | <positive digit>

<positive digit> ::= "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"

<letter> ::= "A" | "B" | "C" | "D" | "E" | "F" | "G" | "H" | "I" | "J"
           | "K" | "L" | "M" | "N" | "O" | "P" | "Q" | "R" | "S" | "T"
           | "U" | "V" | "W" | "X" | "Y" | "Z" | "a" | "b" | "c" | "d"
           | "e" | "f" | "g" | "h" | "i" | "j" | "k" | "l" | "m" | "n"
           | "o" | "p" | "q" | "r" | "s" | "t" | "u" | "v" | "w" | "x"
           | "y" | "z"
        */
        private static readonly StringComparer _comparer = StringComparer.OrdinalIgnoreCase;

        private readonly ReadOnlyCollection<Segment>? _allSegments;

        public SwVersion(Version? version)
        {
            if (version is null || version.Major == 0 && version.Minor == 0 && version.Build < 0 && version.Revision < 0)
                _allSegments = null;
            else
            {
                Collection<Segment> segments = new();
                segments.Add(new(version.Major));
                segments.Add(new(version.Minor));
                if (version.Build > -1)
                {
                    segments.Add(new(version.Build));
                    if (version.Revision > -1)
                        segments.Add(new(version.Revision));
                }
                _allSegments = new(segments);
            }
        }
    
        public SwVersion(string? versionString)
        {
            if ((versionString = versionString.ToTrimmedOrNullIfEmpty()) is not null)
            {
                Segment[] segments = versionString.Split('.').Select(v => v.ToWsNormalizedOrEmptyIfNull()).Where(v => v.Length > 0).Select(s => new Segment(s)).ToArray();
                if (segments.Length == 1)
                {
                    _allSegments = new(segments.Concat(new Segment[] { new(0) }).ToArray());
                    return;
                }
                if (segments.Length > 1)
                {
                    _allSegments = new(segments);
                    return;
                }
            }
            _allSegments = null;
        }
    
        public bool Equals(SwVersion other)
        {
            if (_allSegments is null)
                return other._allSegments is null;
            return other._allSegments is not null && _allSegments.Count == other._allSegments.Count &&  _allSegments.Zip(other._allSegments).All(z => z.First.Equals(z.Second));
        }

        public override bool Equals(object? obj) => obj is not null && obj is SwVersion other && Equals(other);

        public int CompareTo(SwVersion other)
        {
            if (_allSegments is null)
                return (other._allSegments is null) ? 0 : -1;
            if (other._allSegments is null)
                return 1;
            using IEnumerator<Segment> e1 = _allSegments.GetEnumerator();
            using IEnumerator<Segment> e2 = other._allSegments.GetEnumerator();
            e1.MoveNext();
            e2.MoveNext();
            int result;
            while ((result = e1.Current.CompareTo(e2.Current)) == 0)
            {
                if (e1.MoveNext())
                {
                    if (!e2.MoveNext())
                        return 1;
                }
                else
                    return e2.MoveNext() ? -1 : 0;
            }
            return result;
        }

        public override int GetHashCode()
        {
            if (_allSegments is null)
                return 0;
            int hash = 11;
            unchecked
            {
                foreach (Segment s in _allSegments)
                    hash = hash * 17 + s.GetHashCode();
            }
            return hash;
        }

        public override string ToString() => (_allSegments is null) ? "" : string.Join('.', _allSegments.Select(s => s.ToString()));

        public static bool operator ==(SwVersion left, SwVersion right) => left.Equals(right);

        public static bool operator !=(SwVersion left, SwVersion right) => !left.Equals(right);

        public static bool operator <(SwVersion left, SwVersion right) => left.CompareTo(right) < 0;

        public static bool operator <=(SwVersion left, SwVersion right) => left.CompareTo(right) <= 0;

        public static bool operator >(SwVersion left, SwVersion right) => left.CompareTo(right) > 0;

        public static bool operator >=(SwVersion left, SwVersion right) => left.CompareTo(right) >= 0;

        public static implicit operator SwVersion(Version? v) => new(v);

        public static implicit operator SwVersion(string? s) => new(s);

        public static implicit operator string(SwVersion s) => s.ToString();

        struct Segment : IEquatable<Segment>, IComparable<Segment>
        {
            private readonly int? _value;
            private readonly string _text;

            internal Segment(int value)
            {
                _value = value;
                _text = value.ToString();
            }

            internal Segment(string text)
            {
                if (text.All(char.IsNumber) && int.TryParse(text, out int value))
                    _text = (_value = value).ToString()!;
                else
                {
                    _value = null;
                    _text = text;
                }
            }

            public bool Equals(Segment other) => _value.HasValue ? (other._value.HasValue && _value.Value == other._value) : !other._value.HasValue && _comparer.Equals(_text, other._text);

            public override bool Equals(object? obj) => obj is not null && obj is Segment other && Equals(other);

            public int CompareTo(Segment other) => _value.HasValue ? (other._value.HasValue ? _value.Value - other._value.Value : -1) : other._value.HasValue ? 1 : _comparer.Compare(_text, other._text);

            public override int GetHashCode() => _value ?? _text.GetHashCode();

            public override string ToString() => _text;
        }
    }
}