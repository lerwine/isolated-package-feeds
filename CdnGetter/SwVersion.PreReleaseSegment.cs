using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CdnGetter;

public readonly partial struct SwVersion
{
    /// <summary>
    /// Represents a segment of the <see cref="PreRelease" /> component.
    /// </summary>
    public readonly struct PreReleaseSegment : IEquatable<PreReleaseSegment>, IComparable<PreReleaseSegment>
    {
        /// <summary>
        /// If <see langword="true" />, then this segment starts with the <c>'-'</c> character;
        /// if this is the first segment of the <see cref="PreRelease" /> component, then this segment immediately follows the last numerical component with no separating character;
        /// otherwise, this segment starts with the <c>'.'</c> character.
        /// </summary>
        public bool AltSeparator { get; }

        /// <summary>
        /// The text of the current <see cref="PreRelease" /> component segment, not including the character indicated by the <see cref="AltSeparator" /> property.
        /// </summary>
        public string Value { get; }

        public PreReleaseSegment(bool altSeprator, string value)
        {
            AltSeparator = altSeprator;
            if (value is null)
                Value = "";
            else
            {
                if (value.Contains(LEADCHAR_PreRelease) || value.Contains(SEPARATOR_Dot) || value.Contains(LEADCHAR_Build))
                    throw new ArgumentOutOfRangeException(nameof(value));
                Value = value;
            }
        }

        public int CompareTo(PreReleaseSegment other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(PreReleaseSegment other)
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

        public override string ToString()
        {
            if (Value.Length == 0) return AltSeparator ? "." : "-";
            return $"{(AltSeparator ? '.' : '-')}{Value}";
        }

        /// <summary>
        /// Converts <c>PreReleaseSegment</c> elements into a pre-release version component string.
        /// </summary>
        /// <param name="segments">The segments of the <see cref="PreRelease" /> version component.</param>
        /// <returns>The pre-release version component string or <see langword="null" /> if <paramref name="segments" /> is <see langword="null" /> or empty.</returns>
        /// <remarks>This will never return <see cref="string.Empty" />; It will always return either a non-empty string or <see langword="null" />.</remarks>
        public static string? ToString(IEnumerable<PreReleaseSegment> segments)
        {
            if (segments is null)
                return null;
            using IEnumerator<PreReleaseSegment> enumerator = segments.GetEnumerator();
            if (!enumerator.MoveNext())
                return null;
            string value = enumerator.Current.AltSeparator ? enumerator.Current.Value : $"-{enumerator.Current.Value}";
            if (!enumerator.MoveNext())
                return value;
            StringBuilder stringBuilder = new(value);
            do stringBuilder.Append(enumerator.Current.ToString()); while (enumerator.MoveNext());
            return stringBuilder.ToString();
        }

        public static bool operator ==(PreReleaseSegment left, PreReleaseSegment right) => left.Equals(right);

        public static bool operator !=(PreReleaseSegment left, PreReleaseSegment right) => !(left == right);

        public static bool operator <(PreReleaseSegment left, PreReleaseSegment right) => left.CompareTo(right) < 0;

        public static bool operator <=(PreReleaseSegment left, PreReleaseSegment right) => left.CompareTo(right) <= 0;

        public static bool operator >(PreReleaseSegment left, PreReleaseSegment right) => left.CompareTo(right) > 0;

        public static bool operator >=(PreReleaseSegment left, PreReleaseSegment right) => left.CompareTo(right) >= 0;
    }
}
