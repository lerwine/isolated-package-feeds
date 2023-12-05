using Xunit.Abstractions;

namespace CdnGetter.UnitTests
{
    public class RomanNumeralUnitTest
    {
        private readonly ITestOutputHelper _output;

        public RomanNumeralUnitTest(ITestOutputHelper output) => _output = output;

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
        /// Unit test for <see cref="Parsing.RomanNumeral.ToRomanNumeral(ushort)" />.
        /// </summary>
        [Fact]
        public void ToRomanNumeralTest()
        {
            try
            {
                string? actual = Parsing.RomanNumeral.ToRomanNumeral(0);
                Assert.Null(actual);
            }
            catch
            {
                _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ToRomanNumeral)}(value: 0); // expected: null");
                throw;
            }
            foreach ((ushort value, string expected) in GetRomanNumeralTestValues())
            {
                try
                {
                    string? actual = Parsing.RomanNumeral.ToRomanNumeral(value);
                    Assert.NotNull(actual);
                    Assert.Equal(expected, actual);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ToRomanNumeral)}(value: {value}); // expected: \"{expected.ToCsLikeCode()}\"");
                    throw;
                }
            }
        }
        
        /// <summary>
        /// Unit test for <see cref="Parsing.RomanNumeral.MoveFromI(ReadOnlySpan{char}, int, int)" />.
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
                    int actualResult = Parsing.RomanNumeral.MoveFromI(span, startIndex, endIndex);
                    Assert.Equal(endIndex, actualResult);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromI)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex}); // expectedResult: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        int actualResult = Parsing.RomanNumeral.MoveFromI(span, startIndex, source.Length);
                        Assert.Equal(endIndex, actualResult);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromI)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {source.Length}); // expectedResult: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                        try
                        {
                            int actualResult = Parsing.RomanNumeral.MoveFromI(span, startIndex, endIndex + 1);
                            Assert.Equal(endIndex, actualResult);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromI)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex + 1}); // expectedResult: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Parsing.RomanNumeral.MoveFromV(ReadOnlySpan{char}, int, int)" />.
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
                    int actualResult = Parsing.RomanNumeral.MoveFromV(span, startIndex, endIndex);
                    Assert.Equal(endIndex, actualResult);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromV)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex}); // expectedResult: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        int actualResult = Parsing.RomanNumeral.MoveFromV(span, startIndex, source.Length);
                        Assert.Equal(endIndex, actualResult);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromV)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {source.Length}); // expectedResult: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                        try
                        {
                            int actualResult = Parsing.RomanNumeral.MoveFromV(span, startIndex, endIndex + 1);
                            Assert.Equal(endIndex, actualResult);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromV)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex + 1}); // expectedResult: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Parsing.RomanNumeral.MoveFromX(ReadOnlySpan{char}, int, int)" />.
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
                    int actualResult = Parsing.RomanNumeral.MoveFromX(span, startIndex, endIndex);
                    Assert.Equal(endIndex, actualResult);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromX)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex}); // expectedResult: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        int actualResult = Parsing.RomanNumeral.MoveFromX(span, startIndex, source.Length);
                        Assert.Equal(endIndex, actualResult);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromX)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {source.Length}); // expectedResult: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                        try
                        {
                            int actualResult = Parsing.RomanNumeral.MoveFromX(span, startIndex, endIndex + 1);
                            Assert.Equal(endIndex, actualResult);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromX)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex + 1}); // expectedResult: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Parsing.RomanNumeral.MoveFromL(ReadOnlySpan{char}, int, int)" />.
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
                    int actualResult = Parsing.RomanNumeral.MoveFromL(span, startIndex, endIndex);
                    Assert.Equal(endIndex, actualResult);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromL)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex}); // expectedResult: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        int actualResult = Parsing.RomanNumeral.MoveFromL(span, startIndex, source.Length);
                        Assert.Equal(endIndex, actualResult);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromL)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {source.Length}); // expectedResult: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                        try
                        {
                            int actualResult = Parsing.RomanNumeral.MoveFromL(span, startIndex, endIndex + 1);
                            Assert.Equal(endIndex, actualResult);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromL)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex + 1}); // expectedResult: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Parsing.RomanNumeral.MoveFromC(ReadOnlySpan{char}, int, int)" />.
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
                    int actualResult = Parsing.RomanNumeral.MoveFromC(span, startIndex, endIndex);
                    Assert.Equal(endIndex, actualResult);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromC)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex}); // expectedResult: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        int actualResult = Parsing.RomanNumeral.MoveFromC(span, startIndex, source.Length);
                        Assert.Equal(endIndex, actualResult);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromC)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {source.Length}); // expectedResult: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                        try
                        {
                            int actualResult = Parsing.RomanNumeral.MoveFromC(span, startIndex, endIndex + 1);
                            Assert.Equal(endIndex, actualResult);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromC)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex + 1}); // expectedResult: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Parsing.RomanNumeral.MoveFromD(ReadOnlySpan{char}, int, int)" />.
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
                    int actualResult = Parsing.RomanNumeral.MoveFromD(span, startIndex, endIndex);
                    Assert.Equal(endIndex, actualResult);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromD)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex}); // expectedResult: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        int actualResult = Parsing.RomanNumeral.MoveFromD(span, startIndex, source.Length);
                        Assert.Equal(endIndex, actualResult);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromD)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {source.Length}); // expectedResult: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                        try
                        {
                            int actualResult = Parsing.RomanNumeral.MoveFromD(span, startIndex, endIndex + 1);
                            Assert.Equal(endIndex, actualResult);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromD)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex + 1}); // expectedResult: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Parsing.RomanNumeral.MoveFromM(ReadOnlySpan{char}, int, int)" />.
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
                    int actualResult = Parsing.RomanNumeral.MoveFromM(span, startIndex, endIndex);
                    Assert.Equal(endIndex, actualResult);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromM)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex}); // expectedResult: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        int actualResult = Parsing.RomanNumeral.MoveFromM(span, startIndex, source.Length);
                        Assert.Equal(endIndex, actualResult);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromM)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {source.Length}); // expectedResult: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                        try
                        {
                            int actualResult = Parsing.RomanNumeral.MoveFromM(span, startIndex, endIndex + 1);
                            Assert.Equal(endIndex, actualResult);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.MoveFromM)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex + 1}); // expectedResult: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Parsing.RomanNumeral.ParseFromI(ReadOnlySpan{char}, int, int, out int)" />.
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
                    ushort actualResult = Parsing.RomanNumeral.ParseFromI(span, startIndex, endIndex, out int actualNextIndex);
                    Assert.Equal(expectedResult, actualResult);
                    Assert.Equal(endIndex, actualNextIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromI)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        ushort actualResult = Parsing.RomanNumeral.ParseFromI(span, startIndex, source.Length, out int actualNextIndex);
                        Assert.Equal(expectedResult, actualResult);
                        Assert.Equal(endIndex, actualNextIndex);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromI)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {source.Length}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                    try
                        {
                            ushort actualResult = Parsing.RomanNumeral.ParseFromI(span, startIndex, endIndex + 1, out int actualNextIndex);
                            Assert.Equal(expectedResult, actualResult);
                            Assert.Equal(endIndex, actualNextIndex);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromI)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex + 1}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Parsing.RomanNumeral.ParseFromV(ReadOnlySpan{char}, int, int, out int)" />.
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
                    ushort actualResult = Parsing.RomanNumeral.ParseFromV(span, startIndex, endIndex, out int actualNextIndex);
                    Assert.Equal(expectedResult, actualResult);
                    Assert.Equal(endIndex, actualNextIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromV)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        ushort actualResult = Parsing.RomanNumeral.ParseFromV(span, startIndex, source.Length, out int actualNextIndex);
                        Assert.Equal(expectedResult, actualResult);
                        Assert.Equal(endIndex, actualNextIndex);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromV)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {source.Length}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                    try
                        {
                            ushort actualResult = Parsing.RomanNumeral.ParseFromV(span, startIndex, endIndex + 1, out int actualNextIndex);
                            Assert.Equal(expectedResult, actualResult);
                            Assert.Equal(endIndex, actualNextIndex);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromV)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex + 1}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Parsing.RomanNumeral.ParseFromX(ReadOnlySpan{char}, int, int, out int)" />.
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
                    ushort actualResult = Parsing.RomanNumeral.ParseFromX(span, startIndex, endIndex, out int actualNextIndex);
                    Assert.Equal(expectedResult, actualResult);
                    Assert.Equal(endIndex, actualNextIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromX)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        ushort actualResult = Parsing.RomanNumeral.ParseFromX(span, startIndex, source.Length, out int actualNextIndex);
                        Assert.Equal(expectedResult, actualResult);
                        Assert.Equal(endIndex, actualNextIndex);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromX)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {source.Length}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                    try
                        {
                            ushort actualResult = Parsing.RomanNumeral.ParseFromX(span, startIndex, endIndex + 1, out int actualNextIndex);
                            Assert.Equal(expectedResult, actualResult);
                            Assert.Equal(endIndex, actualNextIndex);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromX)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex + 1}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Parsing.RomanNumeral.ParseFromL(ReadOnlySpan{char}, int, int, out int)" />.
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
                    ushort actualResult = Parsing.RomanNumeral.ParseFromL(span, startIndex, endIndex, out int actualNextIndex);
                    Assert.Equal(expectedResult, actualResult);
                    Assert.Equal(endIndex, actualNextIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromL)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        ushort actualResult = Parsing.RomanNumeral.ParseFromL(span, startIndex, source.Length, out int actualNextIndex);
                        Assert.Equal(expectedResult, actualResult);
                        Assert.Equal(endIndex, actualNextIndex);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromL)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {source.Length}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                    try
                        {
                            ushort actualResult = Parsing.RomanNumeral.ParseFromL(span, startIndex, endIndex + 1, out int actualNextIndex);
                            Assert.Equal(expectedResult, actualResult);
                            Assert.Equal(endIndex, actualNextIndex);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromL)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex + 1}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Parsing.RomanNumeral.ParseFromC(ReadOnlySpan{char}, int, int, out int)" />.
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
                    ushort actualResult = Parsing.RomanNumeral.ParseFromC(span, startIndex, endIndex, out int actualNextIndex);
                    Assert.Equal(expectedResult, actualResult);
                    Assert.Equal(endIndex, actualNextIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromC)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        ushort actualResult = Parsing.RomanNumeral.ParseFromC(span, startIndex, source.Length, out int actualNextIndex);
                        Assert.Equal(expectedResult, actualResult);
                        Assert.Equal(endIndex, actualNextIndex);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromC)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {source.Length}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                    try
                        {
                            ushort actualResult = Parsing.RomanNumeral.ParseFromC(span, startIndex, endIndex + 1, out int actualNextIndex);
                            Assert.Equal(expectedResult, actualResult);
                            Assert.Equal(endIndex, actualNextIndex);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromC)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex + 1}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Parsing.RomanNumeral.ParseFromD(ReadOnlySpan{char}, int, int, out int)" />.
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
                    ushort actualResult = Parsing.RomanNumeral.ParseFromD(span, startIndex, endIndex, out int actualNextIndex);
                    Assert.Equal(expectedResult, actualResult);
                    Assert.Equal(endIndex, actualNextIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromD)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        ushort actualResult = Parsing.RomanNumeral.ParseFromD(span, startIndex, source.Length, out int actualNextIndex);
                        Assert.Equal(expectedResult, actualResult);
                        Assert.Equal(endIndex, actualNextIndex);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromD)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {source.Length}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                    try
                        {
                            ushort actualResult = Parsing.RomanNumeral.ParseFromD(span, startIndex, endIndex + 1, out int actualNextIndex);
                            Assert.Equal(expectedResult, actualResult);
                            Assert.Equal(endIndex, actualNextIndex);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromD)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex + 1}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Parsing.RomanNumeral.ParseFromM(ReadOnlySpan{char}, int, int, out int)" />.
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
                    ushort actualResult = Parsing.RomanNumeral.ParseFromM(span, startIndex, endIndex, out int actualNextIndex);
                    Assert.Equal(expectedResult, actualResult);
                    Assert.Equal(endIndex, actualNextIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromM)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                    throw;
                }
                int diff = source.Length - endIndex;
                if (diff > 0)
                {
                    try
                    {
                        ushort actualResult = Parsing.RomanNumeral.ParseFromM(span, startIndex, source.Length, out int actualNextIndex);
                        Assert.Equal(expectedResult, actualResult);
                        Assert.Equal(endIndex, actualNextIndex);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromM)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {source.Length}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                        throw;
                    }
                    if (diff > 1)
                        try
                        {
                            ushort actualResult = Parsing.RomanNumeral.ParseFromM(span, startIndex, endIndex + 1, out int actualNextIndex);
                            Assert.Equal(expectedResult, actualResult);
                            Assert.Equal(endIndex, actualNextIndex);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.ParseFromM)}(source: \"{source.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {endIndex + 1}, out int actualNextIndex); // expectedResult: {expectedResult}, expectedNextIndex: {endIndex}");
                            throw;
                        }
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Parsing.RomanNumeral.Parse(string)" /> and <see cref="Parsing.RomanNumeral.Parse(ReadOnlySpan{char})" />.
        /// </summary>
        [Fact]
        public void ParseTest()
        {
            foreach (string text in new[] { " ", " MMMCMXCIX", "MMMCMXCIX ", "XIXM", "XXIIX", "MMMMCMXCIX", "MMMDCCCLXXXVIIX"})
            {
                try { Assert.Throws<ArgumentException>("text", () => Parsing.RomanNumeral.Parse(text)); }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.Parse)}(text: \"{text.ToCsLikeCode()}\")");
                    throw;
                }
                try { Assert.Throws<ArgumentException>("text", () => Parsing.RomanNumeral.Parse(text.AsSpan())); }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.Parse)}(text: \"{text.ToCsLikeCode()}\".AsSpan())");
                    throw;
                }
            }
            foreach ((ushort expected, string text) in GetRomanNumeralTestValues(true))
            {
                try
                {
                    Parsing.RomanNumeral returnValue = Parsing.RomanNumeral.Parse(text);
                    Assert.Equal(text, returnValue.ToString());
                    Assert.Equal(expected, returnValue.Value);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.Parse)}(text: \"{text.ToCsLikeCode()}\")");
                    throw;
                }
                try
                {
                    Parsing.RomanNumeral returnValue = Parsing.RomanNumeral.Parse(text.AsSpan());
                    Assert.Equal(text, returnValue.ToString());
                    Assert.Equal(expected, returnValue.Value);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.Parse)}(text: \"{text.ToCsLikeCode()}\".AsSpan())");
                    throw;
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Parsing.RomanNumeral.TryParse(string, out Parsing.RomanNumeral)" /> and <see cref="Parsing.RomanNumeral.TryParse(string, out Parsing.RomanNumeral)" />.
        /// </summary>
        [Fact]
        public void TryParseTest()
        {
            foreach (string text in new[] { " ", " MMMCMXCIX", "MMMCMXCIX ", "XIXM", "XXIIX", "MMMMCMXCIX", "MMMDCCCLXXXVIIX"})
            {
                try
                {
                    bool returnValue = Parsing.RomanNumeral.TryParse(text, out Parsing.RomanNumeral result);
                    Assert.False(returnValue);
                    Assert.Equal(string.Empty, result.ToString());
                    Assert.Equal<ushort>(0, result.Value);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.TryParse)}(text: \"{text.ToCsLikeCode()}\", out Parsing.RomanNumeral result)");
                    throw;
                }
                try
                {
                    bool returnValue = Parsing.RomanNumeral.TryParse(text.AsSpan(), out Parsing.RomanNumeral result);
                    Assert.False(returnValue);
                    Assert.Equal(string.Empty, result.ToString());
                    Assert.Equal<ushort>(0, result.Value);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.TryParse)}(text: \"{text.ToCsLikeCode()}\".AsSpan(), out Parsing.RomanNumeral result)");
                    throw;
                }
            }
            foreach ((ushort expected, string text) in GetRomanNumeralTestValues(true))
            {
                try
                {
                    bool returnValue = Parsing.RomanNumeral.TryParse(text, out Parsing.RomanNumeral result);
                    Assert.True(returnValue);
                    Assert.Equal(text, result.ToString());
                    Assert.Equal(expected, result.Value);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.TryParse)}(text: \"{text.ToCsLikeCode()}\", out Parsing.RomanNumeral result)");
                    throw;
                }
                try
                {
                    bool returnValue = Parsing.RomanNumeral.TryParse(text.AsSpan(), out Parsing.RomanNumeral result);
                    Assert.True(returnValue);
                    Assert.Equal(text, result.ToString());
                    Assert.Equal(expected, result.Value);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.TryParse)}(text: \"{text.ToCsLikeCode()}\".AsSpan(), out Parsing.RomanNumeral result)");
                    throw;
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Parsing.RomanNumeral.TryParse(Parsing.ParsingSource, int, int, out Parsing.RomanNumeral, out int)" />.
        /// </summary>
        [Fact]
        public void MatcherTryParseTest()
        {
            RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
            RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n", "II", "V");
            using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
            foreach ((string text, int startIndex, int count, int altCount1, int altCount2, string expectedResult, ushort expectedValue, int expectedNextIndex, bool expectedReturnValue) in leadingTextValues.GetValues()
                .SelectMany(lt => trailingTextValues.GetValues().Select(tt => (lt + tt, lt.Length,
                    (tt.Length > 0) ? tt switch { "II" or "V" => lt.Length, _ =>  1 } : 0,
                    (tt.Length > 0) ? tt switch { "II" or "V" => lt.Length, _ =>  tt.Length } : 0)))
                .Select(a => (a.Item1, a.Length, 0, a.Item3, a.Item4, string.Empty, (ushort)0, a.Length, false)).Concat(GetRomanNumeralTestValues().Select((a, i) =>
            {
                enumerator.MoveNext();
                string r = (i % 1 == 0) ? a.Text : a.Text.ToLower();
                (string leadingText, string trailingText) = enumerator.Current;
                return (leadingText + r + trailingText, leadingText.Length, r.Length,
                    (trailingText.Length > 0) ? trailingText switch { "II" or "V" => r.Length, _ =>  r.Length  + 1 } : r.Length,
                    (trailingText.Length > 0) ? trailingText switch { "II" or "V" => r.Length, _ =>  r.Length + trailingText.Length } : r.Length,
                    Result: r, a.Value, leadingText.Length + r.Length, true);
            })))
            {
                Parsing.ParsingSource source = new(text);
                try
                {
                    bool actualReturnValue = Parsing.RomanNumeral.TryParse(source, startIndex, count, out Parsing.RomanNumeral actualResult, out int actualNextIndex);
                    Assert.Equal(expectedReturnValue, actualReturnValue);
                    string s = actualResult.ToString();
                    Assert.NotNull(s);
                    Assert.Equal(expectedResult, s);
                    Assert.Equal(expectedValue, actualResult.Value);
                    Assert.Equal(expectedNextIndex, actualNextIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.Matcher.TryParse)}(target: \"{text.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {count}, out Parsing.RomanNumeral actualResult, out int actualEndIndex); // expectedReturnValue: {expectedReturnValue}, expectedResult: {{{expectedResult}}}, expectedNextIndex: {expectedNextIndex}");
                    throw;
                }
                if (altCount1 > count)
                {
                    try
                    {
                        bool actualReturnValue = Parsing.RomanNumeral.TryParse(source, startIndex, altCount1, out Parsing.RomanNumeral actualResult, out int actualNextIndex);
                        Assert.Equal(expectedReturnValue, actualReturnValue);
                        string s = actualResult.ToString();
                        Assert.NotNull(s);
                        Assert.Equal(expectedResult, s);
                        Assert.Equal(expectedValue, actualResult.Value);
                        Assert.Equal(expectedNextIndex, actualNextIndex);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.Matcher.TryParse)}(target: \"{text.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {altCount1}, out Parsing.RomanNumeral actualResult, out int actualEndIndex); // expectedReturnValue: {expectedReturnValue}, expectedResult: {{{expectedResult}}}, expectedNextIndex: {expectedNextIndex}");
                        throw;
                    }
                    if (altCount2 > altCount1)
                    {
                        try
                        {
                            bool actualReturnValue = Parsing.RomanNumeral.TryParse(source, startIndex, altCount2, out Parsing.RomanNumeral actualResult, out int actualNextIndex);
                            Assert.Equal(expectedReturnValue, actualReturnValue);
                            string s = actualResult.ToString();
                            Assert.NotNull(s);
                            Assert.Equal(expectedResult, s);
                            Assert.Equal(expectedValue, actualResult.Value);
                            Assert.Equal(expectedNextIndex, actualNextIndex);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.Matcher.TryParse)}(target: \"{text.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {altCount2}, out Parsing.RomanNumeral actualResult, out int actualEndIndex); // expectedReturnValue: {expectedReturnValue}, expectedResult: {{{expectedResult}}}, expectedNextIndex: {expectedNextIndex}");
                            throw;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Unit test for <see cref="Parsing.RomanNumeral.Matcher.Match(Parsing.ParsingSource, int, int, out int)" />.
        /// </summary>
        [Fact]
        public void MatcherMatchTest()
        {
            RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
            RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n", "II", "V");
            using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
            foreach ((string text, int startIndex, int count, int altCount1, int altCount2, int expectedNextIndex, bool expectedReturnValue) in leadingTextValues.GetValues()
                .SelectMany(lt => trailingTextValues.GetValues().Select(tt => (lt + tt, lt.Length,
                    (tt.Length > 0) ? tt switch { "II" or "V" => lt.Length, _ =>  1 } : 0,
                    (tt.Length > 0) ? tt switch { "II" or "V" => lt.Length, _ =>  tt.Length } : 0)))
                .Select(a => (a.Item1, a.Length, a.Length, a.Item3, a.Item4, a.Length, false)).Concat(GetRomanNumeralTestValues().Select((a, i) =>
            {
                enumerator.MoveNext();
                string r = (i % 1 == 0) ? a.Text : a.Text.ToLower();
                (string leadingText, string trailingText) = enumerator.Current;
                return (leadingText + r + trailingText, leadingText.Length, r.Length,
                    (trailingText.Length > 0) ? trailingText switch { "II" or "V" => r.Length, _ =>  r.Length  + 1 } : r.Length,
                    (trailingText.Length > 0) ? trailingText switch { "II" or "V" => r.Length, _ =>  r.Length + trailingText.Length } : r.Length,
                    leadingText.Length + r.Length, true);
            })))
            {
                Parsing.ParsingSource source = new(text);
                try
                {
                    bool actualReturnValue = Parsing.RomanNumeral.Matcher.Instance.Match(source, startIndex, count, out int actualNextIndex);
                    Assert.Equal(expectedReturnValue, actualReturnValue);
                    Assert.Equal(expectedNextIndex, actualNextIndex);
                }
                catch
                {
                    _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.Matcher.Match)}(target: \"{text.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {count}, out int actualEndIndex); // expectedReturnValue: {expectedReturnValue}, expectedNextIndex: {expectedNextIndex}");
                    throw;
                }
                if (altCount1 > count)
                {
                    try
                    {
                        bool actualReturnValue = Parsing.RomanNumeral.Matcher.Instance.Match(source, startIndex, altCount1, out int actualNextIndex);
                        Assert.Equal(expectedReturnValue, actualReturnValue);
                        Assert.Equal(expectedNextIndex, actualNextIndex);
                    }
                    catch
                    {
                        _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.Matcher.Match)}(target: \"{text.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {altCount1}, out int actualEndIndex); // expectedReturnValue: {expectedReturnValue}, expectedNextIndex: {expectedNextIndex}");
                        throw;
                    }
                    if (altCount2 > altCount1)
                    {
                        try
                        {
                            bool actualReturnValue = Parsing.RomanNumeral.Matcher.Instance.Match(source, startIndex, altCount2, out int actualNextIndex);
                            Assert.Equal(expectedReturnValue, actualReturnValue);
                            Assert.Equal(expectedNextIndex, actualNextIndex);
                        }
                        catch
                        {
                            _output.WriteLine($"Failing {nameof(Parsing.RomanNumeral.Matcher.Match)}(target: \"{text.ToCsLikeCode()}\".AsSpan(), startIndex: {startIndex}, endIndex: {altCount2}, out int actualEndIndex); // expectedReturnValue: {expectedReturnValue}, expectedNextIndex: {expectedNextIndex}");
                            throw;
                        }
                    }
                }
            }
        }
    }
}