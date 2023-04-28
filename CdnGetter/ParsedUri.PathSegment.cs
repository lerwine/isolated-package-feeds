using System.Text;

namespace CdnGetter;

public partial class ParsedUri
{
    /// <summary>
    /// Represents a path segment of a URI query.
    /// </summary>
    public class PathSegment : IEquatable<PathSegment>, IComparable<PathSegment>
    {
        /// <summary>
        /// Gets the preceding path segment separator character.
        /// </summary>
        /// <value>The character that separates the current path segment from the previous one or <see langword="null" /> if this is the first path segment and paths for the current URI schema is not rooted.</value>
        public char? Separator { get; }

        /// <summary>
        /// Gets the name of the path segment.
        /// </summary>
        /// <value></value>
        public string Name { get; }
        
        /// <summary>
        /// Createa a new <c>PathSegment</c> with a leading separator character.
        /// </summary>
        /// <param name="separator">The path segment separator character.</param>
        /// <param name="name">The name of the path segment.</param>
        public PathSegment(char separator, string name) => (Separator, Name) = (separator, name ?? "");

        /// <summary>
        /// Createa a new <c>PathSegment</c> without any leading separator character.
        /// </summary>
        /// <param name="name">The name of the path segment.</param>
        public PathSegment(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty if no separator is provided.", nameof(name));
            Name = name;
        }

        /// <summary>
        /// A path segment that only contains a single <c>'/'</c> character.
        /// </summary>
        public static readonly PathSegment EmptyRoot = new(DELIMITER_CHAR_SLASH, string.Empty);
        
        /// <summary>
        /// Tries to parse a path string.
        /// </summary>
        /// <param name="pathString">The source path string.</param>
        /// <param name="options">The Normalization options.</param>
        /// <param name="forceRooted"><see langword="true" /> if the returned path segments must represent a rooted path; otherwise, <see langword="false" />.</param>
        /// <param name="normalizePathSeparators"><see langword="true" /> if all path separators should be normalized to a <c>'/'</c> character;
        /// otherwise, <see langword="false" /> to use path separators as-is.</param>
        /// <returns>The <see cref="PathSegment" /> objects representing teh parsed path segments.</returns>
        public static IEnumerable<PathSegment> Parse(string? pathString, NormalizationOptions options, bool forceRooted = false, bool normalizePathSeparators = false)
        {
            if (string.IsNullOrEmpty(pathString))
                return forceRooted ? ExtensionMethods.Enumerate(new PathSegment(DELIMITER_CHAR_SLASH, string.Empty)) : Enumerable.Empty<PathSegment>();
            throw new NotImplementedException();
        }

        public bool Equals(PathSegment? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (Separator.HasValue)
            {
                if (!(other.Separator.HasValue && Separator.Value.Equals(other.Separator.Value)))
                    return false;
            }
            else if (other.Separator.HasValue)
                return false;
            return DefaultComponentComparer.Equals(Name, other.Name);
        }

        public override bool Equals(object? obj) => Equals(obj as PathSegment);

        public int CompareTo(PathSegment? other)
        {
            if (other is null)
                return 1;
            if (ReferenceEquals(this, other))
                return 0;
            int result;
            if (Separator.HasValue)
            {
                if (!other.Separator.HasValue)
                    return 1;
                if ((result = Separator.Value.CompareTo(other.Separator.Value)) != 0)
                    return result;
            }
            else if (other.Separator.HasValue)
                return -1;
            return DefaultComponentComparer.Compare(Name, other.Name);
        }

        public override int GetHashCode()
        {
            int hash = 3;
            unchecked
            {
                if (Separator.HasValue)
                    hash = (hash * 7) + Separator.Value.GetHashCode();
                return (hash * 7) + DefaultComponentComparer.GetHashCode(Name);
            }
        }

        public override string ToString()
        {
            if (Separator.HasValue)
                return (Name.Length > 0) ? $"{Separator.Value}{Encode(Name, PathSegmentEncodeRegex)}" : Separator.Value.ToString();
            return Encode(Name, PathSegmentEncodeRegex);
        }

        internal void AppendTo(StringBuilder sb)
        {
            if (Separator.HasValue)
            {
                sb.Append(Separator.Value);
                if (Name.Length == 0)
                    return;
            }
            sb.Append(Encode(Name, PathSegmentEncodeRegex)) ;
        }
    }
}
