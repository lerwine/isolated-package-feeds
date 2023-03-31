namespace CdnGetter.UnitTests;

public class SwVersionUnitTest
{
    [Theory]
    [InlineData("0", /*Prefix*/"", /*Major*/ 0, /*Minor*/null, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0], SwVersion.VersionStringFormat.Standard)]
    [InlineData("-0", /*Prefix*/"-", /*Major*/ 0, /*Minor*/null, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0], SwVersion.VersionStringFormat.Standard)]
    [InlineData("1", /*Prefix*/"", /*Major*/ 1, /*Minor*/null, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0], SwVersion.VersionStringFormat.Standard)]
    [InlineData(" 1 ", /*Prefix*/"", /*Major*/ 1, /*Minor*/null, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0], SwVersion.VersionStringFormat.Standard)]
    [InlineData("2147483647", /*Prefix*/"", /*Major*/ 2147483647, /*Minor*/null, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0], SwVersion.VersionStringFormat.Standard)]
    [InlineData("-2147483648", /*Prefix*/"", /*Major*/ -2147483648, /*Minor*/null, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0], SwVersion.VersionStringFormat.Standard)]
    [InlineData("-1", /*Prefix*/"", /*Major*/ -1, /*Minor*/null, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0], SwVersion.VersionStringFormat.Standard)]
    [InlineData("1.0", /*Prefix*/"", /*Major*/ 1, /*Minor*/0, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0], SwVersion.VersionStringFormat.Standard)]
    [InlineData("1.0.0", /*Prefix*/"", /*Major*/ 1, /*Minor*/0, /*Patch*/0, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0], SwVersion.VersionStringFormat.Standard)]
    [InlineData("1.0.0.0", /*Prefix*/"", /*Major*/ 1, /*Minor*/0, /*Patch*/0, /*Revision*/0, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0], SwVersion.VersionStringFormat.Standard)]
    [InlineData("1.0.0.0.0", /*Prefix*/"", /*Major*/ 1, /*Minor*/0, /*Patch*/0, /*Revision*/0, /*MinorRevision*/0, /*PreRelease*/new string[0], /*Build*/new string[0], SwVersion.VersionStringFormat.Standard)]
    [InlineData("2.1.1-rc1", /*Prefix*/"", /*Major*/ 2, /*Minor*/1, /*Patch*/1, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[] { "-rc1" }, /*Build*/new string[0], SwVersion.VersionStringFormat.Standard)]
    [InlineData("18.0.0-alpha-34308b5ad-20210729", /*Prefix*/"", /*Major*/ 18, /*Minor*/0, /*Patch*/0, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[] { "-alpha", "-34308b5ad", "-20210729" }, /*Build*/new string[0], SwVersion.VersionStringFormat.Standard)]
    [InlineData("2.7-beta.2", /*Prefix*/"", /*Major*/ 2, /*Minor*/7, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[] { "-beta", ".2" }, /*Build*/new string[0], SwVersion.VersionStringFormat.Standard)]
    [InlineData("1.0.0rc10", /*Prefix*/"", /*Major*/ 1, /*Minor*/0, /*Patch*/0, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[] { "rc10" }, /*Build*/new string[0], SwVersion.VersionStringFormat.Alt)]
    [InlineData("1-pre", /*Prefix*/"", /*Major*/ 1, /*Minor*/null, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[] { "-pre" }, /*Build*/new string[0], SwVersion.VersionStringFormat.Standard)]
    [InlineData("r45", /*Prefix*/"r", /*Major*/ 45, /*Minor*/null, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0], SwVersion.VersionStringFormat.Alt)]
    [InlineData("Utah", /*Prefix*/"Utah", /*Major*/ 0, /*Minor*/null, /*Patch*/null, /*Revision*/null, /*MinorRevision*/null, /*PreRelease*/new string[0], /*Build*/new string[0], SwVersion.VersionStringFormat.NonStandard)]
    public void TryParseTrueTest(string prefix, string versionString, int major, int? minor, int? patch, int? revision, int? minorRevision, string[] preRelease, string[] build, SwVersion.VersionStringFormat format)
    {
        bool actual = SwVersion.TryParse(versionString, out SwVersion? result);
        Assert.True(actual);
        Assert.NotNull(result);
        Assert.Equal(prefix, result!.Value.Prefix);
        Assert.Equal(major, result!.Value.Major);
        Assert.Equal(minor, result!.Value.Minor);
        Assert.Equal(patch, result!.Value.Patch);
        Assert.Equal(revision, result!.Value.Revision);
        Assert.Equal(minorRevision, result!.Value.MinorRevision);
        Assert.Equal(preRelease.Select(s => new SwVersion.ExtraVersionComponent(s[0], (s.Length > 1) ? s[1..] : "")), result!.Value.PreRelease);
        Assert.Equal(build.Select(s => new SwVersion.ExtraVersionComponent(s[0], (s.Length > 1) ? s[1..] : "")), result!.Value.Build);
        Assert.Equal(format, result!.Value.Format);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void TryParseFalseTest(string? versionString)
    {
        bool actual = SwVersion.TryParse(versionString, out SwVersion? result);
        Assert.False(actual);
        Assert.Null(result);
    }
}