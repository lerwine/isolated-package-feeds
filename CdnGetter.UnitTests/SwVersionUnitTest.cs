using System.Text;

namespace CdnGetter.UnitTests;

public class SwVersionUnitTest
{
    /// <summary>
    /// Generates test data for <see cref="PreReleaseSegmentConstructor1Test(bool, string?, string, string)" />.
    /// </summary>
    public class PreReleaseSegmentConstructor1TestData : TheoryData<bool, string?, string, string>
    {
        public PreReleaseSegmentConstructor1TestData()
        {
            Add(false, null, string.Empty, "-");
            Add(true, null, string.Empty, ".");
            Add(false, string.Empty, string.Empty, "-");
            Add(true, string.Empty, string.Empty, ".");
            foreach (string s in new string[] { "r" })
            {
                Add(false, s, s, $"-{s}");
                Add(true, s, s, $".{s}");
            }
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion.PreReleaseSegment.PreReleaseSegment(bool, string)" /> that will not throw an excePATCHaion.
    /// </summary>
    [Theory]
    [ClassData(typeof(PreReleaseSegmentConstructor1TestData))]
    public void PreReleaseSegmentConstructor1Test(bool altSeprator, string? value, string expectedValue, string expectedToString)
    {
        SwVersion.PreReleaseSegment target = new(altSeprator, value!);
        Assert.Equal(altSeprator, target.AltSeparator);
        Assert.Equal(expectedValue, target.Value);
        string actualToString = target.ToString();
        Assert.Equal(expectedToString, actualToString);
    }

    /// <summary>
    /// Generates test data for <see cref="PreReleaseSegmentConstructor2Test(bool, string)" />.
    /// </summary>
    public class PreReleaseSegmentConstructor2TestData : TheoryData<bool, string>
    {
        public PreReleaseSegmentConstructor2TestData()
        {
            foreach (string s in new string[] {
                "-",  ".", "+",
                " - ",  " . ", " + ",
                "r- ",  "r. ", "r+ ",
                " -v",  " .v", " +v" })
            {
                Add(false, s);
                Add(true, s);
            }
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion.PreReleaseSegment.PreReleaseSegment(bool, string)" /> that will throw an excePATCHaion.
    /// </summary>
    [Theory]
    [ClassData(typeof(PreReleaseSegmentConstructor2TestData))]
    public void PreReleaseSegmentConstructor2Test(bool altSeprator, string value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(nameof(value), () => new SwVersion.PreReleaseSegment(altSeprator, value));
    }

    public record VersionValues(string? Prefix, int Major, int? Minor, int? Patch, int? Revision, int[]? AdditionalNumerical, SwVersion.PreReleaseSegment[]? PreRelease, SwVersion.BuildSegment[]? Build,
        SwVersion.VersionStringFormat Format)
    {
        internal void AssertEquals(SwVersion target)
        {
            Assert.Equal(Prefix, target.Prefix);
            Assert.Equal(Major, target.Major);
            Assert.Equal(Minor, target.Minor);
            Assert.Equal(Patch, target.Patch);
            Assert.Equal(Revision, target.Revision);
            if (AdditionalNumerical is null)
                Assert.Null(target.AdditionalNumerical);
            else
            {
                Assert.NotNull(target.AdditionalNumerical);
                Assert.Equal(AdditionalNumerical, target.AdditionalNumerical);
            }
            if (PreRelease is null)
                Assert.Null(target.PreRelease);
            else
            {
                Assert.NotNull(target.PreRelease);
                Assert.Equal(PreRelease, target.PreRelease);
            }
            if (Build is null)
                Assert.Null(target.Build);
            else
            {
                Assert.NotNull(target.Build);
                Assert.Equal(Build, target.Build);
            }
            Assert.Equal(Format, target.Format);
        }
    }
    
    /// <summary>
    /// Generates test data for <see cref="ParsingConstructorTest(string?, VersionValues)" />
    /// </summary>
    public class ParsingConstructorTestData : TheoryData<string?, VersionValues>
    {
        public ParsingConstructorTestData()
        {
            Add(null,
                new(null,   0,              null,   null,   null,   null,
                null, null, SwVersion.VersionStringFormat.NonNumerical));
            Add("",
                new(null,   0,              null,   null,   null,   null,
                null, null, SwVersion.VersionStringFormat.NonNumerical));
            Add(" ",
                new(null,   0,              null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.NonNumerical));
            Add("\t",
                new(null,   0,              null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.NonNumerical));
            Add("\n",
                new(null,   0,              null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.NonNumerical));
            Add("null",
                new("null", 0,              null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.NonNumerical));
            Add("0",
                new(null,   0,              null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.Standard));
            Add("-0",
                new("-",    0,              null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.Standard));
            Add("1",
                new(null,   1,              null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.Standard));
            Add(" 1 ",
                new(null,   1,              null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.Standard));
            Add("2147483647",
                new(null,   2147483647,     null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.Standard));
            Add("-2147483648",
                new(null,   -2147483648,    null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.Standard));
            Add("-1",
                new(null,   -1,             null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.Standard));
            Add("1.0",
                new(null,   1,              0,      null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.Standard));
            Add("1.0.0",
                new(null,   1,              0,      0,      null,   null, 
                null, null, SwVersion.VersionStringFormat.Standard));
            Add("1.0.0.0",
                new(null,   1,              0,      0,      0,      null, 
                null,   null, SwVersion.VersionStringFormat.Standard));
            Add("1.0.0.0.0",
                new(null,   1,              0,      0,      0,      new int [] { 0 }, 
                null,   null, SwVersion.VersionStringFormat.Standard));
            Add("2.1.1-rc1",
                new(null,   2,              1,      1,      null,   null,
                new SwVersion.PreReleaseSegment[] { new(false, "rc1") }, null, SwVersion.VersionStringFormat.Standard));
            Add("18.0.0-alpha-34308b5ad-20210729",
                new(null,   18,             0,      0,      null,   null,
                new SwVersion.PreReleaseSegment[] { new(false, "alpha"), new(false, "34308b5ad"), new(false, "20210729") }, null, SwVersion.VersionStringFormat.Standard));
            Add("2.7-beta.2",
                new(null,   2,              7,      null,   null,   null,
                new SwVersion.PreReleaseSegment[] { new(false, "beta"), new(true, "2") }, null, SwVersion.VersionStringFormat.Standard));
            Add("1.0.0rc10",
                new(null,   1,              0,      0,      null,   null,
                new SwVersion.PreReleaseSegment[] { new(true, "rc10") }, null, SwVersion.VersionStringFormat.Alt));
            Add("1-pre",
                new(null,   1,              null,   null,   null,   null,
                new SwVersion.PreReleaseSegment[] { new(false, "pre") }, null, SwVersion.VersionStringFormat.Standard));
            Add("r45",
                new("r",    45,             null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.Alt));
            Add("Utah",
                new("Utah", 0,              null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.NonNumerical));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string?)" />
    /// </summary>
    [Theory]
    [ClassData(typeof(ParsingConstructorTestData))]
    public void ParsingConstructorTest(string? versionString, VersionValues expected)
    {
        SwVersion target = new(versionString);
        expected.AssertEquals(target);
    }

    [Fact]
    public void ComponentConstructor0Test()
    {
        SwVersion result = new();
        Assert.Null(result.Prefix);
        Assert.Equal(0, result.Major);
        Assert.Null(result.Minor);
        Assert.Null(result.Patch);
        Assert.Null(result.Revision);
        Assert.Null(result.AdditionalNumerical);
        Assert.Null(result.PreRelease);
        Assert.Null(result.Build);
    }
    
    public static readonly Random _random = new();

    private static IEnumerable<int> GetMajorValues()
    {
        yield return int.MinValue;
        yield return _random.Next(int.MinValue + 1, -1);
        yield return -1;
        yield return 0;
        yield return 1;
        yield return _random.Next(2, int.MaxValue - 1);
        yield return int.MaxValue;
    }
    
    private static int GetRandomInt(int minValue, int maxValue, params int[] omit)
    {
        int result = _random.Next(minValue, maxValue);
        if (omit is not null && omit.Length > 0)
            while (omit.Contains(result))
                result = _random.Next(minValue, maxValue);
        return result;
    }
    private static IEnumerable<int> GetMinorValues(out IEnumerable<int> majorValues)
    {
        int sharedMajor = _random.Next(2, int.MaxValue - 1);
        int uniqueMajor = GetRandomInt(2, int.MaxValue - 1, sharedMajor);
        int uniqueMinor = GetRandomInt(2, int.MaxValue - 1, sharedMajor, uniqueMajor);
        majorValues = (new int[] { int.MinValue, _random.Next(int.MinValue + 1, -1), -1, 0, 1, sharedMajor, uniqueMajor, int.MaxValue }).OrderBy(i => i);
        return (new int[] { 0, 1, sharedMajor, uniqueMinor, int.MaxValue }).OrderBy(i => i);
    }
    
    private static IEnumerable<int> GetPatchValues(out IEnumerable<int> majorValues, out IEnumerable<int> minorValues)
    {
        int sharedMajor = _random.Next(2, int.MaxValue - 1);
        int uniqueMajor = GetRandomInt(2, int.MaxValue - 1, sharedMajor);
        int sharedMinor = GetRandomInt(2, int.MaxValue - 1, sharedMajor, uniqueMajor);
        int uniqueMinor = GetRandomInt(2, int.MaxValue - 1, sharedMajor, uniqueMajor, sharedMinor);
        int uniquePatch = GetRandomInt(2, int.MaxValue - 1, sharedMajor, uniqueMajor, sharedMinor, uniqueMinor);
        majorValues = (new int[] { int.MinValue, _random.Next(int.MinValue + 1, -1), -1, 0, 1, sharedMajor, uniqueMajor, int.MaxValue }).OrderBy(i => i);
        minorValues = (new int[] { 0, 1, sharedMajor, sharedMinor, uniqueMinor, int.MaxValue }).OrderBy(i => i);
        return (new int[] { 0, 1, sharedMajor, sharedMinor, uniquePatch, int.MaxValue }).OrderBy(i => i);
    }
    
    private static IEnumerable<IEnumerable<int>> GetAdditionalNumericValues(out IEnumerable<int> majorValues, out IEnumerable<int> minorValues, out IEnumerable<int> patchValues,
        out IEnumerable<int> revisionValues)
    {
        int sharedMajor = _random.Next(2, int.MaxValue - 1);
        int uniqueMajor = GetRandomInt(2, int.MaxValue - 1, sharedMajor);
        int sharedMinor = GetRandomInt(2, int.MaxValue - 1, sharedMajor, uniqueMajor);
        int uniqueMinor = GetRandomInt(2, int.MaxValue - 1, sharedMajor, uniqueMajor, sharedMinor);
        int sharedPatch = GetRandomInt(2, int.MaxValue - 1, sharedMajor, uniqueMajor, sharedMinor, uniqueMinor);
        int uniquePatch = GetRandomInt(2, int.MaxValue - 1, sharedMajor, uniqueMajor, sharedMinor, uniqueMinor, sharedPatch);
        int sharedRevision = GetRandomInt(2, int.MaxValue - 1, sharedMajor, uniqueMajor, sharedMinor, uniqueMinor, sharedPatch, uniquePatch);
        int uniqueRevision = GetRandomInt(2, int.MaxValue - 1, sharedMajor, uniqueMajor, sharedMinor, uniqueMinor, sharedPatch, uniquePatch, sharedRevision);
        int uniqueAddl = GetRandomInt(2, int.MaxValue - 1, sharedMajor, uniqueMajor, sharedMinor, uniqueMinor, sharedPatch, uniquePatch, sharedRevision, uniqueRevision);
        majorValues = (new int[] { int.MinValue, _random.Next(int.MinValue + 1, -1), -1, 0, 1, sharedMajor, uniqueMajor, int.MaxValue }).OrderBy(i => i);
        minorValues = (new int[] { 0, 1, sharedMajor, sharedMinor, uniqueMinor, int.MaxValue }).OrderBy(i => i);
        patchValues = (new int[] { 0, 1, sharedMajor, sharedMinor, sharedPatch, uniquePatch, int.MaxValue }).OrderBy(i => i);
        revisionValues = (new int[] { 0, 1, sharedMajor, sharedMinor, sharedPatch, sharedRevision, uniqueRevision, int.MaxValue }).OrderBy(i => i);
        return (new int[] { sharedMajor, sharedMinor, sharedPatch, sharedRevision, uniqueAddl }).Select(i => new int[] { i })
            .Concat((new int[] { sharedMajor, sharedMinor, sharedPatch, sharedRevision, uniqueAddl }).Select(i => new int[] { i, uniqueAddl }))
            .Concat((new int[] { sharedMajor, sharedMinor, sharedPatch, sharedRevision, uniqueAddl }).Select(i => new int[] { uniqueAddl, i }))
            .Concat(new int[][] { new int[] { uniqueAddl, sharedMajor, sharedMinor, sharedPatch, sharedRevision } });
    }


    private static readonly char[] _letterChars = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    private static readonly char[] _numericChars =  new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    private static readonly char[] _alNumChars = _numericChars.Concat(_letterChars).ToArray();
    private static readonly char[] _allCharsSource = Enumerable.Range(33, 95).Select(i => (char)i).Concat(_alNumChars).Concat(_alNumChars).Concat(_alNumChars).Concat(_alNumChars).ToArray();
    private static readonly char[] _nonNumericChars = Enumerable.Range(33, 95).Select(i => (char)i).Where(c => !char.IsNumber(c)).ToArray();
    private static readonly char[] _nonNumericCharsSource = _allCharsSource.Where(c => !char.IsNumber(c)).ToArray();
    private static readonly char[] _buildSeparatorChars = new char[] { '+', '.', '-' };
    private static readonly char[] _preReleaseSeparatorChars = new char[] { '.', '-' };
    private static readonly char[] _validBuildChars = Enumerable.Range(33, 95).Select(i => (char)i).Where(c => !_buildSeparatorChars.Contains(c)).ToArray();
    private static readonly char[] _buildCharsSource = _allCharsSource.Where(c => !_buildSeparatorChars.Contains(c)).ToArray();
    private static readonly char[] _validPrefixChars = _nonNumericChars.Where(c => !_buildSeparatorChars.Contains(c)).ToArray();
    private static readonly char[] _prefixCharsSource = _nonNumericCharsSource.Where(c => !_buildSeparatorChars.Contains(c)).ToArray();
    
    private static readonly StringComparer _comparer = StringComparer.OrdinalIgnoreCase;
    private static string GetRandomString(char[] source, int minLength, int maxLength, params string[] omit)
    {
        if (maxLength < 1)
            return string.Empty;
        int length;
        if (minLength > maxLength)
            length = (maxLength < 0) ? 0 : maxLength;
        else if (maxLength < 1)
            length = 0;
        else if (maxLength == 1)
            length = (minLength > 0) ? 1 : _random.Next(0, 2);
        else if (maxLength == minLength)
            length = maxLength;
        else
            length = _random.Next((minLength < 0) ? 0 : minLength, maxLength);
        int loopCheck;
        string result;
                int count;
        switch (length)
        {
            case 0:
                return string.Empty;
            case 1:
                result = new String(source[_random.Next(0, source.Length)], 1);
                if (omit == null || omit.Length == 0 || !omit.Contains(result, _comparer))
                    return result;
                loopCheck = source.Length * 4;
                count = 0;
                do
                {
                    if (++count > loopCheck)
                        break;
                    result = new String(source[_random.Next(0, source.Length)], 1);
                }
                while (omit.Contains(result, _comparer));
                return result;
            default:
                StringBuilder sb = new StringBuilder().Append(source[_random.Next(0, source.Length)]);
                for (int i = 1; i < length; i++)
                    sb.Append(source[_random.Next(0, source.Length)]);
                result = sb.ToString();
                if (omit == null || omit.Length == 0 || !omit.Contains(result, _comparer))
                    return result;
                loopCheck = source.Length * 4;
                count = 0;
                do
                {
                    if (++count > loopCheck)
                        break;
                    sb = new StringBuilder().Append(source[_random.Next(0, source.Length)]);
                    for (int i = 1; i < length; i++)
                        sb.Append(source[_random.Next(0, source.Length)]);
                    result = sb.ToString();
                }
                while (omit.Contains(result, _comparer));
                return result;
        }
    }
    
    private static IEnumerable<SwVersion.PreReleaseSegment[]> GetPreReleaseSegments(string shared1, string shared2, string unique1, string unique2, string[] otherShared, params string[] otherUnique)
    {
        foreach (string s in otherShared)
            yield return new SwVersion.PreReleaseSegment[] { new SwVersion.PreReleaseSegment(false, s) };
        string[] omit = otherShared.Concat(otherUnique).Concat(new string[] { shared1, shared2, unique1, unique2 }).ToArray();
        yield return new SwVersion.PreReleaseSegment[] { new SwVersion.PreReleaseSegment(false, shared1) };
        yield return new SwVersion.PreReleaseSegment[] { new SwVersion.PreReleaseSegment(true, shared2) };
        yield return new SwVersion.PreReleaseSegment[] { new SwVersion.PreReleaseSegment(true, unique1) };
        yield return new SwVersion.PreReleaseSegment[] { new SwVersion.PreReleaseSegment(true, unique2) };
        foreach (string s in otherShared)
            yield return new SwVersion.PreReleaseSegment[]
            {
                new SwVersion.PreReleaseSegment(false, s),
                new SwVersion.PreReleaseSegment(false, GetRandomString(_prefixCharsSource, 2, 8, omit))
            };
        yield return new SwVersion.PreReleaseSegment[]
        {
            new SwVersion.PreReleaseSegment(false, shared1),
            new SwVersion.PreReleaseSegment(false, GetRandomString(_prefixCharsSource, 2, 8, omit))
        };
        yield return new SwVersion.PreReleaseSegment[]
        {
            new SwVersion.PreReleaseSegment(true, shared2),
            new SwVersion.PreReleaseSegment(false, GetRandomString(_prefixCharsSource, 2, 8,  omit))
        };
        yield return new SwVersion.PreReleaseSegment[]
        {
            new SwVersion.PreReleaseSegment(true, unique1),
            new SwVersion.PreReleaseSegment(false, GetRandomString(_prefixCharsSource, 2, 8,  omit))
        };
        yield return new SwVersion.PreReleaseSegment[]
        {
            new SwVersion.PreReleaseSegment(true, unique2),
            new SwVersion.PreReleaseSegment(false, GetRandomString(_prefixCharsSource, 2, 8,  omit))
        };
        foreach (string s in otherShared)
            yield return new SwVersion.PreReleaseSegment[]
            {
                new SwVersion.PreReleaseSegment(false, s),
                new SwVersion.PreReleaseSegment(false, shared1), new SwVersion.PreReleaseSegment(false, shared2)
            };
    }
    
    public static IEnumerable<SwVersion.PreReleaseSegment> GetPreReleaseSegments(string shared1, string shared2, string unique1, string unique2)
    {
        yield return new SwVersion.PreReleaseSegment(false, shared1);
        if (unique2.Length > 0)
        {
            char c = shared1[0];
            if (!(char.IsDigit(c) || _buildSeparatorChars.Contains(c)))
                yield return new SwVersion.PreReleaseSegment(true, shared1);
        }
        yield return new SwVersion.PreReleaseSegment(false, shared2);
        if (unique2.Length > 0)
        {
            char c = shared2[0];
            if (!(char.IsDigit(c) || _buildSeparatorChars.Contains(c)))
                yield return new SwVersion.PreReleaseSegment(true, shared2);
        }
        yield return new SwVersion.PreReleaseSegment(false, unique1);
        if (unique2.Length > 0)
        {
            char c = unique1[0];
            if (!(char.IsDigit(c) || _buildSeparatorChars.Contains(c)))
                yield return new SwVersion.PreReleaseSegment(true, unique1);
        }
        yield return new SwVersion.PreReleaseSegment(false, unique2);
        if (unique2.Length > 0)
        {
            char c = unique2[0];
            if (!(char.IsDigit(c) || _buildSeparatorChars.Contains(c)))
                yield return new SwVersion.PreReleaseSegment(true, unique2);
        }
    }
    
    public static IEnumerable<SwVersion.PreReleaseSegment[]> GetAdditonalPreReleaseSegments(string shared1, string shared2, string unique1, string unique2, string[] otherShared, params string[] otherUnique)
    {
        yield return new  SwVersion.PreReleaseSegment[] { new SwVersion.PreReleaseSegment(false, shared1) };
        if (unique2.Length > 0)
        {
            char c = shared1[0];
            if (!(char.IsDigit(c) || _buildSeparatorChars.Contains(c)))
                yield return new  SwVersion.PreReleaseSegment[] { new SwVersion.PreReleaseSegment(true, shared1) };
        }
        yield return new  SwVersion.PreReleaseSegment[] { new SwVersion.PreReleaseSegment(false, shared2) };
        if (unique2.Length > 0)
        {
            char c = shared2[0];
            if (!(char.IsDigit(c) || _buildSeparatorChars.Contains(c)))
                yield return new  SwVersion.PreReleaseSegment[] { new SwVersion.PreReleaseSegment(true, shared2) };
        }
        yield return new  SwVersion.PreReleaseSegment[] { new SwVersion.PreReleaseSegment(false, unique1) };
        if (unique2.Length > 0)
        {
            char c = unique1[0];
            if (!(char.IsDigit(c) || _buildSeparatorChars.Contains(c)))
                yield return new  SwVersion.PreReleaseSegment[] { new SwVersion.PreReleaseSegment(true, unique1) };
        }
        yield return new  SwVersion.PreReleaseSegment[] { new SwVersion.PreReleaseSegment(false, unique2) };
        if (unique2.Length > 0)
        {
            char c = unique2[0];
            if (!(char.IsDigit(c) || _buildSeparatorChars.Contains(c)))
                yield return new  SwVersion.PreReleaseSegment[] { new SwVersion.PreReleaseSegment(true, unique2) };
        }
        foreach (string s in otherShared)
        {
            yield return new SwVersion.PreReleaseSegment[] { new SwVersion.PreReleaseSegment(false, s) };
            yield return new SwVersion.PreReleaseSegment[] { new SwVersion.PreReleaseSegment(true, s) };
        }
        
        yield return new SwVersion.PreReleaseSegment[]
        {
            new SwVersion.PreReleaseSegment(false, unique1), new SwVersion.PreReleaseSegment(true, shared2),
            new SwVersion.PreReleaseSegment(false, shared2), new SwVersion.PreReleaseSegment(true, GetRandomString(_buildCharsSource, 2, 8,
                otherShared.Concat(otherUnique).Concat(new string[] { shared1, shared2, unique1, unique2}).ToArray()))
        };
    }
    
    public static IEnumerable<SwVersion.BuildSegment> GetBuildSegments(string shared1, string shared2, string unique1, string unique2)
    {
        yield return new SwVersion.BuildSegment(SwVersion.BuildSeparator.Plus, shared1);
        yield return new SwVersion.BuildSegment(SwVersion.BuildSeparator.Plus, shared2);
        yield return new SwVersion.BuildSegment(SwVersion.BuildSeparator.Plus, unique1);
        yield return new SwVersion.BuildSegment(SwVersion.BuildSeparator.Plus, unique2);
    }
    
    public static IEnumerable<SwVersion.BuildSegment[]> GetAdditonalBuildSegments(string shared1, string shared2, string unique1, string unique2, string[] otherShared, params string[] otherUnique)
    {
        yield return new  SwVersion.BuildSegment[] { new SwVersion.BuildSegment(SwVersion.BuildSeparator.Plus, shared1) };
        yield return new  SwVersion.BuildSegment[] { new SwVersion.BuildSegment(SwVersion.BuildSeparator.Dash, shared1) };
        yield return new SwVersion.BuildSegment[] { new SwVersion.BuildSegment(SwVersion.BuildSeparator.Dot, shared1) };
        yield return new SwVersion.BuildSegment[] { new SwVersion.BuildSegment(SwVersion.BuildSeparator.Plus, shared2) };
        yield return new SwVersion.BuildSegment[] { new SwVersion.BuildSegment(SwVersion.BuildSeparator.Dash, shared2) };
        yield return new SwVersion.BuildSegment[] { new SwVersion.BuildSegment(SwVersion.BuildSeparator.Dot, shared2) };
        foreach (string s in otherShared)
            yield return new SwVersion.BuildSegment[] { new SwVersion.BuildSegment(SwVersion.BuildSeparator.Plus, s) };
        
        yield return new SwVersion.BuildSegment[]
        {
            new SwVersion.BuildSegment(SwVersion.BuildSeparator.Plus, unique1), new SwVersion.BuildSegment(SwVersion.BuildSeparator.Plus, shared2),
            new SwVersion.BuildSegment(SwVersion.BuildSeparator.Dash, shared2), new SwVersion.BuildSegment(SwVersion.BuildSeparator.Dot, GetRandomString(_prefixCharsSource, 2, 8,
                otherShared.Concat(otherUnique).Concat(new string[] { shared1, shared2, unique1, unique2}).ToArray()))
        };
    }

    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor1Test(string, int, int, int, int, IEnumerable{int}, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor1TestData : TheoryData<string, int, int, int, int, IEnumerable<int>, IEnumerable<SwVersion.PreReleaseSegment>, SwVersion.BuildSegment, SwVersion.BuildSegment[],
        VersionValues>
    {
        public ComponentConstructor1TestData()
        {
            IEnumerable<IEnumerable<int>> additionalNumericalValues = GetAdditionalNumericValues(out IEnumerable<int> majorValues, out IEnumerable<int> minorValues, out IEnumerable<int> patchValues,
                out IEnumerable<int> revisionValues);
            string sharedPrefix1 = GetRandomString(_prefixCharsSource, 1, 1);
            string sharedPrefix2 = GetRandomString(_prefixCharsSource, 2, 8);
            string uniquePrefix1 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix1);
            string uniquePrefix2 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string uniquePrefix3 = "-" + GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string sharedPreRelease1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1);
            string sharedPreRelease2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2);
            string uniquePreRelease1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1, sharedPreRelease1);
            string uniquePreRelease2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2, sharedPreRelease2);
            string sharedBuild1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1, sharedPreRelease1, uniquePreRelease1);
            string sharedBuild2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2, sharedPreRelease2, uniquePreRelease2);
            string uniqueBuild1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1, sharedPreRelease1, uniquePreRelease1, sharedBuild1);
            string uniqueBuild2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2, sharedPreRelease2, uniquePreRelease2, sharedBuild2);
            foreach (string prefix in new string[] { sharedPrefix1, sharedPrefix2, uniquePrefix1, uniquePrefix2, uniquePrefix3 })
                foreach (int major in majorValues)
                    foreach (int minor in minorValues)
                        foreach (int patch in patchValues)
                            foreach (int revision in revisionValues)
                                foreach (IEnumerable<int> additionalNumerical in additionalNumericalValues)
                                    foreach (SwVersion.PreReleaseSegment[] preRelease in GetPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1, uniquePreRelease2,
                                            new string[] { sharedPrefix1, sharedPrefix2, sharedBuild1, sharedBuild2 }, uniquePrefix1, uniquePrefix2, uniqueBuild1, uniqueBuild2))
                                        foreach (SwVersion.BuildSegment build in GetBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2))
                                            foreach (SwVersion.BuildSegment[] additionalBuild in GetAdditonalBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2, 
                                                    new string[] { sharedPrefix1, sharedPrefix2, sharedPreRelease1, sharedPreRelease2 }, uniquePrefix1, uniquePrefix2, uniquePreRelease1, uniquePreRelease2))
                                                Add(prefix, major, minor, patch, revision, additionalNumerical, preRelease, build, additionalBuild,
                                                    new(prefix, major, minor, patch, revision, additionalNumerical.ToArray(), preRelease, additionalBuild.Prepend(build).ToArray(),
                                                        SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, int, int, IEnumerable{int}, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor1TestData))]
    public void ComponentConstructor1Test(string prefix, int major, int minor, int patch, int revision, IEnumerable<int> additionalNumerical, IEnumerable<SwVersion.PreReleaseSegment> preRelease,
        SwVersion.BuildSegment build, SwVersion.BuildSegment[]? additionalBuild, VersionValues expected)
    {
        SwVersion target = (additionalBuild is null) ? new(prefix, major, minor, patch, revision, additionalNumerical, preRelease, build) : new(prefix, major, minor, patch, revision, additionalNumerical,
            preRelease, build, additionalBuild);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor2Test(int, int, int, int, IEnumerable{int}, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor2TestData : TheoryData<int, int, int, int, IEnumerable<int>, IEnumerable<SwVersion.PreReleaseSegment>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor2TestData()
        {
            IEnumerable<IEnumerable<int>> additionalNumericalValues = GetAdditionalNumericValues(out IEnumerable<int> majorValues, out IEnumerable<int> minorValues, out IEnumerable<int> patchValues,
                out IEnumerable<int> revisionValues);
            string sharedPreRelease1 = GetRandomString(_buildCharsSource, 1, 1);
            string sharedPreRelease2 = GetRandomString(_buildCharsSource, 2, 8);
            string uniquePreRelease1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1);
            string uniquePreRelease2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2);
            string sharedBuild1 = GetRandomString(_buildCharsSource, 1, 1);
            string sharedBuild2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2, uniquePreRelease2);
            string uniqueBuild1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1, uniquePreRelease1, sharedBuild1);
            string uniqueBuild2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2, uniquePreRelease2, sharedBuild2);
            foreach (int major in majorValues)
                foreach (int minor in minorValues)
                    foreach (int patch in patchValues)
                        foreach (int revision in revisionValues)
                            foreach (IEnumerable<int> additionalNumerical in additionalNumericalValues)
                                foreach (SwVersion.PreReleaseSegment[] preRelease in GetPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1, uniquePreRelease2,
                                        new string[] { sharedBuild1, sharedBuild2 }, uniqueBuild1, uniqueBuild2))
                                    foreach (SwVersion.BuildSegment build in GetBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2))
                                        foreach (SwVersion.BuildSegment[] additionalBuild in GetAdditonalBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2,
                                                new string[] { sharedPreRelease1, sharedPreRelease2 }, uniquePreRelease1, uniquePreRelease2))
                                            Add(major, minor, patch, revision, additionalNumerical, preRelease, build, additionalBuild,
                                                new(null, major, minor, patch, revision, additionalNumerical.ToArray(), preRelease, additionalBuild.Prepend(build).ToArray(),
                                                    SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, int, int, IEnumerable{int}, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor2TestData))]
    public void ComponentConstructor2Test(int major, int minor, int patch, int revision, IEnumerable<int> additionalNumerical, IEnumerable<SwVersion.PreReleaseSegment> preRelease,
        SwVersion.BuildSegment build, SwVersion.BuildSegment[]? additionalBuild, VersionValues expected)
    {
        SwVersion target = (additionalBuild is null) ? new(major, minor, patch, revision, additionalNumerical, preRelease, build) : new(major, minor, patch, revision, additionalNumerical, preRelease,
            build, additionalBuild);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor3Test(string, int, int, int, int, IEnumerable{int}, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor3TestData : TheoryData<string, int, int, int, int, IEnumerable<int>, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[], VersionValues>
    {
        public ComponentConstructor3TestData()
        {
            IEnumerable<IEnumerable<int>> additionalNumericalValues = GetAdditionalNumericValues(out IEnumerable<int> majorValues, out IEnumerable<int> minorValues, out IEnumerable<int> patchValues,
                out IEnumerable<int> revisionValues);
            string sharedPrefix1 = GetRandomString(_prefixCharsSource, 1, 1);
            string sharedPrefix2 = GetRandomString(_prefixCharsSource, 2, 8);
            string uniquePrefix1 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix1);
            string uniquePrefix2 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string uniquePrefix3 = "-" + GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string sharedPreRelease1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1);
            string sharedPreRelease2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2);
            string uniquePreRelease1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1, sharedPreRelease1);
            string uniquePreRelease2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2, sharedPreRelease2);
            foreach (string prefix in new string[] { sharedPrefix1, sharedPrefix2, uniquePrefix1, uniquePrefix2, uniquePrefix3 })
                foreach (int major in majorValues)
                    foreach (int minor in minorValues)
                        foreach (int patch in patchValues)
                            foreach (int revision in revisionValues)
                                foreach (IEnumerable<int> additionalNumerical in additionalNumericalValues)
                                    foreach (SwVersion.PreReleaseSegment preRelease in GetPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1, uniquePreRelease2))
                                        foreach (SwVersion.PreReleaseSegment[] additionalPreRelease in GetAdditonalPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1,
                                                    uniquePreRelease2, new string[] { sharedPrefix1, sharedPrefix2 }, uniquePrefix1, uniquePrefix2, uniquePrefix3))
                                            Add(prefix, major, minor, patch, revision, additionalNumerical, preRelease, additionalPreRelease, new(prefix, major, minor, patch, revision,
                                                additionalNumerical.ToArray(), additionalPreRelease.Prepend(preRelease).ToArray(), Array.Empty<SwVersion.BuildSegment>(),
                                                SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, int, int, IEnumerable{int}, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor3TestData))]
    public void ComponentConstructor3Test(string prefix, int major, int minor, int patch, int revision, IEnumerable<int> additionalNumerical, SwVersion.PreReleaseSegment preRelease,
        SwVersion.PreReleaseSegment[]? additionalPreRelease, VersionValues expected)
    {
        SwVersion target = (additionalPreRelease is null) ? new(prefix, major, minor, patch, revision, additionalNumerical, preRelease) : new(prefix, major, minor, patch, revision, additionalNumerical,
            preRelease, additionalPreRelease);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor4Test(int, int, int, int, IEnumerable{int}, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor4TestData : TheoryData<int, int, int, int, IEnumerable<int>, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[], VersionValues>
    {
        public ComponentConstructor4TestData()
        {
            IEnumerable<IEnumerable<int>> additionalNumericalValues = GetAdditionalNumericValues(out IEnumerable<int> majorValues, out IEnumerable<int> minorValues, out IEnumerable<int> patchValues,
                out IEnumerable<int> revisionValues);
            string sharedPreRelease1 = GetRandomString(_buildCharsSource, 1, 1);
            string sharedPreRelease2 = GetRandomString(_buildCharsSource, 2, 8);
            string uniquePreRelease1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1);
            string uniquePreRelease2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2);
            foreach (int major in majorValues)
                foreach (int minor in minorValues)
                    foreach (int patch in patchValues)
                        foreach (int revision in revisionValues)
                            foreach (IEnumerable<int> additionalNumerical in additionalNumericalValues)
                                foreach (SwVersion.PreReleaseSegment preRelease in GetPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1, uniquePreRelease2))
                                    foreach (SwVersion.PreReleaseSegment[] additionalPreRelease in GetAdditonalPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1,
                                                uniquePreRelease2, Array.Empty<string>()))
                                        Add(major, minor, patch, revision, additionalNumerical, preRelease, additionalPreRelease, new(null, major, minor, patch, revision,
                                            additionalNumerical.ToArray(), additionalPreRelease.Prepend(preRelease).ToArray(), Array.Empty<SwVersion.BuildSegment>(),
                                            SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, int, int, IEnumerable{int}, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor4TestData))]
    public void ComponentConstructor4Test(int major, int minor, int patch, int revision, IEnumerable<int> additionalNumerical, SwVersion.PreReleaseSegment preRelease, SwVersion.PreReleaseSegment[]? additionalPreRelease, VersionValues expected)
    {
        SwVersion target = (additionalPreRelease is null) ? new(major, minor, patch, revision, additionalNumerical, preRelease) : new(major, minor, patch, revision, additionalNumerical, preRelease,
            additionalPreRelease);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor5Test(string, int, int, int, int, IEnumerable{int}, SwVersion.BuildSegment, SwVersion.BuildSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor5TestData : TheoryData<string, int, int, int, int, IEnumerable<int>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor5TestData()
        {
            IEnumerable<IEnumerable<int>> additionalNumericalValues = GetAdditionalNumericValues(out IEnumerable<int> majorValues, out IEnumerable<int> minorValues, out IEnumerable<int> patchValues,
                 out IEnumerable<int> revisionValues);
            string sharedPrefix1 = GetRandomString(_prefixCharsSource, 1, 1);
            string sharedPrefix2 = GetRandomString(_prefixCharsSource, 2, 8);
            string uniquePrefix1 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix1);
            string uniquePrefix2 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string uniquePrefix3 = "-" + GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string sharedBuild1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1);
            string sharedBuild2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2);
            string uniqueBuild1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1, sharedBuild1);
            string uniqueBuild2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2, sharedBuild2);
            foreach (string prefix in new string[] { sharedPrefix1, sharedPrefix2, uniquePrefix1, uniquePrefix2, uniquePrefix3 })
                foreach (int major in majorValues)
                    foreach (int minor in minorValues)
                        foreach (int patch in patchValues)
                            foreach (int revision in revisionValues)
                                foreach (IEnumerable<int> additionalNumerical in additionalNumericalValues)
                                    foreach (SwVersion.BuildSegment build in GetBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2))
                                        foreach (SwVersion.BuildSegment[] additionalBuild in GetAdditonalBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2,
                                                new string[] { sharedPrefix1, sharedPrefix2 }, uniquePrefix1, uniquePrefix2))
                                            Add(prefix, major, minor, patch, revision, additionalNumerical, build, additionalBuild,
                                                new(prefix, major, minor, patch, revision, additionalNumerical.ToArray(), Array.Empty<SwVersion.PreReleaseSegment>(),
                                                additionalBuild.Prepend(build).ToArray(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, int, int, IEnumerable{int}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor5TestData))]
    public void ComponentConstructor5Test(string prefix, int major, int minor, int patch, int revision, IEnumerable<int> additionalNumerical, SwVersion.BuildSegment build,
        SwVersion.BuildSegment[]? additionalBuild, VersionValues expected)
    {
        SwVersion target = (additionalBuild is null) ? new(prefix, major, minor, patch, revision, additionalNumerical, build) : new(prefix, major, minor, patch, revision, additionalNumerical, build,
            additionalBuild);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor6Test(int, int, int, int, IEnumerable{int}, SwVersion.BuildSegment, SwVersion.BuildSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor6TestData : TheoryData<int, int, int, int, IEnumerable<int>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor6TestData()
        {
            IEnumerable<IEnumerable<int>> additionalNumericalValues = GetAdditionalNumericValues(out IEnumerable<int> majorValues, out IEnumerable<int> minorValues, out IEnumerable<int> patchValues,
                out IEnumerable<int> revisionValues);
            string sharedBuild1 = GetRandomString(_buildCharsSource, 1, 1);
            string sharedBuild2 = GetRandomString(_buildCharsSource, 2, 8);
            string uniqueBuild1 = GetRandomString(_buildCharsSource, 1, 1);
            string uniqueBuild2 = GetRandomString(_buildCharsSource, 2, 8);
            foreach (int major in majorValues)
                foreach (int minor in minorValues)
                    foreach (int patch in patchValues)
                        foreach (int revision in revisionValues)
                            foreach (IEnumerable<int> additionalNumerical in additionalNumericalValues)
                                foreach (SwVersion.BuildSegment build in GetBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2))
                                    foreach (SwVersion.BuildSegment[] additionalBuild in GetAdditonalBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2, Array.Empty<string>()))
                                        Add(major, minor, patch, revision, additionalNumerical, build, additionalBuild,
                                            new(null, major, minor, patch, revision, additionalNumerical.ToArray(), Array.Empty<SwVersion.PreReleaseSegment>(),
                                            additionalBuild.Prepend(build).ToArray(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, int, int, IEnumerable{int}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor6TestData))]
    public void ComponentConstructor6Test(int major, int minor, int patch, int revision, IEnumerable<int> additionalNumerical, SwVersion.BuildSegment build, SwVersion.BuildSegment[]? additionalBuild,
        VersionValues expected)
    {
        SwVersion target = (additionalBuild is null) ? new(major, minor, patch, revision, additionalNumerical, build) : new(major, minor, patch, revision, additionalNumerical, build, additionalBuild);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor7Test(string, int, int, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor7TestData : TheoryData<string, int, int, int, IEnumerable<SwVersion.PreReleaseSegment>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor7TestData()
        {
            IEnumerable<int> patchValues = GetPatchValues(out IEnumerable<int> majorValues, out IEnumerable<int> minorValues);
            string sharedPrefix1 = GetRandomString(_prefixCharsSource, 1, 1);
            string sharedPrefix2 = GetRandomString(_prefixCharsSource, 2, 8);
            string uniquePrefix1 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix1);
            string uniquePrefix2 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string uniquePrefix3 = "-" + GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string sharedPreRelease1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1);
            string sharedPreRelease2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2);
            string uniquePreRelease1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1, sharedPreRelease1);
            string uniquePreRelease2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2, sharedPreRelease2);
            string sharedBuild1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1, sharedPreRelease1, uniquePreRelease1);
            string sharedBuild2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2, sharedPreRelease2, uniquePreRelease2);
            string uniqueBuild1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1, sharedPreRelease1, uniquePreRelease1, sharedBuild1);
            string uniqueBuild2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2, sharedPreRelease2, uniquePreRelease2, sharedBuild2);
            foreach (string prefix in new string[] { sharedPrefix1, sharedPrefix2, uniquePrefix1, uniquePrefix2, uniquePrefix3 })
                foreach (int major in majorValues)
                    foreach (int minor in minorValues)
                        foreach (int patch in patchValues)
                            foreach (SwVersion.PreReleaseSegment[] preRelease in GetPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1, uniquePreRelease2,
                                    new string[] { sharedPrefix1, sharedPrefix2, sharedBuild1, sharedBuild2 }, uniquePrefix1, uniquePrefix2, uniqueBuild1, uniqueBuild2))
                                foreach (SwVersion.BuildSegment build in GetBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2))
                                    foreach (SwVersion.BuildSegment[] additionalBuild in GetAdditonalBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2,
                                            new string[] { sharedPrefix1, sharedPrefix2, sharedPreRelease1, sharedPreRelease2 }, uniquePrefix1, uniquePrefix2, uniquePreRelease1, uniquePreRelease2))
                                        Add(prefix, major, minor, patch, preRelease, build, additionalBuild, new(prefix, major, minor, patch, null, Array.Empty<int>(), preRelease,
                                            additionalBuild.Prepend(build).ToArray(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor7TestData))]
    public void ComponentConstructor7Test(string prefix, int major, int minor, int patch, IEnumerable<SwVersion.PreReleaseSegment> preRelease, SwVersion.BuildSegment build,
        SwVersion.BuildSegment[]? additionalBuild, VersionValues expected)
    {
        SwVersion target = (additionalBuild is null) ? new(prefix, major, minor, patch, preRelease, build) : new(prefix, major, minor, patch, preRelease, build, additionalBuild);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor8Test(string, int, int, int, int, int[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor8TestData : TheoryData<string, int, int, int, int, int[], VersionValues>
    {
        public ComponentConstructor8TestData()
        {
            IEnumerable<IEnumerable<int>> additionalNumericalValues = GetAdditionalNumericValues(out IEnumerable<int> majorValues, out IEnumerable<int> minorValues, out IEnumerable<int> patchValues,
                out IEnumerable<int> revisionValues);
            string sharedPrefix1 = GetRandomString(_prefixCharsSource, 1, 1);
            string sharedPrefix2 = GetRandomString(_prefixCharsSource, 2, 8);
            string uniquePrefix1 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix1);
            string uniquePrefix2 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string uniquePrefix3 = "-" + GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            foreach (string prefix in new string[] { sharedPrefix1, sharedPrefix2, uniquePrefix1, uniquePrefix2, uniquePrefix3 })
                foreach (int major in majorValues)
                    foreach (int minor in minorValues)
                        foreach (int patch in patchValues)
                            foreach (int revision in revisionValues)
                                foreach (IEnumerable<int> additionalNumerical in additionalNumericalValues) // TODO: See if additionalNumerical can be null
                                    Add(prefix, major, minor, patch, revision, additionalNumerical.ToArray(), new(prefix, major, minor, patch, revision, additionalNumerical.ToArray(),
                                        Array.Empty<SwVersion.PreReleaseSegment>(), Array.Empty<SwVersion.BuildSegment>(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, int, int, int[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor8TestData))]
    public void ComponentConstructor8Test(string prefix, int major, int minor, int patch, int revision, int[]? additionalNumerical, VersionValues expected)
    {
        SwVersion target = (additionalNumerical is null) ? new(prefix, major, minor, patch, revision) : new(prefix, major, minor, patch, revision, additionalNumerical);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor9Test(int, int, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor9TestData : TheoryData<int, int, int, IEnumerable<SwVersion.PreReleaseSegment>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor9TestData()
        {
            IEnumerable<int> patchValues = GetPatchValues(out IEnumerable<int> majorValues, out IEnumerable<int> minorValues);
            string sharedPreRelease1 = GetRandomString(_buildCharsSource, 1, 1);
            string sharedPreRelease2 = GetRandomString(_buildCharsSource, 2, 8);
            string uniquePreRelease1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1);
            string uniquePreRelease2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2);
            string sharedBuild1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1, uniquePreRelease1);
            string sharedBuild2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2, uniquePreRelease2);
            string uniqueBuild1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1, uniquePreRelease1, sharedBuild1);
            string uniqueBuild2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2, uniquePreRelease2, sharedBuild2);
            foreach (int major in majorValues)
                foreach (int minor in minorValues)
                    foreach (int patch in patchValues)
                        foreach (SwVersion.PreReleaseSegment[] preRelease in GetPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1, uniquePreRelease2,
                                new string[] { sharedBuild1, sharedBuild2 }, uniqueBuild1, uniqueBuild2))
                            foreach (SwVersion.BuildSegment build in GetBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2))
                                foreach (SwVersion.BuildSegment[] additionalBuild in GetAdditonalBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2,
                                        new string[] { sharedPreRelease1, sharedPreRelease2 }, uniquePreRelease1, uniquePreRelease2))
                                    Add( major, minor, patch, preRelease, build, additionalBuild, new(null, major, minor, patch, null, Array.Empty<int>(), preRelease,
                                        additionalBuild.Prepend(build).ToArray(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor9TestData))]
    public void ComponentConstructor9Test(int major, int minor, int patch, IEnumerable<SwVersion.PreReleaseSegment> preRelease, SwVersion.BuildSegment build, SwVersion.BuildSegment[]? additionalBuild,
        VersionValues expected)
    {
        SwVersion target = (additionalBuild is null) ? new(major, minor, patch, preRelease, build) : new(major, minor, patch, preRelease, build, additionalBuild);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor10Test(string, int, int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor10TestData : TheoryData<string, int, int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[], VersionValues>
    {
        public ComponentConstructor10TestData()
        {
            IEnumerable<int> patchValues = GetPatchValues(out IEnumerable<int> majorValues, out IEnumerable<int> minorValues);
            string sharedPrefix1 = GetRandomString(_prefixCharsSource, 1, 1);
            string sharedPrefix2 = GetRandomString(_prefixCharsSource, 2, 8);
            string uniquePrefix1 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix1);
            string uniquePrefix2 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string uniquePrefix3 = "-" + GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string sharedPreRelease1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1);
            string sharedPreRelease2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2);
            string uniquePreRelease1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1, sharedPreRelease1);
            string uniquePreRelease2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2, sharedPreRelease2);
            foreach (string prefix in new string[] { sharedPrefix1, sharedPrefix2, uniquePrefix1, uniquePrefix2, uniquePrefix3 })
                foreach (int major in majorValues)
                    foreach (int minor in minorValues)
                        foreach (int patch in patchValues)
                            foreach (SwVersion.PreReleaseSegment preRelease in GetPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1, uniquePreRelease2))
                                foreach (SwVersion.PreReleaseSegment[] additionalPreRelease in GetAdditonalPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1,
                                        uniquePreRelease2, new string[] { sharedPrefix1, sharedPrefix2 }, uniquePrefix1, uniquePrefix2, uniquePrefix3))
                                    Add(prefix, major, minor, patch, preRelease, additionalPreRelease, new(prefix, major, minor, patch, null, null, additionalPreRelease.Prepend(preRelease).ToArray(),
                                        Array.Empty<SwVersion.BuildSegment>(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor10TestData))]
    public void ComponentConstructor10Test(string prefix, int major, int minor, int patch, SwVersion.PreReleaseSegment preRelease, SwVersion.PreReleaseSegment[]? additionalPreRelease,
        VersionValues expected)
    {
        SwVersion target = (additionalPreRelease is null) ? new(prefix, major, minor, patch, preRelease) : new(prefix, major, minor, patch, preRelease, additionalPreRelease);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor11Test(int, int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor11TestData : TheoryData<int, int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[], VersionValues>
    {
        public ComponentConstructor11TestData()
        {
            IEnumerable<int> patchValues = GetPatchValues(out IEnumerable<int> majorValues, out IEnumerable<int> minorValues);
            string sharedPreRelease1 = GetRandomString(_buildCharsSource, 1, 1);
            string sharedPreRelease2 = GetRandomString(_buildCharsSource, 2, 8);
            string uniquePreRelease1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1);
            string uniquePreRelease2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2);
                foreach (int major in majorValues)
                    foreach (int minor in minorValues)
                        foreach (int patch in patchValues)
                            foreach (SwVersion.PreReleaseSegment preRelease in GetPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1, uniquePreRelease2))
                                foreach (SwVersion.PreReleaseSegment[] additionalPreRelease in GetAdditonalPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1,
                                        uniquePreRelease2, Array.Empty<string>()))
                                    Add(major, minor, patch, preRelease, additionalPreRelease, new(null, major, minor, patch, null, null, additionalPreRelease.Prepend(preRelease).ToArray(),
                                        Array.Empty<SwVersion.BuildSegment>(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor11TestData))]
    public void ComponentConstructor11Test(int major, int minor, int patch, SwVersion.PreReleaseSegment preRelease, SwVersion.PreReleaseSegment[]? additionalPreRelease, VersionValues expected)
    {
        SwVersion target = (additionalPreRelease is null) ? new(major, minor, patch, preRelease) : new(major, minor, patch, preRelease, additionalPreRelease);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor12Test(string, int, int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor12TestData : TheoryData<string, int, int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor12TestData()
        {
            IEnumerable<int> patchValues = GetPatchValues(out IEnumerable<int> majorValues, out IEnumerable<int> minorValues);
            string sharedPrefix1 = GetRandomString(_prefixCharsSource, 1, 1);
            string sharedPrefix2 = GetRandomString(_prefixCharsSource, 2, 8);
            string uniquePrefix1 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix1);
            string uniquePrefix2 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string uniquePrefix3 = "-" + GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string sharedBuild1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1);
            string sharedBuild2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2);
            string uniqueBuild1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1, sharedBuild1);
            string uniqueBuild2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2, sharedBuild2);
            foreach (string prefix in new string[] { sharedPrefix1, sharedPrefix2, uniquePrefix1, uniquePrefix2, uniquePrefix3 })
                foreach (int major in majorValues)
                    foreach (int minor in minorValues)
                        foreach (int patch in patchValues)
                            foreach (SwVersion.BuildSegment build in GetBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2))
                                foreach (SwVersion.BuildSegment[] additionalBuild in GetAdditonalBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2,
                                        new string[] { sharedPrefix1, sharedPrefix2 }, uniquePrefix1, uniquePrefix2))
                                    Add(prefix, major, minor, patch, build, additionalBuild, new(prefix, major, minor, patch, null, null, Array.Empty<SwVersion.PreReleaseSegment>(),
                                        additionalBuild.Prepend(build).ToArray(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor12TestData))]
    public void ComponentConstructor12Test(string prefix, int major, int minor, int patch, SwVersion.BuildSegment build, SwVersion.BuildSegment[]? additionalBuild, VersionValues expected)
    {
        SwVersion target = (additionalBuild is null) ? new(prefix, major, minor, patch, build) : new(prefix, major, minor, patch, build, additionalBuild);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor13Test(string, int, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor13TestData : TheoryData<string, int, int, IEnumerable<SwVersion.PreReleaseSegment>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor13TestData()
        {
            IEnumerable<int> minorValues = GetMinorValues(out IEnumerable<int> majorValues);
            string sharedPrefix1 = GetRandomString(_prefixCharsSource, 1, 1);
            string sharedPrefix2 = GetRandomString(_prefixCharsSource, 2, 8);
            string uniquePrefix1 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix1);
            string uniquePrefix2 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string uniquePrefix3 = "-" + GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2, uniquePrefix2);
            string sharedPreRelease1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1);
            string sharedPreRelease2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2);
            string uniquePreRelease1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1, uniquePrefix1, sharedPrefix1);
            string uniquePreRelease2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2);
            string sharedBuild1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1, uniquePreRelease1, uniquePrefix1, sharedPrefix1);
            string sharedBuild2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2, uniquePreRelease2, uniquePrefix2, sharedPrefix2);
            string uniqueBuild1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1, uniquePreRelease1, sharedBuild1, uniquePrefix1, sharedPrefix1);
            string uniqueBuild2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2, uniquePreRelease2, sharedBuild2, uniquePrefix2, sharedPrefix2);
            foreach (string prefix in new string[] { sharedPrefix1, sharedPrefix2, uniquePrefix1, uniquePrefix2, uniquePrefix3 })
                foreach (int major in majorValues)
                    foreach (int minor in minorValues)
                        foreach (SwVersion.PreReleaseSegment[] preRelease in GetPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1, uniquePreRelease2,
                                new string[] { sharedBuild1, sharedBuild2 }, uniqueBuild1, uniqueBuild2))
                            foreach (SwVersion.BuildSegment build in GetBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2))
                                foreach (SwVersion.BuildSegment[] additionalBuild in GetAdditonalBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2,
                                        new string[] { sharedPreRelease1, sharedPreRelease2 }, uniquePreRelease1, uniquePreRelease2))
                                    Add(prefix, major, minor, preRelease, build, additionalBuild, new(prefix, major, minor, null, null, null, preRelease,
                                        additionalBuild.Prepend(build).ToArray(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor13TestData))]
    public void ComponentConstructor13Test(string prefix, int major, int minor, IEnumerable<SwVersion.PreReleaseSegment> preRelease, SwVersion.BuildSegment build,
        SwVersion.BuildSegment[]? additionalBuild, VersionValues expected)
    {
        SwVersion target = (additionalBuild is null) ? new(prefix, major, minor, preRelease, build) : new(prefix, major, minor, preRelease, build, additionalBuild);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor14Test(int, int, int, int, int[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor14TestData : TheoryData<int, int, int, int, int[], VersionValues>
    {
        public ComponentConstructor14TestData()
        {
            IEnumerable<IEnumerable<int>> additionalNumericalValues = GetAdditionalNumericValues(out IEnumerable<int> majorValues, out IEnumerable<int> minorValues, out IEnumerable<int> patchValues, 
                out IEnumerable<int> revisionValues);
            foreach (int major in majorValues)
                foreach (int minor in minorValues)
                    foreach (int patch in patchValues)
                        foreach (int revision in revisionValues)
                            foreach (IEnumerable<int> additionalNumerical in additionalNumericalValues) // TODO: See if additionalNumerical can be null
                                Add(major, minor, patch, revision, additionalNumerical.ToArray(), new(null, major, minor, patch, revision, additionalNumerical.ToArray(),
                                    Array.Empty<SwVersion.PreReleaseSegment>(), Array.Empty<SwVersion.BuildSegment>(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, int, int, int[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor14TestData))]
    public void ComponentConstructor14Test(int major, int minor, int patch, int revision, int[]? additionalNumerical, VersionValues expected)
    {
        SwVersion target = (additionalNumerical is null) ? new(major, minor, patch, revision) : new(major, minor, patch, revision, additionalNumerical);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor15Test(int, int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor15TestData : TheoryData<int, int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor15TestData()
        {
            IEnumerable<int> patchValues = GetPatchValues(out IEnumerable<int> majorValues, out IEnumerable<int> minorValues);
            string sharedBuild1 = GetRandomString(_buildCharsSource, 1, 1);
            string sharedBuild2 = GetRandomString(_buildCharsSource, 2, 8);
            string uniqueBuild1 = GetRandomString(_buildCharsSource, 1, 1, sharedBuild1);
            string uniqueBuild2 = GetRandomString(_buildCharsSource, 2, 8, sharedBuild2);
            foreach (int major in majorValues)
                foreach (int minor in minorValues)
                    foreach (int patch in patchValues)
                        foreach (SwVersion.BuildSegment build in GetBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2))
                            foreach (SwVersion.BuildSegment[] additionalBuild in GetAdditonalBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2, Array.Empty<string>()))
                                Add(major, minor, patch, build, additionalBuild, new(null, major, minor, patch, null, null, Array.Empty<SwVersion.PreReleaseSegment>(),
                                    additionalBuild.Prepend(build).ToArray(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor15TestData))]
    public void ComponentConstructor15Test(int major, int minor, int patch, SwVersion.BuildSegment build, SwVersion.BuildSegment[]? additionalBuild, VersionValues expected)
    {
        SwVersion target = (additionalBuild is null) ? new(major, minor, patch, build) : new(major, minor, patch, build, additionalBuild);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor16Test(int, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor16TestData : TheoryData<int, int, IEnumerable<SwVersion.PreReleaseSegment>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor16TestData()
        {
            IEnumerable<int> minorValues = GetMinorValues(out IEnumerable<int> majorValues);
            string sharedPreRelease1 = GetRandomString(_buildCharsSource, 1, 1);
            string sharedPreRelease2 = GetRandomString(_buildCharsSource, 2, 8);
            string uniquePreRelease1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1);
            string uniquePreRelease2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2);
            string sharedBuild1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1, uniquePreRelease1);
            string sharedBuild2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2, uniquePreRelease2);
            string uniqueBuild1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1, uniquePreRelease1, sharedBuild1);
            string uniqueBuild2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2, uniquePreRelease2, sharedBuild2);
            foreach (int major in majorValues)
                foreach (int minor in minorValues)
                    foreach (SwVersion.PreReleaseSegment[] preRelease in GetPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1, uniquePreRelease2,
                            new string[] { sharedBuild1, sharedBuild2 }, uniqueBuild1, uniqueBuild2))
                        foreach (SwVersion.BuildSegment build in GetBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2))
                            foreach (SwVersion.BuildSegment[] additionalBuild in GetAdditonalBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2,
                                    new string[] { sharedPreRelease1, sharedPreRelease2 }, uniquePreRelease1, uniquePreRelease2))
                                Add(major, minor, preRelease, build, additionalBuild, new(null, major, minor, null, null, null, preRelease,
                                    additionalBuild.Prepend(build).ToArray(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor16TestData))]
    public void ComponentConstructor16Test(int major, int minor, IEnumerable<SwVersion.PreReleaseSegment> preRelease, SwVersion.BuildSegment build, SwVersion.BuildSegment[]? additionalBuild,
        VersionValues expected)
    {
        SwVersion target = (additionalBuild is null) ? new(major, minor, preRelease, build) : new(major, minor, preRelease, build, additionalBuild);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor17Test(string, int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor17TestData : TheoryData<string, int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[], VersionValues>
    {
        public ComponentConstructor17TestData()
        {
            IEnumerable<int> minorValues = GetMinorValues(out IEnumerable<int> majorValues);
            string sharedPrefix1 = GetRandomString(_prefixCharsSource, 1, 1);
            string sharedPrefix2 = GetRandomString(_prefixCharsSource, 2, 8);
            string uniquePrefix1 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix1);
            string uniquePrefix2 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string uniquePrefix3 = "-" + GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string sharedPreRelease1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1);
            string sharedPreRelease2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2);
            string uniquePreRelease1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1, sharedPreRelease1);
            string uniquePreRelease2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2, sharedPreRelease2);
            foreach (string prefix in new string[] { sharedPrefix1, sharedPrefix2, uniquePrefix1, uniquePrefix2, uniquePrefix3 })
                foreach (int major in majorValues)
                    foreach (int minor in minorValues)
                        foreach (SwVersion.PreReleaseSegment preRelease in GetPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1, uniquePreRelease2))
                            foreach (SwVersion.PreReleaseSegment[] additionalPreRelease in GetAdditonalPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1,
                                    uniquePreRelease2, new string[] { sharedPrefix1, sharedPrefix2 }, uniquePrefix1, uniquePrefix2, uniquePrefix3))
                                Add(prefix, major, minor, preRelease, additionalPreRelease, new(prefix, major, minor, null, null, null, additionalPreRelease.Prepend(preRelease).ToArray(),
                                    Array.Empty<SwVersion.BuildSegment>(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor17TestData))]
    public void ComponentConstructor17Test(string prefix, int major, int minor, SwVersion.PreReleaseSegment preRelease, SwVersion.PreReleaseSegment[]? additionalPreRelease, VersionValues expected)
    {
        SwVersion target = (additionalPreRelease is null) ? new(prefix, major, minor, preRelease) : new(prefix, major, minor, preRelease, additionalPreRelease);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor18Test(int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor18TestData : TheoryData<int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[], VersionValues>
    {
        public ComponentConstructor18TestData()
        {
            IEnumerable<int> minorValues = GetMinorValues(out IEnumerable<int> majorValues);
            string sharedPreRelease1 = GetRandomString(_buildCharsSource, 1, 1);
            string sharedPreRelease2 = GetRandomString(_buildCharsSource, 2, 8);
            string uniquePreRelease1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1);
            string uniquePreRelease2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2);
            foreach (int major in majorValues)
                foreach (int minor in minorValues)
                    foreach (SwVersion.PreReleaseSegment preRelease in GetPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1, uniquePreRelease2))
                        foreach (SwVersion.PreReleaseSegment[] additionalPreRelease in GetAdditonalPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1,
                                uniquePreRelease2, Array.Empty<string>()))
                            Add(major, minor, preRelease, additionalPreRelease, new(null, major, minor, null, null, null, additionalPreRelease.Prepend(preRelease).ToArray(),
                                Array.Empty<SwVersion.BuildSegment>(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor18TestData))]
    public void ComponentConstructor18Test(int major, int minor, SwVersion.PreReleaseSegment preRelease, SwVersion.PreReleaseSegment[]? additionalPreRelease, VersionValues expected)
    {
        SwVersion target = (additionalPreRelease is null) ? new(major, minor, preRelease) : new(major, minor, preRelease, additionalPreRelease);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor19Test(string, int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor19TestData : TheoryData<string, int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor19TestData()
        {
            IEnumerable<int> minorValues = GetMinorValues(out IEnumerable<int> majorValues);
            string sharedPrefix1 = GetRandomString(_prefixCharsSource, 1, 1);
            string sharedPrefix2 = GetRandomString(_prefixCharsSource, 2, 8);
            string uniquePrefix1 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix1);
            string uniquePrefix2 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string uniquePrefix3 = "-" + GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string sharedBuild1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1);
            string sharedBuild2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2);
            string uniqueBuild1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1, sharedBuild1);
            string uniqueBuild2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2, sharedBuild2);
            foreach (string prefix in new string[] { sharedPrefix1, sharedPrefix2, uniquePrefix1, uniquePrefix2, uniquePrefix3 })
                foreach (int major in majorValues)
                    foreach (int minor in minorValues)
                        foreach (SwVersion.BuildSegment build in GetBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2))
                            foreach (SwVersion.BuildSegment[] additionalBuild in GetAdditonalBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2,
                                    new string[] { sharedPrefix1, sharedPrefix2 }, uniquePrefix1, uniquePrefix2))
                                Add(prefix, major, minor, build, additionalBuild, new(prefix, major, minor, null, null, null, Array.Empty<SwVersion.PreReleaseSegment>(),
                                    additionalBuild.Prepend(build).ToArray(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor19TestData))]
    public void ComponentConstructor19Test(string prefix, int major, int minor, SwVersion.BuildSegment build, SwVersion.BuildSegment[]? additionalBuild, VersionValues expected)
    {
        SwVersion target = (additionalBuild is null) ? new(prefix, major, minor, build) : new(prefix, major, minor, build, additionalBuild);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor20Test(string, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor20TestData : TheoryData<string, int, IEnumerable<SwVersion.PreReleaseSegment>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor20TestData()
        {
            string sharedPrefix1 = GetRandomString(_prefixCharsSource, 1, 1);
            string sharedPrefix2 = GetRandomString(_prefixCharsSource, 2, 8);
            string uniquePrefix1 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix1);
            string uniquePrefix2 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string uniquePrefix3 = "-" + GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2, uniquePrefix2);
            string sharedPreRelease1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1);
            string sharedPreRelease2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2);
            string uniquePreRelease1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1, uniquePrefix1, sharedPrefix1);
            string uniquePreRelease2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2);
            string sharedBuild1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1, uniquePreRelease1, uniquePrefix1, sharedPrefix1);
            string sharedBuild2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2, uniquePreRelease2, uniquePrefix2, sharedPrefix2);
            string uniqueBuild1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1, uniquePreRelease1, sharedBuild1, uniquePrefix1, sharedPrefix1);
            string uniqueBuild2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2, uniquePreRelease2, sharedBuild2, uniquePrefix2, sharedPrefix2);
            foreach (string prefix in new string[] { sharedPrefix1, sharedPrefix2, uniquePrefix1, uniquePrefix2, uniquePrefix3 })
                foreach (int major in GetMajorValues())
                    foreach (SwVersion.PreReleaseSegment[] preRelease in GetPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1, uniquePreRelease2,
                            new string[] { sharedBuild1, sharedBuild2 }, uniqueBuild1, uniqueBuild2))
                        foreach (SwVersion.BuildSegment build in GetBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2))
                            foreach (SwVersion.BuildSegment[] additionalBuild in GetAdditonalBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2,
                                    new string[] { sharedPreRelease1, sharedPreRelease2 }, uniquePreRelease1, uniquePreRelease2))
                                Add(prefix, major, preRelease, build, additionalBuild, new(prefix, major, null, null, null, null, preRelease,
                                    additionalBuild.Prepend(build).ToArray(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor20TestData))]
    public void ComponentConstructor20Test(string prefix, int major, IEnumerable<SwVersion.PreReleaseSegment> preRelease, SwVersion.BuildSegment build, SwVersion.BuildSegment[]? additionalBuild,
        VersionValues expected)
    {
        SwVersion target = (additionalBuild is null) ? new(prefix, major, preRelease, build) : new(prefix, major, preRelease, build, additionalBuild);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor21Test(int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor21TestData : TheoryData<int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor21TestData()
        {
            IEnumerable<int> minorValues = GetMinorValues(out IEnumerable<int> majorValues);
            string sharedBuild1 = GetRandomString(_buildCharsSource, 1, 1);
            string sharedBuild2 = GetRandomString(_buildCharsSource, 2, 8);
            string uniqueBuild1 = GetRandomString(_buildCharsSource, 1, 1, sharedBuild1);
            string uniqueBuild2 = GetRandomString(_buildCharsSource, 2, 8, sharedBuild2);
            foreach (int major in majorValues)
                foreach (int minor in minorValues)
                    foreach (SwVersion.BuildSegment build in GetBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2))
                        foreach (SwVersion.BuildSegment[] additionalBuild in GetAdditonalBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2, Array.Empty<string>()))
                            Add(major, minor, build, additionalBuild, new(null, major, minor, null, null, null, Array.Empty<SwVersion.PreReleaseSegment>(),
                                additionalBuild.Prepend(build).ToArray(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor21TestData))]
    public void ComponentConstructor21Test(int major, int minor, SwVersion.BuildSegment build, SwVersion.BuildSegment[]? additionalBuild, VersionValues expected)
    {
        SwVersion target = (additionalBuild is null) ? new(major, minor, build) : new(major, minor, build, additionalBuild);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor22Test(int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor22TestData : TheoryData<int, IEnumerable<SwVersion.PreReleaseSegment>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor22TestData()
        {
            string sharedPreRelease1 = GetRandomString(_buildCharsSource, 1, 1);
            string sharedPreRelease2 = GetRandomString(_buildCharsSource, 2, 8);
            string uniquePreRelease1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1);
            string uniquePreRelease2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2);
            string sharedBuild1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1, uniquePreRelease1);
            string sharedBuild2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2, uniquePreRelease2);
            string uniqueBuild1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1, uniquePreRelease1, sharedBuild1);
            string uniqueBuild2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2, uniquePreRelease2, sharedBuild2);
            foreach (int major in GetMajorValues())
                foreach (SwVersion.PreReleaseSegment[] preRelease in GetPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1, uniquePreRelease2,
                        new string[] { sharedBuild1, sharedBuild2 }, uniqueBuild1, uniqueBuild2))
                    foreach (SwVersion.BuildSegment build in GetBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2))
                        foreach (SwVersion.BuildSegment[] additionalBuild in GetAdditonalBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2,
                                new string[] { sharedPreRelease1, sharedPreRelease2 }, uniquePreRelease1, uniquePreRelease2))
                            Add(major, preRelease, build, additionalBuild, new(null, major, null, null, null, null, preRelease,
                                additionalBuild.Prepend(build).ToArray(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor22TestData))]
    public void ComponentConstructor22Test(int major, IEnumerable<SwVersion.PreReleaseSegment> preRelease, SwVersion.BuildSegment build, SwVersion.BuildSegment[]? additionalBuild, VersionValues expected)
    {
        SwVersion target = (additionalBuild is null) ? new(major, preRelease, build) : new(major, preRelease, build, additionalBuild);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor23Test(string, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor23TestData : TheoryData<string, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[], VersionValues>
    {
        public ComponentConstructor23TestData()
        {
            string sharedPrefix1 = GetRandomString(_prefixCharsSource, 1, 1);
            string sharedPrefix2 = GetRandomString(_prefixCharsSource, 2, 8);
            string uniquePrefix1 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix1);
            string uniquePrefix2 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string uniquePrefix3 = "-" + GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string sharedPreRelease1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1);
            string sharedPreRelease2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2);
            string uniquePreRelease1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1, sharedPreRelease1);
            string uniquePreRelease2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2, sharedPreRelease2);
            foreach (string prefix in new string[] { sharedPrefix1, sharedPrefix2, uniquePrefix1, uniquePrefix2, uniquePrefix3 })
                foreach (int major in GetMajorValues())
                    foreach (SwVersion.PreReleaseSegment preRelease in GetPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1, uniquePreRelease2))
                        foreach (SwVersion.PreReleaseSegment[] additionalPreRelease in GetAdditonalPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1,
                                uniquePreRelease2, new string[] { sharedPrefix1, sharedPrefix2 }, uniquePrefix1, uniquePrefix2, uniquePrefix3))
                            Add(prefix, major, preRelease, additionalPreRelease, new(prefix, major, null, null, null, null, additionalPreRelease.Prepend(preRelease).ToArray(),
                                Array.Empty<SwVersion.BuildSegment>(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor23TestData))]
    public void ComponentConstructor23Test(string prefix, int major, SwVersion.PreReleaseSegment preRelease, SwVersion.PreReleaseSegment[]? additionalPreRelease, VersionValues expected)
    {
        SwVersion target = (additionalPreRelease is null) ? new(prefix, major, preRelease) : new(prefix, major, preRelease, additionalPreRelease);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor24Test(int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor24TestData : TheoryData<int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[], VersionValues>
    {
        public ComponentConstructor24TestData()
        {
            string sharedPreRelease1 = GetRandomString(_buildCharsSource, 1, 1);
            string sharedPreRelease2 = GetRandomString(_buildCharsSource, 2, 8);
            string uniquePreRelease1 = GetRandomString(_buildCharsSource, 1, 1, sharedPreRelease1);
            string uniquePreRelease2 = GetRandomString(_buildCharsSource, 2, 8, sharedPreRelease2);
                foreach (int major in GetMajorValues())
                    foreach (SwVersion.PreReleaseSegment preRelease in GetPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1, uniquePreRelease2))
                        foreach (SwVersion.PreReleaseSegment[] additionalPreRelease in GetAdditonalPreReleaseSegments(sharedPreRelease1, sharedPreRelease2, uniquePreRelease1,
                                uniquePreRelease2, Array.Empty<string>()))
                            Add(major, preRelease, additionalPreRelease, new(null, major, null, null, null, null, additionalPreRelease.Prepend(preRelease).ToArray(),
                                Array.Empty<SwVersion.BuildSegment>(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor24TestData))]
    public void ComponentConstructor24Test(int major, SwVersion.PreReleaseSegment preRelease, SwVersion.PreReleaseSegment[]? additionalPreRelease, VersionValues expected)
    {
        SwVersion target = (additionalPreRelease is null) ? new(major, preRelease) : new(major, preRelease, additionalPreRelease);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor25Test(string, int, SwVersion.BuildSegment, SwVersion.BuildSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor25TestData : TheoryData<string, int, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor25TestData()
        {
            string sharedPrefix1 = GetRandomString(_prefixCharsSource, 1, 1);
            string sharedPrefix2 = GetRandomString(_prefixCharsSource, 2, 8);
            string uniquePrefix1 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix1);
            string uniquePrefix2 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string uniquePrefix3 = "-" + GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string sharedBuild1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1);
            string sharedBuild2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2);
            string uniqueBuild1 = GetRandomString(_buildCharsSource, 1, 1, uniquePrefix1, sharedPrefix1, sharedBuild1);
            string uniqueBuild2 = GetRandomString(_buildCharsSource, 2, 8, uniquePrefix2, sharedPrefix2, sharedBuild2);
            foreach (string prefix in new string[] { sharedPrefix1, sharedPrefix2, uniquePrefix1, uniquePrefix2, uniquePrefix3 })
                foreach (int major in GetMajorValues())
                    foreach (SwVersion.BuildSegment build in GetBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2))
                        foreach (SwVersion.BuildSegment[] additionalBuild in GetAdditonalBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2,
                                new string[] { sharedPrefix1, sharedPrefix2 }, uniquePrefix1, uniquePrefix2))
                            Add(prefix, major, build, additionalBuild, new(prefix, major, null, null, null, null, Array.Empty<SwVersion.PreReleaseSegment>(),
                                additionalBuild.Prepend(build).ToArray(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor25TestData))]
    public void ComponentConstructor25Test(string prefix, int major, SwVersion.BuildSegment build, SwVersion.BuildSegment[]? additionalBuild, VersionValues expected)
    {
        SwVersion target = (additionalBuild is null) ? new(prefix, major, build) : new(prefix, major, build, additionalBuild);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor26Test(string, int, int, int, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor26TestData : TheoryData<string, int, int, int, VersionValues>
    {
        public ComponentConstructor26TestData()
        {
            IEnumerable<int> patchValues = GetPatchValues(out IEnumerable<int> majorValues, out IEnumerable<int> minorValues);
            string sharedPrefix1 = GetRandomString(_prefixCharsSource, 1, 1);
            string sharedPrefix2 = GetRandomString(_prefixCharsSource, 2, 8);
            string uniquePrefix1 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix1);
            string uniquePrefix2 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string uniquePrefix3 = "-" + GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            foreach (string prefix in new string[] { sharedPrefix1, sharedPrefix2, uniquePrefix1, uniquePrefix2, uniquePrefix3 })
                foreach (int major in majorValues)
                    foreach (int minor in minorValues)
                        foreach (int patch in patchValues)
                            Add(prefix, major, minor, patch, new(prefix, major, minor, patch, null, Array.Empty<int>(),
                                Array.Empty<SwVersion.PreReleaseSegment>(), Array.Empty<SwVersion.BuildSegment>(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, int)" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor26TestData))]
    public void ComponentConstructor26Test(string prefix, int major, int minor, int patch, VersionValues expected)
    {
        SwVersion target = new(prefix, major, minor, patch);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor27Test(int, int, int, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor27TestData : TheoryData<int, int, int, VersionValues>
    {
        public ComponentConstructor27TestData()
        {
            IEnumerable<int> patchValues = GetPatchValues(out IEnumerable<int> majorValues, out IEnumerable<int> minorValues);
            foreach (int major in majorValues)
                foreach (int minor in minorValues)
                    foreach (int patch in patchValues)
                        Add(major, minor, patch, new(null, major, minor, patch, null, Array.Empty<int>(),
                            Array.Empty<SwVersion.PreReleaseSegment>(), Array.Empty<SwVersion.BuildSegment>(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, int)" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor27TestData))]
    public void ComponentConstructor27Test(int major, int minor, int patch, VersionValues expected)
    {
        SwVersion target = new(major, minor, patch);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor28Test(int, SwVersion.BuildSegment, SwVersion.BuildSegment[]?, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor28TestData : TheoryData<int, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor28TestData()
        {
            string sharedBuild1 = GetRandomString(_buildCharsSource, 1, 1);
            string sharedBuild2 = GetRandomString(_buildCharsSource, 2, 8);
            string uniqueBuild1 = GetRandomString(_buildCharsSource, 1, 1, sharedBuild1);
            string uniqueBuild2 = GetRandomString(_buildCharsSource, 2, 8, sharedBuild2);
            foreach (int major in GetMajorValues())
                foreach (SwVersion.BuildSegment build in GetBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2))
                    foreach (SwVersion.BuildSegment[] additionalBuild in GetAdditonalBuildSegments(sharedBuild1, sharedBuild2, uniqueBuild1, uniqueBuild2, Array.Empty<string>()))
                        Add(major, build, additionalBuild, new(null, major, null, null, null, null, Array.Empty<SwVersion.PreReleaseSegment>(),
                            additionalBuild.Prepend(build).ToArray(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor28TestData))]
    public void ComponentConstructor28Test(int major, SwVersion.BuildSegment build, SwVersion.BuildSegment[]? additionalBuild, VersionValues expected)
    {
        SwVersion target = (additionalBuild is null) ? new(major, build) : new(major, build, additionalBuild);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor29Test(string, int, int, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor29TestData : TheoryData<string, int, int, VersionValues>
    {
        public ComponentConstructor29TestData()
        {
            IEnumerable<int> minorValues = GetMinorValues(out IEnumerable<int> majorValues);
            string sharedPrefix1 = GetRandomString(_prefixCharsSource, 1, 1);
            string sharedPrefix2 = GetRandomString(_prefixCharsSource, 2, 8);
            string uniquePrefix1 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix1);
            string uniquePrefix2 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string uniquePrefix3 = "-" + GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            foreach (string prefix in new string[] { sharedPrefix1, sharedPrefix2, uniquePrefix1, uniquePrefix2, uniquePrefix3 })
                foreach (int major in majorValues)
                    foreach (int minor in minorValues)
                        Add(prefix, major, minor, new(prefix, major, minor, null, null, Array.Empty<int>(),
                            Array.Empty<SwVersion.PreReleaseSegment>(), Array.Empty<SwVersion.BuildSegment>(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int)" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor29TestData))]
    public void ComponentConstructor29Test(string prefix, int major, int minor, VersionValues expected)
    {
        SwVersion target = new(prefix, major, minor);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor30Test(int, int, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor30TestData : TheoryData<int, int, VersionValues>
    {
        public ComponentConstructor30TestData()
        {
            IEnumerable<int> minorValues = GetMinorValues(out IEnumerable<int> majorValues);
            foreach (int major in majorValues)
                foreach (int minor in minorValues)
                    Add(major, minor, new(null, major, minor, null, null, Array.Empty<int>(),
                        Array.Empty<SwVersion.PreReleaseSegment>(), Array.Empty<SwVersion.BuildSegment>(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int)" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor30TestData))]
    public void ComponentConstructor30Test(int major, int minor, VersionValues expected)
    {
        SwVersion target = new(major, minor);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor31Test(string, int, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor31TestData : TheoryData<string, int, VersionValues>
    {
        public ComponentConstructor31TestData()
        {
            string sharedPrefix1 = GetRandomString(_prefixCharsSource, 1, 1);
            string sharedPrefix2 = GetRandomString(_prefixCharsSource, 2, 8);
            string uniquePrefix1 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix1);
            string uniquePrefix2 = GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            string uniquePrefix3 = "-" + GetRandomString(_prefixCharsSource, 2, 8, sharedPrefix2);
            foreach (string prefix in new string[] { sharedPrefix1, sharedPrefix2, uniquePrefix1, uniquePrefix2, uniquePrefix3 })
                foreach (int major in GetMajorValues())
                    Add(prefix, major, new(prefix, major, null, null, null, Array.Empty<int>(),
                        Array.Empty<SwVersion.PreReleaseSegment>(), Array.Empty<SwVersion.BuildSegment>(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int)" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor31TestData))]
    public void ComponentConstructor31Test(string prefix, int major, VersionValues expected)
    {
        SwVersion target = new(prefix, major);
        expected.AssertEquals(target);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ComponentConstructor32Test(int, VersionValues)" />.
    /// </summary>
    public class ComponentConstructor32TestData : TheoryData<int, VersionValues>
    {
        public ComponentConstructor32TestData()
        {
            foreach (int major in GetMajorValues())
                Add(major, new(null, major, null, null, null, Array.Empty<int>(), Array.Empty<SwVersion.PreReleaseSegment>(), Array.Empty<SwVersion.BuildSegment>(), SwVersion.VersionStringFormat.Standard));
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int)" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor32TestData))]
    public void ComponentConstructor32Test(int major, VersionValues expected)
    {
        SwVersion target = new(major);
        expected.AssertEquals(target);
    }
}