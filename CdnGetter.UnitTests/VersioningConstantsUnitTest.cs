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

        // public class ToRomanNumeralTestData : TheoryData<ushort, string>
        // {
        //     public ToRomanNumeralTestData()
        //     {
        //         Add(0, string.Empty);
        //         foreach ((ushort value, string text) in GetRomanNumeralTestValues())
        //             Add(value, text);
        //     }
        // }

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
        
        // public class ParseRomanNumeralTestData : TheoryData<int, string, ushort, int>
        // {
        //     public ParseRomanNumeralTestData()
        //     {
        //         RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
        //         RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
        //         foreach (string leadingText in leadingTextValues.GetValues())
        //         {
        //             if (leadingText.Length > 0)
        //                 foreach (string trailingText in trailingTextValues.GetValues())
        //                     if (trailingText.Length > 0)
        //                         Add(leadingText.Length, leadingText + trailingText, 0, leadingText.Length);
        //                     else
        //                         Add(leadingText.Length, leadingText, 0, leadingText.Length);
        //             else
        //                 foreach (string trailingText in trailingTextValues.GetValues())
        //                     Add(0, trailingText, 0, 0);
        //         }
        //         using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
        //         bool lc = true;
        //         foreach ((ushort value, string text) in GetRomanNumeralTestValues())
        //         {
        //             enumerator.MoveNext();
        //             lc = !lc;
        //             (string leadingText, string trailingText) = enumerator.Current;
        //             if (leadingText.Length == 0)
        //             {
        //                 if (trailingText.Length > 0)
        //                     Add(0, (lc ? text.ToLower() : text) + trailingText, value, text.Length);
        //                 else
        //                     Add(0, lc ? text.ToLower() : text, value, text.Length);
        //             }
        //             else if (trailingText.Length > 0)
        //                 Add(leadingText.Length, leadingText + (lc ? text.ToLower() : text) + trailingText, value, leadingText.Length + text.Length);
        //             else
        //                 Add(leadingText.Length, leadingText + (lc ? text.ToLower() : text), value, leadingText.Length + text.Length);
        //         }
        //     }
        // }

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
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseRomanNumeral)}(target: new ReadOnlySpan<char> (\"{target.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {expectedEndIndex}, out int actualEndIndex); // expectedResult: {expectedResult}, expectedEndIndex: {expectedEndIndex}");
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
                        _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseRomanNumeral)}(target: new ReadOnlySpan<char> (\"{target.ToCsLikeCode()}\"), out int actualEndIndex); // expectedResult: {expectedResult}, expectedEndIndex: {expectedEndIndex}");
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
                    _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseRomanNumeral)}(target: new ReadOnlySpan<char> (\"{target.ToCsLikeCode()}\"), startIndex: {startIndex} out int actualEndIndex); // expectedResult: {expectedResult}, expectedEndIndex: {expectedEndIndex}");
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
                        _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseRomanNumeral)}(target: new ReadOnlySpan<char> (\"{target.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {target.Length}, out int actualEndIndex); // expectedResult: {expectedResult}, expectedEndIndex: {expectedEndIndex}");
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
                            _output.WriteLine($"Failing {nameof(Versioning.VersioningConstants.ParseRomanNumeral)}(target: new ReadOnlySpan<char> (\"{target.ToCsLikeCode()}\"), startIndex: {startIndex}, endIndex: {expectedEndIndex + 1}, out int actualEndIndex); // expectedResult: {expectedResult}, expectedEndIndex: {expectedEndIndex}");
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