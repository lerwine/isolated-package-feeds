using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CdnGetter;

public readonly partial struct SwVersion
{
    /// <summary>
    /// Represents a segment of the <see cref="Build" /> component.
    /// </summary>
    [Obsolete("Use types in CdnGetter.Versioning namespace")]
    public readonly struct BuildSegment : IDelimitedTokenList<IToken>
    {
        /// <summary>
        /// Indicates either the leading character of the <see cref="Build" /> component or the separating character of subsequent segments.
        /// </summary>
        /// <remarks>This value for the first segment of the <see cref="Build" /> component is always <see cref="BuildSeparator.Plus" />.</remarks>
        public BuildSeparator Separator { get; }

        public ReadOnlyCollection<IToken> Tokens { get; }

        /// <summary>
        /// Gets the text of the current <see cref="Build" /> component segment, not including the character indicated by the <see cref="Separator" /> property.
        /// </summary>
        public string Value { get; }

        ISeparatorToken? IDelimitedTokenList<IToken>.Delimiter => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public BuildSegment(BuildSeparator separator, string value)
        {
            Separator = separator;
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

        public IEnumerator<IToken> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public static bool operator <(BuildSegment left, BuildSegment right) => left.CompareTo(right) < 0;

        public static bool operator >(BuildSegment left, BuildSegment right) => left.CompareTo(right) > 0;

        public static bool operator <=(BuildSegment left, BuildSegment right) => left.CompareTo(right) <= 0;

        public static bool operator >=(BuildSegment left, BuildSegment right) => left.CompareTo(right) >= 0;

        public static bool operator ==(BuildSegment left, BuildSegment right) => left.Equals(right);

        public static bool operator !=(BuildSegment left, BuildSegment right) => !(left == right);
    }
}
