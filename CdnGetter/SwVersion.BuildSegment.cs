using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CdnGetter;

public readonly partial struct SwVersion
{
    /// <summary>
    /// Represents a segment of the <see cref="Build" /> component.
    /// </summary>
    public readonly struct BuildSegment : IEquatable<BuildSegment>, IComparable<BuildSegment>
    {
        /// <summary>
        /// Indicates either the leading character of the <see cref="Build" /> component or the separating character of subsequent segments.
        /// </summary>
        /// <remarks>This value for the first segment of the <see cref="Build" /> component is always <see cref="BuildSeparator.Plus" />.</remarks>
        public BuildSeparator Separator { get; }

        /// <summary>
        /// The text of the current <see cref="Build" /> component segment, not including the character indicated by the <see cref="Separator" /> property.
        /// </summary>
        public string Value { get; }

        public BuildSegment(BuildSeparator separator, string value)
        {
            Separator = separator;
            if (value is null)
                Value = "";
            else
            {
                if (value.Contains(LEADCHAR_PreRelease) || value.Contains(SEPARATOR_Dot) || value.Contains(LEADCHAR_Build))
                    throw new ArgumentOutOfRangeException(nameof(value));
                Value = value;
            }
        }

        public int CompareTo(BuildSegment other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(BuildSegment other)
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
            if (Value.Length == 0)
                return Separator switch
                {
                    BuildSeparator.Dot => ".",
                    BuildSeparator.Dash => "-",
                    _ => "+"
                };
            return $"{Separator switch { BuildSeparator.Dot => '.', BuildSeparator.Dash => '-', _ => '+' }}{Value}";
        }

        /// <summary>
        /// Converts <c>BuildSegment</c> elements into a build version component string.
        /// </summary>
        /// <param name="segments">The segments of the <see cref="Build" /> version component.</param>
        /// <returns>The build version component string or <see langword="null" /> if <paramref name="segments" /> is <see langword="null" /> or empty.</returns>
        /// <remarks>This will never return <see cref="string.Empty" />; It will always return either a non-empty string or <see langword="null" />.</remarks>
        public static string? ToString(IEnumerable<BuildSegment> segments)
        {
            if (segments is null)
                return null;
            using IEnumerator<BuildSegment> enumerator = segments.GetEnumerator();
            if (!enumerator.MoveNext())
                return null;
            if (!enumerator.MoveNext())
                return enumerator.Current.ToString();
            StringBuilder stringBuilder = new(enumerator.Current.ToString());
            do stringBuilder.Append(enumerator.Current.ToString()); while (enumerator.MoveNext());
            return stringBuilder.ToString();
        }

        public static bool operator <(BuildSegment left, BuildSegment right) => left.CompareTo(right) < 0;

        public static bool operator >(BuildSegment left, BuildSegment right) => left.CompareTo(right) > 0;

        public static bool operator <=(BuildSegment left, BuildSegment right) => left.CompareTo(right) <= 0;

        public static bool operator >=(BuildSegment left, BuildSegment right) => left.CompareTo(right) >= 0;

        public static bool operator ==(BuildSegment left, BuildSegment right) => left.Equals(right);

        public static bool operator !=(BuildSegment left, BuildSegment right) => !(left == right);
    }
}
