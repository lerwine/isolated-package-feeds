namespace CdnGetter.UnitTests;

public class SwVersionUnitTest
{
    /*
    ^
    (?<major>-?\d+)
    (\.
        (?<minor>\d+)
        (\.
            (?<patch>\d+)
            (\.
                (?<revision>\d+)
                (\.
                    (?<minorRevision>\d+)
                )?
            )?
        )?
    )?
    (
        -(?<preRelease>[a-zA-Z\d_-]+(\.[a-zA-Z\d_-]+)*)
    )?
    (\+
        (?<build>[a-zA-Z\d_-]+(\.[a-zA-Z\d_-]+)*)
    )?$

    2.1.1-rc1
    18.0.0-alpha-34308b5ad-20210729
    2.7.0-beta.2
    1.0.0rc10
    r45
    */
    [Theory]
    [InlineData("0", /*Major*/ 0, /*Minor*/null, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0])]
    [InlineData("-0", /*Major*/ 0, /*Minor*/null, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0])]
    [InlineData("1", /*Major*/ 1, /*Minor*/null, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0])]
    [InlineData(" 1 ", /*Major*/ 1, /*Minor*/null, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0])]
    [InlineData("2147483647", /*Major*/ 2147483647, /*Minor*/null, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0])]
    [InlineData("-2147483648", /*Major*/ -2147483648, /*Minor*/null, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0])]
    [InlineData("-1", /*Major*/ -1, /*Minor*/null, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0])]
    [InlineData("1.0", /*Major*/ 1, /*Minor*/0, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0])]
    [InlineData("1.0.0", /*Major*/ 1, /*Minor*/0, /*Patch*/0, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0])]
    [InlineData("1.0.0.0", /*Major*/ 1, /*Minor*/0, /*Patch*/0, /*Revision*/0, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0])]
    [InlineData("1.0.0.0.0", /*Major*/ 1, /*Minor*/0, /*Patch*/0, /*Revision*/0, /*MinorRevision*/0, /*PreRelease*/new string[0], /*Build*/new string[0])]
    [InlineData("1-pre", /*Major*/ 1, /*Minor*/null, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[] { "pre" }, /*Build*/new string[0])]
    public void TryParseTrueTest(string versionString, int major, int? minor, int? patch, int? revision, int? minorRevision, string[] preRelease, string[] build)
    {
        bool actual = SwVersion.TryParse(versionString, out SwVersion? result);
        Assert.True(actual);
        Assert.NotNull(result);
        Assert.Equal(major, result!.Value.Major);
        Assert.Equal(minor, result!.Value.Minor);
        Assert.Equal(patch, result!.Value.Patch);
        Assert.Equal(revision, result!.Value.Revision);
        Assert.Equal(minorRevision, result!.Value.MinorRevision);
        Assert.Equal(preRelease, result!.Value.PreRelease);
        Assert.Equal(build, result!.Value.Build);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("2147483648")]
    [InlineData("-")]
    [InlineData("- 1")]
    [InlineData("-2147483649")]
    [InlineData("1a")]
    [InlineData("1.-1")]
    [InlineData("1.0.0.0.0.0")]
    public void TryParseFalseTest(string? versionString)
    {
        bool actual = SwVersion.TryParse(versionString, out SwVersion? result);
        Assert.False(actual);
        Assert.Null(result);
    }
}