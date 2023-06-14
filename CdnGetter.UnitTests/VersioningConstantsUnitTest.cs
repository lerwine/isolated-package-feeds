using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CdnGetter.UnitTests
{
    public class VersioningConstantsUnitTest
    {
        private static IEnumerable<(ushort Value, string Text)> GetRomanNumeralTestValues()
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
        public class ToRomanNumeralTestData : TheoryData<ushort, string>
        {
            public ToRomanNumeralTestData()
            {
                Add(0, string.Empty);
                foreach ((ushort value, string text) in GetRomanNumeralTestValues())
                    Add(value, text);
            }
        }

        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.ToRomanNumeral(ushort)" />.
        /// </summary>
        [Theory]
        [ClassData(typeof(ToRomanNumeralTestData))]
        public void ToRomanNumeralTest(ushort value, string expected)
        {
            string actual = Versioning.VersioningConstants.ToRomanNumeral(value);
            Assert.Equal(expected, actual);
        }
        
        public class ParseRomanNumeralTestData : TheoryData<int, string, ushort, int>
        {
            public ParseRomanNumeralTestData()
            {
                RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
                RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
                foreach (string leadingText in leadingTextValues.GetValues())
                {
                    if (leadingText.Length > 0)
                        foreach (string trailingText in trailingTextValues.GetValues())
                            if (trailingText.Length > 0)
                                Add(leadingText.Length, leadingText + trailingText, 0, leadingText.Length);
                            else
                                Add(leadingText.Length, leadingText, 0, leadingText.Length);
                    else
                        foreach (string trailingText in trailingTextValues.GetValues())
                            Add(0, trailingText, 0, 0);
                }
                using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
                bool lc = true;
                foreach ((ushort value, string text) in GetRomanNumeralTestValues())
                {
                    enumerator.MoveNext();
                    lc = !lc;
                    (string leadingText, string trailingText) = enumerator.Current;
                    if (leadingText.Length == 0)
                    {
                        if (trailingText.Length > 0)
                            Add(0, (lc ? text.ToLower() : text) + trailingText, value, text.Length);
                        else
                            Add(0, lc ? text.ToLower() : text, value, text.Length);
                    }
                    else if (trailingText.Length > 0)
                        Add(leadingText.Length, leadingText + (lc ? text.ToLower() : text) + trailingText, value, leadingText.Length + text.Length);
                    else
                        Add(leadingText.Length, leadingText + (lc ? text.ToLower() : text), value, leadingText.Length + text.Length);
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.ParseRomanNumeral(int, ReadOnlySpan{char}, out int)" />.
        /// </summary>
        [Theory]
        [ClassData(typeof(ParseRomanNumeralTestData))]
        public void ParseRomanNumeralTest(int startIndex, string target, ushort expectedResult, int expectedEndIndex)
        {
            ushort actualResult = Versioning.VersioningConstants.ParseRomanNumeral(startIndex, target.AsSpan(), out int actualEndIndex);
            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(expectedEndIndex, actualEndIndex);
        }

        public class TestRomanNumeralTestData : TheoryData<int, string, bool, int>
        {
            public TestRomanNumeralTestData()
            {
                RotatingEnumerable<string> leadingTextValues = new(string.Empty, "M", " .", "x", "ab", "5", " -");
                RotatingEnumerable<string> trailingTextValues = new(string.Empty, "z", " ", "1", "-+", "\r\n");
                foreach (string leadingText in leadingTextValues.GetValues())
                {
                    if (leadingText.Length > 0)
                        foreach (string trailingText in trailingTextValues.GetValues())
                            if (trailingText.Length > 0)
                                Add(leadingText.Length, leadingText + trailingText, false, leadingText.Length);
                            else
                                Add(leadingText.Length, leadingText, false, leadingText.Length);
                    else
                        foreach (string trailingText in trailingTextValues.GetValues())
                            Add(0, trailingText, false, 0);
                }
                using IEnumerator<(string LeadingText, string TrailingText)> enumerator = leadingTextValues.SelectMany(l => trailingTextValues.Select(t => (l, t))).GetEnumerator();
                bool lc = true;
                foreach (string text in GetRomanNumeralTestValues().Select(a => a.Text))
                {
                    enumerator.MoveNext();
                    lc = !lc;
                    (string leadingText, string trailingText) = enumerator.Current;
                    if (leadingText.Length == 0)
                    {
                        if (trailingText.Length > 0)
                            Add(0, (lc ? text.ToLower() : text) + trailingText, true, text.Length);
                        else
                            Add(0, lc ? text.ToLower() : text, true, text.Length);
                    }
                    else if (trailingText.Length > 0)
                        Add(leadingText.Length, leadingText + (lc ? text.ToLower() : text) + trailingText, true, leadingText.Length + text.Length);
                    else
                        Add(leadingText.Length, leadingText + (lc ? text.ToLower() : text), true, leadingText.Length + text.Length);
                }
            }
        }

        /// <summary>
        /// Unit test for <see cref="Versioning.VersioningConstants.TestRomanNumeral(int, ReadOnlySpan{char}, out int)" />.
        /// </summary>
        [Theory]
        [ClassData(typeof(TestRomanNumeralTestData))]
        public void TestRomanNumeralTest(int startIndex, string target, bool expectedResult, int expectedEndIndex)
        {
            bool actualResult = Versioning.VersioningConstants.TestRomanNumeral(startIndex, target.AsSpan(), out int actualEndIndex);
            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(expectedEndIndex, actualEndIndex);
        }
    }
}