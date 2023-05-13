using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CdnGetter;

public readonly partial struct SwVersion
{
    /// <summary>
    /// Represents a segment of the <see cref="PreRelease" /> component.
    /// </summary>
    public readonly struct PreReleaseSegment : IDelimitedTokenList<IToken>
    {
        /// <summary>
        /// If <see langword="true" />, then this segment starts with the <c>'-'</c> character;
        /// if this is the first segment of the <see cref="PreRelease" /> component, then this segment immediately follows the last numerical component with no separating character;
        /// otherwise, this segment starts with the <c>'.'</c> character.
        /// </summary>
        public bool AltSeparator { get; }

        /// <summary>
        /// Gets the text of the current <see cref="PreRelease" /> component segment, not including the character indicated by the <see cref="AltSeparator" /> property.
        /// </summary>
        [Obsolete("Use Tokens property")]
        public string Value { get; }

        public ISeparatorToken? Delimiter => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public ReadOnlyCollection<IToken> Tokens { get; }

        public PreReleaseSegment(bool altSeparator, string value)
        {
            AltSeparator = altSeparator;
            if (value is null)
            {
                Tokens = new(Array.Empty<IToken>());
                Value = "";
            }
            else
            {
                if (value.Contains(SEPARATOR_DASH) || value.Contains(SEPARATOR_DOT) || value.Contains(SEPARATOR_PLUS))
                    throw new ArgumentOutOfRangeException(nameof(value));
                Tokens = new(Tokenize(value));
                Value = value;
            }
        }

        public bool Equals([NotNullWhen(true)] IDelimitedTokenList<IToken>? other)
        {
            throw new NotImplementedException();
        }

        public bool Equals([NotNullWhen(true)] IDelimitedTokenList<IToken>? other, ISeparatorToken defaultLeadDelimiter)
        {
            throw new NotImplementedException();
        }

        public override bool Equals([NotNullWhen(true)] object? obj) => Equals(obj as IDelimitedTokenList<IToken>);

        public int CompareTo(IDelimitedTokenList<IToken>? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IDelimitedTokenList<IToken>? other, ISeparatorToken defaultDelimiter)
        {
            throw new NotImplementedException();
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

        public IEnumerator<IToken> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(PreReleaseSegment left, PreReleaseSegment right) => left.Equals(right);

        public static bool operator !=(PreReleaseSegment left, PreReleaseSegment right) => !(left == right);

        public static bool operator <(PreReleaseSegment left, PreReleaseSegment right) => left.CompareTo(right) < 0;

        public static bool operator <=(PreReleaseSegment left, PreReleaseSegment right) => left.CompareTo(right) <= 0;

        public static bool operator >(PreReleaseSegment left, PreReleaseSegment right) => left.CompareTo(right) > 0;

        public static bool operator >=(PreReleaseSegment left, PreReleaseSegment right) => left.CompareTo(right) >= 0;
    }
}
