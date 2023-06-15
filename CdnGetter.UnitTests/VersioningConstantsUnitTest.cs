using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CdnGetter.UnitTests
{
    public class VersioningConstantsUnitTest
    {
        private readonly ITestOutputHelper _output;

        public VersioningConstantsUnitTest(ITestOutputHelper output) => _output = output;

        private static IEnumerable<(ushort Value, string Text)> GetRomanNumeralTestValues(bool includeEmpty = false)
        {
            (ushort, string)[] onesValues =
            {
                (1, "I"),
                (2, "II"),
                (3, "III"),
                (4, "IV"),
                (5, "V"),
                (6, "VI"),
                (7, "VII"),
                (8, "VIII"),
                (9, "IX")
            };
            (ushort, string)[] tensValues =
            {
                (10, "X"),
                (20, "XX"),
                (30, "XXX"),
                (40, "XL"),
                (50, "L"),
                (60, "LX"),
                (70, "LXX"),
                (80, "LXXX"),
                (90, "XC")
            };
            (ushort, string)[] hundredsValues =
            {
                (100, "C"),
                (200, "CC"),
                (300, "CCC"),
                (400, "CD"),
                (500, "D"),
                (600, "DC"),
                (700, "DCC"),
                (800, "DCCC"),
                (900, "CM")
            };
            if (includeEmpty)
                yield return (0, string.Empty);
            foreach ((ushort iV, string iS) in onesValues)
                yield return (iV,iS);
            foreach ((ushort xV, string xS) in tensValues)
            {
                yield return (xV, xS);
                foreach ((ushort iV, string iS) in onesValues)
                    yield return ((ushort)(xV + iV), xS + iS);
            }
            foreach ((ushort cV, string cS) in hundredsValues)
            {
                yield return (cV, cS);
                foreach ((ushort iV, string iS) in onesValues)
                    yield return ((ushort)(cV + iV), cS + iS);
                foreach ((ushort xV, string xS) in tensValues)
                {
                    yield return ((ushort)(cV + xV), cS + xS);
                    foreach ((ushort iV, string iS) in onesValues)
                        yield return ((ushort)(cV + xV + iV), cS + xS + iS);
                }
            }
            foreach ((ushort mV, string mS) in new (ushort, string)[] { (1000, "M"), (2000, "MM"), (3000, "MMM") })
            {
                yield return (mV, mS);
                foreach ((ushort iV, string iS) in onesValues)
                    yield return ((ushort)(mV + iV), mS + iS);
                foreach ((ushort xV, string xS) in tensValues)
                {
                    yield return ((ushort)(mV + xV), mS + xS);
                    foreach ((ushort iV, string iS) in onesValues)
                        yield return ((ushort)(mV + xV + iV), mS + xS + iS);
                }
                foreach ((ushort cV, string cS) in hundredsValues)
                {
                    yield return ((ushort)(mV + cV), mS + cS);
                    foreach ((ushort iV, string iS) in onesValues)
                        yield return ((ushort)(mV + cV + iV), mS + cS + iS);
                    foreach ((ushort xV, string xS) in tensValues)
                    {
                        yield return ((ushort)(mV + cV + xV), mS + cS + xS);
                        foreach ((ushort iV, string iS) in onesValues)
                            yield return ((ushort)(mV + cV + xV + iV), mS + cS + xS + iS);
                    }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.ToRomanNumeral(ushort)" />.
        /// </summary>
        [Fact]
        public void ToRomanNumeralTest()
        {
            foreach ((ushort value, string expected) in GetRomanNumeralTestValues())
            {
                try
                {
                    string actual = Versioning.VersioningConstants.ToRomanNumeral(value);
                    Assert.Equal(expected, actual);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ToRomanNumeral)}:(value: {value}); // expected: \"{expected.ToCsLikeCode()}\"");
                    throw;
                }
            }
        }
        
        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.MoveFromI(ReadOnlySpan{char}, int, int)" />.
        /// </summary>
        [Fact]
        public void MoveFromITest()
        {
            RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
            RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
            using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
            foreach ((string source, int startIndex, int endIndex) in GetRomanNumeralTestValues().Where(r => r.Text[0] == 'I').Select((a, i) =>
            {
                enumerator.MoveNext();
                string r = (i % 1 == 0) ? a.Text : a.Text.ToLower();
                (string lt, string tt) = enumerator.Current;
                return (lt.Length > 0) ? (lt + ((tt.Length > 0) ? r + tt : r), lt.Length, lt.Length + r.Length) : ((tt.Length > 0) ? r + tt : r, 0, r.Length);
            }))
            {
                ReadOnlySpan<char> span = source.AsSpan();
                try
                {
                    int actualResult = Versioning.VersioningConstants.MoveFromI(span, startIndex, endIndex);
                    Assert.Equal(endIndex, actualResult);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromI)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex}); // expectedResult: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        int actualResult = Versioning.VersioningConstants.MoveFromI(span, startIndex, source.Length);
                        Assert.Equal(endIndex, actualResult);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromI)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {source.Length}); // expectedResult: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                        try
                        {
                            int actualResult = Versioning.VersioningConstants.MoveFromI(span, startIndex, endIndex + 1);
                            Assert.Equal(endIndex, actualResult);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromI)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex + 1}); // expectedResult: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.MoveFromV(ReadOnlySpan{char}, int, int)" />.
        /// </summary>
        [Fact]
        public void MoveFromVTest()
        {
            RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
            RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
            using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
            foreach ((string source, int startIndex, int endIndex) in GetRomanNumeralTestValues().Where(r => r.Text[0] == 'V').Select((a, i) =>
            {
                enumerator.MoveNext();
                string r = (i % 1 == 0) ? a.Text : a.Text.ToLower();
                (string lt, string tt) = enumerator.Current;
                return (lt.Length > 0) ? (lt + ((tt.Length > 0) ? r + tt : r), lt.Length, lt.Length + r.Length) : ((tt.Length > 0) ? r + tt : r, 0, r.Length);
            }))
            {
                ReadOnlySpan<char> span = source.AsSpan();
                try
                {
                    int actualResult = Versioning.VersioningConstants.MoveFromV(span, startIndex, endIndex);
                    Assert.Equal(endIndex, actualResult);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromV)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex}); // expectedResult: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        int actualResult = Versioning.VersioningConstants.MoveFromV(span, startIndex, source.Length);
                        Assert.Equal(endIndex, actualResult);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromV)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {source.Length}); // expectedResult: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                        try
                        {
                            int actualResult = Versioning.VersioningConstants.MoveFromV(span, startIndex, endIndex + 1);
                            Assert.Equal(endIndex, actualResult);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromV)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex + 1}); // expectedResult: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.MoveFromX(ReadOnlySpan{char}, int, int)" />.
        /// </summary>
        [Fact]
        public void MoveFromXTest()
        {
            RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
            RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
            using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
            foreach ((string source, int startIndex, int endIndex) in GetRomanNumeralTestValues().Where(r => r.Text[0] == 'X').Select((a, i) =>
            {
                enumerator.MoveNext();
                string r = (i % 1 == 0) ? a.Text : a.Text.ToLower();
                (string lt, string tt) = enumerator.Current;
                return (lt.Length > 0) ? (lt + ((tt.Length > 0) ? r + tt : r), lt.Length, lt.Length + r.Length) : ((tt.Length > 0) ? r + tt : r, 0, r.Length);
            }))
            {
                ReadOnlySpan<char> span = source.AsSpan();
                try
                {
                    int actualResult = Versioning.VersioningConstants.MoveFromX(span, startIndex, endIndex);
                    Assert.Equal(endIndex, actualResult);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromX)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex}); // expectedResult: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        int actualResult = Versioning.VersioningConstants.MoveFromX(span, startIndex, source.Length);
                        Assert.Equal(endIndex, actualResult);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromX)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {source.Length}); // expectedResult: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                        try
                        {
                            int actualResult = Versioning.VersioningConstants.MoveFromX(span, startIndex, endIndex + 1);
                            Assert.Equal(endIndex, actualResult);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromX)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex + 1}); // expectedResult: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.MoveFromL(ReadOnlySpan{char}, int, int)" />.
        /// </summary>
        [Fact]
        public void MoveFromLTest()
        {
            RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
            RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
            using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
            foreach ((string source, int startIndex, int endIndex) in GetRomanNumeralTestValues().Where(r => r.Text[0] == 'L').Select((a, i) =>
            {
                enumerator.MoveNext();
                string r = (i % 1 == 0) ? a.Text : a.Text.ToLower();
                (string lt, string tt) = enumerator.Current;
                return (lt.Length > 0) ? (lt + ((tt.Length > 0) ? r + tt : r), lt.Length, lt.Length + r.Length) : ((tt.Length > 0) ? r + tt : r, 0, r.Length);
            }))
            {
                ReadOnlySpan<char> span = source.AsSpan();
                try
                {
                    int actualResult = Versioning.VersioningConstants.MoveFromL(span, startIndex, endIndex);
                    Assert.Equal(endIndex, actualResult);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromL)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex}); // expectedResult: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        int actualResult = Versioning.VersioningConstants.MoveFromL(span, startIndex, source.Length);
                        Assert.Equal(endIndex, actualResult);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromL)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {source.Length}); // expectedResult: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                        try
                        {
                            int actualResult = Versioning.VersioningConstants.MoveFromL(span, startIndex, endIndex + 1);
                            Assert.Equal(endIndex, actualResult);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromL)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex + 1}); // expectedResult: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.MoveFromC(ReadOnlySpan{char}, int, int)" />.
        /// </summary>
        [Fact]
        public void MoveFromCTest()
        {
            RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
            RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
            using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
            foreach ((string source, int startIndex, int endIndex) in GetRomanNumeralTestValues().Where(r => r.Text[0] == 'C').Select((a, i) =>
            {
                enumerator.MoveNext();
                string r = (i % 1 == 0) ? a.Text : a.Text.ToLower();
                (string lt, string tt) = enumerator.Current;
                return (lt.Length > 0) ? (lt + ((tt.Length > 0) ? r + tt : r), lt.Length, lt.Length + r.Length) : ((tt.Length > 0) ? r + tt : r, 0, r.Length);
            }))
            {
                ReadOnlySpan<char> span = source.AsSpan();
                try
                {
                    int actualResult = Versioning.VersioningConstants.MoveFromC(span, startIndex, endIndex);
                    Assert.Equal(endIndex, actualResult);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromC)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex}); // expectedResult: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        int actualResult = Versioning.VersioningConstants.MoveFromC(span, startIndex, source.Length);
                        Assert.Equal(endIndex, actualResult);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromC)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {source.Length}); // expectedResult: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                        try
                        {
                            int actualResult = Versioning.VersioningConstants.MoveFromC(span, startIndex, endIndex + 1);
                            Assert.Equal(endIndex, actualResult);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromC)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex + 1}); // expectedResult: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.MoveFromD(ReadOnlySpan{char}, int, int)" />.
        /// </summary>
        [Fact]
        public void MoveFromDTest()
        {
            RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
            RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
            using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
            foreach ((string source, int startIndex, int endIndex) in GetRomanNumeralTestValues().Where(r => r.Text[0] == 'D').Select((a, i) =>
            {
                enumerator.MoveNext();
                string r = (i % 1 == 0) ? a.Text : a.Text.ToLower();
                (string lt, string tt) = enumerator.Current;
                return (lt.Length > 0) ? (lt + ((tt.Length > 0) ? r + tt : r), lt.Length, lt.Length + r.Length) : ((tt.Length > 0) ? r + tt : r, 0, r.Length);
            }))
            {
                ReadOnlySpan<char> span = source.AsSpan();
                try
                {
                    int actualResult = Versioning.VersioningConstants.MoveFromD(span, startIndex, endIndex);
                    Assert.Equal(endIndex, actualResult);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromD)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex}); // expectedResult: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        int actualResult = Versioning.VersioningConstants.MoveFromD(span, startIndex, source.Length);
                        Assert.Equal(endIndex, actualResult);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromD)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {source.Length}); // expectedResult: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                        try
                        {
                            int actualResult = Versioning.VersioningConstants.MoveFromD(span, startIndex, endIndex + 1);
                            Assert.Equal(endIndex, actualResult);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromD)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex + 1}); // expectedResult: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.MoveFromM(ReadOnlySpan{char}, int, int)" />.
        /// </summary>
        [Fact]
        public void MoveFromMTest()
        {
            RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
            RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
            using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
            foreach ((string source, int startIndex, int endIndex) in GetRomanNumeralTestValues().Where(r => r.Text[0] == 'M').Select((a, i) =>
            {
                enumerator.MoveNext();
                string r = (i % 1 == 0) ? a.Text : a.Text.ToLower();
                (string lt, string tt) = enumerator.Current;
                return (lt.Length > 0) ? (lt + ((tt.Length > 0) ? r + tt : r), lt.Length, lt.Length + r.Length) : ((tt.Length > 0) ? r + tt : r, 0, r.Length);
            }))
            {
                ReadOnlySpan<char> span = source.AsSpan();
                try
                {
                    int actualResult = Versioning.VersioningConstants.MoveFromM(span, startIndex, endIndex);
                    Assert.Equal(endIndex, actualResult);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromM)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex}); // expectedResult: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        int actualResult = Versioning.VersioningConstants.MoveFromM(span, startIndex, source.Length);
                        Assert.Equal(endIndex, actualResult);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromM)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {source.Length}); // expectedResult: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                        try
                        {
                            int actualResult = Versioning.VersioningConstants.MoveFromM(span, startIndex, endIndex + 1);
                            Assert.Equal(endIndex, actualResult);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.MoveFromM)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex + 1}); // expectedResult: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.ParseFromI(ReadOnlySpan{char}, int, int, out int)" />.
        /// </summary>
        [Fact]
        public void ParseFromITest()
        {
            RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
            RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
            using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
            foreach ((string source, int startIndex, int endIndex, ushort expectedResult) in GetRomanNumeralTestValues().Where(r => r.Text[0] == 'I').Select((a, i) =>
            {
                enumerator.MoveNext();
                string r = (i % 1 == 0) ? a.Text : a.Text.ToLower();
                (string lt, string tt) = enumerator.Current;
                return (lt.Length > 0) ? (lt + ((tt.Length > 0) ? r + tt : r), lt.Length, lt.Length + r.Length, a.Value) : ((tt.Length > 0) ? r + tt : r, 0, r.Length, a.Value);
            }))
            {
                ReadOnlySpan<char> span = source.AsSpan();
                try
                {
                    ushort actualResult = Versioning.VersioningConstants.ParseFromI(span, startIndex, endIndex, out int actualNextIndex);
                    Assert.Equal(expectedResult, actualResult);
                    Assert.Equal(endIndex, actualNextIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromI)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        ushort actualResult = Versioning.VersioningConstants.ParseFromI(span, startIndex, source.Length, out int actualNextIndex);
                        Assert.Equal(expectedResult, actualResult);
                        Assert.Equal(endIndex, actualNextIndex);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromI)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {source.Length}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                    try
                        {
                            ushort actualResult = Versioning.VersioningConstants.ParseFromI(span, startIndex, endIndex + 1, out int actualNextIndex);
                            Assert.Equal(expectedResult, actualResult);
                            Assert.Equal(endIndex, actualNextIndex);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromI)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex + 1}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.ParseFromV(ReadOnlySpan{char}, int, int, out int)" />.
        /// </summary>
        [Fact]
        public void ParseFromVTest()
        {
            RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
            RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
            using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
            foreach ((string source, int startIndex, int endIndex, ushort expectedResult) in GetRomanNumeralTestValues().Where(r => r.Text[0] == 'V').Select((a, i) =>
            {
                enumerator.MoveNext();
                string r = (i % 1 == 0) ? a.Text : a.Text.ToLower();
                (string lt, string tt) = enumerator.Current;
                return (lt.Length > 0) ? (lt + ((tt.Length > 0) ? r + tt : r), lt.Length, lt.Length + r.Length, a.Value) : ((tt.Length > 0) ? r + tt : r, 0, r.Length, a.Value);
            }))
            {
                ReadOnlySpan<char> span = source.AsSpan();
                try
                {
                    ushort actualResult = Versioning.VersioningConstants.ParseFromV(span, startIndex, endIndex, out int actualNextIndex);
                    Assert.Equal(expectedResult, actualResult);
                    Assert.Equal(endIndex, actualNextIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromV)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        ushort actualResult = Versioning.VersioningConstants.ParseFromV(span, startIndex, source.Length, out int actualNextIndex);
                        Assert.Equal(expectedResult, actualResult);
                        Assert.Equal(endIndex, actualNextIndex);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromV)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {source.Length}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                    try
                        {
                            ushort actualResult = Versioning.VersioningConstants.ParseFromV(span, startIndex, endIndex + 1, out int actualNextIndex);
                            Assert.Equal(expectedResult, actualResult);
                            Assert.Equal(endIndex, actualNextIndex);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromV)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex + 1}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.ParseFromX(ReadOnlySpan{char}, int, int, out int)" />.
        /// </summary>
        [Fact]
        public void ParseFromXTest()
        {
            RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
            RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
            using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
            foreach ((string source, int startIndex, int endIndex, ushort expectedResult) in GetRomanNumeralTestValues().Where(r => r.Text[0] == 'X').Select((a, i) =>
            {
                enumerator.MoveNext();
                string r = (i % 1 == 0) ? a.Text : a.Text.ToLower();
                (string lt, string tt) = enumerator.Current;
                return (lt.Length > 0) ? (lt + ((tt.Length > 0) ? r + tt : r), lt.Length, lt.Length + r.Length, a.Value) : ((tt.Length > 0) ? r + tt : r, 0, r.Length, a.Value);
            }))
            {
                ReadOnlySpan<char> span = source.AsSpan();
                try
                {
                    ushort actualResult = Versioning.VersioningConstants.ParseFromX(span, startIndex, endIndex, out int actualNextIndex);
                    Assert.Equal(expectedResult, actualResult);
                    Assert.Equal(endIndex, actualNextIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromX)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        ushort actualResult = Versioning.VersioningConstants.ParseFromX(span, startIndex, source.Length, out int actualNextIndex);
                        Assert.Equal(expectedResult, actualResult);
                        Assert.Equal(endIndex, actualNextIndex);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromX)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {source.Length}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                    try
                        {
                            ushort actualResult = Versioning.VersioningConstants.ParseFromX(span, startIndex, endIndex + 1, out int actualNextIndex);
                            Assert.Equal(expectedResult, actualResult);
                            Assert.Equal(endIndex, actualNextIndex);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromX)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex + 1}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.ParseFromL(ReadOnlySpan{char}, int, int, out int)" />.
        /// </summary>
        [Fact]
        public void ParseFromLTest()
        {
            RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
            RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
            using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
            foreach ((string source, int startIndex, int endIndex, ushort expectedResult) in GetRomanNumeralTestValues().Where(r => r.Text[0] == 'L').Select((a, i) =>
            {
                enumerator.MoveNext();
                string r = (i % 1 == 0) ? a.Text : a.Text.ToLower();
                (string lt, string tt) = enumerator.Current;
                return (lt.Length > 0) ? (lt + ((tt.Length > 0) ? r + tt : r), lt.Length, lt.Length + r.Length, a.Value) : ((tt.Length > 0) ? r + tt : r, 0, r.Length, a.Value);
            }))
            {
                ReadOnlySpan<char> span = source.AsSpan();
                try
                {
                    ushort actualResult = Versioning.VersioningConstants.ParseFromL(span, startIndex, endIndex, out int actualNextIndex);
                    Assert.Equal(expectedResult, actualResult);
                    Assert.Equal(endIndex, actualNextIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromL)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        ushort actualResult = Versioning.VersioningConstants.ParseFromL(span, startIndex, source.Length, out int actualNextIndex);
                        Assert.Equal(expectedResult, actualResult);
                        Assert.Equal(endIndex, actualNextIndex);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromL)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {source.Length}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                    try
                        {
                            ushort actualResult = Versioning.VersioningConstants.ParseFromL(span, startIndex, endIndex + 1, out int actualNextIndex);
                            Assert.Equal(expectedResult, actualResult);
                            Assert.Equal(endIndex, actualNextIndex);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromL)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex + 1}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.ParseFromC(ReadOnlySpan{char}, int, int, out int)" />.
        /// </summary>
        [Fact]
        public void ParseFromCTest()
        {
            RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
            RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
            using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
            foreach ((string source, int startIndex, int endIndex, ushort expectedResult) in GetRomanNumeralTestValues().Where(r => r.Text[0] == 'C').Select((a, i) =>
            {
                enumerator.MoveNext();
                string r = (i % 1 == 0) ? a.Text : a.Text.ToLower();
                (string lt, string tt) = enumerator.Current;
                return (lt.Length > 0) ? (lt + ((tt.Length > 0) ? r + tt : r), lt.Length, lt.Length + r.Length, a.Value) : ((tt.Length > 0) ? r + tt : r, 0, r.Length, a.Value);
            }))
            {
                ReadOnlySpan<char> span = source.AsSpan();
                try
                {
                    ushort actualResult = Versioning.VersioningConstants.ParseFromC(span, startIndex, endIndex, out int actualNextIndex);
                    Assert.Equal(expectedResult, actualResult);
                    Assert.Equal(endIndex, actualNextIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromC)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        ushort actualResult = Versioning.VersioningConstants.ParseFromC(span, startIndex, source.Length, out int actualNextIndex);
                        Assert.Equal(expectedResult, actualResult);
                        Assert.Equal(endIndex, actualNextIndex);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromC)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {source.Length}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                    try
                        {
                            ushort actualResult = Versioning.VersioningConstants.ParseFromC(span, startIndex, endIndex + 1, out int actualNextIndex);
                            Assert.Equal(expectedResult, actualResult);
                            Assert.Equal(endIndex, actualNextIndex);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromC)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex + 1}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.ParseFromD(ReadOnlySpan{char}, int, int, out int)" />.
        /// </summary>
        [Fact]
        public void ParseFromDTest()
        {
            RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
            RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
            using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
            foreach ((string source, int startIndex, int endIndex, ushort expectedResult) in GetRomanNumeralTestValues().Where(r => r.Text[0] == 'D').Select((a, i) =>
            {
                enumerator.MoveNext();
                string r = (i % 1 == 0) ? a.Text : a.Text.ToLower();
                (string lt, string tt) = enumerator.Current;
                return (lt.Length > 0) ? (lt + ((tt.Length > 0) ? r + tt : r), lt.Length, lt.Length + r.Length, a.Value) : ((tt.Length > 0) ? r + tt : r, 0, r.Length, a.Value);
            }))
            {
                ReadOnlySpan<char> span = source.AsSpan();
                try
                {
                    ushort actualResult = Versioning.VersioningConstants.ParseFromD(span, startIndex, endIndex, out int actualNextIndex);
                    Assert.Equal(expectedResult, actualResult);
                    Assert.Equal(endIndex, actualNextIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromD)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        ushort actualResult = Versioning.VersioningConstants.ParseFromD(span, startIndex, source.Length, out int actualNextIndex);
                        Assert.Equal(expectedResult, actualResult);
                        Assert.Equal(endIndex, actualNextIndex);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromD)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {source.Length}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                    try
                        {
                            ushort actualResult = Versioning.VersioningConstants.ParseFromD(span, startIndex, endIndex + 1, out int actualNextIndex);
                            Assert.Equal(expectedResult, actualResult);
                            Assert.Equal(endIndex, actualNextIndex);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromD)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex + 1}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.ParseFromM(ReadOnlySpan{char}, int, int, out int)" />.
        /// </summary>
        [Fact]
        public void ParseFromMTest()
        {
            RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
            RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
            using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
            foreach ((string source, int startIndex, int endIndex, ushort expectedResult) in GetRomanNumeralTestValues().Where(r => r.Text[0] == 'M').Select((a, i) =>
            {
                enumerator.MoveNext();
                string r = (i % 1 == 0) ? a.Text : a.Text.ToLower();
                (string lt, string tt) = enumerator.Current;
                return (lt.Length > 0) ? (lt + ((tt.Length > 0) ? r + tt : r), lt.Length, lt.Length + r.Length, a.Value) : ((tt.Length > 0) ? r + tt : r, 0, r.Length, a.Value);
            }))
            {
                ReadOnlySpan<char> span = source.AsSpan();
                try
                {
                    ushort actualResult = Versioning.VersioningConstants.ParseFromM(span, startIndex, endIndex, out int actualNextIndex);
                    Assert.Equal(expectedResult, actualResult);
                    Assert.Equal(endIndex, actualNextIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromM)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        ushort actualResult = Versioning.VersioningConstants.ParseFromM(span, startIndex, source.Length, out int actualNextIndex);
                        Assert.Equal(expectedResult, actualResult);
                        Assert.Equal(endIndex, actualNextIndex);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromM)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {source.Length}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                    try
                        {
                            ushort actualResult = Versioning.VersioningConstants.ParseFromM(span, startIndex, endIndex + 1, out int actualNextIndex);
                            Assert.Equal(expectedResult, actualResult);
                            Assert.Equal(endIndex, actualNextIndex);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseFromM)}(source: new ReadOnlySpan<char>(\"{source.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {endIndex + 1}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.ParseRomanNumeral(int, ReadOnlySpan{char}, out int)" />.
        /// </summary>
        [Fact]
        public void ParseRomanNumeralTest()// int startIndex, string target, ushort expectedResult, int expectedEndIndex
        {
            RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
            RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
            using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
            foreach ((string target, int startIndex, ushort expectedResult, int expectedEndIndex) in leadingTextValues.GetValues().SelectMany(lt => (lt.Length > 0) ?
                trailingTextValues.GetValues().Select(tt => (tt.Length > 0) ? (lt + tt, lt.Length, lt.Length) : (lt, lt.Length, lt.Length)):
                trailingTextValues.GetValues().Select(tt => (tt, 0, 0))
            ).Select(a => (a.Item1, a.Item2, (ushort)0, a.Item3)).Concat(GetRomanNumeralTestValues().Select((t, i) =>
            {
                enumerator.MoveNext();
                string r = (i % 1 == 0) ? t.Text : t.Text.ToLower();
                (string leadingText, string trailingText) = enumerator.Current;
                if (leadingText.Length == 0)
                {
                    if (trailingText.Length > 0)
                        return (r + trailingText, 0, t.Value, r.Length);
                    return (r, 0, t.Value, r.Length);
                }
                if (trailingText.Length > 0)
                    return (leadingText + r + trailingText, leadingText.Length, t.Value, leadingText.Length + r.Length);
                return (leadingText + r, leadingText.Length, t.Value, leadingText.Length + r.Length);
            })))
            {
                ReadOnlySpan<char> span = target.AsSpan();
                try
                {
                    ushort actualResult = Versioning.VersioningConstants.ParseRomanNumeral(span, startIndex, expectedEndIndex, out int actualEndIndex);
                    Assert.Equal(expectedResult, actualResult);
                    Assert.Equal(expectedEndIndex, actualEndIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseRomanNumeral)}(target: new ReadOnlySpan<char>(\"{target.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {expectedEndIndex}, out int actualEndIndex); // expectedResult: {expectedResult}, expectedEndIndex: {expectedEndIndex}");
                    throw;
                }
                if (startIndex == 0)
                    try
                    {
                        ushort actualResult = Versioning.VersioningConstants.ParseRomanNumeral(span, out int actualEndIndex);
                        Assert.Equal(expectedResult, actualResult);
                        Assert.Equal(expectedEndIndex, actualEndIndex);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseRomanNumeral)}(target: new ReadOnlySpan<char>(\"{target.ToCsLikeCode()}\"), out int actualEndIndex); // expectedResult: {expectedResult}, expectedEndIndex: {expectedEndIndex}");
                        throw;
                    }
                try
                {
                    ushort actualResult = Versioning.VersioningConstants.ParseRomanNumeral(span, startIndex, out int actualEndIndex);
                    Assert.Equal(expectedResult, actualResult);
                    Assert.Equal(expectedEndIndex, actualEndIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseRomanNumeral)}(target: new ReadOnlySpan<char>(\"{target.ToCsLikeCode()}\"), startIndex: {startIndex} out int actualEndIndex); // expectedResult: {expectedResult}, expectedEndIndex: {expectedEndIndex}");
                    throw;
                }
                int diff = target.Length - expectedEndIndex;
                if (diff > 0)
                {
                    try
                    {
                        ushort actualResult = Versioning.VersioningConstants.ParseRomanNumeral(span, startIndex, target.Length, out int actualEndIndex);
                        Assert.Equal(expectedResult, actualResult);
                        Assert.Equal(expectedEndIndex, actualEndIndex);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseRomanNumeral)}(target: new ReadOnlySpan<char>(\"{target.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {target.Length}, out int actualEndIndex); // expectedResult: {expectedResult}, expectedEndIndex: {expectedEndIndex}");
                        throw;
                    }
                    if (diff > 1)
                        try
                        {
                            ushort actualResult = Versioning.VersioningConstants.ParseRomanNumeral(span, startIndex, expectedEndIndex + 1, out int actualEndIndex);
                            Assert.Equal(expectedResult, actualResult);
                            Assert.Equal(expectedEndIndex, actualEndIndex);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseRomanNumeral)}(target: new ReadOnlySpan<char>(\"{target.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {expectedEndIndex + 1}, out int actualEndIndex); // expectedResult: {expectedResult}, expectedEndIndex: {expectedEndIndex}");
                            throw;
                        }
                }
            }
        }

        // public class TestRomanNumeralTestData : TheoryData<int, string, bool, int>
        // {
        //     public TestRomanNumeralTestData()
        //     {
        //         RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
        //         RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
        //         foreach (string leadingText in leadingTextValues.GetValues())
        //         {
        //             if (leadingText.Length > 0)
        //                 foreach (string trailingText in trailingTextValues.GetValues())
        //                     if (trailingText.Length > 0)
        //                         Add(leadingText.Length, leadingText + trailingText, false, leadingText.Length);
        //                     else
        //                         Add(leadingText.Length, leadingText, false, leadingText.Length);
        //             else
        //                 foreach (string trailingText in trailingTextValues.GetValues())
        //                     Add(0, trailingText, false, 0);
        //         }
        //         using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
        //         bool lc = true;
        //         foreach (string text in GetRomanNumeralTestValues().Select(a => a.Text))
        //         {
        //             enumerator.MoveNext();
        //             lc = !lc;
        //             (string leadingText, string trailingText) = enumerator.Current;
        //             if (leadingText.Length == 0)
        //             {
        //                 if (trailingText.Length > 0)
        //                     Add(0, (lc ? text.ToLower() : text) + trailingText, true, text.Length);
        //                 else
        //                     Add(0, lc ? text.ToLower() : text, true, text.Length);
        //             }
        //             else if (trailingText.Length > 0)
        //                 Add(leadingText.Length, leadingText + (lc ? text.ToLower() : text) + trailingText, true, leadingText.Length + text.Length);
        //             else
        //                 Add(leadingText.Length, leadingText + (lc ? text.ToLower() : text), true, leadingText.Length + text.Length);
        //         }
        //     }
        // }

        // [Theory]
        // [ClassData(typeof(TestRomanNumeralTestData))]
        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.TestRomanNumeral(int, ReadOnlySpan{char}, out int)" />.
        /// </summary>
        [Fact]
        public void TestRomanNumeralTest() // int startIndex, string target, bool expectedResult, int expectedEndIndex
        {
            RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
            RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
            using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
            foreach ((int startIndex, string target, bool expectedResult, int expectedEndIndex) in leadingTextValues.GetValues().SelectMany(lt => (lt.Length > 0) ?
                trailingTextValues.GetValues().Select(tt => (tt.Length > 0) ?  (lt.Length, lt + tt, lt.Length) : (lt.Length, lt, lt.Length)):
                trailingTextValues.GetValues().Select(tt => (0, tt, 0))
            ).Select(a => (a.Item1, a.Item2, false, a.Item3)).Concat(GetRomanNumeralTestValues().Select((t, i) =>
            {
                enumerator.MoveNext();
                string r = (i % 1 == 0) ? t.Text : t.Text.ToLower();
                (string leadingText, string trailingText) = enumerator.Current;
                if (leadingText.Length == 0)
                {
                    if (trailingText.Length > 0)
                        return (0, r + trailingText, true, r.Length);
                    return (0, r, true, r.Length);
                }
                if (trailingText.Length > 0)
                    return (leadingText.Length, leadingText + r + trailingText, true, leadingText.Length + r.Length);
                return (leadingText.Length, leadingText + r, true, leadingText.Length + r.Length);
            })))
            {
                try
                {
                    bool actualResult = Versioning.VersioningConstants.TestRomanNumeral(startIndex, target.AsSpan(), out int actualEndIndex);
                    Assert.Equal(expectedResult, actualResult);
                    Assert.Equal(expectedEndIndex, actualEndIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(TestRomanNumeralTest)}: startIndex = {startIndex}, target = \"{target.ToCsLikeCode()}\", expectedResult = {expectedResult}, expectedEndIndex = {expectedEndIndex}");
                    throw;
                }
            }
        }
    }
}