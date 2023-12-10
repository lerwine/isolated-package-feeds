using System.Diagnostics.CodeAnalysis;

namespace IsolatedPackageFeeds.Shared;

/// <summary>
/// Represents an optional value.
/// </summary>
/// <typeparam name="ValueType">The value type</typeparam>
/// <remarks>For this class, a <see langword="null" /> value could be considered a ligitimate value if the <typeparamref name="ValueType" /> supports it.</remarks>
public readonly struct OptionalValue<ValueType> : IEquatable<OptionalValue<ValueType>>, IComparable<OptionalValue<ValueType>>
{
    private static readonly EqualityComparer<ValueType> _defaultEquality = EqualityComparer<ValueType>.Default;
    private static readonly Comparer<ValueType> _defaultComparer = Comparer<ValueType>.Default;
    private static readonly Func<ValueType, int> _getHashCode;
    private static readonly Func<ValueType, string> _toString;

    static OptionalValue()
    {
        Type type = typeof(ValueType);
        if (type.IsValueType && !(type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))))
        {
            _getHashCode = _defaultEquality.GetHashCode!;
            _toString = v => v!.ToString() ?? "";
        }
        else
        {
            _getHashCode = v => (v is null) ? 0 : _defaultEquality.GetHashCode(v);
            _toString = v => v?.ToString() ?? "";
        }
    }

    /// <summary>
    /// Gets the target value
    /// </summary>
    /// <value>The value, which may be the <see langword="default" /> for the <typeparamref name="ValueType" /> if <see cref="HasValue" /> is <see langword="false" />.</value>
    public ValueType Value { get; }

    /// <summary>
    /// Gets a value that indicates whether the <see cref="Value" /> property represents an actual value.
    /// </summary>
    /// <value><see langword="true" /> if  contains an actual value; otherwise, <see langword="false" /> if this object represents no value.</value>
    public bool HasValue { get; }

    private OptionalValue(ValueType value, bool hasValue) => (Value, HasValue) = (value, hasValue);

    /// <summary>
    /// Creates a new <c>OptionalValue</c> that contains an actual value (<see cref="HasValue" /> will be <see langword="true" />).
    /// </summary>
    /// <param name="value">The target value, which may be <see langword="null" /> if the <typeparamref name="ValueType" /> supports it.</param>
    public OptionalValue(ValueType value) => (Value, HasValue) = (value, true);

    /// <summary>
    /// Attempts to get the target value.
    /// </summary>
    /// <param name="result">The target <see cref="Value" /> if this returns <see langword="true" />;
    /// otherwise, <see langword="null" /> or the <see langword="default" /> value, depending upon the <typeparamref name="ValueType" />, if this returns <see langword="false" />.</param>
    /// <returns><see langword="true" /> if the <paramref name="result" /> contains an actual value; otherwise, <see langword="false" /> indicates this object represented no value.</returns>
    public bool TryGetValue([MaybeNullWhen(false)] out ValueType result)
    {
        result = Value;
        return HasValue;
    }

    /// <summary>
    /// Represents an <see cref="OptionalValue{ValueType}" /> that represents no actual value.
    /// </summary>
    /// <returns>An <see cref="OptionalValue{ValueType}" /> where <see cref="HasNoValue" /> is <see langword="false" />.</returns>
    public static readonly OptionalValue<ValueType> None = new(default!, false);

    /// <summary>
    /// Converts a nullable value to an <see cref="OptionalValue{ValueType}" /> whereby a <see langword="null" /> value translates to <see cref="None" />.
    /// </summary>
    /// <param name="value">The target nullable value.</param>
    /// <returns>A new <see cref="OptionalValue{ValueType}" /> if the given <paramref name="value" /> is not <see langword="null" />; otherwise <see cref="None" />.</returns>
    public static OptionalValue<ValueType> Create(ValueType? value) => (value is null) ? None : new(value);

    public int CompareTo(OptionalValue<ValueType> other) => other.HasValue ? (HasValue ? _defaultComparer.Compare(Value, other.Value) : -1) : HasValue ? 1 : 0;

    public bool Equals(OptionalValue<ValueType> other) => other.HasValue ? HasValue && _defaultEquality.Equals(Value, other.Value) : !HasValue;

    public override bool Equals(object? obj) => obj is OptionalValue<ValueType> other && Equals(other);

    public override int GetHashCode() => HasValue ? _getHashCode(Value) : 0;

    public override string ToString() => HasValue ? _toString(Value) : string.Empty;

    public static bool operator ==(OptionalValue<ValueType> left, OptionalValue<ValueType> right) => left.Equals(right);

    public static bool operator !=(OptionalValue<ValueType> left, OptionalValue<ValueType> right) => !(left == right);

    public static bool operator <(OptionalValue<ValueType> left, OptionalValue<ValueType> right) => left.CompareTo(right) < 0;

    public static bool operator <=(OptionalValue<ValueType> left, OptionalValue<ValueType> right) => left.CompareTo(right) <= 0;

    public static bool operator >(OptionalValue<ValueType> left, OptionalValue<ValueType> right) => left.CompareTo(right) > 0;

    public static bool operator >=(OptionalValue<ValueType> left, OptionalValue<ValueType> right) => left.CompareTo(right) >= 0;
}