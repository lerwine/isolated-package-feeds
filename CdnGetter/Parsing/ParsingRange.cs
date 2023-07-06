using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace CdnGetter.Parsing;

[StructLayout(LayoutKind.Sequential)]
public readonly struct ParsingRange : IEquatable<ParsingRange>, IComparable<ParsingRange>
{
    public static readonly ParsingRange Empty = new();

    public int StartIndex { get; }

    public int Length { get; }

    public int EndIndex => StartIndex + Length;

    public ParsingRange(int startIndex, int length)
    {
        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length));
        StartIndex = startIndex;
        Length = length;
    }

    public bool TryGetNext(ParsingSource source, out ParsingRange result)
    {
        if (source is not null)
        {
            int startIndex = EndIndex;
            int length = source.Count - startIndex;
            if (length > 0)
            {
                result = new(startIndex, length);
                return true;
            }
        }
        result = Empty;
        return false;
    }

    public int CompareTo(ParsingRange other)
    {
        int result = StartIndex - other.StartIndex;
        return (result == 0) ? Length - other.Length : result;
    }

    public bool Equals(ParsingRange other) => StartIndex == other.StartIndex && EndIndex == other.EndIndex;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is ParsingRange other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(StartIndex, Length);

    public override string ToString() => $"{{ {nameof(StartIndex)}: {StartIndex}, {nameof(Length)}: {Length} }}";
    
    public static bool operator ==(ParsingRange left, ParsingRange right) => left.Equals(right);

    public static bool operator !=(ParsingRange left, ParsingRange right) => !left.Equals(right);

    public static bool operator <(ParsingRange left, ParsingRange right) => left.CompareTo(right) < 0;

    public static bool operator <=(ParsingRange left, ParsingRange right) => left.CompareTo(right) <= 0;

    public static bool operator >(ParsingRange left, ParsingRange right) => left.CompareTo(right) > 0;

    public static bool operator >=(ParsingRange left, ParsingRange right) => left.CompareTo(right) >= 0;

    public static ParsingRange operator ++(ParsingRange source)
    {
        try { return new(source.StartIndex, source.Length + 1); }
        catch (OverflowException exc) { throw new OverflowException($"{nameof(Length)} cannot be greater than {int.MaxValue}.", exc); }
    }
    
    public static ParsingRange operator --(ParsingRange source)
    {
        try { return new(source.StartIndex, source.Length - 1); }
        catch (ArgumentOutOfRangeException exc) { throw new OverflowException($"{nameof(Length)} cannot be less than zero.", exc); }
    }
    
    public static ParsingRange operator +(ParsingRange source, int value)
    {
        try { return new(source.StartIndex, source.Length + value); }
        catch (ArgumentOutOfRangeException exc) { throw new OverflowException($"{nameof(Length)} cannot be less than zero.", exc); }
        catch (OverflowException exc) { throw new OverflowException($"{nameof(Length)} cannot be greater than {int.MaxValue}.", exc); }
    }
    
    public static ParsingRange operator -(ParsingRange source, int value)
    {
        try { return new(source.StartIndex, source.Length - value); }
        catch (ArgumentOutOfRangeException exc) { throw new OverflowException($"{nameof(Length)} cannot be less than zero.", exc); }
        catch (OverflowException exc) { throw new OverflowException($"{nameof(Length)} cannot be greater than {int.MaxValue}.", exc); }
    }
}