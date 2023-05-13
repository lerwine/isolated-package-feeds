using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace CdnGetter.UnitTests;

public partial class SwVersionUnitTest
{
    private readonly ITestOutputHelper _output;

    public SwVersionUnitTest(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// Unit test for <see cref="SwVersion.SemanticLikeRegex" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(SemanticLikeRegexTestData))]
    public void SemanticLikeRegexTest(string value, string? pfx, string? major, string? minor, string? patch, string? rev, string? xnum, char? delim, string? pre, string? build)
    {
        _output.WriteLine("value = \"{0}\"", value.Replace("\\", "\\\\").Replace("\"", "\\\""));
        Match match = SwVersion.SemanticLikeRegex.Match(value);
        if (major is null)
            Assert.False(match.Success);
        else
        {
            Assert.True(match.Success);
            Group g = match.Groups[SwVersion.REGEX_GROUP_pfx];
            if (pfx is null)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(pfx, g.Value);
            }
            g = match.Groups[SwVersion.REGEX_GROUP_major];
            Assert.True(g.Success);
            Assert.Equal(major, g.Value);
            g = match.Groups[SwVersion.REGEX_GROUP_minor];
            if (minor is null)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(minor, g.Value);
            }
            g = match.Groups[SwVersion.REGEX_GROUP_patch];
            if (patch is null)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(patch, g.Value);
            }
            g = match.Groups[SwVersion.REGEX_GROUP_rev];
            if (rev is null)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(rev, g.Value);
            }
            g = match.Groups[SwVersion.REGEX_GROUP_xnum];
            if (xnum is null)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(xnum, g.Value);
            }
            g = match.Groups[SwVersion.REGEX_GROUP_DELIM];
            if (delim.HasValue)
            {
                Assert.True(g.Success);
                Assert.Equal(delim.Value, g.Value[0]);
            }
            else
                Assert.False(g.Success);
            g = match.Groups[SwVersion.REGEX_GROUP_PRE];
            if (pre is null)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(pre, g.Value);
            }
            g = match.Groups[SwVersion.REGEX_GROUP_BUILD];
            if (build is null)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(build, g.Value);
            }
        }
    }

    public class Pep440RegexTestData : TheoryData<string, string?, string?, string?, char?, string?, string?, string?, string?>
    {
        public Pep440RegexTestData()
        {
            Add(string.Empty, /*pfx*/null, /*epoch*/null, /*numeric*/null, /*delim*/null, /*pre*/null, /*modname*/null, /*modnum*/null, /*build*/null);
            Add("glide-rome-06-23-2021__patch0-07-07-2021", /*pfx*/"glide-rome-", /*epoch*/null, /*numeric*/"06", /*delim*/'-', /*pre*/"23", /*modname*/null, /*modnum*/null, /*build*/"2021__patch0-07-07-2021");
            Add("pip-1.3.1-py33-none-any.whl", /*pfx*/"pip-", /*epoch*/null, /*numeric*/"1.3.1", /*delim*/'-', /*pre*/"py33", /*modname*/"py", /*modnum*/"33", /*build*/"none-any.whl");
            Add("1!2.0", /*pfx*/null, /*epoch*/"1", /*numeric*/"2.0", /*delim*/null, /*pre*/null, /*modname*/null, /*modnum*/null, /*build*/null);
            Add("1.2.3-164-g6f10c", /*pfx*/null, /*epoch*/null, /*numeric*/"1.2.3", /*delim*/'-', /*pre*/"164", /*modname*/null, /*modnum*/null, /*build*/"g6f10c");
        }
    }

    /// <summary>
    /// Unit test for <see cref="SwVersion.Pep440Regex" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(Pep440RegexTestData))]
    public void Pep440RegexTest(string value, string? pfx, string? epoch, string? numeric, char? delim, string? pre, string? modname, string? modnum, string? build)
    {
        _output.WriteLine("value = \"{0}\"", value.Replace("\\", "\\\\").Replace("\"", "\\\""));
        Match match = SwVersion.SemanticLikeRegex.Match(value);
        if (numeric is null)
            Assert.False(match.Success);
        else
        {
            Assert.True(match.Success);
            Group g = match.Groups[SwVersion.REGEX_GROUP_pfx];
            if (pfx is null)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(pfx, g.Value);
            }
            g = match.Groups[SwVersion.REGEX_GROUP_epoch];
            if (epoch is null)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(epoch, g.Value);
            }
            string[] vn = numeric.Split('.');
            g = match.Groups[SwVersion.REGEX_GROUP_major];
            Assert.True(g.Success);
            Assert.Equal(vn[0], g.Value);
            g = match.Groups[SwVersion.REGEX_GROUP_minor];
            if (vn.Length < 2)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(vn[1], g.Value);
            }
            g = match.Groups[SwVersion.REGEX_GROUP_patch];
            if (vn.Length < 3)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(vn[2], g.Value);
            }
            g = match.Groups[SwVersion.REGEX_GROUP_rev];
            if (vn.Length < 4)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(vn[3], g.Value);
            }
            g = match.Groups[SwVersion.REGEX_GROUP_xnum];
            if (vn.Length < 5)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(vn[4], g.Value);
            }
            g = match.Groups[SwVersion.REGEX_GROUP_DELIM];
            if (delim.HasValue)
            {
                Assert.True(g.Success);
                Assert.Equal(delim.Value, g.Value[0]);
            }
            else
                Assert.False(g.Success);
            g = match.Groups[SwVersion.REGEX_GROUP_PRE];
            if (pre is null)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(pre, g.Value);
            }
            
            g = match.Groups[SwVersion.REGEX_GROUP_modname];
            if (modname is null)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(modname, g.Value);
            }
            
            g = match.Groups[SwVersion.REGEX_GROUP_modnum];
            if (modnum is null)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(modnum, g.Value);
            }
            
            g = match.Groups[SwVersion.REGEX_GROUP_BUILD];
            if (build is null)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(build, g.Value);
            }
        }
    }
    
    public class DatedVersionRegexTestData : TheoryData<string, string?, string?, string?, string?, string?, string?, string?>
    {
        public DatedVersionRegexTestData()
        {
            Add(string.Empty, /*pfx*/null, /*relDate*/null, /*delim*/null, /*patch*/null, /*fromDate*/null, /*toDate*/null, /*build*/null);
            Add("pip-1.3.1-py33-none-any.whl", /*pfx*/null, /*relDate*/null, /*delim*/null, /*patch*/null, /*fromDate*/null, /*toDate*/null, /*build*/null);
            Add("06-23-21", /*pfx*/null, /*relDate*/null, /*delim*/null, /*patch*/null, /*fromDate*/null, /*toDate*/null, /*build*/null);
            Add("08-09-2022", /*pfx*/null, /*relDate*/"2022.08.09", /*delim*/null, /*patch*/null, /*fromDate*/null, /*toDate*/null, /*build*/null);
            Add("2021-06-23", /*pfx*/null, /*relDate*/"2021.06.23", /*delim*/null, /*patch*/null, /*fromDate*/null, /*toDate*/null, /*build*/null);
            Add("22-08-09", /*pfx*/null, /*relDate*/"22.08.09", /*delim*/null, /*patch*/null, /*fromDate*/null, /*toDate*/null, /*build*/null);
            Add("glide-rome-06-23-2021__patch0-07-07-2021", /*pfx*/"glide-rome-", /*relDate*/"2021.06.23",
                /*delim*/"__patch", /*patch*/"0", /*fromDate*/"2021.07-07", /*toDate*/null, /*build*/null);
            Add("madrid-12-18-2018__patch4-05-29-2019_06-05-2019_1042", /*pfx*/"madrid-", /*relDate*/"2018.12.18",
                /*delim*/"__patch", /*patch*/"4", /*fromDate*/"2019.05-29", /*toDate*/"2019.06-05", /*build*/"1042");
        }
    }
    
    /// <summary>
    /// Unit test for <see cref="SwVersion.DatedVersionRegex" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(DatedVersionRegexTestData))]
    public void DatedVersionRegexTest(string value, string? pfx, string? relDate, string? delim, string? patch, string? fromDate, string? toDate, string? build)
    {
        // relDate = [major.minor.rel]
        // fromDate = [rev.xnum]
        // toDate =  [endYr.endDay]
        _output.WriteLine("value = \"{0}\"", value.Replace("\\", "\\\\").Replace("\"", "\\\""));
        Match match = SwVersion.SemanticLikeRegex.Match(value);
        if (relDate is null)
            Assert.False(match.Success);
        else
        {
            Assert.True(match.Success);
            Group g = match.Groups[SwVersion.REGEX_GROUP_pfx];
            if (pfx is null)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(pfx, g.Value);
            }
            g = match.Groups[SwVersion.REGEX_GROUP_major];
            Assert.True(g.Success);
            string[] d = relDate.Split('.');
            Assert.Equal(d[0], g.Value);
            g = match.Groups[SwVersion.REGEX_GROUP_minor];
            Assert.True(g.Success);
            Assert.Equal(d[1], g.Value);
            g = match.Groups[SwVersion.REGEX_GROUP_rel];
            Assert.True(g.Success);
            Assert.Equal(d[2], g.Value);
            g = match.Groups[SwVersion.REGEX_GROUP_DELIM];
            if (delim is null)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(delim, g.Value);
            }
            g = match.Groups[SwVersion.REGEX_GROUP_patch];
            if (patch is null)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(patch, g.Value);
            }
            g = match.Groups[SwVersion.REGEX_GROUP_rev];
            if (fromDate is null)
            {
                Assert.False(g.Success);
                Assert.False(match.Groups[SwVersion.REGEX_GROUP_xnum].Success);
            }
            else
            {
                d = relDate.Split('.');
                Assert.True(g.Success);
                Assert.Equal(d[0], g.Value);
                g = match.Groups[SwVersion.REGEX_GROUP_xnum];
                Assert.True(g.Success);
                Assert.Equal(d[1], g.Value);
            }
            g = match.Groups[SwVersion.REGEX_GROUP_endyr];
            if (toDate is null)
            {
                Assert.False(g.Success);
                Assert.False(match.Groups[SwVersion.REGEX_GROUP_endday].Success);
            }
            else
            {
                d = relDate.Split('.');
                Assert.True(g.Success);
                Assert.Equal(d[0], g.Value);
                g = match.Groups[SwVersion.REGEX_GROUP_endday];
                Assert.True(g.Success);
                Assert.Equal(d[1], g.Value);
            }
            g = match.Groups[SwVersion.REGEX_GROUP_BUILD];
            if (build is null)
                Assert.False(g.Success);
            else
            {
                Assert.True(g.Success);
                Assert.Equal(build, g.Value);
            }
        }
    }
    
    /// <summary>
    /// Unit test for constructor <see cref="SwVersion.PreReleaseSegment(bool, string)" /> that will not throw an exception.
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
    /// Unit test for constructor <see cref="SwVersion.PreReleaseSegment(bool, string)" /> that will throw an exception.
    /// </summary>
    [Theory]
    [ClassData(typeof(PreReleaseSegmentConstructor2TestData))]
    public void PreReleaseSegmentConstructor2Test(bool altSeprator, string value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(nameof(value), () => new SwVersion.PreReleaseSegment(altSeprator, value));
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string?)" />
    /// </summary>
    [Theory]
    [ClassData(typeof(ParsingConstructorTestData))]
    public void ParsingConstructorTest(string? versionString, string jsonData)
    {
        SwVersion target = new(versionString);
        JsonObject obj = (JsonObject)JsonNode.Parse(jsonData)!;
        Assert.Equal(obj[nameof(SwVersion.Major_obs)]!.GetValue<int>(), target.Major_obs);
        if (obj.TryGetPropertyInt(nameof(SwVersion.Minor_obs), out int minor))
        {
            Assert.True(target.Minor_obs.HasValue);
            Assert.Equal(minor, target.Minor_obs!.Value);
            if (obj.TryGetPropertyInt(nameof(SwVersion.Patch), out int patch))
            {
                Assert.True(target.Patch.HasValue);
                Assert.Equal(patch, target.Patch!.Value);
                if (obj.TryGetPropertyInt(nameof(SwVersion.Revision), out int revision))
                {
                    Assert.True(target.Revision.HasValue);
                    Assert.Equal(revision, target.Revision!.Value);
                    if (obj.TryGetPropertyArray(nameof(SwVersion.AdditionalNumerical_obs), out JsonArray? additionalNumerical))
                    {
                        Assert.NotNull(target.AdditionalNumerical_obs);
                        Assert.Equal(additionalNumerical.Count, target.AdditionalNumerical_obs!.Count);
                        for (int i = 0; i < additionalNumerical.Count; i++)
                            Assert.Equal(additionalNumerical[i]!.GetValue<int>(), target.AdditionalNumerical_obs[i]);
                    }
                    else
                        Assert.Null(target.AdditionalNumerical_obs);
                }
                else
                {
                    Assert.False(target.Revision.HasValue);
                    Assert.Null(target.AdditionalNumerical_obs);
                }
            }
            else
            {
                Assert.False(target.Patch.HasValue);
                Assert.False(target.Revision.HasValue);
                Assert.Null(target.AdditionalNumerical_obs);
            }
        }
        else
        {
            Assert.False(target.Minor_obs.HasValue);
            Assert.False(target.Patch.HasValue);
            Assert.False(target.Revision.HasValue);
            Assert.Null(target.AdditionalNumerical_obs);
        }
        if (obj.TryGetPropertyArray(nameof(SwVersion.PreRelease), out JsonArray? preRelease))
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(preRelease.Count, target.PreRelease!.Count);
            for (int i = 0; i < preRelease.Count; i++)
            {
                JsonObject expected = (JsonObject)preRelease[i]!;
                SwVersion.PreReleaseSegment actual = target.PreRelease[i];
                Assert.Equal(expected[nameof(SwVersion.PreReleaseSegment.AltSeparator)]!.GetValue<bool>(), actual.AltSeparator);
                Assert.Equal(expected[nameof(SwVersion.PreReleaseSegment.Value)]!.GetValue<string>(), actual.Value);
            }
        }
        else
            Assert.Null(target.PreRelease);
        if (obj.TryGetPropertyArray(nameof(SwVersion.Build), out JsonArray? build))
        {
            Assert.NotNull(target.Build);
            Assert.Equal(build.Count, target.Build!.Count);
            for (int i = 0; i < build.Count; i++)
            {
                JsonObject expected = (JsonObject)build[i]!;
                SwVersion.BuildSegment actual = target.Build[i];
                SwVersion.BuildSeparator separator = expected[nameof(SwVersion.BuildSegment.Value)]!.GetValue<string>() switch
                {
                    "." => SwVersion.BuildSeparator.Dot,
                    "-" => SwVersion.BuildSeparator.Dash,
                    _ => SwVersion.BuildSeparator.Plus,
                };
                Assert.Equal(separator, actual.Separator);
                Assert.Equal(expected[nameof(SwVersion.BuildSegment.Value)]!.GetValue<string>(), actual.Value);
            }
        }
    }

    [Fact]
    public void ComponentConstructor0Test()
    {
        SwVersion result = new();
        Assert.Null(result.Prefix_obs);
        Assert.Equal(0, result.Major_obs);
        Assert.Null(result.Minor_obs);
        Assert.Null(result.Patch);
        Assert.Null(result.Revision);
        Assert.Null(result.AdditionalNumerical_obs);
        Assert.Null(result.PreRelease);
        Assert.Null(result.Build);
    }
}